using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMapUIManager : MonoBehaviour
{
    [SerializeField] private Button StartOpenButton;
    [SerializeField] private Button LoadOpenButton;
    [SerializeField] private Button SettingButton;
    [SerializeField] private Button ExitButton;
    [SerializeField] private Button LoadCloseButton;

    [SerializeField] private GameObject settingWindow;
    [SerializeField] private GameObject SelectObj; // 선택 UI 이미지
    [SerializeField] private RectTransform LoadWindow;

    private float targetXPosition = -2029f;  // 이동할 X 위치
    private float tweenDuration = 0.5f;      // 이동 시간(초)

    private bool isLoadOpen = false;

    private Vector2 originalPosition;

    void Start()
    {
        settingWindow.SetActive(false);

        StartOpenButton.onClick.AddListener(StartRecentGame);
        originalPosition = LoadWindow.anchoredPosition;
        SettingButton.onClick.AddListener(OpenSettingWindow);
        ExitButton.onClick.AddListener(ExitGame);

        LoadOpenButton.onClick.AddListener(() =>
        {
            if (!isLoadOpen)
            {
                Vector2 targetPos = new Vector2(targetXPosition, LoadWindow.anchoredPosition.y);
                LoadWindow.DOAnchorPos(targetPos, tweenDuration).SetEase(Ease.OutQuad);
                isLoadOpen = true;
            }
        });

        LoadCloseButton.onClick.AddListener(() =>
        {
            if (isLoadOpen)
            {
                LoadWindow.DOAnchorPos(originalPosition, tweenDuration).SetEase(Ease.OutQuad);
                isLoadOpen = false;
            }
        });

        // 버튼에 Hover 이벤트 추가
        AddHoverEffect(StartOpenButton);
        AddHoverEffect(LoadOpenButton);
        AddHoverEffect(SettingButton);
        AddHoverEffect(ExitButton);
    }
    private void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    private void StartRecentGame()
    {
        GameData.Instance.ResetGameData();
        GameMapUI.Instance.LoadSceneWithLoading($"Map{GameData.Instance.GetMapById()}");
    }
    private void AddHoverEffect(Button button)
    {
        ButtonHoverHandler handler = button.gameObject.AddComponent<ButtonHoverHandler>();
        handler.Initialize(SelectObj.GetComponent<RectTransform>(), button.GetComponent<RectTransform>());
    }
    private void OpenSettingWindow()
    {
        settingWindow.SetActive(true);
    }
}

// 버튼 Hover 감지 스크립트
public class ButtonHoverHandler : MonoBehaviour, IPointerEnterHandler
{
    private RectTransform selectObj;
    private RectTransform buttonRect;

    public void Initialize(RectTransform select, RectTransform button)
    {
        selectObj = select;
        buttonRect = button;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectObj != null && buttonRect != null)
        {
            Vector2 newPosition = new Vector2(selectObj.anchoredPosition.x, buttonRect.anchoredPosition.y);
            selectObj.DOAnchorPos(newPosition, 0.2f).SetEase(Ease.OutQuad);
        }
    }

    
}
