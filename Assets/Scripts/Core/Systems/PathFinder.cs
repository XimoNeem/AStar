using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
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
        Player.Instance.IsMooving = true;

        foreach (var item in FindObjectsOfType<Tile>())
        {
            item.SetColor(Color.white);
            item.ChangeSize(1);
        }

        _checkTiles = new List<Tile>();
        _closedTiles = new List<Tile>();

        _checkTiles.Add(_player.CurrentTile);

        _target = target;
        StartCoroutine(Search());
    }

    public IEnumerator Search()
    {
        while (true)
        {
            _checkTiles.Sort((x, y) => x.Weight.CompareTo(y.Weight));

            if (_checkTiles.Count == 0)
            {
                Player.Instance.StaySolid();
                StopAllCoroutines();
            }

            _currentTile = _checkTiles[0];
            _checkTiles.Remove(_currentTile);
            _closedTiles.Add(_currentTile);
            _currentTile.SetColor(Color.red);

            if (_currentTile == _target)
            {
                SendPathToPlayer();
                StopAllCoroutines();
            }

            foreach (var item in _currentTile.GetNearestTiles())
            {
                yield return new WaitForEndOfFrame();

                if (item.IsOccupied || _closedTiles.Contains(item)) continue;

                if (!_checkTiles.Contains(item))
                {
                    item.Weight = getTileWeight(item);
                    item.FromTile = _currentTile;
                    _checkTiles.Add(item);
                    item.SetColor(Color.blue);
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void SendPathToPlayer()
    {
        List<Tile> moveQuary = new List<Tile>();
        Tile current = _target;
        while (current.FromTile != _player.CurrentTile)
        {
            moveQuary.Add(current);
            current = current.FromTile;
        }
        moveQuary.Add(current);
        moveQuary.Reverse();
        _player.MoveByPath(moveQuary);
    }

    private float getTileWeight(Tile tile)
    {
        float result = Vector3.Distance(tile.transform.position, _target.transform.position);
        return result;
    }
}
