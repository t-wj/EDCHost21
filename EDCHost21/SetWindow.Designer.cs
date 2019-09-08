namespace EDC21HOST
{
    partial class SetWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkBox_DebugMode = new System.Windows.Forms.CheckBox();
            this.nudAreaL = new System.Windows.Forms.NumericUpDown();
            this.lblAreaL = new System.Windows.Forms.Label();
            this.nudValueL = new System.Windows.Forms.NumericUpDown();
            this.lblValueL = new System.Windows.Forms.Label();
            this.nudSat2L = new System.Windows.Forms.NumericUpDown();
            this.lblSat2L = new System.Windows.Forms.Label();
            this.nudSat1L = new System.Windows.Forms.NumericUpDown();
            this.lblSat1L = new System.Windows.Forms.Label();
            this.nudHue2H = new System.Windows.Forms.NumericUpDown();
            this.lblHue2H = new System.Windows.Forms.Label();
            this.nudHue2L = new System.Windows.Forms.NumericUpDown();
            this.lblHue2L = new System.Windows.Forms.Label();
            this.nudHue1H = new System.Windows.Forms.NumericUpDown();
            this.lblHue1H = new System.Windows.Forms.Label();
            this.nudHue1L = new System.Windows.Forms.NumericUpDown();
            this.lblHue1L = new System.Windows.Forms.Label();
            this.button_ConfigSave = new System.Windows.Forms.Button();
            this.button_ConfigLoad = new System.Windows.Forms.Button();
            this.nudHue0H = new System.Windows.Forms.NumericUpDown();
            this.lblHue0H = new System.Windows.Forms.Label();
            this.nudHue0L = new System.Windows.Forms.NumericUpDown();
            this.lblHue0L = new System.Windows.Forms.Label();
            this.nudSat0L = new System.Windows.Forms.NumericUpDown();
            this.lblSat0L = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudAreaL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudValueL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSat2L)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSat1L)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue2H)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue2L)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue1H)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue1L)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue0H)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue0L)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSat0L)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBox_DebugMode
            // 
            this.checkBox_DebugMode.AutoSize = true;
            this.checkBox_DebugMode.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_DebugMode.Location = new System.Drawing.Point(286, 286);
            this.checkBox_DebugMode.Name = "checkBox_DebugMode";
            this.checkBox_DebugMode.Size = new System.Drawing.Size(91, 24);
            this.checkBox_DebugMode.TabIndex = 74;
            this.checkBox_DebugMode.Text = "调试模式";
            this.checkBox_DebugMode.UseVisualStyleBackColor = true;
            this.checkBox_DebugMode.CheckedChanged += new System.EventHandler(this.checkBox_DebugMode_CheckedChanged);
            // 
            // nudAreaL
            // 
            this.nudAreaL.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudAreaL.Location = new System.Drawing.Point(302, 233);
            this.nudAreaL.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudAreaL.Name = "nudAreaL";
            this.nudAreaL.Size = new System.Drawing.Size(75, 27);
            this.nudAreaL.TabIndex = 90;
            this.nudAreaL.ValueChanged += new System.EventHandler(this.nudAreaL_ValueChanged);
            // 
            // lblAreaL
            // 
            this.lblAreaL.AutoSize = true;
            this.lblAreaL.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblAreaL.Location = new System.Drawing.Point(237, 233);
            this.lblAreaL.Name = "lblAreaL";
            this.lblAreaL.Size = new System.Drawing.Size(55, 20);
            this.lblAreaL.TabIndex = 89;
            this.lblAreaL.Text = "AreaL:";
            // 
            // nudValueL
            // 
            this.nudValueL.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudValueL.Location = new System.Drawing.Point(302, 183);
            this.nudValueL.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudValueL.Name = "nudValueL";
            this.nudValueL.Size = new System.Drawing.Size(75, 27);
            this.nudValueL.TabIndex = 88;
            this.nudValueL.ValueChanged += new System.EventHandler(this.nudValueL_ValueChanged);
            // 
            // lblValueL
            // 
            this.lblValueL.AutoSize = true;
            this.lblValueL.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblValueL.Location = new System.Drawing.Point(237, 183);
            this.lblValueL.Name = "lblValueL";
            this.lblValueL.Size = new System.Drawing.Size(61, 20);
            this.lblValueL.TabIndex = 87;
            this.lblValueL.Text = "ValueL:";
            // 
            // nudSat2L
            // 
            this.nudSat2L.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudSat2L.Location = new System.Drawing.Point(302, 133);
            this.nudSat2L.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudSat2L.Name = "nudSat2L";
            this.nudSat2L.Size = new System.Drawing.Size(75, 27);
            this.nudSat2L.TabIndex = 86;
            this.nudSat2L.ValueChanged += new System.EventHandler(this.nudSat2L_ValueChanged);
            // 
            // lblSat2L
            // 
            this.lblSat2L.AutoSize = true;
            this.lblSat2L.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSat2L.Location = new System.Drawing.Point(237, 133);
            this.lblSat2L.Name = "lblSat2L";
            this.lblSat2L.Size = new System.Drawing.Size(53, 20);
            this.lblSat2L.TabIndex = 85;
            this.lblSat2L.Text = "Sat2L:";
            // 
            // nudSat1L
            // 
            this.nudSat1L.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudSat1L.Location = new System.Drawing.Point(302, 83);
            this.nudSat1L.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudSat1L.Name = "nudSat1L";
            this.nudSat1L.Size = new System.Drawing.Size(75, 27);
            this.nudSat1L.TabIndex = 84;
            this.nudSat1L.ValueChanged += new System.EventHandler(this.nudSat1L_ValueChanged);
            // 
            // lblSat1L
            // 
            this.lblSat1L.AutoSize = true;
            this.lblSat1L.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSat1L.Location = new System.Drawing.Point(237, 83);
            this.lblSat1L.Name = "lblSat1L";
            this.lblSat1L.Size = new System.Drawing.Size(53, 20);
            this.lblSat1L.TabIndex = 83;
            this.lblSat1L.Text = "Sat1L:";
            // 
            // nudHue2H
            // 
            this.nudHue2H.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudHue2H.Location = new System.Drawing.Point(105, 283);
            this.nudHue2H.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.nudHue2H.Name = "nudHue2H";
            this.nudHue2H.Size = new System.Drawing.Size(75, 27);
            this.nudHue2H.TabIndex = 82;
            this.nudHue2H.ValueChanged += new System.EventHandler(this.nudHue2H_ValueChanged);
            // 
            // lblHue2H
            // 
            this.lblHue2H.AutoSize = true;
            this.lblHue2H.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblHue2H.Location = new System.Drawing.Point(40, 283);
            this.lblHue2H.Name = "lblHue2H";
            this.lblHue2H.Size = new System.Drawing.Size(64, 20);
            this.lblHue2H.TabIndex = 81;
            this.lblHue2H.Text = "Hue2H:";
            // 
            // nudHue2L
            // 
            this.nudHue2L.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudHue2L.Location = new System.Drawing.Point(105, 233);
            this.nudHue2L.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.nudHue2L.Name = "nudHue2L";
            this.nudHue2L.Size = new System.Drawing.Size(75, 27);
            this.nudHue2L.TabIndex = 80;
            this.nudHue2L.ValueChanged += new System.EventHandler(this.nudHue2L_ValueChanged);
            // 
            // lblHue2L
            // 
            this.lblHue2L.AutoSize = true;
            this.lblHue2L.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblHue2L.Location = new System.Drawing.Point(40, 233);
            this.lblHue2L.Name = "lblHue2L";
            this.lblHue2L.Size = new System.Drawing.Size(60, 20);
            this.lblHue2L.TabIndex = 79;
            this.lblHue2L.Text = "Hue2L:";
            // 
            // nudHue1H
            // 
            this.nudHue1H.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudHue1H.Location = new System.Drawing.Point(105, 183);
            this.nudHue1H.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.nudHue1H.Name = "nudHue1H";
            this.nudHue1H.Size = new System.Drawing.Size(75, 27);
            this.nudHue1H.TabIndex = 78;
            this.nudHue1H.ValueChanged += new System.EventHandler(this.nudHue1H_ValueChanged);
            // 
            // lblHue1H
            // 
            this.lblHue1H.AutoSize = true;
            this.lblHue1H.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblHue1H.Location = new System.Drawing.Point(40, 183);
            this.lblHue1H.Name = "lblHue1H";
            this.lblHue1H.Size = new System.Drawing.Size(64, 20);
            this.lblHue1H.TabIndex = 77;
            this.lblHue1H.Text = "Hue1H:";
            // 
            // nudHue1L
            // 
            this.nudHue1L.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudHue1L.Location = new System.Drawing.Point(105, 133);
            this.nudHue1L.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.nudHue1L.Name = "nudHue1L";
            this.nudHue1L.Size = new System.Drawing.Size(75, 27);
            this.nudHue1L.TabIndex = 76;
            this.nudHue1L.ValueChanged += new System.EventHandler(this.nudHue1L_ValueChanged);
            // 
            // lblHue1L
            // 
            this.lblHue1L.AutoSize = true;
            this.lblHue1L.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblHue1L.Location = new System.Drawing.Point(40, 133);
            this.lblHue1L.Name = "lblHue1L";
            this.lblHue1L.Size = new System.Drawing.Size(60, 20);
            this.lblHue1L.TabIndex = 75;
            this.lblHue1L.Text = "Hue1L:";
            // 
            // button_ConfigSave
            // 
            this.button_ConfigSave.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_ConfigSave.Location = new System.Drawing.Point(494, 33);
            this.button_ConfigSave.Name = "button_ConfigSave";
            this.button_ConfigSave.Size = new System.Drawing.Size(80, 35);
            this.button_ConfigSave.TabIndex = 91;
            this.button_ConfigSave.Text = "保存";
            this.button_ConfigSave.UseVisualStyleBackColor = true;
            this.button_ConfigSave.Click += new System.EventHandler(this.button_ConfigSave_Click);
            // 
            // button_ConfigLoad
            // 
            this.button_ConfigLoad.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_ConfigLoad.Location = new System.Drawing.Point(494, 84);
            this.button_ConfigLoad.Name = "button_ConfigLoad";
            this.button_ConfigLoad.Size = new System.Drawing.Size(80, 35);
            this.button_ConfigLoad.TabIndex = 92;
            this.button_ConfigLoad.Text = "读取";
            this.button_ConfigLoad.UseVisualStyleBackColor = true;
            this.button_ConfigLoad.Click += new System.EventHandler(this.button_ConfigLoad_Click);
            // 
            // nudHue0H
            // 
            this.nudHue0H.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudHue0H.Location = new System.Drawing.Point(105, 84);
            this.nudHue0H.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.nudHue0H.Name = "nudHue0H";
            this.nudHue0H.Size = new System.Drawing.Size(75, 27);
            this.nudHue0H.TabIndex = 96;
            this.nudHue0H.ValueChanged += new System.EventHandler(this.nudHue0H_ValueChanged);
            // 
            // lblHue0H
            // 
            this.lblHue0H.AutoSize = true;
            this.lblHue0H.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblHue0H.Location = new System.Drawing.Point(40, 84);
            this.lblHue0H.Name = "lblHue0H";
            this.lblHue0H.Size = new System.Drawing.Size(64, 20);
            this.lblHue0H.TabIndex = 95;
            this.lblHue0H.Text = "Hue0H:";
            // 
            // nudHue0L
            // 
            this.nudHue0L.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudHue0L.Location = new System.Drawing.Point(105, 34);
            this.nudHue0L.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.nudHue0L.Name = "nudHue0L";
            this.nudHue0L.Size = new System.Drawing.Size(75, 27);
            this.nudHue0L.TabIndex = 94;
            this.nudHue0L.ValueChanged += new System.EventHandler(this.nudHue0L_ValueChanged);
            // 
            // lblHue0L
            // 
            this.lblHue0L.AutoSize = true;
            this.lblHue0L.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblHue0L.Location = new System.Drawing.Point(40, 34);
            this.lblHue0L.Name = "lblHue0L";
            this.lblHue0L.Size = new System.Drawing.Size(60, 20);
            this.lblHue0L.TabIndex = 93;
            this.lblHue0L.Text = "Hue0L:";
            // 
            // nudSat0L
            // 
            this.nudSat0L.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nudSat0L.Location = new System.Drawing.Point(302, 34);
            this.nudSat0L.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudSat0L.Name = "nudSat0L";
            this.nudSat0L.Size = new System.Drawing.Size(75, 27);
            this.nudSat0L.TabIndex = 98;
            this.nudSat0L.ValueChanged += new System.EventHandler(this.nudSat0L_ValueChanged);
            // 
            // lblSat0L
            // 
            this.lblSat0L.AutoSize = true;
            this.lblSat0L.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSat0L.Location = new System.Drawing.Point(237, 34);
            this.lblSat0L.Name = "lblSat0L";
            this.lblSat0L.Size = new System.Drawing.Size(53, 20);
            this.lblSat0L.TabIndex = 97;
            this.lblSat0L.Text = "Sat0L:";
            // 
            // SetWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 345);
            this.Controls.Add(this.nudSat0L);
            this.Controls.Add(this.lblSat0L);
            this.Controls.Add(this.nudHue0H);
            this.Controls.Add(this.lblHue0H);
            this.Controls.Add(this.nudHue0L);
            this.Controls.Add(this.lblHue0L);
            this.Controls.Add(this.button_ConfigLoad);
            this.Controls.Add(this.button_ConfigSave);
            this.Controls.Add(this.nudAreaL);
            this.Controls.Add(this.lblAreaL);
            this.Controls.Add(this.nudValueL);
            this.Controls.Add(this.lblValueL);
            this.Controls.Add(this.nudSat2L);
            this.Controls.Add(this.lblSat2L);
            this.Controls.Add(this.nudSat1L);
            this.Controls.Add(this.lblSat1L);
            this.Controls.Add(this.nudHue2H);
            this.Controls.Add(this.lblHue2H);
            this.Controls.Add(this.nudHue2L);
            this.Controls.Add(this.lblHue2L);
            this.Controls.Add(this.nudHue1H);
            this.Controls.Add(this.lblHue1H);
            this.Controls.Add(this.nudHue1L);
            this.Controls.Add(this.lblHue1L);
            this.Controls.Add(this.checkBox_DebugMode);
            this.Name = "SetWindow";
            this.Text = "SetWindow";
            ((System.ComponentModel.ISupportInitialize)(this.nudAreaL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudValueL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSat2L)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSat1L)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue2H)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue2L)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue1H)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue1L)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue0H)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue0L)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSat0L)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_DebugMode;
        private System.Windows.Forms.NumericUpDown nudAreaL;
        private System.Windows.Forms.Label lblAreaL;
        private System.Windows.Forms.NumericUpDown nudValueL;
        private System.Windows.Forms.Label lblValueL;
        private System.Windows.Forms.NumericUpDown nudSat2L;
        private System.Windows.Forms.Label lblSat2L;
        private System.Windows.Forms.NumericUpDown nudSat1L;
        private System.Windows.Forms.Label lblSat1L;
        private System.Windows.Forms.NumericUpDown nudHue2H;
        private System.Windows.Forms.Label lblHue2H;
        private System.Windows.Forms.NumericUpDown nudHue2L;
        private System.Windows.Forms.Label lblHue2L;
        private System.Windows.Forms.NumericUpDown nudHue1H;
        private System.Windows.Forms.Label lblHue1H;
        private System.Windows.Forms.NumericUpDown nudHue1L;
        private System.Windows.Forms.Label lblHue1L;
        private System.Windows.Forms.Button button_ConfigSave;
        private System.Windows.Forms.Button button_ConfigLoad;
        private System.Windows.Forms.NumericUpDown nudHue0H;
        private System.Windows.Forms.Label lblHue0H;
        private System.Windows.Forms.NumericUpDown nudHue0L;
        private System.Windows.Forms.Label lblHue0L;
        private System.Windows.Forms.NumericUpDown nudSat0L;
        private System.Windows.Forms.Label lblSat0L;
    }
}