using UnityEngine;

public class ResponsiveObject : MonoBehaviour
{
    public Vector2 Resolution = new Vector2(1080, 2340);

    private void Awake()
    {
        var screenSize = new Vector2(Screen.width, Screen.height);

        Debug.Log(screenSize);

        var originalScale = transform.localScale;

        var ratio = new Vector2(screenSize.x / Resolution.x, screenSize.y / Resolution.y);

        transform.localScale = new Vector3(originalScale.x * ratio.x / ratio.y, originalScale.y, originalScale.z);
    }
}
