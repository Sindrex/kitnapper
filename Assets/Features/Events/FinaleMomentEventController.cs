using System;
using System.Collections.Generic;
using System.Linq;
using MeetAndTalk;
using UnityEngine;
using UnityEngine.UI;

public class FinaleMomentEventController : EventBase
{
    public string Id;
    public GameObject PlayerTeleportPosition;
    public Animator CreditsAnimator;
    public string CreditsAnimation;
    public float CreditsSeconds;
    public float FadeWaitSeconds;
    public Vector3 PlayerStartPosition;
    public string NextEvent;

    //names
    public List<CharacterNameCombo> CharacterNames;

    // Start is called before the first frame update
    public void Setup()
    {
        Id = gameObject.name;
    }

    public override void Activate(bool activateNextEvent)
    {
        CLogger.Log($"Event \"{Id}\" activated!");
        //fade out game
        //credits
        //TP player, lock movement
        //Play credit sequence (one animation)
        //Names of all characters the player has interacted with, rest as ???.
        var gameSettings = GameManager.Instance.CurrentGameSettings;

        foreach(var characterNameCombo in CharacterNames)
        {
            var gameFlag = characterNameCombo.CharacterFlag;
            var currentFlag = gameSettings.GameFlags.FirstOrDefault(x => x.Flag == gameFlag);
            //In case the flag does not exist in settings, check against default values
            currentFlag ??= new GameFlagCombo()
            {
                Flag = gameFlag
            };
            
            if (currentFlag.BoolValue)
            {
                characterNameCombo.NameText.text = characterNameCombo.Name;
            }
            else
            {
                characterNameCombo.NameText.text = "???";
            }
        }

        StartCoroutine(StartCreditsAnimation());
    }

    IEnumerator<WaitForSeconds> StartCreditsAnimation()
    {
        MyDialogueManager.Instance.CloseDialogue();
        CameraController.Instance.FadeOut();
        PlayerController.Instance.CanMove = false;
        yield return new WaitForSeconds(FadeWaitSeconds);
        AudioManager.Instance.PlayMusicClip(AudioLabel.CreditsMusic, true, false);
        CameraController.Instance.FadeIn();
        var newPos = PlayerTeleportPosition.transform.position;
        CameraController.Instance.SetPosition(newPos);
        CameraController.Instance.Player.SetActive(false);
        CreditsAnimator.Play(CreditsAnimation);
        yield return new WaitForSeconds(CreditsSeconds);
        CameraController.Instance.FadeOut();
        yield return new WaitForSeconds(FadeWaitSeconds);
        CameraController.Instance.FadeIn();
        CameraController.Instance.FollowPlayer = true;
        PlayerController.Instance.CanMove = true;
        PlayerController.Instance.gameObject.transform.localPosition = PlayerStartPosition;
        CameraController.Instance.Player.SetActive(true);
        AudioManager.Instance.PlayMusicClip(AudioLabel.MenuMusic, false, true);

        //Achievement
        SteamIntegration.UnlockAchievement(SteamAchievement.ACH_FINISH_GAME);

        //Activate next event
        var nextEvent = GameManager.Instance.FindEvent(NextEvent);
        if(nextEvent != null && nextEvent.CheckRequirements())
        {
            nextEvent.Activate(true);
        }
    }
    
    public bool CheckIfAllNamesAreDiscovered()
    {
        var gameSettings = GameManager.Instance.CurrentGameSettings;
        var isOk = true;
        foreach(var characterNameCombo in CharacterNames)
        {
            var gameFlag = characterNameCombo.CharacterFlag;
            var currentFlag = gameSettings.GameFlags.FirstOrDefault(x => x.Flag == gameFlag);
            //In case the flag does not exist in settings, check against default values
            currentFlag ??= new GameFlagCombo()
            {
                Flag = gameFlag
            };
            
            if (!currentFlag.BoolValue)
            {
                isOk = false;
            }
        }
        return isOk;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        string customName = "EventGizmo.png";
        Gizmos.DrawIcon(transform.position, customName, true);
    }
#endif
}

[Serializable]
public class CharacterNameCombo
{
    public GameFlag CharacterFlag;
    public string Name;
    public Text NameText;
}