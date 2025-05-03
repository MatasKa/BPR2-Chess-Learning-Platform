using UnityEngine;

public class Bishop : Piece
{
    public override void PossibleMoves()
    {

        base.PossibleMoves();

        Vector2Int newPos = base.currentSquare;
        Vector2Int move = new Vector2Int(0, 0);

        Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 1)
    };

        for (int i = 0; i < 4; i++)
        {
            for (int o = 1; o <= 8; o++)
            {
                newPos = base.currentSquare + directions[i] * o;
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
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
