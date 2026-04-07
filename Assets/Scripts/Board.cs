using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Orchestrator: kết nối BoardData (logic) với Cell grid (view) và LineClearHandler.
/// Không tự xử lý logic hay data — chỉ điều phối giữa các thành phần.
/// </summary>
public class Board : MonoBehaviour
{
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private Transform cellContainer;

    private BoardData boardData;
    private LineClearHandler lineClearHandler;
    private Cell[,] cellViews;

    private readonly List<Vector2Int> currentHoverPoints = new();

    private void Start()
    {
        boardData = new BoardData(GameConstants.BoardSize);
        lineClearHandler = new LineClearHandler();
        cellViews = new Cell[GameConstants.BoardSize, GameConstants.BoardSize];

        InitializeCellViews();
    }

    /// <summary>
    /// Tạo grid Cell prefab cho rendering.
    /// </summary>
    private void InitializeCellViews()
    {
        for (var r = 0; r < GameConstants.BoardSize; ++r)
        {
            for (var c = 0; c < GameConstants.BoardSize; ++c)
            {
                cellViews[r, c] = Instantiate(cellPrefab, cellContainer);
                cellViews[r, c].transform.position = new Vector3(
                    c + GameConstants.CellPositionOffset,
                    r + GameConstants.CellPositionOffset,
                    0.0f
                );
                cellViews[r, c].Hide();
            }
        }
    }

    // ─────────────────────────────────────────────
    // Hover: hiển thị preview khi kéo khối lên bảng
    // ─────────────────────────────────────────────

    /// <summary>
    /// Hiển thị preview (hover) của khối tại vị trí point trên bảng.
    /// </summary>
    public void Hover(Vector2Int point, int polyominoIndex)
    {
        var shape = PolyominoData.Get(polyominoIndex);

        ClearCurrentHover();

        var newPoints = boardData.CalculateShapePoints(point, shape);

        if (newPoints.Count > 0)
        {
            currentHoverPoints.AddRange(newPoints);
            boardData.ApplyHover(currentHoverPoints);
            SyncViewsForPoints(currentHoverPoints);
        }
    }

    /// <summary>
    /// Xóa hover hiện tại trên cả data và view.
    /// </summary>
    private void ClearCurrentHover()
    {
        if (currentHoverPoints.Count == 0) return;

        boardData.ClearHover(currentHoverPoints);
        SyncViewsForPoints(currentHoverPoints);
        currentHoverPoints.Clear();
    }

    // ─────────────────────────────────────────────
    // Place: đặt khối lên bảng
    // ─────────────────────────────────────────────

    /// <summary>
    /// Thử đặt khối tại vị trí point. Trả về true nếu đặt thành công.
    /// </summary>
    public bool Place(Vector2Int point, int polyominoIndex)
    {
        var shape = PolyominoData.Get(polyominoIndex);
        var shapeRows = shape.GetLength(0);
        var shapeColumns = shape.GetLength(1);

        ClearCurrentHover();

        var placePoints = boardData.CalculateShapePoints(point, shape);

        if (placePoints.Count == 0) return false;

        // Đặt khối lên data
        boardData.PlaceBlocks(placePoints);
        SyncViewsForPoints(placePoints);

        // Kiểm tra và xóa hàng/cột đầy
        var clearResult = lineClearHandler.CheckAndClear(
            boardData,
            point.x, point.x + shapeColumns,
            point.y, point.y + shapeRows
        );

        if (clearResult.HasClears)
        {
            SyncViewsForClearedLines(clearResult);
        }

        return true;
    }

    // ─────────────────────────────────────────────
    // View sync: đồng bộ Cell view theo BoardData
    // ─────────────────────────────────────────────

    /// <summary>
    /// Cập nhật visual cho một danh sách điểm cụ thể.
    /// </summary>
    private void SyncViewsForPoints(List<Vector2Int> points)
    {
        foreach (var point in points)
        {
            var state = boardData.GetState(point.y, point.x);
            cellViews[point.y, point.x].UpdateVisual(state);
        }
    }

    /// <summary>
    /// Cập nhật visual cho tất cả ô trong các hàng/cột vừa bị xóa.
    /// </summary>
    private void SyncViewsForClearedLines(LineClearHandler.LineClearResult clearResult)
    {
        foreach (var r in clearResult.FullRows)
        {
            for (var c = 0; c < GameConstants.BoardSize; ++c)
            {
                cellViews[r, c].UpdateVisual(CellState.Empty);
            }
        }

        foreach (var c in clearResult.FullColumns)
        {
            for (var r = 0; r < GameConstants.BoardSize; ++r)
            {
                cellViews[r, c].UpdateVisual(CellState.Empty);
            }
        }
    }
}