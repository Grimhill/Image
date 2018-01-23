using System;
using System.Linq;
using Image.ArrayOperations;

//Gradient count
namespace Image
{
    public static class Gradient
    {
        public static double[,] Grad(double[,] Rx, double[,] Ry, double[,] Gx, double[,] Gy, double[,] Bx, double[,] By)
        {
            //ArrayOperations ArrOp = new ArrayOperations();
            //Compute per-plane gradients
            // sqrt(Rx .^ 2 + Ry .^ 2)            
            var RG = Rx.PowArrayElements(2).SumArrays(Ry.PowArrayElements(2)).SqrtArrayElements();

            //sqrt(Gx .^ 2 + Gy .^ 2            
            var GG = Gx.PowArrayElements(2).SumArrays(Gy.PowArrayElements(2)).SqrtArrayElements();

            //sqrt(Bx .^ 2 + By .^ 2)            
            var BG = Bx.PowArrayElements(2).SumArrays(By.PowArrayElements(2)).SqrtArrayElements();

            //Composite gradient image scaled to [0, 1].
            //for ImageOutlines function           
            var PPG = ArrayDoubleExtensions.SumThreeArrays(RG, GG, BG).ArrayDivByConst(ArrayDoubleExtensions.SumThreeArrays(RG, BG, GG).Cast<double>().Max());

            return PPG;
        }

        //Some problems, receive NaN
        public static double[,] GradientExtended(double[,] Rx, double[,] Ry, double[,] Gx, double[,] Gy, double[,] Bx, double[,] By)
        {            
            double[,] Temp = new double[Rx.GetLength(0), Rx.GetLength(1)];
            //Compute the parameters of the vector gradient

            //gxx = Rx .^ 2 + Gx .^ 2 + Bx .^ 2 
            var Gxx = ArrayDoubleExtensions.SumThreeArrays(Rx.PowArrayElements(2), Gx.PowArrayElements(2), Bx.PowArrayElements(2));

            //gyy = Ry .^ 2 + Gy .^ 2 + By .^ 2            
            var Gyy = ArrayDoubleExtensions.SumThreeArrays(Ry.PowArrayElements(2), Gy.PowArrayElements(2), By.PowArrayElements(2));

            //gxy = Rx .* Ry + Gx .* Gy + Bx .* By            
            var Gxy = ArrayDoubleExtensions.SumThreeArrays(Rx.ArrayMultElements(Ry), Gx.ArrayMultElements(Gy), Bx.ArrayMultElements(By));

            //A = 0.5 * (atan((2*gxy) ./ (gxx - gyy + eps))) eps - Floating-point relative accuracy eps = 2.2204e-016            

            var Angle = Gxx.SubArrays(Gyy).ArraySumWithConst((2.2204 * Math.Pow(10, -16)));
            Angle = Gxy.ArrayMultByConst(2).ArraydivElements(Angle).AtanArrayElements().ArrayMultByConst(0.5);
            
            var p1 = Gxx.SumArrays(Gyy);
            
            var p2 = Angle.ArrayMultByConst(2).CosArrayElements();
            p2 = Gxx.SubArrays(Gyy).ArrayMultElements(p2);

            var p3 = Angle.ArrayMultByConst(2).SinArrayElements();
            p3 = Gxy.ArrayMultByConst(2).ArrayMultElements(p3);

            //0.5 * ((gxx + gyy) + (gxx - gyy) .* cos (2*A) + 2 * gxy .* sin (2*A))            
            var Grad = p1.SumArrays(p2).SumArrays(p3).ArrayMultByConst(0.5);

            //repeat for angle + pi / 2            
            Angle = Angle.ArraySumWithConst(1.5708);
            
            var p4 = Gxx.SumArrays(Gyy);
            
            var p5 = Angle.ArrayMultByConst(2).CosArrayElements();
            p5 = Gxx.SubArrays(Gyy).ArrayMultElements(p5);
            
            var p6 = Angle.ArrayMultByConst(2).SinArrayElements();
            p6 = Gxy.ArrayMultByConst(2).ArrayMultElements(p6);

            //0.5 * ((gxx + gyy) + (gxx - gyy) .* cos (2*A) + 2 * gxy .* sin (2*A))            
            var Gra = p4.SumArrays(p5).SumArrays(p6).ArrayMultByConst(0.5);
          
            Grad = Grad.SqrtArrayElements();
            
            Gra = Gra.SqrtArrayElements();

            //Picking the maximum at each (x, y) and then scale to range [0, 1].            
            var VGt = Grad.MaxTwoArrays(Gra).Cast<double>().Max();
            var VG = Grad.MaxTwoArrays(Gra).ArrayDivByConst(VGt);

            return VG;
        }
    }
}
