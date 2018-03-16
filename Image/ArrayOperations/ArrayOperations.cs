using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

//Some array helping array operations
namespace Image
{
    public class ArrGen<T>
    {
        //ArrGen<int> d = new ArrGen<int>();        
        //var p = d.ArrOfSingle(4, 5, 1);

        //generate 2D array of single element with entered parametes
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

        //generate vector of single element with entered parametes
        public T[] VectorOfSingle(int n, T value)
        {
            T[] vect = new T[n];
            for (int i = 0; i < n; i++)
            {
                vect[i] = value;
            }
            return vect;
        }

        //transform vector into 2D array row by row using entered parametes
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

        //transform vector into 2D array col by col using entered parametes
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

        //transponent 2D array
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

        //transform 2D array into vector col by col
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

        //obtain list of all column in 2D array
        public List<VectorsListT<T>> ReturnListof2DArrayCols(T[,] inArr)
        {
            List<VectorsListT<T>> colList = new List<VectorsListT<T>>();
            List<T> arr  = new List<T>();
            List<T> temp = new List<T>();

            int index = 0;
            arr = ArrayToVectorColByCol(inArr).ToList();

            for (int i = 0; i < inArr.GetLength(1); i++)
            {
                temp = arr.GetRange(index, inArr.GetLength(0));
                index = index + inArr.GetLength(0);
                colList.Add(new VectorsListT<T>() { Vect = temp.ToArray() });
            }

            return colList;
        }

        //obtain list of all rows in 2D array
        public List<VectorsListT<T>> ReturnListof2DArrayRows(T[,] inArr)
        {
            List<VectorsListT<T>> rowList = new List<VectorsListT<T>>();
            List<T> arr  = new List<T>();
            List<T> temp = new List<T>();

            int index = 0;
            arr = inArr.Cast<T>().ToList();

            for (int i = 0; i < inArr.GetLength(0); i++)
            {
                temp = arr.GetRange(index, inArr.GetLength(1));
                index = index + inArr.GetLength(1);
                rowList.Add(new VectorsListT<T>() { Vect = temp.ToArray() });
            }

            return rowList;
        }

        //Flip array up to down
        public T[,] FlipArray(T[,] arr)
        {
            var temporalCharge = ReturnListof2DArrayRows(arr);
            List<T> loh = new List<T>();

            for (int i = temporalCharge.Count - 1; i >= 0; i--)
            {
                loh.AddRange(temporalCharge[i].Vect);
            }

            return VecorToArrayRowByRow(arr.GetLength(0), arr.GetLength(1), loh.ToArray());
        }

        //check for array operations size missmatch
        public bool ArraySizeMismatch(T[,] x, T[,] y, [CallerMemberName]string callName = "")
        {
            if (x.GetLength(0) != y.GetLength(0) || x.GetLength(1) != y.GetLength(1))
            {
                Console.WriteLine("Mismatch in size of arrays in operation: " + callName);
                return false;
            }
            else
                return true;
        }
    }
}
