using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MeetAndTalk;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public DialogueContainerSO dialogueContainer;
    public BaseNodeData currentNode;
    public bool IsActive = false;
    public Text InteractText;
    public bool RequireInteract;
    public List<RequiredGameFlagCombo> RequiredFlags;

    // Start is called before the first frame update
    void Start()
    {
        currentNode = dialogueContainer.StartNodeDatas.FirstOrDefault();
        InteractText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive && CheckRequirements())
        {
            if (RequireInteract && !InputController.GetInput(InputPurpose.INTERACT))
            {
                return;
            }

            if (MyDialogueManager.Instance.IsBusySpawningLetters || MyDialogueManager.Instance.IsBusySpawningLettersDialogueChoices)
            {
                return;
            }

            //dialogue choice next node
            if (MyDialogueManager.Instance.IsDialogueChoice)
            {
                var currentChoiceIndex = MyDialogueManager.Instance.CurrentDialogueChoice;
                var dialogueChoiceNodeData = currentNode as DialogueChoiceNodeData;
                var currentChoice = dialogueChoiceNodeData.DialogueNodePorts[currentChoiceIndex];
                currentNode = GetNodeByGuid(currentChoice.InputGuid);
            }
            else
            {
                currentNode = GetNextNode(currentNode);
            }

            //Next node
            if (currentNode is DialogueNodeData dialogueNode)
            {
                MyDialogueManager.Instance.SetText(dialogueNode.TextType[0].LanguageGenericType);
                dialogueNode.EventController?.DoFinishRoutine();
            }
            else if (currentNode is EndNodeData endNode)
            {
                MyDialogueManager.Instance.CloseDialogue();
                currentNode = dialogueContainer.StartNodeDatas.FirstOrDefault();
            }
            else if (currentNode is DialogueChoiceNodeData choiceNodeData)
            {
                MyDialogueManager.Instance.LoadChoiceDialogue(choiceNodeData);
            }
            else
            {
                CLogger.Log($"Found unknown node type: {currentNode.GetType().Name}");
            }
        }
    }

    private bool CheckRequirements()
    {
        var passedRequirements = true;
        var requiresFlags = RequiredFlags.Any(x => x.Flag != GameFlag.Default);
        if (requiresFlags)
        {
            var gameSettings = GameManager.Instance.CurrentGameSettings;
            foreach (var requiredFlag in RequiredFlags)
            {
                var currentFlag = gameSettings.GameFlags.FirstOrDefault(x => x.Flag == requiredFlag.Flag);
                //In case the flag does not exist in settings, check against default values
                currentFlag ??= new GameFlagCombo()
                {
                    Flag = requiredFlag.Flag
                };

                bool intValuePassed = false;
                if (requiredFlag.LessThanIntValue && currentFlag.IntValue < requiredFlag.IntValue) intValuePassed = true;
                else if (requiredFlag.LessThanOrEqualIntValue && currentFlag.IntValue <= requiredFlag.IntValue) intValuePassed = true;
                else if (requiredFlag.MoreThanIntValue && currentFlag.IntValue > requiredFlag.IntValue) intValuePassed = true;
                else if (requiredFlag.MoreThanOrEqualIntValue && currentFlag.IntValue >= requiredFlag.IntValue) intValuePassed = true;
                else if (currentFlag.IntValue == requiredFlag.IntValue) intValuePassed = true;


                if (currentFlag.BoolValue != requiredFlag.BoolValue
                || currentFlag.StringValue != requiredFlag.StringValue
                || !intValuePassed)
                {
                    passedRequirements = false;
                }
            }
        }
        return passedRequirements;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //open for interact -> start dialogue
        if (!other.gameObject.CompareTag("Player")) return;
        if (CheckRequirements())
        {
            currentNode = dialogueContainer.StartNodeDatas.FirstOrDefault();
            InteractText.gameObject.SetActive(true);
            IsActive = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //close for interact -> start dialogue
        if (!other.gameObject.CompareTag("Player")) return;

        InteractText.gameObject.SetActive(false);
        IsActive = false;

        MyDialogueManager.Instance.CloseDialogue();
    }
    
    private BaseNodeData GetNodeByGuid(string _targetNodeGuid)
    {
        return dialogueContainer.AllNodes.Find(node => node.NodeGuid == _targetNodeGuid);
    }

    private BaseNodeData GetNodeByNodePort(DialogueNodePort _nodePort)
    {
        return dialogueContainer.AllNodes.Find(node => node.NodeGuid == _nodePort.InputGuid);
    }

    private BaseNodeData GetNextNode(BaseNodeData _baseNodeData)
    {
        var matchingLinks = dialogueContainer.NodeLinkDatas
            .Where(link => link.BaseNodeGuid == _baseNodeData.NodeGuid)
            .ToList();

        if (matchingLinks.Count == 0)
        {
            CLogger.Log($"Found {matchingLinks.Count} matching links!");
            return null;
        }

        //pick random link if there are multiple
        NodeLinkData selectedLink = matchingLinks.Count == 1
            ? matchingLinks[0]
            : matchingLinks[Random.Range(0, matchingLinks.Count)];

        return GetNodeByGuid(selectedLink.TargetNodeGuid);
    }
}
