using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetzManager : MonoBehaviourPunCallbacks
{
    public static NetzManager instance;
    
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateOrJoinRoom()
    {
        if (PhotonNetwork.CountOfRooms > 0)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 2;
            string roomName = "Room_" + Random.Range(1, 1000);
            PhotonNetwork.CreateRoom(roomName, options);
        }
    }

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
