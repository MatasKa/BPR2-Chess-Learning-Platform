using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
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

        bool AIMove = false;
        if (square.Length != 2)
        {
            AIMove = true;
        }
        Vector2Int newPos = new Vector2Int(newPosX, newPosY);

        CheckEnPassant(newPos, board.GetCurrentPiece());
        if ((newPos.y - board.GetCurrentPiece().GetCurrentSquare().y == 2 || newPos.y - board.GetCurrentPiece().GetCurrentSquare().y == -2) && board.GetCurrentPiece() is Pawn)
        {
            PrepEnPassantTarget(board.GetCurrentPiece(), newPos);
        }
        //Debug.Log("Pawn Promo Check incoming: " + board.GetCurrentPiece() + " and position is " + board.GetCurrentPiece().GetCurrentSquare());
        //Check for Pawn promotion
        if (board.GetCurrentPiece() is Pawn && (newPos.y == 0 || newPos.y == 7))
        {
            if (AIMove == true)
            {
                PromotePawn(0);
            }
            else
            {
                //Debug.Log("Pawn Promo");
                board.uiManager.ShowPawnPromotionUI(board.GetCurrentPiece().GetCurrentSquare(), board.GetCurrentPiece().IsWhite());
                board.turnManager.StopAllPieces(board.GetAllPieceObjects());
                return;
            }
        }

        //Castling
        if (board.GetCurrentPiece() is King king)
        {
            if (king.GetHasMoved() == false)
            {
                int Ypos = (king.IsWhite() == true) ? 0 : 7;
                new Vector2Int(6, Ypos);
                //kingside
                if (newPos == new Vector2Int(6, Ypos))
                {
                    board.GetPieceOnSquare(new Vector2Int(7, Ypos)).transform.position = new Vector3(5, Ypos, board.GetPieceOnSquare(new Vector2Int(7, Ypos)).transform.position.z);
                    board.GetPieceOnSquare(new Vector2Int(7, Ypos)).SetCurrentSquare(new Vector2Int(5, Ypos));
                }
                //queenside
                if (newPos == new Vector2Int(1, Ypos))
                {
                    board.GetPieceOnSquare(new Vector2Int(0, Ypos)).transform.position = new Vector3(2, Ypos, board.GetPieceOnSquare(new Vector2Int(0, Ypos)).transform.position.z);
                    board.GetPieceOnSquare(new Vector2Int(0, Ypos)).SetCurrentSquare(new Vector2Int(2, Ypos));
                }
                king.SetHasMoved(true);
            }
        }
        //if (board.GetCurrentPiece() is Rook rook)
        //{
        //    rook.SetHasMoved(true);
        //}
        board.GetCurrentPiece().SetHasMoved(true);
    }
    public Piece GetPieceFromBoard(Vector2Int Pos)
    {
        return board.GetPieceOnSquare(Pos);
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
    public void PrepEnPassantTarget(Piece pawn, Vector2Int newPos)
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
        //board.uiManager.ChangePieceSprite(board.GetCurrentPiece().gameObject, promotion, board.GetCurrentPiece().IsWhite());
        //board.turnManager.SwitchTurn(board.GetAllPieceObjects(), board);
    }
    IEnumerator ReplacePieceType(int promotion)
    {
        int indexPiece = System.Array.IndexOf(board.GetAllPieces(), board.GetCurrentPiece());
        int indexObject = System.Array.IndexOf(board.GetAllPieceObjects(), board.GetCurrentPiece().gameObject);

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

        board.SelectPiece(newPiece);

        board.GetAllPieces()[indexPiece] = newPiece;
        board.GetAllPieceObjects()[indexObject] = newPiece.gameObject;
        board.turnManager.CheckForEndGame(board);

    }
    public bool HasRookMoved(Piece piece)
    {
        return piece.GetHasMoved();
    }
    public bool PassesCheckKingsideCastle(bool isWhite)
    {
        int Ypos = (isWhite == true) ? 0 : 7;
        Debug.Log("Chech KINGSIDE, Y " + Ypos);
        foreach (Piece piece in board.GetAllPieces())
        {
            if (!piece.IsCaptured() && piece.IsWhite() != isWhite)
            {
                List<Vector2Int> moves;
                if (piece is King king)
                {
                    moves = king.StandartMoves();
                }
                else if (piece is Pawn pawn)
                {
                    moves = pawn.PossibleAttacks();
                }
                else
                {
                    moves = piece.PossibleMoves();
                }

                if (moves.Contains(new Vector2Int(4, Ypos))
                || moves.Contains(new Vector2Int(5, Ypos))
                || moves.Contains(new Vector2Int(6, Ypos)))
                {
                    return true;
                }
            }
        }

        return false;
    }
    public bool PassesCheckQueensideCastle(bool isWhite)
    {
        int Ypos = (isWhite == true) ? 0 : 7;
        Debug.Log("Chech QUEENSide, Y " + Ypos);
        foreach (Piece piece in board.GetAllPieces())
        {
            if (!piece.IsCaptured() && piece.IsWhite() != isWhite)
            {
                List<Vector2Int> moves;
                if (piece is King king)
                {
                    moves = king.StandartMoves();
                }
                else if (piece is Pawn pawn)
                {
                    moves = pawn.PossibleAttacks();
                }
                else
                {
                    moves = piece.PossibleMoves();
                }

                //Debug.Log("moves Queensid check " + moves);

                if (moves.Contains(new Vector2Int(1, Ypos))
                || moves.Contains(new Vector2Int(2, Ypos))
                || moves.Contains(new Vector2Int(3, Ypos))
                || moves.Contains(new Vector2Int(4, Ypos)))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
