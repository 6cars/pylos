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

/// <summary>
/// 入力（マウスクリック）を検知してゲームに伝える
/// 
/// このクラスはGameInitializerに依存しません。
/// 手動設定でも動作します：
/// - GameObjectを作成してこのコンポーネントをアタッチ
/// - InspectorでMain CameraとPresenterを設定（または自動検索されます）
/// </summary>
public class InputController : MonoBehaviour
{
    public Action<PylosCoordinate> OnBoardClicked;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private PylosGamePresenter presenter;

    void Start()
    {
        // カメラが設定されていない場合は自動検索
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
        }

        // プレゼンターが設定されていない場合は自動検索
        if (presenter == null)
        {
            presenter = FindObjectOfType<PylosGamePresenter>();
        }
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var coord = CoordinateConverter.ToPylosCoordinate(hit.point);
            Debug.Log($"Clicked: L{coord.Level} ({coord.X},{coord.Y})");
            
            // イベントを発火
            OnBoardClicked?.Invoke(coord);
            
            // プレゼンターに直接処理を依頼
            if (presenter != null)
            {
                PhaseState phase = presenter.GetCurrentPhase();
                if (phase == PhaseState.Placement)
                {
                    presenter.TryPlaceBall(coord.X, coord.Y, coord.Level);
                }
                else if (phase == PhaseState.Retrieval)
                {
                    presenter.TryRecoverBall(coord.X, coord.Y, coord.Level);
                }
            }
        }
    }
}




