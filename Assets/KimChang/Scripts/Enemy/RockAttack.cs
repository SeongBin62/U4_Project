using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RockAttack : MonoBehaviour
{
    [SerializeField] private LeeBiri leeBiri;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer rockImg;

    private float speed = 10f;
    private Vector2 moveDirection;
    private bool isAttack = false;
    private bool isFlying = false;

    private Rigidbody2D rb;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (leeBiri == null)
            leeBiri = GetComponentInParent<LeeBiri>();
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if(rockImg == null)
            rockImg = GetComponentInChildren<SpriteRenderer>();
    }
    private void OnEnable()
    {
        animator.SetBool("IsCrash", false);

        transform.rotation = Quaternion.identity;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void RockAttackPlayer(Transform player)
    {
        isAttack = false;
        isFlying = true;

        int randomIndex = Random.Range(1, 4); // 1, 2, 3
        rockImg.sprite = SpriteCache.GetSprite($"Rock{randomIndex}");

        Vector2 dir = (player.position - transform.position).normalized;
        moveDirection = dir;
        rb.velocity = moveDirection * speed;
    }

    private void Update()
    {
        if (isFlying)
        {
            transform.Rotate(0, 0, 360 * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isFlying)
        {
            if (collision.CompareTag("Monster") || collision.CompareTag("Platform")) return;

            animator.SetBool("IsCrash", true);
            isFlying = false;
            
            if (collision.CompareTag("Player") && !isAttack)
            {
                leeBiri.AttackPlayer();
                isAttack = true;
            }

            StartCoroutine(DisableAfterDelay(1f));
        }
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        animator.SetBool("IsCrash", false);
        gameObject.SetActive(false);
    }
}
