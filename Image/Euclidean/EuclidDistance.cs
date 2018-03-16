using System;
using System.Linq;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image
{
    public static class EuclidDistance
    {
        public static double [,] EuclidBinary(double[,] arr)
        {
            return EuclidBinaryProcess(arr);
        }

        public static double[,] EuclidBinary(int[,] arr)
        {
            return EuclidBinaryProcess(arr.ArrayToDouble());
        }

        //shorter
        private static double [,] EuclidBinaryProcess(double [,] arr)
        {   
            
            double[,] distanceArray = new double[arr.GetLength(0), arr.GetLength(1)];

            if (arr.Cast<double>().Sum() == 0)
            { distanceArray = arr; }
            else
            {
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    for (int j = 0; j < arr.GetLength(1); j++)
                    {
                        if (arr[i, j] == 1)
                            distanceArray[i, j] = 0;
                        else
                        {
                            int r = 0;
                            int c = 0;
                            int count = 0;
                            List<double> trouble = new List<double>();

                            double downRight = 0;
                            double downLeft  = 0;
                            double topRight  = 0;
                            double topLeft   = 0;

                            //cross row by row to bot and col by col to right  
                            for (r = i; r < arr.GetLength(0); r++)
                            {
                                for (c = j; c < arr.GetLength(1); c++)
                                {
                                    if (arr[r, c] == 1)
                                    {
                                        trouble.Add(r); trouble.Add(c);
                                        count++;
                                        if (count == 2) goto FirstGoto;
                                    }
                                }
                            }

                            FirstGoto:
                            //if passed through, but found nothing
                            if (r == arr.GetLength(0)) downRight = 0;
                            else
                            {
                                if (trouble.Count == 2) downRight = Math.Sqrt(Math.Pow((i - trouble[0]), 2) + Math.Pow((j - trouble[1]), 2));
                                else
                                {
                                    downRight = Math.Min(Math.Sqrt(Math.Pow((i - trouble[0]), 2) + Math.Pow((j - trouble[1]), 2)),
                                        Math.Sqrt(Math.Pow((i - trouble[2]), 2) + Math.Pow((j - trouble[3]), 2)));
                                }
                                trouble = new List<double>(); count = 0;
                            }

                            //cross row by row to bot and col by col to left 
                            for (r = i; r < arr.GetLength(0); r++)
                            {
                                for (c = j; c >= 0; c--)
                                {
                                    if (arr[r, c] == 1)
                                    {
                                        trouble.Add(r); trouble.Add(c);
                                        count++;
                                        if (count == 2) goto SecondGoto;
                                    }
                                }
                            }

                            SecondGoto:
                            //if passed through, but found nothing
                            if (r == arr.GetLength(0)) downLeft = 0;
                            else
                            {
                                if (trouble.Count == 2) downLeft = Math.Sqrt(Math.Pow((i - trouble[0]), 2) + Math.Pow((j - trouble[1]), 2));
                                else
                                {
                                    downLeft = Math.Min(Math.Sqrt(Math.Pow((i - trouble[0]), 2) + Math.Pow((j - trouble[1]), 2)),
                                    Math.Sqrt(Math.Pow((i - trouble[2]), 2) + Math.Pow((j - trouble[3]), 2)));
                                }
                                trouble = new List<double>(); count = 0;
                            }

                            //cross row by row to top and col by col to right
                            for (r = i; r >= 0; r--)
                            {
                                for (c = j; c < arr.GetLength(1); c++)
                                {
                                    if (arr[r, c] == 1)
                                    {
                                        trouble.Add(r); trouble.Add(c);
                                        count++;
                                        if (count == 2) goto ThirdGoto;
                                    }
                                }
                            }

                            ThirdGoto:
                            //if passed through, but found nothing
                            if (r == -1 && c == arr.GetLength(1)) topRight = 0;
                            else
                            {
                                if (trouble.Count == 2) topRight = Math.Sqrt(Math.Pow((i - trouble[0]), 2) + Math.Pow((j - trouble[1]), 2));
                                else
                                {
                                    topRight = Math.Min(Math.Sqrt(Math.Pow((i - trouble[0]), 2) + Math.Pow((j - trouble[1]), 2)),
                                    Math.Sqrt(Math.Pow((i - trouble[2]), 2) + Math.Pow((j - trouble[3]), 2)));
                                }
                                trouble = new List<double>(); count = 0;
                            }

                            //cross row by row to top and col by col to left
                            for (r = i; r >= 0; r--)
                            {
                                for (c = j; c >= 0; c--)
                                {
                                    if (arr[r, c] == 1)
                                    {
                                        trouble.Add(r); trouble.Add(c);
                                        count++;
                                        if (count == 2) goto FourthGoto;
                                    }
                                }
                            }

                            FourthGoto:
                            //if passed through, but found nothing
                            if (r == -1 && c == 1) topLeft = 0;
                            else
                            {
                                if (trouble.Count == 2) topLeft = Math.Sqrt(Math.Pow((i - trouble[0]), 2) + Math.Pow((j - trouble[1]), 2));
                                else
                                {
                                    topLeft = Math.Min(Math.Sqrt(Math.Pow((i - trouble[0]), 2) + Math.Pow((j - trouble[1]), 2)),
                                    Math.Sqrt(Math.Pow((i - trouble[2]), 2) + Math.Pow((j - trouble[3]), 2)));
                                }
                            }

                            var distance = new[] { downRight, downLeft, topRight, topLeft }.Where(z => z > 0).DefaultIfEmpty().Min();
                            distanceArray[i, j] = Math.Round(distance, 4);
                        }
                    }
                }
            }

            return distanceArray;
        }        
    }
}
