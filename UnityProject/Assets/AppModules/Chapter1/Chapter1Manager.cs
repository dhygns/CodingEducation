using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Chapter1Manager : MonoBehaviour
{
    [Serializable]
    public class SelectableObject
    {
        public GameObject gameObject;
        public string messageToDisplay;

        public T[] GetComponentsInChildren<T>()
        {
            return gameObject.GetComponentsInChildren<T>();
        }
    }

    [Serializable]
    public class QuestionAndAnswer
    {
        public string question;
        public GameObject[] answer;
        public bool isAnswered = false;
    }

    enum Mode { Learn, Solve }

    public SelectableObject[] selectableObjects;
    public QuestionAndAnswer[] questionAndAnswers;
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private TMP_Text _subMessageText;
    [SerializeField] private Texture2D _cursorTexture;

    private SelectableObject _targetSO;
    private QuestionAndAnswer _currentQnA;
    private GameObject _clickedLastObject = null;
    private Mode _currentMode = Mode.Learn;
    private bool _isReadyToSolve = false;
    private Coroutine _logicHandler;

    void Awake()
    {
        Cursor.SetCursor(_cursorTexture, new Vector2(24f, 4f), CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
        {
            _targetSO = selectableObjects.FirstOrDefault(so => so.gameObject == hitInfo.collider.gameObject);

            if (_targetSO != null)
            {
                foreach (var GO in selectableObjects)
                {
                    foreach (var rdrr in GO.GetComponentsInChildren<Renderer>())
                    {
                        rdrr.material.color = GO == _targetSO ? Color.white : new Color(0.2f, 0.2f, 0.2f, 1.0f);
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    _clickedLastObject = _targetSO.gameObject;
                }
            }
            else
            {
                foreach (var GO in selectableObjects)
                {
                    foreach (var rdrr in GO.GetComponentsInChildren<Renderer>())
                    {
                        rdrr.material.color = Color.white;
                    }
                }
            }
        }
        else
        {
            _targetSO = null;

            foreach (var GO in selectableObjects)
            {
                foreach (var rdrr in GO.GetComponentsInChildren<Renderer>())
                {
                    rdrr.material.color = Color.white;
                }
            }
        }

        switch (_currentMode)
        {
            case Mode.Learn:
                {
                    if (_logicHandler == null)
                    {
                        _logicHandler = StartCoroutine(LearnLogic());
                    }
                }
                break;
            case Mode.Solve:
                {
                    if (_logicHandler == null)
                    {
                        _logicHandler = StartCoroutine(SolveLogic());
                    }
                }
                break;
        }
    }

    private void UpdateTextAlpha(TMP_Text targetText, float alpha)
    {
        targetText.color = Color.Lerp(targetText.color, new Color(targetText.color.r, targetText.color.g, targetText.color.b, alpha), Time.deltaTime * 8.0f);
    }

    IEnumerator LearnLogic()
    {
        SelectableObject dislpayedSO = null;
        while (_currentMode == Mode.Learn)
        {
            if (_targetSO != null)
            {
                if (dislpayedSO != _targetSO)
                {
                    UpdateTextAlpha(_messageText, 0f);
                    UpdateTextAlpha(_subMessageText, 0f);

                    if (_messageText.color.a < 0.001f)
                    {
                        _messageText.text = _targetSO.messageToDisplay;
                        _subMessageText.text = "";  
                        dislpayedSO = _targetSO;
                    }
                }
                else 
                {
                    UpdateTextAlpha(_messageText, 1.0f);
                    UpdateTextAlpha(_subMessageText, 1.0f);
                }
            }
            else 
            {
                UpdateTextAlpha(_messageText, 0.0f);
                UpdateTextAlpha(_subMessageText, 0.0f);
            }

            yield return null;
        }

        _logicHandler = null;
    }

    IEnumerator SolveLogic()
    {
        _messageText.color = new Color(_messageText.color.r, _messageText.color.g, _messageText.color.b, 1.0f);
        _subMessageText.color = new Color(_subMessageText.color.r, _subMessageText.color.g, _subMessageText.color.b, 1.0f);

        while(_currentMode == Mode.Solve)
        {
            var remainedQnA = questionAndAnswers.Where(qna => !qna.isAnswered);

            System.Random rand = new System.Random();
            int index = rand.Next(remainedQnA.Count());
            _currentQnA = remainedQnA.ElementAt(index);

            _messageText.text = _currentQnA.question;
            _clickedLastObject = null;
            
            while (_currentMode == Mode.Solve)
            {
                if (_currentQnA.answer.Contains(_clickedLastObject))
                {
                    _isReadyToSolve = false;
                    _subMessageText.text = "<color=#00FF00>YES</color>";
                    _clickedLastObject = null;

                    yield return new WaitForSeconds(4.0f);

                    _messageText.text = "";
                    _subMessageText.text = "";
                    _currentQnA.isAnswered = true;

                    break;
                }
                else if (_clickedLastObject != null)
                {
                    _subMessageText.text = "Nahh :P";
                    _clickedLastObject = null;
                }

                yield return null;
            }

            yield return null;
        }

        _logicHandler = null;
    }

    public void MoveToNext()
    {
        Debug.LogWarning(_currentMode);
        _currentMode = Mode.Solve;
    }
}
