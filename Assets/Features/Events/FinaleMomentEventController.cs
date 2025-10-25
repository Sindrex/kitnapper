using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FinaleMomentEventController : EventBase
{
    public string Id;

    // Start is called before the first frame update
    public void Setup()
    {
        Id = gameObject.name;
    }

    public override void Activate(bool activateNextEvent)
    {
        CLogger.Log($"Event \"{Id}\" activated!");
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        string customName = "EventGizmo.png";
        Gizmos.DrawIcon(transform.position, customName, true);
    }
#endif
}
