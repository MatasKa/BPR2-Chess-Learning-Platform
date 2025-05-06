using System;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Board : MonoBehaviour
{
    [SerializeField] private GameObject[] squareArray;
    private GameObject[,] squares = new GameObject[8, 8];
    private Piece[] pieces;
    private Piece currentPiece;
    private bool whiteTurn = true;
    void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int o = 0; o < 8; o++)
            {
                squares[i, o] = squareArray[i * 8 + o];
            }
        }
        pieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);
    }

    public void Highlight(Vector2Int square)
    {
        squares[square.x, square.y].GetComponent<SpriteRenderer>().enabled = true;
        squares[square.x, square.y].GetComponent<Button>().interactable = true;
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
        if (current.gameObject.tag != other.gameObject.tag)
        {
            return true;
        }
        else
        {
            return false;
        }
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
    private void ChangeTurn()
    {
        whiteTurn = !whiteTurn;

        foreach (Piece piece in pieces)
        {
            bool shouldEnable = piece.IsWhite() == whiteTurn;
            piece.gameObject.GetComponent<BoxCollider2D>().enabled = shouldEnable;
        }
    }

    public void SelectPiece(Piece sellectPiece)
    {
        foreach (Piece piece in pieces)
        {
            if (piece.IsSelected() == true)
            {
                piece.SetSelected(false);
            }
        }
        currentPiece = sellectPiece;
        currentPiece.SetSelected(true);
    }

    public void MovePiece(string square)
    {
        Piece maybeEnemyPiece = null;
        int newPosX = int.Parse(square[0].ToString());
        int newPosY = int.Parse(square[1].ToString());
        Vector2Int newPos = new Vector2Int(newPosX, newPosY);
        maybeEnemyPiece = GetPieceOnSquare(newPos);
        if (maybeEnemyPiece != null)
        {
            if (IsEnemyPiece(maybeEnemyPiece, currentPiece) == true)
            {
                CapturePiece(maybeEnemyPiece);
            }
        }
        currentPiece.SetCurrentSquare(newPos);
        currentPiece.gameObject.transform.position = new Vector3(newPosX, newPosY, currentPiece.transform.position.z);
        ResetHighlights();
        ChangeTurn();
    }

    private void CapturePiece(Piece capturedPiece)
    {
        capturedPiece.gameObject.SetActive(false);
        capturedPiece.SetCaptured(true);
    }
}
