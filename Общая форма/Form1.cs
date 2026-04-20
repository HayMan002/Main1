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
            this.Hide();
             Form2 metodpopolam = new Form2();
            metodpopolam.Show();
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
            // Сохраняем выбранный индекс функции в GlobalData
            GlobalData.SelectedIndex = comboBox1.SelectedIndex;
            Form3 f3 = new Form3();
            f3.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            GlobalData.SelectedIndex = comboBox1.SelectedIndex;
            Form5 f5 = new Form5();
            f5.Show();
            this.Hide();
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
        public class Function_x
        {
            // Сама функция f(x)
            public double FuncX(double x0)
            {
                double f;
                switch (GlobalData.SelectedIndex)
                {
                    case 0:
                        f = Math.Pow(x0, 3) - 3 * Math.Pow(x0, 2) - 24 * x0 - 3;
                        return f;
                    case 1:
                        f = Math.Pow(x0, 3) + 3 * Math.Pow(x0, 2) - 2;
                        return f;
                    case 2:
                        f = 2 * Math.Pow(x0, 3) + 9 * Math.Pow(x0, 2) - 10;
                        return f;
                    case 3:
                        f = Math.Pow(x0, 3) + 3 * Math.Pow(x0, 2) - 3.5;
                        return f;
                    default:
                        return 0;
                }
            }

            // Первая производная f'(x)
            public double FuncX1(double x0)
            {
                switch (GlobalData.SelectedIndex)
                {
                    case 0:
                        // f(x) = x³ - 3x² - 24x - 3 → f'(x) = 3x² - 6x - 24
                        return 3 * Math.Pow(x0, 2) - 6 * x0 - 24;
                    case 1:
                        // f(x) = x³ + 3x² - 2 → f'(x) = 3x² + 6x
                        return 3 * Math.Pow(x0, 2) + 6 * x0;
                    case 2:
                        // f(x) = 2x³ + 9x² - 10 → f'(x) = 6x² + 18x
                        return 6 * Math.Pow(x0, 2) + 18 * x0;
                    case 3:
                        // f(x) = x³ + 3x² - 3.5 → f'(x) = 3x² + 6x
                        return 3 * Math.Pow(x0, 2) + 6 * x0;
                    default:
                        return 0;
                }
            }

            // Вторая производная f''(x)
            public double FuncX2(double x0)
            {
                switch (GlobalData.SelectedIndex)
                {
                    case 0:
                        // f'(x) = 3x² - 6x - 24 → f''(x) = 6x - 6
                        return 6 * x0 - 6;
                    case 1:
                        // f'(x) = 3x² + 6x → f''(x) = 6x + 6
                        return 6 * x0 + 6;
                    case 2:
                        // f'(x) = 6x² + 18x → f''(x) = 12x + 18
                        return 12 * x0 + 18;
                    case 3:
                        // f'(x) = 3x² + 6x → f''(x) = 6x + 6
                        return 6 * x0 + 6;
                    default:
                        return 0;
                }
            }

            // Получить название функции для отображения
            public string GetFunctionName()
            {
                switch (GlobalData.SelectedIndex)
                {
                    case 0: return "y = x³ - 3x² - 24x - 3";
                    case 1: return "y = x³ + 3x² - 2";
                    case 2: return "y = 2x³ + 9x² - 10";
                    case 3: return "y = x³ + 3x² - 3.5";
                    default: return "неизвестная функция";
                }
            }

            // Получить формулу для отображения
            public string GetFormula()
            {
                switch (GlobalData.SelectedIndex)
                {
                    case 0: return "F(x) = x³ - 3x² - 24x - 3";
                    case 1: return "F(x) = x³ + 3x² - 2";
                    case 2: return "F(x) = 2x³ + 9x² - 10";
                    case 3: return "F(x) = x³ + 3x² - 3.5";
                    default: return "F(x) = ?";
                }
            }
        }
    }
}
