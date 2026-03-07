using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using DG.Tweening; // DOTween 네임스페이스 추가

public class DeleteUI : MonoBehaviour
{
    [SerializeField] private Button xBtn;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;
    [SerializeField] private GameObject deleteWindow;

    private int saveId = -1;
    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        saveId = ExtractIdFromName(gameObject.name);
        rectTransform.anchoredPosition = originalPosition; // 위치 초기화
    }

    private void Start()
    {
        xBtn.onClick.AddListener(() => CloseWithAnimation());
        noBtn.onClick.AddListener(() => CloseWithAnimation());

        yesBtn.onClick.AddListener(() =>
        {
            string path = Application.dataPath + $"/KimChang/Json/PlayerData_{saveId}.json";

            if (File.Exists(path))
            {
                File.Delete(path);

                GameObject saveCard = GameObject.Find($"SaveCard{saveId}");
                saveCard.GetComponent<LoadUIManager>().InitWindow();
            }

            CloseWithAnimation();
        });
    }

    private int ExtractIdFromName(string objName)
    {
        Match match = Regex.Match(objName, @"\d+");
        return match.Success ? int.Parse(match.Value) : -1;
    }

    // DOTween을 이용한 종료 애니메이션
    private void CloseWithAnimation()
    {
        rectTransform.DOAnchorPos(originalPosition + Vector2.up * 100f, 1f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
