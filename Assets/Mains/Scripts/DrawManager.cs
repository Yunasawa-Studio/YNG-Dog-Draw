using UnityEngine;

public class DrawManager : MonoBehaviour
{
    public const float RESOLUTION = 0.1f;

    [SerializeField] private Line _linePrefab;
    [SerializeField] private float _pointDistance = 0.1f;

    [SerializeField] private Rigidbody2D _player;
    [SerializeField] private Beehive _beehive;

    private Line _currentLine;
    private Vector2 _previousMousePos;
    private bool _isDrawing;

    private void Update()
    {
        Vector3 mousePosition3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition = new Vector2(mousePosition3D.x, mousePosition3D.y);

        if (Input.GetMouseButtonDown(0))
        {
            _currentLine = Instantiate(_linePrefab, new Vector3(mousePosition.x, mousePosition.y, 0), Quaternion.identity);
            _previousMousePos = mousePosition;
            _isDrawing = true;
        }

        if (Input.GetMouseButton(0) && _isDrawing && _currentLine != null)
        {
            float distance = Vector2.Distance(mousePosition, _previousMousePos);
            if (distance > _pointDistance)
            {
                _currentLine.SetPosition(mousePosition);
                _previousMousePos = mousePosition;
            }
        }

        if (Input.GetMouseButtonUp(0) && _isDrawing)
        {
            _currentLine.OnEndDraw();
            _currentLine = null;
            _isDrawing = false;

            _beehive.Begin();

            _player.gravityScale = 1;
        }
    }
}
