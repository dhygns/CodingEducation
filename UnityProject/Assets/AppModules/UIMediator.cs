using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameState;
using static GameState.ActorState;

public class UIMediator : MonoBehaviour
{
    public void OnActionButtonClicked(string actionType)
    {
        globalState.actorState.EnqueueAction((ActionType)System.Enum.Parse(typeof(ActionType), actionType));
    }

    public void OnResetButtonClicked()
    {
        globalState.SetIsGameReady(true);
    }

    public void OnRunButtonClicked()
    {
        globalState.SetIsGameRunning(true);
    }
}