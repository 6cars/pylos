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

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var coord = CoordinateConverter.ToPylosCoordinate(hit.point);
            Debug.Log($"Clicked: L{coord.Level} ({coord.X},{coord.Y})");
            OnBoardClicked?.Invoke(coord);
        }
    }
}




