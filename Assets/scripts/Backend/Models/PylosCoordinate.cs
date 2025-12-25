using System;

public struct PylosCoordinate : IEquatable<PylosCoordinate>
{
    public int Level; // 0:最下段(4x4), 1:3x3, 2:2x2, 3:頂点
    public int X;
    public int Y;

    public PylosCoordinate(int level, int x, int y)
    {
        Level = level;
        X = x;
        Y = y;
    }

    // 比較用（DictionaryのKeyにする場合などに必要）
    public bool Equals(PylosCoordinate other)
    {
        return Level == other.Level && X == other.X && Y == other.Y;
    }

    public override bool Equals(object obj)
    {
        return obj is PylosCoordinate other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Level, X, Y);
    }
}