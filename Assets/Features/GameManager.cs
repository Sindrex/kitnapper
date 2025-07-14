using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public CameraController MainCamera;
    public GameSettings CurrentGameSettings;
    private List<EventController> Events = new List<EventController>();

    //singleton
    public static GameManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //load GameSettings
        CLogger.Log("Loading GameSettings.");
        if (!GameSettingsLoader.GameSettingsExist())
        {
            CLogger.Log("Found no GameSettings file! Creating new.");
            CurrentGameSettings = new GameSettings();
            CurrentGameSettings.Save();
        }
        else
        {
            CurrentGameSettings = GameSettingsLoader.Load();
        }

        //setup camera (BEFORE events)
        CLogger.Log($"Setting up MainCamera at ({CurrentGameSettings.PlayerPosition.X}, {CurrentGameSettings.PlayerPosition.Y}, {CurrentGameSettings.PlayerPosition.Z}).");
        MainCamera.transform.position = CurrentGameSettings.PlayerPosition.ToVector3();

        //Setup events
        var eventGameObjects = GameObject.FindGameObjectsWithTag("Event");
        foreach (var gameObject in eventGameObjects)
        {
            var eventController = gameObject.GetComponent<EventController>();
            eventController.Setup();
            AddEvent(eventController);
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Quit();
        if(Input.GetKeyDown(KeyCode.R)) Reset();
    }

    public void SetFlags(SetGameFlagCombo flagCombo)
    {
        var current = CurrentGameSettings.GameFlags.FirstOrDefault(x => x.Flag == flagCombo.Flag);
        if (current != null)
        {
            current.BoolValue = flagCombo.BoolValue;
            current.StringValue = flagCombo.StringValue;

            if (flagCombo.AddIntValueHasValue) current.IntValue += flagCombo.AddIntValue;
            if (flagCombo.SetIntValueHasValue) current.IntValue = flagCombo.SetIntValue;
        }
        else
        {
            var intValue = 0;
            if (flagCombo.AddIntValueHasValue) intValue += flagCombo.AddIntValue;
            if (flagCombo.SetIntValueHasValue) intValue = flagCombo.SetIntValue;
            CurrentGameSettings.GameFlags.Add(new GameFlagCombo
            {
                Flag = flagCombo.Flag,
                BoolValue = flagCombo.BoolValue,
                StringValue = flagCombo.StringValue,
                IntValue = intValue
            });
        }

        //save CurrentGameSettings
        CurrentGameSettings.Save();
    }

    public void AddEvent(EventController eventController)
    {
        var eventState = CurrentGameSettings.EventStates.FirstOrDefault(x => x.Id == eventController.Id);
        if (eventState != null && eventState.IsFinished)
        {
            eventController.DoFinishRoutine();
        }
        if (eventState != null)
        {
            for (int i = 0; i < eventState.TargetObjectsIsActive.Count; i++)
            {
                var setActive = eventState.TargetObjectsIsActive[i];
                if (i < eventController.TargetObjects.Count)
                {
                    eventController.TargetObjects[i].SetActive(setActive);
                }
                else
                {
                    CLogger.Log($"Missing event targetObject for Id = {eventController.Id}, i = {i}");
                }
            }
        }

        Events.Add(eventController);
    }

    public void SaveEventState(EventController eventController)
    {
        var current = CurrentGameSettings.EventStates.FirstOrDefault(x => x.Id == eventController.Id);
        if (current != null)
        {
            var targetObjectsIsActiveList = new List<bool>();
            foreach (var gameObject in eventController.TargetObjects)
            {
                targetObjectsIsActiveList.Add(gameObject.activeSelf);
            }

            current.IsFinished = eventController.IsFinished;
            current.TargetObjectsIsActive = targetObjectsIsActiveList;
        }
        else
        {
            var targetObjectsIsActiveList = new List<bool>();
            foreach (var gameObject in eventController.TargetObjects)
            {
                targetObjectsIsActiveList.Add(gameObject.activeSelf);
            }
            CurrentGameSettings.EventStates.Add(new EventState
            {
                Id = eventController.Id,
                IsFinished = eventController.IsFinished,
                TargetObjectsIsActive = targetObjectsIsActiveList
            });
        }

        //save CurrentGameSettings
        CurrentGameSettings.Save();
    }

    public void Reset()
    {
        CurrentGameSettings.ResetGame();
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        CurrentGameSettings.Save();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
