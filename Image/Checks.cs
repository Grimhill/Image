using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Image.ArrayOperations;

namespace Image
{
    public static class Checks
    {
        public static List<string> AvailableFormats = new List<string>() { ".jpg", ".jpeg", ".bmp", ".png", ".tif" };
        public static bool CheckForInputFormat(string ImgExtension)
        {
            if (!AvailableFormats.Contains(ImgExtension))
            {
                Console.WriteLine("Unsupport image format (extension. Support: jpg, jpeg, bmp, png, tif");
                return false;
            }
            return true;
        }

        public static List<double> AllowableDepth = new List<double>() { 1, 8, 24, 32, 48 };
        public static bool InputDepthControl(Bitmap img)
        {
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (!AllowableDepth.Contains(Depth))
            {
                Console.WriteLine("Unsupport image pixel format for input.\n" +
                    "Allowable: 1bit, 8bit, 24bit, 32bit. 48bit - Bimap represent as 32bppArgb. All them can be B&W");
                return false;
            }

            return true;
        }

        public static string OutputFileNames(string fullFilePath)
        {
            if (File.Exists(fullFilePath))
            {
                string folder    = Path.GetDirectoryName(fullFilePath);
                string filename  = Path.GetFileNameWithoutExtension(fullFilePath);
                string extension = Path.GetExtension(fullFilePath);
                int number = 0;

                Match regex = Regex.Match(fullFilePath, @"(.+) \((\d+)\)\.\w+");

                if (regex.Success)
                {
                    filename = regex.Groups[1].Value;
                    number = int.Parse(regex.Groups[2].Value);
                }

                do
                {
                    number++;
                    fullFilePath = Path.Combine(folder, string.Format("{0} ({1}){2}", filename, number, extension));
                }
                while (File.Exists(fullFilePath));
            }
            return fullFilePath;

        }

        public static void DirectoryExistance(string path)
        {
            bool exists = Directory.Exists(path);

            if (!exists)
                Directory.CreateDirectory(path);
        }


        public static bool BlackandWhite24bppCheck(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);
            var dif1 = (ColorList[0].Color).SubArrays((ColorList[1].Color));
            var dif2 = (ColorList[0].Color).SubArrays((ColorList[2].Color));

            if (dif1.Cast<int>().Min() <= 0 || dif2.Cast<int>().Min() <= 0
                || dif1.Cast<int>().Sum() != 0 || dif2.Cast<int>().Sum() != 0)
            {
                return false;
            }
            else
                return true;
        }

        public static bool RGBCheck(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);
            var dif1 = (ColorList[0].Color).SubArrays((ColorList[1].Color)).AbsArrayElements();
            var dif2 = (ColorList[0].Color).SubArrays((ColorList[2].Color)).AbsArrayElements();

            if (dif1.Cast<int>().Sum() != 0 || dif2.Cast<int>().Sum() != 0)
            {
                return true;
            }
            else
                return false;
        }

        public static bool NonRGBinput(Bitmap img, [CallerMemberName]string callName = "")
        {
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if ((Depth == 24 || Depth == 32) && RGBCheck(img))
            {
                return true;
            }
            Console.WriteLine("There in non-RGB at input. Method: " + callName);
            return false;
        }

        public static bool NonBWinput(Bitmap img, [CallerMemberName]string callName = "")
        {
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (Depth == 8 && BlackandWhite24bppCheck(img))
            {
                return true;
            }
            Console.WriteLine("There in non-BW at input. Method: " + callName);
            return false;
        }
    }
}
