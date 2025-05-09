using UnityEngine;
using UnityEngine.UI;

public class BoardRenderer : MonoBehaviour
{
    [SerializeField] private GameObject[] squareArray;
    private GameObject[,] squares = new GameObject[8, 8];

    public void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int o = 0; o < 8; o++)
            {
                squares[i, o] = squareArray[i * 8 + o];
            }
        }
    }

    public void Highlight(Vector2Int square)
    {
        squares[square.x, square.y].GetComponent<SpriteRenderer>().enabled = true;
        squares[square.x, square.y].GetComponent<Button>().interactable = true;
    }

    public void ResetHighlights()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int o = 0; o < 8; o++)
            {
                squares[i, o].GetComponent<SpriteRenderer>().enabled = false;
                squares[i, o].GetComponent<Button>().interactable = false;
            }
        }
    }
}
