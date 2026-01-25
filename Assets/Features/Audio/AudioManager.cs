using System;
using System.Collections;
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
    CatPurr1, CatPurr2, CatPurr3,
    CattelatteMixPurrs,
    Walk1, Walk2, Walk3,
    FloristMixPurrs, MayorOfficeMixPurrs, OfficeMixPurrs
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
    public AudioMixer MainMixer;
    public AudioSource MusicSource;
    public GameObject SFXSourceParent;
    public GameObject SFXSourcePrefab;
    public List<AudioSource> SFXInPlay = new List<AudioSource>();

    //mapping
    public List<AudioClipMapping> AudioClips;

    //Singleton pattern
    public static AudioManager Instance;

    //music fade in
    public float MusicFadeStepSeconds;
    public int MusicFadeSteps;
    public float MusicFadeMinVolumeDb;
    public int VolumeMaxIndex;
    private const string MusicVolumeParameterName = "MusicVolume";
    private const string SFXVolumeParameterName = "SFXVolume";

    //Queue
    private List<AudioLabel> MusicQueue = new List<AudioLabel>();

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

    public void PlaySFXClip(AudioLabel audioLabel, bool loop = false)
    {
        var sfxGameObject = Instantiate(SFXSourcePrefab, SFXSourceParent.transform);
        var sfxSource = sfxGameObject.GetComponent<AudioSource>();
        sfxSource.clip = AudioClips.FirstOrDefault(e => e.Label == audioLabel).Clip;
        sfxSource.Play();
        sfxSource.loop = loop;
        SFXInPlay.Add(sfxSource);
    }

    public void DestroySFXClips(AudioLabel audioLabel)
    {
        var clip = AudioClips.FirstOrDefault(e => e.Label == audioLabel).Clip;
        var sfxToDestroy = new List<AudioSource>();
        foreach (var sfxSource in SFXInPlay)
        {
            if (sfxSource.clip == clip)
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

    public void PlayMusicClip(AudioLabel audioLabel, bool fadeIn, bool queue)
    {
        if(fadeIn && queue)
        {
            CLogger.LogWarning("Both fadeIn and queue is true. This is not supported. Defaults to fadeIn only.");
        }

        if (fadeIn)
        {
            StartCoroutine(FadeMusicIn(audioLabel));
        }
        else if (queue)
        {
            MusicQueue = new List<AudioLabel>
            {
                audioLabel
            };
            MusicSource.loop = false;
        }
        else
        {
            var audioClip = AudioClips.FirstOrDefault(e => e.Label == audioLabel).Clip;
            MusicSource.clip = audioClip;
            MusicSource.Play(); //loops
            PlayMusicClipFinal(audioLabel);
        }
    }

    private void PlayMusicClipFinal(AudioLabel audioLabel)
    {
        //add to unlocked songs from game or manu
        var currentGameSettings = GameManager.Instance?.CurrentGameSettings ?? MenuManager.Instance?.CurrentGameSettings;
        if (currentGameSettings != null && !currentGameSettings.UnlockedSongs.Contains(audioLabel))
        {
            CLogger.Log($"Adding {audioLabel} to UnlockedSongs");
            currentGameSettings.UnlockedSongs.Add(audioLabel);
            currentGameSettings.SaveFromMenu();
        }
    }

    IEnumerator FadeMusicIn(AudioLabel audioLabel)
    {
        var currentGameSettings = GameManager.Instance?.CurrentGameSettings ?? MenuManager.Instance?.CurrentGameSettings;
        var musicVolumeIndex = currentGameSettings.MusicVolumeIndex;
        var maxVolumeIndex = (float) VolumeMaxIndex - musicVolumeIndex;
        var volumeIndexPerFadeStep = maxVolumeIndex / MusicFadeSteps;
        
        //Fade out
        for(int i = 0; i < MusicFadeSteps; i++)
        {
            var currentVolumeIndex = maxVolumeIndex - volumeIndexPerFadeStep * i;
            var currentVolume = currentVolumeIndex / VolumeMaxIndex;
            var currentDb = VolumeFunction(currentVolume);
            MainMixer.SetFloat(MusicVolumeParameterName, currentDb);
            yield return new WaitForSeconds(MusicFadeStepSeconds);
        }

        //Fade in
        var audioClip = AudioClips.FirstOrDefault(e => e.Label == audioLabel).Clip;
        MusicSource.clip = audioClip;
        MusicSource.Play(); //loops
        for(int i = 0; i < MusicFadeSteps; i++)
        {
            var currentVolumeIndex = volumeIndexPerFadeStep * i;
            var currentVolume = currentVolumeIndex / VolumeMaxIndex;
            var currentDb = VolumeFunction(currentVolume);
            MainMixer.SetFloat(MusicVolumeParameterName, currentDb);
            yield return new WaitForSeconds(MusicFadeStepSeconds);
        }

        var musicVolume = musicVolumeIndex / (float) VolumeMaxIndex;
        var musicFadeMaxVolumeDb = VolumeFunction(musicVolume);
        MainMixer.SetFloat(MusicVolumeParameterName, musicFadeMaxVolumeDb);
        PlayMusicClipFinal(audioLabel);
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

        if(MusicQueue.Count > 0)
        {
            if (!MusicSource.isPlaying)
            {
                var nextMusicLabel = MusicQueue[0];
                var audioClip = AudioClips.FirstOrDefault(e => e.Label == nextMusicLabel).Clip;
                MusicSource.clip = audioClip;
                MusicSource.loop = true;
                MusicSource.Play();
                MusicQueue.RemoveAt(0);
            }
        }
    }

    //Assumes 0.0001 < x < 1
    public static float VolumeFunction(float x)
    {
        if (x <= 0)
        {
            x = 0.0001f;
        }
        else if (x > 1)
        {
            x = 1;
        }
        return Mathf.Log10(x) * 20;
    }
}
