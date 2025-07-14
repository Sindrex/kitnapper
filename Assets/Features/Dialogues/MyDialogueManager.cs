using System.Collections;
using System.Collections.Generic;
using ASCII.Util;
using MeetAndTalk;
using UnityEngine;
using UnityEngine.UI;

public class MyDialogueManager : MonoBehaviour
{
    public GameObject DialogueParent;
    public Text DialogueText;

    //Dialogue choice
    public GameObject DialogueChoiceContentParent;
    public GameObject DialogueChoicePrefab;
    public List<DialogueChoiceController> ChoiceControllers = new();
    public int CurrentDialogueChoice;
    public bool IsDialogueChoice;

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
            if (Input.GetKeyDown(KeyCode.S)) //list is backwards
            {
                CurrentDialogueChoice++;
                if (CurrentDialogueChoice >= ChoiceControllers.Count)
                {
                    CurrentDialogueChoice = ChoiceControllers.Count - 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.W))
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

    public void SetText(string text)
    {
        CameraController.Instance.CanMove = false;
        DialogueParent.SetActive(true);
        DialogueText.text = text;
    }

    public void LoadChoiceDialogue(DialogueChoiceNodeData choiceNodeData)
    {
        CameraController.Instance.CanMove = false;
        CurrentDialogueChoice = 0;
        IsDialogueChoice = true;
        DialogueParent.SetActive(true);
        DialogueChoiceContentParent.DestroyMyChildren();

        DialogueText.text = choiceNodeData.TextType[0].LanguageGenericType;

        ChoiceControllers = new();
        var choices = choiceNodeData.DialogueNodePorts;
        foreach (var choice in choices)
        {
            var gameObject = Instantiate(DialogueChoicePrefab, DialogueChoiceContentParent.transform);
            var choiceController = gameObject.GetComponent<DialogueChoiceController>();
            choiceController.SetChoiceText(choice.TextLanguage[0].LanguageGenericType);
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
