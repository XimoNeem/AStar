using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Tile CurrentTile;
    private Coroutine _moveCoroutine;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (CurrentTile == null)
            {
                CurrentTile = FindObjectOfType<Tile>();
                transform.position = CurrentTile.transform.position;
            }

            List<Tile> tiles = CurrentTile.GetNearestTiles();
            if (tiles.Count == 0) { SceneManager.LoadScene(0); }
            MoveToTile(tiles[Random.Range(0, tiles.Count)]);
        }
    }
    public void MoveToTile(Tile tile)
    {
        if (_moveCoroutine != null) { StopCoroutine(_moveCoroutine); }
        _moveCoroutine = StartCoroutine(Move(tile.transform.position));
        CurrentTile.SetOccupied(true);
        CurrentTile = tile;
    }

    private IEnumerator Move(Vector3 target)
    {
        while (this.transform.position != target)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, target, Time.fixedDeltaTime * 30);
            yield return new WaitForFixedUpdate();
        }
        StopCoroutine(_moveCoroutine);
    }
}
