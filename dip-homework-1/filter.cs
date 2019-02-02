using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace dip_homework_1
{
   public partial class filter : Form
    {
      
        public filter()
        {
            InitializeComponent();
        }

        private void filter_Load(object sender, EventArgs e)
        {
            //image path
            string img = ".../.../B_noisy.bmp";

            //read image
            Bitmap bmp = new Bitmap(img);

            //load original image in picturebox1
            pictureBox1.Image = Image.FromFile(img);

            // Median
            pictureBox3.Image = Extension_Median.MedianFilter(bmp, 3);

            //End Median

            pictureBox2.Image = Extension_Mean.meanfilter(bmp);
        }

    }
    public static class Extension_Median
    {
        public static Bitmap MedianFilter(this Bitmap sourceBitmap, int matrixSize, int bias = 0, bool grayscale = false)
        {
            BitmapData sourceData =
                       sourceBitmap.LockBits(new Rectangle(0, 0,
                       sourceBitmap.Width, sourceBitmap.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride *
                                          sourceData.Height];

            byte[] resultBuffer = new byte[sourceData.Stride *
                                           sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0,
                                       pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);


            if (grayscale == true)
            {
                float rgb = 0;


                for (int k = 0; k < pixelBuffer.Length; k += 4)
                {
                    rgb = pixelBuffer[k] * 0.11f;
                    rgb += pixelBuffer[k + 1] * 0.59f;
                    rgb += pixelBuffer[k + 2] * 0.3f;


                    pixelBuffer[k] = (byte)rgb;
                    pixelBuffer[k + 1] = pixelBuffer[k];
                    pixelBuffer[k + 2] = pixelBuffer[k];
                    pixelBuffer[k + 3] = 255;
                }
            }


            int filterOffset = (matrixSize - 1) / 2;
            int calcOffset = 0;


            int byteOffset = 0;

            List<int> neighbourPixels = new List<int>();
            byte[] middlePixel;


            for (int offsetY = filterOffset; offsetY <
                sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    sourceBitmap.Width - filterOffset; offsetX++)
                {
                    byteOffset = offsetY *
                                 sourceData.Stride +
                                 offsetX * 4;


                    neighbourPixels.Clear();


                    for (int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++)
                        {


                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                (filterY * sourceData.Stride);


                            neighbourPixels.Add(BitConverter.ToInt32(
                                             pixelBuffer, calcOffset));
                        }
                    }


                    neighbourPixels.Sort();

                    middlePixel = BitConverter.GetBytes(
                                       neighbourPixels[filterOffset]);


                    resultBuffer[byteOffset] = middlePixel[0];
                    resultBuffer[byteOffset + 1] = middlePixel[1];
                    resultBuffer[byteOffset + 2] = middlePixel[2];
                    resultBuffer[byteOffset + 3] = middlePixel[3];
                }
            }


            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);


            BitmapData resultData =
                       resultBitmap.LockBits(new Rectangle(0, 0,
                       resultBitmap.Width, resultBitmap.Height),
                       ImageLockMode.WriteOnly,
                       PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0,
                                       resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);
            return resultBitmap;
        }

    }

    public static class Extension_Mean
    {
        public  static Bitmap meanfilter(Bitmap bmpInput)
        {
            Bitmap temp;
            Color c;
            float sum = 0;
            for (int i = 0; i < bmpInput.Width; i++)
            {
                for (int j = 0; j < bmpInput.Height; j++)
                {
                    c = bmpInput.GetPixel(i, j);
                    byte gray = (byte)(.333 * c.R + .333 * c.G + .333 * c.B);
                    bmpInput.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
            temp = bmpInput;
            for (int i = 0; i <= bmpInput.Width - 3; i++)
                for (int j = 0; j <= bmpInput.Height - 3; j++)
                {
                    for (int x = i; x <= i + 2; x++)
                        for (int y = j; y <= j + 2; y++)
                        {
                            c = bmpInput.GetPixel(x, y);
                            sum = sum + c.R;
                        }
                    int color = (int)Math.Round(sum / 9, 10);
                    temp.SetPixel(i + 1, j + 1, Color.FromArgb(color, color, color));
                    sum = 0;
                }
            return temp;
        }

    }
}
