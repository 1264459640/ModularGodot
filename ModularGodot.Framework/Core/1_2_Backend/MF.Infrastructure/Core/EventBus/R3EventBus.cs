using MF.Infrastructure.Abstractions.EventBus;
using MF.Infrastructure.Abstractions.Logging;
using R3;
using System.Collections.Concurrent;

namespace MF.Infrastructure.EventBus;

/// <summary>
/// 基于R3的事件总线实现
/// </summary>
public class R3EventBus : IEventBus, IDisposable
{
    private readonly ConcurrentDictionary<Type, Subject<object>> _subjects = new();
    private readonly CompositeDisposable _disposables = new();
    private readonly IGameLogger<R3EventBus> _logger;
    private readonly EventBusConfig _config;
    private readonly object _lock = new();
    private bool _disposed;
    
    // 统计信息
    private long _totalPublishedEvents;
    private readonly ConcurrentDictionary<string, long> _eventTypeStatistics = new();
    private long _errorEvents;
    private readonly List<double> _processingTimes = new();
    
    public R3EventBus(IGameLogger<R3EventBus> logger, EventBusConfig? config = null)
    {
        _logger = logger;
        _config = config ?? new EventBusConfig();
        
        _logger.LogInformation("R3EventBus initialized with config: {Config}", _config);
    }
    
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        if (_disposed)
        {
            _logger.LogWarning("Attempted to publish event on disposed EventBus: {EventType}", typeof(TEvent).Name);
            return;
        }
        
        var startTime = DateTime.UtcNow;
        
        try
        {
            _logger.LogDebug("Publishing event asynchronously: {EventType}, EventId: {EventId}", typeof(TEvent).Name, @event.EventId);
            
            var subject = GetOrCreateSubject<TEvent>();
            
            if (_config.EnableAsyncPublishing)
            {
                await Task.Run(() => subject.OnNext(@event), cancellationToken);
            }
            else
            {
                subject.OnNext(@event);
            }
            
            // 更新统计信息
            Interlocked.Increment(ref _totalPublishedEvents);
            _eventTypeStatistics.AddOrUpdate(typeof(TEvent).Name, 1, (key, value) => value + 1);
            
            var processingTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
            lock (_processingTimes)
            {
                _processingTimes.Add(processingTime);
                if (_processingTimes.Count > 1000) // 保持最近1000次的记录
                {
                    _processingTimes.RemoveAt(0);
                }
            }
            
            _logger.LogDebug("Event published successfully: {EventType}, ProcessingTime: {ProcessingTime}ms", typeof(TEvent).Name, processingTime);
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref _errorEvents);
            _logger.LogError(ex, "Failed to publish event: {EventType}, EventId: {EventId}", typeof(TEvent).Name, @event.EventId);
            
            if (_config.ThrowOnPublishError)
                throw;
        }
    }
    
    public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
    {
        if (_disposed)
        {
            _logger.LogWarning("Attempted to publish event on disposed EventBus: {EventType}", typeof(TEvent).Name);
            return;
        }
        
        var startTime = DateTime.UtcNow;
        
        try
        {
            _logger.LogDebug("Publishing event synchronously: {EventType}, EventId: {EventId}", typeof(TEvent).Name, @event.EventId);
            
            var subject = GetOrCreateSubject<TEvent>();
            subject.OnNext(@event);
            
            // 更新统计信息
            Interlocked.Increment(ref _totalPublishedEvents);
            _eventTypeStatistics.AddOrUpdate(typeof(TEvent).Name, 1, (key, value) => value + 1);
            
            var processingTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
            lock (_processingTimes)
            {
                _processingTimes.Add(processingTime);
                if (_processingTimes.Count > 1000)
                {
                    _processingTimes.RemoveAt(0);
                }
            }
            
            _logger.LogDebug("Event published successfully: {EventType}, ProcessingTime: {ProcessingTime}ms", typeof(TEvent).Name, processingTime);
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref _errorEvents);
            _logger.LogError(ex, "Failed to publish event: {EventType}, EventId: {EventId}", typeof(TEvent).Name, @event.EventId);
            
            if (_config.ThrowOnPublishError)
                throw;
        }
    }
    
    public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(R3EventBus));
        
        try
        {
            _logger.LogDebug("Subscribing to event: {EventType}", typeof(TEvent).Name);
            
            var subject = GetOrCreateSubject<TEvent>();
            var subscription = subject.Subscribe(evt =>
            {
                try
                {
                    handler((TEvent)evt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in event handler for: {EventType}", typeof(TEvent).Name);
                    if (_config.ThrowOnHandlerError)
                        throw;
                }
            });
            
            _logger.LogDebug("Subscribed to event: {EventType}", typeof(TEvent).Name);
            return subscription;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to event: {EventType}", typeof(TEvent).Name);
            throw;
        }
    }
    
    public IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
    {
        return Subscribe<TEvent>(evt =>
        {
            // 异步处理，但不阻塞事件发布
            _ = Task.Run(async () =>
            {
                try
                {
                    await handler(evt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in async event handler for: {EventType}", typeof(TEvent).Name);
                }
            });
        });
    }
    
    public IDisposable Subscribe<TEvent>(Func<TEvent, bool> filter, Action<TEvent> handler) where TEvent : IEvent
    {
        return Subscribe<TEvent>(evt =>
        {
            if (filter(evt))
            {
                handler(evt);
            }
        });
    }
    
    public IDisposable SubscribeOnce<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        IDisposable? subscription = null;
        subscription = Subscribe<TEvent>(evt =>
        {
            try
            {
                handler(evt);
            }
            finally
            {
                subscription?.Dispose();
            }
        });
        return subscription;
    }
    
    public async Task<EventBusStatistics> GetStatisticsAsync()
    {
        double averageProcessingTime = 0;
        lock (_processingTimes)
        {
            if (_processingTimes.Count > 0)
            {
                averageProcessingTime = _processingTimes.Average();
            }
        }
        
        return new EventBusStatistics
        {
            TotalPublishedEvents = _totalPublishedEvents,
            ActiveSubscribers = _subjects.Values.Sum(s => s.ObserverCount),
            EventTypeStatistics = new Dictionary<string, long>(_eventTypeStatistics),
            ErrorEvents = _errorEvents,
            AverageProcessingTimeMs = averageProcessingTime
        };
    }
    
    private Subject<object> GetOrCreateSubject<TEvent>() where TEvent : IEvent
    {
        var eventType = typeof(TEvent);
        return _subjects.GetOrAdd(eventType, _ =>
        {
            var subject = new Subject<object>();
            _disposables.Add(subject);
            _logger.LogDebug("Created new subject for event type: {EventType}", eventType.Name);
            return subject;
        });
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        _logger.LogInformation("Disposing R3EventBus");
        
        lock (_lock)
        {
            if (_disposed) return;
            
            _disposables.Dispose();
            _subjects.Clear();
            _disposed = true;
        }
        
        _logger.LogInformation("R3EventBus disposed");
    }
}

/// <summary>
/// 事件总线配置
/// </summary>
public class EventBusConfig
{
    /// <summary>
    /// 是否启用异步发布
    /// </summary>
    public bool EnableAsyncPublishing { get; set; } = false;
    
    /// <summary>
    /// 处理器错误时是否抛出异常
    /// </summary>
    public bool ThrowOnHandlerError { get; set; } = false;
    
    /// <summary>
    /// 发布错误时是否抛出异常
    /// </summary>
    public bool ThrowOnPublishError { get; set; } = true;
    
    /// <summary>
    /// 最大并发处理器数量
    /// </summary>
    public int MaxConcurrentHandlers { get; set; } = 10;
    
    /// <summary>
    /// 处理器超时时间
    /// </summary>
    public TimeSpan HandlerTimeout { get; set; } = TimeSpan.FromSeconds(30);
    
    public override string ToString()
    {
        return $"AsyncPublishing: {EnableAsyncPublishing}, ThrowOnHandlerError: {ThrowOnHandlerError}, MaxConcurrentHandlers: {MaxConcurrentHandlers}";
    }
}