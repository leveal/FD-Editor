//using Atol.Drivers10.Fptr;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DrvFRLib;
using static FR_Operator.FiscalPrinter;
using System.Threading;
using System.Text;
using System.IO;
using FR_Operator.Properties;
using System.Reflection;
using System.Timers;

namespace FR_Operator
{
    public partial class MainForm : Form
    {
        public FiscalPrinter fiscalPrinter = null;
        FiscalCheque fFDoc = new FiscalCheque();
        public CheckBox ConnectSwicher = null;
        System.Timers.Timer timer;
        public MainForm()
        {
            InitializeComponent();
            this.Icon = Resources.fd_editpr_16_2;
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime compileDate = new DateTime((v.Build - 1) * TimeSpan.TicksPerDay + v.Revision * TimeSpan.TicksPerSecond * 2).AddYears(1999);
            LogHandle.ol("Application_" + version);
            LogHandle.ol((Environment.Is64BitProcess ? "64bit" : "32Bit")+ " depth");
            TitleStr = "FD editor "+ version+ " [" + compileDate.ToString("dd.MM.yyyy] ") +  (Environment.Is64BitProcess ? "64bit":"32Bit");
            Text = TitleStr;
            comboBox_cheq_docName.SelectedIndex = 0;
            comboBox_cheq_operationSign.SelectedIndex = 0;
            comboBox_cheq_sno.SelectedIndex = 1;
            comboBox_cheq_itemType.SelectedIndex = AppSettings.ItemProductType;
            comboBox_cheq_itemPaymentTypeSign.SelectedIndex = AppSettings.ItemPaymentType;
            comboBox_cheqItemTaxRate.SelectedIndex = AppSettings.ItemTaxRate;
            comboBox_cheque_correctionType.SelectedIndex = 0;
            groupBox_cheque_correctionProperties.Enabled = false;
            comboBox_fastCorrectionsType.SelectedIndex = 0;
            comboBox_fastCorrectionsNds.SelectedIndex = 5;
            comboBox_fasstCorrectionsAutoDate.SelectedIndex = 1;
            comboBox_fastCorrectionsOperationSign.SelectedIndex = 0;
            comboBox_cdOperationType.SelectedIndex = 0;
            comboBox_cheqItemMeasureType.SelectedIndex = 0;
            comboBox_task2CorrectionType.SelectedIndex = 0;
            textBox_task2CorrectionNumber.Text = AppSettings.CorrectionOrderNumberDefault;
            textBox_fastCorrectionsOrderNum.Text = AppSettings.CorrectionOrderNumberDefault;
            textBox_cheq_cashierName.Text = AppSettings.CashierDefault;
            textBox_cheq_cashierInn.Text = AppSettings.CashierInnDefault;

            if (AppSettings.AtolAbility)
            {
                _atolDriver = AtolAdapter.GetDrvVersion;
            }
            else
            {
                _atolDriver = "нет обертки";
                conn_combo_brand.Items[conn_combo_brand.Items.IndexOf("Атол")] = "Атол.Утерян компонент";
            }
            try
            {
                DrvFR shtrih = new DrvFR();
                _shtrihDriverVersion = shtrih.DriverVersion;
            }
            catch { _shtrihDriverVersion = NO_DRIVER_FOUNDED; }
            LogHandle.ol("Штрих-м драйвер " + _shtrihDriverVersion);
            LogHandle.ol("Атол драйвер " + _atolDriver);
            WriteKKMInfo();

            ReadFormFiscalDocument();
            button_cheque_addPosition.Enabled = CheckFormItem();
            this.MaximizeBox = false;
            dataGridView_task.CurrentCellChanged += DataGridView_task_CellEndEdit;
            dataGridView_task.CellEndEdit += DataGridView_task_CellEndEdit;

            radioButton_task2VariantRange.Checked = true;
            comboBox_task2ReadSeparator.SelectedIndex = 6;
            button_olapRepV2.Enabled = !Environment.Is64BitProcess;
            if (string.IsNullOrEmpty(AppSettings.ItemName))
                textBox_cheqItemName.Text = SAMLE_ITEMS[new Random().Next(0, SAMLE_ITEMS.Length - 1)];
            else
                textBox_cheqItemName.Text = AppSettings.ItemName;
            if(AppSettings.ItemPrice>0)
                textBox_cheqItemPrice.Text = AppSettings.ItemPrice.ToString();
            if(AppSettings.ItemQuantity>0)
                textBox_cheqItemQuantity.Text = AppSettings.ItemQuantity.ToString();
            AutoPayBoxesSet();

            _hidedPageOldVar = tabControl_subTask2.TabPages[3];
            tabControl_subTask2.TabPages.Remove(_hidedPageOldVar);


            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add("Копировать ссылку");
            linkLabel1.ContextMenu = cm;
            cm.MenuItems[0].Click += LinkLabel1_copyLink;

            button_readjsonList.Enabled = AppSettings.jsonAvailable;
            radioButton_task2VariantReadJson.Enabled = AppSettings.jsonAvailable;
            


            foreach (DataGridViewColumn column in dataGridView_jsonList.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            ConnectSwicher = conn_ckb_connected;

            checkBox_task2FiltersSwicher.Checked = false;
            timer = new System.Timers.Timer();
            timer.AutoReset = true;
            timer.Elapsed += TimerTick;
            timer.Interval = 765;
            timer.Enabled = true;
            timer.Start();
        }

        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            TerminalFnExchange tfn = null;
            string dtstr = TitleStr;
            if (fiscalPrinter!=null&&fiscalPrinter is TerminalFnExchange)
            {
                tfn = fiscalPrinter as TerminalFnExchange;
                dtstr = TitleStr+"   ДатаВремя для ФД: " + tfn.GetTimeForFd.ToString(DEFAULT_DT_FORMAT);
                if (Text != dtstr)
                {
                    if (InvokeRequired)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            Text = dtstr;
                        }));
                    }
                    else
                    {
                        Text = dtstr;
                    }
                }
            }

            
            
        }

        public string TitleStr = "";
        private void LinkLabel1_copyLink(object sender, EventArgs e)
        {
            Clipboard.SetText("https://docs.google.com/document/d/1ukm31-qrXfWCKJook8FenadMraMBRKa_Cj4_Oqjtxkc/edit?usp=sharing");
        }

        void WriteKKMInfo()
        {
            kkminfo_lb_model_name.Text = KKMInfoTransmitter[FR_MODEL_KEY];
            kkminfo_lb_serial.Text = KKMInfoTransmitter[FR_SERIAL_KEY];
            kkminfo_lb_shiftstate.Text = KKMInfoTransmitter[FR_SHIFT_STATE_KEY];
            kkminfo_lb_fdnumber.Text = KKMInfoTransmitter[FR_LAST_FD_NUMBER_KEY];
            kkminfo_lb_frtime.Text = KKMInfoTransmitter[FR_TIME_KEY];
            kkminfo_lb_firmware.Text = KKMInfoTransmitter[FR_FIRMWARE_KEY];
            kkminfo_lb_ffd.Text = KKMInfoTransmitter[FR_FFDVER_KEY];
            kkminfo_lb_username.Text = UserConversion( KKMInfoTransmitter[FR_OWNER_USER_KEY] );
            kkminfo_lb_usingsno.Text = KKMInfoTransmitter[FR_REGISTERD_SNO_KEY];
            kkminfo_lb_address.Text = KKMInfoTransmitter[FR_OWNER_ADDRESS_KEY];
            kkminfo_lb_ofdExchange.Text = KKMInfoTransmitter[FR_OFD_EXCHANGE_STATUS_KEY];
            kkminfo_lb_mode.Text = KKMInfoTransmitter[FR_STATUS_MODE_KEY];
        }

        private static string _atolDriver = "10.x",
            _shtrihDriverVersion = "4.x";
        public void UpdateUiKkmDescribtion()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => WriteKKMInfo()));
            }
            else
            {
                WriteKKMInfo();
            }
        }
        private void DriverSettingsWindow(object sender, EventArgs e)
        {
            if ((conn_combo_brand.Text == "Атол" && _atolDriver != NO_DRIVER_FOUNDED) || (conn_combo_brand.Text == "Штрих" && _shtrihDriverVersion != NO_DRIVER_FOUNDED) || conn_combo_brand.Text == "Эмулятор ФР" || conn_combo_brand.Text == "ФН(uart)")
            {
                fiscalPrinter.ConnectionWindow();
            }
            else if (conn_combo_brand.Text != "Атол" && conn_combo_brand.Text != "Штрих")
            {
                PushMessage("Не выбрана или выбрана неизвестная ККМ");
            }
            else { PushMessage("Проблема с инициализацией драйвера ККТ"); }
        }

        public void UpdateConnectionParams(string connectionParams)     {   kkminfo_label_connectionParams.Text = connectionParams;  }



        // Нижняя строка ответа на команду ФР
        public void PushMessage(string msg)
        {
            LogHandle.ol("interface reaction: " + msg);
            if (InvokeRequired)
                Invoke(new Action(() => textBox_operationRezult.Text = msg));
            else
                textBox_operationRezult.Text = msg;
        }
        // Запись состояния ФД с окна
        public void FormFiscalDocConditionWrite(string prefix = "")
        {
            string condition = null;
            if (prefix != null && prefix.Trim() != "")
            {
                condition = "* * * * * * * * * *" + Environment.NewLine
                    + prefix + Environment.NewLine
                    + "* * * * * * * * * *" + Environment.NewLine
                    + fFDoc.Condition();
            }
            else
                condition = fFDoc.Condition();
            if (InvokeRequired)
                Invoke(new Action(() => textBox_chequeDocCondition.Text = condition));
            else
                textBox_chequeDocCondition.Text = condition;
        }

        bool _skipSaveUpReq = false;
        // choisebox чек/чек корреции, признак расчета, СНО, реквизиты коррекции
        private void Control_upperReqvChanged(object sender, EventArgs e)
        {
            if(sender is CheckBox &&( sender == checkBox_cheq_sno_locker || sender == checkBox_cheq_operationType_locker || sender == checkBox_cheq_docType_locker))
            {
                CheckBox ch = sender as CheckBox;
                if (ch.Checked)
                {
                    ch.BackgroundImage = Resources.padlock_cheked2;
                }
                else
                {
                    ch.BackgroundImage = Resources.padlock_uncheked;
                }
                return;
            }

            if (sender == comboBox_cheq_docName)
            {
                if (comboBox_cheq_docName.SelectedIndex == 1)
                {
                    groupBox_cheque_correctionProperties.Enabled = true;
                    if(!_skipSaveUpReq) fFDoc.Document = FD_DOCUMENT_NAME_CORRECTION_CHEQUE;
                }
                else
                {
                    groupBox_cheque_correctionProperties.Enabled = false;
                    if(!_skipSaveUpReq) fFDoc.Document = FD_DOCUMENT_NAME_CHEQUE;
                }
            }
            if (_skipSaveUpReq)return;
            if(sender == comboBox_cheq_operationSign)
            {
                fFDoc.CalculationSign = comboBox_cheq_operationSign.SelectedIndex+1;
            }
            else if(sender == comboBox_cheq_sno)
            {
                int sno = (int)Math.Pow(2, comboBox_cheq_sno.SelectedIndex);
                if (sno == 16) sno = 32;
                fFDoc.Sno = sno;
            }
            else if(sender == comboBox_cheque_correctionType)
            {
                fFDoc.CorrectionTypeNotFtag = comboBox_cheque_correctionType.SelectedIndex;
            }
            else if(sender == dateTimePicker_chequeCorrectionDate)
            {
                fFDoc.CorrectionDocumentDate = dateTimePicker_chequeCorrectionDate.Value;
            }
            else if(sender == textBox_cheque_orderNum)
            {
                fFDoc.CorrectionOrderNumber = textBox_cheque_orderNum.Text;
            }
        }

        // соединение с ККТ
        public void ConnectDevice(object sender, EventArgs e)
        {
            if (conn_ckb_connected.Checked)
            {
                this.Enabled = false;
                if (conn_combo_brand.Text == "")
                {
                    PushMessage("Выберите модель ФР.");
                    conn_ckb_connected.Checked = false;
                }
                else
                {
                    if (fiscalPrinter != null)
                        conn_ckb_connected.Checked = fiscalPrinter.Connect();
                    else
                    {
                        PushMessage(NO_DRIVER_FOUNDED);
                        conn_ckb_connected.Checked = false;
                        return;
                    }
                        
                }
                if (conn_ckb_connected.Checked) 
                {
                    conn_ckb_connected.Text = "Разорвать   соединение";
                    int lastFD = -100000;
                    try { lastFD = int.Parse(KKMInfoTransmitter[FR_LAST_FD_NUMBER_KEY]); } catch { };
                    textBox_readerOneNumber.Text = lastFD.ToString();
                    if (lastFD - 50 > 0) textBox_readFD50.Text = (lastFD - 49).ToString();
                    else textBox_readFD50.Text = "3";
                    if (lastFD - 250 > 0) textBox_readFD250.Text = (lastFD - 249).ToString();
                    else textBox_readFD250.Text = "3";
                    switch (fiscalPrinter.ChosenSno)
                    {
                        case 1:
                            comboBox_cheq_sno.SelectedIndex = 0;
                            break;
                        case 2:
                            comboBox_cheq_sno.SelectedIndex = 1;
                            break;
                        case 4:
                            comboBox_cheq_sno.SelectedIndex = 2;
                            break;
                        case 16:
                            comboBox_cheq_sno.SelectedIndex = 3;
                            break;
                        case 32:
                            comboBox_cheq_sno.SelectedIndex = 4;
                            break;
                            default:
                            break;
                    }
                    if(fiscalPrinter.FfdVer >= 120)
                    {
                        comboBox_cheqItemMeasureType.SelectedIndex = 1;
                        checkBox_cheque_buyerFdd120.Checked = true;
                    }
                    else
                    {
                        comboBox_cheqItemMeasureType.SelectedIndex = 0;
                        checkBox_cheque_buyerFdd120.Checked = false;
                    }  
                }
            }
            else
            {
                if (fiscalPrinter != null)
                    fiscalPrinter.Disconnect();
                conn_ckb_connected.Text = "Установить соединение";
            }
            this.Enabled = true;
        }


        public static bool processInterruptor = false;
        private void Button_frNoncheqOperation(object sender, EventArgs e)
        {
            processInterruptor = false;
            KKMInfoTransmitter[FR_CASHIER_NAME_KEY] = textBox_cheq_cashierName.Text;
            if (CorrectInn(textBox_cheq_cashierInn.Text)) KKMInfoTransmitter[FR_CASHIER_INN_KEY] = textBox_cheq_cashierInn.Text;
            this.Enabled = false;

            if (fiscalPrinter != null && fiscalPrinter.IsConnected)
            {
                if (sender == button_closeShift)
                {
                    LogHandle.ol("Закрываем смену");
                    if (conn_ckb_connected.Checked)
                    {
                        fiscalPrinter.CloseShift();
                        fiscalPrinter.ReadDeviceCondition();
                    }
                }
                else if (sender == button_openShift)
                {
                    LogHandle.ol("Открываем смену");
                    if (conn_ckb_connected.Checked)
                    {
                        fiscalPrinter.OpenShift();
                        fiscalPrinter.ReadDeviceCondition();
                    }
                }
                else if(sender == button_cancelDocument)
                {
                    if(fiscalPrinter.CancelDocument())
                        fiscalPrinter.ReadDeviceCondition();
                }
                else if (sender == button_readSingleDocumentNumber)
                {
                    int docNumber = -1;
                    FnReadedDocument readed = FnReadedDocument.EmptyFD;
                    try { docNumber = int.Parse(textBox_readerOneNumber.Text); } catch { }
                    if (docNumber >= 0)
                    {
                        readed = fiscalPrinter.ReadFD(docNumber, true);

                        if (readed.Type != NONE)
                        {
                            pane_tabPage_frFdContent01.Controls.Clear();
                            AddSingleReadedFd(0, readed);
                            if (readed.Cheque != null)
                            {
                                fFDoc = readed.Cheque;
                                if (fFDoc.Document != FD_DOCUMENT_NAME_CORRECTION_CHEQUE)
                                    dateTimePicker_chequeCorrectionDate.Value = readed.Time;
                                textBox_list1readedFdNum.Text = docNumber.ToString();
                                PushMessage(SUCCESS_MSG);
                            }
                            else
                            {
                                fFDoc = new FiscalCheque();
                            }
                            MapFormFiscalDocument();
                        }
                    }
                    PushMessage("Не прочитано");
                }
                else if (sender == button_kkmInfoUpdate)
                {
                    if (fiscalPrinter != null && fiscalPrinter.IsConnected)
                    {
                        fiscalPrinter.ReadDeviceCondition();
                        PushMessage("Состояние ККМ Обновлено.");
                        var lastFD = int.Parse(KKMInfoTransmitter[FR_LAST_FD_NUMBER_KEY]);
                        textBox_readerOneNumber.Text = lastFD.ToString();
                        if (lastFD - 50 > 0) textBox_readFD50.Text = (lastFD - 49).ToString();
                        else textBox_readFD50.Text = "2";
                        if (lastFD - 250 > 0) textBox_readFD250.Text = (lastFD - 249).ToString();
                        else textBox_readFD250.Text = "2";
                    }
                    else
                    {
                        PushMessage("Соединение не установлено.");
                    }
                }
                else if (sender == button_readFiftyDocuments)
                {
                    pane_tabPage_frFdContent01.Controls.Clear();
                    int docNumber = -100;
                    FnReadedDocument readed;
                    try { docNumber = int.Parse(textBox_readFD50.Text); } catch { }
                    fiscalPrinter.ReadDeviceCondition();
                    if (docNumber > fiscalPrinter.LastFd)
                    {
                        PushMessage("Неправильный номер первого докуменнта");
                        this.Enabled = true;
                        return;
                    }
                    if (docNumber >= -49)
                    {
                        List<FnReadedDocument> listFd = new List<FnReadedDocument>();
                        for (int i = docNumber; i < docNumber + 50 && i <= fiscalPrinter.LastFd; i++)
                        {
                            readed = FnReadedDocument.EmptyFD;
                            if (i > 0)
                                readed = fiscalPrinter.ReadFD(i);
                            if (!readed.Equals(FnReadedDocument.EmptyFD))
                                listFd.Add(readed);
                        }
                        int docs = listFd.Count - 1;
                        for (int i = 0; i <= docs; i++)
                            AddSingleReadedFd(i, listFd[docs - i]);
                        PushMessage(SUCCESS_MSG);
                    }
                }
                else if (sender == button_readTwoFiftyDocuments)
                {
                    // создаем новый поток
                    Thread myThread = new Thread(new ThreadStart(Run_status_window));
                    myThread.Start(); // запускаем поток
                    fiscalPrinter.ReadDeviceCondition();
                    pane_tabPage_frFdContent01.Controls.Clear();
                    int docNumber = -100;
                    FnReadedDocument readed;
                    try { docNumber = int.Parse(textBox_readFD250.Text); } catch { }
                    if(docNumber > fiscalPrinter.LastFd)
                    {
                        this.Enabled = true;
                        PushMessage("Неправильный номер первого документа");
                        return;
                    }
                    if (docNumber >= -49)
                    {
                        List<FnReadedDocument> listFd = new List<FnReadedDocument>();
                        for (int i = docNumber; i < docNumber + 250 && i<= fiscalPrinter.LastFd; i++)
                        {
                            if (processInterruptor) break;
                            readed = FnReadedDocument.EmptyFD;
                            if (i % 10 == 0 && status !=null)
                                status.Message("Прочитан документ " + i);
                            if (i > 0)
                                readed = fiscalPrinter.ReadFD(i);
                            if (!readed.Equals(FnReadedDocument.EmptyFD)) 
                            { 
                                listFd.Add(readed); 
                            }               
                            else
                            {
                                if (i - 1 > int.Parse(KKMInfoTransmitter[FR_LAST_FD_NUMBER_KEY])) break;
                            }
                        }
                        int docs = listFd.Count - 1;
                        for (int i = 0; i <= docs; i++)
                            AddSingleReadedFd(i, listFd[docs - i]);
                        status.AllDone();
                        this.Activate();
                        PushMessage(SUCCESS_MSG);
                    }
                }
                else if (sender == button_readFdArbitrary)
                {
                    if (fiscalPrinter != null && fiscalPrinter.IsConnected)
                    {
                        bool filtrateDates = checkBox_read2Dates.Checked;
                        DateTime fdFrom = dateTimePicker_read2From.Value.Date;
                        DateTime fdTo = dateTimePicker_read2To.Value.Date.AddDays(1).AddMilliseconds(-1);

                        fiscalPrinter.ReadDeviceCondition();
                        checkedListBox_fdListV2.Items.Clear();
                        int docsToRead = 0, docFrom = 0;

                        List<FnReadedDocument> listFd = new List<FnReadedDocument>();
                        FnReadedDocument readed;

                        try
                        {
                            docsToRead = int.Parse(textBox_read2FdCount.Text);
                            docFrom = int.Parse(textBox_read2FdFirst.Text);
                        }
                        catch { }
                        if (docFrom>fiscalPrinter.LastFd)
                        {
                            this.Enabled = true;
                            return;
                        }
                        Thread myThread = new Thread(new ThreadStart(Run_status_window));
                        myThread.Start(); // запускаем поток
                        if (fiscalPrinter is FrEmulator)//задержка для создания окна(эмулятор слишком быстро читает)
                        {
                            Thread.Sleep(500);
                        }

                        int chequesCountr = 0;
                        double chequesSum = 0;

                        int lastFd = docsToRead + docFrom;
                        if (lastFd > fiscalPrinter.LastFd)
                        {
                            lastFd = fiscalPrinter.LastFd+1;
                        }
                        for (int i = docFrom; i < lastFd; i++)
                        {
                            if (processInterruptor) break;
                            readed = FnReadedDocument.EmptyFD;
                            if (i % 10 == 0 && status != null)
                                status.Message("Read:" + i + " of " + lastFd);
                            if (i > 0)
                                readed = fiscalPrinter.ReadFD(i);
                            if (!readed.Equals(FnReadedDocument.EmptyFD))
                            {
                                if (filtrateDates && (readed.Time < fdFrom || readed.Time > fdTo))
                                    continue;
                                checkedListBox_fdListV2.Items.Add(readed.ReeprezentOL);
                                if(readed.Type == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE
                                    || readed.Type == FTAG_FISCAL_DOCUMENT_TYPE_BSO
                                    || readed.Type == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE
                                    || readed.Type == FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION)
                                {
                                    chequesCountr++;
                                    chequesSum += readed.Summ;
                                }
                            }
                            else
                            {
                                
                            }
                        }
                        status.AllDone();
                        this.Activate();
                        PushMessage(SUCCESS_MSG);

                        textBox_chequesTotalsCount.Text = "Чеков(БСО) " + chequesCountr + " Сумма итогов " + Math.Round(chequesSum,2);
                    }
                }
                else if (sender == button_rePrint)
                {
                    if (fiscalPrinter != null && fiscalPrinter.IsConnected)
                    {
                        fiscalPrinter.ContinuePrint();
                        fiscalPrinter.ReadDeviceCondition();
                    }
                }
                else if (sender == button_read2FromTo)
                {
                    bool filtrateDates = checkBox_read2Dates.Checked;
                    DateTime fdFrom = dateTimePicker_read2From.Value.Date;
                    DateTime fdTo = dateTimePicker_read2To.Value.Date.AddDays(1).AddMilliseconds(-1);
                    checkedListBox_fdListV2.Items.Clear();
                    int chequesCountr = 0;
                    double chequesSum = 0;
                    int r2From = 3, 
                        r2To = 4;
                    try
                    {
                        r2From = int.Parse(textBox_read2From.Text);
                        r2To = int.Parse(textBox_read2To.Text);
                        if (r2To > fiscalPrinter.LastFd) r2To = fiscalPrinter.LastFd;
                        if (r2From <= r2To && r2From > 0)
                        {
                            //List<FnReadedDocument> listFd = new List<FnReadedDocument>();
                            FnReadedDocument readed;
                            Thread myThread = new Thread(new ThreadStart(Run_status_window));
                            myThread.Start(); // запускаем поток
                            if (fiscalPrinter is FrEmulator)
                            {
                                Thread.Sleep(500);
                            }
                            for (int i = r2From; i <= r2To; i++)
                            {
                                readed = FnReadedDocument.EmptyFD;
                                if (i % 10 == 0 && status != null)
                                    status.Message("Read:" + i + " of " + r2To);
                                readed = fiscalPrinter.ReadFD(i);
                                if (!readed.Equals(FnReadedDocument.EmptyFD))
                                {
                                    if (filtrateDates && (readed.Time < fdFrom || readed.Time > fdTo))
                                        continue;
                                    checkedListBox_fdListV2.Items.Add(readed.ReeprezentOL);
                                    if (readed.Type == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE 
                                        || readed.Type == FTAG_FISCAL_DOCUMENT_TYPE_BSO 
                                        || readed.Type == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE 
                                        || readed.Type == FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION)
                                    {
                                        chequesCountr++;
                                        chequesSum += readed.Summ;
                                    }
                                }
                                if (processInterruptor) break;    
                            }
                            status.AllDone();
                            this.Activate();
                            PushMessage(SUCCESS_MSG);
                            textBox_chequesTotalsCount.Text = "Чеков(БСО) " + chequesCountr + " Сумма итогов " + Math.Round(chequesSum,2);

                        }
                        else
                        {
                            PushMessage("Проблема с номерами");
                        }
                    }
                    catch
                    {
                        PushMessage("Не удалось распознать номера");
                    }
                }
                else if (sender == button_cashOperation)
                {
                    try
                    {
                        double cashIncome = double.Parse(ReplaceBadDecimalSeparatorPoint(textBox_cdOperationValue.Text));
                        if(cashIncome > 0)
                        {
                            fiscalPrinter.CashRefill(cashIncome, comboBox_cdOperationType.SelectedIndex == 0);
                        }
                    }
                    catch(Exception ex) { PushMessage(ex.Message); }
                }
            }
            else
            {
                PushMessage(CONNECTION_NOT_ESTABLISHED);
            }
            processInterruptor = false;
            this.Enabled = true;
        }

        ProcessingStatus status;
        void Run_status_window()
        {
            status = new ProcessingStatus();
            status.Focus();
            status.Show();
            status.WindowState = FormWindowState.Normal;
            Application.Run(status);
        }


        // выбор модели ККМ
        private void ConnectionStateChanging(object sender, EventArgs e)
        {
            if (conn_combo_brand.Text == "Атол")
            {
                conn_lb_driver_ver.Text = _atolDriver;


                if (_atolDriver != NO_DRIVER_FOUNDED)
                {
                    if (fiscalPrinter != null && fiscalPrinter.GetType() != typeof(AtolAdapter))
                    {
                        if (conn_ckb_connected.Checked)
                            conn_ckb_connected.Checked = false;
                        fiscalPrinter.ReleaseLib();
                        fiscalPrinter = null;
                        GC.Collect();
                    }
                    if (fiscalPrinter == null)
                        fiscalPrinter = new AtolAdapter(this);
                    kkminfo_label_connectionParams.Text = FiscalPrinter.KKMInfoTransmitter[FR_CONNECTION_SETTINGS_KEY];
                }
                else { PushMessage("Проблема с инициализацией драйвера ККТ"); }
            }
            else if (conn_combo_brand.Text == "Штрих")
            {
                conn_lb_driver_ver.Text = _shtrihDriverVersion;
                if (_shtrihDriverVersion != NO_DRIVER_FOUNDED)
                {
                    if (fiscalPrinter != null && fiscalPrinter.GetType() != typeof(ShtrihAdapter))
                    {
                        if (conn_ckb_connected.Checked)
                            conn_ckb_connected.Checked = false;
                        fiscalPrinter.ReleaseLib();
                        fiscalPrinter = null;
                        GC.Collect();
                    }
                    if (fiscalPrinter == null)
                        fiscalPrinter = new ShtrihAdapter(this);
                    kkminfo_label_connectionParams.Text = KKMInfoTransmitter[FR_CONNECTION_SETTINGS_KEY];
                }
                else { PushMessage("Проблема с инициализацией драйвера ККТ"); }
            }
            else if(conn_combo_brand.Text == "Эмулятор ФР")
            {
                if (fiscalPrinter != null && fiscalPrinter.GetType() != typeof(FrEmulator))
                {
                    if (conn_ckb_connected.Checked)
                        conn_ckb_connected.Checked = false;
                    fiscalPrinter.ReleaseLib();
                    fiscalPrinter = null;
                    GC.Collect();
                }
                fiscalPrinter = new FrEmulator(this);
                kkminfo_label_connectionParams.Text = FiscalPrinter.KKMInfoTransmitter[FR_CONNECTION_SETTINGS_KEY];
                conn_ckb_connected.Checked = true;
            }
            else if(conn_combo_brand.Text == "ФН(uart)")
            {
                if (fiscalPrinter != null && fiscalPrinter.GetType() != typeof(TerminalFnExchange))
                {
                    if (conn_ckb_connected.Checked)
                        conn_ckb_connected.Checked = false;
                    fiscalPrinter.ReleaseLib();
                    fiscalPrinter = null;
                    GC.Collect();
                }
                fiscalPrinter = new TerminalFnExchange(this);
                kkminfo_label_connectionParams.Text = FiscalPrinter.KKMInfoTransmitter[FR_CONNECTION_SETTINGS_KEY];
                //conn_ckb_connected.Checked = true;
            }

        }

        private void CheckBox_dontPrint_CheckedChanged(object sender, EventArgs e)
        {
            if (fiscalPrinter == null || !fiscalPrinter.IsConnected)
                checkBox_dontPrint.Checked = false;
            else
                fiscalPrinter.DontPrint = checkBox_dontPrint.Checked;
        }

        void ReadFormFiscalDocument()
        {
            
            if (fFDoc == null)
            {
                LogHandle.ol("Create new interface cheque");
                fFDoc = new FiscalCheque();
            }
            // тег 1192
            fFDoc.PropertiesData = textBox_chequePropertiesData.Text;
            //1085
            fFDoc.PropertiesPropertyName = textBox_cheque1085propertiesPropertyName.Text;
            //1086
            fFDoc.PropertiesPropertyValue = textBox_cheque1086propertiesPropertyValue.Text;
            // тип документа
            if (comboBox_cheq_docName.Text == "Кассовый чек")
            {
                fFDoc.Document = FD_DOCUMENT_NAME_CHEQUE;
            }
            else if (comboBox_cheq_docName.Text == "Чек коррекции")
            {
                fFDoc.Document = FD_DOCUMENT_NAME_CORRECTION_CHEQUE;
            }
            // признак расчета
            fFDoc.CalculationSign = FD_CALCULATION_SIGN + comboBox_cheq_operationSign.Text[0] - '0';
            // СНО
            string snoInterf = comboBox_cheq_sno.Text;
            if (snoInterf == "ОСНО")
            {
                fFDoc.Sno = FR_SNO_OSN;
            }
            else if (snoInterf == "УСНД")
            {
                fFDoc.Sno = FR_SNO_USN_D;
            }
            else if (snoInterf == "УСНДР")
            {
                fFDoc.Sno = FR_SNO_USN_D_R;
            }
            else if (snoInterf == "ЕСХН")
            {
                fFDoc.Sno = FR_SNO_ESHN;
            }
            else if (snoInterf == "ПСН")
            {
                fFDoc.Sno = FR_SNO_PSN;
            }
            // отправка электнонного чека    
            if (textBox_cheq_byyerEmail.Text != "")
            {
                fFDoc.EmailPhone = textBox_cheq_byyerEmail.Text;
            }
            // Кассир
            if (textBox_cheq_cashierName.Text != "")
            {
                fFDoc.Cashier = textBox_cheq_cashierName.Text;
                fFDoc.CashierInn = textBox_cheq_cashierInn.Text;
            }
            // Сведения о покупателе
            if (textBox_cheq_byerName.Text != "")
            {
                fFDoc.BuyerInformationBuyer = textBox_cheq_byerName.Text;
            }
            if (textBox_cheq_buyerInn.Text != "")
            {
                fFDoc.BuyerInformationBuyerInn = textBox_cheq_buyerInn.Text;
            }
            // реквизиты коррекции 

            if (comboBox_cheque_correctionType.Text[0] == '0')
            {
                fFDoc.CorrectionTypeNotFtag = FD_CORRECTION_TYPE_SELF_MADE;
            }
            else if (comboBox_cheque_correctionType.Text[0] == '1')
            {
                fFDoc.CorrectionTypeNotFtag = FD_CORRECTION_TYPE_BY_ORDER;
            }
            fFDoc.CorrectionDocDescriber = textBox_cheque_correctionDescribtion.Text;
            fFDoc.CorrectionOrderNumber = textBox_cheque_orderNum.Text;
            fFDoc.CorrectionDocumentDate = dateTimePicker_chequeCorrectionDate.Value;

            // добавить доп реквизиты 

            // итог чека
            fFDoc.TotalSum = double.Parse(textBox_chequeTotalSum.Text);
            // оплата
            double cash = 0;
            try { cash = double.Parse(textBox_cheq_payCash.Text); } catch { }
            fFDoc.Cash = cash;
            double cashless = 0;
            try { cashless = double.Parse(textBox_cheq_payCashless.Text); } catch { }
            fFDoc.ECash = cashless;
            double prepaid = 0;
            try { prepaid = double.Parse(textBox_cheq_payAvans.Text); } catch { }
            fFDoc.Prepaid = prepaid;
            double postpaid = 0;
            try { postpaid = double.Parse(textBox_cheq_payCredit.Text); } catch { }
            fFDoc.Credit = postpaid;
            double provision = 0;
            try { provision = double.Parse(textBox_cheq_payProvision.Text); } catch { }
            fFDoc.Provision = provision;
            // налоги
            double ndsFree = 0;
            try { ndsFree = double.Parse(textBox_taxPanelNdsFree.Text); } catch { }
            fFDoc.NdsFree = ndsFree;
            double nds20 = 0;
            try { nds20 = double.Parse(textBox_taxPanelNds20.Text); } catch { }
            fFDoc.Nds20 = nds20;
            double nds10 = 0;
            try { nds10 = double.Parse(textBox_taxPanelNds10.Text); } catch { }
            fFDoc.Nds10 = nds10;
            double nds0 = 0;
            try { nds0 = double.Parse(textBox_taxPanelNds0.Text); } catch { }
            fFDoc.Nds0 = nds0;
            double nds20120 = 0;
            try { nds20120 = double.Parse(textBox_taxPanelNds20120.Text); } catch { }
            fFDoc.Nds20120 = nds20120;
            double nds10110 = 0;
            try { nds10110 = double.Parse(textBox_taxPanelNds10110.Text); } catch { }
            fFDoc.Nds10110 = nds10110;

            double nds5 = 0;
            try { nds5 = double.Parse(textBox_taxPanelNds5.Text); } catch { }
            fFDoc.Nds5 = nds5;

            double nds7= 0;
            try { nds7 = double.Parse(textBox_taxPanelNds7.Text); } catch { }
            fFDoc.Nds7 = nds7;

            double nds5105 = 0;
            try { nds5105 = double.Parse(textBox_taxPanelNds5105.Text); } catch { }
            fFDoc.Nds5105 = nds5105;

            double nds7107 = 0;
            try { nds7107 = double.Parse(textBox_taxPanelNds7107.Text); } catch { }
            fFDoc.Nds7107 = nds7107;

            // обсчет и проверка документа
            double totalSum = fFDoc.TotalSum;
            fFDoc.Control();
            if(_roundTally && fFDoc.TotalSum > totalSum && (((int)fFDoc.TotalSum ) == ((int)totalSum) ))fFDoc.TotalSum = totalSum;
        }


        

        private void Button_cheqExecute_Click(object sender, EventArgs e)
        {
            if (fiscalPrinter != null && fiscalPrinter.IsConnected)
            {
                if (fFDoc.IsNotPaid&&!_roundTally)
                {
                    Button btn = null;
                    if (checkBox_autoPayCash.Checked)
                        btn = button_paySurchargeCash;
                    else if (checkBox_autoPayECash.Checked)
                        btn = button_paySurchargeECash;
                    else if (checkBox_autoPayPrepay.Checked)
                        btn = button_paySurchargePrepay;
                    else if (checkBox_autoPayCredit.Checked)
                        btn = button_paySurchargeCredit;
                    else if (checkBox_autoPayProvision.Checked)
                        btn = button_paySurchargeProvision;
                    if (btn != null)
                        ChangeChequeItemFiedls(btn, null);
                    else
                    {
                        PushMessage("Документ не оплачен");
                        return;
                    }
                }

                ReadFormFiscalDocument();
                if(sender == button_chequeLookPf)
                {
                    FormPrelookFd prelook = new FormPrelookFd(fFDoc.ToString(FiscalCheque.EXTENDED_PF));
                    prelook.ShowDialog();
                    return;
                }

                this.Enabled = false;
                if (fiscalPrinter.PerformFD(fFDoc))
                    PushMessage("Документ оформлен");
                this.Enabled = true;

                fiscalPrinter.ReadDeviceCondition();
            }

            else
                PushMessage(CONNECTION_NOT_ESTABLISHED);
        }


        // добавляем/меняем предмет расчета
        private void Button_cheque_addPosition_Click(object sender, EventArgs e)
        {
            ConsumptionItem item;
            int itemsWas = fFDoc.Items.Count;
            if (_editMode == EDIT_MODE_APPEND)
            {
                item = new ConsumptionItem();
                fFDoc.Items.Add(item);
            }
            else if (_editMode > EDIT_MODE_APPEND && _editMode < itemsWas)
            {
                item = fFDoc.Items[_editMode];
            }
            else
            {
                FormFiscalDocConditionWrite("Проблема с количеством предметов расчета докумета");
                return;
            }

            item.Name = textBox_cheqItemName.Text;
            double price = 0;
            try { price = Math.Round(double.Parse(textBox_cheqItemPrice.Text), 2); } catch { }
            item.Price = price;

            double quantity = 0;
            try { quantity = double.Parse(textBox_cheqItemQuantity.Text); } catch { }
            item.Quantity = quantity;

            double sum = 0;
            try { sum = double.Parse(textBox_cheqItemSum.Text); } catch { }
            item.Sum = sum;

            item.NdsRate = comboBox_cheqItemTaxRate.SelectedIndex;
            item.PaymentType = comboBox_cheq_itemPaymentTypeSign.SelectedIndex;
            item.ProductType = comboBox_cheq_itemType.SelectedIndex;
            if (!string.IsNullOrEmpty(textBox_cheqItemMeasure105.Text))
            {
                if (comboBox_cheqItemMeasureType.SelectedIndex == 0)
                    item.Unit105 = textBox_cheqItemMeasure105.Text;
                else if (comboBox_cheqItemMeasureType.SelectedIndex == 1)
                {
                    try
                    {
                        byte type120 = byte.Parse(textBox_cheqItemMeasure105.Text);
                        item.Unit120 = type120;
                    }
                    catch { }
                }
            }
            
                

            if (_editMode == EDIT_MODE_APPEND)
            {
                LogHandle.ol("Append ConsumptioItem");
                AddSingleCItem(item, itemsWas, true);
            }
            else
            {
                LogHandle.ol("Change item " + _editMode);
                panel_consumptionItemsContent.Controls.Clear();
                RecreateItems();
            }
            fFDoc.Control();
            FormFiscalDocChanged();
            //panel_consumptionItemsContent.Refresh();
            panel_consumptionItemsContent.ScrollControlIntoView(panel_consumptionItemsContent.Controls[panel_consumptionItemsContent.Controls.Count-1]);
            //System.Threading.Thread.Sleep(500);
            panel_consumptionItemsContent.VerticalScroll.Value = panel_consumptionItemsContent.VerticalScroll.Maximum;
        }

        const int ITEM_Y_SIZE = 46, 
            ITEM_X_SIZE = 477,//507, 
            BORDER_Y_OFFSET = 3;
        static readonly Point ITEM_CONTROL_DELETE_BUTTON = new Point(416, 21),
            ITEM_NAME_TEXTBOX = new Point(80, 3),
            ITEM_CONTROL_AMEND_CHECKBOX = new Point(313, 21),
            ITEM_CODE_TYPE_TEXTBOX = new Point(3, 45),
            ITEM_CODE_VALUE_TEXTBOX = new Point(73, 45),
            ITEM_PAIMENT_TYPE_LABEL = new Point(355, 4),
            ITEM_NDS_RATE_LABEL = new Point(242, 24),
            ITEM_SUM_TEXTBOX = new Point(164, 24),
            ITEM_PRICE_TEXTBOX = new Point(62, 24),
            ITEM_DESIGN_MULTIPLE_SIGN_LABEL = new Point(54, 25),
            ITEM_DESIGN_EQUAL_SIGN_LABEL = new Point(145, 20),
            ITEM_QUANTITY_TEXTBOX = new Point(3, 24),
            ITEM_PRODUCT_TYPE_LABEL = new Point(28, 3),
            ITEM_MULTIP_LABEL = new Point(44, 24);
        static readonly Size ITEM_NAME_TB_SZ = new Size(270, 16),
            ITEM_QUANTITY_TB_SZ = new Size(43, 16),
            ITEM_PRICE_SUM_TB_SZ = new Size(81, 16)
            ;
        void AddSingleCItem(ConsumptionItem item, int itemIndex, bool checkScroll = false)
        {
            if (item == null)
            {
                return;
                //item = ConsumptionItem.SAMPLE;
            }
            if (checkScroll)
            {
                panel_consumptionItemsContent.VerticalScroll.Value = 0;
            }
            GroupBox groupBox_consumptionItem = new GroupBox();
            groupBox_consumptionItem.BackColor = Color.Gainsboro;
            groupBox_consumptionItem.FlatStyle = FlatStyle.Popup;
            groupBox_consumptionItem.Location = new Point(0, 2 + (ITEM_Y_SIZE + BORDER_Y_OFFSET) * itemIndex);
            groupBox_consumptionItem.Size = new Size(ITEM_X_SIZE, ITEM_Y_SIZE);
            groupBox_consumptionItem.Text = (itemIndex + 1).ToString();
            panel_consumptionItemsContent.Controls.Add(groupBox_consumptionItem);


            CheckBox checkBox_amendSelector = new CheckBox();
            checkBox_amendSelector.FlatStyle = FlatStyle.Flat;
            checkBox_amendSelector.Location = ITEM_CONTROL_AMEND_CHECKBOX;
            checkBox_amendSelector.Margin = new Padding(0);
            checkBox_amendSelector.Size = new Size(103, 19);
            checkBox_amendSelector.Text = "Редактировать";
            checkBox_amendSelector.UseVisualStyleBackColor = true;
            checkBox_amendSelector.Checked = _editMode == itemIndex;
            checkBox_amendSelector.CheckedChanged += new EventHandler(ChangeCheckboxItemAmendSelector);
            groupBox_consumptionItem.Controls.Add(checkBox_amendSelector);

            TextBox textBox_itemName = new TextBox();
            textBox_itemName.BorderStyle = BorderStyle.None;
            textBox_itemName.Location = ITEM_NAME_TEXTBOX;
            textBox_itemName.ReadOnly = true;
            textBox_itemName.Size = ITEM_NAME_TB_SZ;
            textBox_itemName.Text = item.Name;
            groupBox_consumptionItem.Controls.Add(textBox_itemName);

            Button button_deleteItem = new Button();
            button_deleteItem.FlatStyle = FlatStyle.System;
            button_deleteItem.Size = new Size(60, 20);
            button_deleteItem.Location = ITEM_CONTROL_DELETE_BUTTON;
            button_deleteItem.Text = "Удалить";
            button_deleteItem.UseVisualStyleBackColor = true;
            button_deleteItem.Click += DeleteItem;
            groupBox_consumptionItem.Controls.Add(button_deleteItem);

            Label label_itemPaymentType = new Label();
            label_itemPaymentType.AutoSize = true;
            label_itemPaymentType.Location = ITEM_PAIMENT_TYPE_LABEL;
            label_itemPaymentType.Text = ItemPaymentTypeDscr[item.PaymentType];
            groupBox_consumptionItem.Controls.Add(label_itemPaymentType);

            Label label_itemTaxType = new Label();
            label_itemTaxType.AutoSize = true;
            label_itemTaxType.Location = ITEM_NDS_RATE_LABEL;
            label_itemTaxType.Text = TaxRateDscr[item.NdsRate];
            groupBox_consumptionItem.Controls.Add(label_itemTaxType);

            TextBox textbox_itemSum = new TextBox();
            textbox_itemSum.BorderStyle = BorderStyle.None;
            textbox_itemSum.Location = ITEM_SUM_TEXTBOX;
            textbox_itemSum.ReadOnly = true;
            textbox_itemSum.Size = ITEM_PRICE_SUM_TB_SZ;
            textbox_itemSum.Text = item.Sum.ToString();
            textbox_itemSum.TextAlign = HorizontalAlignment.Right;
            groupBox_consumptionItem.Controls.Add(textbox_itemSum);

            Label label_itemDesignEqualSign = new Label();
            label_itemDesignEqualSign.AutoSize = true;
            label_itemDesignEqualSign.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            label_itemDesignEqualSign.Location = ITEM_DESIGN_EQUAL_SIGN_LABEL;
            label_itemDesignEqualSign.Text = "=";
            groupBox_consumptionItem.Controls.Add(label_itemDesignEqualSign);

            TextBox textbox_itemPrice = new TextBox();
            textbox_itemPrice.BorderStyle = BorderStyle.None;
            textbox_itemPrice.Location = ITEM_PRICE_TEXTBOX;
            textbox_itemPrice.ReadOnly = true;
            textbox_itemPrice.Size = ITEM_PRICE_SUM_TB_SZ;
            textbox_itemPrice.Text = item.Price.ToString();
            textbox_itemPrice.TextAlign = HorizontalAlignment.Right;
            groupBox_consumptionItem.Controls.Add(textbox_itemPrice);

            Label label_itemDesignMultipleSign = new Label();
            label_itemDesignMultipleSign.AutoSize = true;
            label_itemDesignMultipleSign.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label_itemDesignMultipleSign.Location = ITEM_MULTIP_LABEL;
            label_itemDesignMultipleSign.Text = "Х";
            groupBox_consumptionItem.Controls.Add(label_itemDesignMultipleSign);

            TextBox textbox_itemQuantity = new TextBox();
            textbox_itemQuantity.BorderStyle = BorderStyle.None;
            textbox_itemQuantity.Location = ITEM_QUANTITY_TEXTBOX;
            textbox_itemQuantity.ReadOnly = true;
            textbox_itemQuantity.Size = ITEM_QUANTITY_TB_SZ;
            textbox_itemQuantity.Text = item.Quantity.ToString() + ' ' + (item.Unit105.Length<3?item.Unit105: item.Unit105.Substring(0,3)+'.');
            textbox_itemQuantity.TextAlign = HorizontalAlignment.Right;
            groupBox_consumptionItem.Controls.Add(textbox_itemQuantity);

            Label label_itemProductType = new Label();
            label_itemProductType.AutoSize = true;
            label_itemProductType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label_itemProductType.Location = ITEM_PRODUCT_TYPE_LABEL;
            label_itemProductType.Text = ItemTypeDscr[item.ProductType, FD_ITEM_TYPE_DESCRIBER_SHORT];
            groupBox_consumptionItem.Controls.Add(label_itemProductType);

            
        }

        void RecreateItems()
        {
            panel_consumptionItemsContent.Controls.Clear();
            if (fFDoc == null || fFDoc.Items.Count == 0) return;
            // add garbage collector 
            for (int i = 0; i < fFDoc.Items.Count; i++)
            {
                AddSingleCItem(fFDoc.Items[i], i);
            }
        }

        private int _editMode = EDIT_MODE_APPEND;

        // 
        //private void _AboutWND(){AboutForm helpWindow = new AboutForm();helpWindow.Focus();helpWindow.Show();helpWindow.WindowState = FormWindowState.Normal;Application.Run(helpWindow);}
        //private void _AppSettings(){AppSettings helpWindow = new AppSettings();helpWindow.Focus();helpWindow.Show();helpWindow.WindowState = FormWindowState.Normal;Application.Run(helpWindow);}

        private void DialogBoxes(object sender, EventArgs e)
        {
            if(sender == button_openSettings)
            {
                LogHandle.ol("Open settings");
                // создаем новый поток
                //Thread myThread = new Thread(new ThreadStart(_AppSettings));
                //myThread.Start(); // запускаем поток
                AppSettings setsWindow = new AppSettings();
                setsWindow.FR = fiscalPrinter;
                setsWindow.ShowDialog();
                
                AutoPayBoxesSet();
            }   //Настройки программы
            else if(sender == button_aboutWnd)
            {
                // создаем новый поток
                //Thread myThread = new Thread(new ThreadStart(_AboutWND));
                //myThread.Start(); // запускаем поток
                AboutForm helpWindow = new AboutForm();
                helpWindow.ShowDialog();
            }   // Окно описание
            else if(sender == button_cheque_buyerFfd120)
            {
                fFDoc.BuyerInformationBuyer = textBox_cheq_byerName.Text;
                fFDoc.BuyerInformationBuyerInn = textBox_cheq_buyerInn.Text;
                BuyerInformationWindow bi = new BuyerInformationWindow();
                bi.AddFormDoc(fFDoc);
                bi.ShowDialog();
                textBox_cheq_byerName.Text = fFDoc.BuyerInformationBuyer;
                textBox_cheq_buyerInn.Text = fFDoc.BuyerInformationBuyerInn;
            }   // Сведения о покупателе
        }
            

        private const int EDIT_MODE_APPEND = -1,
            PROCESSING_FLAG = -2;

        private void ValidateInn(object sender, EventArgs e)
        {
            if(sender == textBox_cheq_cashierInn)
            {
                if (textBox_cheq_cashierInn.Text.Length==12 && CorrectInn(textBox_cheq_cashierInn.Text)) textBox_cheq_cashierInn.ForeColor = Color.Black;
                else textBox_cheq_cashierInn.ForeColor = Color.Red;
            }
            else if(sender == textBox_cheq_buyerInn)
            {
                if (CorrectInn(textBox_cheq_buyerInn.Text)) textBox_cheq_buyerInn.ForeColor = Color.Black;
                else textBox_cheq_buyerInn.ForeColor = Color.Red;
            }
        }

        private void CheckedListBox_fdListV2_ItemCheck(object sender, EventArgs e)
        {
            string sfd = checkedListBox_fdListV2.Items[checkedListBox_fdListV2.SelectedIndex].ToString();
            int tn = sfd.IndexOf(' ');
            if (sfd.Length>tn&&(sfd[tn+1]=='Ч'|| sfd[tn + 1] == 'К')&&fiscalPrinter!=null&&fiscalPrinter.IsConnected)
            {
                int num = int.Parse(sfd.Substring(0,tn));
                FnReadedDocument fd = fiscalPrinter.ReadFD(num,true);
                if (fd.Cheque != null) 
                {
                    fFDoc = fd.Cheque;
                }
                else
                {
                    fFDoc = new FiscalCheque();
                }
                MapFormFiscalDocument();
            }
        }


        public static readonly string[] SAMLE_ITEMS = new string[]
        {
            "Круасан с малиновым джемом",
            "Блинчики с мясом",
            "Пончик",
            "Пирожек с ковидлом",
            "Чебурек с сыром",
            "Слойка",
            "Шаурма или шаверма?",
            "Самса с мясом",
            "Сосиска в тесте",
            "Кола",
            "Капучино",
            "Бутерброд с лососем",
            "Напиток молочный кофейный стерилизованный \"Латте-Ваниль\"",
            "Мясной бульон",
            "Омлет из двух яиц с наполнением",
            "Картофельное пюре 150гр",
            "Краб",
            "Яблоко печеное",
            "Вода Байкал 0,530",
            "Фреш ананас",
            "Шоколадный фондан с базиликом",
            "Каша рисовая",
            "Настойка домашняя",
            "Пельмени креветка/лосось",
            "Окрошка на квасе",
            "Макароны с сыром",
            "Каша монастырская овсяная",
            "Глазунья из двух яиц с наполнителями",
            "Апероль Шприц",
            "Паравой коктейль на чаше",
            "Ром Гавана Клуб 3 года",
            "Том Ям",
            "Овощи гриль 150гр",
            "Совиньон Блан Винкл, Кантина Терлано, Альто-Адидже, Итал",
            "Флэт уайт 180мл",
            "Мохито 0,35мл",
            "Органический зеленый чай",
            "Салат киноа с креветками",
            "Стейк из голубого тунца 180гр",
            "Ассорти овощное 300гр",
            "Бланшированный шпинат",
            "Мидии в сливочно-сырном соусе ",
            "Чай имбирный",
            "Глинтвейн 200 мл",
            "Картошка фри 150 гр",
            "Гренки",
            "Кока-кола 300 мл",
            "Чак Чак с медом и орешками,100г",
            "Фетучини с курицей и грибами,300г",
            "Лимонно-имбирная настойка настойка,40мл",
            "Нагетсы 100 гр",
            "Салат греческий 145гр",
            "Пицца закрытая с курицей",
            "Хот дог",
            "Плетенка маковая",
            "Капуста и яйцо пирог",
            "Графские развалины",
            "Пончик шоколадный",
            "Яблочный пирог",
            "Американо 300 мл",
            "Манты КАРТОФЕЛЬ ФАРШ"
        };

        public static readonly string[,] SAMPLE_BUYERS = new string[,]
        {
            //Имя - 0             ИНН - 1         ДР - 2      Грж - 3   Код док - 4  Данные док - 5     Адрес - 6
            {"Иван Иванович Иванов","444444444410","",          "",       "",         "",                 "123456,Msk,Red Square" },
            {"Иван Иваныч Ивнов",   "",            "11.11.1999","332",    "35",        "1122 334455",     "" },
            {"Вано Ваноыч Ваноов",  "",            "01.01.2001","438",    "37",        "0102 304050",     "" },
            {"Даздранагон Семеныч", "999999999950","",         "",       "",          "",                "111222,MO,Podolsk,Central Square" },
            {"Джон Смит",           "",           "02.03.1990","554",    "32",        "0304 827391",     "" },
        };



        // переключение предмета расчета в режим редактирования
        private void ChangeCheckboxItemAmendSelector(object sender, EventArgs e)
        {
            if (_editMode == PROCESSING_FLAG)
                return;
            int editModeSave = _editMode;
            _editMode = PROCESSING_FLAG;

            if (sender == checkBox_itemEdit) // общее отключение редактора
            {
                if (!checkBox_itemEdit.Checked)
                    editModeSave = EDIT_MODE_APPEND;
                else
                    checkBox_itemEdit.Checked = false;
            }
            else // чекбокс предметов расчета
            {
                CheckBox chSender = (CheckBox)sender;
                if (!chSender.Checked)  // снятие галочки редактора 
                    editModeSave = EDIT_MODE_APPEND;
                else // установка галочки редактора
                {
                    try
                    {
                        editModeSave = int.Parse(((GroupBox)chSender.Parent).Text) - 1; // получаем индекс предмета расчета
                    }
                    catch
                    {
                        LogHandle.ol("unable to parce item " + editModeSave);
                    }
                }
            }

            if (editModeSave > EDIT_MODE_APPEND)
                WriteItemToForm(editModeSave);

            foreach (GroupBox grp in panel_consumptionItemsContent.Controls.OfType<GroupBox>())
            {
                int currentGroupBox = int.Parse(grp.Text) - 1;
                // перебираем предметы расчета и устанавливаем флаги(исключая прошлое значение)
                foreach (CheckBox checkBox in grp.Controls.OfType<CheckBox>())
                {
                    if (checkBox.Location == ITEM_CONTROL_AMEND_CHECKBOX)
                    {
                        checkBox.Checked = currentGroupBox == editModeSave;
                        break;
                    }
                }
            }
            checkBox_itemEdit.Checked = editModeSave != EDIT_MODE_APPEND;
            _editMode = editModeSave;
            FormFiscalDocChanged();
            LogHandle.ol(_editMode == EDIT_MODE_APPEND ? "Режим добавления предмета расчета" : "Режим редактирования предмета расчета " + (_editMode + 1));
            if (checkBox_itemEdit.Checked) button_cheque_addPosition.Text = "Изменить";
            else button_cheque_addPosition.Text = "Добавить";
        }

        // обработка удаления предмета расчета
        private void DeleteItem(object sender, EventArgs e)
        {
            Button control = sender as Button;
            GroupBox grp = (GroupBox)control.Parent;
            fFDoc.Items.RemoveAt(int.Parse(grp.Text) - 1);
            fFDoc.Control();
            if (_editMode > EDIT_MODE_APPEND)
                checkBox_itemEdit.Checked = false;
            RecreateItems();
            FormFiscalDocChanged();
        }


        private void ChangeCheckboxPaymentTaxAutoSelectors(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox.Checked)
            {
                if (checkBox == checkBox_autoPayCash)
                {
                    checkBox_autoPayECash.Checked = false;
                    checkBox_autoPayPrepay.Checked = false;
                    checkBox_autoPayCredit.Checked = false;
                    checkBox_autoPayProvision.Checked = false;
                    AppSettings.CoPayInterfaceDoc = AppSettings.CoPayMethods.CO_PAY_CASH;
                }
                else if (checkBox == checkBox_autoPayECash)
                {
                    checkBox_autoPayCash.Checked = false;
                    checkBox_autoPayPrepay.Checked = false;
                    checkBox_autoPayCredit.Checked = false;
                    checkBox_autoPayProvision.Checked = false;
                    AppSettings.CoPayInterfaceDoc = AppSettings.CoPayMethods.CO_PAY_ECASH;
                }
                else if (checkBox == checkBox_autoPayPrepay)
                {
                    checkBox_autoPayCash.Checked = false;
                    checkBox_autoPayECash.Checked = false;
                    checkBox_autoPayCredit.Checked = false;
                    checkBox_autoPayProvision.Checked = false;
                    AppSettings.CoPayInterfaceDoc = AppSettings.CoPayMethods.CO_PAY_PREPAID;
                }
                else if (checkBox == checkBox_autoPayCredit)
                {
                    checkBox_autoPayCash.Checked = false;
                    checkBox_autoPayECash.Checked = false;
                    checkBox_autoPayPrepay.Checked = false;
                    checkBox_autoPayProvision.Checked = false;
                    AppSettings.CoPayInterfaceDoc = AppSettings.CoPayMethods.CO_PAY_CREDIT;
                }
                else if (checkBox == checkBox_autoPayProvision)
                {
                    checkBox_autoPayCash.Checked = false;
                    checkBox_autoPayECash.Checked = false;
                    checkBox_autoPayPrepay.Checked = false;
                    checkBox_autoPayCredit.Checked = false;
                    AppSettings.CoPayInterfaceDoc = AppSettings.CoPayMethods.CO_PAY_PROVISION;
                }
            }
            else
            {
                if(!checkBox_autoPayProvision.Checked 
                    && !checkBox_autoPayCredit.Checked 
                    && !checkBox_autoPayPrepay.Checked 
                    && !checkBox_autoPayECash.Checked 
                    && !checkBox_autoPayCash.Checked)
                {
                    AppSettings.CoPayInterfaceDoc = AppSettings.CoPayMethods.CO_PAY_OFF;
                }
            }
            AppSettings.SaveSettings();
            FormFiscalDocChanged();
        }


        // проверка предмета рвсчета на форме
        private bool CheckFormItem()
        {
            bool correctName = textBox_cheqItemName.Text != "" && textBox_cheqItemName.Text.Length < 128;
            double price = -1;
            try { price = Math.Round(double.Parse(textBox_cheqItemPrice.Text), 2); } catch { }
            textBox_cheqItemPrice.ForeColor = (price < 0) ? Color.Red : Color.Black;
            double quantity = -1;
            try { quantity = double.Parse(textBox_cheqItemQuantity.Text); } catch { }
            textBox_cheqItemQuantity.ForeColor = (quantity < 0) ? Color.Red : Color.Black;
            double sum = -1;
            try { sum = double.Parse(textBox_cheqItemSum.Text); } catch { }
            textBox_cheqItemSum.ForeColor = (sum < 0) ? Color.Red : Color.Black;
            bool notCorrectSums = Math.Abs(Math.Round(price * quantity, 2) - Math.Round(sum, 2)) > 0.011;
            return correctName && !notCorrectSums && price >= 0 && quantity > 0 && sum >= 0;
        }

        bool _roundTally = false;



        // обработка изменений полей предмета расчета
        private void ChangeChequeItemFiedls(object sender, EventArgs e)
        {
            if (sender == button_clearNdsRateItems)
            {
                foreach (var item in fFDoc.Items)
                {
                    item.NdsRate = NONE;
                    item.Control();
                    fFDoc.Control();
                }
                MapFormFiscalDocument();
            }
            else if (sender == button_cheqRoundTotalSum)
            {
                fFDoc.TotalSum = (int)fFDoc.TotalSum;
                FormFiscalDocChanged();
                _roundTally = true;
            }
            else if (sender == button_cheqDeleteAllItems)
            {
                if (checkBox_itemEdit.Checked)
                    checkBox_itemEdit.Checked = false;
                fFDoc.ClearItems();
                panel_consumptionItemsContent.Controls.Clear();
                FormFiscalDocChanged();
            }
            // предметы расчета
            else if (sender == textBox_cheqItemPrice || sender == textBox_cheqItemQuantity || sender == textBox_cheqItemSum || sender == textBox_cheqItemName)
            {
                /*if (_modeItemChange == PROCESSING_FLAG)
                    return;*/
                if (sender == textBox_cheqItemPrice || sender == textBox_cheqItemQuantity || sender == textBox_cheqItemSum)
                {
                    //WriteCorrectDecimalSeparator((TextBox)sender);
                    TextBox t = sender as TextBox;
                    string original = t.Text;
                    string replaced = ReplaceBadDecimalSeparatorPoint(original);
                    try { double.Parse(replaced); } catch { return; }
                    if (original != replaced)
                    {
                        int cursor = t.SelectionStart;
                        t.Text = replaced;
                        if (cursor <= replaced.Length) t.SelectionStart = cursor;
                    }
                }
                if (sender == textBox_cheqItemPrice || sender == textBox_cheqItemQuantity)
                    try
                    {
                        textBox_cheqItemSum.Text =
                            Math.Round(double.Parse(textBox_cheqItemPrice.Text)
                            * double.Parse(textBox_cheqItemQuantity.Text), 2).ToString();
                    }
                    catch { }
                button_cheque_addPosition.Enabled = CheckFormItem();
            }
            // поля сумм оплаты чека
            else if (sender == textBox_cheq_payCash || sender == textBox_cheq_payCashless
                || sender == textBox_cheq_payAvans || sender == textBox_cheq_payCredit
                || sender == textBox_cheq_payProvision)
            {
                double s = -1;
                try { s = double.Parse((sender as TextBox).Text); } catch { }
                if (s < 0)
                {
                    (sender as TextBox).ForeColor = Color.Red;
                    s = 0;
                }
                else
                {
                    (sender as TextBox).ForeColor = Color.Black;
                }
                s = Math.Round(s, 2);

                if (sender == textBox_cheq_payCash)
                    fFDoc.Cash = s;
                else if (sender == textBox_cheq_payCashless)
                    fFDoc.ECash = s;
                else if (sender == textBox_cheq_payAvans)
                    fFDoc.Prepaid = s;
                else if (sender == textBox_cheq_payCredit)
                    fFDoc.Credit = s;
                else if (sender == textBox_cheq_payProvision)
                    fFDoc.Provision = s;
                fFDoc.Control(false);
                FormFiscalDocChanged();
            }
            else if (sender == textBox_chequeTotalSum || sender == textBox_taxPanelNdsFree
                || sender == textBox_taxPanelNds0 || sender == textBox_taxPanelNds20
                || sender == textBox_taxPanelNds10 || sender == textBox_taxPanelNds20120 || sender == textBox_taxPanelNds10110)
            {
                FormFiscalDocChanged();
            }
            else if (sender is Button btn)
            {
                //Button btn = (Button)sender;
                
                if (btn.Name.StartsWith("button_paySurcharge"))
                {
                    if (!fFDoc.IsNotPaid)
                    {
                        fFDoc.PaySumsClear();
                        if (sender == button_paySurchargeCash)
                        {
                            fFDoc.Cash = fFDoc.TotalSum;
                        }
                        else if (sender == button_paySurchargeECash)
                        {
                            fFDoc.ECash = fFDoc.TotalSum;
                        }
                        else if (sender == button_paySurchargePrepay)
                        {
                            fFDoc.Prepaid = fFDoc.TotalSum;
                        }
                        else if (sender == button_paySurchargeCredit)
                        {
                            fFDoc.Credit = fFDoc.TotalSum;
                        }
                        else if (sender == button_paySurchargeProvision)
                        {
                            fFDoc.Provision = fFDoc.TotalSum;
                        }
                        FormFiscalDocChanged(true);
                    }
                    else if (fFDoc.IsNotPaid)
                    {
                        double delta = fFDoc.TotalSum - fFDoc.Cash - fFDoc.ECash - fFDoc.Prepaid - fFDoc.Credit - fFDoc.Provision;
                        if (sender == button_paySurchargeCash)
                        {
                            fFDoc.Cash += delta;
                        }
                        else if (sender == button_paySurchargeECash)
                        {
                            fFDoc.ECash += delta;
                        }
                        else if (sender == button_paySurchargePrepay)
                        {
                            fFDoc.Prepaid += delta;
                        }
                        else if (sender == button_paySurchargeCredit)
                        {
                            fFDoc.Credit += delta;
                        }
                        else if (sender == button_paySurchargeProvision)
                        {
                            fFDoc.Provision += delta;
                        }
                        FormFiscalDocChanged(true);
                    }
                }
                else if(btn.Name.StartsWith("button_cheqNds")&& btn.Name.EndsWith("Fill"))
                {
                    if (btn == button_cheqNds20Fill)
                    {
                        foreach(var item in fFDoc.Items)
                        {
                            if(item.NdsRate == NONE)
                            {
                                item.NdsRate = NDS_TYPE_20_LOC;
                                item.Control();
                            }
                        }
                    }
                    else if(btn == button_cheqNds10Fill)
                    {
                        foreach (var item in fFDoc.Items)
                        {
                            if (item.NdsRate == NONE)
                            {
                                item.NdsRate = NDS_TYPE_10_LOC;
                                item.Control();
                            }
                        }
                    }
                    else if (btn == button_cheqNds10110Fill)
                    {
                        foreach (var item in fFDoc.Items)
                        {
                            if (item.NdsRate == NONE)
                            {
                                item.NdsRate = NDS_TYPE_10110_LOC;
                                item.Control();
                            }
                        }
                    }
                    else if (btn == button_cheqNds0Fill)
                    {
                        foreach (var item in fFDoc.Items)
                        {
                            if (item.NdsRate == NONE)
                            {
                                item.NdsRate = NDS_TYPE_0_LOC;
                                item.Control();
                            }
                        }
                    }
                    else if (btn == button_cheqNds20120Fill)
                    {
                        foreach (var item in fFDoc.Items)
                        {
                            if (item.NdsRate == NONE)
                            {
                                item.NdsRate = NDS_TYPE_20120_LOC;
                                item.Control();
                            }
                        }
                    }
                    else if (btn == button_cheqNdsFreeFill)
                    {
                        foreach (var item in fFDoc.Items)
                        {
                            if (item.NdsRate == NONE)
                            {
                                item.NdsRate = NDS_TYPE_FREE_LOC;
                                item.Control();
                            }
                        }
                    }
                    else if (btn == button_cheqNds5Fill)
                    {
                        foreach (var item in fFDoc.Items)
                        {
                            if (item.NdsRate == NONE)
                            {
                                item.NdsRate = NDS_TYPE_5_LOC;
                                item.Control();
                            }
                        }
                    }
                    else if (btn == button_cheqNds7Fill)
                    {
                        foreach (var item in fFDoc.Items)
                        {
                            if (item.NdsRate == NONE)
                            {
                                item.NdsRate = NDS_TYPE_7_LOC;
                                item.Control();
                            }
                        }
                    }
                    else if (btn == button_cheqNds5105Fill)
                    {
                        foreach (var item in fFDoc.Items)
                        {
                            if (item.NdsRate == NONE)
                            {
                                item.NdsRate = NDS_TYPE_5105_LOC;
                                item.Control();
                            }
                        }
                    }
                    else if (btn == button_cheqNds7107Fill)
                    {
                        foreach (var item in fFDoc.Items)
                        {
                            if (item.NdsRate == NONE)
                            {
                                item.NdsRate = NDS_TYPE_7107_LOC;
                                item.Control();
                            }
                        }
                    }


                    fFDoc.Control(true);
                    MapFormFiscalDocument();
                    button_cheqNdsFreeFill.Enabled = false;
                    button_cheqNds20Fill.Enabled = false;
                    button_cheqNds10Fill.Enabled = false;
                    button_cheqNds20120Fill.Enabled = false;
                    button_cheqNds10110Fill.Enabled = false;
                    button_cheqNds0Fill.Enabled = false;
                    button_cheqNds5Fill.Enabled = false;
                    button_cheqNds7Fill.Enabled = false;
                    button_cheqNds5105Fill.Enabled = false;
                    button_cheqNds7107Fill.Enabled = false;
                }
            }
            else if (sender == comboBox_cheqItemMeasureType)
            {
                if(comboBox_cheqItemMeasureType.SelectedIndex == 0) textBox_cheqItemMeasure105.Text = "";
                else
                    textBox_cheqItemMeasure105.Text = "0";
                textBox_cheqItemMeasure105.ForeColor = Color.Black;
                /*if(comboBox_cheqItemMeasureType.SelectedIndex == 1)
                {
                    if (_toolTip1 == null)
                    {
                        _toolTip1 = new ToolTip();
                        _toolTip1.AutoPopDelay = 1000;
                        _toolTip1.InitialDelay = 1000;
                        _toolTip1.ReshowDelay = 500;
                        _toolTip1.ShowAlways = true;
                        _toolTip1.SetToolTip(textBox_cheqItemMeasure105, _measureToolTip);
                    }
                    
                }
                else if(comboBox_cheqItemMeasureType.SelectedIndex == 0)
                {
                    if(_toolTip1!=null)
                    {
                        _toolTip1.AutoPopDelay = 1000000;
                        _toolTip1.InitialDelay = 1000000;
                        _toolTip1.ReshowDelay = 500000;
                    }
                }*/
            }
            else if(sender == textBox_cheqItemMeasure105)
            {
                if (comboBox_cheqItemMeasureType.SelectedIndex == 0)
                        return;
                try
                {
                    int m = int.Parse(textBox_cheqItemMeasure105.Text);
                    if (m >= 0 && (
                    m == FD_ITEM_MEASURE_UNIT_LOC ||
                    m == FD_ITEM_MEASURE_GRAM_LOC ||
                    m == FD_ITEM_MEASURE_KG_LOC ||
                    m == FD_ITEM_MEASURE_TON_LOC ||
                    m == FD_ITEM_MEASURE_SM_LOC ||
                    m == FD_ITEM_MEASURE_DM_LOC ||
                    m == FD_ITEM_MEASURE_METR_LOC ||
                    m == FD_ITEM_MEASURE_QSM_LOC ||
                    m == FD_ITEM_MEASURE_QDM_LOC ||
                    m == FD_ITEM_MEASURE_QMETR_LOC ||
                    m == FD_ITEM_MEASURE_ML_LOC ||
                    m == FD_ITEM_MEASURE_LITR_LOC ||
                    m == FD_ITEM_MEASURE_CUBEM_LOC ||
                    m == FD_ITEM_MEASURE_KWH_LOC ||
                    m == FD_ITEM_MEASURE_GKL_LOC ||
                    m == FD_ITEM_MEASURE_DAY_LOC ||
                    m == FD_ITEM_MEASURE_HOUR_LOC ||
                    m == FD_ITEM_MEASURE_MIN_LOC ||
                    m == FD_ITEM_MEASURE_SEC_LOC ||
                    m == FD_ITEM_MEASURE_KBYTE_LOC ||
                    m == FD_ITEM_MEASURE_MBYTE_LOC ||
                    m == FD_ITEM_MEASURE_GBYTE_LOC ||
                    m == FD_ITEM_MEASURE_TBYTE_LOC ||
                    m == FD_ITEM_MEASURE_OTHER_LOC
                    )) textBox_cheqItemMeasure105.ForeColor = Color.Black;
                    else
                        textBox_cheqItemMeasure105.ForeColor = Color.Red;
                } 
                catch 
                {
                    textBox_cheqItemMeasure105.ForeColor = Color.Red;
                }
            }
        }
        //ToolTip _toolTip1 = null;
        const string _measureToolTip = "0 шт. или ед.(поштучно или единицамы)\r\n10 г(Грамм)\r\n11 кг(Килограмм)\r\n12 т(Тонна)\r\n20 см(Сантиметр)\r\n21 дм(Дециметр)\r\n22 м(Метр)\r\n30 кв.см(Квадратный сантиметр)\r\n31 кв.дм(Квадратный дециметр)\r\n32 кв.м(Квадратный метр)\r\n40 мл(Миллилитр)\r\n41 л(Литр)\r\n42 куб. м(Кубический метр)\r\n50 кВт∙ч(Киловатт час)\r\n51 Гкал(Гигакалория)\r\n70 сутки (день)\r\n71 час\r\n72 мин(Минута)\r\n73 с(Секунда)\r\n80 Кбайт\r\n81 Мбайт\r\n82 Гбайт\r\n83 Тбайт\r\n255 Если не похдодят в предыдущие";


        // загрузка предмета расчета в интерфейс
        private void WriteItemToForm(int itemIdex)
        {
            if (itemIdex < 0 || itemIdex > fFDoc.Items.Count - 1)
            {
                FormFiscalDocConditionWrite("Incorrect items number");
                return;
            }
            ConsumptionItem item = fFDoc.Items[itemIdex];
            textBox_cheqItemName.Text = item.Name;
            textBox_cheqItemPrice.Text = item.Price.ToString();
            textBox_cheqItemQuantity.Text = item.Quantity.ToString();
            textBox_cheqItemSum.Text = item.Sum.ToString();
            comboBox_cheq_itemType.SelectedIndex = item.ProductType;
            comboBox_cheq_itemPaymentTypeSign.SelectedIndex = item.PaymentType;
            comboBox_cheqItemTaxRate.SelectedIndex = item.NdsRate;
            if (!string.IsNullOrEmpty(item.Unit105))
            {
                if (comboBox_cheqItemMeasureType.SelectedIndex != 0)
                    comboBox_cheqItemMeasureType.SelectedIndex = 0;
                textBox_cheqItemMeasure105.Text = item.Unit105;
            }
            if (item.Unit120 > -1 && fiscalPrinter != null && fiscalPrinter.IsConnected && fiscalPrinter.FfdVer >= FR_FFD120)
            {
                if (comboBox_cheqItemMeasureType.SelectedIndex != 1) 
                    comboBox_cheqItemMeasureType.SelectedIndex = 1;
                textBox_cheqItemMeasure105.Text = item.Unit120.ToString();
            }
            if (string.IsNullOrEmpty(item.Unit105) && item.Unit120 <= -1) textBox_cheqItemMeasure105.Text = "";
        }

        private bool _askRepeatRead = false;
        private bool _askRepeatClear = true;
        int _tpOldVarUnlocker = 0;
        private TabPage _hidedPageOldVar = null;
        private bool _readed_fd_ok = false;

        private void Task2Actions(object sender, EventArgs e)
        {
            if (sender != button_clearList2)
                _tpOldVarUnlocker = 0;
            if (sender == radioButton_task2VariantRange
                ||sender == radioButton_task2VariantList
                ||sender == radioButton_task2VariantReadJson
                ||sender == textBox_task2FirstNumber
                ||sender == textBox_task2LastNumber
                ||sender == textBox_task2NumberList
                ||sender == comboBox_task2ReadSeparator)
            {
                int docsCount = 0;
                _askRepeatClear = false;
                if (radioButton_task2VariantRange.Checked)
                {
                    button_task2ReadJsonFolder.Enabled = false;
                    button_task2ReadJsonFiles.Enabled = false;
                    textBox_task2FirstNumber.Enabled= true; 
                    textBox_task2LastNumber.Enabled= true;
                    textBox_task2NumberList.Enabled= false;
                    button_checkFdList2.Enabled= false;
                    button_clearList2.Enabled= false;
                    comboBox_task2ReadSeparator.Enabled= false;
                    try
                    {
                        var firstNum = int.Parse(textBox_task2FirstNumber.Text);
                        var lastNum = int.Parse(textBox_task2LastNumber.Text);
                        if(firstNum >= 0 && lastNum >= 0 && lastNum - firstNum >= 0) docsCount = lastNum - firstNum + 1;
                    }
                    catch { }
                }
                else if (radioButton_task2VariantList.Checked)
                {
                    button_task2ReadJsonFolder.Enabled = false;
                    button_task2ReadJsonFiles.Enabled= false;
                    textBox_task2FirstNumber.Enabled = false;
                    textBox_task2LastNumber.Enabled = false;
                    textBox_task2NumberList.Enabled = true;
                    button_checkFdList2.Enabled = true;
                    button_clearList2.Enabled = true;
                    comboBox_task2ReadSeparator.Enabled = true;
                    char separator;
                    switch (comboBox_task2ReadSeparator.SelectedIndex)
                    {
                        case 0:
                        case 1:
                            separator = ',';
                            break;
                        case 2:
                        case 3:
                            separator = ';';
                            break;
                        case 4:
                        case 5:
                            separator = ' ';
                            break;
                        case 6:
                        case 7:
                        default:
                            separator = '\n';
                            break;
                    }
                    var strictValues = comboBox_task2ReadSeparator.SelectedIndex % 2 == 0;
                    string[] docList = textBox_task2NumberList.Text.Split(separator);
                    List<int> docNumbers = new System.Collections.Generic.List<int>();
                    foreach (string num in docList)
                    {
                        if (string.IsNullOrEmpty(num)) 
                            continue;
                        try
                        {
                            var docNumber = int.Parse(num);
                            if (docNumber > 0 && !docNumbers.Contains(docNumber)) 
                            { 
                                docNumbers.Add(docNumber); 
                            }
                            else
                            {
                                if (strictValues)
                                {
                                    docNumbers.Clear();
                                    break;
                                }
                            }
                        }
                        catch 
                        {
                            if (strictValues)
                            {
                                docNumbers.Clear();
                                break;
                            }
                        }
                    }
                    docsCount = docNumbers.Count;
                }
                else if (radioButton_task2VariantReadJson.Checked)
                {
                    button_task2ReadJsonFolder.Enabled = true;
                    button_task2ReadJsonFiles.Enabled = true;
                    textBox_task2FirstNumber.Enabled = false;
                    textBox_task2LastNumber.Enabled = false;
                    textBox_task2NumberList.Enabled = false;
                    button_checkFdList2.Enabled = false;
                    button_clearList2.Enabled = false;
                    comboBox_task2ReadSeparator.Enabled = false;
                    docsCount = -1;
                }
                if(docsCount == -1)
                {
                    textBox_task2ReadMsg.Text = "";
                }
                else
                    textBox_task2ReadMsg.Text = docsCount>0?"Документов для обработки: " + docsCount:"Ошибка ввода номеров"; 
            }
            else if(sender == checkBox_task2FiltersSwicher)
            {
                groupBox_task2FiltersContainer.Enabled = checkBox_task2FiltersSwicher.Checked;
            }
            else if(sender == button_task2ReadJsonFiles)
            {

                checkBox_cleanJsonCollection.Checked = true;
                Task2Actions(button_readjsonList, e);
                if (_readed_fd_ok)
                {
                    _fdReaded.AddRange(_jsonFdCollection);
                    if (_fdReaded.Count > 0)
                    {
                        textBox_readedExtendedInfo.Text = ChequesCount(_fdReaded);
                        textBox_task2ReadedCount.Text = _fdReaded.Count.ToString();
                    }
                    else
                    {
                        textBox_readedExtendedInfo.Text = "Чеков не найдено, проверьте настройки или корректность формата";
                        textBox_task2ReadedCount.Text = _fdReaded.Count.ToString();
                    }
                }
                
                
            }
            else if(sender == button_task2ReadJsonFolder)
            {
                if(fiscalPrinter == null)
                {
                    PushMessage("Выберите и соеденитесь с ККТ");
                    return;
                }
                if (!fiscalPrinter.IsConnected)
                {
                    PushMessage("Установите соединение с ККТ");
                    return;
                }
                processInterruptor = false;
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if(fbd.ShowDialog() == DialogResult.OK)
                {
                    _fdReaded.Clear();
                    _fdFiltrated.Clear();
                    DirectoryInfo d = new DirectoryInfo(fbd.SelectedPath);
                    var files = d.GetFiles();
                    Thread statusReading = null;
                    int filesCount = files.Length;
                    if (filesCount > 50)
                    {
                        statusReading = new Thread(new ThreadStart(Run_status_window));
                        statusReading.Start();
                    }
                    List<FileInfo> emptyFiles = new List<FileInfo>();
                    for(int i = 0; i < filesCount; i++)
                    {
                        int readedFdNumber = _fdReaded.Count;
                        if (processInterruptor)
                            break;
                        try
                        {
                            if (i % 20 == 0)
                            {
                                if (status != null)
                                    status.Message("Разбираем файл: " + i + " из " + filesCount + Environment.NewLine + "распознанно чеков " + _fdReaded.Count);
                            }
                            List<FnReadedDocument> rfdl = JsonChequeConstructor.ReadDocuments(new string[] { files[i].FullName });
                            foreach (var fd in rfdl)
                            {
                                if (fd.Equals(FnReadedDocument.EmptyFD) || fd.Cheque == null)
                                {
                                    LogHandle.ol("Распознан пустой документ из " + files[i].Name);
                                    continue;
                                }
                                _fdReaded.Add(fd);
                            }
                        }
                        catch(Exception exc)
                        {
                            LogHandle.ol("Error while recognition: "+exc.Message);
                        }
                        if(_fdReaded.Count == readedFdNumber)
                        {
                            LogHandle.ol("! ! !Чека в файле "+ files[i].Name+" не обнаружено! ! ! ! ! !");
                            emptyFiles.Add(files[i]);
                        }

                    }

                    if (emptyFiles.Count > 0)
                    {
                        DirectoryInfo dir = Directory.CreateDirectory("json_НЕ_РАСПОЗНАНО_"+DateTime.Now.ToString("yyyyMMddHHmmss"));
                        foreach(var nrf in emptyFiles)
                        {
                            File.Copy(nrf.FullName, Path.Combine(dir.FullName, nrf.Name));
                        }
                    }

                    if (status != null)
                        status.AllDone();
                    this.Enabled = true;
                    this.Activate();
                    textBox_readedExtendedInfo.Text = ChequesCount(_fdReaded);
                    textBox_task2ReadedCount.Text = _fdReaded.Count.ToString();
                }
            }
            else if (sender == button_task2Read)
            {
                if (radioButton_task2VariantReadJson.Checked)
                {
                    Task2Actions(button_task2ReadJsonFiles, e);
                }
                else
                {
                    if (fiscalPrinter != null && fiscalPrinter.IsConnected)
                    {
                        //Thread statusReading = new Thread(new ThreadStart(run_status_window));
                        //statusReading.Start();
                        if (_askRepeatClear)
                            _askRepeatRead = false;
                        MassActionReporter.InitializeT2Vls();
                        _fdReaded.Clear();
                        _fdFiltrated.Clear();
                        List<int> docNumbers = new List<int>();
                        if (radioButton_task2VariantRange.Checked)
                        {
                            try
                            {
                                var firstNum = int.Parse(textBox_task2FirstNumber.Text);
                                var lastNum = int.Parse(textBox_task2LastNumber.Text);
                                if (lastNum > fiscalPrinter.LastFd) lastNum = fiscalPrinter.LastFd;
                                if (firstNum >= 0 && lastNum >= 0 && lastNum - firstNum >= 0)
                                {
                                    do
                                    {
                                        docNumbers.Add(firstNum++);
                                    } while (firstNum <= lastNum);
                                }
                            }
                            catch { }
                        }
                        else if (radioButton_task2VariantList.Checked)
                        {
                            char separator;
                            switch (comboBox_task2ReadSeparator.SelectedIndex)
                            {
                                case 0:
                                case 1:
                                    separator = ',';
                                    break;
                                case 2:
                                case 3:
                                    separator = ';';
                                    break;
                                case 4:
                                case 5:
                                    separator = ' ';
                                    break;
                                case 6:
                                case 7:
                                default:
                                    separator = '\n';
                                    break;
                            }
                            var strictValues = comboBox_task2ReadSeparator.SelectedIndex % 2 == 0;
                            string[] docList = textBox_task2NumberList.Text.Split(separator);
                            foreach (string num in docList)
                            {
                                if (string.IsNullOrEmpty(num))
                                    continue;
                                try
                                {
                                    var docNumber = int.Parse(num);
                                    if (docNumber > 0 && !docNumbers.Contains(docNumber))
                                    {
                                        docNumbers.Add(docNumber);
                                    }
                                    else
                                    {
                                        if (strictValues)
                                        {
                                            docNumbers.Clear();
                                            break;
                                        }
                                    }
                                }
                                catch
                                {
                                    if (strictValues)
                                    {
                                        docNumbers.Clear();
                                        break;
                                    }
                                }
                            }
                        }

                        if (docNumbers.Count > 0)
                        {
                            this.Enabled = false;
                            Thread statusReading = new Thread(new ThreadStart(Run_status_window));
                            //MassActionReporter.ErrorCounter = 0;
                            statusReading.Start();
                            processInterruptor = false;
                            int n = 1, nms = docNumbers.Count/*, sizeR = 0*/;
                            foreach (var docNumber in docNumbers)
                            {
                                if (processInterruptor)
                                    break;
                                if (status != null) status.Message("Читаем ФД: " + docNumber + " (" + n++ + " из " + nms + ")" + Environment.NewLine + "успешно прочитано " + _fdReaded.Count);
                                FnReadedDocument readed = fiscalPrinter.ReadFD(docNumber, true);
                                if (readed.Cheque != null)
                                    _fdReaded.Add(readed);
                                MassActionReporter.AppendReadedFD(readed);
                            }
                            if (status != null)
                                status.AllDone();
                            textBox_readedExtendedInfo.Text = ChequesCount(_fdReaded);
                            textBox_task2ReadedCount.Text = _fdReaded.Count.ToString();
                            MassActionReporter.SummingUpReadRezults();
                            this.Enabled = true;
                            this.Activate();
                        }


                    }
                    else
                        PushMessage("Установите соединение с ККТ");
                }

            }
            else if (sender == button_task2ApplyFilter)
            {
                if (_askRepeatClear)
                    _askRepeatRead = false;
                if (_fdFiltrated.Count > 0)
                    _fdFiltrated.Clear();
                if (_fdReaded.Count == 0)
                    Task2Actions(button_task2Read, e);

                if (_fdReaded.Count > 0)
                {
                    if (fiscalPrinter==null||!fiscalPrinter.IsConnected)
                    {
                        PushMessage("Не установлено соединение с ККТ");
                    }
                    else
                    {
                        if (checkBox_task2FiltersSwicher.Checked)
                        {
                            ReadFilters();
                            foreach (var fd in _fdReaded) if (FiltrateDocument(fd)) _fdFiltrated.Add(fd);
                        }
                        else
                            _fdFiltrated = new List<FnReadedDocument>(_fdReaded);

                        if (!fiscalPrinter.DontPrint && AppSettings.PrintExtendedTextInfo && checkBox_printExtraInfo.Checked)
                        {
                            foreach (var fd in _fdFiltrated)
                            {
                                foreach (var s in AppSettings.ExtendedTextInfo(fd))
                                {
                                    fd.Cheque.ExtendedInfoForPrinting.Add(s);
                                }

                            }
                        }
                    }
                }
                textBox_task2FiltratedExtDescribtion.Text = ChequesCount(_fdFiltrated);
                textBox_filtrateCounter.Text = _fdFiltrated.Count.ToString();
            }
            else if (sender == button_task2PerformCorretions)
            {
                if (fiscalPrinter == null || !fiscalPrinter.IsConnected)
                {
                    PushMessage(CONNECTION_NOT_ESTABLISHED);
                    return;
                }

                if (_askRepeatRead && _fdReaded.Count == 0)
                {
                    if (MessageBox.Show("Возможно вы уже проводили операцию корректировки с этими ФД."
                        + Environment.NewLine + "Буфер считанных чеков очищен."
                        + Environment.NewLine + "Для повторной операции придется повторно считывать чеки"
                        + Environment.NewLine
                        + Environment.NewLine + "Если ФД считаны из другого ФНа нажмите \"Нет\","
                        + Environment.NewLine + "заново его подключите и cчитайте", "Повторить чтение?", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        PushMessage("Корректровка прервана");
                        return;
                    }
                }
                _askRepeatClear = false;
                if (_fdFiltrated.Count == 0) Task2Actions(button_task2ApplyFilter, e);
                if (_fdFiltrated.Count == 0)
                {
                    PushMessage("Нет подходящих документов, проверьте настройки фильтра или чтения");
                    _askRepeatClear = true;
                    return;
                }
                _askRepeatClear = true;

                int errorsAllowed = 10;
                int errorsOccured = 0;
                int.TryParse(textBox_task2allowableErrors.Text, out errorsAllowed);

                this.Enabled = false;
                MassActionReporter.InitializeT2Vls(true);
                Thread statusReading = new Thread(new ThreadStart(Run_status_window));
                //MassActionReporter.ErrorCounter = 0;
                statusReading.Start();
                processInterruptor = false;
                int n = 1, nms = _fdFiltrated.Count/*, sizeR = 0*/;
                // правила корректировки co - canselOperation seo - strightModifiedOperation

                // общие
                bool changeCashier = checkBox_cashierFromInterfase.Checked;
                string interfaceCashier = string.Empty,
                    interfaceCashierInn = string.Empty;
                if (changeCashier)
                {
                    interfaceCashier = textBox_cheq_cashierName.Text;
                    interfaceCashierInn = CorrectInn(textBox_cheq_cashierInn.Text) ? textBox_cheq_cashierInn.Text : string.Empty;
                }
                int corrTypeSmo = comboBox_task2CorrectionType.SelectedIndex;
                string corrDocNumber = textBox_task2CorrectionNumber.Text;

                // обратная операция
                bool coUse = checkBox_task2CancelOperationSwitcher.Checked,
                    coTypeCorretion = checkBox_task2BackDocTypeCorrection.Checked,
                    coReverseMethodReturn = radioButton_task2BackCorrectVariantReturn.Checked,
                    coSmoErrorBlocker = checkBox_task2CorOpStopper.Checked,
                    // прямая операция
                    smoUse = checkBox_task2StrightModified.Checked,
                    smoTypeCorretion = checkBox_task2SmoDocType.Checked;

                int smoPayTypeRemove1 = checkBox_task2changePayment1.Checked ? comboBox_task2Payment1Original.SelectedIndex : 0,
                    smoPayTypeRemove2 = checkBox_task2changePayment2.Checked ? comboBox_task2Payment2Original.SelectedIndex : 0,
                    smoPayTypeSet1 = comboBox_task2Payment1Changed.SelectedIndex,
                    smoPayTypeSet2 = comboBox_task2Payment2Changed.SelectedIndex,
                    smoTaxRateRemove1 = checkBox_task2TaxTypeChanger1.Checked ? comboBox_taxType1Original.SelectedIndex : 0,
                    smoTaxRateRemove2 = checkBox_task2TaxTypeChanger2.Checked ? comboBox_taxType2Original.SelectedIndex : 0,
                    smoTaxRateSet1 = comboBox_task2taxType1Changed.SelectedIndex,
                    smoTaxRateSet2 = comboBox_task2taxType2Changed.SelectedIndex,
                    smoSnoRemove = checkBox_task2SnoChanger.Checked ? comboBox_task2SnoOriginal.SelectedIndex : 0,
                    smoSnoChange = comboBox_task2SnoChanged.SelectedIndex,
                    smoOperationType = checkBox_task2OperationSignChanger.Checked ? comboBox_OperationSignChanged.SelectedIndex : 0;
                if (smoPayTypeSet1 == 0 || smoPayTypeSet1 == smoPayTypeRemove1) smoPayTypeRemove1 = 0;
                if (smoPayTypeSet2 == 0 || smoPayTypeSet2 == smoPayTypeRemove2) smoPayTypeRemove2 = 0;
                if (smoTaxRateSet1 == 0 || smoTaxRateSet1 == smoTaxRateRemove1) smoTaxRateRemove1 = 0;
                if (smoTaxRateSet2 == 0 || smoTaxRateSet2 == smoTaxRateRemove2) smoTaxRateRemove2 = 0;
                if (smoSnoChange == 0 || smoSnoChange == smoSnoRemove) smoSnoRemove = 0;
                if (smoSnoRemove != 0)
                {
                    smoSnoRemove = 1 << smoSnoRemove - 1;
                    if (smoSnoRemove >= 8) smoSnoRemove *= 2;
                    smoSnoChange = 1 << smoSnoChange - 1;
                    if (smoSnoChange >= 8) smoSnoChange *= 2;
                    AppSettings.UsingCustomSno = true;
                }
                bool importData = checkBox_task2excelImport.Checked;
                bool closeShiftOnErrror = checkBox_task2bypass235error.Checked;
                int fdPerformedCounter = 0;
                if (KKMInfoTransmitter[FR_SHIFT_STATE_KEY]=="Смена закрыта")
                {
                    fiscalPrinter.OpenShift();
                }
                List<FnReadedDocument> fdErrors = new List<FnReadedDocument>();
                List<FnReadedDocument> fdNotPerfomed = new List<FnReadedDocument>();
                bool noSkipError = true;
                int xi = 0;
                for (; xi < _fdFiltrated.Count; xi++)
                {
                    if(importData && FormImport.RequirementData && (FormImport.table ==null || FormImport.CurrentRow > FormImport.table.GetUpperBound(0)))
                    {
                        break;
                    }

                    noSkipError = true;
                    var fd = _fdFiltrated[xi];
                    if (errorsOccured >= errorsAllowed)
                    {
                        LogHandle.ol("! ! ! Превышен допустимый уровень ошибок ! ! !");
                        processInterruptor = true;
                    }

                    if (processInterruptor)
                        break;

                    if (status != null) status.Message("Корректируем: " + n++ + " из " + nms + ")");
                    double oldTotal = fd.Cheque.TotalSum;
                    bool returnRezult = true;
                    if (fd.Cheque.Document != FD_DOCUMENT_NAME_CORRECTION_CHEQUE)
                    {
                        fd.Cheque.CorrectionDocumentDate = fd.Time;
                        fd.Cheque.CorrectionTypeNotFtag = corrTypeSmo;
                        fd.Cheque.CorrectionOrderNumber = corrDocNumber;
                    }
                    if (changeCashier)
                    {
                        fd.Cheque.Cashier = interfaceCashier;
                        fd.Cheque.CashierInn = interfaceCashierInn;
                    }
                    MassActionReporter.AppendReadedFD(fd);
                    if (coUse)
                    {
                        int originalOperationType = fd.Cheque.CalculationSign;
                        int originalDocumentType = fd.Cheque.Document;
                        if (coTypeCorretion) fd.Cheque.Document = FD_DOCUMENT_NAME_CORRECTION_CHEQUE;
                        if (coReverseMethodReturn)
                        {
                            if (originalOperationType == 1) fd.Cheque.CalculationSign = 2;
                            else if (originalOperationType == 2) fd.Cheque.CalculationSign = 1;
                            else if (originalOperationType == 3) fd.Cheque.CalculationSign = 4;
                            else if (originalOperationType == 4) fd.Cheque.CalculationSign = 3;
                        }
                        else
                        {
                            if (originalOperationType == 1) fd.Cheque.CalculationSign = 3;
                            else if (originalOperationType == 2) fd.Cheque.CalculationSign = 4;
                            else if (originalOperationType == 3) fd.Cheque.CalculationSign = 1;
                            else if (originalOperationType == 4) fd.Cheque.CalculationSign = 2;
                        }
                        // пробиваем реверсный чек
                        fiscalPrinter.ReadDeviceCondition();
                        int lastFdBeforePerformFdreverse = fiscalPrinter.LastFd;
                        bool firstAttempRev = true;
                    SecondAttempAfterCleseShiftReverse:
                        returnRezult = fiscalPrinter.PerformFD(fd.Cheque);
                        fiscalPrinter.ReadDeviceCondition();

                        if (returnRezult)
                        { 
                            fdPerformedCounter++; 
                        }
                        else
                        {
                            errorsOccured++;
                            if(fiscalPrinter.LastFd > lastFdBeforePerformFdreverse)
                            {
                                LogHandle.ol("ФР вернул ошибку при офомлении ФД, но номер последнего ФД увеличился, возможно закончилась бумага.");
                                noSkipError = false;
                            }
                            else
                            {
                                if (closeShiftOnErrror)
                                {
                                    if (firstAttempRev)
                                    {
                                        MassActionReporter.AppendCorrFd(fd.Cheque, returnRezult);
                                        LogHandle.ol("Ошибка при оформлении ФД, пытаемся открыть и закрыть смену");
                                        firstAttempRev = false;
                                        int lastFdCOR = fiscalPrinter.LastFd;   // последний ФД переж закрытием-открытием смены
                                        fiscalPrinter.CloseShift();
                                        fiscalPrinter.OpenShift();
                                        fiscalPrinter.ReadDeviceCondition();
                                        if (lastFdCOR < fiscalPrinter.LastFd)
                                        {
                                            LogHandle.ol("Закрытие-открытие смены привело к увеличению номера пследнего ФД, пытаемся пробить чек повторно");
                                            goto SecondAttempAfterCleseShiftReverse;
                                        }
                                        else
                                        {
                                            LogHandle.ol("Закрытие-открытие смены произошло неудачно ");
                                            errorsOccured += 2;
                                            
                                        }
                                    }
                                    else
                                    {
                                        LogHandle.ol("Закрытие-открытие смены не исправило ситуацию с ошибкой пробития");
                                    }
                                }
                            }
                            
                            
                        }
                        if ((!returnRezult)&& noSkipError)
                        {
                            fdErrors.Add(fd);
                        }
                        MassActionReporter.AppendCorrFd(fd.Cheque, returnRezult);
                        fd.Cheque.CalculationSign = originalOperationType;
                        fd.Cheque.Document = originalDocumentType;
                    }
                    if (smoUse && (!coSmoErrorBlocker || returnRezult))
                    {
                        noSkipError = true;
                        if (smoTypeCorretion) fd.Cheque.Document = FD_DOCUMENT_NAME_CORRECTION_CHEQUE;

                        if (smoPayTypeRemove1 > 0)
                        {
                            double pay = 0;
                            if (smoPayTypeRemove1 == 1)
                            {
                                pay = fd.Cheque.Cash;
                                fd.Cheque.Cash = 0;
                            }
                            else if (smoPayTypeRemove1 == 2)
                            {
                                pay = fd.Cheque.ECash;
                                fd.Cheque.ECash = 0;
                            }
                            else if (smoPayTypeRemove1 == 3)
                            {
                                pay = fd.Cheque.Prepaid;
                                fd.Cheque.Prepaid = 0;
                            }
                            else if (smoPayTypeRemove1 == 4)
                            {
                                pay = fd.Cheque.Credit;
                                fd.Cheque.Credit = 0;
                            }
                            else if (smoPayTypeRemove1 == 5)
                            {
                                pay = fd.Cheque.Provision;
                                fd.Cheque.Provision = 0;
                            }
                            if (smoPayTypeSet1 == 1)
                            {
                                fd.Cheque.Cash += pay;
                            }
                            else if (smoPayTypeSet1 == 2)
                            {
                                fd.Cheque.ECash += pay;
                            }
                            else if (smoPayTypeSet1 == 3)
                            {
                                fd.Cheque.Prepaid += pay;
                            }
                            else if (smoPayTypeSet1 == 4)
                            {
                                fd.Cheque.Credit += pay;
                            }
                            else if (smoPayTypeSet1 == 5)
                            {
                                fd.Cheque.Provision += pay;
                            }
                        }
                        if (smoPayTypeRemove2 > 0)
                        {
                            double pay = 0;
                            if (smoPayTypeRemove2 == 1)
                            {
                                pay = fd.Cheque.Cash;
                                fd.Cheque.Cash = 0;
                            }
                            else if (smoPayTypeRemove2 == 2)
                            {
                                pay = fd.Cheque.ECash;
                                fd.Cheque.ECash = 0;
                            }
                            else if (smoPayTypeRemove2 == 3)
                            {
                                pay = fd.Cheque.Prepaid;
                                fd.Cheque.Prepaid = 0;
                            }
                            else if (smoPayTypeRemove2 == 4)
                            {
                                pay = fd.Cheque.Credit;
                                fd.Cheque.Credit = 0;
                            }
                            else if (smoPayTypeRemove2 == 5)
                            {
                                pay = fd.Cheque.Provision;
                                fd.Cheque.Provision = 0;
                            }
                            if (smoPayTypeSet2 == 1)
                            {
                                fd.Cheque.Cash += pay;
                            }
                            else if (smoPayTypeSet2 == 2)
                            {
                                fd.Cheque.ECash += pay;
                            }
                            else if (smoPayTypeSet2 == 3)
                            {
                                fd.Cheque.Prepaid += pay;
                            }
                            else if (smoPayTypeSet2 == 4)
                            {
                                fd.Cheque.Credit += pay;
                            }
                            else if (smoPayTypeSet2 == 5)
                            {
                                fd.Cheque.Provision += pay;
                            }
                        }

                        if (smoTaxRateRemove1 > 0)
                        {
                            foreach (var itm in fd.Cheque.Items)
                            {
                                if (
                                    itm.NdsRate == smoTaxRateRemove1
                                    || (smoTaxRateRemove1 == 11 && ((itm.NdsRate > 0 && itm.NdsRate < NDS_TYPE_0_LOC) || (itm.NdsRate > NDS_TYPE_FREE_LOC)))
                                    || (smoTaxRateRemove1 == NDS_TYPE_FREE_LOC && (itm.NdsRate == NDS_TYPE_EMPTY_LOC || itm.NdsRate == NDS_TYPE_FREE_LOC))
                                    )
                                {
                                    itm.NdsRate = smoTaxRateSet1;
                                    itm.Control();
                                }
                            }
                        }
                        if (smoTaxRateRemove2 > 0)
                        {
                            foreach (var i in fd.Cheque.Items)
                            {
                                if (
                                    i.NdsRate == smoTaxRateRemove2
                                    || (smoTaxRateRemove2 == 11 && ((i.NdsRate > 0 && i.NdsRate < NDS_TYPE_0_LOC) || (i.NdsRate > NDS_TYPE_FREE_LOC)))
                                    || (smoTaxRateRemove1 == NDS_TYPE_FREE_LOC && (i.NdsRate == NDS_TYPE_EMPTY_LOC || i.NdsRate == NDS_TYPE_FREE_LOC))
                                    )
                                {
                                    i.NdsRate = smoTaxRateSet2;
                                    i.Control();
                                }
                            }
                        }

                        if (smoSnoRemove > 0)
                        {
                            if (smoSnoRemove == 64 || smoSnoRemove == fd.Cheque.Sno)
                                fd.Cheque.Sno = smoSnoChange;
                        }

                        if (smoOperationType > 0)
                        {
                            fd.Cheque.CalculationSign = smoOperationType;
                        }

                        fd.Cheque.Control();
                        if (Math.Abs(fd.Cheque.TotalSum - oldTotal) > 0.009)
                        {
                            LogHandle.ol("После пересчета сумма итога отличается от суммы оригинального чека, восстанавливем оригинальное значение итога итог в оригинальном ФД " + oldTotal + "  после пересчета " + fd.Cheque.TotalSum);
                            fd.Cheque.TotalSum = oldTotal;
                        }

                        // здесь обрабатываем данные импорта
                        
                        if (importData && FormImport.table != null)
                        {
                            int row = FormImport.CurrentRow;
                            if (FormImport.table.GetUpperBound(0) >= row)
                            {
                                if (FormImport.index_buyerName > 0 && 
                                    FormImport.table[row, FormImport.index_buyerName] != null && 
                                    FormImport.table[row, FormImport.index_buyerName].ToString() != "")
                                {
                                    fd.Cheque.BuyerInformationBuyer = FormImport.table[row, FormImport.index_buyerName].ToString();
                                }
                                if (FormImport.index_buyerInn > 0 &&
                                    FormImport.table[row, FormImport.index_buyerInn] != null &&
                                    FormImport.table[row, FormImport.index_buyerInn].ToString() != "")
                                {
                                    fd.Cheque.BuyerInformationBuyerInn = FormImport.table[row, FormImport.index_buyerInn].ToString();
                                }
                                if (FormImport.index_buyerBirthDate > 0 &&
                                    FormImport.table[row, FormImport.index_buyerBirthDate]!=null &&
                                    FormImport.table[row, FormImport.index_buyerBirthDate].ToString() != "")
                                {
                                    fd.Cheque.BuyerInformationBuyerBirthday = FormImport.table[row, FormImport.index_buyerBirthDate].ToString();
                                }
                                if (FormImport.index_buyerCitizenship > 0 &&
                                    FormImport.table[row, FormImport.index_buyerCitizenship] != null &&
                                    FormImport.table[row, FormImport.index_buyerCitizenship].ToString() != "")
                                {
                                    fd.Cheque.BuyerInformationBuyerCitizenship = FormImport.table[row, FormImport.index_buyerCitizenship].ToString();
                                }
                                if (FormImport.index_buyerDocumentCode > 0 &&
                                    FormImport.table[row, FormImport.index_buyerDocumentCode] != null &&
                                    FormImport.table[row, FormImport.index_buyerDocumentCode].ToString() != "")
                                {
                                    fd.Cheque.BuyerInformationBuyerDocumentCode = FormImport.table[row, FormImport.index_buyerDocumentCode].ToString();
                                }
                                if (FormImport.index_buyerDocumentData > 0 &&
                                    FormImport.table[row, FormImport.index_buyerDocumentData] != null &&
                                    FormImport.table[row, FormImport.index_buyerDocumentData].ToString() != "")
                                {
                                    fd.Cheque.BuyerInformationBuyerDocumentData = FormImport.table[row, FormImport.index_buyerDocumentData].ToString();
                                }
                                if (FormImport.index_buyerAddress > 0 &&
                                    FormImport.table[row, FormImport.index_buyerAddress] != null &&
                                    FormImport.table[row, FormImport.index_buyerAddress].ToString() != "")
                                {
                                    fd.Cheque.BuyerInformationBuyerAddress = FormImport.table[row, FormImport.index_buyerAddress].ToString();
                                }
                            }
                            if (FormImport.ChangeServiceToPaymentCasino)
                            {
                                foreach(var itemCh in fd.Cheque.Items)
                                {
                                    if(itemCh.ProductType == 4)
                                    {
                                        itemCh.ProductType = 26;
                                    }
                                }
                            }
                            if (FormImport.Change1ToPaymentCasino)
                            {
                                foreach (var itemCh in fd.Cheque.Items)
                                {
                                    if (itemCh.ProductType == 1)
                                    {
                                        itemCh.ProductType = 26;
                                    }
                                }
                            }

                            FormImport.CurrentRow++;

                        }

                        fiscalPrinter.ReadDeviceCondition();
                        int lastFdBeforePerformFdStright = fiscalPrinter.LastFd;
                        bool firstAttempSt = true;
                    SecondAttempAfterCleseShiftStright:

                        // пробиваем прямой измененный чек
                        bool corrRezult = fiscalPrinter.PerformFD(fd.Cheque);
                        fiscalPrinter.ReadDeviceCondition();
                        if (corrRezult)
                        { fdPerformedCounter++; }
                        else
                        {
                            errorsOccured++;
                            if (fiscalPrinter.LastFd > lastFdBeforePerformFdStright)
                            {
                                LogHandle.ol("ФР вернул ошибку при офомлении ФД, но номер последнего ФД увеличился, возможно закончилась бумага.");
                                noSkipError = false;
                            }
                            else
                            {
                                if (closeShiftOnErrror)
                                {
                                    if (firstAttempSt)
                                    {
                                        MassActionReporter.AppendCorrFd(fd.Cheque, returnRezult);
                                        LogHandle.ol("Ошибка при оформлении ФД, пытаемся открыть и закрыть смену");
                                        firstAttempSt = false;
                                        int lastFdCOR = fiscalPrinter.LastFd;   // последний ФД переж закрытием-открытием смены
                                        fiscalPrinter.CloseShift();
                                        fiscalPrinter.OpenShift();
                                        fiscalPrinter.ReadDeviceCondition();
                                        if (lastFdCOR < fiscalPrinter.LastFd)
                                        {
                                            LogHandle.ol("Закрытие-открытие смены привело к увеличению номера пследнего ФД, пытаемся пробить чек повторно");
                                            goto SecondAttempAfterCleseShiftStright;
                                        }
                                        else
                                        {
                                            LogHandle.ol("Закрытие-открытие смены произошло неудачно ");
                                            errorsOccured += 2;
                                        }
                                    }
                                    else
                                    {
                                        LogHandle.ol("Закрытие-открытие смены не исправило ситуацию с ошибкой пробития");
                                    }
                                }
                            }

                        }
                        if ((!corrRezult) && noSkipError)
                        {
                            if (fdErrors.Count >0)
                            {
                                var fdLastError = fdErrors[fdErrors.Count - 1];
                                if(fd.Number != fdLastError.Number && fd.Time != fdLastError.Time && fd.Summ != fdLastError.Summ && fd.Type != fdLastError.Type)
                                {
                                    fdErrors.Add(fd);
                                }
                            }
                        }

                        MassActionReporter.AppendCorrFd(fd.Cheque, corrRezult);
                    }

                }
                while(xi < _fdFiltrated.Count)
                {
                    fdNotPerfomed.Add(_fdFiltrated[xi++]);
                }
                if (fdErrors.Count > 0)
                {
                    string dtstr = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Операции_Ошибки_Чеки_" + dtstr);
                    Directory.CreateDirectory(path);
                    int fdCounder = 0;
                    foreach (var fd in fdErrors)
                    {
                        try
                        {
                            string ssss = JsonChequeConstructor.CreateJsonString(fd);
                            if (!string.IsNullOrEmpty(ssss) && ssss.Length > 30)
                            {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(path, fdDocTypesShirt[fd.Type] + "_" + fd.Summ + "_" + fd.Time.ToString("yyyy_MM_dd_HHmm") + "__saved_" + Guid.NewGuid().ToString() + ".json"), false, System.Text.Encoding.UTF8))
                                {
                                    sw.Write(ssss);
                                    fdCounder++;
                                }
                            }
                            else
                            {
                                LogHandle.ol("Ошибка при сохранении файла " + fdDocTypesShirt[fd.Type] + "_" + fd.Summ + "_" + fd.Time.ToString("yyyy_MM_dd_") + "  содержимое " + ssss);
                            }


                        }
                        catch (Exception exc)
                        {
                            LogHandle.ol("saving " + fd.ToString() + " in " + path + " ocures error " + exc.Message);
                        }

                    }
                    PushMessage("Чеков сохранено " + fdCounder);



                }

                if (fdNotPerfomed.Count > 0)
                {
                    string dtstr = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Операции_Не_выполнено_Чеки_" + dtstr);
                    Directory.CreateDirectory(path);
                    int fdCounder = 0;
                    foreach (var fd in fdNotPerfomed)
                    {
                        try
                        {
                            string ssss = JsonChequeConstructor.CreateJsonString(fd);
                            if (!string.IsNullOrEmpty(ssss) && ssss.Length > 30)
                            {
                                using (StreamWriter sw = new StreamWriter(Path.Combine(path, fdDocTypesShirt[fd.Type] + "_" + fd.Summ + "_" + fd.Time.ToString("yyyy_MM_dd_HHmm") + "__saved_" + Guid.NewGuid().ToString() + ".json"), false, System.Text.Encoding.UTF8))
                                {
                                    sw.Write(ssss);
                                    fdCounder++;
                                }
                            }
                            else
                            {
                                LogHandle.ol("Ошибка при сохранении файла " + fdDocTypesShirt[fd.Type] + "_" + fd.Summ + "_" + fd.Time.ToString("yyyy_MM_dd_") + "  содержимое " + ssss);
                            }


                        }
                        catch (Exception exc)
                        {
                            LogHandle.ol("saving " + fd.ToString() + " in " + path + " ocures error " + exc.Message);
                        }

                    }
                    PushMessage("Чеков сохранено " + fdCounder);



                }



                _fdFiltrated.Clear();
                _fdReaded.Clear();
                status.AllDone();
                processInterruptor = false;
                this.Enabled = true;
                this.Activate();
                _askRepeatRead = true;
                textBox_task2ReadedCount.Text = "0";
                textBox_filtrateCounter.Text = "0";
                textBox_macorRezult.Text = fdPerformedCounter + " ФД оформлено";
            }
            else if (sender == label_task2PerformCorrection)
            {
                tabControl_subTask2.SelectedIndex = 2;
            }
            else if (sender == label_task2FilterPage)
            {
                tabControl_subTask2.SelectedIndex = 1;
            }
            else if (sender == label_task2ReadSettings)
            {
                tabControl_subTask2.SelectedIndex = 0;
            }
            else if (sender == checkBox_task2CancelOperationSwitcher)
            {
                groupBox_task2CancelSettings.Enabled = checkBox_task2CancelOperationSwitcher.Checked;
            }
            else if (sender == checkBox_task2StrightModified)
            {
                groupBox_task2CorrectionFinal.Enabled = checkBox_task2StrightModified.Checked;
            }
            else if (sender == checkBox_task2FilterPaidMethod1)
            {
                comboBox_task2FilterPaid1.Enabled = checkBox_task2FilterPaidMethod1.Checked;
            }
            else if (sender == checkBox_task2FilterPaidMethod2)
            {
                comboBox_task2FilterPaid2.Enabled = checkBox_task2FilterPaidMethod2.Checked;
            }
            else if (sender == checkBox_task2NdsType1)
            {
                comboBox_task2NdsType1.Enabled = checkBox_task2NdsType1.Checked;
            }
            else if (sender == checkBox_task2NdsType2)
            {
                comboBox_task2NdsType2.Enabled = checkBox_task2NdsType2.Checked;
            }
            else if (sender == checkBox_task2FilterSno)
            {
                comboBox_task2Sno.Enabled = checkBox_task2FilterSno.Checked;
            }
            else if (sender == checkBox_task2FilterCalculationSign)
            {
                comboBox_task2OperationType.Enabled = checkBox_task2FilterCalculationSign.Checked;
            }
            else if (sender == checkBox_task2changePayment1)
            {
                comboBox_task2Payment1Original.Enabled = checkBox_task2changePayment1.Checked;
                comboBox_task2Payment1Changed.Enabled = checkBox_task2changePayment1.Checked;
            }
            else if (sender == checkBox_task2changePayment2)
            {
                comboBox_task2Payment2Original.Enabled = checkBox_task2changePayment2.Checked;
                comboBox_task2Payment2Changed.Enabled = checkBox_task2changePayment2.Checked;
            }
            else if (sender == checkBox_task2TaxTypeChanger1)
            {
                comboBox_taxType1Original.Enabled = checkBox_task2TaxTypeChanger1.Checked;
                comboBox_task2taxType1Changed.Enabled = checkBox_task2TaxTypeChanger1.Checked;
            }
            else if (sender == checkBox_task2TaxTypeChanger2)
            {
                comboBox_taxType2Original.Enabled = checkBox_task2TaxTypeChanger2.Checked;
                comboBox_task2taxType2Changed.Enabled = checkBox_task2TaxTypeChanger2.Checked;
            }
            else if (sender == checkBox_task2SnoChanger)
            {
                comboBox_task2SnoOriginal.Enabled = checkBox_task2SnoChanger.Checked;
                comboBox_task2SnoChanged.Enabled = checkBox_task2SnoChanger.Checked;
            }
            else if (sender == checkBox_task2OperationSignChanger)
            {
                comboBox_OperationSignChanged.Enabled = checkBox_task2OperationSignChanger.Checked;
            }
            else if (sender == button_clearList2)
            {
                textBox_task2NumberList.Text = "";
                if (_tpOldVarUnlocker < 5)
                {
                    _tpOldVarUnlocker++;
                    if (_tpOldVarUnlocker > 4)
                    {
                        tabControl_subTask2.TabPages.Add(_hidedPageOldVar);
                    }
                }

            }
            else if (sender == button_olapRepV2)
            {
                OlapReportV2 olapForm = new OlapReportV2(this);
                olapForm.FR = fiscalPrinter;
                olapForm.ShowDialog();
            }
            else if (sender == button_checkFdList2)
            {
                char appender;
                switch (comboBox_task2ReadSeparator.SelectedIndex)
                {
                    case 0:
                    case 1:
                        appender = ',';
                        break;
                    case 2:
                    case 3:
                        appender = ';';
                        break;
                    case 4:
                    case 5:
                        appender = ' ';
                        break;
                    case 6:
                    case 7:
                    default:
                        appender = '\n';
                        break;
                }
                string numlist = textBox_task2NumberList.Text;
                string[] numbers = numlist.Split(appender);
                List<int> numsFd = new List<int>();
                foreach (string s in numbers)
                {
                    int n = -1;
                    int.TryParse(s, out n);
                    if (n > 0 && !numsFd.Contains(n))
                    {
                        numsFd.Add(n);
                    }
                }
                numsFd.Sort();
                StringBuilder stringBuilder = new StringBuilder();
                foreach (int n in numsFd)
                {
                    stringBuilder.Append(n);
                    if (appender == '\n')
                        stringBuilder.AppendLine();
                    else
                        stringBuilder.Append(appender);
                }
                textBox_task2NumberList.Text = stringBuilder.ToString();


            }
            else if (sender == button_readjsonList)
            {
                _fdReaded.Clear();
                _fdFiltrated.Clear();
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "JSON format|*.json;|All files (*.*)|*.*";
                fileDialog.Multiselect = true;
                _readed_fd_ok = false;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    _readed_fd_ok = true;
                    dataGridView_jsonList.Rows.Clear();
                    _selectedRowLast = -1;
                    if (checkBox_cleanJsonCollection.Checked)
                    {
                        _jsonFdCollection.Clear();
                    }
                    int filesToRead = fileDialog.FileNames.Length;

                    var l = JsonChequeConstructor.ReadDocuments(fileDialog.FileNames);

                    PushMessage("распознано чеков " + l.Count);
                    _jsonFdCollection.AddRange(l);

                    foreach (var fd in _jsonFdCollection)
                    {
                        string timeReprez;
                        if (fd.Time == null || fd.Time.Year < 2017)
                        {
                            timeReprez = "Нет времени";
                        }
                        else
                        {
                            timeReprez = fd.Time.ToString("dd.MM.yyyy HH:mm");
                        }
                        dataGridView_jsonList.Rows.Add(fdDocTypesShirt[fd.Type], timeReprez, fd.Summ, fd.Number, fd.FiscalSign);
                    }
                    this.Enabled = true;
                }
            }
            else if (sender == button_task2SaveFdToJson || sender == button_task2SaveFdToJsonFiltrated)
            {
                List<FnReadedDocument> _fdRDFl;
                if (sender == button_task2SaveFdToJson)
                    _fdRDFl = _fdReaded;
                else
                {
                    _fdRDFl = _fdFiltrated;
                }
                if (_fdRDFl.Count == 0)
                {
                    PushMessage("Нечего сохранять.");
                }
                else
                {
                    //JsonChequeConstructor.CreateJsonString(_fdReaded[0]);
                    FolderBrowserDialog dialog = new FolderBrowserDialog();
                    dialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
                    dialog.Description = "Выберите путь для сохранения";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string path = dialog.SelectedPath;
                        int fdCounder = 0;
                        foreach (var fd in _fdRDFl)
                        {
                            try
                            {
                                string ssss = JsonChequeConstructor.CreateJsonString(fd);
                                if (!string.IsNullOrEmpty(ssss) && ssss.Length > 30)
                                {
                                    using (StreamWriter sw = new StreamWriter(Path.Combine(path, fdDocTypesShirt[fd.Type]+"_ФД"+fd.Number.ToString("D6") + "_ИТ_" + fd.Summ + "_" + fd.Time.ToString("yyyy_MM_dd_HHmm") + "__sav_" + Guid.NewGuid().ToString().Substring(0,3) + ".json"), false, System.Text.Encoding.UTF8))
                                    {
                                        sw.Write(ssss);
                                        fdCounder++;
                                    }
                                }
                                else
                                {
                                    LogHandle.ol("Ошибка при сохранении файла " + fdDocTypesShirt[fd.Type] + "_" + fd.Summ + "_" + fd.Time.ToString("yyyy_MM_dd_") + "  содержимое " + ssss);
                                }


                            }
                            catch (Exception exc)
                            {
                                LogHandle.ol("saving " + fd.ToString() + " in " + path + " ocures error " + exc.Message);
                            }

                        }
                        PushMessage("Чеков сохранено " + fdCounder);

                    }

                }
            }
            else if(sender == checkBox_task2excelImport)
            {
                if (checkBox_task2excelImport.Checked)
                {
                    FormImport form1 = new FormImport();
                    form1.ShowDialog();
                }
            }
            else if(sender == button_ofdFormat)
            {
                FormOfdExport foe = new FormOfdExport(fiscalPrinter);
                
                foe.ShowDialog();

            }
        }

        private List<FnReadedDocument> _jsonFdCollection = new List<FnReadedDocument>();
        private List<FnReadedDocument> _fdReaded = new List<FnReadedDocument>();
        private List<FnReadedDocument> _fdFiltrated = new List<FnReadedDocument>();

        private void label21_Click_randomSampleUnits(object sender, EventArgs e)
        {
            if(SAMLE_ITEMS.Contains(textBox_cheqItemName.Text)) textBox_cheqItemName.Text = SAMLE_ITEMS[new Random().Next(0, SAMLE_ITEMS.Length - 1)];
        }

        private void FormFiscalDocChanged(bool writePaySums = false, bool rounded = false)
        {
            _roundTally = rounded;
            if (writePaySums)
            {
                textBox_cheq_payCash.Text = fFDoc.Cash.ToString();
                textBox_cheq_payCashless.Text = fFDoc.ECash.ToString();
                textBox_cheq_payAvans.Text = fFDoc.Prepaid.ToString();
                textBox_cheq_payCredit.Text = fFDoc.Credit.ToString();
                textBox_cheq_payProvision.Text = fFDoc.Provision.ToString();
            }

            textBox_chequeTotalSum.Text = fFDoc.TotalSum.ToString();
            textBox_cheqTotalPaidSum.Text = Math.Round((fFDoc.Cash + fFDoc.ECash + fFDoc.Prepaid + fFDoc.Credit + fFDoc.Provision), 2).ToString();
            textBox_taxPanelNdsFree.Text = fFDoc.NdsFree.ToString();
            textBox_taxPanelNds20.Text = fFDoc.Nds20.ToString();
            textBox_taxPanelNds10.Text = fFDoc.Nds10.ToString();
            textBox_taxPanelNds0.Text = fFDoc.Nds0.ToString();
            textBox_taxPanelNds20120.Text = fFDoc.Nds20120.ToString();
            textBox_taxPanelNds10110.Text = fFDoc.Nds10110.ToString();
            textBox_taxPanelNds5.Text = fFDoc.Nds5.ToString();
            textBox_taxPanelNds7.Text = fFDoc.Nds7.ToString();
            textBox_taxPanelNds5105.Text = fFDoc.Nds5105.ToString();
            textBox_taxPanelNds7107.Text = fFDoc.Nds7107.ToString();

            textBox_taxPanel_NDStotal.Text = Math.Round((fFDoc.Nds10 + fFDoc.Nds20 + fFDoc.Nds10110 + fFDoc.Nds20120 + fFDoc.Nds5 + fFDoc.Nds7 + fFDoc.Nds5105+fFDoc.Nds7107), 2).ToString();
            textBox_chequeDocCondition.Text = fFDoc.Condition();

            if (fFDoc.IsNotPaid)
            {
                if (checkBox_autoPayCash.Checked) textBox_chequeDocCondition.Text +=  "Документ будет доплачен наличными";
                else if (checkBox_autoPayECash.Checked) textBox_chequeDocCondition.Text +=  "Документ будет доплачен безналичными";
                else if (checkBox_autoPayPrepay.Checked) textBox_chequeDocCondition.Text += "Документ будет доплачен авансом";
                else if (checkBox_autoPayCredit.Checked) textBox_chequeDocCondition.Text +=  "Документ будет доплачен кредитом";
                else if (checkBox_autoPayProvision.Checked) textBox_chequeDocCondition.Text += "Документ будет доплачен ВП";
                else textBox_chequeDocCondition.Text = fFDoc.Condition();
            }
            if (_roundTally) textBox_chequeDocCondition.Text += Environment.NewLine + "Округление итога";

            if ( fFDoc.Document == FD_DOCUMENT_NAME_CORRECTION_CHEQUE
                && fFDoc.FFDVer <= FR_FFD105
                && fFDoc.Items.Count == 0
                && (textBox_chequeDocCondition.Text == "" || textBox_chequeDocCondition.Text == "нет информации" || textBox_chequeDocCondition.Text == "документ со сдачей"+Environment.NewLine)
                && Math.Round((fFDoc.Cash + fFDoc.ECash + fFDoc.Prepaid + fFDoc.Credit + fFDoc.Provision), 2)>0.00999) 
                    textBox_chequeDocCondition.Text = "Для оформления ненулевого чека коррекции в ФФД 1.1 или 1.2 добавьте предмет расчета равный итогу чека";

            bool manualTaxes = fFDoc.AvailableCommonTaxes;
            button_cheqNdsFreeFill.Enabled = manualTaxes;
            button_cheqNds20Fill.Enabled = manualTaxes;
            button_cheqNds10Fill.Enabled = manualTaxes;
            button_cheqNds0Fill.Enabled = manualTaxes;
            button_cheqNds10110Fill.Enabled = manualTaxes;
            button_cheqNds20120Fill.Enabled = manualTaxes;
            button_cheqNds5105Fill.Enabled = manualTaxes;
            button_cheqNds7107Fill.Enabled = manualTaxes;
            button_cheqNds7Fill.Enabled = manualTaxes;
            button_cheqNds5Fill.Enabled = manualTaxes;
        }

        private void MapFormFiscalDocument()
        {
            _skipSaveUpReq = true;
            if (fFDoc == null)
                fFDoc = new FiscalCheque();
            checkBox_itemEdit.Checked = false;
            textBox_chequePropertiesData.Text = fFDoc.PropertiesData;

            textBox_cheque1085propertiesPropertyName.Text = fFDoc.PropertiesPropertyName;
            textBox_cheque1086propertiesPropertyValue.Text = fFDoc.PropertiesPropertyValue;

            textBox_cheq_byyerEmail.Text = fFDoc.EmailPhone;
            textBox_cheq_cashierInn.Text = fFDoc.CashierInn;
            textBox_cheq_cashierName.Text = fFDoc.Cashier;
            textBox_cheq_byerName.Text = fFDoc.BuyerInformationBuyer;
            textBox_cheq_buyerInn.Text = fFDoc.BuyerInformationBuyerInn;
            textBox_cheque_correctionDescribtion.Text = fFDoc.CorrectionDocDescriber;
            textBox_cheque_orderNum.Text = fFDoc.CorrectionOrderNumber;
            dateTimePicker_chequeCorrectionDate.Value = fFDoc.CorrectionDocumentDate;
            if (!checkBox_cheq_docType_locker.Checked)
            {
                comboBox_cheq_docName.SelectedIndex = fFDoc.Document == FD_DOCUMENT_NAME_CHEQUE ? 0 : 1;
            }
            if (!checkBox_cheq_operationType_locker.Checked)
            {
                comboBox_cheq_operationSign.SelectedIndex = fFDoc.CalculationSign - 1;
            }
            
            if (!checkBox_cheq_sno_locker.Checked)
            {
                if (fFDoc.Sno == FR_SNO_OSNO_BF)
                    comboBox_cheq_sno.SelectedIndex = 0;
                else if (fFDoc.Sno == FR_SNO_USN_D_BF)
                    comboBox_cheq_sno.SelectedIndex = 1;
                else if (fFDoc.Sno == FR_SNO_USN_D_R_BF)
                    comboBox_cheq_sno.SelectedIndex = 2;
                else if (fFDoc.Sno == FR_SNO_ESHN_BF)
                    comboBox_cheq_sno.SelectedIndex = 3;
                else if (fFDoc.Sno == FR_SNO_PSN_BF)
                    comboBox_cheq_sno.SelectedIndex = 4;
            }
            
            comboBox_cheque_correctionType.SelectedIndex = fFDoc.CorrectionTypeNotFtag;
            RecreateItems();
            FormFiscalDocChanged(true);
            if(panel_consumptionItemsContent.Controls.Count > 0)
            {
                foreach (CheckBox c in panel_consumptionItemsContent.Controls[0].Controls.OfType<CheckBox>())
                {
                    c.Checked = true;
                    break;
                }
            }
            _skipSaveUpReq = false;
        }

        private static readonly string READ_BT = "Прочитать";

        static Point READED_FD_LB_INFO = new System.Drawing.Point(3, 0);
        void AddSingleReadedFd(int localNumber, FnReadedDocument doc)
        {
            bool operationsEnable = doc.Type == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE 
                || doc.Type == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE 
                || doc.Type == FTAG_FISCAL_DOCUMENT_TYPE_BSO 
                || doc.Type == FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION;
            //if (localNumber % 100 == 0) Debug.WriteLine(localNumber);
            var checkBox_fdChooser = new CheckBox();
            checkBox_fdChooser.AutoSize = true;
            checkBox_fdChooser.Location = new System.Drawing.Point(297, 26);
            checkBox_fdChooser.Size = new System.Drawing.Size(15, 11);
            checkBox_fdChooser.UseVisualStyleBackColor = true;
            checkBox_fdChooser.Enabled = operationsEnable;
            checkBox_fdChooser.CheckedChanged += HandleFdOfFormList;
            var button_fdRead = new Button();
            button_fdRead.Location = new System.Drawing.Point(250, 1);
            button_fdRead.Size = new System.Drawing.Size(75, 23);
            button_fdRead.Text = READ_BT;
            button_fdRead.UseVisualStyleBackColor = true;
            button_fdRead.Enabled = operationsEnable;
            button_fdRead.Click += HandleFdOfFormList;

            var label_fdFiscalSign = new Label();
            label_fdFiscalSign.BorderStyle = System.Windows.Forms.BorderStyle.None;
            label_fdFiscalSign.Location = READED_FD_LB_INFO;
            label_fdFiscalSign.Size = new System.Drawing.Size(232, 43);
            label_fdFiscalSign.Text = doc.Reprezent;
            label_fdFiscalSign.Font = new System.Drawing.Font("Segoe UI", 8.5F);


            var groupBox_fd = new Panel();
            groupBox_fd.BackColor = System.Drawing.Color.Gainsboro;
            groupBox_fd.Controls.Add(checkBox_fdChooser);
            groupBox_fd.Controls.Add(button_fdRead);
            groupBox_fd.Controls.Add(label_fdFiscalSign);
            groupBox_fd.Location = new System.Drawing.Point(0, 5 + 45 * localNumber);
            groupBox_fd.Name = "FD_"+doc.Number.ToString();
            groupBox_fd.Text = doc.Number.ToString();
            groupBox_fd.Size = new System.Drawing.Size(327, 44);

            pane_tabPage_frFdContent01.Controls.Add(groupBox_fd);

            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add("Копировать информацию для поиска в ОФД");
            cm.MenuItems.Add("Копировать информацию о ФД");
            cm.MenuItems.Add("Копировать ФП");
            cm.MenuItems.Add("Копировать номер ФД");
            cm.MenuItems.Add("Копировать дату ФД");
            cm.MenuItems[0].Click += СopyFdInfo;
            cm.MenuItems[1].Click += СopyFdInfo;
            cm.MenuItems[2].Click += СopyFdInfo;
            cm.MenuItems[3].Click += СopyFdInfo;
            cm.MenuItems[4].Click += СopyFdInfo;
            groupBox_fd.ContextMenu = cm;
        }

        private void СopyFdInfo(object sender, EventArgs e)
        {
            MenuItem m = sender as MenuItem;
            LogHandle.ol(m.Index.ToString());

            ContextMenu cm = m.Parent as ContextMenu;
            
            LogHandle.ol(cm.SourceControl.GetType().ToString());
            Panel p = cm.SourceControl as Panel;
            string fdInfo = p.Controls.OfType<Label>().First().Text;
            if (string.IsNullOrEmpty(fdInfo))
            {
                LogHandle.ol("Ошибка с доступом к информации о ФД");
                return;
            }
            switch (m.Index)
            {
                case 0:     // Копировать информацию о ФД
                    Clipboard.SetText( (KKMInfoTransmitter[FR_OWNER_USER_KEY] + Environment.NewLine +"ЗН ККТ "+ KKMInfoTransmitter[FR_SERIAL_KEY] +Environment.NewLine +fdInfo));
                    break;
                case 1:
                    Clipboard.SetText( fdInfo);
                    break;
                case 2:     // Копировать ФП
                    if (fdInfo.Contains("ФП:"))
                    {
                        string fp = fdInfo.Substring(fdInfo.IndexOf("ФП:") + 4);
                        if (fdInfo.Contains("ОФД"))
                        {
                            int t = 0;
                            while (fp[t]>='0'&& fp[t] <= '9' && t < fp.Length)
                            {
                                t++;
                            }
                            if (t > 0)
                            {
                                fp = fp.Substring(0,t);
                            }
                        }
                        Clipboard.SetText(fp);
                    }
                    break;
                case 3:     // Копировать № ФД
                    if(fdInfo.Contains(' '))
                    {
                        Clipboard.SetText(fdInfo.Substring(0,fdInfo.IndexOf(' ')));
                    }
                    break;
                case 4:     // Копировать дату ФД
                    if(fdInfo.Contains("Время "))
                    {
                        Clipboard.SetText(fdInfo.Substring(fdInfo.IndexOf("Время ")+6,10));
                    }
                    break;
            }
        }

        private void ComboBox_cheq_itemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_cheq_itemType.SelectedIndex == 28 || comboBox_cheq_itemType.SelectedIndex == 29)
                comboBox_cheq_itemType.SelectedIndex = 1;
        }

        private List<int> _numbersForMassCorrections = new List<int>();

        bool _handleFdCheckBoxes = true;
        private void HandleFdOfFormList(object sender, EventArgs e)
        {
            if (sender is Button && (sender as Button).Text == READ_BT)
            {
                FnReadedDocument frd = fiscalPrinter.ReadFD(int.Parse((sender as Button).Parent.Text), true);
                fFDoc = frd.Cheque;
                if (fFDoc == null) fFDoc = new FiscalCheque();
                MapFormFiscalDocument();
                if (fFDoc.Document != FD_DOCUMENT_NAME_CORRECTION_CHEQUE)
                    dateTimePicker_chequeCorrectionDate.Value = frd.Time;
                textBox_list1readedFdNum.Text = (sender as Button).Parent.Text;
                double ism = 0;
                foreach (var i in fFDoc.Items) ism += i.Sum;
                if (ism - fFDoc.TotalSum > 0.0099) _roundTally = true;
            }
            if (sender is CheckBox && _handleFdCheckBoxes)
            {
                CheckBox cb = (CheckBox)sender;
                int docNumber = int.Parse(cb.Parent.Text);
                string appender;
                switch (comboBox_task2ReadSeparator.SelectedIndex)
                {
                    case 0:
                    case 1:
                        appender = ",";
                        break;
                    case 2:
                    case 3:
                        appender = ";";
                        break;
                    case 4:
                    case 5:
                        appender = " ";
                        break;
                    case 6:
                    case 7:
                    default:
                        appender =  Environment.NewLine;
                        break;
                }
                if (cb.Checked)
                {
                    textBox_task2NumberList.Text = textBox_task2NumberList.Text + docNumber + appender;
                }
                else
                {
                    string list = textBox_task2NumberList.Text;
                    if (list.StartsWith(docNumber + appender))
                    {
                        list = list.Substring((docNumber + appender).Length);
                    }
                    if(list.EndsWith(appender+docNumber))
                    {
                        list = list.Substring(0, list.Length - (appender + docNumber).Length);
                    }
                    while (list.Contains(appender+docNumber+appender))
                    {
                        list = list.Replace(appender + docNumber + appender,appender);
                    }

                    textBox_task2NumberList.Text = list;
                }

            }
        }

        private void MainForm_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            DialogBoxes(button_aboutWnd,null);
        }

        int _unlockerMassActions = 0;

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button==MouseButtons.Left)
            {
                try
                {
                    linkLabel1.LinkVisited = true;
                    System.Diagnostics.Process.Start("https://docs.google.com/document/d/1ukm31-qrXfWCKJook8FenadMraMBRKa_Cj4_Oqjtxkc/edit?usp=sharing");
                }
                catch
                {
                    PushMessage("Unable to open link that was clicked.");
                }
            }

        }

        private int _selectedRowLast = -1;
        private void JsonGridSelectFd(object sender, DataGridViewCellEventArgs e)
        {
            int x = e.RowIndex;
            if(x != _selectedRowLast)
            {
                _selectedRowLast = x;
                if(x < _jsonFdCollection.Count)
                {
                    fFDoc = _jsonFdCollection[x].Cheque;
                    MapFormFiscalDocument();
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void MassActionsOldVar(object sender, EventArgs e)
        {
            if (sender == button_MassActionCleanList)
            {
                    if (_unlockerMassActions < 10)
                    {
                        _unlockerMassActions++;
                    }
                    else
                    {
                        if (_unlockerMassActions == 10)
                        {
                            button_olapRepV2.Enabled = true;
                            button_itemExtendedEditor.Enabled = true;
                        }
                        if (_unlockerMassActions < 15)
                        {
                            _unlockerMassActions++;
                        }
                        if (_unlockerMassActions == 15)
                        {
                            button_ofdFormat.Enabled = true;
                            button_testAction.Enabled = true;
                            checkBox_changeDate.Enabled = true;
                            button_testTerminalFnLib.Enabled = true;
                            button_testTerminalFnLib.Visible = true;
                            button_efnTestRun.Visible = true;
                            checkBox_testRun_noExDuTm.Visible = true;
                            checkBox_efnTestRunNoCorrection.Visible = true;
                            textBox_efnTestRunCheques.Visible = true;
                            checkBox_noNewNds.Visible = true;
                        }
                    }
            }
            else if (sender == button_fdReadedSelectAll)
            {
                _handleFdCheckBoxes = false;
                string numbers = "";
                string appender;
                switch (comboBox_task2ReadSeparator.SelectedIndex)
                {
                    case 0:
                    case 1:
                        appender = ",";
                        break;
                    case 2:
                    case 3:
                        appender = ";";
                        break;
                    case 4:
                    case 5:
                        appender = " ";
                        break;
                    case 6:
                    case 7:
                    default:
                        appender = Environment.NewLine;
                        break;
                }
                foreach (Control c in pane_tabPage_frFdContent01.Controls)
                    foreach (CheckBox c2 in c.Controls.OfType<CheckBox>())
                        if (c2.Enabled && !c2.Checked)
                        {
                            c2.Checked = true;
                            numbers += c.Text+ appender;
                            
                        }
                textBox_task2NumberList.Text += numbers;
                _handleFdCheckBoxes = true;
            }
            else if (sender == button_fdReadedSelectNone)
            {
                //textBox_task2NumberList.Text = "";
                string appender;
                switch (comboBox_task2ReadSeparator.SelectedIndex)
                {
                    case 0:
                    case 1:
                        appender = ",";
                        break;
                    case 2:
                    case 3:
                        appender = ";";
                        break;
                    case 4:
                    case 5:
                        appender = " ";
                        break;
                    case 6:
                    case 7:
                    default:
                        appender = Environment.NewLine;
                        break;
                }
                List<string> stringsToRemove = new List<string>();

                _handleFdCheckBoxes = false;
                foreach (Control c in pane_tabPage_frFdContent01.Controls)
                    foreach (CheckBox c2 in c.Controls.OfType<CheckBox>())
                        if (c2.Enabled)
                        {
                            c2.Checked = false;
                            stringsToRemove.Add(c.Text);
                        }


                string list = textBox_task2NumberList.Text;
                foreach (string s in stringsToRemove)
                {

                    if (list.StartsWith(s + appender))
                    {
                        list = list.Substring((s + appender).Length);
                    }
                    if (list.EndsWith(appender + s))
                    {
                        list = list.Substring(0, list.Length - (appender + s).Length);
                    }
                    while (list.Contains(appender + s + appender))
                    {
                        list = list.Replace(appender + s + appender, appender);
                    }
                }

                textBox_task2NumberList.Text = list;
                _handleFdCheckBoxes = true;
            }
            else if (sender == button_chooseCheques2)
            {
                int count = checkedListBox_fdListV2.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    //LogHandle.ol(checkedListBox_fdListV2.Items[i].ToString());
                    if (checkedListBox_fdListV2.Items[i].ToString().Contains("Чек"))
                        checkedListBox_fdListV2.SetItemCheckState(i, CheckState.Checked);
                }
            }
            else if (sender == button_SendCheckedToTask)
            {
                StringBuilder sbList = new StringBuilder(textBox_task2NumberList.Text);
                string separator;
                if(comboBox_task2ReadSeparator.SelectedIndex <=1)
                {
                    separator = ",";
                }
                else if(comboBox_task2ReadSeparator.SelectedIndex <= 3)
                {
                    separator = ";";
                }
                else if(comboBox_task2ReadSeparator.SelectedIndex <= 5)
                {
                    separator = " ";
                }
                else
                {
                    separator = Environment.NewLine;
                }
                //_numbersForMassCorrections.Clear();
                //listBox_fdNumsForMassCorection.Items.Clear();
                int count = checkedListBox_fdListV2.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    if (checkedListBox_fdListV2.GetItemChecked(i))
                    {
                        string fdOl = checkedListBox_fdListV2.Items[i].ToString();
                        int fd = int.Parse(fdOl.Substring(0, fdOl.IndexOf(' ')));
                        //_numbersForMassCorrections.Add(fd);
                        //listBox_fdNumsForMassCorection.Items.Add(fd);
  
                        sbList.Append(fd);
                        sbList.Append(separator);
                    }
                }
                textBox_task2NumberList.Text = sbList.ToString();
            }
            else if (sender == button_fastCorrectionsPerform)
            {
                if (fiscalPrinter == null || !fiscalPrinter.IsConnected)
                {
                    PushMessage("Соединение не установлено");
                    return;
                }
                CalculateGrid();
                processInterruptor = false;
                int fdCount = int.Parse(textBox_fastCorretionDocumentCounter.Text);
                if (fdCount > 0)
                {
                    MassActionReporter reporter = new MassActionReporter(fdCount);
                    this.Enabled = false;
                    List<int> listToRemove = new List<int>();
                    // создаем новый поток
                    Thread myThread = new Thread(new ThreadStart(Run_status_window));
                    myThread.Start(); // запускаем поток
                    this.Enabled = false;
                    int calculationSign = comboBox_fastCorrectionsOperationSign.SelectedIndex + 1;
                    int ndsRate = comboBox_fastCorrectionsNds.SelectedIndex + 1;
                    int correctionType = comboBox_fastCorrectionsType.SelectedIndex;
                    string describtions = textBox_fastCorrectionsDescribtion.Text;
                    string orderNumber = textBox_fastCorrectionsOrderNum.Text;
                    int doc = 0;
                    string ofDocs = " из " + fdCount;
                    foreach (DataGridViewRow row in dataGridView_task.Rows)
                    {
                        if (processInterruptor) break;
                        if (row.Cells[0].Value == null || ((string)(row.Cells[0].Value)).Length == 0 || ((string)(row.Cells[0].Value))[0] == '#') continue;
                        doc++;
                        if (status != null) { status.Message("Обработка документа " + doc + ofDocs); } else { LogHandle.ol("Не открыто окно статуса"); };
                        double tally = double.Parse(row.Cells[0].Value.ToString());
                        ConsumptionItem item = new ConsumptionItem(describtions, tally, 1, tally, 1, 4, ndsRate);
                        FiscalCheque cheque = new FiscalCheque();
                        cheque.AddItem(item);
                        cheque.CalculationSign = calculationSign;
                        cheque.Document = FD_DOCUMENT_NAME_CORRECTION_CHEQUE;
                        cheque.CorrectionTypeNotFtag = correctionType;
                        cheque.CorrectionDocDescriber = describtions;
                        cheque.CorrectionOrderNumber = orderNumber;
                        cheque.CorrectionDocumentDate = DateTime.Parse(row.Cells[3].Value.ToString());
                        cheque.Cash = double.Parse(FiscalPrinter.ReplaceBadDecimalSeparatorPoint(row.Cells[1].Value.ToString()));
                        cheque.ECash = double.Parse(FiscalPrinter.ReplaceBadDecimalSeparatorPoint(row.Cells[2].Value.ToString()));
                        int snoSelected = 1<<comboBox_cheq_sno.SelectedIndex;
                        if (snoSelected >= 8) snoSelected *= 2;
                        cheque.Sno = snoSelected;
                        cheque.Control();
                        reporter.ReadDoc(fiscalPrinter.LastFd + 1, cheque);
                        if (fiscalPrinter.PerformFD(cheque))
                        {
                            reporter.OperationOK();
                            row.Cells[0].Style.BackColor = Color.LightGreen;
                        }
                        else
                        {
                            reporter.Step2Failed();
                            row.Cells[0].Style.BackColor = Color.Red;
                        }
                    }
                    if (status != null) status.AllDone();
                    reporter.ErrorsListOutput();
                    processInterruptor = false;
                    this.Enabled = true;
                    this.Activate();
                }
            }
            else if (sender == button_fastCorrectionsClean)
            {
                dataGridView_task.Rows.Clear();
            }
            else if(sender == button_efnTestRun)
            {
                //Тестовое пробитие чеков чтение сверка и вывод результатов расхождений сверки в лог
                int a = 1;
                bool noExdueTmt = checkBox_testRun_noExDuTm.Checked;
                bool noCorrection = checkBox_efnTestRunNoCorrection.Checked;
                bool noNewNds = checkBox_noNewNds.Checked;
                int sampleBuersLen = SAMPLE_BUYERS.GetUpperBound(0);
                //LogHandle.ol("Запуск самотестирование. Для корректного прогона ККТ должна быть обновлена до прошивок поддерживающих НДС 5,7,105,107 и зарегистрирована с признаком подакциз");
                if (fiscalPrinter != null && fiscalPrinter.IsConnected)
                {
                    int ffd = fiscalPrinter.FfdFtagFormat;
                    int sampleItemsCount = SAMLE_ITEMS.Count();
                    _testRunCheques.Clear();
                    _testRunChequesPerfomed.Clear();
                    _errorsOnTR.Clear();
                    FiscalCheque cheque = null;
                    int lastFdBeforeRun = fiscalPrinter.LastFd;
                    int chequesToRun = 225;
                    int.TryParse(textBox_efnTestRunCheques.Text, out chequesToRun);
                    int sno = 1;
                    double priceInitial = Math.Round(500 + new Random().NextDouble() * 99.0);
                    for (int i = 0; i < chequesToRun; i++)
                    {
                        cheque = new FiscalCheque();

                        sno *= 2;
                        if (sno == 8)
                        {
                            if (i % 7 > 4)
                                sno = 16;
                            else
                                sno = 1;
                        }
                        else if (sno > 32)
                        {
                            sno = 1;
                        }
                        cheque.Sno = sno;
                        cheque.PropertiesData = i.ToString();

                        if (i % 4 == 0)
                        {
                            int bind = i / 4;
                            cheque.BuyerInformationBuyer = SAMPLE_BUYERS[bind % sampleBuersLen, 0];
                            cheque.BuyerInformationBuyerInn = SAMPLE_BUYERS[bind % sampleBuersLen, 1];
                            cheque.BuyerInformationBuyerBirthday = SAMPLE_BUYERS[bind % sampleBuersLen, 2];
                            cheque.BuyerInformationBuyerCitizenship = SAMPLE_BUYERS[bind % sampleBuersLen, 3];
                            cheque.BuyerInformationBuyerDocumentCode = SAMPLE_BUYERS[ bind % sampleBuersLen, 4];
                            cheque.BuyerInformationBuyerDocumentData = SAMPLE_BUYERS[ bind % sampleBuersLen, 5];
                            cheque.BuyerInformationBuyerAddress = SAMPLE_BUYERS[ bind % sampleBuersLen, 6];
                        }

                        double price = Math.Round(priceInitial - i * 1.11);
                        if (price < 0.999)
                            price = 1;
                        double quant = i % 2 + Math.Abs((1 - i % 2) * (1 - i % 3) * 0.1);
                        if (Math.Abs(quant) < 0.0001)
                        {
                            quant = 1;
                        }
                        double sum = Math.Round(price * quant, 2);
                        int productType = (i + 1) % 34;
                        if (productType == 0 || productType == 28 || productType == 29)
                            productType = 1;
                        if (ffd < 4 && productType > 26)
                            productType = 1;
                        if (noExdueTmt && (productType == 2 || productType == 31 || productType == 33))
                        {
                            productType = 1;
                        }

                        int paymentType = (i + 2) % 7;
                        paymentType++;
                        if (paymentType == 3 || paymentType == 7)
                        {
                            paymentType = 4;
                        }
                        int ndsRate = (i + 3) % 10;
                        ndsRate++;
                        if (noNewNds&& ndsRate>6)
                        {
                            ndsRate /= 2; 
                        }

                        var item = new ConsumptionItem(SAMLE_ITEMS[i % sampleItemsCount], price, quant, sum, productType, paymentType, ndsRate);
                        if (ffd == 4)
                        {
                            int unitMeasure = 0;
                            switch (i / 4)
                            {
                                case 1:
                                    unitMeasure = 10;
                                    break;
                                case 2:
                                    unitMeasure = 11;
                                    break;
                                case 3:
                                    unitMeasure = 12;
                                    break;
                                case 4:
                                    unitMeasure = 20;
                                    break;
                                case 5:
                                    unitMeasure = 21;
                                    break;
                                case 6:
                                    unitMeasure = 22;
                                    break;
                                case 7:
                                    unitMeasure = 30;
                                    break;
                                case 8:
                                    unitMeasure = 31;
                                    break;
                                case 9:
                                    unitMeasure = 32;
                                    break;
                                case 10:
                                    unitMeasure = 40;
                                    break;
                                case 11:
                                    unitMeasure = 41;
                                    break;
                                case 12:
                                    unitMeasure = 42;
                                    break;
                                case 13:
                                    unitMeasure = 50;
                                    break;
                                case 14:
                                    unitMeasure = 51;
                                    break;
                                case 15:
                                    unitMeasure = 70;
                                    break;
                                case 16:
                                    unitMeasure = 71;
                                    break;
                                case 17:
                                    unitMeasure = 72;
                                    break;
                                case 18:
                                    unitMeasure = 73;
                                    break;
                                case 19:
                                    unitMeasure = 80;
                                    break;
                                case 20:
                                    unitMeasure = 81;
                                    break;
                                case 21:
                                    unitMeasure = 82;
                                    break;
                                case 22:
                                    unitMeasure = 83;
                                    break;
                                case 23:
                                    unitMeasure = 255;
                                    break;
                                default:
                                    break;

                            }
                            item.Unit120 = unitMeasure;
                        }
                        cheque.AddItem(item);
                        if (i > 60)
                        {
                            price = Math.Round(priceInitial - (i - 20) * 1.11);
                            if (price < 0.999)
                                price = 1;
                            quant = i % 2 + Math.Abs((2 - i % 2) * (2 - i % 3) * 0.3);
                            if (Math.Abs(quant) < 0.001)
                            {
                                quant = 2;
                            }
                            sum = Math.Round(price * quant, 2);
                            productType = (i + 2) % 34;
                            if (productType == 0 || productType == 28 || productType == 29)
                                productType = 1;
                            if (ffd < 4 && productType > 26)
                                productType = 1;
                            if (noExdueTmt && (productType == 2 || productType == 31 || productType == 33))
                            {
                                productType = 1;
                            }
                            paymentType = (i + 3) % 7;
                            paymentType++;
                            if (paymentType == 3 || paymentType == 7)
                            {
                                paymentType = 4;
                            }
                            ndsRate = (i + 4) % 10;
                            ndsRate++;
                            if (noNewNds && ndsRate > 6)
                            {
                                ndsRate /= 2;
                            }

                            item = new ConsumptionItem(SAMLE_ITEMS[(i + a++) % sampleItemsCount], price, quant, sum, productType, paymentType, ndsRate);
                            if (ffd == 4)
                                item.Unit120 = 0;
                            cheque.AddItem(item);
                        }

                        cheque.Control(true);
                        // оплата
                        switch (i / 10)
                        {
                            case 1:
                                cheque.Cash = cheque.TotalSum;
                                break;
                            case 2:
                                cheque.ECash = cheque.TotalSum;
                                break;
                            case 3:
                                cheque.Prepaid = cheque.TotalSum;
                                break;
                            case 4:
                                cheque.Credit = cheque.TotalSum;
                                break;
                            case 5:
                                cheque.Provision = cheque.TotalSum;
                                break;
                            case 6:
                                cheque.Cash = cheque.TotalSum / 2;
                                cheque.ECash = cheque.TotalSum - cheque.Cash;
                                break;
                            case 7:
                                cheque.ECash = cheque.TotalSum / 2;
                                cheque.Credit = cheque.TotalSum - cheque.ECash;
                                break;
                            case 8:
                                cheque.Credit = cheque.TotalSum / 2;
                                cheque.Prepaid = cheque.TotalSum - cheque.Credit;
                                break;
                            case 9:
                                cheque.Provision = cheque.TotalSum / 2;
                                cheque.Prepaid = cheque.TotalSum - cheque.Provision;
                                break;
                            default:
                                cheque.Cash = cheque.TotalSum;
                                break;

                        }
                        if (i > 20)
                        {
                            cheque.CalculationSign = i % 4 + 1;
                        }

                        if (i % 3 == 0 && !noCorrection)
                        {
                            cheque.Document = FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE;
                            cheque.CorrectionDocumentDate = new DateTime(2024, 1, 1).AddDays(i);
                            if ((i / 30) % 2 == 1)
                            {
                                cheque.CorrectionTypeFtag = 0;
                            }
                            else
                            {
                                cheque.CorrectionTypeFtag = 1;
                            }
                            cheque.CorrectionOrderNumber = "Ord " + i;
                            if (ffd == 2)
                            {
                                if (cheque.CalculationSign == 2)
                                {
                                    cheque.CalculationSign = 3;
                                }
                                if (cheque.CalculationSign == 4)
                                {
                                    cheque.CalculationSign = 1;
                                }
                            }
                        }
                        _testRunCheques.Add(cheque);
                    }

                    int fdNumFirst = -1;
                    int x = 0;
                    foreach (var trCheque in _testRunCheques)
                    {
                        x++;
                        if (fiscalPrinter.PerformFD(trCheque))
                        {
                            fiscalPrinter.ReadDeviceCondition();
                            if (fdNumFirst == -1)
                            {
                                fdNumFirst = fiscalPrinter.LastFd;
                            }
                        }
                        else
                        {
                            _errorsOnTR.Add(x);
                        }
                    }
                    int lastFdAfterRun = fiscalPrinter.LastFd;
                    for (int i = lastFdBeforeRun + 1; i <= lastFdAfterRun; i++)
                    {
                        if (i % 15 == 0)
                            LogHandle.ol("read " + i);
                        if (_errorsOnTR.Contains(i))
                        {
                            _testRunChequesPerfomed.Add(FnReadedDocument.EmptyFD);
                            continue;
                        }
                        else
                        {
                            _testRunChequesPerfomed.Add(fiscalPrinter.ReadFD(i, true));
                        }
                    }
                    LogHandle.ol("Сверяем чеки.  Оформлено: " + (lastFdAfterRun - lastFdBeforeRun));

                    FnReadedDocument perfChq;
                    for (int i = 0; i < chequesToRun; i++)
                    {
                        var ogigChq = _testRunCheques[i];
                        perfChq = FnReadedDocument.EmptyFD;
                        string checkPropData = ogigChq.PropertiesData;
                        foreach (var c in _testRunChequesPerfomed)
                        {
                            if (c.Cheque == null)
                            {
                                continue;
                            }
                            if (checkPropData == c.Cheque.PropertiesData)
                            {
                                perfChq = c;
                                break;
                            }
                        }


                        if (perfChq.Cheque == null)
                        {
                            LogHandle.ol("Не найдена пара для чека" + Environment.NewLine + ogigChq.ToString(FiscalCheque.FULL_INFO));
                        }


                        if (false && _errorsOnTR.Contains(i))
                        {
                            LogHandle.ol("Чек не оформлен" + Environment.NewLine + ogigChq.ToString(FiscalCheque.FULL_INFO));
                        }
                        else
                        {
                            if (perfChq.Cheque == null)
                            {
                                LogHandle.ol("Чек не прочитан " + i + "\t" + fdNumFirst);
                                continue;
                            }
                            StringBuilder sb = new StringBuilder();
                            if (ogigChq.Sno != perfChq.Cheque.Sno)
                            {
                                sb.AppendLine("Расходится СНО:\t" + ogigChq.Sno + "\t-\t" + perfChq.Cheque.Sno);
                            }

                            if (ogigChq.CalculationSign != perfChq.Cheque.CalculationSign)
                            {
                                sb.AppendLine("Признак расчета:\t" + ogigChq.CalculationSign + "\t-\t" + perfChq.Cheque.CalculationSign);
                            }
                            if (ogigChq.Document != perfChq.Cheque.Document)
                            {
                                sb.Append("Форма ФД:\t");
                                sb.Append(ogigChq.Document == FD_DOCUMENT_NAME_CHEQUE ? "чек\t-\t" : "ЧК\t-\t");
                                sb.AppendLine(perfChq.Cheque.Document == FD_DOCUMENT_NAME_CHEQUE ? "чек" : "ЧК");
                            }
                            if (ogigChq.Document == FD_DOCUMENT_NAME_CORRECTION_CHEQUE)
                            {
                                if ((ogigChq.CorrectionDocumentDate - perfChq.Cheque.CorrectionDocumentDate).TotalSeconds > 10)
                                {
                                    sb.AppendLine("Отличается дата коррекции:\t" + ogigChq.CorrectionDocumentDate.ToString(DEFAULT_DT_FORMAT) + "\t-\t" + perfChq.Cheque.CorrectionDocumentDate.ToString(DEFAULT_DT_FORMAT));
                                }
                                if (ogigChq.CorrectionTypeFtag != perfChq.Cheque.CorrectionTypeFtag)
                                {
                                    sb.AppendLine("Отличается тип коррекции:\t" + ogigChq.CorrectionTypeFtag + "\t-\t" + perfChq.Cheque.CorrectionTypeFtag);
                                }
                                if (ogigChq.CorrectionTypeNotFtag != perfChq.Cheque.CorrectionTypeNotFtag)
                                {
                                    sb.AppendLine("Отличается Тип коррекции NotFtag:\t" + ogigChq.CorrectionTypeNotFtag + "\t-\t" + perfChq.Cheque.CorrectionTypeNotFtag);
                                }

                                if (ogigChq.CorrectionOrderNumber != perfChq.Cheque.CorrectionOrderNumber)
                                {
                                    if (ogigChq.CorrectionTypeNotFtag != 0)
                                        sb.AppendLine("Отличается номер коррекции:\t" + ogigChq.CorrectionOrderNumber + "\t-\t" + perfChq.Cheque.CorrectionOrderNumber);
                                }
                            }
                            if (ogigChq.Items.Count != perfChq.Cheque.Items.Count)
                            {
                                if (ffd == 2 && ogigChq.DocumentNameFtagType == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE && AppSettings.TFN_SkipItemsInCoorectionFfd2)
                                {
                                    // ФФД 1.05 чеки коррекции
                                }
                                else
                                {
                                    sb.AppendLine("Отличается колчиество ПР\t" + ogigChq.Items.Count + "\t-\t" + perfChq.Cheque.Items.Count);
                                }

                            }
                            if (Math.Abs(ogigChq.TotalSum - perfChq.Cheque.TotalSum) > 0.009)
                            {
                                sb.AppendLine("Расхождение итога");
                            }
                            if (Math.Abs(ogigChq.Cash - perfChq.Cheque.Cash) > 0.009)
                            {
                                sb.AppendLine("Расхождение нал");
                            }
                            if (Math.Abs(ogigChq.ECash - perfChq.Cheque.ECash) > 0.009)
                            {
                                sb.AppendLine("Расхождение БН");
                            }
                            if (Math.Abs(ogigChq.Prepaid - perfChq.Cheque.Prepaid) > 0.009)
                            {
                                sb.AppendLine("Расхождение Prepaid");
                            }
                            if (Math.Abs(ogigChq.Credit - perfChq.Cheque.Credit) > 0.009)
                            {
                                sb.AppendLine("Расхождение Credit");
                            }
                            if (Math.Abs(ogigChq.Provision - perfChq.Cheque.Provision) > 0.009)
                            {
                                sb.AppendLine("Расхождение Provision");
                            }
                            if (Math.Abs(ogigChq.Nds0 - perfChq.Cheque.Nds0) > 0.009)
                            {
                                sb.AppendLine("Расхождение Nds0");
                            }
                            if (Math.Abs(ogigChq.Nds10 - perfChq.Cheque.Nds10) > 0.009)
                            {
                                sb.AppendLine("Расхождение Nds10");
                            }
                            if (Math.Abs(ogigChq.Nds10110 - perfChq.Cheque.Nds10110) > 0.009)
                            {
                                sb.AppendLine("Расхождение Nds10110");
                            }
                            if (Math.Abs(ogigChq.Nds20 - perfChq.Cheque.Nds20) > 0.009)
                            {
                                sb.AppendLine("Расхождение Nds20");
                            }
                            if (Math.Abs(ogigChq.Nds20120 - perfChq.Cheque.Nds20120) > 0.009)
                            {
                                sb.AppendLine("Расхождение 20120");
                            }
                            if (Math.Abs(ogigChq.NdsFree - perfChq.Cheque.NdsFree) > 0.009)
                            {
                                sb.AppendLine("Расхождение nds_free");
                            }
                            if (Math.Abs(ogigChq.Nds5 - perfChq.Cheque.Nds5) > 0.009)
                            {
                                sb.AppendLine("Расхождение nds5");
                            }
                            if (Math.Abs(ogigChq.Nds5105 - perfChq.Cheque.Nds5105) > 0.009)
                            {
                                sb.AppendLine("Расхождение 5105");
                            }
                            if (Math.Abs(ogigChq.Nds7 - perfChq.Cheque.Nds7) > 0.009)
                            {
                                sb.AppendLine("Расхождение nds7");
                            }
                            if (Math.Abs(ogigChq.Nds7107 - perfChq.Cheque.Nds7107) > 0.009)
                            {
                                sb.AppendLine("Расхождение 7107");
                            }
                            if (ogigChq.BuyerInformation||perfChq.Cheque.BuyerInformation)
                            {
                                if(ogigChq.BuyerInformationBuyer != perfChq.Cheque.BuyerInformationBuyer)
                                {
                                    sb.AppendLine("Расходится имя покупателя");
                                }
                                if (ogigChq.BuyerInformationBuyerInn != perfChq.Cheque.BuyerInformationBuyerInn)
                                {
                                    sb.AppendLine("Расходится ИНН покупателя");
                                }
                                if (ogigChq.BuyerInformationBuyerBirthday != perfChq.Cheque.BuyerInformationBuyerBirthday)
                                {
                                    sb.AppendLine("Расходится Birthday покупателя");
                                }
                                if (ogigChq.BuyerInformationBuyerCitizenship != perfChq.Cheque.BuyerInformationBuyerCitizenship)
                                {
                                    sb.AppendLine("Расходится Citizenship покупателя");
                                }
                                if (ogigChq.BuyerInformationBuyerDocumentCode != perfChq.Cheque.BuyerInformationBuyerDocumentCode)
                                {
                                    sb.AppendLine("Расходится DocCode покупателя");
                                }
                                if (ogigChq.BuyerInformationBuyerDocumentData != perfChq.Cheque.BuyerInformationBuyerDocumentData)
                                {
                                    sb.AppendLine("Расходится DocData покупателя");
                                }
                                if (ogigChq.BuyerInformationBuyerAddress != perfChq.Cheque.BuyerInformationBuyerAddress)
                                {
                                    sb.AppendLine("Расходится адрес покупателя");
                                }
                            }
                            
                            for (int k = 0; k < ogigChq.Items.Count && k < perfChq.Cheque.Items.Count; k++)
                            {
                                if (ogigChq.Items[k].Name != perfChq.Cheque.Items[k].Name)
                                {
                                    sb.AppendLine("Расхождение пр.наим " + k);
                                }
                                if (ogigChq.Items[k].NdsRate != perfChq.Cheque.Items[k].NdsRate)
                                {
                                    sb.AppendLine("items.NDS RATE(" + k + ") " + ogigChq.Items[k].NdsRate + "\t-\t" + perfChq.Cheque.Items[k].NdsRate);
                                }
                                if (ogigChq.Items[k].ProductType != perfChq.Cheque.Items[k].ProductType)
                                {
                                    sb.AppendLine("items.ProductType(" + k + ") " + ogigChq.Items[k].ProductType + "\t-\t" + perfChq.Cheque.Items[k].ProductType);
                                }
                                if (ogigChq.Items[k].PaymentType != perfChq.Cheque.Items[k].PaymentType)
                                {
                                    sb.AppendLine("items.PaymentType(" + k + ") " + ogigChq.Items[k].PaymentType + "\t-\t" + perfChq.Cheque.Items[k].PaymentType);
                                }
                                if (ffd == 4 && ogigChq.Items[k].Unit120 != perfChq.Cheque.Items[k].Unit120)
                                {
                                    sb.AppendLine("items.Unit120(" + k + ") " + ogigChq.Items[k].Unit120 + "\t-\t" + perfChq.Cheque.Items[k].Unit120);
                                }
                                if (ffd < 4 && ogigChq.Items[k].Unit105 != null && perfChq.Cheque.Items[k].Unit105 != null && ogigChq.Items[k].Unit105 != perfChq.Cheque.Items[k].Unit105)
                                {
                                    sb.AppendLine("items.Unit105(" + k + ") " + ogigChq.Items[k].Unit105 + "\t-\t" + perfChq.Cheque.Items[k].Unit105);
                                }
                                if (Math.Abs(ogigChq.Items[k].Quantity - perfChq.Cheque.Items[k].Quantity) > 0.009)
                                {
                                    sb.AppendLine("items.Quantity(" + k + ") " + ogigChq.Items[k].Quantity + "\t-\t" + perfChq.Cheque.Items[k].Quantity);
                                }
                                if (Math.Abs(ogigChq.Items[k].Sum - perfChq.Cheque.Items[k].Sum) > 0.009)
                                {
                                    sb.AppendLine("items.Sum(" + k + ") " + ogigChq.Items[k].Sum + "\t-\t" + perfChq.Cheque.Items[k].Sum);
                                }

                            }

                            if (sb.Length > 0)
                            {
                                LogHandle.ol("Расхожденеие ! ! ! ! ! !" + Environment.NewLine +
                                    "Оригинал:" +
                                    ogigChq.ToString(FiscalCheque.FULL_INFO) +
                                    Environment.NewLine + Environment.NewLine +
                                    perfChq.Cheque.ToString(FiscalCheque.FULL_INFO) + Environment.NewLine +
                                    sb.ToString());

                            }
                        }

                    }
                    if (_errorsOnTR.Count > 0)
                    {
                        StringBuilder s = new StringBuilder("Ошибки оформления: ");
                        foreach (var t in _errorsOnTR)
                            s.Append(t + ", ");
                        LogHandle.ol(s.ToString());
                    }


                }
                else
                {
                    LogHandle.ol(CONNECTION_NOT_ESTABLISHED);
                }

            }
            else if(sender == button_testAction)
            {
                // простое заполнение МГМ чеками 
                if (!conn_ckb_connected.Checked)
                {
                    PushMessage(CONNECTION_NOT_ESTABLISHED);
                    return;
                }
                int names = SAMLE_ITEMS.Length;
                int sno = fiscalPrinter.ChosenSno;
                List<FiscalCheque> cheques = new List<FiscalCheque>();
                Random random = new Random();
                int k;
                int l = 0;
                for (int i = 0; i < 10; i++)
                {
                    FiscalCheque fc = new FiscalCheque();
                    fc.Sno = sno;
                    k = random.Next(0, 11) + 1;
                    for (int j = 0; j < k; j++)
                    {
                        ConsumptionItem item = new ConsumptionItem();
                        item.Name = SAMLE_ITEMS[l++ % names];
                        item.Price = i + 100;

                        item.Quantity = 1;

                        item.Sum = item.Price * item.Quantity;

                        item.NdsRate = i % 2 == 0 ? NDS_TYPE_FREE_LOC : NDS_TYPE_20_LOC;
                        item.PaymentType = FD_ITEM_PAYMENT_TOTAL_CALC_LOC;
                        item.ProductType = i % 5 + 1;
                        item.Control();
                        if (fiscalPrinter.FfdVer >= 2)
                        {
                            int itemUnit = 0;
                            int.TryParse(textBox_cheqItemMeasure105.Text, out itemUnit);
                            item.Unit120 = itemUnit;
                        }
                        fc.AddItem(item);

                    }

                    fc.Control(true);
                    if (i % 2 == 0) fc.Cash = fc.TotalSum;
                    else fc.ECash = fc.TotalSum;

                    cheques.Add(fc);
                }
                for (int i = 0; i < 6; i++)
                {
                    foreach (FiscalCheque fc in cheques)
                        fiscalPrinter.PerformFD(fc);
                    fiscalPrinter.CloseShift();
                    if (checkBox_changeDate.Checked) fiscalPrinter.ChangeDate(1);
                }
            }
            else if (sender == button_testTerminalFnLib)
            {
                TerminalFnExchange tfn = new TerminalFnExchange();
                tfn.ShowSettings();
            }
            else if(sender == button_IEE_FOD)
            {
                FormItemExtendedEditor fiee = new FormItemExtendedEditor();
                fiee.ShowDialog();
            }
        }
        private void DataGridView_task_CellClick(object sender, EventArgs e)
        {
            dataGridView_task.BeginEdit(true);
        }
        private void DataGridView_task_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView_task_CellClick(sender,null);
        }

        private void ItemExtendedEditor(object sender, EventArgs e)
        {
            //ReadFormFiscalDocument();
            List<ConsumptionItem> l = new List<ConsumptionItem>();
            l.AddRange(fFDoc.Items);
            FormItemExtendedEditor itemsEditor = new FormItemExtendedEditor(l);
            itemsEditor.ShowDialog();

            fFDoc.Items.Clear();
            fFDoc.Items.AddRange(itemsEditor.CheqItems);
            RecreateItems();
            fFDoc.Control(true);
            FormFiscalDocChanged(true);
        }

        private void CheckCell(DataGridViewCell c)
        {
            if(c==null) return;
            if (c.Value == null && (c.ColumnIndex == 1 || c.ColumnIndex == 2)) c.Value = "0";

            if (c.ColumnIndex == 1 || c.ColumnIndex == 2)
            {
                double d = -1;
                LogHandle.ol("checking summs");
                try
                {
                    d = double.Parse(ReplaceBadDecimalSeparatorPoint(c.Value.ToString()));
                    if (d >= 0)
                    {
                        c.Style.ForeColor = Color.Black;
                        string s2 = "";
                        if (c.ColumnIndex == 1) s2 = dataGridView_task.Rows[c.RowIndex].Cells[2].Value == null ? "" : dataGridView_task.Rows[c.RowIndex].Cells[2].Value.ToString();
                        else s2 = dataGridView_task.Rows[c.RowIndex].Cells[1].Value == null ? "" : dataGridView_task.Rows[c.RowIndex].Cells[1].Value.ToString();
                        double d2 = string.IsNullOrEmpty(s2) ? 0 : double.Parse(ReplaceBadDecimalSeparatorPoint(s2));
                        if (d2 >= 0) dataGridView_task.Rows[c.RowIndex].Cells[0].Value = Math.Round(d2 + d, 2).ToString();
                        CalculateGrid();
                    }

                }
                catch { }
                if(d<0){
                    c.Style.ForeColor = Color.Red;
                    dataGridView_task.Rows[c.RowIndex].Cells[0].Value = BAD_SUMMS_ROW;
                }

            }
            else if(c.ColumnIndex == 3)
            {
                
                if (c.Value != null)
                {
                    string s = c.Value.ToString();

                    DateTime dt = DateTime.MinValue;
                    bool needCorrection = false;
                    try
                    {
                        dt = DateTime.ParseExact(s, DATE_FORMAT_C1, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch 
                    {
                        try
                        {
                            dt = DateTime.ParseExact(s, DATE_FORMAT_C2, System.Globalization.CultureInfo.InvariantCulture);
                            needCorrection = true;
                        }
                        catch 
                        {
                            try
                            {
                                dt = DateTime.ParseExact(s, DATE_FORMAT_C3, System.Globalization.CultureInfo.InvariantCulture);
                                needCorrection = true;
                            }
                            catch 
                            {
                                try
                                {
                                    dt = DateTime.ParseExact(s, DATE_FORMAT_C4, System.Globalization.CultureInfo.InvariantCulture);
                                    needCorrection = true;
                                }
                                catch { }
                            }
                            
                        }
                        
                    }
                    

                    if (dt > MIN_CORRECTION_DATE)
                    {
                        c.Style.ForeColor = Color.Black;
                        if (needCorrection)
                        {
                            c.Value = dt.ToString(DATE_FORMAT_C1);
                        }
                    }
                    else
                    {
                        c.Style.ForeColor = Color.Red;
                        dataGridView_task.Rows[c.RowIndex].Cells[0].Value = BAD_SUMMS_ROW;
                    }
                }
            }

        }
        static readonly string DATE_FORMAT_C1 = "dd.MM.yyyy",
            DATE_FORMAT_C2 = "d.MM.yyyy",
            DATE_FORMAT_C3 = "dd.MM.yy",
            DATE_FORMAT_C4 = "d.MM.yy",
            BAD_SUMMS_ROW = "#ERROR",
            NO_DATE = "#NO DATE",
            LOW_DATE = "#LOW DATE",
            EMPTY = "#VOID";
        private static readonly DateTime MIN_CORRECTION_DATE = DateTime.ParseExact("01.03.2017", DATE_FORMAT_C1, System.Globalization.CultureInfo.InvariantCulture);


        private void DataGridView_task_CellEndEdit(object sender, EventArgs e)
        {
            if (dataGridView_task.Rows[0].Cells[0].Style.BackColor != Color.White)
                foreach (DataGridViewRow row in dataGridView_task.Rows)
                {
                    row.Cells[0].Style.BackColor = Color.White;
                }
            CalculateGrid();
        }
        
        private void CalculateGrid()
        {
            double totalSum = 0,totalCash = 0,totalEcash = 0;
            int documents = 0;
            if(comboBox_fasstCorrectionsAutoDate.SelectedIndex != 3 
                && dataGridView_task.Rows.Count > 1 
                && dataGridView_task.Rows[dataGridView_task.Rows.Count - 1].Cells[3].Value==null 
                && dataGridView_task.Rows[dataGridView_task.Rows.Count - 2].Cells[3].Value != null)
            {
                DateTime d = DateTime.MinValue;
                try { d = DateTime.ParseExact(dataGridView_task.Rows[dataGridView_task.Rows.Count - 2].Cells[3].Value.ToString(), DATE_FORMAT_C1, System.Globalization.CultureInfo.InvariantCulture); } catch { };
                if(d > MIN_CORRECTION_DATE)
                {
                    if (comboBox_fasstCorrectionsAutoDate.SelectedIndex == 1) d = d.AddDays(1);
                    else if (comboBox_fasstCorrectionsAutoDate.SelectedIndex == 2) d = new DateTime(d.Year, d.Month, 1).AddMonths(2).AddDays(-1);
                    dataGridView_task.Rows[dataGridView_task.Rows.Count - 1].Cells[3].Value = d.ToString(DATE_FORMAT_C1);
                }
            }

            foreach(DataGridViewRow row in dataGridView_task.Rows)
            {
                //if(row.Cells[0].Value != null || row.Cells[0].Value.ToString().StartsWith("#")) continue;
                if (row.Cells[0].Value == null) row.Cells[0].Value = "0";
                if (row.Cells[1].Value == null) row.Cells[1].Value = "0";
                if (row.Cells[2].Value == null) row.Cells[2].Value = "0";

                string sTally = row.Cells[0].Value.ToString();
                string sCash =  ReplaceBadDecimalSeparatorPoint(row.Cells[1].Value.ToString());
                string sECash = ReplaceBadDecimalSeparatorPoint(row.Cells[2].Value.ToString());
                string sDate = row.Cells[3].Value == null ? "" : row.Cells[3].Value.ToString();
                double tally = double.MinValue, cash = double.MinValue, ecash = double.MinValue ;
                DateTime date = DateTime.MinValue;

                try { tally = double.Parse(sTally); }catch { }
                try { cash = double.Parse(sCash); }catch { }
                try { ecash = double.Parse(sECash); }catch { }
                bool needCorrectionDate = false;

                sDate = CorrectDateFormat(sDate, ref needCorrectionDate);
                try
                {
                    date = DateTime.ParseExact(sDate, DATE_FORMAT_C1, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch
                {
                    try
                    {
                        date = DateTime.ParseExact(sDate, DATE_FORMAT_C2, System.Globalization.CultureInfo.InvariantCulture);
                        needCorrectionDate = true;
                    }
                    catch
                    {
                        try
                        {
                            date = DateTime.ParseExact(sDate, DATE_FORMAT_C3, System.Globalization.CultureInfo.InvariantCulture);
                            needCorrectionDate = true;
                        }
                        catch
                        {
                            try
                            {
                                date = DateTime.ParseExact(sDate, DATE_FORMAT_C4, System.Globalization.CultureInfo.InvariantCulture);
                                needCorrectionDate = true;
                            }
                            catch { }
                        }
                    }
                }

                bool goodDate = date > MIN_CORRECTION_DATE;
                bool goodSums = false;
                if (needCorrectionDate&&goodDate) row.Cells[3].Value = date.ToString(DATE_FORMAT_C1);
                if (cash >= 0 && ecash >= 0)
                {
                    if (Math.Abs(tally - cash - ecash) > 0.01499999)
                    {
                        tally = Math.Round(cash + ecash,2);
                    }
                    row.Cells[1].Style.ForeColor = Color.Black;
                    row.Cells[2].Style.ForeColor = Color.Black;
                    goodSums = true;
                }
                else
                {
                    if(cash < 0)
                    {
                        row.Cells[0].Style.ForeColor = Color.Red;
                        row.Cells[1].Style.ForeColor = Color.Red;
                    }
                    if (ecash < 0)
                    {
                        row.Cells[0].Style.ForeColor = Color.Red;
                        row.Cells[2].Style.ForeColor = Color.Red;
                    }
                }
                if(goodSums && goodDate)
                {
                    if(tally > 0)
                    {
                        row.Cells[0].Style.ForeColor = Color.Black;
                        row.Cells[0].Value = tally.ToString();
                        totalSum += tally;
                        totalCash += cash;
                        totalEcash += ecash;
                        documents++;
                    }
                    else
                    {
                        row.Cells[0].Style.ForeColor = Color.Red;
                        row.Cells[0].Value = EMPTY;
                    }
                    
                }
                else
                {
                    if (!goodSums)
                    {
                        row.Cells[0].Style.ForeColor = Color.Red;
                        row.Cells[0].Value = BAD_SUMMS_ROW;
                    }
                    else
                    {
                        row.Cells[0].Style.ForeColor = Color.Red;
                        if (date > DateTime.MinValue && date < MIN_CORRECTION_DATE) row.Cells[0].Value = LOW_DATE;
                        else row.Cells[0].Value = NO_DATE;
                        
                    }
                }
            }
            textBox_fastCorrectionsTotalSumm.Text = Math.Round(totalSum,2).ToString();
            textBox_fastCorrectionsCash.Text = Math.Round(totalCash,2).ToString();
            textBox_fastCorrectionsECash.Text = Math.Round(totalEcash,2).ToString();
            double tax = 0;
            if (comboBox_fastCorrectionsNds.SelectedIndex == 0|| comboBox_fastCorrectionsNds.SelectedIndex == 2)
            {
                tax = totalSum / 6;
            }
            else if (comboBox_fastCorrectionsNds.SelectedIndex ==1|| comboBox_fastCorrectionsNds.SelectedIndex == 3)
            {
                tax = totalSum / 11;
            }
            else if (comboBox_fastCorrectionsNds.SelectedIndex == 6 || comboBox_fastCorrectionsNds.SelectedIndex == 8)
            {
                tax = totalSum / 21;
            }
            else if (comboBox_fastCorrectionsNds.SelectedIndex == 7 || comboBox_fastCorrectionsNds.SelectedIndex == 9)
            {
                tax = totalSum * 7.0/ 107.0;
            }
            textBox_fastCorrectionsNds.Text = Math.Round(tax, 2).ToString();
            textBox_fastCorretionDocumentCounter.Text = documents.ToString();
        }

        private static string CorrectDateFormat(string s, ref bool correctDate)
        {
            if (s.Contains('/'))
            {
                correctDate = true;
                s = s.Replace('/', '.');
            }
            if(s.Contains(','))
            {
                correctDate = true;
                s = s.Replace(',', '.');
            }
            if (s.Contains('\\'))
            {
                correctDate = true;
                s = s.Replace('\\', '.');
            }
            return s;
        }

        public static string ChequesCount(List<FnReadedDocument> docs)
        {
            double tallies = 0, taxesAll = 0, noNDS = 0, cashSumm = 0, ecashSum = 0, 
                finRezultTotal = 0, finRezultCash = 0, finRezultEcash = 0, finRezultNds = 0,
                totalIncome = 0, incomeCash = 0, incomeEcash = 0, incomeNds = 0,
                totalBackIncome = 0, backIncomeCash = 0, backIncomeEcash = 0, backIncomeNds = 0,
                totalExpand = 0, expandCash = 0, expandEcash = 0, expandNds = 0,
                totalBackExpand = 0, backExpandCash = 0, backExpandEcash = 0, backExpandNds = 0;
            int cheques = 0;
            foreach (var doc in docs)
            {
                if (doc.Cheque != null)
                {
                    tallies += doc.Cheque.TotalSum;
                    taxesAll += doc.Cheque.Nds10 +
                        doc.Cheque.Nds20 + 
                        doc.Cheque.Nds10110 + 
                        doc.Cheque.Nds20120 + 
                        doc.Cheque.Nds5 + 
                        doc.Cheque.Nds7 + 
                        doc.Cheque.Nds5105 + 
                        doc.Cheque.Nds7107;
                    cashSumm += doc.Cheque.Cash;
                    ecashSum += doc.Cheque.ECash;
                    noNDS += doc.Cheque.NdsFree;
                    cheques++;
                    if(doc.Cheque.CalculationSign == 1)
                    {
                        totalIncome += doc.Cheque.TotalSum;
                        incomeCash += doc.Cheque.Cash;
                        incomeEcash += doc.Cheque.ECash;
                        incomeNds += doc.Cheque.Nds20 + 
                            doc.Cheque.Nds10 + 
                            doc.Cheque.Nds20120 + 
                            doc.Cheque.Nds10110 +
                            doc.Cheque.Nds5 +
                            doc.Cheque.Nds7 +
                            doc.Cheque.Nds5105 +
                            doc.Cheque.Nds7107;
                    }
                    else if (doc.Cheque.CalculationSign == 2)
                    {
                        totalBackIncome += doc.Cheque.TotalSum;
                        backIncomeCash += doc.Cheque.Cash;
                        backIncomeEcash += doc.Cheque.ECash;
                        backIncomeNds += doc.Cheque.Nds20 + 
                            doc.Cheque.Nds10 + 
                            doc.Cheque.Nds20120 + 
                            doc.Cheque.Nds10110 +
                            doc.Cheque.Nds5 +
                            doc.Cheque.Nds7 +
                            doc.Cheque.Nds5105 +
                            doc.Cheque.Nds7107;
                    }
                    else if (doc.Cheque.CalculationSign == 3)
                    {
                        totalExpand += doc.Cheque.TotalSum;
                        expandCash += doc.Cheque.Cash;
                        expandEcash += doc.Cheque.ECash;
                        expandNds += doc.Cheque.Nds20 + 
                            doc.Cheque.Nds10 + 
                            doc.Cheque.Nds20120 + 
                            doc.Cheque.Nds10110 +
                            doc.Cheque.Nds5 +
                            doc.Cheque.Nds7 +
                            doc.Cheque.Nds5105 +
                            doc.Cheque.Nds7107;
                    }
                    else if(doc.Cheque.CalculationSign == 4)
                    {
                        totalBackExpand += doc.Cheque.TotalSum;
                        backExpandCash += doc.Cheque.Cash;
                        backExpandEcash += doc.Cheque.ECash;
                        backExpandNds += doc.Cheque.Nds20 + 
                            doc.Cheque.Nds10 + 
                            doc.Cheque.Nds20120 + 
                            doc.Cheque.Nds10110 +
                            doc.Cheque.Nds5 +
                            doc.Cheque.Nds7 +
                            doc.Cheque.Nds5105 +
                            doc.Cheque.Nds7107;
                    }
                }
            }
            finRezultTotal = totalIncome - totalBackIncome - totalExpand + totalBackExpand;
            finRezultCash = incomeCash - backIncomeCash - expandCash + backExpandCash;
            finRezultEcash = incomeEcash - backIncomeEcash - expandEcash + backExpandEcash;
            finRezultNds = incomeNds - backIncomeNds - expandNds + backExpandNds;
            StringBuilder sb = new StringBuilder();
            if (cheques > 0)
            {
                sb.Append("Документов:");
                sb.Append(cheques);
                sb.Append(Environment.NewLine);
                sb.Append("Все признаки расчета");
                sb.Append(Environment.NewLine);
                sb.Append("итог:");
                sb.Append(Math.Round(tallies, 2));
                sb.Append("; НДС:");
                sb.Append(Math.Round(taxesAll, 2));
                sb.Append("; НАЛ:");
                sb.Append(Math.Round(cashSumm, 2));
                sb.Append("; БН:");
                sb.Append(Math.Round(ecashSum, 2));

                if (totalIncome > 0.0099)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append("Приход:");
                    sb.Append(Environment.NewLine);
                    sb.Append("Итог ");
                    sb.Append(Math.Round(totalIncome, 2));
                    sb.Append("; НАЛ ");
                    sb.Append(Math.Round(incomeCash, 2));
                    sb.Append("; БН ");
                    sb.Append(Math.Round(incomeEcash, 2));
                    sb.Append("; НДС ");
                    sb.Append(Math.Round(incomeNds, 2));
                }
                if (totalBackIncome>0.0099)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append("Возврат прихода:");
                    sb.Append(Environment.NewLine);
                    sb.Append("Итог ");
                    sb.Append(Math.Round(totalBackIncome, 2));
                    sb.Append("; НАЛ ");
                    sb.Append(Math.Round(backIncomeCash, 2));
                    sb.Append("; БН ");
                    sb.Append(Math.Round(backIncomeEcash, 2));
                    sb.Append("; НДС ");
                    sb.Append(Math.Round(backIncomeNds, 2));
                }
                if (totalExpand > 0.0099)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append("Расход:");
                    sb.Append(Environment.NewLine);
                    sb.Append("Итог ");
                    sb.Append(Math.Round(totalExpand, 2));
                    sb.Append("; НАЛ ");
                    sb.Append(Math.Round(expandCash, 2));
                    sb.Append("; БН ");
                    sb.Append(Math.Round(expandEcash, 2));
                    sb.Append("; НДС ");
                    sb.Append(Math.Round(expandNds, 2));
                }
                if (totalBackExpand > 0.0099)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append("Возврат расхода:");
                    sb.Append(Environment.NewLine);
                    sb.Append("Итог ");
                    sb.Append(Math.Round(totalBackExpand, 2));
                    sb.Append("; НАЛ ");
                    sb.Append(Math.Round(backExpandCash, 2));
                    sb.Append("; БН ");
                    sb.Append(Math.Round(backExpandEcash, 2));
                    sb.Append("; НДС ");
                    sb.Append(Math.Round(backExpandNds, 2));
                }

                finRezultTotal = Math.Round(finRezultTotal, 2);
                finRezultCash = Math.Round(finRezultCash, 2);
                finRezultEcash = Math.Round(finRezultEcash, 2);
                finRezultNds = Math.Round(finRezultNds, 2);

                if ( Math.Abs(finRezultTotal) + Math.Abs(finRezultCash) + Math.Abs(finRezultEcash) + Math.Abs(finRezultNds) > 0.0099)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append("Финансовый результат: ");
                    sb.Append(Environment.NewLine);
                    if (finRezultTotal > 0.0099)
                    {
                        sb.Append("Итог увеличение на ");
                        sb.Append(finRezultTotal);
                    }
                    else if(finRezultTotal < -0.0099)
                    {
                        sb.Append("Итог уменьшение на ");
                        sb.Append(-1*finRezultTotal);
                    }
                    else if(finRezultTotal < 0.009 && finRezultTotal > -0.009)
                    {
                        sb.Append("Итог операций 0");
                    }

                    sb.Append(Environment.NewLine);
                    if (finRezultCash > 0.0099)
                    {
                        sb.Append("Выручка наличными увеличение на ");
                        sb.Append(finRezultCash);
                    }
                    else if (finRezultCash < -0.0099)
                    {
                        sb.Append("Выручка наличными уменьшение на ");
                        sb.Append(-1 * finRezultCash);
                    }
                    else if (finRezultCash < 0.009 && finRezultCash > -0.009)
                    {
                        sb.Append("Выручка наличными 0");
                    }

                    sb.Append(Environment.NewLine);
                    if (finRezultEcash > 0.0099)
                    {
                        sb.Append("Выручка безналичными увеличение на ");
                        sb.Append(finRezultEcash);
                    }
                    else if (finRezultEcash < -0.0099)
                    {
                        sb.Append("Выручка безналичными уменьшение на ");
                        sb.Append(-1 * finRezultEcash);
                    }
                    else if (finRezultEcash < 0.009 && finRezultEcash > -0.009)
                    {
                        sb.Append("Выручка безналичными 0");
                    }

                    sb.Append(Environment.NewLine);
                    if (finRezultNds > 0.0099)
                    {
                        sb.Append("НДС увеличение на ");
                        sb.Append(finRezultNds);
                    }
                    else if (finRezultNds < -0.0099)
                    {
                        sb.Append("НДС уменьшение на ");
                        sb.Append(-1 * finRezultNds);
                    }
                    else if (finRezultNds < 0.009 && finRezultNds > -0.009)
                    {
                        sb.Append("НДС 0");
                    }
                }
            }
            return sb.ToString();
        }

        private int _filterPayment1, 
            _filterPayment2, 
            _filterNdsType1, 
            _filterNdsType2, 
            _filterSno, 
            _filterDocumentType, 
            _filterOperationType; 
        private void ReadFilters()
        {
            _filterPayment1 = 0; _filterPayment2 = 0; _filterNdsType1 = 0; _filterNdsType2 = 0; _filterSno = 0; _filterDocumentType = 0; _filterOperationType = 0;
            if (checkBox_task2FilterPaidMethod1.Checked) _filterPayment1 = comboBox_task2FilterPaid1.SelectedIndex == -1 ? 0 : comboBox_task2FilterPaid1.SelectedIndex;
            if(checkBox_task2FilterPaidMethod2.Checked) _filterPayment2 = comboBox_task2FilterPaid2.SelectedIndex == -1 ? 0 : comboBox_task2FilterPaid2.SelectedIndex;
            if (checkBox_task2NdsType1.Checked) _filterNdsType1 = comboBox_task2NdsType1.SelectedIndex == -1 ? 0 : comboBox_task2NdsType1.SelectedIndex;
            if (checkBox_task2NdsType2.Checked) _filterNdsType2 = comboBox_task2NdsType2.SelectedIndex == -1 ? 0 : comboBox_task2NdsType2.SelectedIndex;
            if (checkBox_task2FilterSno.Checked)
            {
                int sno = comboBox_task2Sno.SelectedIndex == -1 ? 0 : comboBox_task2Sno.SelectedIndex;
                if(sno== 3)
                    _filterSno = 4;
                else if(sno == 4)
                    _filterSno = 16;
                else if(sno == 5)
                    _filterSno= 32;
                else _filterSno = sno;
            }
            if (checkBox_task2FilterOnlyCheque.Checked) _filterDocumentType = 3;
            if(checkBox_task2FilterCalculationSign.Checked) _filterOperationType = comboBox_task2OperationType.SelectedIndex == -1 ? 0 : comboBox_task2OperationType.SelectedIndex;
        }
        private bool FiltrateDocument(FnReadedDocument fd)
        {
            if (fd.Cheque == null)return false;

            if(_filterDocumentType == 3 && fd.Cheque.Document == FD_DOCUMENT_NAME_CORRECTION_CHEQUE) return false; // пока фильтруется только на кассовые чеки

            if(_filterOperationType!=0 && _filterOperationType != fd.Cheque.CalculationSign) return false;

            if(_filterSno!=0 && _filterSno!=fd.Cheque.Sno) return false;

            if (_filterPayment1 + _filterPayment2 > 0)
            {
                bool paymentCondition = false;
                if ((_filterPayment1 == 1 || _filterPayment2 == 1) && fd.Cheque.Cash>0.0099) paymentCondition = true;
                if(!paymentCondition && (_filterPayment1 == 2 || _filterPayment2 == 2) && fd.Cheque.ECash > 0.0099 ) paymentCondition = true;
                if(!paymentCondition && (_filterPayment1 == 3 || _filterPayment2 == 3) && fd.Cheque.Prepaid > 0.0099 ) paymentCondition = true;
                if(!paymentCondition && (_filterPayment1 == 4 || _filterPayment2 == 4) && fd.Cheque.Credit > 0.0099 ) paymentCondition = true;
                if(!paymentCondition && (_filterPayment1 == 5 || _filterPayment2 == 5) && fd.Cheque.Provision > 0.0099 ) paymentCondition = true;
                if(!paymentCondition) return false;
            }

            if( _filterNdsType1 + _filterNdsType2 > 0 )
            {
                bool ndsCondition = false;
                if( _filterNdsType1 == NDS_TYPE_20_LOC || _filterNdsType2 == NDS_TYPE_20_LOC)
                {
                    if(fd.Cheque.Nds20>0.0099) ndsCondition = true;
                    if (!ndsCondition)
                    {
                        foreach (var i in fd.Cheque.Items)
                        {
                            if (i.NdsRate == NDS_TYPE_20_LOC)
                            {
                                ndsCondition = true;
                                break;
                            }
                        }
                    }
                }
                if(!ndsCondition && _filterNdsType1 == NDS_TYPE_10_LOC || _filterNdsType2 == NDS_TYPE_10_LOC)
                {
                    if (fd.Cheque.Nds10 > 0.0099) ndsCondition = true;
                    if (!ndsCondition)
                    {
                        foreach (var i in fd.Cheque.Items)
                        {
                            if (i.NdsRate == NDS_TYPE_10_LOC)
                            {
                                ndsCondition = true;
                                break;
                            }
                        }
                    }
                }
                if(!ndsCondition && _filterNdsType1 == NDS_TYPE_20120_LOC || _filterNdsType2 == NDS_TYPE_20120_LOC)
                {
                    if (fd.Cheque.Nds20120 > 0.0099) ndsCondition = true;
                    if (!ndsCondition)
                    {
                        foreach (var i in fd.Cheque.Items)
                        {
                            if (i.NdsRate == NDS_TYPE_20120_LOC)
                            {
                                ndsCondition = true;
                                break;
                            }
                        }
                    }
                }
                if(!ndsCondition && _filterNdsType1 == NDS_TYPE_10110_LOC || _filterNdsType2 == NDS_TYPE_10110_LOC)
                {
                    if (fd.Cheque.Nds10110 > 0.0099) ndsCondition = true;
                    if (!ndsCondition)
                    {
                        foreach (var i in fd.Cheque.Items)
                        {
                            if (i.NdsRate == NDS_TYPE_10110_LOC)
                            {
                                ndsCondition = true;
                                break;
                            }
                        }
                    }
                }
                if(!ndsCondition && _filterNdsType1 == NDS_TYPE_0_LOC || _filterNdsType2 == NDS_TYPE_0_LOC)
                {
                    if (fd.Cheque.Nds0 > 0.0099) ndsCondition = true;
                    if (!ndsCondition)
                    {
                        foreach (var i in fd.Cheque.Items)
                        {
                            if (i.NdsRate == NDS_TYPE_0_LOC)
                            {
                                ndsCondition = true;
                                break;
                            }
                        }
                    }
                }
                if(!ndsCondition && _filterNdsType1 == NDS_TYPE_FREE_LOC || _filterNdsType2 == NDS_TYPE_FREE_LOC)
                {
                    if (fd.Cheque.NdsFree > 0.0099) ndsCondition = true;
                    if (!ndsCondition)
                    {
                        foreach (var i in fd.Cheque.Items)
                        {
                            if (i.NdsRate == NDS_TYPE_FREE_LOC)
                            {
                                ndsCondition = true;
                                break;
                            }
                        }
                    }
                }
                if (!ndsCondition && _filterNdsType1 == 7||_filterNdsType2 == 7)
                {
                    if(fd.Cheque.Nds20+fd.Cheque.Nds10+fd.Cheque.Nds20120+fd.Cheque.Nds10110>0.0099) ndsCondition =true;
                    if (!ndsCondition)
                    {
                        foreach(var i in fd.Cheque.Items)
                        {
                            if (i.NdsRate != NDS_TYPE_0_LOC && i.NdsRate != NDS_TYPE_FREE_LOC)
                            {
                                ndsCondition = true;
                                break;
                            }
                        }
                    }
                }   // тут ставки НДС 20, НДС 10, 20120, 10110 
                // возможно стоит добавить условие все кроме без НДС
                if(!ndsCondition)return false;
            }
            return true;
        }

        public bool DontPrintFlag
        {
            get => checkBox_dontPrint.Checked;
            set => checkBox_dontPrint.Checked = value;
        }

        public int ChosenSno
        {
            get
            {
                return comboBox_cheq_sno.SelectedIndex;
            }
        }

        private void AutoPayBoxesSet()
        {
            if (AppSettings.CoPayInterfaceDoc == AppSettings.CoPayMethods.CO_PAY_OFF)
            {
                checkBox_autoPayCash.Checked = false;
                checkBox_autoPayECash.Checked = false;
                checkBox_autoPayPrepay.Checked = false;
                checkBox_autoPayCredit.Checked = false;
                checkBox_autoPayProvision.Checked = false;
            }
            else if (AppSettings.CoPayInterfaceDoc == AppSettings.CoPayMethods.CO_PAY_CASH)
            {
                checkBox_autoPayCash.Checked = true;
            } 
            else if (AppSettings.CoPayInterfaceDoc == AppSettings.CoPayMethods.CO_PAY_ECASH)
            {
                checkBox_autoPayECash.Checked = true;
            } 
            else if (AppSettings.CoPayInterfaceDoc == AppSettings.CoPayMethods.CO_PAY_PREPAID)
            {
                checkBox_autoPayPrepay.Checked = true;
            }
            else if (AppSettings.CoPayInterfaceDoc == AppSettings.CoPayMethods.CO_PAY_CREDIT)
            {
                checkBox_autoPayCredit.Checked = true;
            }
            else if (AppSettings.CoPayInterfaceDoc == AppSettings.CoPayMethods.CO_PAY_PROVISION)
            {
                checkBox_autoPayProvision.Checked = true;
            }
                
        }


        List<FiscalCheque> _testRunCheques = new List<FiscalCheque>();
        List<FnReadedDocument> _testRunChequesPerfomed = new List<FnReadedDocument>();
        List<int> _errorsOnTR = new List<int>();
    }
}
