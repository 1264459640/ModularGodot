namespace MF.Data.Core.Interfaces;

/// <summary>
/// 序列化器接口
/// </summary>
public interface ISerializer
{
    /// <summary>
    /// 序列化对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="obj">要序列化的对象</param>
    /// <returns>序列化后的字符串</returns>
    string Serialize<T>(T obj);
    
    /// <summary>
    /// 反序列化对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="data">序列化的数据</param>
    /// <returns>反序列化后的对象</returns>
    T? Deserialize<T>(string data);
    
    /// <summary>
    /// 序列化对象到字节数组
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="obj">要序列化的对象</param>
    /// <returns>序列化后的字节数组</returns>
    byte[] SerializeToBytes<T>(T obj);
    
    /// <summary>
    /// 从字节数组反序列化对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="data">序列化的字节数据</param>
    /// <returns>反序列化后的对象</returns>
    T? DeserializeFromBytes<T>(byte[] data);
    
    /// <summary>
    /// 异步序列化对象到流
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="obj">要序列化的对象</param>
    /// <param name="stream">目标流</param>
    /// <returns>序列化任务</returns>
    Task SerializeToStreamAsync<T>(T obj, Stream stream);
    
    /// <summary>
    /// 异步从流反序列化对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="stream">源流</param>
    /// <returns>反序列化后的对象</returns>
    Task<T?> DeserializeFromStreamAsync<T>(Stream stream);
}

/// <summary>
/// JSON序列化器接口
/// </summary>
public interface IJsonSerializer : ISerializer
{
    /// <summary>
    /// 设置序列化选项
    /// </summary>
    /// <param name="options">序列化选项</param>
    void SetOptions(object options);
    
    /// <summary>
    /// 格式化JSON输出
    /// </summary>
    bool Indented { get; set; }
}

/// <summary>
/// 二进制序列化器接口
/// </summary>
public interface IBinarySerializer : ISerializer
{
    /// <summary>
    /// 是否启用压缩
    /// </summary>
    bool EnableCompression { get; set; }
    
    /// <summary>
    /// 压缩级别
    /// </summary>
    int CompressionLevel { get; set; }
}