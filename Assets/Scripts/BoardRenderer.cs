using UnityEngine;
using UnityEngine.UI;

public class BoardRenderer : MonoBehaviour
{
    [SerializeField] private GameObject[] squareArray;
    [SerializeField] private GameObject[] LastMoveSquares;
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

    public void ShowLastMove(Vector2Int squareFrom, Vector2Int squareTo)
    {
        float offset = -3.5f;
        if (LastMoveSquares[0].activeSelf == false)
        {
            LastMoveSquares[0].SetActive(true);
            LastMoveSquares[1].SetActive(true);
        }
        float x = squareFrom.x + offset;
        float y = squareFrom.y + offset;
        Debug.Log("LastMoveSquares " + squareFrom + " " + squareTo + ", setting one's position to " + x + " " + y);
        LastMoveSquares[0].transform.localPosition = new Vector3(x, y, LastMoveSquares[0].transform.position.z);
        LastMoveSquares[1].transform.localPosition = new Vector3(squareTo.x + offset, squareTo.y + offset, LastMoveSquares[1].transform.position.z);
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
