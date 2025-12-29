// AIの「共通ルール」を決めるインターフェース
public interface IAIAlgorithm
{
    // 1. 設置フェーズ：どう動くか決める（配置 or 移動）
    AIAction GetSetupMove(BoardModel board, PlayerModel me);

    // 2. 回収フェーズ：どれを回収するか決める（回収 or パス）
    PylosCoordinate GetRecoveryMove(BoardModel board, PlayerModel me);
}