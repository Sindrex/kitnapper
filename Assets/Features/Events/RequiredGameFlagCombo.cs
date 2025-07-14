using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Serializable]
public class RequiredGameFlagCombo
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
}