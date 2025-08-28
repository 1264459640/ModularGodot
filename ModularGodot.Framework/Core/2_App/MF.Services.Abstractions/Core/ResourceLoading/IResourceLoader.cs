using MF.Commons.Core.Enums.Infrastructure;

namespace MF.Services.Abstractions.Core.ResourceLoading;

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
    /// 异步加载资源（带缓存策略）
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <param name="cacheStrategy">缓存策略</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>资源实例</returns>
    Task<T?> LoadAsync<T>(string path, ResourceCacheStrategy cacheStrategy, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// 检查资源是否存在
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 预加载资源
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>预加载任务</returns>
    Task PreloadAsync(string path, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 批量预加载资源
    /// </summary>
    /// <param name="paths">资源路径列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>预加载任务</returns>
    Task PreloadBatchAsync(IEnumerable<string> paths, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns>卸载任务</returns>
    Task UnloadAsync(string path);
    
    /// <summary>
    /// 清理缓存
    /// </summary>
    /// <param name="force">是否强制清理</param>
    /// <returns>清理任务</returns>
    Task ClearCacheAsync(bool force = false);
}