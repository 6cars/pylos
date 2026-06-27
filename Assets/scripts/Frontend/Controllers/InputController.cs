using System;
using UnityEngine;
using Pylos.Backend.Models;

public class InputController : MonoBehaviour
{
    public Action<PylosCoordinate> OnBoardClicked;

    [SerializeField] private Camera mainCamera;

    private Vector3 mousePressPosition;
    private const float ClickDragThreshold = 10f; // ドラッグとみなす移動距離（ピクセル）

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePressPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            // UI上のクリックはゲーム盤への配置処理を無視する
            if (IsPointerOverUI()) return;

            float dragDistance = Vector3.Distance(Input.mousePosition, mousePressPosition);
            if (dragDistance < ClickDragThreshold)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    var coord = CoordinateConverter.ToPylosCoordinate(hit.point);
                    Debug.Log($"Clicked: L{coord.Level} ({coord.X},{coord.Y})");
                    OnBoardClicked?.Invoke(coord);
                }
            }
        }
    }

    private bool IsPointerOverUI()
    {
        Vector3 mPos = Input.mousePosition; // 左下が(0,0)
        float x = mPos.x;
        float y = mPos.y;

        float sh = Screen.height;
        float sw = Screen.width;

        // 左上情報パネル＆ボタンエリアの判定範囲 (X: 20..340, Y: 20..260)
        if (x >= 20 && x <= 340 && y >= (sh - 260) && y <= sh)
        {
            return true;
        }

        // 右上カメラボタンエリアの判定範囲 (X: sw-170..sw-10, Y: 20..150)
        if (x >= (sw - 170) && x <= sw && y >= (sh - 150) && y <= sh)
        {
            return true;
        }

        return false;
    }
}
