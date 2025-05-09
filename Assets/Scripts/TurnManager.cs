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

    public void SwitchTurn(Piece[] pieces)
    {
        whiteTurn = !whiteTurn;

        foreach (Piece piece in pieces)
        {
            bool shouldEnable = piece.IsWhite() == whiteTurn;
            piece.GetComponent<BoxCollider2D>().enabled = shouldEnable;
        }
    }
}
