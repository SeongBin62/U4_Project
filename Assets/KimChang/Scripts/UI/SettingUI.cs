using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private Button countinueBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button goTitleBtn;
    [SerializeField] private Button exitBtn;

    [SerializeField] private GameObject audioWindow;

    private void Start()
    {
        countinueBtn.onClick.AddListener(ToggleObject);
        settingBtn.onClick.AddListener(OpenAudioWindow);
        goTitleBtn.onClick.AddListener(GoToTitleScene);
        exitBtn.onClick.AddListener(ExitGame);
    }
    private void ToggleObject()
    {
        gameObject.SetActive(false);
    }

    private void OpenAudioWindow()
    {
        audioWindow.SetActive(true);
    }

    //메인 화면 가기
    private void GoToTitleScene()
    {
        GameMapUI.Instance.GamePlay();
        SceneManager.LoadScene("Title"); // "Main"은 메인 씬의 이름
    }
    //게임 종료
    private void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
