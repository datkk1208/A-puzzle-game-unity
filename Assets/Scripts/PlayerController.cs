using UnityEngine;

[RequireComponent(typeof(GridMovement))]
public class PlayerController : MonoBehaviour
{
    private GridMovement gridMovement;
     public SubjectType entityType = SubjectType.Player; 

    private void Awake()
    {
        gridMovement = GetComponent<GridMovement>();
    }

    private void Update()
    {
        Vector2 inputDir = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) inputDir = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) inputDir = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) inputDir = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) inputDir = Vector2.right;

        if (inputDir != Vector2.zero)
        {
            TryMove(inputDir);
        }
    }

    private void TryMove(Vector2 direction)
    {
        // Kiểm tra Rule Manager trước khi di chuyển [cite: 124, 125]
        if (RuleManager.Instance.IsActionAllowed(entityType, ActionType.Move))
        {
            gridMovement.Move(direction);
        }
        else
        {
            Debug.Log("Movement blocked by rule!");
        }
    }
}