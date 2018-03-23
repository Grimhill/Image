using System;
using System.Runtime.CompilerServices;

namespace Image
{
    //structure-forming element for morph operation
    public static class StructureElement
    {
        public static int[,] Line(int size, LineStructElementDegree lineType)
        {
            return LineProcess(size, lineType);
        }

        public static int[,] Square(int size)
        {
            return SquareProcess(size);
        }

        public static int[,] Rectangle(int height, int width)
        {
            return RectangleProcess(height, width);
        }

        public static int[,] Disk(int radius)
        {
            return DiskProcess(radius);
        }

        public static int[,] Diamond(int dist)
        {
            return DiamondProcess(dist);
        }

        public static int[,] Octagon(int dist)
        {
            return OctagonProcess(dist);
        }


        //create line with lenght and diretion
        private static int[,] LineProcess(int size, LineStructElementDegree lineType)
        {         
            int[,] resultH = new int [1,size];
            int[,] resultV = new int [size, 1];
            int[,] tempRes = new int [size, size];
            ArrGen<int> d = new ArrGen<int>();

            if (HelpMe(size, size))
            {
                switch (lineType)
                {
                    case LineStructElementDegree.horizontal:
                        resultH = d.ArrOfSingle(1, size, 1);
                        return resultH;

                    case LineStructElementDegree.vertical:
                        resultV = d.ArrOfSingle(size, 1, 1);
                        return resultV;

                    case LineStructElementDegree.plus45:
                        int m = size - 1;
                        int n = 0;
                        for (int i = 0; i < size; i++)
                        {
                            for (int j = 0; j < size; j++)
                            {
                                if (i == n && j == m)
                                    tempRes[i, j] = 1;
                            }
                            m--;
                            n++;
                        }
                        return tempRes;

                    case LineStructElementDegree.minus45:
                        for (int i = 0; i < size; i++)
                        {
                            for (int j = 0; j < size; j++)
                            {
                                if (i == j)
                                    tempRes[i, j] = 1;
                            }
                        }
                        return tempRes;
                }
            }          

            return tempRes;
        }        

        //create square with entered square side size
        private static int[,] SquareProcess(int size)
        {
            ArrGen<int> d = new ArrGen<int>();
            if (HelpMe(size, size))
            {
                return d.ArrOfSingle(size, size, 1);
            }

            return new int[size,size];
        }        

        //create rectangle with entred height and width 
        private static int[,] RectangleProcess(int height, int width)
        {
            ArrGen<int> d = new ArrGen<int>();
            if (HelpMe(height, width))
            {
                return d.ArrOfSingle(height, width, 1);
            }
            return new int[height, width];
        }
        
        //create disk with radius
        private static int[,] DiskProcess(int radius)
        {
            int[,] result = new int[(radius * 2) + 1, (radius * 2) + 1];

            ArrGen<int> d = new ArrGen<int>();

            if (HelpMe(radius, radius))
            {
                //if (radius == 0)
                //    result = new int[1; 1] { { 1 } };
                if (radius == 1)
                    result = new int[3, 3] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
                else if (radius == 3)
                    result = d.ArrOfSingle(5, 5, 1);
                else
                {
                    if (radius != 2)
                        result = new int[(radius * 2) - 1, (radius * 2) - 1];

                    result = d.ArrOfSingle(result.GetLength(0), result.GetLength(0), 1);

                    for (int i = 0; i < result.GetLength(0); i++)
                    {
                        for (int j = 0; j < result.GetLength(0); j++)
                        {
                            if (i == 0 || i == result.GetLength(0) - 1)
                            {
                                if (j < 2 || j > result.GetLength(0) - 3)
                                {
                                    result[i, j] = 0;
                                }
                            }
                            else if (i == 1 || i == result.GetLength(0) - 2)
                            {
                                if (j < 1 || j > result.GetLength(0) - 2)
                                {
                                    result[i, j] = 0;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        //create diamond with dist from center point 1 to each direction 
        private static int[,] DiamondProcess(int dist) 
        {
            int[,] result = new int[(dist * 2) + 1, (dist * 2) + 1];
            //int[] resVect = result.Cast<int>().ToArray();      

            ArrGen<int> d = new ArrGen<int>();

            if (HelpMe(dist, dist))
            {
                result = d.ArrOfSingle(result.GetLength(0), result.GetLength(0), 1);
                
                for (int i = 0; i < result.GetLength(0); i++)
                {
                    for (int j = 0; j < result.GetLength(0); j++)
                    {
                        if (i < dist && j < dist - i || i < dist && j > i + dist)
                        {
                            result[i, j] = 0;
                        }
                        else if (i > dist && j < i - dist || i > dist && j > result.GetLength(0) - (i - dist + 1))
                        {
                            result[i, j] = 0;
                        }
                    }
                }
            }

            return result;
        }

        //create octagon with dist from center point 1 to each direction 
        private static int[,] OctagonProcess(int dist)
        {            
            ArrGen<int> d = new ArrGen<int>();

            int[,] result = d.ArrOfSingle((dist * 2) + 1, (dist * 2) + 1, 1);

            var baka = dist / 3 * 2;

            if (HelpMe(dist,dist))
            {
                for (int i = 0; i < result.GetLength(0); i++)
                {
                    for (int j = 0; j < result.GetLength(0); j++)
                    {
                        if (i < baka && j < baka - i || i < baka && j >= result.GetLength(0) - baka + i)
                        {
                            result[i, j] = 0;
                        }
                        else if(i >= result.GetLength(0) - baka && j <= baka - (result.GetLength(0) - i)
                            || i >= result.GetLength(0) - baka && j >= result.GetLength(0) - (i - (baka*2 + 1)) - 1)
                        {
                            result[i, j] = 0;
                        }      
                    }
                }             
            }

            return result;
        }

        //check for acceptability of creating structure elements
        private static bool HelpMe(int m, int n, [CallerMemberName]string callName = "")
        {
            bool check = true;

            if(m < 2 && n < 2)
            {
                if (m == 1 && n == 1 && (callName == "DiskProcess" || callName == "DiamondProcess"))
                {
                    check = true;
                }             
                else
                {
                    check = false;
                    Console.WriteLine("I do not want to make structure with such low size. Or you entered negative value.\n"
                        + "Obtain zeros muchachos. Structure element: " + callName);
                    Console.WriteLine("Line: > 1, Square: > 1, Rectangle: > [1 1], Diamond: > 0, Disk: > 0, Octagon: nonnegative multiple of 3.");
                }
            }
            else if(m % 3 != 0 && callName == "OctagonProcess")
            {
                check = false;
                Console.WriteLine("Octagon imput parameter must be a nonnegative multiple of 3.\n"
                    + "Return zeros. Structure element: " + callName);
            }           

            return check;
        }
    }    

    public enum LineStructElementDegree
    {
        plus45,
        minus45,
        vertical,
        horizontal
    }
}
