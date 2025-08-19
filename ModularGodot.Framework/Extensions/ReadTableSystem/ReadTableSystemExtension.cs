using System.Reflection;
using Autofac;
using Godot;
using TO.Commons.Abstractions;
using TO.Commons.Extensions;

namespace Extensions.ReadTableSystem;

/// <summary>
/// ReadTableSystem鎵╁睍
/// </summary>
public class ReadTableSystemExtension : FrameworkExtensionBase
{
    public override string Name => "ReadTableSystem Extension";
    public override Version Version => new Version(1, 0, 0);
    public override string Description => "Provides data table reading functionality, supporting attribute and effect data caching and querying";
    public override ExtensionPriority Priority => ExtensionPriority.Low;
    
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
        
        GD.Print("ReadTableSystem Extension: Services configured");
    }
    
    public override void Initialize(IExtensionContext context)
    {
        base.Initialize(context);
        
        // 娉ㄥ唽鎵╁睍鐐?        RegisterExtensionPoint<IReadTableSystemExtensionPoint>(new ReadTableSystemExtensionPoint());
        
        GD.Print("ReadTableSystem Extension: Initialized");
    }
    
    public override void OnStarted(IContainer container)
    {
        base.OnStarted(container);
        
        GD.Print("ReadTableSystem Extension: Started");
    }
    
    protected override void OnDisposing()
    {
        GD.Print("ReadTableSystem Extension: Disposing");
    }
}

/// <summary>
/// ReadTableSystem鎵╁睍鐐规帴鍙?/// </summary>
public interface IReadTableSystemExtensionPoint
{
    void OnSystemInitialized();
    void OnSystemStarted();
    void OnSystemStopped();
}

/// <summary>
/// ReadTableSystem鎵╁睍鐐瑰疄鐜?/// </summary>
public class ReadTableSystemExtensionPoint : IReadTableSystemExtensionPoint
{
    public void OnSystemInitialized()
    {
        GD.Print("ReadTableSystem system initialized");
    }
    
    public void OnSystemStarted()
    {
        GD.Print("ReadTableSystem system started");
    }
    
    public void OnSystemStopped()
    {
        GD.Print("ReadTableSystem system stopped");
    }
}
