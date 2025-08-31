using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandManager : MonoBehaviour
{
    public GameObject CommandParent;
    public InputField CommandInputField;
    public bool IsCommandTexting;

    //singleton
    public static CommandManager Instance { get; private set; }
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
        CommandParent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (InputController.GetInput(InputPurpose.COMMAND_OPEN) && !IsCommandTexting)
        {
            CameraController.Instance.CanMove = false;
            CommandParent.SetActive(true);
            EventSystem.current.SetSelectedGameObject(CommandInputField.gameObject, null);
            //CommandInputField.OnPointerClick(null);
            CommandInputField.ActivateInputField();
            IsCommandTexting = true;
        }
        else if (InputController.GetInput(InputPurpose.COMMAND_ENTER))
        {
            ParseCommand(CommandInputField.text);
            CameraController.Instance.CanMove = true;
            IsCommandTexting = false;
            CommandInputField.text = "";
            CommandParent.SetActive(false);
        }
    }

    private void ParseCommand(string command)
    {
        if (string.IsNullOrEmpty(command)) return;

        var parsed = command.Split(" ");
        if (parsed.Length < 2)
        {
            CLogger.Log("Command length less than 2.");
            return;
        }

        var flagName = parsed[0];
        var boolValue = parsed[1];

        var gameFlags = Enum.GetNames(typeof(GameFlag));
        var gameFlagValues = (GameFlag[])Enum.GetValues(typeof(GameFlag));
        var setFlag = new SetGameFlagCombo();
        foreach (var flag in gameFlags)
        {
            if (flag.ToLower().Equals(flagName.ToLower()))
            {
                setFlag.Flag = gameFlagValues[Array.IndexOf(gameFlags, flag)];
                CLogger.Log($"Command: Flag found {setFlag.Flag}");
            }
        }

        if (boolValue.ToLower().Equals("true"))
        {
            setFlag.BoolValue = true;
        }

        if (parsed.Length > 3)
        {
            var stringValue = parsed[2];
            setFlag.StringValue = stringValue;
        }

        if (parsed.Length > 4)
        {
            var intValueString = parsed[3];
            if (int.TryParse(intValueString, out var intValue))
            {
                setFlag.SetIntValueHasValue = true;
                setFlag.SetIntValue = intValue;
            }
        }

        if (parsed.Length > 5)
        {
            var addIntValueString = parsed[4];
            if (int.TryParse(addIntValueString, out var intValue))
            {
                setFlag.AddIntValueHasValue = true;
                setFlag.AddIntValue = intValue;
            }
        }

        CLogger.Log($"Command: Setting flag values: " +
            $"BoolValue: {setFlag.BoolValue}, StringValue: {setFlag.StringValue}, " +
            $"SetIntValue: {setFlag.SetIntValue}, AddIntValue: {setFlag.AddIntValue}");
        GameManager.Instance.SetFlags(setFlag);
    }
}
