namespace MF.Data.Transient.Infrastructure.Monitoring;

/// <summary>
/// 性能统计信息（简化版）
/// </summary>
public class PerformanceStatistics
{
    /// <summary>
    /// 总指标数量
    /// </summary>
    public int TotalMetrics { get; set; }
    
    /// <summary>
    /// 总计时器数量
    /// </summary>
    public int TotalTimers { get; set; }
    
    /// <summary>
    /// 内存使用量（字节）
    /// </summary>
    public long MemoryUsage { get; set; }
}