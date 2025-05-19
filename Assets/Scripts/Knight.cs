using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override List<Vector2Int> PossibleMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
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
            Vector2Int newPos = currentSquare + dir;
            if (CanMoveToSquare(newPos, this) || CanCapture(this, newPos))
            {
                moves.Add(newPos);
            }
        }

        return moves;
    }
}
