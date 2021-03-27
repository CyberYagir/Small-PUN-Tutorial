using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviourPun, IPunObservable {


    public int playerHp = 100;
    int playerHpOld = 100;

    public PlayerLocal lp;
    public Transform gun;
    public int k, d;
    public GameObject canvas;

    float timeLastDamage = 0;
    float hpadd = 0;

    bool dead = false;
    public string lastDamagePlayer;
    public void Start()
    {
        transform.name = photonView.Owner.NickName;
    }
    private void Awake()
    {
        if (!photonView.IsMine)
        {
            lp.enabled = false;
            GetComponent<PlayerUI>().enabled = false;
            Destroy(lp.camera.gameObject);
            canvas.SetActive(false);
        }
    }
    [PunRPC]
    public void AddForceExplode(Vector2 pos, int dmg, string sender)
    {
        float dist = Vector2.Distance(pos, transform.position);
        if (dist < 2)
        {
            GetComponent<Rigidbody2D>().AddForce(((Vector2)transform.position - pos) * dist * 8f, ForceMode2D.Impulse);
            if (sender != photonView.Owner.NickName)
                TakeDamage((int)(dmg * (dist / 2f)), sender);
            else
                TakeDamage((int)((dmg/2f) * (dist / 2f)), sender);
        }
    } 

    public void Update()
    {
        timeLastDamage += Time.deltaTime;
        if (timeLastDamage > 10)
        {
            hpadd += Time.deltaTime*2;
            if ((int)hpadd == 1)
            {
                playerHp += 1;
                hpadd = 0;
            }
        }
        if (playerHp > 100)
        {
            playerHp = 100;
        }
        if (photonView.IsMine)
        {
            if (playerHp < playerHpOld)
            {
                playerHpOld = playerHp;
                print("Damage");
            }
            if (photonView.Owner.CustomProperties["K"] != null)
            {
                k = (int)photonView.Owner.CustomProperties["K"];
                d = (int)photonView.Owner.CustomProperties["D"];
            }
            if (playerHp <= 0 || transform.position.y < FindObjectOfType<GameManager>().deathLine.transform.position.y)
            {
                Dead();
                playerHp = 100;
            }
        }
        else
        {
            if (playerHp <= 0)
            {
                dead = true;
            }
        }

    }

    public void Dead()
    {
        if (photonView.IsMine)
        {
            if (dead == false)
            {
                if (lastDamagePlayer != "" && lastDamagePlayer != photonView.Owner.NickName)
                {
                    var ldp = PhotonNetwork.PlayerList.ToList().Find(x => x.NickName == lastDamagePlayer);
                    if (ldp != null)
                    {
                        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
                        h.Add("K", (int)(((int)ldp.CustomProperties["K"]) + 1));
                        h.Add("D", (int)ldp.CustomProperties["D"]);
                        ldp.SetCustomProperties(h);
                    }
                }
                d++;
                SaveKD();
                PhotonNetwork.Destroy(gameObject);
                dead = true;
                FindObjectOfType<GameManager>().StartCoroutine(FindObjectOfType<GameManager>().Respawn());
            }
        }
    }

    public void Kick(Photon.Realtime.Player pl)
    {
        PhotonNetwork.CloseConnection(pl);
    }

    [PunRPC]
    public void TakeDamage(int dmg, string actorName)
    {
        if (photonView.IsMine)
        {
            lastDamagePlayer = actorName;
            playerHp -= dmg;
            timeLastDamage = 0; hpadd = 0;
        }
    } 

    [PunRPC]
    public void ShootAnim(string actorName)
    {
        Player player = GameObject.Find(actorName).GetComponent<Player>();
    }


    [PunRPC]
    public void AddKill()
    {
        k++;
        SaveKD();
    }
    public void  SaveKD()
    {
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h.Add("K", k);
        h.Add("D", d);
        photonView.Owner.SetCustomProperties(h);
    }

    public static void RefreshInstance(ref Player player, Player playerPrefab, bool withMasterClient = false)
    {
        if (PhotonNetwork.IsMasterClient == false || withMasterClient == true)
        {
            print("Respawn");
            var pos = FindObjectOfType<GameManager>().spawns[Random.Range(0, FindObjectOfType<GameManager>().spawns.Length)].position;
            var rot = Quaternion.identity;
            if (player != null)
            {
                pos = player.transform.position;
                rot = player.transform.rotation;
                PhotonNetwork.Destroy(player.gameObject);
            }
            player = PhotonNetwork.Instantiate(playerPrefab.gameObject.name, pos, rot).GetComponent<Player>();

        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerHp);
            stream.SendNext(k);
            stream.SendNext(d);
            stream.SendNext(lp.gun.rotation);

        }
        else
        {

            playerHp = (int)stream.ReceiveNext();
            k = (int)stream.ReceiveNext();
            d = (int)stream.ReceiveNext();
            lp.gun.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
