


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundFadeIn : MonoBehaviour 
{
    public delegate void FinishNotify(GameObject gameObject);
    public float duration;
    public FinishNotify onFinish;

    protected float timeStart;
    protected float volume;
    protected new AudioSource audio; // = GetComponent<AudioSource>();
    public static SoundFadeIn Add(
        GameObject gameObject, float duration = 1f, FinishNotify finishNotify = null)
    {
        SoundFadeIn fadeIn = gameObject.AddComponent<SoundFadeIn>();
        fadeIn.duration = duration;
        fadeIn.onFinish = finishNotify;
        return fadeIn;
    }

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        if (audio)
            volume = audio.volume;
    }

    protected void Start()
    {
        timeStart = Time.realtimeSinceStartup;
        audio = GetComponent<AudioSource>();
        if (audio)
            audio.volume = 0f;
    }

    protected void FixedUpdate()
    {
        if (audio)
        {
            float d = Time.realtimeSinceStartup - timeStart;
            if (d <= duration)
                audio.volume = Mathf.Lerp(0f, volume, d / duration);
            else
            {
                audio.volume = volume;
                if (onFinish != null)
                    onFinish(gameObject);
                Object.Destroy(this);
            }
        }
    }
}

public class SoundFadeOut : MonoBehaviour
{
    public delegate void FinishNotify(GameObject gameObject);
    public float duration;
    public FinishNotify onFinish;

    protected float timeStart;
    protected float volume;
    public new AudioSource audio;
    public static SoundFadeOut Add(
        GameObject gameObject, float duration = 1f, FinishNotify finishNotify = null)
    {
        SoundFadeOut fadeOut = gameObject.AddComponent<SoundFadeOut>();
        fadeOut.duration = duration;
        fadeOut.onFinish = finishNotify;
        return fadeOut;
    }

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        if (audio)
            volume = audio.volume;
    }

    protected void Start()
    {
        timeStart = Time.realtimeSinceStartup;
    }

    protected void FixedUpdate()
    {
        if (audio)
        {
            float d = Time.realtimeSinceStartup - timeStart;
            if (d <= duration)
                audio.volume = Mathf.Lerp(volume, 0f, d / duration);
            else
            {
                audio.volume = 0f;
                audio.Stop();
                if (onFinish != null)
                    onFinish(gameObject);
                Object.Destroy(this);
            }
        }
    }
}
