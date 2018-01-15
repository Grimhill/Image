using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Gradient count
namespace Image
{
    public static class Gradient
    {
        public static double[,] Grad(double[,] Rx, double[,] Ry, double[,] Gx, double[,] Gy, double[,] Bx, double[,] By)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            //Compute per-plane gradients
            // sqrt(Rx .^ 2 + Ry .^ 2)
            var RG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(Rx, 2), ArrOp.PowArrayElements(Ry, 2)));
            //sqrt(Gx .^ 2 + Gy .^ 2
            var GG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(Gx, 2), ArrOp.PowArrayElements(Gy, 2)));
            //sqrt(Bx .^ 2 + By .^ 2)
            var BG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(Bx, 2), ArrOp.PowArrayElements(By, 2)));

            //Composite gradient image scaled to [0, 1].
            //for ImageOutlines function
            var PPG = ArrOp.ArrayDivByConst(ArrOp.SumThreeArrays(RG, GG, BG), ArrOp.SumThreeArrays(RG, GG, BG).Cast<double>().Max()); //per-line gradient

            return PPG;
        }

        //Some problems, receive NaN
        public static double[,] GradientExtended(double[,] Rx, double[,] Ry, double[,] Gx, double[,] Gy, double[,] Bx, double[,] By)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            double[,] Temp = new double[Rx.GetLength(0), Rx.GetLength(1)];
            //Compute the parameters of the vector gradient

            //gxx = Rx .^ 2 + Gx .^ 2 + Bx .^ 2           
            var Gxx = ArrOp.SumThreeArrays(ArrOp.PowArrayElements(Rx, 2), ArrOp.PowArrayElements(Gx, 2), ArrOp.PowArrayElements(Bx, 2));
            //gyy = Ry .^ 2 + Gy .^ 2 + By .^ 2
            var Gyy = ArrOp.SumThreeArrays(ArrOp.PowArrayElements(Ry, 2), ArrOp.PowArrayElements(Gy, 2), ArrOp.PowArrayElements(By, 2));
            //gxy = Rx .* Ry + Gx .* Gy + Bx .* By
            var Gxy = ArrOp.SumThreeArrays(ArrOp.ArrayMultElements(Rx, Ry), ArrOp.ArrayMultElements(Gx, Gy), ArrOp.ArrayMultElements(Bx, By));

            //A = 0.5 * (atan((2*gxy) ./ (gxx - gyy + eps))) eps - Floating-point relative accuracy eps = 2.2204e-016
            var Angle = ArrOp.ArrayMultByConst(ArrOp.AtanArrayElements(ArrOp.ArraydivElements(ArrOp.ArrayMultByConst(Gxy, 2), ArrOp.ArraySumWithConst(ArrOp.SubArrays(Gxx, Gyy), 2.2204 * Math.Pow(10, -16)))), 0.5);

            var p1 = ArrOp.SumArrays(Gxx, Gyy);
            var p2 = ArrOp.ArrayMultElements(ArrOp.SubArrays(Gxx, Gyy), ArrOp.CosArrayElements(ArrOp.ArrayMultByConst(Angle, 2)));
            var p3 = ArrOp.ArrayMultElements(ArrOp.ArrayMultByConst(Gxy, 2), ArrOp.SinArrayElements(ArrOp.ArrayMultByConst(Angle, 2)));
            //0.5 * ((gxx + gyy) + (gxx - gyy) .* cos (2*A) + 2 * gxy .* sin (2*A))
            var Grad = ArrOp.ArrayMultByConst(ArrOp.SumArrays(ArrOp.SumArrays(p1, p2), p3), 0.5);

            //repeat for angle + pi / 2
            Angle = ArrOp.ArraySumWithConst(Angle, 1.5708);
            var p4 = ArrOp.SumArrays(Gxx, Gyy);
            var p5 = ArrOp.ArrayMultElements(ArrOp.SubArrays(Gxx, Gyy), ArrOp.CosArrayElements(ArrOp.ArrayMultByConst(Angle, 2)));
            var p6 = ArrOp.ArrayMultElements(ArrOp.ArrayMultByConst(Gxy, 2), ArrOp.SinArrayElements(ArrOp.ArrayMultByConst(Angle, 2)));
            //0.5 * ((gxx + gyy) + (gxx - gyy) .* cos (2*A) + 2 * gxy .* sin (2*A))
            var Gra = ArrOp.ArrayMultByConst(ArrOp.SumArrays(ArrOp.SumArrays(p4, p5), p6), 0.5);

            Grad = ArrOp.SqrtArrayElements(Grad);
            Gra = ArrOp.SqrtArrayElements(Gra);

            //Picking the maximum at each (x, y) and then scale to range [0, 1].
            var VG = ArrOp.ArrayDivByConst(ArrOp.MaxTwoArrays(Grad, Gra), ArrOp.MaxTwoArrays(Grad, Gra).Cast<double>().Max());

            return VG;
        }
    }
}
