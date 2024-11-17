using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;

public class MenuScript : MonoBehaviour
{
    [SerializeField] GameObject MainMenu, SettingParent, SettingsMenu, DifficultyMenu, VideoSettingsMenu, AudioSettingsMenu; //The gameobjects that hold the main menu and the settings menu

    [SerializeField] GameObject FirstSettingsButton, FirstMenuButton, FirstDifficultyButton, FirstVideoButton, FirstAudioButton; //The buttons that should be automatically selected when entering a new menu (makes the buttons work with controllers)

    [SerializeField] Slider BrightnessSlider, MusicSlider, SFXSlider, VoiceSlider, m_difficultySlider, m_menuSizeSlider;

    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] Volume _volume;
    ColorAdjustments _colorAdjustments;

    [SerializeField] bool isMainMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SettingsMenu.SetActive(false);
        if (isMainMenu)
        {
            MainMenu.SetActive(true);
            DifficultyMenu.SetActive(false);
        }
        /*
        else
        {
            DifficultyMenu = null;
            FirstDifficultyButton = null;
        }
        */

        if (PlayerPrefs.HasKey("TextSize"))
        {
            m_menuSizeSlider.value = PlayerPrefs.GetFloat("TextSize");
            SetTextSize();
        }
        else
        {
            m_menuSizeSlider.value = 0.75f;
            SetTextSize();
        }

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
            SetVoiceVolume();
        }

        ColorAdjustments tmp;
        if (_volume.profile.TryGet<ColorAdjustments>(out tmp))
        {
            _colorAdjustments = tmp;
        }

        SetLocale(PlayerPrefs.GetInt("lang", 0));
    }

    // Update is called once per frame
    void Update()
    {
        _colorAdjustments.postExposure.value = BrightnessSlider.value; 
    }

    public void StartGame() 
    {
        PlayerPrefs.SetFloat("Difficulty", (m_difficultySlider.value / m_difficultySlider.maxValue) * 2);
        GameManager.GetInstance().ChangeState(new GameplayState());
    }

    public void StartTutorial()
    {
        GameManager.GetInstance().ChangeState(new TutorialState());
    }

    public void StartButton()
    {
        DifficultyMenu.SetActive(true);
        MainMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstDifficultyButton);
    }


    public void ChangeContrast()
    {
        _colorAdjustments.contrast.overrideState = !_colorAdjustments.contrast.overrideState;
    }

    //Audio settings
    public void SetMusicVolume()
    {
        float volume = MusicSlider.value;
        _audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        _audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    public void SetVoiceVolume()
    {
        float volume = VoiceSlider.value;
        _audioMixer.SetFloat("voice", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("voiceVolume", volume);
    }

    public void SetTextSize()
    {
        MainMenu.transform.localScale = new Vector3(m_menuSizeSlider.value, m_menuSizeSlider.value, 0.0f);
        SettingsMenu.transform.localScale = new Vector3(m_menuSizeSlider.value, m_menuSizeSlider.value, 0.0f);
        PlayerPrefs.SetFloat("TextSize", m_menuSizeSlider.value);
    }

    void LoadVolume()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        VoiceSlider.value = PlayerPrefs.GetFloat("voiceVolume");

        SetMusicVolume();
        SetSFXVolume();
        SetVoiceVolume();
    }
    //End of audio settings

    public void OpenSettings()
    {
        MainMenu.SetActive(false);
        SettingParent.SetActive(true);
        SettingsMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstSettingsButton);
    }

    public void VideoSettings()
    {
        SettingsMenu.SetActive(false);
        VideoSettingsMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstVideoButton);
    }

    public void AudioSettings()
    {
        SettingsMenu.SetActive(false);
        AudioSettingsMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstAudioButton);
    }

    public void ToggleFullscreen()
    {
        if (Screen.fullScreen == true)
        {
            Screen.fullScreen = false;
        }
        else
        {
            Screen.fullScreen = true;
        }
    }

    public void GoBack() //Code for the back button in the settings menu
    {
        SettingsMenu.SetActive(false);
        SettingParent.SetActive(false);
        MainMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstMenuButton);
        SetTextSize();
    }

    public void BackToSettings()
    {
        SettingsMenu.SetActive(true);
        VideoSettingsMenu.SetActive(false);
        AudioSettingsMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstSettingsButton);

        SetTextSize();
    }

    public void ChangeLanguage(int p_localeID)
    {
        StartCoroutine(SetLocale(p_localeID));
    }

    public void SetBrightness()
    {
        _colorAdjustments.postExposure.value = BrightnessSlider.value;
    }

    IEnumerator SetLocale(int _localeID)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        PlayerPrefs.SetInt("lang", _localeID);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
