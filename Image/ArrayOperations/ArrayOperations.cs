//Some array helping array operations
namespace Image
{
    public class ArrGen<T>
    {
        //ArrGen<int> d = new ArrGen<int>();        
        //var p = d.ArrOfSingle(4, 5, 1);
        public T[,] ArrOfSingle(int r, int c, T value)
        {
            T[,] arr = new T[r, c];
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    arr[i, j] = value;
                }
            }

            return arr;
        }

        public T[,] VecorToArrayRowByRow(int r, int c, T[] inVector)
        {
            T[,] arr = new T[r, c];

            int count = 0;
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    arr[i, j] = inVector[count];
                    count++;
                }
            }

            return arr;
        }

        public T[,] VecorToArrayColbyCol(int r, int c, T[] inVector)
        {
            T[,] arr = new T[r, c];

            int count = 0;
            for (int i = 0; i < c; i++)
            {
                for (int j = 0; j < r; j++)
                {
                    arr[j, i] = inVector[count];
                    count++;
                }
            }

            return arr;
        }

        public T[,] TransposeArray(T[,] inArr)
        {
            //for transpose!
            int rows = inArr.GetLength(1);
            int cols = inArr.GetLength(0);

            T[,] arr = new T[rows, cols];

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    arr[j, i] = inArr[i, j];
                }
            }

            return arr;
        }

        public T[] ArrayToVectorColByCol(T[,] inArr)
        {
            //for transpose!
            int rows = inArr.GetLength(0);
            int cols = inArr.GetLength(1);

            T[] arr = new T[inArr.Length];
            int count = 0;

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    arr[count] = inArr[j, i];
                    count++;
                }
            }

            return arr;
        }
    }
}
