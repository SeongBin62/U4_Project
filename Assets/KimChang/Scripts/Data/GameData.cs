
using System.Collections.Generic;
using System.Linq;

using UnityEngine;


public class GameData
{
    private static GameData instance;  // 싱글톤 인스턴스
    private Dictionary<int, List<ItemData>> inventory = new(); // typeId 기준으로 관리

    private int id;
    private int playerHealth = 4;
    private int baseHealth;
    private int playerMaxHealth = 4;
    private int playerEnergy;
    private int baseEnergy;
    private float playerDefense;
    private float baseDefense;
    private float playerAttack;
    private float baseAttack;
    private float speed;
    private float baseSpeed;
    private int maxEnergy;
    private int hitEnergy;
    private int equipItemId;
    private int money;
    private int selectType;
    private bool isPaused = false;
    private float bgmVolume = 0.5f;
    private float allVolume = 1.0f;
    private float brightValue = 1;
    private readonly float defenseFactor = 20;
    private string lastHitMonsterName = "";
    private int currentPortalId = -1;
    private List<int> killMonsterIds = new();
    private HashSet<int> killIds = new();
    private HashSet<int> visitMapIds = new();
    private float scaleX = 1f;
    private int spawnId = -1;
    
    private static readonly Dictionary<int, string> mapNameDictionary = new()
{
    { -1, "공주의 방" }, { 1, "공주의 방" },
    { 0, "천상궁 1" }, { 14, "천상궁 1" }, { 15, "천상궁 1" },
    { 2, "이비리 굴 1" }, { 5, "이비리 굴 1" },
    { 4, "이비리 굴 2" }, { 7, "이비리 굴 2" },
    { 6, "이비리 굴 3" }, { 10, "이비리 굴 3" }, { 11, "이비리 굴 3" },
    { 8, "이비리 굴 4" }, { 9, "이비리 굴 4" },
    { 12, "천상궁 2" }, { 13, "천상궁 2" }, { 17, "천상궁 2" },
    { 16, "암자도서관 복도" }
};
    // 생성자를 private으로 설정하여 외부에서 인스턴스화 방지
    private GameData()
    {
        id = 1;
        playerAttack = 6;// 공격력
        baseAttack = playerAttack;
        playerEnergy = 0;
        baseEnergy = playerEnergy;
        playerHealth = 4; // 기본 체력 설정
        baseHealth = playerHealth;
        playerMaxHealth = 4;
        playerDefense = 0;//회피치
        baseDefense = playerDefense;
        speed = 10;
        baseSpeed = speed;
        maxEnergy = 100;
        hitEnergy = 10;
        equipItemId = 3;
        money = 0;
        bgmVolume = 0.5f;
        brightValue = 1;
        selectType = 1;
        inventory = new();
        AddItem(3);

    }

    public static GameData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameData();
            }
            return instance;
        }
    }

    // 체력 값 가져오기
    public int GetPlayerHealth()
    {
        if (playerHealth > GetMaxHealth()) playerHealth = GetMaxHealth();
        return playerHealth;
    }

    // 체력 설정하기
    public void SetPlayerHealth(int health)
    {
        playerHealth = Mathf.Min(GetMaxHealth(), health);
    }

    // 체력 감소 (데미지 적용)
    public int DecreaseHealth(string monsterName, int damage, bool isTrueDamage = false)
    {
        if (isTrueDamage)
        {
            playerHealth = Mathf.Max(0, playerHealth - damage);
            lastHitMonsterName = monsterName;
            return damage;
        }
        else if (playerHealth > 0 && CalculatePlayerDefense())
        {
            playerHealth = Mathf.Max(0, playerHealth - damage);
            lastHitMonsterName = monsterName;
            return 1;
        }
        return 0;
    }

    // 체력 회복
    public void IncreaseHealth(int healAmount)
    {
        playerHealth = Mathf.Min(playerHealth + healAmount, GetMaxHealth());
    }
    // 
    public int GetMaxHealth()
    {
        int addHealth = 0;
        addHealth += equipItemId == 2 ? 4 : 0;
        return playerMaxHealth+ addHealth;
    }

    //최대 애너지 가져와기
    public int GetMaxEnergy()
    {
        return maxEnergy;
    }
    //최대 애너지 수정
    public void SetMaxEnergy(int energy)
    {
        maxEnergy = energy;
    }
    //애너지 가져오기
    public int GetPlayerEnergy()
    {
        return playerEnergy;
    }
    //애너지 더하기
    public void AddPlayerEnergy(int e)
    {
        playerEnergy = Mathf.Clamp(playerEnergy + e, 0, maxEnergy);
    }
    //애너지 설정
    public void SetPlayerEnergy(int e)
    {
        playerEnergy = e;
    }
    //타격 애너지 가져오기
    public int GetHitEnergy()
    {
        return hitEnergy;
    }
    //공격력 가져오기
    public float GetPlayerAttack()
    {
        return playerAttack;
    }
    //공격력 설정
    public void SetPlayerAttack(float attack)
    {
        playerAttack = attack;
    }

    // 플레이어 회피 계산
    private bool CalculatePlayerDefense()
    {
        float dodgeRate = playerDefense / (playerDefense + defenseFactor);
        return Random.value > dodgeRate;
    }

    // BGM 볼륨 가져오기
    public float GetBgmVolume()
    {
        return bgmVolume;
    }

    // BGM 볼륨 설정
    public void SetBgmVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
    }

    // 전체 볼륨 가져오기
    public float GetAllVolume()
    {
        return allVolume;
    }

    // 전체 볼륨 설정
    public void SetAllVolume(float volume)
    {
        allVolume = Mathf.Clamp01(volume);
    }
    //게임 밝기 가져오기
    public float GetBrightValue()
    {
        return brightValue;
    }
    //게임 밝기 수정
    public void SetBrightValue(float bright)
    {
        brightValue = bright;
    }
    // 게임 멈춤 유무 가져오기
    public bool IsPaused()
    {
        return isPaused;
    }

    // 게임 상태 변경
    public void ChangePaused()
    {
        isPaused = !isPaused;
    }
    //선택 타입 가져오기
    public int GetSelectType()
    {
        return selectType;
    }
    //선택 타입 수정
    public void SetSelectType(int type)
    {
        selectType = type;
    }
    // 해당 typeId의 아이템만 가져오기
    public List<ItemData> GetInventoryByType(int typeId)
    {
        if (inventory.ContainsKey(typeId))
        {
            return inventory[typeId];
        }
        return new List<ItemData>();
    }
    // 아이템 추가하기
    public bool AddItem(int itemId)
    {
        ItemData item = ItemDataBase.GetItemById(itemId);
        if (item == null)
        {
            Debug.LogError($"아이템 ID {itemId} 를 찾을 수 없습니다.");
            return false;
        }

        if (!inventory.ContainsKey(item.typeId))
        {
            inventory[item.typeId] = new List<ItemData>();
        }

        if (!inventory[item.typeId].Contains(item))
        {
            inventory[item.typeId].Add(item);
            return true;
        }

        Debug.Log($"아이템 {item.name} (ID: {itemId}) 이미 보유 중.");
        return false;
    }
    // 아이템 삭제하기
    public bool RemoveItem(int itemId)
    {
        ItemData item = ItemDataBase.GetItemById(itemId);
        if (item != null && inventory.ContainsKey(item.typeId))
        {
            return inventory[item.typeId].Remove(item);
        }

        Debug.LogError($"아이템 ID {itemId} 를 보유하고 있지 않습니다.");
        return false;
    }
    //장착 아이템 확인
    public int GetEquipItemId()
    {
        return equipItemId;
    }
    //장착 아이템 변경
    public void SetEquipItemId(int id)
    {
        equipItemId = id;
        switch (equipItemId)
        {
            case 0:
                speed = baseSpeed + 3;
                break;
            case 1:
                break;
            case 2:

                break;
        }

    }
    // 특정 아이템 보유 여부 확인
    public bool HasItem(int itemId)
    {
        ItemData item = ItemDataBase.GetItemById(itemId);
        return item != null && inventory.ContainsKey(item.typeId) && inventory[item.typeId].Contains(item);
    }
    public string GetLastHitMonsterName()
    {
        return lastHitMonsterName;
    }
    //
    public SavePlayerData GetPlayerData(int id = 0)
    {
        SavePlayerData saveData = new(id, playerHealth, playerMaxHealth, playerEnergy, maxEnergy, playerDefense, playerAttack,
            speed, inventory.Values.SelectMany(list => list).ToList(), equipItemId, money, selectType, bgmVolume, allVolume,
            currentPortalId, lastHitMonsterName, killMonsterIds, visitMapIds.ToList(),spawnId,killIds.ToList());

        return saveData;
    }
    public void SetLoadedData(SavePlayerData saveData)
    {
        id = saveData.id;
        playerHealth = saveData.playerHealth;
        playerMaxHealth = saveData.playerMaxHealth;
        playerEnergy = saveData.playerEnergy;
        maxEnergy = saveData.maxEnergy;
        playerDefense = saveData.playerDefense;
        playerAttack = saveData.playerAttack;
        speed = saveData.speed;
        inventory = saveData.items
            .GroupBy(item => item.typeId)
            .ToDictionary(g => g.Key, g => g.ToList());
        equipItemId = saveData.equipItemId;
        money = saveData.money;
        selectType = saveData.selectType;
        bgmVolume = saveData.bgmVolume;
        allVolume = saveData.allVolume;
        currentPortalId = saveData.currentPortalId;
        lastHitMonsterName = saveData.lastHitMonsterName;
        killMonsterIds = saveData.killMonsterIds;

        killIds.UnionWith(saveData.killUnique);
        spawnId = saveData.spawnId;
    }
    public int GetCurrentPortalId()
    {
        return currentPortalId;
    }

    public void SetPortalId(int id)
    {
        currentPortalId = id;
    }

    public Vector3 GetSpawnPosition()
    {
        return currentPortalId switch
        {
            -1 => new Vector2(8, -2.1f),
            0 => new Vector2(8, 0f),
            1 => new Vector2(-10f, -1.8f),
            2 => new Vector2(-10, -5),
            3 => new Vector2(8.1f, 14.8f),
            4 => new Vector2(-4, -2),
            5 => new Vector2(11, -0.5f),
            6 => new Vector2(-10, 0),
            7 => new Vector2(4, -2),
            8 => new Vector2(-10, 0.5f),
            9 => new Vector2(-10, 3),
            10 => new Vector2(15.5f, -1.4f),
            11 => new Vector2(15.5f, 6.5f),
            12 => new Vector2(28.2f, 13),
            13 => new Vector2(28.2f, 5.5f),
            14 => new Vector2(-19, 17),
            15 => new Vector2(-19, 10),
            16 => new Vector2(0, 2),
            17 => new Vector2(-6.6f, -7),
            18 => new Vector2(9, -2.7f),
            19 => new Vector2(15.5f, -4.3f),
            20 => new Vector2(-3, -22.5f),
            21 => new Vector2(8.2f, -23),
            22 => new Vector2(19.7f, 8.6f),
            23 => new Vector2(-3.5f, -1f),
            24 => new Vector2(13.8f, -2),
            25 => new Vector2(-1.6f, -8),

            //천상궁 2 저장
            27 => new Vector2(-0.4f, 15),
            28 => new Vector2(36, -2),
            29 => new Vector2(8, 20.5f),
            // 추가 포탈은 여기 계속 작성
            _ => Vector3.zero,
        };
    }
    public int GetMapById()
    {
        return currentPortalId switch
        {
            -1 => 1,
            0 => 2,
            1 => 1,
            2 => 3,
            3 => 2,
            4 => 4,
            5 => 3,
            6 => 5,
            7 => 4,
            8 => 6,
            9 => 6,
            10 => 5,
            11 => 5,
            12 => 7,
            13 => 7,
            14 => 2,
            15 => 2,
            16 => 8,
            17 => 7,
            18 => 9,
            19 => 9,
            20 => 8,
            21 => 8,
            22 => 11,
            23 => 10,
            24 => 10,
            25 => 9,
            26 => 12,

            27 => 7,
            28 => 11,
            29 => 2,
            _ => 1,
        };
    }
    public string GetMapNameById(int mapId)
    {
        return mapNameDictionary.TryGetValue(mapId, out var name) ? name : "알 수 없음";
    }
    public List<int> GetKillMonsterIds()
    {
        return killMonsterIds;
    }
    public void AddKillMonsterId(int id)
    {
        killMonsterIds.Add(id);
    }
    public HashSet<int> GetkillIds()
    {
        return killIds;
    }
    public void AddKillIds(int id)
    {
        killIds.Add(id);
    }

    public int GetPlayerId()
    {
        return id;
    }
    public void SetPlayerId(int playerId)
    {
        id = playerId;
    }
    public int GetMoney()
    {
        return money;
    }
    public void SetMoney(int money)
    {
        this.money = money;
    }
    public void AddMoney(int add)
    {
        money += add;
    }
    public List<int> GetMapIds()
    {
        return visitMapIds.ToList();
    }
    public void AddMapId(int mapId)
    {
        visitMapIds.Add(mapId);
    }

    public float GetScaleX()
    {
        return scaleX;
    }
    public void SetScaleX(float x)
    {
        scaleX = x;
    }
    // 게임 데이터를 완전히 초기화하는 함수
    public void ResetGameData()
    {
        id = 1;
        playerHealth = baseHealth = playerMaxHealth = 4;
        playerEnergy = baseEnergy = 0;
        playerDefense = baseDefense = 0;
        playerAttack = baseAttack = 6;
        speed = baseSpeed = 10;
        maxEnergy = 100;
        hitEnergy = 10;
        equipItemId = 3;
        money = 0;
        selectType = 1;
        bgmVolume = 0.5f;
        allVolume = 1.0f;
        brightValue = 1;
        currentPortalId = -1;
        lastHitMonsterName = "";

        killMonsterIds.Clear();
        killIds.Clear();
        visitMapIds.Clear();

        scaleX = 1f;

        inventory.Clear();
        AddItem(3);
    }
    public int GetSpawnId()
    {
        return spawnId;
    }
    public void SetSpawnId(int id)
    {
        spawnId = id;
    }

}
