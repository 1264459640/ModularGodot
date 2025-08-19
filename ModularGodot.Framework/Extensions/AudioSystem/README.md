# 音频系统扩展 (AudioSystem Extension)

这是一个完整的音频管理扩展，提供背景音乐、音效、语音等音频功能。

## 功能特性

- 🎵 **背景音乐管理**：支持淡入淡出、循环播放
- 🔊 **音效播放**：支持多音效同时播放
- 🎤 **语音播放**：独立的语音通道
- 🔧 **音量控制**：分类型音量控制
- 🔇 **静音功能**：全局静音控制
- 📦 **资源缓存**：音频资源预加载和缓存
- 🎛️ **音频总线**：自动创建和管理音频总线
- 🔌 **扩展点**：支持其他扩展监听音频事件

## 使用方法

### 1. 注册扩展

```csharp
public partial class Main : Node
{
    public override void _Ready()
    {
        // 注册音频系统扩展
        ExtendedContexts.Instance.RegisterExtension<AudioSystemExtension>();
        
        // 获取音频服务
        var audioService = ExtendedContexts.Instance.GetService<IAudioManagerService>();
    }
}
```

### 2. 播放音频

```csharp
// 播放背景音乐
audioService.PlayMusic("res://audio/bgm.ogg", fadeDuration: 2.0f, loop: true);

// 播放音效
audioService.PlaySound("res://audio/click.wav", volume: 0.8f);

// 播放语音
audioService.PlayVoice("res://audio/voice.wav", volume: 1.0f);
```

### 3. 音量控制

```csharp
// 设置主音量
audioService.SetVolume(AudioEnums.AudioType.Master, 0.8f);

// 设置音乐音量
audioService.SetVolume(AudioEnums.AudioType.Music, 0.6f);

// 设置音效音量
audioService.SetVolume(AudioEnums.AudioType.SoundEffect, 0.7f);

// 静音/取消静音
audioService.SetMute(true);
```

### 4. 使用命令模式

```csharp
// 通过MediatR发送命令
var mediator = ExtendedContexts.Instance.GetService<IMediator>();

// 播放音效
await mediator.Send(new PlaySoundCommand("res://audio/click.wav", 0.8f));

// 播放音乐
await mediator.Send(new PlayMusicCommand("res://audio/bgm.ogg", 2.0f, true));

// 设置音量
await mediator.Send(new SetVolumeCommand(AudioEnums.AudioType.Music, 0.6f));
```

### 5. 预加载音频

```csharp
// 预加载音频资源
audioService.PreloadAudio("res://audio/bgm.ogg", AudioEnums.AudioType.Music);
audioService.PreloadAudio("res://audio/click.wav", AudioEnums.AudioType.SoundEffect);
```

### 6. 音乐控制

```csharp
// 停止音乐
audioService.StopMusic();

// 暂停音乐
audioService.PauseMusic();

// 恢复音乐
audioService.ResumeMusic();

// 淡出音乐
audioService.FadeOutMusic(2.0f);
```

## 扩展点

其他扩展可以监听音频系统的事件：

```csharp
public class MyAudioExtension : FrameworkExtensionBase, IAudioSystemExtensionPoint
{
    public override void Initialize(IExtensionContext context)
    {
        // 注册为音频系统扩展点
        RegisterExtensionPoint<IAudioSystemExtensionPoint>(this);
    }
    
    public void OnAudioPlayed(string path, float volume)
    {
        GD.Print($"Audio played: {path} at volume {volume}");
    }
    
    public void OnVolumeChanged(AudioEnums.AudioType type, float volume)
    {
        GD.Print($"Volume changed: {type} to {volume}");
    }
    
    public void OnMuteStateChanged(bool muted)
    {
        GD.Print($"Mute state changed: {muted}");
    }
}
```

## 音频类型

- `Master`: 主音量
- `Music`: 背景音乐
- `SoundEffect`: 音效
- `Voice`: 语音
- `Ambient`: 环境音

## 音频总线

扩展会自动创建以下音频总线：
- Master (主总线)
- Music (音乐总线)
- SFX (音效总线)
- Voice (语音总线)
- Ambient (环境音总线)

所有子总线都连接到Master总线，便于统一控制。

## 注意事项

1. 确保音频文件路径正确
2. 音频文件格式建议使用 .ogg 或 .wav
3. 大文件建议预加载以提高性能
4. 音量值范围为 0.0 - 1.0
5. 循环音乐需要在导入时设置循环点