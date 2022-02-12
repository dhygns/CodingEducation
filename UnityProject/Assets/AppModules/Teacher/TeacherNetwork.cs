using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TeacherNetwork : MonoBehaviourPunCallbacks
{
    public void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        TeacherManager.ShowConnectingView();
    }

    public void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }

    private IEnumerator WaitAndJoinLobby()
    {
        yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady); ;
        PhotonNetwork.JoinLobby();
        TeacherManager.ShowJoiningLobbyView();
    }

    public override void OnConnected()
    {
        base.OnConnected();
        StartCoroutine(WaitAndJoinLobby());
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinOrCreateRoom($"{Random.Range(0, 10)}{Random.Range(0, 10)}{Random.Range(0, 10)}{Random.Range(0, 10)}{Random.Range(0, 10)}", new RoomOptions(), TypedLobby.Default);
        TeacherManager.ShowCreatingRoomView();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        TeacherManager.ShowLevelSelectionView(PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        DataCarrier.RequestSetupInfoToStudent(newPlayer, TeacherManager.level, TeacherManager.allowedInputs);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        TeacherManager.RemoveStudent(otherPlayer.ActorNumber);
    }
}
