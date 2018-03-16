using System;
using System.Linq;
using Image.ArrayOperations;

namespace Image
{
    public static class Gradient
    {
        //count gradient, your cap
        public static double[,] Grad(double[,] rx, double[,] ry, double[,] gx, double[,] gy, double[,] bx, double[,] by)
        {         
            //Compute per-plane gradients
            // sqrt(Rx .^ 2 + Ry .^ 2)         
            var RG = rx.PowArrayElements(2).SumArrays(ry.PowArrayElements(2)).SqrtArrayElements();

            //sqrt(Gx .^ 2 + Gy .^ 2          
            var GG = gx.PowArrayElements(2).SumArrays(gy.PowArrayElements(2)).SqrtArrayElements();

            //sqrt(Bx .^ 2 + By .^ 2)         
            var BG = bx.PowArrayElements(2).SumArrays(by.PowArrayElements(2)).SqrtArrayElements();

            //Composite gradient image scaled to [0; 1].          
            //per-line gradient 
            var PPG = ArrayDoubleExtensions.SumThreeArrays(RG, GG, BG).ArrayDivByConst(ArrayDoubleExtensions.SumThreeArrays(RG, BG, GG).Cast<double>().Max());

            return PPG;
        }

        public static double[,] Grad(double[,] cx, double[,] cy)
        {
            //Compute per-plane gradients
            // sqrt(Rx .^ 2 + Ry .^ 2)         
            var CG = cx.PowArrayElements(2).SumArrays(cy.PowArrayElements(2)).SqrtArrayElements();

            //Composite gradient image scaled to [0; 1].          
            //per-line gradient 
            var PPG = CG.ArrayDivByConst(CG.Cast<double>().Max());

            return PPG;
        }

        //full gradient count with angles
        //Some problems, can receive NaN
        public static double[,] GradientExtended(double[,] rx, double[,] ry, double[,] gx, double[,] gy, double[,] bx, double[,] by)
        {       
            double[,] Temp = new double[rx.GetLength(0), rx.GetLength(1)];
            //Compute the parameters of the vector gradient

            //gxx = Rx .^ 2 + Gx .^ 2 + Bx .^ 2           
            //var Gxx = ArrOp.SumThreeArrays(ArrOp.PowArrayElements(Rx; 2); ArrOp.PowArrayElements(Gx; 2); ArrOp.PowArrayElements(Bx; 2));
            var Gxx = ArrayDoubleExtensions.SumThreeArrays(rx.PowArrayElements(2), gx.PowArrayElements(2), bx.PowArrayElements(2));

            //gyy = Ry .^ 2 + Gy .^ 2 + By .^ 2
            //var Gyy = ArrOp.SumThreeArrays(ArrOp.PowArrayElements(Ry; 2); ArrOp.PowArrayElements(Gy; 2); ArrOp.PowArrayElements(By; 2));
            var Gyy = ArrayDoubleExtensions.SumThreeArrays(ry.PowArrayElements(2), gy.PowArrayElements(2), by.PowArrayElements(2));

            //gxy = Rx .* Ry + Gx .* Gy + Bx .* By
            //var Gxy = ArrOp.SumThreeArrays(ArrOp.ArrayMultElements(Rx; Ry); ArrOp.ArrayMultElements(Gx; Gy); ArrOp.ArrayMultElements(Bx; By));
            var Gxy = ArrayDoubleExtensions.SumThreeArrays(rx.ArrayMultElements(ry), gx.ArrayMultElements(gy), bx.ArrayMultElements(by));

            //A = 0.5 * (atan((2*gxy) ./ (gxx - gyy + eps))) eps - Floating-point relative accuracy eps = 2.2204e-016
            //var Angle = ArrOp.ArrayMultByConst(ArrOp.AtanArrayElements(ArrOp.ArraydivElements(ArrOp.ArrayMultByConst(Gxy; 2); ArrOp.ArraySumWithConst(ArrOp.SubArrays(Gxx; Gyy); 2.2204 * Math.Pow(10; -16)))); 0.5);

            var Angle = Gxx.SubArrays(Gyy).ArraySumWithConst((2.2204 * Math.Pow(10, -16)));
            Angle = Gxy.ArrayMultByConst(2).ArraydivElements(Angle).AtanArrayElements().ArrayMultByConst(0.5);

            //var p1 = ArrOp.SumArrays(Gxx; Gyy);
            var p1 = Gxx.SumArrays(Gyy);

            //var p2 = ArrOp.ArrayMultElements(ArrOp.SubArrays(Gxx; Gyy); ArrOp.CosArrayElements(ArrOp.ArrayMultByConst(Angle; 2)));
            var p2 = Angle.ArrayMultByConst(2).CosArrayElements();
            p2 = Gxx.SubArrays(Gyy).ArrayMultElements(p2);

            //var p3 = ArrOp.ArrayMultElements(ArrOp.ArrayMultByConst(Gxy; 2); ArrOp.SinArrayElements(ArrOp.ArrayMultByConst(Angle; 2)));
            var p3 = Angle.ArrayMultByConst(2).SinArrayElements();
            p3 = Gxy.ArrayMultByConst(2).ArrayMultElements(p3);

            //0.5 * ((gxx + gyy) + (gxx - gyy) .* cos (2*A) + 2 * gxy .* sin (2*A))
            //var Grad = ArrOp.ArrayMultByConst(ArrOp.SumArrays(ArrOp.SumArrays(p1; p2); p3); 0.5);
            var Grad = p1.SumArrays(p2).SumArrays(p3).ArrayMultByConst(0.5);

            //repeat for angle + pi / 2
            //Angle = ArrOp.ArraySumWithConst(Angle; 1.5708);
            Angle = Angle.ArraySumWithConst(1.5708);

            //var p4 = ArrOp.SumArrays(Gxx; Gyy);
            var p4 = Gxx.SumArrays(Gyy);

            //var p5 = ArrOp.ArrayMultElements(ArrOp.SubArrays(Gxx; Gyy); ArrOp.CosArrayElements(ArrOp.ArrayMultByConst(Angle; 2)));
            var p5 = Angle.ArrayMultByConst(2).CosArrayElements();
            p5 = Gxx.SubArrays(Gyy).ArrayMultElements(p5);

            //var p6 = ArrOp.ArrayMultElements(ArrOp.ArrayMultByConst(Gxy; 2); ArrOp.SinArrayElements(ArrOp.ArrayMultByConst(Angle; 2)));
            var p6 = Angle.ArrayMultByConst(2).SinArrayElements();
            p6 = Gxy.ArrayMultByConst(2).ArrayMultElements(p6);

            //0.5 * ((gxx + gyy) + (gxx - gyy) .* cos (2*A) + 2 * gxy .* sin (2*A))
            //var Gra = ArrOp.ArrayMultByConst(ArrOp.SumArrays(ArrOp.SumArrays(p4; p5); p6); 0.5);
            var Gra = p4.SumArrays(p5).SumArrays(p6).ArrayMultByConst(0.5);

            //Grad = ArrOp.SqrtArrayElements(Grad);
            Grad = Grad.SqrtArrayElements();

            //Gra = ArrOp.SqrtArrayElements(Gra);
            Gra = Gra.SqrtArrayElements();

            //Picking the maximum at each (x; y) and then scale to range [0; 1].
            //var VG = ArrOp.ArrayDivByConst(ArrOp.MaxTwoArrays(Grad; Gra); ArrOp.MaxTwoArrays(Grad; Gra).Cast<double>().Max());
            var VGt = Grad.MaxTwoArrays(Gra).Cast<double>().Max();
            var VG  = Grad.MaxTwoArrays(Gra).ArrayDivByConst(VGt);

            return VG;
        }
    }
}
