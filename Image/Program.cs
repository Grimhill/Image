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
            string ImgExtension = Path.GetExtension(ImageFilePath).ToLower();
            string FileName = Path.GetFileName(ImageFilePath);

            if (MoreHelpers.CheckForInputFormat(ImgExtension))
            {
                Bitmap image = new Bitmap(ImageFilePath);                         

                Contour.GlobalContour(image, CountourVariant.Variant6_RGB, FileName);      

            }
            else { }

            Console.ReadLine();
        }
    }    

    public enum SupportFormats
    {
        jpg,
        jpeg,
        png,
        bmp,
        tiff,
        gif
    }

    public enum OutType
    {
        OneBpp,
        EightBpp,
        TwentyFourBpp
    }
}
