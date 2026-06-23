using System.Collections.Generic;
using UnityEngine;
using Pylos.Backend.Models;

public class CustomRetrievalAI : IAIRetrievalAlgorithm
{
    public PylosCoordinate DecideRetrieval(BoardModel board, BallColor myColor)
    {
        // TODO: 後輩がここに回収アルゴリズムを記述します。
        // （デフォルトでは、最初に見つかった回収可能球を選択して回収します。10%の確率でスキップします）
        
        if (Random.value < 0.1f)
        {
            return new PylosCoordinate { Level = -1 }; // スキップ
        }

        for (int l = 0; l < 4; l++)
        {
            int size = 4 - l;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (board.BallGrid[l, x, y] != myColor) continue;
                    
                    if (!IsSupportingOthers(board, l, x, y))
                    {
                        return new PylosCoordinate { Level = l, X = x, Y = y };
                    }
                }
            }
        }
        return new PylosCoordinate { Level = -1 };
    }

    private bool IsSupportingOthers(BoardModel board, int l, int x, int y)
    {
        int upL = l + 1;
        if (upL >= 4) return false;

        int upSize = 4 - upL;
        int[] dx = { 0, -1, 0, -1 };
        int[] dy = { 0, 0, -1, -1 };

        for (int i = 0; i < 4; i++)
        {
            int upX = x + dx[i];
            int upY = y + dy[i];

            if (upX >= 0 && upX < upSize && upY >= 0 && upY < upSize)
            {
                if (board.BallGrid[upL, upX, upY] != BallColor.None)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
