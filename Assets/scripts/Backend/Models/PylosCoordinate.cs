using UnityEngine;

[System.Serializable]
public struct PylosCoordinate
{
    public int Level;
    public int X;
    public int Y;

    public PylosCoordinate(int x, int y, int level)
    {
        this.X = x;
        this.Y = y;
        this.Level = level;
    }
}