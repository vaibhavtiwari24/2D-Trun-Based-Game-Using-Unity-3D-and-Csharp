using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;

    [Header("Main Screen")]
    public Button playButton;

    [Header("Lobby Screen")]
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI gameStartingText;

    private void Start()
    {
        playButton.interactable = false;
        gameStartingText.gameObject.SetActive(false);
    }

    
    public override void OnConnectedToMaster()
    {
        playButton.interactable = true;
    }

  
    public void SetScreen(GameObject screen)
    {
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        screen.SetActive(true);
    }

   
    public void OnUpdatePlayerNameInput(TMP_InputField nameInput)
    {
        PhotonNetwork.NickName = nameInput.text;
    }

    
    public void OnPlayButton()
    {
        NetzManager.instance.CreateOrJoinRoom();
    }

   
    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    
    [PunRPC]
    void UpdateLobbyUI()
    {
        player1NameText.text = PhotonNetwork.CurrentRoom.GetPlayer(1).NickName;
        player2NameText.text = PhotonNetwork.PlayerList.Length == 2 ? PhotonNetwork.CurrentRoom.GetPlayer(2).NickName : "...";

        if (PhotonNetwork.PlayerList.Length == 2)
        {
            gameStartingText.gameObject.SetActive(true);
            if (PhotonNetwork.IsMasterClient)
                Invoke("TryStartGame", 4.0f);
        }
        else
        {
            gameStartingText.gameObject.SetActive(false);
        }
    }

   
    void TryStartGame()
    {
        if (PhotonNetwork.PlayerList.Length == 2)
            NetzManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
        else
            gameStartingText.gameObject.SetActive(false);
    }

    
    public void OnLeaveButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }
}
