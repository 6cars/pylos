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
            BallGrid[coord.Level, coord.X, coord.Y] = color;
        }

        public void RemoveBall(PylosCoordinate coord)
        {
            BallGrid[coord.Level, coord.X, coord.Y] = BallColor.None;
        }
    }
}
