/*using UnityEngine;

// クリック検知など
public class InputController : MonoBehaviour
{
    // TODO: 実装
}*/
//ここから実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    public Action<PylosCoordinate> OnBoardClicked;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private PylosGamePresenter presenter;

    private void Awake()
    {
        Debug.Log("InputController: 初期化開始");
        
        // プレゼンターが設定されていない場合は自動検索
        if (presenter == null)
        {
            presenter = FindObjectOfType<PylosGamePresenter>();
        }
        
        if (presenter == null)
        {
            Debug.LogError("InputController: PylosGamePresenterが見つかりません！");
        }
        else
        {
            Debug.Log("InputController: PylosGamePresenterを検出しました");
        }
        
        // カメラが設定されていない場合は自動検索
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera == null)
        {
            Debug.LogError("InputController: カメラが見つかりません！");
        }
        else
        {
            Debug.Log($"InputController: カメラを検出しました - {mainCamera.name}");
        }
    }
    
    private void Start()
    {
        Debug.Log("InputController: Start() が呼ばれました");
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // クリック位置からマス（x, y）を取得
            Vector3 relativePos = hit.point;
            int x = Mathf.RoundToInt(relativePos.x / 1.0f);
            int y = Mathf.RoundToInt(relativePos.z / 1.0f); // UnityではZが奥行き
            
            // 範囲チェック
            if (x < 0 || x > 3 || y < 0 || y > 3)
            {
                Debug.Log($"クリック位置が盤面外: ({x}, {y})");
                return;
            }
            
            // プレゼンターに処理を委譲
            if (presenter != null)
            {
                PhaseState currentPhase = presenter.GetCurrentPhase();
                
                if (currentPhase == PhaseState.Placement)
                {
                    // 設置フェーズ: そのマスで置ける最も低いレベルを自動選択
                    int bestLevel = FindBestPlacementLevel(presenter, x, y);
                    if (bestLevel >= 0)
                    {
                        Debug.Log($"配置試行: ({x}, {y}, {bestLevel})");
                        presenter.TryPlaceBall(x, y, bestLevel);
                    }
                    else
                    {
                        Debug.Log($"配置不可: ({x}, {y}) - このマスには置けません");
                    }
                }
                else if (currentPhase == PhaseState.Retrieval)
                {
                    // 回収フェーズ: そのマスの最高レベルのボールを回収
                    int topLevel = FindTopLevel(presenter, x, y);
                    if (topLevel >= 0)
                    {
                        Debug.Log($"回収試行: ({x}, {y}, {topLevel})");
                        presenter.TryRecoverBall(x, y, topLevel);
                    }
                    else
                    {
                        Debug.Log($"回収不可: ({x}, {y}) - このマスにボールがありません");
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 指定マス（x, y）で置ける最も低いレベルを探す
    /// </summary>
    private int FindBestPlacementLevel(PylosGamePresenter presenter, int x, int y)
    {
        // z=0から順に試して、最初に置けるレベルを返す
        for (int z = 0; z < 4; z++)
        {
            // そのレベルでの範囲チェック
            int limit = 4 - z;
            if (x >= limit || y >= limit)
            {
                // このレベルでは範囲外なので、それ以上はチェックしない
                Debug.Log($"配置判定: ({x}, {y}) はレベル{z}では範囲外 (limit={limit})");
                break;
            }
            
            // ルールチェック
            bool canPlace = presenter.CanPlaceAt(x, y, z);
            Debug.Log($"配置判定: ({x}, {y}, {z}) = {canPlace}");
            
            if (canPlace)
            {
                Debug.Log($"配置可能なレベルを発見: ({x}, {y}, {z})");
                return z;
            }
        }
        
        Debug.Log($"配置不可: ({x}, {y}) には置けるレベルが見つかりませんでした");
        return -1; // 置けるレベルが見つからない
    }
    
    /// <summary>
    /// 指定マス（x, y）の最高レベルを取得
    /// </summary>
    private int FindTopLevel(PylosGamePresenter presenter, int x, int y)
    {
        var board = presenter.GetBoardModel();
        if (board == null) return -1;
        
        // 上から順にチェックして、最初に見つかったボールのレベルを返す
        for (int z = 3; z >= 0; z--)
        {
            int limit = 4 - z;
            if (x >= limit || y >= limit) continue;
            
            if (board.HasBall(x, y, z))
            {
                return z;
            }
        }
        
        return -1; // ボールが見つからない
    }
}




