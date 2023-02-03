using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class Tile : MonoBehaviour
{
    [SerializeField] private bool IsOccupied = false;
    [SerializeField] private bool IsTarget = false;
    public int ID { get; }
    [SerializeField] private LayerMask _layerMask;

    public float TileLength { get; private set; } = 1;
    public float TileHeight { get; private set; } = 1;
    public int depth { get; private set; }

    private SpriteRenderer _spriteRenderer;
    private TMP_Text _text;
    private Collider _collider;
    private Vector3 _defaulSize = Vector3.one;
    private Vector3 _defaultPosition = Vector3.zero;

    virtual public void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<Collider>();
        _text = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        _defaultPosition = this.transform.position;
        SetDepth();
        ChangeSize(1);
    }

    public List<Tile> GetNearestTiles()
    {
        Tile tile = this;
        List<Tile> result = new List<Tile>();
        List<RaycastHit2D> raycastHits = new List<RaycastHit2D>();

        raycastHits.AddRange(Physics2D.RaycastAll(new Vector2(tile._collider.bounds.center.x, tile._collider.bounds.center.y), Vector2.Lerp(Vector2.up, Vector2.right, 0.5f), 0.5f, _layerMask));
        raycastHits.AddRange(Physics2D.RaycastAll(new Vector2(tile._collider.bounds.center.x, tile._collider.bounds.center.y), Vector2.Lerp(Vector2.up, Vector2.left, 0.5f), 0.5f, _layerMask));
        raycastHits.AddRange(Physics2D.RaycastAll(new Vector2(tile._collider.bounds.center.x, tile._collider.bounds.center.y), Vector2.Lerp(Vector2.down, Vector2.right, 0.5f), 0.5f, _layerMask));
        raycastHits.AddRange(Physics2D.RaycastAll(new Vector2(tile._collider.bounds.center.x, tile._collider.bounds.center.y), Vector2.Lerp(Vector2.down, Vector2.left, 0.5f), 0.5f, _layerMask));

        foreach (var item in raycastHits)
        {
            Debug.Log(item.transform.name);
            try
            {
                Tile newTile = item.transform.GetComponent<Tile>();
                if (newTile != this & !newTile.IsOccupied) { result.Add(newTile); }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        return result;
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
        {
            SetOccupied(!IsOccupied);
        }

        if (Input.GetMouseButtonDown(0))
        {
            print("Target");
        }
    }
    private void OnMouseEnter()
    {
        ChangeSize(0.8f, 0.5f);
    }

    private void OnMouseExit()
    {
        ChangeSize(1f);
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
    public void SetDepth(int value)
    {
        depth = value;
        if (_spriteRenderer != null) { _spriteRenderer.sortingOrder = depth; }
        if (_text != null) { }
    }
    public void SetDepth()
    {
        SetDepth(depth);
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
            this.transform.position = Vector3.Lerp(this.transform.position, newPosition, Time.fixedDeltaTime * 5);
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, newSize, Time.fixedDeltaTime * 5);
            yield return new WaitForFixedUpdate();
        }

        StopAllCoroutines();
    }
}
