using Godot;
using MF.Commons.Core.Enums.System;
using MF.Commons.Extensions.AudioSystem.System;

namespace MF.Services.Abstractions.Extensions.AudioSystem;

public interface IAudioManagerService
{
    void PreloadAudio(string path, AudioEnums.AudioType type);
    void PlayMusic(string path, float fadeDuration = 1.0f, bool loop = true);
    AudioStreamPlayer PlaySound(string path, float volume = 1.0f);
    AudioStreamPlayer PlayVoice(string path, float volume = 1.0f);
    void SetVolume(AudioEnums.AudioType type, float volume);
    void SetMute(bool mute);
    void StopAll();
    void SetupAudioBuses();
    void StopMusic();
    void PauseMusic();
    void ResumeMusic();
}
