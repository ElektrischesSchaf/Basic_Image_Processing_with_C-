using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dip_homework_1
{
  
    public partial class histogram : Form
    {

      //  int w, h;
        public histogram()
        {
            InitializeComponent();
        }

        private void histogram_Load(object sender, EventArgs e)
        {
            //image path
            string img = ".../.../D_dark1.bmp";

            //read image
            Bitmap bmp = new Bitmap(img);

            //load original image in picturebox1
            pictureBox1.Image = Image.FromFile(img);

            //draw histogram of original image start
            /*
              w = bmp.Width;
            h = bmp.Height;

            int[] histogram_r = new int[256];
            float max = 0;

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int redValue = bmp.GetPixel(i, j).R;
                    histogram_r[redValue]++;
                    if (max < histogram_r[redValue])
                        max = histogram_r[redValue];
                }
            }

            int histHeight = 232;
            Bitmap img1 = new Bitmap(256, histHeight + 10);
            using (Graphics g = Graphics.FromImage(img1))
            {
                for (int i = 0; i < histogram_r.Length; i++)
                {
                    float pct = histogram_r[i] / max;   // What percentage of the max is this value?
                    g.DrawLine(Pens.Black,
                        new Point(i, img1.Height - 5),
                        new Point(i, img1.Height - 5 - (int)(pct * histHeight))  // Use that percentage of the height
                        );
                }
            }
            */

            pictureBox2.Image = Extension_Histogram.drawhistogram(bmp);
            //draw histogram of originial image end
            pictureBox3.Image = Extension_Histogram.equalization(bmp);
            pictureBox4.Image = Extension_Histogram.drawhistogram(Extension_Histogram.equalization(bmp));

        }
    }
    public static class Extension_Histogram
    {
        public static Bitmap drawhistogram(this Bitmap origImage)
        {
          
            int[] histogram_r = new int[256];
            float max = 0;

            for (int i = 0; i < origImage.Width; i++)
            {
                for (int j = 0; j < origImage.Height; j++)
                {
                    int redValue = origImage.GetPixel(i, j).R;
                    histogram_r[redValue]++;
                    if (max < histogram_r[redValue])
                        max = histogram_r[redValue];
                }
            }

            int histHeight = 232;
            Bitmap img1 = new Bitmap(256, histHeight + 10);
            using (Graphics g = Graphics.FromImage(img1))
            {
                for (int i = 0; i < histogram_r.Length; i++)
                {
                    float pct = histogram_r[i] / max;   // What percentage of the max is this value?
                    g.DrawLine(Pens.Black,
                        new Point(i, img1.Height - 5),
                        new Point(i, img1.Height - 5 - (int)(pct * histHeight))  // Use that percentage of the height
                        );
                }
            }
            return img1;
        }

        public static double Clamp(double val, double min, double max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static Bitmap equalization(this Bitmap origImage)
        {

            //image path
          //  string img = ".../.../C_dark2.bmp";

            //read image
            //Bitmap origImage = new Bitmap(img);
         //   origImage = new Bitmap(img);
            double blackPointPercent = 0.01;
            double whitePointPercent = 0.03;

            BitmapData srcData = origImage.LockBits(new Rectangle(0, 0, origImage.Width, origImage.Height), ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);
            Bitmap destImage = new Bitmap(origImage.Width, origImage.Height);
            BitmapData destData = destImage.LockBits(new Rectangle(0, 0, destImage.Width, destImage.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int stride = srcData.Stride;
            IntPtr srcScan0 = srcData.Scan0;
            IntPtr destScan0 = destData.Scan0;
            var freq = new int[256];

            unsafe
            {
                byte* src = (byte*)srcScan0;
                for (int y = 0; y < origImage.Height; ++y)
                {
                    for (int x = 0; x < origImage.Width; ++x)
                    {
                        ++freq[src[y * stride + x * 4]];
                    }
                }

                int numPixels = origImage.Width * origImage.Height;
                int minI = 0;
                var blackPixels = numPixels * blackPointPercent;
                int accum = 0;

                while (minI < 255)
                {
                    accum += freq[minI];
                    if (accum > blackPixels) break;
                    ++minI;
                }

                int maxI = 255;
                var whitePixels = numPixels * whitePointPercent;
                accum = 0;

                while (maxI > 0)
                {
                    accum += freq[maxI];
                    if (accum > whitePixels) break;
                    --maxI;
                }
                double spread = 255d / (maxI - minI);
                byte* dst = (byte*)destScan0;
                for (int y = 0; y < origImage.Height; ++y)
                {
                    for (int x = 0; x < origImage.Width; ++x)
                    {
                        int i = y * stride + x * 4;

                        byte val = (byte)Clamp(Math.Round((src[i] - minI) * spread), 0, 255);
                        dst[i] = val;
                        dst[i + 1] = val;
                        dst[i + 2] = val;
                        dst[i + 3] = 255;
                    }
                }
                origImage.UnlockBits(srcData);
                destImage.UnlockBits(destData);
                //  pictureBox7.SizeMode = PictureBoxSizeMode.StretchImage;
                //  pictureBox7.Image = destImage;
                return destImage;
            }
        }

    }
}
