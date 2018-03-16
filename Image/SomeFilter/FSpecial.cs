using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image
{
    public static class FSpecial
    {
        public static double[,] Average(uint size)
        {
            return AverageProcess(size);
        }
        //default
        public static double[,] Average()
        {
            uint size = 3;
            return AverageProcess(size);
        }       

        //size - square filter side
        private static double[,] AverageProcess(uint size)
        {
            double[,] result = new double[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    result[i, j] = 1 / (double)(size * size);
                }
            }

            return result;
        }

       
        public static double[,] Gaussian(uint size, double sigma)
        {
            return GaussianProcess(size, sigma);
        }
        //default
        public static double[,] Gaussian()
        {
            uint size = 3;
            double sigma = 0.5;
            return GaussianProcess(size, sigma);
        }

        //size - square filter side
        //sigma - standard deviation
        private static double[,] GaussianProcess(uint size, double sigma)
        {
            double[,] result = new double[size, size];
            double siz = ((double)size - 1) / 2;
            double[,] x = new double[size, size];
            double[,] y = new double[size, size];
            double[,] z = new double[size, size];

            if (sigma > 0)
            {
                var trap = -siz;
                for(int i = 0; i < size; i++)
                {
                    for(int j = 0; j < size; j++)
                    {
                        x[i, j] = trap + j;
                        y[i, j] = trap + i;
                    }                   
                }

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        z[i, j] = -(x[i, j] * x[i, j] + y[i, j] * y[i, j])/(2 * sigma * sigma);
                    }
                }

                result = z.ExpArrayElements();
                var cut = 2.2204 * Math.Pow(10, -16) * result.Cast<double>().Max();
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if(result[i, j] < cut) { result[i, j] = 0; }                        
                    }
                }

                var sum = result.Cast<double>().Sum();
                if(sum != 0)
                {
                    result = result.ArrayDivByConst(sum);
                }
            }
            else
            { Console.WriteLine("Sigma must be positive."); }

            return result;
        }


        //use with image: var f = filter(img, Laplacian) than result = img - f;
        public static double[,] Laplacian(double alpha)
        {
            return LaplacianProcess(alpha);
        }
        //default
        public static double[,] Laplacian()
        {
            double alpha = 0.2;
            return LaplacianProcess(alpha);
        }
        
        //alpha controls the shape of the Laplacian and must be in the range 0.0 to 1.0
        private static double[,] LaplacianProcess(double alpha)
        {
            double[,] result = new double[3,3];
            if (alpha > 0 && alpha <= 1)
            {
                var h1 = alpha / (alpha + 1);
                var h2 = (1 - alpha) / (alpha + 1);

                result = new double[3, 3]
                { {h1, h2, h1 }, {h2, (-4)/(alpha + 1), h2}, {h1, h2, h1 } };
            }            
            else
            { Console.WriteLine("Alpha must be in range [0..1]"); }
            return result;
        }


        //use with image: var f = filter(img, LapofGas) than result = img - f;
        //sigma reccomended not more, than 1
        public static double[,] LaplacofGauss(uint size, double sigma)
        {
            return LaplacofGaussProcess(size, sigma);
        }
        //default
        public static double[,] LaplacofGauss()
        {
            uint size = 5;
            double sigma = 0.5;
            return LaplacofGaussProcess(size, sigma);
        }

        //rotationally symmetric Laplacian of Gaussian filter of size size 
        //with standard deviation sigma (positive)
        private static double[,] LaplacofGaussProcess(uint size, double sigma)
        {
            double[,] result = new double[size, size];
            double[,] temp = new double[size, size];

            double siz = ((double)size - 1) / 2;
            double[,] x = new double[size, size];
            double[,] y = new double[size, size];
            double[,] z = new double[size, size];

            if (sigma > 0)
            {
                var trap = -siz;
                var std = sigma * sigma;

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        x[i, j] = trap + j;
                        y[i, j] = trap + i;
                    }
                }             

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        z[i, j] = -(x[i, j] * x[i, j] + y[i, j] * y[i, j]) / (2 * std);
                    }
                }

                temp = z.ExpArrayElements();
                var cut = 2.2204 * Math.Pow(10, -16) * temp.Cast<double>().Max();
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (temp[i, j] < cut) { temp[i, j] = 0; }
                    }
                }

                var sum = temp.Cast<double>().Sum();
                if (sum != 0)
                {
                    temp = temp.ArrayDivByConst(sum);
                }

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        temp[i, j] = temp[i, j] * (x[i, j] * x[i, j] + y[i, j] * y[i, j] - 2 * std) / (std * std);
                    }
                }
                sum = temp.Cast<double>().Sum();

                result = temp.ArraySubWithConst(sum/(size * size));
            }
            else
            { Console.WriteLine("Sigma must be positive."); }
                        
            return result;
        }


        public static double[,] Unsharp(double alpha)
        {
            return UnsharpProcess(alpha);
        }
        //default
        public static double[,] Unsharp()
        {
            double alpha = 0.2;
            return UnsharpProcess(alpha);
        }

        //alpha controls the shape of the Laplacian and must be in the range 0.0 to 1.0
        private static double[,] UnsharpProcess(double alpha)
        {
            double[,] result = new double[3, 3];

            if (alpha > 0 && alpha <= 1)
            {
                double[,] temp = new double[3, 3] { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };

                result = temp.SubArrays(LaplacianProcess(alpha));
            }
            else
            { Console.WriteLine("Alpha must be in range [0..1]. Return black square filter"); }

            return result;
        }


        public static double[,] Motion(uint len, double theta)
        {
            return MotionProcess(len, theta);
        }
        //default
        public static double[,] Motion()
        {
            uint Len = 9;
            double Theta = 0;
            return MotionProcess(Len, Theta);
        }

        //linear motion of a camera by Len pixels
        //with an angle of Theta degrees
        //realization at the level "Bratiwka, ya tebe pokywat prines"
        private static double[,] MotionProcess(uint len, double theta)
        {
            double[,] result = new double[1, 1];
            ArrGen<double> d = new ArrGen<double>();

            if (len > 1)
            {
                //Floating-point relative accuracy
                double eps = 2.2204 * Math.Pow(10, -16);

                //rotate half length around center
                var half = ((double)len - 1) / 2;
                var phi = (theta % 180) / 180 * Math.PI;

                var cosPhi = Math.Cos(phi);
                var sinPhi = Math.Sin(phi);
                var xsign  = Math.Sign(cosPhi);
                if (xsign == 0 || xsign == -1)
                { Console.WriteLine("Something wrong with input parameters. Return empty."); return result; }
                double linewdt = 1;

                //define mesh for the half matrix, eps takes care of the right size for 0 & 90 rotation
                var sx = half * cosPhi + linewdt * xsign - len * eps;
                if (sx > 0) sx = Math.Floor(sx);
                else { sx = -(Math.Floor(-sx)); }

                var sy = half * sinPhi + linewdt - len * eps;
                if (sy > 0) sy = Math.Floor(sy);
                else { sy = -(Math.Floor(-sy)); }

                double[,] x = new double[(int)sy + 1, (int)sx + 1];
                double[,] y = new double[(int)sy + 1, (int)sx + 1];

                for(int i = 0; i <= sy; i++)
                {
                    for(int j = 0; j <= sx; j++)
                    {
                        x[i, j] = j;
                        y[i, j] = i;
                    }
                }

                //define shortest distance from a pixel to the rotated line 
                var dist2line = y.ArrayMultByConst(cosPhi).SubArrays(x.ArrayMultByConst(sinPhi));
                var rad = x.PowArrayElements(2).SumArrays(y.PowArrayElements(2)).SqrtArrayElements();

                //find points beyond the line's end-point but within the line width
                var tempGreater = rad.MarkGreaterEqual(half, BabaYaga.logic);
                var tempLess = dist2line.AbsArrayElements().MarkLessEqual(linewdt, BabaYaga.logic);
                tempGreater = tempGreater.ArrayMultElements(tempLess);

                List<int> lastpix = new List<int>();                     

                var lastPixVector = d.ArrayToVectorColByCol(tempGreater);
                for(int i = 0; i < lastPixVector.Count(); i++)
                {
                    if(lastPixVector[i] != 0) { lastpix.Add(i); }
                }

                //distance to the line's end-point parallel to the line
                //transform into vectors, for more convenient process (for me naturally :D)
                var xVector = d.ArrayToVectorColByCol(x);
                var dist2lineVector = d.ArrayToVectorColByCol(dist2line);

                List<double> x2lastpix = new List<double>();
                for(int i = 0; i < lastpix.Count; i++)
                {
                    x2lastpix.Add(half - Math.Abs((xVector[lastpix[i]] + dist2lineVector[lastpix[i]] * sinPhi)/cosPhi));
                    dist2lineVector[lastpix[i]] = Math.Sqrt(Math.Pow(dist2lineVector[lastpix[i]],2) + Math.Pow(x2lastpix[i],2));
                }

                for(int i = 0; i < dist2lineVector.Length; i++)
                {
                    dist2lineVector[i] = linewdt + eps - Math.Abs(dist2lineVector[i]);
                    if (dist2lineVector[i] < 0) dist2lineVector[i] = 0; //zero out anything beyond line width
                }

                //back to 2D
                dist2line = d.VecorToArrayColbyCol(dist2line.GetLength(0), dist2line.GetLength(1), dist2lineVector);

                //unfold half - matrix to the full size
                result = new double[dist2line.GetLength(0) * 2 - 1, dist2line.GetLength(1) * 2 - 1];
                var temp = Rotate90.RotateArray90(dist2line, 2);               
                var reverse = d.VecorToArrayRowByRow(temp.GetLength(0), temp.GetLength(1), temp.Cast<double>().Reverse().ToArray());
                
                for (int i = 0; i < temp.GetLength(0); i++)
                {
                    for (int j = 0; j < temp.GetLength(1); j++)
                    {
                        result[i, j] = temp[i, j];
                    }
                }

                for (int i = 0; i < reverse.GetLength(0); i++)
                {
                    for (int j = 0; j < reverse.GetLength(1); j++)
                    {
                        result[i + reverse.GetLength(0) - 1, j + reverse.GetLength(1) - 1] = reverse[i, j];
                    }
                }

                result = result.ArrayDivByConst(result.Cast<double>().Sum() + eps * len * len);

                if(cosPhi > 0)
                {
                    result = d.FlipArray(result);
                }
               
            }
            else
            { Console.WriteLine("Len must be positive integer value, greater than 1. Method: FSpecial.Motion(). Return black rectangle."); }

            return result;
        }


        public static double[,] Disk(double radius)
        {
            return DiskProcess(radius);
        }
        //default
        public static double[,] Disk()
        {          
            double radius = 5;
            return DiskProcess(radius);
        }

        //circular averaging filter
        //within the square matrix of side 2*radius+1
        //realization at the level "Bratiwka, ya tebe pokywat prines"
        private static double[,] DiskProcess(double radius)
        {
            double[,] result = new double[1, 1];
            ArrGen<double> d = new ArrGen<double>();

            if (radius > 0)
            {
                var crad = Math.Ceiling(radius - 0.5);
                int rc = 0;
                for(int i = (int)(-crad); i <= crad; i++)
                { rc++; }

                double[,] x = new double[rc, rc];
                double[,] y = new double[rc, rc];

                var trap = -crad;
                for (int i = 0; i < rc; i++)
                {
                    for (int j = 0; j < rc; j++)
                    {
                        x[i, j] = trap + j;
                        y[i, j] = trap + i;
                    }
                }

                var maxxy = x.AbsArrayElements().MaxTwoArrays(y.AbsArrayElements());
                var minxy = x.AbsArrayElements().MinTwoArrays(y.AbsArrayElements());
                                
                var maxxyVector = maxxy.Cast<double>().ToArray();
                var minxyVector = minxy.Cast<double>().ToArray();

                //temp vectors and variables
                double[] temp1 = new double[rc * rc];
                double[] temp2 = new double[rc * rc];               
                double temp = 0; 
                Complex tempComplex;

                //m1 count by partial 
                double[] m1tempCusum = new double[rc * rc];                            
               
                for (int i = 0; i < m1tempCusum.Length; i++)
                {
                    //a = (rad^2 < (maxxy+0.5).^2 + (minxy-0.5).^2).*(minxy-0.5)
                    if (Math.Pow(radius, 2) < (Math.Pow(maxxyVector[i] + 0.5, 2) + Math.Pow(minxyVector[i] - 0.5, 2)))
                    { temp = 1; }                    
                    temp1[i] = temp * (minxyVector[i] - 0.5); temp = 0;

                    // b =(rad^2 >= (maxxy+0.5).^2 + (minxy-0.5).^2).* sqrt(rad ^ 2 - (maxxy + 0.5).^ 2)
                    if (Math.Pow(radius, 2) >= (Math.Pow(maxxyVector[i] + 0.5, 2) + Math.Pow(minxyVector[i] - 0.5, 2)))
                    { temp = 1; }                    
                    tempComplex = Complex.Sqrt(Math.Pow(radius, 2) - Math.Pow(maxxyVector[i] + 0.5, 2));
                    temp2[i] = temp * (tempComplex.Real + tempComplex.Imaginary); temp = 0;
                }                
                 //a + b
                m1tempCusum = temp1.SumVectors(temp2);
                var m1 = d.VecorToArrayRowByRow(rc, rc, m1tempCusum);

                temp1 = new double[rc * rc]; temp2 = new double[rc * rc];
                //m2 count by partial
                double[] m2tempCusum = new double[rc * rc];

                for (int i = 0; i < m2tempCusum.Length; i++)
                {
                    //a = (rad^2 > (maxxy-0.5).^2 + (minxy+0.5).^2).*(minxy+0.5)
                    if (Math.Pow(radius, 2) > (Math.Pow(maxxyVector[i] - 0.5, 2) + Math.Pow(minxyVector[i] + 0.5, 2)))
                    { temp = 1; }                    
                    temp1[i] = temp * (minxyVector[i] + 0.5); temp = 0;

                    // b = (rad^2 <= (maxxy-0.5).^2 + (minxy+0.5).^2).* sqrt(rad ^ 2 - (maxxy - 0.5).^ 2)
                    if (Math.Pow(radius, 2) <= (Math.Pow(maxxyVector[i] - 0.5, 2) + Math.Pow(minxyVector[i] + 0.5, 2)))
                    { temp = 1; }
                    temp2[i] = temp * Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(maxxyVector[i] - 0.5, 2)); temp = 0;
                }
                //a + b
                m2tempCusum = temp1.SumVectors(temp2);
                var m2 = d.VecorToArrayRowByRow(rc, rc, m2tempCusum);

                temp1 = new double[rc * rc]; temp2 = new double[rc * rc];
                //sgrid count by partial
                double[] sgridVector = new double[rc * rc];

                for (int i = 0; i < sgridVector.Length; i++)
                {
                    //rad^2*(0.5*(asin(m2/rad) - asin(m1/rad)) + 0.25 * (sin(2 * asin(m2 / rad)) - sin(2 * asin(m1 / rad)))) - (maxxy - 0.5).* (m2 - m1) + (m1 - minxy + 0.5)
                    temp1[i] = Math.Pow(radius, 2) * (0.5 * (Math.Asin(m2tempCusum[i]/radius) - Math.Asin(m1tempCusum[i]/radius)) + 
                        0.25 * (Math.Sin(2 * Math.Asin(m2tempCusum[i]/radius)) - Math.Sin(2 * Math.Asin(m1tempCusum[i]/radius)))) -
                        (maxxyVector[i] - 0.5) * (m2tempCusum[i] - m1tempCusum[i]) + 
                        (m1tempCusum[i] - minxyVector[i] + 0.5);

                    // ((rad^2 < (maxxy+0.5).^2 + (minxy+0.5).^2) & (rad ^ 2 > (maxxy - 0.5).^ 2 + (minxy - 0.5).^ 2)) | ((minxy == 0) & (maxxy - 0.5 < rad) & (maxxy + 0.5 >= rad))
                    if (Math.Pow(radius, 2) < (Math.Pow(maxxyVector[i] + 0.5, 2) + Math.Pow(minxyVector[i] + 0.5, 2)))
                    { temp = 1; }

                    if(Math.Pow(radius, 2) > (Math.Pow(maxxyVector[i] - 0.5, 2) + Math.Pow(minxyVector[i] - 0.5, 2)) && temp == 1)
                    { temp = 1; } else { temp = 0; }

                    if(((minxyVector[i] == 0) && (maxxyVector[i] - 0.5 < radius) && (maxxyVector[i] + 0.5 >= radius)) || temp == 1)
                    { temp = 1; } else { temp = 0; }

                    temp2[i] = temp; temp = 0;
                }

                //a * b
                sgridVector = temp1.MultVectors(temp2);

                for (int i = 0; i < sgridVector.Length; i++)
                {
                    if((Math.Pow(maxxyVector[i] + 0.5, 2) + Math.Pow(minxyVector[i] + 0.5, 2)) < Math.Pow(radius, 2))
                    { temp = 1; }

                    sgridVector[i] = sgridVector[i] + temp; temp = 0;
                }

                var sgrid = d.VecorToArrayRowByRow(rc, rc, sgridVector);
                sgrid[(int)crad, (int)crad] = Math.Min(Math.PI * Math.Pow(radius, 2), Math.PI / 2);

                if((crad > 0) && (radius > (crad - 0.5)) && (Math.Pow(radius, 2) < (Math.Pow(crad - 0.5, 2) + 0.25)))
                {
                    var tempm1 = Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(crad - 0.5, 2));
                    var tempm1n = tempm1 / radius;
                    var sg0 = 2 * (Math.Pow(radius, 2) * (0.5 * Math.Asin(tempm1n) + 
                        0.25 * Math.Sin(2 * Math.Asin(tempm1n))) - tempm1 * (crad - 0.5));

                    sgrid[2 * (int)crad, (int)crad] = sg0;
                    sgrid[(int)crad, 2 * (int)crad] = sg0;
                    sgrid[(int)crad, 0] = sg0;
                    sgrid[0, (int)crad] = sg0;

                    sgrid[2 * (int)crad - 1, (int)crad] = sgrid[(2 * (int)crad - 1), (int)crad] - sg0;
                    sgrid[(int)crad, 2 * (int)crad - 1] = sgrid[(int)crad, 2 * (int)crad - 1] - sg0;
                    sgrid[(int)crad, 1] = sgrid[(int)crad, 1] - sg0;
                    sgrid[1, (int)crad] = sgrid[1, (int)crad] - sg0;                   
                }
                sgrid[(int)crad, (int)crad] = Math.Min(sgrid[(int)crad, (int)crad], 1);                

                result = new double[rc, rc];
                result = d.VecorToArrayRowByRow(rc, rc, sgrid.Cast<double>().ToArray().VectorDivByConst(sgrid.Cast<double>().ToArray().Sum()));                
            }
            else { Console.WriteLine("Radius must be greater 0. Method: FSpecial.Radius(). Return black point."); }

            return result;
        }
    }
}
