using Accord.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SpectrApp
{
    class BadApple
    {
        public Chart chart1;
        public int W = 102;
        public int H = 76;
        public int offset = 36;

        public BadApple(Chart chart)
        {
            chart1 = chart;
        }

        delegate void ShowFrameDelegate(Bitmap bmap);
        delegate void MethodDelegate();



        public void MyMethod2()
        {
            if (chart1.InvokeRequired)
            {
                MethodDelegate method = new MethodDelegate(MyMethod2);
                chart1.BeginInvoke(method, new object[] { });
            }
            else
            {
                chart1.Series[0].ChartType = SeriesChartType.FastPoint;
                chart1.Series[0].MarkerBorderColor = Color.Black;
                chart1.ChartAreas[0].AxisX.Maximum = W;
                chart1.ChartAreas[0].AxisX.Minimum = 0;
                chart1.ChartAreas[0].AxisY.Minimum = -H;
                chart1.ChartAreas[0].AxisY.Maximum = 0;
                SoundPlayer simpleSound = new SoundPlayer(@"D:\Videos\BadApple.wav");
                simpleSound.Play();
                using (VideoFileReader reader = new VideoFileReader())
                {
                    reader.Open(@"D:\Videos\BadApple.mp4");

                    Bitmap frame = reader.ReadVideoFrame(offset);
                    Bitmap videoframe;
                    for (; frame != null;)
                    {
                        //chart1.DrawToBitmap(videoFrame, size);
                        videoframe = new Bitmap(frame, W, H);
                        ShowFrame(videoframe);
                        frame.Dispose();
                        //videoFrame.Save($@"D:\ Pictures\tumbs\t{i}.jpeg");
                        reader.ReadVideoFrame().Dispose();
                        frame = reader.ReadVideoFrame();
                    }

                }
            }
        }

        private void ShowFrame(Bitmap bmap)
        {
            if (chart1.InvokeRequired)
            {
                ShowFrameDelegate showFrame = new ShowFrameDelegate(ShowFrame);
                chart1.BeginInvoke(showFrame, new object[] { bmap });
            }
            else
            {
                chart1.Series[0].Points.Clear();
                chart1.Series[1].Points.Clear();
                for (int i = 0; i < W; i++)
                {
                    for (int j = 0; j < H; j++)
                    {
                        Color pix = bmap.GetPixel(i, j);
                        if (pix.G > 100)
                            chart1.Series[0].Points.AddXY(i, -j);
                        else
                        {
                           chart1.Series[1].Points.AddXY(i, -j);
                        }
                    }
                }
                bmap.Dispose();
                chart1.Update();

            }

        }



    }
}
