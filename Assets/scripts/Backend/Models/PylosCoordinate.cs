// 座標（X, Y, Z）をセットで扱うためのクラス
[System.Serializable]
public class PylosCoordinate
{
    public int X;
    public int Y;
    public int Z;

    // コンストラクタ（作るときに数字を入れる）
    public PylosCoordinate(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }
}