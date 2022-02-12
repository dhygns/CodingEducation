using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DataCarrier;

public class StudentManager : MonoBehaviour
{
    public static StudentManager instance => _instance;
    private static StudentManager _instance;
    
    
    [Header("Views")]
    [SerializeField] private GameObject _connectingView;
    [SerializeField] private GameObject _joiningLobbyView;
    [SerializeField] private GameObject _joinRoomView;
    [SerializeField] private GameObject _joiningRoomView;
    [SerializeField] private GameObject _waitingLevelSelectionView;
    [SerializeField] private GameObject _gameView;

    [Header("Join Room View")]
    [SerializeField] private TMP_InputField _joinRoomInputField;
    [SerializeField] private TMP_InputField _userNameInputField;
    [SerializeField] private Button _joinRoomButton;
    [SerializeField] private TMP_Text _roomFailedMessage;

    [Header("Game View")]
    [SerializeField] private TMP_Text _gameViewTitle;
    [SerializeField] private TMP_Text _allowedButtonsLayout;
    
    private int _level = -1;
    private StudentInput _allowedInputs;
    
    public void Awake()
    {
        if (_instance == null)
            _instance = this;
        else 
            Destroy(gameObject);
    }

    public void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    static public void SetupInfo(int level, StudentInput allowedInputs)
    {
        _instance._level = level;
        _instance._allowedInputs = allowedInputs;
        ShowGameView(level, allowedInputs);
    }

    static public void ShowConnectingView()
    {
        _instance._connectingView.SetActive(true);
        _instance._joiningLobbyView.SetActive(false);
        _instance._joinRoomView.SetActive(false);
        _instance._joiningRoomView.SetActive(false);
        _instance._waitingLevelSelectionView.SetActive(false);
        _instance._gameView.SetActive(false);
    }

    static public void ShowJoiningLobbyView()
    {
        _instance._connectingView.SetActive(false);
        _instance._joiningLobbyView.SetActive(true);
        _instance._joinRoomView.SetActive(false);
        _instance._joiningRoomView.SetActive(false);
        _instance._waitingLevelSelectionView.SetActive(false);
        _instance._gameView.SetActive(false);
    }

    static public void ShowJoinRoomView(Action<string, string> listener, string message)
    {
        _instance._connectingView.SetActive(false);
        _instance._joiningLobbyView.SetActive(false);
        _instance._joinRoomView.SetActive(true);
        _instance._joiningRoomView.SetActive(false);
        _instance._waitingLevelSelectionView.SetActive(false);
        _instance._gameView.SetActive(false);

        _instance._joinRoomButton.onClick.AddListener(() => {
            listener?.Invoke(_instance._joinRoomInputField.text, _instance._userNameInputField.text);
        });

        _instance._roomFailedMessage.text = message;
    }

    static public void ShowJoiningRoomView()
    {
        _instance._joinRoomButton.onClick.RemoveAllListeners();

        _instance._connectingView.SetActive(false);
        _instance._joiningLobbyView.SetActive(false);
        _instance._joinRoomView.SetActive(false);
        _instance._joiningRoomView.SetActive(true);
        _instance._waitingLevelSelectionView.SetActive(false);
        _instance._gameView.SetActive(false);
    }

    static public void ShowGameView(int level = -1, StudentInput allowedInputs = (StudentInput)0)
    {
        _instance._connectingView.SetActive(false);
        _instance._joiningLobbyView.SetActive(false);
        _instance._joinRoomView.SetActive(false);
        _instance._joiningRoomView.SetActive(false);

        _instance._waitingLevelSelectionView.SetActive(_instance._level == -1);
        _instance._gameView.SetActive(_instance._level != -1);

        _instance._gameViewTitle.text = $"{level}";
        _instance._allowedButtonsLayout.text = $"{allowedInputs}";
    }
}
