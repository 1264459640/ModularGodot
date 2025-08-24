namespace MF.Infrastructure.Abstractions.Caching;

/// <summary>
/// 缓存服务抽象接口 - Critical级别
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// 异步获取缓存值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">缓存键</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>缓存值，如果不存在则返回null</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// 异步设置缓存值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">缓存键</param>
    /// <param name="value">缓存值</param>
    /// <param name="expiration">过期时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设置任务</returns>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// 检查缓存键是否存在
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 移除缓存项
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>移除任务</returns>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 清空所有缓存
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>清空任务</returns>
    Task ClearAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取缓存统计信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>统计信息</returns>
    Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 批量获取缓存值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="keys">缓存键集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>键值对字典</returns>
    Task<Dictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// 批量设置缓存值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="items">键值对字典</param>
    /// <param name="expiration">过期时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设置任务</returns>
    Task SetManyAsync<T>(Dictionary<string, T> items, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;
}