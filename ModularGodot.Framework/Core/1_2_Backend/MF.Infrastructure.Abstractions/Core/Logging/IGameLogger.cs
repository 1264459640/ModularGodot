using Microsoft.Extensions.Logging;

namespace MF.Infrastructure.Abstractions.Logging;

/// <summary>
/// 游戏日志接口 - Standard级别
/// </summary>
public interface IGameLogger
{
    /// <summary>
    /// 记录调试信息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="args">参数</param>
    void LogDebug(string message, params object[] args);
    
    /// <summary>
    /// 记录一般信息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="args">参数</param>
    void LogInformation(string message, params object[] args);
    
    /// <summary>
    /// 记录警告信息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="args">参数</param>
    void LogWarning(string message, params object[] args);
    
    /// <summary>
    /// 记录错误信息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="args">参数</param>
    void LogError(string message, params object[] args);
    
    /// <summary>
    /// 记录错误信息（带异常）
    /// </summary>
    /// <param name="exception">异常</param>
    /// <param name="message">消息</param>
    /// <param name="args">参数</param>
    void LogError(Exception exception, string message, params object[] args);
    
    /// <summary>
    /// 记录严重错误信息
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="args">参数</param>
    void LogCritical(string message, params object[] args);
    
    /// <summary>
    /// 记录严重错误信息（带异常）
    /// </summary>
    /// <param name="exception">异常</param>
    /// <param name="message">消息</param>
    /// <param name="args">参数</param>
    void LogCritical(Exception exception, string message, params object[] args);
    
    /// <summary>
    /// 设置日志级别
    /// </summary>
    /// <param name="level">日志级别</param>
    void SetLevel(LogLevel level);
    
    /// <summary>
    /// 启用或禁用文件日志
    /// </summary>
    /// <param name="enable">是否启用</param>
    void EnableFileLogging(bool enable);
    
    /// <summary>
    /// 设置日志文件路径
    /// </summary>
    /// <param name="path">文件路径</param>
    void SetLogFilePath(string path);
}

/// <summary>
/// 泛型游戏日志接口
/// </summary>
/// <typeparam name="T">日志类别类型</typeparam>
public interface IGameLogger<T> : IGameLogger
{
}

/// <summary>
/// 日志工厂接口
/// </summary>
public interface ILoggerFactory
{
    /// <summary>
    /// 创建日志器
    /// </summary>
    /// <param name="categoryName">类别名称</param>
    /// <returns>日志器实例</returns>
    IGameLogger CreateLogger(string categoryName);
    
    /// <summary>
    /// 创建泛型日志器
    /// </summary>
    /// <typeparam name="T">类别类型</typeparam>
    /// <returns>日志器实例</returns>
    IGameLogger<T> CreateLogger<T>();
}

/// <summary>
/// 日志配置
/// </summary>
public class LoggingConfig
{
    /// <summary>
    /// 最小日志级别
    /// </summary>
    public LogLevel MinLevel { get; set; } = LogLevel.Information;
    
    /// <summary>
    /// 是否启用控制台日志
    /// </summary>
    public bool EnableConsoleLogging { get; set; } = true;
    
    /// <summary>
    /// 是否启用文件日志
    /// </summary>
    public bool EnableFileLogging { get; set; } = false;
    
    /// <summary>
    /// 日志文件路径
    /// </summary>
    public string LogFilePath { get; set; } = "user://logs/game.log";
    
    /// <summary>
    /// 日志文件最大大小（字节）
    /// </summary>
    public long MaxLogFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
    
    /// <summary>
    /// 保留的日志文件数量
    /// </summary>
    public int RetainedFileCount { get; set; } = 5;
    
    /// <summary>
    /// 是否启用结构化日志
    /// </summary>
    public bool EnableStructuredLogging { get; set; } = false;
}