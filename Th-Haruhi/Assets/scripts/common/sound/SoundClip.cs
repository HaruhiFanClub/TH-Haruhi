

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundClip : Conditionable
{
    public AudioClip clip;
    public float volume;

    public SoundClip()
    {
    }

    public SoundClip(AudioClip _clip, float _volume = 1f)
    {
        clip = _clip;
        volume = _volume;
    }
}