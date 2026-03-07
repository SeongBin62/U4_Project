using System.Collections;
using UnityEngine;
using System.Linq;

public class MonsterPatrol : MonoBehaviour
{
    [SerializeField] private MonsterStatus monsterStatus;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameMapUI gameMapUI;
    [SerializeField] private Animator animator;

    private Vector2 originPos;
    private bool isMovingRight = true;
    private bool isAttackDelay = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private Coroutine attackCoroutine;
    public bool isWaiting = false;
    private bool hasDealtDamage = false;


    private IEnumerator Start()
    {
        while (GameMapUI.Instance == null || GameMapUI.Instance.GetComponentInChildren<ObjectPool>() == null)
            yield return null;

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        monsterStatus ??= GetComponent<MonsterStatus>();
        animator ??= GetComponent<Animator>() ?? transform.GetChild(0).GetComponent<Animator>();
        gameMapUI = GameMapUI.Instance;
        originPos = transform.position;
    }


    public void SetDead()
    {
        isDead = true;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        animator.SetBool("IsAttack", false);
        animator.SetBool("IsWalk", false);
        isAttacking = false;
    }

    private void Update()
    {
        if (isDead || playerTransform == null || monsterStatus == null)
            return;

        if ((monsterStatus.IsDamaged || monsterStatus.IsDead) && isAttacking)
        {
            CancelAttack();
        }

        if (monsterStatus.IsDamaged) return;

        switch (monsterStatus.patrol)
        {
            case PatrolType.Fixed: HandleFixed(); break;
            case PatrolType.Patrol: HandlePatrol(); break;
            case PatrolType.Chase: HandleChase(); break;
            case PatrolType.Leash: HandleLeash(); break;
        }
    }
    private void CancelAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        animator.SetBool("IsAttack", false);
        isAttacking = false;
    }

    private void HandleLeash()
    {
        float leashRange = monsterStatus.patrolRange;
        float speed = monsterStatus.speed * Time.deltaTime;
        float distanceToPlayerFromOrigin = Mathf.Abs(playerTransform.position.x - originPos.x);

        if (Mathf.Approximately(transform.position.x, originPos.x))
            animator.SetBool("IsWalk", false);

        if (distanceToPlayerFromOrigin <= leashRange)
        {
            if (!isAttacking && !isAttackDelay)
            {
                if (IsPlayerInAttackRange())
                {
                    animator.SetBool("IsWalk", false);
                    animator.SetBool("IsAttack", true);
                    isAttacking = true;
                    attackCoroutine = StartCoroutine(LeashAttackRoutine());
                }
                else
                {
                    animator.SetBool("IsWalk", true);
                    MoveTowardsXOnly(playerTransform.position, speed);
                }
            }
        }
        else if (!isAttacking && !isAttackDelay)
        {
            animator.SetBool("IsAttack", false);
            animator.SetBool("IsWalk", true);

            MoveTowardsXOnly(originPos, speed);

            if (Mathf.Approximately(transform.position.x, originPos.x))
                animator.SetBool("IsWalk", false);
        }
    }

    private IEnumerator LeashAttackRoutine()
    {
        float speed = monsterStatus.speed * 2 * Time.deltaTime;

        while (IsPlayerInAttackRange() && !monsterStatus.IsDamaged && !monsterStatus.IsDead)
        {
            MoveTowardsXOnly(playerTransform.position, speed);

            if (IsPlayerCollided())
            {
                AttackPlayer();
                break;
            }

            yield return null;
        }

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.SetBool("IsAttack", false);
        yield return StartCoroutine(AttackDelayRoutine(monsterStatus.attackDelay));

        isAttacking = false;
        attackCoroutine = null;
    }

    private void HandleFixed()
    {
        if (IsPlayerInAttackRange()) AttackPlayer();
    }

    private void HandlePatrol()
    {
        if (isDead || isAttacking || isAttackDelay || monsterStatus.IsDamaged) return;

        if (IsPlayerInAttackRange())
        {
            attackCoroutine = StartCoroutine(PatrolAttackRoutine());
            return;
        }

        float speed = monsterStatus.speed * Time.deltaTime;
        float left = originPos.x - monsterStatus.patrolRange;
        float right = originPos.x + monsterStatus.patrolRange;

        Vector3 moveDir = isMovingRight ? Vector3.right : Vector3.left;
        Vector3 nextPos = transform.position + moveDir * 0.3f;

        // 땅이 없거나 벽이 앞에 있을 경우 방향 반전
        if (!HasGroundBelow(nextPos) || IsWallAhead(moveDir))
        {
            isMovingRight = !isMovingRight;
            return;
        }

        transform.position += moveDir * speed;

        if (transform.position.x >= right) isMovingRight = false;
        else if (transform.position.x <= left) isMovingRight = true;

        transform.localScale = new Vector3(isMovingRight ? 1 : -1, transform.localScale.y, transform.localScale.z);
    }

    private IEnumerator PatrolAttackRoutine()
    {
        isAttacking = true;
        hasDealtDamage = false;

        float waitTime = 0.5f;
        float elapsed = 0f;

        while (elapsed < waitTime)
        {
            if (monsterStatus.IsDead)
            {
                ResetAttackFlags();
                yield break;
            }

            if (!monsterStatus.IsDamaged)
            {
                elapsed += Time.deltaTime;
            }

            yield return null;
        }

        animator.SetBool("IsAttack", true);

        yield return new WaitForSeconds(0.4f);

        if (IsPlayerInAttackRange() && !hasDealtDamage)
        {
            AttackPlayer();
            hasDealtDamage = true;
        }

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.4f);
        animator.SetBool("IsAttack", false);

        yield return StartCoroutine(AttackDelayRoutine(monsterStatus.attackDelay));
        ResetAttackFlags();
    }


    private void ResetAttackFlags()
    {
        isAttacking = false;
        isAttackDelay = false;
        isWaiting = false;
        hasDealtDamage = false;
        attackCoroutine = null;
    }
    private void HandleChase()
    {
        if (isAttacking || isAttackDelay) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        FlipToTarget(playerTransform.position.x);

        if (distanceToPlayer <= monsterStatus.attackRange)
        {
            isAttacking = true;
            attackCoroutine = StartCoroutine(ChaseAttackRoutine());
        }
        else
        {
            Vector3 target = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
            MoveTowards(target, monsterStatus.speed * Time.deltaTime);
        }
    }
    private IEnumerator ChaseAttackRoutine()
    {
        // 공격 사거리 진입 시 0.5초 대기
        float waitTime = 0.5f;
        float elapsed = 0f;

        while (elapsed < waitTime)
        {
            if (monsterStatus.IsDamaged)
            {
                yield return new WaitUntil(() => !monsterStatus.IsDamaged);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (monsterStatus.IsDead)
        {
            isAttacking = false;
            yield break;
        }

        animator.SetBool("IsAttack", true);

        yield return new WaitForSeconds(0.1f);

        if (IsPlayerInAttackRange())
            AttackPlayer();

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        animator.SetBool("IsAttack", false);
        yield return StartCoroutine(AttackDelayRoutine(monsterStatus.attackDelay));

        isAttacking = false;
        attackCoroutine = null;
    }

    private void AttackPlayer()
    {
        if (monsterStatus.IsDamaged || GameData.Instance.GetPlayerHealth()==0) return;

        int damage = GameData.Instance.DecreaseHealth(monsterStatus.monsterName, 1);
        gameMapUI.ShowDamageText(transform.position, damage, true);
        StartCoroutine(AttackDelayRoutine(monsterStatus.attackDelay));
    }

    private IEnumerator AttackDelayRoutine(float delay)
    {
        isAttackDelay = true;
        yield return new WaitForSeconds(delay);
        isAttackDelay = false;
    }

    private bool IsPlayerInAttackRange()
    {
        Vector2 origin = new Vector2(transform.position.x + 0.5f * transform.localScale.x, transform.position.y); // 몬스터의 방향을 고려하여 오른쪽 기준
        float scaledRange = monsterStatus.attackRange * 0.7f;

        return monsterStatus.attackType switch
        {
            AttackType.Circle => Physics2D.OverlapCircle(origin, scaledRange, playerLayer),
            AttackType.Box => Physics2D.OverlapBox(origin, Vector2.one * scaledRange, 0f, playerLayer),
            _ => false,
        };
    }


    private bool IsPlayerCollided()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return false;

        Collider2D[] hits = Physics2D.OverlapBoxAll(col.bounds.center, col.bounds.size, 0f, playerLayer);
        return hits.Length > 0;
    }

    private void MoveTowards(Vector3 target, float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed);
    }
    private void MoveTowardsXOnly(Vector3 target, float speed)
    {
        Vector3 current = transform.position;
        Vector3 targetPos = new Vector3(target.x, current.y, current.z);

        Vector3 dir = (targetPos - current).normalized;
        Vector3 nextPos = current + dir * speed;

        if (dir.x > 0.01f)
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        else if (dir.x < -0.01f)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);

        if (!HasGroundBelow(nextPos) || IsWallAhead(dir))
            return;

        transform.position = Vector2.MoveTowards(current, targetPos, speed);
    }


    private void FlipToTarget(float targetX)
    {
        float dir = targetX - transform.position.x;
        if (Mathf.Abs(dir) > 0.01f)
            transform.localScale = new Vector3(dir > 0 ? 1 : -1, transform.localScale.y, transform.localScale.z);
    }
    private bool HasGroundBelow(Vector3 position)
    {
        Vector2 origin = new Vector2(position.x, position.y - 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.2f, monsterStatus.groundLayer);
        Debug.DrawRay(origin, Vector2.down * 0.2f, Color.green);
        return hit.collider != null;
    }
    private bool IsWallAhead(Vector3 direction)
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y);
        Vector2 rayDir = new Vector2(Mathf.Sign(direction.x), 0); // 현재 진행 방향
        float rayDistance = 0.3f;

        RaycastHit2D hit = Physics2D.Raycast(origin, rayDir, rayDistance, monsterStatus.groundLayer);
        Debug.DrawRay(origin, rayDir * rayDistance, Color.red);
        return hit.collider != null;
    }


    private void OnDrawGizmosSelected()
    {
        if (monsterStatus == null) return;

        Gizmos.color = Color.red;
        if (monsterStatus.attackType == AttackType.Circle)
            Gizmos.DrawWireSphere(transform.position, monsterStatus.attackRange);
        else if (monsterStatus.attackType == AttackType.Box)
            Gizmos.DrawWireCube(transform.position, Vector2.one * monsterStatus.attackRange);

        if (monsterStatus.patrol == PatrolType.Leash)
        {
            Gizmos.color = Color.green;
            Vector3 left = originPos + Vector2.left * monsterStatus.patrolRange;
            Vector3 right = originPos + Vector2.right * monsterStatus.patrolRange;
            Gizmos.DrawLine(left, right);
        }
    }
}
