using UnityEngine;

/// <summary>
/// バックとフロントをつなぐ橋渡し役
/// </summary>
public class PylosGamePresenter : MonoBehaviour
{
    private GameContext _gameContext;
    private PhaseManager _phaseManager;
    private Rules _rules;
    
    private void Awake()
    {
        // ゲームコンテキストとフェーズマネージャーの初期化
        _gameContext = new GameContext();
        _rules = new Rules();
        _phaseManager = new PhaseManager(_gameContext, _rules);
        
        // フェーズ変更イベントの購読
        _phaseManager.OnPhaseChanged += OnPhaseChanged;
        
        // ゲームイベントの購読
        GameEvents.OnBallPlaced += OnBallPlaced;
        GameEvents.OnBallRecovered += OnBallRecovered;
        GameEvents.OnPlayerChanged += OnPlayerChanged;
        GameEvents.OnGameOver += OnGameOver;
        GameEvents.OnRecoveryRightGranted += OnRecoveryRightGranted;
        GameEvents.OnRecoveryRightChanged += OnRecoveryRightChanged;
    }
    
    private void OnDestroy()
    {
        // イベントの購読解除
        if (_phaseManager != null)
        {
            _phaseManager.OnPhaseChanged -= OnPhaseChanged;
        }
        
        GameEvents.OnBallPlaced -= OnBallPlaced;
        GameEvents.OnBallRecovered -= OnBallRecovered;
        GameEvents.OnPlayerChanged -= OnPlayerChanged;
        GameEvents.OnGameOver -= OnGameOver;
        GameEvents.OnRecoveryRightGranted -= OnRecoveryRightGranted;
        GameEvents.OnRecoveryRightChanged -= OnRecoveryRightChanged;
    }
    
    // ============================================
    // パブリックメソッド：フロントエンドから呼ばれる
    // ============================================
    
    /// <summary>
    /// ボールを配置する（設置フェーズ時）
    /// </summary>
    public bool TryPlaceBall(int x, int y, int z)
    {
        return _phaseManager.PlaceBall(x, y, z);
    }
    
    /// <summary>
    /// ボールを回収する（回収フェーズ時）
    /// </summary>
    public bool TryRecoverBall(int x, int y, int z)
    {
        return _phaseManager.RecoverBall(x, y, z);
    }
    
    /// <summary>
    /// 回収権をスキップする（回収フェーズ時）
    /// </summary>
    public void SkipRecovery()
    {
        _phaseManager.SkipRecovery();
    }
    
    /// <summary>
    /// 現在のフェーズを取得
    /// </summary>
    public PhaseState GetCurrentPhase()
    {
        return _phaseManager.CurrentPhase;
    }
    
    /// <summary>
    /// 現在のプレイヤーの色を取得
    /// </summary>
    public BallColor GetCurrentPlayerColor()
    {
        return _phaseManager.CurrentPlayerColor;
    }
    
    /// <summary>
    /// ゲームコンテキストを取得（読み取り専用アクセス用）
    /// </summary>
    public GameContext GetGameContext()
    {
        return _gameContext;
    }
    
    /// <summary>
    /// 盤面モデルを取得
    /// </summary>
    public BoardModel GetBoardModel()
    {
        return _gameContext.Board;
    }
    
    /// <summary>
    /// 回収フェーズ開始時に回収権を配る
    /// </summary>
    public void GrantRecoveryRightsAtRetrievalPhaseStart()
    {
        _phaseManager.GrantRecoveryRightsAtRetrievalPhaseStart();
    }
    
    // ============================================
    // イベントハンドラー：バックエンドからの通知を受け取る
    // ============================================
    
    private void OnPhaseChanged(PhaseState newPhase)
    {
        Debug.Log($"フェーズ変更: {newPhase}");
        // フロントエンドのViewに通知する処理はここに追加
    }
    
    private void OnBallPlaced(int x, int y, int z, PlayerColor color)
    {
        Debug.Log($"ボール配置: ({x}, {y}, {z}), 色: {color}");
        // フロントエンドのViewに通知する処理はここに追加
    }
    
    private void OnBallRecovered(int x, int y, int z)
    {
        Debug.Log($"ボール回収: ({x}, {y}, {z})");
        // フロントエンドのViewに通知する処理はここに追加
    }
    
    private void OnPlayerChanged(BallColor player)
    {
        Debug.Log($"プレイヤー変更: {player}");
        // フロントエンドのViewに通知する処理はここに追加
    }
    
    private void OnGameOver(BallColor winner)
    {
        Debug.Log($"ゲーム終了: 勝者 {winner}");
        // フロントエンドのViewに通知する処理はここに追加
    }
    
    private void OnRecoveryRightGranted(BallColor player)
    {
        Debug.Log($"回収権獲得: {player}");
        // フロントエンドのViewに通知する処理はここに追加
    }
    
    private void OnRecoveryRightChanged(BallColor player, int count)
    {
        Debug.Log($"回収権変更: {player} = {count}");
        // フロントエンドのViewに通知する処理はここに追加
    }
}
