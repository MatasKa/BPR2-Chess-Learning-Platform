using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialMoveChecker : MonoBehaviour
{
    [SerializeField] private Board board;
    private Piece enPassantTargetPawn { get; set; }
    private Vector2Int enPassantSquare;

    public void PrepEnPassantTarget(Piece pawn, Vector2Int newPos) // could be private
    {
        if (pawn != null)
        {
            int dir = (pawn.IsWhite() == true) ? -1 : 1;
            enPassantSquare = new Vector2Int(newPos.x, newPos.y + dir);
        }
        enPassantTargetPawn = pawn;
    }

    public Piece GetEnPassantTarget()
    {
        return enPassantTargetPawn;
    }

    public void CheckEnPassant(Vector2Int newPos, Piece piece)
    {
        if (newPos == enPassantSquare && piece is Pawn)
        {
            board.CapturePiece(enPassantTargetPawn);
        }
        PrepEnPassantTarget(null, new Vector2Int(-1, -1)); //cant call pawn.IsWhite if pawn is null
        enPassantSquare = new Vector2Int(-1, -1);
    }

    public void PromotePawn(int promotion)
    {
        StartCoroutine(ReplacePieceType(promotion));
        board.uiManager.ChangePieceSprite(board.GetCurrentPiece().gameObject, promotion, board.GetCurrentPiece().IsWhite());
        board.turnManager.SwitchTurn(board.GetAllPieces(), board);
    }

    IEnumerator ReplacePieceType(int promotion)
    {
        //promotions: 0 - Queen, 1 - Rook, 2 - Bishop, 3 - Knight
        int index = System.Array.IndexOf(board.GetAllPieces(), board.GetCurrentPiece());
        Vector2Int pos = board.GetCurrentPiece().GetCurrentSquare();
        bool white = board.GetCurrentPiece().IsWhite();
        Destroy(board.GetCurrentPiece().gameObject.GetComponent<Pawn>());
        if (promotion == 0)
        {
            board.GetCurrentPiece().gameObject.AddComponent<Queen>();
            board.GetAllPieces()[index] = board.GetCurrentPiece().GetComponent<Queen>();
        }
        else if (promotion == 1)
        {
            board.GetCurrentPiece().gameObject.AddComponent<Rook>();
            board.GetAllPieces()[index] = board.GetCurrentPiece().GetComponent<Rook>();

        }
        else if (promotion == 2)
        {
            board.GetCurrentPiece().gameObject.AddComponent<Bishop>();
            board.GetAllPieces()[index] = board.GetCurrentPiece().GetComponent<Bishop>();

        }
        else
        {
            board.GetCurrentPiece().gameObject.AddComponent<Knight>();
            board.GetAllPieces()[index] = board.GetCurrentPiece().GetComponent<Knight>();

        }

        board.GetAllPieces()[index].SetWhite(white);
        board.GetAllPieces()[index].SetCurrentSquare(pos);

        yield return null;
    }

    public bool HasRookMoved(Piece piece)
    {
        if (piece is Rook rook)
        {
            return rook.GetHasMoved();
        }
        else
        {
            return true;
        }
    }

    //checks if king does not pass (and enter) a check when castling (Kingside)
    public bool PassesCheckKingsideCastle(bool isWhite)
    {
        int Ypos = (isWhite == true) ? 0 : 7;
        foreach (Piece piece in board.GetAllPieces())
        {
            if (!piece.IsCaptured() && piece.IsWhite() != isWhite)
            {
                List<Vector2Int> moves;
                if (piece is King king)
                {
                    moves = king.StandartMoves();
                }
                else
                {
                    moves = piece.PossibleMoves();
                }
                if (moves.Contains(new Vector2Int(4, Ypos))
                || moves.Contains(new Vector2Int(5, Ypos))
                || moves.Contains(new Vector2Int(6, Ypos)))
                    return true;
            }
        }
        return false;
    }

    public bool PassesCheckQueensideCastle(bool isWhite)
    {
        int Ypos = (isWhite == true) ? 0 : 7;
        foreach (Piece piece in board.GetAllPieces())
        {
            if (!piece.IsCaptured() && piece.IsWhite() != isWhite)
            {
                List<Vector2Int> moves;
                if (piece is King king)
                {
                    moves = king.StandartMoves();
                }
                else
                {
                    moves = piece.PossibleMoves();
                }
                Debug.Log("checkina Y: " + Ypos);
                if (moves.Contains(new Vector2Int(1, Ypos))
                || moves.Contains(new Vector2Int(2, Ypos))
                || moves.Contains(new Vector2Int(3, Ypos))
                || moves.Contains(new Vector2Int(4, Ypos)))
                    return true;
            }
        }
        return false;
    }
}
