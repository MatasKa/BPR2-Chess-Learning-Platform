using UnityEngine;
using UnityEngine.UI;

public class MainMenuBackground : MonoBehaviour
{
    public float scrollSpeed = 0.1f;
    private RawImage image;
    private Rect uvRect;

    void Start()
    {
        image = GetComponent<RawImage>();
        uvRect = image.uvRect;
    }

    void Update()
    {
        uvRect.x += scrollSpeed * Time.deltaTime;
        uvRect.y += scrollSpeed * Time.deltaTime;
        image.uvRect = uvRect;
    }
}