using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatnipEvent : EventBase
{
    public float NewSpeed = 10;
    public float WaitForSeconds = 10;
    private float OldSpeed;

    public override void Activate(bool activateNextEvent)
    {
        CLogger.Log("CatnipEvent activated!");
        OldSpeed = CameraController.Instance.Speed;
        CameraController.Instance.Speed = NewSpeed;
        StartCoroutine(LowerSpeed());
    }

    IEnumerator LowerSpeed()
    {
        CLogger.Log("LowerSpeed starting!");
        yield return new WaitForSeconds(WaitForSeconds);
        CameraController.Instance.Speed = OldSpeed;
        CLogger.Log("LowerSpeed finished!");
    }
}
