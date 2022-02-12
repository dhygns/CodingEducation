using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class StudentNetwork : MonoBehaviourPunCallbacks
{
    private string _message;
    private string _name;

    public void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        StudentManager.ShowConnectingView();
    }

    public void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }

    private IEnumerator WaitAndJoinLobby()
    {
        yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);;
        PhotonNetwork.JoinLobby();
        StudentManager.ShowJoiningLobbyView();
    }

    public override void OnConnected()
    {
        base.OnConnected();
        StartCoroutine(WaitAndJoinLobby());
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        StudentManager.ShowJoinRoomView((roomName, userName) => {
            if (!string.IsNullOrEmpty(roomName))
            {
                _name = userName;
                PhotonNetwork.JoinRoom(roomName);
                StudentManager.ShowJoiningRoomView();
            }
        }, _message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        _message = message;
        OnJoinedLobby();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        DataCarrier.RequestSetupStudentInfo(_name);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        _message = "teacher left, won't be in room :'(";
        if (PhotonNetwork.MasterClient == otherPlayer)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
