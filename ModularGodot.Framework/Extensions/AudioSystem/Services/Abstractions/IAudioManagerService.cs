using Godot;
using TO.Commons.Enums.System;

namespace Extensions.AudioSystem.Services.Abstractions;

/// <summary>
/// 音频管理服务接口
/// </summary>
public interface IAudioManagerService
{
    /// <summary>
    /// 预加载音频资源
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <param name="type">音频类型</param>
    void PreloadAudio(string path, AudioEnums.AudioType type);
    
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <param name="fadeDuration">淡入淡出时间</param>
    /// <param name="loop">是否循环</param>
    void PlayMusic(string path, float fadeDuration = 1.0f, bool loop = true);
    
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <param name="volume">音量</param>
    /// <returns>音频播放器实例</returns>
    AudioStreamPlayer PlaySound(string path, float volume = 1.0f);
    
    /// <summary>
    /// 播放语音
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <param name="volume">音量</param>
    /// <returns>音频播放器实例</returns>
    AudioStreamPlayer PlayVoice(string path, float volume = 1.0f);
    
    /// <summary>
    /// 设置音量
    /// </summary>
    /// <param name="type">音频类型</param>
    /// <param name="volume">音量值</param>
    void SetVolume(AudioEnums.AudioType type, float volume);
    
    /// <summary>
    /// 获取音量
    /// </summary>
    /// <param name="type">音频类型</param>
    /// <returns>音量值</returns>
    float GetVolume(AudioEnums.AudioType type);
    
    /// <summary>
    /// 设置全局静音状态
    /// </summary>
    /// <param name="mute">是否静音</param>
    void SetMute(bool mute);
    
    /// <summary>
    /// 获取当前静音状态
    /// </summary>
    /// <returns>是否静音</returns>
    bool GetMuteState();
    
    /// <summary>
    /// 停止所有音频播放
    /// </summary>
    void StopAll();
    
    /// <summary>
    /// 初始化音频总线
    /// </summary>
    void SetupAudioBuses();
    
    /// <summary>
    /// 淡入音乐
    /// </summary>
    /// <param name="duration">淡入时间</param>
    void FadeInMusic(float duration);
    
    /// <summary>
    /// 淡出音乐
    /// </summary>
    /// <param name="duration">淡出时间</param>
    void FadeOutMusic(float duration);
    
    /// <summary>
    /// 停止音乐
    /// </summary>
    void StopMusic();
    
    /// <summary>
    /// 暂停音乐
    /// </summary>
    void PauseMusic();
    
    /// <summary>
    /// 恢复音乐
    /// </summary>
    void ResumeMusic();
}