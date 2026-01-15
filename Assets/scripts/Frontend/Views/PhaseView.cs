using UnityEngine;
using UnityEngine.UI;

// 現在のフェーズやメッセージのUI表示
public class PhaseView : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private Text phaseText; // フェーズ表示用テキスト
    [SerializeField] private Text playerText; // 現在のプレイヤー表示用テキスト
    [SerializeField] private Text messageText; // メッセージ表示用テキスト（勝敗など）
    [SerializeField] private GameObject messagePanel; // メッセージパネル（勝敗表示用）
    
    [Header("プレゼンター参照")]
    [SerializeField] private PylosGamePresenter presenter;
    
    private void Awake()
    {
        // プレゼンターが設定されていない場合は自動検索
        if (presenter == null)
        {
            presenter = FindObjectOfType<PylosGamePresenter>();
        }
        
        // 初期状態ではメッセージパネルを非表示
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }
    
    private void OnEnable()
    {
        // イベントの購読
        GameEvents.OnPhaseChanged += OnPhaseChanged;
        GameEvents.OnPlayerChanged += OnPlayerChanged;
        GameEvents.OnGameOver += OnGameOver;
        
        // 初期状態を更新
        UpdateUI();
    }
    
    private void OnDisable()
    {
        // イベントの購読解除
        GameEvents.OnPhaseChanged -= OnPhaseChanged;
        GameEvents.OnPlayerChanged -= OnPlayerChanged;
        GameEvents.OnGameOver -= OnGameOver;
    }
    
    /// <summary>
    /// フェーズが変更された時のイベントハンドラー
    /// </summary>
    private void OnPhaseChanged(PhaseState phase)
    {
        UpdateUI();
    }
    
    /// <summary>
    /// プレイヤーが変更された時のイベントハンドラー
    /// </summary>
    private void OnPlayerChanged(BallColor player)
    {
        UpdateUI();
    }
    
    /// <summary>
    /// ゲームが終了した時のイベントハンドラー
    /// </summary>
    private void OnGameOver(BallColor winner)
    {
        ShowGameOverMessage(winner);
    }
    
    /// <summary>
    /// UIを更新する
    /// </summary>
    private void UpdateUI()
    {
        if (presenter == null) return;
        
        // フェーズ表示を更新
        if (phaseText != null)
        {
            PhaseState currentPhase = presenter.GetCurrentPhase();
            phaseText.text = GetPhaseDisplayText(currentPhase);
        }
        
        // プレイヤー表示を更新
        if (playerText != null)
        {
            BallColor currentPlayer = presenter.GetCurrentPlayerColor();
            playerText.text = GetPlayerDisplayText(currentPlayer);
        }
    }
    
    /// <summary>
    /// フェーズの表示テキストを取得
    /// </summary>
    private string GetPhaseDisplayText(PhaseState phase)
    {
        switch (phase)
        {
            case PhaseState.PlayerSelection:
                return "プレイヤー選択";
            case PhaseState.Placement:
                return "設置フェーズ";
            case PhaseState.Retrieval:
                return "回収フェーズ";
            case PhaseState.End:
                return "ゲーム終了";
            default:
                return "不明";
        }
    }
    
    /// <summary>
    /// プレイヤーの表示テキストを取得
    /// </summary>
    private string GetPlayerDisplayText(BallColor player)
    {
        switch (player)
        {
            case BallColor.White:
                return "現在のプレイヤー: 白";
            case BallColor.Black:
                return "現在のプレイヤー: 黒";
            default:
                return "現在のプレイヤー: -";
        }
    }
    
    /// <summary>
    /// ゲーム終了メッセージを表示
    /// </summary>
    private void ShowGameOverMessage(BallColor winner)
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
        }
        
        if (messageText != null)
        {
            string winnerText = winner == BallColor.White ? "白" : "黒";
            messageText.text = $"ゲーム終了！\n勝者: {winnerText}";
        }
    }
    
    /// <summary>
    /// メッセージパネルを非表示にする（外部から呼び出し可能）
    /// </summary>
    public void HideMessage()
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }
}

