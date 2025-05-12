using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] protected Vector2Int currentSquare;
    protected Board board;
    protected bool selected = false;
    protected bool captured = false;
    [SerializeField] protected bool white = false;

    void Awake()
    {
        board = FindAnyObjectByType<Board>();
    }

    private void OnMouseDown()
    {
        if (captured == false)
        {
            board.SelectPiece(this);
            ShowLegalMoves();
        }
    }

    public bool IsSelected()
    {
        return selected;
    }
    public void SetSelected(bool sel)
    {
        selected = sel;
    }
    public bool IsWhite()
    {
        return white;
    }

    public void SetWhite(bool w)
    {
        white = w;
    }

    public void SetCaptured(bool cap)
    {
        captured = cap;
    }
    public bool IsCaptured()
    {
        return captured;
    }
    public Vector2Int GetCurrentSquare()
    {
        return currentSquare;
    }
    public void SetCurrentSquare(Vector2Int square)
    {
        currentSquare = square;
    }

    public virtual List<Vector2Int> PossibleMoves()
    {
        return new List<Vector2Int>();
    }

    public void ShowLegalMoves()
    {
        board.ResetHighlights();

        List<Vector2Int> allMoves = PossibleMoves();
        foreach (var move in allMoves)
        {
            if (IsMoveLegal(move))
            {
                board.Highlight(move);
            }
        }
    }

    public bool IsMoveLegal(Vector2Int targetPos)
    {
        board.DoSimulatedMove(this, targetPos);
        bool inCheck = board.IsKingInCheck(white);
        board.UndoSimulatedMove(this);

        return !inCheck;
    }
}
