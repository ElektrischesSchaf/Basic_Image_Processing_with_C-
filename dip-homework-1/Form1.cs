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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();       
         
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            extraction ex = new extraction();
            ex.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            filter fff = new filter();
           fff.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            histogram hhh = new histogram();
            hhh.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            threshold ttt = new threshold();
            ttt.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            edgedetection eee = new edgedetection();
            eee.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            overlapping ooo = new overlapping();
            ooo.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            component ccc = new component();
            ccc.Show();
        }


    }
}
