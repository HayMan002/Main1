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
using static Общая_форма.Form1;

namespace Общая_форма
{
    public partial class Form5 : Form
    {
        private Function_x funcObj = new Function_x();
        private const double Epsilon = 0.000001;
        private const int MaxIterations = 1000;

        public Form5()
        {
            InitializeComponent();
            SetupLabels();
            DrawGraph();
        }

        private void SetupLabels()
        {
            lblFunctionName.Text = "Выбранная функция: " + funcObj.GetFunctionName();
            lblFormula.Text = funcObj.GetFormula();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            // Инициализация при загрузке
        }

        /// <summary>
        /// Построение графика функции
        /// </summary>
        private void DrawGraph()
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Titles.Clear();
            chart1.Legends.Clear();

            // Область графика
            ChartArea chartArea = new ChartArea("main");
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

            chartArea.AxisY.Title = "Y";
            chartArea.AxisY.TitleFont = new Font("Arial", 8);
            chartArea.AxisY.LabelStyle.Font = new Font("Arial", 7);
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea.AxisY.LineColor = Color.Black;
            chartArea.AxisY.Crossing = 0;
            chartArea.AxisY.Minimum = -10;
            chartArea.AxisY.Maximum = 10;
            chartArea.AxisY.Interval = 2;

            chartArea.InnerPlotPosition = new ElementPosition(8, 5, 88, 90);
            chartArea.BackColor = Color.White;
            chart1.ChartAreas.Add(chartArea);

            // Серия графика функции
            Series series = new Series("f(x)");
            series.ChartType = SeriesChartType.Line;
            series.Color = Color.SteelBlue;
            series.BorderWidth = 2;
            series.ChartArea = "main";
            series.IsVisibleInLegend = false;

            double xMin = -10;
            double xMax = 10;
            double step = 0.02;
            double prevY = double.NaN;

            for (double x = xMin; x <= xMax; x += step)
            {
                try
                {
                    double y = funcObj.FuncX(x);

                    if (y < -10 || y > 10)
                    {
                        series.Points.AddXY(x, DBNull.Value);
                        prevY = double.NaN;
                        continue;
                    }

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

            // Ось Y=0
            Series zeroLine = new Series("zero");
            zeroLine.ChartType = SeriesChartType.Line;
            zeroLine.Color = Color.Black;
            zeroLine.BorderWidth = 1;
            zeroLine.ChartArea = "main";
            zeroLine.IsVisibleInLegend = false;
            zeroLine.Points.AddXY(-10, 0);
            zeroLine.Points.AddXY(10, 0);
            chart1.Series.Add(zeroLine);

            // Заголовок
            Title title = new Title();
            title.Text = funcObj.GetFunctionName();
            title.Font = new Font("Arial", 8, FontStyle.Bold);
            title.ForeColor = Color.DarkBlue;
            title.Docking = Docking.Top;
            chart1.Titles.Add(title);

            chart1.BackColor = Color.White;
            chart1.BorderlineColor = Color.LightGray;
            chart1.BorderlineDashStyle = ChartDashStyle.Solid;
            chart1.BorderlineWidth = 1;
        }

        /// <summary>
        /// Показать корень на графике
        /// </summary>
        private void ShowRootOnGraph(double root)
        {
            // Удаляем старые точки корня
            for (int i = chart1.Series.Count - 1; i >= 0; i--)
            {
                if (chart1.Series[i].Name.StartsWith("root"))
                    chart1.Series.Remove(chart1.Series[i]);
            }

            Series rootPoint = new Series("root0");
            rootPoint.ChartType = SeriesChartType.Point;
            rootPoint.Color = Color.Red;
            rootPoint.MarkerStyle = MarkerStyle.Circle;
            rootPoint.MarkerSize = 10;
            rootPoint.ChartArea = "main";
            rootPoint.IsVisibleInLegend = false;
            rootPoint.Points.AddXY(root, 0);
            chart1.Series.Add(rootPoint);
        }

        /// <summary>
        /// Поиск коэффициента λ для метода итераций
        /// </summary>
        private double FindLambda(double x0)
        {
            double fPrime = funcObj.FuncX1(x0);
            
            if (Math.Abs(fPrime) < 0.0001)
            {
                // Если производная близка к нулю, пробуем маленькие значения λ
                for (double lambda = 0.1; lambda <= 10.0; lambda += 0.1)
                {
                    if (CheckConvergence(lambda, x0))
                        return lambda;
                }
                return double.NaN;
            }

            // Оптимальное λ = -1/f'(x₀)
            double lambdaOpt = -1.0 / fPrime;
            
            if (CheckConvergence(lambdaOpt, x0))
                return lambdaOpt;

            // Перебираем значения вокруг оптимального
            for (double lambda = -10.0; lambda <= 10.0; lambda += 0.1)
            {
                if (CheckConvergence(lambda, x0))
                    return lambda;
            }

            return double.NaN;
        }

        /// <summary>
        /// Проверка условия сходимости |1 + λ·f'(x)| < 1
        /// </summary>
        private bool CheckConvergence(double lambda, double x)
        {
            double fPrime = funcObj.FuncX1(x);
            double phiPrime = Math.Abs(1 + lambda * fPrime);
            return phiPrime < 0.95; // Запас для надежности
        }

        /// <summary>
        /// Метод простой итерации: xₙ₊₁ = xₙ + λ·f(xₙ)
        /// </summary>
        private double? IterationMethod(double x0, out double lambda, out int iterations)
        {
            lambda = FindLambda(x0);
            iterations = 0;

            if (double.IsNaN(lambda))
            {
                return null;
            }

            double currentX = x0;
            double xNew;

            for (int i = 0; i < MaxIterations; i++)
            {
                double fx = funcObj.FuncX(currentX);
                xNew = currentX + lambda * fx;

                iterations = i + 1;

                // Проверка сходимости
                if (Math.Abs(xNew - currentX) < Epsilon)
                {
                    // Дополнительная проверка: f(x) должно быть близко к 0
                    if (Math.Abs(funcObj.FuncX(xNew)) < Epsilon * 100)
                    {
                        return xNew;
                    }
                }

                currentX = xNew;
            }

            return null;
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            // Сброс
            txtRoot.Clear();
            txtRoot.BackColor = SystemColors.Window;
            lblInfo.Text = "";

            if (!double.TryParse(txtStart.Text.Replace('.', ','), out double startPoint))
            {
                MessageBox.Show("Введите корректную начальную точку!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                double lambda;
                int iterations;
                double? root = IterationMethod(startPoint, out lambda, out iterations);

                if (root.HasValue)
                {
                    txtRoot.Text = root.Value.ToString("F6");
                    txtRoot.BackColor = Color.LightGreen;
                    lblInfo.Text = $"λ = {lambda:F6}, Итераций: {iterations}";
                    ShowRootOnGraph(root.Value);
                }
                else
                {
                    txtRoot.Text = "корни по данной начальной точки х не найдены";
                    txtRoot.BackColor = Color.LightCoral;
                    lblInfo.Text = $"λ = {lambda:F6}, Метод расходится";
                }
            }
            catch (Exception ex)
            {
                txtRoot.Text = "корни по данной начальной точки х не найдены";
                txtRoot.BackColor = Color.LightCoral;
                lblInfo.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            foreach (Form frm in Application.OpenForms)
            {
                if (frm is Form1)
                {
                    frm.Show();
                    break;
                }
            }
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtStart_TextChanged(object sender, EventArgs e)
        {
            // Обработка изменения текста
        }

        private void txtRoot_TextChanged(object sender, EventArgs e)
        {
            // Обработка изменения текста
        }

        private void lblFunctionName_Click(object sender, EventArgs e)
        {
            // Обработка клика
        }

        private void lblFormula_Click(object sender, EventArgs e)
        {
            // Обработка клика
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            // Обработка клика
        }

        private void lblInfo_Click(object sender, EventArgs e)
        {
            // Обработка клика
        }
    }
}
