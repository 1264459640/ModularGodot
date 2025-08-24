namespace MF.Infrastructure.Abstractions.ResourceLoading;

/// <summary>
/// 资源加载器接口 - Standard级别
/// </summary>
public interface IResourceLoader
{
    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>资源实例</returns>
    Task<T?> LoadAsync<T>(string path, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// 检查资源是否存在
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取资源元数据
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>资源元数据</returns>
    Task<ResourceMetadata> GetMetadataAsync(string path, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 预加载资源
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>预加载任务</returns>
    Task PreloadAsync(string path, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns>卸载任务</returns>
    Task UnloadAsync(string path);
    
    /// <summary>
    /// 批量加载资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="paths">资源路径集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>资源字典</returns>
    Task<Dictionary<string, T?>> LoadManyAsync<T>(IEnumerable<string> paths, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// 获取加载器统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    Task<ResourceLoaderStatistics> GetStatisticsAsync();
}