/// <summary>
/// Trạng thái của một ô trên bảng.
/// Thay thế magic numbers (0, 1, 2) bằng enum rõ nghĩa.
/// </summary>
public enum CellState
{
    Empty = 0,
    Hovered = 1,
    Placed = 2
}
