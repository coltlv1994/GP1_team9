using Dialogue;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


public class SettingMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject m_previous;
    [SerializeField] private GameObject m_this;
    [SerializeField] private GameObject m_parent;

    [SerializeField] GameObject VideoSettingsMenu, AudioSettingsMenu; //The gameobjects that hold the main menu and the settings menu

    [SerializeField] GameObject FirstSettingsButton, FirstMenuButton, FirstVideoButton, FirstAudioButton; //The buttons that should be automatically selected when entering a new menu (makes the buttons work with controllers)

    [SerializeField] Slider BrightnessSlider, MusicSlider, SFXSlider, VoiceSlider, m_menuSizeSlider;

    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] Volume _volume;
    ColorAdjustments _colorAdjustments;

    public void OpenSetting()
    {
        m_previous.SetActive(false);
        m_parent.SetActive(true);
        m_this.SetActive(true);
    }

    public void CloseSetting()
    {
        m_previous.SetActive(true);
        m_parent.SetActive(false);
        m_this.SetActive(false);
    }

    public void Start()
    {
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

        ColorAdjustments tmp;
        if (_volume.profile.TryGet<ColorAdjustments>(out tmp))
        {
            _colorAdjustments = tmp;
        }
    }

    void Update()
    {
    }

    public void ToSwedish()
    {
        DialogueManager.GetInstance().ChangeLanguage(Language.SE);
    }

    public void ToEnglish()
    {
        DialogueManager.GetInstance().ChangeLanguage(Language.ENG);
    }

    public void ChangeContrast()
    {
        _colorAdjustments.contrast.overrideState = !_colorAdjustments.contrast.overrideState;
    }

    public void SetBrightness()
    {
        _colorAdjustments.postExposure.value = BrightnessSlider.value;
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
        m_previous.transform.localScale = new Vector3(m_menuSizeSlider.value, m_menuSizeSlider.value, 0.0f);
        m_this.transform.localScale = new Vector3(m_menuSizeSlider.value, m_menuSizeSlider.value, 0.0f);
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

    public void VideoSettings()
    {
        m_this.SetActive(false);
        VideoSettingsMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstVideoButton);
    }

    public void AudioSettings()
    {
        m_this.SetActive(false);
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

    public void BackToSettings()
    {
        m_this.SetActive(true);
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
