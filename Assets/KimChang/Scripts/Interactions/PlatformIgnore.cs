using System.Collections.Generic;
using UnityEngine;

public class PlatformIgnore : MonoBehaviour
{
    [SerializeField] private Collider2D platformCollider;

    public int id;
    public bool isTigger = false;
    public GameObject player= null;

    private void Start()
    {
        List<int> kill = GameData.Instance.GetKillMonsterIds();
        if (kill.Contains(id)) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
    private void Update()
    {
        if (isTigger && Input.GetKeyDown(KeyCode.S) && player !=null)
        {
            Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), platformCollider, true);

        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTigger = true;
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTigger = false;
            Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), platformCollider, false);
        }
    }
}
