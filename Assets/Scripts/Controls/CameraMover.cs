using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMover : MonoBehaviour
{
    private Camera _camera;
    private Vector3 _mousePosition;
    private Vector3 _camOffset;

    [SerializeField][Range(0.001f, 0.1f)] float _sensivity = 0.01f;

    void Start()
    {
        _camera = this.GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 newPos = Input.mousePosition;

            _camOffset = _mousePosition - newPos;

            Vector3 currentPos = _camera.transform.position;

            currentPos.x += _camOffset.x * _sensivity;
            currentPos.y += _camOffset.y * _sensivity;

            _camera.transform.position = currentPos;

            _mousePosition = Input.mousePosition;
        }
    }
}
