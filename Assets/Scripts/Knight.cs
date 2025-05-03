using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override void PossibleMoves()
    {
        base.PossibleMoves();
        Vector2Int[] moves = new Vector2Int[]
        {
        new Vector2Int(+2, +1),
        new Vector2Int(+2, -1),
        new Vector2Int(-2, +1),
        new Vector2Int(-2, -1),
        new Vector2Int(+1, +2),
        new Vector2Int(+1, -2),
        new Vector2Int(-1, +2),
        new Vector2Int(-1, -2)
        };

        foreach (Vector2Int move in moves)
        {
            Vector2Int newPos = base.currentSquare + move;
            if (board.IsInsideBoard(newPos))
            {
                Piece temp = null;
                temp = board.GetPieceOnSquare(newPos);
                if (temp == null)
                {
                    board.Highlight(newPos);
                }
                else if (board.IsEnemyPiece(this, temp) == true)
                {
                    board.Highlight(newPos);
                }
            }
        }
    }
}
