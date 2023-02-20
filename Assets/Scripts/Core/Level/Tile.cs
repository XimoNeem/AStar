using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Tile : MonoBehaviour
{
    public bool IsOccupied = false;
    public float TileLength = 1, TileHeight = 1;

    public float Weight { get; set; }
    public Tile FromTile { get; set; }

    private Vector3 _defaulSize = Vector3.one, _defaultPosition = Vector3.zero;
    private Color _defautColor;

    private const float ANIMATION_SPEED = 10;

    virtual public void Awake()
    {
        _defautColor = GetComponentInChildren<MeshRenderer>().material.color;
    }

    private void Start()
    {
        _defaultPosition = this.transform.position;
        ChangeSize(1f);
    }

    public List<Tile> GetNearestTiles()
    {
        Tile tile = this;
        List<Tile> result = new List<Tile>();

        RaycastHit hit;

        Ray[] directions = new Ray[]
                {
                    new Ray(tile.transform.position, tile.transform.forward),
                    new Ray(tile.transform.position, -tile.transform.forward),
                    new Ray(tile.transform.position, tile.transform.right),
                    new Ray(tile.transform.position, -tile.transform.right),
                };

        foreach (var item in directions)
        {
            if (Physics.Raycast(item, out hit, 1))
            {
                try
                {
                    Tile newTile = hit.transform.GetComponent<Tile>();
                    if (newTile != this & !newTile.IsOccupied) { result.Add(newTile); }
                }
                catch (System.Exception)
                {
                    throw;
                }
            }
        }

        return result;
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if (Player.Instance.IsMooving || this == Player.Instance.CurrentTile) return;
            SetOccupied(!IsOccupied);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Player.Instance.IsMooving || this == Player.Instance.CurrentTile || this.IsOccupied) return;
            FindObjectOfType<PathFinder>().GeneratePath(this);
        }
    }
    private void OnMouseEnter()
    {
        if (Player.Instance.IsMooving || this == Player.Instance.CurrentTile) return;
        ChangeSize(0.8f, 0.35f);
    }

    private void OnMouseExit()
    {
        if (Player.Instance.IsMooving) return;
        ChangeSize(1f);
    }

    public void SetDefaultColor(Color color)
    {
        _defautColor = color;
    }

    public void SetColor(Color color)
    {
        GetComponentInChildren<MeshRenderer>().material.color = color;
        if (_defautColor == null)
        {
            _defautColor = color;
        }
    }

    public void ResetColor()
    {
        GetComponentInChildren<MeshRenderer>().material.color = _defautColor;
    }

    public void SetOccupied(bool state)
    {
        IsOccupied = state;
        if (state)
        {
            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.transform.parent = this.transform;
            obstacle.transform.localScale = new Vector3(0.3f, 0.7f, 0.3f);
            obstacle.transform.localPosition = new Vector3(0, 0.6f, 0);
            obstacle.AddComponent<Obstacle>();
            Destroy(obstacle.GetComponent<Collider>());
        }
        else 
        {
            foreach (var item in GetComponentsInChildren<Obstacle>())
            {
                Destroy(item.gameObject);
            }
        }
    }
    public void ChangeSize(float size)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateSize(size, _defaultPosition));
    }
    public void ChangeSize(float size, float yOffset)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateSize(size, new Vector3(this.transform.position.x, this.transform.position.y + yOffset, this.transform.position.z)));
    }
    private IEnumerator UpdateSize(float size, Vector3 newPosition)
    {
        Vector3 newSize = _defaulSize * size;

        while (this.transform.localScale != newSize || this.transform.position != newPosition)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, newPosition, Time.fixedDeltaTime * ANIMATION_SPEED);
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, newSize, Time.fixedDeltaTime * ANIMATION_SPEED);
            yield return new WaitForFixedUpdate();
        }

        StopAllCoroutines();
    }
}
