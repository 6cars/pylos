using UnityEngine;

public class Rules
{
    // =======================================================
    // 1. 置けるか判定 (CanPlaceAt)
    // =======================================================

    // ★六車くん(PhaseManager)用：数字4つで呼ばれてもOKにする
    public bool CanPlaceAt(BoardModel board, int x, int y, int z)
    {
        return CanPlaceAt(board, new PylosCoordinate(x, y, z));
    }

    // ★Notion設計用：座標クラスで処理する
    public bool CanPlaceAt(BoardModel board, PylosCoordinate coord)
    {
        int x = coord.X;
        int y = coord.Y;
        int z = coord.Level; // 手順1を直せば、ここの赤線が消えます！

        if (board.HasBall(x, y, z)) return false;
        if (z == 0) return true;

        bool support1 = board.HasBall(x, y, z - 1);
        bool support2 = board.HasBall(x + 1, y, z - 1);
        bool support3 = board.HasBall(x, y + 1, z - 1);
        bool support4 = board.HasBall(x + 1, y + 1, z - 1);

        return support1 && support2 && support3 && support4;
    }

    // =======================================================
    // 2. 回収可能か判定 (CanRemoveAt / CanRemoveBall)
    // =======================================================

    // ★六車くん用
    public bool CanRemoveBall(BoardModel board, int x, int y, int z)
    {
        return CanRemoveAt(board, new PylosCoordinate(x, y, z));
    }

    // ★Notion設計用
    public bool CanRemoveAt(BoardModel board, PylosCoordinate coord)
    {
        int x = coord.X;
        int y = coord.Y;
        int z = coord.Level;

        if (!board.HasBall(x, y, z)) return false;
        if (z == 3) return true;

        var offsets = new (int dx, int dy)[] { (-1, -1), (0, -1), (-1, 0), (0, 0) };
        foreach (var (dx, dy) in offsets)
        {
            int upperX = x + dx;
            int upperY = y + dy;
            int upperZ = z + 1;
            if (board.HasBall(upperX, upperY, upperZ)) return false;
        }
        return true;
    }

    // =======================================================
    // 3. 回収権発生判定 (CheckRecovery)
    // =======================================================

    // ★六車くん用 (引数が多いやつに対応)
    public bool CheckRecovery(BoardModel board, int x, int y, int z, PlayerColor color)
    {
        return CheckRecovery(board, new PylosCoordinate(x, y, z), color);
    }

    // ★Notion設計用
    public bool CheckRecovery(BoardModel board, PylosCoordinate coord, PlayerColor myColor)
    {
        if (CheckSquareFormation(board, coord)) return true;
        if (CheckLineFormation(board, coord)) return true;
        return false;
    }

    // =======================================================
    // 4. ゲーム終了判定 (IsGameOver / CheckWin)
    // =======================================================

    // ★Notion設計用
    public bool IsGameOver(BoardModel board)
    {
        return board.HasBall(0, 0, 3);
    }

    // ★六車くん(PhaseManager)用：CheckWinという名前で呼ばれても対応する
    public bool CheckWin(BoardModel board)
    {
        return IsGameOver(board);
    }

    // オーバーロード（引数が違う場合用）
    public bool CheckWin(int z)
    {
        return z == 3;
    }

    // =======================================================
    // 内部ロジック (Public)
    // =======================================================
    public bool CheckSquareFormation(BoardModel board, PylosCoordinate coord)
    {
        int placedX = coord.X;
        int placedY = coord.Y;
        int placedZ = coord.Level;
        PlayerColor myColor = board.GetColor(placedX, placedY, placedZ);
        if (myColor == PlayerColor.None) return false;

        var offsets = new (int dx, int dy)[] { (0, 0), (-1, 0), (0, -1), (-1, -1) };
        foreach (var (dx, dy) in offsets)
        {
            int cx = placedX + dx;
            int cy = placedY + dy;
            if (board.GetColor(cx, cy, placedZ) == myColor &&
                board.GetColor(cx + 1, cy, placedZ) == myColor &&
                board.GetColor(cx, cy + 1, placedZ) == myColor &&
                board.GetColor(cx + 1, cy + 1, placedZ) == myColor) return true;
        }
        return false;
    }

    public bool CheckLineFormation(BoardModel board, PylosCoordinate coord)
    {
        int placedX = coord.X;
        int placedY = coord.Y;
        int z = coord.Level;
        PlayerColor myColor = board.GetColor(placedX, placedY, z);
        if (myColor == PlayerColor.None) return false;

        int limit = 4 - z;
        bool rowOk = true;
        for (int x = 0; x < limit; x++) if (board.GetColor(x, placedY, z) != myColor) { rowOk = false; break; }
        if (rowOk) return true;

        bool colOk = true;
        for (int y = 0; y < limit; y++) if (board.GetColor(placedX, y, z) != myColor) { colOk = false; break; }
        if (colOk) return true;

        return false;
    }
}