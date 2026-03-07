using System.Collections;
using UnityEngine;

public class GTArmAttack : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GateKeeper gateKeeper;
    private Vector2 attackBoxSize = new Vector2(4f, 3.2f);
    private Vector2 attackBoxOffset = new Vector2(0, -0.8f);

    private void OnEnable()
    {
        StartCoroutine(AttackProcess());
    }

    private IEnumerator AttackProcess()
    {
        yield return new WaitForSeconds(1.08f);

        if (CheckAttackHit())
        {
            gateKeeper.Attack();
        }

        yield return new WaitForSeconds(1.64f);
        gameObject.SetActive(false);
    }

    private bool CheckAttackHit()
    {
        Vector2 boxPosition = (Vector2)transform.position + attackBoxOffset;
        Collider2D hitPlayer = Physics2D.OverlapBox(boxPosition, attackBoxSize, 0f, playerLayer);
        return hitPlayer != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 boxPosition = (Vector2)transform.position + attackBoxOffset;
        Gizmos.DrawWireCube(boxPosition, attackBoxSize);
    }
}
