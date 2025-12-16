using UnityEngine;

public class Player : MonoBehaviour
{
    // プレイヤーの種類 (CPU or Player)
    public PlayerType Type;

    // 担当するボールの色
    public BallType MyColor;

    // 回収権（表のクラスを使用）
    public RetrievalBallTurn RecoveryRights = new RetrievalBallTurn();

    // 初期化処理
    public void Init(PlayerType type, BallType color)
    {
        Type = type;
        MyColor = color;
        RecoveryRights.RetrievalBallTurn_Number = 1; // 最初は1つ持っているルールの場合
    }

    // ここに行動決定などのメソッドを後で追加していきます
}