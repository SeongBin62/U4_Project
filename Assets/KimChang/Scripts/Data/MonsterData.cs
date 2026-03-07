using UnityEngine;

[System.Serializable]
public class MonsterData
{
    public int id;
    public string name;
    public float health;
    public float damage;
    public AttackType attackType;
    public float attackRange;
    public float attackDelay;
    public int defense;
    public int speed;
    public string spawnLocation;
    public int energyReward;
    public PatrolType parol;
    public float partolRange;
    public float deathTime;
    public int phase;

    public MonsterData(int id, string name, float health, float damage, AttackType attackType,float attackRange,float attackDelay,int defense, int speed,
                       string spawnLocation, int energyReward, PatrolType patrol,float partolRange, float deathTime, int phase)
    {
        this.id = id;
        this.name = name;
        this.health = health;
        this.damage = damage;
        this.attackType = attackType;
        this.attackRange = attackRange;
        this.attackDelay = attackDelay;
        this.defense = defense;
        this.speed = speed;
        this.spawnLocation = spawnLocation;
        this.energyReward = energyReward;
        this.parol = patrol;
        this.partolRange = partolRange;
        this.deathTime = deathTime;
        this.phase = phase;
    }
}
public enum AttackType 
{ 
    None,
    Circle,
    Box
}

public enum PatrolType 
{ 
    Fixed,
    Patrol,
    Chase,
    Leash,
    Pattern
}

