using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

public abstract class ReqFlagBase : ScriptableObject
{
    public abstract GameFlag GetFlag();
    public abstract bool Result();
}

[CreateAssetMenu(menuName = "Kitnapper/RequiredFlag")]
[Serializable]
public class ReqFlag : ReqFlagBase
{
    //cannot use { get; set; } pattern, that makes them invisible from inspector.
    [JsonConverter(typeof(StringEnumConverter))]
    public GameFlag Flag;
    public bool BoolValue;
    public string StringValue;
    public int IntValue;
    // =, <, <=, >, >=
    public bool LessThanIntValue;
    public bool LessThanOrEqualIntValue;
    public bool MoreThanIntValue;
    public bool MoreThanOrEqualIntValue;

    public override GameFlag GetFlag() => Flag;

    public override bool Result()
    {
        var gameSettings = GameManager.Instance.CurrentGameSettings;
        var currentFlag = gameSettings.GameFlags.FirstOrDefault(x => x.Flag == Flag);
        //In case the flag does not exist in settings, check against default values
        currentFlag ??= new GameFlagCombo()
        {
            Flag = Flag
        };

        bool intValuePassed = false;
        if (LessThanIntValue && currentFlag.IntValue < IntValue) intValuePassed = true;
        else if (LessThanOrEqualIntValue && currentFlag.IntValue <= IntValue) intValuePassed = true;
        else if (MoreThanIntValue && currentFlag.IntValue > IntValue) intValuePassed = true;
        else if (MoreThanOrEqualIntValue && currentFlag.IntValue >= IntValue) intValuePassed = true;
        else if (currentFlag.IntValue == IntValue) intValuePassed = true;

        var stringValuePassed = false;
        if (string.IsNullOrEmpty(currentFlag.StringValue) && string.IsNullOrEmpty(StringValue)) stringValuePassed = true;
        else if (currentFlag.StringValue == StringValue) stringValuePassed = true;

        if (currentFlag.BoolValue != BoolValue
        || !stringValuePassed
        || !intValuePassed)
        {
            return false;
        }
        return true;
    }
}