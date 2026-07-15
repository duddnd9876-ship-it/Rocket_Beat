using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundGroup
    {
        public string groupName;   // 구분용 이름 (예: A, B, C) - 그냥 인스펙터에서 보기 편하라고 있는 필드
        public Button[] buttons;   // 이 그룹에 속한 버튼들 (개수 제한 없음)
        public AudioClip clip;     // 이 그룹 버튼이 눌렸을 때 재생할 소리
    }

    public AudioSource sfxSource;
    public SoundGroup[] groups;

    void Start()
    {
        foreach (SoundGroup group in groups)
        {
            AudioClip clip = group.clip; // 그룹별 클립 캡처

            foreach (Button btn in group.buttons)
            {
                if (btn == null)
                    continue;

                btn.onClick.AddListener(() =>
                {
                    if (clip != null)
                        sfxSource.PlayOneShot(clip);
                });
            }
        }
    }
}