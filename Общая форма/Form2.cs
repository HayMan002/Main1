using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using System.Windows.Forms.DataVisualization.Charting;

// GlobalData.SelectedIndex находится внутри Form1
using static Общая_форма.Form1;

namespace Общая_форма
{
    public partial class Form2 : Form
    {
        private const double Eps = 1e-6;
        private const int MaxIter = 200;
        private const int ScanSegments = 2000;

        // Для графика
        private const int PlotSamples = 2000;
        private const int PlotGridLines = 10;
        private const int MaxRootsToShow = 3;

        private Func<double, double> _f = x => 0.0;
        private string _fName = "Функция не выбрана";
        private int _lastSeenIndex = int.MinValue;

        private Form1 _form1;
        private bool _exitRequested = false;

        // Кэш последнего графика (для перерисовки при Resize)
        private bool _hasLastPlot = false;
        private double _lastX0, _lastX1;
        private List<double> _lastRoots = new List<double>();

        public Form2()
        {
            InitializeComponent();

            Load += Form2_Load;
            Shown += (_, __) => RefreshFunctionAndUI(force: true);
            Activated += (_, __) => RefreshFunctionAndUI(force: false);
            FormClosing += Form2_FormClosing;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Text = "Метод половинного деления";

            _form1 = Owner as Form1 ?? Application.OpenForms.Cast<Form>().OfType<Form1>().FirstOrDefault();

            WireButtonsByTag();

            // Дефолт x0/x1 если пусто
            var tbX0 = FindByTag<TextBoxBase>("x0");
            var tbX1 = FindByTag<TextBoxBase>("x1");
            if (tbX0 != null && string.IsNullOrWhiteSpace(tbX0.Text)) tbX0.Text = "-10";
            if (tbX1 != null && string.IsNullOrWhiteSpace(tbX1.Text)) tbX1.Text = "10";

            PrepareGraphTarget();
            RefreshFunctionAndUI(force: true);
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_exitRequested) return;

            // Возвращаем Form1, чтобы приложение не осталось "живым" без окон
            try
            {
                if (_form1 != null)
                {
                    _form1.Show();
                    _form1.WindowState = FormWindowState.Normal;
                    _form1.Activate();
                }
            }
            catch { }
        }

        // =========================
        // Кнопки
        // =========================
        private void WireButtonsByTag()
        {
            var btnSolve = FindByTag<Button>("solve");
            if (btnSolve != null) btnSolve.Click += (_, __) => Solve();

            var btnBack = FindByTag<Button>("back");
            if (btnBack != null) btnBack.Click += (_, __) => GoBack();

            var btnExit = FindByTag<Button>("exit");
            if (btnExit != null) btnExit.Click += (_, __) => ExitApp();
        }

        private void GoBack()
        {
            try
            {
                if (_form1 != null)
                {
                    _form1.Show();
                    _form1.WindowState = FormWindowState.Normal;
                    _form1.Activate();
                }
            }
            catch { }

            Close();
        }

        private void ExitApp()
        {
            _exitRequested = true;

            try { Application.Exit(); } catch { }

            // гарантированное завершение процесса
            Environment.Exit(0);
        }

        // =========================
        // Выбор функции (без правок Form1)
        // =========================
        private void RefreshFunctionAndUI(bool force)
        {
            int idx = GetSelectedIndexFromForm1ComboFirst(out string comboText);
            if (idx < 0) idx = GlobalData.SelectedIndex;
            if (idx >= 0) GlobalData.SelectedIndex = idx;

            if (!force && idx == _lastSeenIndex)
                return;

            _lastSeenIndex = idx;

            var fi = GetFunctionBySelectedIndex(idx);
            _f = fi.F;
            _fName = !string.IsNullOrWhiteSpace(comboText) ? comboText : fi.Name;

            var lblFunc = FindByTag<Label>("function");
            if (lblFunc != null) lblFunc.Text = _fName;
        }

        private int GetSelectedIndexFromForm1ComboFirst(out string comboText)
        {
            comboText = null;

            var f1 = _form1 ?? Application.OpenForms.Cast<Form>().OfType<Form1>().FirstOrDefault();
            if (f1 == null) return -1;

            var cb = f1.Controls.Find("comboBox1", true).OfType<ComboBox>().FirstOrDefault();
            if (cb == null) return -1;

            comboText = cb.SelectedItem?.ToString();
            return cb.SelectedIndex;
        }

        // =========================
        // Решение
        // =========================
        private void Solve()
        {
            RefreshFunctionAndUI(force: true);

            if (!TryReadDoubleFromTag("x0", out double x0))
            {
                ShowError("Не удалось прочитать начальную точку x0 (Tag=\"x0\").");
                return;
            }

            if (!TryReadDoubleFromTag("x1", out double x1))
            {
                ShowError("Не удалось прочитать конечную точку x1 (Tag=\"x1\").");
                return;
            }

            if (x0 == x1)
            {
                ShowError("x0 и x1 не должны быть равны.");
                return;
            }

            if (x0 > x1)
            {
                double t = x0; x0 = x1; x1 = t;
                WriteTextToTag("x0", FormatNumber(x0));
                WriteTextToTag("x1", FormatNumber(x1));
            }

            var brackets = FindBrackets(_f, x0, x1, ScanSegments);

            var results = new List<BisectionResult>();
            var uniqueRoots = new List<double>();

            foreach (var br in brackets)
            {
                var res = Bisect(_f, br.A, br.B, Eps, MaxIter);
                if (!res.Converged) continue;

                if (IsRootDuplicate(uniqueRoots, res.Root, 50 * Eps))
                    continue;

                uniqueRoots.Add(res.Root);
                results.Add(res);

                if (results.Count >= MaxRootsToShow)
                    break;
            }

            WriteRootsToUI(results);
            WriteTablesToUI(results);

            _hasLastPlot = true;
            _lastX0 = x0;
            _lastX1 = x1;
            _lastRoots = results.Select(r => r.Root).ToList();

            RenderGraph(_f, x0, x1, _lastRoots);

            if (results.Count == 0)
            {
                ShowInfo("На заданном отрезке не найдено корней методом половинного деления.\n" +
                         "Метод требует смену знака f(x) на подотрезке.");
            }
        }

        // =========================
        // Корни / Таблицы
        // =========================
        private void WriteRootsToUI(List<BisectionResult> results)
        {
            WriteTextToTag("root1", "");
            WriteTextToTag("root2", "");
            WriteTextToTag("root3", "");

            for (int i = 0; i < results.Count && i < 3; i++)
            {
                string tag = "root" + (i + 1).ToString(CultureInfo.InvariantCulture);
                WriteTextToTag(tag, FormatNumber(results[i].Root));
            }
        }

        private void WriteTablesToUI(List<BisectionResult> results)
        {
            var t1 = FindByTag<DataGridView>("table1");
            var t2 = FindByTag<DataGridView>("table2");
            var t3 = FindByTag<DataGridView>("table3");
            var tAll = FindByTag<DataGridView>("table");

            if (t1 != null || t2 != null || t3 != null)
            {
                BindGrid(t1, results.ElementAtOrDefault(0)?.Iterations);
                BindGrid(t2, results.ElementAtOrDefault(1)?.Iterations);
                BindGrid(t3, results.ElementAtOrDefault(2)?.Iterations);
                return;
            }

            if (tAll != null)
            {
                var merged = new BindingList<IterationRowMerged>();

                for (int r = 0; r < results.Count; r++)
                {
                    foreach (var it in results[r].Iterations)
                    {
                        merged.Add(new IterationRowMerged
                        {
                            RootNo = r + 1,
                            Iter = it.Iter,
                            A = it.A,
                            B = it.B,
                            C = it.C,
                            FC = it.FC,
                            Width = it.Width
                        });
                    }
                }

                tAll.AutoGenerateColumns = true;
                tAll.DataSource = merged;
            }
        }

        private static void BindGrid(DataGridView grid, object dataSource)
        {
            if (grid == null) return;
            grid.AutoGenerateColumns = true;
            grid.DataSource = null;
            grid.DataSource = dataSource;
        }

        // =========================
        // ГРАФИК (PictureBox или Chart)
        // =========================
        private void PrepareGraphTarget()
        {
            var pb = FindGraphPictureBox();
            if (pb != null)
            {
                pb.BackColor = Color.White;
                pb.SizeMode = PictureBoxSizeMode.Normal;
                pb.Resize += (_, __) =>
                {
                    if (_hasLastPlot)
                        RenderGraph(_f, _lastX0, _lastX1, _lastRoots);
                };
            }

            var ch = FindGraphChart();
            if (ch != null)
            {
                if (ch.ChartAreas.Count == 0)
                    ch.ChartAreas.Add(new ChartArea("Main"));

                ch.Series.Clear();
                ch.Legends.Clear();
                ch.AntiAliasing = AntiAliasingStyles.All;
                ch.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
            }
        }

        private void RenderGraph(Func<double, double> f, double x0, double x1, List<double> roots)
        {
            var pb = FindGraphPictureBox();
            if (pb != null)
            {
                RenderToPictureBox(pb, f, x0, x1, roots);
                return;
            }

            var ch = FindGraphChart();
            if (ch != null)
            {
                RenderToChart(ch, f, x0, x1, roots);
                return;
            }
        }

        private PictureBox FindGraphPictureBox()
        {
            var pb = FindByTag<PictureBox>("graph");
            if (pb != null) return pb;

            // запасной вариант
            pb = FindByTag<PictureBox>("chart");
            if (pb != null) return pb;

            return null;
        }

        private Chart FindGraphChart()
        {
            var ch = FindByTag<Chart>("chart");
            if (ch != null) return ch;

            return EnumerateControls(this).OfType<Chart>().FirstOrDefault();
        }

        private static void RenderToChart(Chart chart, Func<double, double> f, double x0, double x1, List<double> roots)
        {
            chart.Series.Clear();

            var area = chart.ChartAreas.Count > 0 ? chart.ChartAreas[0] : new ChartArea("Main");
            if (chart.ChartAreas.Count == 0) chart.ChartAreas.Add(area);

            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisX.Minimum = x0;
            area.AxisX.Maximum = x1;

            var xs = new List<double>(PlotSamples);
            var ys = new List<double>(PlotSamples);

            double dx = (x1 - x0) / (PlotSamples - 1);
            for (int i = 0; i < PlotSamples; i++)
            {
                double x = x0 + dx * i;
                double y = SafeEval(f, x);
                if (double.IsNaN(y) || double.IsInfinity(y)) continue;
                xs.Add(x);
                ys.Add(y);
            }

            ComputeNiceYRange(ys, out double yMin, out double yMax);
            yMin = Math.Min(yMin, 0);
            yMax = Math.Max(yMax, 0);

            area.AxisY.Minimum = yMin;
            area.AxisY.Maximum = yMax;

            area.AxisX.Crossing = 0;
            area.AxisY.Crossing = 0;

            var sFunc = new Series("f(x)")
            {
                ChartType = SeriesChartType.FastLine,
                BorderWidth = 2,
                Color = Color.RoyalBlue
            };

            for (int i = 0; i < xs.Count; i++)
                sFunc.Points.AddXY(xs[i], ys[i]);

            var sRoots = new Series("roots")
            {
                ChartType = SeriesChartType.Point,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 8,
                Color = Color.Red
            };

            if (roots != null)
                foreach (var r in roots)
                    sRoots.Points.AddXY(r, 0);

            chart.Series.Add(sFunc);
            if (roots != null && roots.Count > 0)
                chart.Series.Add(sRoots);

            chart.Invalidate();
        }

        private static void RenderToPictureBox(PictureBox pb, Func<double, double> f, double x0, double x1, List<double> roots)
        {
            int w = Math.Max(10, pb.ClientSize.Width);
            int h = Math.Max(10, pb.ClientSize.Height);

            var bmp = new Bitmap(w, h);
            using (var g = Graphics.FromImage(bmp))
            using (var gridPen = new Pen(Color.Gainsboro, 1f))
            using (var axisPen = new Pen(Color.Gray, 2f))
            using (var funcPen = new Pen(Color.RoyalBlue, 2f))
            using (var rootBrush = new SolidBrush(Color.Red))
            using (var textBrush = new SolidBrush(Color.Black))
            using (var font = new Font("Segoe UI", 9f))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                int marginL = 50, marginR = 15, marginT = 10, marginB = 35;
                var rect = Rectangle.FromLTRB(
                    marginL,
                    marginT,
                    Math.Max(marginL + 1, w - marginR),
                    Math.Max(marginT + 1, h - marginB)
                );

                // Семплинг под ширину
                int n = Math.Max(200, rect.Width);
                var ys = new List<double>(n);
                var pts = new List<PointF>(n);

                for (int i = 0; i < n; i++)
                {
                    double x = x0 + (x1 - x0) * i / (n - 1.0);
                    double y = SafeEval(f, x);
                    if (!double.IsNaN(y) && !double.IsInfinity(y))
                        ys.Add(y);
                }

                ComputeNiceYRange(ys, out double yMin, out double yMax);
                yMin = Math.Min(yMin, 0);
                yMax = Math.Max(yMax, 0);

                if (Math.Abs(yMax - yMin) < 1e-12)
                {
                    yMin -= 1;
                    yMax += 1;
                }

                // Сетка
                for (int i = 0; i <= PlotGridLines; i++)
                {
                    float x = rect.Left + rect.Width * i / (float)PlotGridLines;
                    g.DrawLine(gridPen, x, rect.Top, x, rect.Bottom);

                    float y = rect.Top + rect.Height * i / (float)PlotGridLines;
                    g.DrawLine(gridPen, rect.Left, y, rect.Right, y);
                }

                // Оси (если 0 в пределах)
                if (x0 <= 0 && 0 <= x1)
                {
                    float xPix0 = XToPix(0.0, x0, x1, rect);
                    g.DrawLine(axisPen, xPix0, rect.Top, xPix0, rect.Bottom);
                }
                if (yMin <= 0 && 0 <= yMax)
                {
                    float yPix0 = YToPix(0.0, yMin, yMax, rect);
                    g.DrawLine(axisPen, rect.Left, yPix0, rect.Right, yPix0);
                }

                // подписи
                g.DrawString(FormatNumber(x0), font, textBrush, rect.Left - 5, rect.Bottom + 5);
                g.DrawString(FormatNumber(x1), font, textBrush, rect.Right - 35, rect.Bottom + 5);
                g.DrawString(FormatNumber(yMax), font, textBrush, 5, rect.Top);
                g.DrawString(FormatNumber(yMin), font, textBrush, 5, rect.Bottom - 15);

                // функция
                pts.Clear();
                for (int i = 0; i < n; i++)
                {
                    double x = x0 + (x1 - x0) * i / (n - 1.0);
                    double y = SafeEval(f, x);
                    if (double.IsNaN(y) || double.IsInfinity(y)) continue;

                    float px = XToPix(x, x0, x1, rect);
                    float py = YToPix(y, yMin, yMax, rect);
                    pts.Add(new PointF(px, py));
                }

                if (pts.Count >= 2)
                    g.DrawLines(funcPen, pts.ToArray());

                // корни
                if (roots != null)
                {
                    foreach (var r in roots)
                    {
                        if (r < x0 || r > x1) continue;

                        float px = XToPix(r, x0, x1, rect);
                        float py = YToPix(0.0, yMin, yMax, rect);
                        g.FillEllipse(rootBrush, px - 4, py - 4, 8, 8);
                    }
                }

                g.DrawRectangle(Pens.LightGray, rect);
            }

            var old = pb.Image;
            pb.Image = bmp;
            old?.Dispose();
        }

        // FIX компиляции: работаем с double, возвращаем float (для пикселей)
        private static float XToPix(double x, double x0, double x1, Rectangle rect)
            => (float)(rect.Left + (x - x0) * rect.Width / (x1 - x0));

        private static float YToPix(double y, double yMin, double yMax, Rectangle rect)
            => (float)(rect.Top + (yMax - y) * rect.Height / (yMax - yMin));

        private static void ComputeNiceYRange(List<double> ys, out double yMin, out double yMax)
        {
            yMin = -1;
            yMax = 1;

            if (ys == null || ys.Count == 0)
                return;

            var sorted = ys.Where(v => !double.IsNaN(v) && !double.IsInfinity(v)).OrderBy(v => v).ToList();
            if (sorted.Count == 0) return;

            int n = sorted.Count;
            int lo = (int)Math.Floor(0.02 * (n - 1));
            int hi = (int)Math.Ceiling(0.98 * (n - 1));

            yMin = sorted[Math.Max(0, Math.Min(n - 1, lo))];
            yMax = sorted[Math.Max(0, Math.Min(n - 1, hi))];

            if (yMin == yMax)
            {
                yMin -= 1;
                yMax += 1;
            }

            double pad = 0.10 * (yMax - yMin);
            yMin -= pad;
            yMax += pad;
        }

        // =========================
        // Bisection
        // =========================
        private sealed class BisectionResult
        {
            public bool Converged;
            public double Root;
            public BindingList<IterationRow> Iterations = new BindingList<IterationRow>();
        }

        private sealed class IterationRow
        {
            public int Iter { get; set; }
            public double A { get; set; }
            public double B { get; set; }
            public double C { get; set; }
            public double FC { get; set; }
            public double Width { get; set; }
        }

        private sealed class IterationRowMerged
        {
            public int RootNo { get; set; }
            public int Iter { get; set; }
            public double A { get; set; }
            public double B { get; set; }
            public double C { get; set; }
            public double FC { get; set; }
            public double Width { get; set; }
        }

        private static BisectionResult Bisect(Func<double, double> f, double a, double b, double eps, int maxIter)
        {
            var res = new BisectionResult();

            double fa = SafeEval(f, a);
            double fb = SafeEval(f, b);

            if (double.IsNaN(fa) || double.IsNaN(fb) || double.IsInfinity(fa) || double.IsInfinity(fb))
                return res;

            if (Math.Abs(fa) <= eps)
            {
                res.Converged = true;
                res.Root = a;
                res.Iterations.Add(new IterationRow { Iter = 0, A = a, B = b, C = a, FC = fa, Width = Math.Abs(b - a) });
                return res;
            }

            if (Math.Abs(fb) <= eps)
            {
                res.Converged = true;
                res.Root = b;
                res.Iterations.Add(new IterationRow { Iter = 0, A = a, B = b, C = b, FC = fb, Width = Math.Abs(b - a) });
                return res;
            }

            if (fa * fb > 0)
                return res;

            double left = a, right = b;
            double fLeft = fa;

            for (int iter = 1; iter <= maxIter; iter++)
            {
                double c = 0.5 * (left + right);
                double fc = SafeEval(f, c);
                double width = Math.Abs(right - left);

                res.Iterations.Add(new IterationRow
                {
                    Iter = iter,
                    A = left,
                    B = right,
                    C = c,
                    FC = fc,
                    Width = width
                });

                if (double.IsNaN(fc) || double.IsInfinity(fc))
                    return res;

                if (Math.Abs(fc) <= eps || 0.5 * width <= eps)
                {
                    res.Converged = true;
                    res.Root = c;
                    return res;
                }

                if (fLeft * fc < 0)
                {
                    right = c;
                }
                else
                {
                    left = c;
                    fLeft = fc;
                }
            }

            return res;
        }

        private static List<Bracket> FindBrackets(Func<double, double> f, double x0, double x1, int segments)
        {
            var brackets = new List<Bracket>();
            double step = (x1 - x0) / segments;

            double prevX = x0;
            double prevF = SafeEval(f, prevX);

            if (!double.IsNaN(prevF) && !double.IsInfinity(prevF) && Math.Abs(prevF) <= Eps)
                brackets.Add(new Bracket(prevX, prevX));

            for (int i = 1; i <= segments; i++)
            {
                double x = x0 + step * i;
                double fx = SafeEval(f, x);

                if (double.IsNaN(prevF) || double.IsInfinity(prevF) || double.IsNaN(fx) || double.IsInfinity(fx))
                {
                    prevX = x;
                    prevF = fx;
                    continue;
                }

                if (Math.Abs(fx) <= Eps)
                {
                    brackets.Add(new Bracket(x, x));
                }
                else if (prevF * fx < 0)
                {
                    brackets.Add(new Bracket(prevX, x));
                }

                prevX = x;
                prevF = fx;
            }

            return brackets;
        }

        private readonly struct Bracket
        {
            public readonly double A;
            public readonly double B;
            public Bracket(double a, double b) { A = a; B = b; }
        }

        private static bool IsRootDuplicate(List<double> roots, double candidate, double tol)
        {
            for (int i = 0; i < roots.Count; i++)
                if (Math.Abs(roots[i] - candidate) <= tol)
                    return true;
            return false;
        }

        private static double SafeEval(Func<double, double> f, double x)
        {
            try { return f(x); }
            catch { return double.NaN; }
        }

        // =========================
        // Функции (как у вас 0..3)
        // =========================
        private sealed class FunctionInfo
        {
            public string Name { get; }
            public Func<double, double> F { get; }
            public FunctionInfo(string name, Func<double, double> f) { Name = name; F = f; }
        }

        private static FunctionInfo GetFunctionBySelectedIndex(int idx)
        {
            switch (idx)
            {
                case 0:
                    return new FunctionInfo("f(x) = x^3 - 3x^2 - 24x - 3",
                        x => Math.Pow(x, 3) - 3 * Math.Pow(x, 2) - 24 * x - 3);
                case 1:
                    return new FunctionInfo("f(x) = x^3 + 3x^2 - 2",
                        x => Math.Pow(x, 3) + 3 * Math.Pow(x, 2) - 2);
                case 2:
                    return new FunctionInfo("f(x) = 2x^3 + 9x^2 - 10",
                        x => 2 * Math.Pow(x, 3) + 9 * Math.Pow(x, 2) - 10);
                case 3:
                    return new FunctionInfo("f(x) = x^3 + 3x^2 - 3.5",
                        x => Math.Pow(x, 3) + 3 * Math.Pow(x, 2) - 3.5);
                default:
                    return new FunctionInfo("Функция не выбрана", x => 0.0);
            }
        }

        // =========================
        // Tag helpers
        // =========================
        private bool TryReadDoubleFromTag(string tag, out double value)
        {
            value = 0.0;
            var tb = FindByTag<TextBoxBase>(tag);
            if (tb == null) return false;
            return TryParseDouble(tb.Text, out value);
        }

        private void WriteTextToTag(string tag, string text)
        {
            var tb = FindByTag<TextBoxBase>(tag);
            if (tb != null) { tb.Text = text ?? ""; return; }

            var lbl = FindByTag<Label>(tag);
            if (lbl != null) { lbl.Text = text ?? ""; return; }
        }

        private static bool TryParseDouble(string s, out double value)
        {
            value = 0.0;
            if (s == null) return false;

            string t = s.Trim().Replace(',', '.');
            return double.TryParse(
                t,
                NumberStyles.Float | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out value
            );
        }

        private static string FormatNumber(double x)
            => x.ToString("0.##########", CultureInfo.InvariantCulture);

        private static void ShowError(string msg)
            => MessageBox.Show(msg, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private static void ShowInfo(string msg)
            => MessageBox.Show(msg, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private T FindByTag<T>(string tag) where T : Control
        {
            if (string.IsNullOrWhiteSpace(tag)) return null;

            foreach (var c in EnumerateControls(this))
            {
                if (c is T typed)
                {
                    if (typed.Tag is string s && string.Equals(s, tag, StringComparison.OrdinalIgnoreCase))
                        return typed;
                }
            }
            return null;
        }

        private static IEnumerable<Control> EnumerateControls(Control root)
        {
            if (root == null) yield break;

            foreach (Control c in root.Controls)
            {
                yield return c;
                foreach (var inner in EnumerateControls(c))
                    yield return inner;
            }
        }

        // Заглушки под события из Designer (если были привязаны)
        private void label1_Click(object sender, EventArgs e) { }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}