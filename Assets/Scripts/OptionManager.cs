using UnityEngine;
using UnityEngine.UI;
public class OptionManager : MonoBehaviour
{
    public Button OptionOpen;
    public GameObject OptionPanel;
    public Button GameQuit;
    public Button OptionQuit;

    [Header("UI")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("Mute Buttons")]
    public Button bgmMuteButton;
    public Image bgmMuteButtonImage;
    public Button sfxMuteButton;
    public Image sfxMuteButtonImage;
    public Sprite unmutedSprite;    // 초기 상태 이미지 (A)
    public Sprite mutedSprite;      // 음소거 상태 이미지 (B)

    private const string BGM_PREF_KEY = "bgmVolume";
    private const string SFX_PREF_KEY = "sfxVolume";
    private const string BGM_MUTE_PREF_KEY = "isBgmMuted";
    private const string SFX_MUTE_PREF_KEY = "isSfxMuted";
    private const float DEFAULT_VOLUME = 0.75f;

    private bool isBgmMuted;
    private bool isSfxMuted;

    void Start()
    {
        LoadAndApplySettings();
        OptionPanel.SetActive(false);
        OptionOpen.onClick.AddListener(OpenOption);
        OptionQuit.onClick.AddListener(CloseOption);
        GameQuit.onClick.AddListener(GameQuitManager.QuitGame);

        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);

        if (bgmMuteButton != null)
            bgmMuteButton.onClick.AddListener(OnBGMMuteButtonClicked);

        if (sfxMuteButton != null)
            sfxMuteButton.onClick.AddListener(OnSFXMuteButtonClicked);
    }
    void OpenOption()
    {
        OptionPanel.SetActive(true);
    }
    void CloseOption()
    {
        OptionPanel.SetActive(false);
    }
    void LoadAndApplySettings()
    {
        float bgmVolume = PlayerPrefs.GetFloat(BGM_PREF_KEY, DEFAULT_VOLUME);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_PREF_KEY, DEFAULT_VOLUME);

        if (bgmSlider != null)
            bgmSlider.SetValueWithoutNotify(bgmVolume);

        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(sfxVolume);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.ApplyBGMVolume(bgmVolume);
            SoundManager.Instance.ApplySFXVolume(sfxVolume);
        }

        isBgmMuted = PlayerPrefs.GetInt(BGM_MUTE_PREF_KEY, 0) == 1;
        isSfxMuted = PlayerPrefs.GetInt(SFX_MUTE_PREF_KEY, 0) == 1;
        ApplyBgmMuteState();
        ApplySfxMuteState();
    }

    void OnBGMSliderChanged(float value)
    {
        SoundManager.Instance?.ApplyBGMVolume(value);
        PlayerPrefs.SetFloat(BGM_PREF_KEY, value);

        // 슬라이더를 직접 움직이면 음소거는 자연스럽게 해제
        if (isBgmMuted)
        {
            isBgmMuted = false;
            PlayerPrefs.SetInt(BGM_MUTE_PREF_KEY, 0);
            SetMuteImage(bgmMuteButtonImage, false);
        }
    }

    void OnSFXSliderChanged(float value)
    {
        SoundManager.Instance?.ApplySFXVolume(value);
        PlayerPrefs.SetFloat(SFX_PREF_KEY, value);

        if (isSfxMuted)
        {
            isSfxMuted = false;
            PlayerPrefs.SetInt(SFX_MUTE_PREF_KEY, 0);
            SetMuteImage(sfxMuteButtonImage, false);
        }
    }

    void OnBGMMuteButtonClicked()
    {
        isBgmMuted = !isBgmMuted;
        PlayerPrefs.SetInt(BGM_MUTE_PREF_KEY, isBgmMuted ? 1 : 0);
        ApplyBgmMuteState();
    }

    void OnSFXMuteButtonClicked()
    {
        isSfxMuted = !isSfxMuted;
        PlayerPrefs.SetInt(SFX_MUTE_PREF_KEY, isSfxMuted ? 1 : 0);
        ApplySfxMuteState();
    }

    void ApplyBgmMuteState()
    {
        float currentValue = bgmSlider != null ? bgmSlider.value : DEFAULT_VOLUME;
        SoundManager.Instance?.SetBGMMute(isBgmMuted, currentValue);
        SetMuteImage(bgmMuteButtonImage, isBgmMuted);
    }

    void ApplySfxMuteState()
    {
        float currentValue = sfxSlider != null ? sfxSlider.value : DEFAULT_VOLUME;
        SoundManager.Instance?.SetSFXMute(isSfxMuted, currentValue);
        SetMuteImage(sfxMuteButtonImage, isSfxMuted);
    }

    void SetMuteImage(Image img, bool muted)
    {
        if (img != null)
            img.sprite = muted ? mutedSprite : unmutedSprite;
    }

    void OnDestroy()
    {
        if (bgmSlider != null)
            bgmSlider.onValueChanged.RemoveListener(OnBGMSliderChanged);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(OnSFXSliderChanged);

        if (bgmMuteButton != null)
            bgmMuteButton.onClick.RemoveListener(OnBGMMuteButtonClicked);

        if (sfxMuteButton != null)
            sfxMuteButton.onClick.RemoveListener(OnSFXMuteButtonClicked);
    }
}
