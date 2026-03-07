using System.Collections.Generic;
using UnityEngine;

public static class MonsterDataBase
{
    private static Dictionary<int, MonsterData> monsterDictionary = new();

    static MonsterDataBase()
    {
        InitializeDatabase();
    }

    private static void InitializeDatabase()
    {
        monsterDictionary.Add(0, new MonsterData(0, "기억망령", 21, 1,AttackType.Circle,1.5f, 2,0,10, "천상궁 복도1,2", 40, PatrolType.Chase,0,0.6f,1));
        monsterDictionary.Add(1, new MonsterData(1, "먼지뭉치", 8, 1, AttackType.Circle, 1, 4, 0,5, "천상궁 복도1,2, 암자도서관, 성전복도", 20, PatrolType.Patrol,5,0.6f,1));
        monsterDictionary.Add(2, new MonsterData(2, "책벌래", 6, 1, AttackType.Circle, 2f, 3, 0, 2, "천상궁 복도2, 암자도서관", 10, PatrolType.Leash, 5, 0.7f,1));
        monsterDictionary.Add(3, new MonsterData(3, "원숭이유령", 84, 1, AttackType.Circle, 3, 3,0, 12, "고서보관실", 140, PatrolType.Patrol, 5, 0, 1));
        monsterDictionary.Add(4, new MonsterData(4, "이비리", 58, 1, AttackType.None, 0, 2.5f,0, 8, "이비리의 굴", 140, PatrolType.Pattern, 0, 1, 1));
        monsterDictionary.Add(5, new MonsterData(5, "문지기", 73, 1, AttackType.None, 0, 1.5f,0,10,"성전 복도", 130, PatrolType.Pattern, 0, 1.9f,2));

    }
    public static MonsterData GetMonsterById(int id)
    {
        if (monsterDictionary.ContainsKey(id))
        {
            return monsterDictionary[id];
        }
        else
        {
            return null;
        }
    }

    
}
