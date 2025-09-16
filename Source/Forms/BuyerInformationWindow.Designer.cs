namespace FR_Operator
{
    partial class BuyerInformationWindow
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_BuyerInformationBuyer = new System.Windows.Forms.TextBox();
            this.textBox_BuyerInformationBuyerInn = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_BuyerInformationBuyerBirthday = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_BuyerInformationBuyerCitizenship = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_BuyerInformationBuyerDocumentCode = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_BuyerInformationBuyerDocumentData = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_BuyerInformationBuyerAddress = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBox_alwaysSendBuyerData = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Покупатель";
            // 
            // textBox_BuyerInformationBuyer
            // 
            this.textBox_BuyerInformationBuyer.Location = new System.Drawing.Point(147, 7);
            this.textBox_BuyerInformationBuyer.Name = "textBox_BuyerInformationBuyer";
            this.textBox_BuyerInformationBuyer.Size = new System.Drawing.Size(334, 20);
            this.textBox_BuyerInformationBuyer.TabIndex = 1;
            this.textBox_BuyerInformationBuyer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_BuyerInformationBuyer.TextChanged += new System.EventHandler(this.BuyerInformation_fieldChanged);
            // 
            // textBox_BuyerInformationBuyerInn
            // 
            this.textBox_BuyerInformationBuyerInn.Location = new System.Drawing.Point(341, 37);
            this.textBox_BuyerInformationBuyerInn.Name = "textBox_BuyerInformationBuyerInn";
            this.textBox_BuyerInformationBuyerInn.Size = new System.Drawing.Size(140, 20);
            this.textBox_BuyerInformationBuyerInn.TabIndex = 3;
            this.textBox_BuyerInformationBuyerInn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_BuyerInformationBuyerInn.TextChanged += new System.EventHandler(this.BuyerInformation_fieldChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(311, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Дата рождения покупателя строка в формате ДД.ММ.ГГГГ";
            this.toolTip1.SetToolTip(this.label3, "тег 1243\r\nРеквизит \"дата рождения покупателя (клиента)\" \r\nвключаются в состав ФД " +
        "в случае не включения \r\nв состав ФД реквизита \"ИНН покупателя (клиента)\"");
            // 
            // textBox_BuyerInformationBuyerBirthday
            // 
            this.textBox_BuyerInformationBuyerBirthday.Location = new System.Drawing.Point(341, 67);
            this.textBox_BuyerInformationBuyerBirthday.Name = "textBox_BuyerInformationBuyerBirthday";
            this.textBox_BuyerInformationBuyerBirthday.Size = new System.Drawing.Size(140, 20);
            this.textBox_BuyerInformationBuyerBirthday.TabIndex = 5;
            this.textBox_BuyerInformationBuyerBirthday.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_BuyerInformationBuyerBirthday.TextChanged += new System.EventHandler(this.BuyerInformation_fieldChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(269, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Гражданство покупателя трехзначное целое число ";
            this.toolTip1.SetToolTip(this.label2, "тег 1244\r\nРеквизит \"гражданство\" включается в состав ФД\r\nтолько в случае, если ли" +
        "цо (покупатель, клиент), \r\nне является гражданином РФ");
            // 
            // textBox_BuyerInformationBuyerCitizenship
            // 
            this.textBox_BuyerInformationBuyerCitizenship.Location = new System.Drawing.Point(341, 97);
            this.textBox_BuyerInformationBuyerCitizenship.Name = "textBox_BuyerInformationBuyerCitizenship";
            this.textBox_BuyerInformationBuyerCitizenship.Size = new System.Drawing.Size(139, 20);
            this.textBox_BuyerInformationBuyerCitizenship.TabIndex = 7;
            this.textBox_BuyerInformationBuyerCitizenship.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_BuyerInformationBuyerCitizenship.TextChanged += new System.EventHandler(this.BuyerInformation_fieldChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(267, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "ИНН покупателя(проверяется контрольная сумма)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 130);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(227, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Код документа удостоверяющего личность";
            // 
            // comboBox_BuyerInformationBuyerDocumentCode
            // 
            this.comboBox_BuyerInformationBuyerDocumentCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_BuyerInformationBuyerDocumentCode.FormattingEnabled = true;
            this.comboBox_BuyerInformationBuyerDocumentCode.Items.AddRange(new object[] {
            "",
            "21 Паспорт гр РФ",
            "22 Загранпаспорт гр РФ",
            "26 Временное удостоверение личности гр РФ",
            "27 Свидетельство о рождении гр РФ",
            "28 Иные документы гр РФ",
            "31 Паспорт иностранного гражданина",
            "32 Иные документы ин гр",
            "33 Документ, выданный ин гр лица без гражданства",
            "34 Вид на жительство",
            "35 Разрешение на временное проживание",
            "36",
            "37 Удостоверение беженца",
            "38",
            "40"});
            this.comboBox_BuyerInformationBuyerDocumentCode.Location = new System.Drawing.Point(246, 127);
            this.comboBox_BuyerInformationBuyerDocumentCode.Name = "comboBox_BuyerInformationBuyerDocumentCode";
            this.comboBox_BuyerInformationBuyerDocumentCode.Size = new System.Drawing.Size(234, 21);
            this.comboBox_BuyerInformationBuyerDocumentCode.TabIndex = 10;
            this.comboBox_BuyerInformationBuyerDocumentCode.SelectedIndexChanged += new System.EventHandler(this.BuyerInformation_fieldChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 160);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(105, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Данные документа";
            this.toolTip1.SetToolTip(this.label6, "тег 1246\r\nРеквизит \"данные документа, удостоверяющего личность\"\r\nвключаются в сос" +
        "тав ФД в случае не включения в состав ФД\r\n реквизита \"ИНН покупателя (клиента)\"");
            // 
            // textBox_BuyerInformationBuyerDocumentData
            // 
            this.textBox_BuyerInformationBuyerDocumentData.Location = new System.Drawing.Point(147, 157);
            this.textBox_BuyerInformationBuyerDocumentData.Name = "textBox_BuyerInformationBuyerDocumentData";
            this.textBox_BuyerInformationBuyerDocumentData.Size = new System.Drawing.Size(334, 20);
            this.textBox_BuyerInformationBuyerDocumentData.TabIndex = 12;
            this.textBox_BuyerInformationBuyerDocumentData.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_BuyerInformationBuyerDocumentData.TextChanged += new System.EventHandler(this.BuyerInformation_fieldChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 190);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(99, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Адрес покупателя";
            this.toolTip1.SetToolTip(this.label7, "тег 1254\r\nРеквизит \"адрес покупателя (клиента)\" включается в состав ФД\r\n в случае" +
        " осуществления расчетов между организациями и (или)\r\n индивидуальными предприним" +
        "ателями.");
            // 
            // textBox_BuyerInformationBuyerAddress
            // 
            this.textBox_BuyerInformationBuyerAddress.Location = new System.Drawing.Point(147, 187);
            this.textBox_BuyerInformationBuyerAddress.Name = "textBox_BuyerInformationBuyerAddress";
            this.textBox_BuyerInformationBuyerAddress.Size = new System.Drawing.Size(333, 20);
            this.textBox_BuyerInformationBuyerAddress.TabIndex = 14;
            this.textBox_BuyerInformationBuyerAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_BuyerInformationBuyerAddress.TextChanged += new System.EventHandler(this.BuyerInformation_fieldChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(363, 217);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "Очистить";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 217);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(252, 26);
            this.label8.TabIndex = 16;
            this.label8.Text = "Если шрифт красный, данные не проходят ФЛК\r\nполе не попадет в чек";
            // 
            // checkBox_alwaysSendBuyerData
            // 
            this.checkBox_alwaysSendBuyerData.AutoSize = true;
            this.checkBox_alwaysSendBuyerData.Location = new System.Drawing.Point(16, 252);
            this.checkBox_alwaysSendBuyerData.Name = "checkBox_alwaysSendBuyerData";
            this.checkBox_alwaysSendBuyerData.Size = new System.Drawing.Size(422, 17);
            this.checkBox_alwaysSendBuyerData.TabIndex = 17;
            this.checkBox_alwaysSendBuyerData.Text = "Передавать данные документа покупателя вне зависимости от наличия ИНН";
            this.checkBox_alwaysSendBuyerData.UseVisualStyleBackColor = true;
            this.checkBox_alwaysSendBuyerData.CheckedChanged += new System.EventHandler(this.BuyerInformation_fieldChanged);
            // 
            // BuyerInformationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 281);
            this.Controls.Add(this.checkBox_alwaysSendBuyerData);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox_BuyerInformationBuyerAddress);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox_BuyerInformationBuyerDocumentData);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBox_BuyerInformationBuyerDocumentCode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_BuyerInformationBuyerCitizenship);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_BuyerInformationBuyerBirthday);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_BuyerInformationBuyerInn);
            this.Controls.Add(this.textBox_BuyerInformationBuyer);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(620, 350);
            this.MinimumSize = new System.Drawing.Size(513, 250);
            this.Name = "BuyerInformationWindow";
            this.Text = "Информация о покупателе(данные сохраняются автоматически при заполнении)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_BuyerInformationBuyer;
        private System.Windows.Forms.TextBox textBox_BuyerInformationBuyerInn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_BuyerInformationBuyerBirthday;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_BuyerInformationBuyerCitizenship;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox_BuyerInformationBuyerDocumentCode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_BuyerInformationBuyerDocumentData;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_BuyerInformationBuyerAddress;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBox_alwaysSendBuyerData;
    }
}