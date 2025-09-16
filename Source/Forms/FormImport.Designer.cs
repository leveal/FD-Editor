namespace FR_Operator
{
    partial class FormImport
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_excelSetting = new System.Windows.Forms.TabPage();
            this.checkBox_changeProductType1to26 = new System.Windows.Forms.CheckBox();
            this.checkBox_requirementData = new System.Windows.Forms.CheckBox();
            this.checkBox_changeProductType2to26 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_indBuyerAddress = new System.Windows.Forms.ComboBox();
            this.comboBox_indBuyerCitizenship = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBox_indBuyerName = new System.Windows.Forms.ComboBox();
            this.comboBox_indBuyerDocData = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_indBuyerDocumentCode = new System.Windows.Forms.ComboBox();
            this.comboBox_indBuyerInn = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox_indBuyerBirthDate = new System.Windows.Forms.ComboBox();
            this.comboBox_checkLinkRowIndex = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_numberStrToRead = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_readFromStr = new System.Windows.Forms.TextBox();
            this.button_openExcel = new System.Windows.Forms.Button();
            this.tabPage_authorize = new System.Windows.Forms.TabPage();
            this.tabPage_createTask = new System.Windows.Forms.TabPage();
            this.textBox_loadPauseMc = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.button_loadAndCreateTask = new System.Windows.Forms.Button();
            this.textBox_taskPath = new System.Windows.Forms.TextBox();
            this.button_selectFolder = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBox_simpleLink = new System.Windows.Forms.TextBox();
            this.button_load = new System.Windows.Forms.Button();
            this.button_brake = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage_excelSetting.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage_createTask.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_excelSetting);
            this.tabControl1.Controls.Add(this.tabPage_authorize);
            this.tabControl1.Controls.Add(this.tabPage_createTask);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 431);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage_excelSetting
            // 
            this.tabPage_excelSetting.Controls.Add(this.checkBox_changeProductType1to26);
            this.tabPage_excelSetting.Controls.Add(this.checkBox_requirementData);
            this.tabPage_excelSetting.Controls.Add(this.checkBox_changeProductType2to26);
            this.tabPage_excelSetting.Controls.Add(this.groupBox1);
            this.tabPage_excelSetting.Controls.Add(this.comboBox_checkLinkRowIndex);
            this.tabPage_excelSetting.Controls.Add(this.label3);
            this.tabPage_excelSetting.Controls.Add(this.label2);
            this.tabPage_excelSetting.Controls.Add(this.textBox_numberStrToRead);
            this.tabPage_excelSetting.Controls.Add(this.label1);
            this.tabPage_excelSetting.Controls.Add(this.textBox_readFromStr);
            this.tabPage_excelSetting.Controls.Add(this.button_openExcel);
            this.tabPage_excelSetting.Location = new System.Drawing.Point(4, 22);
            this.tabPage_excelSetting.Name = "tabPage_excelSetting";
            this.tabPage_excelSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_excelSetting.Size = new System.Drawing.Size(792, 405);
            this.tabPage_excelSetting.TabIndex = 1;
            this.tabPage_excelSetting.Text = "Excel";
            this.tabPage_excelSetting.UseVisualStyleBackColor = true;
            // 
            // checkBox_changeProductType1to26
            // 
            this.checkBox_changeProductType1to26.AutoSize = true;
            this.checkBox_changeProductType1to26.Location = new System.Drawing.Point(17, 265);
            this.checkBox_changeProductType1to26.Name = "checkBox_changeProductType1to26";
            this.checkBox_changeProductType1to26.Size = new System.Drawing.Size(454, 17);
            this.checkBox_changeProductType1to26.TabIndex = 25;
            this.checkBox_changeProductType1to26.Text = "Меняем во втором файле признак предмета расчета  1-Товар на 26-Платеж казино";
            this.checkBox_changeProductType1to26.UseVisualStyleBackColor = true;
            this.checkBox_changeProductType1to26.CheckedChanged += new System.EventHandler(this.ControlsActionHandler);
            // 
            // checkBox_requirementData
            // 
            this.checkBox_requirementData.AutoSize = true;
            this.checkBox_requirementData.Location = new System.Drawing.Point(17, 324);
            this.checkBox_requirementData.Name = "checkBox_requirementData";
            this.checkBox_requirementData.Size = new System.Drawing.Size(321, 30);
            this.checkBox_requirementData.TabIndex = 24;
            this.checkBox_requirementData.Text = "Прервать корректировку при окончании данных\r\n(Если дошли до конца Excel-файла, а " +
    "JSON еще остались)";
            this.checkBox_requirementData.UseVisualStyleBackColor = true;
            this.checkBox_requirementData.CheckedChanged += new System.EventHandler(this.ControlsActionHandler);
            // 
            // checkBox_changeProductType2to26
            // 
            this.checkBox_changeProductType2to26.AutoSize = true;
            this.checkBox_changeProductType2to26.Checked = true;
            this.checkBox_changeProductType2to26.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_changeProductType2to26.Location = new System.Drawing.Point(17, 228);
            this.checkBox_changeProductType2to26.Name = "checkBox_changeProductType2to26";
            this.checkBox_changeProductType2to26.Size = new System.Drawing.Size(459, 17);
            this.checkBox_changeProductType2to26.TabIndex = 23;
            this.checkBox_changeProductType2to26.Text = "Меняем во втором файле признак предмета расчета  4-Услуга на 26-Платеж казино";
            this.checkBox_changeProductType2to26.UseVisualStyleBackColor = true;
            this.checkBox_changeProductType2to26.CheckedChanged += new System.EventHandler(this.ControlsActionHandler);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboBox_indBuyerAddress);
            this.groupBox1.Controls.Add(this.comboBox_indBuyerCitizenship);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.comboBox_indBuyerName);
            this.groupBox1.Controls.Add(this.comboBox_indBuyerDocData);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.comboBox_indBuyerDocumentCode);
            this.groupBox1.Controls.Add(this.comboBox_indBuyerInn);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.comboBox_indBuyerBirthDate);
            this.groupBox1.Location = new System.Drawing.Point(8, 77);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(595, 144);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Данные покупателя (добавятся во второй файл)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "ФИО покупателя";
            // 
            // comboBox_indBuyerAddress
            // 
            this.comboBox_indBuyerAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_indBuyerAddress.FormattingEnabled = true;
            this.comboBox_indBuyerAddress.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "AA",
            "AB",
            "AC",
            "AD",
            "AE",
            "AF",
            "AG",
            "AH",
            "AI",
            "AJ",
            "AK",
            "AL",
            "AM",
            "AN",
            "AO",
            "AP",
            "AQ",
            "AR",
            "AS",
            "AT",
            "AU"});
            this.comboBox_indBuyerAddress.Location = new System.Drawing.Point(489, 81);
            this.comboBox_indBuyerAddress.Name = "comboBox_indBuyerAddress";
            this.comboBox_indBuyerAddress.Size = new System.Drawing.Size(88, 21);
            this.comboBox_indBuyerAddress.TabIndex = 19;
            this.comboBox_indBuyerAddress.SelectedIndexChanged += new System.EventHandler(this.ControlsActionHandler);
            // 
            // comboBox_indBuyerCitizenship
            // 
            this.comboBox_indBuyerCitizenship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_indBuyerCitizenship.FormattingEnabled = true;
            this.comboBox_indBuyerCitizenship.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "AA",
            "AB",
            "AC",
            "AD",
            "AE",
            "AF",
            "AG",
            "AH",
            "AI",
            "AJ",
            "AK",
            "AL",
            "AM",
            "AN",
            "AO",
            "AP",
            "AQ",
            "AR",
            "AS",
            "AT",
            "AU"});
            this.comboBox_indBuyerCitizenship.Location = new System.Drawing.Point(157, 108);
            this.comboBox_indBuyerCitizenship.Name = "comboBox_indBuyerCitizenship";
            this.comboBox_indBuyerCitizenship.Size = new System.Drawing.Size(88, 21);
            this.comboBox_indBuyerCitizenship.TabIndex = 21;
            this.comboBox_indBuyerCitizenship.SelectedIndexChanged += new System.EventHandler(this.ControlsActionHandler);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(302, 84);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(99, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Адрес покупателя";
            // 
            // comboBox_indBuyerName
            // 
            this.comboBox_indBuyerName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_indBuyerName.FormattingEnabled = true;
            this.comboBox_indBuyerName.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "AA",
            "AB",
            "AC",
            "AD",
            "AE",
            "AF",
            "AG",
            "AH",
            "AI",
            "AJ",
            "AK",
            "AL",
            "AM",
            "AN",
            "AO",
            "AP",
            "AQ",
            "AR",
            "AS",
            "AT",
            "AU"});
            this.comboBox_indBuyerName.Location = new System.Drawing.Point(157, 27);
            this.comboBox_indBuyerName.Name = "comboBox_indBuyerName";
            this.comboBox_indBuyerName.Size = new System.Drawing.Size(88, 21);
            this.comboBox_indBuyerName.TabIndex = 9;
            this.comboBox_indBuyerName.SelectedIndexChanged += new System.EventHandler(this.ControlsActionHandler);
            // 
            // comboBox_indBuyerDocData
            // 
            this.comboBox_indBuyerDocData.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_indBuyerDocData.FormattingEnabled = true;
            this.comboBox_indBuyerDocData.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "AA",
            "AB",
            "AC",
            "AD",
            "AE",
            "AF",
            "AG",
            "AH",
            "AI",
            "AJ",
            "AK",
            "AL",
            "AM",
            "AN",
            "AO",
            "AP",
            "AQ",
            "AR",
            "AS",
            "AT",
            "AU"});
            this.comboBox_indBuyerDocData.Location = new System.Drawing.Point(489, 54);
            this.comboBox_indBuyerDocData.Name = "comboBox_indBuyerDocData";
            this.comboBox_indBuyerDocData.Size = new System.Drawing.Size(88, 21);
            this.comboBox_indBuyerDocData.TabIndex = 17;
            this.comboBox_indBuyerDocData.SelectedIndexChanged += new System.EventHandler(this.ControlsActionHandler);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 111);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(135, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Гражданство покупателя";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(302, 57);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(105, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Данные документа";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "ИНН покупателя";
            // 
            // comboBox_indBuyerDocumentCode
            // 
            this.comboBox_indBuyerDocumentCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_indBuyerDocumentCode.FormattingEnabled = true;
            this.comboBox_indBuyerDocumentCode.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "AA",
            "AB",
            "AC",
            "AD",
            "AE",
            "AF",
            "AG",
            "AH",
            "AI",
            "AJ",
            "AK",
            "AL",
            "AM",
            "AN",
            "AO",
            "AP",
            "AQ",
            "AR",
            "AS",
            "AT",
            "AU"});
            this.comboBox_indBuyerDocumentCode.Location = new System.Drawing.Point(488, 27);
            this.comboBox_indBuyerDocumentCode.Name = "comboBox_indBuyerDocumentCode";
            this.comboBox_indBuyerDocumentCode.Size = new System.Drawing.Size(88, 21);
            this.comboBox_indBuyerDocumentCode.TabIndex = 15;
            this.comboBox_indBuyerDocumentCode.SelectedIndexChanged += new System.EventHandler(this.ControlsActionHandler);
            // 
            // comboBox_indBuyerInn
            // 
            this.comboBox_indBuyerInn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_indBuyerInn.FormattingEnabled = true;
            this.comboBox_indBuyerInn.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "AA",
            "AB",
            "AC",
            "AD",
            "AE",
            "AF",
            "AG",
            "AH",
            "AI",
            "AJ",
            "AK",
            "AL",
            "AM",
            "AN",
            "AO",
            "AP",
            "AQ",
            "AR",
            "AS",
            "AT",
            "AU"});
            this.comboBox_indBuyerInn.Location = new System.Drawing.Point(157, 54);
            this.comboBox_indBuyerInn.Name = "comboBox_indBuyerInn";
            this.comboBox_indBuyerInn.Size = new System.Drawing.Size(88, 21);
            this.comboBox_indBuyerInn.TabIndex = 11;
            this.comboBox_indBuyerInn.SelectedIndexChanged += new System.EventHandler(this.ControlsActionHandler);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(302, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(149, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Код документа уд. личность";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 84);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Дата рожд. покупателя";
            // 
            // comboBox_indBuyerBirthDate
            // 
            this.comboBox_indBuyerBirthDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_indBuyerBirthDate.FormattingEnabled = true;
            this.comboBox_indBuyerBirthDate.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "AA",
            "AB",
            "AC",
            "AD",
            "AE",
            "AF",
            "AG",
            "AH",
            "AI",
            "AJ",
            "AK",
            "AL",
            "AM",
            "AN",
            "AO",
            "AP",
            "AQ",
            "AR",
            "AS",
            "AT",
            "AU"});
            this.comboBox_indBuyerBirthDate.Location = new System.Drawing.Point(157, 81);
            this.comboBox_indBuyerBirthDate.Name = "comboBox_indBuyerBirthDate";
            this.comboBox_indBuyerBirthDate.Size = new System.Drawing.Size(88, 21);
            this.comboBox_indBuyerBirthDate.TabIndex = 13;
            this.comboBox_indBuyerBirthDate.SelectedIndexChanged += new System.EventHandler(this.ControlsActionHandler);
            // 
            // comboBox_checkLinkRowIndex
            // 
            this.comboBox_checkLinkRowIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_checkLinkRowIndex.Enabled = false;
            this.comboBox_checkLinkRowIndex.FormattingEnabled = true;
            this.comboBox_checkLinkRowIndex.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "AA",
            "AB",
            "AC",
            "AD",
            "AE",
            "AF",
            "AG",
            "AH",
            "AI",
            "AJ",
            "AK",
            "AL"});
            this.comboBox_checkLinkRowIndex.Location = new System.Drawing.Point(159, 39);
            this.comboBox_checkLinkRowIndex.Name = "comboBox_checkLinkRowIndex";
            this.comboBox_checkLinkRowIndex.Size = new System.Drawing.Size(88, 21);
            this.comboBox_checkLinkRowIndex.TabIndex = 7;
            this.comboBox_checkLinkRowIndex.SelectedIndexChanged += new System.EventHandler(this.ControlsActionHandler);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(8, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Ряд со ссылкой на чек";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(548, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "строк обрабатываем";
            // 
            // textBox_numberStrToRead
            // 
            this.textBox_numberStrToRead.Enabled = false;
            this.textBox_numberStrToRead.Location = new System.Drawing.Point(668, 39);
            this.textBox_numberStrToRead.Name = "textBox_numberStrToRead";
            this.textBox_numberStrToRead.Size = new System.Drawing.Size(72, 20);
            this.textBox_numberStrToRead.TabIndex = 4;
            this.textBox_numberStrToRead.Text = "333";
            this.textBox_numberStrToRead.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(274, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Первая строка с данными";
            // 
            // textBox_readFromStr
            // 
            this.textBox_readFromStr.Location = new System.Drawing.Point(456, 39);
            this.textBox_readFromStr.Name = "textBox_readFromStr";
            this.textBox_readFromStr.Size = new System.Drawing.Size(72, 20);
            this.textBox_readFromStr.TabIndex = 2;
            this.textBox_readFromStr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_readFromStr.TextChanged += new System.EventHandler(this.ControlsActionHandler);
            // 
            // button_openExcel
            // 
            this.button_openExcel.Location = new System.Drawing.Point(9, 11);
            this.button_openExcel.Name = "button_openExcel";
            this.button_openExcel.Size = new System.Drawing.Size(305, 23);
            this.button_openExcel.TabIndex = 0;
            this.button_openExcel.Text = "Открыть эксель данными покупателя";
            this.button_openExcel.UseVisualStyleBackColor = true;
            this.button_openExcel.Click += new System.EventHandler(this.ControlsActionHandler);
            // 
            // tabPage_authorize
            // 
            this.tabPage_authorize.Location = new System.Drawing.Point(4, 22);
            this.tabPage_authorize.Name = "tabPage_authorize";
            this.tabPage_authorize.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_authorize.Size = new System.Drawing.Size(792, 405);
            this.tabPage_authorize.TabIndex = 0;
            this.tabPage_authorize.Text = "Авторизация";
            this.tabPage_authorize.UseVisualStyleBackColor = true;
            // 
            // tabPage_createTask
            // 
            this.tabPage_createTask.Controls.Add(this.textBox_loadPauseMc);
            this.tabPage_createTask.Controls.Add(this.label11);
            this.tabPage_createTask.Controls.Add(this.button_loadAndCreateTask);
            this.tabPage_createTask.Controls.Add(this.textBox_taskPath);
            this.tabPage_createTask.Controls.Add(this.button_selectFolder);
            this.tabPage_createTask.Location = new System.Drawing.Point(4, 22);
            this.tabPage_createTask.Name = "tabPage_createTask";
            this.tabPage_createTask.Size = new System.Drawing.Size(792, 405);
            this.tabPage_createTask.TabIndex = 2;
            this.tabPage_createTask.Text = "Формирование задания";
            this.tabPage_createTask.UseVisualStyleBackColor = true;
            // 
            // textBox_loadPauseMc
            // 
            this.textBox_loadPauseMc.Enabled = false;
            this.textBox_loadPauseMc.Location = new System.Drawing.Point(176, 47);
            this.textBox_loadPauseMc.Name = "textBox_loadPauseMc";
            this.textBox_loadPauseMc.Size = new System.Drawing.Size(100, 20);
            this.textBox_loadPauseMc.TabIndex = 4;
            this.textBox_loadPauseMc.Text = "1500";
            this.textBox_loadPauseMc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Enabled = false;
            this.label11.Location = new System.Drawing.Point(19, 50);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(140, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "Пауза после загрузки(мс)";
            // 
            // button_loadAndCreateTask
            // 
            this.button_loadAndCreateTask.Enabled = false;
            this.button_loadAndCreateTask.Location = new System.Drawing.Point(8, 146);
            this.button_loadAndCreateTask.Name = "button_loadAndCreateTask";
            this.button_loadAndCreateTask.Size = new System.Drawing.Size(282, 23);
            this.button_loadAndCreateTask.TabIndex = 2;
            this.button_loadAndCreateTask.Text = "Приступить к загрузке и формированию задания";
            this.button_loadAndCreateTask.UseVisualStyleBackColor = true;
            this.button_loadAndCreateTask.Click += new System.EventHandler(this.ControlsActionHandler);
            // 
            // textBox_taskPath
            // 
            this.textBox_taskPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_taskPath.Enabled = false;
            this.textBox_taskPath.Location = new System.Drawing.Point(296, 5);
            this.textBox_taskPath.Name = "textBox_taskPath";
            this.textBox_taskPath.Size = new System.Drawing.Size(493, 20);
            this.textBox_taskPath.TabIndex = 1;
            // 
            // button_selectFolder
            // 
            this.button_selectFolder.Enabled = false;
            this.button_selectFolder.Location = new System.Drawing.Point(8, 3);
            this.button_selectFolder.Name = "button_selectFolder";
            this.button_selectFolder.Size = new System.Drawing.Size(282, 23);
            this.button_selectFolder.TabIndex = 0;
            this.button_selectFolder.Text = "Выбрать папку для формирования задания";
            this.button_selectFolder.UseVisualStyleBackColor = true;
            this.button_selectFolder.Click += new System.EventHandler(this.ControlsActionHandler);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(4, 458);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox1.Size = new System.Drawing.Size(792, 218);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // textBox_simpleLink
            // 
            this.textBox_simpleLink.Location = new System.Drawing.Point(4, 432);
            this.textBox_simpleLink.Name = "textBox_simpleLink";
            this.textBox_simpleLink.Size = new System.Drawing.Size(159, 20);
            this.textBox_simpleLink.TabIndex = 0;
            this.textBox_simpleLink.Visible = false;
            // 
            // button_load
            // 
            this.button_load.Location = new System.Drawing.Point(169, 430);
            this.button_load.Name = "button_load";
            this.button_load.Size = new System.Drawing.Size(135, 23);
            this.button_load.TabIndex = 1;
            this.button_load.Text = "button_test";
            this.button_load.UseVisualStyleBackColor = true;
            this.button_load.Visible = false;
            this.button_load.Click += new System.EventHandler(this.ControlsActionHandler);
            // 
            // button_brake
            // 
            this.button_brake.Location = new System.Drawing.Point(310, 430);
            this.button_brake.Name = "button_brake";
            this.button_brake.Size = new System.Drawing.Size(282, 23);
            this.button_brake.TabIndex = 23;
            this.button_brake.Text = "Прервать";
            this.button_brake.UseVisualStyleBackColor = true;
            this.button_brake.Click += new System.EventHandler(this.ControlsActionHandler);
            // 
            // FormImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 676);
            this.Controls.Add(this.button_brake);
            this.Controls.Add(this.button_load);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.textBox_simpleLink);
            this.Controls.Add(this.richTextBox1);
            this.MinimumSize = new System.Drawing.Size(800, 700);
            this.Name = "FormImport";
            this.Text = "Импорт данных";
            this.tabControl1.ResumeLayout(false);
            this.tabPage_excelSetting.ResumeLayout(false);
            this.tabPage_excelSetting.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage_createTask.ResumeLayout(false);
            this.tabPage_createTask.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_authorize;
        private System.Windows.Forms.TabPage tabPage_excelSetting;
        private System.Windows.Forms.Button button_load;
        private System.Windows.Forms.TextBox textBox_simpleLink;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button_openExcel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_numberStrToRead;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_readFromStr;
        private System.Windows.Forms.ComboBox comboBox_checkLinkRowIndex;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBox_indBuyerCitizenship;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboBox_indBuyerAddress;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBox_indBuyerDocData;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBox_indBuyerDocumentCode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBox_indBuyerBirthDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox_indBuyerInn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox_indBuyerName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabPage_createTask;
        private System.Windows.Forms.TextBox textBox_taskPath;
        private System.Windows.Forms.Button button_selectFolder;
        private System.Windows.Forms.Button button_loadAndCreateTask;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox_loadPauseMc;
        private System.Windows.Forms.Button button_brake;
        private System.Windows.Forms.CheckBox checkBox_changeProductType2to26;
        private System.Windows.Forms.CheckBox checkBox_requirementData;
        private System.Windows.Forms.CheckBox checkBox_changeProductType1to26;
    }
}