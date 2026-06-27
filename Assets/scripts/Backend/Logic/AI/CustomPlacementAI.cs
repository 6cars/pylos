using System.Collections.Generic;
using UnityEngine;
using Pylos.Backend.Models;

public class CustomPlacementAI : IAIPlacementAlgorithm
{
    public PylosCoordinate DecidePlacement(BoardModel board, BallColor myColor)
    {
        // TODO: 後輩がここに設置アルゴリズムを記述します。
        // （デフォルトでは、最初に見つかった配置可能スロットを選択します）
        
        for (int l = 0; l < 4; l++)
        {
            int size = 4 - l;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (board.BallGrid[l, x, y] != BallColor.None) continue;

                    bool canPlace = false;
                    if (l == 0)
                    {
                        canPlace = true;
                    }
                    else
                    {
                        int underL = l - 1;
                        canPlace = 
                            board.BallGrid[underL, x, y] != BallColor.None &&
                            board.BallGrid[underL, x + 1, y] != BallColor.None &&
                            board.BallGrid[underL, x, y + 1] != BallColor.None &&
                            board.BallGrid[underL, x + 1, y + 1] != BallColor.None;
                    }

                    if (canPlace)
                    {
                        return new PylosCoordinate { Level = l, X = x, Y = y };
                    }
                }
            }
        }
        return new PylosCoordinate { Level = -1 };
    }
}
