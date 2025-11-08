using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] private LineRenderer _renderer;
    [SerializeField] private EdgeCollider2D _collider;

    private List<Vector2> _points = new();

    public void SetPosition(Vector2 position)
    {
        if (CamAppend(position) == false) return;

        Vector2 localPosition = transform.InverseTransformPoint(position);

        _points.Add(localPosition);

        _renderer.positionCount++;
        _renderer.SetPosition(_renderer.positionCount - 1, localPosition);
    }

    public void OnEndDraw()
    {
        if (_points.Count < 2) return;

        this.tag = "Wall";

        Vector2 center = Vector2.zero;
        foreach (var p in _points)
        {
            center += p;
        }
        center /= _points.Count;

        transform.position += (Vector3)center;

        for (int i = 0; i < _points.Count; i++)
        {
            _points[i] -= center;
        }

        _renderer.positionCount = _points.Count;
        for (int i = 0; i < _points.Count; i++)
        {
            _renderer.SetPosition(i, _points[i]);
        }

        _collider.points = _points.ToArray();

        var rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.mass = 10;
    }

    private bool CamAppend(Vector2 position)
    {
        if (_renderer.positionCount == 0) return true;

        return Vector2.Distance(_renderer.GetPosition(_renderer.positionCount - 1), position) > DrawManager.RESOLUTION;
    }
}