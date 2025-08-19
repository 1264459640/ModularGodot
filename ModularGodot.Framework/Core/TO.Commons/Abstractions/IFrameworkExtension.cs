using Autofac;

namespace TO.Commons.Abstractions;

/// <summary>
/// 框架扩展接口
/// </summary>
public interface IFrameworkExtension
{
    /// <summary>
    /// 扩展名称
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// 扩展版本
    /// </summary>
    Version Version { get; }
    
    /// <summary>
    /// 扩展描述
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// 扩展优先级
    /// </summary>
    ExtensionPriority Priority { get; }
    
    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="builder">容器构建器</param>
    void ConfigureServices(ContainerBuilder builder);
    
    /// <summary>
    /// 初始化扩展
    /// </summary>
    /// <param name="context">扩展上下文</param>
    void Initialize(IExtensionContext context);
    
    /// <summary>
    /// 扩展启动完成
    /// </summary>
    /// <param name="container">容器</param>
    void OnStarted(IContainer container);
    
    /// <summary>
    /// 扩展停止前
    /// </summary>
    /// <param name="container">容器</param>
    void OnStopping(IContainer container);
    
    /// <summary>
    /// 释放资源
    /// </summary>
    void Dispose();
}

/// <summary>
/// 扩展优先级
/// </summary>
public enum ExtensionPriority
{
    /// <summary>
    /// 最低优先级
    /// </summary>
    Lowest = 0,
    
    /// <summary>
    /// 低优先级
    /// </summary>
    Low = 25,
    
    /// <summary>
    /// 普通优先级
    /// </summary>
    Normal = 50,
    
    /// <summary>
    /// 高优先级
    /// </summary>
    High = 75,
    
    /// <summary>
    /// 最高优先级
    /// </summary>
    Highest = 100
}

/// <summary>
/// 扩展上下文接口
/// </summary>
public interface IExtensionContext
{
    /// <summary>
    /// 容器
    /// </summary>
    IContainer Container { get; }
    
    /// <summary>
    /// 获取服务
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务实例</returns>
    T GetService<T>() where T : notnull;
    
    /// <summary>
    /// 尝试获取服务
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <param name="service">服务实例</param>
    /// <returns>是否获取成功</returns>
    bool TryGetService<T>(out T? service) where T : class;
}