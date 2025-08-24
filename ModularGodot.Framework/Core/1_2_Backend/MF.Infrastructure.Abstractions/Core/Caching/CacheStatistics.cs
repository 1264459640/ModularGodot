namespace MF.Infrastructure.Abstractions.Caching;

/// <summary>
/// 缓存统计信息
/// </summary>
public class CacheStatistics
{
    /// <summary>
    /// 缓存命中次数
    /// </summary>
    public long Hits { get; set; }
    
    /// <summary>
    /// 缓存未命中次数
    /// </summary>
    public long Misses { get; set; }
    
    /// <summary>
    /// 缓存设置次数
    /// </summary>
    public long Sets { get; set; }
    
    /// <summary>
    /// 缓存驱逐次数
    /// </summary>
    public long Evictions { get; set; }
    
    /// <summary>
    /// 错误次数
    /// </summary>
    public long Errors { get; set; }
    
    /// <summary>
    /// 缓存命中率
    /// </summary>
    public double HitRate { get; set; }
    
    /// <summary>
    /// 当前缓存项数量
    /// </summary>
    public long TotalItems { get; set; }
    
    /// <summary>
    /// 当前缓存大小（字节）
    /// </summary>
    public long TotalSize { get; set; }
    
    /// <summary>
    /// 驱逐原因统计
    /// </summary>
    public Dictionary<string, long> EvictionReasons { get; set; } = new();
}