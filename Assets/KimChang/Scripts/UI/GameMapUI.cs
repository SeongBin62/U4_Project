using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameMapUI : MonoBehaviour
{
    public static GameMapUI Instance { get; private set; }

    [SerializeField] private GameObject hpUI;
    [SerializeField] private GameObject settingWindow;
    [SerializeField] private GameObject audioWindow;
    [SerializeField] private Button countinueBtn;
    [SerializeField] private Button invenBackBtn;
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private GameObject player;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private Image energyImg;
    [SerializeField] private DeathUI deathUI;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TalkUI talkUI;
    [SerializeField] private GameObject videoWindow;
    [SerializeField] private GameObject miniMapImg;
    [SerializeField] private VideoPlayer video;
    [SerializeField] private GameObject explainUI;
    [SerializeField] private GameObject loadingWindow;
    private List<GameObject> hpUnits = new List<GameObject>();

    public bool isPaused = false;
    private bool isInventoryOpen = false;
    private bool isDeath =false;

    private bool isTitle = true;
    public bool isTalk = false;
    private bool isOpenning =false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        objectPool = GetComponentInChildren<ObjectPool>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isTitle = false;
        GamePlay();
        AudioManager.instance.PlayBgmByScene();
        Match match = Regex.Match(scene.name, @"\d+");
        int sceneNumber = match.Success ? int.Parse(match.Value) : -1;

        GameData.Instance.AddMapId(sceneNumber);
        if (scene.name == "Title")
        {
            GameData.Instance.SetPlayerHealth(10);
            isTitle= true;
            canvas.SetActive(false);
        }
        else if (scene.name == "Map1")
        {
            List<int> ids = GameData.Instance.GetKillMonsterIds();
            if (!ids.Contains(-10))
            {
                Camera cam = Camera.main;
                cam.orthographicSize = 2;
                talkUI.gameObject.SetActive(true);
                StartCoroutine(PlayVideoAfterPrepare());
            }
            else
            {
                CloseVideo();
                talkUI.gameObject.SetActive(false);
            }
        }
        else if(scene.name == "Map12")
        {
            isTitle = true;
        }
        else
        {
            CloseVideo();
            talkUI.gameObject.SetActive(false);
        }

        Camera[] allCameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in allCameras)
        {
            if (cam.gameObject == GetComponentInChildren<Camera>()?.gameObject)
                continue;

            AudioListener listener = cam.GetComponent<AudioListener>();
            if (listener != null)
                Destroy(listener);
        }

        InitUIForScene();

    }
    private IEnumerator PlayVideoAfterPrepare()
    {
        isOpenning = true;

        videoWindow.SetActive(true);
        video.Prepare();

        while (!video.isPrepared)
            yield return null;

        video.Play();

        StartCoroutine(HideAfterDelay((float)video.length));

    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        CloseVideo();

    }
    private void InitUIForScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Title")
        {
            if (player != null) player.SetActive(false);
            GamePlay();
            return;
        }
        // UI 요소 다시 찾기
        if (hpUI == null) hpUI = GameObject.Find("HpUI");
        if (settingWindow == null) settingWindow = GameObject.Find("SettingWindow");
        if (audioWindow == null) audioWindow = GameObject.Find("AudioWindow");
        if (energyImg == null) energyImg = GameObject.Find("EnergyImg")?.GetComponent<Image>();
        if (deathUI == null) deathUI = FindObjectOfType<DeathUI>();
        if (objectPool == null) objectPool = FindObjectOfType<ObjectPool>();
        if (inventoryUI == null) inventoryUI = FindObjectOfType<InventoryUI>();

        if (player == null)
        {
            Vector3 spawnPos = GameData.Instance.GetSpawnPosition();
            player = Instantiate(Resources.Load<GameObject>("Prefabs/Player"), spawnPos, Quaternion.identity);
            Vector3 scale = player.transform.localScale;
            scale.x = GameData.Instance.GetScaleX();
            player.transform.localScale = scale;
        }

        // HP 리스트 초기화
        hpUnits.Clear();
        if (hpUI != null)
        {
            for (int i = 0; i < hpUI.transform.childCount; i++)
                hpUnits.Add(hpUI.transform.GetChild(i).gameObject);
        }

        // 버튼 이벤트 다시 연결
        if (countinueBtn == null)
            countinueBtn = GameObject.Find("CountinueBtn")?.GetComponent<Button>();
        if (invenBackBtn == null)
            invenBackBtn = GameObject.Find("InvenBackBtn")?.GetComponent<Button>();

        countinueBtn.onClick.RemoveAllListeners();
        countinueBtn.onClick.AddListener(CloseSettingWindow);

        invenBackBtn.onClick.RemoveAllListeners();
        invenBackBtn.onClick.AddListener(CloseInventoryWindow);

        miniMapImg.SetActive(false);

        explainUI.SetActive(false);
        settingWindow.SetActive(false);
        audioWindow.SetActive(false);
        deathUI.gameObject.SetActive(false);
        inventoryUI.gameObject.SetActive(true);
        inventoryUI.OnEnableFrontInventory();

        isDeath = false;
        cameraFollow??=GetComponentInChildren<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.SetSceneContext(player.transform);
        }

    }

    void Start()
    {
        if (objectPool == null) objectPool = FindObjectOfType<ObjectPool>();

        if (player == null && SceneManager.GetActiveScene().name !="Title")
        {
            Vector3 spawnPos = GameData.Instance.GetSpawnPosition();
            player = Instantiate(Resources.Load<GameObject>("Player"), spawnPos, Quaternion.identity);
            
            Vector3 scale = player.transform.localScale;
            scale.x = GameData.Instance.GetScaleX();
            player.transform.localScale = scale;

        }

        UpdateHealthUI();

        countinueBtn.onClick.AddListener(CloseSettingWindow);
        invenBackBtn.onClick.AddListener(CloseInventoryWindow);

        settingWindow.SetActive(false);
    }

    void Update()
    {
        if (isOpenning && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseVideo();
        }

        if (isTitle || isTalk || isDeath) return;

        UpdateHealthUI();
        UpdateEnergy();
        UpdateMoney();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMapImg();
            if (inventoryUI.isBackOpen)
            {
                ToggleInventoryWindow();
            }
            else
            {
                ToggleSettingWindow();
            }            
        }
        else if (Input.GetKeyDown(KeyCode.I) && !settingWindow.activeSelf)
        {
            CloseMapImg();
            ToggleInventoryWindow();
        }
    }
    IEnumerator DelayedDeathWindow()
    {
        yield return new WaitForSeconds(1.5f);
        OpenDeathWindow();
        GamePuase();
    }
    private void UpdateHealthUI()
    {
        int currentHealth = GameData.Instance.GetPlayerHealth();
        int maxHealth = GameData.Instance.GetMaxHealth();
        
        if (currentHealth == 0 && !isDeath)
        {
            isDeath = true;
            StartCoroutine(DelayedDeathWindow());
        }
   
        for (int i = 0; i < hpUnits.Count; i++)
        {
            if (i < maxHealth)
            {
                hpUnits[i].SetActive(i < currentHealth);
            }
            else
            {
                hpUnits[i].SetActive(false);
            }
        }
    }

    private void UpdateEnergy()
    {
        float energy = GameData.Instance.GetPlayerEnergy();
        float maxEnergy = GameData.Instance.GetMaxEnergy();

        energyImg.fillAmount = Mathf.Clamp01(energy / maxEnergy);
    }
    public void ShowDamageText(Vector3 position, int damage, bool isMe)
    {
        if (isMe && GameData.Instance.GetPlayerHealth() < 0) return;
        GameObject dmgTextObj = objectPool.GetDamageText();

        Vector3 spawnPosition = isMe ? player.transform.position : position;
        dmgTextObj.transform.position = spawnPosition;
        TMP_Text tmp = dmgTextObj.GetComponent<TMP_Text>();
        tmp.text = damage > 0 ? $"{-1 * damage}" : "MISS";
        tmp.color = isMe ? new Color32(128, 0, 128, 255) : Color.red;

        dmgTextObj.GetComponent<DamageMove>().Play();

        if (isMe) PlayerDamaged(damage);
        StartCoroutine(HideDamageText(dmgTextObj));
    }


    private IEnumerator HideDamageText(GameObject dmgTextObj)
    {
        yield return new WaitForSeconds(0.8f);
        objectPool.ReturnDamageText(dmgTextObj);
    }

    private void ToggleSettingWindow()
    {
        isPaused = !isPaused;
        settingWindow.SetActive(isPaused);
        audioWindow.SetActive(false);

        if (isPaused)
        {
            Time.timeScale = 0f;
            AudioManager.instance.EffectBgm(true);
        }
        else
        {
            Time.timeScale = 1f;            
            AudioManager.instance.EffectBgm(false);
        }
    }

    private void CloseSettingWindow()
    {
        GamePlay();

        settingWindow.SetActive(false);
        AudioManager.instance.EffectBgm(false);
    }

    private void ToggleInventoryWindow()
    {
        isInventoryOpen = !isInventoryOpen;

        if (isInventoryOpen)
        {
            GamePuase();

            inventoryUI.OnEnableBackInventory();
            AudioManager.instance.EffectBgm(true);
        }
        else
        {
            GamePlay();

            inventoryUI.OnEnableFrontInventory();
            AudioManager.instance.EffectBgm(false);
        }
    }

    private void CloseInventoryWindow()
    {
        GamePlay();

        isInventoryOpen = false;
        inventoryUI.OnEnableFrontInventory();
        AudioManager.instance.EffectBgm(false);
    }

    public void CreateAttackEffect(GameObject target, float x)
    {
        GameObject obj = objectPool.GetAttackEffect();

        if (obj != null && target != null)
        {
            Vector3 scale = obj.transform.localScale;
            scale.x = x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            obj.transform.localScale = scale;

            obj.transform.position = target.transform.position;
            obj.SetActive(true);

            StartCoroutine(ReturnEffectAfterDelay(obj, 0.84f));
        }
    }
    public void PlayerDamaged(int damage)
    {
        PlayerMove playerMove = player.GetComponent<PlayerMove>();
        playerMove.PlayerDamaged(damage);
    }
    private IEnumerator ReturnEffectAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        objectPool.ReturnAttackEffect(obj);
    }
    public void OpenDeathWindow()
    {
        deathUI.OpenDeathWindow();
    }

    private void UpdateMoney()
    {
        int money = GameData.Instance.GetMoney();
        moneyText.text = $"{money}";
    }
    public void ToggleWindow()
    {
        miniMapImg.SetActive(!miniMapImg.activeSelf);
    }
    public void OpenMapImg()
    {
        miniMapImg.SetActive(true);
    }
    public void CloseMapImg()
    {
        miniMapImg.SetActive(false);
    }

    public void GamePuase()
    {
        isPaused = true;
        Time.timeScale = 0;
    }
    public void GamePlay()
    {
        isPaused = false;
        Time.timeScale = 1;
    }
    public void CloseVideo()
    {
        videoWindow.SetActive(false);
        isOpenning = false;
    }

    public void OpenKeyExplain()
    {
        GamePuase();
        explainUI.SetActive(true);
    }
    public void CloseKeyExplain()
    {
        GamePlay();
        explainUI.SetActive(false);
    }
    public void LoadSceneWithLoading(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingWindow.SetActive(true);

        yield return new WaitForSecondsRealtime(0.1f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        canvas.SetActive(true);
        loadingWindow.SetActive(false); 
    }

}
