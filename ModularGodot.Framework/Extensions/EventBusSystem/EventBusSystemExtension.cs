using System.Reflection;
using Autofac;
using Godot;
using TO.Commons.Abstractions;
using TO.Commons.Extensions;

namespace Extensions.EventBusSystem;

/// <summary>
/// EventBusSystem鎵╁睍
/// </summary>
public class EventBusSystemExtension : FrameworkExtensionBase
{
    public override string Name => "EventBusSystem Extension";
    public override Version Version => new Version(1, 0, 0);
    public override string Description => "Provides event bus functionality, supporting publish-subscribe pattern event communication";
    public override ExtensionPriority Priority => ExtensionPriority.High;
    
    public override void ConfigureServices(ContainerBuilder builder)
    {
        // 鑷姩娉ㄥ唽褰撳墠绋嬪簭闆嗕腑鐨勬墍鏈夋湇鍔?        
        var assembly = Assembly.GetExecutingAssembly();
        
        // 娉ㄥ唽鏈嶅姟
        builder.RegisterAssemblyTypes(assembly)
            .Where(t => t.Name.EndsWith("Service") && !t.IsAbstract)
            .AsImplementedInterfaces()
            .SingleInstance();
            
        // 娉ㄥ唽浠撳偍
        builder.RegisterAssemblyTypes(assembly)
            .Where(t => t.Name.EndsWith("Repo") && !t.IsAbstract)
            .AsImplementedInterfaces()
            .SingleInstance();
            
        // 娉ㄥ唽鍛戒护澶勭悊鍣?        builder.RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(MediatR.IRequestHandler<,>))
            .AsImplementedInterfaces();
            
        builder.RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(MediatR.IRequestHandler<>))
            .AsImplementedInterfaces();
        
        GD.Print("EventBusSystem Extension: Services configured");
    }
    
    public override void Initialize(IExtensionContext context)
    {
        base.Initialize(context);
        
        // 娉ㄥ唽鎵╁睍鐐?        RegisterExtensionPoint<IEventBusSystemExtensionPoint>(new EventBusSystemExtensionPoint());
        
        GD.Print("EventBusSystem Extension: Initialized");
    }
    
    public override void OnStarted(IContainer container)
    {
        base.OnStarted(container);
        
        GD.Print("EventBusSystem Extension: Started");
    }
    
    protected override void OnDisposing()
    {
        GD.Print("EventBusSystem Extension: Disposing");
    }
}

/// <summary>
/// EventBusSystem鎵╁睍鐐规帴鍙?/// </summary>
public interface IEventBusSystemExtensionPoint
{
    void OnSystemInitialized();
    void OnSystemStarted();
    void OnSystemStopped();
}

/// <summary>
/// EventBusSystem鎵╁睍鐐瑰疄鐜?/// </summary>
public class EventBusSystemExtensionPoint : IEventBusSystemExtensionPoint
{
    public void OnSystemInitialized()
    {
        GD.Print("EventBusSystem system initialized");
    }
    
    public void OnSystemStarted()
    {
        GD.Print("EventBusSystem system started");
    }
    
    public void OnSystemStopped()
    {
        GD.Print("EventBusSystem system stopped");
    }
}
