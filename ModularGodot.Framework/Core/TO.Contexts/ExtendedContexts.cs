using Autofac;
using TO.Commons;
using TO.Commons.Abstractions;
using TO.Commons.Extensions;
using TO.Nodes.Abstractions.Bases;
using TO.Repositories.Bases;
using TO.Services.Abstractions.Core.SequenceSystem;
using TO.Services.Abstractions.Core.UISystem;
using TO.Services.Bases;

namespace TO.Contexts;

/// <summary>
/// 扩展支持的上下文管理器
/// </summary>
public class ExtendedContexts : LazySingleton<ExtendedContexts>
{
    private IContainer Container { get; init; }
    private NodeRegister? _nodeRegister;
    private readonly ExtensionManager _extensionManager;
    
    public ExtendedContexts()
    {
        _extensionManager = ExtensionManager.Instance;
        
        var builder = new ContainerBuilder();
        
        // 注册原有模块
        builder.RegisterType<NodeRegister>().SingleInstance();
        builder.RegisterModule<SingleModule>();
        builder.RegisterModule<MediatorModule>();
        
        // 配置扩展服务
        _extensionManager.ConfigureServices(builder);
        
        Container = builder.Build();
        
        // 初始化扩展
        var context = new ExtensionContext(Container);
        _extensionManager.InitializeExtensions(context);
        
        // 启动扩展
        _extensionManager.StartExtensions(Container);
        
        _nodeRegister = Container.Resolve<NodeRegister>();
        
        // 解析核心服务
        try
        {
            Container.Resolve<IUIManagerService>();
        }
        catch
        {
            // UI服务可能不存在，忽略错误
        }
        
        try
        {
            Container.Resolve<ISequenceManagerService>();
        }
        catch
        {
            // 序列服务可能不存在，忽略错误
        }
    }
    
    /// <summary>
    /// 注册单例节点
    /// </summary>
    /// <typeparam name="T">节点类型</typeparam>
    /// <param name="singleton">节点实例</param>
    /// <returns>是否注册成功</returns>
    public bool RegisterSingleNode<T>(T singleton) where T : INode
    {
        return _nodeRegister != null && _nodeRegister.Register(singleton);
    }
    
    /// <summary>
    /// 注册节点
    /// </summary>
    /// <typeparam name="TNode">节点类型</typeparam>
    /// <typeparam name="TRepo">仓储类型</typeparam>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <param name="scene">场景节点</param>
    /// <returns>生命周期作用域</returns>
    public ILifetimeScope RegisterNode<TNode, TRepo, TService>(TNode scene)
        where TNode : class, INode
        where TRepo : NodeRepo<TNode>
        where TService : BaseService
    {
        var scope = Container.BeginLifetimeScope(builder =>
        {
            builder.RegisterInstance(scene).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TRepo>().AsSelf().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TService>().AsSelf().AsImplementedInterfaces().SingleInstance();
        });
        scope.Resolve<TRepo>();
        scope.Resolve<TService>();
        return scope;
    }
    
    /// <summary>
    /// 注册扩展
    /// </summary>
    /// <typeparam name="T">扩展类型</typeparam>
    public void RegisterExtension<T>() where T : IFrameworkExtension, new()
    {
        _extensionManager.RegisterExtension<T>();
    }
    
    /// <summary>
    /// 获取服务
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务实例</returns>
    public T GetService<T>() where T : notnull
    {
        return Container.Resolve<T>();
    }
    
    /// <summary>
    /// 尝试获取服务
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <param name="service">服务实例</param>
    /// <returns>是否获取成功</returns>
    public bool TryGetService<T>(out T? service) where T : class
    {
        return Container.TryResolve(out service);
    }
    
    /// <summary>
    /// 获取扩展点
    /// </summary>
    /// <typeparam name="T">扩展点类型</typeparam>
    /// <returns>扩展点集合</returns>
    public IEnumerable<T> GetExtensionPoints<T>() where T : class
    {
        return _extensionManager.GetExtensionPoints<T>();
    }
    
    /// <summary>
    /// 获取所有扩展
    /// </summary>
    /// <returns>扩展集合</returns>
    public IReadOnlyList<IFrameworkExtension> GetExtensions()
    {
        return _extensionManager.GetExtensions();
    }
    
    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing">是否正在释放</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _extensionManager.StopExtensions(Container);
            Container?.Dispose();
        }
        
        base.Dispose(disposing);
    }
}