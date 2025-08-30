using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CommandManager : MonoBehaviour
{
    public GameObject CommandParent;
    public TextField CommandTextField;

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
        if (InputController.GetInput(InputPurpose.COMMAND_OPEN))
        {
            CommandParent.SetActive(!CommandParent.activeSelf);
        }

        if (InputController.GetInput(InputPurpose.COMMAND_ENTER))
        {
            ParseCommand(CommandTextField.text);
        }
    }

    private void ParseCommand(string command)
    {
        if (string.IsNullOrEmpty(command)) return;

        var parsed = command.Split(" ");
        if (parsed.Length < 2) return;

        var flagName = parsed[0];
        var BoolValue = parsed[1];

        var gameFlags = Enum.GetNames(typeof(GameFlag));
        var gameFlagValues = (GameFlag[])Enum.GetValues(typeof(GameFlag));
        var setFlag = new SetGameFlagCombo();
        foreach (var flag in gameFlags)
        {
            if (flag.ToLower().Equals(parsed[0].ToLower()))
            {
                setFlag.Flag = gameFlagValues[Array.IndexOf(gameFlags, flag)];
                if (parsed[1].ToLower().Equals("true"))
                {
                    setFlag.BoolValue = true;
                    GameManager.Instance.SetFlags(setFlag);
                }
                else
                {
                    setFlag.BoolValue = false;
                }
            }
        }

        if (parsed.Length < 3) return;
        var intValue = parsed[2];

        if (parsed.Length < 4) return;
        var stringValue = parsed[3];
    }
}
