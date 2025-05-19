using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] protected Vector2Int currentSquare;
    [SerializeField] protected bool white = false;
    protected Board board;
    protected bool selected = false;
    protected bool captured = false;

    void Start()
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
    public bool IsCaptured()
    {
        return captured;
    }
    public void SetCaptured(bool cap)
    {
        captured = cap;
    }
    public Vector2Int GetCurrentSquare()
    {
        return currentSquare;
    }
    public void SetCurrentSquare(Vector2Int square)
    {
        currentSquare = square;
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
    public bool CanMoveToSquare(Vector2Int pos, Piece piece)
    {
        if (board == null)
        {
            board = FindAnyObjectByType<Board>();
        }
        //Debug.Log(piece.gameObject.name + " tikrins, type: " + piece);
        //Debug.Log(pos + " Inside board " + board.IsInsideBoard(pos));
        //Debug.Log(pos + " Get piece " + board.GetPieceOnSquare(pos));

        if (board.IsInsideBoard(pos) && (board.GetPieceOnSquare(pos) == null))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CanCapture(Piece piece, Vector2Int pos)
    {
        if (board.GetPieceOnSquare(pos) != null && board.IsEnemyPiece(piece, board.GetPieceOnSquare(pos)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual List<Vector2Int> PossibleMoves()
    {
        return new List<Vector2Int>();
    }


}
