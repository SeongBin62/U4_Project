using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractPortal : MonoBehaviour
{
    [SerializeField] private GameObject keyObj;
    public int portalId;

    private Coroutine keyAnimCoroutine;
    private bool isPlayerIn = false;
    private Transform player;
    private List<int> kills = GameData.Instance.GetkillIds().ToList();
    private void Start()
    {
        keyObj.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(portalId == 26)
        {
            List<int> ids = GameData.Instance.GetKillMonsterIds();
            if (!ids.Contains(5)) return;
        }
        else if(portalId == 3)
        {
            if(kills.Contains(4))
            {
                portalId = 29;
            }
        }
        if (collision.CompareTag("Player"))
        {
            isPlayerIn = true;
            player = collision.transform;
            keyObj.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerIn = false;
            player = null;

            if (keyAnimCoroutine != null)
            {
                StopCoroutine(keyAnimCoroutine);
                keyAnimCoroutine = null;
                
                keyObj.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (isPlayerIn && Input.GetKeyDown(KeyCode.G) && player != null)
        {
            GameData.Instance.SetScaleX(player.localScale.x);
            GameData.Instance.SetPortalId(portalId);
            SceneManager.LoadScene($"Map{GameData.Instance.GetMapById()}");
        }
    }

}
