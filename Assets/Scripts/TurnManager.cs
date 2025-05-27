using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private bool playerSideWhite;
    [SerializeField] private Timer timer;
    private bool whiteTurn = true;

    void Start()
    {
        timer.OnTimeEnd += OutOfTime;
    }
    public void SwitchTurn(GameObject[] pieces, Board board)
    {
        whiteTurn = !whiteTurn;
        string PlayerPieceColor = (playerSideWhite == true) ? "White" : "Black";
        //Debug.Log("TurnP " + pieces[0].name);
        foreach (GameObject piece in pieces)
        {
            if (playerSideWhite == whiteTurn)
            {
                if (piece.name.Contains(PlayerPieceColor))
                {
                    piece.GetComponent<BoxCollider2D>().enabled = true;
                }
                //else
                //{
                //    piece.GetComponent<BoxCollider2D>().enabled = false; ///for tests
                //}
            }
            else
            {
                if (piece.name.Contains(PlayerPieceColor))
                {
                    piece.GetComponent<BoxCollider2D>().enabled = false;
                }
                //else
                //{
                //    piece.GetComponent<BoxCollider2D>().enabled = true;  ///for tests
                //}
            }
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

    public void StopAllPieces(GameObject[] pieces)
    {
        foreach (GameObject piece in pieces)
        {
            piece.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public bool IsPlayerWhite()
    {
        return playerSideWhite;
    }

    private void OutOfTime()
    {
        uiManager.ShowGameEndUI(4);
    }
    public void CheckForEndGame(Board board)
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
            StopAllPieces(board.GetAllPieceObjects());
        }
    }

}