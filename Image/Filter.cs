using System;
using System.Linq;

//filter operation (convolution) and some filter windows
namespace Image
{
    //Sorry, but can`t here generic, coz T * T opeartions in ArrayOperations class

    //Linear image filter
    public static class Filter
    {
        public static double[,] Filter_double(double[,] arr, double[,] filter, PadType padType)
        {
            //helpers definition       
            int width  = arr.GetLength(1);
            int height = arr.GetLength(0);

            //padarray or not, here we save  
            double[,] temp;

            PadMyArray<double> padArr;
            padArr = new PadMyArray<double>();

            if (padType.ToString() == "replicate")
            {
                temp = padArr.PadArray(arr, 1, 1, PadType.replicate, Direction.both);
            }
            else
            {
                //Symmetric? why not, coz 1 row and col, lul
                temp = padArr.PadArray(arr, 1, 1, PadType.symmetric, Direction.both);
            }

            double[,] result = new double[height, width];
            double[,] toConv = new double[filter.GetLength(0), filter.GetLength(1)];

            //filtering
            try
            {
                for (int i = 1; i <= height; i++)
                {
                    for (int j = 1; j <= width; j++)
                    {
                        for (int m = 0; m < filter.GetLength(0); m++)
                        {
                            for (int n = 0; n < filter.GetLength(1); n++)
                            {
                                toConv[m, n] = temp[i + m - 1, j + n - 1];
                            }
                        }

                        //such size coz array mult by elements, and we take part same size as filter window
                        double[,] convolution = new double[filter.GetLength(0), filter.GetLength(1)];                        
                        convolution = toConv.ArrayMultElements(filter);

                        result[i - 1, j - 1] = convolution.Cast<double>().Sum(); //get elemt after filter
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem in filter, most likely OutOfRangeException, here message:\n" +
                    e.Message);
            }

            return result;
        }

        public static double[,] Filter_double(double[,] arr, double[,] filter)
        {
            //helpers definition            
            int width  = arr.GetLength(1);
            int height = arr.GetLength(0);

            double[,] result = new double[height, width];
            double[,] toConv = new double[filter.GetLength(0), filter.GetLength(1)];

            //filtering
            try
            {
                for (int i = 1; i < height - 1; i++)
                {
                    for (int j = 1; j < width - 1; j++)
                    {
                        for (int m = 0; m < filter.GetLength(0); m++)
                        {
                            for (int n = 0; n < filter.GetLength(1); n++)
                            {
                                toConv[m, n] = arr[i + m - 1, j + n - 1];
                            }
                        }

                        //such size coz array mult by elements, and we take part same size as filter window
                        double[,] convolution = new double[filter.GetLength(0), filter.GetLength(1)];                       
                        convolution = toConv.ArrayMultElements(filter);

                        result[i, j] = convolution.Cast<double>().Sum(); //get elemt after filter
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem in filter, most likely OutOfRangeException, here message:\n" +
                    e.Message);
            }

            return result;
        }

        public static int[,] Filter_int(int[,] arr, int[,] filter, PadType padType)
        {
            //helpers definition            
            int width  = arr.GetLength(1);
            int height = arr.GetLength(0);

            //padarray or not, here we save         
            int[,] temp;

            PadMyArray<int> padArr;
            padArr = new PadMyArray<int>();

            if (padType.ToString() == "replicate")
            {
                temp = padArr.PadArray(arr, 1, 1, padType, Direction.both);
            }
            else
            {
                //Symmetric? why not, coz 1 row and col, lul
                temp = padArr.PadArray(arr, 1, 1, PadType.symmetric, Direction.both);
            }

            int[,] result = new int[height, width];
            int[,] toConv = new int[filter.GetLength(0), filter.GetLength(1)];

            //filtering
            try
            {
                for (int i = 1; i <= height; i++)
                {
                    for (int j = 1; j <= width; j++)
                    {
                        for (int m = 0; m < filter.GetLength(0); m++)
                        {
                            for (int n = 0; n < filter.GetLength(1); n++)
                            {
                                toConv[m, n] = temp[i + m - 1, j + n - 1];
                            }
                        }

                        //such size coz array mult by elements, and we take part same size as filter window
                        int[,] convolution = new int[filter.GetLength(0), filter.GetLength(1)];                        
                        convolution = toConv.ArrayMultElements(filter);

                        //get elemt after filter
                        if (convolution.Cast<int>().Sum() < 0) { result[i - 1, j - 1] = 0; }
                        else if (convolution.Cast<int>().Sum() > 255) { result[i - 1, j - 1] = 255; }
                        else { result[i - 1, j - 1] = convolution.Cast<int>().Sum(); }
                        //result[i - 1, j - 1] = convolution.Cast<int>().Sum(); //get elemt after filter                  
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem in filter, most likely OutOfRangeException, here message:\n" +
                    e.Message);
            }

            return result;
        }

        public static int[,] Filter_int(int[,] arr, int[,] filter)
        {
            //helpers definition            
            int width  = arr.GetLength(1);
            int height = arr.GetLength(0);

            int[,] result = new int[height, width];
            int[,] toConv = new int[filter.GetLength(0), filter.GetLength(1)];

            //filtering
            try
            {
                for (int i = 1; i < height - 1; i++)
                {
                    for (int j = 1; j < width - 1; j++)
                    {
                        for (int m = 0; m < filter.GetLength(0); m++)
                        {
                            for (int n = 0; n < filter.GetLength(1); n++)
                            {
                                toConv[m, n] = arr[i + m - 1, j + n - 1];
                            }
                        }

                        //such size coz array mult by elements, and we take part same size as filter window
                        int[,] convolution = new int[filter.GetLength(0), filter.GetLength(1)];                        
                        convolution = toConv.ArrayMultElements(filter);

                        //get elemt after filter
                        if (convolution.Cast<int>().Sum() < 0) { result[i - 1, j - 1] = 0; }
                        else if (convolution.Cast<int>().Sum() > 255) { result[i - 1, j - 1] = 255; }
                        else { result[i - 1, j - 1] = convolution.Cast<int>().Sum(); }
                        //result[i, j] = convolution.Cast<int>().Sum(); //get elemt after filter                  
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem in filter, most likely OutOfRangeException, here message:\n" +
                    e.Message);
            }

            return result;
        }


        #region Shorters
        //shorter double
        public static double[,] Filter_double(int[,] arr, string filterType)
        {           
            return Filter_double(arr.ArrayToDouble(), Filter.Dx3FWindow(filterType), PadType.replicate);
        }

        public static double[,] Filter_double(double[,] arr, string filterType)
        {          
            return Filter_double(arr, Filter.Dx3FWindow(filterType), PadType.replicate);
        }

        public static double[,] Filter_double(int[,] arr, double[,] filter, double fdiv)
        {          
            return Filter_double(arr.ArrayToDouble(), filter.ArrayDivByConst(fdiv), PadType.replicate);
        }

        public static double[,] Filter_double(int[,] arr, double[,] filter)
        {            
            return Filter_double(arr.ArrayToDouble(), filter, PadType.replicate);
        }

        //shorter int
        public static int[,] Filter_int(int[,] arr, string filterType)
        {           
            return Filter_int(arr, Filter.Ix3FWindow(filterType), PadType.replicate);
        }

        #endregion Shorters

        //double 3x3 filters
        public static double[,] Dx3FWindow(string fWindowName)
        {
            double[,] result = new double[3, 3];
            switch (fWindowName)
            {
                case "Sobel":
                    double[,] Sobel = { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };
                    result = Sobel;
                    break;
                case "SobelT":
                    double[,] SobelT = { { 1, 0, -1 }, { 2, 0, -2 }, { 1, 0, -1 } };
                    result = SobelT;
                    break;
                case "Prewitt":
                    double[,] Prewitt = { { 1, 1, 1 }, { 0, 0, 0 }, { -1, -1, -1 } };
                    result = Prewitt;
                    break;
                case "PrewittT":
                    double[,] PrewittT = { { 1, 0, -1 }, { 1, 0, -1 }, { 1, 0, -1 } };
                    result = PrewittT;
                    break;
                case "Laplassian1":
                    double[,] Laplassian1 = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
                    result = Laplassian1;
                    break;
                case "Laplassian2":
                    double[,] Laplassian2 = { { 1, 1, 1 }, { 1, -8, 1 }, { 1, 1, 1 } };
                    result = Laplassian2;
                    break;
                case "unsharp":
                    double[,] unsharp = { { -0.1667, -0.6667, -0.1667 }, { -0.6667, 4.3333, -0.6667 }, { -0.1667, -0.6667, -0.1667 } };
                    result = unsharp;
                    break;
                case "Roberts":
                    double[,] Roberts = { { 0, 1 }, { -1, 0 } };
                    result = Roberts;
                    break;
                case "RobertsT":
                    double[,] RobertsT = { { 1, 0 }, { 0, -1 } };
                    result = RobertsT;
                    break;
                default:
                    double[,] def = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
                    result = def;
                    break;
            }
            return result;
        }

        //int 3x3 filters
        public static int[,] Ix3FWindow(string fWindowName)
        {
            int[,] result = new int[3, 3];
            switch (fWindowName)
            {
                case "Sobel":
                    int[,] Sobel = { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };
                    result = Sobel;
                    break;
                case "SobelT":
                    int[,] SobelT = { { 1, 0, -1 }, { 2, 0, -2 }, { 1, 0, -1 } };
                    result = SobelT;
                    break;
                case "Prewitt":
                    int[,] Prewitt = { { 1, 1, 1 }, { 0, 0, 0 }, { -1, -1, -1 } };
                    result = Prewitt;
                    break;
                case "PrewittT":
                    int[,] PrewittT = { { 1, 0, -1 }, { 1, 0, -1 }, { 1, 0, -1 } };
                    result = PrewittT;
                    break;
                case "Laplassian1":
                    int[,] Laplassian1 = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
                    result = Laplassian1;
                    break;
                case "Laplassian2":
                    int[,] Laplassian2 = { { 1, 1, 1 }, { 1, -8, 1 }, { 1, 1, 1 } };
                    result = Laplassian2;
                    break;
                case "Roberts":
                    int[,] Roberts = { { 0, 1 }, { -1, 0 } };
                    result = Roberts;
                    break;
                case "RobertsT":
                    int[,] RobertsT = { { 1, 0 }, { 0, -1 } };
                    result = RobertsT;
                    break;
                default:
                    int[,] def = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
                    result = def;
                    break;
            }
            return result;
        }

        public static double[,] Fspecial(int m, int n, string type)
        {
            double[,] result = new double[m, n];

            switch (type)
            {
                case "average":
                    for (int i = 0; i < m; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            result[i, j] = (double)1 / (double)(m * n);
                        }
                    }
                    break;
            }

            return result;
        }
    }
}
