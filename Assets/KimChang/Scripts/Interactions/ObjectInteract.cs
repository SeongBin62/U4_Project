using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectInteract : MonoBehaviour
{
    [SerializeField] private GameObject keyObj;

    private Coroutine keyAnimCoroutine;
    private bool isPlayerIn = false;
    private Transform player;

    private void Start()
    {
        keyObj.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

                
            }
            keyObj.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerIn && Input.GetKeyDown(KeyCode.G) && player != null && !GameMapUI.Instance.isPaused)
        {
            GameMapUI.Instance.OpenKeyExplain();
        }
    }
}
