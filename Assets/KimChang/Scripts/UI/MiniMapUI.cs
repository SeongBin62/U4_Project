using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class MiniMapUI : MonoBehaviour
{
    [SerializeField] private Transform miniMapBox;

    private void OnEnable()
    {
        HashSet<int> mapIdSet = new(GameData.Instance.GetMapIds());

        foreach (Transform miniMap in miniMapBox)
        {
            string digits = new string(miniMap.name.Where(char.IsDigit).ToArray());
            if (int.TryParse(digits, out int mapId))
            {
                miniMap.gameObject.SetActive(mapIdSet.Contains(mapId));
            }
            else
            {
                miniMap.gameObject.SetActive(false);
            }
        }
    }
}
