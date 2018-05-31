using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Image.ArrayOperations;
using Image.ColorSpaces;

//Scary
//Obtain presented color space; and write them to file
namespace Image
{
    public static class ColorSpace
    {
        //obtain L component as CIE1976 experiment, somewhere find coefficient, but less a bit ~ 2.4 :3
        public static double[,] FakeCIE1976L(Bitmap img)
        {
            List<ArraysListDouble> lab = RGBandLab.RGB2Lab(img);

            double[,] L = lab[0].Color;
            L = L.ArrayMultByConst(2.57);

            return L;
        }
    }

    //Sorry for such Curved Hands realization and method
    public static class ColorSpaceToFile
    {  
        //all 2rgb looks good, if obtained not from file, but from rgb and made some filtering or another process, and saved as rgb back
        //list at input - coz bitmap object contain uint values (byte), and we can lost some color difference in other color space represent is
        public static void AnothercolorSpacetoRGBXYZLabtoFile(List<ArraysListDouble> colorPlanes, AnotherColorSpacetoRGBaXYZLab colorSpace, string nameForImage)
        {
            string defPath = GetImageInfo.MyPath("ColorSpace") + nameForImage;

            int width  = colorPlanes[0].Color.GetLength(1);
            int height = colorPlanes[0].Color.GetLength(0);
            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            //back result [0 .. 255]
            int[,] colorPlaneOne   = new int[height, width];
            int[,] colorPlaneTwo   = new int[height, width];
            int[,] colorPlaneThree = new int[height, width];

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();
            string outName = String.Empty;

            if (colorPlanes[0].Color.Length != colorPlanes[1].Color.Length || colorPlanes[0].Color.Length != colorPlanes[2].Color.Length)
            {
                Console.WriteLine("Image plane arrays size dismatch in operation -> " +
                    "AnothercolorSpacetoRGBXYZLabtoFile(List<ArraysListDouble> Colors, AnotherColorSpacetoRGBaXYZLab colorSpace) <-");
            }
            else
            {
                switch (colorSpace.ToString())
                {
                    case "hsv2rgb":
                        rgbResult = RGBandHSV.HSV2RGB(colorPlanes);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        outName = defPath + "_hsv2rgb.jpeg";
                        break;

                    case "ntsc2rgb":
                        rgbResult = RGBandNTSC.NTSC2RGB(colorPlanes);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //when ntsc2rgb from file
                        //approximate result in file, coz we lost negative values in I and Q, when saving ntsc result in file [0..255]
                        outName = defPath + "_ntsc2rgb.jpeg";
                        break;

                    case "cmy2rgb":
                        rgbResult = RGBandCMY.CMY2RGB(colorPlanes);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        outName = defPath + "_cmy2rgb.jpeg";
                        break;

                    case "YCbCr2rgb":
                        rgbResult = RGBandYCbCr.YCbCr2RGB(colorPlanes);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        outName = defPath + "_YCbCr2rgb.jpeg";
                        break;

                    case "xyz2rgb":
                        rgbResult = RGBandXYZ.XYZ2RGB(colorPlanes);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //bad when from file, coz using heavy rounded X Y Z values, when writing them to file
                        outName = defPath + "_xyz2rgb.jpeg";
                        break;

                    case "xyz2lab":
                        var xyzlabResult = XYZandLab.XYZ2Lab(colorPlanes);

                        colorPlaneOne   = (xyzlabResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (xyzlabResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (xyzlabResult[2].Color).ArrayToUint8();

                        //bad when from file, coz xyz values rounded; and lost negative value in a & b when saving in [0..255] range into file
                        outName = defPath + "_xyz2lab.jpeg";
                        break;

                    case "lab2xyz":
                        var labxyzResult = XYZandLab.Lab2XYZ(colorPlanes);

                        colorPlaneOne   = (labxyzResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (labxyzResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (labxyzResult[2].Color).ArrayToUint8();

                        //bad when from file, coz lost a and b negative value when save to file. And lost X Y Z values when round before save in [0..255] range into file
                        outName = defPath + "_lab2xyz.jpeg";
                        break;

                    case "lab2rgb":
                        rgbResult = RGBandLab.Lab2RGB(colorPlanes);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //if from file
                        //very bad, coz lost a lot in converting and round everywhere...
                        outName = defPath + "_lab2rgb.jpeg";
                        break;

                    default:

                        colorPlaneOne   = Helpers.RandArray(height, width, 0, 255);
                        colorPlaneTwo   = Helpers.RandArray(height, width, 0, 255);
                        colorPlaneThree = Helpers.RandArray(height, width, 0, 255);

                        outName = defPath + "_defaultNonColorSpace.jpeg";
                        break;
                }
            }

            image = Helpers.SetPixels(image, colorPlaneOne, colorPlaneTwo, colorPlaneThree);

            Helpers.SaveOptions(image, outName, ".jpeg");
        }
        
        //some rgb2 looks good, some lost negative values, when ranged to uint8 [0..255] for saving
        public static void RGBtoAnothercolorSpacetoFile(List<ArraysListInt> colorPlanes, RGBtoAnotherColorSpace colorSpace, string nameForImage)
        {
            string defPath = GetImageInfo.MyPath("ColorSpace") + nameForImage;

            int width  = colorPlanes[0].Color.GetLength(1);
            int height = colorPlanes[0].Color.GetLength(0);
            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            //back result [0 .. 255]
            int[,] colorPlaneOne   = new int[height, width];
            int[,] colorPlaneTwo   = new int[height, width];
            int[,] colorPlaneThree = new int[height, width];

            string outName = String.Empty;

            if (colorPlanes[0].Color.Length != colorPlanes[1].Color.Length || colorPlanes[0].Color.Length != colorPlanes[2].Color.Length)
            {
                Console.WriteLine("Image plane arrays size dismatch in operation -> " +
                    "RGBtoAnothercolorSpacetoFile(List<arraysListInt> colorPlanes, RGBtoAnotherColorSpace colorSpace) <-");
            }
            else
            {
                switch (colorSpace.ToString())
                {
                    case "rgb2hsv":
                        var hsvResult = RGBandHSV.RGB2HSV(colorPlanes);

                        colorPlaneOne   = (hsvResult[0].Color).ArrayDivByConst(360).ImageDoubleToUint8();
                        colorPlaneTwo   = (hsvResult[1].Color).ImageDoubleToUint8();
                        colorPlaneThree = (hsvResult[2].Color).ImageDoubleToUint8();

                        outName = defPath + "_rgb2hsv.jpeg";
                        break;

                    case "rgb2ntsc":
                        var ntscResult = RGBandNTSC.RGB2NTSC(colorPlanes);

                        colorPlaneOne   = (ntscResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (ntscResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (ntscResult[2].Color).ArrayToUint8();

                        //if we want to save rgb2ntsc result in file
                        //approximate result in file, coz we lost negative values in I and Q
                        outName = defPath + "_rgb2ntsc.jpeg";
                        break;

                    case "rgb2cmy":
                        var cmyResult = RGBandCMY.RGB2CMY(colorPlanes);

                        colorPlaneOne   = (cmyResult[0].Color).ImageDoubleToUint8();
                        colorPlaneTwo   = (cmyResult[1].Color).ImageDoubleToUint8();
                        colorPlaneThree = (cmyResult[2].Color).ImageDoubleToUint8();

                        outName = defPath + "_rgb2cmy.jpeg";
                        break;

                    case "rgb2YCbCr":
                        var YCbCrResult = RGBandYCbCr.RGB2YCbCr(colorPlanes);

                        colorPlaneOne   = (YCbCrResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (YCbCrResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (YCbCrResult[2].Color).ArrayToUint8();

                        outName = defPath + "_rgb2YCbCr.jpeg";
                        break;

                    case "rgb2xyz":
                        var xyzrgbResult = RGBandXYZ.RGB2XYZ(colorPlanes);

                        colorPlaneOne   = (xyzrgbResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (xyzrgbResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (xyzrgbResult[2].Color).ArrayToUint8();

                        //approximate result in file, coz we lost values after comma in saving ntsc result in file [0..255] and heavy round them
                        outName = defPath + "_rgb2xyz.jpeg";
                        break;

                    case "rgb2lab":
                        var rgblabResult = RGBandLab.RGB2Lab(colorPlanes);

                        colorPlaneOne   = (rgblabResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (rgblabResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (rgblabResult[2].Color).ArrayToUint8();

                        //bad, coz lost negative value in a & b when saving in [0..255] range into file
                        outName = defPath + "_rgb2lab.jpeg";
                        break;

                    case "rgb2lab1976":
                        var rgblab1976Result = RGBandLab.RGB2Lab1976(colorPlanes);

                        colorPlaneOne   = (rgblab1976Result[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (rgblab1976Result[1].Color).ArrayToUint8();
                        colorPlaneThree = (rgblab1976Result[2].Color).ArrayToUint8();

                        //bad, coz lost negative value in a & b when saving in [0..255] range into file                    
                        outName = defPath + "_rgb2lab1976.jpeg";
                        break;

                    default:
                        colorPlaneOne   = colorPlanes[0].Color;
                        colorPlaneTwo   = colorPlanes[1].Color;
                        colorPlaneThree = colorPlanes[2].Color;

                        outName = defPath + "_defaultNonColorSpace.jpeg";
                        break;
                }
            }

            image = Helpers.SetPixels(image, colorPlaneOne, colorPlaneTwo, colorPlaneThree);

            Helpers.SaveOptions(image, outName, ".jpeg");
        }

        public static void RGBtoAnothercolorSpacetoFile(Bitmap img, RGBtoAnotherColorSpace colorSpace)
        {
            string imgName = GetImageInfo.Imginfo(Imageinfo.FileName);
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            RGBtoAnothercolorSpacetoFile(ColorList, colorSpace, imgName);
        }

        //if direct from file. Alert: don`t recommended to use
        public static void ColorSpaceToFileDirectFromImage(Bitmap img, ColorSpaceType colorSpace)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("ColorSpace");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            //back result [0 .. 255]
            int[,] colorPlaneOne   = new int[img.Height, img.Width];
            int[,] colorPlaneTwo   = new int[img.Height, img.Width];
            int[,] colorPlaneThree = new int[img.Height, img.Width];

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();
            string outName = String.Empty;

            if (Checks.RGBinput(img))
            {
                switch (colorSpace.ToString())
                {
                    case "rgb2hsv":
                        var hsvResult = RGBandHSV.RGB2HSV(img);

                        colorPlaneOne   = (hsvResult[0].Color).ArrayDivByConst(360).ImageDoubleToUint8();
                        colorPlaneTwo   = (hsvResult[1].Color).ImageDoubleToUint8();
                        colorPlaneThree = (hsvResult[2].Color).ImageDoubleToUint8();

                        outName = defPath + imgName + "_rgb2hsv" + imgExtension;
                        break;

                    case "hsv2rgb":
                        rgbResult = RGBandHSV.HSV2RGB(img);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        outName = defPath + imgName + "_hsv2rgb" + imgExtension;
                        break;

                    case "rgb2ntsc":
                        var ntscResult = RGBandNTSC.RGB2NTSC(img);

                        colorPlaneOne   = (ntscResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (ntscResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (ntscResult[2].Color).ArrayToUint8();

                        //if we want to save rgb2ntsc result in file
                        //approximate result in file, coz we lost negative values in I and Q
                        outName = defPath + imgName + "_rgb2ntsc" + imgExtension;
                        break;

                    case "ntsc2rgb":
                        rgbResult = RGBandNTSC.NTSC2RGB(img);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //when ntsc2rgb from file
                        //approximate result in file, coz we lost negative values in I and Q when saving ntsc result in file [0..255]
                        outName = defPath + imgName + "_ntsc2rgb" + imgExtension;
                        break;

                    case "rgb2cmy":
                        var cmyResult = RGBandCMY.RGB2CMY(img);

                        colorPlaneOne   = (cmyResult[0].Color).ImageDoubleToUint8();
                        colorPlaneTwo   = (cmyResult[1].Color).ImageDoubleToUint8();
                        colorPlaneThree = (cmyResult[2].Color).ImageDoubleToUint8();

                        outName = defPath + imgName + "_rgb2cmy" + imgExtension;
                        break;

                    case "cmy2rgb":
                        rgbResult = RGBandCMY.CMY2RGB(img);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        outName = defPath + imgName + "_cmy2rgb" + imgExtension;
                        break;

                    case "rgb2YCbCr":
                        var YCbCrResult = RGBandYCbCr.RGB2YCbCr(img);

                        colorPlaneOne   = (YCbCrResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (YCbCrResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (YCbCrResult[2].Color).ArrayToUint8();

                        outName = defPath + imgName + "_rgb2YCbCr" + imgExtension;
                        break;

                    case "YCbCr2rgb":
                        rgbResult = RGBandYCbCr.YCbCr2RGB(img);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        outName = defPath + imgName + "_YCbCr2rgb" + imgExtension;
                        break;

                    case "rgb2xyz":
                        var xyzrgbResult = RGBandXYZ.RGB2XYZ(img);

                        colorPlaneOne   = (xyzrgbResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (xyzrgbResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (xyzrgbResult[2].Color).ArrayToUint8();

                        //approximate result in file, coz we lost values after comma in saving ntsc result in file [0..255] and heavy round them                    
                        outName = defPath + imgName + "_rgb2xyz" + imgExtension;
                        break;

                    case "xyz2rgb":
                        rgbResult = RGBandXYZ.XYZ2RGB(img);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //bad when from file, coz using heavy rounded X Y Z values; when writing them to file                  
                        outName = defPath + imgName + "_xyz2rgb" + imgExtension;
                        break;

                    case "xyz2lab":
                        var xyzlabResult = XYZandLab.XYZ2Lab(img);

                        colorPlaneOne   = (xyzlabResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (xyzlabResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (xyzlabResult[2].Color).ArrayToUint8();

                        //bad when from file, coz xyz values rounded; and lost negative value in a & b when saving in [0..255] range into file                    
                        outName = defPath + imgName + "_xyz2lab" + imgExtension;
                        break;

                    case "lab2xyz":
                        var labxyzResult = XYZandLab.Lab2XYZ(img);

                        colorPlaneOne   = (labxyzResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (labxyzResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (labxyzResult[2].Color).ArrayToUint8();

                        //bad when from file, coz lost a and b negative value when save to file. And lost X Y Z values when round before save in [0..255] range into file                    
                        outName = defPath + imgName + "_lab2xyz" + imgExtension;
                        break;

                    case "rgb2lab":
                        var rgblabResult = RGBandLab.RGB2Lab(img);

                        colorPlaneOne   = (rgblabResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (rgblabResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (rgblabResult[2].Color).ArrayToUint8();

                        //bad, coz lost negative value in a & b when saving in [0..255] range into file                    
                        outName = defPath + imgName + "_rgb2lab" + imgExtension;
                        break;

                    case "rgb2lab1976":
                        var rgblab1976Result = RGBandLab.RGB2Lab1976(img);

                        colorPlaneOne   = (rgblab1976Result[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (rgblab1976Result[1].Color).ArrayToUint8();
                        colorPlaneThree = (rgblab1976Result[2].Color).ArrayToUint8();

                        //bad, coz lost negative value in a & b when saving in [0..255] range into file                    
                        outName = defPath + imgName + "_rgb2lab1976" + imgExtension;
                        break;

                    case "lab2rgb":
                        rgbResult = RGBandLab.Lab2RGB(img);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //very bad, coz lost a lot in converting and round everywhere...                    
                        outName = defPath + imgName + "_lab2rgb" + imgExtension;
                        break;

                    default:
                        colorPlaneOne   = Helpers.GetPixels(img)[0].Color;
                        colorPlaneTwo   = Helpers.GetPixels(img)[1].Color;
                        colorPlaneThree = Helpers.GetPixels(img)[2].Color;

                        outName = defPath + imgName + "_defaultNonColorSpace" + imgExtension;
                        break;
                }

                image = Helpers.SetPixels(image, colorPlaneOne, colorPlaneTwo, colorPlaneThree);

                Helpers.SaveOptions(image, outName, imgExtension);
            }
        }

        //L component CIE1976 experiment to file how it`s look like
        public static void FakeCIE1976LtoFile(Bitmap img)
        {
            string imgName = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath = GetImageInfo.MyPath("ColorSpace\\ColorSpacePlane");

            int[,] fake = ColorSpace.FakeCIE1976L(img).ArrayToUint8();

            img = Helpers.SetPixels(img, fake, fake, fake);
            img = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(img);

            string outName = defPath + imgName + "_FakeCIE1976L.png";
            Helpers.SaveOptions(img, outName, ".png");
        }
    }

    public enum ColorSpaceType
    {
        rgb2hsv,
        hsv2rgb,
        rgb2ntsc,
        ntsc2rgb,
        rgb2cmy,
        cmy2rgb,
        rgb2YCbCr,
        YCbCr2rgb,
        rgb2xyz,
        xyz2rgb,
        xyz2lab,
        lab2xyz,
        rgb2lab,
        lab2rgb,
        rgb2lab1976
    }

    //RGB to another color plane
    public enum RGBtoAnotherColorSpace
    {
        rgb2hsv,
        rgb2ntsc,
        rgb2cmy,
        rgb2YCbCr,
        rgb2xyz,
        rgb2lab,
        rgb2lab1976
    }

    //
    public enum AnotherColorSpacetoRGBaXYZLab
    {
        hsv2rgb,
        ntsc2rgb,
        cmy2rgb,
        YCbCr2rgb,
        xyz2rgb,
        xyz2lab,
        lab2xyz,
        lab2rgb
    }
}
