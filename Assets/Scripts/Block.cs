using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;


public class Block : MonoBehaviour
{
    public const int size = 5;
    private readonly Vector3 inputOffset =  new(0.0f,2.0f,0.0f);
    [SerializeField] private Cell cellPrefab;
    private readonly Cell[,] cells = new Cell[size, size];

    private Vector3 position;
    private Vector3 scale;
    private Vector2 inputPoint;
    private Vector3 previousMousePosition = Vector3.positiveInfinity;
    //cache
    private Camera mainCamera;
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
        Hide();
        var polyomino = Polyominos.get(polyominoIndex);
        var polyominoRows  = polyomino.GetLength(0);
        var polyominosColums = polyomino.GetLength(1);
        var center = new Vector2(polyominosColums * 0.5f, polyominoRows * 0.5f);
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
        
        previousMousePosition = Input.mousePosition;
    }
    private void OnMouseDrag()
    {
        var currentMousePosition = Input.mousePosition;
        if (currentMousePosition != previousMousePosition)
        {
            Debug.Log("OnMouseDrag");
            var inputDelta = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition) -  inputPoint;
            transform.localPosition = position + inputOffset + (Vector3)inputDelta*1.4f;
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
