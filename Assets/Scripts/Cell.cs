using UnityEngine;

/// <summary>
/// View component cho một ô (cell) trên bảng hoặc trong khối preview.
/// Chịu trách nhiệm duy nhất: hiển thị visual theo trạng thái được yêu cầu.
/// </summary>
public class Cell : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite highlightSprite;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Cập nhật visual dựa trên CellState.
    /// Board chỉ cần gọi 1 method này thay vì phải biết chi tiết render.
    /// </summary>
    public void UpdateVisual(CellState state)
    {
        switch (state)
        {
            case CellState.Empty:
                Hide();
                break;
            case CellState.Hovered:
                ShowHovered();
                break;
            case CellState.Placed:
                ShowNormal();
                break;
        }
    }

    /// <summary>
    /// Hiển thị ô ở trạng thái bình thường (đã đặt khối).
    /// </summary>
    public void ShowNormal()
    {
        gameObject.SetActive(true);
        spriteRenderer.color = Color.white;
        spriteRenderer.sprite = normalSprite;
    }

    /// <summary>
    /// Hiển thị ô được highlight (ví dụ: vừa xóa hàng).
    /// </summary>
    public void ShowHighlight()
    {
        gameObject.SetActive(true);
        spriteRenderer.color = Color.white;
        spriteRenderer.sprite = highlightSprite;
    }

    /// <summary>
    /// Hiển thị ô ở trạng thái hover (kéo khối lướt qua).
    /// </summary>
    public void ShowHovered()
    {
        gameObject.SetActive(true);
        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, GameConstants.HoverAlpha);
        spriteRenderer.sprite = normalSprite;
    }

    /// <summary>
    /// Ẩn ô.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
