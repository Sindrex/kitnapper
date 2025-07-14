using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class GameSettings
{
    public FullScreenMode FullScreenMode { get; set; } = FullScreenMode.FullScreenWindow;
    public float MasterVolume { get; set; } = 100;
    public List<GameFlagCombo> GameFlags { get; set; } = new List<GameFlagCombo>();
    public List<EventState> EventStates { get; set; } = new List<EventState>();
    public GamePosition PlayerPosition { get; set; } = new GamePosition();

    private GamePosition GetPlayerPosition() => new GamePosition
    {
        X = CameraController.Instance.Position.x,
        Y = CameraController.Instance.Position.y
    };

    public void Save()
    {
        PlayerPosition = GetPlayerPosition();
        GameSettingsLoader.Save(this);
    }

    public void ResetGame()
    {
        GameFlags = new List<GameFlagCombo>();
        EventStates = new List<EventState>();
        PlayerPosition = new GamePosition();
        GameSettingsLoader.Save(this);
    }
}

public class GamePosition
{
    public float X { get; set; } = 0;
    public float Y { get; set; } = 0;
    public float Z { get; set; } = -10;

    public Vector3 ToVector3() => new Vector3(X, Y, Z);
}

[Serializable]
public class GameFlagCombo
{
    //cannot use { get; set; } pattern, that makes them invisible from inspector.
    [JsonConverter(typeof(StringEnumConverter))]
    public GameFlag Flag;
    public bool BoolValue;
    public int IntValue;
    public string StringValue;
}

[Serializable]
public enum GameFlag
{
    Default,
    HelloWorldFlag,
    TestWorldFlag2
}