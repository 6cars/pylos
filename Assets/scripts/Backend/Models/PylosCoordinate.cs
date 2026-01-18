using System;
using UnityEngine;

[Serializable]
public class PylosCoordinate
{
    public int X;
    public int Y;

    //（Zを使う）
    public int Z;

    // （Levelを使う）
    // 「Level」と呼ばれたら、自動的に「Z」の中身を返す！
    public int Level
    {
        get { return Z; }
        set { Z = value; }
    }

    // コンストラクタ
    public PylosCoordinate(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }
}