using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameState
{
    private static GameState _globalState = null;
    public static GameState globalState => _globalState = _globalState ?? new GameState();

    private static ActorState _actorState = null;
    public ActorState actorState => _actorState = _actorState ?? new ActorState();

    private static MapState _mapState = null;
    public MapState mapState = _mapState = _mapState ?? new MapState();


    private bool _isGameReady = false;
    public bool isGameReady => _isGameReady;

    public Action isReadyChanged;

    public void SetIsGameReady(bool value)
    {
        if (_isGameReady != value)
        {
            _isGameReady = value;
            isReadyChanged?.Invoke();
        }
    }

    private bool _isGameRunning = false;
    public bool isGameRunning => _isGameRunning;

    public Action isRunningChanged;

    public void SetIsGameRunning(bool value)
    {
        if (_isGameRunning != value)
        {
            _isGameRunning = value;
            isRunningChanged?.Invoke();
        }
    }

    [Serializable]
    public class ActorState
    {
        [Serializable]
        public enum ActionType { 
            Move = 1, 
            TurnLeft = 2 
        }
        
        public ActionType lastActionState { get; private set; }
        public Vector2Int lastPosition { get; private set; }
        public Vector2Int position { 
            get => _position; 
            private set {
                lastPosition = _position;
                _position = value;
            }
        }

        public Vector2Int direction { get; private set; }

        public Queue<ActionType> actionQ = new Queue<ActionType>();
        public Action<ActionType> onActionEnqueued;
        public Action<ActionType> onActionDequeued;

        private Vector2Int _position;

        public void Reset()
        {
            lastPosition = Vector2Int.zero;
            position = Vector2Int.zero;
            direction = Vector2Int.right;
        }

        public void EnqueueAction(ActionType type)
        {
            actionQ.Enqueue(type);
            onActionEnqueued?.Invoke(type);
        }

        public bool TryExecuteQueuedAction()
        {
            if (actionQ.Count > 0)
            {
                ActionType type = actionQ.Dequeue();

                switch(type)
                {
                    case ActionType.Move:
                        {
                            position += direction;
                        }
                        break;

                    case ActionType.TurnLeft:
                        {
                            direction = new Vector2Int(-direction.y, direction.x);
                        }
                        break;
                }

                onActionDequeued?.Invoke(type);
                return true;
            }

            return false;
        }
    }

    [Serializable]
    public class MapState
    {
        [Serializable, Flags]
        public enum WallType : byte 
        { 
            None = 0, 
            N = 1, 
            E = 2, 
            W = 4, 
            S = 8 
        }

        public int row { get; private set; }
        public int column { get; private set; }
        public Dictionary<(int, int), WallType> tiles = new Dictionary<(int, int), WallType>();
        
        public Action onMapChanged;
        public Action<(int, int)> onTileChanged;

        public void ClearMap()
        {
            this.row = 0;
            this.column = 0;
            tiles.Clear();

            onMapChanged?.Invoke();
        }

        public void SetMap(int column, int row)
        {
            tiles.Clear();

            this.row = row;
            this.column = column;

            for (int x = 0; x < column; x++)
            {
                for (int y = 0; y < row; y++)
                {
                    tiles.Add((x, y), WallType.None);
                }
            }

            onMapChanged?.Invoke();
        }

        public void AddWall(int x, int y, WallType wallType = WallType.None)
        {
            if (tiles.ContainsKey((x, y)))
            {
                tiles[(x, y)] = wallType;
                onTileChanged?.Invoke((x, y));
            }
        }

        public bool IsMovable(Vector2Int prevPos, Vector2Int currPos)
        {
            Vector2Int dir = currPos - prevPos;

            if (tiles.TryGetValue((currPos.x, currPos.y), out WallType wallType))
            {
                return 
                    !((wallType & WallType.W) != WallType.None && dir.x == 1) &&
                    !((wallType & WallType.E) != WallType.None && dir.x == -1) &&
                    !((wallType & WallType.N) != WallType.None && dir.y == -1) &&
                    !((wallType & WallType.S) != WallType.None && dir.y == 1);
            }
            else 
            {
                return false;
            }
        }
    }
}
