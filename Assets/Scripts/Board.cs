using System;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.UIElements;
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
                Debug.Log("square [" + o + ", " + i + "] name is " + squares[i, o].gameObject.name);
            }
        }
        pieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);
        Debug.Log("pieces count:" + pieces.Length);
    }

    public void Highlight(Vector2Int square)
    {
        //Debug.Log("Highlight square:" + square.x + " " + square.y);
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
                //return IsEnemyPiece(piece.gameObject);
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
        if (whiteTurn == true)
        {
            foreach (Piece piece in pieces)
            {
                if (piece.IsWhite() == false)
                {
                    piece.gameObject.GetComponent<BoxCollider2D>().enabled = true;
                }
                else
                {
                    piece.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
            whiteTurn = false;
        }
        else
        {
            foreach (Piece piece in pieces)
            {
                if (piece.IsWhite() == true)
                {
                    piece.gameObject.GetComponent<BoxCollider2D>().enabled = true;
                }
                else
                {
                    piece.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
            whiteTurn = true;
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
        Debug.Log("selected " + currentPiece.name);
    }

    public void MovePiece(string square)
    {
        Debug.Log("square pressed:" + square);
        int newPosX = int.Parse(square[0].ToString());
        int newPosY = int.Parse(square[1].ToString());
        Vector2Int newPos = new Vector2Int(newPosX, newPosY);
        currentPiece.SetCurrentSquare(newPos);
        currentPiece.gameObject.transform.position = new Vector3(newPosX, newPosY, currentPiece.transform.position.z);
        ResetHighlights();
        ChangeTurn();
    }

    private void CapturePiece(Piece capturedPiece)
    {

    }
}
