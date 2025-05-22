using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private BoardRenderer boardRenderer;
    [SerializeField] private MoveHistory moveHistory;
    [SerializeField] private SpecialMoveChecker specialMoveChecker;

    public TurnManager turnManager { get; set; }
    public UIManager uiManager { get; set; }

    private Piece[] pieces;
    private GameObject[] pieceObjects;
    private Piece whiteKing;
    private Piece blackKing;
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
        whiteKing = GameObject.Find("White King").GetComponent<Piece>();
        blackKing = GameObject.Find("Black King").GetComponent<Piece>();
        pieceObjects = new GameObject[pieces.Length];
        for (int i = 0; i < pieces.Length; i++)
        {
            pieceObjects[i] = pieces[i].gameObject;
        }
        //Debug.Log("Board " + pieceObjects[0].name);

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
    public GameObject[] GetAllPieceObjects()
    {
        return pieceObjects;
    }
    public void MovePlayerPiece(string square)
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

        //add move to history of moves
        string move = moveHistory.TranslateMoveToUci(currentPiece.GetCurrentSquare(), newPos);
        moveHistory.AddMove(move);

        //moving the piece
        currentPiece.SetCurrentSquare(newPos);
        currentPiece.transform.position = new Vector3(newPosX, newPosY, currentPiece.transform.position.z);
        ResetHighlights();

        //Debug.Log("Will change turns soon");
        currentPiece.SetHasMoved(true);
        turnManager.SwitchTurn(pieceObjects, this);
    }

    public bool IsAIMoveLegal(Vector2Int fromPos, Vector2Int ToPos)
    {
        if (GetPieceOnSquare(fromPos) != null)
        {
            if (GetPieceOnSquare(fromPos).PossibleMoves().Contains(ToPos) && turnManager.IsPlayerWhite() != GetPieceOnSquare(fromPos).IsWhite() && GetPieceOnSquare(fromPos).IsMoveLegal(ToPos))
            {
                return true;
            }
            else
            {
                Debug.Log("AI bande judint " + GetPieceOnSquare(fromPos).name + " iš " + fromPos + " į " + ToPos);
                return false;
            }
        }
        else
        {
            Debug.Log("AI bande judint nieka iš " + fromPos + " į " + ToPos);
            return false;

        }
    }

    public void MoveAIPiece(Vector2Int fromPos, Vector2Int ToPos)
    {
        currentPiece = GetPieceOnSquare(fromPos);

        //capture piece if there is one
        Piece maybeEnemyPiece = GetPieceOnSquare(ToPos);
        if (maybeEnemyPiece != null && IsEnemyPiece(currentPiece, maybeEnemyPiece))
        {
            CapturePiece(maybeEnemyPiece);
        }

        //add move to history of moves
        string move = moveHistory.TranslateMoveToUci(fromPos, ToPos);
        moveHistory.AddMove(move);


        specialMoveChecker.CheckSpecialMoves(ToPos.x.ToString() + ToPos.y.ToString() + "AI");
        //checking special moves not in the AI checker :(
        /*/Castling
        int W = (piece.IsWhite() == true) ? 0 : 7;
        if (piece.GetHasMoved() == false && fromPos == new Vector2Int(4, W) && (ToPos == new Vector2Int(1, W) || ToPos == new Vector2Int(6, W)))
        {
            if (ToPos == new Vector2Int(6, W))
            {
                GetPieceOnSquare(new Vector2Int(7, W)).transform.position = new Vector3(5, W, GetPieceOnSquare(new Vector2Int(7, W)).transform.position.z);
                GetPieceOnSquare(new Vector2Int(7, W)).SetCurrentSquare(new Vector2Int(5, W));
            }
            //queenside
            if (ToPos == new Vector2Int(1, W))
            {
                GetPieceOnSquare(new Vector2Int(0, W)).transform.position = new Vector3(2, W, GetPieceOnSquare(new Vector2Int(0, W)).transform.position.z);
                GetPieceOnSquare(new Vector2Int(0, W)).SetCurrentSquare(new Vector2Int(0, W));
            }
        }/*/


        //moving the piece
        currentPiece.SetCurrentSquare(ToPos);
        currentPiece.transform.position = new Vector3(ToPos.x, ToPos.y, currentPiece.transform.position.z);
        ResetHighlights();
        /*/
                if ((piece.GetCurrentSquare().y == 0 || piece.GetCurrentSquare().y == 0) && piece is Pawn)
                {
                    specialMoveChecker.PromotePawn(0);
                }/*/
        //Debug.Log("Will change turns soon");
        turnManager.SwitchTurn(pieceObjects, this);
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
