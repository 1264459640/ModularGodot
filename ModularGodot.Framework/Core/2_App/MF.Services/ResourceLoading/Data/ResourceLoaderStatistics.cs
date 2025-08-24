namespace MF.Services.ResourceLoading.Data;

/// <summary>
/// 资源加载器统计信息
/// </summary>
public class ResourceLoaderStatistics
{
    /// <summary>
    /// 已加载资源总数
    /// </summary>
    public long TotalLoadedResources { get; set; }
    
    /// <summary>
    /// 加载成功次数
    /// </summary>
    public long SuccessfulLoads { get; set; }
    
    /// <summary>
    /// 加载失败次数
    /// </summary>
    public long FailedLoads { get; set; }
    
    /// <summary>
    /// 平均加载时间（毫秒）
    /// </summary>
    public double AverageLoadTimeMs { get; set; }
    
    /// <summary>
    /// 当前缓存的资源数量
    /// </summary>
    public int CachedResourceCount { get; set; }
    
    /// <summary>
    /// 缓存使用的内存大小（字节）
    /// </summary>
    public long CacheMemoryUsage { get; set; }
    
    /// <summary>
    /// 按资源类型统计
    /// </summary>
    public Dictionary<string, long> ResourceTypeStatistics { get; set; } = new();
}