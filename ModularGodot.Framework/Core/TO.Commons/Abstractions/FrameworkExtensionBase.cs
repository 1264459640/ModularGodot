using Autofac;
using Godot;
using TO.Commons.Extensions;

namespace TO.Commons.Abstractions;

/// <summary>
/// 框架扩展基类
/// </summary>
public abstract class FrameworkExtensionBase : IFrameworkExtension
{
    /// <summary>
    /// 扩展名称
    /// </summary>
    public abstract string Name { get; }
    
    /// <summary>
    /// 扩展版本
    /// </summary>
    public abstract Version Version { get; }
    
    /// <summary>
    /// 扩展描述
    /// </summary>
    public virtual string Description => string.Empty;
    
    /// <summary>
    /// 扩展优先级
    /// </summary>
    public virtual ExtensionPriority Priority => ExtensionPriority.Normal;
    
    /// <summary>
    /// 是否已释放
    /// </summary>
    protected bool IsDisposed { get; private set; }
    
    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="builder">容器构建器</param>
    public virtual void ConfigureServices(ContainerBuilder builder)
    {
        // 默认实现为空，子类可以重写
    }
    
    /// <summary>
    /// 初始化扩展
    /// </summary>
    /// <param name="context">扩展上下文</param>
    public virtual void Initialize(IExtensionContext context)
    {
        GD.Print($"Initializing extension: {Name}");
    }
    
    /// <summary>
    /// 扩展启动完成
    /// </summary>
    /// <param name="container">容器</param>
    public virtual void OnStarted(IContainer container)
    {
        GD.Print($"Extension started: {Name}");
    }
    
    /// <summary>
    /// 扩展停止前
    /// </summary>
    /// <param name="container">容器</param>
    public virtual void OnStopping(IContainer container)
    {
        GD.Print($"Extension stopping: {Name}");
    }
    
    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing">是否正在释放</param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
            return;
            
        if (disposing)
        {
            // 释放托管资源
            OnDisposing();
        }
        
        IsDisposed = true;
    }
    
    /// <summary>
    /// 释放资源时调用
    /// </summary>
    protected virtual void OnDisposing()
    {
        // 子类可以重写此方法来释放资源
    }
    
    /// <summary>
    /// 注册扩展点
    /// </summary>
    /// <typeparam name="T">扩展点类型</typeparam>
    /// <param name="extensionPoint">扩展点实例</param>
    protected void RegisterExtensionPoint<T>(T extensionPoint) where T : class
    {
        ExtensionManager.Instance.RegisterExtensionPoint(extensionPoint);
    }
    
    /// <summary>
    /// 获取扩展点
    /// </summary>
    /// <typeparam name="T">扩展点类型</typeparam>
    /// <returns>扩展点集合</returns>
    protected IEnumerable<T> GetExtensionPoints<T>() where T : class
    {
        return ExtensionManager.Instance.GetExtensionPoints<T>();
    }
    
    /// <summary>
    /// 析构函数
    /// </summary>
    ~FrameworkExtensionBase()
    {
        Dispose(false);
    }
}