using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private MapGenerator _mapGenerator;
    private Player _player;
    private Tile _target;


    public void SetPlayer(Player player)
    {
        _player = player;
    }
    public void GeneratePath(Tile target)
    {
        _target = target;
    }

    public float GetTileWeight(Tile tile)
    {
        return 5;
    }
}
