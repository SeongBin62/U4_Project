using UnityEngine;

public class Stalactite : MonoBehaviour
{
    public float fallSpeed = 10f;
    public float xDetectRange = 0.5f;

    private bool hasFallen = false;
    private Rigidbody2D rb;
    private Animator animator;

    private bool isDropping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0;
    }

    void Update()
    {
        if (!hasFallen)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && Mathf.Abs(player.transform.position.x - transform.position.x) < xDetectRange)
            {
                hasFallen = true;
                rb.velocity = new Vector2(0, -fallSpeed);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDropping) return;
        else if (collision.CompareTag("Player"))
        {
            string name = "Á¾À¯¼®";
            isDropping = true;
            rb.velocity = Vector2.zero;
            animator.SetTrigger("IsDrop");
            int damage = GameData.Instance.DecreaseHealth(name, 1, true);
            GameMapUI.Instance.ShowDamageText(transform.position, damage, true);
        }

        StartCoroutine(DeactivateAfterAnimation());
    }

    private System.Collections.IEnumerator DeactivateAfterAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
