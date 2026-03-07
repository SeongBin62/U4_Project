using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MonsterStatus : MonoBehaviour
{
    [SerializeField] private List<GameObject> dropItem = new();
    [SerializeField] private Animator animator;
    [SerializeField] public LayerMask groundLayer;
    public GameObject stateUI=null;
    public Image hpBar;

    public int uniqueId = -1;
    public int id = 0;
    public string monsterName;
    public float maxHealth;
    public float health;
    public float damage;
    public AttackType attackType;
    public float attackRange;
    public float attackDelay;
    public int defense;
    public int speed;
    public string spawnLocation;
    public int energyReward;
    public PatrolType patrol;
    public float patrolRange;
    public float deathTime;
    public int phase;

    private Coroutine deathCoroutine;
    private bool isDead = false;
    public bool IsDamaged => animator.GetBool("IsDamaged");
    public bool IsDead => isDead;
    public bool isImmun =false;

    private void Awake()
    {
        if (uniqueId == -1) return;
        HashSet<int> killIds = GameData.Instance.GetkillIds();
        if(!killIds.Contains(uniqueId)) return;
        gameObject.SetActive(false);
    }
    void OnEnable()
    {
        InitializeMonsterData();
        if (transform.childCount > 0)
        {
            animator = transform.GetChild(0).GetComponent<Animator>();
        }
    }

    public void OpenHpBar()
    {
        if(stateUI == null) return;
        stateUI.SetActive(true);
    }
    public void UpdateHp()
    {
        hpBar.fillAmount = health / maxHealth;
    }
    public void CloseHpBar()
    {
        if (stateUI == null) return;
        stateUI.SetActive(false);
    }

    private void OnDisable()
    {
        if (isDead && gameObject.scene.isLoaded)
        {
            DropItem();
        }
    }

    private void InitializeMonsterData()
    {
        MonsterData monsterData = MonsterDataBase.GetMonsterById(id);
        if (monsterData == null)
        {
            Debug.LogError($"MonsterDataBase에서 ID {id}에 해당하는 몬스터를 찾을 수 없습니다.");
            return;
        }

        monsterName = monsterData.name;
        maxHealth =monsterData.health;
        health = monsterData.health;
        damage = monsterData.damage;
        attackType = monsterData.attackType;
        attackRange = monsterData.attackRange;
        attackDelay = monsterData.attackDelay;
        speed = monsterData.speed;
        defense = monsterData.defense;
        spawnLocation = monsterData.spawnLocation;
        energyReward = monsterData.energyReward;
        patrol = monsterData.parol;
        patrolRange = monsterData.partolRange;
        deathTime = monsterData.deathTime;
        phase = monsterData.phase;
    }

    private void DropItem()
    {
        switch (id)
        {
            case 3:
                Instantiate(dropItem[0], transform.position, Quaternion.identity);
                break;
            case 4:
                Instantiate(dropItem[2], transform.position, Quaternion.identity);
                break;
            case 5:
                Instantiate(dropItem[1], transform.position, Quaternion.identity);
                break;
        }
    }

    public int SecondPhase()
    {
        if (phase < 2) return 1;
        if (health > (maxHealth * 0.5f)) return 1;
        animator.SetBool("SecondPhase", true);
        return 2;
    }
    public void MonsterDamaged()
    {
        var animatorParameters = animator.parameters;

        bool hasIsDamagedParam = animatorParameters.Any(param => param.name == "IsDamaged");

        if (hasIsDamagedParam && !animator.GetBool("IsAttack"))
        {
            animator.SetBool("IsDamaged", true);
            StartCoroutine(ResetDamagedState());
        }
    }

    private IEnumerator ResetDamagedState()
    {
        float wait = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(wait);

        animator.SetBool("IsDamaged", false);
    }


    public void MonsterDie()
    {
        if (isDead) return;
        isDead = true;

        if (deathCoroutine != null)
            StopCoroutine(deathCoroutine);
        animator.SetBool("IsAttack", false);
        MonsterPatrol patrol = GetComponent<MonsterPatrol>();
        if (patrol != null)
        {
            patrol.SetDead();
            patrol.StopAllCoroutines();
            patrol.enabled = false;
        }
        else
        {
            GateKeeper gateKeeper = GetComponent<GateKeeper>();
            if (gateKeeper != null)
            {
                gateKeeper.StopAllCoroutines();
                gateKeeper.enabled = false;
            }
        }

        CloseHpBar();
        int money = Math.Max(1,(int)(maxHealth * 0.5f));
        GameData.Instance.AddMoney(money);
        animator.SetTrigger("IsDie");
        isImmun=true;
        GameData.Instance.AddKillMonsterId(id);
        GameData.Instance.AddKillIds(uniqueId);
        if(id ==4)
        {
            AudioManager.instance.PlayBgmByBoss(3);
        }
        else if(id == 5)
        {
            AudioManager.instance.PlayBgmByBoss(6);
        }
        deathCoroutine = StartCoroutine(HandleDeath(deathTime));
    }


    private IEnumerator HandleDeath(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
