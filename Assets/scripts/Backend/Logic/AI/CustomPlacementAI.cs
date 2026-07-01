using System.Collections.Generic;
using UnityEngine;
using Pylos.Backend.Models;

public class CustomPlacementAI : IAIPlacementAlgorithm
{
    public PylosMove DecidePlacement(BoardModel board, BallColor myColor, int remainingBalls)
    {
        // 1. 配置可能な空スロットをリストアップ
        List<PylosCoordinate> candidates = new List<PylosCoordinate>();
        
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
                        candidates.Add(new PylosCoordinate { Level = l, X = x, Y = y });
                    }
                }
            }
        }

        // 配置できる場所がない場合
        if (candidates.Count == 0)
        {
            return new PylosMove { To = new PylosCoordinate { Level = -1 } };
        }

        // 配置先をランダムに1つ選択
        int idx = Random.Range(0, candidates.Count);
        PylosCoordinate to = candidates[idx];

        // 2. 盤面の移動可能球をリストアップする
        List<PylosCoordinate> myMovableBalls = new List<PylosCoordinate>();
        for (int l = 0; l < 4; l++)
        {
            int size = 4 - l;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (board.BallGrid[l, x, y] == myColor && !IsSupportingOthers(board, l, x, y))
                    {
                        myMovableBalls.Add(new PylosCoordinate { Level = l, X = x, Y = y });
                    }
                }
            }
        }

        // 3. 移動元候補から「移動先より低いレベル」かつ「移動先の土台ではない」球を絞り込む
        List<PylosCoordinate> validSources = new List<PylosCoordinate>();
        foreach (var src in myMovableBalls)
        {
            if (src.Level < to.Level)
            {
                int underL = to.Level - 1;
                bool isBaseOfDest = 
                    (src.Level == underL && src.X == to.X && src.Y == to.Y) ||
                    (src.Level == underL && src.X == to.X + 1 && src.Y == to.Y) ||
                    (src.Level == underL && src.X == to.X && src.Y == to.Y + 1) ||
                    (src.Level == underL && src.X == to.X + 1 && src.Y == to.Y + 1);

                if (!isBaseOfDest)
                {
                    validSources.Add(src);
                }
            }
        }

        // 4. 手元に球がない、またはランダム（50%の確率）で移動を選択する
        if (validSources.Count > 0 && (remainingBalls <= 0 || Random.value < 0.5f))
        {
            int srcIdx = Random.Range(0, validSources.Count);
            return new PylosMove { From = validSources[srcIdx], To = to };
        }

        // 手元から置く
        return new PylosMove { From = null, To = to };
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
