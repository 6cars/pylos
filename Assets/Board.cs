using UnityEngine;

public class Board : MonoBehaviour
{
    // 盤面の状態管理（4段, 4x4）
    // 配列の中身は BallType (None, White, Black)
    private BallType[,,] grid = new BallType[4, 4, 4];

    // 初期化
    public void Init()
    {
        // 全て「None（なし）」で埋める
        for (int z = 0; z < 4; z++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    grid[z, y, x] = BallType.None;
                }
            }
        }
    }

    // 指定された Position にボールを置く
    public void PutBall(Position pos, BallType color)
    {
        if (IsValidPosition(pos))
        {
            grid[pos.z, pos.y, pos.x] = color;
        }
    }

    // 座標が範囲内かチェックする便利関数
    private bool IsValidPosition(Position pos)
    {
        // TODO: ピラミッドの形状に合わせた詳細な範囲チェックが必要
        // 今は簡易的に配列外参照を防ぐ
        if (pos.z < 0 || pos.z >= 4) return false;
        if (pos.y < 0 || pos.y >= 4) return false;
        if (pos.x < 0 || pos.x >= 4) return false;
        return true;
    }
}