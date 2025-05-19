using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    public override List<Vector2Int> PossibleMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int[] directions = new Vector2Int[]
            {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 1)
    };

        for (int i = 0; i < 8; i++)
        {
            for (int o = 1; o <= 8; o++)
            {
                Vector2Int newPos = base.currentSquare + directions[i] * o;
                if (CanMoveToSquare(newPos))
                {
                    moves.Add(newPos);
                }
                else if (CanCapture(this, newPos))
                {
                    moves.Add(newPos);
                    break;
                }
                else
                {
                    break;
                }
            }
        }

        return moves;
    }
}
