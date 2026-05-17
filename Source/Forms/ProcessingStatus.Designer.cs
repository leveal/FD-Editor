namespace FR_Operator
{
    partial class ProcessingStatus
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
            this.button_processingBreak = new System.Windows.Forms.Button();
            this.textBox_ProcessingStatusMessage = new System.Windows.Forms.TextBox();
            this.textBox_errors = new System.Windows.Forms.TextBox();
            this.comboBox_dynPause = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // button_processingBreak
            // 
            this.button_processingBreak.Location = new System.Drawing.Point(124, 188);
            this.button_processingBreak.Name = "button_processingBreak";
            this.button_processingBreak.Size = new System.Drawing.Size(174, 23);
            this.button_processingBreak.TabIndex = 0;
            this.button_processingBreak.Text = "Прервать";
            this.button_processingBreak.UseVisualStyleBackColor = true;
            this.button_processingBreak.Click += new System.EventHandler(this.button_processingBreak_Click);
            // 
            // textBox_ProcessingStatusMessage
            // 
            this.textBox_ProcessingStatusMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_ProcessingStatusMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox_ProcessingStatusMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox_ProcessingStatusMessage.Location = new System.Drawing.Point(0, 0);
            this.textBox_ProcessingStatusMessage.Multiline = true;
            this.textBox_ProcessingStatusMessage.Name = "textBox_ProcessingStatusMessage";
            this.textBox_ProcessingStatusMessage.ReadOnly = true;
            this.textBox_ProcessingStatusMessage.Size = new System.Drawing.Size(440, 150);
            this.textBox_ProcessingStatusMessage.TabIndex = 1;
            this.textBox_ProcessingStatusMessage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_errors
            // 
            this.textBox_errors.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_errors.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox_errors.Location = new System.Drawing.Point(0, 160);
            this.textBox_errors.Name = "textBox_errors";
            this.textBox_errors.ReadOnly = true;
            this.textBox_errors.Size = new System.Drawing.Size(395, 22);
            this.textBox_errors.TabIndex = 2;
            this.textBox_errors.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // comboBox_dynPause
            // 
            this.comboBox_dynPause.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_dynPause.FormattingEnabled = true;
            this.comboBox_dynPause.Items.AddRange(new object[] {
            "Без пауз",
            "пауза 1с",
            "пауза 2с",
            "пауза 4с",
            "пауза 8с",
            "пауза 16с",
            "пауза 32с",
            "пауза 64с"});
            this.comboBox_dynPause.Location = new System.Drawing.Point(297, 188);
            this.comboBox_dynPause.Name = "comboBox_dynPause";
            this.comboBox_dynPause.Size = new System.Drawing.Size(135, 21);
            this.comboBox_dynPause.TabIndex = 3;
            this.comboBox_dynPause.Visible = false;
            this.comboBox_dynPause.SelectedIndexChanged += new System.EventHandler(this.ComboBox_dynPause_SelectedIndexChanged);
            // 
            // ProcessingStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 214);
            this.Controls.Add(this.comboBox_dynPause);
            this.Controls.Add(this.textBox_errors);
            this.Controls.Add(this.textBox_ProcessingStatusMessage);
            this.Controls.Add(this.button_processingBreak);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ProcessingStatus";
            this.Text = "Статус обработки";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_processingBreak;
        private System.Windows.Forms.TextBox textBox_ProcessingStatusMessage;
        private System.Windows.Forms.TextBox textBox_errors;
        private System.Windows.Forms.ComboBox comboBox_dynPause;
    }
}