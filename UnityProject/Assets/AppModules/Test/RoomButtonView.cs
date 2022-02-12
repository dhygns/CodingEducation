using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class RoomButtonView : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomName;
    
    public void SetName(string name)
    {
        _roomName.text = name;
    }

    public void OnJoinButtonClicked()
    {
        PhotonNetwork.JoinRoom(_roomName.text);
    }

    public void OnDeleteButtonClicked()
    {
        
    }
}
