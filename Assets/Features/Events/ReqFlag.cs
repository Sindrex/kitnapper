using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

[CreateAssetMenu(menuName = "Kitnapper/RequiredFlag")]
[Serializable]
public class ReqFlag : ScriptableObject
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

    public bool Result()
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

        if (currentFlag.BoolValue != BoolValue
        || currentFlag.StringValue != StringValue
        || !intValuePassed)
        {
            return false;
        }
        return true;
    }
}