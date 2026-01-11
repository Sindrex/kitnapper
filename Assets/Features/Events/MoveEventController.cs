using System.Collections.Generic;
using UnityEngine;

public class MoveEventController : EventBase
{
    public Vector3 TargetPosition;
    public GameObject Target;
    public Animator TargetAnimator;
    public string TargetAnimatorState;
    public string NextAnimatorState;
    public GameObject EndTriggerObject;
    public bool AnimationChangeOnly;
    public bool AnimationFinished;
    public bool Started;
    public bool IsFinished;
    public string Id;
    public List<SetGameFlagCombo> SetFlags;
    public string NextEvent;

    // Start is called before the first frame update
    public void Start()
    {
        Id = gameObject.name;
    }

    public void Setup()
    {
        Started = false;
        IsFinished = false;
        if(EndTriggerObject != null) EndTriggerObject.SetActive(false);
        Id = gameObject.name;
        CLogger.Log($"MoveEvent \"{Id}\" setting up!");
    }

    void Update()
    {
        if (Started)
        {
            if (!AnimationFinished)
            {
                return;
            }
            DoFinishRoutine(true);
        }
    }

    public override void Activate(bool activateNextEvent)
    {
        CLogger.Log($"MoveEvent \"{Id}\" activated!");
        TargetAnimator.Play(TargetAnimatorState);

        if(AnimationChangeOnly) return;
        
        Started = true;
        IsFinished = false;
        AnimationFinished = false;
    }

    public void DoFinishRoutine(bool activateNextEvent)
    {
        CLogger.Log($"MoveEvent \"{Id}\" doing Finish Routine!");
        Started = false;
        IsFinished = true;
        if(EndTriggerObject != null) EndTriggerObject.SetActive(false);

        Target.transform.localPosition = TargetPosition;
        CLogger.Log($"MoveEvent \"{Id}\" moved target to Position: {Target.transform.localPosition}");
        
        TargetAnimator.Play(NextAnimatorState);

        //set flags
        foreach (var flag in SetFlags)
        {
            GameManager.Instance.SetFlags(flag);
        }

        GameManager.Instance.SaveMoveEventState(this);

        if (activateNextEvent)
        {
            //Activate next event
            var nextEvent = GameManager.Instance.FindEvent(NextEvent);
            if(nextEvent != null && nextEvent.CheckRequirements())
            {
                nextEvent.Activate(true);
            }
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        string customName = "MoveGizmo.png";
        Gizmos.DrawIcon(transform.position, customName, true);
    }
#endif
}
