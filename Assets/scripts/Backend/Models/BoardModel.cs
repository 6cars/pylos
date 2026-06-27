using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pylos.Backend.Models
{
    public class BoardModel
    {
        // 碁の配置データ（全レベル）
        public BallColor[,,] BallGrid { get; private set; }

        public BoardModel()
        {
            // 各レベルの最大サイズで確保（インデックスは0から）
            BallGrid = new BallColor[4, 4, 4];

            for (int l = 0; l < 4; l++)
                for (int x = 0; x < 4; x++)
                    for (int y = 0; y < 4; y++)
                        BallGrid[l, x, y] = BallColor.None;
        }

        public void PlaceBall(PylosCoordinate coord, BallColor color)
        {
            if (IsInsideBoard(coord))
            {
                BallGrid[coord.Level, coord.X, coord.Y] = color;
            }
        }

        public void RemoveBall(PylosCoordinate coord)
        {
            if (IsInsideBoard(coord))
            {
                BallGrid[coord.Level, coord.X, coord.Y] = BallColor.None;
            }
        }

        // --- 盤面外アクセス防止用のセーフティヘルパーメソッド ---
        
        public bool IsInsideBoard(int x, int y, int level)
        {
            if (level < 0 || level > 3) return false;
            int limit = 4 - level;
            return (x >= 0 && x < limit) && (y >= 0 && y < limit);
        }

        public bool IsInsideBoard(PylosCoordinate coord)
        {
            return IsInsideBoard(coord.X, coord.Y, coord.Level);
        }

        public BallColor GetColor(int x, int y, int level)
        {
            if (!IsInsideBoard(x, y, level)) return BallColor.None;
            return BallGrid[level, x, y];
        }

        public BallColor GetColor(PylosCoordinate coord)
        {
            return GetColor(coord.X, coord.Y, coord.Level);
        }

        public bool HasBall(int x, int y, int level)
        {
            return GetColor(x, y, level) != BallColor.None;
        }

        public bool HasBall(PylosCoordinate coord)
        {
            return HasBall(coord.X, coord.Y, coord.Level);
        }
    }
}
