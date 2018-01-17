using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Image
{
    class Program
    {
        static void Main(string[] args)
        {
            //recommended 24bbp format image (8 bit per color)
            //!!!!!
            //Все входные не 8 разрядные (uint8), а выше (uint16) првести к uint8. Binary & 8b - some troubles 

            string ImageFilePath = "Contrast_8.jpg";

            string ImgExtension = Path.GetExtension(ImageFilePath).ToLower();

            if (MoreHelpers.CheckForInputFormat(ImgExtension))
            {
                ArrayOperations ArrOp = new ArrayOperations();

                System.Drawing.Bitmap image = new System.Drawing.Bitmap(ImageFilePath);

                int[,] Rc = new int[image.Height, image.Width];
                int[,] Gc = new int[image.Height, image.Width];
                int[,] Bc = new int[image.Height, image.Width];

                //read row by row image R\G\B pixels value
                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        Color pixelColor = image.GetPixel(j, i);
                        Rc[i, j] = pixelColor.R;
                        Gc[i, j] = pixelColor.G;
                        Bc[i, j] = pixelColor.B;
                    }
                }

                #region exist operations list            
                //serach contours on image. Parameters: image, variants: 1-6
                //!!! if will be enter interface (or in console) lead variant number to enum or delete enum and use only numbers    
                //image for test - imgfortest\dragon.jpg 
                //Contour.GlobalContour(image, CountourVariant.Variant6_RGB, ImgExtension);

                //contrast for black & white image
                //image for test - imgfortest\contrast\Contrast_24.jpg
                //all default
                //Contrast.ContrastBW(image, ImgExtension);
                //Parameters: low_in & high_in contrast limits, gamma coefficient
                //Contrast.ContrastBW(image, 0.3, 0.7, 1, 0.8, ImgExtension);

                //contrast for RGB image
                //Parameters: low_in & high_in contrast limits for all color components
                //image for test - imgfortest\contrast\contrastRGB.png
                //Contrast.ContrastRGB(image, 0.2, 0.6, 0.3, 0.7, 0, 1, ImgExtension);
                #endregion exist operations list             

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
}
