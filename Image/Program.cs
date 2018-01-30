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
            string ImgExtension = Path.GetExtension(ImageFilePath).ToLower();
            string FileName = Path.GetFileName(ImageFilePath);
            Bitmap image;

            if (Checks.CheckForInputFormat(ImgExtension))
            {
                image = new Bitmap(ImageFilePath);

                if (Checks.InputDepthControl(image))
                {
                    double Depth = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat);
                    if (Depth == 48 || Depth == 64)
                    {
                        image = MoreHelpers.Uint16toUint8Compression(image);
                    }

                    Contour.GlobalContour(image, CountourVariant.Variant6_RGB, FileName);
                }
            }
            else { }

            Console.ReadLine();
        }
    } 
}
