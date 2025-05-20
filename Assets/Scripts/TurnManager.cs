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
    public void SwitchTurn(GameObject[] pieces, Board board)
    {
        whiteTurn = !whiteTurn;
        //Debug.Log("TurnP " + pieces[0].name);
        foreach (GameObject piece in pieces)
        {
            //Debug.Log("TurnP " + piece.name);
            bool shouldEnable = piece.name.Contains("White") == whiteTurn;
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
        //Debug.Log("turns changed");
        CheckForEndGame(board);
    }

    public void StopAllPieces(GameObject[] pieces)
    {
        foreach (GameObject piece in pieces)
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
            StopAllPieces(board.GetAllPieceObjects());
        }
    }

}