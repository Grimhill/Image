using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image
{
    //fogive me...    
    public static class NoiseTry
    {
        public static void SaltandPepperNoise(Bitmap img, double density, SaltandPapperNoise noiseType)
        {
            SaltandPepperShapkaProcess(img, density, density, noiseType);
        }
        //default
        public static void SaltandPepperNoise(Bitmap img, SaltandPapperNoise noiseType)
        {
            double density = 0.05;
            SaltandPepperShapkaProcess(img, density, density, noiseType);
        }

        public static void SaltandPepperNoise(Bitmap img, double densitySalt, double densityPepper)
        {
            SaltandPepperShapkaProcess(img, densitySalt, densityPepper, SaltandPapperNoise.saltandpepper);
        }

        delegate int[,] SaltPepper(int[,] arr);

        private static void SaltandPepperShapkaProcess(Bitmap img, double densitySalt, double densityPepper, SaltandPapperNoise noiseType)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Noise");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            string outName = string.Empty;

            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];

            if (densitySalt > 0 && densitySalt <= 1 && densityPepper > 0 && densityPepper <= 1)
            {
                int white = 1;
                double[,] noise = Helpers.RandArray(img.Height, img.Width);

                if (Depth != 1 && Depth != 8)
                {
                    white = 255;
                    resultR = SaltandPepper(ColorList[0].Color, noise, densitySalt, densityPepper, white, noiseType);
                    resultG = SaltandPepper(ColorList[1].Color, noise, densitySalt, densityPepper, white, noiseType);
                    resultB = SaltandPepper(ColorList[2].Color, noise, densitySalt, densityPepper, white, noiseType);
                }
                else
                {
                    resultR = SaltandPepper(ColorList[0].Color, noise, densitySalt, densityPepper, white, noiseType);
                    resultG = resultR; resultB = resultR;
                }

                image = Helpers.SetPixels(image, resultR, resultG, resultB);

                if (Depth == 8) { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }
                if (Depth == 1) { image = PixelFormatWorks.ImageTo1BppBitmap(image, 0.5); }
            }
            else { Console.WriteLine("Density must be in range [0..1]. Method NoiseTry.SaltandPepperNoise(). Return black rectangle."); }

            if(noiseType == SaltandPapperNoise.salt)
                outName = defPath + imgName + "_SaltNoise_Density_" + densitySalt + imgExtension;
            else if(noiseType == SaltandPapperNoise.pepper)
                outName = defPath + imgName + "_PepperNoise_Density_" + densityPepper + imgExtension;
            else
                outName = defPath + imgName + "_SalptandPepperNoise_SaltDensity_" + densitySalt + "_PepperDensity_" + densityPepper + imgExtension;

            Helpers.SaveOptions(image, outName, imgExtension);
        }

        private static int[,] SaltandPepper(int[,] arr, double[,] noise, double densitySalt, double densityPepper, int white, SaltandPapperNoise noiseType)
        {
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];
            ArrGen<int> d = new ArrGen<int>();
            
            var vectorArr = arr.Cast<int>().ToArray();
            var vectorNoise = noise.Cast<double>().ToArray();            

            switch (noiseType)
            {
                case SaltandPapperNoise.salt:
                    for(int i = 0; i < noise.Length; i++)
                    { if(vectorNoise[i] >= (1 - densitySalt/2)) { vectorArr[i] = white; } }                   
                    result = d.VecorToArrayRowByRow(arr.GetLength(0), arr.GetLength(1), vectorArr);
                    break;
                case SaltandPapperNoise.pepper:
                    for (int i = 0; i < noise.Length; i++)
                    { if (vectorNoise[i] <= densityPepper / 2) { vectorArr[i] = 0; } }
                    result = d.VecorToArrayRowByRow(arr.GetLength(0), arr.GetLength(1), vectorArr);
                    break;
                case SaltandPapperNoise.saltandpepper:
                    for (int i = 0; i < noise.Length; i++)
                    { if (vectorNoise[i] >= (1 - densitySalt / 2)) { vectorArr[i] = white; } }
                    for (int i = 0; i < noise.Length; i++)
                    { if (vectorNoise[i] <= densityPepper / 2) { vectorArr[i] = 0; } }
                    result = d.VecorToArrayRowByRow(arr.GetLength(0), arr.GetLength(1), vectorArr);
                    break;
            }

            return result;
        }


        public static void SpeckleNoise(Bitmap img, double variance)
        {
            SpeckleShapkaProcess(img, variance);
        }
        //default
        public static void SpeckleNoise(Bitmap img)
        {
            double variance = 0.05;
            SpeckleShapkaProcess(img, variance);
        }

        private static void SpeckleShapkaProcess(Bitmap img, double variance)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Noise");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
           
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];

            if (variance > 0)
            {               
                double[,] noise = Helpers.RandArray(img.Height, img.Width);

                if (Depth != 1 && Depth != 8)
                {
                    resultR = Speckle(ColorList[0].Color, variance, noise);
                    resultG = Speckle(ColorList[1].Color, variance, noise);
                    resultB = Speckle(ColorList[2].Color, variance, noise);
                }
                else
                {
                    Depth = 8;
                    resultR = Speckle(ColorList[0].Color, variance, noise);
                    resultG = resultR; resultB = resultR;
                }

                image = Helpers.SetPixels(image, resultR, resultG, resultB);

                if (Depth == 8) { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }                
            }
            else { Console.WriteLine("Variance must be greater 0. Method NoiseTry.SpeckleNoise(). Return black rectangle."); }

            string outName = defPath + imgName + "_speckleNoise_variance_" + variance + imgExtension;

            Helpers.SaveOptions(image, outName, imgExtension);
        }

        private static int[,] Speckle(int[,] arr, double variance, double[,] noise)
        {
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];            

            var temp = noise.ArrayMultByConst(Math.Sqrt(variance)).ArraySumWithConst(1);

            result = arr.ImageUint8ToDouble().ArrayMultElements(temp).ImageArrayToUint8();

            return result;
        }
    }

    public enum SaltandPapperNoise
    {
        salt,
        pepper,
        saltandpepper
    }
}
