using UnityEngine;

// Pylos座標とUnity(Vector3)座標の変換
public static class CoordinateConverter
{
    // グリッドのサイズ（1マスのサイズ）
    // 実際のゲームの設定に合わせて調整してください
    private const float GridSize = 1.0f;
    
    // 盤面の原点オフセット（必要に応じて調整）
    private static Vector3 BoardOrigin = Vector3.zero;
    
    /// <summary>
    /// Unityのワールド座標（Vector3）をPylos座標に変換
    /// </summary>
    public static PylosCoordinate ToPylosCoordinate(Vector3 worldPosition)
    {
        // 原点からの相対位置を計算
        Vector3 relativePos = worldPosition - BoardOrigin;
        
        // グリッド座標に変換（四捨五入）
        int x = Mathf.RoundToInt(relativePos.x / GridSize);
        int y = Mathf.RoundToInt(relativePos.z / GridSize); // UnityではZが奥行き
        int z = Mathf.RoundToInt(relativePos.y / GridSize); // UnityではYが高さ（レベル）
        
        return new PylosCoordinate(x, y, z);
    }
    
    /// <summary>
    /// Pylos座標をUnityのワールド座標に変換
    /// </summary>
    public static Vector3 ToWorldPosition(PylosCoordinate coord)
    {
        float x = coord.X * GridSize;
        float y = coord.Z * GridSize; // Zがレベル（高さ）
        float z = coord.Y * GridSize; // Yが奥行き
        
        return BoardOrigin + new Vector3(x, y, z);
    }
}
