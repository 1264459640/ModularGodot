using System.Reflection;
using Autofac;
using Godot;
using TO.Commons.Abstractions;
using TO.Commons.Extensions;

namespace Extensions.UISystem;

/// <summary>
/// UISystem鎵╁睍
/// </summary>
public class UISystemExtension : FrameworkExtensionBase
{
    public override string Name => "UISystem Extension";
    public override Version Version => new Version(1, 0, 0);
    public override string Description => "Provides complete UI management functionality, including UI display, hiding, layer management, lifecycle management and animation system";
    public override ExtensionPriority Priority => ExtensionPriority.High;
    
    public override void ConfigureServices(ContainerBuilder builder)
    {
        // 鑷姩娉ㄥ唽褰撳墠绋嬪簭闆嗕腑鐨勬墍鏈夋湇鍔?        var assembly = Assembly.GetExecutingAssembly();
        
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
        
        GD.Print("UISystem Extension: Services configured");
    }
    
    public override void Initialize(IExtensionContext context)
    {
        base.Initialize(context);
        
        // 娉ㄥ唽鎵╁睍鐐?        RegisterExtensionPoint<IUISystemExtensionPoint>(new UISystemExtensionPoint());
        
        GD.Print("UISystem Extension: Initialized");
    }
    
    public override void OnStarted(IContainer container)
    {
        base.OnStarted(container);
        
        GD.Print("UISystem Extension: Started");
    }
    
    protected override void OnDisposing()
    {
        GD.Print("UISystem Extension: Disposing");
    }
}

/// <summary>
/// UISystem鎵╁睍鐐规帴鍙?/// </summary>
public interface IUISystemExtensionPoint
{
    void OnSystemInitialized();
    void OnSystemStarted();
    void OnSystemStopped();
}

/// <summary>
/// UISystem鎵╁睍鐐瑰疄鐜?/// </summary>
public class UISystemExtensionPoint : IUISystemExtensionPoint
{
    public void OnSystemInitialized()
    {
        GD.Print("UISystem system initialized");
    }
    
    public void OnSystemStarted()
    {
        GD.Print("UISystem system started");
    }
    
    public void OnSystemStopped()
    {
        GD.Print("UISystem system stopped");
    }
}
