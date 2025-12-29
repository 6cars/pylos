using UnityEngine;

public class Rules
{
    // -------------------------------------------------------
    // 1. 置けるか判定 (CanPlaceAt)
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
    // 2. 動かせるか/回収できるか判定 (CanRemoveBall)
    // -------------------------------------------------------
    public bool CanRemoveBall(BoardModel board, int x, int y, int z)
    {
        if (!board.HasBall(x, y, z)) return false;
        if (z == 3) return true; // 頂上は絶対OK

        // 自分より上の段 (z+1) にあるボールから支えられていないかチェック
        var offsets = new (int dx, int dy)[]
        {
            (-1, -1), (0, -1), (-1, 0), (0, 0)
        };

        foreach (var (dx, dy) in offsets)
        {
            int upperX = x + dx;
            int upperY = y + dy;
            int upperZ = z + 1;

            if (board.HasBall(upperX, upperY, upperZ))
            {
                return false;
            }
        }
        return true;
    }

    // -------------------------------------------------------
    // 3. 回収権発生判定 (CheckRecovery)
    // -------------------------------------------------------
    public bool CheckRecovery(BoardModel board, int x, int y, int z, PlayerColor myColor)
    {
        // A. 正方形判定（公式ルール）
        if (CheckSquareFormation(board, x, y, z, myColor)) return true;

        // B. 縦横ライン判定（オリジナルルール：全段有効）
        // ※z=0なら4つ、z=1なら3つ、z=2なら2つ揃えばOKとします
        if (CheckLineFormation(board, x, y, z, myColor)) return true;

        return false;
    }

    // -------------------------------------------------------
    // 4. 勝利判定
    // -------------------------------------------------------
    public bool CheckWin(int z)
    {
        return z == 3;
    }

    // =======================================================
    // 内部ロジック (Private)
    // =======================================================

    // 正方形判定
    private bool CheckSquareFormation(BoardModel board, int placedX, int placedY, int placedZ, PlayerColor myColor)
    {
        var offsets = new (int dx, int dy)[] { (0, 0), (-1, 0), (0, -1), (-1, -1) };

        foreach (var (dx, dy) in offsets)
        {
            int cx = placedX + dx;
            int cy = placedY + dy;

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

    // 縦横ライン判定 (全段対応)
    private bool CheckLineFormation(BoardModel board, int placedX, int placedY, int z, PlayerColor myColor)
    {
        // その段の1辺の長さ (z=0->4, z=1->3, z=2->2)
        int limit = 4 - z;

        // 1. 横方向 (Row) チェック
        // 今置いた行 (placedY) の端から端まで全て自分の色か？
        bool rowOk = true;
        for (int x = 0; x < limit; x++)
        {
            if (board.GetColor(x, placedY, z) != myColor)
            {
                rowOk = false;
                break;
            }
        }
        if (rowOk) return true;

        // 2. 縦方向 (Col) チェック
        // 今置いた列 (placedX) の端から端まで全て自分の色か？
        bool colOk = true;
        for (int y = 0; y < limit; y++)
        {
            if (board.GetColor(placedX, y, z) != myColor)
            {
                colOk = false;
                break;
            }
        }
        if (colOk) return true;

        return false;
    }
}