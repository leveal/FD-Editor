namespace FR_Operator
{
    partial class FormItemExtendedEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormItemExtendedEditor));
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_itemsName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_itemsQuantity = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_itemsPrice = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_itemsSum = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_itemsProductType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox_itemsPaymentType = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox_itemsTaxRate = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_itemsPaymentAgentDataTransferOperatorInn = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBox_itemsPaymentAgentDataTransferOperatorAddress = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox_itemsPaymentAgentDataTransferOperatorName = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox_itemsPaymentAgentDataPaymentOperatorPhone = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox_itemsPaymentAgentDataPaymentAgentPhone = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox_itemsPaymentAgentDataTransferOperatorPhone = new System.Windows.Forms.TextBox();
            this.textBox_itemsPaymentAgentDataPpaymentAgentOperation = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox_itemsPaymentAgentByProductType = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_itemsProviderDataProviderName = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.textBox_itemsProviderDataProviderPhone = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.textBox_itemsProviderInn = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.textBox_unitMeasure105 = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.comboBox_unitMasure120 = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.textBox_itemsCustomEntryNum = new System.Windows.Forms.TextBox();
            this.textBox_itemsExciseDuty = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.button_addPosition = new System.Windows.Forms.Button();
            this.button_removePosition = new System.Windows.Forms.Button();
            this.dataGridView_items = new System.Windows.Forms.DataGridView();
            this.Items = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBox_calculateSums = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_items)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(116, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Наименование";
            // 
            // textBox_itemsName
            // 
            this.textBox_itemsName.Location = new System.Drawing.Point(213, 2);
            this.textBox_itemsName.Name = "textBox_itemsName";
            this.textBox_itemsName.Size = new System.Drawing.Size(350, 20);
            this.textBox_itemsName.TabIndex = 2;
            this.textBox_itemsName.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(116, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Количество*";
            this.toolTip1.SetToolTip(this.label2, "Если значение не распознано, нулевое или меньше нуля то устанвлиеется 1.0");
            // 
            // textBox_itemsQuantity
            // 
            this.textBox_itemsQuantity.Location = new System.Drawing.Point(213, 24);
            this.textBox_itemsQuantity.Name = "textBox_itemsQuantity";
            this.textBox_itemsQuantity.Size = new System.Drawing.Size(94, 20);
            this.textBox_itemsQuantity.TabIndex = 4;
            this.textBox_itemsQuantity.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(313, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Цена";
            // 
            // textBox_itemsPrice
            // 
            this.textBox_itemsPrice.Location = new System.Drawing.Point(355, 24);
            this.textBox_itemsPrice.Name = "textBox_itemsPrice";
            this.textBox_itemsPrice.Size = new System.Drawing.Size(113, 20);
            this.textBox_itemsPrice.TabIndex = 6;
            this.textBox_itemsPrice.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(483, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Сумма";
            // 
            // textBox_itemsSum
            // 
            this.textBox_itemsSum.Location = new System.Drawing.Point(535, 24);
            this.textBox_itemsSum.Name = "textBox_itemsSum";
            this.textBox_itemsSum.Size = new System.Drawing.Size(113, 20);
            this.textBox_itemsSum.TabIndex = 8;
            this.textBox_itemsSum.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(116, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Признак п.р.";
            // 
            // comboBox_itemsProductType
            // 
            this.comboBox_itemsProductType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_itemsProductType.FormattingEnabled = true;
            this.comboBox_itemsProductType.Items.AddRange(new object[] {
            "0. Не выбрано",
            "1. Товар",
            "2. Подакцизный товар",
            "3. Работа",
            "4. Услуга",
            "5. Ставка азартной игры",
            "6. Выигрыш азартной игры",
            "7. Лотерейный билет",
            "8. Выигрыш лотереи",
            "9. Предоставление РИД",
            "10. Платеж",
            "11. Агеннтское вознаграждение",
            "12. Составной предмет расчета",
            "13. Иной предмет расчета",
            "14. Имущественное право",
            "15. Внереализационный доход",
            "16. Страховые взносы",
            "17. Торговый сбор",
            "18. Курортный сбор",
            "19. Залог",
            "20. Расход",
            "21. Взносы ОПС ИП",
            "22. Взносы ОПС",
            "23. Взносы ОМС ИП",
            "24. Взносы ОМС",
            "25. Взносы ОСС",
            "26. Платеж казино",
            "27. Выдача ДС",
            "28.",
            "29.",
            "30. АТНМ",
            "31. АТМ",
            "32. ТНМ",
            "33. ТМ"});
            this.comboBox_itemsProductType.Location = new System.Drawing.Point(213, 52);
            this.comboBox_itemsProductType.Name = "comboBox_itemsProductType";
            this.comboBox_itemsProductType.Size = new System.Drawing.Size(231, 21);
            this.comboBox_itemsProductType.TabIndex = 509;
            this.comboBox_itemsProductType.SelectedIndexChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(472, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 510;
            this.label6.Text = "Способ расчета";
            // 
            // comboBox_itemsPaymentType
            // 
            this.comboBox_itemsPaymentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_itemsPaymentType.FormattingEnabled = true;
            this.comboBox_itemsPaymentType.Items.AddRange(new object[] {
            "0 - Не выбрано",
            "1 - Предоплата 100%",
            "2 - Частичная предоплата",
            "3 - Аванс",
            "4 - Полный расчет",
            "5 - Частичный расчет и кредит",
            "6 - Передача в кредит",
            "7 - Оплата куредита"});
            this.comboBox_itemsPaymentType.Location = new System.Drawing.Point(572, 52);
            this.comboBox_itemsPaymentType.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_itemsPaymentType.Name = "comboBox_itemsPaymentType";
            this.comboBox_itemsPaymentType.Size = new System.Drawing.Size(226, 21);
            this.comboBox_itemsPaymentType.TabIndex = 511;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(578, 5);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 512;
            this.label7.Text = "Ставка НДС";
            // 
            // comboBox_itemsTaxRate
            // 
            this.comboBox_itemsTaxRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_itemsTaxRate.FormattingEnabled = true;
            this.comboBox_itemsTaxRate.Items.AddRange(new object[] {
            "Не выбрана",
            "НДС 20%",
            "НДС 10%",
            "НДС 20/120",
            "НДС 10/110",
            "НДС 0%",
            "Без НДС",
            "НДС 5%",
            "НДС 7%",
            "НДС 5/105",
            "НДС 7/107",
            "НДС 22%",
            "НДС 22/122"});
            this.comboBox_itemsTaxRate.Location = new System.Drawing.Point(656, 2);
            this.comboBox_itemsTaxRate.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_itemsTaxRate.Name = "comboBox_itemsTaxRate";
            this.comboBox_itemsTaxRate.Size = new System.Drawing.Size(142, 21);
            this.comboBox_itemsTaxRate.TabIndex = 513;
            this.comboBox_itemsTaxRate.SelectedIndexChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_itemsPaymentAgentDataTransferOperatorInn);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.textBox_itemsPaymentAgentDataTransferOperatorAddress);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.textBox_itemsPaymentAgentDataTransferOperatorName);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.textBox_itemsPaymentAgentDataPaymentOperatorPhone);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.textBox_itemsPaymentAgentDataPaymentAgentPhone);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.textBox_itemsPaymentAgentDataTransferOperatorPhone);
            this.groupBox1.Controls.Add(this.textBox_itemsPaymentAgentDataPpaymentAgentOperation);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.textBox_itemsPaymentAgentByProductType);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(105, 73);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(464, 176);
            this.groupBox1.TabIndex = 514;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Агент";
            // 
            // textBox_itemsPaymentAgentDataTransferOperatorInn
            // 
            this.textBox_itemsPaymentAgentDataTransferOperatorInn.Location = new System.Drawing.Point(262, 32);
            this.textBox_itemsPaymentAgentDataTransferOperatorInn.Name = "textBox_itemsPaymentAgentDataTransferOperatorInn";
            this.textBox_itemsPaymentAgentDataTransferOperatorInn.Size = new System.Drawing.Size(196, 20);
            this.textBox_itemsPaymentAgentDataTransferOperatorInn.TabIndex = 15;
            this.textBox_itemsPaymentAgentDataTransferOperatorInn.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(12, 35);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(171, 13);
            this.label15.TabIndex = 14;
            this.label15.Text = "1016 - ИНН оператора перевода";
            // 
            // textBox_itemsPaymentAgentDataTransferOperatorAddress
            // 
            this.textBox_itemsPaymentAgentDataTransferOperatorAddress.Location = new System.Drawing.Point(262, 72);
            this.textBox_itemsPaymentAgentDataTransferOperatorAddress.Name = "textBox_itemsPaymentAgentDataTransferOperatorAddress";
            this.textBox_itemsPaymentAgentDataTransferOperatorAddress.Size = new System.Drawing.Size(196, 20);
            this.textBox_itemsPaymentAgentDataTransferOperatorAddress.TabIndex = 13;
            this.textBox_itemsPaymentAgentDataTransferOperatorAddress.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 75);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(177, 13);
            this.label14.TabIndex = 12;
            this.label14.Text = "1005 - адрес оператора перевода";
            // 
            // textBox_itemsPaymentAgentDataTransferOperatorName
            // 
            this.textBox_itemsPaymentAgentDataTransferOperatorName.Location = new System.Drawing.Point(262, 92);
            this.textBox_itemsPaymentAgentDataTransferOperatorName.Name = "textBox_itemsPaymentAgentDataTransferOperatorName";
            this.textBox_itemsPaymentAgentDataTransferOperatorName.Size = new System.Drawing.Size(196, 20);
            this.textBox_itemsPaymentAgentDataTransferOperatorName.TabIndex = 11;
            this.textBox_itemsPaymentAgentDataTransferOperatorName.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 95);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(221, 13);
            this.label13.TabIndex = 10;
            this.label13.Text = "1026 - наименование оператора перевода";
            // 
            // textBox_itemsPaymentAgentDataPaymentOperatorPhone
            // 
            this.textBox_itemsPaymentAgentDataPaymentOperatorPhone.Location = new System.Drawing.Point(262, 112);
            this.textBox_itemsPaymentAgentDataPaymentOperatorPhone.Name = "textBox_itemsPaymentAgentDataPaymentOperatorPhone";
            this.textBox_itemsPaymentAgentDataPaymentOperatorPhone.Size = new System.Drawing.Size(196, 20);
            this.textBox_itemsPaymentAgentDataPaymentOperatorPhone.TabIndex = 9;
            this.textBox_itemsPaymentAgentDataPaymentOperatorPhone.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 115);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(246, 13);
            this.label12.TabIndex = 8;
            this.label12.Text = "1074 - телефон оператора по приему платежей";
            // 
            // textBox_itemsPaymentAgentDataPaymentAgentPhone
            // 
            this.textBox_itemsPaymentAgentDataPaymentAgentPhone.Location = new System.Drawing.Point(262, 152);
            this.textBox_itemsPaymentAgentDataPaymentAgentPhone.Name = "textBox_itemsPaymentAgentDataPaymentAgentPhone";
            this.textBox_itemsPaymentAgentDataPaymentAgentPhone.Size = new System.Drawing.Size(196, 20);
            this.textBox_itemsPaymentAgentDataPaymentAgentPhone.TabIndex = 7;
            this.textBox_itemsPaymentAgentDataPaymentAgentPhone.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 155);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(183, 13);
            this.label11.TabIndex = 6;
            this.label11.Text = "1073 - телефон платежного агента";
            // 
            // textBox_itemsPaymentAgentDataTransferOperatorPhone
            // 
            this.textBox_itemsPaymentAgentDataTransferOperatorPhone.Location = new System.Drawing.Point(262, 52);
            this.textBox_itemsPaymentAgentDataTransferOperatorPhone.Name = "textBox_itemsPaymentAgentDataTransferOperatorPhone";
            this.textBox_itemsPaymentAgentDataTransferOperatorPhone.Size = new System.Drawing.Size(196, 20);
            this.textBox_itemsPaymentAgentDataTransferOperatorPhone.TabIndex = 5;
            this.textBox_itemsPaymentAgentDataTransferOperatorPhone.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // textBox_itemsPaymentAgentDataPpaymentAgentOperation
            // 
            this.textBox_itemsPaymentAgentDataPpaymentAgentOperation.Location = new System.Drawing.Point(262, 132);
            this.textBox_itemsPaymentAgentDataPpaymentAgentOperation.Name = "textBox_itemsPaymentAgentDataPpaymentAgentOperation";
            this.textBox_itemsPaymentAgentDataPpaymentAgentOperation.Size = new System.Drawing.Size(196, 20);
            this.textBox_itemsPaymentAgentDataPpaymentAgentOperation.TabIndex = 4;
            this.textBox_itemsPaymentAgentDataPpaymentAgentOperation.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 135);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(188, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "1044 - операция платежного агента";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 55);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(190, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "1075 - телефон оператора перевода";
            // 
            // textBox_itemsPaymentAgentByProductType
            // 
            this.textBox_itemsPaymentAgentByProductType.Location = new System.Drawing.Point(262, 12);
            this.textBox_itemsPaymentAgentByProductType.Name = "textBox_itemsPaymentAgentByProductType";
            this.textBox_itemsPaymentAgentByProductType.Size = new System.Drawing.Size(196, 20);
            this.textBox_itemsPaymentAgentByProductType.TabIndex = 1;
            this.toolTip1.SetToolTip(this.textBox_itemsPaymentAgentByProductType, resources.GetString("textBox_itemsPaymentAgentByProductType.ToolTip"));
            this.textBox_itemsPaymentAgentByProductType.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(115, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "1222-Признак агента";
            this.toolTip1.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_itemsProviderDataProviderName);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.textBox_itemsProviderDataProviderPhone);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.textBox_itemsProviderInn);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Location = new System.Drawing.Point(105, 251);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(464, 75);
            this.groupBox2.TabIndex = 515;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Поставщик";
            // 
            // textBox_itemsProviderDataProviderName
            // 
            this.textBox_itemsProviderDataProviderName.Location = new System.Drawing.Point(262, 31);
            this.textBox_itemsProviderDataProviderName.Name = "textBox_itemsProviderDataProviderName";
            this.textBox_itemsProviderDataProviderName.Size = new System.Drawing.Size(196, 20);
            this.textBox_itemsProviderDataProviderName.TabIndex = 21;
            this.textBox_itemsProviderDataProviderName.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(9, 35);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(179, 13);
            this.label18.TabIndex = 20;
            this.label18.Text = "1225 - наименование поставщика";
            // 
            // textBox_itemsProviderDataProviderPhone
            // 
            this.textBox_itemsProviderDataProviderPhone.Location = new System.Drawing.Point(262, 51);
            this.textBox_itemsProviderDataProviderPhone.Name = "textBox_itemsProviderDataProviderPhone";
            this.textBox_itemsProviderDataProviderPhone.Size = new System.Drawing.Size(196, 20);
            this.textBox_itemsProviderDataProviderPhone.TabIndex = 19;
            this.textBox_itemsProviderDataProviderPhone.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(9, 55);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(148, 13);
            this.label17.TabIndex = 18;
            this.label17.Text = "1171 - телефон поставщика";
            // 
            // textBox_itemsProviderInn
            // 
            this.textBox_itemsProviderInn.Location = new System.Drawing.Point(262, 11);
            this.textBox_itemsProviderInn.Name = "textBox_itemsProviderInn";
            this.textBox_itemsProviderInn.Size = new System.Drawing.Size(196, 20);
            this.textBox_itemsProviderInn.TabIndex = 17;
            this.textBox_itemsProviderInn.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 14);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(131, 13);
            this.label16.TabIndex = 16;
            this.label16.Text = "1226 - ИНН Поставщика";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(574, 84);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(184, 13);
            this.label19.TabIndex = 516;
            this.label19.Text = "1197 - Мера только ФФД 1.05, 1.1";
            // 
            // textBox_unitMeasure105
            // 
            this.textBox_unitMeasure105.Location = new System.Drawing.Point(572, 100);
            this.textBox_unitMeasure105.Name = "textBox_unitMeasure105";
            this.textBox_unitMeasure105.Size = new System.Drawing.Size(226, 20);
            this.textBox_unitMeasure105.TabIndex = 517;
            this.textBox_unitMeasure105.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(574, 133);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(157, 13);
            this.label20.TabIndex = 518;
            this.label20.Text = "2108 - Мера только ФФД 1.2";
            // 
            // comboBox_unitMasure120
            // 
            this.comboBox_unitMasure120.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_unitMasure120.FormattingEnabled = true;
            this.comboBox_unitMasure120.Items.AddRange(new object[] {
            "0 шт. или ед.(поштучно или единицами)",
            "10 г(Грамм)",
            "11 кг(Килограмм)",
            "12 т(Тонна)",
            "20 см(Сантиметр)",
            "21 дм(Дециметр)",
            "22 м(Метр)",
            "30 кв.см(Квадратный сантиметр)",
            "31 кв.дм(Квадратный дециметр",
            "32 кв.м(Квадратный метр)",
            "40 мл(Миллилитр)",
            "41 л(Литр)",
            "42 куб. м(Кубический метр)",
            "50 кВт∙ч(Киловатт час)",
            "51 Гкал(Гигакалория)",
            "70 сутки (день)",
            "71 час",
            "72 мин(Минута)",
            "73 с(Секунда)",
            "80 Кбайт",
            "81 Мбайт",
            "82 Гбайт",
            "83 Тбайт",
            "255 Если не подхдодят в предыдущие"});
            this.comboBox_unitMasure120.Location = new System.Drawing.Point(572, 149);
            this.comboBox_unitMasure120.Name = "comboBox_unitMasure120";
            this.comboBox_unitMasure120.Size = new System.Drawing.Size(226, 21);
            this.comboBox_unitMasure120.TabIndex = 519;
            this.comboBox_unitMasure120.SelectedIndexChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(113, 333);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(203, 13);
            this.label21.TabIndex = 520;
            this.label21.Text = "1231 - Номер таможенной декларации";
            // 
            // textBox_itemsCustomEntryNum
            // 
            this.textBox_itemsCustomEntryNum.Location = new System.Drawing.Point(322, 329);
            this.textBox_itemsCustomEntryNum.Name = "textBox_itemsCustomEntryNum";
            this.textBox_itemsCustomEntryNum.Size = new System.Drawing.Size(241, 20);
            this.textBox_itemsCustomEntryNum.TabIndex = 521;
            this.textBox_itemsCustomEntryNum.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // textBox_itemsExciseDuty
            // 
            this.textBox_itemsExciseDuty.Enabled = false;
            this.textBox_itemsExciseDuty.Location = new System.Drawing.Point(656, 181);
            this.textBox_itemsExciseDuty.Name = "textBox_itemsExciseDuty";
            this.textBox_itemsExciseDuty.Size = new System.Drawing.Size(142, 20);
            this.textBox_itemsExciseDuty.TabIndex = 523;
            this.textBox_itemsExciseDuty.TextChanged += new System.EventHandler(this.ItemsFieldsEditActionHandler);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Enabled = false;
            this.label22.Location = new System.Drawing.Point(576, 184);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(71, 13);
            this.label22.TabIndex = 522;
            this.label22.Text = "1229 - Акциз";
            // 
            // button_addPosition
            // 
            this.button_addPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_addPosition.Location = new System.Drawing.Point(0, 366);
            this.button_addPosition.Name = "button_addPosition";
            this.button_addPosition.Size = new System.Drawing.Size(102, 23);
            this.button_addPosition.TabIndex = 524;
            this.button_addPosition.Text = "Добавить";
            this.button_addPosition.UseVisualStyleBackColor = true;
            this.button_addPosition.Click += new System.EventHandler(this.ChangeList);
            // 
            // button_removePosition
            // 
            this.button_removePosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_removePosition.Location = new System.Drawing.Point(0, 390);
            this.button_removePosition.Name = "button_removePosition";
            this.button_removePosition.Size = new System.Drawing.Size(102, 23);
            this.button_removePosition.TabIndex = 525;
            this.button_removePosition.Text = "Удалить";
            this.button_removePosition.UseVisualStyleBackColor = true;
            this.button_removePosition.Click += new System.EventHandler(this.ChangeList);
            // 
            // dataGridView_items
            // 
            this.dataGridView_items.AllowUserToAddRows = false;
            this.dataGridView_items.AllowUserToDeleteRows = false;
            this.dataGridView_items.AllowUserToResizeColumns = false;
            this.dataGridView_items.AllowUserToResizeRows = false;
            this.dataGridView_items.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView_items.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_items.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Items});
            this.dataGridView_items.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_items.MultiSelect = false;
            this.dataGridView_items.Name = "dataGridView_items";
            this.dataGridView_items.RowHeadersWidth = 4;
            this.dataGridView_items.Size = new System.Drawing.Size(102, 360);
            this.dataGridView_items.TabIndex = 526;
            this.dataGridView_items.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_items_RowEnter);
            // 
            // Items
            // 
            this.Items.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Items.HeaderText = "Список 1059-items";
            this.Items.MinimumWidth = 6;
            this.Items.Name = "Items";
            this.Items.ReadOnly = true;
            this.Items.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // checkBox_calculateSums
            // 
            this.checkBox_calculateSums.AutoSize = true;
            this.checkBox_calculateSums.Checked = true;
            this.checkBox_calculateSums.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_calculateSums.Location = new System.Drawing.Point(663, 27);
            this.checkBox_calculateSums.Name = "checkBox_calculateSums";
            this.checkBox_calculateSums.Size = new System.Drawing.Size(120, 17);
            this.checkBox_calculateSums.TabIndex = 527;
            this.checkBox_calculateSums.Text = "Автоподсчет сумм";
            this.checkBox_calculateSums.UseVisualStyleBackColor = true;
            // 
            // FormItemExtendedEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 416);
            this.Controls.Add(this.checkBox_calculateSums);
            this.Controls.Add(this.dataGridView_items);
            this.Controls.Add(this.button_removePosition);
            this.Controls.Add(this.button_addPosition);
            this.Controls.Add(this.textBox_itemsExciseDuty);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.textBox_itemsCustomEntryNum);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.comboBox_unitMasure120);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.textBox_unitMeasure105);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBox_itemsTaxRate);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.comboBox_itemsPaymentType);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBox_itemsProductType);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_itemsSum);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_itemsPrice);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_itemsQuantity);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_itemsName);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(810, 453);
            this.Name = "FormItemExtendedEditor";
            this.Text = "Расширенный редактор предмета расчета";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormItemExtendedEditor_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_items)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_itemsName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_itemsQuantity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_itemsPrice;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_itemsSum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox_itemsProductType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox_itemsPaymentType;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBox_itemsTaxRate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_itemsPaymentAgentByProductType;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox_itemsPaymentAgentDataTransferOperatorPhone;
        private System.Windows.Forms.TextBox textBox_itemsPaymentAgentDataPpaymentAgentOperation;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox_itemsPaymentAgentDataPaymentAgentPhone;
        private System.Windows.Forms.TextBox textBox_itemsPaymentAgentDataTransferOperatorInn;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBox_itemsPaymentAgentDataTransferOperatorAddress;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox_itemsPaymentAgentDataTransferOperatorName;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox_itemsPaymentAgentDataPaymentOperatorPhone;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox_itemsProviderInn;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBox_itemsProviderDataProviderName;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox textBox_itemsProviderDataProviderPhone;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox textBox_unitMeasure105;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox comboBox_unitMasure120;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox textBox_itemsCustomEntryNum;
        private System.Windows.Forms.TextBox textBox_itemsExciseDuty;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button button_addPosition;
        private System.Windows.Forms.Button button_removePosition;
        private System.Windows.Forms.DataGridView dataGridView_items;
        private System.Windows.Forms.DataGridViewTextBoxColumn Items;
        private System.Windows.Forms.CheckBox checkBox_calculateSums;
    }
}