using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý dữ liệu logic thuần của bảng game.
/// Không biết gì về rendering/Unity view — chỉ làm việc với mảng state.
/// Single Responsibility: chỉ quản lý trạng thái ô.
/// </summary>
public class BoardData
{
    private readonly CellState[,] cells;
    private readonly int size;

    public int Size => size;

    public BoardData(int size)
    {
        this.size = size;
        cells = new CellState[size, size];
    }

    public CellState GetState(int row, int col) => cells[row, col];

    public void SetState(int row, int col, CellState state)
    {
        cells[row, col] = state;
    }

    /// <summary>
    /// Tính toán các điểm mà hình khối sẽ chiếm nếu đặt tại origin.
    /// Trả về danh sách rỗng nếu không thể đặt (ngoài bảng hoặc ô đã có khối).
    /// </summary>
    public List<Vector2Int> CalculateShapePoints(Vector2Int origin, int[,] shape)
    {
        var points = new List<Vector2Int>();
        var rows = shape.GetLength(0);
        var columns = shape.GetLength(1);

        for (var r = 0; r < rows; ++r)
        {
            for (var c = 0; c < columns; ++c)
            {
                if (shape[r, c] > 0)
                {
                    var point = origin + new Vector2Int(c, r);

                    if (!IsValidEmptyPoint(point))
                    {
                        return new List<Vector2Int>(); // Không hợp lệ → trả về rỗng
                    }

                    points.Add(point);
                }
            }
        }

        return points;
    }

    /// <summary>
    /// Đặt hover state lên các điểm chỉ định.
    /// </summary>
    public void ApplyHover(List<Vector2Int> points)
    {
        foreach (var point in points)
        {
            cells[point.y, point.x] = CellState.Hovered;
        }
    }

    /// <summary>
    /// Xóa hover state khỏi các điểm chỉ định.
    /// </summary>
    public void ClearHover(List<Vector2Int> points)
    {
        foreach (var point in points)
        {
            cells[point.y, point.x] = CellState.Empty;
        }
    }

    /// <summary>
    /// Đặt khối (Placed) lên các điểm chỉ định.
    /// </summary>
    public void PlaceBlocks(List<Vector2Int> points)
    {
        foreach (var point in points)
        {
            cells[point.y, point.x] = CellState.Placed;
        }
    }

    /// <summary>
    /// Kiểm tra một điểm có nằm trong bảng VÀ đang trống không.
    /// </summary>
    private bool IsValidEmptyPoint(Vector2Int point)
    {
        if (point.x < 0 || point.x >= size) return false;
        if (point.y < 0 || point.y >= size) return false;
        if (cells[point.y, point.x] != CellState.Empty) return false;

        return true;
    }
}
