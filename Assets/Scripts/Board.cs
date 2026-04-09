using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Board : MonoBehaviour
    {
        public const int Size = 8;

        private const string BestScoreKey = "best_score";

        [SerializeField] private Cell cellPrefabs;
        [SerializeField] private Transform cellTransform;
        
        [Space(8.0f)]

        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text bestScoreText;

        [SerializeField] private SpriteRenderer backgroundRenderer;

        private readonly Cell[,] cells = new Cell[Size, Size];
        private readonly int[,] data = new int[Size, Size]; // 0 la empty, 1 la hover, 3 la normal

        private readonly List<Vector2Int> hoverPoints = new();
        private readonly List<int> hightlightPolyominoColumns = new();
        private readonly List<int> hightlightPolyominoRows = new();

        private readonly List<int> fullLineColumns = new();

        private readonly List<int> fullLineRows = new();

        private Vector2Int previousHoverPoint;

        private List<Vector2Int> previousHoverPoints = new();

        private int score;

        private int bestScore;
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
            score = 0;
            bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
            scoreText.text    = score.ToString();
            bestScoreText.text = bestScore.ToString();


           var blockCellWith = (float)Size / (Block.Size * 3 + 3 + 1); // Đổi Size thành size
            
            // Tính giá trị offset và truyền đủ 2 trục x, y cho Vector2
            var offset = new Vector2(0.25f + 0.5f, 0.25f + blockCellWith*8);
            var gameCamera = Camera.main.GetComponent<GameCamera>();
            // Sửa tên hàm thành ViewFrame và chỉ truyền 1 tham số Rect
            gameCamera.View(new Rect(-offset.x, -offset.y, Size + offset.x * 2.0f, Size + offset.y  + 0.25f),new(Size,Size));

        }





        public void Hover(Vector2Int point, int polymoinoIndex)
        {
            var polyomino = Polyominos.get(polymoinoIndex); // Sửa: Truyền đúng tham số polymoinoIndex
            var polyominoRows = polyomino.GetLength(0);
            var polyominoColumns = polyomino.GetLength(1);

            Unhover();

            Unhightlight();

            hightlightPolyominoColumns.Clear();

            hightlightPolyominoRows.Clear();
            HoverPoints(point, polyominoRows, polyominoColumns, polyomino);



            if (hoverPoints.Count > 0)
            {

                previousHoverPoint = point;
                previousHoverPoints.Clear();
                previousHoverPoints.AddRange(hoverPoints);

                Hover();
                Hightlight(point, polyominoColumns, polyominoRows);


            }
            else if (previousHoverPoints.Count > 0 && Mathf.Abs(point.x - previousHoverPoint.x) < 2 && Mathf.Abs(point.y - previousHoverPoint.y) < 2)
            {
                point = previousHoverPoint;
                hoverPoints.Clear();
                hoverPoints.AddRange(previousHoverPoints);

                Hover();
                Hightlight(point, polyominoColumns, polyominoRows);
            }
            else
            {
                previousHoverPoints.Clear();
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

            HoverPoints(point, polyominoRows, polyominoColumns, polyomino);

            if (hoverPoints.Count > 0) // Sửa: count thành Count
            {
                Place(point, polyominoColumns, polyominoRows);
                previousHoverPoints.Clear();
                return true;
            }
            else if (previousHoverPoints.Count > 0 && Mathf.Abs(point.x - previousHoverPoint.x) < 2 && Mathf.Abs(point.y - previousHoverPoint.y) < 2)
            {
                point = previousHoverPoint;
                hoverPoints.Clear();
                hoverPoints.AddRange(previousHoverPoints);

                Place(point, polyominoColumns, polyominoRows);
                previousHoverPoints.Clear();
                return true;
            }
            previousHoverPoints.Clear();

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
            FullLineRows(point.y, point.y + polyominoRows);

            AddScore(fullLineColumns.Count*Size + fullLineRows.Count*Size);

            ClearFullLineColumns();
            ClearFullLineRows();
        }
        private void FullLineColumns(int fromColumn, int topColumnExclusive)
        {
            fullLineColumns.Clear();
            for (var c = fromColumn; c < topColumnExclusive; ++c)
            {
                if (c < 0 || c >= Size) continue;
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
            for (var r = fromRow; r < topRowExclusive; ++r)
            {
                if (r < 0 || r >= Size) continue;
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
        private void ClearFullLineColumns()
        {
            foreach (var c in fullLineColumns)
            {
                for (var r = 0; r < Size; ++r)
                {
                    data[r, c] = 0;
                    cells[r, c].Hide();
                }
            }
        }
        private void ClearFullLineRows()
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
            foreach (var fullLineColumn in fullLineColumns)
            {
                hightlightPolyominoColumns.Add(fullLineColumn - point.x);
            }

            // LƯU Ý: Sửa fullLineColumns thành fullLineRows ở vòng lặp này
            foreach (var fullLineRow in fullLineRows)
            {
                hightlightPolyominoRows.Add(fullLineRow - point.y);
            }
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
                if (c < 0 || c >= Size) continue;
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
                if (r < 0 || r >= Size) continue;

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
                    fullLineRows.Add(r);
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

        public bool CheckPlace(int polyominoIndex)
        {
            var polyomino = Polyominos.get(polyominoIndex);
            var polyominoRows = polyomino.GetLength(0);
            var polyominoColumns = polyomino.GetLength(1);

            // Dùng <= để không bỏ sót hàng/cột cuối cùng cúa board
            for (var r = 0; r <= Size - polyominoRows; ++r)
            {
                for (var c = 0; c <= Size - polyominoColumns; ++c)
                {
                    if (CheckPlace(c, r, polyominoColumns, polyominoRows, polyomino))
                        return true;
                }
            }
            return false;
        }

        private bool CheckPlace(int column, int row, int polyominoColumns, int polyominoRows, int[,] polyomino)
        {
            for (var r = 0; r < polyominoRows; ++r)
            {
                for (var c = 0; c < polyominoColumns; ++c)
                {

                    if (polyomino[r, c] > 0 && data[row + r, column + c] == 2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void AddScore(int amount)
        {
            score += amount;
            if (score > bestScore)
            {
                bestScore = score;
                PlayerPrefs.SetInt(BestScoreKey, bestScore);
                PlayerPrefs.Save(); // Đảm bảo lưu ngay, tránh mất dữ liệu khi force-quit
            }
            scoreText.text     = score.ToString();
            bestScoreText.text = bestScore.ToString();
        }

        public List<int> HightlightPolyominoColumns => hightlightPolyominoColumns;
        public List<int> HightlightPolyominoRows => hightlightPolyominoRows;


    }
  
}