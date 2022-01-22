using System.Collections;
using UnityEngine;
using static GameState;

public class ActorMediator : MonoBehaviour
{
    private Vector3 TransformStatePositionToRealPosition(Vector2Int statePosition)
    {
        return new Vector3(statePosition.y, 0.0f, statePosition.x);
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition, 
            TransformStatePositionToRealPosition(globalState.actorState.position), 
            Time.deltaTime * 4.0f);
    }
}
