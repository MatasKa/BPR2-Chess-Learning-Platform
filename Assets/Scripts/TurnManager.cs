using UnityEngine;

public class TurnManager
{
    private bool whiteTurn = true;

    public bool GetWhiteTurn()
    {
        return whiteTurn;
    }

    public void SetWhiteTurn(bool value)
    {
        whiteTurn = value;
    }

    public void SwitchTurn(Piece[] pieces, Board board)
    {
        whiteTurn = !whiteTurn;

        foreach (Piece piece in pieces)
        {
            bool shouldEnable = piece.IsWhite() == whiteTurn;
            piece.GetComponent<BoxCollider2D>().enabled = shouldEnable;
        }

        CheckForEndGame(board);
    }

    private void CheckForEndGame(Board board)
    {
        bool currentIsWhite = whiteTurn;

        if (board.HasAnyLegalMoves(currentIsWhite) == false)
        {
            if (board.IsKingInCheck(currentIsWhite) == true)
            {
                Debug.Log("Checkmate! " + (currentIsWhite ? "Black" : "White") + " wins.");
            }
            else
            {
                Debug.Log("Stalemate! It's a draw.");
            }
        }
    }
}