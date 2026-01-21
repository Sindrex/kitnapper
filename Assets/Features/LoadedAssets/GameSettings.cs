using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class GameSettings
{
    public FullScreenMode FullScreenMode { get; set; } = FullScreenMode.FullScreenWindow;
    public int MasterVolumeIndex { get; set; }
    public bool FirstTimeBoot { get; set; } = true;
    public GamePosition PlayerPosition { get; set; } = new GamePosition();
    public List<GameFlagCombo> GameFlags { get; set; } = new List<GameFlagCombo>();
    public List<EventState> EventStates { get; set; } = new List<EventState>();
    public List<AudioLabel> UnlockedSongs { get; set; } = new List<AudioLabel>();
    public List<string> DialogueLog { get; set; } = new List<string>();

    private GamePosition GetPlayerPosition() => new GamePosition
    {
        X = PlayerController.Instance.Position.x,
        Y = PlayerController.Instance.Position.y
    };

    public void Save()
    {
        PlayerPosition = GetPlayerPosition();
        GameSettingsLoader.Save(this);
    }

    public void SaveFromMenu()
    {
        GameSettingsLoader.Save(this);
    }

    public void ResetGame()
    {
        GameFlags = new List<GameFlagCombo>();
        EventStates = new List<EventState>();
        PlayerPosition = new GamePosition();
        UnlockedSongs = new List<AudioLabel>();
        DialogueLog = new List<string>();
        GameSettingsLoader.Save(this);
    }
}

public class GamePosition
{
    public float X { get; set; } = 0;
    public float Y { get; set; } = 0;
    public float Z { get; set; } = 0;

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
    TestWorldFlag2,
    SleepingCat,
    PlayerHouseTelephoneMayor,
    MayorInitialDialogue,
    ScientistInitialDialogue,
    ScientistButterflyEvent1,
    FloristInitialDialogue,
    LunaMidnightRoseDialogue1,
    LunaFloristMomentDialogue1,
    RichardMidnightRoseDialogue1,
    RichardInitialDialogue,
    FindusInitialDialogue,
    FindusMiloDialogue,
    FindusSinbadDialogue1,
    SinbadInitialDialogue,
    SinbadFindusMomentToyFoundEvent,
    SinbadFindusMomentDialogue1,
    FindusRichardMomentDialogue1,
    MayorShrineActivated,
    MayorShrineDialogue1,
    FinaleMomentDialogue,
    FinaleMoment,
    SindrexNameChange,
    SindrexFeedbackYes,
    MilkcatNameKnown, KiraNameKnown, PusurNameKnown, PadagastNameKnown, SarumanNameKnown, MarriNameKnown, OnyxNameKnown, CatnipDealerNameKnown, CatsuneMikuNameKnown,
    ManekinekoNameKnown, MilkcatGuardNameKnown, Musician1NameKnown, Musician2NameKnown,
    LongSFXIsPlaying
}