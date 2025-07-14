using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Serializable]
public class SetGameFlagCombo
{
    //cannot use { get; set; } pattern, that makes them invisible from inspector.
    [JsonConverter(typeof(StringEnumConverter))]
    public GameFlag Flag;
    public bool BoolValue;
    public string StringValue;
    public bool SetIntValueHasValue;
    public int SetIntValue;
    public bool AddIntValueHasValue;
    public int AddIntValue;
}