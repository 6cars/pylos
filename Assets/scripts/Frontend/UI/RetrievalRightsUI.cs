using UnityEngine;
using UnityEngine.UI;
using System;

// 回収権の表示パネル制御
public class RetrievalRightsUI : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private GameObject panel; // 回収権パネル全体
    [SerializeField] private Text whiteRightsText; // 白の回収権表示テキスト
    [SerializeField] private Text blackRightsText; // 黒の回収権表示テキスト
    
    [Header("プレゼンター参照")]
    [SerializeField] private PylosGamePresenter presenter;
    
    // 現在の回収権数
    private int whiteRights = 0;
    private int blackRights = 0;
    
    private void Awake()
    {
        // プレゼンターが設定されていない場合は自動検索
        if (presenter == null)
        {
            presenter = FindObjectOfType<PylosGamePresenter>();
        }
        
        // 初期状態ではパネルを非表示
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
    
    private void OnEnable()
    {
        // イベントの購読
        GameEvents.OnRecoveryRightChanged += OnRecoveryRightChanged;
        GameEvents.OnPhaseChanged += OnPhaseChanged;
        
        // 初期状態を更新
        UpdateUI();
    }
    
    private void OnDisable()
    {
        // イベントの購読解除
        GameEvents.OnRecoveryRightChanged -= OnRecoveryRightChanged;
        GameEvents.OnPhaseChanged -= OnPhaseChanged;
    }
    
    /// <summary>
    /// 回収権が変更された時のイベントハンドラー
    /// </summary>
    private void OnRecoveryRightChanged(BallColor player, int count)
    {
        // 回収権数を更新
        if (player == BallColor.White)
        {
            whiteRights = count;
        }
        else if (player == BallColor.Black)
        {
            blackRights = count;
        }
        
        UpdateUI();
    }
    
    /// <summary>
    /// フェーズが変更された時のイベントハンドラー
    /// </summary>
    private void OnPhaseChanged(PhaseState phase)
    {
        UpdateUI();
    }
    
    /// <summary>
    /// UIを更新する
    /// </summary>
    private void UpdateUI()
    {
        if (presenter == null) return;
        
        // 現在のフェーズを取得
        PhaseState currentPhase = presenter.GetCurrentPhase();
        
        // 回収フェーズの時だけパネルを表示
        bool shouldShow = (currentPhase == PhaseState.Retrieval);
        
        if (panel != null)
        {
            panel.SetActive(shouldShow);
        }
        
        // 回収権数を取得して更新
        if (presenter.GetGameContext() != null)
        {
            var context = presenter.GetGameContext();
            whiteRights = context.WhiteRecoveryRights;
            blackRights = context.BlackRecoveryRights;
        }
        
        // テキストを更新
        if (whiteRightsText != null)
        {
            whiteRightsText.text = $"白: {whiteRights}";
        }
        
        if (blackRightsText != null)
        {
            blackRightsText.text = $"黒: {blackRights}";
        }
    }
}

