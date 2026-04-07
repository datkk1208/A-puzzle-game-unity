/// <summary>
/// Dữ liệu tĩnh về các hình khối (polyomino).
/// Không kế thừa MonoBehaviour vì đây là pure data — không cần Unity lifecycle.
/// </summary>
public static class PolyominoData
{
    private static readonly int[][,] Shapes = new int[][,]
    {
        new int[,]
        {
            { 0, 0, 1 },
            { 0, 0, 1 },
            { 1, 1, 1 }
        }
    };

    static PolyominoData()
    {
        foreach (var shape in Shapes)
        {
            FlipVertically(shape);
        }
    }

    /// <summary>
    /// Lấy hình khối theo index.
    /// </summary>
    public static int[,] Get(int index) => Shapes[index];

    /// <summary>
    /// Tổng số hình khối có sẵn.
    /// </summary>
    public static int Count => Shapes.Length;

    /// <summary>
    /// Lật ngược hình khối theo chiều dọc (hàng trên ↔ hàng dưới).
    /// Cần thiết vì Unity dùng hệ tọa độ Y-up trong khi mảng 2D đếm từ trên xuống.
    /// </summary>
    private static void FlipVertically(int[,] shape)
    {
        var rows = shape.GetLength(0);
        var columns = shape.GetLength(1);

        for (var r = 0; r < rows / 2; ++r)
        {
            var mirrorRow = rows - 1 - r;
            for (var c = 0; c < columns; ++c)
            {
                (shape[r, c], shape[mirrorRow, c]) = (shape[mirrorRow, c], shape[r, c]);
            }
        }
    }
}
