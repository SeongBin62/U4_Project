using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioUI : MonoBehaviour
{
    [SerializeField] private Button backBtn;

    [SerializeField] private Slider allSlider;  // 전체 볼륨 슬라이더
    [SerializeField] private Slider bgmSlider;  // BGM 볼륨 슬라이더
    [SerializeField] private Slider soundSlider; // 효과음 볼륨 슬라이더

    // Start is called before the first frame update
    void Start()
    {
        backBtn.onClick.AddListener(CloseAudioWindow);

        // 슬라이더 값 설정
        float savedBgmVolume = GameData.Instance.GetBgmVolume();
        float savedAllVolume = GameData.Instance.GetAllVolume(); // 전체 볼륨 추가

        allSlider.value = savedAllVolume;
        bgmSlider.value = savedBgmVolume;

        allSlider.onValueChanged.AddListener(ChangeAllVolume);
        bgmSlider.onValueChanged.AddListener(ChangeBgmVolume);
    }

    // 설정창 닫기
    private void CloseAudioWindow()
    {
        this.gameObject.SetActive(false);
    }

    // 전체 볼륨 조절
    private void ChangeAllVolume(float volume)
    {
        GameData.Instance.SetAllVolume(volume);

        // 전체 볼륨을 bgm, sound에 적용
        float newBgmVolume = volume * bgmSlider.value;
        float newSoundVolume = volume * soundSlider.value;

        AudioManager.instance.UpdateBgmVolume(newBgmVolume);
        //AudioManager.instance.UpdateSoundVolume(newSoundVolume);
    }

    // 개별 BGM 볼륨 조절
    private void ChangeBgmVolume(float volume)
    {
        GameData.Instance.SetBgmVolume(volume);

        // 전체 볼륨이 적용된 BGM 볼륨 조절
        float newBgmVolume = allSlider.value * volume;
        AudioManager.instance.UpdateBgmVolume(newBgmVolume);
    }
}
