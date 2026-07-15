using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class SoundGroup
    {
        public string groupName;   // 구분용 이름 (예: A, B, C) - 인스펙터에서 보기 편하라고 있는 필드
        public Button[] buttons;   // 이 그룹에 속한 버튼들 (개수 제한 없음)
        public AudioClip clip;     // 이 그룹 버튼이 눌렸을 때 재생할 소리
    }

    [Header("Mixer")]
    public AudioMixer mainMixer;   // MainMixer 에셋 연결 (BGMVolume / SFXVolume 파라미터 Expose 필요)

    [Header("Audio Sources")]
    public AudioSource bgmSource;  // 브금 재생용 (Output = BGM 그룹)
    public AudioSource sfxSource;  // 효과음 재생용 (Output = SFX 그룹)

    [Header("Button SFX")]
    public SoundGroup[] groups;

    private const string BGM_MIXER_PARAM = "BGMVolume";
    private const string SFX_MIXER_PARAM = "SFXVolume";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        RegisterButtonSounds();
    }

    void RegisterButtonSounds()
    {
        foreach (SoundGroup group in groups)
        {
            AudioClip clip = group.clip; // 그룹별 클립 캡처

            foreach (Button btn in group.buttons)
            {
                if (btn == null)
                    continue;

                btn.onClick.AddListener(() => PlaySFX(clip));
            }
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null || bgmSource == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    /// <summary>
    /// 슬라이더 값(0~1)을 dB로 변환해 Mixer에 적용. 값 자체를 저장/기억하진 않음 (OptionManager 담당).
    /// </summary>
    public void ApplyBGMVolume(float sliderValue)
    {
        ApplyMixerVolume(BGM_MIXER_PARAM, sliderValue);
    }

    public void ApplySFXVolume(float sliderValue)
    {
        ApplyMixerVolume(SFX_MIXER_PARAM, sliderValue);
    }

    void ApplyMixerVolume(string paramName, float sliderValue)
    {
        if (mainMixer == null) return;
        mainMixer.SetFloat(paramName, SliderToDb(sliderValue));
    }

    float SliderToDb(float sliderValue)
    {
        float clamped = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        return Mathf.Log10(clamped) * 20f;
    }

    /// <summary>
    /// 배경음악만 개별 음소거. muted=false일 때는 unmutedSliderValue로 복원.
    /// </summary>
    public void SetBGMMute(bool muted, float unmutedSliderValue)
    {
        if (mainMixer == null) return;
        float dB = muted ? -80f : SliderToDb(unmutedSliderValue);
        mainMixer.SetFloat(BGM_MIXER_PARAM, dB);
    }

    /// <summary>
    /// 효과음만 개별 음소거. muted=false일 때는 unmutedSliderValue로 복원.
    /// </summary>
    public void SetSFXMute(bool muted, float unmutedSliderValue)
    {
        if (mainMixer == null) return;
        float dB = muted ? -80f : SliderToDb(unmutedSliderValue);
        mainMixer.SetFloat(SFX_MIXER_PARAM, dB);
    }
}