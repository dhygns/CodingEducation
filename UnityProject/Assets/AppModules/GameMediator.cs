using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameState;

public class GameMediator : MonoBehaviour
{
    [SerializeField] private float _waitingTimeBetweenActions = 1.0f;
    private Coroutine _coroutineHandler = null;

    private void Awake()
    {
        globalState.isRunningChanged += OnIsRunningChanged;
        globalState.isReadyChanged += OnIsReadyChanged;
    }

    private void OnDestroy()
    {
        globalState.isRunningChanged -= OnIsRunningChanged;
        globalState.isReadyChanged -= OnIsReadyChanged;
    }

    private IEnumerator InternalRunActions()
    {
        globalState.actorState.Reset();

        do {
            yield return new WaitForSeconds(_waitingTimeBetweenActions);
        }
        while (
            globalState.actorState.TryExecuteQueuedAction() && 
            globalState.mapState.IsMovable(globalState.actorState.lastPosition, globalState.actorState.position));

        _coroutineHandler = null;
    }

    private void OnIsReadyChanged()
    {
        if (globalState.isGameReady)
        {
            globalState.mapState.SetMap(UnityEngine.Random.Range(2, 10), 1);
            globalState.actorState.Reset();
            
            globalState.SetIsGameRunning(false);
        }
    }

    private void OnIsRunningChanged()
    {
        if (globalState.isGameReady && globalState.isGameRunning)
        {
            if (_coroutineHandler != null)
            {
                StopCoroutine(_coroutineHandler);
            }
            _coroutineHandler = StartCoroutine(InternalRunActions());
            globalState.SetIsGameReady(false);
        }
    }
}
