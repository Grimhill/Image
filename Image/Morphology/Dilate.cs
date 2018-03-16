using System;
using System.Linq;
using Image.ArrayOperations;

namespace Image
{
    public static class Dilate
    {
        //Morphological dilatation operation
        public static int[,] DilateMe(int[,] arr, int[,] structElement)
        {
            return DilateProcess(arr, structElement);
        }

        private static int[,] DilateProcess(int[,] arr, int[,] structElement)
        {
            int width  = arr.GetLength(1);
            int height = arr.GetLength(0);

            int[,] temp;
            PadMyArray<int> padArr = new PadMyArray<int>();

            int[,] result = new int[height, width];
            int[,] toConv = new int[structElement.GetLength(0), structElement.GetLength(1)];

            if (arr.Length < structElement.Length || arr.GetLength(0) < structElement.GetLength(0) || arr.GetLength(1) < structElement.GetLength(1))
            {
                Console.WriteLine("Cannot operate with image; less then structure element. Returned array with zeros. Method: DilatMe");
                return result;
            }

            int padsizeR, padsizeC = 0;
            if (structElement.GetLength(0) % 2 == 0)
                padsizeR = structElement.GetLength(0) / 2;
            else
                padsizeR = (structElement.GetLength(0) - 1) / 2;

            if (structElement.GetLength(1) % 2 == 0)
                padsizeC = structElement.GetLength(1) / 2;
            else
                padsizeC = (structElement.GetLength(1) - 1) / 2;

            //form array at different structure element size at input
            //[1; 2]
            if (structElement.GetLength(0) == 1 && structElement.GetLength(1) == 2)
            {
                temp = padArr.PadArray(arr, padsizeR, padsizeC, PadType.replicate, Direction.pre);
            }

            //[1; >2 & %2 == 0]
            else if (structElement.GetLength(0) == 1 && structElement.GetLength(1) % 2 == 0)
            {
                padsizeR = 0;
                padsizeC = structElement.GetLength(1) / 2;
                temp = padArr.PadArray(arr, padsizeR, padsizeC, PadType.replicate, Direction.pre);

                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.post);
            }

            //[2; 1]
            else if (structElement.GetLength(0) == 2 && structElement.GetLength(1) == 1)
            {
                temp = padArr.PadArray(arr, padsizeR, padsizeC, PadType.replicate, Direction.pre);
            }

            //[>2 & %2 == 0; 1]
            else if (structElement.GetLength(0) % 2 == 0 && structElement.GetLength(1) == 1)
            {
                padsizeC = 0;
                padsizeR = structElement.GetLength(0) / 2;
                temp = padArr.PadArray(arr, padsizeR, padsizeC, PadType.replicate, Direction.pre);

                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.post);
            }

            //[2;2]
            else if (structElement.GetLength(0) == 2 && structElement.GetLength(1) == 2)
            {
                temp = padArr.PadArray(arr, padsizeR, padsizeC, PadType.replicate, Direction.pre);
            }

            //[2; >2 & %2 == 0]
            else if (structElement.GetLength(0) == 2 && structElement.GetLength(1) % 2 == 0)
            {
                padsizeC = 0;
                temp = padArr.PadArray(arr, padsizeR, padsizeC, PadType.replicate, Direction.pre);

                padsizeR = 0;
                padsizeC = structElement.GetLength(1) / 2;
                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.pre);

                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.post);
            }

            //[>2 & %2 == 0; 2]
            else if (structElement.GetLength(0) % 2 == 0 && structElement.GetLength(1) == 2)
            {
                padsizeR = 0;
                temp = padArr.PadArray(arr, padsizeR, padsizeC, PadType.replicate, Direction.pre);

                padsizeC = 0;
                padsizeR = structElement.GetLength(0) / 2;
                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.pre);

                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.post);
            }

            //[>2 & %2 == 0; >2 & %2 == 0]
            else if (structElement.GetLength(0) % 2 == 0 && structElement.GetLength(1) % 2 == 0)
            {
                padsizeR = 0;
                padsizeC = structElement.GetLength(1) / 2;
                temp = padArr.PadArray(arr, padsizeR, padsizeC, PadType.replicate, Direction.pre);

                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.post);

                padsizeC = 0;
                padsizeR = structElement.GetLength(0) / 2;
                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.pre);

                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.post);
            }

            //[%2 != 0; 2]
            else if (structElement.GetLength(0) % 2 != 0 && structElement.GetLength(1) == 2)
            {
                padsizeR = 0;
                temp = padArr.PadArray(arr, padsizeR, padsizeC, PadType.replicate, Direction.pre);

                padsizeC = 0;
                padsizeR = (structElement.GetLength(0) - 1) / 2;
                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.both);
            }

            //[%2 !=0; >2 & %2 == 0]
            else if (structElement.GetLength(0) % 2 != 0 && structElement.GetLength(1) % 2 == 0)
            {
                padsizeR = 0;
                padsizeC = structElement.GetLength(1) / 2;
                temp = padArr.PadArray(arr, padsizeR, padsizeC, PadType.replicate, Direction.pre);

                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.post);

                padsizeC = 0;
                padsizeR = (structElement.GetLength(0) - 1) / 2;
                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.both);
            }

            //[2; %2 != 0]
            else if (structElement.GetLength(0) == 2 && structElement.GetLength(1) % 2 != 0)
            {
                padsizeC = 0;
                temp = padArr.PadArray(arr, padsizeR, padsizeC, PadType.replicate, Direction.pre);

                padsizeR = 0;
                padsizeC = (structElement.GetLength(1) - 1) / 2;
                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.both);
            }

            //[>2 & %2 == 0; %2 != 0]
            else if (structElement.GetLength(0) % 2 == 0 && structElement.GetLength(1) % 2 != 0)
            {
                padsizeC = 0;
                padsizeR = structElement.GetLength(0) / 2;
                temp = padArr.PadArray(arr, padsizeR, padsizeC, PadType.replicate, Direction.pre);

                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.post);

                padsizeR = 0;
                padsizeC = (structElement.GetLength(1) - 1) / 2;
                temp = padArr.PadArray(temp, padsizeR, padsizeC, PadType.replicate, Direction.both);
            }

            //[%2 != 0; %2 != 0]
            else
            {
                temp = padArr.PadArray(arr, padsizeR, padsizeC, PadType.replicate, Direction.both);
            }

            //obtain part of image array for dilate by structure element
            try
            {
                for (int i = 1; i <= height; i++)
                {
                    for (int j = 1; j <= width; j++)
                    {
                        for (int m = 0; m < structElement.GetLength(0); m++)
                        {
                            for (int n = 0; n < structElement.GetLength(1); n++)
                            {
                                toConv[m, n] = temp[i + m - 1, j + n - 1];
                            }
                        }

                        int[,] convolution = new int[structElement.GetLength(0), structElement.GetLength(1)];
                        convolution = toConv.ArrayMultElements(structElement);

                        result[i - 1, j - 1] = convolution.Cast<int>().Max();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem; most likely OutOfRangeException. Dilate Method. \nHere message: " +
                    e.Message);
            }

            return result;
        }
    }
}
