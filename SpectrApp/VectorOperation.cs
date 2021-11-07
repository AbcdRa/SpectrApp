using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrApp
{
    class VectorOperation

    {
        static public double[] applyF(double[] xs, Func<double, double> f)
        {
            double[] result = new double[xs.Length];
            for (int i = 0; i < xs.Length; i++) result[i] = f(xs[i]);
            return result;
        }

        static public void AddScalar(ref double[] xs, double scalar)
        {
            for(int i = 0; i< xs.Length; i++)
            {
                xs[i] += scalar;
            }
        }

        static public void MulScalar(ref double[] xs, double scalar)
        {
            for (int i = 0; i < xs.Length; i++)
            {
                xs[i] *= scalar;
            }
        }

        static public double[] MulVector(double[] xs, double[] ys)
        {
            double[] result = new double[xs.Length];
            for (int i = 0; i < xs.Length; i++)
            {
                result[i] = xs[i]*ys[i];
            }
            return result;
        }

        static public void PushElement(ref double[] xs, double el)
        {
            double[] ys = new double[xs.Length+1];
            xs.CopyTo(ys, 0);
            ys[xs.Length] = el;
            xs = ys;
        }

        static public double[] PushElement(double[] xs, double el)
        {
            double[] ys = new double[xs.Length + 1];
            xs.CopyTo(ys, 0);
            ys[xs.Length] = el;
            return ys;
        }

        static public Func<double[], double[], double[]> Vectorize(Func<double, double[], double> f)
        {
            Func<double[], double[], double[]> vectorFunc =
                (xs, ps) => {
                    
                    double[] ys = new double[xs.Length];
                    for (int i = 0; i < xs.Length; i++)
                    {
                        ys[i] = f(xs[i], ps);
                    }
                    return ys;
                };
            return vectorFunc;
        }
    }
}
