using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public GameObject attackEffectPrefab;
    public int initialPoolSize = 10;

    private List<GameObject> damageTextPool = new();
    private List<GameObject> attackEffectPool = new();

    private bool isInitialized = false;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (isInitialized) return;
        isInitialized = true;

        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject damageObj = Instantiate(damageTextPrefab, transform);
            GameObject attackObj = Instantiate(attackEffectPrefab, transform);

            damageObj.SetActive(false);
            attackObj.SetActive(false);

            damageTextPool.Add(damageObj);
            attackEffectPool.Add(attackObj);
        }
    }

    public GameObject GetDamageText()
    {
        while (damageTextPool.Count > 0)
        {
            GameObject obj = damageTextPool[0];
            damageTextPool.RemoveAt(0);

            if (obj != null)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject newObj = Instantiate(damageTextPrefab, transform);
        newObj.SetActive(true);
        return newObj;
    }

    public void ReturnDamageText(GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(false);
        if (!damageTextPool.Contains(obj))
            damageTextPool.Add(obj);
    }

    public GameObject GetAttackEffect()
    {
        while (attackEffectPool.Count > 0)
        {
            GameObject obj = attackEffectPool[0];
            attackEffectPool.RemoveAt(0);

            if (obj != null)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject newObj = Instantiate(attackEffectPrefab, transform);
        newObj.SetActive(true);
        return newObj;
    }

    public void ReturnAttackEffect(GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(false);
        if (!attackEffectPool.Contains(obj))
            attackEffectPool.Add(obj);
    }
}
