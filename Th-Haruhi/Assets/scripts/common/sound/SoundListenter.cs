

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundListenter : MonoBehaviour 
{
    private static GameObject soundListenter;
    private static AudioReverbZone reverbZone;

    public new static Transform transform
    {
        get
        {
            if (soundListenter)
                return soundListenter.transform;
            else
                return null;
        }
    }

    public static void Init()
    {
        if (!soundListenter)
        {
            soundListenter = 
                GameObjectTools.CreateGameObject("soundListenter", typeof(AudioListener));
            soundListenter.AddComponent<SoundListenter>();
            reverbZone = soundListenter.AddComponent<AudioReverbZone>();
            GameObjectTools.DontDestroyOnSceneChanged(soundListenter);
            EnabledReverbZone(false);
        }

        Sound.GlobalMusicVolume = GameSetting.MusicVolume;
        Sound.GlobalAudioVolume = GameSetting.AudioVolume;
    }

    public static void ChangeReverbZoneType(AudioReverbPreset type)
    {
        EnabledReverbZone(true);
        reverbZone.reverbPreset = type;
    }

    public static void EnabledReverbZone(bool b)
    {
        if (reverbZone != null)
            reverbZone.enabled = b;
    }
    protected void LateUpdate()
    {
        var mainCamera = Camera.main;
        if (mainCamera)
        {
            transform.position = mainCamera.transform.position;
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}