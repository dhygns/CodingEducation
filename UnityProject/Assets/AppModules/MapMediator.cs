using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameState;

public class MapMediator : MonoBehaviour
{
    [SerializeField] private Transform _mapRoot;
    [SerializeField] private GameObject _movableTilePrefab;

    private Dictionary<(int, int), GameObject> _createdMapTiles = new Dictionary<(int, int), GameObject>();

    public void Awake()
    {
        globalState.mapState.onMapChanged += OnMapChanged;
        globalState.mapState.onTileChanged += OnTileChanged;
    }

    public void OnDestroy()
    {
        globalState.mapState.onMapChanged -= OnMapChanged;
        globalState.mapState.onTileChanged -= OnTileChanged;
    }

    public void OnMapChanged()
    {
        foreach(var item in _createdMapTiles.Values)
        {
            Destroy(item);
        }
        _createdMapTiles.Clear();

        int column = globalState.mapState.column;
        int row = globalState.mapState.row;

        transform.position = Vector3.left * ((column - 1) / 2.0f) + Vector3.back * ((row - 1) / 2.0f);

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                GameObject tileGO = Instantiate(_movableTilePrefab, Vector3.right * x + Vector3.forward * y, Quaternion.identity);
                _createdMapTiles.Add((x, y), tileGO);
                tileGO.transform.SetParent(transform, false);
            }
        }
    }

    public void OnTileChanged((int, int) coord)
    {
        
    }
}
