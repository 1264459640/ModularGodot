using MF.Commons.Core.Enums.System;

namespace MF.Commons.Core.Configs;

public static class AudioConfigs
{
    
    public static Dictionary<AudioEnums.Audio, string> AudioPath = new()
    {
        {  AudioEnums.Audio.Button, "res://Assets/Sounds/SFX/Classic Status Effects/Dispelled.wav"}
    };
}