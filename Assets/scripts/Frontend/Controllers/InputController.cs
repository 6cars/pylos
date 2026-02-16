using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    public Action<PylosCoordinate> OnBoardClicked;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private PylosGamePresenter presenter;

    private void Awake()
    {
        if (presenter == null)
        {
            presenter = FindObjectOfType<PylosGamePresenter>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (mainCamera == null || presenter == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        if (hits == null || hits.Length == 0) return;

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (TryHandleBallHit(hit)) return;
            if (TryHandleLevelSurfaceHit(hit)) return;
        }

        // 盤面メッシュなどへのフォールバック（1段目）
        TryHandleBoardHit(hits[0]);
    }

    private bool TryHandleBallHit(RaycastHit hit)
    {
        BallView ballView = hit.collider.GetComponentInParent<BallView>();
        if (ballView == null) return false;

        PylosCoordinate coord = ballView.GetCoordinate();
        PhaseState phase = presenter.GetCurrentPhase();

        if (phase == PhaseState.Placement)
        {
            // ボールの真上（1段上）への配置を優先
            int aboveLevel = coord.Z + 1;
            if (CoordinateConverter.IsInsideLevel(coord.X, coord.Y, aboveLevel) &&
                presenter.CanPlaceAt(coord.X, coord.Y, aboveLevel))
            {
                TryPlace(coord.X, coord.Y, aboveLevel);
                return true;
            }

            // 同じマスで置ける段があればそこへ
            if (TryPlaceAtColumn(coord.X, coord.Y, preferredLevel: coord.Z))
            {
                return true;
            }
        }
        else if (phase == PhaseState.Retrieval)
        {
            TryRecover(coord.X, coord.Y, coord.Z);
            return true;
        }

        return false;
    }

    private bool TryHandleLevelSurfaceHit(RaycastHit hit)
    {
        BoardLevelSurface surface = hit.collider.GetComponent<BoardLevelSurface>();
        if (surface == null) return false;

        var (x, y) = CoordinateConverter.ToBoardCell(hit.point);
        int level = surface.Level;

        if (!CoordinateConverter.IsInsideLevel(x, y, level)) return false;

        PhaseState phase = presenter.GetCurrentPhase();
        if (phase == PhaseState.Placement)
        {
            if (presenter.CanPlaceAt(x, y, level))
            {
                TryPlace(x, y, level);
                return true;
            }

            // クリックした段に置けない場合、同じ列の有効な段を探す
            return TryPlaceAtColumn(x, y, preferredLevel: level);
        }

        if (phase == PhaseState.Retrieval)
        {
            int topLevel = FindTopLevel(x, y);
            if (topLevel >= 0)
            {
                TryRecover(x, y, topLevel);
                return true;
            }
        }

        return false;
    }

    private void TryHandleBoardHit(RaycastHit hit)
    {
        var (x, y) = CoordinateConverter.ToBoardCell(hit.point);
        if (!CoordinateConverter.IsInsideLevel(x, y, 0)) return;

        PhaseState phase = presenter.GetCurrentPhase();
        if (phase == PhaseState.Placement)
        {
            TryPlaceAtColumn(x, y, preferredLevel: 0);
        }
        else if (phase == PhaseState.Retrieval)
        {
            int topLevel = FindTopLevel(x, y);
            if (topLevel >= 0)
            {
                TryRecover(x, y, topLevel);
            }
        }
    }

    /// <summary>
    /// 指定列 (x, y) で置ける段を探して配置。preferredLevel から上下に探索。
    /// </summary>
    private bool TryPlaceAtColumn(int x, int y, int preferredLevel)
    {
        if (presenter.CanPlaceAt(x, y, preferredLevel))
        {
            TryPlace(x, y, preferredLevel);
            return true;
        }

        for (int offset = 1; offset < 4; offset++)
        {
            int above = preferredLevel + offset;
            if (CoordinateConverter.IsInsideLevel(x, y, above) && presenter.CanPlaceAt(x, y, above))
            {
                TryPlace(x, y, above);
                return true;
            }

            int below = preferredLevel - offset;
            if (below >= 0 && presenter.CanPlaceAt(x, y, below))
            {
                TryPlace(x, y, below);
                return true;
            }
        }

        return false;
    }

    private void TryPlace(int x, int y, int z)
    {
        presenter.TryPlaceBall(x, y, z);
        OnBoardClicked?.Invoke(new PylosCoordinate(x, y, z));
    }

    private void TryRecover(int x, int y, int z)
    {
        presenter.TryRecoverBall(x, y, z);
        OnBoardClicked?.Invoke(new PylosCoordinate(x, y, z));
    }

    private int FindTopLevel(int x, int y)
    {
        var board = presenter.GetBoardModel();
        if (board == null) return -1;

        for (int z = 3; z >= 0; z--)
        {
            if (!CoordinateConverter.IsInsideLevel(x, y, z)) continue;
            if (board.HasBall(x, y, z)) return z;
        }

        return -1;
    }
}
