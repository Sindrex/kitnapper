using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionEvent : EventBase
{
    public ReactionController Controller;
    public Reaction SelectedReaction;

    public override void Activate(bool activateNextEvent)
    {
        CLogger.Log("ReactionEvent activated!");
        Controller.StartReaction(SelectedReaction);
    }
}
