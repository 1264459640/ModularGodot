using Godot;
using MF.Infrastructure.Abstractions.Logging;
using MF.Data.Transient.Infrastructure.Monitoring;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace MF.Infrastructure.Monitoring;

/// <summary>
/// 性能监控实现
/// </summary>
public class PerformanceMonitor : IPerformanceMonitor, IDisposable
{
    private readonly IGameLogger<PerformanceMonitor> _logger;
    private readonly ConcurrentDictionary<string, MetricData> _metrics = new();
    private readonly ConcurrentDictionary<string, long> _counters = new();
    private readonly ConcurrentDictionary<string, TimerData> _timers = new();
    private readonly ConcurrentDictionary<string, ActiveTimer> _activeTimers = new();
    private readonly Timer _reportTimer;
    private readonly DateTime _startTime;
    private bool _disposed;
    
    public PerformanceMonitor(IGameLogger<PerformanceMonitor> logger)
    {
        _logger = logger;
        _startTime = DateTime.UtcNow;
        
        // 每分钟自动生成报告
        _reportTimer = new Timer(AutoGenerateReport, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        
        _logger.LogInformation("PerformanceMonitor initialized");
    }
    
    public void RecordMetric(string name, double value, Dictionary<string, string>? tags = null)
    {
        if (_disposed) return;
        
        try
        {
            var key = CreateKey(name, tags);
            _metrics.AddOrUpdate(key, 
                new MetricData { Name = name, Tags = tags ?? new Dictionary<string, string>() },
                (k, existing) => existing);
            
            var metricData = _metrics[key];
            lock (metricData)
            {
                metricData.Values.Add(value);
                metricData.Count++;
                metricData.Sum += value;
                metricData.Min = Math.Min(metricData.Min, value);
                metricData.Max = Math.Max(metricData.Max, value);
                metricData.LastUpdated = DateTime.UtcNow;
                
                // 保持最近1000个值
                if (metricData.Values.Count > 1000)
                {
                    metricData.Values.RemoveAt(0);
                }
            }
            
            _logger.LogDebug("Metric recorded: {Name} = {Value}", name, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording metric: {Name}", name);
        }
    }
    
    public void RecordCounter(string name, long value = 1, Dictionary<string, string>? tags = null)
    {
        if (_disposed) return;
        
        try
        {
            var key = CreateKey(name, tags);
            _counters.AddOrUpdate(key, value, (k, existing) => existing + value);
            
            _logger.LogDebug("Counter recorded: {Name} += {Value}", name, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording counter: {Name}", name);
        }
    }
    
    public void RecordTimer(string name, TimeSpan duration, Dictionary<string, string>? tags = null)
    {
        if (_disposed) return;
        
        try
        {
            var key = CreateKey(name, tags);
            _timers.AddOrUpdate(key,
                new TimerData { Name = name, Tags = tags ?? new Dictionary<string, string>() },
                (k, existing) => existing);
            
            var timerData = _timers[key];
            lock (timerData)
            {
                timerData.Durations.Add(duration);
                timerData.Count++;
                timerData.TotalTime += duration;
                timerData.MinTime = timerData.MinTime == TimeSpan.Zero ? duration : TimeSpan.FromTicks(Math.Min(timerData.MinTime.Ticks, duration.Ticks));
                timerData.MaxTime = TimeSpan.FromTicks(Math.Max(timerData.MaxTime.Ticks, duration.Ticks));
                timerData.LastUpdated = DateTime.UtcNow;
                
                // 保持最近1000个值
                if (timerData.Durations.Count > 1000)
                {
                    timerData.Durations.RemoveAt(0);
                }
            }
            
            _logger.LogDebug("Timer recorded: {Name} = {Duration}ms", name, duration.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording timer: {Name}", name);
        }
    }
    
    public IDisposable StartTimer(string name, Dictionary<string, string>? tags = null)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(PerformanceMonitor));
        
        var timer = new ActiveTimer(name, tags, this);
        var key = CreateKey(name, tags);
        _activeTimers.TryAdd(key + "_" + timer.Id, timer);
        
        _logger.LogDebug("Timer started: {Name}", name);
        return timer;
    }
    
    public async Task<PerformanceReport> GenerateReportAsync(TimeSpan period)
    {
        try
        {
            var report = new PerformanceReport
            {
                GeneratedAt = DateTime.UtcNow,
                Period = period
            };
            
            // 生成指标统计
            foreach (var kvp in _metrics)
            {
                var metricData = kvp.Value;
                lock (metricData)
                {
                    if (metricData.Values.Count > 0)
                    {
                        var values = metricData.Values.ToArray();
                        var average = values.Average();
                        var variance = values.Select(v => Math.Pow(v - average, 2)).Average();
                        var standardDeviation = Math.Sqrt(variance);
                        
                        report.Metrics[kvp.Key] = new MetricStatistics
                        {
                            Count = metricData.Count,
                            Sum = metricData.Sum,
                            Average = average,
                            Min = metricData.Min,
                            Max = metricData.Max,
                            StandardDeviation = standardDeviation
                        };
                    }
                }
            }
            
            // 生成计数器统计
            foreach (var kvp in _counters)
            {
                report.Counters[kvp.Key] = kvp.Value;
            }
            
            // 生成计时器统计
            foreach (var kvp in _timers)
            {
                var timerData = kvp.Value;
                lock (timerData)
                {
                    if (timerData.Count > 0)
                    {
                        var durations = timerData.Durations.OrderBy(d => d.Ticks).ToArray();
                        var p95Index = (int)(durations.Length * 0.95);
                        var p99Index = (int)(durations.Length * 0.99);
                        
                        report.Timers[kvp.Key] = new TimerStatistics
                        {
                            Count = timerData.Count,
                            TotalTime = timerData.TotalTime,
                            AverageTime = TimeSpan.FromTicks(timerData.TotalTime.Ticks / timerData.Count),
                            MinTime = timerData.MinTime,
                            MaxTime = timerData.MaxTime,
                            P95Time = durations.Length > 0 ? durations[Math.Min(p95Index, durations.Length - 1)] : TimeSpan.Zero,
                            P99Time = durations.Length > 0 ? durations[Math.Min(p99Index, durations.Length - 1)] : TimeSpan.Zero
                        };
                    }
                }
            }
            
            // 生成系统信息
            report.SystemInfo = new SystemInfo
            {
                OperatingSystem = OS.GetName(),
                ProcessorCount = OS.GetProcessorCount(),
                TotalMemory = GC.GetTotalMemory(false),
                AvailableMemory = GC.GetTotalMemory(false), // Godot中难以获取可用内存
                ProcessStartTime = _startTime,
                Uptime = DateTime.UtcNow - _startTime
            };
            
            _logger.LogInformation("Performance report generated with {MetricCount} metrics, {CounterCount} counters, {TimerCount} timers", 
                report.Metrics.Count, report.Counters.Count, report.Timers.Count);
            
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating performance report");
            return new PerformanceReport();
        }
    }
    
    public PerformanceStatistics GetStatistics()
    {
        try
        {
            return new PerformanceStatistics
            {
                TotalMetrics = _metrics.Count,
                TotalCounters = _counters.Count,
                TotalTimers = _timers.Count,
                ActiveTimers = _activeTimers.Count,
                MemoryUsage = GC.GetTotalMemory(false),
                CpuUsage = 0.0 // Godot中难以获取CPU使用率
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting performance statistics");
            return new PerformanceStatistics();
        }
    }
    
    public void Reset()
    {
        try
        {
            _metrics.Clear();
            _counters.Clear();
            _timers.Clear();
            
            _logger.LogInformation("Performance monitor statistics reset");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting performance monitor");
        }
    }
    
    private string CreateKey(string name, Dictionary<string, string>? tags)
    {
        if (tags == null || tags.Count == 0)
            return name;
        
        var tagString = string.Join(",", tags.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        return $"{name}[{tagString}]";
    }
    
    private void AutoGenerateReport(object? state)
    {
        try
        {
            _ = Task.Run(async () =>
            {
                var report = await GenerateReportAsync(TimeSpan.FromMinutes(1));
                
                // 记录关键指标
                if (report.SystemInfo.MemoryUsage > 500 * 1024 * 1024) // 超过500MB
                {
                    _logger.LogWarning("High memory usage detected: {MemoryUsage} MB", 
                        report.SystemInfo.MemoryUsage / (1024 * 1024));
                }
                
                // 记录慢操作
                foreach (var timer in report.Timers.Where(t => t.Value.AverageTime.TotalMilliseconds > 100))
                {
                    _logger.LogWarning("Slow operation detected: {TimerName}, Average: {AverageTime}ms", 
                        timer.Key, timer.Value.AverageTime.TotalMilliseconds);
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in auto report generation");
        }
    }
    
    internal void CompleteTimer(ActiveTimer timer)
    {
        try
        {
            var key = CreateKey(timer.Name, timer.Tags) + "_" + timer.Id;
            _activeTimers.TryRemove(key, out _);
            
            RecordTimer(timer.Name, timer.Elapsed, timer.Tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing timer: {TimerName}", timer.Name);
        }
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        _logger.LogInformation("Disposing PerformanceMonitor");
        
        _reportTimer.Dispose();
        
        // 完成所有活跃的计时器
        foreach (var timer in _activeTimers.Values)
        {
            timer.Dispose();
        }
        
        _metrics.Clear();
        _counters.Clear();
        _timers.Clear();
        _activeTimers.Clear();
        
        _disposed = true;
        
        _logger.LogInformation("PerformanceMonitor disposed");
    }
}

/// <summary>
/// 指标数据
/// </summary>
internal class MetricData
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> Tags { get; set; } = new();
    public List<double> Values { get; set; } = new();
    public long Count { get; set; }
    public double Sum { get; set; }
    public double Min { get; set; } = double.MaxValue;
    public double Max { get; set; } = double.MinValue;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 计时器数据
/// </summary>
internal class TimerData
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> Tags { get; set; } = new();
    public List<TimeSpan> Durations { get; set; } = new();
    public long Count { get; set; }
    public TimeSpan TotalTime { get; set; }
    public TimeSpan MinTime { get; set; }
    public TimeSpan MaxTime { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 活跃计时器
/// </summary>
internal class ActiveTimer : IDisposable
{
    private readonly PerformanceMonitor _monitor;
    private readonly Stopwatch _stopwatch;
    private bool _disposed;
    
    public string Id { get; } = Guid.NewGuid().ToString("N")[..8];
    public string Name { get; }
    public Dictionary<string, string>? Tags { get; }
    public TimeSpan Elapsed => _stopwatch.Elapsed;
    
    public ActiveTimer(string name, Dictionary<string, string>? tags, PerformanceMonitor monitor)
    {
        Name = name;
        Tags = tags;
        _monitor = monitor;
        _stopwatch = Stopwatch.StartNew();
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        _stopwatch.Stop();
        _monitor.CompleteTimer(this);
        _disposed = true;
    }
}