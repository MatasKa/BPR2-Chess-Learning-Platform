using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override void PossibleMoves()
    {
        base.PossibleMoves();
        Vector2Int[] directions = new Vector2Int[]
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

        foreach (Vector2Int dir in directions)
        {
            Vector2Int newPos = base.currentSquare + dir;
            if (board.IsInsideBoard(newPos))
            {
                Piece temp = null;
                temp = board.GetPieceOnSquare(newPos);
                if (temp == null)
                {
                    board.Highlight(newPos);
                }
                else if (board.IsEnemyPiece(temp) == true)
                {
                    board.Highlight(newPos);
                }
            }
        }
    }
}
