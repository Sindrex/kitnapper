using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    // Button Select State machine
    public int SelectedRowButton = 0;
    public int MaxSelectedRowButton = 3;
    public int MinSelectedRowButton = 0;
    public int SelectedColumnButton = 0;
    public int MaxSelectedColumnButton = 2;
    public int MinSelectedColumnButton = -2;
    public int SelectedRowWithColumns = 3;

    // Buttons
    public Text ContinueText;
    public Text NewGameText;
    public Text QuitText;
    public Text CreditsText;
    public Text WindowedText;
    public Text SoundText;
    public Text MusicCreditsText;
    public Text VersionText;
    private Dictionary<Text, Action> AllTexts;

    //Game settings
    public GameSettings CurrentGameSettings;

    //Overwrite
    public GameObject MainButtons;
    public GameObject NewGameOverwrite;
    public bool IsOnYes;
    public Text YesText;
    public Text NoText;

    //Intro keybind tutorial
    public GameObject IntroKeys;
    public Text EscapeText;
    public Text WText;
    public Text AText;
    public Text SText;
    public Text DText;
    public Text EText;
    public Text RText;
    public int KeysClicked = 0;
    public float IntroKeysWaitForSeconds = 1;
    public GameObject MainView;

    //Settings
    public Text FullScreenModeText;
    public GameObject MusicSliderParent;
    public GameObject SFXSliderParent;
    public int MaxIndex = 9;
    public int MinIndex = 0;
    public string Prefix = "[";
    public string Postfix = "]";
    public string SoundLetter = "+";
    public string FillLetter = "¨";
    public Text MusicSliderText;
    public Text MusicSliderLabel;
    public Text SFXSliderText;
    public Text SFXSliderLabel;
    public int MusicCurrentIndex;
    public int SFXCurrentIndex;
    public bool MusicSelected;
    public Vector3 MusicPawPosition;
    public Vector3 SFXPawPosition;
    private const string MusicVolumeParameterName = "MusicVolume";
    private const string SFXVolumeParameterName = "SFXVolume";

    //paw
    public GameObject Paw;
    public Vector3 PawPosition;

    //Load game anim
    public Text TitleText;
    public List<string> TitleAnimTexts;
    public float TitleAnimWaitSeconds;

    //singleton
    public static MenuManager Instance { get; private set; }
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
        AllTexts = new Dictionary<Text, Action>
        {
            { ContinueText, ContinueGame },
            { NewGameText, NewGame },
            { QuitText, Quit },
            { CreditsText, Credits },
            { WindowedText, WindowedSetting },
            { SoundText, SoundSetting },
            { MusicCreditsText, MusicCredits },
            { VersionText, Version }
        };

        //Setup
        NewGameOverwrite.SetActive(false);
        MainButtons.SetActive(false);
        MainView.SetActive(false);
        Paw.SetActive(false);
        IntroKeys.SetActive(true);
        MusicSliderParent.SetActive(false);
        SFXSliderParent.SetActive(false);

        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        FullScreenModeText.text = "FullScreenWindow";

        //load GameSettings
        CLogger.Log("Loading GameSettings.");
        if (!GameSettingsLoader.GameSettingsExist())
        {
            CLogger.Log("Found no GameSettings file!");
            ContinueText.color = Color.black;
            SelectedRowButton = 1;
        }
        else
        {
            CurrentGameSettings = GameSettingsLoader.Load();
            Screen.fullScreenMode = CurrentGameSettings.FullScreenMode;
            FullScreenModeText.text = GetFullScreenModeName(Screen.fullScreenMode);
            MusicCurrentIndex = CurrentGameSettings.MusicVolumeIndex;
            SFXCurrentIndex = CurrentGameSettings.MusicVolumeIndex;
            SetMusicVolumeSlider(false);
            SetSFXVolumeSlider(false);
        }

        InputController.InputEnabled = true;

        //cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //menu music
        AudioManager.Instance.PlayMusicClip(AudioLabel.MenuMusic, false, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (MainButtons.activeSelf)
        {
            UpdateMainButtons();
        }
        else if (NewGameOverwrite.activeSelf)
        {
            UpdateNewGameOverwriteButtons();
        }
        else if (IntroKeys.activeSelf)
        {
            if (CurrentGameSettings.FirstTimeBoot)
            {
                UpdateIntroKeys();
            }
            else
            {
                MainButtons.SetActive(true);
                MainView.SetActive(true);
                Paw.SetActive(true);
                IntroKeys.SetActive(false);
            }
        }
    }

    public void UpdateIntroKeys()
    {
        if (InputController.GetInput(InputPurpose.QUIT))
        {
            if (EscapeText.fontStyle != FontStyle.Bold)
            {
                EscapeText.fontStyle = FontStyle.Bold;
                EscapeText.text = $"<color=\"Green\">{EscapeText.text}</color>";
                KeysClicked++;
            }
        }
        if (InputController.GetInput(InputPurpose.MOVE_UP))
        {
            if (WText.fontStyle != FontStyle.Bold)
            {
                WText.fontStyle = FontStyle.Bold;
                WText.text = $"<color=\"Green\">{WText.text}</color>";
                KeysClicked++;
            }
        }
        if (InputController.GetInput(InputPurpose.MOVE_DOWN))
        {
            if (SText.fontStyle != FontStyle.Bold)
            {
                SText.fontStyle = FontStyle.Bold;
                SText.text = $"<color=\"Green\">{SText.text}</color>";
                KeysClicked++;
            }
        }
        if (InputController.GetInput(InputPurpose.MOVE_LEFT))
        {
            if (AText.fontStyle != FontStyle.Bold)
            {
                AText.fontStyle = FontStyle.Bold;
                AText.text = $"<color=\"Green\">{AText.text}</color>";
                KeysClicked++;
            }
        }
        if (InputController.GetInput(InputPurpose.MOVE_RIGHT))
        {
            if (DText.fontStyle != FontStyle.Bold)
            {
                DText.fontStyle = FontStyle.Bold;
                DText.text = $"<color=\"Green\">{DText.text}</color>";
                KeysClicked++;
            }
        }
        if (InputController.GetInput(InputPurpose.INTERACT))
        {
            if (EText.fontStyle != FontStyle.Bold)
            {
                EText.fontStyle = FontStyle.Bold;
                EText.text = $"<color=\"Green\">{EText.text}</color>";
                KeysClicked++;
            }
        }
        if (InputController.GetInput(InputPurpose.DIALOGUE_LOG_OPEN))
        {
            if (RText.fontStyle != FontStyle.Bold)
            {
                RText.fontStyle = FontStyle.Bold;
                RText.text = $"<color=\"Green\">{RText.text}</color>";
                KeysClicked++;
            }
        }

        if (KeysClicked == 7)
        {
            StartCoroutine(WaitForIntroKeysFinished());
        }
    }

    IEnumerator WaitForIntroKeysFinished()
    {
        yield return new WaitForSeconds(IntroKeysWaitForSeconds);
        MainView.SetActive(true);
        MainButtons.SetActive(true);
        Paw.SetActive(true);
        IntroKeys.SetActive(false);
        CurrentGameSettings.FirstTimeBoot = false;
        CurrentGameSettings.SaveFromMenu();
    }

    public void UpdateNewGameOverwriteButtons()
    {
        if (IsOnYes && InputController.GetInput(InputPurpose.INTERACT))
        {
            CurrentGameSettings.ResetGame();
            StartCoroutine(StartGame());
        }
        else if (!IsOnYes && InputController.GetInput(InputPurpose.INTERACT))
        {
            NewGameOverwrite.SetActive(false);
            MainButtons.SetActive(true);
        }

        if (InputController.GetInput(InputPurpose.MENU_CHOICE_UP))
        {
            IsOnYes = true;
            YesText.fontStyle = FontStyle.Bold;
            NoText.fontStyle = FontStyle.Normal;
            Paw.transform.localPosition = YesText.transform.localPosition + PawPosition;
            AudioManager.Instance.PlaySFXClip(AudioLabel.SwapSelectSFX);
        }
        else if (InputController.GetInput(InputPurpose.MENU_CHOICE_DOWN))
        {
            IsOnYes = false;
            YesText.fontStyle = FontStyle.Normal;
            NoText.fontStyle = FontStyle.Bold;
            Paw.transform.localPosition = NoText.transform.localPosition + PawPosition;
            AudioManager.Instance.PlaySFXClip(AudioLabel.SwapSelectSFX);
        }
    }

    public void UpdateMainButtons()
    {
        if (MusicSliderParent.activeSelf || SFXSliderParent.activeSelf) //always on together but whatever
        {
            SetSoundSetting();
            return;
        }

        if (InputController.GetInput(InputPurpose.MENU_CHOICE_UP))
        {
            SelectedRowButton--;
            SelectedColumnButton = 0;
            if (CurrentGameSettings == null && SelectedRowButton < 1)
            {
                SelectedRowButton = 1;
            }
            if (SelectedRowButton < MinSelectedRowButton)
            {
                SelectedRowButton = MinSelectedRowButton;
            }
            AudioManager.Instance.PlaySFXClip(AudioLabel.SwapSelectSFX);
        }
        else if (InputController.GetInput(InputPurpose.MENU_CHOICE_DOWN))
        {
            SelectedRowButton++;
            SelectedColumnButton = 0;
            if (SelectedRowButton > MaxSelectedRowButton)
            {
                SelectedRowButton = MaxSelectedRowButton;
            }
            AudioManager.Instance.PlaySFXClip(AudioLabel.SwapSelectSFX);
        }
        else if (InputController.GetInput(InputPurpose.MENU_CHOICE_LEFT))
        {
            if (SelectedRowButton == SelectedRowWithColumns)
            {
                SelectedColumnButton--;
                if (SelectedColumnButton < MinSelectedColumnButton)
                {
                    SelectedColumnButton = MinSelectedColumnButton;
                }
                AudioManager.Instance.PlaySFXClip(AudioLabel.SwapSelectSFX);
            }
        }
        else if (InputController.GetInput(InputPurpose.MENU_CHOICE_RIGHT))
        {
            if (SelectedRowButton == SelectedRowWithColumns)
            {
                SelectedColumnButton++;
                if (SelectedColumnButton > MaxSelectedColumnButton)
                {
                    SelectedColumnButton = MaxSelectedColumnButton;
                }
                AudioManager.Instance.PlaySFXClip(AudioLabel.SwapSelectSFX);
            }
        }
        else if (InputController.GetInput(InputPurpose.INTERACT))
        {
            var selectedText = GetSelectedText();
            selectedText.Value();
            return;
        }

        UpdateSelectedButton();
    }

    public void ContinueGame()
    {
        //check if player has gamesettings
        //if not, this is disabled
        //else start game

        AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
        CLogger.Log("Continuing game!");
        if (CurrentGameSettings != null)
        {
            StartCoroutine(StartGame());
        }
        else
        {
            CLogger.LogError("Player requested to Continue game, but there is no GameSettings!");
        }
    }

    public void NewGame()
    {
        //Check if player already has gamesettings
        //if yes, open overwrite pane
        //else start game

        AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
        CLogger.Log("Starting new game!");
        if (CurrentGameSettings != null)
        {
            //open overwrite pane
            NewGameOverwrite.SetActive(true);
            MainButtons.SetActive(false);
            IsOnYes = false;
            NoText.fontStyle = FontStyle.Bold;
            Paw.transform.localPosition = NoText.transform.localPosition + PawPosition;
        }
        else
        {
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        //load game animation
        foreach(var text in TitleAnimTexts)
        {
            TitleText.text = text;
            yield return new WaitForSeconds(TitleAnimWaitSeconds);
        }
        SceneManager.LoadScene(1); //Game
    }

    public void Quit()
    {
        CLogger.Log("Quitting game!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Credits()
    {
        AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
        Application.OpenURL("https://sindrex.com/");
    }

    public void WindowedSetting()
    {
        AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
        if (CurrentGameSettings.FullScreenMode == FullScreenMode.FullScreenWindow)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            CurrentGameSettings.FullScreenMode = FullScreenMode.Windowed;
            CurrentGameSettings.SaveFromMenu();
            FullScreenModeText.text = GetFullScreenModeName(FullScreenMode.Windowed);
        }
        else if (CurrentGameSettings.FullScreenMode == FullScreenMode.Windowed)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            CurrentGameSettings.FullScreenMode = FullScreenMode.FullScreenWindow;
            CurrentGameSettings.SaveFromMenu();
            FullScreenModeText.text = GetFullScreenModeName(FullScreenMode.FullScreenWindow);
        }
    }

    private string GetFullScreenModeName(FullScreenMode mode)
    {
        switch (mode)
        {
            case FullScreenMode.FullScreenWindow:
                return "FullScreen Window";
            case FullScreenMode.Windowed:
                return "Windowed";
        }
        return ":)?";
    }

    public void SoundSetting()
    {
        AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
        MusicSliderParent.SetActive(true);
        SFXSliderParent.SetActive(true);
        MusicSelected = true;
        Paw.transform.localPosition = MusicPawPosition;
        MusicSliderText.fontStyle = FontStyle.Bold;
        MusicSliderLabel.fontStyle = FontStyle.Bold;
        SFXSliderText.fontStyle = FontStyle.Normal;
        SFXSliderLabel.fontStyle = FontStyle.Normal;
    }

    public void SetSoundSetting()
    {
        if (MusicSelected)
        {
            if (InputController.GetInput(InputPurpose.MENU_CHOICE_LEFT))
            {
                MusicCurrentIndex--;
                if (MusicCurrentIndex < MinIndex)
                {
                    MusicCurrentIndex++;
                }
                SetMusicVolumeSlider(true);
                AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
            }
            else if (InputController.GetInput(InputPurpose.MENU_CHOICE_RIGHT))
            {
                MusicCurrentIndex++;
                if (MusicCurrentIndex > MaxIndex)
                {
                    MusicCurrentIndex--;
                }
                SetMusicVolumeSlider(true);
                AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
            }
            else if (InputController.GetInput(InputPurpose.MENU_CHOICE_UP))
            {
                MusicSelected = false;
                Paw.transform.localPosition = SFXPawPosition;
                MusicSliderText.fontStyle = FontStyle.Normal;
                MusicSliderLabel.fontStyle = FontStyle.Normal;
                SFXSliderText.fontStyle = FontStyle.Bold;
                SFXSliderLabel.fontStyle = FontStyle.Bold;
                AudioManager.Instance.PlaySFXClip(AudioLabel.SwapSelectSFX);
            }
        }
        else if (!MusicSelected)
        {
            if (InputController.GetInput(InputPurpose.MENU_CHOICE_LEFT))
            {
                SFXCurrentIndex--;
                if (SFXCurrentIndex < MinIndex)
                {
                    SFXCurrentIndex++;
                }
                SetSFXVolumeSlider(true);
                AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
            }
            else if (InputController.GetInput(InputPurpose.MENU_CHOICE_RIGHT))
            {
                SFXCurrentIndex++;
                if (SFXCurrentIndex > MaxIndex)
                {
                    SFXCurrentIndex--;
                }
                SetSFXVolumeSlider(true);
                AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
            }
            else if (InputController.GetInput(InputPurpose.MENU_CHOICE_DOWN))
            {
                MusicSelected = true;
                Paw.transform.localPosition = MusicPawPosition;
                MusicSliderText.fontStyle = FontStyle.Bold;
                MusicSliderLabel.fontStyle = FontStyle.Bold;
                SFXSliderText.fontStyle = FontStyle.Normal;
                SFXSliderLabel.fontStyle = FontStyle.Normal;
                AudioManager.Instance.PlaySFXClip(AudioLabel.SwapSelectSFX);
            }
        }
        
        if (InputController.GetInput(InputPurpose.INTERACT))
        {
            MusicSliderParent.SetActive(false);
            SFXSliderParent.SetActive(false);
            AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
        }
    }

    private void SetMusicVolumeSlider(bool withSave)
    {
        if (withSave)
        {
            CurrentGameSettings.MusicVolumeIndex = MusicCurrentIndex;
            CurrentGameSettings.SaveFromMenu();
        }

        //visuals
        var soundSliderText = "[";
        for (int i = 0; i < MusicCurrentIndex; i++)
        {
            soundSliderText += SoundLetter;
        }
        for (int i = MusicCurrentIndex; i < MaxIndex; i++)
        {
            soundSliderText += FillLetter;
        }
        soundSliderText += "]";
        MusicSliderText.text = soundSliderText;

        //Volume
        var volume = (float) MusicCurrentIndex / MaxIndex;
        var musicVolume = AudioManager.VolumeFunction(volume);
        AudioManager.Instance.MainMixer.SetFloat(MusicVolumeParameterName, musicVolume);
        CLogger.Log($"Music set: MusicCurrentIndex={MusicCurrentIndex}, volumeStep={volume}, musicVolume={musicVolume}, MaxIndex={MaxIndex}");
    }

    private void SetSFXVolumeSlider(bool withSave)
    {
        if (withSave)
        {
            CurrentGameSettings.SFXVolumeIndex = SFXCurrentIndex;
            CurrentGameSettings.SaveFromMenu();
        }

        //visuals
        var soundSliderText = "[";
        for (int i = 0; i < SFXCurrentIndex; i++)
        {
            soundSliderText += SoundLetter;
        }
        for (int i = SFXCurrentIndex; i < MaxIndex; i++)
        {
            soundSliderText += FillLetter;
        }
        soundSliderText += "]";
        SFXSliderText.text = soundSliderText;

        //Volume
        var volume = (float) SFXCurrentIndex / MaxIndex;
        var sfxVolume = AudioManager.VolumeFunction(volume);
        AudioManager.Instance.MainMixer.SetFloat(SFXVolumeParameterName, sfxVolume);
        CLogger.Log($"SFX set: SFXCurrentIndex={SFXCurrentIndex}, volumeStep={volume}, sfxVolume={sfxVolume}, MaxIndex={MaxIndex}");
    }

    public void MusicCredits()
    {
        AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
        CLogger.LogError("MusicCredits button not implemented!");
        AudioManager.Instance.PlayMusicClip(AudioLabel.FloristMoment, false, true);
    }

    public void Version()
    {
        AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
        CLogger.LogError("Version button not implemented!");
    }

    private void UpdateSelectedButton()
    {
        foreach (var keyValue in AllTexts)
        {
            keyValue.Key.fontStyle = FontStyle.Normal;
        }

        var selectedText = GetSelectedText();
        selectedText.Key.fontStyle = FontStyle.Bold;

        if(!NewGameOverwrite.activeSelf)
        {
            Paw.transform.localPosition = selectedText.Key.transform.localPosition + PawPosition;
        }
    }

    private KeyValuePair<Text, Action> GetSelectedText()
    {
        if (SelectedRowButton == 0)
        {
            return AllTexts.FirstOrDefault(e => e.Key.Equals(ContinueText));
        }
        else if (SelectedRowButton == 1)
        {
            return AllTexts.FirstOrDefault(e => e.Key.Equals(NewGameText));
        }
        else if (SelectedRowButton == 2)
        {
            return AllTexts.FirstOrDefault(e => e.Key.Equals(QuitText));
        }
        else if (SelectedRowButton == 3 && SelectedColumnButton == 0)
        {
            return AllTexts.FirstOrDefault(e => e.Key.Equals(CreditsText));
        }
        else if (SelectedRowButton == 3 && SelectedColumnButton == -1)
        {
            return AllTexts.FirstOrDefault(e => e.Key.Equals(WindowedText));
        }
        else if (SelectedRowButton == 3 && SelectedColumnButton == -2)
        {
            return AllTexts.FirstOrDefault(e => e.Key.Equals(SoundText));
        }
        else if (SelectedRowButton == 3 && SelectedColumnButton == 1)
        {
            return AllTexts.FirstOrDefault(e => e.Key.Equals(MusicCreditsText));
        }
        else if (SelectedRowButton == 3 && SelectedColumnButton == 2)
        {
            return AllTexts.FirstOrDefault(e => e.Key.Equals(VersionText));
        }
        return AllTexts.FirstOrDefault(e => e.Key.Equals(QuitText)); ;
    }
}
