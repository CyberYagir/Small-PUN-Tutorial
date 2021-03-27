using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatItem : MonoBehaviour
{
    public Photon.Realtime.Player player;

    public TMPro.TMP_Text nm, k, d;

    private void Start()
    {
        if (player.CustomProperties["K"] != null)
        {
            nm.text = player.NickName.Split('-')[0];
            k.text = ((int)player.CustomProperties["K"]).ToString("000");
            d.text = ((int)player.CustomProperties["D"]).ToString("000");
        }
        else
        {
            Destroy(gameObject);
        }     
    }
}
