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
        HoverPoints(point, polyominoRows, polyominoColumns, polyomino);

        if (hoverPoints.Count > 0) // Sửa: count thành Count
        {
            Hover();
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
            Place();
            Hightlight(point, polyominoRows, polyominoColumns);

        }
        return false;
    }
    private void Place()
    {
        foreach (var hoverPoint in hoverPoints)
        {
            data[hoverPoint.y, hoverPoint.x] = 2; // Sửa: Đồng bộ data[y, x]
            cells[hoverPoint.y, hoverPoint.x].Normal();
        }
        hoverPoints.Clear();
    } 

    private void Hightlight(Vector2Int point, int polyminoColumns, int polymominoRows)
    {
       
    }
    private void Unhightlight()
    {
        //abc
        //abc1
    }    
}