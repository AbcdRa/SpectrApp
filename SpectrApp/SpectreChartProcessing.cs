using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SpectrApp
{
    class SpectreChartProcessing
    {
        private Chart chart;
        private Signal signal;
        private TableLayoutPanel tlpSpectreParams;
        private SignalChartProcessing signalChartProcessing;

        private double f0;
        private double f1;
        private double df;
        private double eps = 0.00001;

        public SpectreChartProcessing(Chart chart, TableLayoutPanel tlpSpectreParams, 
            SignalChartProcessing signalChartProcessing)
        {
            this.chart = chart;
            this.tlpSpectreParams = tlpSpectreParams;
            this.signalChartProcessing = signalChartProcessing;
        }

        public void UpdateSpectreParams()
        {
            f0 = decimal.ToDouble(((NumericUpDown)tlpSpectreParams.GetControlFromPosition(1,0)).Value);
            f1 = decimal.ToDouble(((NumericUpDown)tlpSpectreParams.GetControlFromPosition(1,1)).Value);
            df = 1;
        }

        public void ShowAmplitudeSpectre()
        {
            UpdateSpectreParams();
            chart.Series[0].Points.Clear();
            double[] xs = signalChartProcessing.GetCurrentXs();
            double[] ys = signalChartProcessing.GetCurrentYs();

            for (double n = f0; n < f1; n += df)
            {
                double[] Cn = GetCn(xs, ys, n);
                PrepareCn(ref Cn);
                double AbsCn = GetAbsCn(Cn);
                chart.Series[0].Points.AddXY(n, AbsCn);
            }
        }


        public void ShowPhaseSpectre()
        {
            UpdateSpectreParams();
            chart.Series[0].Points.Clear();
            double[] xs = signalChartProcessing.GetCurrentXs();
            double[] ys = signalChartProcessing.GetCurrentYs();

            for (double n = f0; n < f1; n += df)
            {
                double[] Cn = GetCn(xs, ys, n);
                PrepareCn(ref Cn);
                double ArgCn = GetArgCn(Cn);
                chart.Series[0].Points.AddXY(n, ArgCn);
            }
        }


        private double GetAbsCn(double[] cnComplex) {
            return Math.Sqrt(cnComplex[0] * cnComplex[0] + cnComplex[1] * cnComplex[1]);
        }


        private void PrepareCn(ref double[] cnComplex)
        {
            if (Math.Abs(cnComplex[0]) < eps ) cnComplex[0] = 0;
            if (Math.Abs(cnComplex[1]) < eps  ) cnComplex[1] = 0;
            
        }

        private double GetArgCn(double[] cnComplex)
        {

            return Math.Atan2(cnComplex[1], cnComplex[0]);
        }

        private double[] GetCn(double[] xs, double[] ys, double n)
        {
            int N = ys.Length;
            double re_sum = 0;
            double im_sum = 0;
            for(int i=0; i<N; i++)
            {
                re_sum += ys[i] * Math.Cos(2 * Math.PI * i * n / N);
                im_sum += ys[i] * Math.Sin(2 * Math.PI * i * n / N);
            }
            return new double[] { re_sum, -im_sum };
        }

    }
}
