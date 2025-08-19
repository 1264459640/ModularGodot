using System.Reflection;
using Autofac;
using Godot;
using TO.Commons.Abstractions;

namespace TO.Commons.Extensions;

/// <summary>
/// 扩展管理器
/// </summary>
public class ExtensionManager : LazySingleton<ExtensionManager>
{
    private readonly List<IFrameworkExtension> _extensions = new();
    private readonly Dictionary<Type, List<object>> _extensionPoints = new();
    private bool _initialized = false;
    
    /// <summary>
    /// 注册扩展
    /// </summary>
    /// <typeparam name="T">扩展类型</typeparam>
    public void RegisterExtension<T>() where T : IFrameworkExtension, new()
    {
        if (_initialized)
        {
            GD.PrintErr("Cannot register extensions after initialization");
            return;
        }
        
        var extension = new T();
        _extensions.Add(extension);
        
        // 按优先级排序
        _extensions.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        
        GD.Print($"Registered extension: {extension.Name} v{extension.Version}");
    }
    
    /// <summary>
    /// 注册扩展点
    /// </summary>
    /// <typeparam name="T">扩展点类型</typeparam>
    /// <param name="extensionPoint">扩展点实例</param>
    public void RegisterExtensionPoint<T>(T extensionPoint) where T : class
    {
        var type = typeof(T);
        if (!_extensionPoints.ContainsKey(type))
            _extensionPoints[type] = new List<object>();
        
        _extensionPoints[type].Add(extensionPoint);
    }
    
    /// <summary>
    /// 获取扩展点
    /// </summary>
    /// <typeparam name="T">扩展点类型</typeparam>
    /// <returns>扩展点集合</returns>
    public IEnumerable<T> GetExtensionPoints<T>() where T : class
    {
        var type = typeof(T);
        return _extensionPoints.ContainsKey(type) 
            ? _extensionPoints[type].Cast<T>() 
            : Enumerable.Empty<T>();
    }
    
    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="builder">容器构建器</param>
    public void ConfigureServices(ContainerBuilder builder)
    {
        foreach (var extension in _extensions)
        {
            try
            {
                extension.ConfigureServices(builder);
                GD.Print($"Configured services for extension: {extension.Name}");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Failed to configure services for extension {extension.Name}: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// 初始化扩展
    /// </summary>
    /// <param name="context">扩展上下文</param>
    public void InitializeExtensions(IExtensionContext context)
    {
        if (_initialized)
            return;
            
        foreach (var extension in _extensions)
        {
            try
            {
                extension.Initialize(context);
                GD.Print($"Initialized extension: {extension.Name}");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Failed to initialize extension {extension.Name}: {ex.Message}");
            }
        }
        
        _initialized = true;
    }
    
    /// <summary>
    /// 启动扩展
    /// </summary>
    /// <param name="container">容器</param>
    public void StartExtensions(IContainer container)
    {
        foreach (var extension in _extensions)
        {
            try
            {
                extension.OnStarted(container);
                GD.Print($"Started extension: {extension.Name}");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Failed to start extension {extension.Name}: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// 停止扩展
    /// </summary>
    /// <param name="container">容器</param>
    public void StopExtensions(IContainer container)
    {
        // 按相反顺序停止
        for (int i = _extensions.Count - 1; i >= 0; i--)
        {
            try
            {
                _extensions[i].OnStopping(container);
                GD.Print($"Stopped extension: {_extensions[i].Name}");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Failed to stop extension {_extensions[i].Name}: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// 释放资源
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var extension in _extensions)
            {
                try
                {
                    extension.Dispose();
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"Failed to dispose extension {extension.Name}: {ex.Message}");
                }
            }
            
            _extensions.Clear();
            _extensionPoints.Clear();
        }
        
        base.Dispose(disposing);
    }
    
    /// <summary>
    /// 获取所有扩展
    /// </summary>
    /// <returns>扩展集合</returns>
    public IReadOnlyList<IFrameworkExtension> GetExtensions()
    {
        return _extensions.AsReadOnly();
    }
}

/// <summary>
/// 扩展上下文实现
/// </summary>
public class ExtensionContext : IExtensionContext
{
    public IContainer Container { get; }
    
    public ExtensionContext(IContainer container)
    {
        Container = container;
    }
    
    public T GetService<T>() where T : notnull
    {
        return Container.Resolve<T>();
    }
    
    public bool TryGetService<T>(out T? service) where T : class
    {
        return Container.TryResolve(out service);
    }
}