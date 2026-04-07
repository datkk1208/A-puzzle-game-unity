using UnityEngine;

/// <summary>
/// Đại diện cho một khối hình có thể kéo thả.
/// Chịu trách nhiệm: nhận input chuột + hiển thị khối preview.
/// </summary>
public class Block : MonoBehaviour
{
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private Board board;
    [SerializeField] private BlockSpawner spawner;

    private int polyominoIndex;
    private readonly Cell[,] cells = new Cell[GameConstants.BlockPreviewSize, GameConstants.BlockPreviewSize];

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Vector2 inputStartWorldPoint;
    private Vector3 previousMousePosition = Vector3.positiveInfinity;
    private Vector2Int previousDragPoint;
    private Vector2Int currentDragPoint;
    private Vector2 shapeCenter;

    private Camera mainCamera;

    private static readonly Vector3 InputOffset = new(0.0f, GameConstants.InputYOffset, 0.0f);

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    /// <summary>
    /// Khởi tạo grid cell cho preview. Gọi một lần khi game bắt đầu.
    /// </summary>
    public void Initialize()
    {
        for (var r = 0; r < GameConstants.BlockPreviewSize; ++r)
        {
            for (var c = 0; c < GameConstants.BlockPreviewSize; ++c)
            {
                cells[r, c] = Instantiate(cellPrefab, transform);
            }
        }

        originalPosition = transform.localPosition;
        originalScale = transform.localScale;
    }

    /// <summary>
    /// Hiển thị hình khối theo polyomino index.
    /// </summary>
    public void Show(int polyominoIndex)
    {
        this.polyominoIndex = polyominoIndex;
        HideAllCells();

        var shape = PolyominoData.Get(polyominoIndex);
        var shapeRows = shape.GetLength(0);
        var shapeColumns = shape.GetLength(1);
        shapeCenter = new Vector2(shapeColumns * 0.5f, shapeRows * 0.5f);

        for (var r = 0; r < shapeRows; ++r)
        {
            for (var c = 0; c < shapeColumns; ++c)
            {
                if (shape[r, c] > 0)
                {
                    cells[r, c].transform.localPosition = new Vector3(
                        c - shapeCenter.x + GameConstants.CellPositionOffset,
                        r - shapeCenter.y + GameConstants.CellPositionOffset,
                        0.0f
                    );
                    cells[r, c].ShowNormal();
                }
            }
        }
    }

    private void HideAllCells()
    {
        for (var r = 0; r < GameConstants.BlockPreviewSize; ++r)
        {
            for (var c = 0; c < GameConstants.BlockPreviewSize; ++c)
            {
                cells[r, c].Hide();
            }
        }
    }

    // ─────────────────────────────────────────────
    // Mouse input handling
    // ─────────────────────────────────────────────

    private void OnMouseDown()
    {
        inputStartWorldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.localPosition = originalPosition + InputOffset;
        transform.localScale = Vector3.one;

        currentDragPoint = Vector2Int.RoundToInt((Vector2)transform.position - shapeCenter);
        board.Hover(currentDragPoint, polyominoIndex);

        previousDragPoint = currentDragPoint;
        previousMousePosition = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        var currentMousePosition = Input.mousePosition;
        if (currentMousePosition == previousMousePosition) return;

        previousMousePosition = currentMousePosition;

        var inputDelta = (Vector2)mainCamera.ScreenToWorldPoint(currentMousePosition) - inputStartWorldPoint;
        transform.localPosition = originalPosition + InputOffset + (Vector3)inputDelta;

        currentDragPoint = Vector2Int.RoundToInt((Vector2)transform.position - shapeCenter);

        if (currentDragPoint != previousDragPoint)
        {
            previousDragPoint = currentDragPoint;
            board.Hover(currentDragPoint, polyominoIndex);
        }
    }

    private void OnMouseUp()
    {
        previousMousePosition = Vector3.positiveInfinity;
        currentDragPoint = Vector2Int.RoundToInt((Vector2)transform.position - shapeCenter);

        if (board.Place(currentDragPoint, polyominoIndex))
        {
            gameObject.SetActive(false);
            spawner.OnBlockPlaced();
        }

        transform.localPosition = originalPosition;
        transform.localScale = originalScale;
    }
}
