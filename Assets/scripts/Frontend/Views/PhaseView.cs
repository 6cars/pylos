using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 現在のフェーズやメッセージのUI表示
/// 
/// このクラスはGameInitializerに依存しません。
/// 手動設定でも動作します：
/// - Canvasにこのコンポーネントをアタッチ
/// - InspectorでUI要素（Text、Panel）を設定
/// - または、UI要素を名前（PhaseText、PlayerText、MessageText、MessagePanel）で作成すると自動検索されます
/// </summary>
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
        
        // UI要素が設定されていない場合は自動検索
        // 注意: 手動設定の場合は、以下の名前でGameObjectを作成してください
        // - PhaseText, PlayerText, MessageText, MessagePanel
        if (phaseText == null)
        {
            phaseText = FindUIElement<Text>("PhaseText");
        }
        
        if (playerText == null)
        {
            playerText = FindUIElement<Text>("PlayerText");
        }
        
        if (messageText == null)
        {
            messageText = FindUIElement<Text>("MessageText");
        }
        
        if (messagePanel == null)
        {
            GameObject panelObj = FindUIElementByName("MessagePanel");
            if (panelObj != null)
            {
                messagePanel = panelObj;
            }
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

    /// <summary>
    /// UI要素を検索する（GameInitializerに依存しない独立した検索機能）
    /// まず名前で検索し、見つからない場合はCanvas内を検索
    /// </summary>
    private T FindUIElement<T>(string name) where T : Component
    {
        GameObject obj = FindUIElementByName(name);
        if (obj != null)
        {
            T component = obj.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
        }

        return null;
    }

    /// <summary>
    /// GameObjectを名前で検索（GameInitializerに依存しない独立した検索機能）
    /// </summary>
    private GameObject FindUIElementByName(string name)
    {
        // まず名前で直接検索
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            return obj;
        }

        // Canvas内を検索
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        if (canvas != null)
        {
            // Canvasの子要素を再帰的に検索
            Transform found = FindChildByName(canvas.transform, name);
            if (found != null)
            {
                return found.gameObject;
            }
        }

        return null;
    }

    /// <summary>
    /// 子要素を名前で再帰的に検索
    /// </summary>
    private Transform FindChildByName(Transform parent, string name)
    {
        if (parent.name == name)
        {
            return parent;
        }

        foreach (Transform child in parent)
        {
            Transform found = FindChildByName(child, name);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }
}

