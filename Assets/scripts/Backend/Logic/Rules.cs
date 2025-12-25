using UnityEngine;

public class Rules
{
    // -------------------------------------------------------
    // 1. 物理法則の実装 (CanPlaceAt)
    // -------------------------------------------------------
    public bool CanPlaceAt(BoardModel board, int x, int y, int z)
    {
        // すでにボールがあるならNG
        if (board.HasBall(x, y, z)) return false;

        // 1段目 (z=0) は土台不要なのでOK
        if (z == 0) return true;

        // 2段目以降は、下の段 (z-1) の4箇所にボールが必要
        bool support1 = board.HasBall(x, y, z - 1);
        bool support2 = board.HasBall(x + 1, y, z - 1);
        bool support3 = board.HasBall(x, y + 1, z - 1);
        bool support4 = board.HasBall(x + 1, y + 1, z - 1);

        return support1 && support2 && support3 && support4;
    }

    // -------------------------------------------------------
    // 2. 回収判定：正方形 ＋ オリジナル(縦横ライン)
    // -------------------------------------------------------
    public bool CheckRecovery(BoardModel board, int x, int y, int z, PlayerColor myColor)
    {
        // A. 正方形判定（公式ルール）
        if (CheckSquareFormation(board, x, y, z, myColor)) return true;

        // B. 縦横ライン判定（オリジナルルール：1段目のみ）
        if (z == 0 && CheckLineFormation(board, x, y, myColor)) return true;

        return false;
    }

    // --- 内部ロジック：正方形判定 ---
    private bool CheckSquareFormation(BoardModel board, int placedX, int placedY, int placedZ, PlayerColor myColor)
    {
        var offsets = new (int dx, int dy)[] { (0, 0), (-1, 0), (0, -1), (-1, -1) };

        foreach (var (dx, dy) in offsets)
        {
            int cx = placedX + dx;
            int cy = placedY + dy;

            // 4つ全てが自分の色ならOK
            if (board.GetColor(cx, cy, placedZ) == myColor &&
                board.GetColor(cx + 1, cy, placedZ) == myColor &&
                board.GetColor(cx, cy + 1, placedZ) == myColor &&
                board.GetColor(cx + 1, cy + 1, placedZ) == myColor)
            {
                return true;
            }
        }
        return false;
    }

    // --- 内部ロジック：縦横ライン判定 ---
    private bool CheckLineFormation(BoardModel board, int placedX, int placedY, PlayerColor myColor)
    {
        // 横 (Row) チェック
        bool rowOk = true;
        for (int x = 0; x < 4; x++) if (board.GetColor(x, placedY, 0) != myColor) rowOk = false;
        if (rowOk) return true;

        // 縦 (Col) チェック
        bool colOk = true;
        for (int y = 0; y < 4; y++) if (board.GetColor(placedX, y, 0) != myColor) colOk = false;
        if (colOk) return true;

        return false;
    }

    // -------------------------------------------------------
    // 3. 勝利判定
    // -------------------------------------------------------
    /// <summary>
    /// 勝敗が決まったか判定（頂上に置いたら勝ち）
    /// </summary>
    public bool CheckWin(int z)
    {
        // ピロスの頂上は「3段目 (z=3)」です
        // 3段目に置けた＝そのプレイヤーの勝利！
        if (z == 3)
        {
            return true;
        }
        return false;
    }
}