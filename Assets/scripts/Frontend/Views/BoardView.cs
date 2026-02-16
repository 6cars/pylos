using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 盤面の3D表示管理
public class BoardView : MonoBehaviour
{
    [Header("ボールプレハブ")]
    [SerializeField] private GameObject whiteBallPrefab; // 白いボールのプレハブ
    [SerializeField] private GameObject blackBallPrefab; // 黒いボールのプレハブ
    
    // 注意: 古いballPrefabフィールドは削除されました。whiteBallPrefabとblackBallPrefabを使用してください。
    
    [Header("プレゼンター参照")]
    [SerializeField] private PylosGamePresenter presenter;
    
    // 座標からBallViewへのマッピング
    private Dictionary<string, BallView> _ballViews = new Dictionary<string, BallView>();
    
    // ボールの親オブジェクト（整理用）
    private Transform _ballsParent;
    
    // グリッド線の親オブジェクト
    private Transform _gridParent;
    
    // ハイライトの親オブジェクト
    private Transform _highlightParent;
    private Dictionary<string, GameObject> _highlights = new Dictionary<string, GameObject>();
    
    [Header("ハイライト設定")]
    [SerializeField] private Color highlightColor = new Color(0f, 1f, 0f, 0.3f); // ハイライトの色（緑、半透明）
    [SerializeField] private float highlightHeight = 0.05f; // ハイライトの高さ
    [SerializeField] private bool showPlacementHighlights = true; // 配置可能な場所をハイライト表示するか
    
    [Header("グリッド設定")]
    [SerializeField] private Color gridLineColor = new Color(0.5f, 0.5f, 0.5f, 1.0f); // グリッド線の色
    [SerializeField] private float gridLineWidth = 0.02f; // グリッド線の太さ
    [SerializeField] private float gridHeight = 0.01f; // グリッド線の高さ（盤面より少し上）
    [SerializeField] private Material gridLineMaterial; // グリッド線用のマテリアル（オプション）
    
    private void Awake()
    {
        // プレゼンターが設定されていない場合は自動検索
        if (presenter == null)
        {
            presenter = FindObjectOfType<PylosGamePresenter>();
        }
        
        // ボールの親オブジェクトを作成
        _ballsParent = new GameObject("Balls").transform;
        _ballsParent.SetParent(transform);
        
        // グリッド線の親オブジェクトを作成
        _gridParent = new GameObject("GridLines").transform;
        _gridParent.SetParent(transform);
        
        // ハイライトの親オブジェクトを作成
        _highlightParent = new GameObject("Highlights").transform;
        _highlightParent.SetParent(transform);
        
        // グリッド線を描画
        DrawGridLines();
    }
    
    private void OnEnable()
    {
        // イベントの購読
        GameEvents.OnBallPlaced += OnBallPlaced;
        GameEvents.OnBallRecovered += OnBallRecovered;
        GameEvents.OnPhaseChanged += OnPhaseChanged;
        GameEvents.OnPlayerChanged += OnPlayerChanged;
        
        // 初期状態を同期
        SyncBoardState();
        
        // ハイライトを更新
        UpdateHighlights();
    }
    
    private void OnDisable()
    {
        // イベントの購読解除
        GameEvents.OnBallPlaced -= OnBallPlaced;
        GameEvents.OnBallRecovered -= OnBallRecovered;
        GameEvents.OnPhaseChanged -= OnPhaseChanged;
        GameEvents.OnPlayerChanged -= OnPlayerChanged;
    }
    
    /// <summary>
    /// ボールが配置された時のイベントハンドラー
    /// </summary>
    private void OnBallPlaced(int x, int y, int z, PlayerColor color)
    {
        PylosCoordinate coord = new PylosCoordinate(x, y, z);
        string key = GetCoordinateKey(coord);
        
        // 既にボールが存在する場合は更新、存在しない場合は新規作成
        if (_ballViews.ContainsKey(key))
        {
            var ballView = _ballViews[key];
            ballView.PlaceBall(coord, color);
        }
        else
        {
            CreateBall(coord, color);
        }
    }
    
    /// <summary>
    /// ボールが回収された時のイベントハンドラー
    /// </summary>
    private void OnBallRecovered(int x, int y, int z)
    {
        PylosCoordinate coord = new PylosCoordinate(x, y, z);
        string key = GetCoordinateKey(coord);
        
        if (_ballViews.ContainsKey(key))
        {
            var ballView = _ballViews[key];
            ballView.RecoverBall();
            
            // アニメーション完了後に削除（コルーチンで待機）
            StartCoroutine(RemoveBallAfterAnimation(ballView, key));
        }
        
        // ハイライトを更新
        UpdateHighlights();
    }
    
    /// <summary>
    /// フェーズ変更時のイベントハンドラー
    /// </summary>
    private void OnPhaseChanged(PhaseState newPhase)
    {
        UpdateHighlights();
    }
    
    /// <summary>
    /// プレイヤー変更時のイベントハンドラー
    /// </summary>
    private void OnPlayerChanged(BallColor player)
    {
        UpdateHighlights();
    }
    
    /// <summary>
    /// ボールを作成
    /// </summary>
    private void CreateBall(PylosCoordinate coordinate, PlayerColor color)
    {
        // 色に応じてプレハブを選択
        GameObject prefabToUse = null;
        string prefabName = "";
        switch (color)
        {
            case PlayerColor.White:
                prefabToUse = whiteBallPrefab;
                prefabName = "WhiteBallPrefab";
                break;
            case PlayerColor.Black:
                prefabToUse = blackBallPrefab;
                prefabName = "BlackBallPrefab";
                break;
            default:
                Debug.LogError($"無効な色: {color}");
                return;
        }
        
        // プレハブが設定されていない場合は自動生成
        if (prefabToUse == null)
        {
            Debug.LogWarning($"ボールプレハブが設定されていません。色: {color} ({prefabName})。自動生成します。");
            prefabToUse = CreateDefaultBallPrefab(color);
        }
        else
        {
            Debug.Log($"ボール作成: 色={color}, プレハブ={prefabToUse.name}, 座標=({coordinate.X}, {coordinate.Y}, {coordinate.Z})");
        }
        
        if (prefabToUse == null)
        {
            Debug.LogError("ボールプレハブを作成できませんでした");
            return;
        }
        
        // プレハブからインスタンスを作成
        GameObject ballObj = Instantiate(prefabToUse, _ballsParent);
        ballObj.name = $"Ball_{coordinate.X}_{coordinate.Y}_{coordinate.Z}_{color}";
        
        // BallViewコンポーネントを取得
        BallView ballView = ballObj.GetComponent<BallView>();
        if (ballView == null)
        {
            ballView = ballObj.AddComponent<BallView>();
        }
        
        // ボールを初期化
        ballView.Initialize(coordinate, color);
        
        // 配置アニメーションを実行
        ballView.PlaceBall(coordinate, color);
        
        // 辞書に追加
        string key = GetCoordinateKey(coordinate);
        _ballViews[key] = ballView;
        
        // ハイライトを更新
        UpdateHighlights();
    }
    　
    /// <summary>
    /// デフォルトのボールプレハブを作成（プレハブが設定されていない場合）
    /// </summary>
    private GameObject CreateDefaultBallPrefab(PlayerColor color)
    {
        // Sphereを作成
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = Vector3.one * 0.8f;
        
        // Colliderを削除（ボール同士の衝突を防ぐため）
        Collider collider = sphere.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        // マテリアルを作成（複数のシェーダーを試行）
        Material mat = null;
        string[] shaderNames = new string[]
        {
            "Universal Render Pipeline/Lit",
            "Standard",
            "Unlit/Color"
        };
        
        foreach (string shaderName in shaderNames)
        {
            Shader shader = Shader.Find(shaderName);
            if (shader != null)
            {
                mat = new Material(shader);
                break;
            }
        }
        
        if (mat == null)
        {
            Debug.LogError("マテリアルを作成できませんでした");
            DestroyImmediate(sphere);
            return null;
        }
        
        // 色を設定
        if (color == PlayerColor.White)
        {
            mat.color = Color.white;
        }
        else if (color == PlayerColor.Black)
        {
            mat.color = Color.black;
        }
        
        // Rendererにマテリアルを設定
        Renderer renderer = sphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = mat;
        }
        
        return sphere;
    }
    
    /// <summary>
    /// アニメーション完了後にボールを削除
    /// </summary>
    private IEnumerator RemoveBallAfterAnimation(BallView ballView, string key)
    {
        // アニメーションが完了するまで待機
        while (ballView.IsAnimating())
        {
            yield return null;
        }
        
        // 辞書から削除
        if (_ballViews.ContainsKey(key))
        {
            _ballViews.Remove(key);
        }
        
        // GameObjectを削除
        if (ballView != null && ballView.gameObject != null)
        {
            Destroy(ballView.gameObject);
        }
    }
    
    /// <summary>
    /// 座標からキー文字列を生成
    /// </summary>
    private string GetCoordinateKey(PylosCoordinate coord)
    {
        return $"{coord.X}_{coord.Y}_{coord.Z}";
    }
    
    /// <summary>
    /// 座標からキー文字列を生成（オーバーロード）
    /// </summary>
    private string GetCoordinateKey(int x, int y, int z)
    {
        return $"{x}_{y}_{z}";
    }
    
    /// <summary>
    /// 盤面の状態を同期（初期化時やリセット時に使用）
    /// </summary>
    private void SyncBoardState()
    {
        if (presenter == null || presenter.GetBoardModel() == null) return;
        
        var board = presenter.GetBoardModel();
        
        // 全ての座標をチェック
        for (int z = 0; z < 4; z++)
        {
            int limit = 4 - z;
            for (int x = 0; x < limit; x++)
            {
                for (int y = 0; y < limit; y++)
                {
                    if (board.HasBall(x, y, z))
                    {
                        PlayerColor color = board.GetColor(x, y, z);
                        PylosCoordinate coord = new PylosCoordinate(x, y, z);
                        string key = GetCoordinateKey(coord);
                        
                        // 既に存在しない場合のみ作成
                        if (!_ballViews.ContainsKey(key))
                        {
                            CreateBall(coord, color);
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 指定座標のボールを取得
    /// </summary>
    public BallView GetBallView(int x, int y, int z)
    {
        string key = GetCoordinateKey(x, y, z);
        return _ballViews.ContainsKey(key) ? _ballViews[key] : null;
    }
    
    /// <summary>
    /// 全てのボールをクリア
    /// </summary>
    public void ClearAllBalls()
    {
        foreach (var ballView in _ballViews.Values)
        {
            if (ballView != null && ballView.gameObject != null)
            {
                Destroy(ballView.gameObject);
            }
        }
        _ballViews.Clear();
    }
    
    /// <summary>
    /// 4×4のグリッド線を描画
    /// </summary>
    private void DrawGridLines()
    {
        const int gridSize = 4; // 4×4のマス目
        const float cellSize = 1.0f; // 1マスのサイズ
        
        // 縦線（X方向に固定、Z方向に伸びる）
        for (int x = 0; x <= gridSize; x++)
        {
            float xPos = x * cellSize;
            Vector3 start = new Vector3(xPos, gridHeight, 0);
            Vector3 end = new Vector3(xPos, gridHeight, gridSize * cellSize);
            CreateGridLine($"GridLine_X_{x}", start, end);
        }
        
        // 横線（Z方向に固定、X方向に伸びる）
        for (int z = 0; z <= gridSize; z++)
        {
            float zPos = z * cellSize;
            Vector3 start = new Vector3(0, gridHeight, zPos);
            Vector3 end = new Vector3(gridSize * cellSize, gridHeight, zPos);
            CreateGridLine($"GridLine_Z_{z}", start, end);
        }
    }
    
    /// <summary>
    /// グリッド線を作成
    /// </summary>
    private void CreateGridLine(string name, Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(_gridParent);
        
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        
        // マテリアルの設定
        if (gridLineMaterial != null)
        {
            // Inspectorで設定されたマテリアルを使用
            lineRenderer.material = gridLineMaterial;
        }
        else
        {
            // デフォルトのマテリアルを作成（複数のシェーダーを試行）
            Material mat = CreateDefaultLineMaterial();
            if (mat != null)
            {
                lineRenderer.material = mat;
            }
            else
            {
                Debug.LogWarning("グリッド線用のマテリアルを作成できませんでした。Inspectorでマテリアルを設定してください。");
                Destroy(lineObj);
                return;
            }
        }
        
        // LineRendererの基本設定
        lineRenderer.startWidth = gridLineWidth;
        lineRenderer.endWidth = gridLineWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startColor = gridLineColor;
        lineRenderer.endColor = gridLineColor;
        
        // 影の設定（不要なので無効化）
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        
        // 位置を設定
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
    
    /// <summary>
    /// デフォルトのLineRenderer用マテリアルを作成
    /// </summary>
    private Material CreateDefaultLineMaterial()
    {
        // 複数のシェーダーを試行（URP対応）
        string[] shaderNames = new string[]
        {
            "Universal Render Pipeline/Unlit",  // URP用
            "Unlit/Color",                      // 標準のUnlit
            "Sprites/Default",                  // スプライト用
            "Standard"                          // 標準シェーダー
        };
        
        foreach (string shaderName in shaderNames)
        {
            Shader shader = Shader.Find(shaderName);
            if (shader != null)
            {
                Material mat = new Material(shader);
                mat.color = gridLineColor;
                return mat;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// 配置可能な場所をハイライト表示
    /// </summary>
    private void UpdateHighlights()
    {
        if (!showPlacementHighlights)
        {
            ClearHighlights();
            return;
        }
        
        if (presenter == null)
        {
            Debug.LogWarning("BoardView: Presenterが設定されていません");
            return;
        }
        
        // 既存のハイライトを削除
        ClearHighlights();
        
        // 設置フェーズの場合のみハイライトを表示
        PhaseState currentPhase = presenter.GetCurrentPhase();
        if (currentPhase != PhaseState.Placement)
        {
            Debug.Log($"BoardView: 設置フェーズではないためハイライトを表示しません。現在のフェーズ: {currentPhase}");
            return;
        }
        
        int highlightCount = 0;
        
        // 全てのマスをチェック
        for (int z = 0; z < 4; z++)
        {
            int limit = 4 - z;
            for (int x = 0; x < limit; x++)
            {
                for (int y = 0; y < limit; y++)
                {
                    if (presenter.CanPlaceAt(x, y, z))
                    {
                        CreateHighlight(x, y, z);
                        highlightCount++;
                    }
                }
            }
        }
        
        Debug.Log($"BoardView: {highlightCount}個のハイライトを作成しました");
    }
    
    /// <summary>
    /// ハイライトを作成
    /// </summary>
    private void CreateHighlight(int x, int y, int z)
    {
        string key = GetCoordinateKey(x, y, z);
        if (_highlights.ContainsKey(key)) return;
        
        // ハイライト用のQuadを作成
        GameObject highlight = GameObject.CreatePrimitive(PrimitiveType.Quad);
        highlight.name = $"Highlight_{x}_{y}_{z}";
        highlight.transform.SetParent(_highlightParent);
        
        // 位置を設定
        Vector3 worldPos = CoordinateConverter.ToWorldPosition(new PylosCoordinate(x, y, z));
        highlight.transform.position = new Vector3(worldPos.x, highlightHeight, worldPos.z);
        
        // 回転を設定（QuadはY軸を向くように）
        highlight.transform.rotation = Quaternion.Euler(90, 0, 0);
        
        // サイズを設定（マスより少し小さく）
        highlight.transform.localScale = Vector3.one * 0.9f;
        
        // Colliderを削除
        Collider collider = highlight.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }
        
        // マテリアルを作成（複数のシェーダーを試行）
        Material mat = null;
        string[] shaderNames = new string[]
        {
            "Universal Render Pipeline/Unlit",
            "Unlit/Color",
            "Sprites/Default",
            "Standard"
        };
        
        foreach (string shaderName in shaderNames)
        {
            Shader shader = Shader.Find(shaderName);
            if (shader != null)
            {
                mat = new Material(shader);
                break;
            }
        }
        
        if (mat == null)
        {
            Debug.LogWarning("ハイライト用のマテリアルを作成できませんでした");
            DestroyImmediate(highlight);
            return;
        }
        
        mat.color = highlightColor;
        
        // 透明度を有効にする（URPの場合）
        if (mat.HasProperty("_Surface"))
        {
            mat.SetFloat("_Surface", 1); // Transparent
        }
        if (mat.HasProperty("_Blend"))
        {
            mat.SetFloat("_Blend", 0); // Alpha
        }
        
        // Rendererにマテリアルを設定
        Renderer renderer = highlight.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = mat;
            // レンダリング順序を調整（他のオブジェクトの上に表示）
            if (renderer is SpriteRenderer spriteRenderer)
            {
                spriteRenderer.sortingOrder = 1;
            }
        }
        
        _highlights[key] = highlight;
    }
    
    /// <summary>
    /// 全てのハイライトを削除
    /// </summary>
    private void ClearHighlights()
    {
        foreach (var highlight in _highlights.Values)
        {
            if (highlight != null)
            {
                Destroy(highlight);
            }
        }
        _highlights.Clear();
    }
}

