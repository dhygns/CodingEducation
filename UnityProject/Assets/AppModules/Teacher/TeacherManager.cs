using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static DataCarrier;

public class TeacherManager : MonoBehaviour
{
    public static TeacherManager instance => _instance;
    private static TeacherManager _instance;

    static public int level => _instance._level;
    static public StudentInput allowedInputs => _instance._allowedInputs;

    [Header("Required Prefabs")]
    [SerializeField] private StudentPanel _studentPanelPrefab;
    [SerializeField] private GameObject _levelPanelPrefab;
    [SerializeField] private SubmittedStudentDataView _submittedStudentDataViewPrefab;

    [Header("Views")]
    [SerializeField] private GameObject _connectingView;
    [SerializeField] private GameObject _joiningLobbyView;
    [SerializeField] private GameObject _creatingRoomView;
    [SerializeField] private GameObject _levelSelectionView;
    [SerializeField] private GameObject _gameView;
    
    [Header("Level Selection View")]
    [SerializeField] private TMP_Text _roomNameText;
    [SerializeField] private Transform _studentListRoot;
    [SerializeField] private Transform _levelPanelRoot;

    [Header("Game View")]
    [SerializeField] private TMP_Text _gameViewTitle;
    [SerializeField] private Transform _submittedStudentDataViewLayout;
    
    private Dictionary<int, StudentPanel> _studentPanels = new Dictionary<int, StudentPanel>();

    private string _roomName;
    private int _level = -1;
    private StudentInput _allowedInputs;
    private string _map;

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

    static public void OnBackToLevelSelectionView()
    {
        SetupInfo();
        DataCarrier.RequestSetupInfoToAllStudent();
        TeacherManager.ShowLevelSelectionView(_instance._roomName);
    }

    static public void SetupInfo(int level = -1, StudentInput allowedInputs = 0, string map = "")
    {
        _instance._level = level;
        _instance._allowedInputs = allowedInputs;
        _instance._map = map;
        ShowGameView(level, map);
    }

    static public void PlayInputs(SubmittedStudentDataView submittedStudentData)
    {
        Destroy(submittedStudentData.gameObject); // TODO : 
    }

    static public void ShowConnectingView()
    {
        _instance._connectingView.SetActive(true);
        _instance._joiningLobbyView.SetActive(false);
        _instance._creatingRoomView.SetActive(false);
        _instance._levelSelectionView.SetActive(false);
        _instance._gameView.SetActive(false);
    }

    static public void ShowJoiningLobbyView()
    {
        _instance._connectingView.SetActive(false);
        _instance._joiningLobbyView.SetActive(true);
        _instance._creatingRoomView.SetActive(false);
        _instance._levelSelectionView.SetActive(false);
        _instance._gameView.SetActive(false);
    }

    static public void ShowCreatingRoomView()
    {
        _instance._connectingView.SetActive(false);
        _instance._joiningLobbyView.SetActive(false);
        _instance._creatingRoomView.SetActive(true);
        _instance._levelSelectionView.SetActive(false);
        _instance._gameView.SetActive(false);
    }

    static public void ShowLevelSelectionView(string roomName)
    {
        _instance._connectingView.SetActive(false);
        _instance._joiningLobbyView.SetActive(false);
        _instance._creatingRoomView.SetActive(false);
        _instance._levelSelectionView.SetActive(true);
        _instance._gameView.SetActive(false);

        _instance._roomName = roomName;
        _instance._roomNameText.text = roomName;
    }

    static public void ShowGameView(int level, string map)
    {
        ClearStudentRequests();

        _instance._connectingView.SetActive(false);
        _instance._joiningLobbyView.SetActive(false);
        _instance._creatingRoomView.SetActive(false);
        _instance._levelSelectionView.SetActive(false);
        _instance._gameView.SetActive(true);

        //set map / level
        _instance._gameViewTitle.text = $"{level}";
    }

    static public void RemoveStudent(int actorNumber)
    {
        Destroy(_instance._studentPanels[actorNumber].gameObject);
        _instance._studentPanels.Remove(actorNumber);
    }

    static public void AddStudent(int actorNumber, string studentName)
    {
        StudentPanel studentPanel = Instantiate(_instance._studentPanelPrefab, _instance._studentListRoot);
        _instance._studentPanels.Add(actorNumber, studentPanel);
        studentPanel.Setup(studentName);
    }

    static public void EnqueueStudentRequest(int actorNumber, IEnumerable<StudentInput> inputs)
    {
        Instantiate(_instance._submittedStudentDataViewPrefab, _instance._submittedStudentDataViewLayout).Setup(actorNumber, inputs);
    }

    static public void ClearStudentRequests()
    {
        foreach(Transform submittedView in _instance._submittedStudentDataViewLayout)
        {
            Destroy(submittedView);
        }
    }
    
    static public string GetStudentName(int actorNumber)
    {
        return _instance._studentPanels[actorNumber].studentName;
    }
}
