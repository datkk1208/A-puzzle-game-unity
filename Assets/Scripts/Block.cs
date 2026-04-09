using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Rendering;


namespace Game
{

    public class Block : MonoBehaviour
    {
        public const int size = 5;
        private readonly Vector3 inputOffset = new(0.0f, 2.0f, 0.0f);
        [SerializeField] private Cell cellPrefab;
        [SerializeField] private Board board;
        [SerializeField] private Blocks blocks;
        private SortingGroup sortingGroup;
        private int polyominoIndex;
        private readonly Cell[,] cells = new Cell[size, size];

        private Vector3 position;
        private Vector3 scale;
        private Vector2 inputPoint;
        private Vector3 previousMousePosition = Vector3.positiveInfinity;
        private Vector2Int previousDragPoint;
        private Vector2Int currentDragPoint;
        //cache
        private Camera mainCamera;
        private Vector2 center;
        private void Awake()
        {
            mainCamera = Camera.main;
            sortingGroup = gameObject.GetComponent<SortingGroup>();
        }
        public void Initialize()
        {
            for (var r = 0; r < size; ++r)
            {
                for (var c = 0; c < size; ++c)
                {
                    cells[r, c] = Instantiate(cellPrefab, transform);
                }
            }
            position = transform.localPosition;
            scale = transform.localScale;
        }
        public void Show(int polyominoIndex)
        {
            this.polyominoIndex = polyominoIndex;
            Hide();
            var polyomino = Polyominos.get(polyominoIndex);
            var polyominoRows = polyomino.GetLength(0);
            var polyominosColums = polyomino.GetLength(1);
            center = new Vector2(polyominosColums * 0.5f, polyominoRows * 0.5f);
            for (var r = 0; r < polyominoRows; ++r)
            {

                for (var c = 0; c < polyominosColums; ++c)
                {
                    if (polyomino[r, c] > 0)
                    {
                        cells[r, c].transform.localPosition = new(c - center.x + 0.5f, r - center.y + 0.5f, 0.0f);
                        cells[r, c].Normal();
                    }
                }
            }

        }
        private void Hide()
        {
            for (var r = 0; r < size; ++r)
            {
                for (var c = 0; c < size; ++c)
                {
                    cells[r, c].Hide();
                }
            }
        }
        private void OnMouseDown()
        {
            //Debug.Log("OnMouseDown");
            inputPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            transform.localPosition = position + inputOffset;
            transform.localScale = Vector3.one;

            blocks.ResetBlockSortingOrders();
            SetSortingOrder(1);

            currentDragPoint = Vector2Int.RoundToInt((Vector2)transform.position - center);
            board.Hover(currentDragPoint, polyominoIndex);

            Hightlight(board.HightlightPolyominoColumns, board.HightlightPolyominoRows);

            previousDragPoint = currentDragPoint;


            previousMousePosition = Input.mousePosition;
        }
        private void OnMouseDrag()
        {
            var currentMousePosition = Input.mousePosition;
            if (currentMousePosition != previousMousePosition)
            {
                previousMousePosition = currentMousePosition;
                //Debug.Log("OnMouseDrag");
                var inputDelta = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition) - inputPoint;
                transform.localPosition = position + inputOffset + (Vector3)inputDelta * 1f;
                currentDragPoint = Vector2Int.RoundToInt((Vector2)transform.position - center);
                if (currentDragPoint != previousDragPoint)
                {
                    previousDragPoint = currentDragPoint;
                    //goi board de cap nhat

                    board.Hover(currentDragPoint, polyominoIndex);

                    Hightlight(board.HightlightPolyominoColumns, board.HightlightPolyominoRows);
                }
            }
        }
        private void OnMouseUp()
        {
            previousMousePosition = Vector3.positiveInfinity;
            currentDragPoint = Vector2Int.RoundToInt((Vector2)transform.position - center);
            if (board.Place(currentDragPoint, polyominoIndex) == true)
            {
                gameObject.SetActive(false);
                blocks.Remove();
            }

            transform.localPosition = position;
            transform.localScale = scale;


        }
        private void Hightlight(List<int> columns, List<int> rows)
        {
            var polyomino = Polyominos.get(polyominoIndex);
            var polyominoRows = polyomino.GetLength(0);
            var polyominoColumns = polyomino.GetLength(1);

            Unhightlight(polyominoColumns, polyominoRows, polyomino);

            HightlightColumns(polyominoRows, polyomino, columns);


            HightlightRows(polyominoColumns, polyomino, rows);
        }

        private void Unhightlight(int polyominoColumns, int polyominoRows, int[,] polyomino)
        {
            for (var r = 0; r < polyominoRows; r++)
            {
                for (var c = 0; c < polyominoColumns; c++)
                {
                    if (polyomino[r, c] > 0)
                    {
                        cells[r, c].Normal();
                    }
                }
            }
        }
        private void HightlightColumns(int polyominoRows, int[,] polyomino, List<int> columns)
        {
            foreach (var c in columns)
            {
                for (var r = 0; r < polyominoRows; ++r)
                {
                    if (polyomino[r, c] > 0)
                    {
                        cells[r, c].Hightlight();
                    }
                }
            }
        }
        private void HightlightRows(int polyominoColumns, int[,] polyomino, List<int> rows)
        {
            foreach (var r in rows)
            {
                for (var c = 0; c < polyominoColumns; ++c)
                {
                    if (polyomino[r, c] > 0)
                    {
                        cells[r, c].Hightlight();
                    }
                }
            }
        }
        public void SetSortingOrder(int sortingOrder)
        {
            sortingGroup.sortingOrder = sortingOrder;
        }
    }
}