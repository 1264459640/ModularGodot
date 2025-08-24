namespace MF.Data.Transient.Infrastructure.Monitoring;

/// <summary>
/// 基础统计信息
/// </summary>
public class BasicStatistics
{
    /// <summary>
    /// 样本数量
    /// </summary>
    public long Count { get; set; }
    
    /// <summary>
    /// 总和（用于数值统计）
    /// </summary>
    public double Sum { get; set; }
    
    /// <summary>
    /// 平均值
    /// </summary>
    public double Average { get; set; }
    
    /// <summary>
    /// 最小值
    /// </summary>
    public double Min { get; set; }
    
    /// <summary>
    /// 最大值
    /// </summary>
    public double Max { get; set; }
}

/// <summary>
/// 时间统计信息
/// </summary>
public class TimeStatistics
{
    /// <summary>
    /// 调用次数
    /// </summary>
    public long Count { get; set; }
    
    /// <summary>
    /// 总时间
    /// </summary>
    public TimeSpan TotalTime { get; set; }
    
    /// <summary>
    /// 平均时间
    /// </summary>
    public TimeSpan AverageTime { get; set; }
    
    /// <summary>
    /// 最小时间
    /// </summary>
    public TimeSpan MinTime { get; set; }
    
    /// <summary>
    /// 最大时间
    /// </summary>
    public TimeSpan MaxTime { get; set; }
}