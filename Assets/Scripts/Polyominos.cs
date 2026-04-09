using UnityEngine;
namespace Game
{
    public class Polyominos : MonoBehaviour
    {
        private static readonly int[][,] polymonios = new int[][,]
      {
    new int[,] {
        { 0, 0, 1 },
        { 0, 0, 1 },
        { 1, 1, 1 },
    } ,
    new int[,]
    {
        { 0, 1, 0 },
        { 0, 1, 0 },
        { 1, 1, 1 }
    },
      new int[,]
    {
        { 0, 0, 1 },
        { 0, 0, 1 },
        { 1, 1, 1 }
    },
      new int[,]
    {
        { 1, 0, 0 },
        { 1, 0, 0 },
        { 1, 1, 1 }
    },
      new int[,]
    {
        { 1, 1, 1 },
        { 0, 0, 1 },
        { 0, 0, 1 }
    },
      new int[,]
    {
        { 1, 1, 1 },
        { 1, 0, 0 },
        { 1, 0, 0 }
    },
      new int[,]
    {
        { 0, 0, 1 },
        { 0, 0, 1 },
        { 1, 1, 1 }
    },
      new int[,]
    {
        { 1, 1, 1 },
        { 0, 1, 0 },
        { 0, 1, 0 }
    },
      new int[,]
    {
        { 1, 1, 0 },
        { 1, 0, 0 },
        { 0, 0, 0 }
    },
      new int[,]
    {
        { 0, 1, 1 },
        { 0, 0, 1 },
        { 0, 0, 0 }
    },
      new int[,]
    {
        { 0, 0, 0 },
        { 1, 0, 0 },
        { 1, 1, 0 }
    },
      new int[,]
    {
        { 0, 0, 0 },
        { 0, 0, 1 },
        { 0, 1, 1 }
    },
      new int[,]
    {
        { 0, 0, 0 },
        { 0, 1, 0 },
        { 1, 1, 1 }
    },
      new int[,]
    {
        { 1, 1, 1 },
        { 0, 1, 0 },
        { 0, 0, 0 }
    }

      };
        static Polyominos()
        {
            foreach (var polyomino in polymonios)
            {
                ReverseRow(polyomino);
            }
        }

        public static int[,] get(int index) => polymonios[index];
        public static int length => polymonios.Length;
        private static void ReverseRow(int[,] polyomino)
        {
            var polyominoRows = polyomino.GetLength(0);
            var polyominoColumns = polyomino.GetLength(1);
            for (var r = 0; r < polyominoRows / 2; ++r)
            {
                var topRow = r;
                var bottomRow = polyominoRows - 1 - r;
                for (var c = 0; c < polyominoColumns; ++c)
                {
                    var tmp = polyomino[topRow, c];
                    polyomino[topRow, c] = polyomino[bottomRow, c];
                    polyomino[bottomRow, c] = tmp;
                }
            }
        }
    }

}