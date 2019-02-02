using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using System.Threading;
using System.Drawing.Imaging;
using System.Collections;

namespace dip_homework_1
{
   
    public partial class component : Form
    {
       
        public component()
        {
            InitializeComponent();
     
        }
     
        private void component_Load(object sender, EventArgs e)
        {
            AllocConsole();

            string img = ".../.../F_connect.bmp";

            //read image
            Bitmap bmp = new Bitmap(img);

            int w = bmp.Width;
            int h = bmp.Height;          
            int[,] mat = new int[w, h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {

                    /*   //for testing the original code inside the loop
                    Console.ReadKey();
                    int[,] data = Extension_8.OutData();
                    Extension_8.CalConnections(data);
                    */

                    /*     //for show all the value of image by pixel
                    Color pixelColor = bmp.GetPixel(x, y);               
                     Console.WriteLine(pixelColor.B.ToString()); 
                     */                    
                    Color pixelColor = bmp.GetPixel(x, y);
                    if (pixelColor.B == 0)
                    {
                        mat[x, y] = 1;
                    }
                    
                }             
            }

             /*    //for testing the original code outside the loop
            Console.ReadKey();
            int[,] data = Extension_8.OutData();
            Extension_8.CalConnections(data);
            */



            Console.WriteLine("Original:");
            Console.WriteLine(" ");
            for (int x = 0; x < w; x++)
            {
                for(int y = 0; y < h; y++)
                {
                    Console.Write(mat[x, y].ToString());
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();


            //    Console.WriteLine("");

            /*
           Console.WriteLine("Processed");
           Console.WriteLine(" ");
           Extension_8.CalConnections(mat); 
           */


            /*
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Console.ReadKey();
              //      int[,] data = Extension_8.OutData();
                    Extension_8.CalConnections(mat);
                }
            }
            */

            //       Console.ReadKey();
            //int[,] data = Extension_8.OutData();
            Extension_8.CalConnections(mat);
            pictureBox2.Image = bmp;
            pictureBox1.Image = Extension_8.drawit(mat);
            label3.Text= Extension_8.partnumber(mat).ToString();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();


        public static class Extension_8
        {
            public static string img = ".../.../F_connect.bmp";

            public static int label = 1;
            public static Random rnd = new Random();
            //   static int width = 10;
            //     static int height = 10;
            public static void CalConnections(int[,] data)
            {
                Dictionary<int, List<Point>> dic_label_p = new Dictionary<int, List<Point>>();
           //     int label = 1;
                for (int y = 0; y < data.GetLength(0); y++)
                {
                    for (int x = 0; x < data.GetLength(1); x++)
                    {
                        //如果该数据不为0
                        if (data[y, x] != 0)
                        {
                            List<int> ContainsLabel = new List<int>();

                            #region 第一行
                            if (y == 0)//第一行只看左边
                            {
                                //第一行第一列，如果不为0，那么填入标记
                                if (x == 0)
                                {
                                    data[y, x] = label;
                                    label++;
                                }
                                //第一行，非第一列
                                else
                                {
                                    //如果该列的左侧数据不为0，那么该数据标记填充为左侧的标记
                                    if (data[y, x - 1] != 0)
                                    {
                                        data[y, x] = data[y, x - 1];
                                    }
                                    //否则，填充自增标记
                                    else
                                    {
                                        data[y, x] = label;
                                        label++;
                                    }
                                }
                            }
                            #endregion

                            #region 非第一行
                            else
                            {
                                if (x == 0)//最左边  --->不可能出现衔接情况
                                {
                                    /*分析上和右上*/
                                    //如果上方数据不为0，则该数据填充上方数据的标记
                                    if (data[y - 1, x] != 0)
                                    {
                                        data[y, x] = data[y - 1, x];
                                    }
                                    //上方数据为0，右上方数据不为0，则该数据填充右上方数据的标记
                                    else if (data[y - 1, x + 1] != 0)
                                    {
                                        data[y, x] = data[y - 1, x + 1];
                                    }
                                    //都为0，则填充自增标记
                                    else
                                    {
                                        data[y, x] = label;
                                        label++;
                                    }
                                }
                                else if (x == data.GetLength(1) - 1)//最右边   --->不可能出现衔接情况
                                {
                                    /*分析左上和上*/
                                    //如果左上数据不为0，则则该数据填充左上方数据的标记
                                    if (data[y - 1, x - 1] != 0)
                                    {
                                        data[y, x] = data[y - 1, x - 1];
                                    }
                                    //左上方数据为0，上方数据不为0，则该数据填充上方数据的标记
                                    else if (data[y - 1, x] != 0)
                                    {
                                        data[y, x] = data[y - 1, x];
                                    }
                                    //左上和上都为0
                                    else
                                    {
                                        //如果左侧数据不为0，则该数据填充左侧数据的标记
                                        if (data[y, x - 1] != 0)
                                        {
                                            data[y, x] = data[y, x - 1];
                                        }
                                        //否则填充自增标记
                                        else
                                        {
                                            data[y, x] = label;
                                            label++;
                                        }
                                    }
                                }
                                else//中间    --->可能出现衔接情况
                                {
                                    //重新实例化需要改变的标记
                                    ContainsLabel = new List<int>();
                                    /*分析左上、上和右上*/
                                    //上方数据不为0（中间数据），直接填充上方标记
                                    if (data[y - 1, x] != 0)
                                    {
                                        data[y, x] = data[y - 1, x];
                                    }
                                    //上方数据为0
                                    else
                                    {
                                        //左上和右上都不为0，填充左上标记
                                        if (data[y - 1, x - 1] != 0 && data[y - 1, x + 1] != 0)
                                        {
                                            data[y, x] = data[y - 1, x - 1];
                                            //如果右上和左上数据标记不同，则右上标记需要更改
                                            if (data[y - 1, x + 1] != data[y - 1, x - 1])
                                            {
                                                ContainsLabel.Add(data[y - 1, x + 1]);
                                            }
                                        }
                                        //左上为0，右上不为0
                                        else if (data[y - 1, x - 1] == 0 && data[y - 1, x + 1] != 0)
                                        {
                                            //左侧不为0，则填充左侧标记
                                            if (data[y, x - 1] != 0)
                                            {
                                                data[y, x] = data[y, x - 1];
                                                //如果左侧和右上标记不同，，则右上标记需要更改
                                                if (data[y - 1, x + 1] != data[y, x - 1])
                                                {
                                                    ContainsLabel.Add(data[y - 1, x + 1]);
                                                }
                                            }
                                            //左侧为0，则直接填充右上标记
                                            else
                                            {
                                                data[y, x] = data[y - 1, x + 1];
                                            }
                                        }
                                        //左上不为0，右上为0，填充左上标记
                                        else if (data[y - 1, x - 1] != 0 && data[y - 1, x + 1] == 0)
                                        {
                                            data[y, x] = data[y - 1, x - 1];
                                        }
                                        //左上和右上都为0
                                        else if (data[y - 1, x - 1] == 0 && data[y - 1, x + 1] == 0)
                                        {
                                            //如果左侧不为0，则填充左侧标记
                                            if (data[y, x - 1] != 0)
                                            {
                                                data[y, x] = data[y, x - 1];
                                            }
                                            //否则填充自增标记
                                            else
                                            {
                                                data[y, x] = label;
                                                label++;
                                            }

                                        }
                                    }

                                }
                            }
                            #endregion

                            //如果当前字典不存在该标记，那么创建该标记的Key
                            if (!dic_label_p.ContainsKey(data[y, x]))
                            {
                                dic_label_p.Add(data[y, x], new List<Point>());
                            }
                            //添加当前标记的点位
                            dic_label_p[data[y, x]].Add(new Point(x, y));

                            //备份需要更改标记的位置
                            List<Point> NeedChangedPoints = new List<Point>();
                            //如果有需要更改的标记
                            for (int i = 0; i < ContainsLabel.Count; i++)
                            {
                                for (int pcount = 0; pcount < dic_label_p[ContainsLabel[i]].Count;)
                                {
                                    Point p = dic_label_p[ContainsLabel[i]][pcount];
                                    NeedChangedPoints.Add(p);
                                    data[p.Y, p.X] = data[y, x];
                                    dic_label_p[ContainsLabel[i]].Remove(p);
                                    dic_label_p[data[y, x]].Add(p);
                                }
                                dic_label_p.Remove(ContainsLabel[i]);
                            }


                            /*   Display digit one by one
                            Thread.Sleep(500);                          
                            Console.Clear();
                            for (int r = 0; r < data.GetLength(0); r++)
                            {
                                for (int c = 0; c < data.GetLength(1); c++)
                                {
                                    
                                    if (r == y && c == x)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                    }
                                    else if (NeedChangedPoints.Contains(new Point(c, r)))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                    
                                    Console.Write(data[r, c].ToString() +"");
                                }
                                Console.WriteLine();
                            }
                            */
                        }
                    }
                }

                /* Display all at once
                for (int r = 0; r < data.GetLength(0); r++)
                {
                    for (int c = 0; c < data.GetLength(1); c++)
                    {
                        Console.Write(data[r, c].ToString() + "");
                    }
                    Console.WriteLine();
                }
                */

            }

            
            public  static int[,] OutData()
            {
                 int width = 10;
                 int height = 10;
                int[,] Data = new int[height, width];
                Random r = new Random();
                for (int y = 0; y < Data.GetLength(0); y++)
                {
                    for (int x = 0; x < Data.GetLength(1); x++)
                    {
                        int SeekNum = r.Next(0, 100);
                        if (SeekNum < 50)
                        {
                            Data[y, x] = 1;
                        }
                        else
                        {
                            Data[y, x] = 0;
                        }
                    }
                }
                return Data;
            }
            

            public static Bitmap drawit(int[,] data)
            {
                int count;

          //      string img = ".../.../test.bmp";

                //read image
                Bitmap myBitmap = new Bitmap(img);

                Bitmap imgtarget = myBitmap.Clone(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height), PixelFormat.Format24bppRgb);

                //    e.Graphics.DrawImage(myBitmap, 0, 0, myBitmap.Width, myBitmap.Height);

                // Set each pixel in myBitmap to black.
                for (count = 1; count <= label; count++)
                {
                    Color randomColor = Color.FromArgb(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256));

                    for (int Xcount = 0; Xcount < imgtarget.Width; Xcount++)
                    {
                        for (int Ycount = 0; Ycount < imgtarget.Height; Ycount++)
                        {
                            if (data[Xcount, Ycount] == count)
                            {                                
                                imgtarget.SetPixel(Xcount, Ycount, randomColor);
                                // imgtarget.SetPixel(Xcount, Ycount, Color.Yellow);
                            }
                        }
                     
                    }
                }
                return imgtarget;
            }

            ///
            public static int partnumber(int[,] data)
            {
                IList test = new List<int>();
                int count;
                int number = 0;
     //           string img = ".../.../test.bmp";

                //read image
                Bitmap myBitmap = new Bitmap(img);

                Bitmap imgtarget = myBitmap.Clone(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height), PixelFormat.Format24bppRgb);

                //    e.Graphics.DrawImage(myBitmap, 0, 0, myBitmap.Width, myBitmap.Height);

                for (count = 1; count <= label; count++)
                {
                 
                    Color randomColor = Color.FromArgb(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256));
                    for (int Xcount = 0; Xcount < imgtarget.Width; Xcount++)
                    {
                        for (int Ycount = 0; Ycount < imgtarget.Height; Ycount++)
                        {
                            if (data[Xcount, Ycount] == count)
                            { 
                                if (!test.Contains(count))
                                {
                                    test.Add(count);
                                }
                                imgtarget.SetPixel(Xcount, Ycount, randomColor);
                                // imgtarget.SetPixel(Xcount, Ycount, Color.Yellow);
                            }
                        }
                    }
                }
                number = test.Count;
                return number;
            }
            ///

        }
    }
}
