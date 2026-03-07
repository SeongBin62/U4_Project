using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GateKeeper gateKeeper;
    [SerializeField] private Transform playerTransform;
    private float followSpeed = 8f;

    private Vector2 attackBoxSize = new Vector2(1f, 1f);
    private Vector2 attackBoxOffset = new Vector2(0f, -2.5f);
    private float moveDuration = 0.24f;
    private float constPosX=0;

    
    public float targetY = 2.4f;
    public float posY = 0;
    private void OnEnable()
    {
        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }
        if(gameObject.name == "SwordLeft") 
        {
            constPosX = -1.5f;
            attackBoxOffset = new Vector2(1, -2.5f); 
        }
        else if (gameObject.name == "SwordRight")
        {
            constPosX = 1.5f;
            moveDuration += 0.09f;
            attackBoxOffset = new Vector2(-1, -2.5f);
        }


        // 활성화되면 검사를 시작하는 코루틴 실행
        StartCoroutine(AttackProcess());
    }

    private IEnumerator AttackProcess()
    {
        float followDuration = 1.51f; // 플레이어를 따라다니는 시간
        float elapsedTime = 0f;

        // 1.7초 동안 플레이어를 따라다니기
        while (elapsedTime < followDuration)
        {
            // 플레이어의 X 위치를 따라다님, Y 값은 1.5로 고정
            FollowPlayerX();
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 0.05초 동안 특정 위치로 이동 (transform.position = new Vector2(transform.position.x, -0.3f);)

        elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            // 목표 위치로 이동
            Vector2 targetPosition = new Vector2(transform.position.x, posY);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, 50f * Time.deltaTime); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 0.05초 동안 지속적으로 공격 범위를 검사
        float checkDuration = 0.05f;
        float checkEndTime = Time.time + checkDuration;

        while (Time.time < checkEndTime)
        {
            if (CheckAttackHit())
            {
                gateKeeper.Attack();
                break;
            }
            yield return null;
        }

        // 공격 후 0.2초 뒤에 비활성화
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }

    private void FollowPlayerX()
    {
        if (playerTransform == null) return;

        // 현재 위치의 X 값은 플레이어의 X 값으로 변경, Y 값은 입력받은 targetY로 고정
        Vector2 targetPosition = new Vector2(playerTransform.position.x+constPosX, targetY);

        // 부드럽게 플레이어의 X 위치로 이동
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private bool CheckAttackHit()
    {
        // 공격 범위의 위치 계산 (오브젝트의 위치 + 오프셋)
        float directionOffsetX = transform.localScale.x > 0 ? attackBoxOffset.x : -attackBoxOffset.x;
        Vector2 boxPosition = (Vector2)transform.position + new Vector2(directionOffsetX, attackBoxOffset.y);

        // 플레이어 감지 검사 (OverlapBox)
        Collider2D hitPlayer = Physics2D.OverlapBox(boxPosition, attackBoxSize, 0f, playerLayer);

        return hitPlayer != null; // 플레이어가 범위 안에 있으면 true 반환
    }

    private void OnDrawGizmos()
    {
        // 공격 범위를 노란색으로 표시
        Gizmos.color = Color.yellow;

        float directionOffsetX = transform.localScale.x > 0 ? attackBoxOffset.x : -attackBoxOffset.x;
        Vector2 boxPosition = (Vector2)transform.position + new Vector2(directionOffsetX, attackBoxOffset.y);

        Gizmos.DrawWireCube(boxPosition, attackBoxSize);
    }
}
