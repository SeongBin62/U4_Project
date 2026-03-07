using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBlock : MonoBehaviour
{
    public int id;

    // Start is called before the first frame update
    void Start()
    {
        List<int> kills = GameData.Instance.GetKillMonsterIds();
        if(id >= 0)
        {
            if (kills.Contains(id)) transform.GetChild(0).gameObject.SetActive(false);
            else transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            id = -id;
            if (kills.Contains(id)) transform.GetChild(0).gameObject.SetActive(true);
            else transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
