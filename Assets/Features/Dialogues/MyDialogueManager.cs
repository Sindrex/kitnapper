using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ASCII.Util;
using MeetAndTalk;
using UnityEngine;
using UnityEngine.UI;

public class MyDialogueManager : MonoBehaviour
{
    public GameObject DialogueParent;
    public Text DialogueText;
    public bool IsDialogue;

    //Spawn letters one by one
    public bool IsBusySpawningLettersDialogueChoices => ChoiceControllers.Any(e => e.IsBusySpawningLetters);
    public bool IsBusySpawningLetters;
    public float TimeBetweenEachLetterSpawn;
    public float CurrentLetterSpawnTime;
    public string CurrentText;
    public List<char> CurrentTextList; //reversed

    //Dialogue choice
    public GameObject DialogueChoiceContentParent;
    public GameObject DialogueChoicePrefab;
    public List<DialogueChoiceController> ChoiceControllers = new();
    public int CurrentDialogueChoice;
    public bool IsDialogueChoice;
    public List<string> DialogueChoiceTexts;
    public List<ReqFlagBase> DialogueChoiceRequiredFlags;

    //SFX
    private readonly List<AudioLabel> DialogueSFX = new List<AudioLabel>()
    {
        AudioLabel.DialogueSFX1, AudioLabel.DialogueSFX2
    };

    //singleton
    public static MyDialogueManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        DialogueParent.SetActive(false);
        IsDialogue = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDialogueChoice && !IsBusySpawningLetters)
        {
            var changed = CurrentDialogueChoice;
            if (InputController.GetInput(InputPurpose.DIALOGUE_CHOICE_DOWN)) //list is backwards
            {
                CurrentDialogueChoice++;
                if (CurrentDialogueChoice >= ChoiceControllers.Count)
                {
                    CurrentDialogueChoice = ChoiceControllers.Count - 1;
                }
            }
            else if (InputController.GetInput(InputPurpose.DIALOGUE_CHOICE_UP))
            {
                CurrentDialogueChoice--;
                if (CurrentDialogueChoice < 0)
                {
                    CurrentDialogueChoice = 0;
                }
            }

            if (changed != CurrentDialogueChoice)
            {
                foreach (var choice in ChoiceControllers)
                {
                    choice.SetAsNotActiveChoice();
                }
                ChoiceControllers[CurrentDialogueChoice].SetAsActiveChoice();
                AudioManager.Instance.PlaySFXClip(AudioLabel.SwapSelectSFX);
            }
        }
    }

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
                    DialogueText.text += currentLetter;
                    CurrentLetterSpawnTime = 0;

                    //Dialogue SFX
                    var randomNumber = new System.Random().Next(0, DialogueSFX.Count - 1);
                    var sfxClip = DialogueSFX[randomNumber];
                    AudioManager.Instance?.PlaySFXClip(sfxClip);
                }
                else
                {
                    IsBusySpawningLetters = false;
                    PlayerController.Instance.ShowInteractHint();
                    if (IsDialogueChoice)
                    {
                        LoadCurrentChoiceDialogueChoices();
                    }
                }
            }
            else
            {
                CurrentLetterSpawnTime += Time.deltaTime;
            }
        }
    }

    public void SkipTextAnimation()
    {
        DialogueText.text = CurrentText;
        CurrentTextList = new List<char>();       
    }

    public void SetText(string text)
    {
        CLogger.Log("Setting text");
        IsDialogue = true;
        PlayerController.Instance.CanMove = false;
        DialogueChoiceContentParent.DestroyMyChildren();
        IsDialogueChoice = false;
        DialogueParent.SetActive(true);
        DialogueText.text = "";

        CurrentText = text;
        CurrentTextList = text.ToCharArray().Reverse().ToList();
        IsBusySpawningLetters = true;
        CurrentLetterSpawnTime = 0;

        //save in GameSettings
        GameManager.Instance.CurrentGameSettings.DialogueLog.Add(text);
        GameManager.Instance.CurrentGameSettings.Save();
    }

    public void LoadChoiceDialogue(DialogueChoiceNodeData choiceNodeData)
    {
        CLogger.Log("Loading Choice Dialogue");
        IsDialogue = true;
        PlayerController.Instance.CanMove = false;
        CurrentDialogueChoice = 0;
        IsDialogueChoice = true;
        DialogueParent.SetActive(true);
        DialogueChoiceContentParent.DestroyMyChildren();

        DialogueText.text = "";
        var text = choiceNodeData.TextType[0].LanguageGenericType;
        CurrentText = text;
        CurrentTextList = text.ToCharArray().Reverse().ToList();
        IsBusySpawningLetters = true;
        CurrentLetterSpawnTime = 0;

        DialogueChoiceTexts = new List<string>();
        DialogueChoiceRequiredFlags = new List<ReqFlagBase>();
        foreach (var choice in choiceNodeData.DialogueNodePorts)
        {
            DialogueChoiceTexts.Add(choice.TextLanguage[0].LanguageGenericType);
            DialogueChoiceRequiredFlags.Add(choice.RequiredFlag);
        }

        //save in GameSettings
        GameManager.Instance.CurrentGameSettings.DialogueLog.Add(text);
        GameManager.Instance.CurrentGameSettings.Save();
    }

    private void LoadCurrentChoiceDialogueChoices()
    {
        ChoiceControllers = new();
        for (int i = 0; i < DialogueChoiceTexts.Count; i++)
        {
            if (DialogueChoiceRequiredFlags[i] != null && DialogueChoiceRequiredFlags[i].Result())
            {
                var choice = DialogueChoiceTexts[i];
                var gameObject = Instantiate(DialogueChoicePrefab, DialogueChoiceContentParent.transform);
                var choiceController = gameObject.GetComponent<DialogueChoiceController>();
                choiceController.SetChoiceText(choice);
                ChoiceControllers.Add(choiceController);
            }
            else if (DialogueChoiceRequiredFlags[i] == null) //no requirement for choice to show up
            {
                var choice = DialogueChoiceTexts[i];
                var gameObject = Instantiate(DialogueChoicePrefab, DialogueChoiceContentParent.transform);
                var choiceController = gameObject.GetComponent<DialogueChoiceController>();
                choiceController.SetChoiceText(choice);
                ChoiceControllers.Add(choiceController);
            }
        }

        ChoiceControllers[0].SetAsActiveChoice();
    }

    public void CloseDialogue()
    {
        CLogger.Log("Closing Dialogue");
        IsDialogue = false;
        DialogueParent.SetActive(false);
        DialogueChoiceContentParent.DestroyMyChildren();
        IsDialogueChoice = false;
        PlayerController.Instance.CanMove = true;
        CameraController.Instance.FollowPlayer = true;
    }
}
