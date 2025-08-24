namespace MF.Infrastructure.Abstractions.Configuration;

/// <summary>
/// 配置服务抽象接口 - Critical级别
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// 获取配置值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">配置键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>配置值</returns>
    T GetValue<T>(string key, T defaultValue = default!);
    
    /// <summary>
    /// 异步获取配置值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">配置键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>配置值</returns>
    Task<T> GetValueAsync<T>(string key, T defaultValue = default!);
    
    /// <summary>
    /// 设置配置值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    void SetValue<T>(string key, T value);
    
    /// <summary>
    /// 异步设置配置值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    /// <returns>设置任务</returns>
    Task SetValueAsync<T>(string key, T value);
    
    /// <summary>
    /// 检查配置键是否存在
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>是否存在</returns>
    bool HasKey(string key);
    
    /// <summary>
    /// 移除配置键
    /// </summary>
    /// <param name="key">配置键</param>
    void RemoveKey(string key);
    
    /// <summary>
    /// 获取所有配置键
    /// </summary>
    /// <returns>配置键集合</returns>
    IEnumerable<string> GetAllKeys();
    
    /// <summary>
    /// 获取指定前缀的所有配置
    /// </summary>
    /// <param name="prefix">键前缀</param>
    /// <returns>配置字典</returns>
    Dictionary<string, object> GetSection(string prefix);
    
    /// <summary>
    /// 保存配置到持久化存储
    /// </summary>
    /// <returns>保存任务</returns>
    Task SaveAsync();
    
    /// <summary>
    /// 重新加载配置
    /// </summary>
    /// <returns>重新加载任务</returns>
    Task ReloadAsync();
    
    /// <summary>
    /// 配置值变化事件
    /// </summary>
    event Action<string, object?> ValueChanged;
    
    /// <summary>
    /// 配置重新加载事件
    /// </summary>
    event Action ConfigurationReloaded;
}

/// <summary>
/// 配置变化事件参数
/// </summary>
public class ConfigurationChangedEventArgs : EventArgs
{
    /// <summary>
    /// 配置键
    /// </summary>
    public string Key { get; }
    
    /// <summary>
    /// 旧值
    /// </summary>
    public object? OldValue { get; }
    
    /// <summary>
    /// 新值
    /// </summary>
    public object? NewValue { get; }
    
    /// <summary>
    /// 变化时间
    /// </summary>
    public DateTime Timestamp { get; }
    
    public ConfigurationChangedEventArgs(string key, object? oldValue, object? newValue)
    {
        Key = key;
        OldValue = oldValue;
        NewValue = newValue;
        Timestamp = DateTime.UtcNow;
    }
}

/// <summary>
/// 配置提供者接口
/// </summary>
public interface IConfigurationProvider
{
    /// <summary>
    /// 加载配置
    /// </summary>
    /// <returns>配置字典</returns>
    Task<Dictionary<string, object>> LoadAsync();
    
    /// <summary>
    /// 保存配置
    /// </summary>
    /// <param name="configuration">配置字典</param>
    /// <returns>保存任务</returns>
    Task SaveAsync(Dictionary<string, object> configuration);
    
    /// <summary>
    /// 监听配置变化
    /// </summary>
    /// <param name="callback">变化回调</param>
    /// <returns>监听句柄</returns>
    IDisposable Watch(Action callback);
}

/// <summary>
/// 配置源类型
/// </summary>
public enum ConfigurationSourceType
{
    /// <summary>
    /// JSON文件
    /// </summary>
    JsonFile,
    
    /// <summary>
    /// 环境变量
    /// </summary>
    EnvironmentVariables,
    
    /// <summary>
    /// 命令行参数
    /// </summary>
    CommandLine,
    
    /// <summary>
    /// 内存配置
    /// </summary>
    Memory,
    
    /// <summary>
    /// 数据库配置
    /// </summary>
    Database,
    
    /// <summary>
    /// 远程配置
    /// </summary>
    Remote
}