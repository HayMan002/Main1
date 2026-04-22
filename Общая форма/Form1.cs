using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

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

            DrawGraph();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GlobalData.SelectedIndex = comboBox1.SelectedIndex;
            DrawGraph();
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
            Application.Exit();
        }

        private void DrawGraph()
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Titles.Clear();
            chart1.Legends.Clear(); // убираем легенду полностью

            // ── Область графика ───────────────────────────────────
            ChartArea chartArea = new ChartArea("main");

            // Ось X
            chartArea.AxisX.Title = "X";
            chartArea.AxisX.TitleFont = new Font("Arial", 8);
            chartArea.AxisX.LabelStyle.Font = new Font("Arial", 7);
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea.AxisX.LineColor = Color.Black;
            chartArea.AxisX.Crossing = 0;
            chartArea.AxisX.Minimum = -10;
            chartArea.AxisX.Maximum = 10;
            chartArea.AxisX.Interval = 2;

            // Ось Y
            chartArea.AxisY.Title = "Y";
            chartArea.AxisY.TitleFont = new Font("Arial", 8);
            chartArea.AxisY.LabelStyle.Font = new Font("Arial", 7);
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea.AxisY.LineColor = Color.Black;
            chartArea.AxisY.Crossing = 0;
            chartArea.AxisY.Minimum = -10;  // ограничиваем диапазон
            chartArea.AxisY.Maximum = 10;   // чтобы график не улетал
            chartArea.AxisY.Interval = 2;

            // Отступы внутри области графика
            chartArea.InnerPlotPosition = new ElementPosition(8, 5, 88, 90);
            chartArea.BackColor = Color.White;

            chart1.ChartAreas.Add(chartArea);

            // ── Серия графика функции ─────────────────────────────
            Series series = new Series("f(x)");
            series.ChartType = SeriesChartType.Line; // Line вместо Spline
            series.Color = Color.SteelBlue;
            series.BorderWidth = 2;
            series.ChartArea = "main";
            series.IsVisibleInLegend = false; // скрываем из легенды

            // Заполнение точек с мелким шагом
            double xMin = -10;
            double xMax = 10;
            double step = 0.02;
            double prevY = double.NaN;

            Function_x obj = new Function_x();

            for (double x = xMin; x <= xMax; x += step)
            {
                try
                {
                    double y = obj.FuncX(x);

                    // Если значение выходит за диапазон - разрываем линию
                    if (y < -10 || y > 10)
                    {
                        series.Points.AddXY(x, DBNull.Value); // разрыв
                        prevY = double.NaN;
                        continue;
                    }

                    // Если слишком резкий скачок - разрываем линию
                    if (!double.IsNaN(prevY) && Math.Abs(y - prevY) > 8)
                    {
                        series.Points.AddXY(x, DBNull.Value);
                    }

                    series.Points.AddXY(x, y);
                    prevY = y;
                }
                catch
                {
                    series.Points.AddXY(x, DBNull.Value);
                    prevY = double.NaN;
                }
            }

            chart1.Series.Add(series);

            // ── Ось Y=0 ───────────────────────────────────────────
            Series zeroLine = new Series("zero");
            zeroLine.ChartType = SeriesChartType.Line;
            zeroLine.Color = Color.Black;
            zeroLine.BorderWidth = 1;
            zeroLine.ChartArea = "main";
            zeroLine.IsVisibleInLegend = false;
            zeroLine.Points.AddXY(-10, 0);
            zeroLine.Points.AddXY(10, 0);
            chart1.Series.Add(zeroLine);

            // ── Заголовок (маленький, сверху) ────────────────────
            Title title = new Title();
            title.Text = obj.GetFunctionName();
            title.Font = new Font("Arial", 8, FontStyle.Bold);
            title.ForeColor = Color.DarkBlue;
            title.Docking = Docking.Top;
            chart1.Titles.Add(title);

            // Фон
            chart1.BackColor = Color.White;
            chart1.BorderlineColor = Color.LightGray;
            chart1.BorderlineDashStyle = ChartDashStyle.Solid;
            chart1.BorderlineWidth = 1;
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
            GlobalData.SelectedIndex = comboBox1.SelectedIndex;
            Form4 f4 = new Form4();
            f4.Show();
            this.Hide();
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
            GlobalData.SelectedIndex = comboBox1.SelectedIndex;
            DrawGraph();
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
