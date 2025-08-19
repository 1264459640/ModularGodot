using System.Reflection;
using Autofac;
using Godot;
using TO.Commons.Abstractions;
using TO.Commons.Extensions;

namespace Extensions.ResourceSystem;

/// <summary>
/// ResourceSystem鎵╁睍
/// </summary>
public class ResourceSystemExtension : FrameworkExtensionBase
{
    public override string Name => "ResourceSystem Extension";
    public override Version Version => new Version(1, 0, 0);
    public override string Description => "Provides resource management functionality, including resource loading, caching, memory monitoring and reference tracking";
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
        
        GD.Print("ResourceSystem Extension: Services configured");
    }
    
    public override void Initialize(IExtensionContext context)
    {
        base.Initialize(context);
        
        // 娉ㄥ唽鎵╁睍鐐?        RegisterExtensionPoint<IResourceSystemExtensionPoint>(new ResourceSystemExtensionPoint());
        
        GD.Print("ResourceSystem Extension: Initialized");
    }
    
    public override void OnStarted(IContainer container)
    {
        base.OnStarted(container);
        
        GD.Print("ResourceSystem Extension: Started");
    }
    
    protected override void OnDisposing()
    {
        GD.Print("ResourceSystem Extension: Disposing");
    }
}

/// <summary>
/// ResourceSystem鎵╁睍鐐规帴鍙?/// </summary>
public interface IResourceSystemExtensionPoint
{
    void OnSystemInitialized();
    void OnSystemStarted();
    void OnSystemStopped();
}

/// <summary>
/// ResourceSystem鎵╁睍鐐瑰疄鐜?/// </summary>
public class ResourceSystemExtensionPoint : IResourceSystemExtensionPoint
{
    public void OnSystemInitialized()
    {
        GD.Print("ResourceSystem system initialized");
    }
    
    public void OnSystemStarted()
    {
        GD.Print("ResourceSystem system started");
    }
    
    public void OnSystemStopped()
    {
        GD.Print("ResourceSystem system stopped");
    }
}
