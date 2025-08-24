using Godot;
using MF.Infrastructure.Abstractions.Logging;
using MF.Data.Transient.Infrastructure.Monitoring;
using System.Collections.Concurrent;

namespace MF.Infrastructure.Monitoring;

/// <summary>
/// 内存监控服务 - Minimal级别（直接实现）
/// </summary>
public class MemoryMonitor : IDisposable
{
    private readonly IGameLogger<MemoryMonitor> _logger;
    private readonly Timer _monitorTimer;
    private long _lastMemoryUsage;
    private bool _disposed;
    private readonly ConcurrentQueue<MemorySnapshot> _memoryHistory = new();
    
    public event Action<MemoryPressureEventArgs>? MemoryPressureDetected;
    public event Action? AutoReleaseTriggered;
    
    /// <summary>
    /// 自动释放阈值（字节）
    /// </summary>
    public long AutoReleaseThreshold { get; set; } = 800 * 1024 * 1024; // 800MB
    
    /// <summary>
    /// 检查间隔
    /// </summary>
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromSeconds(15);
    
    /// <summary>
    /// 内存压力阈值
    /// </summary>
    public double MemoryPressureThreshold { get; set; } = 0.8; // 80%
    
    public MemoryMonitor(IGameLogger<MemoryMonitor> logger)
    {
        _logger = logger;
        _monitorTimer = new Timer(CheckMemoryUsage, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        
        _logger.LogInformation("MemoryMonitor initialized with threshold: {Threshold} bytes", AutoReleaseThreshold);
    }
    
    /// <summary>
    /// 开始监控
    /// </summary>
    public void StartMonitoring()
    {
        if (_disposed) return;
        
        _monitorTimer.Change(CheckInterval, CheckInterval);
        _logger.LogInformation("Memory monitoring started with interval: {Interval}", CheckInterval);
    }
    
    /// <summary>
    /// 停止监控
    /// </summary>
    public void StopMonitoring()
    {
        if (_disposed) return;
        
        _monitorTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        _logger.LogInformation("Memory monitoring stopped");
    }
    
    /// <summary>
    /// 检查内存压力
    /// </summary>
    /// <param name="currentUsage">当前内存使用量</param>
    public void CheckMemoryPressure(long currentUsage)
    {
        if (currentUsage > AutoReleaseThreshold)
        {
            _logger.LogWarning("Memory pressure detected: {CurrentUsage} > {Threshold}", 
                FormatBytes(currentUsage), FormatBytes(AutoReleaseThreshold));
            
            var eventArgs = new MemoryPressureEventArgs
            {
                CurrentUsage = currentUsage,
                PreviousUsage = _lastMemoryUsage,
                Threshold = AutoReleaseThreshold,
                PressureLevel = CalculatePressureLevel(currentUsage)
            };
            
            MemoryPressureDetected?.Invoke(eventArgs);
            
            // 如果内存压力很高，触发自动释放
            if (eventArgs.PressureLevel >= MemoryPressureLevel.High)
            {
                AutoReleaseTriggered?.Invoke();
            }
        }
    }
    
    /// <summary>
    /// 获取当前内存使用量
    /// </summary>
    /// <returns>内存使用量（字节）</returns>
    public long GetCurrentMemoryUsage()
    {
        return GC.GetTotalMemory(false);
    }
    
    /// <summary>
    /// 强制垃圾回收
    /// </summary>
    public void ForceGarbageCollection()
    {
        _logger.LogInformation("Forcing garbage collection");
        
        var beforeGC = GetCurrentMemoryUsage();
        
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        var afterGC = GetCurrentMemoryUsage();
        var freed = beforeGC - afterGC;
        
        _logger.LogInformation("Garbage collection completed. Freed: {FreedMemory}, Before: {BeforeMemory}, After: {AfterMemory}", 
            FormatBytes(freed), FormatBytes(beforeGC), FormatBytes(afterGC));
    }
    
    /// <summary>
    /// 获取内存统计信息
    /// </summary>
    /// <returns>内存统计信息</returns>
    public MemoryStatistics GetStatistics()
    {
        var currentUsage = GetCurrentMemoryUsage();
        var snapshots = _memoryHistory.ToArray();
        
        var statistics = new MemoryStatistics
        {
            CurrentUsage = currentUsage,
            PeakUsage = snapshots.Length > 0 ? snapshots.Max(s => s.Usage) : currentUsage,
            AverageUsage = snapshots.Length > 0 ? (long)snapshots.Average(s => s.Usage) : currentUsage,
            MinUsage = snapshots.Length > 0 ? snapshots.Min(s => s.Usage) : currentUsage,
            PressureLevel = CalculatePressureLevel(currentUsage),
            GCCollectionCounts = new Dictionary<int, int>()
        };
        
        // 获取GC统计信息
        for (int generation = 0; generation <= GC.MaxGeneration; generation++)
        {
            statistics.GCCollectionCounts[generation] = GC.CollectionCount(generation);
        }
        
        return statistics;
    }
    
    /// <summary>
    /// 获取内存历史记录
    /// </summary>
    /// <param name="count">获取的记录数量</param>
    /// <returns>内存快照列表</returns>
    public List<MemorySnapshot> GetMemoryHistory(int count = 100)
    {
        var snapshots = _memoryHistory.ToArray();
        return snapshots.TakeLast(count).ToList();
    }
    
    private void CheckMemoryUsage(object? state)
    {
        try
        {
            var currentUsage = GetCurrentMemoryUsage();
            
            // 记录内存快照
            var snapshot = new MemorySnapshot
            {
                Usage = currentUsage,
                Timestamp = DateTime.UtcNow,
                PressureLevel = CalculatePressureLevel(currentUsage)
            };
            
            _memoryHistory.Enqueue(snapshot);
            
            // 保持历史记录在合理范围内
            while (_memoryHistory.Count > 1000)
            {
                _memoryHistory.TryDequeue(out _);
            }
            
            // 检查内存压力
            CheckMemoryPressure(currentUsage);
            
            // 记录内存使用变化
            if (_lastMemoryUsage > 0)
            {
                var change = currentUsage - _lastMemoryUsage;
                var changePercent = (double)change / _lastMemoryUsage * 100;
                
                if (Math.Abs(changePercent) > 10) // 变化超过10%时记录
                {
                    _logger.LogDebug("Memory usage changed: {Change} ({ChangePercent:F1}%), Current: {CurrentUsage}", 
                        FormatBytes(change), changePercent, FormatBytes(currentUsage));
                }
            }
            
            _lastMemoryUsage = currentUsage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during memory monitoring");
        }
    }
    
    private MemoryPressureLevel CalculatePressureLevel(long currentUsage)
    {
        var pressureRatio = (double)currentUsage / AutoReleaseThreshold;
        
        return pressureRatio switch
        {
            < 0.5 => MemoryPressureLevel.Low,
            < 0.8 => MemoryPressureLevel.Medium,
            < 1.0 => MemoryPressureLevel.High,
            _ => MemoryPressureLevel.Critical
        };
    }
    
    private static string FormatBytes(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int counter = 0;
        decimal number = bytes;
        
        while (Math.Round(number / 1024) >= 1)
        {
            number /= 1024;
            counter++;
        }
        
        return $"{number:n1} {suffixes[counter]}";
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        _logger.LogInformation("Disposing MemoryMonitor");
        
        StopMonitoring();
        _monitorTimer.Dispose();
        
        while (_memoryHistory.TryDequeue(out _)) { }
        
        _disposed = true;
        
        _logger.LogInformation("MemoryMonitor disposed");
    }
}