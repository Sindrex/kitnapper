using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeboxEvent : EventBase
{
    public int Index;

    public override void Activate(bool activateNextEvent)
    {
        CLogger.Log("JukeboxEvent activated!");
        var gameSettings = GameManager.Instance.CurrentGameSettings;

        //find unlocked songs for jukebox
        var unlockedSongs = gameSettings.UnlockedSongs;
        var selectedSong = unlockedSongs[Index];
        AudioManager.Instance.PlayMusicClip(selectedSong);

        //increment Index and check if too big
        Index++;
        if(Index >= unlockedSongs.Count)
        {
            Index = 0;
        }
    }
}
