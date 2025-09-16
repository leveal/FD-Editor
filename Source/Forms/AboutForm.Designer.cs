namespace FR_Operator
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.linkLabel_doc = new System.Windows.Forms.LinkLabel();
            this.linkLabel_update = new System.Windows.Forms.LinkLabel();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(4, 30);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(890, 415);
            this.textBox1.TabIndex = 100;
            this.textBox1.TabStop = false;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // linkLabel_doc
            // 
            this.linkLabel_doc.AutoSize = true;
            this.linkLabel_doc.Location = new System.Drawing.Point(16, 7);
            this.linkLabel_doc.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabel_doc.Name = "linkLabel_doc";
            this.linkLabel_doc.Size = new System.Drawing.Size(127, 13);
            this.linkLabel_doc.TabIndex = 101;
            this.linkLabel_doc.TabStop = true;
            this.linkLabel_doc.Text = "Описание функционала";
            this.linkLabel_doc.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_doc_LinkClicked);
            // 
            // linkLabel_update
            // 
            this.linkLabel_update.AutoSize = true;
            this.linkLabel_update.Location = new System.Drawing.Point(199, 7);
            this.linkLabel_update.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabel_update.Name = "linkLabel_update";
            this.linkLabel_update.Size = new System.Drawing.Size(178, 13);
            this.linkLabel_update.TabIndex = 102;
            this.linkLabel_update.TabStop = true;
            this.linkLabel_update.Text = "Проверить наличие новой версии";
            this.linkLabel_update.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_doc_LinkClicked);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(683, 7);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(171, 20);
            this.textBox2.TabIndex = 105;
            this.textBox2.Text = "5536 9141 0937 9473";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(542, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 106;
            this.label1.Text = "Отблагодарить автора";
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.linkLabel_update);
            this.Controls.Add(this.linkLabel_doc);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(996, 502);
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.LinkLabel linkLabel_doc;
        private System.Windows.Forms.LinkLabel linkLabel_update;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
    }
}