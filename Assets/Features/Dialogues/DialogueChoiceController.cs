using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoiceController : MonoBehaviour
{
    public Text BorderText;
    public Text ChoiceText;
    public bool IsActive = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetChoiceText(string text)
    {
        ChoiceText.text = text;
    }

    public void SetAsActiveChoice()
    {
        IsActive = true;
        ChoiceText.fontStyle = FontStyle.Bold;
    }

    public void SetAsNotActiveChoice()
    {
        IsActive = false;
        ChoiceText.fontStyle = FontStyle.Normal;
    }
}
