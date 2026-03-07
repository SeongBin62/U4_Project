using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase
{
    private static Dictionary<int, ItemData> itemDictionary = new(); 

    static ItemDataBase()
    {
        InitializeDatabase();
    }

    private static void InitializeDatabase()
    {
        itemDictionary.Add(0, new ItemData(0, "나는 새의 깃", "Bell",1 ,3, 0, 3, "속도 +3"));
        itemDictionary.Add(1, new ItemData(1, "조화 보자기", "Bell", 1,2, 0, 5, "고리 슬롯 +5"));
        itemDictionary.Add(2, new ItemData(2, "체리빛 격상석", "Stone",2, 5, 1, 4, "플레이어 하트 +4"));
        itemDictionary.Add(3, new ItemData(3, "치유의 울림", "Bell", 1, 1, 30, -1, "에너지를 30 소모해, 3초간 [기도] 하며 체력을 회복할 수 있다."));
    }
    public static ItemData GetItemById(int id)
    {
        return itemDictionary[id];
    }
}
