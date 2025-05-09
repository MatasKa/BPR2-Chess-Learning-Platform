using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private BoardRenderer boardRenderer;
    private TurnManager turnManager = new TurnManager();

    private Piece[] pieces;
    private Piece whiteKing;
    private Piece blackKing;
    private Piece currentPiece;

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

    public void MovePiece(string square)
    {
        int newPosX = int.Parse(square[0].ToString());
        int newPosY = int.Parse(square[1].ToString());
        Vector2Int newPos = new Vector2Int(newPosX, newPosY);

        Piece maybeEnemyPiece = GetPieceOnSquare(newPos);
        if (maybeEnemyPiece != null && IsEnemyPiece(currentPiece, maybeEnemyPiece))
        {
            maybeEnemyPiece.gameObject.SetActive(false);
            maybeEnemyPiece.SetCaptured(true);
        }

        currentPiece.SetCurrentSquare(newPos);
        currentPiece.transform.position = new Vector3(newPosX, newPosY, currentPiece.transform.position.z);

        ResetHighlights();
        turnManager.SwitchTurn(pieces);
        IsKingInCheck(true);
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

    public void SimulateMove(Piece piece, Vector2Int newPos, Piece capturedPiece, out Action restoreAction)
    {
        Vector2Int oldPos = piece.GetCurrentSquare();
        bool wasCaptured = false;

        if (capturedPiece != null)
        {
            wasCaptured = capturedPiece.IsCaptured();
            capturedPiece.SetCaptured(true);
            capturedPiece.gameObject.SetActive(false);
        }

        piece.SetCurrentSquare(newPos);
        piece.transform.position = new Vector3(newPos.x, newPos.y, piece.transform.position.z);

        restoreAction = () =>
        {
            piece.SetCurrentSquare(oldPos);
            piece.transform.position = new Vector3(oldPos.x, oldPos.y, piece.transform.position.z);

            if (capturedPiece != null)
            {
                capturedPiece.SetCaptured(wasCaptured);
                capturedPiece.gameObject.SetActive(true);
            }
        };
    }
}
