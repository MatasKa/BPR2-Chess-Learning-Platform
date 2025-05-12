using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
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

    //Disables all colliders, so that pieces can't be selected (used for pawn promotion)
    public void StopAllPieces(Piece[] pieces)
    {
        foreach (Piece piece in pieces)
        {
            piece.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void CheckForEndGame(Board board)
    {
        if (board.HasAnyLegalMoves(whiteTurn) == false)
        {
            if (board.IsKingInCheck(whiteTurn) == true)
            {
                int win = whiteTurn ? 2 : 1;
                uiManager.ShowGameEndUI(win);
                //Debug.Log("sending 1 to UI manager");
            }
            else
            {
                uiManager.ShowGameEndUI(3);
                //Debug.Log("sending 3 to UI manager");
            }
        }
    }
}