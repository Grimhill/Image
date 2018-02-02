using System;
using System.Drawing;
using System.IO;

namespace Image
{
    class Program
    {
        static void Main(string[] args)
        {
            string ImageFilePath = "dragon.jpg";
            //string ImageModFilePath = ""; //for difference method
            //Bitmap img; //for difference method
            string ImgExtension = Path.GetExtension(ImageFilePath).ToLower();
            string FileName = Path.GetFileName(ImageFilePath);
            Bitmap image;

            if (Checks.CheckForInputFormat(ImgExtension))
            {
                image = new Bitmap(ImageFilePath);
                //img = new Bitmap(ImageModFilePath); //for difference method

                if (Checks.InputDepthControl(image))
                { 
                    Contour.GlobalContour(image, CountourVariant.Variant6_RGB, FileName);
                }
            }
            else { }

            Console.ReadLine();
        }
    } 
}
