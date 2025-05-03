using System;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject[] squareArray;
    private GameObject[,] squares = new GameObject[8, 8];
    private Piece[] pieces;
    private String lastTurnSide = "Black";
    void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int o = 0; o < 8; o++)
            {
                squares[i, o] = squareArray[i * 8 + o];
                Debug.Log("square [" + o + ", " + i + "] name is " + squares[i, o].gameObject.name);
            }
        }
        pieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);
        Debug.Log("pieces count:" + pieces.Length);
    }

    public void Highlight(Vector2Int square)
    {
        Debug.Log("Highlight square:" + square.x + " " + square.y);
        squares[square.x, square.y].GetComponent<SpriteRenderer>().enabled = true;
    }

    public Piece GetPieceOnSquare(Vector2Int checkPos)
    {
        foreach (Piece piece in pieces)
        {
            if (piece.GetCurrentSquare() == checkPos && piece.IsCaptured() == false)
            {
                return piece;
                //return IsEnemyPiece(piece.gameObject);
            }
        }
        return null;
    }

    public bool IsInsideBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8;
    }

    public bool IsEnemyPiece(Piece piece)
    {
        if (piece.gameObject.tag == lastTurnSide)
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
                squares[i, o].GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    private void InvertLastTurn()
    {
        if (lastTurnSide == "Black")
        {
            lastTurnSide = "White";
        }
        else
        {
            lastTurnSide = "Black";
        }
    }
}
