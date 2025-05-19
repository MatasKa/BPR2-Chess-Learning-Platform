using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private BoardRenderer boardRenderer;
    public TurnManager turnManager { get; set; }
    public UIManager uiManager { get; set; }
    public SpecialMoveChecker specialMoveChecker;

    private Piece[] pieces;
    private King whiteKing;
    private King blackKing;
    private Piece currentPiece;

    //for simulating moves
    private Vector2Int prevPos;
    private bool prevCapState;
    private Piece simCapPiece;

    void Start()
    {
        turnManager = gameObject.GetComponent<TurnManager>();
        uiManager = FindAnyObjectByType<UIManager>();
        pieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);
        whiteKing = GameObject.Find("White King").GetComponent<King>();
        blackKing = GameObject.Find("Black King").GetComponent<King>();
    }
    public void Highlight(Vector2Int square)
    {
        boardRenderer.Highlight(square);
    }
    public Piece GetPieceOnSquare(Vector2Int checkPos)
    {
        foreach (Piece piece in pieces)
        {
            if (piece.GetCurrentSquare() == checkPos && piece.IsCaptured() == false)
            {
                return piece;
            }
        }
        return null;
    }
    public Piece[] GetPieces()
    {
        return pieces;
    }
    public bool IsInsideBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8;
    }
    public bool IsEnemyPiece(Piece current, Piece other)
    {
        return current.IsWhite() != other.IsWhite();
    }
    public void ResetHighlights()
    {
        boardRenderer.ResetHighlights();
    }
    public void SelectPiece(Piece selectPiece)
    {
        foreach (Piece piece in pieces)
        {
            if (piece.IsSelected())
            {
                piece.SetSelected(false);
            }
        }
        currentPiece = selectPiece;
        currentPiece.SetSelected(true);
    }
    public Piece GetCurrentPiece()
    {
        return currentPiece;
    }
    public Piece[] GetAllPieces()
    {
        return pieces;
    }
    public void MovePiece(string square)
    {
        int newPosX = int.Parse(square[0].ToString());
        int newPosY = int.Parse(square[1].ToString());
        Vector2Int newPos = new Vector2Int(newPosX, newPosY);

        //capture piece if there is one
        Piece maybeEnemyPiece = GetPieceOnSquare(newPos);
        if (maybeEnemyPiece != null && IsEnemyPiece(currentPiece, maybeEnemyPiece))
        {
            CapturePiece(maybeEnemyPiece);
        }
        //En Passant checks
        specialMoveChecker.CheckEnPassant(newPos, currentPiece);
        if ((newPos.y - currentPiece.GetCurrentSquare().y == 2 || newPos.y - currentPiece.GetCurrentSquare().y == -2) && currentPiece is Pawn)
        {
            specialMoveChecker.PrepEnPassantTarget(currentPiece, newPos);
        }
        //moving the piece
        currentPiece.SetCurrentSquare(newPos);
        currentPiece.transform.position = new Vector3(newPosX, newPosY, currentPiece.transform.position.z);
        ResetHighlights();
        //Check for Pawn promotion
        if (currentPiece is Pawn && (currentPiece.GetCurrentSquare().y == 0 || currentPiece.GetCurrentSquare().y == 7))
        {
            uiManager.ShowPawnPromotionUI(currentPiece.GetCurrentSquare(), currentPiece.IsWhite());
            turnManager.StopAllPieces(pieces);
            return;
        }
        //Castling
        if (currentPiece is King king)
        {
            if (king.GetHasMoved() == false)
            {
                int Ypos = (king.IsWhite() == true) ? 0 : 7;
                new Vector2Int(6, Ypos);
                //kingside
                if (newPos == new Vector2Int(6, Ypos))
                {
                    GetPieceOnSquare(new Vector2Int(7, Ypos)).transform.position = new Vector3(5, Ypos, GetPieceOnSquare(new Vector2Int(7, Ypos)).transform.position.z);
                    GetPieceOnSquare(new Vector2Int(7, Ypos)).SetCurrentSquare(new Vector2Int(5, Ypos));
                }
                //queenside
                if (newPos == new Vector2Int(1, Ypos))
                {
                    GetPieceOnSquare(new Vector2Int(0, Ypos)).transform.position = new Vector3(2, Ypos, GetPieceOnSquare(new Vector2Int(0, Ypos)).transform.position.z);
                    GetPieceOnSquare(new Vector2Int(0, Ypos)).SetCurrentSquare(new Vector2Int(0, Ypos));
                }
                king.SetHasMoved(true);
            }
        }
        if (currentPiece is Rook rook)
        {
            rook.SetHasMoved(true);
        }
        turnManager.SwitchTurn(pieces, this);
    }



    public void CapturePiece(Piece piece)
    {
        piece.SetCaptured(true);
        piece.gameObject.SetActive(false);
    }


    public bool HasAnyLegalMoves(bool isWhite)
    {
        foreach (Piece piece in pieces)
        {
            if (!piece.IsCaptured() && piece.IsWhite() == isWhite)
            {
                var moves = piece.PossibleMoves();
                foreach (var move in moves)
                {
                    if (piece.IsMoveLegal(move))
                        return true;
                }
            }
        }
        return false;
    }

    public bool IsKingInCheck(bool isWhite)
    {
        Piece king = isWhite ? whiteKing : blackKing;
        Vector2Int kingPos = king.GetCurrentSquare();
        //Debug.Log(king.gameObject.name + " IsKingInCheck");
        foreach (Piece piece in pieces)
        {
            if (!piece.IsCaptured() && piece.IsWhite() != isWhite)
            {
                var moves = piece.PossibleMoves();
                if (moves.Contains(kingPos))
                    return true;
            }
        }
        return false;
    }

    public void DoSimulatedMove(Piece piece, Vector2Int newPos)
    {
        prevPos = piece.GetCurrentSquare();
        simCapPiece = GetPieceOnSquare(newPos);
        if (simCapPiece != null)
        {
            prevCapState = simCapPiece.IsCaptured();
            simCapPiece.SetCaptured(true);
            simCapPiece.gameObject.SetActive(false);
        }
        piece.SetCurrentSquare(newPos);
    }

    public void UndoSimulatedMove(Piece piece)
    {
        piece.SetCurrentSquare(prevPos);
        if (simCapPiece != null)
        {
            simCapPiece.SetCaptured(prevCapState);
            simCapPiece.gameObject.SetActive(true);
        }
        simCapPiece = null;
    }
}
