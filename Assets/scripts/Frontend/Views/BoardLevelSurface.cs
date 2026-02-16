using UnityEngine;

/// <summary>
/// 各段のクリック判定用（見えない当たり判定）
/// </summary>
public class BoardLevelSurface : MonoBehaviour
{
    [SerializeField] private int level;

    public int Level => level;

    public void Initialize(int z)
    {
        level = z;
    }
}
