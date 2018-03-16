using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Image.ArrayOperations;

namespace Image
{
    public static class BWMorph
    {
        //Some morph operations with binary image, or represented as binary
        public static void Bwmorph(Bitmap img, BwmorphOpearion operation, int repeat, OutType type)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Morph\\BWMorph");
            
            var result = BwmorphHelper(img, operation, repeat);
            string outName = defPath + imgName + "_" + operation.ToString() + "_repeat_" + repeat + imgExtension;       

            MoreHelpers.WriteImageToFile(result, result, result, outName, type);
        }

        //return result as 1/8/24 bitmap
        public static Bitmap BwmorphBitmap(Bitmap img, BwmorphOpearion operation, int repeat, BwmorphOut type)
        {
            var result = BwmorphHelper(img, operation, repeat);
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            image = Helpers.SetPixels(image, result, result, result);

            if (type == BwmorphOut.OneBpp)            
                image = PixelFormatWorks.ImageTo1BppBitmap(image, 0.5);
            
            else if (type == BwmorphOut.EightBpp)            
                image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); 

            return image;
        }

        //return in as [0..255], for binary can use extenstion Uint8ArrayToBinary
        public static int[,] BwmorphImageArray(Bitmap img, BwmorphOpearion operation, int repeat)
        {
            return BwmorphHelper(img, operation, repeat); 
        }

        //process bwmotph operations
        private static int[,] BwmorphHelper(Bitmap img, BwmorphOpearion operation, int repeat)
        {
            double[,] tempResult = new double[img.Height, img.Width];
            double[,] loopStrike = new double[img.Height, img.Width];
            int[,] result        = new int[img.Height, img.Width];

            var rec = Helpers.Image2BinaryArray(img);

            if (repeat == 0 || repeat < 0)
            {
                repeat = 1;
                Console.WriteLine("Number of operation  repeat must be non-negative value greater 0. Set to defalut: repeat - 1 time");
            }

            loopStrike = rec.ArrayToDouble();

            for (int i = 0; i < repeat; i++)
            {
                switch (operation)
                {
                    //thins objects to lines. It removes pixels so that an object without holes shrinks to a minimally connected stroke, 
                    //and an object with holes shrinks to a connected ring halfway between each hole and the outer boundary.
                    case BwmorphOpearion.thin:
                        var thinLut = ReadLut("ThinLut1.txt");
                        tempResult = ApplyLut(loopStrike, thinLut);

                        thinLut = ReadLut("ThinLut2.txt");
                        tempResult = ApplyLut(tempResult, thinLut);
                        break;

                    //Bridges unconnected pixels, that is, sets 0-valued pixels to 1 if they have two nonzero neighbors that are not connected
                    case BwmorphOpearion.bridge:
                        var bridgeLut = ReadLut("BridgeLut.txt");
                        tempResult = ApplyLut(loopStrike, bridgeLut);
                        break;

                    //Uses diagonal fill to eliminate 8-connectivity of the background
                    case BwmorphOpearion.diag:
                        var diagLut = ReadLut("DiagLut.txt");
                        tempResult = ApplyLut(loopStrike, diagLut);
                        break;

                        //shrinks objects to points. It removes pixels so that objects without holes shrink to a point, 
                        //and objects with holes shrink to a connected ring halfway between each hole and the outer boundary
                    case BwmorphOpearion.shrink:
                        var shrinkLut = ReadLut("ShrinkLut1.txt");
                        tempResult = ApplyLut(loopStrike, shrinkLut);

                        shrinkLut = ReadLut("ShrinkLut2.txt");
                        tempResult = ApplyLut(tempResult, shrinkLut);
                        break;

                        //Get the image skeleton
                        //removes pixels on the boundaries of objects but does not allow objects to break apart. 
                        //The pixels remaining make up the image skeleton.
                    case BwmorphOpearion.skel:
                        var skelLut = ReadLut("SkelLut1.txt");
                        tempResult = ApplyLut(loopStrike, skelLut);

                        skelLut = ReadLut("SkelLut2.txt");
                        tempResult = ApplyLut(tempResult, skelLut);
                        break;

                    //Removes spur pixels
                    case BwmorphOpearion.spur:
                        var spurLut = ReadLut("SpurLut.txt");
                        tempResult = ApplyLut(loopStrike, spurLut);
                        break;
                    
                    //Conway game "life"
                    case BwmorphOpearion.conwaylaw:
                        var conwaylawLut = ReadLut("ConwaylawLut.txt");
                        tempResult = ApplyLut(loopStrike, conwaylawLut);
                        break;
                }

                //here logic & bettwen input and after lut, but no matter - input represented as binary, so we can mult 
                tempResult = tempResult.ArrayMultElements(loopStrike);
                loopStrike = tempResult;
            }

            result = loopStrike.DoubleArrayToInt();

            //make result saveble
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    if (result[i, j] == 1)
                        result[i, j] = 255;
                }
            }

            return result;
        }

        //In future can be independent function
        //List coz may be have seveal sources?
        private static List<string> PreparedLutPath = new List<string>()
        { "\\Morphology\\bwmorphLut\\", };
        
        private static double[] ReadLut(string lutName, [CallerMemberName]string callName = "")
        {
            int index = 0;
            foreach (var n in Enum.GetValues(typeof(PreparedLutPath)).Cast<PreparedLutPath>())
            {
                if (n.ToString() == callName) { index = (int)n; break; };
            }

            string lutPath = GetImageInfo.LutPath(PreparedLutPath.ElementAt(index) + lutName);
            string line;
            int count = 0;
            List<string> temp = new List<string>();

            using (StreamReader lutFile = new StreamReader(lutPath))
            {
                while ((line = lutFile.ReadLine()) != null)
                {
                    temp.Add(line);
                    count++;
                }
            }
            
            string concat = String.Join(" ", temp.ToArray()).Replace(" ", "");
            temp = new List<string>();
            foreach (char s in concat)
            {
                temp.Add(s.ToString());
            }

            double[] lut = new double[temp.Count];
            for (int i = 0; i < temp.Count; i++)
            {
                lut[i] = Convert.ToDouble(temp[i]);
            }

            return lut;
        }

        //Applylut realized in particular case, for bwmorph operation
        //for convolution using prepared vectors, obtained in matlab for 16 and 512 lut (uint8 images) 
        //In future can be independent function
        private static double[,] ApplyLut(double[,] arr, double[] lut)
        {
            double check = Math.Sqrt(Math.Log(lut.Length, 2));
            bool isInt = check % 1 == 0; //check for all lut, if using in future lut except 16, 512 size

            double[,] result = new double[arr.GetLength(0), arr.GetLength(1)];

            if (check == 2 || check == 3)
            {
                var conVectors = PreparedConvVectors((int)check);
                var filter2 = Convolution2.Conv2(arr, conVectors[0].Vect, conVectors[1].Vect, Convback.same).RoundArrayElements();
                var forApplyLut = filter2.ArraySumWithConst(1).DoubleArrayToInt();

                //Проверить lut для 16
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    for (int j = 0; j < arr.GetLength(1); j++)
                    {
                        if (forApplyLut[i, j] >= lut.Length) //not good
                        {
                            result[i, j] = lut[lut.Length - 1];
                        }
                        else if (forApplyLut[i, j] == 0) //not good
                        {
                            result[i, j] = lut[lut.Length];
                        }
                        else
                        {
                            result[i, j] = lut[forApplyLut[i, j]-1];
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine("Sorry, can work only with lut where 16 or 512 lut.");
            }
            
            return result;
        }        

        //prepared vector for 16 and 512 lut for convolution operation
        private static List<VectorsListDouble> PreparedConvVectors(int num)
        {
            double[] hcol = new double[num];
            double[] hrow = new double[num];

            List<VectorsListDouble> List = new List<VectorsListDouble>();

            if (num == 2)
            {
                hcol = new double[2] { -1.3579, -2.7158 };
                hrow = new double[2] { -0.7364, -2.9457 };
                List.Add(new VectorsListDouble() { Vect = hrow });
                List.Add(new VectorsListDouble() { Vect = hcol });
            }

            else if (num == 3)
            {
                hcol = new double[3] { -3.7518, -7.5037, -15.0074 };
                hrow = new double[3] { -0.2665, -2.1323, -17.0583 };
                List.Add(new VectorsListDouble() { Vect = hrow });
                List.Add(new VectorsListDouble() { Vect = hcol });
            }
            else
            {
                Console.WriteLine("I have conv vectors only for lut 16 and 512");
            }

            return List;
        }
    }    

    public enum BwmorphOut
    {
        OneBpp,
        EightBpp,
        TwentyFourBpp
    }    

    public enum BwmorphOpearion
    {
        thin,
        bridge,
        diag,
        shrink,
        skel,
        spur,
        conwaylaw
    }
}
