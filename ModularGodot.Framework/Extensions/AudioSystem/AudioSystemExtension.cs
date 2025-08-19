using System.Reflection;
using Autofac;
using Godot;
using TO.Commons.Abstractions;
using TO.Commons.Extensions;

namespace Extensions.AudioSystem;

/// <summary>
/// 音频系统扩展
/// </summary>
public class AudioSystemExtension : FrameworkExtensionBase
{
    public override string Name => "Audio System Extension";
    public override Version Version => new Version(1, 0, 0);
    public override string Description => "提供完整的音频管理功能，包括背景音乐、音效、语音等";
    public override ExtensionPriority Priority => ExtensionPriority.High;
    
    public override void ConfigureServices(ContainerBuilder builder)
    {
        // 自动注册当前程序集中的所有服务
        var assembly = Assembly.GetExecutingAssembly();
        
        // 注册音频服务
        builder.RegisterAssemblyTypes(assembly)
            .Where(t => t.Name.EndsWith("Service") && !t.IsAbstract)
            .AsImplementedInterfaces()
            .SingleInstance();
            
        // 注册音频仓储
        builder.RegisterAssemblyTypes(assembly)
            .Where(t => t.Name.EndsWith("Repo") && !t.IsAbstract)
            .AsImplementedInterfaces()
            .SingleInstance();
            
        // 注册命令处理器
        builder.RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(MediatR.IRequestHandler<,>))
            .AsImplementedInterfaces();
            
        builder.RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(MediatR.IRequestHandler<>))
            .AsImplementedInterfaces();
        
        GD.Print("Audio System Extension: Services configured");
    }
    
    public override void Initialize(IExtensionContext context)
    {
        base.Initialize(context);
        
        // 注册音频系统扩展点
        RegisterExtensionPoint<IAudioSystemExtensionPoint>(new AudioSystemExtensionPoint());
        
        GD.Print("Audio System Extension: Initialized");
    }
    
    public override void OnStarted(IContainer container)
    {
        base.OnStarted(container);
        
        // 初始化音频总线
        if (container.TryResolve<Services.Abstractions.IAudioManagerService>(out var audioService))
        {
            audioService.SetupAudioBuses();
            GD.Print("Audio System Extension: Audio buses initialized");
        }
    }
    
    protected override void OnDisposing()
    {
        GD.Print("Audio System Extension: Disposing");
    }
}

/// <summary>
/// 音频系统扩展点接口
/// </summary>
public interface IAudioSystemExtensionPoint
{
    void OnAudioPlayed(string path, float volume);
    void OnVolumeChanged(TO.Commons.Enums.System.AudioEnums.AudioType type, float volume);
    void OnMuteStateChanged(bool muted);
}

/// <summary>
/// 音频系统扩展点实现
/// </summary>
public class AudioSystemExtensionPoint : IAudioSystemExtensionPoint
{
    public void OnAudioPlayed(string path, float volume)
    {
        // 扩展点实现，其他扩展可以监听音频播放事件
        GD.Print($"Audio played: {path} at volume {volume}");
    }
    
    public void OnVolumeChanged(TO.Commons.Enums.System.AudioEnums.AudioType type, float volume)
    {
        // 扩展点实现，其他扩展可以监听音量变化事件
        GD.Print($"Volume changed: {type} to {volume}");
    }
    
    public void OnMuteStateChanged(bool muted)
    {
        // 扩展点实现，其他扩展可以监听静音状态变化事件
        GD.Print($"Mute state changed: {muted}");
    }
}