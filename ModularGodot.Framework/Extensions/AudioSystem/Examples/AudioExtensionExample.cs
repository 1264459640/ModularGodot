using Godot;
using MediatR;
using TO.Commons.Enums.System;
using TO.Contexts;
using Extensions.AudioSystem;
using Extensions.AudioSystem.Commands;
using Extensions.AudioSystem.Services.Abstractions;

namespace Extensions.AudioSystem.Examples;

/// <summary>
/// 音频扩展使用示例
/// </summary>
public partial class AudioExtensionExample : Node
{
    private IAudioManagerService? _audioService;
    private IMediator? _mediator;
    
    public override void _Ready()
    {
        // 注册音频系统扩展
        ExtendedContexts.Instance.RegisterExtension<AudioSystemExtension>();
        
        // 获取服务
        _audioService = ExtendedContexts.Instance.GetService<IAudioManagerService>();
        _mediator = ExtendedContexts.Instance.GetService<IMediator>();
        
        GD.Print("Audio Extension Example: Ready");
        
        // 演示基本功能
        DemonstrateBasicFeatures();
    }
    
    private async void DemonstrateBasicFeatures()
    {
        if (_audioService == null || _mediator == null)
        {
            GD.PrintErr("Audio services not available");
            return;
        }
        
        GD.Print("=== Audio Extension Demo ===");
        
        // 1. 设置音量
        GD.Print("1. Setting volumes...");
        _audioService.SetVolume(AudioEnums.AudioType.Master, 0.8f);
        _audioService.SetVolume(AudioEnums.AudioType.Music, 0.6f);
        _audioService.SetVolume(AudioEnums.AudioType.SoundEffect, 0.7f);
        
        // 2. 使用命令模式设置音量
        GD.Print("2. Using command pattern...");
        await _mediator.Send(new SetVolumeCommand(AudioEnums.AudioType.Voice, 0.9f));
        
        // 3. 预加载音频（如果文件存在）
        GD.Print("3. Preloading audio...");
        try
        {
            // 注意：这些路径需要根据实际项目调整
            // _audioService.PreloadAudio("res://Assets/Sounds/SFX/Classic Status Effects/Dispelled.wav", AudioEnums.AudioType.SoundEffect);
        }
        catch (Exception ex)
        {
            GD.Print($"Preload failed (expected if file doesn't exist): {ex.Message}");
        }
        
        // 4. 演示静音功能
        GD.Print("4. Testing mute functionality...");
        _audioService.SetMute(true);
        GD.Print($"Mute state: {_audioService.GetMuteState()}");
        
        await Task.Delay(1000);
        
        _audioService.SetMute(false);
        GD.Print($"Mute state: {_audioService.GetMuteState()}");
        
        // 5. 获取音量信息
        GD.Print("5. Current volumes:");
        GD.Print($"   Master: {_audioService.GetVolume(AudioEnums.AudioType.Master)}");
        GD.Print($"   Music: {_audioService.GetVolume(AudioEnums.AudioType.Music)}");
        GD.Print($"   SFX: {_audioService.GetVolume(AudioEnums.AudioType.SoundEffect)}");
        GD.Print($"   Voice: {_audioService.GetVolume(AudioEnums.AudioType.Voice)}");
        
        // 6. 演示命令模式
        GD.Print("6. Using more commands...");
        await _mediator.Send(new StopAllAudioCommand());
        
        GD.Print("=== Demo Complete ===");
    }
    
    // 演示如何在UI中使用
    public void OnPlayButtonPressed()
    {
        if (_audioService != null)
        {
            // 播放按钮音效
            try
            {
                _audioService.PlaySound("res://Assets/Sounds/SFX/Classic Status Effects/Dispelled.wav", 0.8f);
            }
            catch (Exception ex)
            {
                GD.Print($"Play sound failed: {ex.Message}");
            }
        }
    }
    
    public async void OnPlayMusicButtonPressed()
    {
        if (_mediator != null)
        {
            // 使用命令播放音乐
            try
            {
                await _mediator.Send(new PlayMusicCommand("res://Assets/Music/background.ogg", 2.0f, true));
            }
            catch (Exception ex)
            {
                GD.Print($"Play music failed: {ex.Message}");
            }
        }
    }
    
    public void OnVolumeSliderChanged(float value)
    {
        if (_audioService != null)
        {
            // 调整主音量
            _audioService.SetVolume(AudioEnums.AudioType.Master, value);
        }
    }
    
    public void OnMuteTogglePressed(bool muted)
    {
        if (_audioService != null)
        {
            _audioService.SetMute(muted);
        }
    }
}