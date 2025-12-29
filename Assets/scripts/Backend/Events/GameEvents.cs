using System;
using UnityEngine;

/// <summary>
/// フロントに変化を伝えるためのC#イベント定義
/// </summary>
public static class GameEvents
{
    // ボールが配置された
    public static event Action<int, int, int, PlayerColor> OnBallPlaced;
    
    // ボールが回収された
    public static event Action<int, int, int> OnBallRecovered;
    
    // フェーズが変更された
    public static event Action<PhaseState> OnPhaseChanged;
    
    // プレイヤーが切り替わった
    public static event Action<BallColor> OnPlayerChanged;
    
    // ゲームが終了した
    public static event Action<BallColor> OnGameOver;
    
    // 回収権が発生した
    public static event Action<BallColor> OnRecoveryRightGranted;
    
    // 回収権が変更された（色と数）
    public static event Action<BallColor, int> OnRecoveryRightChanged;
    
    // イベント発火メソッド
    public static void BallPlaced(int x, int y, int z, PlayerColor color)
    {
        OnBallPlaced?.Invoke(x, y, z, color);
    }
    
    public static void BallRecovered(int x, int y, int z)
    {
        OnBallRecovered?.Invoke(x, y, z);
    }
    
    public static void PhaseChanged(PhaseState phase)
    {
        OnPhaseChanged?.Invoke(phase);
    }
    
    public static void PlayerChanged(BallColor player)
    {
        OnPlayerChanged?.Invoke(player);
    }
    
    public static void GameOver(BallColor winner)
    {
        OnGameOver?.Invoke(winner);
    }
    
    public static void RecoveryRightGranted(BallColor player)
    {
        OnRecoveryRightGranted?.Invoke(player);
    }
    
    public static void RecoveryRightChanged(BallColor player, int count)
    {
        OnRecoveryRightChanged?.Invoke(player, count);
    }
}
