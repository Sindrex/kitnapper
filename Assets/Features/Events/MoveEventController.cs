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
                CLogger.Log($"MoveEvent \"{Id}\" endTrigger is not on!");
                return;
            }
            DoFinishRoutine();
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

        //GameManager.Instance.SaveMoveEventState(this);

        //Save event state
        //GameManager.Instance.SaveEventState(this);
    }
}
