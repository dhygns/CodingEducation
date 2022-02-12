using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;

public class PhotonTest : MonoBehaviourPunCallbacks
{
    [SerializeField] Button _connectButton;
    [SerializeField] Button _joinLobbyButton;
    [SerializeField] Button _createRoomButton;
    [SerializeField] TMP_InputField _roomNameInputField;
    [SerializeField] Transform _roomButtonViewRoot;
    [SerializeField] RoomButtonView _roomButtonViewTemplate;

    private Dictionary<string, RoomButtonView> _roomButtonViews = new Dictionary<string, RoomButtonView>();

    private void Awake()
    {
        _connectButton.interactable = true;
        _joinLobbyButton.interactable = false;
        _createRoomButton.interactable = false;

        _connectButton.onClick.AddListener(OnConnectButtonClicked);
        _joinLobbyButton.onClick.AddListener(OnJoinLobbyButtonClicked);
        _createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
    }

    private void OnDestroy()
    {
        _connectButton.onClick.RemoveAllListeners();
        _joinLobbyButton.onClick.RemoveAllListeners();
        _createRoomButton.onClick.RemoveAllListeners();

        PhotonNetwork.Disconnect();
    }

    #region CONNECT

    public void OnConnectButtonClicked()
    {
        PhotonNetwork.ConnectUsingSettings();
        _connectButton.interactable = false;
        Debug.LogWarning("Connecting...");
    }

    public override void OnConnected()
    {
        base.OnConnected();
        Debug.LogWarning("OnConnected");
        _joinLobbyButton.interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogWarning($"OnDisconnected : {cause}");
        _connectButton.interactable = true;
        _joinLobbyButton.interactable = false;
    }

    #endregion

    #region LOBBY

    public void OnJoinLobbyButtonClicked()
    {
        PhotonNetwork.JoinLobby();
        _joinLobbyButton.interactable = false;
        Debug.LogWarning("Joinning lobby...");
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.LogWarning("OnJoinedLobby");
        _createRoomButton.interactable = true;
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        Debug.LogWarning("OnLeftLobby");
        _joinLobbyButton.interactable = true;
        _createRoomButton.interactable = false;
    }

    #endregion

    public void OnCreateRoomButtonClicked()
    {
        PhotonNetwork.CreateRoom(_roomNameInputField.text, new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true
        });
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Room Created successfully");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("Room Created failed : " + message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        Debug.Log("Room List Updated : " + roomList.Count);
        foreach (var room in roomList)
        {
            if (!_roomButtonViews.ContainsKey(room.Name))
            {
                RoomButtonView buttonView = Instantiate(_roomButtonViewTemplate, _roomButtonViewRoot);
                _roomButtonViews.Add(room.Name, buttonView);

                buttonView.gameObject.SetActive(true);
                buttonView.SetName(room.Name);
            }
        }

        List<string> roomNameList = roomList.Select(room => room.Name) as List<string>;
        foreach (var removedRoom in _roomButtonViews.Where(view => !roomNameList.Contains(view.Key)))
        {
            Destroy(removedRoom.Value);
        }

        _roomButtonViews = _roomButtonViews.Where(item => item.Value != null).ToDictionary(x => x.Key, y => y.Value);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("room joined");
        Debug.Log(PhotonNetwork.CurrentRoom.Name + "/" + PhotonNetwork.CurrentRoom.Players.Count);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("SendData", RpcTarget.All, "hello");
        }
    }

    [PunRPC]
    public void SendData(string str)
    {
        Debug.LogWarning(str);   
    }
}
