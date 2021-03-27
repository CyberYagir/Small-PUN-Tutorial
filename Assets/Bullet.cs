using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPun
{
    public float speed;
    public string sender;
    public int dmg;
    float time;

    public GameObject explode;
    void Update()
    {
        if (time > 20)
        {
            GetComponent<PhotonView>().RPC("Del", RpcTarget.AllBuffered);
        }
        time += Time.deltaTime;
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    [PunRPC]
    public void Set(string sn)
    {
        sender = sn;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            GetComponent<PhotonView>().RPC("Del", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    public void Del()
    {
        foreach (var item in FindObjectsOfType<Player>())
        {
            item.photonView.RPC("AddForceExplode", RpcTarget.AllBuffered, (Vector2)transform.position, dmg, sender);
        }
        Destroy(Instantiate(explode.gameObject, transform.position, transform.rotation), 2);
        Destroy(gameObject);
        
    }
}
