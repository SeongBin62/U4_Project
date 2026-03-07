using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SmokeTower : MonoBehaviour
{
    [SerializeField] private TextMeshPro saveText; 
    [SerializeField] private GameObject interaction;
    private SaveData saveData = new SaveData();
    private bool isPlayerInRange = false;
    private bool isSaving = false;
    Transform fChild = null;
    Transform sChild = null;

    public int bossId = -1;
    public int saveId = -1;
    private void Start()
    {
        saveText.gameObject.SetActive(false);
        interaction.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(bossId != -1)
            {
                List<int> ids=GameData.Instance.GetKillMonsterIds();
                if (!ids.Contains(bossId)) return;
            }
            interaction.SetActive(true);
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            interaction.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.G) && !isSaving)
        {
            StartCoroutine(SaveSequence());
        }
    }

    private IEnumerator SaveSequence()
    {
        fChild ??= transform.GetChild(0);
        sChild ??= transform.GetChild(1);

        fChild.gameObject.SetActive(false);
        sChild.gameObject.SetActive(true);
        isSaving = true;
        interaction.SetActive(false);
        GameData.Instance.SetSpawnId(saveId);
        saveData.SaveDataFile(GameData.Instance.GetPlayerId());

        saveText.gameObject.SetActive(true);
        Vector3 startPos = saveText.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0f, 1.5f, 0f);
        Color originalColor = saveText.color;
        float duration = 1f;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            saveText.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            saveText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f - t); 
            time += Time.deltaTime;
            yield return null;
        }

        fChild.gameObject.SetActive(true);
        sChild.gameObject.SetActive(false);

        saveText.gameObject.SetActive(false);
        saveText.transform.localPosition = startPos;
        saveText.color = originalColor;
        isSaving = false;
        if (isPlayerInRange)
        {
            interaction.SetActive(true);
        }
        else
        {
            interaction.SetActive(false);
        }
    }
}
