using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMDisLabSys.BLL.Formulas
{
    public class FormulaNH
    {
        public static FormulaNH Instance = new FormulaNH();
        /// <summary>
        /// 直线拟合
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        /// <param name="newY"></param>
        /// <returns></returns>
        public bool CalcLineFitting(double[] x, double[] y, ref double[] a, ref double[] newY)
        {
            double[] ax = new double[a.Length];
            bool sucess = LineFitting(x, y, ref ax);
            a[0] = ax[1];
            a[1] = ax[0];
            if (sucess)
            {
                newY = new double[x.Length];
                for (int i = 0; i < x.Length; i++)
                {
                    newY[i] = CalculateLineFit(x[i], a[0], a[1]);
                }
            }
            return sucess;

        }
        public bool LineFitting(double[] X, double[] Y, ref double[] A)
        {
            //X,Y --  X,Y两轴的坐标
            //A   --  结果参数

            int len = X.Length;
            if (len <= 1)
            {
                return false;
            }

            double tmp = 0;
            long i, j, k;
            double Z, D1, D2, C, P, G, Q;
            double[] B = new double[len];
            double[] T = new double[len];
            double[] S = new double[len];

            for (i = 0; i < 2; i++)
                A[i] = 0;

            Z = 0;
            B[0] = 1;
            D1 = len;
            P = 0;
            C = 0;
            for (i = 0; i < len; i++)
            {
                P = P + X[i] - Z;
                C = C + Y[i];
            }

            //加除零判断
            if (D1 >= 0.0 && D1 <= 0.0)
            {
                return false;
            }

            C = C / D1;
            P = P / D1;
            A[0] = C * B[0];

            if (2 > 1)
            {
                T[1] = 1;
                T[0] = -P;
                D2 = 0;
                C = 0;
                G = 0;
                for (i = 0; i < len; i++)
                {
                    Q = X[i] - Z - P;
                    D2 = D2 + Q * Q;
                    C = Y[i] * Q + C;
                    G = (X[i] - Z) * Q * Q + G;
                }

                //加除零判断
                if (D2 >= 0.0 && D2 <= 0.0)
                {
                    return false;
                }

                C = C / D2;
                P = G / D2;

                //加除零判断
                if (D1 >= 0.0 && D1 <= 0.0)
                {
                    return false;
                }

                Q = D2 / D1;
                D1 = D2;
                A[1] = C * T[1];
                A[0] = C * T[0] + A[0];

            }
            for (j = 2; j < 2; j++)
            {
                S[j] = T[j - 1];
                S[j - 1] = -P * T[j - 1] + T[j - 2];
                if (j >= 3)
                {
                    for (k = j - 2; k >= 1; k--)
                        S[k] = -P * T[k] + T[k - 1] - Q * B[k];
                }
                S[0] = -P * T[0] - Q * B[0];
                D2 = 0;
                C = 0;
                G = 0;
                for (i = 0; i < len; i++)
                {
                    Q = S[j];
                    for (k = j - 1; k >= 0; k--)
                        Q = Q * (X[i] - Z) + S[k];

                    D2 = D2 + Q * Q;
                    C = Y[i] * Q + C;
                    G = (X[i] - Z) * Q * Q + G;

                }

                //加除零判断
                if (D2 >= 0.0 && D2 <= 0.0)
                {
                    return false;
                }

                C = C / D2;
                P = G / D2;

                //加除零判断
                if (D1 >= 0.0 && D1 <= 0.0)
                {
                    return false;
                }

                Q = D2 / D1;
                D1 = D2;
                A[j] = C * S[j];

                T[j] = S[j];
                for (k = j - 1; k >= 0; k--)
                {
                    A[k] = C * S[k] + A[k];
                    B[k] = T[k];
                    T[k] = S[k];
                }
            }

            for (int nk = 0; nk < 2; nk++)
                tmp = A[nk];

            return true;
        }
        public double CalculateLineFit(double X, double A, double B)
        {
            return A * X + B;
        }
        /// <summary>
        /// 二次抛物线
        /// </summary>
        /// <param name="d"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public bool PolyFitting(int d, double[] X, double[] Y, ref double[] C)
        {
            int order = d + 1;
            int rows = X.Length;

            double[] Base = new double[order * rows];
            double[] alpha = new double[order * order];
            double[] alpha2 = new double[order * order];
            double[] beta = new double[order];

            try
            {
                // calc base
                for (int i = 0; i < order; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        int k = i + j * order;
                        Base[k] = i == 0 ? 1.0 : X[j] * Base[k - 1];
                    }
                }

                // calc alpha2
                for (int i = 0; i < order; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        int k2 = 0;
                        int k3 = 0;
                        double sum = 0.0;
                        for (int k = 0; k < rows; k++)
                        {
                            k2 = i + k * order;
                            k3 = j + k * order;
                            sum += Base[k2] * Base[k3];
                        }

                        k2 = i + j * order;

                        alpha2[k2] = sum;

                        if (i != j)
                        {
                            k2 = j + i * order;
                            alpha2[k2] = sum;
                        }
                    }
                }

                // calc beta
                for (int j = 0; j < order; j++)
                {
                    double sum = 0;
                    int k3 = 0;
                    for (int k = 0; k < rows; k++)
                    {
                        k3 = j + k * order;
                        sum += Y[k] * Base[k3];
                    }

                    beta[j] = sum;
                }

                // get alpha
                for (int j = 0; j < order * order; j++)
                    alpha[j] = alpha2[j];

                // solve for params
                bool bRes = PolySolve(alpha, beta, order);

                for (int j = 0; j < order; j++)
                    C[j] = beta[j];
                return bRes;
            }
            catch
            {
                for (int v = 0; v < C.Length; v++)
                {
                    C[v] = 0;
                }
                return false;
            }

        }
        /// <summary>
        /// 2次拟合求Y
        /// </summary>
        /// <param name="X"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public double CalculatePolyFitting2(double X, double A, double B, double C)
        {
            return A * X * X + B * X + C;
        }
        /// <summary>
        /// 3次拟合求Y
        /// </summary>
        /// <param name="X"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public double CalculatePolyFitting3(double X, double A, double B, double C, double D)
        {
            return A * X * X * X + B * X * X + C * X + D;
        }
        private bool PolySolve(double[] a, double[] b, int n)
        {
            for (int i = 0; i < n; i++)
            {
                // find pivot
                double mag = 0;
                int pivot = -1;

                for (int j = i; j < n; j++)
                {
                    double mag2 = Math.Abs(a[i + j * n]);
                    if (mag2 > mag)
                    {
                        mag = mag2;
                        pivot = j;
                    }
                }

                // no pivot: error
                if (pivot == -1 || mag == 0)
                    return false;

                // move pivot row into position
                if (pivot != i)
                {
                    double temp;
                    for (int j = i; j < n; j++)
                    {
                        temp = a[j + i * n];
                        a[j + i * n] = a[j + pivot * n];
                        a[j + pivot * n] = temp;
                    }

                    temp = b[i];
                    b[i] = b[pivot];
                    b[pivot] = temp;
                }

                // normalize pivot row
                mag = a[i + i * n];
                for (int j = i; j < n; j++)
                    a[j + i * n] /= mag;
                b[i] /= mag;

                // eliminate pivot row component from other rows
                for (int i2 = 0; i2 < n; i2++)
                {
                    if (i2 == i) continue;

                    double mag2 = a[i + i2 * n];

                    for (int j = i; j < n; j++)
                        a[j + i2 * n] -= mag2 * a[j + i * n];

                    b[i2] -= mag2 * b[i];
                }
            }

            return true;
        }
        /// <summary>
        /// 反比例拟合
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        /// <param name="newY"></param>
        /// <returns></returns>
        public bool CalcCurveFitting4(double[] x, double[] y, ref double[] a, ref double[] newY)//反比例拟合
        {
            bool sucess = CurveFitting4(x, y, ref a);
            if (sucess)
            {
                newY = new double[x.Length];
                for (int i = 0; i < x.Length; i++)
                {
                    newY[i] = CalculateCurveFitting4(x[i], a[0], a[1]);
                }
            }
            return sucess;

        }
        public bool CurveFitting4(double[] X, double[] Y, ref double[] A)
        {
            double[] newX = new double[X.Length];
            for (int i = 0; i < X.Length; i++)
            {
                if (X[i] > 0)//排除零值
                {
                    newX[i] = 1 / X[i];
                }
                else
                {
                    if (i > 0)
                    {
                        newX[i] = newX[i - 1];
                    }
                    else
                    {
                        newX[i] = Math.Log(0.0001);
                    }
                }
            }
            double[] newY = new double[Y.Length];
            for (int i = 0; i < Y.Length; i++)
            {
                newY[i] = Y[i];
            }
            return LineFitting(newX, newY, ref A);
        }
        public double CalculateCurveFitting4(double X, double A, double B)
        {
            if (X != 0)
            {
                return B / X + A;
            }
            else
            {
                return A;
            }
        }
        /// <summary>
        ///  
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public bool CalMaxMin(double[] X, double[] Y, ref double max, ref double min)
        {
            return true;
        }
        #region Sin函数拟合
        public bool CalcSinFitting(double[] x, double[] y, ref double[] a, ref double[] newY)
        {
            bool sucess = SinFitting(x, y, ref a);
            if (sucess)
            {
                newY = new double[x.Length];
                for (int i = 0; i < x.Length; i++)
                {
                    newY[i] = CalculateSinFitting(x[i], a[0], a[1], a[2], a[3]);
                }
            }
            return sucess;
        }
        /// <summary>
        /// Sin拟合Y = A * Sin( 2Pie / B * X + C ) + D
        /// </summary>
        /// <param name="X">X轴数列</param>
        /// <param name="?">Y轴数列</param>
        /// <param name="A">参数数列</param>
        /// <returns>拟合参数</returns>
        public bool SinFitting(double[] X, double[] Y, ref double[] A)
        {
            if (A.Length < 4)
            {
                return false;
            }
            if (!this.CalCurveFreq(X, Y, ref A[1]))
            {
                return false;
            }
            double maxY = this.Max(Y);
            double medianY = this.Median(Y);
            double minY = this.Min(Y);
            A[1] = 2 * A[1];
            A[0] = Math.Abs(maxY - medianY);
            A[3] = medianY;
            A[2] = 0;
            double m = medianY;
            bool begin = false;
            for (int i = 1; i < Y.Length; i++)
            {
                double n = Y[i];
                if (n < m)
                {
                    begin = true;
                }
                if (begin)
                {
                    if (n > m)
                    {
                        A[2] = -1 * X[i];
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 正弦拟合求Y
        /// </summary>
        /// <param name="X"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <returns></returns>
        public double CalculateSinFitting(double X, double A, double B, double C, double D)
        {
            return A * Math.Sin(2 * Math.PI / B * (X + C)) + D;
        }
        /// <summary>
        /// 求半周期
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="fFreqlValue"></param>
        /// <returns></returns>
        public bool CalCurveFreq(double[] X, double[] Y, ref double fFreqlValue)
        {
            if (X.Length < 2 || Y.Length < 2 || X.Length != Y.Length)
            {
                return false;
            }

            double[] Xsort = new double[X.Length];
            double[] Ysort = new double[X.Length];
            double[] Xtemp = new double[X.Length];
            int Xindex = -1;
            X.CopyTo(Xtemp, 0);
            for (int i = 0; i < X.Length; i++)//按照X排序
            {
                double xmin = this.Min(Xtemp);
                Xsort[++Xindex] = xmin;
                for (int j = 0; j < Y.Length; j++)
                {
                    if (X[j] == xmin)
                    {
                        Ysort[Xindex] = Y[j];
                    }
                }
                double[] Xtemp2 = new double[Xtemp.Length - 1];
                int tempno = -1;
                for (int j = 0; j < Xtemp.Length; j++)
                {
                    if (Xtemp[j] != xmin)
                    {
                        Xtemp2[++tempno] = Xtemp[j];
                    }
                }
                Xtemp = Xtemp2;
            }

            double[] Xmark = new double[X.Length];
            int no = -1;
            double Ysum = this.Average(Y);
            double prevY = Ysort[0];
            bool up = Ysort[0] < Ysum && Ysort[0] < Ysort[1];
            for (int i = 0; i < Ysort.Length; i++)
            {
                if (up && prevY > Ysum)//向上过平均点
                {
                    Xmark[++no] = Xsort[i];
                    up = false;
                }
                if (!up && prevY < Ysum)//向下过平均点
                {
                    //                    Xmark[++no] = Xsort[i];
                    up = true;
                }
                prevY = Ysort[i];
            }
            if (no < 1)
            {
                return false;
            }
            double Xsum = 0; ;
            for (int i = 0; i < no; i++)
            {
                Xsum += Math.Abs(Xmark[i + 1] - Xmark[i]);
            }
            //            fFreqlValue = Xsum / no;
            fFreqlValue = Xsum / no / 2;
            return true;
        }
        public double Min(double[] values)
        {
            if (values.Length <= 0)
            {
                return 0;
            }
            double min = values[0];
            for (int i = 0; i < values.Length; i++)
            {
                if (min > values[i])
                {
                    min = values[i];
                }
            }
            return min;
        }
        /// <summary>
        /// Max
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public double Max(double[] values)
        {
            if (values.Length <= 0)
            {
                return 0;
            }
            double max = values[0];
            for (int i = 0; i < values.Length; i++)
            {
                if (max < values[i])
                {
                    max = values[i];
                }
            }
            return max;
        }
        /// <summary>
        /// 求中值
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public double Median(double[] values)
        {
            if (values.Length < 2)
            {
                return 0;
            }
            double max = values[0];
            double min = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (max < values[i])
                {
                    max = values[i];
                }
                if (min > values[i])
                {
                    min = values[i];
                }
            }
            return (max + min) / 2;
        }
        /// <summary>
        /// 求平均值
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public double Average(double[] values)
        {
            if (values.Length <= 0)
            {
                return 0;
            }
            double sum = 0;
            for (int i = 0; i < values.Length; i++)
            {
                sum += values[i];
            }
            return sum / values.Length;
        }

        /// <summary>
        /// 梯形面积计算
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public double CalculateQtrap(double[] X, double[] Y,int Decimal)
        {
            double result = 0;
            int len = Math.Min(X.Length, Y.Length);
            for (int i = 1; i < len; i++)
            {
                result += 0.5 * (X[i] - X[i - 1]) * (Y[i] + Y[i - 1]);
            }
            return Math.Round(result,Decimal);
        }
        #endregion
    }

}
