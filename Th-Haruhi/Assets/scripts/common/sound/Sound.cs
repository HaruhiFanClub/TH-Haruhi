

using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.Collections;

public static class Sound
{
    public class MusicAudioSource
    {
        public AudioSource AudioSource;
        public string Resources;
    }

    private static MusicAudioSource _music;
    private static MusicAudioSource _environment;

    private static float _musicVolume;
    private static float _environmentVolume;
    private static float _audioVolume;

    private static float _globalMusicVolume;
    private static float _globalAudioVolume;

    private static AudioSource _uiAudio;
    private static TableT<SoundDeploy> _soundTableT;
  

    public static void CloseSound()
    {
        GlobalAudioVolume = 0;
        GlobalMusicVolume = 0;
    }

    public static void OpenSound()
    {
        GlobalAudioVolume = GameSetting.AudioVolume;
        GlobalMusicVolume = GameSetting.MusicVolume;
    }
    
    
    public static void MuteAudioExceptUi(bool b)
    {
        _audioVolume = b ? 0f : 1f;
        _environmentVolume = b ? 0f : 1f;
    }

    public static float GlobalMusicVolume
    {
        get { return _globalMusicVolume; }
        set
        {
            _globalMusicVolume = value;
            RefreshMusicVolume();
        }
    }

    private static void RefreshMusicVolume()
    {
        if (_music != null && _music.AudioSource != null)
        {
            _music.AudioSource.volume = _musicVolume * _globalMusicVolume;
        }

        if (_environment != null && _environment.AudioSource != null)
        {
            _environment.AudioSource.volume = _environmentVolume * _globalMusicVolume;
        }
    }

    public static float GlobalAudioVolume
    {
        get { return _globalAudioVolume; }
        set
        {
            _globalAudioVolume = value;
        }
    }

    static Sound()
    {
        _audioVolume = 1f;
        _musicVolume = 1f;
        _environmentVolume = 1f;
        GlobalMusicVolume = 1f;
        GlobalAudioVolume = 1f;
        _soundTableT = TableUtility.GetTable<SoundDeploy>();
    }

    private static void CreateUiAudioSource()
    {
        GameObject gameObject = new GameObject("UI Audio Source");
        Object.DontDestroyOnLoad(gameObject);
        _uiAudio = gameObject.AddComponent<AudioSource>();
        _uiAudio.playOnAwake = false;
        _uiAudio.reverbZoneMix = 0;     //混响比例
        _uiAudio.spatialize = false;      //空间声音
        _uiAudio.spatialBlend = 0;      //3d音效比例
        _uiAudio.spread = 0;           //传播角度,影响左右声道
    }

    private static void Create2DAudioSource(string resource, float volume = 1f, Action<AudioSource> notify = null)
    {
        LoadSoundResource(resource, clipObject =>
        {
            AudioClip audioClip = clipObject as AudioClip;
            if (audioClip)
            {
                GameObject sourceObject = new GameObject("2DAudioSource");
                AudioSource source = sourceObject.AddComponent<AudioSource>();
                source.reverbZoneMix = 0;     //混响比例
                source.spatialize = false;      //空间声音
                source.spatialBlend = 0;      //3d音效比例
                source.spread = 0;           //传播角度,影响左右声道
                source.clip = audioClip;
                source.volume = volume;
                if (notify != null) notify.Invoke(source);
            }
            else if (notify != null)
            {
                Debug.LogError(string.Format("The AudioSource[resource={0}] is null.", resource));
                notify(null);
            }
        });
    }


    public static void StopEnvironmentMusic(float fade = 3f)
    {
        if (_environment != null && _environment.AudioSource != null)
        {
            CachePool.Remove(_environment.Resources);
            Resources.UnloadAsset(_environment.AudioSource.clip);
            Object.Destroy(_environment.AudioSource.gameObject);
        }
    }

    public static void PlayEnvironmentMusic(int id, bool loop = true, float fade = 1f)
    {
        SoundDeploy deploy = _soundTableT.GetSection(id);
        if (!deploy)
            return;
        PlayEnvironmentMusic(deploy.resource, loop, deploy.volume, fade);
    }

    public static void PlayEnvironmentMusic(string resource, bool loop = true, float volume = 1f, float fade = 1f)
    {
        Create2DAudioSource(resource, volume, sourceObject =>
        {
            AudioSource source = sourceObject as AudioSource;
            if (source)
            {
                source.volume = volume * _globalMusicVolume;

                if (_environment == null)
                {
                    _environment = new MusicAudioSource();
                }
                if (_environment.AudioSource != null)
                {
                    Object.Destroy(_environment.AudioSource.gameObject);
                }


                _environment.Resources = resource;
                _environment.AudioSource = source;
                _environmentVolume = volume;
                _environment.AudioSource.loop = loop;
                _environment.AudioSource.name = "EnvironmentAudio";
                _environment.AudioSource.Play();
                GameObjectTools.DontDestroyOnSceneChanged(_environment.AudioSource.gameObject);
            }
            else if (_environment != null && _environment.AudioSource != null)
            {
                Object.Destroy(_environment.AudioSource.gameObject);
                _environment.AudioSource = null;
            }
        });
    }

    public static void PlayMusic(int id, bool loop = true, float fade = 1f, string tag = "")
    {
        SoundDeploy deploy = _soundTableT.GetSection(id);
        if (!deploy)
            return;

        PlayMusic(deploy.resource, tag, loop, deploy.volume, fade);
    }

    public static void PlayMusic(string resource, string name = null, bool loop = true, float volume = 1f, float fade = 1f)
    {
        if (fade <= 0) fade = 0.1f;
        Create2DAudioSource(resource, volume, sourceObject =>
        {
            AudioSource source = sourceObject as AudioSource;
            if (source)
            {
                source.volume = volume * _globalMusicVolume;

                if (_music == null)
                {
                    _music = new MusicAudioSource();
                }
                if (_music.AudioSource != null)
                {
                    Object.Destroy(_music.AudioSource.gameObject);
                }

                _music.Resources = resource;
                _music.AudioSource = source;
                _musicVolume = volume;
                _music.AudioSource.loop = loop;
                _music.AudioSource.name = "BgmAudio";
                _music.AudioSource.Play();
                GameObjectTools.DontDestroyOnSceneChanged(_music.AudioSource.gameObject);
            }
            else if (_music != null && _music.AudioSource != null)
            {
                Object.Destroy(_music.AudioSource.gameObject);
                _music.AudioSource = null;
            }
        });
    }

    public static void FadeOutMusic(float fade = 1f)
    {
        if (_music != null && _music.AudioSource != null)
        {
            CachePool.Remove(_music.Resources);
            Resources.UnloadAsset(_music.AudioSource.clip);
            Object.Destroy(_music.AudioSource.gameObject);
        }
    }

    public static void StopMusic()
    {
        if (_music != null && _music.AudioSource != null)
        {
            CachePool.Remove(_music.Resources);
            Resources.UnloadAsset(_music.AudioSource.clip);
            Object.Destroy(_music.AudioSource.gameObject);
            _music.AudioSource = null;
        }
    }

    public static void PauseMusic()
    {
        if (_music != null && _music.AudioSource != null)
            _music.AudioSource.Pause();
    }

    public static void UnPauseMusic()
    {
        if (_music != null && _music.AudioSource != null)
            _music.AudioSource.UnPause();
    }

    public static void UnloadSoundAsset(AudioSource audioSource, int soundId)
    {
        if (audioSource != null)
        {
            SoundDeploy deploy = _soundTableT.GetSection(soundId);
            CachePool.Remove(deploy.resource);
            Resources.UnloadAsset(audioSource.clip);
        }
    }

    public static void LateUpdate()
    {
        _oneFrameDic.Clear();
    }

    private static Dictionary<string, bool> _oneFrameDic = new Dictionary<string, bool>();
    public static void PlayTHSound(string name, bool oneFrameOnce = false, float volume = 1f)
    {
        Load(name, soundClip=>
        {
            if (oneFrameOnce)
            {
                if (_oneFrameDic.ContainsKey(name))
                {
                    return;
                }
                _oneFrameDic[name] = true;
            }
            PlayThSoundOneShot(soundClip, volume);
        });
    }

    private static void Load(string name, Action<SoundClip> notify)
    {
        var fileNmae = string.Format("audios/se/se_{0}.wav", name);
        Load(fileNmae, 1f, notify);
    }

    private static void Load(string resource, float volume = 1f, Action<SoundClip> notify = null)
    {
        LoadSoundResource(resource, audioClip =>
        {
            SoundClip soundClip = null;
            if (audioClip == null)
            {
                Debug.LogError(string.Format("The AudioClip[resource={0}] is null.", resource));
            }
            else
            {
                soundClip = new SoundClip(audioClip as AudioClip, volume);
            }

            if (notify != null) notify.Invoke(soundClip);
        });
    }

    private static void PlayThSoundOneShot(SoundClip soundClip, float volume)
    {
        if (_globalAudioVolume <= 0)
        {
            return;
        }

        if (_uiAudio == null)
        {
            CreateUiAudioSource();
        }

        if (_uiAudio != null)
        {
            _uiAudio.PlayOneShot(soundClip.clip, soundClip.volume * volume * _globalAudioVolume);
        }
    }
    public static void ClearSoundCache()
    {
        if (!GameSetting.CacheSound)
        {
            var e = CachePool.GetEnumerator();
            using (e)
            {
                while (e.MoveNext())
                {
                    Resources.UnloadAsset(e.Current.Value);
                }
            }
            CachePool.Clear();
        }
    }

    private static readonly Dictionary<string, AudioClip> CachePool = new Dictionary<string, AudioClip>();

    private static void LoadSoundResource(string soundName, Action<AudioClip> notify)
    {
        LoadSoundResourceImpl(soundName, notify);
    }

    public static IEnumerator CacheSound(int soundId)
    {
        var soundName = _soundTableT.GetSection(soundId).resource;
        if(CachePool.ContainsKey(soundName))
        {
            yield break;
        }

        var soundObj = ResourceMgr.LoadImmediately(soundName);
        var resource = soundObj as AudioClip;
        if (!resource)
        {
            Debug.LogError("加载声音失败:" + soundName);
        }
        else
        {
            if (!CachePool.ContainsKey(soundName))
                CachePool.Add(soundName, resource);
        }
        yield return Yielders.Frame;
    }

    private static void LoadSoundResourceImpl(string soundName, Action<AudioClip> notify)
    {
        AudioClip resource;
        if (CachePool.TryGetValue(soundName, out resource))
        {
            notify(resource);
        }
        else
        {
            ResourceMgr.Load(soundName, _object =>
            {
                resource = _object as AudioClip;
                if (!resource)
                {
                    Debug.LogError("加载声音失败:" + soundName);
                }
                else
                {
                    if (!CachePool.ContainsKey(soundName))
                        CachePool.Add(soundName, resource);
                    notify(resource);
                }
            });
        }
    }


    public static IEnumerator CacheAllBgm()
    {
        yield return CacheSound(2);
        yield return CacheSound(3);
        yield return CacheSound(12);
    }
}

public class SoundDeploy : Conditionable
{
    public int id;
    public string name;
    public string resource;
    public float volume;
}
