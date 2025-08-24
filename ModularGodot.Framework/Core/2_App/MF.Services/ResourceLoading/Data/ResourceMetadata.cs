namespace MF.Services.ResourceLoading.Data;

/// <summary>
/// 资源元数据
/// </summary>
public class ResourceMetadata
{
    /// <summary>
    /// 资源路径
    /// </summary>
    public string Path { get; set; } = string.Empty;
    
    /// <summary>
    /// 资源是否存在
    /// </summary>
    public bool Exists { get; set; }
    
    /// <summary>
    /// 资源大小（字节）
    /// </summary>
    public long Size { get; set; }
    
    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime LastModified { get; set; }
    
    /// <summary>
    /// 资源类型
    /// </summary>
    public string ResourceType { get; set; } = string.Empty;
    
    /// <summary>
    /// 资源MIME类型
    /// </summary>
    public string MimeType { get; set; } = string.Empty;
    
    /// <summary>
    /// 资源哈希值
    /// </summary>
    public string Hash { get; set; } = string.Empty;
    
    /// <summary>
    /// 扩展属性
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = new();
}