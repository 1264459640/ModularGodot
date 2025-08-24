namespace MF.Infrastructure.Abstractions.Caching;

/// <summary>
/// 缓存策略
/// </summary>
public enum CachePolicy
{
    /// <summary>
    /// 普通缓存
    /// </summary>
    Normal,
    
    /// <summary>
    /// 高优先级缓存
    /// </summary>
    High,
    
    /// <summary>
    /// 低优先级缓存
    /// </summary>
    Low,
    
    /// <summary>
    /// 永不过期
    /// </summary>
    NeverExpire
}