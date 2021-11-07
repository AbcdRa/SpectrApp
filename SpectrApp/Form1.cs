using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpectrApp
{
    public partial class Form1 : Form
    {
        private SignalChartProcessing signalChartProcessing;
        private Modulation modulation;
        private SpectreChartProcessing spectreChartProcessing;
        public Form1()
        {
            InitializeComponent();
            InitListBox();
            this.modulation = new Modulation(tlpCarrier);
            this.signalChartProcessing = new SignalChartProcessing(cSignal, 
                tlpGraphParams, tlpSignalParams);
            this.spectreChartProcessing = new SpectreChartProcessing(cSpectre, 
                tlpSpectreParams, signalChartProcessing);
        }

        private void InitListBox()
        {
            List<Signal> signals = Signal.GetDefaultSignals();
            foreach (var signal in signals) lbSignals.Items.Add(signal);
        }

        private void lbSignals_SelectedIndexChanged(object sender, EventArgs e)
        {
            signalChartProcessing.SetSignal((Signal)lbSignals.SelectedItem);
            signalChartProcessing.Show();
        }

        private void cSignal_Click(object sender, EventArgs e)
        {
            signalChartProcessing.Show();
        }

        private void tlpGraphParams_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bAM_Click(object sender, EventArgs e)
        {
     
            Signal s = (Signal)lbSignals.SelectedItem;
            signalChartProcessing.SetSignal(modulation.GetAMSignal(s));
            signalChartProcessing.Show();

        }

        private void bFM_Click(object sender, EventArgs e)
        {
      
            Signal s = (Signal)lbSignals.SelectedItem;
            signalChartProcessing.SetSignal(modulation.GetFMSignal(s));
            signalChartProcessing.Show();
        }

        private void bAS_Click(object sender, EventArgs e)
        {
            spectreChartProcessing.ShowAmplitudeSpectre();
        }

        private void bFS_Click(object sender, EventArgs e)
        {
            spectreChartProcessing.ShowPhaseSpectre();
        }

        private int countClick = 0;
        private void lACarrier_Click(object sender, EventArgs e)
        {
            
            countClick += 1;
            if (countClick == 5)
            {
                int N = decimal.ToInt32(nudN.Value);
                int W = N * 4;
                int H = N * 3;
                int offset = decimal.ToInt32(nudX1.Value);
                var ch = new BadApple(cSignal);
                ch.W = W;
                ch.H = H;
                ch.offset = offset;
                backgroundWorker1.DoWork += (s, er) => {new Thread(new ThreadStart(ch.MyMethod2)).Start(); };
                backgroundWorker1.RunWorkerAsync();
                countClick = 0;
            }
        }


        private void label7_Click(object sender, EventArgs e)
        {
            signalChartProcessing.AddNoise(decimal.ToDouble(nudNoise.Value));
        }
    }
}
