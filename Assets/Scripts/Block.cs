using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;


public class Block : MonoBehaviour
{
    public const int size = 5;
    private readonly Vector3 inputOffset =  new(0.0f,2.0f,0.0f);
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private Board board;
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
        var polyominoRows  = polyomino.GetLength(0);
        var polyominosColums = polyomino.GetLength(1);
         center = new Vector2(polyominosColums * 0.5f, polyominoRows * 0.5f);
        for (var r = 0; r< polyominoRows; ++r)
        {

            for(var c = 0; c < polyominosColums; ++c)
            {
                if(polyomino[r, c] > 0)
                {
                    cells[r, c].transform.localPosition = new(c-center.x+0.5f, r-center.y+0.5f, 0.0f);
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
        Debug.Log("OnMouseDown");
        inputPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.localPosition = position + inputOffset;
        transform.localScale = Vector3.one;

        currentDragPoint = Vector2Int.RoundToInt((Vector2)transform.position - center);
        board.Hover(currentDragPoint, polyominoIndex);

     
        previousDragPoint = currentDragPoint;


        previousMousePosition = Input.mousePosition;
    }
    private void OnMouseDrag()
    {
        var currentMousePosition = Input.mousePosition;
        if (currentMousePosition != previousMousePosition)
        {
            previousMousePosition = currentMousePosition;
            Debug.Log("OnMouseDrag");
            var inputDelta = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition) -  inputPoint;
            transform.localPosition = position + inputOffset + (Vector3)inputDelta*1f;
            currentDragPoint = Vector2Int.RoundToInt((Vector2)transform.position - center);
            if(currentDragPoint != previousDragPoint)
            {
                previousDragPoint = currentDragPoint;
                //goi board de cap nhat
                Debug.Log($"Drag point {currentDragPoint}");
                board.Hover(currentDragPoint, polyominoIndex);
            }
        }
    }
    private void OnMouseUp()
    {
        Debug.Log("OnMouseUp");
        transform.localPosition = position ;
        transform.localScale = scale;
        
        previousMousePosition = Vector3.positiveInfinity;
    }
}
