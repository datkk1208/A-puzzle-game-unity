using UnityEngine;

/// <summary>
/// Quản lý việc sinh và theo dõi các khối hình cho người chơi.
/// Khi tất cả khối đã được đặt, tự động sinh set khối mới.
/// </summary>
public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private Block[] blocks;

    private int remainingBlocks;

    private void Start()
    {
        InitializeBlockPositions();
        SpawnNewSet();
    }

    /// <summary>
    /// Tính toán vị trí và scale cho từng block dựa trên kích thước bảng.
    /// </summary>
    private void InitializeBlockPositions()
    {
        var blockWidth = (float)GameConstants.BoardSize / blocks.Length;
        var cellSize = (float)GameConstants.BoardSize / (GameConstants.BlockPreviewSize * blocks.Length + blocks.Length + 1);

        for (var i = 0; i < blocks.Length; ++i)
        {
            blocks[i].transform.localPosition = new Vector3(
                blockWidth * (i + 0.5f),
                -0.25f - cellSize * 4.0f,
                0.0f
            );
            blocks[i].transform.localScale = new Vector3(cellSize, cellSize, cellSize);
            blocks[i].Initialize();
        }
    }

    /// <summary>
    /// Sinh một set khối mới cho người chơi.
    /// </summary>
    private void SpawnNewSet()
    {
        for (var i = 0; i < blocks.Length; ++i)
        {
            blocks[i].gameObject.SetActive(true);
            blocks[i].Show(0);
            ++remainingBlocks;
        }
    }

    /// <summary>
    /// Được gọi khi một khối đã được đặt thành công lên bảng.
    /// Nếu hết khối thì sinh set mới.
    /// </summary>
    public void OnBlockPlaced()
    {
        --remainingBlocks;

        if (remainingBlocks <= 0)
        {
            remainingBlocks = 0;
            SpawnNewSet();
        }
    }
}
