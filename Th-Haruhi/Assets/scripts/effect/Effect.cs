
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Effect : MonoBehaviour
{
    private bool _destroyOnNotalive;
    private Action<Effect> _playFinishNotify;
    public bool InCache;
    public string CacheName;
    private Vector3 _originScale;

    private ParticleSystem[] _particleSystems;

    public ParticleSystem[] ParticleSystemList
    {
        get
        {
            if (_particleSystems == null && gameObject != null)
                _particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
            return _particleSystems;
        }
    }
    public void Init()
    {
        _destroyOnNotalive = false;
        _originScale = transform.localScale;
    }

    
    private bool _hasFinishedNotify;
    public virtual void Play(Action<Effect> finishNotify = null, bool playSound = true)
    {
        _playFinishNotify = finishNotify;
        _hasFinishedNotify = finishNotify != null;
        for (int i = 0; i < ParticleSystemList.Length; i++)
        {
            var pSystem = ParticleSystemList[i];
            if (pSystem)
            {
                pSystem.Play(false);
            }
        }
    }


    private bool IsAlive()
    {
        for (int i = 0; i < ParticleSystemList.Length; i++)
        {
            if (!ParticleSystemList[i].isStopped)
                return true;
        }
        return false;
    }

    public void AutoDestory(bool enable = true)
    {
        _destroyOnNotalive = enable;
    }

    private float _lastCheckTime;
    protected virtual void Update()
    {
        if (!_destroyOnNotalive && !_hasFinishedNotify) return;

        if (Time.time - _lastCheckTime < 1f)
        {
            return;
        }
        _lastCheckTime = Time.time;

        if (!IsAlive())
        {
            if (_hasFinishedNotify)
            {
                _playFinishNotify(this);
                _playFinishNotify = null;
                _hasFinishedNotify = false;
            }
            if (_destroyOnNotalive)
            {
                EffectFactory.DestroyEffect(this);
            }
        }
    }


    public void OnPreDestroy()
    {
        transform.localScale = _originScale;
    }

    void OnDestroy()
    {
        _particleSystems = null;
        _playFinishNotify = null;
    }
}