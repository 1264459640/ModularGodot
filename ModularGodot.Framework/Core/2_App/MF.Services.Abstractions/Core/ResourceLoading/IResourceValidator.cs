namespace MF.Infrastructure.Abstractions.ResourceLoading;

/// <summary>
/// 资源验证器接口
/// </summary>
public interface IResourceValidator
{
    /// <summary>
    /// 验证资源路径是否有效
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns>是否有效</returns>
    bool IsValidPath(string path);
    
    /// <summary>
    /// 验证资源类型是否支持
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <returns>是否支持</returns>
    bool IsSupportedType(Type resourceType);
    
    /// <summary>
    /// 验证资源大小是否在限制范围内
    /// </summary>
    /// <param name="size">资源大小</param>
    /// <returns>是否在限制范围内</returns>
    bool IsValidSize(long size);
}