using UnityEngine;

public class TeleportEvent : EventBase
{
    public GameObject TargetPosition;

    public override void Activate(bool activateNextEvent)
    {
        CLogger.Log("TeleportEvent activated!");
        PlayerController.Instance.transform.position = TargetPosition.transform.position;
    }
}
