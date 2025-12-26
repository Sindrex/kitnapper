using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEvent : EventBase
{
    public AudioLabel AudioLabelToPlay;
    public bool IsMusic;
    public override void Activate(bool activateNextEvent)
    {
        CLogger.Log("SoundEvent activated!");
        if (IsMusic)
        {
            AudioManager.Instance.PlayMusicClip(AudioLabelToPlay);
        }
        else
        {
            AudioManager.Instance.PlaySFXClip(AudioLabelToPlay);
        }
    }
}
