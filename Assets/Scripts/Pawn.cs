using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{

    //Don't forget to check En Passant!!!

    //private bool firstMove = true;

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
        if (board.IsInsideBoard(newPos) == true && board.GetPieceOnSquare(newPos) == null)
        {
            moves.Add(newPos);

            newPos = base.currentSquare + new Vector2Int(0, +2 * reverse);
            int yStartPos = (reverse == 1) ? 1 : 6;
            if (board.GetPieceOnSquare(newPos) == null && currentSquare.y == yStartPos)
            {
                //Debug.Log("piece name: " + gameObject.name + ". Is the piece is white? " + white + ". what is its reverse? " + reverse + " . its Y start position is set as " + yStartPos);
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
            if (board.IsInsideBoard(newPos))
            {
                Piece temp = board.GetPieceOnSquare(newPos);
                if (temp != null && board.IsEnemyPiece(this, temp) == true)
                    moves.Add(newPos);
            }
        }

        return moves;
    }
}
