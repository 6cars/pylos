using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pylos.Backend.Models;

[ExecuteAlways]
public class BoardView : MonoBehaviour
{
    private Dictionary<string, GameObject> spawnPyramidSlotObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> placedBallObjects = new Dictionary<string, GameObject>();

    private Material whiteMaterial;
    private Material blackMaterial;
    private Material slotMaterial;
    private Material boardMaterial;

    private void InitializeMaterials()
    {
        if (whiteMaterial != null) return;

        // マテリアルの動的作成
        // URP とビルトインパイプラインの両方に対応するため、適切なシェーダーを順に探索
        Shader standardShader = Shader.Find("Universal Render Pipeline/Lit");
        bool isURP = true;
        if (standardShader == null)
        {
            isURP = false;
            standardShader = Shader.Find("Standard");
        }
        if (standardShader == null)
        {
            standardShader = Shader.Find("Diffuse");
        }

        whiteMaterial = new Material(standardShader);
        whiteMaterial.color = Color.white;
        if (isURP)
        {
            whiteMaterial.SetFloat("_Smoothness", 0.8f);
        }
        else
        {
            whiteMaterial.SetFloat("_Glossiness", 0.8f);
        }

        blackMaterial = new Material(standardShader);
        blackMaterial.color = new Color(0.1f, 0.1f, 0.1f);
        if (isURP)
        {
            blackMaterial.SetFloat("_Smoothness", 0.8f);
        }
        else
        {
            blackMaterial.SetFloat("_Glossiness", 0.8f);
        }

        boardMaterial = new Material(standardShader);
        boardMaterial.color = new Color(0.5f, 0.35f, 0.2f); // 木製風の茶色
        if (isURP)
        {
            boardMaterial.SetFloat("_Smoothness", 0.2f);
        }
        else
        {
            boardMaterial.SetFloat("_Glossiness", 0.2f);
        }

        slotMaterial = new Material(standardShader);
        // 黄色い光るスロット（半透明、アルファ高め）
        slotMaterial.color = new Color(0.9f, 0.8f, 0.1f, 0.7f);
        
        if (isURP)
        {
            // URP用の半透明設定
            slotMaterial.SetFloat("_Surface", 1f); // Transparent
            slotMaterial.SetFloat("_Blend", 0f); // Alpha
            slotMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            slotMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            slotMaterial.SetInt("_ZWrite", 0);
            slotMaterial.DisableKeyword("_ALPHATEST_ON");
            slotMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            slotMaterial.renderQueue = 3000;
        }
        else
        {
            // Standard Shader の Fade モードを設定
            slotMaterial.SetFloat("_Mode", 2f); // 2 = Fade
            slotMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            slotMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            slotMaterial.SetInt("_ZWrite", 0);
            slotMaterial.DisableKeyword("_ALPHATEST_ON");
            slotMaterial.EnableKeyword("_ALPHABLEND_ON");
            slotMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            slotMaterial.renderQueue = 3000;
        }
    }

    private void Awake()
    {
        InitializeMaterials();
    }

    private void OnEnable()
    {
        InitializeMaterials();
        RecreateBoardAndSlots();
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            // エディタ上での生存確認と、必要に応じた再生成
            if (transform.childCount == 0)
            {
                RecreateBoardAndSlots();
            }
        }
    }

    public void RecreateBoardAndSlots()
    {
        // 既存の子オブジェクトをクリーンアップ
        CleanupChildren();

        InitializeMaterials();

        CreateBoardBase();
        CreateSlots();
    }

    private void CleanupChildren()
    {
        // エディタモードと実行モードの両方で安全に削除する
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        foreach (var child in children)
        {
            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }

        spawnPyramidSlotObjects.Clear();
        placedBallObjects.Clear();
    }

    private void CreateBoardBase()
    {
        GameObject boardBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boardBase.name = "BoardBase";
        boardBase.transform.parent = this.transform;
        
        // 土台のサイズ (size = 1.2fなので、4x4の範囲は 3.6f。余白を含め 5.2f 四方)
        boardBase.transform.localScale = new Vector3(5.2f, 0.3f, 5.2f);
        // 中心位置。CoordinateConverterでのピラミッドの中心座標に合わせる
        // ピラミッドの中心は、Level 0 の X=1.5, Y=1.5、すなわちワールド座標 (1.8, 0, 1.8)
        boardBase.transform.position = new Vector3(1.8f, -0.2f, 1.8f);
        boardBase.GetComponent<Renderer>().sharedMaterial = boardMaterial;
    }

    private void CreateSlots()
    {
        for (int l = 0; l < 4; l++)
        {
            int size = 4 - l;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    PylosCoordinate coord = new PylosCoordinate { Level = l, X = x, Y = y };
                    Vector3 pos = CoordinateConverter.ToWorldPosition(coord);

                    GameObject slot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    slot.name = $"Slot_L{l}_{x}_{y}";
                    slot.transform.parent = this.transform;
                    slot.transform.position = pos;
                    slot.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // スロットは小さめの球
                    slot.GetComponent<Renderer>().sharedMaterial = slotMaterial;
                    slot.SetActive(l == 0); // 最下層のみ最初から表示してチラつきと無表示を防ぐ

                    // コライダーをトリガーにして、Raycastが当たるようにする
                    var col = slot.GetComponent<Collider>();
                    if (col != null)
                    {
                        col.isTrigger = true;
                    }

                    string key = GetKey(coord);
                    spawnPyramidSlotObjects[key] = slot;
                }
            }
        }
    }

    public void PlaceBall(PylosCoordinate coord, BallColor color)
    {
        string key = GetKey(coord);
        if (placedBallObjects.TryGetValue(key, out GameObject existingBall))
        {
            RemoveBall(coord);
        }

        Vector3 pos = CoordinateConverter.ToWorldPosition(coord);
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = $"Ball_L{coord.Level}_{coord.X}_{coord.Y}_{color}";
        ball.transform.parent = this.transform;
        ball.transform.position = pos;
        ball.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // 置かれる球は標準サイズ (1.0)
        ball.GetComponent<Renderer>().sharedMaterial = (color == BallColor.White) ? whiteMaterial : blackMaterial;

        // BallViewスクリプトを追加
        BallView ballView = ball.AddComponent<BallView>();
        
        placedBallObjects[key] = ball;

        // スロットを非表示にする
        if (spawnPyramidSlotObjects.TryGetValue(key, out GameObject slot))
        {
            slot.SetActive(false);
        }
    }

    public void RemoveBall(PylosCoordinate coord)
    {
        string key = GetKey(coord);
        if (placedBallObjects.TryGetValue(key, out GameObject ball))
        {
            var ballView = ball.GetComponent<BallView>();
            if (ballView != null)
            {
                ballView.Remove();
            }
            else
            {
                if (Application.isPlaying)
                {
                    Destroy(ball);
                }
                else
                {
                    DestroyImmediate(ball);
                }
            }
            placedBallObjects.Remove(key);
        }

        // スロットを再表示
        if (spawnPyramidSlotObjects.TryGetValue(key, out GameObject slot))
        {
            slot.SetActive(true);
        }
    }

    public void UpdateSlotVisibility(BoardModel boardModel)
    {
        for (int l = 0; l < 4; l++)
        {
            int size = 4 - l;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    PylosCoordinate coord = new PylosCoordinate { Level = l, X = x, Y = y };
                    string key = GetKey(coord);
                    
                    if (!spawnPyramidSlotObjects.TryGetValue(key, out GameObject slot)) continue;

                    // すでに球が置かれている場合は非表示
                    if (boardModel.BallGrid[l, x, y] != BallColor.None)
                    {
                        slot.SetActive(false);
                        continue;
                    }

                    // 配置可能かどうかの判定
                    bool canPlace = false;
                    if (l == 0)
                    {
                        canPlace = true;
                    }
                    else
                    {
                        int underL = l - 1;
                        canPlace = 
                            boardModel.BallGrid[underL, x, y] != BallColor.None &&
                            boardModel.BallGrid[underL, x + 1, y] != BallColor.None &&
                            boardModel.BallGrid[underL, x, y + 1] != BallColor.None &&
                            boardModel.BallGrid[underL, x + 1, y + 1] != BallColor.None;
                    }

                    slot.SetActive(canPlace);
                }
            }
        }
    }

    private GameObject highlightedBallObj = null;
    private Color originalColor;

    public void HighlightBall(PylosCoordinate coord, bool highlight)
    {
        if (!highlight)
        {
            if (highlightedBallObj != null)
            {
                var renderer = highlightedBallObj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = originalColor;
                }
                highlightedBallObj = null;
            }
            return;
        }

        string key = GetKey(coord);
        if (placedBallObjects.TryGetValue(key, out GameObject ball))
        {
            if (highlightedBallObj != null)
            {
                HighlightBall(coord, false); // 前のハイライトをクリア
            }

            var renderer = ball.GetComponent<Renderer>();
            if (renderer != null)
            {
                highlightedBallObj = ball;
                originalColor = renderer.sharedMaterial.color;
                renderer.material.color = Color.red; // 赤色にハイライト
            }
        }
    }

    private string GetKey(PylosCoordinate coord)
    {
        return $"{coord.Level}_{coord.X}_{coord.Y}";
    }
}
