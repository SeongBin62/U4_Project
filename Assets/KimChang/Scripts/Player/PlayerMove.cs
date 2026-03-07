using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Transform pcObject;
    [SerializeField] private Transform spriteObject;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private Vector2 attackBoxSize = new(2.4f, 1.2f);
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private CapsuleCollider2D capsuleCollider;

    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private PhysicsMaterial2D normalPhysics;
    [SerializeField] private PhysicsMaterial2D hardPhysics;
    private Vector2 groundCheckOffset = new(0f, -0.7f);

    private Rigidbody2D rb;
    private Animator pcAnimator;
    private Animator spriteAnimator;

    private float moveSpeed = 6f;
    private float jumpForce = 10.5f;
    private float groundCheckDistance = 1f;
    private float originalMoveSpeed;
    private float maxClimbAngle = 85f;

    private bool isGrounded = false;
    private bool isAttacking = false;
    private bool isHealing = false;
    private bool isDamaged = false;
    private bool isDead = false;
    private bool isOnLadder =false;

    private bool isLadder = false;
    private bool prevIsWalking = false;

    private Coroutine healingCoroutine;
    private Coroutine attackMoveCoroutine;
    private RaycastHit2D groundHitInfo;
    
    List<Collider2D> ignoredGrounds = new(); 
    [SerializeField] private float maxJumpVelocity = 12f; // 최대 Y속도 제한

    void FixedUpdate()
    {
        ClampVerticalVelocity();
    }
    private void ClampVerticalVelocity()
    {
        if (rb.velocity.y > maxJumpVelocity)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxJumpVelocity);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pcAnimator = pcObject.GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteAnimator = spriteObject.GetComponent<Animator>();
        originalMoveSpeed = moveSpeed;
        SwitchToPCMode();
    }

    void Update()
    {
        if (isDead || isAttacking || isDamaged)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }
        if (GameMapUI.Instance.isPaused)
        {
            return;
        }

        HandleGroundCheck();
        HandleMove();
        HandleLadderMove();
        
        if (!isOnLadder)
        {
            HandleDirection();
            HandleJump();
            HandleAttack();
            ProcessSkill();
        }
        
        ViewMiniMap();
    }
    void HandleMove()
    {
        float h = Input.GetAxisRaw("Horizontal");

        if (h == 0)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            SetWalkAnim(false);
            capsuleCollider.sharedMaterial = hardPhysics;
            return;
        }

        // 발 밑에서 정면 방향으로 Ray
        Vector2 rayOrigin = (Vector2)transform.position + groundCheckOffset; // 발밑 위치
        Vector2 rayDir = Vector2.right * Mathf.Sign(h); // 좌/우 방향
        float rayLength = 0.3f;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDir, rayLength, groundLayer);
        Debug.DrawRay(rayOrigin, rayDir * rayLength, Color.yellow);

        if (hit.collider != null)
        {
            Vector2 normal = hit.normal;
            Vector2 tangent = new Vector2(normal.y, -normal.x); // 노멀에 수직인 접선
            tangent *= Mathf.Sign(h); // 입력 방향 유지

            Vector2 moveVec = tangent.normalized * moveSpeed;
            rb.velocity = new Vector2(moveVec.x, rb.velocity.y); // Y는 중력 유지
            SetWalkAnim(true);
            capsuleCollider.sharedMaterial = normalPhysics;
        }
        else
        {
            rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);
            SetWalkAnim(true);
            capsuleCollider.sharedMaterial = normalPhysics;
        }

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }


    bool TryGetSlopeInfo(float h, out Vector2 moveDir, out float angle)
    {
        moveDir = Vector2.zero;
        angle = 0f;

        Vector2 forwardOrigin = new Vector2(transform.position.x, transform.position.y - 0.7f);
        Vector2 forwardDir = Vector2.right * Mathf.Sign(h);

        RaycastHit2D forwardHit = Physics2D.Raycast(forwardOrigin, forwardDir, 0.3f, groundLayer);
        Debug.DrawRay(forwardOrigin, forwardDir * 0.3f, Color.cyan);

        RaycastHit2D hit = groundHitInfo.collider ? groundHitInfo : forwardHit;

        if (hit.collider != null)
        {
            angle = Vector2.Angle(hit.normal, Vector2.up);

            Vector2 inputDir = new Vector2(Mathf.Sign(h), 0);
            Vector2 tangent = Vector2.Perpendicular(hit.normal);
            float dot = Vector2.Dot(tangent, inputDir);
            moveDir = tangent.normalized * Mathf.Sign(dot);

            return true;
        }

        return false;
    }

    void SetWalkAnim(bool isWalking)
    {
        if (prevIsWalking != isWalking)
        {
            SwitchToPCMode();
            pcAnimator.SetBool("IsWalk", isWalking);
            prevIsWalking = isWalking;
        }
    }
    void HandleLadderMove()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (!isLadder || Mathf.Abs(h) > 0)
        {
            rb.gravityScale = 2f;
            isOnLadder = false;
            return;
        }

        if (Mathf.Abs(v) > 0.01f)
        {
            isOnLadder = true;
        }

        if (isOnLadder)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, v * moveSpeed * 0.8f);

            if (Mathf.Abs(v) > 0.01f)
            {
                SwitchToSpriteMode();
                spriteAnimator.SetBool("IsBack", true);
                spriteObject.localScale = new Vector2(0.75f, 0.75f);
            }

            spriteAnimator.SetFloat("BackSpeed", Mathf.Abs(v) * 0.5f);
        }
    }
    void HandleJump()
    {
        //bool isFalling = rb.velocity.y < 0f;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void HandleDirection()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if (h < 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (h > 0) transform.localScale = new Vector3(1, 1, 1);
    }

    void HandleGroundCheck()
    {
        Vector2 origin = (Vector2)transform.position + groundCheckOffset;
        isGrounded = Physics2D.OverlapCircle(origin, groundCheckRadius, groundLayer);

        Debug.DrawRay(origin, Vector2.down * groundCheckRadius, Color.green);
    }

    void HandleAttack()
    {
        if (!isAttacking && Input.GetMouseButtonDown(0) && !GameMapUI.Instance.isPaused)
        {
            isAttacking = true;
            pcAnimator.SetBool("IsWalk", false);
            SwitchToSpriteMode();
            spriteAnimator.SetBool("IsAttack", true);
            StartCoroutine(AttackAndCheckMonsters());
            Invoke(nameof(EndAttack), spriteAnimator.GetCurrentAnimatorStateInfo(0).length);
        }
    }

    IEnumerator AttackAndCheckMonsters()
    {
        yield return new WaitForSeconds(0.35f);
        float duration = spriteAnimator.GetCurrentAnimatorStateInfo(0).length - 0.35f;
        attackMoveCoroutine = StartCoroutine(AttackMoveRoutine(duration));

        AudioManager.instance.PlaySfx(1);

        Vector2 center = (Vector2)transform.position + Vector2.right * attackRange * transform.localScale.x;
        foreach (Collider2D col in Physics2D.OverlapBoxAll(center, attackBoxSize, 0, monsterLayer))
        {
            if (!col.TryGetComponent(out MonsterStatus monster)) continue;

            if (monster.isImmun) continue;
            float playerAtk = GameData.Instance.GetPlayerAttack();
            float damage = playerAtk * (1 - (monster.defense / (monster.defense + 50f)));
            monster.health -= Mathf.RoundToInt(damage);

            int hitEnergy = GameData.Instance.GetHitEnergy();
            GameData.Instance.AddPlayerEnergy(hitEnergy);
            monster.energyReward -= hitEnergy;

            GameMapUI.Instance.CreateAttackEffect(col.gameObject, transform.localScale.x);
            GameMapUI.Instance.ShowDamageText(col.transform.position, Mathf.RoundToInt(damage), false);

            if (monster.health < 1) 
            {
                AudioManager.instance.PlaySfx(2);
                monster.MonsterDie();
            }
            else 
            {
                AudioManager.instance.PlaySfx(2);
                monster.MonsterDamaged(); 
            }
        }
    }

    IEnumerator AttackMoveRoutine(float duration)
    {
        float moveDistance = 0.5f;
        float elapsed = 0f;
        Vector3 dir = new(transform.localScale.x, 0, 0);

        float movePerFrame = (moveDistance / duration);
        // Vector2 rayDir = new Vector2(transform.localScale.x, 0);
        // float rayLength = (moveDistance / duration) + 0.05f;

        while (elapsed < duration)
        {
            // 타임스케일이 0이면 대기만 함
            if (Time.timeScale == 0f)
            {
                yield return null;
                continue;
            }

            // Vector2 origin = transform.position;
            // RaycastHit2D hit = Physics2D.Raycast(origin, rayDir, rayLength * Time.deltaTime, groundLayer);
            // if (hit.collider != null)
            //     break;

            // 공격 중 전진 이동 (현재 비활성화됨)
            // transform.position += dir * movePerFrame * Time.deltaTime;

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void EndAttack()
    {
        if (attackMoveCoroutine != null) StopCoroutine(attackMoveCoroutine);
        spriteAnimator.SetBool("IsAttack", false);
        SwitchToPCMode();
        isAttacking = false;
        moveSpeed = originalMoveSpeed;

        float h = Input.GetAxisRaw("Horizontal");
        prevIsWalking = false;
        SetWalkAnim(h != 0);
    }

    public void PlayerDamaged(int damage)
    {
        if (damage <= 0) return;
        spriteObject.localScale = new Vector2(0.28f, 0.28f);
        SwitchToSpriteMode();

        if (GameData.Instance.GetPlayerHealth() > 0)
        {
            StartCoroutine(DamageRoutine());
        }
        else
        {
            isDead = true;
            StopAllCoroutines();
            ResetAllAnimatorBools(pcAnimator);
            ResetAllAnimatorBools(spriteAnimator);
            spriteAnimator.SetTrigger("IsDie");
        }
    }
    private void ResetAllAnimatorBools(Animator animator)
    {
        if (animator == null) return;

        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(param.name, false);
            }
        }
    }

    IEnumerator DamageRoutine()
    {
        isDamaged = true;
        spriteAnimator.SetBool("IsDamaged", true);
        yield return new WaitForSeconds(0.7f);
        spriteAnimator.SetBool("IsDamaged", false);
        isDamaged = false;
        SwitchToPCMode();
    }

    void ProcessSkill()
    {
        if (Input.GetMouseButtonDown(1) && !isHealing && GameData.Instance.GetPlayerEnergy() >= 30 &&
            GameData.Instance.GetEquipItemId()==3 && GameData.Instance.GetPlayerHealth() != GameData.Instance.GetMaxHealth() && !prevIsWalking
            && isGrounded)
        {
            healingCoroutine = StartCoroutine(HealingRoutine());
        }
    }

    IEnumerator HealingRoutine()
    {
        isHealing = true;
        SwitchToSpriteMode();
        spriteAnimator.SetBool("IsHeal", true);

        float elapsed = 0f;
        while (elapsed < 1.5f)
        {
            if (!spriteAnimator.GetBool("IsHeal") || ShouldInterruptHealing())
            {
                CancelHeal();
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        GameData.Instance.IncreaseHealth(1);
        GameData.Instance.AddPlayerEnergy(-30);
        spriteAnimator.SetBool("IsHeal", false);
        isHealing = false;
        SwitchToPCMode();
    }

    void CancelHeal()
    {
        if (healingCoroutine != null) StopCoroutine(healingCoroutine);
        healingCoroutine = null;
        GameData.Instance.AddPlayerEnergy(-30);
        spriteAnimator.SetBool("IsHeal", false);
        isHealing = false;
        SwitchToPCMode();
    }

    bool ShouldInterruptHealing()
    {
        bool isMoving = rb.velocity.sqrMagnitude > 0.01f;
        bool isPressingMoveKey = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        bool isJumpPressed = Input.GetKeyDown(KeyCode.Space);
        bool isAttackPressed = Input.GetMouseButtonDown(0);

        return
            isAttackPressed || isJumpPressed || isPressingMoveKey || isMoving ||
            spriteAnimator.GetBool("IsAttack") ||
            spriteAnimator.GetBool("IsDamaged") ||
            spriteAnimator.GetBool("IsDie");
    }



    void SwitchToPCMode()
    {
        if (isDead) return;
        pcObject.gameObject.SetActive(true);
        spriteObject.gameObject.SetActive(false);
    }

    void SwitchToSpriteMode()
    {
        spriteObject.localScale = isAttacking ? new Vector3(0.25f, 0.25f, 1f) : new Vector3(0.28f, 0.28f, 1f);
        pcObject.gameObject.SetActive(false);
        spriteObject.gameObject.SetActive(true);
        spriteAnimator.SetBool("IsBack", false);
    }

    private void ViewMiniMap()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameMapUI.Instance.ToggleWindow();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            AudioManager.instance.PlaySfx(3);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            if (int.TryParse(Regex.Match(other.name, "\\d+").Value, out int itemId))
            {
                GameData.Instance.AddItem(itemId);
                other.gameObject.SetActive(false);
            }
        }
        else if (other.CompareTag("Ladder"))
        {
            isLadder = true;
        }
        else if (other.CompareTag("Hole"))
        {
            GameData.Instance.SetPlayerHealth(0);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
            isLadder = false;
    }

    void OnDrawGizmosSelected()
    {
        Vector2 origin = (Vector2)transform.position + groundCheckOffset;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(origin, groundCheckRadius);
    }
}
