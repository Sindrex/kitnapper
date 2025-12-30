using System.Collections;
using System.Collections.Generic;
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
        var evenSum = sum % 2 == 0;
        OldSpeed = PlayerController.Instance.Speed;
        if (evenSum)
        {
            PlayerController.Instance.Speed = NewSpeed;
        }
        else
        {
            PlayerController.Instance.Speed = NewSpeedSlow;
        }
        StartCoroutine(ReturnSpeed());
    }

    IEnumerator ReturnSpeed()
    {
        CLogger.Log("ReturnSpeed starting!");
        yield return new WaitForSeconds(WaitForSeconds);
        PlayerController.Instance.Speed = OldSpeed;
        CLogger.Log("ReturnSpeed finished!");
    }
}
