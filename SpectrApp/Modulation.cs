using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SpectrApp
{
    class Modulation
    {
        private TableLayoutPanel tblCarrier;
        private double m;
        private double w;
        private double A;

        public Modulation(TableLayoutPanel tblCarrier)
        {
            this.tblCarrier = tblCarrier;
        }

        public void UpdateCarrier()
        {
            m = decimal.ToDouble(((NumericUpDown)tblCarrier.GetControlFromPosition(1, 0)).Value);
            w = decimal.ToDouble(((NumericUpDown)tblCarrier.GetControlFromPosition(1, 1)).Value);
            A = decimal.ToDouble(((NumericUpDown)tblCarrier.GetControlFromPosition(1, 2)).Value);
        }

        public Signal GetAMSignal(Signal signal)
        {
            UpdateCarrier();
            Func<double[], double[], double[]> AM_func = (xs, ps) =>
            {
                
                double[] ys = signal.GetYs(xs);
                for(int i=0; i < ys.Length; i++)
                {
                    ys[i] = A*(m * ys[i] + 1) * Math.Cos(xs[i] * w);
                }
                return ys;
            };
            return new Signal(AM_func, "AM_"+signal.name, signal.paramNames, 
                signal.paramValues, signal.transformXs);
        }


        public Signal GetFMSignal(Signal signal)
        {
            UpdateCarrier();
            Func<double[], double[], double[]> FM_func = (xs, ps) =>
            {

                double[] ys = signal.GetYs(xs);
                double integral = 0;
                ys[0] = 0;
                for (int i = 1; i < ys.Length; i++)
                {
                    integral += (ys[i] + ys[i - 1]) / 2 * (xs[i] - xs[i - 1]);
                    ys[i] = A*Math.Cos(xs[i] * w + m*integral);
                    integral = integral % (2 * Math.PI / m );
                }
                return ys;
            };
            return new Signal(FM_func, "FM_" + signal.name, signal.paramNames,
                signal.paramValues, signal.transformXs);
        }
    }
}
