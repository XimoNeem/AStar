using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public bool IsMooving = false;
    public Tile CurrentTile;
    private Coroutine _moveCoroutine;

    [SerializeField] private float _jumpHeight = 1;
    [SerializeField] private AnimationCurve _jumpCurve, _angerCurve;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        FindObjectOfType<PathFinder>().SetPlayer(this);
    }

    public void MoveByPath(List<Tile> tiles)
    {
        if (_moveCoroutine != null) { StopCoroutine(_moveCoroutine); }

        _moveCoroutine = StartCoroutine(Move(tiles));
        foreach (var item in FindObjectsOfType<Tile>()) { item.ResetColor(); }
    }

    public void StaySolid()
    {
        foreach (var item in FindObjectsOfType<Tile>()) { item.ResetColor(); }
        StartCoroutine(Jump());
    }

    private IEnumerator Jump()
    {
        float t = 0;
        Vector3 defaultPos = this.transform.position;
        while (t < 1)
        {
            t += Time.fixedDeltaTime;
            this.transform.position = defaultPos + Vector3.up * _angerCurve.Evaluate(t);
            yield return new WaitForEndOfFrame();
        }
        IsMooving = false;
        this.transform.position = defaultPos;
    }

    private IEnumerator Move(List<Tile> path)
    {
        Vector3 startPoint, endPoint, currentPosition;
        float distance, t;

        foreach (var item in path)
        {
            CurrentTile = item;

            currentPosition = startPoint = this.transform.position;
            endPoint = item.transform.position;
            distance = Vector3.Distance(startPoint, endPoint);
            t = 0;

            float yOffset = this.transform.position.y - item.transform.position.y;
            this.transform.LookAt(item.transform.position + Vector3.up * yOffset);

            while (currentPosition != endPoint)
            {
                t = Mathf.Lerp(t, 1, Time.fixedDeltaTime * 30);
                currentPosition = Vector3.Lerp(startPoint, endPoint, t);
                this.transform.position = currentPosition;
                this.transform.position += Vector3.up * _jumpHeight * _jumpCurve.Evaluate(t);

                yield return new WaitForFixedUpdate();
            }
        }
        IsMooving = false;
        StopCoroutine(_moveCoroutine);
    }
}
