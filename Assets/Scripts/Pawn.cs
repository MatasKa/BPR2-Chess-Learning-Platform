using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class Pawn : Piece
{

    //Don't forget to check En Passant!!!

    private bool FirstMove = true;

    public override void PossibleMoves()
    {
        base.PossibleMoves();

        int reverse = 1;
        if (white == false)
        {
            reverse = -1;
        }

        Vector2Int[] AttackDir = new Vector2Int[]
        {
            new Vector2Int(-1, +1 * reverse),
            new Vector2Int(+1, +1 * reverse),
        };

        // Passive move checks
        Vector2Int newPos = base.currentSquare + new Vector2Int(0, +1 * reverse);

        if (board.IsInsideBoard(newPos) == true && board.GetPieceOnSquare(newPos) == null)
        {
            board.Highlight(newPos);

            newPos = base.currentSquare + new Vector2Int(0, +2 * reverse);
            if (board.IsInsideBoard(newPos) == true && FirstMove == true && board.GetPieceOnSquare(newPos) == null)
            {
                board.Highlight(newPos);
            }
        }

        // Attacking move checks
        for (int i = 0; i <= 1; i++)
        {
            newPos = base.currentSquare + AttackDir[i];
            if (board.IsInsideBoard(newPos))
            {
                Piece temp = board.GetPieceOnSquare(newPos);
                if (temp != null && board.IsEnemyPiece(this, temp) == true)
                    board.Highlight(newPos);
            }
        }
    }
}
