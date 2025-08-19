using Godot;
using TO.Contexts;
using Extensions.AudioSystem;
using Extensions.UISystem;
using Extensions.SceneSystem;
using Extensions.ResourceSystem;
using Extensions.EventBusSystem;
using Extensions.LogSystem;
using Extensions.SequenceSystem;
using Extensions.SerializationSystem;
using Extensions.GameAbilitySystem;
using Extensions.ReadTableSystem;

namespace Extensions;

/// <summary>
/// 扩展注册示例
/// 展示如何注册和使用所有系统扩展
/// </summary>
public partial class ExtensionRegistrationExample : Node
{
    public override void _Ready()
    {
        GD.Print("=== ModularGodot Extensions Registration ===");
        
        // 按优先级顺序注册扩展
        RegisterHighPriorityExtensions();
        RegisterMediumPriorityExtensions();
        RegisterLowPriorityExtensions();
        
        GD.Print("=== All Extensions Registered ===");
        
        // 演示扩展使用
        DemonstrateExtensionUsage();
    }
    
    /// <summary>
    /// 注册高优先级扩展
    /// </summary>
    private void RegisterHighPriorityExtensions()
    {
        GD.Print("Registering High Priority Extensions...");
        
        // 核心基础设施扩展
        ExtendedContexts.Instance.RegisterExtension<LogSystemExtension>();
        ExtendedContexts.Instance.RegisterExtension<EventBusSystemExtension>();
        ExtendedContexts.Instance.RegisterExtension<ResourceSystemExtension>();
        
        // 音频系统扩展
        ExtendedContexts.Instance.RegisterExtension<AudioSystemExtension>();
        
        // UI系统扩展
        ExtendedContexts.Instance.RegisterExtension<UISystemExtension>();
        
        // 场景系统扩展
        ExtendedContexts.Instance.RegisterExtension<SceneSystemExtension>();
    }
    
    /// <summary>
    /// 注册中等优先级扩展
    /// </summary>
    private void RegisterMediumPriorityExtensions()
    {
        GD.Print("Registering Medium Priority Extensions...");
        
        // 序列系统扩展
        ExtendedContexts.Instance.RegisterExtension<SequenceSystemExtension>();
        
        // 序列化系统扩展
        ExtendedContexts.Instance.RegisterExtension<SerializationSystemExtension>();
        
        // 游戏能力系统扩展
        ExtendedContexts.Instance.RegisterExtension<GameAbilitySystemExtension>();
    }
    
    /// <summary>
    /// 注册低优先级扩展
    /// </summary>
    private void RegisterLowPriorityExtensions()
    {
        GD.Print("Registering Low Priority Extensions...");
        
        // 数据表读取系统扩展
        ExtendedContexts.Instance.RegisterExtension<ReadTableSystemExtension>();
    }
    
    /// <summary>
    /// 演示扩展使用
    /// </summary>
    private void DemonstrateExtensionUsage()
    {
        GD.Print("\n=== Extension Usage Demonstration ===");
        
        // 获取所有已注册的扩展
        var extensions = ExtendedContexts.Instance.GetExtensions();
        GD.Print($"Total registered extensions: {extensions.Count}");
        
        foreach (var extension in extensions)
        {
            GD.Print($"- {extension.Name} v{extension.Version} (Priority: {extension.Priority})");
        }
        
        // 演示服务使用
        DemonstrateServiceUsage();
        
        // 演示扩展点使用
        DemonstrateExtensionPoints();
    }
    
    /// <summary>
    /// 演示服务使用
    /// </summary>
    private void DemonstrateServiceUsage()
    {
        GD.Print("\n--- Service Usage Examples ---");
        
        try
        {
            // 音频服务示例
            if (ExtendedContexts.Instance.TryGetService<Extensions.AudioSystem.Services.Abstractions.IAudioManagerService>(out var audioService))
            {
                GD.Print("✓ Audio Service available");
                // audioService.SetVolume(AudioEnums.AudioType.Master, 0.8f);
            }
            
            // UI服务示例（如果实现了的话）
            // if (ExtendedContexts.Instance.TryGetService<IUIManagerService>(out var uiService))
            // {
            //     GD.Print("✓ UI Service available");
            // }
            
            // 场景服务示例（如果实现了的话）
            // if (ExtendedContexts.Instance.TryGetService<ISceneManagerService>(out var sceneService))
            // {
            //     GD.Print("✓ Scene Service available");
            // }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Service usage error: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 演示扩展点使用
    /// </summary>
    private void DemonstrateExtensionPoints()
    {
        GD.Print("\n--- Extension Points Examples ---");
        
        try
        {
            // 音频系统扩展点
            var audioExtensionPoints = ExtendedContexts.Instance.GetExtensionPoints<IAudioSystemExtensionPoint>();
            GD.Print($"Audio extension points: {audioExtensionPoints.Count()}");
            
            foreach (var point in audioExtensionPoints)
            {
                point.OnAudioPlayed("example.wav", 1.0f);
            }
            
            // UI系统扩展点
            var uiExtensionPoints = ExtendedContexts.Instance.GetExtensionPoints<IUISystemExtensionPoint>();
            GD.Print($"UI extension points: {uiExtensionPoints.Count()}");
            
            // 其他扩展点...
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Extension points error: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 节点退出时的清理
    /// </summary>
    public override void _ExitTree()
    {
        GD.Print("Extension Registration Example: Exiting");
        base._ExitTree();
    }
}

/// <summary>
/// 自定义扩展示例
/// 展示如何创建监听其他扩展事件的扩展
/// </summary>
public class CustomExtensionExample : TO.Commons.Abstractions.FrameworkExtensionBase, 
    IAudioSystemExtensionPoint, 
    IUISystemExtensionPoint
{
    public override string Name => "Custom Extension Example";
    public override Version Version => new Version(1, 0, 0);
    public override string Description => "演示如何创建自定义扩展并监听其他扩展的事件";
    public override TO.Commons.Abstractions.ExtensionPriority Priority => TO.Commons.Abstractions.ExtensionPriority.Low;
    
    public override void Initialize(TO.Commons.Abstractions.IExtensionContext context)
    {
        base.Initialize(context);
        
        // 注册为多个扩展点的监听器
        RegisterExtensionPoint<IAudioSystemExtensionPoint>(this);
        RegisterExtensionPoint<IUISystemExtensionPoint>(this);
        
        GD.Print("Custom Extension Example: Initialized as extension point listener");
    }
    
    // 实现音频系统扩展点
    public void OnAudioPlayed(string path, float volume)
    {
        GD.Print($"Custom Extension: Audio played - {path} at volume {volume}");
    }
    
    public void OnVolumeChanged(TO.Commons.Enums.System.AudioEnums.AudioType type, float volume)
    {
        GD.Print($"Custom Extension: Volume changed - {type} to {volume}");
    }
    
    public void OnMuteStateChanged(bool muted)
    {
        GD.Print($"Custom Extension: Mute state changed - {muted}");
    }
    
    // 实现UI系统扩展点
    public void OnUIShown(TO.Commons.Enums.UI.UIName uiName)
    {
        GD.Print($"Custom Extension: UI shown - {uiName}");
    }
    
    public void OnUIHidden(TO.Commons.Enums.UI.UIName uiName)
    {
        GD.Print($"Custom Extension: UI hidden - {uiName}");
    }
    
    public void OnUICreated(TO.Commons.Enums.UI.UIName uiName)
    {
        GD.Print($"Custom Extension: UI created - {uiName}");
    }
    
    public void OnUIDestroyed(TO.Commons.Enums.UI.UIName uiName)
    {
        GD.Print($"Custom Extension: UI destroyed - {uiName}");
    }
    
    public void OnUIAnimationStarted(TO.Commons.Enums.UI.UIName uiName, string animationName)
    {
        GD.Print($"Custom Extension: UI animation started - {uiName}: {animationName}");
    }
    
    public void OnUIAnimationCompleted(TO.Commons.Enums.UI.UIName uiName, string animationName)
    {
        GD.Print($"Custom Extension: UI animation completed - {uiName}: {animationName}");
    }
}