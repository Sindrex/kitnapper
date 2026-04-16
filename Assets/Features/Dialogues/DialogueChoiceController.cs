using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoiceController : MonoBehaviour
{
    public Text BorderText;
    public Text ChoiceText; //max 2 lines of 10 characters
    public bool IsActive = false;

    //Spawn letters one by one
    public bool IsBusySpawningLetters;
    public float TimeBetweenEachLetterSpawn;
    public float CurrentLetterSpawnTime;
    public List<char> CurrentTextList; //reversed

    //Controls hint paw
    public int Index; //index of this choice among the choices spawned
    public int MaxIndex;
    public bool IsFirstChoice => Index == 0;
    public bool IsLastChoice => Index == MaxIndex;
    public GameObject Paw;
    public GameObject PawW;
    public GameObject PawS;

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

        Paw.SetActive(false);
    }

    public void SetAsActiveChoice()
    {
        IsActive = true;
        ChoiceText.fontStyle = FontStyle.Bold;
        Paw.SetActive(true);
        PawW.SetActive(!IsFirstChoice);
        PawS.SetActive(!IsLastChoice);
    }

    public void SetAsNotActiveChoice()
    {
        IsActive = false;
        ChoiceText.fontStyle = FontStyle.Normal;
        Paw.SetActive(false);
    }
}
