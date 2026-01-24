using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEvent : EventBase
{
    public AudioLabel AudioLabelToPlay;
    public bool IsMusic;
    public bool FadeInMusic;
    public bool QueueMusic;
    public bool DestroySFX;

    public override void Activate(bool activateNextEvent)
    {
        CLogger.Log("SoundEvent activated!");
        if (IsMusic)
        {
            AudioManager.Instance?.PlayMusicClip(AudioLabelToPlay, FadeInMusic, QueueMusic);
        }
        else
        {
            if (DestroySFX)
            {
                AudioManager.Instance?.DestroySFXClips(AudioLabelToPlay);
            }
            else
            {
                AudioManager.Instance?.PlaySFXClip(AudioLabelToPlay);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        string customName = "SoundEventGizmo.png";
        Gizmos.DrawIcon(transform.position, customName, true);
    }
#endif
}
