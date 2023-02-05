using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _grassTile;

    [SerializeField] private int _length = 10, _height = 10;

    [SerializeField] private Transform _levelParent;
    //private Vector3 _offset;

    public List<List<Tile>> _tiles; 


    private void Start()
    {
        _tiles = new List<List<Tile>>();
        //_offset = new Vector3(0, (_height * -_grassTile.GetComponent<Tile>().TileHeight) / 2, 0);
        StartCoroutine(GenerateLevel());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) { SceneManager.LoadScene(0); }
    }

    public IEnumerator GenerateLevel()
    {
        float xSize = _grassTile.GetComponent<Tile>().TileLength;
        float ySize = _grassTile.GetComponent<Tile>().TileHeight;

        for (int h = 0; h < _height; h++)
        {
            _tiles.Add(new List<Tile>());
            for (int l = 0; l < _length; l++)
            {
                float xPos = ((xSize * l));
                float yPos = (ySize * h);

                Vector3 pos = new Vector3(xPos, 0, -yPos);

                CreateTile(_grassTile, pos, h + l, h);

                yield return new WaitForSecondsRealtime(0.01f);
            }
        }

        //StartCoroutine(GenerateBuildings());

        /*foreach (var item in _tiles)
        {
            foreach (var tile in item)
            {
                Debug.Log(tile.gameObject.name);
            }
        }*/
    }

   /* private IEnumerator GenerateBuildings()
    {
        foreach (var item in _tiles)
        {
            foreach (var tile in item)
            {
                if (Random.Range(0, 100) > 95)
                {
                    GameObject b = Instantiate(_building, tile.transform.position, Quaternion.identity);
                    b.GetComponentInChildren<SpriteRenderer>().sortingOrder = tile.depth + 2;
                    yield return new WaitForSecondsRealtime(0.005f);
                }
            }

            yield return new WaitForSecondsRealtime(0.005f);
        }
    }*/

    private void CreateTile(GameObject prefab, Vector3 position, int depth, int row)
    {
        GameObject newTile = Instantiate(prefab, _levelParent);
        newTile.transform.position = position;
        newTile.transform.localScale = Vector3.zero;

        Tile tile = newTile.GetComponent<Tile>();

        _tiles[row].Add(tile);
    }
}
