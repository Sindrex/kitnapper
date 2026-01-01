using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EventController : EventBase
{
    public bool IsActive;
    public bool IsFinishable;
    public bool IsFinished;

    public bool RequireInteract;
    public bool NotInteractable;
    public List<ReqFlagBase> RequiredFlags = new List<ReqFlagBase>();

    public List<GameObject> TargetObjects;
    public List<SetGameFlagCombo> SetFlags;
    public string NextEvent;
    public EventBase NextEventBase; //activate ad-hoc events (without state)
    public string Id;

    // Start is called before the first frame update
    public void Setup()
    {
        IsFinished = false;
        Id = gameObject.name;
    }

    void Update()
    {
        var isFinishableAndFinished = IsFinishable && IsFinished;
        if (IsActive && !isFinishableAndFinished && !NotInteractable)
        {
            if (!CheckRequirements())
            {
                return;
            }

            if (RequireInteract && !InputController.GetInput(InputPurpose.INTERACT))
            {
                return;
            }

            Activate(true);
        }
    }

    public override void Activate(bool activateNextEvent)
    {
        CLogger.Log($"Event \"{Id}\" activated!");

        //turn on or off TargetObjects
        foreach (GameObject target in TargetObjects)
        {
            target.SetActive(!target.activeSelf);
        }

        //set flags
        foreach (var flag in SetFlags)
        {
            GameManager.Instance.SetFlags(flag);
        }

        if (IsFinishable)
        {
            IsFinished = true;
            PlayerController.Instance.StopShowInteractHint();
            IsActive = false;
        }

        //Save event state
        GameManager.Instance.SaveEventState(this);

        if (activateNextEvent)
        {
            //Activate next event
            var nextEvent = GameManager.Instance.FindEvent(NextEvent);
            if(nextEvent != null && nextEvent.CheckRequirements())
            {
                nextEvent.Activate(true);
            }

            //run ad hoc event
            if(NextEventBase != null && NextEventBase.CheckRequirements())
            {
                CLogger.Log("Activating NextEventBase");
                NextEventBase.Activate(true);
            }
        }
    }

    public override bool CheckRequirements()
    {
        var passedRequirements = true;
        var requiresFlags = RequiredFlags.Any(x => x.GetFlag() != GameFlag.Default);
        if (requiresFlags)
        {
            var results = new List<bool>();
            foreach (var requiredFlag in RequiredFlags)
            {
                results.Add(requiredFlag.Result());
            }

            if (results.Any(e => e == false))
            {
                passedRequirements = false;
            }
        }
        return passedRequirements;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsFinished || NotInteractable) return;
        if (!other.gameObject.CompareTag("Player")) return;

        if (CheckRequirements())
        {
            PlayerController.Instance.ShowInteractHint();
            IsActive = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (IsFinished || NotInteractable) return;
        if (!other.gameObject.CompareTag("Player")) return;

        PlayerController.Instance.StopShowInteractHint();
        IsActive = false;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        string customName = "EventGizmo.png";
        Gizmos.DrawIcon(transform.position, customName, true);
    }
#endif
}
