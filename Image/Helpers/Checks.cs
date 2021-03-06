﻿using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using Image.ArrayOperations;

namespace Image
{
    public static class Checks
    {
        //check for allowable input formats
        private static List<string> AvailableFormats = new List<string>() { ".jpg", ".jpeg", ".bmp", ".png", ".tif" };
        public static bool CheckForInputFormat(string ImgExtension)
        {
            if (!AvailableFormats.Contains(ImgExtension))
            {
                Console.WriteLine("Unsupport image format (extension. Support: jpg, jpeg, bmp, png, tif");
                return false;
            }
            return true;
        }

        //check for allowable depth - not actual
        private static List<double> AllowableDepth = new List<double>() { 1, 8, 24, 32, 48 };
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

        //check for file name duplicates
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

        //check if such directory exist, if not - create
        public static void DirectoryExistance(string path)
        {
            bool exists = Directory.Exists(path);

            if (!exists)
                Directory.CreateDirectory(path);
        }

        public static string DirectoryExist(this string path)
        {
            bool exists = Directory.Exists(path);

            if (!exists)
                Directory.CreateDirectory(path);
            return path;
        }

        //my bad
        //check if 24bpp BW actually BW
        public static bool BlackandWhite24bppCheck(Bitmap img)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            var dif1 = (ColorList[0].Color).SubArrays((ColorList[1].Color)).AbsArrayElements();
            var dif2 = (ColorList[0].Color).SubArrays((ColorList[2].Color)).AbsArrayElements();
            var dif3 = (ColorList[1].Color).SubArrays((ColorList[2].Color)).AbsArrayElements();

            if (dif1.Cast<int>().Max() <= 3 && dif2.Cast<int>().Max() <= 3 && dif3.Cast<int>().Max() <= 3)  //lets difference be not more than 1% ~3
                //|| dif1.Cast<int>().Sum() != 0 || dif2.Cast<int>().Sum() != 0)
            {
                return true;
            }
            else
                return false;
        }

        //my bad
        public static bool BlackandWhite24bppCheck(List<ArraysListInt> list)
        {        
            var dif1 = (list[0].Color).SubArrays((list[1].Color)).AbsArrayElements();
            var dif2 = (list[0].Color).SubArrays((list[2].Color)).AbsArrayElements();
            var dif3 = (list[1].Color).SubArrays((list[2].Color)).AbsArrayElements();

            if (dif1.Cast<int>().Max() <= 3 && dif2.Cast<int>().Max() <= 3 && dif3.Cast<int>().Max() <= 3) //lets difference be not more than 1% ~3
            {
                return true;
            }
            else
                return false;
        }

        //Check if image RGB
        public static bool RGBCheck(Bitmap img)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            var dif1 = (ColorList[0].Color).SubArrays((ColorList[1].Color)).AbsArrayElements();
            var dif2 = (ColorList[0].Color).SubArrays((ColorList[2].Color)).AbsArrayElements();
            var dif3 = (ColorList[1].Color).SubArrays((ColorList[2].Color)).AbsArrayElements();

            //my bad
            //if (dif1.Cast<int>().Sum() != 0 || dif2.Cast<int>().Sum() != 0 || dif3.Cast<int>().Sum() != 0)
            
            if ((dif1.Cast<int>().Max() > 3 && dif2.Cast<int>().Max() > 3 && dif3.Cast<int>().Max() > 3) //if min difference greater then threshold in 1% as implemented in 24bpp BW check
                || (dif1.Cast<int>().Max() > 3 && dif2.Cast<int>().Max() > 3) //R-G && R-B
                || (dif1.Cast<int>().Max() > 3 && dif3.Cast<int>().Max() > 3) //G-R && G-B
                || (dif2.Cast<int>().Max() > 3 && dif3.Cast<int>().Max() > 3) //B-R && B-G
                || (ColorList[0].Color.Cast<int>().Sum() != 0 && ColorList[1].Color.Cast<int>().Sum() == 0 && ColorList[2].Color.Cast<int>().Sum() == 0)  //R color plane image
                || (ColorList[0].Color.Cast<int>().Sum() == 0 && ColorList[1].Color.Cast<int>().Sum() != 0 && ColorList[2].Color.Cast<int>().Sum() == 0)  //G color plane image
                || (ColorList[0].Color.Cast<int>().Sum() == 0 && ColorList[1].Color.Cast<int>().Sum() == 0 && ColorList[2].Color.Cast<int>().Sum() != 0)) //B color plane image 
            {
                return true;
            }
            else
                return false;
        }

        //check if binary 
        public static bool BinaryCheck(int[,] arr)
        {
            int count = 0;
            for(int i = 0; i < arr.GetLength(0); i++)
            {
                for(int j = 0; j < arr.GetLength(1); j++)
                {
                    if (arr[i, j] == 0 || arr[i, j] == 255)
                        count++;
                }
            }
            if (count == arr.Length - 1)
                return false;
            else
                return true;
        }

        public static bool BinaryCheck(Bitmap img)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);

            int count = 0;
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    if (ColorList[0].Color[i, j] == 0 || ColorList[0].Color[i, j] == 255)
                        count++;
                }
            }
            if (count == ColorList[0].Color.Length)
                return true;
            else
                return false;
        }

        //Check if at input needed type of image
        public static bool BinaryInput(Bitmap img, [CallerMemberName]string callName = "")
        {
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            if (Depth == 1 || BinaryCheck(img))
                return true;
            else
            {
                //Console.WriteLine("There is non binary image at input. Method: " + callName);
                return false;
            }
        }

        public static bool RGBinput(Bitmap img, [CallerMemberName]string callName = "")
        {
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if ((Depth == 24 || Depth == 32) && RGBCheck(img))
            {
                return true;
            }
            Console.WriteLine("There in non-RGB at input. Method: " + callName);
            return false;
        }

        public static bool BWinput(Bitmap img, [CallerMemberName]string callName = "")
        {
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (Depth == 1 || Depth == 8 || BlackandWhite24bppCheck(img))
            {
                return true;
            }
            Console.WriteLine("There in non-BW at input. Method: " + callName);
            return false;
        } 
    }
}
