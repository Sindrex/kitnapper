using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EventController : MonoBehaviour
{
    public bool IsActive;
    public bool IsFinishable;
    public bool IsFinished;

    public Text InteractText;
    public bool RequireInteract;
    public List<RequiredGameFlagCombo> RequiredFlags;

    public List<GameObject> TargetObjects;
    public List<SetGameFlagCombo> SetFlags;
    public string Id;

    // Start is called before the first frame update
    public void Setup()
    {
        IsFinished = false;
        InteractText.gameObject.SetActive(false);
        Id = gameObject.name;
    }

    void Update()
    {
        var passedRequirementsCheck = CheckRequirements();
        var isFinishableAndFinished = IsFinishable && IsFinished;
        if (IsActive && !isFinishableAndFinished && passedRequirementsCheck)
        {
            if (RequireInteract && !Input.GetKeyDown(KeyCode.Space))
            {
                return;
            }

            DoFinishRoutine();
        }
    }

    public void DoFinishRoutine()
    {
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
            InteractText.gameObject.SetActive(false);
            IsActive = false;
        }

        //Save event state
        GameManager.Instance.SaveEventState(this);
    }

    private bool CheckRequirements()
    {
        var passedRequirements = true;
        var requiresFlags = RequiredFlags.Any(x => x.Flag != GameFlag.Default);
        if (requiresFlags)
        {
            var gameSettings = GameManager.Instance.CurrentGameSettings;
            foreach (var requiredFlag in RequiredFlags)
            {
                var currentFlag = gameSettings.GameFlags.FirstOrDefault(x => x.Flag == requiredFlag.Flag);
                //In case the flag does not exist in settings, check against default values
                currentFlag ??= new GameFlagCombo()
                {
                    Flag = requiredFlag.Flag
                };

                bool intValuePassed = false;
                if (requiredFlag.LessThanIntValue               && currentFlag.IntValue < requiredFlag.IntValue) intValuePassed = true;
                else if (requiredFlag.LessThanOrEqualIntValue   && currentFlag.IntValue <= requiredFlag.IntValue) intValuePassed = true;
                else if (requiredFlag.MoreThanIntValue          && currentFlag.IntValue > requiredFlag.IntValue) intValuePassed = true;
                else if (requiredFlag.MoreThanOrEqualIntValue   && currentFlag.IntValue >= requiredFlag.IntValue) intValuePassed = true;
                else if (currentFlag.IntValue == requiredFlag.IntValue) intValuePassed = true;


                if (currentFlag.BoolValue != requiredFlag.BoolValue
                || currentFlag.StringValue != requiredFlag.StringValue
                || !intValuePassed)
                {
                    passedRequirements = false;
                }
            }
        }
        return passedRequirements;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsFinished) return;
        if (!other.gameObject.CompareTag("Player")) return;

        if (CheckRequirements())
        {
            InteractText.gameObject.SetActive(true);
            IsActive = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (IsFinished) return;
        if (!other.gameObject.CompareTag("Player")) return;

        InteractText.gameObject.SetActive(false);
        IsActive = false;
    }
}
