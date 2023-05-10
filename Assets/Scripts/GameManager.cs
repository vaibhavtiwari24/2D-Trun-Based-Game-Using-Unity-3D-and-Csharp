using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPun
{
    public PlayerController leftPlayer;
    public PlayerController rightPlayer;

    public PlayerController currentPlayer;

    public float postGameTime;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            SetPlayers();
    }

    void SetPlayers()
    {
        leftPlayer.photonView.TransferOwnership(1);
        rightPlayer.photonView.TransferOwnership(2);

        leftPlayer.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.GetPlayer(1));
        rightPlayer.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.GetPlayer(2));

        this.photonView.RPC("SetNextTurn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void SetNextTurn()
    {
        if (currentPlayer == null)
            currentPlayer = leftPlayer;
        else
            currentPlayer = currentPlayer == leftPlayer ? rightPlayer : leftPlayer;

        if (currentPlayer == PlayerController.me)
            PlayerController.me.BeginTurn();

        GameUI.instance.ToggleEndTurnButton(currentPlayer == PlayerController.me);
    }

    public PlayerController GetOtherPlayer(PlayerController player)
    {
        return player == leftPlayer ? rightPlayer : leftPlayer;
    }

    public void CheckWinCondition()
    {
        if (PlayerController.me.units.Count == 0)
        {
            photonView.RPC("WinGame", RpcTarget.All, PlayerController.enemy == leftPlayer ? 0 : 1);
        }
    }

    [PunRPC]
    void WinGame(int winner)
    {
        PlayerController player = winner == 0 ? leftPlayer : rightPlayer;
        GameUI.instance.SetWinText(player.photonPlayer.NickName);

        Invoke("GoBackToMenu", postGameTime);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetzManager.instance.ChangeScene("Menu");
    }
}
