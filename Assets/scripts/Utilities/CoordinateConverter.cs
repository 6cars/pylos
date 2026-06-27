using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pylos.Backend.Models;

public static class CoordinateConverter
{
    private const float Size = 1.2f;
    private const float HeightFactor = 0.8f; // ピラミッドの沈み込みを考慮した高さ係数

    public static Vector3 ToWorldPosition(PylosCoordinate coord)
    {
        float x = (coord.X + coord.Level * 0.5f) * Size;
        float y = coord.Level * Size * HeightFactor;
        float z = (coord.Y + coord.Level * 0.5f) * Size;
        return new Vector3(x, y, z);
    }

    public static PylosCoordinate ToPylosCoordinate(Vector3 worldPos)
    {
        int level = Mathf.RoundToInt(worldPos.y / (Size * HeightFactor));
        // レベル範囲外のクランプ
        level = Mathf.Clamp(level, 0, 3);

        int x = Mathf.RoundToInt(worldPos.x / Size - level * 0.5f);
        int y = Mathf.RoundToInt(worldPos.z / Size - level * 0.5f);

        // 各レベルのグリッドサイズは 4 - level
        int maxIndex = 3 - level;
        x = Mathf.Clamp(x, 0, maxIndex);
        y = Mathf.Clamp(y, 0, maxIndex);

        return new PylosCoordinate
        {
            Level = level,
            X = x,
            Y = y
        };
    }
}
