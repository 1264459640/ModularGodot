namespace MF.Data.Transient.Infrastructure.Monitoring;

/// <summary>
/// 性能报告
/// </summary>
public class PerformanceReport
{
    /// <summary>
    /// 报告生成时间
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 报告周期
    /// </summary>
    public TimeSpan Period { get; set; }
    
    /// <summary>
    /// 基础指标统计
    /// </summary>
    public Dictionary<string, BasicStatistics> Metrics { get; set; } = new();
    
    /// <summary>
    /// 计数器统计
    /// </summary>
    public Dictionary<string, long> Counters { get; set; } = new();
    
    /// <summary>
    /// 时间统计
    /// </summary>
    public Dictionary<string, TimeStatistics> Timers { get; set; } = new();
    
    /// <summary>
    /// 系统信息
    /// </summary>
    public SystemInfo SystemInfo { get; set; } = new();
}