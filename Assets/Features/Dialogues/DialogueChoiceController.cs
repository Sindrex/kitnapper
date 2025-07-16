using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoiceController : MonoBehaviour
{
    public Text BorderText;
    public Text ChoiceText;
    public bool IsActive = false;

    //Spawn letters one by one
    public bool IsBusySpawningLetters;
    public float TimeBetweenEachLetterSpawn;
    public float CurrentLetterSpawnTime;
    public List<char> CurrentTextList; //reversed

    void FixedUpdate()
    {
        if (IsBusySpawningLetters)
        {
            if (CurrentLetterSpawnTime >= TimeBetweenEachLetterSpawn)
            {
                if (CurrentTextList.Count != 0)
                {
                    var currentLetter = CurrentTextList.Last();
                    CurrentTextList.RemoveAt(CurrentTextList.Count - 1);
                    ChoiceText.text += currentLetter;

                    CurrentLetterSpawnTime = 0;
                }
                else
                {
                    IsBusySpawningLetters = false;
                }
            }
            else
            {
                CurrentLetterSpawnTime += Time.deltaTime;
            }
        }
    }

    public void SetChoiceText(string text)
    {
        ChoiceText.text = "";
        CurrentTextList = text.ToCharArray().Reverse().ToList();
        IsBusySpawningLetters = true;
        CurrentLetterSpawnTime = 0;
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
