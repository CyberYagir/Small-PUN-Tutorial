using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks {

    public Player playerPrefab;

    public Player LocalPlayer;

    public int currMap = -1;
    public TMP_Text scoresHeader;
    [Space]
    public Transform deathLine;
    [Space]
    public bool dead, canRespawn;
    public bool pause;
    public GameObject pauseMenu;
    [HideInInspector]
    public Transform[] spawns;
    public Map[] maps;
    [System.Serializable]
    public class Map {
        public GameObject map;
        public Transform[] spawns;
    }


    private void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Menu");
            return;
        }
    }


    public void RespawnPlayer()
    {
        pause = false;
        if (canRespawn == true)
        {
            Player.RefreshInstance(ref LocalPlayer, playerPrefab, true);
            dead = false;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause = !pause;
            pauseMenu.SetActive(pause);
        }
        Application.targetFrameRate = 60;
        if (!pause)
        {
            if (dead == true)
            {
                if (canRespawn == true)
                {
                    RespawnPlayer();
                    dead = false;
                }
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
        }
        
       
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("K", out object k);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("D", out object d);
        if (k != null && d != null)
        {
            scoresHeader.text = "K: " + ((int)k).ToString("000") + " D:" + ((int)d).ToString("000");
        }
    }
    public void Disconnect()
    {
        if (LocalPlayer != null)
        {
            PhotonNetwork.Destroy(LocalPlayer.gameObject);
        }
        PhotonNetwork.LeaveRoom();
        Destroy(GameObject.Find("Manager"));
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
    }
    public IEnumerator Respawn()
    {
        canRespawn = false;
        dead = true;
        yield return new WaitForSeconds(2);
        //deadCam.GetComponentInChildren<TMPro.TMP_Text>().text = "Вы мертвы";
        canRespawn = true;
        yield return null;
    }

    void SetMap()
    {
        currMap = (int)PhotonNetwork.CurrentRoom.CustomProperties["Map"];
        for (int i = 0; i < maps.Length; i++)
        {
            if (currMap == i)
            {
                maps[i].map.SetActive(true);
                spawns = maps[i].spawns;
            }
            else
                maps[i].map.SetActive(false);
        }
    }

    private void Start()
    {
        SetMap();
        RespawnPlayer();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Player.RefreshInstance(ref LocalPlayer, playerPrefab);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (otherPlayer.IsLocal)
        {
            Disconnect();
        }
        base.OnPlayerLeftRoom(otherPlayer);
    }
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
        {
            Disconnect();
        }
        PlayerPrefs.SetString("Disconnect", "Server close connection");
        base.OnMasterClientSwitched(newMasterClient);
    }

    private void OnApplicationQuit()
    {
        PhotonLobby.ClearErrorPrefs();
    }
    public void Deselect()
    {
        FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
    }

    public void Suicide()
    {
        if (LocalPlayer != null)
        {
            LocalPlayer.GetComponent<Player>().playerHp = 0;
        }
    }
}
