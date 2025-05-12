using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private BoardRenderer boardRenderer;
    [SerializeField] private TurnManager turnManager;

    private Piece[] pieces;
    private Piece whiteKing;
    private Piece blackKing;
    private Piece currentPiece;


    //for simulating moves
    private Vector2Int prevPos;
    private bool prevCapState;
    private Piece simCapPiece;

    private Piece enPassantTargetPawn = null;
    private Vector2Int enPassantSquare;

    void Start()
    {
        pieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);
        whiteKing = GameObject.Find("White King").GetComponent<Piece>();
        blackKing = GameObject.Find("Black King").GetComponent<Piece>();
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

    public bool IsInsideBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8;
    }

    public bool IsEnemyPiece(Piece current, Piece other)
    {
        return current.IsWhite() != other.IsWhite();
    }

    public void PrepEnPassantTarget(Piece pawn, Vector2Int newPos) // could be private
    {
        if (pawn != null)
        {
            int dir = (pawn.IsWhite() == true) ? -1 : 1;
            enPassantSquare = new Vector2Int(newPos.x, newPos.y + dir);
        }
        enPassantTargetPawn = pawn;
        //Debug.Log("en passant Target set: " + enPassantTargetPawn.name);
        //Debug.Log("en passant Square set: " + enPassantSquare);
    }

    public Piece GetEnPassantTarget()
    {
        return enPassantTargetPawn;
    }

    public void CheckEnPassant(Vector2Int newPos, Piece piece)
    {
        if (newPos == enPassantSquare && piece is Pawn)
        {
            CapturePiece(enPassantTargetPawn);
        }
        PrepEnPassantTarget(null, new Vector2Int(-1, -1));
        enPassantSquare = new Vector2Int(-1, -1);
    }


    public void ResetHighlights()
    {
        boardRenderer.ResetHighlights();
    }

    public void SelectPiece(Piece sellectPiece)
    {
        foreach (Piece piece in pieces)
        {
            if (piece.IsSelected())
            {
                piece.SetSelected(false);
            }
        }
        currentPiece = sellectPiece;
        currentPiece.SetSelected(true);
    }

    public void MovePiece(string square) //MOOOOOOOOOOVEEEEEEEEEEE
    {
        int newPosX = int.Parse(square[0].ToString());
        int newPosY = int.Parse(square[1].ToString());
        Vector2Int newPos = new Vector2Int(newPosX, newPosY);



        Piece maybeEnemyPiece = GetPieceOnSquare(newPos);
        if (maybeEnemyPiece != null && IsEnemyPiece(currentPiece, maybeEnemyPiece))
        {
            CapturePiece(maybeEnemyPiece);
        }



        CheckEnPassant(newPos, currentPiece);
        if ((newPos.y - currentPiece.GetCurrentSquare().y == 2 || newPos.y - currentPiece.GetCurrentSquare().y == -2) && currentPiece is Pawn)
        {
            PrepEnPassantTarget(currentPiece, newPos);
        }

        currentPiece.SetCurrentSquare(newPos);
        currentPiece.transform.position = new Vector3(newPosX, newPosY, currentPiece.transform.position.z);

        ResetHighlights();
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
        piece.transform.position = new Vector3(newPos.x, newPos.y, piece.transform.position.z);
    }

    public void UndoSimulatedMove(Piece piece)
    {
        piece.SetCurrentSquare(prevPos);
        piece.transform.position = new Vector3(prevPos.x, prevPos.y, piece.transform.position.z);

        if (simCapPiece != null)
        {
            simCapPiece.SetCaptured(prevCapState);
            simCapPiece.gameObject.SetActive(true);
        }

        simCapPiece = null;
    }
}
