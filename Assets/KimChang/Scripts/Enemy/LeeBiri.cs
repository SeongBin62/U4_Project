using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeeBiri : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player;
    [SerializeField] private MonsterStatus monsterStatus;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameMapUI gameMapUI;
    [SerializeField] private GameObject rockObject;
    [SerializeField] private RockAttack rockAttack;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject map;

    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;

    private Vector2 startPos= new(0,-0.75f);
    private bool hasHitPlayer = false;
    private bool isArrived = false;
    private float attackMove = 0.15f;

    private Vector2? debugBoxCenter = null;
    private Vector2 debugBoxSize = Vector2.zero;
    private float debugBoxDuration = 0f;


    private void Start()
    {
        List<int> kills = GameData.Instance.GetKillMonsterIds();
        if (kills.Contains(4))
        {
            gameObject.SetActive(false);
            return;
        }

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        gameMapUI ??= FindObjectOfType<GameMapUI>();
        if(rb == null) rb = GetComponent<Rigidbody2D>();
        if (monsterStatus == null)
        {
            monsterStatus = GetComponent<MonsterStatus>();
        }
        rockObject.SetActive(false);
        animator.SetBool("IsWalk", true);
    }

    private void Update()
    {
        if (player == null || monsterStatus == null || monsterStatus.health <= 0)
            return;
        if (monsterStatus.stateUI.activeSelf) monsterStatus.UpdateHp(); 
        if (isArrived) return;

        float distanceX = Mathf.Abs(transform.position.x - player.position.x);

        if (distanceX > 0.05f)
        {
            animator.SetBool("IsWalk", true);

            float directionX = Mathf.Sign(player.position.x - transform.position.x);
            Vector3 move = new (directionX, 0, 0);

            Vector3 newPosition = transform.position + move * monsterStatus.speed * Time.deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            transform.position = newPosition;

            transform.localScale = new (directionX > 0 ? 1 : -1, 1, 1);
        }
        else
        {
            monsterStatus.OpenHpBar();
            animator.SetBool("IsWalk", false);
            isArrived = true;
            AudioManager.instance.PlayBgmByBoss(monsterStatus.id);
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return PerformAttack("IsAttack1");
            yield return new WaitForSeconds(2f);

            yield return PerformAttack("IsAttack2");

            yield return PerformAttack("IsAttack3");
            yield return PerformAttack("IsDig");
            yield return PerformAttack("IsScream");

            yield return PerformAttack("IsGoGround");

            yield return new WaitForSeconds(2f);
        }
    }
    private IEnumerator PerformAttack(string boolName)
    {
        if (boolName == "IsAttack1" || boolName == "IsAttack2")
        {
            rb.velocity = Vector2.zero;

            transform.position = startPos;
            Vector3 pos = transform.position;
            float clampedX = Mathf.Clamp(player.position.x, minX, maxX);
            transform.position = new Vector3(clampedX, pos.y, pos.z);
            yield return new WaitForSeconds(0.3f);
        }
        if (boolName == "IsAttack1")
        {
            monsterStatus.isImmun = true;
        }

        if (boolName == "IsAttack3")
        {
            float playerX = player.position.x;
            float myX = transform.position.x;
            float newScaleX = playerX <= myX ? 1f : -1f;
            transform.localScale = new Vector3(newScaleX, 1, 1);
        }

        animator.SetBool(boolName, true);
        hasHitPlayer = false;
        string expectedStateName = boolName.Replace("Is", "");

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(expectedStateName))
            yield return null;

        float animTime = animator.GetCurrentAnimatorStateInfo(0).length - 0.16f;

        if (boolName == "IsAttack1")
        {
            yield return new WaitForSeconds(0.14f);
            rb.gravityScale = 20;
            monsterStatus.isImmun = false;
            yield return new WaitForSeconds(0.17f);
            
            Vector2 center = (Vector2)transform.position;
            Vector2 size = new Vector2(6f, 10f);
            CheckAttackBox(center, size, 0.16f);

            yield return new WaitForSeconds(1.5f);
            monsterStatus.isImmun =true;

            yield return new WaitForSeconds(animTime - 1.5f);
        }
        else if (boolName == "IsAttack2")
        {
            float waitBeforeAttack = 2.417f;
            float attackDuration = 0.16f;
            float remaining = Mathf.Max(0f, animTime - waitBeforeAttack - attackDuration);
            RockThrow();
            yield return new WaitForSeconds(waitBeforeAttack-0.15f);
            monsterStatus.isImmun = false;
            rb.gravityScale = 20;
            yield return new WaitForSeconds(0.15f);
            Vector2 center = (Vector2)transform.position;
            Vector2 size = new Vector2(6f, 10f);
            CheckAttackBox(center, size, attackDuration);

            yield return new WaitForSeconds(attackDuration + remaining);
        }

        else if (boolName == "IsAttack3")
        {
            yield return new WaitForSeconds(1f);

            Vector2 offset = new Vector2(transform.localScale.x > 0 ? -3f : 3f, -1);
            Vector2 center = (Vector2)transform.position + offset;
            Vector2 size = new Vector2(2f, 2f);
            CheckAttackBox(center, size, 0.2f);
            yield return new WaitForSeconds(0.2f);

            yield return new WaitForSeconds(0.33f);
            CheckAttackBox(center, size, 0.1f);

            float moveDirection = -transform.localScale.x;
            Vector3 move = new(attackMove * moveDirection, 0, 0);
            transform.position += move;
        }
        else if (boolName == "IsDig")
        {
            yield return new WaitForSeconds(0.583f);

            Vector2 offset = new Vector2(transform.localScale.x > 0 ? -2.5f : 2.5f, -1);
            Vector2 center = (Vector2)transform.position + offset;
            Vector2 size = new Vector2(2f, 2f);
            CheckAttackBox(center, size, 0.583f);
            yield return new WaitForSeconds(0.583f);
        }
        else if(boolName == "IsGoGround")
        {
            yield return new WaitForSeconds(0.583f);
            monsterStatus.isImmun = true;
        }
        else
        {
            yield return new WaitForSeconds(animTime);
        }

        animator.SetBool(boolName, false);
    }
    private void CheckAttackBox(Vector2 center, Vector2 size, float duration)
    {
        if (hasHitPlayer)
            return;

        Collider2D hit = Physics2D.OverlapBox(center, size, 0, playerLayer);

        debugBoxCenter = center;
        debugBoxSize = size;
        debugBoxDuration = duration;
        StartCoroutine(ClearDebugBoxAfterTime(duration));

        if (hit != null)
        {
            AttackPlayer();
            hasHitPlayer = true;
        }
    }

    private void RockThrow()
    {
        rockObject.transform.position = new Vector2 (transform.position.x,transform.position.y+3f);
        rockObject.SetActive(true);
        rockAttack ??= rockObject.GetComponent<RockAttack>();
        rockAttack.RockAttackPlayer(player);
    }


    public void AttackPlayer()
    {
        if (GameData.Instance == null) return;
        int damage = GameData.Instance.DecreaseHealth(monsterStatus.monsterName, 1);
        gameMapUI.ShowDamageText(transform.position, damage, true);
    }


    private IEnumerator ClearDebugBoxAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        debugBoxCenter = null;
        debugBoxSize = Vector2.zero;
        debugBoxDuration = 0f;
    }

    private void OnDrawGizmos()
    {
        if (debugBoxCenter.HasValue && debugBoxDuration > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(debugBoxCenter.Value, debugBoxSize);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
    }

}
