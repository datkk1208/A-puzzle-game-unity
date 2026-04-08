using System.Collections.Generic; // Thêm dòng này
using UnityEngine;

public class Board : MonoBehaviour
{
    public const int Size = 8;
    [SerializeField] private Cell cellPrefabs;
    [SerializeField] private Transform cellTransform;
    private readonly Cell[,] cells = new Cell[Size, Size];
    private readonly int[,] data = new int[Size, Size]; // 0 la empty, 1 la hover, 3 la normal

    private readonly List<Vector2Int> hoverPoints = new();

    private readonly List<int> fullLineColumns = new();

    private readonly List<int> fullLineRows = new();
    private void Start()
    {
        for (var r = 0; r < Size; ++r)
        {
            for (var c = 0; c < Size; ++c)
            {
                cells[r, c] = Instantiate(cellPrefabs, cellTransform);
                cells[r, c].transform.position = new(c + 0.5f, r + 0.5f, 0.0f);
                cells[r, c].Hide();
            }
        }
    }

    

   

    public void Hover(Vector2Int point, int polymoinoIndex)
    {
        var polyomino = Polyominos.get(polymoinoIndex); // Sửa: Truyền đúng tham số polymoinoIndex
        var polyominoRows = polyomino.GetLength(0);
        var polyominoColumns = polyomino.GetLength(1);

        Unhover();
        Unhightlight();
        HoverPoints(point, polyominoRows, polyominoColumns, polyomino);

        if (hoverPoints.Count > 0) // Sửa: count thành Count
        {
            Hover();
            Hightlight(point,polyominoColumns,polyominoRows);
        }
    }
    private void HoverPoints(Vector2Int point, int polyominoRows, int polyomioColumns, int[,] polyomino)
    {
        for (var r = 0; r < polyominoRows; ++r)
        {
            for (var c = 0; c < polyomioColumns; ++c)
            {
                if (polyomino[r, c] > 0)
                {
                    var hoverPoint = point + new Vector2Int(c, r);
                    if (IsValidPoint(hoverPoint) == false)
                    {
                        hoverPoints.Clear();
                        return;
                    }
                    hoverPoints.Add(hoverPoint); // Sửa: hoverPoints thay vì hoverPoint
                }
            }
        }
    }
    private bool IsValidPoint(Vector2Int point)
    {
        if (point.x < 0 || Size <= point.x) return false;
        if (point.y < 0 || Size <= point.y) return false; // Sửa: point.x thành point.y
        if (data[point.y, point.x] > 0) return false;

        return true;
    }

    private void Hover()
    {
        foreach (var hoverPoint in hoverPoints)
        {
            data[hoverPoint.y, hoverPoint.x] = 1; // Sửa: Đồng bộ data[y, x]
            cells[hoverPoint.y, hoverPoint.x].Hover();
        }
    }

    private void Unhover()
    {
        foreach (var hoverPoint in hoverPoints)
        {
            data[hoverPoint.y, hoverPoint.x] = 0;
            cells[hoverPoint.y, hoverPoint.x].Hide();
        }
        hoverPoints.Clear();
    }
    public bool Place(Vector2Int point, int polymoinoIndex)
    {
        var polyomino = Polyominos.get(polymoinoIndex); // Sửa: Truyền đúng tham số polymoinoIndex
        var polyominoRows = polyomino.GetLength(0);
        var polyominoColumns = polyomino.GetLength(1);

        Unhover();
        Unhightlight();
        HoverPoints(point, polyominoRows, polyominoColumns, polyomino);

        if (hoverPoints.Count > 0) // Sửa: count thành Count
        {
            Place(point, polyominoColumns, polyominoRows);
            Hightlight(point, polyominoRows, polyominoColumns);
            return true;
        }
        return false;
    }
    private void Place(Vector2Int point, int polyominoColumns, int polyominoRows)
    {
        foreach (var hoverPoint in hoverPoints)
        {
            data[hoverPoint.y, hoverPoint.x] = 2; // Sửa: Đồng bộ data[y, x]
            cells[hoverPoint.y, hoverPoint.x].Normal();
        }

        ClearFullLines(point, polyominoColumns, polyominoRows);

        hoverPoints.Clear();
    } 
    private void ClearFullLines(Vector2Int point, int polyominoColumns, int polyominoRows)

    {
        FullLineColumns(point.x, point.x + polyominoColumns);
        FullLineRows(point.y, point.y + polyominoColumns);

        ClearFullLineColumns();
        ClearFullLineRows();
    }    
    private void FullLineColumns(int fromColumn, int topColumnExclusive)
    {
        fullLineColumns.Clear();
        for(var c = fromColumn; c < topColumnExclusive; ++c)
        {
            var isFullLine = true;
            for (var r = 0; r < Size; ++r)
            {
                if (data[r, c] != 2)
                {
                    isFullLine = false;
                    break;
                }    
            }
            if (isFullLine == true)
            {
                fullLineColumns.Add(c);
            }    
        }    
    }    
     private void FullLineRows(int fromRow, int topRowExclusive)
    {
        fullLineRows.Clear();
        for(var r = fromRow; r < topRowExclusive; ++r)
        {
            var isFullLine = true;
            for (var c = 0; c < Size; ++c)
            {
                if (data[r, c] != 2)
                {
                    isFullLine = false;
                    break;
                }    
            }
            if (isFullLine == true)
            {
                fullLineRows.Add(r);
            }    
        }    
    }
   private void   ClearFullLineColumns()
    {
        foreach(var c in fullLineColumns)
        {
            for (var r = 0;r < Size; ++r)
            {
                data[r, c] = 0;
                cells[r,c].Hide();
            }    
        }    
}
  private void   ClearFullLineRows()
    {
        foreach (var r in fullLineRows)
        {
            for (var c = 0; c < Size; ++c)
            {
                data[r, c] = 0; 
                cells[r, c].Hide();
            }
        }
    }    

    private void Hightlight(Vector2Int point, int polyminoColumns, int polymominoRows)
    {
        PredictFullLineColumns(point.x, point.x + polyminoColumns);
        PredictFullLineRows(point.y, point.y + polymominoRows);

        HightlightFullLineColumns();
        HightlightFullLineRows();
    }
    private void Unhightlight()
    {
        UnhightlightFullLineColumns();
        UnhightlightFullLineRows();
    }
    private void PredictFullLineColumns(int fromColumn, int topColumnExclusive)
    {
        fullLineColumns.Clear();
        for (var c = fromColumn; c < topColumnExclusive; ++c)
        {
            var isFullLine = true;
            for (var r = 0; r < Size; ++r)
            {
                if (data[r, c] != 1 && data[r, c] != 2)
                {
                    isFullLine = false;
                    break;
                }
            }
            if (isFullLine == true)
            {
                fullLineColumns.Add(c);
            }
        }
    }
    private void PredictFullLineRows(int fromColumn, int topColumnExclusive)
    {
        fullLineRows.Clear();
        for (var r = fromColumn; r < topColumnExclusive; ++r)
        {
            var isFullLine = true;
            for (var c = 0; c < Size; ++c)
            {
                if ((data[r, c] != 1 && data[r, c] != 2))
                {
                    isFullLine = false;
                    break;
                }
            }
            if (isFullLine == true)
            {
                fullLineColumns.Add(r);
            }
        }
    }
    private void HightlightFullLineColumns()
    {
        foreach (var c in fullLineColumns)
        {
            for (var r = 0; r < Size; ++r)
            {
                if (data[r, c] == 2)
                {

                    cells[r, c].Hightlight();
                }
            }
        }
    }
    private void HightlightFullLineRows()
    {
        foreach (var r in fullLineRows)
        {
            for (var c = 0; c < Size; ++c)
            {
                if (data[r, c] == 2)
                {

                    cells[r, c].Hightlight();
                }
            }
        }
    }
    private void UnhightlightFullLineColumns()
    {
        foreach (var c in fullLineColumns)
        {
            for (var r = 0; r < Size; ++r)
            {
                if (data[r, c] == 2)
                {

                    cells[r, c].Normal();
                }
            }
        }
    }
    private void UnhightlightFullLineRows()
    {
        foreach (var r in fullLineRows)
        {
            for (var c = 0; c < Size; ++c)
            {
                if (data[r, c] == 2)
                {

                    cells[r, c].Normal();
                }
            }
        }
    }


}