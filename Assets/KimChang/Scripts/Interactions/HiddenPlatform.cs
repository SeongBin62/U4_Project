using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenPlatform : MonoBehaviour
{
    private HashSet<int> ids = new HashSet<int>();
    private bool isKill=false;
    public BoxCollider2D bc;
    public int id;
    private void Start()
    {
        ids = GameData.Instance.GetkillIds();
        if (ids.Contains(id))
        {
            SetEnabled();
        }
        else bc.enabled = false;
    }
    private void Update()
    {
        if (!isKill) return;
        if (ids.Contains(id))
        {
            SetEnabled();
        }
    }

    private void SetEnabled()
    {
        isKill = true;
        bc.enabled = true;
        enabled = false;
    }
}
