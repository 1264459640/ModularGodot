namespace MF.Infrastructure.Abstractions.EventBus;

/// <summary>
/// 事件总线抽象接口 - Critical级别
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// 异步发布事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="event">事件实例</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>发布任务</returns>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
    
    /// <summary>
    /// 同步发布事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="event">事件实例</param>
    void Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    
    /// <summary>
    /// 订阅事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="handler">事件处理器</param>
    /// <returns>订阅句柄，用于取消订阅</returns>
    IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
    
    /// <summary>
    /// 异步订阅事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="handler">异步事件处理器</param>
    /// <returns>订阅句柄，用于取消订阅</returns>
    IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent;
    
    /// <summary>
    /// 条件订阅事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="filter">过滤条件</param>
    /// <param name="handler">事件处理器</param>
    /// <returns>订阅句柄，用于取消订阅</returns>
    IDisposable Subscribe<TEvent>(Func<TEvent, bool> filter, Action<TEvent> handler) where TEvent : IEvent;
    
    /// <summary>
    /// 一次性订阅事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="handler">事件处理器</param>
    /// <returns>订阅句柄，用于取消订阅</returns>
    IDisposable SubscribeOnce<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
    
    /// <summary>
    /// 获取事件总线统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    Task<EventBusStatistics> GetStatisticsAsync();
}

/// <summary>
/// 事件总线统计信息
/// </summary>
public class EventBusStatistics
{
    /// <summary>
    /// 已发布事件总数
    /// </summary>
    public long TotalPublishedEvents { get; set; }
    
    /// <summary>
    /// 当前订阅者数量
    /// </summary>
    public int ActiveSubscribers { get; set; }
    
    /// <summary>
    /// 事件类型统计
    /// </summary>
    public Dictionary<string, long> EventTypeStatistics { get; set; } = new();
    
    /// <summary>
    /// 错误事件数量
    /// </summary>
    public long ErrorEvents { get; set; }
    
    /// <summary>
    /// 平均处理时间（毫秒）
    /// </summary>
    public double AverageProcessingTimeMs { get; set; }
}