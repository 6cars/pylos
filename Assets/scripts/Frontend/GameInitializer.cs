using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム開始時に必要なGameObjectを自動生成する
/// 
/// このクラスはオプショナルな自動初期化機能を提供します。
/// このクラスが存在しなくても、他のスクリプト（BoardView、PhaseView、InputControllerなど）
/// は手動設定で動作可能です。
/// 
/// 使用方法:
/// - このファイルをプロジェクトに含めることで、自動初期化が有効になります
/// - 手動設定を使用する場合は、このファイルを削除または無効化してください
/// </summary>
public static class GameInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitializeGame()
    {
        // 既に存在する場合は何もしない
        if (Object.FindObjectOfType<PylosGamePresenter>() != null)
        {
            return;
        }

        // GameManagerを作成
        GameObject gameManager = new GameObject("GameManager");
        gameManager.AddComponent<PylosGamePresenter>();

        // InputControllerを作成
        GameObject inputController = new GameObject("InputController");
        inputController.AddComponent<InputController>();

        // BoardViewを作成
        GameObject boardView = new GameObject("BoardView");
        boardView.AddComponent<BoardView>();

        // UI (PhaseView)を作成
        CreateUI();

        // カメラの位置を調整
        AdjustCamera();
    }

    /// <summary>
    /// カメラの位置を盤面が見えるように調整
    /// </summary>
    static void AdjustCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = Object.FindObjectOfType<Camera>();
        }

        if (mainCamera != null)
        {
            // 盤面が見える位置にカメラを配置
            mainCamera.transform.position = new Vector3(0, 5, -5);
            mainCamera.transform.rotation = Quaternion.Euler(45, 0, 0);
        }
    }

    /// <summary>
    /// UI要素を自動生成
    /// </summary>
    static void CreateUI()
    {
        // Canvasが存在しない場合は作成
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // PhaseViewが既に存在する場合は何もしない
        if (canvas.GetComponent<PhaseView>() != null)
        {
            return;
        }

        // PhaseViewコンポーネントを追加
        PhaseView phaseView = canvas.gameObject.AddComponent<PhaseView>();

        // PhaseTextを作成
        GameObject phaseTextObj = new GameObject("PhaseText");
        phaseTextObj.transform.SetParent(canvas.transform, false);
        Text phaseText = phaseTextObj.AddComponent<Text>();
        phaseText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        phaseText.fontSize = 24;
        phaseText.alignment = TextAnchor.UpperLeft;
        phaseText.text = "設置フェーズ";
        RectTransform phaseRect = phaseTextObj.GetComponent<RectTransform>();
        phaseRect.anchorMin = new Vector2(0, 1);
        phaseRect.anchorMax = new Vector2(0, 1);
        phaseRect.pivot = new Vector2(0, 1);
        phaseRect.anchoredPosition = new Vector2(10, -10);
        phaseRect.sizeDelta = new Vector2(200, 30);

        // PlayerTextを作成
        GameObject playerTextObj = new GameObject("PlayerText");
        playerTextObj.transform.SetParent(canvas.transform, false);
        Text playerText = playerTextObj.AddComponent<Text>();
        playerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        playerText.fontSize = 24;
        playerText.alignment = TextAnchor.UpperLeft;
        playerText.text = "現在のプレイヤー: 白";
        RectTransform playerRect = playerTextObj.GetComponent<RectTransform>();
        playerRect.anchorMin = new Vector2(0, 1);
        playerRect.anchorMax = new Vector2(0, 1);
        playerRect.pivot = new Vector2(0, 1);
        playerRect.anchoredPosition = new Vector2(10, -50);
        playerRect.sizeDelta = new Vector2(200, 30);

        // MessagePanelを作成
        GameObject messagePanelObj = new GameObject("MessagePanel");
        messagePanelObj.transform.SetParent(canvas.transform, false);
        Image panelImage = messagePanelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        RectTransform panelRect = messagePanelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(400, 200);
        messagePanelObj.SetActive(false);

        // MessageTextを作成
        GameObject messageTextObj = new GameObject("MessageText");
        messageTextObj.transform.SetParent(messagePanelObj.transform, false);
        Text messageText = messageTextObj.AddComponent<Text>();
        messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        messageText.fontSize = 32;
        messageText.alignment = TextAnchor.MiddleCenter;
        messageText.text = "ゲーム終了！";
        RectTransform messageRect = messageTextObj.GetComponent<RectTransform>();
        messageRect.anchorMin = Vector2.zero;
        messageRect.anchorMax = Vector2.one;
        messageRect.sizeDelta = Vector2.zero;
        messageRect.anchoredPosition = Vector2.zero;

        // PhaseViewのAwakeメソッドで自動的に検索されるので、ここでは何もしない
    }
}
