using System.Collections;
using System.Collections.Generic;
using Pylos.Backend.Models;

namespace Pylos.Backend.Logic
{
    public static class Rules
    {
        // =======================================================
        // 1. 配置可能判定 (CanPlaceAt)
        // =======================================================
        public static bool CanPlaceAt(BoardModel board, PylosCoordinate coord)
        {
            int x = coord.X;
            int y = coord.Y;
            int level = coord.Level;

            // そもそも盤面の範囲外なら置けない
            int limit = 4 - level;
            if (x < 0 || x >= limit || y < 0 || y >= limit || level < 0 || level > 3)
            {
                return false;
            }

            // すでにボールが置いてあれば置けない
            if (board.HasBall(x, y, level)) return false;

            // 0段目なら土台なしで置ける
            if (level == 0) return true;

            // 1段目以上なら、1つ下の段（level - 1）の 2x2（4マス分）にボールがある必要がある
            bool support1 = board.HasBall(x, y, level - 1);
            bool support2 = board.HasBall(x + 1, y, level - 1);
            bool support3 = board.HasBall(x, y + 1, level - 1);
            bool support4 = board.HasBall(x + 1, y + 1, level - 1);

            return support1 && support2 && support3 && support4;
        }

        // =======================================================
        // 2. 除去可能判定 (CanRemoveAt)
        // =======================================================
        public static bool CanRemoveAt(BoardModel board, PylosCoordinate coord)
        {
            int x = coord.X;
            int y = coord.Y;
            int level = coord.Level;

            // そこにボールがないなら除去できない
            if (!board.HasBall(x, y, level)) return false;

            // 最上段（level 3）なら上に何も乗らないので常に除去可能
            if (level == 3) return true;

            // 自分が土台になっている可能性のある上の段（level + 1）の4箇所を直接チェック
            int upperZ = level + 1;
            if (board.HasBall(x - 1, y - 1, upperZ)) return false;
            if (board.HasBall(x,     y - 1, upperZ)) return false;
            if (board.HasBall(x - 1, y,     upperZ)) return false;
            if (board.HasBall(x,     y,     upperZ)) return false;

            return true;
        }

        // =======================================================
        // 3. 2x2 正方形の完成判定 (CheckSquareFormation)
        // =======================================================
        public static bool CheckSquareFormation(BoardModel board, PylosCoordinate coord)
        {
            int x = coord.X;
            int y = coord.Y;
            int level = coord.Level;

            BallColor myColor = board.GetColor(x, y, level);
            if (myColor == BallColor.None) return false;

            int limit = 4 - level;

            // 置いた点を含む可能性がある2x2の左上候補（最大4通り）を調べる
            for (int dx = -1; dx <= 0; dx++)
            {
                for (int dy = -1; dy <= 0; dy++)
                {
                    int sx = x + dx;
                    int sy = y + dy;

                    // 範囲外は除外
                    if (sx < 0 || sy < 0 || sx + 1 >= limit || sy + 1 >= limit) continue;

                    if (board.GetColor(sx, sy, level) == myColor &&
                        board.GetColor(sx + 1, sy, level) == myColor &&
                        board.GetColor(sx, sy + 1, level) == myColor &&
                        board.GetColor(sx + 1, sy + 1, level) == myColor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // =======================================================
        // 4. 一列（ライン）の完成判定 (CheckLineFormation)
        // =======================================================
        public static bool CheckLineFormation(BoardModel board, PylosCoordinate coord)
        {
            int placedX = coord.X;
            int placedY = coord.Y;
            int level = coord.Level;

            // 3段目 (最上段) は 1x1 なのでライン判定から除外する (4段目はラインを作れない)
            if (level >= 3) return false;

            BallColor myColor = board.GetColor(placedX, placedY, level);
            if (myColor == BallColor.None) return false;

            int limit = 4 - level;

            // 横一列のチェック
            bool rowOk = true;
            for (int x = 0; x < limit; x++)
            {
                if (board.GetColor(x, placedY, level) != myColor)
                {
                    rowOk = false;
                    break;
                }
            }
            if (rowOk) return true;

            // 縦一列のチェック
            bool colOk = true;
            for (int y = 0; y < limit; y++)
            {
                if (board.GetColor(placedX, y, level) != myColor)
                {
                    colOk = false;
                    break;
                }
            }
            if (colOk) return true;

            return false;
        }

        // =======================================================
        // 5. ゲーム終了判定 (IsGameOver)
        // =======================================================
        public static bool IsGameOver(BoardModel board)
        {
            // 最上段にボールが置かれたらゲーム終了
            return board.HasBall(0, 0, 3);
        }

        // =======================================================
        // 6. ボールの移動判定 (CanMoveBall)
        // =======================================================
        public static bool CanMoveBall(BoardModel board, PylosCoordinate from, PylosCoordinate to, BallColor myColor)
        {
            // 1. 動かそうとしているボールが自分の色か？
            if (board.GetColor(from) != myColor) return false;

            // 2. そのボールは上に何も乗っていなくて、動かせる状態（回収可能）か？
            if (!CanRemoveAt(board, from)) return false;

            // 3. 移動先は「今より高い段」か？（ピロスでは同じ段や下には移動できない）
            if (to.Level <= from.Level) return false;

            // 4. 【重要】自分が自分の土台になるような移動は物理的に不可
            // from(移動元)が、to(移動先)を直接支える4つのボールのどれかである場合はNG
            if (to.Level - from.Level == 1)
            {
                if ((from.X == to.X || from.X == to.X + 1) &&
                    (from.Y == to.Y || from.Y == to.Y + 1))
                {
                    return false; // 自分が支えになっている場所には上がれない
                }
            }

            // 5. 通常の「置けるか判定」で土台が揃っているか確認
            return CanPlaceAt(board, to);
        }
    }
}