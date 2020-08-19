


using UnityEngine;

public class PlayBgm : MonoBehaviour
{
    public int SoundId;
    private void Awake()
    {
        Sound.PlayMusic(SoundId);
    }
}