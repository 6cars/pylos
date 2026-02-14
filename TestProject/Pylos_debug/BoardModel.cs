using UnityEngine;

public class BoardModel
{
    // 盤面データ：3次元配列 [段(z), 横(x), 奥(y)]
    // ピロスは最大4段、広さは最大4x4なので [4, 4, 4] で確保します
    private PlayerColor[,,] _balls = new PlayerColor[4, 4, 4];

    // -------------------------------------------------------
    // 必要なメソッド：HasBall (ボールがあるか？)
    // -------------------------------------------------------
    public bool HasBall(int x, int y, int z)
    {
        // そもそも盤面の範囲外なら「ボールはない」として扱う
        if (!IsInsideBoard(x, y, z)) return false;

        // None じゃなければ「ボールがある」
        return _balls[z, x, y] != PlayerColor.None;
    }

    // -------------------------------------------------------
    // 必要なメソッド：GetColor (そこの色は？)
    // -------------------------------------------------------
    public PlayerColor GetColor(int x, int y, int z)
    {
        // 範囲外なら「なし」を返す
        if (!IsInsideBoard(x, y, z)) return PlayerColor.None;

        return _balls[z, x, y];
    }

    // -------------------------------------------------------
    // （おまけ）ボールを置くメソッド
    // これがないとテストできないと思うので入れておきます
    // -------------------------------------------------------
    public void PlaceBall(int x, int y, int z, PlayerColor color)
    {
        if (IsInsideBoard(x, y, z))
        {
            _balls[z, x, y] = color;
        }
    }

    // -------------------------------------------------------
    // 補助：その座標は盤面の中か？（ピラミッド型チェック）
    // -------------------------------------------------------
    private bool IsInsideBoard(int x, int y, int z)
    {
        // 1. 段(z)のチェック: 0〜3段目まで
        if (z < 0 || z > 3) return false;

        // 2. 広さのチェック: 段が上がるごとに狭くなる
        // 0段目(z=0) -> 4x4 (index 0~3)
        // 1段目(z=1) -> 3x3 (index 0~2)
        // 2段目(z=2) -> 2x2 (index 0~1)
        // 3段目(z=3) -> 1x1 (index 0)

        int limit = 4 - z; // その段の最大サイズ

        // xとyが 0以上 かつ limit未満 ならOK
        return (x >= 0 && x < limit) && (y >= 0 && y < limit);
    }
}