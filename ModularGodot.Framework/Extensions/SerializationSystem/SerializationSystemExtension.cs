using System.Reflection;
using Autofac;
using Godot;
using TO.Commons.Abstractions;
using TO.Commons.Extensions;

namespace Extensions.SerializationSystem;

/// <summary>
/// SerializationSystem鎵╁睍
/// </summary>
public class SerializationSystemExtension : FrameworkExtensionBase
{
    public override string Name => "SerializationSystem Extension";
    public override Version Version => new Version(1, 0, 0);
    public override string Description => "Provides serialization functionality, supporting game data saving and loading";
    public override ExtensionPriority Priority => ExtensionPriority.Medium;
    
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
        
        GD.Print("SerializationSystem Extension: Services configured");
    }
    
    public override void Initialize(IExtensionContext context)
    {
        base.Initialize(context);
        
        // 娉ㄥ唽鎵╁睍鐐?        RegisterExtensionPoint<ISerializationSystemExtensionPoint>(new SerializationSystemExtensionPoint());
        
        GD.Print("SerializationSystem Extension: Initialized");
    }
    
    public override void OnStarted(IContainer container)
    {
        base.OnStarted(container);
        
        GD.Print("SerializationSystem Extension: Started");
    }
    
    protected override void OnDisposing()
    {
        GD.Print("SerializationSystem Extension: Disposing");
    }
}

/// <summary>
/// SerializationSystem鎵╁睍鐐规帴鍙?/// </summary>
public interface ISerializationSystemExtensionPoint
{
    void OnSystemInitialized();
    void OnSystemStarted();
    void OnSystemStopped();
}

/// <summary>
/// SerializationSystem鎵╁睍鐐瑰疄鐜?/// </summary>
public class SerializationSystemExtensionPoint : ISerializationSystemExtensionPoint
{
    public void OnSystemInitialized()
    {
        GD.Print("SerializationSystem system initialized");
    }
    
    public void OnSystemStarted()
    {
        GD.Print("SerializationSystem system started");
    }
    
    public void OnSystemStopped()
    {
        GD.Print("SerializationSystem system stopped");
    }
}
