using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueLogItemController : MonoBehaviour
{
    public Text MyText;

    public void Setup(string text)
    {
        MyText.text = text;
    }
}
