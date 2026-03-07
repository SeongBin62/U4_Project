using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    [SerializeField] private int portalId;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(portalId == -10)
            {
                string name = "±∏µ¢¿Ã";
                int damage = GameData.Instance.DecreaseHealth(name, 100,true);
                GameMapUI.Instance.ShowDamageText(transform.position, damage, true);
                return;
            }
            if (portalId ==10 || portalId ==11)
            {
                List<int> kills = GameData.Instance.GetKillMonsterIds();
                if (!kills.Contains(4)) return;

            }
            GameData.Instance.SetScaleX(other.transform.localScale.x);
            GameData.Instance.SetPortalId(portalId);
            SceneManager.LoadScene($"Map{GameData.Instance.GetMapById()}");
        }
    }
}
