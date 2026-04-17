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
    public partial class Form3 : Form
    {

        private void Form3_Load(object sender, EventArgs e)
        {

        }
        private void txtRoot3_TextChanged(object sender, EventArgs e)
        {

        }
        private void txtStart_TextChanged(object sender, EventArgs e)
        {

        }
        private void txtEnd_TextChanged(object sender, EventArgs e)
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
        // Объект класса с функциям
        private Function_x funcObj = new Function_x();

        private const double Epsilon = 0.000001;
        private const int MaxIterations = 100;

        public Form3()
        {
            InitializeComponent();
            SetupLabels();  // установка подписей
            DrawGraph();    // построение графика
        }

        /// <summary>
        /// Устанавливает названия функции на форме
        /// </summary>
        private void SetupLabels()
        {
            lblFunctionName.Text = funcObj.GetFunctionName();
        }

        /// <summary>
        /// Метод Хорд для нахождения корня на отрезке [a, b]
        /// </summary>
        private double ChordMethod(double a, double b)
        {
            double x0 = a;
            double x1 = b;
            double x2 = 0;
            int iteration = 0;

            if (funcObj.FuncX(a) * funcObj.FuncX(b) > 0)
                throw new Exception($"На отрезке [{a:F4}, {b:F4}] нет корня.");

            do
            {
                double fa = funcObj.FuncX(x0);
                double fb = funcObj.FuncX(x1);

                // Формула метода хорд
                x2 = x1 - fb * (x1 - x0) / (fb - fa);

                double fx2 = funcObj.FuncX(x2);

                if (fa * fx2 < 0)
                    x1 = x2;
                else if (fb * fx2 < 0)
                    x0 = x2;
                else
                    break;

                iteration++;

            } while (Math.Abs(funcObj.FuncX(x2)) > Epsilon
                     && iteration < MaxIterations);

            return x2;
        }

        /// <summary>
        /// Поиск всех корней на отрезке [a, b]
        /// </summary>
        private List<double> FindAllRoots(double a, double b)
        {
            List<double> roots = new List<double>();
            int n = 1000;
            double h = (b - a) / n;

            for (int i = 0; i < n; i++)
            {
                double x1 = a + i * h;
                double x2 = x1 + h;

                if (funcObj.FuncX(x1) * funcObj.FuncX(x2) <= 0)
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

                        if (!isDuplicate &&
                            Math.Abs(funcObj.FuncX(root)) < Epsilon * 100)
                        {
                            roots.Add(root);
                        }
                    }
                    catch { }
                }
            }

            roots.Sort();
            return roots;
        }

        /// <summary>
        /// Построение графика
        /// </summary>
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

            for (double x = xMin; x <= xMax; x += step)
            {
                try
                {
                    double y = funcObj.FuncX(x);

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
            title.Text = funcObj.GetFunctionName();
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

        /// <summary>
        /// Показать корни на графике (без легенды)
        /// </summary>
        private void ShowRootsOnGraph(List<double> roots)
        {
            // Удаляем старые точки
            for (int i = chart1.Series.Count - 1; i >= 0; i--)
            {
                if (chart1.Series[i].Name.StartsWith("root"))
                    chart1.Series.Remove(chart1.Series[i]);
            }

            Color[] colors = { Color.Red, Color.Green, Color.Orange };

            for (int i = 0; i < roots.Count && i < 3; i++)
            {
                Series rootPoint = new Series($"root{i}");
                rootPoint.ChartType = SeriesChartType.Point;
                rootPoint.Color = colors[i];
                rootPoint.MarkerStyle = MarkerStyle.Circle;
                rootPoint.MarkerSize = 10;
                rootPoint.ChartArea = "main";
                rootPoint.IsVisibleInLegend = false; // без легенды
                rootPoint.Points.AddXY(roots[i], 0);
                chart1.Series.Add(rootPoint);
            }
        }

        // ── Кнопки ────────────────────────────────────────────────
        private void btnSolve_Click(object sender, EventArgs e)
        {
            // Сброс
            txtRoot1.Clear(); txtRoot1.BackColor = SystemColors.Window;
            txtRoot2.Clear(); txtRoot2.BackColor = SystemColors.Window;
            txtRoot3.Clear(); txtRoot3.BackColor = SystemColors.Window;

            if (!double.TryParse(txtStart.Text.Replace('.', ','),
                out double startPoint))
            {
                MessageBox.Show("Введите корректную начальную точку!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(txtEnd.Text.Replace('.', ','),
                out double endPoint))
            {
                MessageBox.Show("Введите корректную конечную точку!",
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
                    MessageBox.Show("Корни не найдены на указанном отрезке.",
                        "Результат", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                TextBox[] boxes = { txtRoot1, txtRoot2, txtRoot3 };

                for (int i = 0; i < roots.Count && i < boxes.Length; i++)
                {
                    boxes[i].Text = roots[i].ToString("F6");
                    boxes[i].BackColor = Color.LightGreen;
                }

                ShowRootsOnGraph(roots);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}