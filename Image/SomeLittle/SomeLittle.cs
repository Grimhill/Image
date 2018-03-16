using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image
{
    //Some little operations with image
    public static class SomeLittle
    {
        #region Negative
        //make negative of image

        public static void MakeNegativeAndBack(Bitmap img)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            image = MakeNegativeAndBackHelper(img);

            string outName = defPath + imgName + "_NegativeOrRestored" + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        public static Bitmap MakeNegativeAndBackBitmap(Bitmap img)
        {
            return MakeNegativeAndBackHelper(img);
        }

        private static Bitmap MakeNegativeAndBackHelper(Bitmap img)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            List<ArraysListInt> ColorList = Helpers.GetPixels(img);

            var Rcn = ColorList[0].Color.ConstSubArrayElements(255); //R plane  
            var Gcn = ColorList[1].Color.ConstSubArrayElements(255); //G plane  
            var Bcn = ColorList[2].Color.ConstSubArrayElements(255); //B plane

            image = Helpers.SetPixels(image, Rcn, Gcn, Bcn);

            if (Depth == 8)
            { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }
            else if(Depth == 1)
            { image = PixelFormatWorks.ImageTo1BppBitmap(image); }

            return image;
        }

        #endregion

        #region GammaCorrection
        //make gamma correction of image or image color plane

        private static List<string> GammaVariant = new List<string>()
        { "_GammaCorrectionR", "_GammaCorrectionG", "_GammaCorrectionB", "_GammaCorrection" };

        //higher multCoeff and gamma - lighter image after correction   
        public static void GammaCorrectionFun(Bitmap img, GammaPlane plane, double multCoeff, double gamma) //multCoeff = 40 gamma = 0.3
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            image = GammaCorrectionHelper(img, plane, multCoeff, gamma);

            int index = 0;
            foreach (var n in Enum.GetValues(typeof(GammaPlane)).Cast<GammaPlane>())
            {
                if (Depth == 8) { index = 3; break; }//ugly
                if (n == plane) { index = (int)n; break; };                
            }

            string outName = defPath + imgName + GammaVariant.ElementAt(index) + "c_" + multCoeff + "_gamma_" + gamma + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        public static Bitmap GammaCorrectionFunBitmap(Bitmap img, GammaPlane plane, double multCoeff, double gamma)
        {
            return GammaCorrectionHelper(img, plane, multCoeff, gamma);
        }

        private static Bitmap GammaCorrectionHelper(Bitmap img, GammaPlane plane, double multCoeff, double gamma)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);

            if (!Checks.BinaryInput(img))
            { 
                switch (plane)
                {
                    case GammaPlane.GammaR:
                        resultR = ColorList[0].Color.ArrayToDouble().PowArrayElements(gamma).ArrayMultByConst(multCoeff).ArrayToUint8(); //R plane  
                        resultG = ColorList[1].Color; resultB = ColorList[2].Color;
                        break;

                    case GammaPlane.GammaG:
                        resultG = ColorList[1].Color.ArrayToDouble().PowArrayElements(gamma).ArrayMultByConst(multCoeff).ArrayToUint8(); //G plane  
                        resultR = ColorList[0].Color; resultB = ColorList[2].Color;
                        break;

                    case GammaPlane.GammaB:
                        resultB = ColorList[2].Color.ArrayToDouble().PowArrayElements(gamma).ArrayMultByConst(multCoeff).ArrayToUint8(); //B plane
                        resultR = ColorList[0].Color; resultG = ColorList[1].Color;
                        break;

                    case GammaPlane.GammaRGB:
                        resultR = ColorList[0].Color.ArrayToDouble().PowArrayElements(gamma).ArrayMultByConst(multCoeff).ArrayToUint8(); //R plane            
                        resultG = ColorList[1].Color.ArrayToDouble().PowArrayElements(gamma).ArrayMultByConst(multCoeff).ArrayToUint8(); //G plane         
                        resultB = ColorList[2].Color.ArrayToDouble().PowArrayElements(gamma).ArrayMultByConst(multCoeff).ArrayToUint8(); //B plane
                        break;
                }

                image = Helpers.SetPixels(image, resultR, resultG, resultB);

                if (Depth == 8)
                { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }
               
            }
            else { Console.WriteLine("What did you expected to make gamma correction with binaty image? Return black square."); }

            return image;
        }

        #endregion

        #region Mirror
        //obtain mirror of image from selected side

        private static List<string> MirrorVariant = new List<string>()
        { "_mirrorLeft", "_mirrorRight", "_mirrorTop", "_mirrorBot" };

        public static void Mirror(Bitmap img, MirrorOption side)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            image = MirrorHelper(img, side);

            int index = 0;
            foreach (var n in Enum.GetValues(typeof(MirrorOption)).Cast<MirrorOption>())
            {
                if (n == side) { index = (int)n; break; };
            }

            string outName = defPath + imgName + MirrorVariant.ElementAt(index) + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        public static Bitmap MirrorBitmap(Bitmap img, MirrorOption side)
        {
            return MirrorHelper(img, side);
        }

        private static Bitmap MirrorHelper(Bitmap img, MirrorOption side)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];

            PadMyArray<int> padArr = new PadMyArray<int>();

            if (side == MirrorOption.left || side == MirrorOption.right)
            {
                resultR = padArr.PadArray(ColorList[0].Color, 0, img.Width, PadType.symmetric, Direction.pre);
                resultG = padArr.PadArray(ColorList[1].Color, 0, img.Width, PadType.symmetric, Direction.pre);
                resultB = padArr.PadArray(ColorList[2].Color, 0, img.Width, PadType.symmetric, Direction.pre);
            }
            else
            {
                resultR = padArr.PadArray(ColorList[0].Color, img.Height, 0, PadType.symmetric, Direction.pre);
                resultG = padArr.PadArray(ColorList[1].Color, img.Height, 0, PadType.symmetric, Direction.pre);
                resultB = padArr.PadArray(ColorList[2].Color, img.Height, 0, PadType.symmetric, Direction.pre);
            }

            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            if (Depth == 8)
            { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }
            else if (Depth == 1)
            { image = PixelFormatWorks.ImageTo1BppBitmap(image); }

            return image;
        }

        #endregion

        #region CropImage
        //crom image by entered values form sides

        public static void CropImage(Bitmap img, int cutLeft, int cutRight, int cutTop, int cutBottom)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            image = CropImageHelper(img, cutLeft, cutRight, cutTop, cutBottom);

            string outName = defPath + imgName + "_cropped" + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        public static Bitmap CropImageBitmap(Bitmap img, int cutLeft, int cutRight, int cutTop, int cutBottom)
        {
            return CropImageHelper(img, cutLeft, cutRight, cutTop, cutBottom);
        }

        private static Bitmap CropImageHelper(Bitmap img, int cutLeft, int cutRight, int cutTop, int cutBottom)
        {
            //count new width and height
            int newWidth  = img.Width - cutLeft - cutRight;
            int newHeight = img.Height - cutTop - cutBottom;

            Bitmap cropped = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            if (newWidth <= 0 || newHeight <= 0 || 
                cutLeft < 0 || cutRight < 0|| cutTop < 0 || cutBottom < 0)
            {
                Console.WriteLine("Crop width or height more than image`s one. Or entered negative value.");
            }
            else
            {
                //left x-coordinate of the upper-left corner of the rectangle.
                //right y-coordinate of the upper-left corner of the rectangle.
                Rectangle rect = new Rectangle(cutLeft, cutTop, newWidth, newHeight);

                //clone selected area into new bitmap object
                cropped = img.Clone(rect, img.PixelFormat);
                //cropped = img.Clone(rect, PixelFormat.Format24bppRgb);
            }

            return cropped;
        }

        #endregion

        #region Inverse
        //Invert binaty image or colored by some plane

        public static void InverseBinary(Bitmap img, OutType type)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            image = InverseBinaryHelper(img, type);

            string outName = defPath + imgName + "_InverseBinary" + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        public static Bitmap InverseBinaryBitmap(Bitmap img, OutType type)
        {
            return InverseBinaryHelper(img, type);
        }

        private static Bitmap InverseBinaryHelper(Bitmap img, OutType type)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];

            if(Checks.BWinput(img))
            {
                List<ArraysListInt> ColorList = Helpers.GetPixels(img);
                result = MoreHelpers.InvertBinaryArray(ColorList[0].Color);

                if (result.Cast<int>().Max() == 1)
                {
                    for (int i = 0; i < result.GetLength(0); i++)
                    {
                        for (int j = 0; j < result.GetLength(1); j++)
                        {
                            if (result[i, j] == 1)
                                result[i, j] = 255;
                        }
                    }
                }

                image = Helpers.SetPixels(image, result, result, result);

                if (type == OutType.OneBpp)
                    image = PixelFormatWorks.ImageTo1BppBitmap(image, 0.5);

                else if (type == OutType.EightBpp)
                    image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image);
            }

            return image;
        }

        public static void InvertColorPlane(Bitmap img, InveseVariant variant)
        {
            string ImgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            image = InvertColorPlaneHelper(img, variant);

            string outName = defPath + imgName + "_" + variant.ToString() + ImgExtension;
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        public static Bitmap InvertColorPlaneBitmap(Bitmap img, InveseVariant variant)
        {
            return InvertColorPlaneHelper(img, variant);
        }

        public static Bitmap InvertColorPlaneHelper(Bitmap img, InveseVariant variant)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];

            if (Checks.RGBinput(img))
            {
                List<ArraysListInt> ColorList = Helpers.GetPixels(img);

                switch (variant)
                {
                    case InveseVariant.InverseR:
                        resultR = ColorList[0].Color.ConstSubArrayElements(255);
                        resultG = ColorList[1].Color; resultB = ColorList[2].Color;
                        break;

                    case InveseVariant.InverseG:
                        resultG = ColorList[1].Color.ConstSubArrayElements(255);
                        resultR = ColorList[0].Color; resultB = ColorList[2].Color;
                        break;

                    case InveseVariant.InverseB:
                        resultB = ColorList[2].Color.ConstSubArrayElements(255);
                        resultR = ColorList[0].Color; resultG = ColorList[1].Color;
                        break;

                    case InveseVariant.InverseRG:
                        resultR = ColorList[0].Color.ConstSubArrayElements(255);
                        resultG = ColorList[1].Color.ConstSubArrayElements(255);
                        resultB = ColorList[2].Color;
                        break;

                    case InveseVariant.InverseRB:
                        resultR = ColorList[0].Color.ConstSubArrayElements(255);
                        resultB = ColorList[2].Color.ConstSubArrayElements(255);
                        resultG = ColorList[1].Color;
                        break;

                    case InveseVariant.InverseGB:
                        resultG = ColorList[1].Color.ConstSubArrayElements(255);
                        resultB = ColorList[2].Color.ConstSubArrayElements(255);
                        resultR = ColorList[0].Color;
                        break;

                    case InveseVariant.InverseRGB: //negative
                        resultR = ColorList[0].Color.ConstSubArrayElements(255);
                        resultG = ColorList[1].Color.ConstSubArrayElements(255);
                        resultB = ColorList[2].Color.ConstSubArrayElements(255);
                        break;
                }
            }
            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            return image;
        }

        #endregion

        #region SetAlpha
        //set alpha for image

        public static void SetAlpha(Bitmap img, double alpha)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Rand");

            Bitmap image = new Bitmap(img.Width, img.Height);
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (Depth == 24)
            {
                image = Helpers.SetPixelsAlpha(img, alpha);
            }
            else { Console.WriteLine("Cant implement alpha channel to non-24bpp image. Method: SomeLittle.SetAlpha(Bitmap img, double alpha). Return Black Rectnagle."); }

            string outName = defPath + imgName + "_imageAlpha_" + alpha.ToString() + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        #endregion 

        //save image in othe format, your cap
        public static void SaveImageInOtherFormat(Bitmap image, SupportFormats newFormat)
        {
            string imgName = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath = GetImageInfo.MyPath("Rand");

            string outName = defPath + imgName + newFormat.ToString();
            Helpers.SaveOptions(image, outName, newFormat.ToString().ToLower());
        }
    }

    public enum GammaPlane
    {
        GammaR,
        GammaG,
        GammaB,
        GammaRGB
    }

    public enum MirrorOption
    {
        left,
        right,
        top,
        bot
    }

    public enum InveseVariant
    {
        InverseR,
        InverseG,
        InverseB,
        InverseRG,
        InverseRB,
        InverseGB,
        InverseRGB
    }

    public enum SupportFormats
    {
        jpg,
        jpeg,
        png,
        bmp,
        tiff
    }
}


