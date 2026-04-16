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
    public partial class Form3 : Form
    {

        private void Form3_Load(object sender, EventArgs e)
        {

        }
        private void txtRoot3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
        
        // Выбранный номер функции (передаётся с главной формы)
        private int selectedFunction = 1;

        // Делегат для функции и её производных
        private Func<double, double> f;
        private Func<double, double> fPrime;
        private Func<double, double> fDoublePrime;

        private const double Epsilon = 0.000001;
        private const int MaxIterations = 100;

        public Form3(int functionIndex)
        {
            InitializeComponent();
            selectedFunction = functionIndex;
            SetupFunction();
            DrawGraph(); // ← вызов построения графика
        }

        /// <summary>
        /// Устанавливает функцию и отображает её на форме
        /// </summary>
        private void SetupFunction()
        {
            switch (selectedFunction)
            {
                case 1:
                    f = x => Math.Pow(x, 3) + 3 * Math.Pow(x, 2) - 2;
                    fPrime = x => 3 * Math.Pow(x, 2) + 6 * x;
                    fDoublePrime = x => 6 * x + 6;
                    lblFunctionName.Text = "y = x³ + 3x² - 2";
                    lblFormula.Text = "F(x) = x³ + 3x² - 2";
                    break;

                case 2:
                    f = x => Math.Pow(x, 3) - x - 1;
                    fPrime = x => 3 * Math.Pow(x, 2) - 1;
                    fDoublePrime = x => 6 * x;
                    lblFunctionName.Text = "y = x³ - x - 1";
                    lblFormula.Text = "F(x) = x³ - x - 1";
                    break;

                case 3:
                    f = x => Math.Pow(x, 3) - 2 * Math.Pow(x, 2) - 5;
                    fPrime = x => 3 * Math.Pow(x, 2) - 4 * x;
                    fDoublePrime = x => 6 * x - 4;
                    lblFunctionName.Text = "y = x³ - 2x² - 5";
                    lblFormula.Text = "F(x) = x³ - 2x² - 5";
                    break;

                default:
                    f = x => Math.Pow(x, 3) + 3 * Math.Pow(x, 2) - 2;
                    fPrime = x => 3 * Math.Pow(x, 2) + 6 * x;
                    fDoublePrime = x => 6 * x + 6;
                    lblFunctionName.Text = "y = x³ + 3x² - 2";
                    lblFormula.Text = "F(x) = x³ + 3x² - 2";
                    break;
            }
        }
        /// <summary>
        /// Построение графика функции
        /// </summary>
        private void DrawGraph()
        {
            // Очищаем график
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Legends.Clear();

            // ── Настройка области графика ─────────────────────────
            ChartArea chartArea = new ChartArea("main");

            // Настройка осей
            chartArea.AxisX.Title = "X";
            chartArea.AxisX.TitleFont = new Font("Arial", 10, FontStyle.Bold);
            chartArea.AxisX.LabelStyle.Font = new Font("Arial", 8);
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea.AxisX.LineColor = Color.Black;
            chartArea.AxisX.ArrowStyle = AxisArrowStyle.Triangle;

            chartArea.AxisY.Title = "Y";
            chartArea.AxisY.TitleFont = new Font("Arial", 10, FontStyle.Bold);
            chartArea.AxisY.LabelStyle.Font = new Font("Arial", 8);
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea.AxisY.LineColor = Color.Black;
            chartArea.AxisY.ArrowStyle = AxisArrowStyle.Triangle;

            // Пересечение осей в нуле
            chartArea.AxisX.Crossing = 0;
            chartArea.AxisY.Crossing = 0;

            // Диапазон отображения
            chartArea.AxisX.Minimum = -5;
            chartArea.AxisX.Maximum = 5;
            chartArea.AxisY.Minimum = -5;
            chartArea.AxisY.Maximum = 5;

            chartArea.BackColor = Color.White;
            chart1.ChartAreas.Add(chartArea);

            // ── Серия для графика функции ─────────────────────────
            Series series = new Series("f(x)");
            series.ChartType = SeriesChartType.Spline; // плавная линия
            series.Color = Color.SteelBlue;
            series.BorderWidth = 2;
            series.ChartArea = "main";
            series.IsVisibleInLegend = true;
            series.LegendText = lblFunctionName.Text;

            // ── Заполнение точек графика ──────────────────────────
            double xMin = -5;
            double xMax = 5;
            double step = 0.05; // шаг

            for (double x = xMin; x <= xMax; x += step)
            {
                try
                {
                    double y = f(x);

                    // Ограничиваем значения Y чтобы график не "улетал"
                    if (y >= -10 && y <= 10)
                    {
                        series.Points.AddXY(x, y);
                    }
                }
                catch
                {
                    // Пропускаем точки с ошибками
                }
            }

            chart1.Series.Add(series);

            // ── Линия Y = 0 (ось X) ───────────────────────────────
            Series zeroLine = new Series("y=0");
            zeroLine.ChartType = SeriesChartType.Line;
            zeroLine.Color = Color.Black;
            zeroLine.BorderWidth = 1;
            zeroLine.ChartArea = "main";
            zeroLine.IsVisibleInLegend = false;
            zeroLine.BorderDashStyle = ChartDashStyle.Solid;

            zeroLine.Points.AddXY(-5, 0);
            zeroLine.Points.AddXY(5, 0);
            chart1.Series.Add(zeroLine);

            // ── Настройка заголовка графика ───────────────────────
            chart1.Titles.Clear();
            Title title = new Title();
            title.Text = lblFunctionName.Text;
            title.Font = new Font("Arial", 11, FontStyle.Bold);
            title.ForeColor = Color.DarkBlue;
            chart1.Titles.Add(title);

            // ── Настройка легенды ─────────────────────────────────
            Legend legend = new Legend();
            legend.Font = new Font("Arial", 9);
            legend.BackColor = Color.Transparent;
            legend.Docking = Docking.Top;
            chart1.Legends.Add(legend);

            // Фон самого chart
            chart1.BackColor = Color.WhiteSmoke;
        }

        /// <summary>
        /// Отображение корней на графике после нахождения
        /// </summary>
        private void ShowRootsOnGraph(List<double> roots)
        {
            // Удаляем предыдущие точки корней если есть
            for (int i = chart1.Series.Count - 1; i >= 0; i--)
            {
                if (chart1.Series[i].Name.StartsWith("root"))
                    chart1.Series.Remove(chart1.Series[i]);
            }

            // Добавляем точки корней на график
            Color[] rootColors = { Color.Red, Color.Green, Color.Orange };

            for (int i = 0; i < roots.Count && i < 3; i++)
            {
                Series rootPoint = new Series($"root{i}");
                rootPoint.ChartType = SeriesChartType.Point;
                rootPoint.Color = rootColors[i];
                rootPoint.MarkerStyle = MarkerStyle.Circle;
                rootPoint.MarkerSize = 10;
                rootPoint.ChartArea = "main";
                rootPoint.IsVisibleInLegend = true;
                rootPoint.LegendText = $"x{i + 1} = {roots[i]:F4}";

                rootPoint.Points.AddXY(roots[i], 0);
                chart1.Series.Add(rootPoint);
            }
        }

        /// <summary>
        /// Метод хорд для нахождения корня на отрезке [a, b]
        /// </summary>
        private double ChordMethod(double a, double b)
        {
            double x0 = a;
            double x1 = b;
            double x2 = 0;
            int iteration = 0;

            if (f(a) * f(b) > 0)
                throw new Exception($"На отрезке [{a:F4}, {b:F4}] нет корня.");

            do
            {
                double fa = f(x0);
                double fb = f(x1);

                x2 = x1 - fb * (x1 - x0) / (fb - fa);

                double fx2 = f(x2);

                if (fa * fx2 < 0)
                    x1 = x2;
                else if (fb * fx2 < 0)
                    x0 = x2;
                else
                    break;

                iteration++;

            } while (Math.Abs(f(x2)) > Epsilon && iteration < MaxIterations);

            return x2;
        }

        private List<double> FindAllRoots(double a, double b)
        {
            List<double> roots = new List<double>();
            int n = 1000;
            double h = (b - a) / n;

            for (int i = 0; i < n; i++)
            {
                double x1 = a + i * h;
                double x2 = x1 + h;

                if (f(x1) * f(x2) <= 0)
                {
                    try
                    {
                        double root = ChordMethod(x1, x2);
                        bool isDuplicate = false;

                        foreach (double r in roots)
                        {
                            if (Math.Abs(r - root) < 0.001)
                            {
                                isDuplicate = true;
                                break;
                            }
                        }

                        if (!isDuplicate && Math.Abs(f(root)) < Epsilon * 100)
                            roots.Add(root);
                    }
                    catch { }
                }
            }

            roots.Sort();
            return roots;
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            txtRoot1.Clear();
            txtRoot2.Clear();
            txtRoot3.Clear();
            txtRoot1.BackColor = SystemColors.Window;
            txtRoot2.BackColor = SystemColors.Window;
            txtRoot3.BackColor = SystemColors.Window;

            if (!double.TryParse(txtStart.Text.Replace('.', ','), out double startPoint))
            {
                MessageBox.Show("Введите корректное значение начальной точки!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(txtEnd.Text.Replace('.', ','), out double endPoint))
            {
                MessageBox.Show("Введите корректное значение конечной точки!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (startPoint >= endPoint)
            {
                MessageBox.Show("Начальная точка должна быть меньше конечной!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                List<double> roots = FindAllRoots(startPoint, endPoint);

                if (roots.Count == 0)
                {
                    MessageBox.Show("Корни на указанном отрезке не найдены.",
                        "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                TextBox[] rootBoxes = { txtRoot1, txtRoot2, txtRoot3 };

                for (int i = 0; i < roots.Count && i < rootBoxes.Length; i++)
                {
                    rootBoxes[i].Text = roots[i].ToString("F6");
                    rootBoxes[i].BackColor = Color.LightGreen;
                }

                // Показываем корни на графике
                ShowRootsOnGraph(roots);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            txtRoot1.BackColor = SystemColors.Window;
            txtRoot2.BackColor = SystemColors.Window;
            txtRoot3.BackColor = SystemColors.Window;
            txtRoot1.Clear();
            txtRoot2.Clear();
            txtRoot3.Clear();
        }

        private void txtEnd_TextChanged(object sender, EventArgs e)
        {
            txtStart_TextChanged(sender, e);
        }

        private void txtRoot1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtRoot2_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblFormula_Click(object sender, EventArgs e)
        {

        }

        private void Chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
