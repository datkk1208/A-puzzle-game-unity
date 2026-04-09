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
        private Rect viewFrameRect = new Rect(0f, 0f, 1f, 1f); // KHỞI TẠO MẶC ĐỊNH
        public Rect viewRect = new Rect(0, 0, 10, 10);
        public Vector2Int boardSize = new Vector2Int(8, 8);
        
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
            if (mainCamera == null) return;
            // Chặn lỗi chia cho 0 lúc vừa load view
            if (viewFrameRect.width <= 0 || viewFrameRect.height <= 0 || viewRect.width <= 0 || viewRect.height <= 0) return;
            
            var center = viewFrameRect.center;
            var size = viewRect.size / viewFrameRect.size;
            var height = Mathf.Max(size.x / mainCamera.aspect, size.y);
            var orthographicSize = height * 0.5f;

            mainCamera.orthographicSize = orthographicSize;
            
            // Canh lại màn hình và camera transform
            transform.position = new Vector3(viewRect.center.x, viewRect.center.y - (center.y - 0.5f) * height, transform.position.z);

            // Tự động kéo dãn ảnh cover toàn bộ Background của Camera
            if (backgroundTransform != null)
            {
                backgroundTransform.position = new Vector3(transform.position.x, transform.position.y, backgroundTransform.position.z);
                var spriteRenderer = backgroundTransform.GetComponent<SpriteRenderer>();
                
                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    var nativeSize = spriteRenderer.sprite.bounds.size;
                    
                    float scaleX = (height * mainCamera.aspect) / nativeSize.x;
                    float scaleY = height / nativeSize.y;
                    
                    // Lấy scale lớn nhất ở 1 trong 2 cạnh để Fill màn hình, không bị viền đen
                    float scaleFactor = Mathf.Max(scaleX, scaleY);
                    backgroundTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
                }
            }

            // Đồng bộ Score UI với GameObject bằng RectTransformUtility
            if (scoreRectTransform != null && scoreRectTransform.parent != null)
            {
                var screenPoint = mainCamera.WorldToScreenPoint(new Vector3(boardSize.x * 0.5f, boardSize.y + 0.25f, 0.0f));
                
                if (
                    !float.IsNaN(screenPoint.x) && !float.IsNaN(screenPoint.y) && !float.IsNaN(screenPoint.z) &&
                    !float.IsInfinity(screenPoint.x) && !float.IsInfinity(screenPoint.y) && !float.IsInfinity(screenPoint.z)
                )
                {
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        scoreRectTransform.parent.GetComponent<RectTransform>(), 
                        screenPoint, 
                        null, // Truyền null do Canvas RenderMode mặc định là ScreenSpaceOverlay
                        out Vector2 localPoint))
                    {
                        scoreRectTransform.localPosition = new Vector3(localPoint.x, localPoint.y, 0f);
                    }
                }
            }
        }
    }
}