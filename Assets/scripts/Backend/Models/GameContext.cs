using System;

/// <summary>
/// ゲーム全体の状態コンテナ
/// 盤面、プレイヤー、現在のフェーズなどを保持
/// </summary>
public class GameContext
{
    public BoardModel Board { get; private set; }
    public PlayerModel PlayerWhite { get; private set; }
    public PlayerModel PlayerBlack { get; private set; }
    
    // 現在のプレイヤーの色（BallColorで管理）
    private BallColor _currentPlayerColor;
    public BallColor CurrentPlayerColor
    {
        get => _currentPlayerColor;
        set
        {
            _currentPlayerColor = value;
            GameEvents.PlayerChanged(value);
        }
    }
    
    // プレイヤーの回収権数
    public int WhiteRecoveryRights { get; set; }
    public int BlackRecoveryRights { get; set; }
    
    // ゲーム終了フラグ
    public bool IsGameOver { get; set; }
    public BallColor? Winner { get; set; }
    
    public GameContext()
    {
        Board = new BoardModel();
        PlayerWhite = new PlayerModel(PlayerColor.White);
        PlayerBlack = new PlayerModel(PlayerColor.Black);
        CurrentPlayerColor = BallColor.White; // 白から開始
        WhiteRecoveryRights = 0;
        BlackRecoveryRights = 0;
        IsGameOver = false;
        Winner = null;
    }
    
    /// <summary>
    /// 現在のプレイヤーのモデルを取得
    /// </summary>
    public PlayerModel GetCurrentPlayerModel()
    {
        return CurrentPlayerColor == BallColor.White ? PlayerWhite : PlayerBlack;
    }
    
    /// <summary>
    /// 現在のプレイヤーの回収権数を取得
    /// </summary>
    public int GetCurrentPlayerRecoveryRights()
    {
        return CurrentPlayerColor == BallColor.White ? WhiteRecoveryRights : BlackRecoveryRights;
    }
    
    /// <summary>
    /// 現在のプレイヤーの回収権を増やす
    /// </summary>
    public void AddRecoveryRightToCurrentPlayer()
    {
        if (CurrentPlayerColor == BallColor.White)
        {
            WhiteRecoveryRights++;
        }
        else
        {
            BlackRecoveryRights++;
        }
        GameEvents.RecoveryRightChanged(CurrentPlayerColor, GetCurrentPlayerRecoveryRights());
    }
    
    /// <summary>
    /// 現在のプレイヤーの回収権を減らす
    /// </summary>
    public void ConsumeRecoveryRightFromCurrentPlayer()
    {
        if (CurrentPlayerColor == BallColor.White)
        {
            if (WhiteRecoveryRights > 0) WhiteRecoveryRights--;
        }
        else
        {
            if (BlackRecoveryRights > 0) BlackRecoveryRights--;
        }
        GameEvents.RecoveryRightChanged(CurrentPlayerColor, GetCurrentPlayerRecoveryRights());
    }
    
    /// <summary>
    /// ターンを切り替える
    /// </summary>
    public void SwitchPlayer()
    {
        CurrentPlayerColor = CurrentPlayerColor == BallColor.White ? BallColor.Black : BallColor.White;
    }
    
    /// <summary>
    /// BallColorをPlayerColorに変換
    /// </summary>
    public static PlayerColor ToPlayerColor(BallColor ballColor)
    {
        switch (ballColor)
        {
            case BallColor.White:
                return PlayerColor.White;
            case BallColor.Black:
                return PlayerColor.Black;
            default:
                return PlayerColor.None;
        }
    }
    
    /// <summary>
    /// PlayerColorをBallColorに変換
    /// </summary>
    public static BallColor ToBallColor(PlayerColor playerColor)
    {
        switch (playerColor)
        {
            case PlayerColor.White:
                return BallColor.White;
            case PlayerColor.Black:
                return BallColor.Black;
            default:
                return BallColor.None;
        }
    }
}

