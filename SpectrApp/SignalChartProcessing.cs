using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SpectrApp
{
    class SignalChartProcessing
    {
        private Chart chart;
        private Signal signal;
        private TableLayoutPanel tlbGraphParams;
        private TableLayoutPanel tlbSignalParams;
        private List<Control> tbParams;

        private double x0;
        private double x1;
        private double A;
        private double offX;
        private double offY;
        private int N;

        private double[] currentXs = null;
        private double[] currentYs = null;

        public double[] GetCurrentXs() { return currentXs; }
        public double[] GetCurrentYs() { return currentYs; }

        //Заменить на чекбокс
        private bool isShowError = true;

        public SignalChartProcessing(Chart chart, TableLayoutPanel tlbGraphParams, TableLayoutPanel tlbSignalParams )
        {
            this.chart = chart;
            this.tlbGraphParams = tlbGraphParams;
            this.tlbSignalParams = tlbSignalParams;
        }
     
        public void UpdateGraphParams()
        {
            x0 = decimal.ToDouble(((NumericUpDown) tlbGraphParams.Controls[5]).Value);
            x1 = decimal.ToDouble(((NumericUpDown)tlbGraphParams.Controls[6]).Value);
            N = decimal.ToInt32(((NumericUpDown)tlbGraphParams.Controls[7]).Value);
            A = decimal.ToDouble(((NumericUpDown)tlbGraphParams.Controls[8]).Value);
            offX = decimal.ToDouble(((NumericUpDown)tlbGraphParams.Controls[9]).Value);
            offY = decimal.ToDouble(((NumericUpDown)tlbGraphParams.Controls[10]).Value);
        }


        public void SetSignal(Signal signal)
        {
            tlbSignalParams.Enabled = true;
            this.signal = signal;
            int rowCount = tlbSignalParams.RowCount;
            int paramCount = signal.paramNames.Length;
            int i = 0;
            tbParams = new List<Control>();
            
            for(; i < paramCount; i++)
            {
                tlbSignalParams.GetControlFromPosition(0, i).Visible = true;
                tlbSignalParams.GetControlFromPosition(1, i).Enabled = true;
                tlbSignalParams.GetControlFromPosition(0, i).Text = signal.paramNames[i];
                tlbSignalParams.GetControlFromPosition(1, i).Text = $"{signal.paramValues[i]}";
                tbParams.Add(tlbSignalParams.GetControlFromPosition(1, i));
            }
            
            for(; i < rowCount; i++)
            {
                tlbSignalParams.GetControlFromPosition(0, i).Visible = false;
                tlbSignalParams.GetControlFromPosition(1, i).Enabled = false;
            }
            
            Func<double[], double[]> updateFunc = (currentParamValues) =>
            {
                double[] paramValues = new double[paramCount];
                for (int j = 0; j < tbParams.Count; j++)
                {
                    try
                    {
                        paramValues[j] = double.Parse(tbParams[j].Text);
                    } catch
                    {
                        if (isShowError) MessageBox.Show($"Не удалось преобразовать {tbParams[j].Text} в число");
                        paramValues[j] = currentParamValues[j];
                    }
                }
                return paramValues;
            };

            signal.updateFunc = updateFunc;
        }

        public void Show()
        {
            if (signal is null) return;

            UpdateGraphParams();
            double[] xs = GetXs();
            VectorOperation.AddScalar(ref xs, offX);
            xs = signal.transformXs(xs);
            List<double> list_xs = (new List<double>(xs));
            list_xs.Sort();
            xs = list_xs.ToArray();
            double[] ys = signal.GetYs(xs);
            VectorOperation.MulScalar(ref ys, A);
            VectorOperation.AddScalar(ref ys, offY);
           
            currentXs = xs;
            currentYs = ys;

            ShowCurrentPoints();
        }


        private void ShowCurrentPoints()
        {
            double[] xs = currentXs;
            double[] ys = currentYs;
            chart.Series[0].Points.Clear();
            for (int i = 0; i < xs.Length; i++)
            {
                chart.Series[0].Points.AddXY(xs[i], ys[i]);
            }
        }

        public void AddNoise(double noise)
        {

            double[] ys = currentYs;
            chart.Series[0].Points.Clear();
            Random random = new Random();
            
            for (int i = 0; i < ys.Length; i++)
            {
                ys[i] = ys[i]+noise*(random.NextDouble()-0.5);
            }

            currentYs = ys;
            ShowCurrentPoints();
        }

        private double[] GetXs()
        {
            double x = x0;
            double dx = (x1 - x0) / N;
            double[] xs = new double[N];
            for(int i=0; i < N; i++, x+=dx)
            {
                xs[i] = x;
            }
            return xs;
        }
        
    }
}
