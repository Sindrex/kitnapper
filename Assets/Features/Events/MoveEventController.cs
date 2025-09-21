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
    public bool Started;
    public bool IsFinished;
    public string Id;
    public List<SetGameFlagCombo> SetFlags;
    public string NextEvent; //note that this bypasses any requirements


    // Start is called before the first frame update
    public void Setup()
    {
        Started = false;
        IsFinished = false;
        EndTriggerObject.SetActive(false);
        Id = gameObject.name;
        CLogger.Log($"MoveEvent \"{Id}\" setting up!");
    }

    void Update()
    {
        if (Started)
        {
            var endTriggerIsOn = EndTriggerObject.activeSelf;
            if (!endTriggerIsOn)
            {
                //CLogger.Log($"MoveEvent \"{Id}\" endTrigger is not on!");
                return;
            }
            DoFinishRoutine();
            
            //Activate next event
            GameManager.Instance.FindEvent(NextEvent)?.Activate();
        }
    }

    public override void Activate()
    {
        CLogger.Log($"MoveEvent \"{Id}\" activated!");
        TargetAnimator.Play(TargetAnimatorState);

        Started = true;
        IsFinished = false;
    }

    public void DoFinishRoutine()
    {
        CLogger.Log($"MoveEvent \"{Id}\" doing Finish Routine!");
        Started = false;
        IsFinished = true;
        EndTriggerObject.SetActive(false);

        Target.transform.localPosition = TargetPosition;
        CLogger.Log($"MoveEvent \"{Id}\" moved target to Position: {Target.transform.localPosition}");

        TargetAnimator.Play(NextAnimatorState);

        //set flags
        foreach (var flag in SetFlags)
        {
            GameManager.Instance.SetFlags(flag);
        }

        GameManager.Instance.SaveMoveEventState(this);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        string customName = "MoveGizmo.png";
        Gizmos.DrawIcon(transform.position, customName, true);
    }
#endif
}
