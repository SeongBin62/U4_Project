using System.Collections;
using UnityEngine;

public class HillInteraction : MonoBehaviour
{
    [SerializeField] private Collider2D collider2d;
    private string groundLayerName = "Ground";
    private string defaultLayerName = "Default";

    private int groundLayer;
    private int defaultLayer;

    public bool isPlayerFeet = false;
    private Coroutine dropCoroutine;
    private bool isDropping = false;

    private void Start()
    {
        collider2d = GetComponent<Collider2D>();
        groundLayer = LayerMask.NameToLayer(groundLayerName);
        defaultLayer = LayerMask.NameToLayer(defaultLayerName);

        gameObject.layer = defaultLayer;
    }

    private void Update()
    {
        if (!isPlayerFeet || isDropping) return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (dropCoroutine != null)
                StopCoroutine(dropCoroutine);
            dropCoroutine = StartCoroutine(TemporarilyDisableCollision());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerFeet") && !isDropping)
        {
            collider2d.isTrigger = false;
            isPlayerFeet = true;
            gameObject.layer = groundLayer;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerFeet"))
        {
            collider2d.isTrigger = true;
            isPlayerFeet = false;
            gameObject.layer = defaultLayer;
        }
    }

    private IEnumerator TemporarilyDisableCollision()
    {
        isDropping = true;
        collider2d.isTrigger = true;
        gameObject.layer = defaultLayer;
        yield return new WaitForSeconds(0.3f);
        isDropping = false;

        if (isPlayerFeet)
        {
            collider2d.isTrigger = false;
            gameObject.layer = groundLayer;
        }
    }
}
