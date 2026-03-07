using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deathCauseText;
    [SerializeField] private Button retryBtn;
    [SerializeField] private Button goTitleBtn;
    [SerializeField] private GameObject selectArrow;
    
    private void OnEnable()
    {
        retryBtn.onClick.AddListener(ClickRetryBtn);
        goTitleBtn.onClick.AddListener(ClickGoTitleBtn);
        
        AddHoverEvents(retryBtn, new Vector2(-840f, -447f), -1f);
        AddHoverEvents(goTitleBtn, new Vector2(840f, -447f), 1f);
    }

    public void OpenDeathWindow()
    {
        gameObject.SetActive(true);
        string deathCause = GameData.Instance.GetLastHitMonsterName();
        deathCauseText.text = $"{deathCause}에게 사망 하였습니다.";
    }

    private void ClickGoTitleBtn()
    {
        GameMapUI.Instance.GamePlay();
        SceneManager.LoadScene("Title");
    }

    private void ClickRetryBtn()
    {
        SaveData saveData = new SaveData();

        int id = GameData.Instance.GetPlayerId();
        GameMapUI.Instance.GamePlay();
        GameData.Instance.SetPlayerHealth(10);
        saveData.LoadDataFile(id);
        int spawnId = GameData.Instance.GetSpawnId();
        GameData.Instance.SetPortalId(spawnId);
        GameData.Instance.SetPlayerEnergy(0);
        SceneManager.LoadScene($"Map{GameData.Instance.GetMapById()}");
    }

    private void AddHoverEvents(Button button, Vector2 targetPos, float scaleX)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((eventData) =>
        {
            Vector3 newScale = selectArrow.transform.localScale;
            newScale.x = scaleX;
            selectArrow.transform.localScale = newScale;

            selectArrow.GetComponent<RectTransform>().anchoredPosition = targetPos;
        });

        trigger.triggers.Add(entry);
    }
}
