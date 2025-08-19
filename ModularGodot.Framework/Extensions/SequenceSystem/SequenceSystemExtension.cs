using System.Reflection;
using Autofac;
using Godot;
using TO.Commons.Abstractions;
using TO.Commons.Extensions;

namespace Extensions.SequenceSystem;

/// <summary>
/// SequenceSystem鎵╁睍
/// </summary>
public class SequenceSystemExtension : FrameworkExtensionBase
{
    public override string Name => "SequenceSystem Extension";
    public override Version Version => new Version(1, 0, 0);
    public override string Description => "Provides sequence management functionality, supporting timeline and action sequences";
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
        
        GD.Print("SequenceSystem Extension: Services configured");
    }
    
    public override void Initialize(IExtensionContext context)
    {
        base.Initialize(context);
        
        // 娉ㄥ唽鎵╁睍鐐?        RegisterExtensionPoint<ISequenceSystemExtensionPoint>(new SequenceSystemExtensionPoint());
        
        GD.Print("SequenceSystem Extension: Initialized");
    }
    
    public override void OnStarted(IContainer container)
    {
        base.OnStarted(container);
        
        GD.Print("SequenceSystem Extension: Started");
    }
    
    protected override void OnDisposing()
    {
        GD.Print("SequenceSystem Extension: Disposing");
    }
}

/// <summary>
/// SequenceSystem鎵╁睍鐐规帴鍙?/// </summary>
public interface ISequenceSystemExtensionPoint
{
    void OnSystemInitialized();
    void OnSystemStarted();
    void OnSystemStopped();
}

/// <summary>
/// SequenceSystem鎵╁睍鐐瑰疄鐜?/// </summary>
public class SequenceSystemExtensionPoint : ISequenceSystemExtensionPoint
{
    public void OnSystemInitialized()
    {
        GD.Print("SequenceSystem system initialized");
    }
    
    public void OnSystemStarted()
    {
        GD.Print("SequenceSystem system started");
    }
    
    public void OnSystemStopped()
    {
        GD.Print("SequenceSystem system stopped");
    }
}
