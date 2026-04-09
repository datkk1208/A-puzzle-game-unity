using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class GameCamera : MonoBehaviour // Đã thêm 'class'
    {
        [SerializeField] private Transform backgroundTransform;
        [SerializeField] private RectTransform scoreRectTransform;
        
        private Camera mainCamera;
        private Rect viewFrameRect;
        public Rect viewRect = new Rect(0, 0, 10, 10); // LƯU Ý: Phải gán giá trị cho viewRect, ví dụ 10x10
        public Vector2Int boardSize = new Vector2Int(8, 8); // LƯU Ý: Phải có giá trị
        
        private void Awake()
        {
            Assert.IsNotNull(backgroundTransform);
            Assert.IsNotNull(scoreRectTransform);
            
            mainCamera = gameObject.GetComponent<Camera>();
        }
        
        // Bạn có thể truyền boardSize từ ngoài vào nếu muốn
        public void ViewFrame(Rect rect)
        {
            this.viewFrameRect = rect;
            // this.boardSize = boardSize; // Dòng này vô nghĩa nếu không truyền boardSize vào hàm
            Apply();
        }
        public void View(Rect rect , Vector2Int boardSize)
        {
            this.viewRect = rect;
            this.boardSize = boardSize;

            Apply();
        }
        
        private void Apply()
        {
            if(mainCamera == null) return;
            
            var center = viewFrameRect.center;
            var size = viewRect.size / viewFrameRect.size;
            var height = Mathf.Max(size.x / mainCamera.aspect, size.y);
            var orthographicSize = height * 0.5f;

            mainCamera.orthographicSize = orthographicSize;
            
            // Đã gộp trục Y và thêm đủ 3 tham số cho Vector3
            transform.position = new Vector3(viewRect.center.x, viewRect.center.y - (center.y - 0.5f) * height, transform.position.z);

            // Đã sửa lại tham số Vector3 và khai báo scaleFactor
            var scaleFactor = Mathf.Max(height * mainCamera.aspect / 1080.0f, height / 1920.0f) * 100.0f;
            backgroundTransform.position = new Vector3(height * mainCamera.aspect / 1080.0f, height / 1920.0f, 0f) * 100.0f;
            backgroundTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

            // Đã thêm dấu đóng ngoặc
            var screenPoint = mainCamera.WorldToScreenPoint(new Vector3(boardSize.x * 0.5f, boardSize.y + 0.25f, 0.0f));
            
            // Sửa lại logic if để không bị thừa &&
            if (
                !float.IsNaN(screenPoint.x) &&
                !float.IsNaN(screenPoint.y) &&
                !float.IsNaN(screenPoint.z) &&
                !float.IsInfinity(screenPoint.x) &&
                !float.IsInfinity(screenPoint.y) &&
                !float.IsInfinity(screenPoint.z)
            )
            {
                // Gọi chuẩn hàm RectTransformUtility và tạo biến out localPoint
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(scoreRectTransform.parent.GetComponent<RectTransform>(), screenPoint, mainCamera, out Vector2 localPoint))
                {
                    scoreRectTransform.localPosition = localPoint;
                }
            }
        }
    }
}