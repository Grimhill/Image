using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

//unSharp, Smooth and histeq operations
namespace Image
{
    public static class Sharp
    {
        //only for RGB images, b&w 24bbp.
        public static void unSharp(Bitmap img, unSharpInColorSpace cSpace, FilterType filterType)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width  = img.Width;
            int height = img.Height;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.getPixels(img);

            List<arraysListInt> Result = new List<arraysListInt>();
 
            string outName = String.Empty;         

            switch(cSpace.ToString())
            {
                case "RGB":
                    Result.Add(new arraysListInt() { c = unSharpHelperI(ColorList[0].c, filterType.ToString()) });
                    Result.Add(new arraysListInt() { c = unSharpHelperI(ColorList[1].c, filterType.ToString()) });
                    Result.Add(new arraysListInt() { c = unSharpHelperI(ColorList[2].c, filterType.ToString()) });                 

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\unSharpRGB" + filterType.ToString() + ".jpg";                
                    break;

                case "HSVi":                                        
                    var hsvi = ColorSpace.rgb2hsv(img);

                    var hsvi_temp = unSharpHelperI(ArrOp.ArrayToUint8(ArrOp.ArrayMultByConst(hsvi[2].c, 100)), filterType.ToString());

                    //Filter by V - Value (Brightness/яркость)
                    Result = ColorSpace.hsv2rgb(hsvi[0].c, hsvi[1].c, ArrOp.ToBorderGreaterZero(ArrOp.ArrayDivByConst(ArrOp.ArrayToDouble(hsvi_temp), 100), 1));

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\unSharpHSVi" + filterType.ToString() + ".jpg";                   
                    break;

                case "HSVd":
                    var hsvd = ColorSpace.rgb2hsv(img);

                    var hsvd_temp = unSharpHelperD(ArrOp.ArrayMultByConst(hsvd[2].c, 100), filterType.ToString()); //can with out mult, coz using double

                    //Filter by V - Value (Brightness/яркость)
                    Result = ColorSpace.hsv2rgb(hsvd[0].c, hsvd[1].c, ArrOp.ToBorderGreaterZero(ArrOp.ArrayDivByConst(hsvd_temp, 100), 1)); //artificially if V > 1, make him 1

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\unSharpHSVd" + filterType.ToString() + ".jpg";                    
                    break;

                case "Labi":
                    var labi = ColorSpace.rgb2lab(img);

                    var labi_temp = unSharpHelperI(ArrOp.ArrayToUint8(labi[0].c), filterType.ToString());

                    //Filter by L - lightness
                    Result = ColorSpace.lab2rgb(ArrOp.ArrayToDouble(labi_temp), labi[1].c, labi[2].c);

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\unSharpLabi" + filterType.ToString() + ".jpg"; 
                    break;

                case "Labd":
                    var labd = ColorSpace.rgb2lab(img);

                    var labd_temp = unSharpHelperD(labd[0].c, filterType.ToString());

                    //Filter by L - lightness
                    Result = ColorSpace.lab2rgb(ArrOp.ToBorderGreaterZero(labd_temp, 255), labd[1].c, labd[2].c);

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\unSharpLabd" + filterType.ToString() + ".jpg";
                    break;

                case "fakeCIE1976L":

                    var fakeCIE1976L = ColorSpace.rgb2lab(img);

                    var fakeCIE1976L_temp = unSharpHelperI(ArrOp.ArrayToUint8(ArrOp.ArrayMultByConst(fakeCIE1976L[0].c, 2.57)), filterType.ToString());

                    //Filter by L - lightness
                    Result = ColorSpace.lab2rgb(ArrOp.ArrayDivByConst(ArrOp.ArrayToDouble(fakeCIE1976L_temp), 2.57), fakeCIE1976L[1].c, fakeCIE1976L[2].c);

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\unSharpfakeCIE1976L" + filterType.ToString() + ".jpg"; 

                    break;
            }          

            image = Helpers.setPixels(image, Result[0].c, Result[1].c, Result[2].c);
            
            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        public static int[,] unSharpHelperI(int[,] cPlane, string fType)
        {
            int [,] Result;

            ArrayOperations ArrOp = new ArrayOperations();
            if (fType == "unsharp")
            {
                Result = ArrOp.ArrayToUint8(Filter.filter_double(ArrOp.ArrayToDouble(cPlane), Filter.Dx3FWindow(fType), PadType.replicate));                
            }
            else
            {             
                Result = ArrOp.Uint8Range(ArrOp.SubArrays(cPlane, Filter.filter_int(cPlane, Filter.Ix3FWindow(fType), PadType.replicate))); 
            }

            return Result;
        }

        public static double[,] unSharpHelperD(double[,] cPlane, string fType)
        {
            double[,] Result;

            ArrayOperations ArrOp = new ArrayOperations();
            if (fType == "unsharp")
            {
                Result = Filter.filter_double(cPlane, Filter.Dx3FWindow(fType), PadType.replicate);              
            }
            else
            {
                Result = ArrOp.SubArrays(cPlane, Filter.filter_double(cPlane, Filter.Dx3FWindow(fType), PadType.replicate));       
            }

            return Result;
        }
    }

    public static class Smoothing
    {
        //only for RGB images, b&w 24bbp.
        public static void Smooth(Bitmap img, SmoothFilterWindow filWindow, SmoothInColorSpace cSpace)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width  = img.Width;
            int height = img.Height;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.getPixels(img);

            List<arraysListInt> Result = new List<arraysListInt>();

            double[,] filter;
            string outName = String.Empty;

            if (filWindow.ToString() == "window2")
            {
                filter = Filter.fspecial(2, 2, "average"); //default 3x3, still problem with filter more than
            }
            else
            {
                filter = Filter.fspecial(3, 3, "average"); //default 3x3, still problem with filter more than
            }            

            switch (cSpace.ToString())
            {
                case "RGB":
                    Result.Add(new arraysListInt() { c = ArrOp.ArrayToUint8(Filter.filter_double(ArrOp.ArrayToDouble(ColorList[0].c), filter, PadType.replicate)) });
                    Result.Add(new arraysListInt() { c = ArrOp.ArrayToUint8(Filter.filter_double(ArrOp.ArrayToDouble(ColorList[1].c), filter, PadType.replicate)) });
                    Result.Add(new arraysListInt() { c = ArrOp.ArrayToUint8(Filter.filter_double(ArrOp.ArrayToDouble(ColorList[2].c), filter, PadType.replicate)) });

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\SmoothRGB.jpg";
                    break;

                case "HSV":
                    var hsv = ColorSpace.rgb2hsv(img);

                    var hsv_temp = Filter.filter_double(hsv[2].c, filter, PadType.replicate);              

                    //Filter by V - Value (Brightness/яркость)
                    Result = ColorSpace.hsv2rgb(hsv[0].c, hsv[1].c, ArrOp.ToBorderGreaterZero(hsv_temp, 1)); //artificially if V > 1, make him 1

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\SmoothHSV.jpg";
                    break;

                case "Lab":
                    var lab = ColorSpace.rgb2lab(img);

                    var lab_temp = Filter.filter_double(lab[0].c, filter, PadType.replicate);                           

                    //Filter by L - lightness
                    Result = ColorSpace.lab2rgb(ArrOp.ToBorderGreaterZero(lab_temp, 255), lab[1].c, lab[2].c);

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\SmoothLab.jpg";
                    break;

                case "fakeCIE1976L":

                    var fakeCIE1976L = ColorSpace.rgb2lab(img);

                    var fakeCIE1976L_temp = Filter.filter_double(ArrOp.ArrayMultByConst(fakeCIE1976L[0].c, 2.57), filter, PadType.replicate); 

                    //Filter by L - lightness
                    Result = ColorSpace.lab2rgb(ArrOp.ArrayDivByConst(fakeCIE1976L_temp, 2.57), fakeCIE1976L[1].c, fakeCIE1976L[2].c);

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\SmoothfakeCIE1976L.jpg";

                    break;
            }

            image = Helpers.setPixels(image, Result[0].c, Result[1].c, Result[2].c);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }
    }

    //histeq realization only for input array
    public static class histeq
    {
        //only for RGB images, b&w 24bbp.
        //Ahtung! for HSV & Lab lost in accuracy, coz convert double->int->double
        public static void hist(Bitmap img, HisteqColorSpace cSpace)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width  = img.Width;
            int height = img.Height;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.getPixels(img);

            List<arraysListInt> Result = new List<arraysListInt>();
            string outName = String.Empty;

            switch (cSpace.ToString())
            {
                case "RGB":
                    Result.Add(new arraysListInt() { c = histeqHelper(ColorList[0].c)});
                    Result.Add(new arraysListInt() { c = histeqHelper(ColorList[1].c)});
                    Result.Add(new arraysListInt() { c = histeqHelper(ColorList[2].c) });

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\HisteqRGB.jpg";
                    break;

                case "HSV":
                    var hsv = ColorSpace.rgb2hsv(img);

                    var hsv_temp = histeqHelper(ArrOp.ImageArrayToUint8(hsv[2].c));                        

                    //Filter by V - Value (Brightness/яркость)                 
                    Result = ColorSpace.hsv2rgb(hsv[0].c, hsv[1].c, ArrOp.ToBorderGreaterZero(ArrOp.ImageUint8ToDouble(hsv_temp), 1)); //artificially if V > 1, make him 1

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\HisteqHSV.jpg";
                    break;

                case "Lab":
                    var lab = ColorSpace.rgb2lab(img);

                    var lab_temp = histeqHelper(ArrOp.ArrayToUint8(lab[0].c));

                    //Filter by L - lightness
                    Result = ColorSpace.lab2rgb(ArrOp.ToBorderGreaterZero(ArrOp.ArrayToDouble(lab_temp), 255), lab[1].c, lab[2].c);

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\HisteqLab.jpg";
                    break;

                case "fakeCIE1976L":

                    var fakeCIE1976L = ColorSpace.rgb2lab(img);

                    var fakeCIE1976L_temp = histeqHelper(ArrOp.ArrayToUint8(ArrOp.ArrayMultByConst(fakeCIE1976L[0].c, 2.57)));                        

                    //Filter by L - lightness
                    Result = ColorSpace.lab2rgb(ArrOp.ArrayDivByConst(ArrOp.ArrayToDouble(fakeCIE1976L_temp), 2.57), fakeCIE1976L[1].c, fakeCIE1976L[2].c);

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\HisteqfakeCIE1976L.jpg";

                    break;
            }

            image = Helpers.setPixels(image, Result[0].c, Result[1].c, Result[2].c);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        public static int[,] histeqHelper(int[,] cPlane)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            //gen array of the same values
            arrGen<double> d;
            d = new arrGen<double>();

            double eps = 2.2204 * Math.Pow(10, -16); 
            const int nDef   = 64; //default
            const int nUint8 = 256; //for uint8 image. At this time operationg only with them

            int[,] Result;

            //hgram contain integer counts for equally spaced bins with intensity values
            //here count it based on input array
            
            //array on ones 1 x nDef mult by input array length dived by nDef
            //obtain vector 1 x nDef size
            var hgram = ArrOp.VectorMultByConst(d.arrOfSingle(1, nDef, 1).Cast<double>().ToArray(), 
                (ArrOp.ArrayToDouble(cPlane).Cast<double>().ToArray().Length) / (double)nDef);

            //Normalize hgram. hgram = hgram mult by input array dived by hgram elements sum
            hgram = ArrOp.VectorMultByConst(hgram, (ArrOp.ArrayToDouble(cPlane).Cast<double>().ToArray().Length) / (hgram.Cast<double>().ToArray().Sum()));
            var m = hgram.Length;

            //compute Cumulative and Histogram
            var imhist = Contrast.imHist(cPlane); //imhist only for uint8 arrays realization ([0..255])

            double[] CumuSum = new double[nUint8];
            for (int i = 0; i < nUint8; i++)
            {
                if(i == 0)
                {
                    CumuSum[i] = imhist[i];
                }
                else
                {
                    CumuSum[i] = imhist[i] + CumuSum[i - 1];
                }                
            }

            //cumulative distribution function
            //create Transformation To Intensity Image
            var cumdInput = ArrOp.VectorMultByConst(hgram, (ArrOp.ArrayToDouble(cPlane).Cast<double>().ToArray().Length) / (hgram.Cast<double>().ToArray().Sum()));

            double[] cumd = new double[nDef];
            for (int i = 0; i < nDef; i++)
            {
                if (i == 0)
                {
                    cumd[i] = cumdInput[i];
                }
                else
                {
                    cumd[i] = cumdInput[i] + cumd[i - 1];
                }                
            }

            //Create transformation to an intensity image by minimizing the error
            //between desired and actual cumulative histogram.
            //tol saturates equal fractions at low and high pixel values

            //sory for this wtf code
            var z = new double[2 * imhist.Length];
            var partOne = imhist.ToList();
            partOne[nUint8 - 1] = 0;
            partOne.ToArray().CopyTo(z, 0);

            var partTwo = imhist.ToList();
            partTwo[0] = 0;
            partTwo.ToArray().CopyTo(z, imhist.Length);

            var tolPart = d.vecorToArrayRowByRow(1, nUint8, ArrOp.MinInColumns(d.vecorToArrayRowByRow(2, nUint8, z)));
            var tol = ArrOp.MultArrays(d.arrOfSingle(m, 1, 1), ArrOp.ArrayDivByConst(tolPart, 2));

            //Error
            var cumdArr = d.TransposeArray(d.vecorToArrayRowByRow(1, nDef, cumd));
            var CumuSumArr = d.vecorToArrayRowByRow(1, nUint8, CumuSum);

            var errPartOne = ArrOp.MultArrays(cumdArr, d.arrOfSingle(1, nUint8, 1));
            var errPartTwo = ArrOp.MultArrays(d.arrOfSingle(m, 1, 1), CumuSumArr);
            var err = ArrOp.SumArrays(ArrOp.SubArrays(errPartOne, errPartTwo), tol);
            
            //Find all places with error. Yep, wtf code continues!
            List<double> erroIndexes = new List<double>();

            //present array as vector col by col. Coz this is cool. Cast dont need!
            //Sorry, stupid copy step by step matlab logic           
            //var errVector = d.ArrayToVectorColByCol(err);

            //with cast faster a little?
            var errVector = err.Cast<double>().ToArray();
            for (int i = 0; i < errVector.Length; i++)
            {
                if (errVector[i] < - (cPlane.Length * Math.Sqrt(eps)))
                {
                    erroIndexes.Add(i);
                }
            }

            //fuck the error values
            double[,] rest = new double[err.GetLength(0), err.GetLength(1)];
            if (erroIndexes.Any())
            {
                var newErrVector = errVector.ToList();
                var newValue = ArrOp.VectorMultByConst(d.arrOfSingle(erroIndexes.Count(), 1, 1).Cast<double>().ToArray(), cPlane.Length); //coz erroIndexes - vector

                for (int i = 0; i < errVector.Length; i++)
                {
                    for (int j = 0; j < erroIndexes.Count(); j++)
                    {
                        if (i == erroIndexes[j])
                        {
                            newErrVector[i] = cPlane.Length; //newValue[j];
                        }
                    }                    
                }
                //rest = d.vecorToArrayColbyCol(rest.GetLength(0), rest.GetLength(1), newErrVector.ToArray());
                rest = d.vecorToArrayRowByRow(rest.GetLength(0), rest.GetLength(1), newErrVector.ToArray());
            }
            else
            {
                //err vector back to array
                rest = err;
            }       

            //find row number of mins in each col rest array
            double[] lut = new double[rest.GetLength(1)];
      
            double min;
            int index = 0;

            for (int i = 0; i < rest.GetLength(1); i++)
            {
                min = rest[0, i];
                index = 0;
                for (int j = 0; j < rest.GetLength(0); j++)
                {
                    if (min > rest[j, i])
                    {
                        min = rest[j, i];

                        index = j;
                    }
                }
                lut[i] = index;
            }

            //count lut
            for(int i = 0; i < lut.Length; i++)
            {
                lut[i] = lut[i] / (m - 1);
            }

            Result = ArrOp.ArrayToUint8(inlutHisteq(cPlane, lut));

            return Result;
        }

        //recount array using look up table (lut)
        public static double[,] inlutHisteq(int[,] arr, double[] lut)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            double[,] lutResult = new double[arr.GetLength(0), arr.GetLength(1)];            

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (arr[i, j] == 255) //bad condition
                    {
                        lutResult[i, j] = 255 * lut[arr[i, j]];                       
                    }
                    else
                    {
                        lutResult[i, j] = 255 * lut[arr[i, j] + 1];                     
                    }
                }
            }

            return lutResult;
        }

    }

    public enum unSharpInColorSpace
    {
        RGB,
        HSVi,
        HSVd,
        Labi,
        Labd,
        fakeCIE1976L
    }

    public enum FilterType
    {
        unsharp     = 0,
        Laplassian1 = 1,
        Laplassian2 = 2,
    }

    public enum SmoothInColorSpace
    {
        RGB,      
        HSV,     
        Lab,
        fakeCIE1976L
    }

    public enum SmoothFilterWindow
    {
        window2,
        window3
    }

    public enum HisteqColorSpace
    {
        RGB,      
        HSV,     
        Lab,
        fakeCIE1976L
    }
}
