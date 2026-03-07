using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI talkText;
    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private Button nextBtn;
    [SerializeField] private GameObject pet;

    private float zoomSpeed = 5f;
    private float maxSize = 5f;

    private Dictionary<int, string> dialog = new();
    private int currentIndex = -1;
    private Camera cam;

    private bool isZoomingOut = false;
    private bool isEnding = false;

    private void OnEnable()
    {
        GameMapUI.Instance.isPaused = true;
        GameMapUI.Instance.isTalk =true;
        InitDialog();
        currentIndex = -1;
        ShowNextText();
        nextBtn.gameObject.SetActive(true);
        nextBtn.onClick.AddListener(ShowNextText);
        nextBtn.interactable = true;
        pet.SetActive(true);
        cam = Camera.main;
        Time.timeScale = 0;
        isZoomingOut = false;
        isEnding = false;
    }

    private void OnDisable()
    {
        nextBtn.onClick.RemoveListener(ShowNextText);
    }

    private void Update()
    {
        if (isZoomingOut && cam != null)
        {
            cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, maxSize, zoomSpeed * Time.unscaledDeltaTime);
            if (Mathf.Approximately(cam.orthographicSize, maxSize))
            {
                isZoomingOut = false;
                if (isEnding)
                {
                    isEnding = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private void InitDialog()
    {
        dialog.Clear();
        dialog.Add(1, "극에 달하는 전쟁, 폐허가 되어가는 왕국. 몰락을 막기 위해서라면 어떠한 희생도 감수할 수 있습니다. 그것이 번영해왔던 우리의 시대일지라도.");
        dialog.Add(2, "");
        dialog.Add(3, "윽! 큰일 날 뻔 했잖아! 겨우 받아냈네. 괜찮아?");
        dialog.Add(4, "응. 괜찮아.");
        dialog.Add(5, " ...뭐.");
        dialog.Add(6, "내가 한 말 기억해?");
        dialog.Add(7, "네가 한 말? ... .... 설마 그 예언?! 그런거 신경쓰지 않겠다고 했잖아!");
        dialog.Add(8, "...거짓말이었어");
        dialog.Add(9, "이 고지식한 토깽이가!!");
        dialog.Add(10, "욕하지마.");
        dialog.Add(11, "에휴. 내가 왜 수호신 계약을 해서.\n뭐해? 가자. 시계 태엽은 잘 있지?");
        dialog.Add(12, "응. 물론이지");
    }

    private void ShowNextText()
    {
        if (!dialog.ContainsKey(currentIndex + 2))
        {
            talkText.text = "";
            answerText.text = "";
            nextBtn.interactable = false;
            nextBtn.gameObject.SetActive(false);
            GameMapUI.Instance.isTalk = false;
            GameMapUI.Instance.isPaused = false;
            GameData.Instance.AddKillMonsterId(-10);

            isZoomingOut = true;
            isEnding = true;
            Time.timeScale = 1;
            pet.SetActive(false);
            SaveData saveData = new SaveData();
            saveData.SaveDataFile(GameData.Instance.GetPlayerId());

            return;
        }

        currentIndex += 2;
        dialog.TryGetValue(currentIndex, out string talkLine);
        dialog.TryGetValue(currentIndex + 1, out string answerLine);

        talkText.text = talkLine;
        answerText.text = answerLine;

        
    }
}
