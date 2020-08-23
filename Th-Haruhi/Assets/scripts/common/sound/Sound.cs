

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
    private static float _uiVolume;
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
        _uiVolume = 1f;
        GlobalMusicVolume = 1f;
        GlobalAudioVolume = 1f;
        _soundTableT = TableUtility.GetTable<SoundDeploy>();
    }

    public static AudioSource Create3DAudioSource(GameObject parent, float maxDistance = 30, bool useLinear = false)
    {
        var audioSource = parent.AddComponent<AudioSource>();
        audioSource.rolloffMode = AudioRolloffMode.Linear;//useLinear ? AudioRolloffMode.Linear : AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 8;
        audioSource.maxDistance = maxDistance;
        audioSource.reverbZoneMix = 1f;     //混响比例
        audioSource.spatialize = true;      //空间声音
        audioSource.spatialBlend = 1f;      //3d音效比例
        audioSource.spread = 300;           //传播角度,影响左右声道
        audioSource.reverbZoneMix = 1f;
        return audioSource;
    }

    public static void Load(int id, Action<SoundClip> notify)
    {
        SoundDeploy deploy = _soundTableT.GetSection(id);
        if (!deploy || string.IsNullOrEmpty(deploy.resource)) return;
        Load(deploy.resource, deploy.volume, notify);
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

    public static void UnloadSoundAsset(AudioSource audioSource, int soundId)
    {
        if (audioSource != null)
        {
            SoundDeploy deploy = _soundTableT.GetSection(soundId);
            CachePool.Remove(deploy.resource);
            Resources.UnloadAsset(audioSource.clip);
        }
    }
    #region PlayAudioOneShot

    public static void PlayAudioOneShot(AudioSource audioSource, int soundId, float volume = 1f)
    {
        if (_globalAudioVolume <= 0 || _audioVolume <= 0)
        {
            return;
        }

        SoundDeploy deploy = _soundTableT.GetSection(soundId);
        if (!deploy || string.IsNullOrEmpty(deploy.resource)) return;

        AudioClip resource;
        if (CachePool.TryGetValue(deploy.resource, out resource))
        {
            PlayAudioOneShot(audioSource, resource, volume, deploy.volume);
        }
        else
        {
            RLoadPlayAudioOneShot(deploy.resource, audioSource, volume, deploy.volume);
        }
    }

    private static void RLoadPlayAudioOneShot(string soundName, AudioSource audioSource, float volume = 1f, float gvolume = 1f)
    {
        AudioClip resource;
        ResourceMgr.Load(soundName, _object =>
        {
            resource = _object as AudioClip;
            if (resource != null)
            {
                if (!CachePool.ContainsKey(soundName))
                    CachePool.Add(soundName, resource);

                if (audioSource != null)
                    PlayAudioOneShot(audioSource, resource, volume, gvolume);
            }
            else
            {
                Debug.LogError(string.Format("The AudioClip[resource={0}] is null.", resource));
            }
        });
    }

    private static void PlayAudioOneShot(AudioSource audioSource, AudioClip clip, float volume = 1f, float gvolume = 1f)
    {
        if (audioSource == null) return;
        if (!audioSource.gameObject.activeInHierarchy) return;
        audioSource.PlayOneShot(clip, volume * _globalAudioVolume * gvolume * _audioVolume);
    }
    #endregion

    #region PlayAudio

    public static void PlayAudio(AudioSource audioSource, int soundId, bool loop = true, Action playCallBack = null)
    {
        if (audioSource == null) return;
        LoadPlayAudio(soundId, audioSource, loop, playCallBack);
    }
    private static void LoadPlayAudio(int id, AudioSource audioSource, bool loop, Action playCallBack)
    {
        if (!loop && (_globalAudioVolume <= 0 || _audioVolume <= 0))
            return;

        SoundDeploy deploy = _soundTableT.GetSection(id);
        if (!deploy || string.IsNullOrEmpty(deploy.resource)) return;
        LoadPlayAudio(deploy.resource, audioSource, deploy.volume, loop, playCallBack);
    }

    private static void LoadPlayAudio(string soundName, AudioSource audioSource, float volume, bool loop, Action playCallBack)
    {
        AudioClip resource;
        if (CachePool.TryGetValue(soundName, out resource))
        {
            DoPlayAudio(audioSource, resource, volume, loop, playCallBack);
        }
        else
        {
            RLoadPlayAudio(soundName, audioSource, volume, loop, playCallBack);
        }
    }

    private static void RLoadPlayAudio(string soundName, AudioSource audioSource, float volume, bool loop, Action playCallBack)
    {
        AudioClip resource;
        ResourceMgr.Load(soundName, _object =>
        {
            resource = _object as AudioClip;
            if (resource)
            {
                if (!CachePool.ContainsKey(soundName))
                    CachePool.Add(soundName, resource);

                DoPlayAudio(audioSource, resource, volume, loop, playCallBack);
            }
            else
            {
                Debug.LogError("加载声音失败:" + soundName);
            }
        });
    }

    private static void DoPlayAudio(AudioSource audioSource, AudioClip clip, float volume, bool loop, Action playCallBack)
    {
        if (audioSource == null || clip == null) return;
        if (!audioSource.gameObject.activeInHierarchy) return;

        audioSource.playOnAwake = false;
        audioSource.loop = loop;
        audioSource.clip = clip;

        audioSource.volume = volume * _globalAudioVolume * _audioVolume;
        audioSource.Play();
        if (playCallBack != null)
            playCallBack.Invoke();
    }

    public static void StopAudio(AudioSource audioSource)
    {
        if (audioSource)
            audioSource.Stop();
    }

    #endregion

    public static void LateUpdate()
    {
        _oneFrameDic.Clear();
    }

    private static Dictionary<int, bool> _oneFrameDic = new Dictionary<int, bool>();
    public static void PlayUiAudioOneShot(int soundId, bool oneFrameOnce = false)
    {
        Load(soundId, soundClip=>
        {
            if (oneFrameOnce)
            {
                if (_oneFrameDic.ContainsKey(soundId))
                {
                    return;
                }
                _oneFrameDic[soundId] = true;
            }
            PlayUiAudioOneShot(soundClip);
        });
    }

    private static void PlayUiAudioOneShot(SoundClip soundClip)
    {
        if (_globalAudioVolume <= 0 || _uiVolume <= 0)
        {

            return;
        }

        if (_uiAudio == null)
        {
            CreateUiAudioSource();
        }

        if (_uiAudio != null)
        {
            _uiAudio.PlayOneShot(soundClip.clip, soundClip.volume * _uiVolume * _globalAudioVolume);
        }
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

        ResourceMgr.Load(soundName, _object =>
        {
            var resource = _object as AudioClip;
            if (!resource)
            {
                Debug.LogError("加载声音失败:" + soundName);
            }
            else
            {
                if (!CachePool.ContainsKey(soundName))
                    CachePool.Add(soundName, resource);
            }
        });
        yield return Yielders.Frame;
        yield return Yielders.Frame;
        yield return Yielders.Frame;
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

}

public class SoundDeploy : Conditionable
{
    public int id;
    public string resource;
    public float volume;
}
