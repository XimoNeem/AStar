using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;

    [SerializeField] private GameObject _grassTile;

    [SerializeField] private int _length = 10, _height = 10;
    [SerializeField] private float _noiseScale = 3;
    [SerializeField] private Gradient _color;

    [SerializeField] private Transform _levelParent;

    public List<List<Tile>> _tiles;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        _tiles = new List<List<Tile>>();
        StartCoroutine(GenerateLevel());
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

    public IEnumerator GenerateLevel()
    {
        float xSize = _grassTile.GetComponent<Tile>().TileLength;
        float ySize = _grassTile.GetComponent<Tile>().TileHeight;

        float noiseX = Random.Range(0f, 100f);
        float noiseY = Random.Range(0f, 100f);

        for (int h = 0; h < _height; h++)
        {
            _tiles.Add(new List<Tile>());
            for (int l = 0; l < _length; l++)
            {
                float xPos = ((xSize * l));
                float yPos = (ySize * h);

                float xNoise = noiseX + (float)h / (float)_height * _noiseScale;
                float yNoise = noiseY + (float)l / (float)_length * _noiseScale;
                float zPos = Mathf.PerlinNoise(xNoise, yNoise);

                Vector3 pos = new Vector3(xPos, zPos, -yPos);

                CreateTile(_grassTile, pos, h + l, h, zPos);

                yield return new WaitForSecondsRealtime(0.01f);
            }
        }

        List<Tile> target = new List<Tile>() { _tiles[0][0] };
        FindObjectOfType<Player>().MoveByPath(target);
    }

    private void CreateTile(GameObject prefab, Vector3 position, int depth, int row, float noise)
    {
        GameObject newTile = Instantiate(prefab, _levelParent);
        newTile.transform.position = position;
        newTile.transform.localScale = Vector3.zero;

        Tile tile = newTile.GetComponent<Tile>();

        _tiles[row].Add(tile);
        tile.SetColor(_color.Evaluate(noise));
        tile.SetDefaultColor(_color.Evaluate(noise));
    }
}
