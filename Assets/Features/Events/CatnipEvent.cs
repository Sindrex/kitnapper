using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatnipEvent : EventBase
{
    public float NewSpeed = 400;
    public float NewSpeedSlow = 200;
    public float WaitForSeconds = 15;
    private float OldSpeed;

    public override void Activate(bool activateNextEvent)
    {
        CLogger.Log("CatnipEvent activated!");
        var gameSettings = GameManager.Instance.CurrentGameSettings;
        var sum = gameSettings.GameFlags.Count;
        var evenSum = sum % 2 == 0; //Pseudo-random
        OldSpeed = PlayerController.Instance.Speed;
        if (evenSum)
        {
            PlayerController.Instance.Speed = NewSpeed;
            GameManager.Instance.SetFlags(new SetGameFlagCombo
            {
                Flag = GameFlag.CatnipUp,
                BoolValue = true
            });
        }
        else
        {
            PlayerController.Instance.Speed = NewSpeedSlow;
            GameManager.Instance.SetFlags(new SetGameFlagCombo
            {
                Flag = GameFlag.CatnipDown,
                BoolValue = true
            });
        }
        StartCoroutine(ReturnSpeed());
    }

    IEnumerator ReturnSpeed()
    {
        CLogger.Log("ReturnSpeed starting!");
        yield return new WaitForSeconds(WaitForSeconds);
        PlayerController.Instance.Speed = OldSpeed;
        CLogger.Log("ReturnSpeed finished!");

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
        if(catnipUpFlag.BoolValue && catnipDownFlag.BoolValue)
        {
            //Achievement
            SteamIntegration.UnlockAchievement(SteamAchievement.ACH_CATNIP_UP_DOWN);
        }
    }
}
