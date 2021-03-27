using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class PlayerUI : MonoBehaviour
{
    public Player player;
    public RectTransform hp;
    [Space]
    public GameObject item, holder;
    public GameObject statsMenu;
    void Update()
    {
        hp.localScale = new Vector3(player.playerHp / 100f, 1f, 1f);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            statsMenu.SetActive(!statsMenu.active);
            if (statsMenu.active)
            {
                UpdateStats();
            }
        }
    }


    public void UpdateStats()
    {
        foreach (Transform item in holder.transform)
        {
            Destroy(item.gameObject);
        }
        var pl = PhotonNetwork.PlayerList;
        pl.ToList().OrderBy(x => x.CustomProperties.TryGetValue("K", out object k)).ToArray();
        for (int i = 0; i < pl.Length; i++)
        {
            var gm = Instantiate(item, holder.transform);
            var t = gm.GetComponent<PlayerStatItem>();
            t.player = pl[i];
            gm.SetActive(true);
        }
    }
}
