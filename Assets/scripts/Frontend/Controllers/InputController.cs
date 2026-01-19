using UnityEngine;

public class InputController : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;           // カメラ
    public PylosGamePresenter presenter; // ★これが足りなかった！

    void Update()
    {
        // 左クリック (0) を押した瞬間
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        if (mainCamera == null || presenter == null) return;

        // マウスの位置からビームを飛ばす
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // ビームが何かに当たったら
        if (Physics.Raycast(ray, out hit))
        {
            // 当たった場所の座標を取得
            Vector3 hitPoint = hit.point;

            // Unityの座標(X, Z)を、ボードのマス目(x, y)に変換する計算
            // (1.0fはボールの間隔。BoardViewの設定と合わせる必要があります)
            int x = Mathf.RoundToInt(hitPoint.x / 1.0f);
            int y = Mathf.RoundToInt(hitPoint.z / 1.0f);

            // 高さ(Z)は、とりあえず一番下(0)から順に置いていく前提、
            // またはクリックした高さから判定するなどロジックが必要ですが、
            // まずは「一番下の段(0)に置く」か「Presenterに任せる」形で渡します。

            // ここでは簡易的に「クリックした位置に置く」命令を出します
            // ※本来は高さの計算も必要ですが、まずは平面(0段目)でテストしましょう
            presenter.TryPlaceBall(x, y, 0);
        }
    }
}