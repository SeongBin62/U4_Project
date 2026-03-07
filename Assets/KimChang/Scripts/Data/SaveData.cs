using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SavePlayerData
{
    public int id;
    public int playerHealth;
    public int playerMaxHealth;
    public int playerEnergy;
    public int maxEnergy;
    public float playerDefense;
    public float playerAttack;
    public float speed;
    public List<ItemData> items;
    public int equipItemId;
    public int money;
    public int selectType;
    public float bgmVolume;
    public float allVolume;
    public int currentPortalId;
    public string lastHitMonsterName;
    public List<int> killMonsterIds;
    public List<int> killUnique;
    public List<int> visitMapIds;
    public int spawnId;
    public SavePlayerData(int id, int playerHealth, int playerMaxHealth, int playerEnergy, int maxEnergy, float playerDefense, float playerAttack,
        float speed, List<ItemData> items, int equipItemId, int money, int selectType, float bgmVolume, float allVolume, 
        int currentPortalId, string lastHitMonsterName, List<int> killMonsterIds, List<int> visitMapIds, int spawnId, List<int> killUnique)
    {
        this.id = id;
        this.playerHealth = playerHealth;
        this.playerMaxHealth = playerMaxHealth;
        this.playerEnergy = playerEnergy;
        this.maxEnergy = maxEnergy;
        this.playerDefense = playerDefense;
        this.playerAttack = playerAttack;
        this.speed = speed;
        this.items = items ?? new List<ItemData>();
        this.equipItemId = equipItemId;
        this.money = money;
        this.selectType = selectType;
        this.bgmVolume = bgmVolume;
        this.allVolume = allVolume;
        this.currentPortalId = currentPortalId;
        this.lastHitMonsterName = lastHitMonsterName;
        this.killMonsterIds = killMonsterIds;
        this.visitMapIds = visitMapIds;
        this.spawnId = spawnId;
        this.killUnique = killUnique;
    }
}

public class SaveData
{
    private string GetFilePath(int id)
    {
        return Application.persistentDataPath + $"/PlayerData_{id}.json";
    }

    public void SaveDataFile(int id)
    {
        GameData.Instance.SetPlayerHealth(10);
        SavePlayerData savePlayerData = GameData.Instance.GetPlayerData(id);
        string jsonData = JsonUtility.ToJson(savePlayerData, true);
        File.WriteAllText(GetFilePath(id), jsonData);
    }

    public void LoadDataFile(int id)
    {
        string filePath = GetFilePath(id);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"저장된 파일이 존재하지 않습니다: {filePath}");
            return;
        }
        try
        {
            string jsonData = File.ReadAllText(filePath);
            SavePlayerData savePlayerData = JsonUtility.FromJson<SavePlayerData>(jsonData);
            GameData.Instance.SetLoadedData(savePlayerData);
            GameData.Instance.SetPlayerHealth(10);
        }
        catch (Exception e)
        {
            Debug.LogError($"데이터 불러오기 실패: {e.Message}");
        }
    }
    public void DeleteDataFile(int id)
    {
        string filePath = GetFilePath(id);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"파일 삭제 완료: {filePath}");
        }
        else
        {
            Debug.LogWarning($"삭제할 파일이 없습니다: {filePath}");
        }
    }



}
