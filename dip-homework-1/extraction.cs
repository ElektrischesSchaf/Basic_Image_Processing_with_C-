using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dip_homework_1
{
    public partial class extraction : Form
    {
        public extraction()
        {
            InitializeComponent();
        }

        private void extraction_Load(object sender, EventArgs e)
        {
            //image path
            string img = ".../.../A_RGB.bmp";

            //read image
            Bitmap bmp = new Bitmap(img);

            //load original image in picturebox1
            pictureBox1.Image = Image.FromFile(img);

            //get image dimension
            int width = bmp.Width;
            int height = bmp.Height;

            //3 bitmap for red green blue image
            Bitmap rbmp = new Bitmap(bmp);
            Bitmap gbmp = new Bitmap(bmp);
            Bitmap bbmp = new Bitmap(bmp);

            Bitmap abmp = new Bitmap(bmp);

            //red green blue image
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //get pixel value
                    Color p = bmp.GetPixel(x, y);

                    //extract ARGB value from p
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;
                    //set red image pixel
                    rbmp.SetPixel(x, y, Color.FromArgb(a, r, 0, 0));
                    //set green image pixel
                    gbmp.SetPixel(x, y, Color.FromArgb(a, 0, g, 0));
                    //set blue image pixel
                    bbmp.SetPixel(x, y, Color.FromArgb(a, 0, 0, b));

                    abmp.SetPixel(x, y, Color.FromArgb(a, 0, 0,0));
                }
            }
            //load red image in picturebox2
            pictureBox2.Image = rbmp;
            //load green image in picturebox3
            pictureBox3.Image = gbmp;
            //load blue image in picturebox4
            pictureBox4.Image = bbmp;


            Bitmap c = new Bitmap(img);
            Bitmap d;
            int x1, y1;

            // Loop through the images pixels to reset color.
            for (x1 = 0; x1 < c.Width; x1++)
            {
                for (y1 = 0; y1 < c.Height; y1++)
                {            
                    Color oc = c.GetPixel(x1,y1);
                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                    Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                    c.SetPixel(x1, y1, nc);
                }
            }
            d = c;   // d is grayscale version of c  

            pictureBox5.Image = d;
            //write (save) red image
            //        rbmp.Save("D:\\Image\\Red.png");

            //write(save) green image
            //       gbmp.Save("D:\\Image\\Green.png");

            //write (save) blue image
            //        bbmp.Save("D:\\Image\\Blue.png");      
            
              


        }
    }
}
