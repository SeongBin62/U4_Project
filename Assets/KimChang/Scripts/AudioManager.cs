using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public List<AudioClip> bgmClip;
    public float bgmVolum;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmHighPassFilter;

    public AudioClip[] sfxClip;
    public float sfxVolum;
    public int channels;
    AudioSource[] sfxPlayers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // BGM 볼륨 갱신
    public void UpdateBgmVolume(float volume)
    {
        bgmPlayer.volume = volume;
    }

    // BGM 재생/정지
    public void PlayBgm(bool isPlay)
    {
        if (isPlay) bgmPlayer.Play();
        else bgmPlayer.Stop();
    }

    // 고역필터 적용
    public void EffectBgm(bool isPlay)
    {
        bgmHighPassFilter.enabled = isPlay;
    }

    // 특정 인덱스의 BGM 재생
    public void PlayBgmByIndex(int index)
    {
        if (index < 0 || index >= bgmClip.Count) return;
        if (bgmPlayer.clip == bgmClip[index]) return; // 이미 같은 음악이면 무시

        bgmPlayer.clip = bgmClip[index];
        bgmPlayer.Play();
    }

    // 씬 이름에 따라 BGM 재생
    public void PlayBgmByScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int bgmIndex;

        // 씬 이름 기반으로 인덱스 결정
        switch (sceneName)
        {
            case "Title":
                bgmIndex = 0;
                break;
            case "Map1":
                bgmIndex = 1;
                break;
            case "Map2":
                bgmIndex = 2;
                break;
            case "Map7":
                bgmIndex = 2;
                break;
            case "Map3":
                bgmIndex = 3;
                break;
            case "Map4":
                bgmIndex = 3;
                break;
            case "Map5":
                bgmIndex = 3;
                break;
            case "Map6":
                bgmIndex = 6;
                break;
            case "Map8":
                bgmIndex = 6;
                break;
            case "Map9":
                bgmIndex = 6;
                break;
            case "Map10":
                bgmIndex = 6;
                break;
            case "Map11":
                bgmIndex = 6;
                break;
            default:
                bgmIndex = -1;
                break;
        }

        PlayBgmByIndex(bgmIndex);
    }

    // 초기화 함수
    private void Init()
    {
        GameObject bgmObject = new("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = GameData.Instance.GetBgmVolume();
        bgmHighPassFilter = Camera.main.GetComponent<AudioHighPassFilter>();

        GameObject sfxObject = new("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].bypassEffects = true;
            sfxPlayers[i].volume = sfxVolum;
        }
    }
    // 보스 등장 시 BGM 교체
    public void PlayBgmByBoss(int bossId)
    {
        if (bossId >= 0 && bossId < bgmClip.Count)
        {
            if (bgmPlayer.clip != bgmClip[bossId])
            {
                bgmPlayer.clip = bgmClip[bossId];
                bgmPlayer.Play();
            }
        }
    }

    // 지정된 효과음 인덱스를 재생
    public void PlaySfx(int index)
    {
        if (index < 0 || index >= sfxClip.Length) return;

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            if (!sfxPlayers[i].isPlaying)
            {
                sfxPlayers[i].clip = sfxClip[index];
                sfxPlayers[i].Play();
                return;
            }
        }
    }

}
