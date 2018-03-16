using System;
using System.Linq;
using Image.ArrayOperations;

namespace Image.CotrastProcess
{
    public static class ContrastProcess
    {
        
        public static double[] Stretchlims(int[,] grayColor, double IntensityProcent)
        {
            return StretchlimsCount(grayColor, IntensityProcent);
        }

        public static double[] Stretchlims(int[,] grayColor, double low, double high)
        {
            return StretchlimsCount(grayColor, low, high);
        }        

        public static double[] CountLut(double[] inLim, double[] outLim, double gamma)
        {
            return CountLutProcess(inLim, outLim, gamma);
        }

        public static int[,] InlutContrast(int[,] im, double[] lut)
        {
            return InlutContrastProcess(im, lut);
        }


        //Find limits to contrast stretch an image\\ Using for default BW contrast
        //IntensityProcent - intensity in % pixels saturated at low and high intensities of image
        private static double[] StretchlimsCount(int[,] grayColor, double intensityProcent)
        {
            double[] imLH = new double[2]; //contatin a pair of gray values, which represent image low & high limits to contrast stretch an image
            double[] tol  = new double[2]; //tol saturates equal fractions at low and high pixel values

            if (intensityProcent == 0.01)
            {
                tol[0] = .01; //default
                tol[1] = .99; //default
            }
            else
            {
                tol[0] = intensityProcent;
                tol[1] = 1 - intensityProcent;
            }

            if (tol[0] < tol[1]) // tol[0] - low, tol[1] - high
            {
                var ImHist = ContrastProcess.ImHist(grayColor); //obtain img histohram

                int[] CumulativeSum = new int[256];
                //CumulativeSum[0] = ImHist[0];
                for (int i = 0; i < 256; i++)
                {
                    if (i == 0)
                    {
                        CumulativeSum[i] = ImHist[i];
                    }
                    else
                    {
                        CumulativeSum[i] = ImHist[i] + CumulativeSum[i - 1];
                    }
                }

                //cumulative distribution function             
                var cdf   = CumulativeSum.VectorToDouble().VectorDivByConst(ImHist.Sum());
                var ilow  = Array.IndexOf(cdf, cdf.First(x => x > tol[0])); //index first low
                var ihigh = Array.IndexOf(cdf, cdf.First(x => x >= tol[1])); //index first high

                if (ilow == ihigh) //this could happen if img is flat
                {
                    //no implementation exception c(: ImLH[0, 0] = 1; ImLH[1, 0] = 256;
                }
                else
                {
                    imLH[0] = ((double)ilow - 1) / 255; imLH[1] = ((double)ihigh - 1) / 255; //convert to range [0 1]
                }
            }
            else
            {
                imLH[0] = 0; imLH[1] = 1;
            }

            return imLH;
        }

        //if want use for RGB image, input Rc, Gc, Bc args. Trying for def lims such as for balckwhite image
        private static double[] StretchlimsCount(int[,] grayColor, double low, double high)
        {
            double[] imLH = new double[2];
            double[] tol  = { low, high };

            if (tol[0] < tol[1]) // tol[0] - low, tol[1] - high
            {
                var ImHist = ContrastProcess.ImHist(grayColor); //obtain img histohram

                int[] CumulativeSum = new int[256];
                //CumulativeSum[0] = ImHist[0];
                for (int i = 0; i < 256; i++)
                {
                    if (i == 0)
                    {
                        CumulativeSum[i] = ImHist[i];
                    }
                    else
                    {
                        CumulativeSum[i] = ImHist[i] + CumulativeSum[i - 1];
                    }
                }

                //cumulative distribution function               
                var cdf   = CumulativeSum.VectorToDouble().VectorDivByConst(ImHist.Sum());
                var ilow  = Array.IndexOf(cdf, cdf.First(x => x > tol[0])); //index first low
                var ihigh = Array.IndexOf(cdf, cdf.First(x => x >= tol[1])); //index first high

                if (ilow == ihigh) //this could happen if img is flat
                {
                    //no implementation exception c(: ImLH[0, 0] = 1; ImLH[1, 0] = 256;
                }
                else
                {
                    imLH[0] = ((double)ilow - 1) / 255; imLH[1] = ((double)ihigh - 1) / 255;
                }
            }
            else
            {
                imLH[0] = 0; imLH[1] = 1;
            }

            return imLH;
        }


        //image histogram (for BW or RC\GC\Bc)
        //only for uint8 images realization
        private static int[] ImHist(int[,] arr) //imHist(int[,] Im, n) where n - uint size. 8 - 256, 16 - 65536
        {
            int[] tempData     = arr.Cast<int>().ToArray();
            int[] imHistResult = new int[256];
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


        //count Look up table
        private static double[] CountLutProcess(double[] inLim, double[] outLim, double gamma)
        {
            //prepare for look up table, receive vector of doubles for uint8 [0 255]
            //Cumulating sum of elemets 1/256
            double[] temp = new double[256];
            for (int k = 1; k < temp.Length; k++)
            {
                temp[k] = temp[k - 1] + (double)1 / (double)256;
            }

            double[] Lut = new double[256];

            if (outLim[0] == 0 && outLim[1] == 1) //if out [0 1]
            {
                //(arr - low_in) ./ (high_in - low_in)) .^ gamma               
                Lut = temp.VectorSubConst(inLim[0]).VectorDivByConst((inLim[1] - inLim[0])).PowVectorElements(gamma);

                for (int i = 0; i < Lut.Length; i++)
                {
                    if (Lut[i] < 0) { Lut[i] = 0; }
                    else if (Lut[i] > 1) { Lut[i] = 1; }
                }
            }
            else if (outLim[0] == 1 && outLim[1] == 0) //if out [1 0]
            {
                //(arr - low_in) ./ (high_in - low_in)) .^ gamma                
                //1 - result               
                Lut = temp.VectorSubConst(inLim[0]).VectorDivByConst((inLim[1] - inLim[0])).PowVectorElements(gamma).ConstSubVectorElements(1);

                for (int i = 0; i < Lut.Length; i++)
                {
                    if (Lut[i] < 0) { Lut[i] = 0; }
                    else if (Lut[i] > 1) { Lut[i] = 1; }
                }
            }
            else //if out in range [0 1] or [1 0]
            {
                //low_in & low_out
                //(arr < low_in) .* low_out
                double[] partOne = new double[256];
                for (int i = 0; i < partOne.Length; i++)
                {
                    if (temp[i] < inLim[0])
                    {
                        partOne[i] = outLim[0];
                    }
                    else { partOne[i] = 0; }
                }

                //arr >= low_in & arr < high_in
                double[] partTwo = new double[256];
                for (int i = 0; i < partTwo.Length; i++)
                {
                    if (temp[i] >= inLim[0] & temp[i] < inLim[1])
                    {
                        partTwo[i] = 1;
                    }
                    else { partTwo[i] = 0; }
                }

                //(arr >= hign_in) .* high_out
                double[] partThree = new double[256];
                for (int i = 0; i < partThree.Length; i++)
                {
                    if (temp[i] >= inLim[1])
                    {
                        partThree[i] = outLim[1];
                    }
                    else { partThree[i] = 0; }
                }

                //  1) = (arr < low_in) .* low_out
                //  2) = (1) + (arr >= low_in & arr < high_in) .* (low_out + (high_out - low_out) .* ((arr - low_in) ./ (high_in - low_in)) .^ gamma)
                //  3) = (2) + (arr >= hign_in) .* high_out                

                Lut = temp.VectorSubConst(inLim[0]).VectorDivByConst((inLim[1] - inLim[0])).PowVectorElements(gamma);
                Lut = Lut.VectorMultByConst((outLim[1] - outLim[0])).VectorSumConst(outLim[0]);

                //sum with arr >= low_in & arr < high_in                
                Lut = Lut.MultVectors(partTwo);

                //sum with (arr < low_in) .* low_out               
                Lut = partOne.SumVectors(Lut);

                //sum with (arr >= hign_in) .* high_out                
                Lut = Lut.SumVectors(partThree);
            }

            return Lut;
        }


        //redefine array using look up table 
        private static int[,] InlutContrastProcess(int[,] im, double[] lut)
        {
            int[,] lutResult = new int[im.GetLength(0), im.GetLength(1)];
            var luts = lut.ImageVectorToUint8();

            for (int i = 0; i < im.GetLength(0); i++)
            {
                for (int j = 0; j < im.GetLength(1); j++)
                {
                    if (im[i, j] == 255) //bad condition
                    {
                        lutResult[i, j] = luts[im[i, j]];
                    }
                    else
                    {
                        lutResult[i, j] = luts[im[i, j] + 1];
                    }
                }
            }

            return lutResult;
        }
    }
}
