using System.Collections.Generic;

/// <summary>
/// Chịu trách nhiệm phát hiện và xóa các hàng/cột đã đầy trên bảng.
/// Tách riêng từ Board để tuân thủ Single Responsibility Principle.
/// </summary>
public class LineClearHandler
{
    /// <summary>
    /// Kết quả sau khi kiểm tra xóa hàng/cột.
    /// </summary>
    public struct LineClearResult
    {
        public List<int> FullRows;
        public List<int> FullColumns;

        public bool HasClears => FullRows.Count > 0 || FullColumns.Count > 0;
    }

    /// <summary>
    /// Kiểm tra và xóa các hàng/cột đầy trong phạm vi ảnh hưởng của khối vừa đặt.
    /// Chỉ kiểm tra các hàng/cột mà khối vừa đặt chạm tới (tối ưu, không quét toàn bảng).
    /// </summary>
    public LineClearResult CheckAndClear(BoardData data, int fromCol, int toColExclusive, int fromRow, int toRowExclusive)
    {
        var result = new LineClearResult
        {
            FullRows = FindFullRows(data, fromRow, toRowExclusive),
            FullColumns = FindFullColumns(data, fromCol, toColExclusive)
        };

        if (result.HasClears)
        {
            ClearRows(data, result.FullRows);
            ClearColumns(data, result.FullColumns);
        }

        return result;
    }

    private List<int> FindFullColumns(BoardData data, int fromCol, int toColExclusive)
    {
        var fullColumns = new List<int>();

        for (var c = fromCol; c < toColExclusive; ++c)
        {
            var isFull = true;
            for (var r = 0; r < data.Size; ++r)
            {
                if (data.GetState(r, c) != CellState.Placed)
                {
                    isFull = false;
                    break;
                }
            }

            if (isFull)
            {
                fullColumns.Add(c);
            }
        }

        return fullColumns;
    }

    private List<int> FindFullRows(BoardData data, int fromRow, int toRowExclusive)
    {
        var fullRows = new List<int>();

        for (var r = fromRow; r < toRowExclusive; ++r)
        {
            var isFull = true;
            for (var c = 0; c < data.Size; ++c)
            {
                if (data.GetState(r, c) != CellState.Placed)
                {
                    isFull = false;
                    break;
                }
            }

            if (isFull)
            {
                fullRows.Add(r);
            }
        }

        return fullRows;
    }

    private void ClearRows(BoardData data, List<int> rows)
    {
        foreach (var r in rows)
        {
            for (var c = 0; c < data.Size; ++c)
            {
                data.SetState(r, c, CellState.Empty);
            }
        }
    }

    private void ClearColumns(BoardData data, List<int> columns)
    {
        foreach (var c in columns)
        {
            for (var r = 0; r < data.Size; ++r)
            {
                data.SetState(r, c, CellState.Empty);
            }
        }
    }
}
