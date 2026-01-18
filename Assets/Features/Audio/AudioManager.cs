using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public enum AudioLabel
{
    MenuMusic,
    ClickSFX,
    FloristMoment,
    DialogueSFX1, DialogueSFX2,
    TelephoneSFX,
    DoorOpenSFX, DoorLockedSFX,
    SwapSelectSFX,
    PickupSFX, 
    ScienceSFX,
    ForestRustle1, ForestRustle2, ForestRustle3,
    CatMeow1, CatMeow2, CatMeow3,
    CatPurr1, CatPurr2, CatPurr3
    //music trond: Menu/Player House, Forest, Town, Cattelatte, (Shrine, University, Credits)
    //music cf: FloristMoment
}

[Serializable]
public class AudioClipMapping
{
    public AudioLabel Label;
    public AudioClip Clip;
}

public class AudioManager : MonoBehaviour
{
    public AudioMixer AudioMixer;
    public AudioSource MusicSource;
    public GameObject SFXSourceParent;
    public GameObject SFXSourcePrefab;
    public List<AudioSource> SFXInPlay = new List<AudioSource>();

    //mapping
    public List<AudioClipMapping> AudioClips;

    //Singleton pattern
    public static AudioManager Instance;

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); //keep this between scenes. NB! GameObject must be placed on root level on hierarchy!
            return;
        }
        Destroy(this.gameObject);
    }

    public async Task Setup(List<string> assetPacks)
    {
        CLogger.Log($"AudioManager: Starting up!");
    }

    public void PlaySFXClip(AudioLabel audioLabel)
    {
        var sfxGameObject = Instantiate(SFXSourcePrefab, SFXSourceParent.transform);
        var sfxSource = sfxGameObject.GetComponent<AudioSource>();
        sfxSource.clip = AudioClips.FirstOrDefault(e => e.Label == audioLabel).Clip;
        sfxSource.Play();
        SFXInPlay.Add(sfxSource);
    }

    public void PlayMusicClip(AudioLabel audioLabel)
    {
        var audioClip = AudioClips.FirstOrDefault(e => e.Label == audioLabel).Clip;
        MusicSource.clip = audioClip;
        MusicSource.Play(); //loops

        //add to unlocked songs from game or manu
        var gameSettingsGame = GameManager.Instance?.CurrentGameSettings;
        var gameSettignsMenu = MenuManager.Instance?.CurrentGameSettings;
        if (gameSettingsGame != null && !gameSettingsGame.UnlockedSongs.Contains(audioLabel))
        {
            CLogger.Log($"Adding {audioLabel} to UnlockedSongs from game");
            gameSettingsGame.UnlockedSongs.Add(audioLabel);
            gameSettingsGame.SaveFromMenu();
        }
        if (gameSettignsMenu != null && !gameSettignsMenu.UnlockedSongs.Contains(audioLabel))
        {
            CLogger.Log($"Adding {audioLabel} to UnlockedSongs from menu");
            gameSettignsMenu.UnlockedSongs.Add(audioLabel);
            gameSettignsMenu.SaveFromMenu();
        }
    }

    public void FixedUpdate()
    {
        //check if any SFX are done playing, clean up objects when done
        if (SFXInPlay.Count > 0)
        {
            var sfxToDestroy = new List<AudioSource>();
            foreach (var sfxSource in SFXInPlay)
            {
                if (!sfxSource.isPlaying)
                {
                    sfxToDestroy.Add(sfxSource);
                }
            }
            for (int i = sfxToDestroy.Count - 1; i >= 0; i--)
            {
                SFXInPlay.Remove(sfxToDestroy[i]);
                var gameObjectToDestroy = sfxToDestroy[i].gameObject;
                sfxToDestroy.RemoveAt(i);
                Destroy(gameObjectToDestroy);
            }
        }
    }
}
