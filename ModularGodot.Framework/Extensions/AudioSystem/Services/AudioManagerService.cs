using Godot;
using GodotTask;
using MF.Commons.Core.Enums.System;
using MF.Commons.Extensions.AudioSystem.System;
using MF.Data.Serialization;
using MF.Repositories.Abstractions.Core.LogSystem;
using MF.Repositories.Abstractions.Core.ResourceSystem;
using MF.Repositories.Abstractions.Extensions.AudioSystem;
using MF.Services.Abstractions.Extensions.AudioSystem;
using MF.Services.Bases;

namespace MF.Services.Extensions.AudioSystem;

/// <summary>
/// 音频管理服务实现
/// </summary>
public class AudioManagerService : BaseService, IAudioManagerService
{
    private readonly IAudioManagerRepo _audioManagerRepo;
    private readonly ILoggerRepo _logger;
    private readonly IResourceLoaderRepo _resourceLoader;

    public AudioManagerService(
        IAudioManagerRepo audioManagerRepo,
        ILoggerRepo logger,
        IResourceLoaderRepo resourceLoader)
    {
        _audioManagerRepo = audioManagerRepo;
        _logger = logger;
        _resourceLoader = resourceLoader;
    }

    /// <summary>
    /// 预加载音频资源
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <param name="type">音频类型</param>
    public void PreloadAudio(string path, AudioEnums.AudioType type)
    {
        if (_audioManagerRepo.AudioCache.ContainsKey(path)) return;

        if (_resourceLoader.LoadResource(path) is AudioStream audioResource)
        {
            _audioManagerRepo.AudioCache[path] = (audioResource, type);

        }
        else
        {
            throw new Exception($"Failed to load audio resource: {path}");
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <param name="fadeDuration">淡入淡出时间</param>
    /// <param name="loop">是否循环</param>
    public void PlayMusic(string path, float fadeDuration = 1.0f, bool loop = true)
    {
        if (_audioManagerRepo.CurrentMusic is { Playing: true })
        {
            FadeOutMusic(fadeDuration);
        }

        if (_audioManagerRepo.CurrentMusic != null && _audioManagerRepo.CurrentMusic.IsConnected("finished",
                Callable.From(OnMusicFinished)))
        {
            _audioManagerRepo.CurrentMusic.Disconnect("finished",
                Callable.From(OnMusicFinished));
        }

        var audioResource = GetAudioResource(path);
        if (!loop && IsAudioLoop(audioResource))
        {
            _logger.Fatal("Audio is imported with loop enabled, and it will always loop regardless of loop parameter!");
        }

        _audioManagerRepo.CurrentMusic = GetAudioPlayer(AudioEnums.AudioType.Music);
        if (_audioManagerRepo.CurrentMusic != null)
        {
            _audioManagerRepo.CurrentMusic.Stream = audioResource;
            _audioManagerRepo.CurrentMusic.Bus = _audioManagerRepo.DefaultBuses[AudioEnums.AudioType.Music];
            _audioManagerRepo.CurrentMusic.VolumeDb =
                Mathf.LinearToDb(_audioManagerRepo.Volumes[AudioEnums.AudioType.Music]);
            _audioManagerRepo.CurrentMusic.Play();

            if (loop)
            {
                _audioManagerRepo.CurrentMusic.Connect("finished",
                    Callable.From(OnMusicFinished));
            }

        }
        else
        {
            throw new Exception($"Failed to load audio resource: {path}");
        }

        if (fadeDuration > 0)
        {
            FadeInMusic(fadeDuration);
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <param name="volume">音量</param>
    /// <returns>音频播放器实例</returns>
    public AudioStreamPlayer PlaySound(string path, float volume = 1.0f)
    {
        var audioResource = GetAudioResource(path);
        var player = GetAudioPlayer(AudioEnums.AudioType.SoundEffect);
        player.Stream = audioResource;
        player.Bus = _audioManagerRepo.DefaultBuses[AudioEnums.AudioType.SoundEffect];
        player.VolumeDb = Mathf.LinearToDb(_audioManagerRepo.Volumes[AudioEnums.AudioType.SoundEffect] * volume);
        player.Play();

        return player;
    }

    /// <summary>
    /// 播放语音
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <param name="volume">音量</param>
    /// <returns>音频播放器实例</returns>
    public AudioStreamPlayer PlayVoice(string path, float volume = 1.0f)
    {
        var audioResource = GetAudioResource(path);
        var player = GetAudioPlayer(AudioEnums.AudioType.Voice);
        player.Stream = audioResource;
        player.Bus = _audioManagerRepo.DefaultBuses[AudioEnums.AudioType.Voice];
        player.VolumeDb = Mathf.LinearToDb(_audioManagerRepo.Volumes[AudioEnums.AudioType.Voice] * volume);
        player.Play();


        return player;
    }

    /// <summary>
    /// 设置音量
    /// </summary>
    /// <param name="type">音频类型</param>
    /// <param name="volume">音量值</param>
    public void SetVolume(AudioEnums.AudioType type, float volume)
    {
        _audioManagerRepo.Volumes[type] = Mathf.Clamp(volume, 0.0f, 1.0f);
        _audioManagerRepo.OnVolumeChanged?.Invoke(type, volume);

        var busIndex = AudioServer.GetBusIndex(_audioManagerRepo.DefaultBuses[type]);
        if (busIndex != -1)
        {
            AudioServer.SetBusVolumeDb(busIndex, Mathf.LinearToDb(volume));
        }

    }

    /// <summary>
    /// 获取音量
    /// </summary>
    /// <param name="type">音频类型</param>
    /// <returns>音量值</returns>
    public float GetVolume(AudioEnums.AudioType type)
    {
        return _audioManagerRepo.Volumes.TryGetValue(type, out var volume) ? volume : 1.0f;
    }

    /// <summary>
    /// 设置全局静音状态
    /// </summary>
    /// <param name="mute">是否静音</param>
    public void SetMute(bool mute)
    {
        _audioManagerRepo.MuteState = mute;
        _audioManagerRepo.OnMuteStateChanged?.Invoke(mute);
        AudioServer.SetBusMute(AudioServer.GetBusIndex(_audioManagerRepo.DefaultBuses[AudioEnums.AudioType.Master]),
            mute);


    }

    /// <summary>
    /// 获取当前静音状态
    /// </summary>
    /// <returns>是否静音</returns>
    public bool GetMuteState() => _audioManagerRepo.MuteState;

    /// <summary>
    /// 停止所有音频播放
    /// </summary>
    public void StopAll()
    {
        _audioManagerRepo.CurrentMusic?.Stop();

        foreach (var player in _audioManagerRepo.AudioPlayers.Values.SelectMany(players => players))
        {
            player?.Stop();
        }
    }

    /// <summary>
    /// 初始化音频总线
    /// </summary>
    public void SetupAudioBuses()
    {
        // 创建主音频Bus（如果不存在）
        if (AudioServer.GetBusIndex("Master") == -1)
        {
            AudioServer.AddBus();
            var masterBusIdx = AudioServer.GetBusCount() - 1;
            AudioServer.SetBusName(masterBusIdx, "Master");
            AudioServer.SetBusVolumeDb(masterBusIdx, Mathf.LinearToDb(1.0f));
        }

        // 创建其他音频Bus
        foreach (var busName in _audioManagerRepo.DefaultBuses.Select(pair => pair.Value)
                     .Where(busName => AudioServer.GetBusIndex(busName) == -1))
        {
            AudioServer.AddBus();
            var busIdx = AudioServer.GetBusCount() - 1;
            AudioServer.SetBusName(busIdx, busName);
            AudioServer.SetBusVolumeDb(busIdx, Mathf.LinearToDb(1.0f));

            // 将Bus设置为Master的子Bus
            AudioServer.SetBusSend(busIdx, "Master");
        }
    }

    /// <summary>
    /// 淡入音乐
    /// </summary>
    /// <param name="duration">淡入时间</param>
    public void FadeInMusic(float duration)
    {
        if (_audioManagerRepo.CurrentMusic == null) return;

        var tween = _audioManagerRepo.CurrentMusic.CreateTween();
        _audioManagerRepo.CurrentMusic.VolumeDb = Mathf.LinearToDb(0.01f);
        tween.TweenProperty(_audioManagerRepo.CurrentMusic, "volume_db",
            Mathf.LinearToDb(_audioManagerRepo.Volumes[AudioEnums.AudioType.Music]), duration);
    }

    /// <summary>
    /// 淡出音乐
    /// </summary>
    /// <param name="duration">淡出时间</param>
    public void FadeOutMusic(float duration)
    {
        if (_audioManagerRepo.CurrentMusic == null) return;

        var tween = _audioManagerRepo.CurrentMusic.CreateTween();
        tween.TweenProperty(_audioManagerRepo.CurrentMusic, "volume_db", Mathf.LinearToDb(0.01f), duration);
        tween.TweenCallback(Callable.From(() => _audioManagerRepo.CurrentMusic?.Stop()));
    }

    /// <summary>
    /// 停止音乐
    /// </summary>
    public void StopMusic()
    {
        _audioManagerRepo.CurrentMusic?.Stop();
    }

    /// <summary>
    /// 暂停音乐
    /// </summary>
    public void PauseMusic()
    {
        if (_audioManagerRepo.CurrentMusic != null)
        {
            _audioManagerRepo.CurrentMusic.StreamPaused = true;
        }
    }

    /// <summary>
    /// 恢复音乐
    /// </summary>
    public void ResumeMusic()
    {
        if (_audioManagerRepo.CurrentMusic != null)
        {
            _audioManagerRepo.CurrentMusic.StreamPaused = false;
        }
    }

    /// <summary>
    /// 获取音频资源
    /// </summary>
    /// <param name="path">音频路径</param>
    /// <returns>音频流资源</returns>
    private AudioStream GetAudioResource(string path)
    {
        if (_audioManagerRepo.AudioCache.TryGetValue(path, out var cached))
        {
            return cached.Resource;
        }

        if (_resourceLoader.LoadResource(path) is not AudioStream audioResource)
            throw new Exception($"Failed to load audio resource: {path}");

        _audioManagerRepo.AudioCache[path] = (audioResource, AudioEnums.AudioType.SoundEffect);
        return audioResource;
    }

    /// <summary>
    /// 获取可用的音频播放器
    /// </summary>
    /// <param name="type">音频类型</param>
    /// <returns>音频播放器</returns>
    private AudioStreamPlayer GetAudioPlayer(AudioEnums.AudioType type)
    {
        foreach (var player in _audioManagerRepo.AudioPlayers[type].OfType<AudioStreamPlayer>()
                     .Where(player => !player.Playing))
        {
            return player;
        }

        var newPlayer = new AudioStreamPlayer();
        _audioManagerRepo.AudioNodeRoot?.AddChild(newPlayer);
        _audioManagerRepo.AudioPlayers[type].Add(newPlayer);
        return newPlayer;
    }

    /// <summary>
    /// 音乐播放完毕回调
    /// </summary>
    private void OnMusicFinished()
    {
        _audioManagerRepo.CurrentMusic?.Play();
    }

    /// <summary>
    /// 判断音频是否支持循环播放
    /// </summary>
    /// <param name="audio">音频资源</param>
    /// <returns>是否循环</returns>
    private bool IsAudioLoop(AudioStream? audio)
    {
        return audio switch
        {
            null => false,
            AudioStreamWav wav => wav.LoopMode != AudioStreamWav.LoopModeEnum.Disabled,
            AudioStreamMP3 or AudioStreamOggVorbis or AudioStreamPlaylist => audio._HasLoop(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// 保存音频设置
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <returns>音频设置</returns>
    public async GDTask<T?> SaveAsync<T>() where T : class
    {
        var audioSettings = new UserSettings.AudioSettings
        {
            MasterVolume = _audioManagerRepo.Volumes[AudioEnums.AudioType.Master],
            MusicVolume = _audioManagerRepo.Volumes[AudioEnums.AudioType.Music],
            SfxVolume = _audioManagerRepo.Volumes[AudioEnums.AudioType.SoundEffect],
            VoiceVolume = _audioManagerRepo.Volumes[AudioEnums.AudioType.Voice],
            AmbienceVolume = _audioManagerRepo.Volumes[AudioEnums.AudioType.Ambient],
            Mute = _audioManagerRepo.MuteState
        };

        var result = await GDTask.FromResult(audioSettings);
        return result as T;
    }
}
