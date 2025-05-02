using System.Text.RegularExpressions;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] protected Vector2Int currentSquare;
    protected Board board;
    protected bool selected = false;
    protected bool captured = false;
    [SerializeField] protected bool white = false;

    void Start()
    {
        board = FindAnyObjectByType<Board>();
    }
    private void OnMouseDown()
    {
        Debug.Log("tipo pasirinktas");
        selected = true;
        PossibleMoves();
    }

    public bool IsSelected()
    {
        return selected;
    }

    public bool IsWhite()
    {
        return selected;
    }

    public void SetCaptured(bool cap)
    {
        captured = cap;
    }
    public bool IsCaptured()
    {
        return captured;
    }
    public virtual void PossibleMoves()
    {
        board.ResetHighlights();
    }

    public void SetCurrentSquare(Vector2Int square)
    {
        currentSquare = square;
    }
    public Vector2Int GetCurrentSquare()
    {
        return currentSquare;
    }
}