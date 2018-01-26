using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Image.ColorSpaces;
using Image.ArrayOperations;

//Obtain presented color space, and write them to file
namespace Image
{
    public static class ColorSpace
    {
        public static double[,] FakeCIE1976L(Bitmap img)
        {
            var lab = RGBandLab.RGB2Lab(img);

            double[,] L = lab[0].Color;
            L = L.ArrayMultByConst(2.57);

            return L;
        }
    }

    public static class ColorSpaceToFile
    {
        //all 2rgb looks good, if obtained not from file, but from rgb and made some filtering or another process, and saved as rgb back
        public static void AnothercolorSpacetoRGBXYZLabtoFile(List<ArraysListDouble> colorPlanes, AnotherColorSpacetoRGBaXYZLab colorSpace) //how generic arraysListT here?
        {
            int width  = colorPlanes[0].Color.GetLength(1);
            int height = colorPlanes[0].Color.GetLength(0);
            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\ColorSpace");

            //back result [0 .. 255]
            int[,] colorPlaneOne   = new int[height, width];
            int[,] colorPlaneTwo   = new int[height, width];
            int[,] colorPlaneThree = new int[height, width];

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            string outName = String.Empty;

            if (colorPlanes[0].Color.Length != colorPlanes[1].Color.Length || colorPlanes[0].Color.Length != colorPlanes[2].Color.Length)
            {
                Console.WriteLine("Image plane arrays size dismatch in operation -> colorSpaceToFile(List<arraysListInt> Colors, ColorSpaceType colorSpace) <-");
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

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\hsv2rgb.jpeg";
                        break;

                    case "ntsc2rgb":
                        rgbResult = RGBandNTSC.NTSC2RGB(colorPlanes);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //when ntsc2rgb from file
                        //approximate result in file, coz we lost negative values in I and Q when saving ntsc result in file [0..255]
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\ntsc2rgb.jpeg";
                        break;

                    case "cmy2rgb":
                        rgbResult = RGBandCMY.CMY2RGB(colorPlanes);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\cmy2rgb.jpeg";
                        break;

                    case "YCbCr2rgb":
                        rgbResult = RGBandYCbCr.YCbCr2RGB(colorPlanes);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\YCbCr2rgb.jpeg";
                        break;

                    case "xyz2rgb":
                        rgbResult = RGBandXYZ.XYZ2RGB(colorPlanes);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //bad when from file, coz using heavy rounded X Y Z values, when writing them to file
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\xyz2rgb.jpeg";
                        break;

                    case "xyz2lab":
                        var xyzlabResult = XYZandLab.XYZ2Lab(colorPlanes);

                        colorPlaneOne   = (xyzlabResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (xyzlabResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (xyzlabResult[2].Color).ArrayToUint8();

                        //bad when from file, coz xyz values rounded, and lost negative value in a & b when saving in [0..255] range into file
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\xyz2lab.jpeg";
                        break;

                    case "lab2xyz":
                        var labxyzResult = XYZandLab.Lab2XYZ(colorPlanes);

                        colorPlaneOne   = (labxyzResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (labxyzResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (labxyzResult[2].Color).ArrayToUint8();

                        //bad when from file, coz lost a and b negative value when save to file. And lost X Y Z values when round before save in [0..255] range into file
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\lab2xyz.jpeg";
                        break;

                    case "lab2rgb":
                        rgbResult = RGBandLab.Lab2RGB(colorPlanes);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //if from file
                        //very bad, coz lost a lot in converting and round everywhere...
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\lab2rgb.jpeg";
                        break;

                    default:

                        colorPlaneOne   = Helpers.RandArray(height, width, 0, 255);
                        colorPlaneTwo   = Helpers.RandArray(height, width, 0, 255);
                        colorPlaneThree = Helpers.RandArray(height, width, 0, 255);

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\defaultNonColorSpace.jpeg";
                        break;
                }
            }

            image = Helpers.SetPixels(image, colorPlaneOne, colorPlaneTwo, colorPlaneThree);
            outName = MoreHelpers.OutputFileNames(outName);

            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ".jpeg");
        }
        
        //some rgb2 looks good, some lost negative values, when ranged to [0..255] for saving
        public static void RGBtoAnothercolorSpacetoFile(List<ArraysListInt> colorPlanes, RGBtoAnotherColorSpace colorSpace) //how generic arraysListT here?
        {
            int width  = colorPlanes[0].Color.GetLength(1);
            int height = colorPlanes[0].Color.GetLength(0);
            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\ColorSpace");

            //back result [0 .. 255]
            int[,] colorPlaneOne   = new int[height, width];
            int[,] colorPlaneTwo   = new int[height, width];
            int[,] colorPlaneThree = new int[height, width];

            string outName = String.Empty;

            if (colorPlanes[0].Color.Length != colorPlanes[1].Color.Length || colorPlanes[0].Color.Length != colorPlanes[2].Color.Length)
            {
                Console.WriteLine("Image plane arrays size dismatch in operation -> colorSpaceToFile(List<arraysListInt> Colors, ColorSpaceType colorSpace) <-");
            }
            else
            {
                switch (colorSpace.ToString())
                {
                    case "rgb2hsv":
                        var hsvResult = RGBandHSV.RGB2HSV(colorPlanes);

                        colorPlaneOne   = (hsvResult[0].Color).ArrayDivByConst(360).ImageArrayToUint8();
                        colorPlaneTwo   = (hsvResult[1].Color).ImageArrayToUint8();
                        colorPlaneThree = (hsvResult[2].Color).ImageArrayToUint8();

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2hsv.jpeg";
                        break;

                    case "rgb2ntsc":
                        var ntscResult = RGBandNTSC.RGB2NTSC(colorPlanes);

                        colorPlaneOne   = (ntscResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (ntscResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (ntscResult[2].Color).ArrayToUint8();

                        //if we want to save rgb2ntsc result in file
                        //approximate result in file, coz we lost negative values in I and Q
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2ntsc.jpeg";
                        break;

                    case "rgb2cmy":
                        var cmyResult = RGBandCMY.RGB2CMY(colorPlanes);

                        colorPlaneOne   = (cmyResult[0].Color).ImageArrayToUint8();
                        colorPlaneTwo   = (cmyResult[1].Color).ImageArrayToUint8();
                        colorPlaneThree = (cmyResult[2].Color).ImageArrayToUint8();

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2cmy.jpeg";
                        break;

                    case "rgb2YCbCr":
                        var YCbCrResult = RGBandYCbCr.RGB2YCbCr(colorPlanes);

                        colorPlaneOne   = (YCbCrResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (YCbCrResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (YCbCrResult[2].Color).ArrayToUint8();

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2YCbCr.jpeg";
                        break;

                    case "rgb2xyz":
                        var xyzrgbResult = RGBandXYZ.RGB2XYZ(colorPlanes);

                        colorPlaneOne   = (xyzrgbResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (xyzrgbResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (xyzrgbResult[2].Color).ArrayToUint8();

                        //approximate result in file, coz we lost values after comma in saving ntsc result in file [0..255] and heavy round them
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2xyz.jpeg";
                        break;

                    case "rgb2lab":
                        var rgblabResult = RGBandLab.RGB2Lab(colorPlanes);

                        colorPlaneOne   = (rgblabResult[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (rgblabResult[1].Color).ArrayToUint8();
                        colorPlaneThree = (rgblabResult[2].Color).ArrayToUint8();

                        //bad, coz lost negative value in a & b when saving in [0..255] range into file
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2lab.jpeg";
                        break;

                    case "rgb2lab1976":
                        var rgblab1976Result = RGBandLab.RGB2Lab1976(colorPlanes);

                        colorPlaneOne   = (rgblab1976Result[0].Color).ArrayToUint8();
                        colorPlaneTwo   = (rgblab1976Result[1].Color).ArrayToUint8();
                        colorPlaneThree = (rgblab1976Result[2].Color).ArrayToUint8();

                        //bad, coz lost negative value in a & b when saving in [0..255] range into file                    
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2lab1976.jpeg";
                        break;

                    default:
                        colorPlaneOne   = colorPlanes[0].Color;
                        colorPlaneTwo   = colorPlanes[1].Color;
                        colorPlaneThree = colorPlanes[2].Color;

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\defaultNonColorSpace.jpeg";
                        break;
                }
            }

            image = Helpers.SetPixels(image, colorPlaneOne, colorPlaneTwo, colorPlaneThree);
            outName = MoreHelpers.OutputFileNames(outName);

            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ".jpeg");
        }        

        //if direct from file
        public static void ColorSpaceToFileDirectFromImage(Bitmap img, ColorSpaceType colorSpace, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\ColorSpace");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            //back result [0 .. 255]
            int[,] colorPlaneOne   = new int[img.Height, img.Width];
            int[,] colorPlaneTwo   = new int[img.Height, img.Width];
            int[,] colorPlaneThree = new int[img.Height, img.Width];

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            string outName = String.Empty;

            switch (colorSpace.ToString())
            {
                case "rgb2hsv":
                    var hsvResult = RGBandHSV.RGB2HSV(img);

                    colorPlaneOne   = (hsvResult[0].Color).ArrayDivByConst(360).ImageArrayToUint8();
                    colorPlaneTwo   = (hsvResult[1].Color).ImageArrayToUint8();
                    colorPlaneThree = (hsvResult[2].Color).ImageArrayToUint8();

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_rgb2hsv" + ImgExtension;
                    break;

                case "hsv2rgb":
                    rgbResult = RGBandHSV.HSV2RGB(img);

                    colorPlaneOne   = rgbResult[0].Color;
                    colorPlaneTwo   = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_hsv2rgb" + ImgExtension;
                    break;

                case "rgb2ntsc":
                    var ntscResult = RGBandNTSC.RGB2NTSC(img);

                    colorPlaneOne   = (ntscResult[0].Color).ArrayToUint8();
                    colorPlaneTwo   = (ntscResult[1].Color).ArrayToUint8();
                    colorPlaneThree = (ntscResult[2].Color).ArrayToUint8();

                    //if we want to save rgb2ntsc result in file
                    //approximate result in file, coz we lost negative values in I and Q
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_rgb2ntsc" + ImgExtension;
                    break;

                case "ntsc2rgb":
                    rgbResult = RGBandNTSC.NTSC2RGB(img);

                    colorPlaneOne   = rgbResult[0].Color;
                    colorPlaneTwo   = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    //when ntsc2rgb from file
                    //approximate result in file, coz we lost negative values in I and Q when saving ntsc result in file [0..255]
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_ntsc2rgb" + ImgExtension;
                    break;

                case "rgb2cmy":
                    var cmyResult = RGBandCMY.RGB2CMY(img);

                    colorPlaneOne   = (cmyResult[0].Color).ImageArrayToUint8();
                    colorPlaneTwo   = (cmyResult[1].Color).ImageArrayToUint8();
                    colorPlaneThree = (cmyResult[2].Color).ImageArrayToUint8();

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_rgb2cmy" + ImgExtension;
                    break;

                case "cmy2rgb":
                    rgbResult = RGBandCMY.CMY2RGB(img);

                    colorPlaneOne   = rgbResult[0].Color;
                    colorPlaneTwo   = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_cmy2rgb" + ImgExtension;
                    break;

                case "rgb2YCbCr":
                    var YCbCrResult = RGBandYCbCr.RGB2YCbCr(img);

                    colorPlaneOne   = (YCbCrResult[0].Color).ArrayToUint8();
                    colorPlaneTwo   = (YCbCrResult[1].Color).ArrayToUint8();
                    colorPlaneThree = (YCbCrResult[2].Color).ArrayToUint8();

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_rgb2YCbCr" + ImgExtension;
                    break;

                case "YCbCr2rgb":
                    rgbResult = RGBandYCbCr.YCbCr2RGB(img);

                    colorPlaneOne   = rgbResult[0].Color;
                    colorPlaneTwo   = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_YCbCr2rgb" + ImgExtension;
                    break;

                case "rgb2xyz":
                    var xyzrgbResult = RGBandXYZ.RGB2XYZ(img);

                    colorPlaneOne   = (xyzrgbResult[0].Color).ArrayToUint8();
                    colorPlaneTwo   = (xyzrgbResult[1].Color).ArrayToUint8();
                    colorPlaneThree = (xyzrgbResult[2].Color).ArrayToUint8();

                    //approximate result in file, coz we lost values after comma in saving ntsc result in file [0..255] and heavy round them                    
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_rgb2xyz" + ImgExtension;
                    break;

                case "xyz2rgb":
                    rgbResult = RGBandXYZ.XYZ2RGB(img);

                    colorPlaneOne   = rgbResult[0].Color;
                    colorPlaneTwo   = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    //bad when from file, coz using heavy rounded X Y Z values, when writing them to file                  
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_xyz2rgb" + ImgExtension;
                    break;

                case "xyz2lab":
                    var xyzlabResult = XYZandLab.XYZ2Lab(img);

                    colorPlaneOne   = (xyzlabResult[0].Color).ArrayToUint8();
                    colorPlaneTwo   = (xyzlabResult[1].Color).ArrayToUint8();
                    colorPlaneThree = (xyzlabResult[2].Color).ArrayToUint8();

                    //bad when from file, coz xyz values rounded, and lost negative value in a & b when saving in [0..255] range into file                    
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_xyz2lab" + ImgExtension;
                    break;

                case "lab2xyz":
                    var labxyzResult = XYZandLab.Lab2XYZ(img);

                    colorPlaneOne   = (labxyzResult[0].Color).ArrayToUint8();
                    colorPlaneTwo   = (labxyzResult[1].Color).ArrayToUint8();
                    colorPlaneThree = (labxyzResult[2].Color).ArrayToUint8();

                    //bad when from file, coz lost a and b negative value when save to file. And lost X Y Z values when round before save in [0..255] range into file                    
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_lab2xyz" + ImgExtension;
                    break;

                case "rgb2lab":
                    var rgblabResult = RGBandLab.RGB2Lab(img);

                    colorPlaneOne   = (rgblabResult[0].Color).ArrayToUint8();
                    colorPlaneTwo   = (rgblabResult[1].Color).ArrayToUint8();
                    colorPlaneThree = (rgblabResult[2].Color).ArrayToUint8();

                    //bad, coz lost negative value in a & b when saving in [0..255] range into file                    
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_rgb2lab" + ImgExtension;
                    break;

                case "rgb2lab1976":
                    var rgblab1976Result = RGBandLab.RGB2Lab1976(img);

                    colorPlaneOne   = (rgblab1976Result[0].Color).ArrayToUint8();
                    colorPlaneTwo   = (rgblab1976Result[1].Color).ArrayToUint8();
                    colorPlaneThree = (rgblab1976Result[2].Color).ArrayToUint8();

                    //bad, coz lost negative value in a & b when saving in [0..255] range into file                    
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_rgb2lab1976" + ImgExtension;
                    break;

                case "lab2rgb":
                    rgbResult = RGBandLab.Lab2RGB(img);

                    colorPlaneOne   = rgbResult[0].Color;
                    colorPlaneTwo   = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    //very bad, coz lost a lot in converting and round everywhere...                    
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_lab2rgb" + ImgExtension;
                    break;

                default:
                    colorPlaneOne   = Helpers.GetPixels(img)[0].Color;
                    colorPlaneTwo   = Helpers.GetPixels(img)[1].Color;
                    colorPlaneThree = Helpers.GetPixels(img)[2].Color;

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_defaultNonColorSpace" + ImgExtension;
                    break;
            }

            image = Helpers.SetPixels(image, colorPlaneOne, colorPlaneTwo, colorPlaneThree);
            outName = MoreHelpers.OutputFileNames(outName);

            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        public static void FakeCIE1976LtoFile(Bitmap img, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\ColorSpace");

            var fake = ColorSpace.FakeCIE1976L(img);

            fileName = fileName + "_FakeCIE1976L" + ImgExtension;
            Helpers.WriteImageToFile(fake, fake, fake, fileName, "ColorSpace");
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
