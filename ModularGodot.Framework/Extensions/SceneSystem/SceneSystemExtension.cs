using System.Reflection;
using Autofac;
using Godot;
using TO.Commons.Abstractions;
using TO.Commons.Extensions;

namespace Extensions.SceneSystem;

/// <summary>
/// SceneSystem鎵╁睍
/// </summary>
public class SceneSystemExtension : FrameworkExtensionBase
{
    public override string Name => "SceneSystem Extension";
    public override Version Version => new Version(1, 0, 0);
    public override string Description => "Provides scene management functionality, including scene switching, transition effects and scene lifecycle management";
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
        
        GD.Print("SceneSystem Extension: Services configured");
    }
    
    public override void Initialize(IExtensionContext context)
    {
        base.Initialize(context);
        
        // 娉ㄥ唽鎵╁睍鐐?        RegisterExtensionPoint<ISceneSystemExtensionPoint>(new SceneSystemExtensionPoint());
        
        GD.Print("SceneSystem Extension: Initialized");
    }
    
    public override void OnStarted(IContainer container)
    {
        base.OnStarted(container);
        
        GD.Print("SceneSystem Extension: Started");
    }
    
    protected override void OnDisposing()
    {
        GD.Print("SceneSystem Extension: Disposing");
    }
}

/// <summary>
/// SceneSystem鎵╁睍鐐规帴鍙?/// </summary>
public interface ISceneSystemExtensionPoint
{
    void OnSystemInitialized();
    void OnSystemStarted();
    void OnSystemStopped();
}

/// <summary>
/// SceneSystem鎵╁睍鐐瑰疄鐜?/// </summary>
public class SceneSystemExtensionPoint : ISceneSystemExtensionPoint
{
    public void OnSystemInitialized()
    {
        GD.Print("SceneSystem system initialized");
    }
    
    public void OnSystemStarted()
    {
        GD.Print("SceneSystem system started");
    }
    
    public void OnSystemStopped()
    {
        GD.Print("SceneSystem system stopped");
    }
}
