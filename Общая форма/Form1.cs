using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Общая_форма
{
   

    public partial class Form1 : Form
    {
        public static class GlobalData
        {
            public static int SelectedIndex = -1;
        }

        public class Function_x
        {
            Double FuncX(double x0)
            {
                double f;
                switch(GlobalData.SelectedIndex)
                {
                    case 0:
                        f = Math.Pow(x0, 3) -3*Math.Pow(x0,2) -24*x0 - 3;
                        return f;
                    case 1:
                        f = Math.Pow(x0, 3) + 3 * Math.Pow(x0, 2) - 2;
                        return f;
                    case 2:
                        f = 2 * Math.Pow(x0, 3) + 9 * Math.Pow(x0, 2) - 10;
                        return f;
                    case 3:
                        f = Math.Pow(x0,3) + 3*Math.Pow(x0,2) - 3.5;
                        return f;
                    default:
                        return 0;
                }
            }
        }

        public Form1()
        {
            InitializeComponent();

            comboBox1.SelectedIndex = GlobalData.SelectedIndex;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GlobalData.SelectedIndex = comboBox1.SelectedIndex;
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
             
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form6 f2 = new Form6();
            f2.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 f2 = new Form3();
            f2.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
