using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class PhotonLobby : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public static PhotonLobby lobby;
    public GameObject mainUI;
    public TMP_InputField inputField;
    public TMP_Text errorText, lobbyText;

    public List<RoomInfo> rooms;

    public int mapsCount = 1;

    private void OnApplicationQuit()
    {
        ClearErrorPrefs();
    }
    private void Start()
    {
        mainUI.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        errorText.text = "";
        lobby = this;
        if (PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.GameVersion = Application.version;
            PhotonNetwork.ConnectUsingSettings();
            if (PlayerPrefs.HasKey("NickName"))
            {
                PhotonNetwork.NickName = PlayerPrefs.GetString("NickName") + "-" + Random.Range(0, int.MaxValue).ToString("00000000");
            }
            else
            {
                PhotonNetwork.NickName = "User-" + Random.Range(0, int.MaxValue).ToString("00000000");
            }
        }
        else
        {
            mainUI.SetActive(true);
            lobbyText.text = PhotonNetwork.CurrentLobby.Name;
        }
        inputField.text = PhotonNetwork.NickName.Split('-')[0];
        if (PlayerPrefs.HasKey("Disconnect"))
        {
            errorText.text = PlayerPrefs.GetString("Disconnect");
        }
    }
    public void ChangeNickName()
    {
        string name = PhotonNetwork.NickName.Split('-')[0];
        string id = PhotonNetwork.NickName.Split('-')[1];
        PhotonNetwork.NickName = inputField.text + "-" + id;
        PlayerPrefs.SetString("NickName", inputField.text);
    }
    public static void ClearErrorPrefs()
    {
        PlayerPrefs.DeleteKey("Disconnect");
    }
    public void ClearError()
    {
        ClearErrorPrefs();
    }
    public void Exit()
    {
        Application.Quit();
    }
    private void Update()
    {
        if (PhotonNetwork.InLobby)
        {
            lobbyText.text = PhotonNetwork.CurrentLobby.Name + " " + PhotonNetwork.CountOfRooms;
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(new TypedLobby("DEFAULT", LobbyType.Default));
        mainUI.SetActive(true);
        base.OnConnectedToMaster();
    }


    public override void OnJoinedLobby()
    {
        lobbyText.text = PhotonNetwork.CurrentLobby.Name;
        //PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, "C0");
        base.OnJoinedLobby();
    }

    public void ToBattle()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public void CreateRoom()
    {
        int map = Random.Range(0, mapsCount);
        string name = "Room #" + Random.Range(0, 1000).ToString("0000") + "_"+ map;
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h.Add("Map", map);
        RoomOptions roomOptions = new RoomOptions() {IsVisible = true, IsOpen = true, MaxPlayers = 16, CustomRoomProperties = h};
        PhotonNetwork.CreateRoom(name, roomOptions, PhotonNetwork.CurrentLobby);
    }
    public void CreateRoom(string name, bool visible, bool open, byte players, int map = 0)
    {
        if (name.Replace(" ", "") == "")
        {
            name = "Room #" + Random.Range(0, 1000).ToString("0000") + "_"+map;
        }
        else
        {
            name = name + "_" + map;
        }

        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h.Add("Map", map);
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = players, CustomRoomProperties = h };
        PhotonNetwork.CreateRoom(name, roomOptions);
    }
    public void JoinRoom(TMP_Text mn)
    {
        PhotonNetwork.JoinRoom(mn.text);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
    }
    public override void OnJoinedRoom()
    {
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h.Add("K", 0);
        h.Add("D", 0);
        PhotonNetwork.LocalPlayer.SetCustomProperties(h);
        PhotonNetwork.LoadLevel(1);
        base.OnJoinedRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        errorText.text = "Join room Error";
        CreateRoom();
        base.OnJoinRandomFailed(returnCode, message);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        rooms = roomList;
        FindObjectOfType<Menu>().UpdateRooms();
        print("ROOMS COUNT: " + rooms.Count);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Failed to create room";
        base.OnCreateRoomFailed(returnCode, message);
    }
}
