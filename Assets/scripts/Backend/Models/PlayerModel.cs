using UnityEngine;

public class PlayerModel
{
    // プレイヤーの色
    public PlayerColor Color { get; private set; }

    // 手持ちのボールの数（15個スタート）
    public int BallCount { get; private set; }

    // 【追加】回収権のストック数（オリジナルルール用）
    public int RecoveryRights { get; private set; }

    // コンストラクタ
    public PlayerModel(PlayerColor color)
    {
        Color = color;
        BallCount = 15;
        RecoveryRights = 0; // 最初は0個
    }

    // ------------------------------------
    // ボールの増減（設置・回収）
    // ------------------------------------
    public void DecreaseBall()
    {
        if (BallCount > 0) BallCount--;
    }

    public void IncreaseBall(int amount = 1)
    {
        BallCount += amount;
    }

    // まだボールを持っているか（負け判定用）
    public bool HasBalls() => BallCount > 0;

    // ------------------------------------
    // 【追加】回収権の管理
    // ------------------------------------

    // 権利を獲得する（四角やラインを作った時、または回収フェーズ開始時）
    public void AddRecoveryRight(int amount = 1)
    {
        RecoveryRights += amount;
    }

    // 権利を使う（ボールを回収する時）
    // 成功したらtrue、権利不足ならfalseを返す
    public bool TryUseRecoveryRight()
    {
        if (RecoveryRights > 0)
        {
            RecoveryRights--;
            return true;
        }
        return false;
    }
}