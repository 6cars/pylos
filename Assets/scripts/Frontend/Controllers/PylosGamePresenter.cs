using UnityEngine;

/// <summary>
/// バックとフロントをつなぐ橋渡し役
/// </summary>
public class PylosGamePresenter : MonoBehaviour
{
    // ★ここを追加：Unityの画面でBoardViewを割り当てられるようにする
    [SerializeField] private BoardView _boardView;

    private GameContext _gameContext;
    private PhaseManager _phaseManager;
    private Rules _rules;

    private void Awake()
    {
        // 初期化
        _gameContext = new GameContext();
        _rules = new Rules();
        _phaseManager = new PhaseManager(_gameContext, _rules);

        // 購読
        _phaseManager.OnPhaseChanged += OnPhaseChanged;
        GameEvents.OnBallPlaced += OnBallPlaced;
        GameEvents.OnBallRecovered += OnBallRecovered;
        GameEvents.OnPlayerChanged += OnPlayerChanged;
        GameEvents.OnGameOver += OnGameOver;
        GameEvents.OnRecoveryRightGranted += OnRecoveryRightGranted;
        GameEvents.OnRecoveryRightChanged += OnRecoveryRightChanged;
    }

    private void OnDestroy()
    {
        // 解除
        if (_phaseManager != null) _phaseManager.OnPhaseChanged -= OnPhaseChanged;
        GameEvents.OnBallPlaced -= OnBallPlaced;
        GameEvents.OnBallRecovered -= OnBallRecovered;
        GameEvents.OnPlayerChanged -= OnPlayerChanged;
        GameEvents.OnGameOver -= OnGameOver;
        GameEvents.OnRecoveryRightGranted -= OnRecoveryRightGranted;
        GameEvents.OnRecoveryRightChanged -= OnRecoveryRightChanged;
    }

    // ============================================
    // Public Methods
    // ============================================
    public bool TryPlaceBall(int x, int y, int z) => _phaseManager.PlaceBall(x, y, z);
    public bool TryRecoverBall(int x, int y, int z) => _phaseManager.RecoverBall(x, y, z);
    public void SkipRecovery() => _phaseManager.SkipRecovery();
    public PhaseState GetCurrentPhase() => _phaseManager.CurrentPhase;
    public BallColor GetCurrentPlayerColor() => _phaseManager.CurrentPlayerColor;
    public GameContext GetGameContext() => _gameContext;
    public BoardModel GetBoardModel() => _gameContext.Board;
    public void GrantRecoveryRightsAtRetrievalPhaseStart() => _phaseManager.GrantRecoveryRightsAtRetrievalPhaseStart();

    // ============================================
    // Event Handlers (ここをつないだ！)
    // ============================================

    private void OnPhaseChanged(PhaseState newPhase)
    {
        Debug.Log($"フェーズ変更: {newPhase}");
    }

    private void OnBallPlaced(int x, int y, int z, PlayerColor color) // PlayerColor型に注意
    {
        Debug.Log($"ボール配置: ({x}, {y}, {z}), 色: {color}");
        // ★Viewに描画命令を出す
        if (_boardView != null)
        {
            // PlayerColor型とBallColor型の変換が必要ならここで行う
            // とりあえずキャストで対応（同じenum定義ならOK）
            _boardView.PlaceBallView(x, y, z, (BallColor)color);
        }
    }

    private void OnBallRecovered(int x, int y, int z)
    {
        Debug.Log($"ボール回収: ({x}, {y}, {z})");
        // ★Viewに削除命令を出す
        if (_boardView != null)
        {
            _boardView.RemoveBallView(x, y, z);
        }
    }

    private void OnPlayerChanged(BallColor player) { Debug.Log($"プレイヤー変更: {player}"); }
    private void OnGameOver(BallColor winner) { Debug.Log($"ゲーム終了: 勝者 {winner}"); }
    private void OnRecoveryRightGranted(BallColor player) { Debug.Log($"回収権獲得: {player}"); }
    private void OnRecoveryRightChanged(BallColor player, int count) { Debug.Log($"回収権変更: {player} = {count}"); }
}