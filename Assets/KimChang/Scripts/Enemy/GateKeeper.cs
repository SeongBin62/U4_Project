using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GateKeeper : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private GameMapUI gameMapUI;
    [SerializeField] private GameObject sword;
    [SerializeField] private Transform swordTwo;
    [SerializeField] private GameObject arm;
    [SerializeField] private MonsterStatus monsterStatus;

    private float detectionRadius = 3.5f;
    private Vector2 attackBoxSize = new(2.5f, 2f);
    private Vector2 attackBoxOffset = new(1.4f, -2.2f);
    private float moveSpeed;

    private bool isAttacking, isMoving, isOnAttackCooldown, isWakingUp = true;
    private int currentAttackIndex = 1;
    private int phase = 1;
    private bool hasStartedWakeUp = false;

    private void Start()
    {
        if (GameData.Instance.GetKillMonsterIds().Contains(5))
        {
            gameObject.SetActive(false);
            return;
        }

        monsterStatus ??= GetComponent<MonsterStatus>();
        monsterStatus.isImmun = true;

        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        gameMapUI ??= FindObjectOfType<GameMapUI>();
        moveSpeed = monsterStatus.speed;
    }
    private void Update()
    {
        if (monsterStatus.IsDead) return;

        if (isWakingUp && !hasStartedWakeUp && IsPlayerInRange())
        {
            hasStartedWakeUp = true;
            animator.SetBool("IsWakeUp", true);
            StartCoroutine(WakeUpRoutine());
            AudioManager.instance.PlayBgmByBoss(monsterStatus.id);
            monsterStatus.OpenHpBar();
        }

        if (isWakingUp) return;
        monsterStatus.UpdateHp();
        phase = monsterStatus.SecondPhase();

        if (!isAttacking && !isMoving && !isOnAttackCooldown)
        {
            if (IsPlayerInRange())
                StartCoroutine(AttackPlayer());
            else
                StartCoroutine(MoveTowardsPlayer());
        }
    }

    private IEnumerator WakeUpRoutine()
    {
        yield return new WaitForSeconds(1.4f);
        isWakingUp = false;
        monsterStatus.isImmun = false;
    }
    private IEnumerator MoveTowardsPlayer()
    {
        isMoving = true;
        FlipToPlayer();
        SetAnim("IsMove1", true);

        yield return new WaitForSeconds(0.25f);

        float moveDuration = 0.5f;
        float elapsed = 0f;
        float yPos = transform.position.y;

        while (elapsed < moveDuration)
        {
            transform.position = Vector2.MoveTowards(transform.position,
                new Vector2(playerTransform.position.x, yPos),
                moveSpeed * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        SetAnim("IsMove1", false);
        isMoving = false;
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        FlipToPlayer();

        float attackTime = PlayAttackAnim(currentAttackIndex);

        yield return new WaitForSeconds(0.7f);

        if (currentAttackIndex == 3)
            yield return TryAttackDown();

        yield return new WaitForSeconds(attackTime - 0.7f);
        ResetAttackAnim();

        isAttacking = false;
        isOnAttackCooldown = true;
        currentAttackIndex = currentAttackIndex % 3 + 1;

        yield return new WaitForSeconds(1.5f);
        isOnAttackCooldown = false;
    }

    private float PlayAttackAnim(int index)
    {
        SetAnim($"IsAttack{index}", true);

        if (index == 1)
        {
            if(phase == 1) sword.SetActive(true);
            else 
            {
                swordTwo.localScale = gameObject.transform.localScale;
                foreach (Transform sword in swordTwo)
                {
                    sword.gameObject.SetActive(true);
                }
            }
            return 2.4f;
        }
        else if (index == 2)
        {
            arm.SetActive(true);
            FlipToPlayer(reverse: true);
            return (phase == 2) ? 2.7f : 3.4f;
        }

        return 2.4f;
    }

    private IEnumerator TryAttackDown()
    {
        float checkTime = 0.05f;
        float endTime = Time.time + checkTime;
        while (Time.time < endTime)
        {
            if (CheckAttackDownHit())
            {
                Attack();
                break;
            }
            yield return null;
        }
    }

    private void ResetAttackAnim()
    {
        for (int i = 1; i <= 3; i++)
            SetAnim($"IsAttack{i}", false);
    }

    private void FlipToPlayer(bool reverse = false)
    {
        if (playerTransform == null) return;

        float dir = playerTransform.position.x > transform.position.x ? 1 : -1;

        if (reverse)
            dir *= -1;

        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y, transform.localScale.z);
    }

    private bool CheckAttackDownHit()
    {
        float offsetX = transform.localScale.x > 0 ? 2f : -2f;
        Vector2 boxPos = (Vector2)transform.position + new Vector2(offsetX, attackBoxOffset.y);
        return Physics2D.OverlapBox(boxPos, attackBoxSize, 0f, playerLayer) != null;
    }

    private bool IsPlayerInRange() =>
        Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer) != null;

    public void Attack()
    {
        if (GameData.Instance == null) return;
        int damage = GameData.Instance.DecreaseHealth(monsterStatus.monsterName, 1);
        gameMapUI.ShowDamageText(transform.position, damage, true);
    }

    private void SetAnim(string name, bool value)
    {
        if (animator != null)
            animator.SetBool(name, value);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
