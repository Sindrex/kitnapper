using System.Linq;
using UnityEngine;

public class ColorController : MonoBehaviour
{
    public GameObject ColorObject;
    public GameObject BWObject;

    public void FixedUpdate()
    {
        if (BWObject.activeSelf)
        {
            var gameSettings = GameManager.Instance.CurrentGameSettings;
            var currentFlag = gameSettings.GameFlags.FirstOrDefault(x => x.Flag == GameFlag.FinaleMomentDialogue);
            if (currentFlag != null)
            {
                ActivateColor();
            }
        }
    }

    public void ActivateColor()
    {
        BWObject.SetActive(false);
        ColorObject.SetActive(true);
    }
    
    public void ActivateBW()
    {
        BWObject.SetActive(true);
        ColorObject.SetActive(false);
    }
}

/*
<color=aqua>Aqua Text</color>
<color=black>Black Text</color>
<color=blue>Blue Text</color>
<color=brown>Brown Text</color>
<color=cyan>Cyan Text</color>
<color=darkblue>Dark Blue Text</color>
<color=fuchsia>Fuchsia Text</color>
<color=green>Green Text</color>
<color=grey>Grey Text</color>
<color=lightblue>Light Blue Text</color>
<color=lime>Lime Text</color>
<color=magenta>Magenta Text</color>
<color=maroon>Maroon Text</color>
<color=navy>Navy Text</color>
<color=olive>Olive Text</color>
<color=orange>Orange Text</color>
<color=purple>Purple Text</color>
<color=red>Red Text</color>
<color=silver>Silver Text</color>
<color=teal>Teal Text</color>
<color=white>White Text</color>
<color=yellow>Yellow Text</color>

<color=#RRGGBB>Text with RGB</color>
<color=#RRGGBBAA>Text with RGBA</color>
*/