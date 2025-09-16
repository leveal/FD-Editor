using System.Drawing;
using System.Windows.Forms;

namespace FR_Operator
{
    partial class TerminalUi
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
            this.components = new System.ComponentModel.Container();
            this.tabControlSpace = new System.Windows.Forms.TabControl();
            this.tabPage_connectionParams = new System.Windows.Forms.TabPage();
            this.textBox_fnBriefInfo = new System.Windows.Forms.TextBox();
            this.button_getFullFnInfo = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label_timeModuleShowTime = new System.Windows.Forms.Label();
            this.tabControl_timeModuleMainSwitcher = new System.Windows.Forms.TabControl();
            this.tabPage_useCurrentTime = new System.Windows.Forms.TabPage();
            this.tabPage_shiftedTime = new System.Windows.Forms.TabPage();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label_dtAlarmingT1 = new System.Windows.Forms.Label();
            this.radioButton_timerStop1 = new System.Windows.Forms.RadioButton();
            this.radioButton_timerStart1 = new System.Windows.Forms.RadioButton();
            this.dateTimePicker_setDtForFd = new System.Windows.Forms.DateTimePicker();
            this.tabPage_useLastFd = new System.Windows.Forms.TabPage();
            this.label38 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label_dtAlarmingT2 = new System.Windows.Forms.Label();
            this.numericUpDown_plusMin = new System.Windows.Forms.NumericUpDown();
            this.label39 = new System.Windows.Forms.Label();
            this.radioButton_timerStop2 = new System.Windows.Forms.RadioButton();
            this.radioButton_timerStart2 = new System.Windows.Forms.RadioButton();
            this.button_updateComNames = new System.Windows.Forms.Button();
            this.checkBox_connectConnectionParamsTab = new System.Windows.Forms.CheckBox();
            this.label_settsAvailabilityMsg = new System.Windows.Forms.Label();
            this.textBox_connTimeout = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_connsttsBaudrate = new System.Windows.Forms.ComboBox();
            this.comboBox_connsttsPortName = new System.Windows.Forms.ComboBox();
            this.tabPage_xcg = new System.Windows.Forms.TabPage();
            this.checkBox_terminalDontLock = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button_sampleGetShiftParams = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.button_getArchivedFdInfo = new System.Windows.Forms.Button();
            this.button_getFnExchangeStatus = new System.Windows.Forms.Button();
            this.button_getRegistrationParams = new System.Windows.Forms.Button();
            this.button_requestExpirationDate = new System.Windows.Forms.Button();
            this.button_requestFnNumber = new System.Windows.Forms.Button();
            this.button_fnGetStatus = new System.Windows.Forms.Button();
            this.button_fnInfoGetFfd = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_xcgIgnoreStartSign = new System.Windows.Forms.CheckBox();
            this.button_xcgCrcCcit = new System.Windows.Forms.Button();
            this.checkBox_xcgExtendedLog = new System.Windows.Forms.CheckBox();
            this.checkBox_xcgOpenPort = new System.Windows.Forms.CheckBox();
            this.button_xcgClean = new System.Windows.Forms.Button();
            this.button_xcgSend = new System.Windows.Forms.Button();
            this.comboBox_xcgMsgFormat = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_xcgMessage = new System.Windows.Forms.TextBox();
            this.textBox_xcgRawData = new System.Windows.Forms.TextBox();
            this.tabPage_fnCommands = new System.Windows.Forms.TabPage();
            this.textBox_cconstructorLength = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton_cconstructorDateFormat = new System.Windows.Forms.RadioButton();
            this.radioButton_cconstructorUint40 = new System.Windows.Forms.RadioButton();
            this.radioButton_hexData = new System.Windows.Forms.RadioButton();
            this.button_addData = new System.Windows.Forms.Button();
            this.radioButton_cconstructorDtFormat = new System.Windows.Forms.RadioButton();
            this.textBox_valueAddToConstructor = new System.Windows.Forms.TextBox();
            this.radioButton_cconstructorAsciiStr = new System.Windows.Forms.RadioButton();
            this.radioButton_cconstructorUint32 = new System.Windows.Forms.RadioButton();
            this.radioButton_cconstructorUint16 = new System.Windows.Forms.RadioButton();
            this.radioButton_cconstructorByte = new System.Windows.Forms.RadioButton();
            this.textBox_showCmdParams = new System.Windows.Forms.TextBox();
            this.textBox_outDataConstructor = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label_fnc1 = new System.Windows.Forms.Label();
            this.comboBox_fncCommand = new System.Windows.Forms.ComboBox();
            this.tabPage_FnInfo = new System.Windows.Forms.TabPage();
            this.button_getFullFnInfo2 = new System.Windows.Forms.Button();
            this.richTextBox_fnTechCondition = new System.Windows.Forms.RichTextBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.dataGridView_regParams = new System.Windows.Forms.DataGridView();
            this.Column_tagNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_tagDescriber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_tagValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage_readFd = new System.Windows.Forms.TabPage();
            this.checkBox_getExtraTlvInfo = new System.Windows.Forms.CheckBox();
            this.button_readFdVariant2 = new System.Windows.Forms.Button();
            this.textBox_strHexToParce = new System.Windows.Forms.TextBox();
            this.button_saveFdToFile = new System.Windows.Forms.Button();
            this.button_parceHexString = new System.Windows.Forms.Button();
            this.comboBox_formatToSave = new System.Windows.Forms.ComboBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBox_tagInfoRawData = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.textBox_tagInfoTagDataType = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.textBox_tagInfoTagNumericValue = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.textBox_tagInfoStringPresentation = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.textBox_tagInfoUserFriendlyName = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.textBox_tagInfoFnsName = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.textBox_tagInfoTagNumber = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.checkBox_preferUserFrandly = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.numericUpDown_fdNumber2 = new System.Windows.Forms.NumericUpDown();
            this.treeView_fnReadedContent = new System.Windows.Forms.TreeView();
            this.numericUpDown_fdNumber1 = new System.Windows.Forms.NumericUpDown();
            this.button_readFdTask = new System.Windows.Forms.Button();
            this.tabPage_createFdRules = new System.Windows.Forms.TabPage();
            this.button_fdRulesTabApplyCurrentTable = new System.Windows.Forms.Button();
            this.button_loadTFNR = new System.Windows.Forms.Button();
            this.button_saveTFNCommonFdRules = new System.Windows.Forms.Button();
            this.label31 = new System.Windows.Forms.Label();
            this.comboBox_ruleTabFdTypeChooser = new System.Windows.Forms.ComboBox();
            this.radioButton_ffdRulesSwitcher_4 = new System.Windows.Forms.RadioButton();
            this.radioButton_ffdRulesSwitcher_3 = new System.Windows.Forms.RadioButton();
            this.dataGridView_termFdRules = new System.Windows.Forms.DataGridView();
            this.ColumnTN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_tn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Criticality = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column_sourceData = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column_dataOverride = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.radioButton_ffdRulesSwitcher_2 = new System.Windows.Forms.RadioButton();
            this.tabPage_fdAction = new System.Windows.Forms.TabPage();
            this.button_perfTabItemsConstructor = new System.Windows.Forms.Button();
            this.textBox_perfTabSelectedTNType = new System.Windows.Forms.TextBox();
            this.button_perfTabRemoveTlvElem = new System.Windows.Forms.Button();
            this.button_perfTabAppendTlvElem = new System.Windows.Forms.Button();
            this.textBox_perfTab_ftagUFName = new System.Windows.Forms.TextBox();
            this.treeView_tabPerfStlvMiniConstructor = new System.Windows.Forms.TreeView();
            this.textBox__tabFdToPerf_EditorValueMain = new System.Windows.Forms.TextBox();
            this.textBox_tabFdToPerf_EditorNumberMain = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.labenumt = new System.Windows.Forms.Label();
            this.groupBox_performTabAll = new System.Windows.Forms.GroupBox();
            this.button_itemsEditor = new System.Windows.Forms.Button();
            this.textBox_perfTabDataForClosing = new System.Windows.Forms.RichTextBox();
            this.groupBox_performTabSteps = new System.Windows.Forms.GroupBox();
            this.button_perfTabCancelFd = new System.Windows.Forms.Button();
            this.button_perfTabFinishDocument = new System.Windows.Forms.Button();
            this.button_sendAllRows = new System.Windows.Forms.Button();
            this.button_perfTabSendSingleRow = new System.Windows.Forms.Button();
            this.button_perfTabBeginDocument = new System.Windows.Forms.Button();
            this.button__performTabPerformFast = new System.Windows.Forms.Button();
            this.checkBox_perfTabDataForBegin = new System.Windows.Forms.CheckBox();
            this.richTextBox_perfTabDataForBegin = new System.Windows.Forms.RichTextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.dataGridView_ftagListToPerform = new System.Windows.Forms.DataGridView();
            this.ColumnTagNamber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_tagUfName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label_PerfTabtimeForFd = new System.Windows.Forms.Label();
            this.comboBox_performTabFfdToPerform = new System.Windows.Forms.ComboBox();
            this.label33 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.comboBox_performTabDocType = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.textBox_downMsgLeft = new System.Windows.Forms.TextBox();
            this.textBox_downMidleMsg = new System.Windows.Forms.TextBox();
            this.progressBar_downProgress = new System.Windows.Forms.ProgressBar();
            this.button_brakeOperation = new System.Windows.Forms.Button();
            this.tabControlSpace.SuspendLayout();
            this.tabPage_connectionParams.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabControl_timeModuleMainSwitcher.SuspendLayout();
            this.tabPage_shiftedTime.SuspendLayout();
            this.tabPage_useLastFd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_plusMin)).BeginInit();
            this.tabPage_xcg.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabPage_fnCommands.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage_FnInfo.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_regParams)).BeginInit();
            this.tabPage_readFd.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_fdNumber2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_fdNumber1)).BeginInit();
            this.tabPage_createFdRules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_termFdRules)).BeginInit();
            this.tabPage_fdAction.SuspendLayout();
            this.groupBox_performTabAll.SuspendLayout();
            this.groupBox_performTabSteps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ftagListToPerform)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControlSpace
            // 
            this.tabControlSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlSpace.Controls.Add(this.tabPage_connectionParams);
            this.tabControlSpace.Controls.Add(this.tabPage_xcg);
            this.tabControlSpace.Controls.Add(this.tabPage_fnCommands);
            this.tabControlSpace.Controls.Add(this.tabPage_FnInfo);
            this.tabControlSpace.Controls.Add(this.tabPage_readFd);
            this.tabControlSpace.Controls.Add(this.tabPage_createFdRules);
            this.tabControlSpace.Controls.Add(this.tabPage_fdAction);
            this.tabControlSpace.Location = new System.Drawing.Point(1, 0);
            this.tabControlSpace.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabControlSpace.Name = "tabControlSpace";
            this.tabControlSpace.SelectedIndex = 0;
            this.tabControlSpace.Size = new System.Drawing.Size(832, 486);
            this.tabControlSpace.TabIndex = 0;
            // 
            // tabPage_connectionParams
            // 
            this.tabPage_connectionParams.Controls.Add(this.textBox_fnBriefInfo);
            this.tabPage_connectionParams.Controls.Add(this.button_getFullFnInfo);
            this.tabPage_connectionParams.Controls.Add(this.groupBox7);
            this.tabPage_connectionParams.Controls.Add(this.button_updateComNames);
            this.tabPage_connectionParams.Controls.Add(this.checkBox_connectConnectionParamsTab);
            this.tabPage_connectionParams.Controls.Add(this.label_settsAvailabilityMsg);
            this.tabPage_connectionParams.Controls.Add(this.textBox_connTimeout);
            this.tabPage_connectionParams.Controls.Add(this.label4);
            this.tabPage_connectionParams.Controls.Add(this.label3);
            this.tabPage_connectionParams.Controls.Add(this.label2);
            this.tabPage_connectionParams.Controls.Add(this.comboBox_connsttsBaudrate);
            this.tabPage_connectionParams.Controls.Add(this.comboBox_connsttsPortName);
            this.tabPage_connectionParams.Location = new System.Drawing.Point(4, 22);
            this.tabPage_connectionParams.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage_connectionParams.Name = "tabPage_connectionParams";
            this.tabPage_connectionParams.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage_connectionParams.Size = new System.Drawing.Size(824, 460);
            this.tabPage_connectionParams.TabIndex = 1;
            this.tabPage_connectionParams.Text = "Настройки связи/времени";
            this.tabPage_connectionParams.UseVisualStyleBackColor = true;
            // 
            // textBox_fnBriefInfo
            // 
            this.textBox_fnBriefInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_fnBriefInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_fnBriefInfo.Location = new System.Drawing.Point(173, 144);
            this.textBox_fnBriefInfo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_fnBriefInfo.Multiline = true;
            this.textBox_fnBriefInfo.Name = "textBox_fnBriefInfo";
            this.textBox_fnBriefInfo.ReadOnly = true;
            this.textBox_fnBriefInfo.Size = new System.Drawing.Size(649, 28);
            this.textBox_fnBriefInfo.TabIndex = 11;
            this.textBox_fnBriefInfo.Text = "8710000101589417Эксплуатация ФН с формированием фискальных документов...";
            // 
            // button_getFullFnInfo
            // 
            this.button_getFullFnInfo.Location = new System.Drawing.Point(7, 146);
            this.button_getFullFnInfo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_getFullFnInfo.Name = "button_getFullFnInfo";
            this.button_getFullFnInfo.Size = new System.Drawing.Size(160, 23);
            this.button_getFullFnInfo.TabIndex = 10;
            this.button_getFullFnInfo.Text = "Опросить ФН";
            this.button_getFullFnInfo.UseVisualStyleBackColor = true;
            this.button_getFullFnInfo.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // groupBox7
            // 
            this.groupBox7.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.groupBox7.Controls.Add(this.label_timeModuleShowTime);
            this.groupBox7.Controls.Add(this.tabControl_timeModuleMainSwitcher);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox7.Location = new System.Drawing.Point(2, 290);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox7.Size = new System.Drawing.Size(820, 168);
            this.groupBox7.TabIndex = 9;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Время для формируемых ФД";
            // 
            // label_timeModuleShowTime
            // 
            this.label_timeModuleShowTime.AutoSize = true;
            this.label_timeModuleShowTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_timeModuleShowTime.Location = new System.Drawing.Point(4, 18);
            this.label_timeModuleShowTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_timeModuleShowTime.Name = "label_timeModuleShowTime";
            this.label_timeModuleShowTime.Size = new System.Drawing.Size(193, 25);
            this.label_timeModuleShowTime.TabIndex = 1;
            this.label_timeModuleShowTime.Text = "15.09.2024 22:22:22";
            // 
            // tabControl_timeModuleMainSwitcher
            // 
            this.tabControl_timeModuleMainSwitcher.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl_timeModuleMainSwitcher.Controls.Add(this.tabPage_useCurrentTime);
            this.tabControl_timeModuleMainSwitcher.Controls.Add(this.tabPage_shiftedTime);
            this.tabControl_timeModuleMainSwitcher.Controls.Add(this.tabPage_useLastFd);
            this.tabControl_timeModuleMainSwitcher.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl_timeModuleMainSwitcher.Location = new System.Drawing.Point(2, 45);
            this.tabControl_timeModuleMainSwitcher.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabControl_timeModuleMainSwitcher.Name = "tabControl_timeModuleMainSwitcher";
            this.tabControl_timeModuleMainSwitcher.SelectedIndex = 0;
            this.tabControl_timeModuleMainSwitcher.Size = new System.Drawing.Size(816, 121);
            this.tabControl_timeModuleMainSwitcher.TabIndex = 0;
            this.tabControl_timeModuleMainSwitcher.SelectedIndexChanged += new System.EventHandler(this.TimeSettingChanging);
            this.tabControl_timeModuleMainSwitcher.TabIndexChanged += new System.EventHandler(this.TimeSettingChanging);
            // 
            // tabPage_useCurrentTime
            // 
            this.tabPage_useCurrentTime.Location = new System.Drawing.Point(4, 25);
            this.tabPage_useCurrentTime.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage_useCurrentTime.Name = "tabPage_useCurrentTime";
            this.tabPage_useCurrentTime.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage_useCurrentTime.Size = new System.Drawing.Size(808, 92);
            this.tabPage_useCurrentTime.TabIndex = 0;
            this.tabPage_useCurrentTime.Text = "Текущее время ПК";
            this.tabPage_useCurrentTime.UseVisualStyleBackColor = true;
            // 
            // tabPage_shiftedTime
            // 
            this.tabPage_shiftedTime.Controls.Add(this.textBox2);
            this.tabPage_shiftedTime.Controls.Add(this.label_dtAlarmingT1);
            this.tabPage_shiftedTime.Controls.Add(this.radioButton_timerStop1);
            this.tabPage_shiftedTime.Controls.Add(this.radioButton_timerStart1);
            this.tabPage_shiftedTime.Controls.Add(this.dateTimePicker_setDtForFd);
            this.tabPage_shiftedTime.Location = new System.Drawing.Point(4, 25);
            this.tabPage_shiftedTime.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage_shiftedTime.Name = "tabPage_shiftedTime";
            this.tabPage_shiftedTime.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage_shiftedTime.Size = new System.Drawing.Size(809, 92);
            this.tabPage_shiftedTime.TabIndex = 1;
            this.tabPage_shiftedTime.Text = "Смещенное настраеваемое";
            this.tabPage_shiftedTime.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(500, 73);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(259, 20);
            this.textBox2.TabIndex = 10;
            this.textBox2.Text = "min_allowed_fd_time=01.01.2020";
            // 
            // label_dtAlarmingT1
            // 
            this.label_dtAlarmingT1.AutoSize = true;
            this.label_dtAlarmingT1.Location = new System.Drawing.Point(14, 65);
            this.label_dtAlarmingT1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_dtAlarmingT1.Name = "label_dtAlarmingT1";
            this.label_dtAlarmingT1.Size = new System.Drawing.Size(475, 26);
            this.label_dtAlarmingT1.TabIndex = 9;
            this.label_dtAlarmingT1.Text = "Минимальное разрешенное время для формирования ФД 12.12.2012 для изменения \r\nдоба" +
    "вьте строку приведенную ниже в конфигурационный файл и перезапустите программу";
            // 
            // radioButton_timerStop1
            // 
            this.radioButton_timerStop1.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_timerStop1.AutoSize = true;
            this.radioButton_timerStop1.Checked = true;
            this.radioButton_timerStop1.Location = new System.Drawing.Point(423, 38);
            this.radioButton_timerStop1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButton_timerStop1.Name = "radioButton_timerStop1";
            this.radioButton_timerStop1.Size = new System.Drawing.Size(146, 23);
            this.radioButton_timerStop1.TabIndex = 2;
            this.radioButton_timerStop1.TabStop = true;
            this.radioButton_timerStop1.Text = "Запуск при обмене с ФН";
            this.radioButton_timerStop1.UseVisualStyleBackColor = true;
            this.radioButton_timerStop1.CheckedChanged += new System.EventHandler(this.TimeSettingChanging);
            // 
            // radioButton_timerStart1
            // 
            this.radioButton_timerStart1.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_timerStart1.AutoSize = true;
            this.radioButton_timerStart1.Location = new System.Drawing.Point(308, 38);
            this.radioButton_timerStart1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButton_timerStart1.Name = "radioButton_timerStart1";
            this.radioButton_timerStart1.Size = new System.Drawing.Size(107, 23);
            this.radioButton_timerStart1.TabIndex = 1;
            this.radioButton_timerStart1.Text = "Запустить сейчас";
            this.radioButton_timerStart1.UseVisualStyleBackColor = true;
            this.radioButton_timerStart1.CheckedChanged += new System.EventHandler(this.TimeSettingChanging);
            // 
            // dateTimePicker_setDtForFd
            // 
            this.dateTimePicker_setDtForFd.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.dateTimePicker_setDtForFd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dateTimePicker_setDtForFd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_setDtForFd.Location = new System.Drawing.Point(16, 35);
            this.dateTimePicker_setDtForFd.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dateTimePicker_setDtForFd.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker_setDtForFd.Name = "dateTimePicker_setDtForFd";
            this.dateTimePicker_setDtForFd.Size = new System.Drawing.Size(194, 26);
            this.dateTimePicker_setDtForFd.TabIndex = 0;
            this.dateTimePicker_setDtForFd.ValueChanged += new System.EventHandler(this.TimeSettingChanging);
            // 
            // tabPage_useLastFd
            // 
            this.tabPage_useLastFd.Controls.Add(this.label38);
            this.tabPage_useLastFd.Controls.Add(this.textBox1);
            this.tabPage_useLastFd.Controls.Add(this.label_dtAlarmingT2);
            this.tabPage_useLastFd.Controls.Add(this.numericUpDown_plusMin);
            this.tabPage_useLastFd.Controls.Add(this.label39);
            this.tabPage_useLastFd.Controls.Add(this.radioButton_timerStop2);
            this.tabPage_useLastFd.Controls.Add(this.radioButton_timerStart2);
            this.tabPage_useLastFd.Location = new System.Drawing.Point(4, 25);
            this.tabPage_useLastFd.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage_useLastFd.Name = "tabPage_useLastFd";
            this.tabPage_useLastFd.Size = new System.Drawing.Size(809, 92);
            this.tabPage_useLastFd.TabIndex = 2;
            this.tabPage_useLastFd.Text = "Использовать данные  о последнем ФД";
            this.tabPage_useLastFd.UseVisualStyleBackColor = true;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(16, 4);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(448, 26);
            this.label38.TabIndex = 9;
            this.label38.Text = "Для использования этой настройки необходимо чтобы был выполнен запрос  статуса\r\nЕ" +
    "сли запроса статуса не было будет использовано текущее время";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(500, 73);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(259, 20);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = "min_allowed_fd_time=01.01.2020";
            // 
            // label_dtAlarmingT2
            // 
            this.label_dtAlarmingT2.AutoSize = true;
            this.label_dtAlarmingT2.Location = new System.Drawing.Point(14, 65);
            this.label_dtAlarmingT2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_dtAlarmingT2.Name = "label_dtAlarmingT2";
            this.label_dtAlarmingT2.Size = new System.Drawing.Size(475, 26);
            this.label_dtAlarmingT2.TabIndex = 7;
            this.label_dtAlarmingT2.Text = "Минимальное разрешенное время для формирования ФД 12.12.2012 для изменения \r\nдоба" +
    "вьте строку приведенную ниже в конфигурационный файл и перезапустите программу";
            // 
            // numericUpDown_plusMin
            // 
            this.numericUpDown_plusMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.numericUpDown_plusMin.Location = new System.Drawing.Point(16, 35);
            this.numericUpDown_plusMin.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.numericUpDown_plusMin.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_plusMin.Name = "numericUpDown_plusMin";
            this.numericUpDown_plusMin.Size = new System.Drawing.Size(70, 26);
            this.numericUpDown_plusMin.TabIndex = 6;
            this.numericUpDown_plusMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_plusMin.ValueChanged += new System.EventHandler(this.TimeSettingChanging);
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(94, 43);
            this.label39.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(193, 13);
            this.label39.TabIndex = 5;
            this.label39.Text = "Сдвиг в минутах от последнего ФД*";
            this.toolTip1.SetToolTip(this.label39, "Для определения времени последнего ФД\r\nДолжен быть выполнен запрос статуса ФН");
            // 
            // radioButton_timerStop2
            // 
            this.radioButton_timerStop2.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_timerStop2.AutoSize = true;
            this.radioButton_timerStop2.Checked = true;
            this.radioButton_timerStop2.Location = new System.Drawing.Point(423, 38);
            this.radioButton_timerStop2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButton_timerStop2.Name = "radioButton_timerStop2";
            this.radioButton_timerStop2.Size = new System.Drawing.Size(146, 23);
            this.radioButton_timerStop2.TabIndex = 4;
            this.radioButton_timerStop2.TabStop = true;
            this.radioButton_timerStop2.Text = "Запуск при обмене с ФН";
            this.radioButton_timerStop2.UseVisualStyleBackColor = true;
            this.radioButton_timerStop2.CheckedChanged += new System.EventHandler(this.TimeSettingChanging);
            // 
            // radioButton_timerStart2
            // 
            this.radioButton_timerStart2.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_timerStart2.AutoSize = true;
            this.radioButton_timerStart2.Location = new System.Drawing.Point(308, 38);
            this.radioButton_timerStart2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButton_timerStart2.Name = "radioButton_timerStart2";
            this.radioButton_timerStart2.Size = new System.Drawing.Size(107, 23);
            this.radioButton_timerStart2.TabIndex = 3;
            this.radioButton_timerStart2.Text = "Запустить сейчас";
            this.radioButton_timerStart2.UseVisualStyleBackColor = true;
            this.radioButton_timerStart2.CheckedChanged += new System.EventHandler(this.TimeSettingChanging);
            // 
            // button_updateComNames
            // 
            this.button_updateComNames.Location = new System.Drawing.Point(8, 79);
            this.button_updateComNames.Name = "button_updateComNames";
            this.button_updateComNames.Size = new System.Drawing.Size(160, 23);
            this.button_updateComNames.TabIndex = 8;
            this.button_updateComNames.Text = "обновить список портов";
            this.button_updateComNames.UseVisualStyleBackColor = true;
            this.button_updateComNames.Click += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // checkBox_connectConnectionParamsTab
            // 
            this.checkBox_connectConnectionParamsTab.AutoSize = true;
            this.checkBox_connectConnectionParamsTab.Location = new System.Drawing.Point(8, 111);
            this.checkBox_connectConnectionParamsTab.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBox_connectConnectionParamsTab.Name = "checkBox_connectConnectionParamsTab";
            this.checkBox_connectConnectionParamsTab.Size = new System.Drawing.Size(73, 17);
            this.checkBox_connectConnectionParamsTab.TabIndex = 7;
            this.checkBox_connectConnectionParamsTab.Text = "Open port";
            this.checkBox_connectConnectionParamsTab.UseVisualStyleBackColor = true;
            this.checkBox_connectConnectionParamsTab.CheckedChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // label_settsAvailabilityMsg
            // 
            this.label_settsAvailabilityMsg.AutoSize = true;
            this.label_settsAvailabilityMsg.Location = new System.Drawing.Point(186, 10);
            this.label_settsAvailabilityMsg.Name = "label_settsAvailabilityMsg";
            this.label_settsAvailabilityMsg.Size = new System.Drawing.Size(126, 13);
            this.label_settsAvailabilityMsg.TabIndex = 6;
            this.label_settsAvailabilityMsg.Text = "label_settsAvailabilityMsg";
            // 
            // textBox_connTimeout
            // 
            this.textBox_connTimeout.Location = new System.Drawing.Point(76, 54);
            this.textBox_connTimeout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_connTimeout.Name = "textBox_connTimeout";
            this.textBox_connTimeout.Size = new System.Drawing.Size(92, 20);
            this.textBox_connTimeout.TabIndex = 5;
            this.textBox_connTimeout.Text = "1000";
            this.textBox_connTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_connTimeout.TextChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 56);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "TIMEOUT";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 35);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "BAUDRATE";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "PORT";
            // 
            // comboBox_connsttsBaudrate
            // 
            this.comboBox_connsttsBaudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_connsttsBaudrate.FormattingEnabled = true;
            this.comboBox_connsttsBaudrate.Items.AddRange(new object[] {
            "1200",
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "28800",
            "38400",
            "57600",
            "76800",
            "115200",
            "230400",
            "460800",
            "921600"});
            this.comboBox_connsttsBaudrate.Location = new System.Drawing.Point(76, 30);
            this.comboBox_connsttsBaudrate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox_connsttsBaudrate.Name = "comboBox_connsttsBaudrate";
            this.comboBox_connsttsBaudrate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.comboBox_connsttsBaudrate.Size = new System.Drawing.Size(92, 21);
            this.comboBox_connsttsBaudrate.TabIndex = 1;
            this.comboBox_connsttsBaudrate.SelectedIndexChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // comboBox_connsttsPortName
            // 
            this.comboBox_connsttsPortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_connsttsPortName.FormattingEnabled = true;
            this.comboBox_connsttsPortName.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10"});
            this.comboBox_connsttsPortName.Location = new System.Drawing.Point(76, 6);
            this.comboBox_connsttsPortName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox_connsttsPortName.Name = "comboBox_connsttsPortName";
            this.comboBox_connsttsPortName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.comboBox_connsttsPortName.Size = new System.Drawing.Size(92, 21);
            this.comboBox_connsttsPortName.TabIndex = 0;
            this.comboBox_connsttsPortName.SelectedIndexChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // tabPage_xcg
            // 
            this.tabPage_xcg.Controls.Add(this.checkBox_terminalDontLock);
            this.tabPage_xcg.Controls.Add(this.groupBox4);
            this.tabPage_xcg.Controls.Add(this.groupBox1);
            this.tabPage_xcg.Controls.Add(this.checkBox_xcgExtendedLog);
            this.tabPage_xcg.Controls.Add(this.checkBox_xcgOpenPort);
            this.tabPage_xcg.Controls.Add(this.button_xcgClean);
            this.tabPage_xcg.Controls.Add(this.button_xcgSend);
            this.tabPage_xcg.Controls.Add(this.comboBox_xcgMsgFormat);
            this.tabPage_xcg.Controls.Add(this.label1);
            this.tabPage_xcg.Controls.Add(this.textBox_xcgMessage);
            this.tabPage_xcg.Controls.Add(this.textBox_xcgRawData);
            this.tabPage_xcg.Location = new System.Drawing.Point(4, 22);
            this.tabPage_xcg.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage_xcg.Name = "tabPage_xcg";
            this.tabPage_xcg.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage_xcg.Size = new System.Drawing.Size(824, 460);
            this.tabPage_xcg.TabIndex = 0;
            this.tabPage_xcg.Text = "Terminal";
            this.tabPage_xcg.UseVisualStyleBackColor = true;
            // 
            // checkBox_terminalDontLock
            // 
            this.checkBox_terminalDontLock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_terminalDontLock.AutoSize = true;
            this.checkBox_terminalDontLock.Location = new System.Drawing.Point(122, 359);
            this.checkBox_terminalDontLock.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBox_terminalDontLock.Name = "checkBox_terminalDontLock";
            this.checkBox_terminalDontLock.Size = new System.Drawing.Size(161, 17);
            this.checkBox_terminalDontLock.TabIndex = 12;
            this.checkBox_terminalDontLock.Text = "Не блокировать отправку*";
            this.toolTip1.SetToolTip(this.checkBox_terminalDontLock, "Для частичных запроссов.\r\nБез этой галочки после посылки команды \r\nuart блокирует" +
        "ся на отправку до получения ответа\r\nили истечения таймаута");
            this.checkBox_terminalDontLock.UseVisualStyleBackColor = true;
            this.checkBox_terminalDontLock.CheckedChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox4.Controls.Add(this.button_sampleGetShiftParams);
            this.groupBox4.Controls.Add(this.numericUpDown1);
            this.groupBox4.Controls.Add(this.button_getArchivedFdInfo);
            this.groupBox4.Controls.Add(this.button_getFnExchangeStatus);
            this.groupBox4.Controls.Add(this.button_getRegistrationParams);
            this.groupBox4.Controls.Add(this.button_requestExpirationDate);
            this.groupBox4.Controls.Add(this.button_requestFnNumber);
            this.groupBox4.Controls.Add(this.button_fnGetStatus);
            this.groupBox4.Controls.Add(this.button_fnInfoGetFfd);
            this.groupBox4.Location = new System.Drawing.Point(291, 355);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox4.Size = new System.Drawing.Size(499, 91);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Тестовые запросы";
            // 
            // button_sampleGetShiftParams
            // 
            this.button_sampleGetShiftParams.Location = new System.Drawing.Point(292, 64);
            this.button_sampleGetShiftParams.Name = "button_sampleGetShiftParams";
            this.button_sampleGetShiftParams.Size = new System.Drawing.Size(134, 23);
            this.button_sampleGetShiftParams.TabIndex = 19;
            this.button_sampleGetShiftParams.Text = "Параметры смены";
            this.button_sampleGetShiftParams.UseVisualStyleBackColor = true;
            this.button_sampleGetShiftParams.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.numericUpDown1.Location = new System.Drawing.Point(429, 15);
            this.numericUpDown1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(65, 20);
            this.numericUpDown1.TabIndex = 18;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Visible = false;
            // 
            // button_getArchivedFdInfo
            // 
            this.button_getArchivedFdInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_getArchivedFdInfo.Location = new System.Drawing.Point(286, 13);
            this.button_getArchivedFdInfo.Name = "button_getArchivedFdInfo";
            this.button_getArchivedFdInfo.Size = new System.Drawing.Size(140, 23);
            this.button_getArchivedFdInfo.TabIndex = 17;
            this.button_getArchivedFdInfo.Text = "Запрос ФД из архива";
            this.button_getArchivedFdInfo.UseVisualStyleBackColor = true;
            this.button_getArchivedFdInfo.Visible = false;
            // 
            // button_getFnExchangeStatus
            // 
            this.button_getFnExchangeStatus.Location = new System.Drawing.Point(145, 64);
            this.button_getFnExchangeStatus.Name = "button_getFnExchangeStatus";
            this.button_getFnExchangeStatus.Size = new System.Drawing.Size(140, 23);
            this.button_getFnExchangeStatus.TabIndex = 16;
            this.button_getFnExchangeStatus.Text = "Статус обмена с ОФД";
            this.button_getFnExchangeStatus.UseVisualStyleBackColor = true;
            this.button_getFnExchangeStatus.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // button_getRegistrationParams
            // 
            this.button_getRegistrationParams.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_getRegistrationParams.Location = new System.Drawing.Point(145, 39);
            this.button_getRegistrationParams.Name = "button_getRegistrationParams";
            this.button_getRegistrationParams.Size = new System.Drawing.Size(140, 23);
            this.button_getRegistrationParams.TabIndex = 15;
            this.button_getRegistrationParams.Text = "Рег.  параметры*";
            this.toolTip1.SetToolTip(this.button_getRegistrationParams, "Если не было запроса срока действия ФН,\r\nто сперва будет выполнен он для получени" +
        "я \r\nномера последней регистрации\r\n");
            this.button_getRegistrationParams.UseVisualStyleBackColor = true;
            this.button_getRegistrationParams.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // button_requestExpirationDate
            // 
            this.button_requestExpirationDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_requestExpirationDate.Location = new System.Drawing.Point(145, 13);
            this.button_requestExpirationDate.Name = "button_requestExpirationDate";
            this.button_requestExpirationDate.Size = new System.Drawing.Size(140, 23);
            this.button_requestExpirationDate.TabIndex = 14;
            this.button_requestExpirationDate.Text = "Срок действия ФН";
            this.button_requestExpirationDate.UseVisualStyleBackColor = true;
            this.button_requestExpirationDate.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // button_requestFnNumber
            // 
            this.button_requestFnNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_requestFnNumber.Location = new System.Drawing.Point(3, 13);
            this.button_requestFnNumber.Name = "button_requestFnNumber";
            this.button_requestFnNumber.Size = new System.Drawing.Size(140, 23);
            this.button_requestFnNumber.TabIndex = 10;
            this.button_requestFnNumber.Text = "Номер ФН";
            this.button_requestFnNumber.UseVisualStyleBackColor = true;
            this.button_requestFnNumber.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // button_fnGetStatus
            // 
            this.button_fnGetStatus.Location = new System.Drawing.Point(3, 64);
            this.button_fnGetStatus.Name = "button_fnGetStatus";
            this.button_fnGetStatus.Size = new System.Drawing.Size(140, 23);
            this.button_fnGetStatus.TabIndex = 12;
            this.button_fnGetStatus.Text = "Статус ФН";
            this.button_fnGetStatus.UseVisualStyleBackColor = true;
            this.button_fnGetStatus.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // button_fnInfoGetFfd
            // 
            this.button_fnInfoGetFfd.Location = new System.Drawing.Point(3, 39);
            this.button_fnInfoGetFfd.Name = "button_fnInfoGetFfd";
            this.button_fnInfoGetFfd.Size = new System.Drawing.Size(140, 23);
            this.button_fnInfoGetFfd.TabIndex = 13;
            this.button_fnInfoGetFfd.Text = "Формат ФН";
            this.button_fnInfoGetFfd.UseVisualStyleBackColor = true;
            this.button_fnInfoGetFfd.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.checkBox_xcgIgnoreStartSign);
            this.groupBox1.Controls.Add(this.button_xcgCrcCcit);
            this.groupBox1.Location = new System.Drawing.Point(7, 381);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(187, 38);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CRC ITCC";
            // 
            // checkBox_xcgIgnoreStartSign
            // 
            this.checkBox_xcgIgnoreStartSign.AutoSize = true;
            this.checkBox_xcgIgnoreStartSign.Checked = true;
            this.checkBox_xcgIgnoreStartSign.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_xcgIgnoreStartSign.Location = new System.Drawing.Point(83, 14);
            this.checkBox_xcgIgnoreStartSign.Name = "checkBox_xcgIgnoreStartSign";
            this.checkBox_xcgIgnoreStartSign.Size = new System.Drawing.Size(101, 17);
            this.checkBox_xcgIgnoreStartSign.TabIndex = 9;
            this.checkBox_xcgIgnoreStartSign.Text = "Ignore start sign";
            this.toolTip1.SetToolTip(this.checkBox_xcgIgnoreStartSign, "Признак начала сообщения ФН - 1 байт = \"04\"");
            this.checkBox_xcgIgnoreStartSign.UseVisualStyleBackColor = true;
            // 
            // button_xcgCrcCcit
            // 
            this.button_xcgCrcCcit.Location = new System.Drawing.Point(5, 13);
            this.button_xcgCrcCcit.Name = "button_xcgCrcCcit";
            this.button_xcgCrcCcit.Size = new System.Drawing.Size(64, 20);
            this.button_xcgCrcCcit.TabIndex = 8;
            this.button_xcgCrcCcit.Text = "ADD CRC CCIT";
            this.button_xcgCrcCcit.UseVisualStyleBackColor = true;
            this.button_xcgCrcCcit.Click += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // checkBox_xcgExtendedLog
            // 
            this.checkBox_xcgExtendedLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_xcgExtendedLog.AutoSize = true;
            this.checkBox_xcgExtendedLog.Location = new System.Drawing.Point(7, 428);
            this.checkBox_xcgExtendedLog.Name = "checkBox_xcgExtendedLog";
            this.checkBox_xcgExtendedLog.Size = new System.Drawing.Size(137, 17);
            this.checkBox_xcgExtendedLog.TabIndex = 7;
            this.checkBox_xcgExtendedLog.Text = "Расширенный журнал";
            this.checkBox_xcgExtendedLog.UseVisualStyleBackColor = true;
            this.checkBox_xcgExtendedLog.CheckedChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // checkBox_xcgOpenPort
            // 
            this.checkBox_xcgOpenPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_xcgOpenPort.AutoSize = true;
            this.checkBox_xcgOpenPort.Location = new System.Drawing.Point(210, 399);
            this.checkBox_xcgOpenPort.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBox_xcgOpenPort.Name = "checkBox_xcgOpenPort";
            this.checkBox_xcgOpenPort.Size = new System.Drawing.Size(73, 17);
            this.checkBox_xcgOpenPort.TabIndex = 6;
            this.checkBox_xcgOpenPort.Text = "Open port";
            this.checkBox_xcgOpenPort.UseVisualStyleBackColor = true;
            this.checkBox_xcgOpenPort.CheckedChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // button_xcgClean
            // 
            this.button_xcgClean.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_xcgClean.Location = new System.Drawing.Point(164, 422);
            this.button_xcgClean.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_xcgClean.Name = "button_xcgClean";
            this.button_xcgClean.Size = new System.Drawing.Size(112, 24);
            this.button_xcgClean.TabIndex = 5;
            this.button_xcgClean.Text = "Очистить историю";
            this.button_xcgClean.UseVisualStyleBackColor = true;
            this.button_xcgClean.Click += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // button_xcgSend
            // 
            this.button_xcgSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_xcgSend.Location = new System.Drawing.Point(0, 355);
            this.button_xcgSend.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_xcgSend.Name = "button_xcgSend";
            this.button_xcgSend.Size = new System.Drawing.Size(112, 24);
            this.button_xcgSend.TabIndex = 4;
            this.button_xcgSend.Text = "send";
            this.button_xcgSend.UseVisualStyleBackColor = true;
            this.button_xcgSend.Click += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // comboBox_xcgMsgFormat
            // 
            this.comboBox_xcgMsgFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBox_xcgMsgFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_xcgMsgFormat.FormattingEnabled = true;
            this.comboBox_xcgMsgFormat.Items.AddRange(new object[] {
            "HEX: FF-FF..."});
            this.comboBox_xcgMsgFormat.Location = new System.Drawing.Point(1, 332);
            this.comboBox_xcgMsgFormat.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox_xcgMsgFormat.Name = "comboBox_xcgMsgFormat";
            this.comboBox_xcgMsgFormat.Size = new System.Drawing.Size(144, 21);
            this.comboBox_xcgMsgFormat.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label1.Location = new System.Drawing.Point(147, 331);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.label1.Size = new System.Drawing.Size(39, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "MSG";
            // 
            // textBox_xcgMessage
            // 
            this.textBox_xcgMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_xcgMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F);
            this.textBox_xcgMessage.Location = new System.Drawing.Point(190, 332);
            this.textBox_xcgMessage.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_xcgMessage.Name = "textBox_xcgMessage";
            this.textBox_xcgMessage.Size = new System.Drawing.Size(627, 22);
            this.textBox_xcgMessage.TabIndex = 1;
            this.textBox_xcgMessage.TextChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // textBox_xcgRawData
            // 
            this.textBox_xcgRawData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_xcgRawData.Location = new System.Drawing.Point(2, 2);
            this.textBox_xcgRawData.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_xcgRawData.Multiline = true;
            this.textBox_xcgRawData.Name = "textBox_xcgRawData";
            this.textBox_xcgRawData.ReadOnly = true;
            this.textBox_xcgRawData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_xcgRawData.Size = new System.Drawing.Size(816, 325);
            this.textBox_xcgRawData.TabIndex = 0;
            // 
            // tabPage_fnCommands
            // 
            this.tabPage_fnCommands.Controls.Add(this.textBox_cconstructorLength);
            this.tabPage_fnCommands.Controls.Add(this.label6);
            this.tabPage_fnCommands.Controls.Add(this.groupBox2);
            this.tabPage_fnCommands.Controls.Add(this.textBox_showCmdParams);
            this.tabPage_fnCommands.Controls.Add(this.textBox_outDataConstructor);
            this.tabPage_fnCommands.Controls.Add(this.label5);
            this.tabPage_fnCommands.Controls.Add(this.label_fnc1);
            this.tabPage_fnCommands.Controls.Add(this.comboBox_fncCommand);
            this.tabPage_fnCommands.Location = new System.Drawing.Point(4, 22);
            this.tabPage_fnCommands.Name = "tabPage_fnCommands";
            this.tabPage_fnCommands.Size = new System.Drawing.Size(824, 460);
            this.tabPage_fnCommands.TabIndex = 2;
            this.tabPage_fnCommands.Text = "FN.Commands.Constructor";
            this.tabPage_fnCommands.UseVisualStyleBackColor = true;
            // 
            // textBox_cconstructorLength
            // 
            this.textBox_cconstructorLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_cconstructorLength.Location = new System.Drawing.Point(731, 3);
            this.textBox_cconstructorLength.Name = "textBox_cconstructorLength";
            this.textBox_cconstructorLength.ReadOnly = true;
            this.textBox_cconstructorLength.Size = new System.Drawing.Size(85, 20);
            this.textBox_cconstructorLength.TabIndex = 9;
            this.textBox_cconstructorLength.Text = "0";
            this.textBox_cconstructorLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(678, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Размер";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton_cconstructorDateFormat);
            this.groupBox2.Controls.Add(this.radioButton_cconstructorUint40);
            this.groupBox2.Controls.Add(this.radioButton_hexData);
            this.groupBox2.Controls.Add(this.button_addData);
            this.groupBox2.Controls.Add(this.radioButton_cconstructorDtFormat);
            this.groupBox2.Controls.Add(this.textBox_valueAddToConstructor);
            this.groupBox2.Controls.Add(this.radioButton_cconstructorAsciiStr);
            this.groupBox2.Controls.Add(this.radioButton_cconstructorUint32);
            this.groupBox2.Controls.Add(this.radioButton_cconstructorUint16);
            this.groupBox2.Controls.Add(this.radioButton_cconstructorByte);
            this.groupBox2.Location = new System.Drawing.Point(10, 122);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(570, 119);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Добавить параметр";
            // 
            // radioButton_cconstructorDateFormat
            // 
            this.radioButton_cconstructorDateFormat.AutoSize = true;
            this.radioButton_cconstructorDateFormat.Location = new System.Drawing.Point(356, 20);
            this.radioButton_cconstructorDateFormat.Name = "radioButton_cconstructorDateFormat";
            this.radioButton_cconstructorDateFormat.Size = new System.Drawing.Size(113, 17);
            this.radioButton_cconstructorDateFormat.TabIndex = 6;
            this.radioButton_cconstructorDateFormat.TabStop = true;
            this.radioButton_cconstructorDateFormat.Text = "Дата(гггг.ММ.дд)";
            this.radioButton_cconstructorDateFormat.UseVisualStyleBackColor = true;
            // 
            // radioButton_cconstructorUint40
            // 
            this.radioButton_cconstructorUint40.AutoSize = true;
            this.radioButton_cconstructorUint40.Location = new System.Drawing.Point(151, 20);
            this.radioButton_cconstructorUint40.Name = "radioButton_cconstructorUint40";
            this.radioButton_cconstructorUint40.Size = new System.Drawing.Size(86, 17);
            this.radioButton_cconstructorUint40.TabIndex = 3;
            this.radioButton_cconstructorUint40.TabStop = true;
            this.radioButton_cconstructorUint40.Text = "UINT40(LE)*";
            this.toolTip1.SetToolTip(this.radioButton_cconstructorUint40, "Скорей всего ненужная опция, в данном формате видел только нарастающие итоги");
            this.radioButton_cconstructorUint40.UseVisualStyleBackColor = true;
            // 
            // radioButton_hexData
            // 
            this.radioButton_hexData.AutoSize = true;
            this.radioButton_hexData.Location = new System.Drawing.Point(356, 43);
            this.radioButton_hexData.Name = "radioButton_hexData";
            this.radioButton_hexData.Size = new System.Drawing.Size(116, 17);
            this.radioButton_hexData.TabIndex = 7;
            this.radioButton_hexData.TabStop = true;
            this.radioButton_hexData.Text = "HEX DATA IMAGE";
            this.radioButton_hexData.UseVisualStyleBackColor = true;
            // 
            // button_addData
            // 
            this.button_addData.Location = new System.Drawing.Point(6, 87);
            this.button_addData.Name = "button_addData";
            this.button_addData.Size = new System.Drawing.Size(134, 23);
            this.button_addData.TabIndex = 6;
            this.button_addData.Text = "Добавить";
            this.button_addData.UseVisualStyleBackColor = true;
            this.button_addData.Click += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // radioButton_cconstructorDtFormat
            // 
            this.radioButton_cconstructorDtFormat.AutoSize = true;
            this.radioButton_cconstructorDtFormat.Location = new System.Drawing.Point(151, 66);
            this.radioButton_cconstructorDtFormat.Name = "radioButton_cconstructorDtFormat";
            this.radioButton_cconstructorDtFormat.Size = new System.Drawing.Size(195, 17);
            this.radioButton_cconstructorDtFormat.TabIndex = 5;
            this.radioButton_cconstructorDtFormat.TabStop = true;
            this.radioButton_cconstructorDtFormat.Text = "Дата время(гггг.ММ.дд чч:мм:сс)";
            this.radioButton_cconstructorDtFormat.UseVisualStyleBackColor = true;
            // 
            // textBox_valueAddToConstructor
            // 
            this.textBox_valueAddToConstructor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_valueAddToConstructor.Location = new System.Drawing.Point(151, 89);
            this.textBox_valueAddToConstructor.Name = "textBox_valueAddToConstructor";
            this.textBox_valueAddToConstructor.Size = new System.Drawing.Size(414, 20);
            this.textBox_valueAddToConstructor.TabIndex = 5;
            // 
            // radioButton_cconstructorAsciiStr
            // 
            this.radioButton_cconstructorAsciiStr.AutoSize = true;
            this.radioButton_cconstructorAsciiStr.Location = new System.Drawing.Point(151, 43);
            this.radioButton_cconstructorAsciiStr.Name = "radioButton_cconstructorAsciiStr";
            this.radioButton_cconstructorAsciiStr.Size = new System.Drawing.Size(99, 17);
            this.radioButton_cconstructorAsciiStr.TabIndex = 4;
            this.radioButton_cconstructorAsciiStr.TabStop = true;
            this.radioButton_cconstructorAsciiStr.Text = "STRING_ASCII";
            this.radioButton_cconstructorAsciiStr.UseVisualStyleBackColor = true;
            // 
            // radioButton_cconstructorUint32
            // 
            this.radioButton_cconstructorUint32.AutoSize = true;
            this.radioButton_cconstructorUint32.Location = new System.Drawing.Point(7, 66);
            this.radioButton_cconstructorUint32.Name = "radioButton_cconstructorUint32";
            this.radioButton_cconstructorUint32.Size = new System.Drawing.Size(82, 17);
            this.radioButton_cconstructorUint32.TabIndex = 2;
            this.radioButton_cconstructorUint32.TabStop = true;
            this.radioButton_cconstructorUint32.Text = "UINT32(LE)";
            this.radioButton_cconstructorUint32.UseVisualStyleBackColor = true;
            // 
            // radioButton_cconstructorUint16
            // 
            this.radioButton_cconstructorUint16.AutoSize = true;
            this.radioButton_cconstructorUint16.Location = new System.Drawing.Point(7, 43);
            this.radioButton_cconstructorUint16.Name = "radioButton_cconstructorUint16";
            this.radioButton_cconstructorUint16.Size = new System.Drawing.Size(82, 17);
            this.radioButton_cconstructorUint16.TabIndex = 1;
            this.radioButton_cconstructorUint16.TabStop = true;
            this.radioButton_cconstructorUint16.Text = "UINT16(LE)";
            this.radioButton_cconstructorUint16.UseVisualStyleBackColor = true;
            // 
            // radioButton_cconstructorByte
            // 
            this.radioButton_cconstructorByte.AutoSize = true;
            this.radioButton_cconstructorByte.Location = new System.Drawing.Point(7, 20);
            this.radioButton_cconstructorByte.Name = "radioButton_cconstructorByte";
            this.radioButton_cconstructorByte.Size = new System.Drawing.Size(61, 17);
            this.radioButton_cconstructorByte.TabIndex = 0;
            this.radioButton_cconstructorByte.TabStop = true;
            this.radioButton_cconstructorByte.Text = "UBYTE";
            this.radioButton_cconstructorByte.UseVisualStyleBackColor = true;
            // 
            // textBox_showCmdParams
            // 
            this.textBox_showCmdParams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_showCmdParams.Location = new System.Drawing.Point(10, 50);
            this.textBox_showCmdParams.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_showCmdParams.Multiline = true;
            this.textBox_showCmdParams.Name = "textBox_showCmdParams";
            this.textBox_showCmdParams.ReadOnly = true;
            this.textBox_showCmdParams.Size = new System.Drawing.Size(807, 67);
            this.textBox_showCmdParams.TabIndex = 4;
            // 
            // textBox_outDataConstructor
            // 
            this.textBox_outDataConstructor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_outDataConstructor.Location = new System.Drawing.Point(59, 27);
            this.textBox_outDataConstructor.Name = "textBox_outDataConstructor";
            this.textBox_outDataConstructor.ReadOnly = true;
            this.textBox_outDataConstructor.Size = new System.Drawing.Size(757, 20);
            this.textBox_outDataConstructor.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "HEX out";
            // 
            // label_fnc1
            // 
            this.label_fnc1.AutoSize = true;
            this.label_fnc1.Location = new System.Drawing.Point(7, 6);
            this.label_fnc1.Name = "label_fnc1";
            this.label_fnc1.Size = new System.Drawing.Size(52, 13);
            this.label_fnc1.TabIndex = 1;
            this.label_fnc1.Text = "Команда";
            // 
            // comboBox_fncCommand
            // 
            this.comboBox_fncCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_fncCommand.FormattingEnabled = true;
            this.comboBox_fncCommand.Items.AddRange(new object[] {
            "02h – Начать отчет о регистрации/перерегистрации ФФД-1.0 или ФФД 1.05",
            "03h – Сформировать отчет о регистрации/перерегистрации ФФД-1.0 или ФФД 1.05",
            "04h – Начать закрытие ФН",
            "05h – Закрыть ФН",
            "06h – Отменить документ",
            "07h – Передать данные документа",
            "",
            "10h – Запрос параметров текущей смены",
            "11h – Начать открытие смены",
            "12h – Открыть смену",
            "13h – Начать закрытие смены",
            "14h – Закрыть смену",
            "15h – Начать формирование чека (БСО)",
            "16h – Сформировать чек",
            "17h – Начать формирование чека коррекции (БСО)",
            "18h – Начать формирование отчета о состоянии расчетов",
            "19h – Сформировать отчет о текущем состоянии расчетов",
            "",
            "20h – Получить статус информационного обмена",
            "21h – Передать статус транспортного соединения с Сервером ОФД",
            "22h – Начать чтение Сообщения для Сервера ОФД",
            "23h – Прочитать блок сообщения для Сервера ОФД",
            "24h – Отменить чтение Сообщения для Сервера ОФД",
            "25h – Завершить чтение Сообщения для Сервера ОФД",
            "26h – Передать Квитанцию от Сервера ОФД",
            "",
            "30h – Запрос статуса ФН",
            "31h – Запрос номера ФН",
            "32h – Запрос срока действия ФН",
            "33h – Запрос версии ФН",
            "35h – Запрос последних ошибок ФН",
            "36h – Запрос счетчиков ФН",
            "37h – Запрос счетчиков операций ФН",
            "38h – Запрос счетчиков ФН по заданному типу расчетов",
            "39h – Запрос счётчиков итогов непереданных документов",
            "3Ah – Запрос формата ФН",
            "3Вh – Запрос оставшегося срока действия ФН",
            "3Dh – Запрос ресурса свободной памяти в ФН",
            "",
            "40h – Найти фискальный документ по номеру(данные из архива)",
            "41h – Запрос квитанции о получении фискального документа ОФД по номеру документа",
            "42h – Запрос количества ФД, на которые нет квитанции",
            "43h – Запрос итогов открытия/регистрации/перерегистрации ККТ",
            "44h – Запрос параметра открытия ФН",
            "45h – Запрос фискального документа в TLV формате",
            "46h – Чтение TLV фискального документа",
            "47h – Чтение TLV параметров открытия ФН",
            "",
            "A2h – Начать отчет о регистрации/перерегистрации ФФД-1.1 или больше",
            "A3h – Сформировать отчет о регистрации/перерегистрации",
            "",
            "A7h – Запрос общего размера данных, переданных командой 07h или B7h",
            "ABh – переход на повышенную скорость работы по UART",
            "",
            "B0h – Запрос статуса ФН по работе с кодами маркировки",
            "B1h – Передать код маркировки для проверки в ФН",
            "B2h – Сохранить результаты проверки КМ",
            "B3h – Очистить все результаты проверки КМ",
            "B5h – Сформировать запрос о коде маркировки",
            "B6h – Передать ответ на запрос о КМ",
            "B7h – Передать данные для формирования фискальных документов, содержащих данные о" +
                " маркированных товарах",
            "",
            "BBh – Начать чтение уведомления",
            "BDh – Отменить чтение уведомления",
            "BEh – Завершить чтение уведомления",
            "ВFh – Передать квитанцию на уведомление",
            "",
            "D1h – Передать ответ на запрос на обновление ключей проверки",
            "",
            "D4h – Перейти к следующему уведомлению, или получить параметры текущего уведомлен" +
                "ия",
            "D5h – Прочитать блок данных текущего уведомления",
            "D6h – Подтвердить выгрузку уведомления или получить информацию по неподтверждённы" +
                "м уведомлениям",
            "D7h – Получить адрес сервера обновления ключей проверки"});
            this.comboBox_fncCommand.Location = new System.Drawing.Point(59, 3);
            this.comboBox_fncCommand.Name = "comboBox_fncCommand";
            this.comboBox_fncCommand.Size = new System.Drawing.Size(602, 21);
            this.comboBox_fncCommand.TabIndex = 0;
            this.comboBox_fncCommand.SelectedIndexChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // tabPage_FnInfo
            // 
            this.tabPage_FnInfo.Controls.Add(this.button_getFullFnInfo2);
            this.tabPage_FnInfo.Controls.Add(this.richTextBox_fnTechCondition);
            this.tabPage_FnInfo.Controls.Add(this.groupBox8);
            this.tabPage_FnInfo.Location = new System.Drawing.Point(4, 22);
            this.tabPage_FnInfo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage_FnInfo.Name = "tabPage_FnInfo";
            this.tabPage_FnInfo.Size = new System.Drawing.Size(824, 460);
            this.tabPage_FnInfo.TabIndex = 3;
            this.tabPage_FnInfo.Text = "Информация о ФН";
            this.tabPage_FnInfo.UseVisualStyleBackColor = true;
            // 
            // button_getFullFnInfo2
            // 
            this.button_getFullFnInfo2.Location = new System.Drawing.Point(6, 3);
            this.button_getFullFnInfo2.Name = "button_getFullFnInfo2";
            this.button_getFullFnInfo2.Size = new System.Drawing.Size(129, 23);
            this.button_getFullFnInfo2.TabIndex = 24;
            this.button_getFullFnInfo2.Text = "Опросить ФН";
            this.button_getFullFnInfo2.UseVisualStyleBackColor = true;
            this.button_getFullFnInfo2.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // richTextBox_fnTechCondition
            // 
            this.richTextBox_fnTechCondition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_fnTechCondition.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_fnTechCondition.Location = new System.Drawing.Point(8, 29);
            this.richTextBox_fnTechCondition.Name = "richTextBox_fnTechCondition";
            this.richTextBox_fnTechCondition.Size = new System.Drawing.Size(807, 219);
            this.richTextBox_fnTechCondition.TabIndex = 0;
            this.richTextBox_fnTechCondition.Text = "";
            // 
            // groupBox8
            // 
            this.groupBox8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox8.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox8.Controls.Add(this.dataGridView_regParams);
            this.groupBox8.Location = new System.Drawing.Point(6, 250);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox8.Size = new System.Drawing.Size(814, 208);
            this.groupBox8.TabIndex = 23;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Данные о регистрации фискального накопителя";
            // 
            // dataGridView_regParams
            // 
            this.dataGridView_regParams.ColumnHeadersHeight = 24;
            this.dataGridView_regParams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column_tagNumber,
            this.Column_tagDescriber,
            this.Column_tagValue});
            this.dataGridView_regParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_regParams.Location = new System.Drawing.Point(2, 15);
            this.dataGridView_regParams.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dataGridView_regParams.Name = "dataGridView_regParams";
            this.dataGridView_regParams.RowHeadersWidth = 5;
            this.dataGridView_regParams.RowTemplate.Height = 24;
            this.dataGridView_regParams.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView_regParams.Size = new System.Drawing.Size(810, 191);
            this.dataGridView_regParams.TabIndex = 14;
            // 
            // Column_tagNumber
            // 
            this.Column_tagNumber.HeaderText = "Номер тега";
            this.Column_tagNumber.MinimumWidth = 6;
            this.Column_tagNumber.Name = "Column_tagNumber";
            this.Column_tagNumber.Width = 94;
            // 
            // Column_tagDescriber
            // 
            this.Column_tagDescriber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column_tagDescriber.FillWeight = 70F;
            this.Column_tagDescriber.HeaderText = "Название тега";
            this.Column_tagDescriber.MinimumWidth = 6;
            this.Column_tagDescriber.Name = "Column_tagDescriber";
            this.Column_tagDescriber.ReadOnly = true;
            // 
            // Column_tagValue
            // 
            this.Column_tagValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column_tagValue.HeaderText = "Значение тега";
            this.Column_tagValue.MinimumWidth = 6;
            this.Column_tagValue.Name = "Column_tagValue";
            // 
            // tabPage_readFd
            // 
            this.tabPage_readFd.Controls.Add(this.checkBox_getExtraTlvInfo);
            this.tabPage_readFd.Controls.Add(this.button_readFdVariant2);
            this.tabPage_readFd.Controls.Add(this.textBox_strHexToParce);
            this.tabPage_readFd.Controls.Add(this.button_saveFdToFile);
            this.tabPage_readFd.Controls.Add(this.button_parceHexString);
            this.tabPage_readFd.Controls.Add(this.comboBox_formatToSave);
            this.tabPage_readFd.Controls.Add(this.groupBox5);
            this.tabPage_readFd.Controls.Add(this.checkBox_preferUserFrandly);
            this.tabPage_readFd.Controls.Add(this.label15);
            this.tabPage_readFd.Controls.Add(this.numericUpDown_fdNumber2);
            this.tabPage_readFd.Controls.Add(this.treeView_fnReadedContent);
            this.tabPage_readFd.Controls.Add(this.numericUpDown_fdNumber1);
            this.tabPage_readFd.Controls.Add(this.button_readFdTask);
            this.tabPage_readFd.Location = new System.Drawing.Point(4, 22);
            this.tabPage_readFd.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage_readFd.Name = "tabPage_readFd";
            this.tabPage_readFd.Size = new System.Drawing.Size(824, 460);
            this.tabPage_readFd.TabIndex = 4;
            this.tabPage_readFd.Text = "Чтение ФД";
            this.tabPage_readFd.UseVisualStyleBackColor = true;
            // 
            // checkBox_getExtraTlvInfo
            // 
            this.checkBox_getExtraTlvInfo.AutoSize = true;
            this.checkBox_getExtraTlvInfo.Checked = true;
            this.checkBox_getExtraTlvInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_getExtraTlvInfo.Location = new System.Drawing.Point(6, 27);
            this.checkBox_getExtraTlvInfo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBox_getExtraTlvInfo.Name = "checkBox_getExtraTlvInfo";
            this.checkBox_getExtraTlvInfo.Size = new System.Drawing.Size(158, 17);
            this.checkBox_getExtraTlvInfo.TabIndex = 12;
            this.checkBox_getExtraTlvInfo.Text = "Запрашивать TLV данные";
            this.checkBox_getExtraTlvInfo.UseVisualStyleBackColor = true;
            // 
            // button_readFdVariant2
            // 
            this.button_readFdVariant2.Location = new System.Drawing.Point(5, 2);
            this.button_readFdVariant2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_readFdVariant2.Name = "button_readFdVariant2";
            this.button_readFdVariant2.Size = new System.Drawing.Size(128, 21);
            this.button_readFdVariant2.TabIndex = 3;
            this.button_readFdVariant2.Text = "Просмотр архива";
            this.button_readFdVariant2.UseVisualStyleBackColor = true;
            this.button_readFdVariant2.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // textBox_strHexToParce
            // 
            this.textBox_strHexToParce.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_strHexToParce.AutoCompleteCustomSource.AddRange(new string[] {
            "FF-FF-FF-FF-FF",
            "FF FF FF FF FF"});
            this.textBox_strHexToParce.Location = new System.Drawing.Point(515, 1);
            this.textBox_strHexToParce.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_strHexToParce.Multiline = true;
            this.textBox_strHexToParce.Name = "textBox_strHexToParce";
            this.textBox_strHexToParce.Size = new System.Drawing.Size(307, 64);
            this.textBox_strHexToParce.TabIndex = 10;
            this.toolTip1.SetToolTip(this.textBox_strHexToParce, "FF-FF-FF...\r\nFF FF FF...");
            // 
            // button_saveFdToFile
            // 
            this.button_saveFdToFile.Enabled = false;
            this.button_saveFdToFile.Location = new System.Drawing.Point(205, 43);
            this.button_saveFdToFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_saveFdToFile.Name = "button_saveFdToFile";
            this.button_saveFdToFile.Size = new System.Drawing.Size(195, 23);
            this.button_saveFdToFile.TabIndex = 8;
            this.button_saveFdToFile.Text = "Сохранить структуру TLV ФД как";
            this.button_saveFdToFile.UseVisualStyleBackColor = true;
            this.button_saveFdToFile.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // button_parceHexString
            // 
            this.button_parceHexString.Location = new System.Drawing.Point(320, 1);
            this.button_parceHexString.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_parceHexString.Name = "button_parceHexString";
            this.button_parceHexString.Size = new System.Drawing.Size(192, 21);
            this.button_parceHexString.TabIndex = 9;
            this.button_parceHexString.Text = "Разобрать HEX TLV строку";
            this.toolTip1.SetToolTip(this.button_parceHexString, "FF-FF-FF...\r\nFF FF FF...");
            this.button_parceHexString.UseVisualStyleBackColor = true;
            this.button_parceHexString.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // comboBox_formatToSave
            // 
            this.comboBox_formatToSave.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_formatToSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBox_formatToSave.FormattingEnabled = true;
            this.comboBox_formatToSave.Items.AddRange(new object[] {
            "JSON формат ФНС",
            "STLV binary file",
            "HEX IMAGE txt"});
            this.comboBox_formatToSave.Location = new System.Drawing.Point(404, 44);
            this.comboBox_formatToSave.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox_formatToSave.Name = "comboBox_formatToSave";
            this.comboBox_formatToSave.Size = new System.Drawing.Size(107, 21);
            this.comboBox_formatToSave.TabIndex = 7;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.textBox_tagInfoRawData);
            this.groupBox5.Controls.Add(this.label22);
            this.groupBox5.Controls.Add(this.textBox_tagInfoTagDataType);
            this.groupBox5.Controls.Add(this.label21);
            this.groupBox5.Controls.Add(this.textBox_tagInfoTagNumericValue);
            this.groupBox5.Controls.Add(this.label20);
            this.groupBox5.Controls.Add(this.textBox_tagInfoStringPresentation);
            this.groupBox5.Controls.Add(this.label19);
            this.groupBox5.Controls.Add(this.textBox_tagInfoUserFriendlyName);
            this.groupBox5.Controls.Add(this.label18);
            this.groupBox5.Controls.Add(this.textBox_tagInfoFnsName);
            this.groupBox5.Controls.Add(this.label17);
            this.groupBox5.Controls.Add(this.textBox_tagInfoTagNumber);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Location = new System.Drawing.Point(630, 67);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox5.Size = new System.Drawing.Size(194, 387);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Информация о фискальном теге";
            // 
            // textBox_tagInfoRawData
            // 
            this.textBox_tagInfoRawData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox_tagInfoRawData.Location = new System.Drawing.Point(0, 215);
            this.textBox_tagInfoRawData.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_tagInfoRawData.Multiline = true;
            this.textBox_tagInfoRawData.Name = "textBox_tagInfoRawData";
            this.textBox_tagInfoRawData.ReadOnly = true;
            this.textBox_tagInfoRawData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_tagInfoRawData.Size = new System.Drawing.Size(194, 168);
            this.textBox_tagInfoRawData.TabIndex = 13;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(39, 200);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(129, 13);
            this.label22.TabIndex = 12;
            this.label22.Text = "Массив данных rawData";
            // 
            // textBox_tagInfoTagDataType
            // 
            this.textBox_tagInfoTagDataType.Location = new System.Drawing.Point(70, 36);
            this.textBox_tagInfoTagDataType.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_tagInfoTagDataType.Name = "textBox_tagInfoTagDataType";
            this.textBox_tagInfoTagDataType.ReadOnly = true;
            this.textBox_tagInfoTagDataType.Size = new System.Drawing.Size(120, 20);
            this.textBox_tagInfoTagDataType.TabIndex = 11;
            this.textBox_tagInfoTagDataType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(109, 16);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(66, 13);
            this.label21.TabIndex = 10;
            this.label21.Text = "Тип данных";
            // 
            // textBox_tagInfoTagNumericValue
            // 
            this.textBox_tagInfoTagNumericValue.Location = new System.Drawing.Point(4, 180);
            this.textBox_tagInfoTagNumericValue.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_tagInfoTagNumericValue.Name = "textBox_tagInfoTagNumericValue";
            this.textBox_tagInfoTagNumericValue.ReadOnly = true;
            this.textBox_tagInfoTagNumericValue.Size = new System.Drawing.Size(185, 20);
            this.textBox_tagInfoTagNumericValue.TabIndex = 9;
            this.textBox_tagInfoTagNumericValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(27, 164);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(160, 13);
            this.label20.TabIndex = 8;
            this.label20.Text = "Числовое значение если есть";
            // 
            // textBox_tagInfoStringPresentation
            // 
            this.textBox_tagInfoStringPresentation.Location = new System.Drawing.Point(4, 144);
            this.textBox_tagInfoStringPresentation.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_tagInfoStringPresentation.Name = "textBox_tagInfoStringPresentation";
            this.textBox_tagInfoStringPresentation.ReadOnly = true;
            this.textBox_tagInfoStringPresentation.Size = new System.Drawing.Size(185, 20);
            this.textBox_tagInfoStringPresentation.TabIndex = 7;
            this.textBox_tagInfoStringPresentation.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(32, 128);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(141, 13);
            this.label19.TabIndex = 6;
            this.label19.Text = "Строковое представление";
            // 
            // textBox_tagInfoUserFriendlyName
            // 
            this.textBox_tagInfoUserFriendlyName.Location = new System.Drawing.Point(4, 107);
            this.textBox_tagInfoUserFriendlyName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_tagInfoUserFriendlyName.Name = "textBox_tagInfoUserFriendlyName";
            this.textBox_tagInfoUserFriendlyName.ReadOnly = true;
            this.textBox_tagInfoUserFriendlyName.Size = new System.Drawing.Size(185, 20);
            this.textBox_tagInfoUserFriendlyName.TabIndex = 5;
            this.textBox_tagInfoUserFriendlyName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(27, 92);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(141, 13);
            this.label18.TabIndex = 4;
            this.label18.Text = "Название тега userFriendly";
            // 
            // textBox_tagInfoFnsName
            // 
            this.textBox_tagInfoFnsName.Location = new System.Drawing.Point(4, 72);
            this.textBox_tagInfoFnsName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_tagInfoFnsName.Name = "textBox_tagInfoFnsName";
            this.textBox_tagInfoFnsName.ReadOnly = true;
            this.textBox_tagInfoFnsName.Size = new System.Drawing.Size(185, 20);
            this.textBox_tagInfoFnsName.TabIndex = 3;
            this.textBox_tagInfoFnsName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(39, 56);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(124, 13);
            this.label17.TabIndex = 2;
            this.label17.Text = "Название токена ФНС";
            // 
            // textBox_tagInfoTagNumber
            // 
            this.textBox_tagInfoTagNumber.Location = new System.Drawing.Point(4, 36);
            this.textBox_tagInfoTagNumber.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_tagInfoTagNumber.Name = "textBox_tagInfoTagNumber";
            this.textBox_tagInfoTagNumber.ReadOnly = true;
            this.textBox_tagInfoTagNumber.Size = new System.Drawing.Size(55, 20);
            this.textBox_tagInfoTagNumber.TabIndex = 1;
            this.textBox_tagInfoTagNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(11, 16);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(41, 13);
            this.label16.TabIndex = 0;
            this.label16.Text = "Номер";
            // 
            // checkBox_preferUserFrandly
            // 
            this.checkBox_preferUserFrandly.AutoSize = true;
            this.checkBox_preferUserFrandly.Checked = true;
            this.checkBox_preferUserFrandly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_preferUserFrandly.Font = new System.Drawing.Font("Microsoft Sans Serif", 4.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBox_preferUserFrandly.Location = new System.Drawing.Point(284, 26);
            this.checkBox_preferUserFrandly.Margin = new System.Windows.Forms.Padding(0);
            this.checkBox_preferUserFrandly.Name = "checkBox_preferUserFrandly";
            this.checkBox_preferUserFrandly.Size = new System.Drawing.Size(65, 14);
            this.checkBox_preferUserFrandly.TabIndex = 5;
            this.checkBox_preferUserFrandly.Text = "UserFriendly tag";
            this.checkBox_preferUserFrandly.UseVisualStyleBackColor = true;
            this.checkBox_preferUserFrandly.Visible = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(138, 6);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(58, 13);
            this.label15.TabIndex = 4;
            this.label15.Text = "Диапазон";
            // 
            // numericUpDown_fdNumber2
            // 
            this.numericUpDown_fdNumber2.Location = new System.Drawing.Point(260, 2);
            this.numericUpDown_fdNumber2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.numericUpDown_fdNumber2.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this.numericUpDown_fdNumber2.Name = "numericUpDown_fdNumber2";
            this.numericUpDown_fdNumber2.Size = new System.Drawing.Size(56, 20);
            this.numericUpDown_fdNumber2.TabIndex = 2;
            this.numericUpDown_fdNumber2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_fdNumber2.ValueChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // treeView_fnReadedContent
            // 
            this.treeView_fnReadedContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView_fnReadedContent.Location = new System.Drawing.Point(5, 67);
            this.treeView_fnReadedContent.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.treeView_fnReadedContent.Name = "treeView_fnReadedContent";
            this.treeView_fnReadedContent.Size = new System.Drawing.Size(624, 383);
            this.treeView_fnReadedContent.TabIndex = 200;
            this.treeView_fnReadedContent.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_fnReadedContent_AfterSelect);
            // 
            // numericUpDown_fdNumber1
            // 
            this.numericUpDown_fdNumber1.Location = new System.Drawing.Point(200, 2);
            this.numericUpDown_fdNumber1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.numericUpDown_fdNumber1.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this.numericUpDown_fdNumber1.Name = "numericUpDown_fdNumber1";
            this.numericUpDown_fdNumber1.Size = new System.Drawing.Size(56, 20);
            this.numericUpDown_fdNumber1.TabIndex = 1;
            this.numericUpDown_fdNumber1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_fdNumber1.ValueChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // button_readFdTask
            // 
            this.button_readFdTask.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_readFdTask.Location = new System.Drawing.Point(166, 25);
            this.button_readFdTask.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_readFdTask.Name = "button_readFdTask";
            this.button_readFdTask.Size = new System.Drawing.Size(116, 16);
            this.button_readFdTask.TabIndex = 0;
            this.button_readFdTask.Text = "Прочитать ФД в TLV";
            this.button_readFdTask.UseVisualStyleBackColor = true;
            this.button_readFdTask.Visible = false;
            this.button_readFdTask.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // tabPage_createFdRules
            // 
            this.tabPage_createFdRules.Controls.Add(this.button_fdRulesTabApplyCurrentTable);
            this.tabPage_createFdRules.Controls.Add(this.button_loadTFNR);
            this.tabPage_createFdRules.Controls.Add(this.button_saveTFNCommonFdRules);
            this.tabPage_createFdRules.Controls.Add(this.label31);
            this.tabPage_createFdRules.Controls.Add(this.comboBox_ruleTabFdTypeChooser);
            this.tabPage_createFdRules.Controls.Add(this.radioButton_ffdRulesSwitcher_4);
            this.tabPage_createFdRules.Controls.Add(this.radioButton_ffdRulesSwitcher_3);
            this.tabPage_createFdRules.Controls.Add(this.dataGridView_termFdRules);
            this.tabPage_createFdRules.Controls.Add(this.radioButton_ffdRulesSwitcher_2);
            this.tabPage_createFdRules.Location = new System.Drawing.Point(4, 22);
            this.tabPage_createFdRules.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabPage_createFdRules.Name = "tabPage_createFdRules";
            this.tabPage_createFdRules.Size = new System.Drawing.Size(824, 460);
            this.tabPage_createFdRules.TabIndex = 5;
            this.tabPage_createFdRules.Text = "Правила формирования ФД";
            this.tabPage_createFdRules.UseVisualStyleBackColor = true;
            // 
            // button_fdRulesTabApplyCurrentTable
            // 
            this.button_fdRulesTabApplyCurrentTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_fdRulesTabApplyCurrentTable.Location = new System.Drawing.Point(323, 434);
            this.button_fdRulesTabApplyCurrentTable.Name = "button_fdRulesTabApplyCurrentTable";
            this.button_fdRulesTabApplyCurrentTable.Size = new System.Drawing.Size(146, 23);
            this.button_fdRulesTabApplyCurrentTable.TabIndex = 8;
            this.button_fdRulesTabApplyCurrentTable.Text = "Применить изменения";
            this.toolTip1.SetToolTip(this.button_fdRulesTabApplyCurrentTable, "Применить изменения текущей таблицы");
            this.button_fdRulesTabApplyCurrentTable.UseVisualStyleBackColor = true;
            this.button_fdRulesTabApplyCurrentTable.Click += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // button_loadTFNR
            // 
            this.button_loadTFNR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_loadTFNR.Location = new System.Drawing.Point(650, 434);
            this.button_loadTFNR.Name = "button_loadTFNR";
            this.button_loadTFNR.Size = new System.Drawing.Size(166, 23);
            this.button_loadTFNR.TabIndex = 7;
            this.button_loadTFNR.Text = "Загрузить правила заново";
            this.toolTip1.SetToolTip(this.button_loadTFNR, "Загрузить правила из файла \"terminalfn_ffdstructure.xml\"\r\nЭто сбросит все измения" +
        " таблиц на то что записано в файле");
            this.button_loadTFNR.UseVisualStyleBackColor = true;
            this.button_loadTFNR.Click += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // button_saveTFNCommonFdRules
            // 
            this.button_saveTFNCommonFdRules.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_saveTFNCommonFdRules.Location = new System.Drawing.Point(484, 434);
            this.button_saveTFNCommonFdRules.Name = "button_saveTFNCommonFdRules";
            this.button_saveTFNCommonFdRules.Size = new System.Drawing.Size(146, 23);
            this.button_saveTFNCommonFdRules.TabIndex = 6;
            this.button_saveTFNCommonFdRules.Text = "Сохранить правила";
            this.toolTip1.SetToolTip(this.button_saveTFNCommonFdRules, "Сохранить правила в файл \"terminalfn_ffdstructure.xml\"\r\nВ дальнейшем при запуске " +
        "программы будут работать измененные правила\r\nРекомендуется сделать бэкап правил " +
        "перед сохранением");
            this.button_saveTFNCommonFdRules.UseVisualStyleBackColor = true;
            this.button_saveTFNCommonFdRules.Click += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(401, 10);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(56, 13);
            this.label31.TabIndex = 5;
            this.label31.Text = "Код STLV";
            // 
            // comboBox_ruleTabFdTypeChooser
            // 
            this.comboBox_ruleTabFdTypeChooser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_ruleTabFdTypeChooser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ruleTabFdTypeChooser.FormattingEnabled = true;
            this.comboBox_ruleTabFdTypeChooser.Items.AddRange(new object[] {
            "[1] =    Отчет о регистрации",
            "[2] =    Открытие смены",
            "[3] =    Кассовый чек(4-БСО)",
            "[5] =    Закрытие смены",
            "[6] =    Закрытие ФН",
            "[7] =    Подтверждение оператора",
            "[11] =   Перерегистрация",
            "[21] =   Отчет о состоянии расчетов",
            "[31] =   Чек(41-БСО) коррекции",
            "Субструктура [1059] = предмет расчета  items",
            "Субструктура [1084] = Дополнительный реквизит пользователя properties",
            "Субструктура [1115] = суммы ндс чека       (обновление НДС 5,7)",
            "Субструктура [1119] = сумма НДС чека       (обновление НДС 5,7)",
            "Субструктура [1163] = код товара ФФД 1.2   items.productCodeNew",
            "Субструктура [1174] = Основание для коррекции  сorrectionBase",
            "Субструктура [1223] = Данные агента    items.paymentAgentData",
            "Субструктура [1224] = Данные поставщика    items.providerData",
            "Субструктура [1234] = сведения  обо всехоплатах почекубезналичными",
            "Субструктура [1235] = сведения об оплате безналичными",
            "Субструктура [1256] = сведения о покупателе(клиенте)",
            "Субструктура [1260] = отраслевой реквизит предмета расчета",
            "Субструктура [1261] = отраслевой реквизит чека",
            "Субструктура [1270] = операционный реквизит чека",
            "Субструктура [1291] = дробное количество маркированного товара"});
            this.comboBox_ruleTabFdTypeChooser.Location = new System.Drawing.Point(463, 6);
            this.comboBox_ruleTabFdTypeChooser.Name = "comboBox_ruleTabFdTypeChooser";
            this.comboBox_ruleTabFdTypeChooser.Size = new System.Drawing.Size(353, 21);
            this.comboBox_ruleTabFdTypeChooser.TabIndex = 4;
            this.comboBox_ruleTabFdTypeChooser.DropDown += new System.EventHandler(this.CommonConrolsEventHandler);
            this.comboBox_ruleTabFdTypeChooser.SelectedIndexChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // radioButton_ffdRulesSwitcher_4
            // 
            this.radioButton_ffdRulesSwitcher_4.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_ffdRulesSwitcher_4.AutoSize = true;
            this.radioButton_ffdRulesSwitcher_4.Location = new System.Drawing.Point(198, 5);
            this.radioButton_ffdRulesSwitcher_4.Name = "radioButton_ffdRulesSwitcher_4";
            this.radioButton_ffdRulesSwitcher_4.Size = new System.Drawing.Size(81, 23);
            this.radioButton_ffdRulesSwitcher_4.TabIndex = 3;
            this.radioButton_ffdRulesSwitcher_4.TabStop = true;
            this.radioButton_ffdRulesSwitcher_4.Text = "ФФД 1.2 (4)";
            this.radioButton_ffdRulesSwitcher_4.UseVisualStyleBackColor = true;
            this.radioButton_ffdRulesSwitcher_4.CheckedChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // radioButton_ffdRulesSwitcher_3
            // 
            this.radioButton_ffdRulesSwitcher_3.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_ffdRulesSwitcher_3.AutoSize = true;
            this.radioButton_ffdRulesSwitcher_3.Location = new System.Drawing.Point(102, 5);
            this.radioButton_ffdRulesSwitcher_3.Name = "radioButton_ffdRulesSwitcher_3";
            this.radioButton_ffdRulesSwitcher_3.Size = new System.Drawing.Size(81, 23);
            this.radioButton_ffdRulesSwitcher_3.TabIndex = 2;
            this.radioButton_ffdRulesSwitcher_3.TabStop = true;
            this.radioButton_ffdRulesSwitcher_3.Text = "ФФД 1.1 (3)";
            this.radioButton_ffdRulesSwitcher_3.UseVisualStyleBackColor = true;
            this.radioButton_ffdRulesSwitcher_3.CheckedChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // dataGridView_termFdRules
            // 
            this.dataGridView_termFdRules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_termFdRules.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_termFdRules.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnTN,
            this.Column_tn,
            this.Column_Criticality,
            this.Column_sourceData,
            this.Column_dataOverride});
            this.dataGridView_termFdRules.Location = new System.Drawing.Point(4, 33);
            this.dataGridView_termFdRules.MultiSelect = false;
            this.dataGridView_termFdRules.Name = "dataGridView_termFdRules";
            this.dataGridView_termFdRules.RowHeadersWidth = 15;
            this.dataGridView_termFdRules.Size = new System.Drawing.Size(817, 395);
            this.dataGridView_termFdRules.TabIndex = 0;
            // 
            // ColumnTN
            // 
            this.ColumnTN.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnTN.FillWeight = 35F;
            this.ColumnTN.HeaderText = "Номер тега";
            this.ColumnTN.MinimumWidth = 6;
            this.ColumnTN.Name = "ColumnTN";
            // 
            // Column_tn
            // 
            this.Column_tn.FillWeight = 80F;
            this.Column_tn.HeaderText = "Название тега";
            this.Column_tn.MinimumWidth = 6;
            this.Column_tn.Name = "Column_tn";
            this.Column_tn.Width = 125;
            // 
            // Column_Criticality
            // 
            this.Column_Criticality.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column_Criticality.FillWeight = 110F;
            this.Column_Criticality.HeaderText = "Поведение при отказе ФН в приеме тега";
            this.Column_Criticality.Items.AddRange(new object[] {
            "Продолжить формирование ФД - Не критичный тег",
            "Отмена ФД - Критичный тег"});
            this.Column_Criticality.MinimumWidth = 6;
            this.Column_Criticality.Name = "Column_Criticality";
            this.Column_Criticality.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column_Criticality.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Column_sourceData
            // 
            this.Column_sourceData.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column_sourceData.HeaderText = "Источник данных";
            this.Column_sourceData.Items.AddRange(new object[] {
            "Формируется ФНом или необяз.(программа игнорирует)",
            "Из регистрационных параметров",
            "Перезапись значения(из данных по умолчанию)",
            "Параметры чека"});
            this.Column_sourceData.MinimumWidth = 6;
            this.Column_sourceData.Name = "Column_sourceData";
            // 
            // Column_dataOverride
            // 
            this.Column_dataOverride.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column_dataOverride.FillWeight = 140F;
            this.Column_dataOverride.HeaderText = "По умолчанию";
            this.Column_dataOverride.MinimumWidth = 6;
            this.Column_dataOverride.Name = "Column_dataOverride";
            // 
            // radioButton_ffdRulesSwitcher_2
            // 
            this.radioButton_ffdRulesSwitcher_2.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton_ffdRulesSwitcher_2.AutoSize = true;
            this.radioButton_ffdRulesSwitcher_2.Location = new System.Drawing.Point(6, 5);
            this.radioButton_ffdRulesSwitcher_2.Name = "radioButton_ffdRulesSwitcher_2";
            this.radioButton_ffdRulesSwitcher_2.Size = new System.Drawing.Size(84, 23);
            this.radioButton_ffdRulesSwitcher_2.TabIndex = 1;
            this.radioButton_ffdRulesSwitcher_2.TabStop = true;
            this.radioButton_ffdRulesSwitcher_2.Text = "ФФД 1.05(2)";
            this.radioButton_ffdRulesSwitcher_2.UseVisualStyleBackColor = true;
            this.radioButton_ffdRulesSwitcher_2.CheckedChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // tabPage_fdAction
            // 
            this.tabPage_fdAction.Controls.Add(this.button_perfTabItemsConstructor);
            this.tabPage_fdAction.Controls.Add(this.textBox_perfTabSelectedTNType);
            this.tabPage_fdAction.Controls.Add(this.button_perfTabRemoveTlvElem);
            this.tabPage_fdAction.Controls.Add(this.button_perfTabAppendTlvElem);
            this.tabPage_fdAction.Controls.Add(this.textBox_perfTab_ftagUFName);
            this.tabPage_fdAction.Controls.Add(this.treeView_tabPerfStlvMiniConstructor);
            this.tabPage_fdAction.Controls.Add(this.textBox__tabFdToPerf_EditorValueMain);
            this.tabPage_fdAction.Controls.Add(this.textBox_tabFdToPerf_EditorNumberMain);
            this.tabPage_fdAction.Controls.Add(this.label36);
            this.tabPage_fdAction.Controls.Add(this.label35);
            this.tabPage_fdAction.Controls.Add(this.labenumt);
            this.tabPage_fdAction.Controls.Add(this.groupBox_performTabAll);
            this.tabPage_fdAction.Controls.Add(this.label34);
            this.tabPage_fdAction.Controls.Add(this.dataGridView_ftagListToPerform);
            this.tabPage_fdAction.Controls.Add(this.label_PerfTabtimeForFd);
            this.tabPage_fdAction.Controls.Add(this.comboBox_performTabFfdToPerform);
            this.tabPage_fdAction.Controls.Add(this.label33);
            this.tabPage_fdAction.Controls.Add(this.label32);
            this.tabPage_fdAction.Controls.Add(this.comboBox_performTabDocType);
            this.tabPage_fdAction.Location = new System.Drawing.Point(4, 22);
            this.tabPage_fdAction.Name = "tabPage_fdAction";
            this.tabPage_fdAction.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabPage_fdAction.Size = new System.Drawing.Size(824, 460);
            this.tabPage_fdAction.TabIndex = 6;
            this.tabPage_fdAction.Text = "Формирование ФД";
            this.tabPage_fdAction.UseVisualStyleBackColor = true;
            // 
            // button_perfTabItemsConstructor
            // 
            this.button_perfTabItemsConstructor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_perfTabItemsConstructor.Enabled = false;
            this.button_perfTabItemsConstructor.Location = new System.Drawing.Point(6, 422);
            this.button_perfTabItemsConstructor.Name = "button_perfTabItemsConstructor";
            this.button_perfTabItemsConstructor.Size = new System.Drawing.Size(41, 36);
            this.button_perfTabItemsConstructor.TabIndex = 221;
            this.button_perfTabItemsConstructor.Text = "1059\r\nitems";
            this.button_perfTabItemsConstructor.UseVisualStyleBackColor = true;
            // 
            // textBox_perfTabSelectedTNType
            // 
            this.textBox_perfTabSelectedTNType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_perfTabSelectedTNType.Location = new System.Drawing.Point(382, 410);
            this.textBox_perfTabSelectedTNType.Name = "textBox_perfTabSelectedTNType";
            this.textBox_perfTabSelectedTNType.ReadOnly = true;
            this.textBox_perfTabSelectedTNType.Size = new System.Drawing.Size(204, 20);
            this.textBox_perfTabSelectedTNType.TabIndex = 218;
            // 
            // button_perfTabRemoveTlvElem
            // 
            this.button_perfTabRemoveTlvElem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_perfTabRemoveTlvElem.Location = new System.Drawing.Point(6, 392);
            this.button_perfTabRemoveTlvElem.Name = "button_perfTabRemoveTlvElem";
            this.button_perfTabRemoveTlvElem.Size = new System.Drawing.Size(41, 27);
            this.button_perfTabRemoveTlvElem.TabIndex = 217;
            this.button_perfTabRemoveTlvElem.Text = "-";
            this.button_perfTabRemoveTlvElem.UseVisualStyleBackColor = true;
            this.button_perfTabRemoveTlvElem.Click += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // button_perfTabAppendTlvElem
            // 
            this.button_perfTabAppendTlvElem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_perfTabAppendTlvElem.Location = new System.Drawing.Point(6, 366);
            this.button_perfTabAppendTlvElem.Name = "button_perfTabAppendTlvElem";
            this.button_perfTabAppendTlvElem.Size = new System.Drawing.Size(41, 26);
            this.button_perfTabAppendTlvElem.TabIndex = 216;
            this.button_perfTabAppendTlvElem.Text = "+";
            this.button_perfTabAppendTlvElem.UseVisualStyleBackColor = true;
            this.button_perfTabAppendTlvElem.Click += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // textBox_perfTab_ftagUFName
            // 
            this.textBox_perfTab_ftagUFName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_perfTab_ftagUFName.Location = new System.Drawing.Point(319, 368);
            this.textBox_perfTab_ftagUFName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_perfTab_ftagUFName.Name = "textBox_perfTab_ftagUFName";
            this.textBox_perfTab_ftagUFName.ReadOnly = true;
            this.textBox_perfTab_ftagUFName.Size = new System.Drawing.Size(267, 20);
            this.textBox_perfTab_ftagUFName.TabIndex = 215;
            // 
            // treeView_tabPerfStlvMiniConstructor
            // 
            this.treeView_tabPerfStlvMiniConstructor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView_tabPerfStlvMiniConstructor.HideSelection = false;
            this.treeView_tabPerfStlvMiniConstructor.Location = new System.Drawing.Point(56, 365);
            this.treeView_tabPerfStlvMiniConstructor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.treeView_tabPerfStlvMiniConstructor.Name = "treeView_tabPerfStlvMiniConstructor";
            this.treeView_tabPerfStlvMiniConstructor.Size = new System.Drawing.Size(260, 93);
            this.treeView_tabPerfStlvMiniConstructor.TabIndex = 201;
            this.treeView_tabPerfStlvMiniConstructor.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.Stlv_miniConstructorSelecttag);
            // 
            // textBox__tabFdToPerf_EditorValueMain
            // 
            this.textBox__tabFdToPerf_EditorValueMain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox__tabFdToPerf_EditorValueMain.Location = new System.Drawing.Point(382, 431);
            this.textBox__tabFdToPerf_EditorValueMain.Name = "textBox__tabFdToPerf_EditorValueMain";
            this.textBox__tabFdToPerf_EditorValueMain.Size = new System.Drawing.Size(204, 20);
            this.textBox__tabFdToPerf_EditorValueMain.TabIndex = 21;
            this.textBox__tabFdToPerf_EditorValueMain.TextChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // textBox_tabFdToPerf_EditorNumberMain
            // 
            this.textBox_tabFdToPerf_EditorNumberMain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_tabFdToPerf_EditorNumberMain.Location = new System.Drawing.Point(382, 389);
            this.textBox_tabFdToPerf_EditorNumberMain.Name = "textBox_tabFdToPerf_EditorNumberMain";
            this.textBox_tabFdToPerf_EditorNumberMain.ReadOnly = true;
            this.textBox_tabFdToPerf_EditorNumberMain.Size = new System.Drawing.Size(204, 20);
            this.textBox_tabFdToPerf_EditorNumberMain.TabIndex = 19;
            // 
            // label36
            // 
            this.label36.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(322, 435);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(55, 13);
            this.label36.TabIndex = 18;
            this.label36.Text = "Значение";
            // 
            // label35
            // 
            this.label35.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(324, 414);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(26, 13);
            this.label35.TabIndex = 17;
            this.label35.Text = "Тип";
            // 
            // labenumt
            // 
            this.labenumt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labenumt.AutoSize = true;
            this.labenumt.Location = new System.Drawing.Point(321, 392);
            this.labenumt.Name = "labenumt";
            this.labenumt.Size = new System.Drawing.Size(41, 13);
            this.labenumt.TabIndex = 16;
            this.labenumt.Text = "Номер";
            // 
            // groupBox_performTabAll
            // 
            this.groupBox_performTabAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_performTabAll.Controls.Add(this.button_itemsEditor);
            this.groupBox_performTabAll.Controls.Add(this.textBox_perfTabDataForClosing);
            this.groupBox_performTabAll.Controls.Add(this.groupBox_performTabSteps);
            this.groupBox_performTabAll.Controls.Add(this.button__performTabPerformFast);
            this.groupBox_performTabAll.Controls.Add(this.checkBox_perfTabDataForBegin);
            this.groupBox_performTabAll.Controls.Add(this.richTextBox_perfTabDataForBegin);
            this.groupBox_performTabAll.Location = new System.Drawing.Point(588, 29);
            this.groupBox_performTabAll.Name = "groupBox_performTabAll";
            this.groupBox_performTabAll.Size = new System.Drawing.Size(231, 418);
            this.groupBox_performTabAll.TabIndex = 15;
            this.groupBox_performTabAll.TabStop = false;
            // 
            // button_itemsEditor
            // 
            this.button_itemsEditor.Enabled = false;
            this.button_itemsEditor.Location = new System.Drawing.Point(13, 387);
            this.button_itemsEditor.Name = "button_itemsEditor";
            this.button_itemsEditor.Size = new System.Drawing.Size(202, 23);
            this.button_itemsEditor.TabIndex = 225;
            this.button_itemsEditor.Text = "Заполнить предметы расчета";
            this.button_itemsEditor.UseVisualStyleBackColor = true;
            this.button_itemsEditor.Click += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // textBox_perfTabDataForClosing
            // 
            this.textBox_perfTabDataForClosing.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_perfTabDataForClosing.Location = new System.Drawing.Point(5, 284);
            this.textBox_perfTabDataForClosing.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_perfTabDataForClosing.Name = "textBox_perfTabDataForClosing";
            this.textBox_perfTabDataForClosing.ReadOnly = true;
            this.textBox_perfTabDataForClosing.Size = new System.Drawing.Size(220, 32);
            this.textBox_perfTabDataForClosing.TabIndex = 224;
            this.textBox_perfTabDataForClosing.Text = "";
            // 
            // groupBox_performTabSteps
            // 
            this.groupBox_performTabSteps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_performTabSteps.Controls.Add(this.button_perfTabCancelFd);
            this.groupBox_performTabSteps.Controls.Add(this.button_perfTabFinishDocument);
            this.groupBox_performTabSteps.Controls.Add(this.button_sendAllRows);
            this.groupBox_performTabSteps.Controls.Add(this.button_perfTabSendSingleRow);
            this.groupBox_performTabSteps.Controls.Add(this.button_perfTabBeginDocument);
            this.groupBox_performTabSteps.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox_performTabSteps.Location = new System.Drawing.Point(10, 82);
            this.groupBox_performTabSteps.Name = "groupBox_performTabSteps";
            this.groupBox_performTabSteps.Size = new System.Drawing.Size(211, 162);
            this.groupBox_performTabSteps.TabIndex = 13;
            this.groupBox_performTabSteps.TabStop = false;
            this.groupBox_performTabSteps.Text = "Пошаговое формирование ФД";
            // 
            // button_perfTabCancelFd
            // 
            this.button_perfTabCancelFd.Location = new System.Drawing.Point(2, 135);
            this.button_perfTabCancelFd.Name = "button_perfTabCancelFd";
            this.button_perfTabCancelFd.Size = new System.Drawing.Size(203, 23);
            this.button_perfTabCancelFd.TabIndex = 4;
            this.button_perfTabCancelFd.Text = "Отменить документ";
            this.button_perfTabCancelFd.UseVisualStyleBackColor = true;
            this.button_perfTabCancelFd.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // button_perfTabFinishDocument
            // 
            this.button_perfTabFinishDocument.Location = new System.Drawing.Point(2, 106);
            this.button_perfTabFinishDocument.Name = "button_perfTabFinishDocument";
            this.button_perfTabFinishDocument.Size = new System.Drawing.Size(203, 23);
            this.button_perfTabFinishDocument.TabIndex = 3;
            this.button_perfTabFinishDocument.Text = "Завершить документ";
            this.button_perfTabFinishDocument.UseVisualStyleBackColor = true;
            this.button_perfTabFinishDocument.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // button_sendAllRows
            // 
            this.button_sendAllRows.Location = new System.Drawing.Point(2, 77);
            this.button_sendAllRows.Name = "button_sendAllRows";
            this.button_sendAllRows.Size = new System.Drawing.Size(203, 23);
            this.button_sendAllRows.TabIndex = 2;
            this.button_sendAllRows.Text = "Передать все теги";
            this.button_sendAllRows.UseVisualStyleBackColor = true;
            this.button_sendAllRows.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // button_perfTabSendSingleRow
            // 
            this.button_perfTabSendSingleRow.Location = new System.Drawing.Point(2, 48);
            this.button_perfTabSendSingleRow.Name = "button_perfTabSendSingleRow";
            this.button_perfTabSendSingleRow.Size = new System.Drawing.Size(203, 23);
            this.button_perfTabSendSingleRow.TabIndex = 1;
            this.button_perfTabSendSingleRow.Text = "Передать один тег";
            this.button_perfTabSendSingleRow.UseVisualStyleBackColor = true;
            this.button_perfTabSendSingleRow.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // button_perfTabBeginDocument
            // 
            this.button_perfTabBeginDocument.Location = new System.Drawing.Point(2, 19);
            this.button_perfTabBeginDocument.Name = "button_perfTabBeginDocument";
            this.button_perfTabBeginDocument.Size = new System.Drawing.Size(203, 23);
            this.button_perfTabBeginDocument.TabIndex = 0;
            this.button_perfTabBeginDocument.Text = "Начать формирование документа";
            this.button_perfTabBeginDocument.UseVisualStyleBackColor = true;
            this.button_perfTabBeginDocument.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // button__performTabPerformFast
            // 
            this.button__performTabPerformFast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button__performTabPerformFast.Location = new System.Drawing.Point(13, 19);
            this.button__performTabPerformFast.Name = "button__performTabPerformFast";
            this.button__performTabPerformFast.Size = new System.Drawing.Size(203, 23);
            this.button__performTabPerformFast.TabIndex = 14;
            this.button__performTabPerformFast.Text = "Сформировать документ";
            this.button__performTabPerformFast.UseVisualStyleBackColor = true;
            this.button__performTabPerformFast.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // checkBox_perfTabDataForBegin
            // 
            this.checkBox_perfTabDataForBegin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_perfTabDataForBegin.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_perfTabDataForBegin.AutoSize = true;
            this.checkBox_perfTabDataForBegin.Enabled = false;
            this.checkBox_perfTabDataForBegin.Location = new System.Drawing.Point(7, 53);
            this.checkBox_perfTabDataForBegin.Name = "checkBox_perfTabDataForBegin";
            this.checkBox_perfTabDataForBegin.Size = new System.Drawing.Size(130, 23);
            this.checkBox_perfTabDataForBegin.TabIndex = 219;
            this.checkBox_perfTabDataForBegin.Text = "Данные для открытия";
            this.checkBox_perfTabDataForBegin.UseVisualStyleBackColor = true;
            // 
            // richTextBox_perfTabDataForBegin
            // 
            this.richTextBox_perfTabDataForBegin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_perfTabDataForBegin.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_perfTabDataForBegin.DetectUrls = false;
            this.richTextBox_perfTabDataForBegin.Location = new System.Drawing.Point(142, 53);
            this.richTextBox_perfTabDataForBegin.Multiline = false;
            this.richTextBox_perfTabDataForBegin.Name = "richTextBox_perfTabDataForBegin";
            this.richTextBox_perfTabDataForBegin.ReadOnly = true;
            this.richTextBox_perfTabDataForBegin.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBox_perfTabDataForBegin.Size = new System.Drawing.Size(82, 21);
            this.richTextBox_perfTabDataForBegin.TabIndex = 220;
            this.richTextBox_perfTabDataForBegin.Text = "";
            this.richTextBox_perfTabDataForBegin.ZoomFactor = 1.3F;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(6, 35);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(146, 13);
            this.label34.TabIndex = 12;
            this.label34.Text = "Список тегов для передачи";
            // 
            // dataGridView_ftagListToPerform
            // 
            this.dataGridView_ftagListToPerform.AllowUserToAddRows = false;
            this.dataGridView_ftagListToPerform.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_ftagListToPerform.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_ftagListToPerform.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnTagNamber,
            this.ColumnType,
            this.Column_tagUfName,
            this.ColumnValue,
            this.ColumnStatus});
            this.dataGridView_ftagListToPerform.Location = new System.Drawing.Point(3, 51);
            this.dataGridView_ftagListToPerform.MultiSelect = false;
            this.dataGridView_ftagListToPerform.Name = "dataGridView_ftagListToPerform";
            this.dataGridView_ftagListToPerform.ReadOnly = true;
            this.dataGridView_ftagListToPerform.RowHeadersWidth = 20;
            this.dataGridView_ftagListToPerform.Size = new System.Drawing.Size(581, 312);
            this.dataGridView_ftagListToPerform.TabIndex = 11;
            this.dataGridView_ftagListToPerform.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.FdPerfGridRowSelect);
            // 
            // ColumnTagNamber
            // 
            this.ColumnTagNamber.FillWeight = 30F;
            this.ColumnTagNamber.HeaderText = "Тег";
            this.ColumnTagNamber.MinimumWidth = 6;
            this.ColumnTagNamber.Name = "ColumnTagNamber";
            this.ColumnTagNamber.ReadOnly = true;
            this.ColumnTagNamber.Width = 50;
            // 
            // ColumnType
            // 
            this.ColumnType.FillWeight = 60F;
            this.ColumnType.HeaderText = "Название тега";
            this.ColumnType.MinimumWidth = 6;
            this.ColumnType.Name = "ColumnType";
            this.ColumnType.ReadOnly = true;
            this.ColumnType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnType.Width = 120;
            // 
            // Column_tagUfName
            // 
            this.Column_tagUfName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column_tagUfName.HeaderText = "Наименование реквизита";
            this.Column_tagUfName.MinimumWidth = 6;
            this.Column_tagUfName.Name = "Column_tagUfName";
            this.Column_tagUfName.ReadOnly = true;
            // 
            // ColumnValue
            // 
            this.ColumnValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnValue.FillWeight = 80F;
            this.ColumnValue.HeaderText = "Значение";
            this.ColumnValue.MinimumWidth = 6;
            this.ColumnValue.Name = "ColumnValue";
            this.ColumnValue.ReadOnly = true;
            // 
            // ColumnStatus
            // 
            this.ColumnStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnStatus.FillWeight = 70F;
            this.ColumnStatus.HeaderText = "Статус";
            this.ColumnStatus.MinimumWidth = 6;
            this.ColumnStatus.Name = "ColumnStatus";
            this.ColumnStatus.ReadOnly = true;
            // 
            // label_PerfTabtimeForFd
            // 
            this.label_PerfTabtimeForFd.AutoSize = true;
            this.label_PerfTabtimeForFd.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_PerfTabtimeForFd.Location = new System.Drawing.Point(588, 4);
            this.label_PerfTabtimeForFd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_PerfTabtimeForFd.Name = "label_PerfTabtimeForFd";
            this.label_PerfTabtimeForFd.Size = new System.Drawing.Size(159, 18);
            this.label_PerfTabtimeForFd.TabIndex = 10;
            this.label_PerfTabtimeForFd.Text = "15.09.2024 22:22:22";
            // 
            // comboBox_performTabFfdToPerform
            // 
            this.comboBox_performTabFfdToPerform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_performTabFfdToPerform.FormattingEnabled = true;
            this.comboBox_performTabFfdToPerform.Items.AddRange(new object[] {
            "1.05",
            "1.1",
            "1.2"});
            this.comboBox_performTabFfdToPerform.Location = new System.Drawing.Point(521, 3);
            this.comboBox_performTabFfdToPerform.Name = "comboBox_performTabFfdToPerform";
            this.comboBox_performTabFfdToPerform.Size = new System.Drawing.Size(53, 21);
            this.comboBox_performTabFfdToPerform.TabIndex = 9;
            this.comboBox_performTabFfdToPerform.SelectedIndexChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(468, 6);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(38, 13);
            this.label33.TabIndex = 8;
            this.label33.Text = "ФФД";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(8, 6);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(108, 13);
            this.label32.TabIndex = 7;
            this.label32.Text = "Выберите документ";
            // 
            // comboBox_performTabDocType
            // 
            this.comboBox_performTabDocType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_performTabDocType.FormattingEnabled = true;
            this.comboBox_performTabDocType.Items.AddRange(new object[] {
            "[1] =    Отчет о регистрации",
            "[2] =    Открытие смены",
            "[3] =    Кассовый чек(4-БСО)",
            "[5] =    Закрытие смены",
            "[6] =    Закрытие ФН",
            "[11] =   Перерегистрация",
            "[21] =   Отчет о состоянии расчетов",
            "[31] =   Чек коррекции(41-БСО к.)"});
            this.comboBox_performTabDocType.Location = new System.Drawing.Point(154, 3);
            this.comboBox_performTabDocType.Name = "comboBox_performTabDocType";
            this.comboBox_performTabDocType.Size = new System.Drawing.Size(298, 21);
            this.comboBox_performTabDocType.TabIndex = 6;
            this.comboBox_performTabDocType.SelectedIndexChanged += new System.EventHandler(this.CommonConrolsEventHandler);
            // 
            // textBox_downMsgLeft
            // 
            this.textBox_downMsgLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox_downMsgLeft.BackColor = System.Drawing.Color.Silver;
            this.textBox_downMsgLeft.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_downMsgLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox_downMsgLeft.Location = new System.Drawing.Point(5, 492);
            this.textBox_downMsgLeft.Margin = new System.Windows.Forms.Padding(0);
            this.textBox_downMsgLeft.Name = "textBox_downMsgLeft";
            this.textBox_downMsgLeft.ReadOnly = true;
            this.textBox_downMsgLeft.Size = new System.Drawing.Size(314, 14);
            this.textBox_downMsgLeft.TabIndex = 1;
            // 
            // textBox_downMidleMsg
            // 
            this.textBox_downMidleMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_downMidleMsg.BackColor = System.Drawing.Color.Silver;
            this.textBox_downMidleMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_downMidleMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox_downMidleMsg.Location = new System.Drawing.Point(320, 492);
            this.textBox_downMidleMsg.Margin = new System.Windows.Forms.Padding(0);
            this.textBox_downMidleMsg.Name = "textBox_downMidleMsg";
            this.textBox_downMidleMsg.ReadOnly = true;
            this.textBox_downMidleMsg.Size = new System.Drawing.Size(279, 14);
            this.textBox_downMidleMsg.TabIndex = 2;
            // 
            // progressBar_downProgress
            // 
            this.progressBar_downProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar_downProgress.ForeColor = System.Drawing.Color.DimGray;
            this.progressBar_downProgress.Location = new System.Drawing.Point(604, 489);
            this.progressBar_downProgress.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.progressBar_downProgress.Name = "progressBar_downProgress";
            this.progressBar_downProgress.Size = new System.Drawing.Size(139, 19);
            this.progressBar_downProgress.TabIndex = 3;
            // 
            // button_brakeOperation
            // 
            this.button_brakeOperation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_brakeOperation.Location = new System.Drawing.Point(747, 488);
            this.button_brakeOperation.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_brakeOperation.Name = "button_brakeOperation";
            this.button_brakeOperation.Size = new System.Drawing.Size(74, 21);
            this.button_brakeOperation.TabIndex = 6;
            this.button_brakeOperation.Text = "Прервать";
            this.button_brakeOperation.UseVisualStyleBackColor = true;
            this.button_brakeOperation.Click += new System.EventHandler(this.ExampleFnRequest);
            // 
            // TerminalUi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 510);
            this.Controls.Add(this.button_brakeOperation);
            this.Controls.Add(this.progressBar_downProgress);
            this.Controls.Add(this.textBox_downMidleMsg);
            this.Controls.Add(this.textBox_downMsgLeft);
            this.Controls.Add(this.tabControlSpace);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimumSize = new System.Drawing.Size(750, 427);
            this.Name = "TerminalUi";
            this.Text = "TerminalFn";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StopUpdateTime);
            this.tabControlSpace.ResumeLayout(false);
            this.tabPage_connectionParams.ResumeLayout(false);
            this.tabPage_connectionParams.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tabControl_timeModuleMainSwitcher.ResumeLayout(false);
            this.tabPage_shiftedTime.ResumeLayout(false);
            this.tabPage_shiftedTime.PerformLayout();
            this.tabPage_useLastFd.ResumeLayout(false);
            this.tabPage_useLastFd.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_plusMin)).EndInit();
            this.tabPage_xcg.ResumeLayout(false);
            this.tabPage_xcg.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage_fnCommands.ResumeLayout(false);
            this.tabPage_fnCommands.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage_FnInfo.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_regParams)).EndInit();
            this.tabPage_readFd.ResumeLayout(false);
            this.tabPage_readFd.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_fdNumber2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_fdNumber1)).EndInit();
            this.tabPage_createFdRules.ResumeLayout(false);
            this.tabPage_createFdRules.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_termFdRules)).EndInit();
            this.tabPage_fdAction.ResumeLayout(false);
            this.tabPage_fdAction.PerformLayout();
            this.groupBox_performTabAll.ResumeLayout(false);
            this.groupBox_performTabAll.PerformLayout();
            this.groupBox_performTabSteps.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ftagListToPerform)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlSpace;
        private System.Windows.Forms.TabPage tabPage_xcg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_xcgMessage;
        private System.Windows.Forms.TextBox textBox_xcgRawData;
        private System.Windows.Forms.TabPage tabPage_connectionParams;
        private System.Windows.Forms.ComboBox comboBox_connsttsBaudrate;
        private System.Windows.Forms.ComboBox comboBox_connsttsPortName;
        private System.Windows.Forms.ComboBox comboBox_xcgMsgFormat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox_xcgOpenPort;
        private System.Windows.Forms.Button button_xcgClean;
        private System.Windows.Forms.Button button_xcgSend;
        private System.Windows.Forms.TextBox textBox_connTimeout;
        private System.Windows.Forms.Label label4;
        private Label label_settsAvailabilityMsg;
        private CheckBox checkBox_xcgExtendedLog;
        private Button button_xcgCrcCcit;
        private GroupBox groupBox1;
        private CheckBox checkBox_xcgIgnoreStartSign;
        private ToolTip toolTip1;
        private TabPage tabPage_fnCommands;
        private Label label_fnc1;
        private ComboBox comboBox_fncCommand;
        private TextBox textBox_outDataConstructor;
        private Label label5;
        private CheckBox checkBox_connectConnectionParamsTab;
        private Button button_updateComNames;
        private TextBox textBox_showCmdParams;
        private TextBox textBox_valueAddToConstructor;
        private Button button_addData;
        private GroupBox groupBox2;
        private RadioButton radioButton_hexData;
        private RadioButton radioButton_cconstructorDtFormat;
        private RadioButton radioButton_cconstructorAsciiStr;
        private RadioButton radioButton_cconstructorUint32;
        private RadioButton radioButton_cconstructorUint16;
        private RadioButton radioButton_cconstructorByte;
        private TextBox textBox_cconstructorLength;
        private Label label6;
        private RadioButton radioButton_cconstructorUint40;
        private Button button_requestFnNumber;
        private RadioButton radioButton_cconstructorDateFormat;
        private TabPage tabPage_FnInfo;
        private Button button_fnGetStatus;
        private Button button_fnInfoGetFfd;
        private CheckBox checkBox_terminalDontLock;
        private GroupBox groupBox4;
        private TabPage tabPage_readFd;
        private NumericUpDown numericUpDown_fdNumber1;
        private Button button_readFdTask;
        private TreeView treeView_fnReadedContent;
        private NumericUpDown numericUpDown_fdNumber2;
        private TextBox textBox_downMsgLeft;
        private TextBox textBox_downMidleMsg;
        private ProgressBar progressBar_downProgress;
        private Label label15;
        private CheckBox checkBox_preferUserFrandly;
        private Button button_brakeOperation;
        private GroupBox groupBox5;
        private Label label17;
        private TextBox textBox_tagInfoTagNumber;
        private Label label16;
        private TextBox textBox_tagInfoRawData;
        private Label label22;
        private TextBox textBox_tagInfoTagDataType;
        private Label label21;
        private TextBox textBox_tagInfoTagNumericValue;
        private Label label20;
        private TextBox textBox_tagInfoStringPresentation;
        private Label label19;
        private TextBox textBox_tagInfoUserFriendlyName;
        private Label label18;
        private TextBox textBox_tagInfoFnsName;
        private Button button_saveFdToFile;
        private ComboBox comboBox_formatToSave;
        private Button button_requestExpirationDate;
        private Button button_parceHexString;
        private TextBox textBox_strHexToParce;
        private TabPage tabPage_createFdRules;
        private Button button_getRegistrationParams;
        private Button button_getFnExchangeStatus;
        private NumericUpDown numericUpDown1;
        private Button button_getArchivedFdInfo;
        private Button button_readFdVariant2;
        private CheckBox checkBox_getExtraTlvInfo;
        private GroupBox groupBox7;
        private TabControl tabControl_timeModuleMainSwitcher;
        private TabPage tabPage_useCurrentTime;
        private TabPage tabPage_shiftedTime;
        private DateTimePicker dateTimePicker_setDtForFd;
        private TabPage tabPage_useLastFd;
        private Label label_timeModuleShowTime;
        private RadioButton radioButton_timerStop1;
        private RadioButton radioButton_timerStart1;
        private NumericUpDown numericUpDown_plusMin;
        private Label label39;
        private RadioButton radioButton_timerStop2;
        private RadioButton radioButton_timerStart2;
        private TextBox textBox2;
        private Label label_dtAlarmingT1;
        private TextBox textBox1;
        private Label label_dtAlarmingT2;
        private Label label38;
        private DataGridView dataGridView_regParams;
        private DataGridViewTextBoxColumn Column_tagNumber;
        private DataGridViewTextBoxColumn Column_tagDescriber;
        private DataGridViewTextBoxColumn Column_tagValue;
        private Button button_getFullFnInfo;
        private TextBox textBox_fnBriefInfo;
        private GroupBox groupBox8;
        private RadioButton radioButton_ffdRulesSwitcher_2;
        private DataGridView dataGridView_termFdRules;
        private RadioButton radioButton_ffdRulesSwitcher_4;
        private RadioButton radioButton_ffdRulesSwitcher_3;
        private ComboBox comboBox_ruleTabFdTypeChooser;
        private Label label31;
        private Button button_saveTFNCommonFdRules;
        private Button button_loadTFNR;
        private TabPage tabPage_fdAction;
        private Label label32;
        private ComboBox comboBox_performTabDocType;
        private ComboBox comboBox_performTabFfdToPerform;
        private Label label33;
        private Label label_PerfTabtimeForFd;
        internal DataGridView dataGridView_ftagListToPerform;
        private GroupBox groupBox_performTabSteps;
        private Button button_perfTabSendSingleRow;
        private Button button_perfTabBeginDocument;
        private Button button_perfTabFinishDocument;
        private Button button_sendAllRows;
        private Button button__performTabPerformFast;
        private GroupBox groupBox_performTabAll;
        private Button button_perfTabCancelFd;
        private DataGridViewTextBoxColumn ColumnTN;
        private DataGridViewTextBoxColumn Column_tn;
        private DataGridViewComboBoxColumn Column_Criticality;
        private DataGridViewComboBoxColumn Column_sourceData;
        private DataGridViewTextBoxColumn Column_dataOverride;
        private Label labenumt;
        private Label label36;
        private Label label35;
        private TextBox textBox__tabFdToPerf_EditorValueMain;
        private TextBox textBox_tabFdToPerf_EditorNumberMain;
        private TreeView treeView_tabPerfStlvMiniConstructor;
        private Button button_fdRulesTabApplyCurrentTable;
        private TextBox textBox_perfTab_ftagUFName;
        private Button button_perfTabRemoveTlvElem;
        private Button button_perfTabAppendTlvElem;
        private TextBox textBox_perfTabSelectedTNType;
        private DataGridViewTextBoxColumn ColumnTagNamber;
        private DataGridViewTextBoxColumn ColumnType;
        private DataGridViewTextBoxColumn Column_tagUfName;
        private DataGridViewTextBoxColumn ColumnValue;
        private DataGridViewTextBoxColumn ColumnStatus;
        private Button button_perfTabItemsConstructor;
        private CheckBox checkBox_perfTabDataForBegin;
        private RichTextBox richTextBox_perfTabDataForBegin;
        private RichTextBox textBox_perfTabDataForClosing;
        private Label label34;
        private RichTextBox richTextBox_fnTechCondition;
        private Button button_getFullFnInfo2;
        private Button button_sampleGetShiftParams;
        private Button button_itemsEditor;
    }
}