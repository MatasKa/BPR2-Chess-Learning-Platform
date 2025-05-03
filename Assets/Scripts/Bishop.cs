using UnityEngine;

public class Bishop : Piece
{
    public override void PossibleMoves()
    {

        base.PossibleMoves();

        Vector2Int newPos = base.currentSquare;
        Vector2Int dir = new Vector2Int(0, 0);
        for (int i = 0; i < 4; i++)
        {
            for (int o = 1; o <= 8; o++)
            {

                //Feels like there could be a better way
                if (i == 0)
                {
                    dir = new Vector2Int(o, o);
                }
                else if (i == 1)
                {
                    dir = new Vector2Int(o, -o);
                }
                else if (i == 2)
                {
                    dir = new Vector2Int(-o, -o);
                }
                else
                {
                    dir = new Vector2Int(-o, o);
                }

                newPos = base.currentSquare + dir;
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
