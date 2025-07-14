using System.Collections.Generic;

public class EventState
{
    public string Id;
    public bool IsFinished;
    public List<bool> TargetObjectsIsActive = new List<bool>();
}