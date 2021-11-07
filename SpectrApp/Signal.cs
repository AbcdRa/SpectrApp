using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrApp
{
    class Signal
    {

        public Func<double[], double[], double[]> f { get; }
        public string name { get; }
        public string[] paramNames { get; }
        public double[] paramValues { set; get; }
        public Func<double[], double[]> updateFunc { get; set; }

        public Func<double[], double[]> transformXs;


    public Signal(Func<double, double[], double> rawf, string name, string[] paramNames,double[] paramValues, Func<double[], double[]> transformXs = null)
        : this(VectorOperation.Vectorize(rawf), name, paramNames, paramValues, transformXs) { }
        

    public Signal(Func<double[], double[], double[]> f, string name, string[] paramNames,
            double[] paramValues, Func<double[], double[]> transformXs = null)
        {
            this.f = f;
            this.name = name;
            this.paramNames = paramNames;
            this.paramValues = paramValues;
            this.updateFunc = (values) => values;
            if (transformXs is null) transformXs = xs => xs;
            this.transformXs = transformXs;
        }


        public double[] GetYs(double[] xs)
        {
            this.paramValues = updateFunc(this.paramValues);
            return f(xs, paramValues);
        }



        public static List<Signal> GetDefaultSignals()
        {
            List<Signal> defaultSignals = new List<Signal>();

            string name;
            string[] paramNames;
            double[] paramValues;

            Func<double, double[], double> func_x = (x, ps) => x;
            name = "x";
            paramNames = new string[] {  };
            paramValues = new double[] {  };
            defaultSignals.Add(new Signal(func_x, name, paramNames, paramValues));

            Func<double, double[], double> func_sin = (x, ps) => Math.Sin(2*Math.PI*ps[0] * x);
            name = "sin(fx)";
            paramNames = new string[] { "f" };
            paramValues = new double[] { 1 };
            defaultSignals.Add(new Signal(func_sin, name, paramNames, paramValues));

            Func<double, double[], double> func_cos = (x, ps) => Math.Cos(2 * Math.PI * ps[0] * x);
            name = "cos(fx)";
            paramNames = new string[] { "f" };
            paramValues = new double[] { 1 };
            defaultSignals.Add(new Signal(func_cos, name, paramNames, paramValues));

            Func<double, double[], double> func_sigma = (x, ps) => x >= 0 ? 1 : 0;
            name = "sigma(x)";
            paramNames = new string[] { };
            paramValues = new double[] { };
            defaultSignals.Add(new Signal(func_sigma, name, paramNames, paramValues));

            Func<double, double[], double> func_delta = (x, ps) => x == 0 ? 1 : 0;
            Func<double[], double[]> transform_delta = (xs) => VectorOperation.PushElement(xs, 0);
            name = "delta(x)";
            paramNames = new string[] { };
            paramValues = new double[] { };
            defaultSignals.Add(new Signal(func_delta, name, paramNames, paramValues, transform_delta));

            Random random = new Random();
            Func<double, double[], double> func_random = (x, ps) => random.NextDouble();
            name = "random(x)";
            paramNames = new string[] { };
            paramValues = new double[] { };
            defaultSignals.Add(new Signal(func_random, name, paramNames, paramValues));

            Func<double, double[], double> func_ssp = (x, ps) => (x > 0) ? ( (x % ps[0] > ps[1]) ? 0 : 1) : ( (Math.Abs(x % ps[0]) > ps[0] - ps[1]) ? 1 : 0);
            name = "ssp(x, T, tau)";
            paramNames = new string[] { "T", "tau" };
            paramValues = new double[] { 3, 1 };
            defaultSignals.Add(new Signal(func_ssp, name, paramNames, paramValues));

            Func<double, double[], double> func_rp = 
                (x, ps) => (x > ps[0]) && (x < ps[1] * 2 * Math.PI + ps[0]) ? 
                Math.Sin(2 * Math.PI * x - ps[0]) : 0;
            name = "rp(x, f, startX, N)";
            paramNames = new string[] { "f", "startX", "NT" };
            paramValues = new double[] { 1, 4, 2 };
            defaultSignals.Add(new Signal(func_rp, name, paramNames, paramValues));

            Func<double, double[], double> func_sinc = (x, ps) => x != 0 ?
                Math.Sin(2 * Math.PI * ps[0] * x)/ps[0]/x : 2 * Math.PI * ps[0];
            name = "sinc(2pi*fx)";
            paramNames = new string[] { "f" };
            paramValues = new double[] { 1 };
            defaultSignals.Add(new Signal(func_sinc, name, paramNames, paramValues));

            Func<double, double[], double> func_ebxsin = 
                (x, ps) => Math.Exp(-x*ps[1])*Math.Sin(2 * Math.PI * ps[0] * x) / ps[0] ;
            name = "e^(-beta*x)*sin(fx)";
            paramNames = new string[] { "f", "beta"};
            paramValues = new double[] { 1, 0.12 };
            defaultSignals.Add(new Signal(func_ebxsin, name, paramNames, paramValues));
            Func<double[], double[], double[]> func_randdigit =
                (xs, ps) =>
                {
                    double[] ys = new double[xs.Length];
                    double x1 = xs[xs.Length - 1];
                    int N = (int)((x1 - xs[0]) / ps[0])+1;
                    bool[] rs = new bool[N];
                    
                    for(int i=0; i < N; i++) rs[i] = random.NextDouble() > 0.5;

                    for(int i=0; i < xs.Length; i++) {
                        ys[i] = rs[(int)((xs[i] - xs[0]) / ps[0])] ? 1 : 0;
                    }
                    return ys;
                };
            name = "rand_digit(T)";
            paramNames = new string[] { "T" };
            paramValues = new double[] { 1 };
            defaultSignals.Add(new Signal(func_randdigit , name, paramNames, paramValues));
            return defaultSignals;
        }


        public override string ToString()
        {
            return name;
        }
    }
}
