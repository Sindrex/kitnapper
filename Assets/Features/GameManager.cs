using System.Collections.Generic;
using System.Linq;
using Assets.Features.Util.GlobalException;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public CameraController MainCamera;
    public GameSettings CurrentGameSettings;
    public FinaleMomentEventController FinaleMomentEvent;
    private List<EventController> Events = new List<EventController>();
    private List<MoveEventController> MoveEvents = new List<MoveEventController>();

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
        //Load Steam
        SteamIntegration.Initialize();

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
        CLogger.Log($"Setting up Player at ({CurrentGameSettings.PlayerPosition.X}, {CurrentGameSettings.PlayerPosition.Y}, {CurrentGameSettings.PlayerPosition.Z}).");
        PlayerController.Instance.transform.position = CurrentGameSettings.PlayerPosition.ToVector3();

        //Setup events
        CLogger.Log($"Setting up {CurrentGameSettings.EventStates.Count} events from EventStates!");
        foreach (var eventState in CurrentGameSettings.EventStates)
        {
            if (string.IsNullOrEmpty(eventState.Id))
            {
                CLogger.LogError("Invalid EventState id.");
                continue;
            }

            var gameObject = GameObject.Find(eventState.Id);
            var eventController = gameObject.GetComponent<EventController>();
            if (eventController != null)
            {
                eventController.Setup();
                AddEvent(eventController);
            }
            var moveEventController = gameObject.GetComponent<MoveEventController>();
            if(moveEventController != null)
            {
                moveEventController.Setup();
                AddMoveEvent(moveEventController); //Add to / load state
            }
        }

        var allEventGameObjects = GameObject.FindGameObjectsWithTag("Event");
        CLogger.Log($"Checking all {allEventGameObjects.Length} events!");
        foreach (var gameObject in allEventGameObjects)
        {
            var eventController = gameObject.GetComponent<EventController>();
            if (!Events.Contains(eventController))
            {
                eventController.Setup();
                AddEvent(eventController);
            }
        }

        //Setup move events
        var moveEventGameObjects = GameObject.FindGameObjectsWithTag("MoveEvent");
        CLogger.Log($"Checking all {moveEventGameObjects.Length} move events!");
        var retries = new List<MoveEventController>();
        foreach (var gameObject in moveEventGameObjects)
        {
            var eventController = gameObject.GetComponent<MoveEventController>();
            if (!MoveEvents.Contains(eventController))
            {
                eventController.Setup();
                AddMoveEvent(eventController);
            }
        }

        //setup finale event
        FinaleMomentEvent.Setup();

        //Setup globals
        InputController.InputEnabled = true;

        //Start fade in
        CameraController.Instance.FadeIn();
    }

    void Update()
    {
        SteamIntegration.OnUpdate();

        if (InputController.GetInput(InputPurpose.QUIT)) Quit();
        if (InputController.GetInput(InputPurpose.CLOSE_EXCEPTION)) GlobalExceptionManager.Instance.CloseWindow();
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

        if (FinaleMomentEvent.CheckIfAllNamesAreDiscovered())
        {
            //Achievement
            SteamIntegration.UnlockAchievement(SteamAchievement.ACH_FIND_ALL_NAMES);
        }
    }

    public void AddEvent(EventController eventController)
    {
        var eventState = CurrentGameSettings.EventStates.FirstOrDefault(x => x.Id == eventController.Id);
        if (eventState != null && eventState.IsFinished)
        {
            eventController.Activate(false);
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

    public void AddMoveEvent(MoveEventController moveEventController)
    {
        var eventState = CurrentGameSettings.EventStates.FirstOrDefault(x => x.Id == moveEventController.Id);
        if (eventState != null && eventState.IsFinished)
        {
            moveEventController.DoFinishRoutine(false);
        }

        MoveEvents.Add(moveEventController);
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

    public void SaveMoveEventState(MoveEventController moveEventController)
    {
        var current = CurrentGameSettings.EventStates.FirstOrDefault(x => x.Id == moveEventController.Id);
        if (current != null)
        {
            current.IsFinished = moveEventController.IsFinished;
        }
        else
        {
            CurrentGameSettings.EventStates.Add(new EventState
            {
                Id = moveEventController.Id,
                IsFinished = moveEventController.IsFinished
            });
        }

        //save CurrentGameSettings
        CurrentGameSettings.Save();
    }

    public EventBase FindEvent(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        var eventController = Events.FirstOrDefault(x => x.Id == id);
        var moveEventController = MoveEvents.FirstOrDefault(x => x.Id == id);

        if (eventController != null && moveEventController != null)
        {
            CLogger.LogError($"FindEvent found both Event and MoveEvent searching for id \"{id}\"! Returning Event.");
        }
        if(FinaleMomentEvent.Id == id)
        {
            return FinaleMomentEvent;
        }
        if (eventController == null && moveEventController == null)
        {
            CLogger.LogError($"FindEvent found no matching event searching for id \"{id}\"!");
            return null;
        }
        return (EventBase) eventController ?? moveEventController;
    }

    public void Quit()
    {
        //Cannot exit during dialogue, to ensure valid savestate
        if (MyDialogueManager.Instance.IsDialogue)
        {
            return;
        }

        CurrentGameSettings.Save();
        SceneManager.LoadScene(0); //Menu
    }
}
