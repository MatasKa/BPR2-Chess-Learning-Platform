using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private bool playerSideWhite = true;
    [SerializeField] private Timer timer;
    private bool whiteTurn = true;

    void Start()
    {
        timer.OnTimeEnd += OutOfTime;
    }
    public void SwitchTurn(Piece[] pieces, Board board)
    {
        whiteTurn = !whiteTurn;

        foreach (Piece piece in pieces)
        {
            bool shouldEnable = piece.IsWhite() == whiteTurn;
            piece.GetComponent<BoxCollider2D>().enabled = shouldEnable;
        }

        if (playerSideWhite == whiteTurn)
        {
            timer.StartTimer();
        }
        else
        {
            timer.StopTimer();
        }

        CheckForEndGame(board);
    }

    public void StopAllPieces(Piece[] pieces)
    {
        foreach (Piece piece in pieces)
        {
            piece.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void OutOfTime()
    {
        uiManager.ShowGameEndUI(4);
    }
    private void CheckForEndGame(Board board)
    {
        if (board.HasAnyLegalMoves(whiteTurn) == false)
        {
            if (board.IsKingInCheck(whiteTurn) == true)
            {
                int win = whiteTurn ? 2 : 1;
                uiManager.ShowGameEndUI(win);
            }
            else
            {
                uiManager.ShowGameEndUI(3);
            }
            StopAllPieces(board.GetPieces());
        }
    }

}