using System;
using System.Linq;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image
{
    public static class Convolution2
    {
        //try make T? - nope, there good enough with double operate. We always can convert type to int etc.
        //and problem  "cannot be applied to operands of type 'T' and 'T'" - need additional code for operator overload?
        //(A[,], hrow, hcol, Convback) -  make partial - with vectors - done
        //(A[,], u[,], Convback)

        //Convolution of 2D array and 2 vectors represented as row and col
        public static double[,] Conv2(double[,] inArray, double[] hrow,double[] hcol, Convback convback)
        {
            return Conv2Process(inArray, hrow, hcol, convback);
        }

        //Overloads Need or not?
        public static int[,] Conv2(int[,] inArray, int[] hrow, int[] hcol, Convback convback)
        {
            return Conv2Process(inArray.ArrayToDouble(), hrow.VectorToDouble(), hcol.VectorToDouble(), convback).DoubleArrayToInt();
        }

        public static double[,] Conv2(int[,] inArray, double[] hrow, double[] hcol, Convback convback)
        {
            return Conv2Process(inArray.ArrayToDouble(), hrow, hcol, convback);
        }

        public static double[,] Conv2(int[,] inArray, double[] hrow, int[] hcol, Convback convback)
        {
            return Conv2Process(inArray.ArrayToDouble(), hrow, hcol.VectorToDouble(), convback);
        }

        public static double[,] Conv2(int[,] inArray, int[] hrow, double[] hcol, Convback convback)
        {
            return Conv2Process(inArray.ArrayToDouble(), hrow.VectorToDouble(), hcol, convback);
        }

        public static double[,] Conv2(double[,] inArray, double[] hrow, int[] hcol, Convback convback)
        {
            return Conv2Process(inArray, hrow, hcol.VectorToDouble(), convback);
        }

        public static double[,] Conv2(double[,] inArray, int[] hrow, double[] hcol, Convback convback)
        {
            return Conv2Process(inArray, hrow.VectorToDouble(), hcol, convback);
        }


        //Convolution of 2D arrays      
        public static double[,] Conv2(double[,] inArray, double[,] convArray, Convback convback)
        {
            return Conv2Process(inArray, convArray, convback);
        }

        //Overloads Need or not?
        public static int[,] Conv2(int[,] inArray, int[,] convArray, Convback convback)
        {
            return Conv2Process(inArray.ArrayToDouble(), convArray.ArrayToDouble(), convback).DoubleArrayToInt();
        }

        public static double[,] Conv2(int[,] inArray, double[,] convArray, Convback convback)
        {
            return Conv2Process(inArray.ArrayToDouble(), convArray, convback);
        }

        public static double[,] Conv2(double[,] inArray, int[,] convArray, Convback convback)
        {
            return Conv2Process(inArray, convArray.ArrayToDouble(), convback);
        }


        private static double[,] Conv2Process(double[,] inArray, double[] hrow, double[] hcol, Convback convback)
        {
            ArrGen<double> d = new ArrGen<double>();

            double[,] result = new double[inArray.GetLength(0) + hrow.Length - 1, inArray.GetLength(1) + hcol.Length - 1];

            List<double> tempColList = new List<double>();
            List<double> tempRowList = new List<double>();
            List<double> tempResults = new List<double>();

            var arrayCols = d.ReturnListof2DArrayCols(inArray);

            //first convolves columns with col vector
            for (int i = 0; i < inArray.GetLength(1); i++)
            {
                tempResults = Convolution.Conv(arrayCols[i].Vect, hrow, Convback.full).ToList();
                tempColList.AddRange(tempResults);
            }

            var colsResult = d.VecorToArrayColbyCol(result.GetLength(0), inArray.GetLength(1), tempColList.ToArray());
            var arrayRows = d.ReturnListof2DArrayRows(colsResult);

            //convolves rows of the result with row vector
            for (int i = 0; i < colsResult.GetLength(0); i++)
            {
                tempResults = Convolution.Conv(arrayRows[i].Vect, hcol, Convback.full).ToList();
                tempRowList.AddRange(tempResults);
            }
            
            var convResult = d.VecorToArrayRowByRow(result.GetLength(0), result.GetLength(1), tempRowList.ToArray());
            result = convResult;

            if (convback == Convback.same)
            {
                int indexCols = 0;
                int indexRows = 0;

                if (hcol.Length % 2 != 0)
                { indexCols = (hcol.Length - 1) / 2; }
                else
                { indexCols = hcol.Length / 2; }

                if (hrow.Length % 2 != 0)
                { indexRows = (hrow.Length - 1) / 2; }
                else
                { indexRows = hrow.Length / 2; }

                tempResults = new List<double>();
                var RowsCut = d.ArrayToVectorColByCol(convResult).ToList();
                for (int i = 0; i < convResult.GetLength(1); i++)
                {
                    int index = convResult.GetLength(0) * i + indexRows;
                    var rowPal = RowsCut.GetRange(index, inArray.GetLength(0));
                    tempResults.AddRange(rowPal);
                }

                var sameRows = d.VecorToArrayColbyCol(inArray.GetLength(0), convResult.GetLength(1), tempResults.ToArray());

                tempResults = new List<double>();
                var ColsCut = sameRows.Cast<double>().ToList();
                for (int i = 0; i < inArray.GetLength(0); i++)
                {
                    int index = convResult.GetLength(1) * i + indexCols;
                    var colPal = ColsCut.GetRange(index, inArray.GetLength(1));
                    tempResults.AddRange(colPal);
                }

                result = new double[inArray.GetLength(0), inArray.GetLength(1)];
                result = d.VecorToArrayRowByRow(inArray.GetLength(0), inArray.GetLength(1), tempResults.ToArray());
            }

            result.DecimalCorrection();
            return result;
        }

        //forgive for such implementation. Slow and ugly...
        private static double[,] Conv2Process(double[,] inArray, double[,] convArray, Convback convback)
        {
            ArrGen<double> d = new ArrGen<double>();          

            double[,] result = new double[inArray.GetLength(0) + convArray.GetLength(0) - 1, inArray.GetLength(1) + convArray.GetLength(1) - 1];
            
            List<double> tempResults = new List<double>();
            double[] tempResult = new double[result.GetLength(0)];

            List<VectorsListDouble> aRListIn  = new List<VectorsListDouble>();
            List<VectorsListDouble> aRListOut = new List<VectorsListDouble>();
            List<VectorsListDouble> temp      = new List<VectorsListDouble>();
            List<VectorsListDouble> tempRes   = new List<VectorsListDouble>();

            int sameRow = convArray.GetLength(0); int sameRowPart = inArray.GetLength(0);
            int sameCol = convArray.GetLength(1); int sameColPart = inArray.GetLength(1);

            //lazy cheat
            if (inArray.GetLength(1) < convArray.GetLength(1))
            {
                var tempIn = inArray;
                inArray = new double[convArray.GetLength(0), convArray.GetLength(1)];
                inArray = convArray;
                convArray = new double[inArray.GetLength(0), inArray.GetLength(1)];
                convArray = tempIn;
            }

            var arrayCols = d.ReturnListof2DArrayCols(inArray);
            var convCols = d.ReturnListof2DArrayCols(convArray);

            //conv col by col
            for (int i = 0; i < convArray.GetLength(1); i++)
            {
                for (int j = 0; j < inArray.GetLength(1); j++)
                {
                    tempResult = Convolution.Conv(arrayCols[j].Vect, convCols[i].Vect, Convback.full);
                    aRListIn.Add(new VectorsListDouble() { Vect = tempResult });
                }
            }

            if (convArray.GetLength(1) == 1)
            {
                aRListOut = aRListIn;
            }
            else
            {
                var first = aRListIn[0].Vect;
                var last = aRListIn[aRListIn.Count - 1].Vect;
                aRListOut.Add(new VectorsListDouble() { Vect = first });

                aRListIn.RemoveAt(0); aRListIn.RemoveAt(aRListIn.Count - 1);

                int maxAmount = convArray.GetLength(1); //максимальное число суммируемых стобцов в один //max number of cols to one sum
                int maxSumCount = 0;                    //количество maxAmount  //count maxAmount
                int simpleSumCount = 0;                 //количество остальных сумм //count another sums
                int sumCount = result.GetLength(1) - 2; //кроличество всех сумм //count all sums
                int step = 0;                           //шаг через который суммируются //sum step

                if (convArray.GetLength(1) == 2)
                {
                    simpleSumCount = inArray.GetLength(1) - 1;
                    step = inArray.GetLength(1) - 1;

                    for (int i = 0; i < sumCount; i++)
                    {
                        aRListOut.Add(new VectorsListDouble() { Vect = aRListIn[i].Vect.SumVectors(aRListIn[i + step].Vect) });
                    }
                }
                else
                {                   
                    step = inArray.GetLength(1) - 2 + 1;
                    maxSumCount = Math.Abs(inArray.GetLength(1) - convArray.GetLength(1)) + 1;
                    simpleSumCount = result.GetLength(1) - maxSumCount - 2;

                    tempResult = new double[result.GetLength(0)];
                    for (int i = 0; i < simpleSumCount / 2; i++)
                    {
                        int baka = i + 1;
                        int c = 0;
                        for (int j = 0; j <= baka; j++)
                        {
                            temp.Add(new VectorsListDouble() { Vect = aRListIn[i + c].Vect });
                            c = c + step;
                        }

                        for (int k = 0; k < temp.Count; k++)
                        {
                            tempResult = tempResult.SumVectors(temp[k].Vect);
                        }
                        aRListOut.Add(new VectorsListDouble() { Vect = tempResult });

                        temp = new List<VectorsListDouble>(); tempResult = new double[result.GetLength(0)];
                    }

                    for (int i = 0; i < maxSumCount; i++)
                    {
                        int maxstep = 0;
                        int maxFirstIndex = simpleSumCount / 2;
                        for (int j = 0; j < maxAmount; j++)
                        {
                            temp.Add(new VectorsListDouble() { Vect = aRListIn[i + maxFirstIndex + maxstep].Vect });
                            maxstep = maxstep + step;
                        }

                        for (int k = 0; k < temp.Count; k++)
                        {
                            tempResult = tempResult.SumVectors(temp[k].Vect);
                        }
                        aRListOut.Add(new VectorsListDouble() { Vect = tempResult });

                        temp = new List<VectorsListDouble>(); tempResult = new double[result.GetLength(0)];
                    }

                    for (int i = 0; i < simpleSumCount / 2; i++)
                    {
                        int baka = i + 1;
                        int c = 0;
                        int kap = aRListIn.Count - 1;
                        for (int j = 0; j <= baka; j++)
                        {
                            temp.Add(new VectorsListDouble() { Vect = aRListIn[kap - i - c].Vect });
                            c = c + step;
                        }

                        for (int k = 0; k < temp.Count; k++)
                        {
                            tempResult = tempResult.SumVectors(temp[k].Vect);
                        }

                        tempRes.Add(new VectorsListDouble() { Vect = tempResult });

                        temp = new List<VectorsListDouble>(); tempResult = new double[result.GetLength(0)];
                    }

                    for (int i = tempRes.Count - 1; i >= 0; i--)
                    {
                        aRListOut.Add(new VectorsListDouble() { Vect = tempRes[i].Vect });
                    }
                }
                aRListOut.Add(new VectorsListDouble() { Vect = last });
            }

            for (int i = 0; i < aRListOut.Count; i++)
            {
                var getCol = aRListOut[i].Vect;
                tempResults.AddRange(getCol);
            }

            var convResult = d.VecorToArrayColbyCol(result.GetLength(0), result.GetLength(1), tempResults.ToArray());
            result = convResult;

            if (convback == Convback.same)
            {
                int indexCols = 0;
                int indexRows = 0;

                if (sameCol % 2 != 0)
                { indexCols = (sameCol - 1) / 2; }
                else
                { indexCols = sameCol / 2; }

                if (sameRow % 2 != 0)
                { indexRows = (sameRow - 1) / 2; }
                else
                { indexRows = sameRow / 2; }

                tempResults = new List<double>();
                var RowsCut = d.ArrayToVectorColByCol(convResult).ToList();
                for (int i = 0; i < convResult.GetLength(1); i++)
                {
                    int index = convResult.GetLength(0) * i + indexRows;
                    var rowPal = RowsCut.GetRange(index, sameRowPart);
                    tempResults.AddRange(rowPal);
                }

                var sameRows = d.VecorToArrayColbyCol(sameRowPart, convResult.GetLength(1), tempResults.ToArray());

                tempResults = new List<double>();
                var ColsCut = sameRows.Cast<double>().ToList();
                for (int i = 0; i < sameRowPart; i++)
                {
                    int index = convResult.GetLength(1) * i + indexCols;
                    var colPal = ColsCut.GetRange(index, sameColPart);
                    tempResults.AddRange(colPal);
                }

                result = new double[sameRowPart, sameColPart];
                result = d.VecorToArrayRowByRow(sameRowPart, sameColPart, tempResults.ToArray());
            }

            result.DecimalCorrection();
            return result;
        }
    }

    public enum Convback
    {
        full,
        same
    }
}
