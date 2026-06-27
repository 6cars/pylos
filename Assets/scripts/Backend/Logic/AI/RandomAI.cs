using System.Collections.Generic;
using UnityEngine;
using Pylos.Backend.Models;

public class RandomAI : IAIPlacementAlgorithm, IAIRetrievalAlgorithm
{
    public PylosCoordinate DecidePlacement(BoardModel board, BallColor myColor)
    {
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

        if (candidates.Count > 0)
        {
            int idx = Random.Range(0, candidates.Count);
            return candidates[idx];
        }

        return new PylosCoordinate { Level = -1 };
    }

    public PylosCoordinate DecideRetrieval(BoardModel board, BallColor myColor)
    {
        List<PylosCoordinate> candidates = new List<PylosCoordinate>();

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
                        candidates.Add(new PylosCoordinate { Level = l, X = x, Y = y });
                    }
                }
            }
        }

        // 20%の確率で回収をスキップする
        if (Random.value < 0.2f || candidates.Count == 0)
        {
            return new PylosCoordinate { Level = -1 }; // スキップ
        }

        int idx = Random.Range(0, candidates.Count);
        return candidates[idx];
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
