using UnityEngine;
using System.Collections.Generic;

public class BoardView : MonoBehaviour
{
    [Header("Settings")]
    // ★ここを変更：白と黒、別々のプレハブを受け取れるようにしました
    public GameObject whiteBallPrefab;
    public GameObject blackBallPrefab;

    public float ballSpacing = 1.0f;

    private Dictionary<string, GameObject> _activeBalls = new Dictionary<string, GameObject>();

    public void PlaceBallView(int x, int y, int z, BallColor color)
    {
        // ★色によって使うプレハブを切り替える
        GameObject prefabToUse = (color == BallColor.White) ? whiteBallPrefab : blackBallPrefab;

        if (prefabToUse == null)
        {
            Debug.LogError($"BoardView: {color}色のプレハブが設定されていません！インスペクターを確認してください。");
            return;
        }

        // 座標計算
        Vector3 position = new Vector3(x * ballSpacing, z * ballSpacing * 0.8f, y * ballSpacing);

        // 生成
        GameObject newBall = Instantiate(prefabToUse, position, Quaternion.identity);
        newBall.transform.SetParent(this.transform);

        // 管理リストに登録
        string key = $"{x},{y},{z}";
        if (_activeBalls.ContainsKey(key)) Destroy(_activeBalls[key]);
        _activeBalls[key] = newBall;
    }

    public void RemoveBallView(int x, int y, int z)
    {
        string key = $"{x},{y},{z}";
        if (_activeBalls.ContainsKey(key))
        {
            Destroy(_activeBalls[key]);
            _activeBalls.Remove(key);
        }
    }
}