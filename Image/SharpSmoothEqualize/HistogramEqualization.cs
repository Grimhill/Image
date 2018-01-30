using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Image.ColorSpaces;
using Image.ArrayOperations;

namespace Image.SharpSmoothEqualize
{
    //histeq realization only for input array
    public static class HistogramEqualization
    {
        //only for RGB images, b&w 24bbp.
        //Ahtung! for HSV & Lab lost in accuracy, coz convert double->int->double
        public static void Equalize(Bitmap img, HisteqColorSpace cSpace, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            string defPass = Directory.GetCurrentDirectory() + "\\Sharp\\Histeq\\";
            Checks.DirectoryExistance(defPass);

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);

            List<ArraysListInt> Result = new List<ArraysListInt>();
            string outName = String.Empty;

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            if (Depth == 8) { ImgExtension = ".png"; }

            switch (cSpace)
            {
                case HisteqColorSpace.RGB:
                    Result.Add(new ArraysListInt() { Color = HisteqHelper(ColorList[0].Color) });
                    Result.Add(new ArraysListInt() { Color = HisteqHelper(ColorList[1].Color) });
                    Result.Add(new ArraysListInt() { Color = HisteqHelper(ColorList[2].Color) });

                    outName = defPass + fileName + "_HisteqRGB" + ImgExtension;
                    break;

                case HisteqColorSpace.HSV:
                    var hsv = RGBandHSV.RGB2HSV(img);
                    var hsv_temp = HisteqHelper((hsv[2].Color).ImageArrayToUint8());

                    //Filter by V - Value (Brightness/яркость)                 
                    //artificially if V > 1, make him 1
                    Result = RGBandHSV.HSV2RGB(hsv[0].Color, hsv[1].Color, hsv_temp.ImageUint8ToDouble().ToBorderGreaterZero(1));

                    outName = defPass + fileName + "_HisteqHSV" + ImgExtension;
                    break;

                case HisteqColorSpace.Lab:
                    var lab = RGBandLab.RGB2Lab(img);
                    var lab_temp = HisteqHelper((lab[0].Color).ArrayToUint8());

                    //Filter by L - lightness                   
                    Result = RGBandLab.Lab2RGB(lab_temp.ArrayToDouble().ToBorderGreaterZero(255), lab[1].Color, lab[2].Color);

                    outName = defPass + fileName + "_HisteqLab" + ImgExtension;
                    break;

                case HisteqColorSpace.fakeCIE1976L:
                    var fakeCIE1976L = RGBandLab.RGB2Lab(img);
                    var fakeCIE1976L_temp = HisteqHelper((fakeCIE1976L[0].Color).ArrayMultByConst(2.57).ArrayToUint8());

                    //Filter by L - lightness                
                    Result = RGBandLab.Lab2RGB(fakeCIE1976L_temp.ArrayToDouble().ArrayDivByConst(2.57), fakeCIE1976L[1].Color, fakeCIE1976L[2].Color);

                    outName = defPass + fileName + "_HisteqfakeCIE1976L" + ImgExtension;
                    break;
            }

            image = Helpers.SetPixels(image, Result[0].Color, Result[1].Color, Result[2].Color);
            outName = Checks.OutputFileNames(outName);

            if (Depth == 8)
            { image = MoreHelpers.Bbp24Gray2Gray8bppHelper(image); }

            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        private static int[,] HisteqHelper(int[,] cPlane)
        {
            //gen array of the same values
            ArrGen<double> d = new ArrGen<double>();

            double eps = 2.2204 * Math.Pow(10, -16);
            const int nDef = 64; //default
            const int nUint8 = 256; //for uint8 image. At this time operationg only with them

            int[,] Result;

            //hgram contain integer counts for equally spaced bins with intensity values
            //here count it based on input array

            //array on ones 1 x nDef mult by input array length dived by nDef
            //obtain vector 1 x nDef size            
            var hgram = (d.ArrOfSingle(1, nDef, 1).Cast<double>().ToArray()).VectorMultByConst((cPlane.ArrayToDouble().Cast<double>().ToArray().Length) / (double)nDef);

            //Normalize hgram. hgram = hgram mult by input array dived by hgram elements sum            
            hgram = hgram.VectorMultByConst((cPlane.ArrayToDouble().Cast<double>().ToArray().Length) / (hgram.Cast<double>().ToArray().Sum()));
            var m = hgram.Length;

            //compute Cumulative and Histogram
            var imhist = ImHist(cPlane); //imhist only for uint8 arrays realization ([0..255])

            double[] CumuSum = new double[nUint8];
            for (int i = 0; i < nUint8; i++)
            {
                if (i == 0)
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
            var cumdInput = hgram.VectorMultByConst((cPlane.ArrayToDouble().Cast<double>().ToArray().Length) / (hgram.Cast<double>().ToArray().Sum()));

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

            var tolPart = d.VecorToArrayRowByRow(1, nUint8, (d.VecorToArrayRowByRow(2, nUint8, z)).MinInColumns());

            var tol = (d.ArrOfSingle(m, 1, 1)).MultArrays(tolPart.ArrayDivByConst(2));

            //Error
            var cumdArr = d.TransposeArray(d.VecorToArrayRowByRow(1, nDef, cumd));
            var CumuSumArr = d.VecorToArrayRowByRow(1, nUint8, CumuSum);

            var errPartOne = cumdArr.MultArrays(d.ArrOfSingle(1, nUint8, 1));

            var errPartTwo = (d.ArrOfSingle(m, 1, 1)).MultArrays(CumuSumArr);

            var err = errPartOne.SubArrays(errPartTwo).SumArrays(tol);

            //Find all places with error. Yep, wtf code continues!
            List<double> erroIndexes = new List<double>();

            //present array as vector col by col. Coz this is cool. Cast dont need!
            //Sorry, stupid copy step by step matlab logic           
            //var errVector = d.ArrayToVectorColByCol(err);

            //with cast faster a little?
            var errVector = err.Cast<double>().ToArray();
            for (int i = 0; i < errVector.Length; i++)
            {
                if (errVector[i] < -(cPlane.Length * Math.Sqrt(eps)))
                {
                    erroIndexes.Add(i);
                }
            }

            //fuck the error values
            double[,] rest = new double[err.GetLength(0), err.GetLength(1)];
            if (erroIndexes.Any())
            {
                var newErrVector = errVector.ToList();
                var newValue = (d.ArrOfSingle(erroIndexes.Count(), 1, 1).Cast<double>().ToArray()).VectorMultByConst(cPlane.Length); //coz erroIndexes - vector

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

                rest = d.VecorToArrayRowByRow(rest.GetLength(0), rest.GetLength(1), newErrVector.ToArray());
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
            for (int i = 0; i < lut.Length; i++)
            {
                lut[i] = lut[i] / (m - 1);
            }

            Result = (InlutHisteq(cPlane, lut)).ArrayToUint8();

            return Result;
        }

        private static int[] ImHist(int[,] Im)
        {
            int[] tempData = Im.Cast<int>().ToArray();
            int[] imHistResult = new int[256]; //uint8 size
            int count = 0;

            int[] temp = new int[256];
            for (int k = 0; k < temp.Length; k++)
            {
                temp[k] = k;
            }

            for (int i = 0; i < 256; i++)
            {
                count = 0;
                for (int j = 0; j < tempData.Length; j++)
                {
                    if (temp[i] == tempData[j])
                    {
                        count++;
                    }
                }
                imHistResult[i] = count;
            }
            return imHistResult;
        }

        //recount array using look up table (lut)
        private static double[,] InlutHisteq(int[,] arr, double[] lut)
        {
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

    public enum HisteqColorSpace
    {
        RGB,
        HSV,
        Lab,
        fakeCIE1976L
    }
}
