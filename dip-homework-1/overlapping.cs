using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dip_homework_1
{
    public partial class overlapping : Form
    {
        public overlapping()
        {
            InitializeComponent();
        }

        private void overlapping_Load(object sender, EventArgs e)
        {
            string img = ".../.../E_I-edge.bmp";

            //read image
            Bitmap bmp = new Bitmap(img);

            pictureBox4.Image = bmp;

            pictureBox1.Image= Extraction_overlapping.ConvolutionFilter(bmp, Extraction_overlapping.xSobel, Extraction_overlapping.ySobel, 1.0, 0, true);


        }

        private void scrollhaha(object sender, ScrollEventArgs e)
        {

            string img = ".../.../E_I-edge.bmp";

            //read image
            Bitmap bmp = new Bitmap(img);

            Bitmap bmp1 = new Bitmap(bmp.Width, bmp.Height);
            Bitmap bmp2 = new Bitmap(bmp.Width, bmp.Height);
            Bitmap bmp3 = new Bitmap(bmp.Width, bmp.Height);

            bmp1 = Extraction_overlapping.ConvolutionFilter(bmp, Extraction_overlapping.xSobel, Extraction_overlapping.ySobel, 1.0, 0, true);

            label5.Text = "Threshold Value:  " + (255 - Convert.ToInt32(e.NewValue));

            bmp2= Extraction_overlapping.binarization(bmp1, 255 - Convert.ToInt32(e.NewValue));

            pictureBox2.Image = bmp2;

         

            // overlay
           int x, y;

            // Loop through the images pixels to reset color.
            for (x = 0; x < bmp.Width; x++)
            {
                for (y = 0; y < bmp.Height; y++)
                {
                    Color pixelColor = bmp2.GetPixel(x, y);
                    Color newColor = Color.FromArgb(100,  0,pixelColor.G, 0);
                    bmp3.SetPixel(x, y, newColor); // Now greyscale
                }
            }

            // pictureBox3.Image = bmp3;
        //    Bitmap baseImage;
          //  Bitmap overlayImage;
          //  baseImage = (Bitmap)Image.FromFile(@"C:\temp\base.jpg");
          //  overlayImage = (Bitmap)Image.FromFile(@"C:\temp\overlay.png");

            var finalImage = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(finalImage);
            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.DrawImage(bmp, 0, 0);
            graphics.DrawImage(bmp3, 0, 0);

            //show in a winform picturebox
            pictureBox3.Image = finalImage;




        }
    }

    public class Extraction_overlapping
    {

        public static double[,] xSobel
        {
            get
            {
                return new double[,]
                {
                    { -1, 0, 1 },
                    { -2, 0, 2 },
                    { -1, 0, 1 }
                };
            }
        }

        //Sobel operator kernel for vertical pixel changes
        public static double[,] ySobel
        {
            get
            {
                return new double[,]
                {
                    {  1,  2,  1 },
                    {  0,  0,  0 },
                    { -1, -2, -1 }
                };
            }
        }

        public static Bitmap ConvolutionFilter(Bitmap sourceImage, double[,] xkernel, double[,] ykernel, double factor = 1, int bias = 0, bool grayscale = false)
        {

            //Image dimensions stored in variables for convenience
            int width = sourceImage.Width;
            int height = sourceImage.Height;

            //Lock source image bits into system memory
            BitmapData srcData = sourceImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            //Get the total number of bytes in your image - 32 bytes per pixel x image width x image height -> for 32bpp images
            int bytes = srcData.Stride * srcData.Height;

            //Create byte arrays to hold pixel information of your image
            byte[] pixelBuffer = new byte[bytes];
            byte[] resultBuffer = new byte[bytes];

            //Get the address of the first pixel data
            IntPtr srcScan0 = srcData.Scan0;

            //Copy image data to one of the byte arrays
            Marshal.Copy(srcScan0, pixelBuffer, 0, bytes);

            //Unlock bits from system memory -> we have all our needed info in the array
            sourceImage.UnlockBits(srcData);

            //Convert your image to grayscale if necessary
            if (grayscale == true)
            {
                float rgb = 0;
                for (int i = 0; i < pixelBuffer.Length; i += 4)
                {
                    rgb = pixelBuffer[i] * .21f;
                    rgb += pixelBuffer[i + 1] * .71f;
                    rgb += pixelBuffer[i + 2] * .071f;
                    pixelBuffer[i] = (byte)rgb;
                    pixelBuffer[i + 1] = pixelBuffer[i];
                    pixelBuffer[i + 2] = pixelBuffer[i];
                    pixelBuffer[i + 3] = 255;
                }
            }

            //Create variable for pixel data for each kernel
            double xr = 0.0;
            double xg = 0.0;
            double xb = 0.0;
            double yr = 0.0;
            double yg = 0.0;
            double yb = 0.0;
            double rt = 0.0;
            double gt = 0.0;
            double bt = 0.0;

            //This is how much your center pixel is offset from the border of your kernel
            //Sobel is 3x3, so center is 1 pixel from the kernel border
            int filterOffset = 1;
            int calcOffset = 0;
            int byteOffset = 0;

            //Start with the pixel that is offset 1 from top and 1 from the left side
            //this is so entire kernel is on your image
            for (int OffsetY = filterOffset; OffsetY < height - filterOffset; OffsetY++)
            {
                for (int OffsetX = filterOffset; OffsetX < width - filterOffset; OffsetX++)
                {
                    //reset rgb values to 0
                    xr = xg = xb = yr = yg = yb = 0;
                    rt = gt = bt = 0.0;

                    //position of the kernel center pixel
                    byteOffset = OffsetY * srcData.Stride + OffsetX * 4;

                    //kernel calculations
                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset + filterX * 4 + filterY * srcData.Stride;
                            xb += (double)(pixelBuffer[calcOffset]) * xkernel[filterY + filterOffset, filterX + filterOffset];
                            xg += (double)(pixelBuffer[calcOffset + 1]) * xkernel[filterY + filterOffset, filterX + filterOffset];
                            xr += (double)(pixelBuffer[calcOffset + 2]) * xkernel[filterY + filterOffset, filterX + filterOffset];
                            yb += (double)(pixelBuffer[calcOffset]) * ykernel[filterY + filterOffset, filterX + filterOffset];
                            yg += (double)(pixelBuffer[calcOffset + 1]) * ykernel[filterY + filterOffset, filterX + filterOffset];
                            yr += (double)(pixelBuffer[calcOffset + 2]) * ykernel[filterY + filterOffset, filterX + filterOffset];
                        }
                    }

                    //total rgb values for this pixel
                    bt = Math.Sqrt((xb * xb) + (yb * yb));
                    gt = Math.Sqrt((xg * xg) + (yg * yg));
                    rt = Math.Sqrt((xr * xr) + (yr * yr));

                    //set limits, bytes can hold values from 0 up to 255;
                    if (bt > 255) bt = 255;
                    else if (bt < 0) bt = 0;
                    if (gt > 255) gt = 255;
                    else if (gt < 0) gt = 0;
                    if (rt > 255) rt = 255;
                    else if (rt < 0) rt = 0;

                    //set new data in the other byte array for your image data
                    resultBuffer[byteOffset] = (byte)(bt);
                    resultBuffer[byteOffset + 1] = (byte)(gt);
                    resultBuffer[byteOffset + 2] = (byte)(rt);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            //Create new bitmap which will hold the processed data
            Bitmap resultImage = new Bitmap(width, height);

            //Lock bits into system memory
            BitmapData resultData = resultImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            //Copy from byte array that holds processed data to bitmap
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);

            //Unlock bits from system memory
            resultImage.UnlockBits(resultData);

            //Return processed image
            return resultImage;
        }


        class ImageExtract
        {

            public readonly static int COLOR_B = 0, COLOR_G = 1, COLOR_R = 2;
            public static byte[,] getimageArray(Bitmap bitmap)
            {
                int width = bitmap.Width, height = bitmap.Height;

                System.IntPtr srcScan;
                BitmapData srcBmData;

                InitPonitMethod(bitmap, width, height, out srcScan, out srcBmData);

                byte[,] result = new byte[3, width * height];
                int pos = 0;
                unsafe
                {
                    byte* srcP = (byte*)srcScan;
                    int srcOffset = srcBmData.Stride - width * 3;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, srcP += 3)
                        {
                            pos = x + y * width;
                            result[COLOR_B, pos] = *(srcP + COLOR_B);
                            result[COLOR_G, pos] = *(srcP + COLOR_G);
                            result[COLOR_R, pos] = *(srcP + COLOR_R);
                        }
                    }
                }

                bitmap.UnlockBits(srcBmData);
                return result;
            }

            public static byte[,,] getimageMartix(Bitmap bitmap)
            {
                int width = bitmap.Width, height = bitmap.Height;

                System.IntPtr srcScan;
                BitmapData srcBmData;

                InitPonitMethod(bitmap, width, height, out srcScan, out srcBmData);

                byte[,,] result = new byte[3, width, height];
                unsafe
                {
                    byte* srcP = (byte*)srcScan;
                    int srcOffset = srcBmData.Stride - width * 3;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, srcP += 3)
                        {
                            result[COLOR_B, x, y] = *(srcP + COLOR_B);
                            result[COLOR_G, x, y] = *(srcP + COLOR_G);
                            result[COLOR_R, x, y] = *(srcP + COLOR_R);
                        }
                    }
                }

                bitmap.UnlockBits(srcBmData);
                return result;
            }

            public static void writeImageByMartix(byte[,,] pix, Bitmap dstBitmap)
            {
                int width = dstBitmap.Width, height = dstBitmap.Height;
                System.IntPtr srcScan;
                BitmapData srcBmData;

                InitPonitMethod(dstBitmap, width, height, out srcScan, out srcBmData);

                unsafe
                {
                    byte* srcP = (byte*)srcScan;
                    int srcOffset = srcBmData.Stride - width * 3;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, srcP += 3)
                        {
                            *srcP = pix[COLOR_B, x, y];
                            *(srcP + 1) = pix[COLOR_G, x, y];
                            *(srcP + 2) = pix[COLOR_R, x, y];
                        }
                    }
                }

                dstBitmap.UnlockBits(srcBmData);
            }

            public static void writeImageByArray(byte[,] pix, Bitmap dstBitmap)
            {
                int width = dstBitmap.Width, height = dstBitmap.Height;
                System.IntPtr srcScan;
                BitmapData srcBmData;

                InitPonitMethod(dstBitmap, width, height, out srcScan, out srcBmData);
                int pos = 0;
                unsafe
                {
                    byte* srcP = (byte*)srcScan;
                    int srcOffset = srcBmData.Stride - width * 3;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, srcP += 3)
                        {
                            pos = x + y * width;
                            *(srcP + COLOR_B) = pix[COLOR_B, pos];
                            *(srcP + COLOR_G) = pix[COLOR_G, pos];
                            *(srcP + COLOR_R) = pix[COLOR_R, pos];
                        }
                    }
                }

                dstBitmap.UnlockBits(srcBmData);
            }

            public static Bitmap InitPonitMethod(Bitmap srcBitmap, int width, int height, out System.IntPtr srcScan, out System.IntPtr dstScan, out BitmapData srcBmData, out BitmapData dstBmData)
            {

                Rectangle rect = new Rectangle(0, 0, width, height);

                Bitmap dstBitmap = new Bitmap(srcBitmap);

                //將srcBitmap鎖定到系統內的記憶體的某個區塊中，並將這個結果交給BitmapData類別的srcBimap
                srcBmData = srcBitmap.LockBits(rect, ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);

                //將dstBitmap鎖定到系統內的記憶體的某個區塊中，並將這個結果交給BitmapData類別的dstBimap
                dstBmData = dstBitmap.LockBits(rect, ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);

                //位元圖中第一個像素數據的地址。它也可以看成是位圖中的第一個掃描行
                //目的是設兩個起始旗標srcPtr、dstPtr，為srcBmData、dstBmData的掃描行的開始位置
                srcScan = srcBmData.Scan0;
                dstScan = dstBmData.Scan0;

                return dstBitmap;
            }
            public static void InitPonitMethod(Bitmap srcBitmap, int width, int height, out System.IntPtr srcScan, out BitmapData srcBmData)
            {
                Rectangle rect = new Rectangle(0, 0, width, height);

                //將srcBitmap鎖定到系統內的記憶體的某個區塊中，並將這個結果交給BitmapData類別的srcBimap
                srcBmData = srcBitmap.LockBits(rect, ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);

                //位元圖中第一個像素數據的地址。它也可以看成是位圖中的第一個掃描行
                //目的是設兩個起始旗標srcPtr、dstPtr，為srcBmData、dstBmData的掃描行的開始位置
                srcScan = srcBmData.Scan0;
            }


            public static Bitmap extract(Bitmap bitmap, out byte[,] pix, out byte[,] resPix)
            {
                int width = bitmap.Width, height = bitmap.Height;
                Bitmap dstBitmap = new Bitmap(bitmap);
                pix = ImageExtract.getimageArray(bitmap);
                resPix = new byte[3, width * height];
                return dstBitmap;
            }
        }
        public static Bitmap binarization(Bitmap bitmap, int value)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            System.IntPtr srcScan, dstScan;
            BitmapData srcBmData, dstBmData;
            Bitmap dstBitmap = ImageExtract.InitPonitMethod(bitmap, width, height, out srcScan, out dstScan, out srcBmData, out dstBmData);

            unsafe //啟動不安全代碼
            {
                byte* srcP = (byte*)srcScan;
                byte* dstP = (byte*)dstScan;
                int srcOffset = srcBmData.Stride - width * 3;
                int dstOffset = dstBmData.Stride - width * 3;
                byte MAX = 255, MIN = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, srcP += 3, dstP += 3)
                    {
                        *dstP = (srcP[0] > value) ? MAX : MIN;//blue
                        *(dstP + 1) = (srcP[1] > value) ? MAX : MIN;//green
                        *(dstP + 2) = (srcP[2] > value) ? MAX : MIN; //red   
                    }
                    srcP += srcOffset;
                    dstP += dstOffset;
                }
            }

            bitmap.UnlockBits(srcBmData);
            dstBitmap.UnlockBits(dstBmData);

            return dstBitmap;
        }


    }
}
