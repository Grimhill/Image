using System;
using System.Linq;
using Image.ArrayOperations;

//filter operation (convolution) and some filter windows
namespace Image
{
    //Sorry, but can`t here generic, coz T * T opeartions in ArrayOperations class

    //Linear image filter
    public static class ImageFilter
    {
        public static double[,] Filter_double(double[,] arr, double[,] filter, PadType padType)
        {                
            int width  = arr.GetLength(1);
            int height = arr.GetLength(0); 
            
            double[,] temp;            
            PadMyArray<double> padArr = new PadMyArray<double>();

            double[,] result = new double[height, width];
            double[,] toConv = new double[filter.GetLength(0), filter.GetLength(1)];

            if (arr.Length < filter.Length)
            {
                Console.WriteLine("Cannot filter image, less than filter window. Returned array with zeros. Method: filter_double");
                return result;
            }

            int padsizeR, padsizeC = 0;
            if (filter.GetLength(0) % 2 == 0)
                padsizeR = filter.GetLength(0) / 2;
            else
                padsizeR = (filter.GetLength(0) - 1) / 2;

            if (filter.GetLength(1) % 2 == 0)
                padsizeC = filter.GetLength(1) / 2;
            else
                padsizeC = (filter.GetLength(1) - 1) / 2;                                 
            
            temp = padArr.PadArray(arr, padsizeR, padsizeC, padType, Direction.both);

            //filtering
            //obtain part of image array and convolution with filter window
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

                        //such size, coz array mult by elements, and we take part same size as filter window
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
        
        public static int[,] Filter_int(int[,] arr, int[,] filter, PadType padType)
        {                  
            int width  = arr.GetLength(1);
            int height = arr.GetLength(0);
                   
            int[,] temp;
            PadMyArray<int> padArr = new PadMyArray<int>();

            int[,] result = new int[height, width];
            int[,] toConv = new int[filter.GetLength(0), filter.GetLength(1)];

            if (arr.Length < filter.Length)
            {
                Console.WriteLine("Cannot filter image, less than filter window. Returned array with zeros. Method: filter_int");
                return result;
            }

            int padsizeR, padsizeC = 0;
            if (filter.GetLength(0) % 2 == 0)
                padsizeR = filter.GetLength(0) / 2;
            else
                padsizeR = (filter.GetLength(0) - 1) / 2;

            if (filter.GetLength(1) % 2 == 0)
                padsizeC = filter.GetLength(1) / 2;
            else
                padsizeC = (filter.GetLength(1) - 1) / 2;

            temp = padArr.PadArray(arr, padsizeR, padsizeC, padType, Direction.both);

            //filtering
            //obtain part of image array and convolution with filter window
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
        
        
        #region Shorters
        //Shorter some operation with using filter
        //shorter double
        public static double[,] Filter_double(int[,] arr, string filterType)
        {
            return Filter_double(arr.ArrayToDouble(), ImageFilter.Dx3FWindow(filterType), PadType.replicate);
        }

        public static double[,] Filter_double(double[,] arr, string filterType)
        {
            return Filter_double(arr, ImageFilter.Dx3FWindow(filterType), PadType.replicate);
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
            return Filter_int(arr, ImageFilter.Ix3FWindow(filterType), PadType.replicate);
        }

        #endregion Shorters

        //Some prepared filters
        //double 3x3 filters
        public static double[,] Dx3FWindow (string fWindowName)
        {
            double[,] result = new double[3,3];
            switch (fWindowName)
            {
                case "Sobel":
                    double[,] Sobel  = {{1, 2, 1}, {0, 0, 0}, {-1, -2, -1}};
                    result = Sobel;
                    break;
                case "SobelT":
                    double[,] SobelT = {{1, 0, -1}, {2, 0, -2}, {1, 0, -1}};
                    result = SobelT;
                    break;
                case "Prewitt":
                    double[,] Prewitt = {{1, 1, 1}, { 0, 0, 0 }, {-1, -1, -1}};
                    result = Prewitt;
                    break;
                case "PrewittT":
                    double[,] PrewittT = { { 1, 0, -1 }, {1, 0, -1 }, { 1, 0, -1 } };
                    result = PrewittT;
                    break;
                case "Laplacian1":
                    double[,] Laplacian1 = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
                    result = Laplacian1;
                    break;
                case "Laplacian2":
                    double[,] Laplacian2 = { { 1, 1, 1 }, { 1, -8, 1 }, { 1, 1, 1 } };
                    result = Laplacian2;
                    break;
                case "unsharp":
                    double[,] unsharp = {{-0.1667, -0.6667, -0.1667}, {-0.6667, 4.3333, -0.6667}, {-0.1667, -0.6667, -0.1667}};
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
                case "HighContrast1":
                    double[,] HighContrast1 = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
                    result = HighContrast1;
                    break;
                case "HighContrast2":
                    double[,] HighContrast2 = { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } };
                    result = HighContrast2;
                    break;
                default:
                    double[,] def = {{1, 1, 1}, {1, 1, 1}, {1, 1, 1}};
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
                case "Laplacian1":
                    int[,] Laplacian1 = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
                    result = Laplacian1;
                    break;
                case "Laplacian2":
                    int[,] Laplacian2 = { { 1, 1, 1 }, { 1, -8, 1 }, { 1, 1, 1 } };
                    result = Laplacian2;
                    break;
                case "Roberts":
                    int[,] Roberts = { { 0, 1 }, { -1, 0 } };
                    result = Roberts;
                    break;
                case "RobertsT":
                    int[,] RobertsT = { { 1, 0 }, { 0, -1 } };
                    result = RobertsT;
                    break;
                case "HighContrast1":
                    int[,] HighContrast1 = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
                    result = HighContrast1;
                    break;
                case "HighContrast2":
                    int[,] HighContrast2 = { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } };
                    result = HighContrast2;
                    break;
                default:
                    int[,] def = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
                    result = def;
                    break;
            }
            return result;
        }

        //create filter with size
        //for now only average. kick away idea with randomed filter with size.
        public static double[,] FspecialSize(int m, int n, string type)
        {
	        double[,] result = new double[m,n];
	
	        switch(type)
	        {
		        case "average":
			        for(int i = 0; i < m; i++)
			        {
				        for(int j = 0; j < n; j++)
				        {
					        result[i, j] = (double)1/(double)(m*n);
				        }
			        }
		            break;
	         }
	
	        return result;
        }
    }
}
