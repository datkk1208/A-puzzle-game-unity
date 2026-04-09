using System.CodeDom.Compiler;
using UnityEditor.Build;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
namespace Game
{

    public class Blocks : MonoBehaviour
    {
        [SerializeField] private Board board;
        [SerializeField] private Block[] blocks;

        [Space(8.0f)]
        [SerializeField] private GameObject lostGameObject;

        private int[] polyominoIndexes;
        private int blockCount = 0;

        private void Start()
        {
            var blockWidth = (float)Board.Size / blocks.Length;
            var cellSize = (float)Board.Size / (Block.size * blocks.Length + blocks.Length + 1);
            for (var i = 0; i < blocks.Length; ++i)
            {
                blocks[i].transform.localPosition = new(blockWidth * (i + 0.5f), -0.25f - cellSize * 4.0f, 0.0f);
                blocks[i].transform.localScale = new(cellSize, cellSize, cellSize);
                blocks[i].Initialize();
            }

            polyominoIndexes = new int[blocks.Length];
            Generate();
        }
        private void Generate()
        {
            for (var i = 0; i < blocks.Length; ++i)
            {
                polyominoIndexes[i] = Random.Range(0, Polyominos.length);
                blocks[i].gameObject.SetActive(true);
                blocks[i].Show(polyominoIndexes[i]);

                ++blockCount;
            }
        }
        public void Remove()
        {
            --blockCount;
            if (blockCount <= 0)
            {
                blockCount = 0;
                Generate();

            }

            var lose = true;

            for (var i = 0; i < blocks.Length; ++i)
            {
                if (blocks[i].gameObject.activeSelf == true && board.CheckPlace(polyominoIndexes[i]) == true)
                {
                    lose = false; break;
                }
            }
            if (lose == true)
            {
               Lose();
            }
        }
        public void ResetBlockSortingOrders()
        {
            for (var i = 0; i < blocks.Length; ++i)
            {
                blocks[i].SetSortingOrder(0);
            }
        }
        private void Lose()
        {
            lostGameObject.SetActive(true);
            StartCoroutine(DelayAnyLoseCoroutine());
        }
        private IEnumerator DelayAnyLoseCoroutine()
        {
            yield return new  WaitForSeconds(3.0f);
            SceneManager.LoadScene("Gameplay");
            
        }
    }

}