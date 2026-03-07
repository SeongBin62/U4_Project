using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Button leftWindow;
    [SerializeField] private Button selectWindow;
    [SerializeField] private Button rightWindow;
    [SerializeField] private GameObject ownedItem;
    [SerializeField] private GameObject selectItem;
    [SerializeField] private GameObject backInventory;
    [SerializeField] private GameObject frontInventory;
    [SerializeField] private Image equipItemImg;

    [SerializeField] private Button backBtn;

    private Dictionary<string, Sprite> spriteCache = new();
    private Transform[] itemSlots;
    private Button[] itemButtons;
    private ItemData[] itemDataSlots;

    private int selectType=1;

    public bool isBackOpen =false;
    private void Awake()
    {
        CacheItemSlots();
    }

    private void Start()
    {
        CacheItemSlots();

        backBtn.onClick.AddListener(ClickBack);
        leftWindow.onClick.AddListener(ClickLeft);
        rightWindow.onClick.AddListener(ClickRight);
        OnEnableFrontInventory();
        UpdateInventory();
    }
    //frotIventory가 활성화
    public void OnEnableFrontInventory()
    {
        isBackOpen = false;
        frontInventory.SetActive(true);
        int equipItemId = GameData.Instance.GetEquipItemId();
        backInventory.SetActive(false);
        if (equipItemId == -1)
        {
            equipItemImg.gameObject.SetActive(false);
            return;
        }
        equipItemImg.gameObject.SetActive(true);
        Sprite selectedSprite = null;
        switch (equipItemId)
        {
            case 0:
                selectedSprite = GetCachedSprite("Item0");
                break;
            case 1:
                selectedSprite = GetCachedSprite("Item1");
                break;
            case 2:
                selectedSprite = GetCachedSprite("Item2");
                break;
            case 3:
                selectedSprite = GetCachedSprite("Item3");
                break;
            default:
                Debug.LogWarning("알 수 없는 equipItemId: " + equipItemId);
                break;
        }

        equipItemImg.sprite = selectedSprite;
    }

    // backInventory가 활성화
    public void OnEnableBackInventory()
    {
        isBackOpen = true;
        backInventory.SetActive(true);
        selectType = GameData.Instance.GetSelectType();

        int equipItemId = GameData.Instance.GetEquipItemId();

        // equipItemId가 -1이면 없는 것으로 간주합니다.
        OnItemClick(equipItemId == -1 ? -1 : equipItemId, true);

        frontInventory.SetActive(false);
        UpdateInventory();
    }
    private void CacheItemSlots()
    {
        // 비활성화 포함 직속 자식만 수집
        var allChildren = ownedItem.GetComponentsInChildren<Transform>(true)
            .Where(t => t.parent == ownedItem.transform && t != ownedItem.transform)
            .ToArray();

        int totalSlots = allChildren.Length;
        itemSlots = new Transform[totalSlots];
        itemButtons = new Button[totalSlots];
        itemDataSlots = new ItemData[totalSlots];

        for (int i = 0; i < totalSlots; i++)
        {
            itemSlots[i] = allChildren[i];

            // 버튼 없으면 추가
            itemButtons[i] = itemSlots[i].GetComponent<Button>();
            if (itemButtons[i] == null)
                itemButtons[i] = itemSlots[i].gameObject.AddComponent<Button>();

            int index = i;
            itemButtons[i].onClick.AddListener(() => OnItemClick(index));
        }
    }

    //이미지 가져오기
    private Sprite GetCachedSprite(string key)
    {
        return SpriteCache.GetSprite($"{key}");
    }
    //인벤 배열 업데이트
    private void UpdateInventory()
    {
        GameData.Instance.SetSelectType(selectType);
        List<ItemData> inventory = GameData.Instance.GetInventoryByType(selectType);

        UpdateWindowPositions();

        int index = 0;
        int totalSlots = itemSlots.Length;

        foreach (var item in inventory)
        {
            if (index >= totalSlots) break;

            itemDataSlots[index] = item;
            Transform itemSlot = itemSlots[index];

            Transform child = itemSlot.GetChild(0);
            
            child.GetComponent<Image>().sprite = GetCachedSprite($"Item{item.id}");
            child.gameObject.SetActive(true);

            index++;
        }

        for (int i = index; i < totalSlots; i++)
        {
            itemDataSlots[i] = null;
            itemSlots[i].GetChild(0).gameObject.SetActive(false);
        }
    }
    //backInventory에서 인벤 아이템 눌렀을 때 
    private void OnItemClick(int index,bool isId=false)
    {
        ItemData item =null;
        if (isId)
        {
            item = ItemDataBase.GetItemById(index);
        }
        else
        {
            if (index != -1)
            {
                item = itemDataSlots[index];
            }
        }
        if (index >= 0 && index < itemDataSlots.Length && itemDataSlots[index] != null)
        {
            int itemId = itemDataSlots[index].id >= 0 ? itemDataSlots[index].id : -1;
            GameData.Instance.SetEquipItemId(itemId);
        }
        if (item != null)
        {
            selectItem.SetActive(true);

            selectItem.transform.GetChild(0).GetComponent<Image>().sprite = GetCachedSprite($"Item{item.id}");
            selectItem.transform.GetChild(1).GetComponent<Image>().sprite = GetCachedSprite($"ItemText{item.id}");
        }
        else
        {
            selectItem.SetActive(false);
        }
    }
    private void UpdateWindowPositions()
    {
        UpdateWindow(leftWindow.gameObject, (selectType + 2) % 3);
        UpdateWindow(selectWindow.gameObject, selectType);
        UpdateWindow(rightWindow.gameObject, (selectType + 1) % 3);
    }
    private void UpdateWindow(GameObject window, int activeIndex)
    {
        for (int i = 0; i < window.transform.childCount; i++)
        {
            window.transform.GetChild(i).gameObject.SetActive(i == activeIndex);
        }
    }
    private void ClickLeft()
    {
        selectType = (selectType + 2) % 3;
        UpdateInventory();
    }
    private void ClickRight()
    {
        selectType = (selectType + 1) % 3;
        UpdateInventory();
    }
    private void ClickBack()
    {
        OnEnableFrontInventory();
        backInventory.SetActive(false);
    }

}
