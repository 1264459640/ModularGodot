using MF.Commons.Extensions.AudioSystem.System;

namespace MF.Commons.Extensions.AudioSystem.Configs;

public static class AudioConfigs
{

    public static Dictionary<AudioEnums.Audio, string> AudioPath = new()
    {
        {  AudioEnums.Audio.Button, "res://Assets/Sounds/SFX/Classic Status Effects/Dispelled.wav"}
    };
}
