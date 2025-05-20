using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{

    //enPassantTarget - can be taken by en passant
    private bool enPassantTarget = false;

    public override List<Vector2Int> PossibleMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        int reverse = 1;
        if (white == false)
        {
            reverse = -1;
        }

        // Passive move checks
        Vector2Int newPos = base.currentSquare + new Vector2Int(0, +1 * reverse);
        if (CanMoveToSquare(newPos))
        {
            moves.Add(newPos);

            newPos = base.currentSquare + new Vector2Int(0, +2 * reverse);
            int yStartPos = (reverse == 1) ? 1 : 6;
            if (CanMoveToSquare(newPos) && currentSquare.y == yStartPos)
            {
                moves.Add(newPos);
            }
        }

        // Attacking move checks
        Vector2Int[] AttackDir = new Vector2Int[]
        {
            new Vector2Int(-1, +1 * reverse),
            new Vector2Int(+1, +1 * reverse),
        };

        for (int i = 0; i <= 1; i++)
        {
            newPos = base.currentSquare + AttackDir[i];
            if (CanCapture(this, newPos))
            {
                moves.Add(newPos);
            }
        }

        Piece enPassantPawn = specialMoveChecker.GetEnPassantTarget();

        if (enPassantPawn != null && enPassantPawn.GetCurrentSquare().y == currentSquare.y)
        {
            if (enPassantPawn.GetCurrentSquare().x - currentSquare.x == 1 || enPassantPawn.GetCurrentSquare().x - currentSquare.x == -1)
            {
                Vector2Int EnPassantPos = new Vector2Int(enPassantPawn.GetCurrentSquare().x, currentSquare.y + (IsWhite() ? 1 : -1));
                moves.Add(EnPassantPos);
            }
        }
        return moves;
    }

    public List<Vector2Int> PossibleAttacks()
    {
        List<Vector2Int> attacks = new List<Vector2Int>();

        int reverse = 1;
        if (white == false)
        {
            reverse = -1;
        }

        attacks.Add(base.currentSquare + new Vector2Int(-1, +1 * reverse));
        attacks.Add(base.currentSquare + new Vector2Int(+1, +1 * reverse));

        return attacks;
    }

}
