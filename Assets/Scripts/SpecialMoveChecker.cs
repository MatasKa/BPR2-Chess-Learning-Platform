using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialMoveChecker : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private GameObject[] promotionPrefabs;
    private Piece enPassantTargetPawn { get; set; }
    private Vector2Int enPassantSquare;



    public void CheckSpecialMoves(string square)
    {
        int newPosX = int.Parse(square[0].ToString());
        int newPosY = int.Parse(square[1].ToString());
        Vector2Int newPos = new Vector2Int(newPosX, newPosY);
        CheckEnPassant(newPos, board.GetCurrentPiece());
        if ((newPos.y - board.GetCurrentPiece().GetCurrentSquare().y == 2 || newPos.y - board.GetCurrentPiece().GetCurrentSquare().y == -2) && board.GetCurrentPiece() is Pawn)
        {
            PrepEnPassantTarget(board.GetCurrentPiece(), newPos);
        }
    }



    public void CheckEnPassant(Vector2Int newPos, Piece piece)
    {
        Debug.Log("en pas " + newPos + " square " + enPassantSquare + " piece " + piece);
        if (newPos == enPassantSquare && piece is Pawn)
        {
            Debug.Log("en passantAAAAA");
            board.CapturePiece(enPassantTargetPawn);
        }
        PrepEnPassantTarget(null, new Vector2Int(-1, -1)); //cant call pawn.IsWhite if pawn is null
        enPassantSquare = new Vector2Int(-1, -1);
    }

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


    public void PromotePawn(int promotion)
    {
        StartCoroutine(ReplacePieceType(promotion));
        board.uiManager.ChangePieceSprite(board.GetCurrentPiece().gameObject, promotion, board.GetCurrentPiece().IsWhite());
        board.turnManager.SwitchTurn(board.GetAllPieces(), board);
    }

    IEnumerator ReplacePieceType(int promotion)
    {
        int index = System.Array.IndexOf(board.GetAllPieces(), board.GetCurrentPiece());

        Vector2Int pos = board.GetCurrentPiece().GetCurrentSquare();
        Quaternion savedRotation = board.GetCurrentPiece().transform.rotation;
        bool isWhite = board.GetCurrentPiece().IsWhite();

        Destroy(board.GetCurrentPiece().gameObject);
        yield return null;

        int prefab = isWhite ? promotion : promotion + 4;
        GameObject newPieceObj = Instantiate(promotionPrefabs[prefab], new Vector3(pos.x, pos.y, 0), savedRotation);

        Piece newPiece = newPieceObj.GetComponent<Piece>();
        newPiece.SetWhite(isWhite);
        newPiece.SetCurrentSquare(pos);

        board.GetAllPieces()[index] = newPiece;
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
