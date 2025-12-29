using System;
using UnityEngine;

/// <summary>
/// フェーズ遷移のステートマシン管理
/// </summary>
public class PhaseManager
{
    private GameContext _context;
    private Rules _rules;
    
    // 現在のフェーズ
    private PhaseState _currentPhase;
    public PhaseState CurrentPhase
    {
        get => _currentPhase;
        private set
        {
            if (_currentPhase != value)
            {
                _currentPhase = value;
                OnPhaseChanged?.Invoke(value);
                GameEvents.PhaseChanged(value);
            }
        }
    }
    
    // 現在の手番プレイヤーの色
    public BallColor CurrentPlayerColor => _context.CurrentPlayerColor;
    
    // フェーズが変わった時にフロントへ通知する
    public event Action<PhaseState> OnPhaseChanged;
    
    public PhaseManager(GameContext context, Rules rules)
    {
        _context = context;
        _rules = rules;
        CurrentPhase = PhaseState.Placement;
    }
    
    /// <summary>
    /// ボールを配置する（設置フェーズ）
    /// </summary>
    public bool PlaceBall(int x, int y, int z)
    {
        if (CurrentPhase != PhaseState.Placement)
        {
            Debug.LogWarning("現在は設置フェーズではありません");
            return false;
        }
        
        // ルールチェック
        if (!_rules.CanPlaceAt(_context.Board, x, y, z))
        {
            return false;
        }
        
        // プレイヤーの色を取得
        PlayerColor playerColor = GameContext.ToPlayerColor(_context.CurrentPlayerColor);
        
        // ボールを配置
        _context.Board.PlaceBall(x, y, z, playerColor);
        
        // プレイヤーのボール数を減らす
        _context.GetCurrentPlayerModel().DecreaseBall();
        
        // イベント通知
        GameEvents.BallPlaced(x, y, z, playerColor);
        
        // 回収権のチェック
        if (_rules.CheckRecovery(_context.Board, x, y, z, playerColor))
        {
            _context.AddRecoveryRightToCurrentPlayer();
            GameEvents.RecoveryRightGranted(_context.CurrentPlayerColor);
        }
        
        // 勝利判定
        if (_rules.CheckWin(z))
        {
            _context.IsGameOver = true;
            _context.Winner = _context.CurrentPlayerColor;
            CurrentPhase = PhaseState.End;
            GameEvents.GameOver(_context.CurrentPlayerColor);
            return true;
        }
        
        // ターン終了、次のプレイヤーへ
        _context.SwitchPlayer();
        
        // 次のプレイヤーに回収権があるかチェック
        if (_context.GetCurrentPlayerRecoveryRights() > 0)
        {
            CurrentPhase = PhaseState.Retrieval;
        }
        else
        {
            CurrentPhase = PhaseState.Placement;
        }
        
        return true;
    }
    
    /// <summary>
    /// ボールを回収する（回収フェーズ）
    /// </summary>
    public bool RecoverBall(int x, int y, int z)
    {
        if (CurrentPhase != PhaseState.Retrieval)
        {
            Debug.LogWarning("現在は回収フェーズではありません");
            return false;
        }
        
        // 回収権があるかチェック
        if (_context.GetCurrentPlayerRecoveryRights() <= 0)
        {
            return false;
        }
        
        PlayerColor playerColor = GameContext.ToPlayerColor(_context.CurrentPlayerColor);
        
        // 回収可能かチェック（自分のボールで、上にボールがないこと）
        if (_context.Board.GetColor(x, y, z) != playerColor)
        {
            return false;
        }
        
        // 上にボールがないかチェック
        if (_context.Board.HasBall(x, y, z + 1))
        {
            return false;
        }
        
        // ボールを回収（Noneで上書き）
        _context.Board.PlaceBall(x, y, z, PlayerColor.None);
        
        // プレイヤーのボール数を増やす（1個回収）
        _context.GetCurrentPlayerModel().IncreaseBall(1);
        
        // イベント通知
        GameEvents.BallRecovered(x, y, z);
        
        // 回収権を消費
        _context.ConsumeRecoveryRightFromCurrentPlayer();
        
        // ターン終了
        _context.SwitchPlayer();
        
        // 次のプレイヤーに回収権があるかチェック
        if (_context.GetCurrentPlayerRecoveryRights() > 0)
        {
            CurrentPhase = PhaseState.Retrieval;
        }
        else
        {
            CurrentPhase = PhaseState.Placement;
        }
        
        return true;
    }
    
    /// <summary>
    /// 回収権をスキップする（回収フェーズ時）
    /// </summary>
    public void SkipRecovery()
    {
        if (CurrentPhase == PhaseState.Retrieval)
        {
            // ターン終了
            _context.SwitchPlayer();
            
            // 次のプレイヤーに回収権があるかチェック
            if (_context.GetCurrentPlayerRecoveryRights() > 0)
            {
                CurrentPhase = PhaseState.Retrieval;
            }
            else
            {
                CurrentPhase = PhaseState.Placement;
            }
        }
    }
    
    /// <summary>
    /// 回収フェーズ開始時に各プレイヤーに回収権を1つずつ配る
    /// </summary>
    public void GrantRecoveryRightsAtRetrievalPhaseStart()
    {
        _context.WhiteRecoveryRights = 1;
        _context.BlackRecoveryRights = 1;
        GameEvents.RecoveryRightChanged(BallColor.White, 1);
        GameEvents.RecoveryRightChanged(BallColor.Black, 1);
    }
}

/// <summary>
/// ゲームのフェーズ定義
/// </summary>
public enum PhaseState
{
    PlayerSelection,  // プレイヤー選択フェーズ
    Placement,        // 設置フェーズ
    Retrieval,        // 回収フェーズ
    End               // エンドフェーズ
}
