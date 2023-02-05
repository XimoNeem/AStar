using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public Transform marker;
    private Player _player;
    private Tile _target;

    private List<Tile> _checkTiles;
    private List<Tile> _closedTiles;
    private Tile _currentTile;

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public void GeneratePath(Tile target)
    {
        _target = target;
        StartCoroutine(Search());
    }

    public IEnumerator Search()
    {
        //List<Tile> result = new List<Tile>();
        _player = FindObjectOfType<Player>();
        _currentTile = _player.CurrentTile;

        _checkTiles = new List<Tile>();
        _closedTiles = new List<Tile>();

        while (_currentTile != _target)
        {
            List<Tile> newTiles = new List<Tile>();
            foreach (var item in _currentTile.GetNearestTiles())
            {
                if (!item.IsOccupied && !_checkTiles.Contains(item) && !_closedTiles.Contains(item))
                {
                    item.Weight = getTileWeight(item);
                    newTiles.Add(item);
                    item.ChangeSize(0.7f);
                }
            }
            if (newTiles.Count == 0)
            {
                _checkTiles.Remove(_currentTile);
                _closedTiles.Add(_currentTile);
            }

            _checkTiles.AddRange(newTiles);
            _checkTiles.Sort((x, y) => x.Weight.CompareTo(y.Weight));

            _closedTiles.Add(_currentTile);
            _currentTile = _checkTiles[0];
            _closedTiles.Add(_currentTile);
            marker.position = _currentTile.transform.position;

            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Done");
        //return result;
    }

    private float getTileWeight(Tile tile)
    {
        float result = Vector3.Distance(tile.transform.position, _target.transform.position);

        return result;
    }
}
