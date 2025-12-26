using UnityEngine;

public class EventBase : MonoBehaviour
{
    public virtual void Activate(bool activateNextEvent) { }
    public virtual bool CheckRequirements() { return true; }
}