using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;

public class LoadUIManager : MonoBehaviour
{
    [SerializeField] private Button toggleDeleteWindow;
    [SerializeField] private Button startBtn;
    [SerializeField] private GameObject deleteWindow;
    [SerializeField] private Image stageImage;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI scoreText;
    private int id=0;
    private int stage = 0;

    private void Awake()
    {
        id = ExtractIdFromName(gameObject.name);
    }

    private void Start()
    {
        toggleDeleteWindow.onClick.AddListener(ToggleDeleteWindow);
        startBtn.onClick.AddListener(HandleStartButton);

        InitWindow();
    }

    public void InitWindow()
    {
        string path = Application.dataPath + $"/KimChang/Json/PlayerData_{id}.json";
        if (!File.Exists(path))
        {
            stageText.text = "알 수 없음";
            scoreText.text = "Score";
            stageImage.sprite = SpriteCache.GetSprite("SaveNone");
            return;
        }

        string json = File.ReadAllText(path);
        SavePlayerData data = JsonUtility.FromJson<SavePlayerData>(json);

        stage = data.currentPortalId;

        string stageName = GameData.Instance.GetMapNameById(stage);

        if (stage == 1)
        {   
            stageImage.sprite = SpriteCache.GetSprite("SaveStartRoom");
        }
        else if (stage == 2)
        {
            stageImage.sprite = SpriteCache.GetSprite("SavePalace");
        }
        else
        {
            stageImage.sprite = SpriteCache.GetSprite("SaveNone");
            toggleDeleteWindow.gameObject.SetActive(false);
        }

        stageText.text = stageName;
        scoreText.text = $"Score: {data.money}";
    }


    private void ToggleDeleteWindow()
    {
        if(stage != 0)
        {
            deleteWindow.SetActive(!deleteWindow.activeSelf);
            deleteWindow.name = $"{ExtractIdFromName(gameObject.name)}";
        }
    }

    private void HandleStartButton()
    {
        if(id==0) id = ExtractIdFromName(gameObject.name);

        if (id < 0)
        {
            Debug.LogError("오브젝트 이름에서 ID를 추출할 수 없습니다.");
            return;
        }

        string path = Application.dataPath + $"/KimChang/Json/PlayerData_{id}.json";

        SaveData saveData = new SaveData();

        if (File.Exists(path))
        {
            // 저장 파일이 존재하면 로드 후 stage로 이동
            saveData.LoadDataFile(id);
            SceneManager.LoadScene($"Map{GameData.Instance.GetMapById()}");
        }
        else
        {
            // 저장 파일이 없으면 초기 저장 생성하고 이동
            GameData.Instance.SetPortalId(-1); // 기본 스테이지 설정
            saveData.SaveDataFile(id);     // 파일 저장
            SceneManager.LoadScene("Map1");
        }
    }



    private int ExtractIdFromName(string objName)
    {
        Match match = Regex.Match(objName, @"\d+");
        return match.Success ? int.Parse(match.Value) : -1;
    }
}
