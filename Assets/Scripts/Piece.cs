using UnityEngine;

public class Piece : MonoBehaviour
{
    private Vector3 offset;
    private bool selected = false;

    private void OnMouseDown()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        offset = transform.position - Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        selected = true;
        Debug.Log(gameObject.name + " is selected");
    }

    private void OnMouseUp()
    {
        selected = false;
    }

    private void Update()
    {
        if (selected)
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            transform.position = Camera.main.ScreenToWorldPoint(mouseScreenPosition) + offset;
        }
    }
}