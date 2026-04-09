using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class GameFrameView : UIBehaviour // Đã thêm 'class'
    {
        private GameCamera gameCamera;
        private GameCamera GameCamera
        {
            get
            {
                if (gameCamera == null)
                {
                    gameCamera = Camera.main.GetComponent<GameCamera>();
                }
                return gameCamera;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }   

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetDirty();
        }

        private void SetDirty()
        {
            if (IsActive() == false)
            {
                return;
            }
            
            var worldCorners = new Vector3[4];
            // Lấy component RectTransform hiện tại thay vì gọi class tĩnh
            GetComponent<RectTransform>().GetWorldCorners(worldCorners);
            
            var min = new Vector2(worldCorners[0].x / Screen.width, worldCorners[0].y / Screen.height);
            var max = new Vector2(worldCorners[2].x / Screen.width, worldCorners[2].y / Screen.height);
            
            GameCamera.ViewFrame(new Rect(min, max - min));
        }
    }
}