using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatnipEvent : EventBase
{
    public float NewSpeed = 400;
    public float NewSpeedSlow = 200;
    public float WaitForSeconds = 15;
    public string FlowerSpawnEventId;
    public bool OverrideForceCatnipUp;
    public bool OverrideForceCatnipDown;

    public override void Activate(bool activateNextEvent)
    {
        CLogger.Log($"CatnipEvent activated with OverrideForceCatnipUp: {OverrideForceCatnipUp}, OverrideForceCatnipDown: {OverrideForceCatnipDown}!");

        if (OverrideForceCatnipUp)
        {
            PlayerController.Instance.CurrentSpeed = NewSpeed;
            PlayerController.Instance.SetCatnipAnim(true);
            GameManager.Instance.SetFlags(new SetGameFlagCombo
            {
                Flag = GameFlag.CatnipUp,
                BoolValue = true
            });
        }
        else if(OverrideForceCatnipDown)
        {
            PlayerController.Instance.CurrentSpeed = NewSpeedSlow;
            PlayerController.Instance.SetCatnipAnim(false);
            GameManager.Instance.SetFlags(new SetGameFlagCombo
            {
                Flag = GameFlag.CatnipDown,
                BoolValue = true
            });
        }
        else //random
        {
            var gameSettings = GameManager.Instance.CurrentGameSettings;
            var sum = gameSettings.GameFlags.Count;
            var evenSum = sum % 2 == 0; //Pseudo-random
            if (evenSum)
            {
                PlayerController.Instance.CurrentSpeed = NewSpeed;
                PlayerController.Instance.SetCatnipAnim(true);
                GameManager.Instance.SetFlags(new SetGameFlagCombo
                {
                    Flag = GameFlag.CatnipUp,
                    BoolValue = true
                });
            }
            else
            {
                PlayerController.Instance.CurrentSpeed = NewSpeedSlow;
                PlayerController.Instance.SetCatnipAnim(false);
                GameManager.Instance.SetFlags(new SetGameFlagCombo
                {
                    Flag = GameFlag.CatnipDown,
                    BoolValue = true
                });
            }
        }
        StartCoroutine(ReturnSpeed());
    }

    IEnumerator ReturnSpeed()
    {
        CLogger.Log("ReturnSpeed starting!");
        yield return new WaitForSeconds(WaitForSeconds);
        PlayerController.Instance.CurrentSpeed = PlayerController.Speed;
        PlayerController.Instance.StopCatnipAnim();
        CLogger.Log("ReturnSpeed finished!");

        //check for achievement
        var gameSettings = GameManager.Instance.CurrentGameSettings;
        var catnipUpFlag = gameSettings.GameFlags.FirstOrDefault(x => x.Flag == GameFlag.CatnipUp);
        var catnipDownFlag = gameSettings.GameFlags.FirstOrDefault(x => x.Flag == GameFlag.CatnipDown);
        //In case the flag does not exist in settings, check against default values
        catnipUpFlag ??= new GameFlagCombo()
        {
            Flag = GameFlag.CatnipUp
        };
        catnipDownFlag ??= new GameFlagCombo()
        {
            Flag = GameFlag.CatnipDown
        };
        if(catnipUpFlag.BoolValue && catnipDownFlag.BoolValue && !SteamIntegration.IsAchievementUnlocked(SteamAchievement.ACH_CATNIP_UP_DOWN))
        {
            //Achievement
            SteamIntegration.UnlockAchievement(SteamAchievement.ACH_CATNIP_UP_DOWN);
            GameManager.Instance.FindEvent(FlowerSpawnEventId)?.Activate(true);
        }
    }
}
