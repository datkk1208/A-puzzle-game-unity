 using UnityEngine;

public class Board : MonoBehaviour
{
   public const int Size = 8;
    [SerializeField] private Cell cellPrefabs;
    [SerializeField] private Transform cellTransform;
    private readonly Cell[,] cells = new Cell[Size, Size];

    private void Start()
    {
        for (var r = 0; r < Size; ++r)
        {

            for (var c = 0; c < Size; ++c)
            {
                cells[r, c] = Instantiate(cellPrefabs,cellTransform);
                cells[r, c].transform.position = new(c + 0.5f, r + 0.5f, 0.0f);
            } 
        }
                


            
    }

}
