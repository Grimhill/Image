using System;
using System.IO;
using System.Drawing;

namespace Image
{
    class Program
    {
        public static String FILE_Path = string.Empty;
        public static String Save_FILE_Path = string.Empty;

        static void Main(string[] args)
        {
            string ImageFilePath = "dragon.jpg";
            //string ImageModFilePath = ""; //for difference method
            //Bitmap img; //for difference method
            string ImgExtension = Path.GetExtension(ImageFilePath).ToLower();            
            Bitmap image;

            FILE_Path = ImageFilePath;
            Save_FILE_Path = Directory.GetCurrentDirectory();

            if (Checks.CheckForInputFormat(ImgExtension))
            {
                image = new Bitmap(ImageFilePath);
                //img = new Bitmap(ImageModFilePath); //for difference method

                if (Checks.InputDepthControl(image))
                { 
                    //example
                    //before use look function at Functions.txt
                    Contour.FindContour(image, CountourVariant.Variant6_RGB);
                }
            }
            else { }

            Console.ReadLine();
        }
    } 
}
