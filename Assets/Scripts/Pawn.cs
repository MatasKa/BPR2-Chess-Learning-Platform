using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class Pawn : Piece
{
    Vector2Int[] AttackDir = new Vector2Int[]
{
        new Vector2Int(-1, +1),
        new Vector2Int(+1, +1),
    };
    private bool FirstMove = true;

    public override void PossibleMoves()
    {
        base.PossibleMoves();

        // Passive move checks
        Vector2Int newPos = base.currentSquare + new Vector2Int(0, +1);

        if (board.IsInsideBoard(newPos) == true && board.IsSquareFree(newPos) == null)
        {
            board.Highlight(newPos);

            newPos = base.currentSquare + new Vector2Int(0, +2);
            if (board.IsInsideBoard(newPos) == true && FirstMove == true && board.IsSquareFree(newPos) == null)
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
                Piece temp = board.IsSquareFree(newPos);
                if (temp != null && board.IsEnemyPiece(temp) == true)
                    board.Highlight(newPos);
            }
        }
    }
}
