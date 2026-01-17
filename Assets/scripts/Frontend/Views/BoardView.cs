using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 盤面の3D表示管理
/// 
/// 手動設定方法：
/// - GameObjectを作成してこのコンポーネントをアタッチ
/// - InspectorでPresenterとボールプレハブを設定（または自動生成されます）
/// </summary>
public class BoardView : MonoBehaviour
{
    [Header("プレゼンター参照")]
    [SerializeField] private PylosGamePresenter presenter;
    
    [Header("ボールプレハブ")]
    [SerializeField] private GameObject whiteBallPrefab;
    [SerializeField] private GameObject blackBallPrefab;
    
    // 配置されたボールのGameObjectを管理
    private Dictionary<string, GameObject> _ballObjects = new Dictionary<string, GameObject>();
    
    private void Awake()
    {
        // プレゼンターが設定されていない場合は自動検索
        if (presenter == null)
        {
            presenter = FindObjectOfType<PylosGamePresenter>();
        }

        // プレハブが設定されていない場合は自動生成
        if (whiteBallPrefab == null)
        {
            whiteBallPrefab = CreateBallPrefab(PlayerColor.White);
        }
        if (blackBallPrefab == null)
        {
            blackBallPrefab = CreateBallPrefab(PlayerColor.Black);
        }
    }

    /// <summary>
    /// ボールプレハブを自動生成
    /// </summary>
    private GameObject CreateBallPrefab(PlayerColor color)
    {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = color == PlayerColor.White ? "WhiteBall" : "BlackBall";
        
        // マテリアルを作成して色を設定
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color == PlayerColor.White ? Color.white : Color.black;
        ball.GetComponent<Renderer>().material = mat;
        
        // スケールを調整（ボールのサイズ）
        ball.transform.localScale = Vector3.one * 0.8f;
        
        // Colliderを削除（盤面のColliderと干渉しないように）
        Collider collider = ball.GetComponent<Collider>();
        if (collider != null)
        {
            Object.Destroy(collider);
        }
        
        // 非アクティブにしてプレハブとして使用
        ball.SetActive(false);
        
        return ball;
    }
    
    private void OnEnable()
    {
        // イベントの購読
        GameEvents.OnBallPlaced += OnBallPlaced;
        GameEvents.OnBallRecovered += OnBallRecovered;
    }
    
    private void OnDisable()
    {
        // イベントの購読解除
        GameEvents.OnBallPlaced -= OnBallPlaced;
        GameEvents.OnBallRecovered -= OnBallRecovered;
    }
    
    /// <summary>
    /// ボールが配置された時のイベントハンドラー
    /// </summary>
    private void OnBallPlaced(int x, int y, int z, PlayerColor color)
    {
        CreateBall(x, y, z, color);
    }
    
    /// <summary>
    /// ボールが回収された時のイベントハンドラー
    /// </summary>
    private void OnBallRecovered(int x, int y, int z)
    {
        RemoveBall(x, y, z);
    }
    
    /// <summary>
    /// ボールのGameObjectを作成
    /// </summary>
    private void CreateBall(int x, int y, int z, PlayerColor color)
    {
        string key = GetBallKey(x, y, z);
        
        // 既に存在する場合は削除
        if (_ballObjects.ContainsKey(key))
        {
            Destroy(_ballObjects[key]);
            _ballObjects.Remove(key);
        }
        
        // プレハブを選択
        GameObject prefab = color == PlayerColor.White ? whiteBallPrefab : blackBallPrefab;
        if (prefab == null)
        {
            Debug.LogWarning($"ボールプレハブが設定されていません: {color}");
            return;
        }
        
        // ワールド座標に変換
        PylosCoordinate coord = new PylosCoordinate(x, y, z);
        Vector3 worldPos = CoordinateConverter.ToWorldPosition(coord);
        
        // ボールを生成
        GameObject ball = Instantiate(prefab, worldPos, Quaternion.identity, transform);
        _ballObjects[key] = ball;
    }
    
    /// <summary>
    /// ボールのGameObjectを削除
    /// </summary>
    private void RemoveBall(int x, int y, int z)
    {
        string key = GetBallKey(x, y, z);
        
        if (_ballObjects.ContainsKey(key))
        {
            Destroy(_ballObjects[key]);
            _ballObjects.Remove(key);
        }
    }
    
    /// <summary>
    /// ボールのキーを生成（座標から一意の文字列を生成）
    /// </summary>
    private string GetBallKey(int x, int y, int z)
    {
        return $"{x}_{y}_{z}";
    }
}

