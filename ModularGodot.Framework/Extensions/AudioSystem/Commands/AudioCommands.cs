using MediatR;
using MF.Commons.Core.Enums.System;
using MF.Commons.Extensions.AudioSystem.System;

namespace MF.Commands.Extensions.AudioSystem;

/// <summary>
/// 播放音效命令
/// </summary>
/// <param name="Path">音频路径</param>
/// <param name="Volume">音量</param>
public record PlaySoundCommand(string Path, float Volume = 1.0f) : IRequest;

/// <summary>
/// 播放背景音乐命令
/// </summary>
/// <param name="Path">音频路径</param>
/// <param name="FadeDuration">淡入淡出时间</param>
/// <param name="Loop">是否循环</param>
public record PlayMusicCommand(string Path, float FadeDuration = 1.0f, bool Loop = true) : IRequest;

/// <summary>
/// 播放语音命令
/// </summary>
/// <param name="Path">音频路径</param>
/// <param name="Volume">音量</param>
public record PlayVoiceCommand(string Path, float Volume = 1.0f) : IRequest;

/// <summary>
/// 设置音量命令
/// </summary>
/// <param name="Type">音频类型</param>
/// <param name="Volume">音量值</param>
public record SetVolumeCommand(AudioEnums.AudioType Type, float Volume) : IRequest;

/// <summary>
/// 设置静音命令
/// </summary>
/// <param name="Mute">是否静音</param>
public record SetMuteCommand(bool Mute) : IRequest;

/// <summary>
/// 停止所有音频命令
/// </summary>
public record StopAllAudioCommand() : IRequest;

/// <summary>
/// 预加载音频命令
/// </summary>
/// <param name="Path">音频路径</param>
/// <param name="Type">音频类型</param>
public record PreloadAudioCommand(string Path, AudioEnums.AudioType Type) : IRequest;

/// <summary>
/// 停止音乐命令
/// </summary>
public record StopMusicCommand() : IRequest;

/// <summary>
/// 暂停音乐命令
/// </summary>
public record PauseMusicCommand() : IRequest;

/// <summary>
/// 恢复音乐命令
/// </summary>
public record ResumeMusicCommand() : IRequest;
