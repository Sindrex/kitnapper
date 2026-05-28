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
    public GameObject PawW;
    public GameObject PawS;
    public GameObject PawA;
    public GameObject PawD;

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
        //Load Steam
        SteamIntegration.Initialize();

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
        PawW.SetActive(false);
        PawS.SetActive(true);
        PawA.SetActive(false);
        PawD.SetActive(false);
        Paw.SetActive(false);
        IntroKeys.SetActive(true);
        MusicSliderParent.SetActive(false);
        SFXSliderParent.SetActive(false);

        //load GameSettings
        CLogger.Log("Loading GameSettings.");
        if (!GameSettingsLoader.GameSettingsExist())
        {
            CLogger.Log("Found no GameSettings file! Making new");
            ContinueText.color = Color.black;
            SelectedRowButton = 1;
            CurrentGameSettings = new GameSettings()
            {
                MusicVolumeIndex = 5,
                SFXVolumeIndex = 5
            };
            CurrentGameSettings.SaveFromMenu();
        }
        else
        {
            CurrentGameSettings = GameSettingsLoader.Load();
            if (!CurrentGameSettings.HasContinueGame)
            {
                CLogger.Log("Found no Continue game!");
                ContinueText.color = Color.black;
                SelectedRowButton = 1;
            }
        }

        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        Screen.fullScreenMode = CurrentGameSettings.FullScreenMode;
        if (CurrentGameSettings.FullScreenMode == FullScreenMode.Windowed)
        {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        }
        FullScreenModeText.text = GetFullScreenModeName(CurrentGameSettings.FullScreenMode);

        MusicCurrentIndex = CurrentGameSettings.MusicVolumeIndex;
        SFXCurrentIndex = CurrentGameSettings.SFXVolumeIndex;
        SetMusicVolumeSlider(false);
        SetSFXVolumeSlider(false);

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
        SteamIntegration.OnUpdate();

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
            PawW.SetActive(true);
            PawS.SetActive(true);
        }

        if (InputController.GetInput(InputPurpose.MENU_CHOICE_UP))
        {
            IsOnYes = true;
            YesText.fontStyle = FontStyle.Bold;
            NoText.fontStyle = FontStyle.Normal;
            Paw.transform.localPosition = YesText.transform.localPosition + PawPosition;
            AudioManager.Instance.PlaySFXClip(AudioLabel.SwapSelectSFX);
            PawW.SetActive(false);
            PawS.SetActive(true);
        }
        else if (InputController.GetInput(InputPurpose.MENU_CHOICE_DOWN))
        {
            IsOnYes = false;
            YesText.fontStyle = FontStyle.Normal;
            NoText.fontStyle = FontStyle.Bold;
            Paw.transform.localPosition = NoText.transform.localPosition + PawPosition;
            AudioManager.Instance.PlaySFXClip(AudioLabel.SwapSelectSFX);
            PawW.SetActive(true);
            PawS.SetActive(false);
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
            if (!CurrentGameSettings.HasContinueGame && SelectedRowButton < 1)
            {
                SelectedRowButton = 1;
            }
            if (SelectedRowButton < MinSelectedRowButton)
            {
                SelectedRowButton = MinSelectedRowButton;
            }
            AudioManager.Instance.PlaySFXClip(AudioLabel.SwapSelectSFX);

            PawW.SetActive(true);
            PawS.SetActive(true);
            PawA.SetActive(false);
            PawD.SetActive(false);
            if(SelectedRowButton == 0 || (!CurrentGameSettings.HasContinueGame && SelectedRowButton == 1))
            {
                PawW.SetActive(false);
            }
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

            PawW.SetActive(true);
            PawS.SetActive(true);
            PawA.SetActive(false);
            PawD.SetActive(false);
            if(SelectedRowButton == MaxSelectedRowButton)
            {
                PawS.SetActive(false);
                PawA.SetActive(true);
                PawD.SetActive(true);
            }
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

                PawW.SetActive(true);
                PawS.SetActive(false);
                PawA.SetActive(true);
                PawD.SetActive(true);
                if(SelectedColumnButton == MinSelectedColumnButton)
                {
                    PawA.SetActive(false);
                }
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

                PawW.SetActive(true);
                PawS.SetActive(false);
                PawA.SetActive(true);
                PawD.SetActive(true);
                if(SelectedColumnButton == MaxSelectedColumnButton)
                {
                    PawD.SetActive(false);
                }
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
        if (CurrentGameSettings.HasContinueGame)
        {
            StartCoroutine(StartGame());
        }
        else
        {
            CLogger.LogError("Player requested to Continue game, but it has no continue game!");
        }
    }

    public void NewGame()
    {
        //Check if player already has gamesettings
        //if yes, open overwrite pane
        //else start game

        AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
        PawW.SetActive(true);
        PawS.SetActive(false);
        PawA.SetActive(false);
        PawD.SetActive(false);

        CLogger.Log("Starting new game!");
        if (CurrentGameSettings.HasContinueGame)
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
        AudioManager.Instance.PlayMusicClip(AudioLabel.MenuMusic2, false, false, true);
        foreach(var text in TitleAnimTexts)
        {
            yield return new WaitForSeconds(TitleAnimWaitSeconds);
            TitleText.text = text;
        }
        CurrentGameSettings.HasContinueGame = true;
        CurrentGameSettings.SaveFromMenu();
        SceneManager.LoadScene(1); //Game
    }

    public void Quit()
    {
        CLogger.Log("Quitting game!");
        SteamIntegration.Shutdown();
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
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            CurrentGameSettings.FullScreenMode = FullScreenMode.Windowed;
            CurrentGameSettings.SaveFromMenu();
            FullScreenModeText.text = GetFullScreenModeName(FullScreenMode.Windowed);
        }
        else if (CurrentGameSettings.FullScreenMode == FullScreenMode.Windowed)
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
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
                return "Full\r\nScreen";
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

        PawW.SetActive(true);
        PawS.SetActive(false);
        PawA.SetActive(true);
        PawD.SetActive(true);
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
                PawW.SetActive(false);
                PawS.SetActive(true);
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
                PawW.SetActive(true);
                PawS.SetActive(false);
            }
        }
        
        if (InputController.GetInput(InputPurpose.INTERACT))
        {
            MusicSliderParent.SetActive(false);
            SFXSliderParent.SetActive(false);
            AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
            PawW.SetActive(true);
            PawS.SetActive(false);
            PawA.SetActive(false);
            PawD.SetActive(true);
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
        Application.OpenURL("https://www.youtube.com/playlist?list=PLy52Uv6TC9d4Y2MO6gZWNBjU___vyK5-Q");
    }

    public void Version()
    {
        AudioManager.Instance.PlaySFXClip(AudioLabel.ClickSFX);
        Application.OpenURL("https://store.steampowered.com/app/4154720/The_Color_Kitnapper/");
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
