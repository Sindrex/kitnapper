using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameCharacterController : MonoBehaviour
{
    public List<MoveEventController> MoveEvents;
    
    public void MoveEventFinished(string moveEventId)
    {
        var moveEvent = MoveEvents.FirstOrDefault(x => x.Id == moveEventId);
        if(moveEvent != null && !moveEvent.IsFinished)
        {
            CLogger.Log($"Found moveEvent: {moveEvent.Id}");
            moveEvent.AnimationFinished = true;
        }
    }
}
