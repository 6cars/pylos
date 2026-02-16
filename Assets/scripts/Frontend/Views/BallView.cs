using UnityEngine;
using System.Collections;

// ボール単体の制御（移動アニメーションなど）
public class BallView : MonoBehaviour
{
    [Header("アニメーション設定")]
    [SerializeField] private float placementAnimationDuration = 0.5f; // 配置アニメーションの時間
    [SerializeField] private float recoveryAnimationDuration = 0.3f; // 回収アニメーションの時間
    [SerializeField] private float moveAnimationDuration = 0.5f; // 移動アニメーションの時間
    [SerializeField] private AnimationCurve placementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 配置アニメーションカーブ
    [SerializeField] private AnimationCurve recoveryCurve = AnimationCurve.EaseInOut(0, 1, 1, 0); // 回収アニメーションカーブ
    
    [Header("マテリアル参照")]
    [SerializeField] private Material whiteMaterial; // 白のマテリアル
    [SerializeField] private Material blackMaterial; // 黒のマテリアル
    
    [Header("コンポーネント参照")]
    [SerializeField] private Renderer ballRenderer; // ボールのRenderer（自動検索も可能）
    
    // ボールの座標と色
    private PylosCoordinate _coordinate;
    private PlayerColor _color;
    
    // アニメーション中かどうか
    private bool _isAnimating = false;
    
    private void Awake()
    {
        // Rendererが設定されていない場合は自動検索
        if (ballRenderer == null)
        {
            ballRenderer = GetComponent<Renderer>();
        }
        
        // 初期状態は非表示
        if (ballRenderer != null)
        {
            ballRenderer.enabled = false;
        }
    }
    
    /// <summary>
    /// ボールを初期化（座標と色を設定）
    /// </summary>
    public void Initialize(PylosCoordinate coordinate, PlayerColor color)
    {
        _coordinate = coordinate;
        _color = color;
        
        // 座標を設定
        Vector3 worldPos = CoordinateConverter.ToWorldPosition(coordinate);
        transform.position = worldPos;
        
        // 色に応じてマテリアルを設定
        SetColor(color);
        
        // 表示
        if (ballRenderer != null)
        {
            ballRenderer.enabled = true;
        }
    }
    
    /// <summary>
    /// ボールを配置する（出現アニメーション付き）
    /// </summary>
    public void PlaceBall(PylosCoordinate coordinate, PlayerColor color)
    {
        if (_isAnimating)
        {
            Debug.LogWarning("アニメーション中です");
            return;
        }
        
        _coordinate = coordinate;
        _color = color;
        
        // 色を設定
        SetColor(color);
        
        // 配置アニメーションを開始
        StartCoroutine(PlacementAnimation(coordinate));
    }
    
    /// <summary>
    /// ボールを回収する（消失アニメーション付き）
    /// </summary>
    public void RecoverBall()
    {
        if (_isAnimating)
        {
            Debug.LogWarning("アニメーション中です");
            return;
        }
        
        // 回収アニメーションを開始
        StartCoroutine(RecoveryAnimation());
    }
    
    /// <summary>
    /// ボールを移動する（移動アニメーション付き）
    /// </summary>
    public void MoveBall(PylosCoordinate newCoordinate)
    {
        if (_isAnimating)
        {
            Debug.LogWarning("アニメーション中です");
            return;
        }
        
        PylosCoordinate oldCoordinate = _coordinate;
        _coordinate = newCoordinate;
        
        // 移動アニメーションを開始
        StartCoroutine(MoveAnimation(oldCoordinate, newCoordinate));
    }
    
    /// <summary>
    /// 色を設定
    /// </summary>
    private void SetColor(PlayerColor color)
    {
        if (ballRenderer == null) return;
        
        Material materialToUse = null;
        switch (color)
        {
            case PlayerColor.White:
                materialToUse = whiteMaterial;
                break;
            case PlayerColor.Black:
                materialToUse = blackMaterial;
                break;
            default:
                // Noneの場合は非表示
                ballRenderer.enabled = false;
                return;
        }
        
        if (materialToUse != null)
        {
            ballRenderer.material = materialToUse;
            ballRenderer.enabled = true;
        }
    }
    
    /// <summary>
    /// 配置アニメーション（スケールアニメーション）
    /// </summary>
    private IEnumerator PlacementAnimation(PylosCoordinate coordinate)
    {
        _isAnimating = true;
        
        Vector3 targetPosition = CoordinateConverter.ToWorldPosition(coordinate);
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;
        
        transform.position = targetPosition;
        transform.localScale = startScale;
        
        if (ballRenderer != null)
        {
            ballRenderer.enabled = true;
        }
        
        float elapsed = 0f;
        while (elapsed < placementAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / placementAnimationDuration;
            float curveValue = placementCurve.Evaluate(t);
            
            transform.localScale = Vector3.Lerp(startScale, endScale, curveValue);
            
            yield return null;
        }
        
        transform.localScale = endScale;
        _isAnimating = false;
    }
    
    /// <summary>
    /// 回収アニメーション（スケールアニメーション）
    /// </summary>
    private IEnumerator RecoveryAnimation()
    {
        _isAnimating = true;
        
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        
        float elapsed = 0f;
        while (elapsed < recoveryAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / recoveryAnimationDuration;
            float curveValue = recoveryCurve.Evaluate(t);
            
            transform.localScale = Vector3.Lerp(startScale, endScale, curveValue);
            
            yield return null;
        }
        
        transform.localScale = endScale;
        
        if (ballRenderer != null)
        {
            ballRenderer.enabled = false;
        }
        
        _isAnimating = false;
    }
    
    /// <summary>
    /// 移動アニメーション
    /// </summary>
    private IEnumerator MoveAnimation(PylosCoordinate from, PylosCoordinate to)
    {
        _isAnimating = true;
        
        Vector3 startPosition = CoordinateConverter.ToWorldPosition(from);
        Vector3 endPosition = CoordinateConverter.ToWorldPosition(to);
        
        float elapsed = 0f;
        while (elapsed < moveAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveAnimationDuration;
            
            // スムーズな移動（イージング）
            t = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            
            yield return null;
        }
        
        transform.position = endPosition;
        _isAnimating = false;
    }
    
    /// <summary>
    /// 現在の座標を取得
    /// </summary>
    public PylosCoordinate GetCoordinate()
    {
        return _coordinate;
    }
    
    /// <summary>
    /// 現在の色を取得
    /// </summary>
    public PlayerColor GetColor()
    {
        return _color;
    }
    
    /// <summary>
    /// アニメーション中かどうか
    /// </summary>
    public bool IsAnimating()
    {
        return _isAnimating;
    }
}

