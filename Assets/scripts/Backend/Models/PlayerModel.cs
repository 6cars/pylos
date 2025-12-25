public class PlayerModel
{
    // プレイヤーの色（白か黒か）
    public PlayerColor Color { get; private set; }

    // 手持ちのボールの数（ピロスは15個スタート）
    public int BallCount { get; private set; }

    // コンストラクタ：色を決めて初期化する
    public PlayerModel(PlayerColor color)
    {
        Color = color;
        BallCount = 15; // 公式ルール通り15個で開始
    }

    // ボールを使う（盤面に置いたとき）
    public void DecreaseBall()
    {
        if (BallCount > 0)
        {
            BallCount--;
        }
    }

    // ボールを回収する（1～2個戻ってきたとき）
    public void IncreaseBall(int amount)
    {
        BallCount += amount;
    }

    // まだボールを持っているか？（持っていなければ負け）
    public bool HasBalls()
    {
        return BallCount > 0;
    }
}