namespace Общая_форма
{
    partial class Form5
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblFunctionName = new System.Windows.Forms.Label();
            this.lblFormula = new System.Windows.Forms.Label();
            this.lblStart = new System.Windows.Forms.Label();
            this.lblRoot = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.txtStart = new System.Windows.Forms.TextBox();
            this.txtRoot = new System.Windows.Forms.TextBox();
            this.btnSolve = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblFunctionName
            // 
            this.lblFunctionName.AutoSize = true;
            this.lblFunctionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblFunctionName.Location = new System.Drawing.Point(12, 15);
            this.lblFunctionName.Name = "lblFunctionName";
            this.lblFunctionName.Size = new System.Drawing.Size(150, 17);
            this.lblFunctionName.TabIndex = 0;
            this.lblFunctionName.Text = "Выбранная функция:";
            this.lblFunctionName.Click += new System.EventHandler(this.lblFunctionName_Click);
            // 
            // lblFormula
            // 
            this.lblFormula.AutoSize = true;
            this.lblFormula.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblFormula.Location = new System.Drawing.Point(12, 40);
            this.lblFormula.Name = "lblFormula";
            this.lblFormula.Size = new System.Drawing.Size(100, 15);
            this.lblFormula.TabIndex = 1;
            this.lblFormula.Text = "F(x) = ...";
            this.lblFormula.Click += new System.EventHandler(this.lblFormula_Click);
            // 
            // lblStart
            // 
            this.lblStart.AutoSize = true;
            this.lblStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblStart.Location = new System.Drawing.Point(12, 80);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(120, 15);
            this.lblStart.TabIndex = 2;
            this.lblStart.Text = "Начальная точка X₀:";
            // 
            // lblRoot
            // 
            this.lblRoot.AutoSize = true;
            this.lblRoot.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblRoot.Location = new System.Drawing.Point(12, 120);
            this.lblRoot.Name = "lblRoot";
            this.lblRoot.Size = new System.Drawing.Size(50, 15);
            this.lblRoot.TabIndex = 3;
            this.lblRoot.Text = "Корень:";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblInfo.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblInfo.Location = new System.Drawing.Point(12, 160);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(60, 13);
            this.lblInfo.TabIndex = 4;
            this.lblInfo.Text = "Информация:";
            this.lblInfo.Click += new System.EventHandler(this.lblInfo_Click);
            // 
            // txtStart
            // 
            this.txtStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.txtStart.Location = new System.Drawing.Point(140, 77);
            this.txtStart.Name = "txtStart";
            this.txtStart.Size = new System.Drawing.Size(150, 21);
            this.txtStart.TabIndex = 5;
            this.txtStart.TextChanged += new System.EventHandler(this.txtStart_TextChanged);
            // 
            // txtRoot
            // 
            this.txtRoot.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.txtRoot.Location = new System.Drawing.Point(140, 117);
            this.txtRoot.Name = "txtRoot";
            this.txtRoot.ReadOnly = true;
            this.txtRoot.Size = new System.Drawing.Size(250, 21);
            this.txtRoot.TabIndex = 6;
            this.txtRoot.TextChanged += new System.EventHandler(this.txtRoot_TextChanged);
            // 
            // btnSolve
            // 
            this.btnSolve.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSolve.Location = new System.Drawing.Point(15, 200);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(100, 35);
            this.btnSolve.TabIndex = 7;
            this.btnSolve.Text = "Решить";
            this.btnSolve.UseVisualStyleBackColor = true;
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            // 
            // btnBack
            // 
            this.btnBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnBack.Location = new System.Drawing.Point(130, 200);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(100, 35);
            this.btnBack.TabIndex = 8;
            this.btnBack.Text = "Назад";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnExit.Location = new System.Drawing.Point(245, 200);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(100, 35);
            this.btnExit.TabIndex = 9;
            this.btnExit.Text = "Выход";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // chart1
            // 
            this.chart1.Location = new System.Drawing.Point(420, 15);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(450, 350);
            this.chart1.TabIndex = 10;
            this.chart1.Text = "chart1";
            this.chart1.Click += new System.EventHandler(this.chart1_Click);
            // 
            // Form5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 380);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnSolve);
            this.Controls.Add(this.txtRoot);
            this.Controls.Add(this.txtStart);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblRoot);
            this.Controls.Add(this.lblStart);
            this.Controls.Add(this.lblFormula);
            this.Controls.Add(this.lblFunctionName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form5";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Метод Итерации";
            this.Load += new System.EventHandler(this.Form5_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblFunctionName;
        private System.Windows.Forms.Label lblFormula;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.Label lblRoot;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.TextBox txtStart;
        private System.Windows.Forms.TextBox txtRoot;
        private System.Windows.Forms.Button btnSolve;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}
