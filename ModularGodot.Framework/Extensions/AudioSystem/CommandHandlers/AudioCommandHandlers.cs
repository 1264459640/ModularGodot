using MediatR;
using Extensions.AudioSystem.Commands;
using Extensions.AudioSystem.Services.Abstractions;

namespace Extensions.AudioSystem.CommandHandlers;

/// <summary>
/// 播放音效命令处理器
/// </summary>
public class PlaySoundCommandHandler : IRequestHandler<PlaySoundCommand>
{
    private readonly IAudioManagerService _audioManagerService;
    
    public PlaySoundCommandHandler(IAudioManagerService audioManagerService)
    {
        _audioManagerService = audioManagerService;
    }
    
    public async Task Handle(PlaySoundCommand request, CancellationToken cancellationToken)
    {
        _audioManagerService.PlaySound(request.Path, request.Volume);
        await Task.CompletedTask;
    }
}

/// <summary>
/// 播放背景音乐命令处理器
/// </summary>
public class PlayMusicCommandHandler : IRequestHandler<PlayMusicCommand>
{
    private readonly IAudioManagerService _audioManagerService;
    
    public PlayMusicCommandHandler(IAudioManagerService audioManagerService)
    {
        _audioManagerService = audioManagerService;
    }
    
    public async Task Handle(PlayMusicCommand request, CancellationToken cancellationToken)
    {
        _audioManagerService.PlayMusic(request.Path, request.FadeDuration, request.Loop);
        await Task.CompletedTask;
    }
}

/// <summary>
/// 播放语音命令处理器
/// </summary>
public class PlayVoiceCommandHandler : IRequestHandler<PlayVoiceCommand>
{
    private readonly IAudioManagerService _audioManagerService;
    
    public PlayVoiceCommandHandler(IAudioManagerService audioManagerService)
    {
        _audioManagerService = audioManagerService;
    }
    
    public async Task Handle(PlayVoiceCommand request, CancellationToken cancellationToken)
    {
        _audioManagerService.PlayVoice(request.Path, request.Volume);
        await Task.CompletedTask;
    }
}

/// <summary>
/// 设置音量命令处理器
/// </summary>
public class SetVolumeCommandHandler : IRequestHandler<SetVolumeCommand>
{
    private readonly IAudioManagerService _audioManagerService;
    
    public SetVolumeCommandHandler(IAudioManagerService audioManagerService)
    {
        _audioManagerService = audioManagerService;
    }
    
    public async Task Handle(SetVolumeCommand request, CancellationToken cancellationToken)
    {
        _audioManagerService.SetVolume(request.Type, request.Volume);
        await Task.CompletedTask;
    }
}

/// <summary>
/// 设置静音命令处理器
/// </summary>
public class SetMuteCommandHandler : IRequestHandler<SetMuteCommand>
{
    private readonly IAudioManagerService _audioManagerService;
    
    public SetMuteCommandHandler(IAudioManagerService audioManagerService)
    {
        _audioManagerService = audioManagerService;
    }
    
    public async Task Handle(SetMuteCommand request, CancellationToken cancellationToken)
    {
        _audioManagerService.SetMute(request.Mute);
        await Task.CompletedTask;
    }
}

/// <summary>
/// 停止所有音频命令处理器
/// </summary>
public class StopAllAudioCommandHandler : IRequestHandler<StopAllAudioCommand>
{
    private readonly IAudioManagerService _audioManagerService;
    
    public StopAllAudioCommandHandler(IAudioManagerService audioManagerService)
    {
        _audioManagerService = audioManagerService;
    }
    
    public async Task Handle(StopAllAudioCommand request, CancellationToken cancellationToken)
    {
        _audioManagerService.StopAll();
        await Task.CompletedTask;
    }
}

/// <summary>
/// 预加载音频命令处理器
/// </summary>
public class PreloadAudioCommandHandler : IRequestHandler<PreloadAudioCommand>
{
    private readonly IAudioManagerService _audioManagerService;
    
    public PreloadAudioCommandHandler(IAudioManagerService audioManagerService)
    {
        _audioManagerService = audioManagerService;
    }
    
    public async Task Handle(PreloadAudioCommand request, CancellationToken cancellationToken)
    {
        _audioManagerService.PreloadAudio(request.Path, request.Type);
        await Task.CompletedTask;
    }
}

/// <summary>
/// 停止音乐命令处理器
/// </summary>
public class StopMusicCommandHandler : IRequestHandler<StopMusicCommand>
{
    private readonly IAudioManagerService _audioManagerService;
    
    public StopMusicCommandHandler(IAudioManagerService audioManagerService)
    {
        _audioManagerService = audioManagerService;
    }
    
    public async Task Handle(StopMusicCommand request, CancellationToken cancellationToken)
    {
        _audioManagerService.StopMusic();
        await Task.CompletedTask;
    }
}

/// <summary>
/// 暂停音乐命令处理器
/// </summary>
public class PauseMusicCommandHandler : IRequestHandler<PauseMusicCommand>
{
    private readonly IAudioManagerService _audioManagerService;
    
    public PauseMusicCommandHandler(IAudioManagerService audioManagerService)
    {
        _audioManagerService = audioManagerService;
    }
    
    public async Task Handle(PauseMusicCommand request, CancellationToken cancellationToken)
    {
        _audioManagerService.PauseMusic();
        await Task.CompletedTask;
    }
}

/// <summary>
/// 恢复音乐命令处理器
/// </summary>
public class ResumeMusicCommandHandler : IRequestHandler<ResumeMusicCommand>
{
    private readonly IAudioManagerService _audioManagerService;
    
    public ResumeMusicCommandHandler(IAudioManagerService audioManagerService)
    {
        _audioManagerService = audioManagerService;
    }
    
    public async Task Handle(ResumeMusicCommand request, CancellationToken cancellationToken)
    {
        _audioManagerService.ResumeMusic();
        await Task.CompletedTask;
    }
}