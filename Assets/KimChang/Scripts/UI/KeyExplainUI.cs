using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class KeyExplainUI : MonoBehaviour
{
    [SerializeField] private GameObject key1Window;
    [SerializeField] private GameObject key2Window;
    [SerializeField] private GameObject key3Window;
    [SerializeField] private Button key1Btn;
    [SerializeField] private Button key2Btn;
    [SerializeField] private Button key22Btn;
    [SerializeField] private Button key3Btn;

    [SerializeField] private Button xBtn;

    private void Start()
    {
        key1Btn.onClick.AddListener(OpenKey1Window);
        key2Btn.onClick.AddListener(OpenKey2Window);
        key22Btn.onClick.AddListener(OpenKey2Window);
        key3Btn.onClick.AddListener(OpenKey3Window);
        xBtn.onClick.AddListener(CloseObject);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.G))
        {
            CloseObject();
        }
    }
    private void CloseObject()
    {
        GameMapUI.Instance.CloseKeyExplain();
    }

    private void OpenKey1Window()
    {
        key1Window.SetActive(true);
        key2Window.SetActive(false);
        key3Window.SetActive(false);

    }
    private void OpenKey2Window()
    {
        key2Window.SetActive(true);
        key1Window.SetActive(false);
        key3Window.SetActive(false);

    }
    private void OpenKey3Window()
    {
        key3Window.SetActive(true);
        key2Window.SetActive(false);
        key1Window.SetActive(false);

    }
}
