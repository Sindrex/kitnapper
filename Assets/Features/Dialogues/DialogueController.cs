using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MeetAndTalk;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueContainerSO dialogueContainer;
    public BaseNodeData currentNode;
    public bool IsActive = false;
    public bool RequireInteract = false;
    public List<ReqFlagBase> RequiredFlags;
    public GameObject CameraTarget;

    private bool isSecondNode;

    // Start is called before the first frame update
    void Start()
    {
        currentNode = dialogueContainer.StartNodeDatas.FirstOrDefault();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            if ((RequireInteract || isSecondNode) && !InputController.GetInput(InputPurpose.INTERACT))
            {
                return;
            }

            //first node interacted with
            if (!isSecondNode && CameraTarget != null)
            {
                CameraController.Instance.SetPosition(CameraTarget.transform.position, true);
            }

            //skip text animation
            if (MyDialogueManager.Instance.IsBusySpawningLetters && MyDialogueManager.Instance.DialogueText.text.Length > 0)
            {
                MyDialogueManager.Instance.SkipTextAnimation();
                return;
            }

            if (MyDialogueManager.Instance.IsBusySpawningLettersDialogueChoices)
            {
                return;
            }

            PlayerController.Instance.StopShowInteractHint();
            GetNextNode();
            RunCurrentNode();
            isSecondNode = true;
        }
    }

    private void GetNextNode()
    {
        //dialogue choice next node
        if (MyDialogueManager.Instance.IsDialogueChoice)
        {
            var currentChoiceIndex = MyDialogueManager.Instance.CurrentDialogueChoice;
            var dialogueChoiceNodeData = currentNode as DialogueChoiceNodeData;
            //this has to take into account that sometimes not all choices are displayed due to requirements
            var activeDialogueNodePorts = dialogueChoiceNodeData.DialogueNodePorts.Where(e =>
                e.RequiredFlag == null ||
                (e.RequiredFlag != null && e.RequiredFlag.Result())
            ).ToList();
            var currentChoice = activeDialogueNodePorts[currentChoiceIndex];
            currentNode = GetNodeByGuid(currentChoice.InputGuid);
        }
        else //other nodes
        {
            currentNode = GetNextNode(currentNode);
        }
    }

    private void RunCurrentNode()
    {
        //Run whatever current node does
        if (currentNode is DialogueNodeData dialogueNode)
        {
            MyDialogueManager.Instance.SetText(dialogueNode.TextType[0].LanguageGenericType);
            GameManager.Instance.FindEvent(dialogueNode.Event)?.Activate(true);
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
        else if (currentNode is ConditionalNodeData conditionalNodeData)
        {
            var matchingLinks = dialogueContainer.NodeLinkDatas
                .Where(link => link.BaseNodeGuid == conditionalNodeData.NodeGuid)
                .ToList();
            var result = conditionalNodeData.RequiredGameFlagCombo.Result();
            if (result)
            {
                var trueOutput = matchingLinks[0].TargetNodeGuid;
                currentNode = GetNodeByGuid(trueOutput);
            }
            else
            {
                var falseOutput = matchingLinks[1].TargetNodeGuid;
                currentNode = GetNodeByGuid(falseOutput);
            }
            RunCurrentNode(); //rerun new node
        }
        else if (currentNode is null)
        {
            CLogger.LogError($"currentNode is null!");
        }
        else
        {
            CLogger.Log($"Found unknown node type: {currentNode.GetType().Name}");
        }
    }

    private bool CheckRequirements()
    {
        var passedRequirements = true;
        var requiresFlags = RequiredFlags.Any(x => x.GetFlag() != GameFlag.Default);
        if (requiresFlags)
        {
            var gameSettings = GameManager.Instance.CurrentGameSettings;
            var results = new List<bool>();
            foreach (var requiredFlag in RequiredFlags)
            {
                results.Add(requiredFlag.Result());
            }

            if (results.Any(e => e == false))
            {
                passedRequirements = false;
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
            CLogger.Log("Setting Dialogue to Active");
            if (!RequireInteract)
            {
                CLogger.Log("Stopping player");
                PlayerController.Instance.CanMove = false;
            }

            currentNode = dialogueContainer.StartNodeDatas.FirstOrDefault();
            PlayerController.Instance.ShowInteractHint();
            IsActive = true;
            isSecondNode = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //close for interact -> start dialogue
        if (!other.gameObject.CompareTag("Player")) return;

        if (IsActive)
        {
            PlayerController.Instance.StopShowInteractHint();
            IsActive = false;
        }
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
        
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        string customName = "DialogueGizmo.png";
        Gizmos.DrawIcon(transform.position, customName, true);
    }
#endif
}
