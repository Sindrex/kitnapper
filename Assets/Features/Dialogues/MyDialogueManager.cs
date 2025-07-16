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

    //Spawn letters one by one
    public bool IsBusySpawningLettersDialogueChoices => ChoiceControllers.Any(e => e.IsBusySpawningLetters);
    public bool IsBusySpawningLetters;
    public float TimeBetweenEachLetterSpawn;
    public float CurrentLetterSpawnTime;
    public List<char> CurrentTextList; //reversed

    //Dialogue choice
    public GameObject DialogueChoiceContentParent;
    public GameObject DialogueChoicePrefab;
    public List<DialogueChoiceController> ChoiceControllers = new();
    public int CurrentDialogueChoice;
    public bool IsDialogueChoice;
    public List<string> DialogueChoiceTexts;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDialogueChoice)
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
                }
                else
                {
                    IsBusySpawningLetters = false;
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

    public void SetText(string text)
    {
        CameraController.Instance.CanMove = false;
        DialogueChoiceContentParent.DestroyMyChildren();
        IsDialogueChoice = false;
        DialogueParent.SetActive(true);
        DialogueText.text = "";

        CurrentTextList = text.ToCharArray().Reverse().ToList();
        IsBusySpawningLetters = true;
        CurrentLetterSpawnTime = 0;
    }

    public void LoadChoiceDialogue(DialogueChoiceNodeData choiceNodeData)
    {
        CameraController.Instance.CanMove = false;
        CurrentDialogueChoice = 0;
        IsDialogueChoice = true;
        DialogueParent.SetActive(true);
        DialogueChoiceContentParent.DestroyMyChildren();

        DialogueText.text = "";
        var text = choiceNodeData.TextType[0].LanguageGenericType;
        CurrentTextList = text.ToCharArray().Reverse().ToList();
        IsBusySpawningLetters = true;
        CurrentLetterSpawnTime = 0;

        DialogueChoiceTexts = new List<string>();
        foreach (var choice in choiceNodeData.DialogueNodePorts)
        {
            DialogueChoiceTexts.Add(choice.TextLanguage[0].LanguageGenericType);
        }
    }

    private void LoadCurrentChoiceDialogueChoices()
    {
        ChoiceControllers = new();
        foreach (var choice in DialogueChoiceTexts)
        {
            var gameObject = Instantiate(DialogueChoicePrefab, DialogueChoiceContentParent.transform);
            var choiceController = gameObject.GetComponent<DialogueChoiceController>();
            choiceController.SetChoiceText(choice);
            ChoiceControllers.Add(choiceController);
        }

        ChoiceControllers[0].SetAsActiveChoice();
    }

    public void CloseDialogue()
    {
        DialogueParent.SetActive(false);
        DialogueChoiceContentParent.DestroyMyChildren();
        IsDialogueChoice = false;
        CameraController.Instance.CanMove = true;
    }
}
