using System.Linq;
using ASCII.Util;
using UnityEngine;

public class DialogueLogManager : MonoBehaviour
{
    public GameObject Parent;
    public GameObject ItemParent;
    public GameObject ItemPrefab;
    public int Offset;
    public int Max = 9;
    public bool IsOpen;
    public bool IsHidden;
    public GameObject TopButtons;

    //singleton
    public static DialogueLogManager Instance { get; private set; }
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
        Parent.SetActive(false);
        IsOpen = false;
        IsHidden = true;
        TopButtons.SetActive(false);
        ItemParent.DestroyMyChildren();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputController.GetInput(InputPurpose.DIALOGUE_LOG_OPEN) && !IsOpen)
        {
            Parent.SetActive(true);
            Offset = 0;
            LoadDialogueLog();
            IsOpen = true;
            PlayerController.Instance.CanMove = false;
        }
        else if (InputController.GetInput(InputPurpose.DIALOGUE_LOG_OPEN) && IsOpen)
        {
            Parent.SetActive(false);
            IsOpen = false;
            PlayerController.Instance.CanMove = true;
        }

        if (IsOpen)
        {
            if (InputController.GetInput(InputPurpose.DIALOGUE_CHOICE_UP))
            {
                Offset++;
                var dialogueLog = GameManager.Instance.CurrentGameSettings.DialogueLog;
                if(Offset >= dialogueLog.Count - Max)
                {
                    Offset = dialogueLog.Count - Max;
                }
                if(Offset < 0)
                {
                    Offset = 0;
                }
                LoadDialogueLog();
            }
            else if (InputController.GetInput(InputPurpose.DIALOGUE_CHOICE_DOWN))
            {
                Offset--;
                if(Offset < 0)
                {
                    Offset = 0;
                }
                LoadDialogueLog();
            }
        }

        if (IsHidden && TopButtons.activeSelf)
        {
            TopButtons.SetActive(false);
        }
        else if (!IsHidden && !TopButtons.activeSelf)
        {
            TopButtons.SetActive(true);
        }
    }

    private void LoadDialogueLog()
    {
        ItemParent.DestroyMyChildren();
        var dialogueLog = GameManager.Instance.CurrentGameSettings.DialogueLog;
        var dialoguesToShow = dialogueLog.TakeLast(Max + Offset).ToList();
        for(int i = 0; i < dialoguesToShow.Count - Offset; i++)
        {
            var item = Instantiate(ItemPrefab, ItemParent.transform);
            var dialogLogItemController = item.GetComponent<DialogueLogItemController>();
            dialogLogItemController.Setup(dialoguesToShow[i]);
        }
    }
}
