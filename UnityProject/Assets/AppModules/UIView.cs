using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameState;
using static GameState.ActorState;

public class UIView : MonoBehaviour
{
    [Serializable]
    public class ActionUI
    {
        public ActionType type;
        public GameObject prefab;
    }

    [SerializeField] private ActionUI[] _actionUIs;

    [SerializeField] private Transform _UIQ;

    [SerializeField] private Transform enqPoint;
    [SerializeField] private Transform deqPoint;

    private Queue<GameObject> deqAnimationQ = new Queue<GameObject>();
    private List<GameObject> queuedObjects = new List<GameObject>();

    private void Awake()
    {
        globalState.actorState.onActionEnqueued += OnActionEnqueued;
        globalState.actorState.onActionDequeued += OnActionDequeued;
    }

    private void OnDestroy()
    {
        globalState.actorState.onActionEnqueued -= OnActionEnqueued;
        globalState.actorState.onActionDequeued -= OnActionDequeued;
    }

    public void Update()
    {
        if (deqAnimationQ.Count > 0)
        {
            Transform deqTarget = deqAnimationQ.Peek().transform;
            deqTarget.localPosition = Vector3.Lerp(deqTarget.localPosition, deqPoint.localPosition, Time.deltaTime * 8.0f);
            
            if (Vector3.Distance(deqTarget.localPosition, deqPoint.localPosition) <= 0.1f)
            {
                // get rid of itme from queue
                deqAnimationQ.Dequeue();
                // and.. destroy it
                Destroy(deqTarget.gameObject);
            }
        }

        for(int i = 0; i < queuedObjects.Count ; i++)
        {
            queuedObjects[i].transform.localPosition =  Vector3.Lerp(queuedObjects[i].transform.localPosition, Vector3.up * 100.0f * (i - (queuedObjects.Count - 1) / 2.0f), Time.deltaTime * 8.0f);
        }
    }

    public void OnActionEnqueued(ActionType actionType)
    {
        // create new action ui prefab
        GameObject actionGO = _actionUIs.Where(item => item.type == actionType).Select(item => item.prefab).First();
        if (actionGO != null)
        {
            actionGO = Instantiate(actionGO, _UIQ);
            queuedObjects.Add(actionGO);
            actionGO.transform.localPosition = enqPoint.localPosition;
        }
        else return;
    }

    public void OnActionDequeued(ActionType actionType)
    {
        deqAnimationQ.Enqueue(queuedObjects[0]);
        queuedObjects.RemoveAt(0);
    }
}
