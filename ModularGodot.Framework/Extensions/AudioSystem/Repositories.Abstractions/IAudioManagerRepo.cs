using Godot;
using MF.Commons.Core.Enums.System;
using MF.Commons.Extensions.AudioSystem.System;
using MF.Nodes.Abstractions.Extensions.AudioSystem;
using MF.Repositories.Abstractions.Bases;

namespace MF.Repositories.Abstractions.Extensions.AudioSystem;

/// <summary>
/// 音频管理仓储接口
/// </summary>
public interface IAudioManagerRepo : ISingletonNodeRepo<IAudioManager>
{
    /// <summary>
    /// 音频节点根
    /// </summary>
    Node? AudioNodeRoot { get; set; }

    /// <summary>
    /// 音量变化事件
    /// </summary>
    Action<AudioEnums.AudioType, double>? OnVolumeChanged { get; set; }

    /// <summary>
    /// 静音状态变化事件
    /// </summary>
    Action<bool>? OnMuteStateChanged { get; set; }

    /// <summary>
    /// 静音状态
    /// </summary>
    bool MuteState { get; set; }

    /// <summary>
    /// 默认音频通道配置
    /// </summary>
    Dictionary<AudioEnums.AudioType, string> DefaultBuses { get; set; }

    /// <summary>
    /// 音频播放器池
    /// </summary>
    Dictionary<AudioEnums.AudioType, List<AudioStreamPlayer?>> AudioPlayers { get; }

    /// <summary>
    /// 当前播放的音乐
    /// </summary>
    AudioStreamPlayer? CurrentMusic { get; set; }

    /// <summary>
    /// 音频资源缓存
    /// </summary>
    Dictionary<string, (AudioStream Resource, AudioEnums.AudioType Type)> AudioCache { get; }

    /// <summary>
    /// 音量设置
    /// </summary>
    Dictionary<AudioEnums.AudioType, float> Volumes { get; }
}
