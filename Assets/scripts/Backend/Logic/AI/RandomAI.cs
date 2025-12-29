using System.Collections.Generic;
using UnityEngine;

// AIが決定した「行動」の中身を入れるクラス
public class AIAction
{
    // アクションの種類（手持ちから置く / 盤上から移動する）
    public enum ActionType { PlaceFromHand, MoveOnBoard }

    public ActionType Type;
    public PylosCoordinate To;   // どこに置くか
    public PylosCoordinate From; // どこから持ってくるか（移動の場合のみ使用）
}

public class RandomAI : IAIAlgorithm
{
    private Rules rules;

    public RandomAI()
    {
        this.rules = new Rules();
    }

    // ----------------------------------------------------
    // 1. 設置フェーズの思考（手持ち配置 or 盤上移動 をランダム選択）
    // ----------------------------------------------------
    public AIAction GetSetupMove(BoardModel board, PlayerModel me)
    {
        // 全ての可能な行動リスト
        List<AIAction> validActions = new List<AIAction>();

        // === A. 手持ちから置くパターンを探す ===
        if (me.BallCount > 0)
        {
            for (int z = 0; z < 4; z++)
            {
                int limit = 4 - z;
                for (int x = 0; x < limit; x++)
                {
                    for (int y = 0; y < limit; y++)
                    {
                        if (rules.CanPlaceAt(board, x, y, z))
                        {
                            validActions.Add(new AIAction
                            {
                                Type = AIAction.ActionType.PlaceFromHand,
                                To = new PylosCoordinate(x, y, z),
                                From = null
                            });
                        }
                    }
                }
            }
        }

        // === B. 盤上のボールを移動させるパターンを探す ===
        // 条件：
        // 1. 自分のボールで、動かせる状態（CanRemoveBall）であること
        // 2. 移動先が空いていて、置ける状態（CanPlaceAt）であること
        // 3. ピロスのルール：移動先は元の位置より高い段でなければならない（段上げ）

        // まず「動かせる自分のボール（移動元）」を全部探す
        List<PylosCoordinate> movableBalls = new List<PylosCoordinate>();
        for (int z = 0; z < 3; z++) // 頂上(3段目)からは上に行けないので探さなくてOK
        {
            int limit = 4 - z;
            for (int x = 0; x < limit; x++)
            {
                for (int y = 0; y < limit; y++)
                {
                    if (board.HasBall(x, y, z) &&
                        board.GetColor(x, y, z) == me.Color &&
                        rules.CanRemoveBall(board, x, y, z))
                    {
                        movableBalls.Add(new PylosCoordinate(x, y, z));
                    }
                }
            }
        }

        // 次に「移動先」を探して組み合わせる
        foreach (var fromPos in movableBalls)
        {
            // 移動先は、移動元より上の段 (fromPos.Z + 1 以上)
            for (int z = fromPos.Z + 1; z < 4; z++)
            {
                int limit = 4 - z;
                for (int x = 0; x < limit; x++)
                {
                    for (int y = 0; y < limit; y++)
                    {
                        // 移動先が置ける場所なら、アクション候補に追加
                        if (rules.CanPlaceAt(board, x, y, z))
                        {
                            validActions.Add(new AIAction
                            {
                                Type = AIAction.ActionType.MoveOnBoard,
                                To = new PylosCoordinate(x, y, z),
                                From = fromPos
                            });
                        }
                    }
                }
            }
        }

        // === 決定 ===
        // 何もできることがなければ null
        if (validActions.Count == 0) return null;

        // 全候補（手持ち配置 ＋ 移動）の中からランダムに1つ選ぶ
        int randomIndex = Random.Range(0, validActions.Count);
        return validActions[randomIndex];
    }

    // ----------------------------------------------------
    // 2. 回収フェーズの思考（回収する or しない をランダム選択）
    // ----------------------------------------------------
    public PylosCoordinate GetRecoveryMove(BoardModel board, PlayerModel me)
    {
        // 回収できるボールのリスト
        List<PylosCoordinate> removableBalls = new List<PylosCoordinate>();

        for (int z = 0; z < 4; z++)
        {
            int limit = 4 - z;
            for (int x = 0; x < limit; x++)
            {
                for (int y = 0; y < limit; y++)
                {
                    if (board.HasBall(x, y, z) &&
                        board.GetColor(x, y, z) == me.Color &&
                        rules.CanRemoveBall(board, x, y, z))
                    {
                        removableBalls.Add(new PylosCoordinate(x, y, z));
                    }
                }
            }
        }

        // リストに「null」を追加することで、「回収しない（パス）」を選択肢に含める
        // ※ただし、ボールが一つもない時はパスしかできないので追加不要（自動でnullが返る）
        if (removableBalls.Count > 0)
        {
            // 例えば「選択肢の数 + 1」の範囲でランダムにする
            // countが3なら、0,1,2,3 の乱数を出す。3が出たら「パス」とする
            int randomIndex = Random.Range(0, removableBalls.Count + 1);

            // 最後のインデックスを引いたら「パス（回収しない）」とする
            if (randomIndex == removableBalls.Count)
            {
                return null; // 回収しない
            }

            return removableBalls[randomIndex];
        }

        return null; // 回収できるものがない
    }
}