namespace FR_Operator.Forms
{
    partial class FormCashierChanger
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
            this.dataGridView_timeDemand = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_timeDemand)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView_timeDemand
            // 
            this.dataGridView_timeDemand.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_timeDemand.Location = new System.Drawing.Point(3, 53);
            this.dataGridView_timeDemand.Name = "dataGridView_timeDemand";
            this.dataGridView_timeDemand.Size = new System.Drawing.Size(477, 389);
            this.dataGridView_timeDemand.TabIndex = 0;
            // 
            // FormCashierChanger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(847, 523);
            this.Controls.Add(this.dataGridView_timeDemand);
            this.Name = "FormCashierChanger";
            this.Text = "Замена кассира";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_timeDemand)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_timeDemand;
    }
}