using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using static FR_Operator.TerminalFnExchange;
using static FR_Operator.FiscalPrinter;
using System.Threading;
using System.Timers;
using FR_Operator.Properties;
using System.Reflection;

namespace FR_Operator
{
    public partial class TerminalUi : Form
    {
        internal TerminalUi(TerminalFnExchange lib)
        {
            this.lib = lib;

            _skipSettingsConEvent = true;
            InitializeComponent();

            string version = System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            _title = Text+ " " + version;
            Text = _title/*" " + version /*+ " от: " + compileDate.ToString("dd.MM.yyyy") + ' '/* + (Environment.Is64BitProcess ? "64bit" : "32Bit")*/;
            LogHandle.ol(Text);
            this.Icon = Resources.fd_editpr_16_2;
            textBox_fnBriefInfo.Text = "";

            UpdateComNanes();

            checkBox_xcgExtendedLog.Checked = lib.LogFormat == 1;
            comboBox_xcgMsgFormat.SelectedIndex = 0;
            if (lib.PortIsOpened)
            {
                if (comboBox_connsttsPortName.Items.Contains(lib.ComPort)) comboBox_connsttsPortName.SelectedIndex = comboBox_connsttsPortName.Items.IndexOf(lib.ComPort);
                if (comboBox_connsttsBaudrate.Items.Contains(lib.Baudrate.ToString())) comboBox_connsttsBaudrate.SelectedIndex = comboBox_connsttsBaudrate.Items.IndexOf(lib.Baudrate.ToString());
                textBox_connTimeout.Text = lib.Timeout.ToString();
                _skipConnectionEvent = true;
                checkBox_xcgOpenPort.Checked = true;

            }
            else
            {
                comboBox_connsttsBaudrate.SelectedIndex = 10;
                if (comboBox_connsttsPortName.Items.Count > 0)
                {
                    comboBox_connsttsPortName.SelectedIndex = comboBox_connsttsPortName.Items.Count - 1;
                    lib.ComPort = "COM" + (comboBox_connsttsPortName.Items.Count - 1);
                }
                if (!string.IsNullOrEmpty(comboBox_connsttsPortName.Text)) lib.ComPort = comboBox_connsttsPortName.Text;
            }
            checkBox_terminalDontLock.Checked = lib.DontLockChannel;
            _skipTimeSetEvent = true;
            numericUpDown_plusMin.Value = lib.PlusMin;
            tabControl_timeModuleMainSwitcher.SelectedIndex = lib.TimeSource;
            radioButton_timerStop2.Checked = !lib.IsTimeLaunched;
            radioButton_timerStop1.Checked = !lib.IsTimeLaunched;
            radioButton_timerStart2.Checked = lib.IsTimeLaunched;
            radioButton_timerStart1.Checked = lib.IsTimeLaunched;
            dateTimePicker_setDtForFd.Value = lib.GetTimeForFd;
            _skipTimeSetEvent = false;
            _skipSettingsConEvent = false;
            _AvailibleSetting(checkBox_xcgOpenPort.Checked);
            if (lib.UartEvents > 0)
            {
                new Thread(() =>
                {
                    Thread.Sleep(1000);
                    lib.LogAddEvent(UartEvent.SLIP);
                }).Start();
            }

            //textBox_xcgRawData.AutoScrollOffset = 
            timer = new System.Timers.Timer();
            timer.AutoReset = true;
            timer.Elapsed += TimerTick;
            timer.Interval = 765;
            timer.Enabled = true;
            timer.Start();
            dateTimePicker_setDtForFd.MinDate = AppSettings.MinAvailableFdTimeFn;
            label_dtAlarmingT1.Text = label_dtAlarmingT2.Text = "Минимальное разрешенное время для формирования ФД " + AppSettings.MinAvailableFdTimeFn.ToString("dd.MM.yyyy ") + "для изменения \r\nдобавьте строку приведенную ниже в конфигурационный файл и перезапустите программу";
            comboBox_performTabFfdToPerform.SelectedIndex = 0;
        }

        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    label_timeModuleShowTime.Text = lib.GetTimeForFd.ToString("dd.MM.yyyy HH:mm:ss");
                    label_PerfTabtimeForFd.Text = lib.GetTimeForFd.ToString("dd.MM.yyyy HH:mm");
                    this.Text = _title + lib.GetTimeForFd.ToString("  время для ФД:  dd.MM.yyyy HH:mm");
                }));
            }
            else
            {
                label_timeModuleShowTime.Text = lib.GetTimeForFd.ToString("dd.MM.yyyy HH:mm:ss");
                label_PerfTabtimeForFd.Text = lib.GetTimeForFd.ToString("dd.MM.yyyy HH:mm");
                this.Text = _title + lib.GetTimeForFd.ToString("  время для ФД:  dd.MM.yyyy HH:mm");
            }
        }

        public void FlowOfTime(bool run)
        {
            /*if(run)
                lib.LaunchTimer();
            else
                lib.StopTimer();*/
            if (this.Created && !this.IsDisposed)
            {
                if (this.InvokeRequired)
                {
                    BeginInvoke(new Action(() =>
                    {
                        int ts = tabControl_timeModuleMainSwitcher.SelectedIndex;
                        if (ts == 1 && radioButton_timerStart1.Checked!=run)
                        {
                            radioButton_timerStart1.Checked = run;
                        }
                        else if (ts == 2 && radioButton_timerStart2.Checked!=run)
                        {
                            radioButton_timerStart2.Checked = run;
                        }
                    }));
                }
                else
                {
                    int ts = tabControl_timeModuleMainSwitcher.SelectedIndex;
                    if (ts == 1 && radioButton_timerStart1.Checked != run)
                    {
                        radioButton_timerStart1.Checked = run;
                    }
                    else if (ts == 2 && radioButton_timerStart2.Checked != run)
                    {
                        radioButton_timerStart2.Checked = run;
                    }
                }
            }
        }

        private string _title = "";
        System.Timers.Timer timer;

        TerminalFnExchange lib;

        private bool _skipConnectionEvent = false;
        private bool _skipSettingsConEvent = false;
        private bool _skipTextBoxUpdatedEvent = false;
        int _fdRulesSelIndLast = -2;
        int _ffdLastChosen = -1;
        List<int> _implementedFds = new List<int>
        {
            2,      // открытие смены
            3,
            5,      // закрытие смены
            21,     // отчет о состоянии расчетов  
            31,
        };

        // обработчик событий не которые не взаиможействуют с ФН
        private void CommonConrolsEventHandler(object sender, EventArgs e)
        {
            // открываем порт
            if (sender == checkBox_xcgOpenPort)
            {
                _AvailibleSetting(checkBox_xcgOpenPort.Checked);
                if (_skipConnectionEvent)
                {
                    // тут ошибки подключения/инициализация приложения
                    _skipConnectionEvent = false;
                }
                else
                {
                    if (checkBox_xcgOpenPort.Checked)// открываем порт
                    {
                        if (lib.OpenPort() != OK)
                        {
                            _skipConnectionEvent = true;
                            checkBox_xcgOpenPort.Checked = false;
                        }
                    }
                    else
                    {
                        if (lib.ClosePort() != OK)
                        {
                            if (lib.PortIsOpened)
                            {
                                _skipConnectionEvent = true;
                                checkBox_xcgOpenPort.Checked = true;
                            }
                        }
                    }

                }


                checkBox_connectConnectionParamsTab.Checked = checkBox_xcgOpenPort.Checked;

            }
            else if (sender == checkBox_terminalDontLock)
            {
                lib.DontLockChannel = checkBox_terminalDontLock.Checked;
            }
            // выбор документа для формирования или ФФД
            else if(sender == comboBox_performTabDocType || sender == comboBox_performTabFfdToPerform )
            {
                dataGridView_ftagListToPerform.Rows.Clear();
                treeView_tabPerfStlvMiniConstructor.Nodes.Clear();
                textBox_perfTab_ftagUFName.Text = "";
                textBox_tabFdToPerf_EditorNumberMain.Text = "";
                textBox_perfTabSelectedTNType.Text = "";
                textBox__tabFdToPerf_EditorValueMain.Text = "";
                richTextBox_perfTabDataForBegin.Text = "";
                textBox__tabFdToPerf_EditorValueMain.ForeColor = Color.Black;
                textBox_perfTabDataForClosing.Text = "";
                textBox_perfTabDataForClosing.Tag = null;

                int fdType = TagNumberChosen(comboBox_performTabDocType.Text);
                int ffdVers = comboBox_performTabFfdToPerform.SelectedIndex+2;
                int key = fdType + 65536 * ffdVers;
                if (FTag.TFNCommonRules.ContainsKey(key) && FTag.TFNCommonRules[key].Rules.Count>0)
                {
                    var ruleset = FTag.TFNCommonRules[key];
                    foreach (var t in ruleset.Rules)
                    {
                        // заполняем таблицу с тегами
                        if (t.DataSource != FTag.TFTagRuleSet.RSOURCE_IGNORE)
                        {
                            string tag_name_s = "";
                            if (FTag.fnsNames.ContainsKey(t.TagNumber))
                            {
                                tag_name_s = FTag.fnsNames[t.TagNumber];
                            }
                            string tag_value = "";
                            FTag f = null;
                            if(t.DataSource == FTag.TFTagRuleSet.RSOURCE_OVERRIDE)
                            {
                                tag_value = t.DefaultData;
                            }
                            else if (t.DataSource == FTag.TFTagRuleSet.RSOURCE_REG_PARAM)
                            {
                                f = lib.GetRegFtag(t.TagNumber);
                                if (f != null)
                                {
                                    tag_value = f.Representation;
                                }
                                else
                                {
                                    if (t.DefaultData != null)
                                    {
                                        tag_value = t.DefaultData;
                                    }
                                }
                            }
                            else if(t.DataSource == FTag.TFTagRuleSet.RSOURCE_INCLASS)
                            {
                                // тут прикрутить какой-нибудь конструктор чеков
                                // или оставить незаполняемые поля(а константа останется для внешних методов формирования)
                                if (t.DefaultData != null)
                                {
                                    tag_value = t.DefaultData;
                                }
                            }
                            string tag_uf_name = "";
                            if (FTag.userFrandlyNames.ContainsKey(t.TagNumber))
                            {
                                tag_uf_name = FTag.userFrandlyNames[t.TagNumber];
                            }

                            object[] rowValues = new object[5];
                            rowValues[IND_CELL_TAG_NUMBER] = t.TagNumber.ToString();
                            rowValues[IND_CELL_TAG_NAME] = tag_name_s;
                            rowValues[IND_CELL_TAG_UF_NAME] = tag_uf_name;
                            rowValues[IND_CELL_TAG_PRESENTATION_STR] = tag_value;
                            rowValues[IND_CELL_PROCESSING_STATUS] = "";


                            int rowInd = dataGridView_ftagListToPerform.Rows.Add(rowValues);
                            if (f == null)
                            {
                                try
                                {
                                    if (FTag.typeMap[t.TagNumber] == FTag.FDataType.STLV)
                                    {

                                        f = new FTag(t.TagNumber, new List<FTag>(),false);
                                    }
                                    else
                                        f = new FTag(t.TagNumber, tag_value, true);
                                }
                                catch(Exception exc)
                                {
                                    dataGridView_ftagListToPerform.Rows[rowInd].Cells[IND_CELL_PROCESSING_STATUS].Value = "ERROR: "+exc.ToString();
                                    dataGridView_ftagListToPerform.Rows[rowInd].Cells[IND_CELL_PROCESSING_STATUS].Style.ForeColor = Color.Red;
                                }
                                if(f == null)
                                {
                                    f = FTag.EmptyNumerredFtag(t.TagNumber);
                                }
                                if (f != null&&f.Type==FTag.FDataType.UNKNOWN)
                                {
                                    dataGridView_ftagListToPerform.Rows[rowInd].Cells[IND_CELL_PROCESSING_STATUS].Value = "Пустой тег";
                                    dataGridView_ftagListToPerform.Rows[rowInd].Cells[IND_CELL_PROCESSING_STATUS].Style.ForeColor = Color.Red;
                                }
                            }
                            // Проверить ФФД на теги которые могут включаться несколько раз в одну структуру

                            List<FTag> l = new List<FTag> { f };
                            dataGridView_ftagListToPerform.Rows[rowInd].Tag = l;

                        }
                    }
                    
                    if (_implementedFds.Contains(fdType))       // тут проверерка реализации метода формирования ФД
                    {
                        if(
                            fdType == FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_SHIFT ||
                            fdType == FTAG_FISCAL_DOCUMENT_TYPE_OPEN_SHIFT ||
                            fdType == FTAG_FISCAL_DOCUMENT_TYPE_CALCULATION_REPORT
                            )
                        {
                            checkBox_perfTabDataForBegin.Checked = true;
                            richTextBox_perfTabDataForBegin.Text = "ДатаВремя";
                            richTextBox_perfTabDataForBegin.ForeColor = Color.Green;
                            richTextBox_perfTabDataForBegin.SelectAll();
                            richTextBox_perfTabDataForBegin.SelectionFont = new Font(richTextBox_perfTabDataForBegin.Font, FontStyle.Bold);
                            richTextBox_perfTabDataForBegin.DeselectAll();
                            groupBox_performTabAll.Enabled = true;
                        }
                        else if(
                            fdType == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE ||
                            fdType == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE
                            )
                        {
                            checkBox_perfTabDataForBegin.Checked = true;
                            richTextBox_perfTabDataForBegin.Text = "ДатаВремя";
                            richTextBox_perfTabDataForBegin.ForeColor = Color.Green;
                            richTextBox_perfTabDataForBegin.SelectAll();
                            richTextBox_perfTabDataForBegin.SelectionFont = new Font(richTextBox_perfTabDataForBegin.Font, FontStyle.Bold);
                            richTextBox_perfTabDataForBegin.DeselectAll();
                            groupBox_performTabAll.Enabled = true;

                            textBox_perfTabDataForClosing.Text = "ДатаВремя"+Environment.NewLine +"Итог в копейках";
                            //richTextBox_perfTabDataForBegin.ForeColor = Color.Green;
                            //richTextBox_perfTabDataForBegin.SelectAll();
                            //richTextBox_perfTabDataForBegin.SelectionFont = new Font(richTextBox_perfTabDataForBegin.Font, FontStyle.Bold);
                            //richTextBox_perfTabDataForBegin.DeselectAll();



                        }
                        groupBox_performTabAll.Enabled = true;
                        UpdateStatusArea("", "", 0);
                    }
                    else
                    {
                        checkBox_perfTabDataForBegin.Checked = false;
                        groupBox_performTabAll.Enabled = false;
                        UpdateStatusArea(NOT_SUPPORTED_THIS_VER, "", 0);
                    }


                }
                else
                {
                    UpdateStatusArea(NO_FD_RULE_SET, "", 0);
                    groupBox_performTabAll.Enabled = false;
                }
            }
            else if (sender == comboBox_connsttsPortName)
            {
                if (!_skipSettingsConEvent) lib.ComPort = comboBox_connsttsPortName.Text;
            }
            else if (sender == comboBox_connsttsBaudrate)
            {
                if (!_skipSettingsConEvent) try { lib.Baudrate = int.Parse(comboBox_connsttsBaudrate.Text); } catch { }
            }
            else if (sender == textBox_connTimeout)
            {
                // добавить обработку строки
                if (!_skipSettingsConEvent) try { lib.Timeout = int.Parse(textBox_connTimeout.Text); } catch { }
            }
            // отправляем данные в порт
            else if (sender == button_xcgSend)
            {
                // добавить преобразование строки в зависимости от comboBox_xcgMsgFormat
                if (!string.IsNullOrEmpty(textBox_xcgMessage.Text))
                {
                    if (comboBox_xcgMsgFormat.SelectedIndex == 0)
                    {
                        string s = textBox_xcgMessage.Text;
                        if (s.Length % 3 == 1)
                        {
                            lib.LogAddEvent(new UartEvent(MsgSource.ERROR, "Сообщение неполное"));
                        }
                        else
                        {
                            lib.SendData(BytesFromImage(s));
                        }

                    }


                }
                else lib.LogAddEvent(new UartEvent(MsgSource.ERROR, "Нечего отправлять"));
            }
            // событие изменения текстбокса обработка строки для отправки
            else if (sender == textBox_xcgMessage)
            {
                if (_skipTextBoxUpdatedEvent)
                {
                    _skipTextBoxUpdatedEvent = false;
                    return;
                }
                // hex image
                if (comboBox_xcgMsgFormat.SelectedIndex == 0)
                {
                    int curentCursor = textBox_xcgMessage.SelectionStart;
                    string s = textBox_xcgMessage.Text;
                    StringBuilder sbHexImage = new StringBuilder();
                    int newCursor = 0;
                    for (int i = 0; i < s.Length; i++)
                    {
                        char c = char.ToUpper(s[i]);
                        if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F'))
                        {
                            if (sbHexImage.Length % 3 == 2)
                            {
                                sbHexImage.Append('-');
                                if (i < curentCursor)
                                    newCursor++;
                            }   // если байт завершен добавляем разделитель
                            sbHexImage = sbHexImage.Append(c);
                            if (i < curentCursor)
                                newCursor++;
                        }
                        string updated = sbHexImage.ToString();
                        if (textBox_xcgMessage.Text.Length != updated.Length)
                        {
                            _skipTextBoxUpdatedEvent = true;
                            textBox_xcgMessage.Text = updated;
                            textBox_xcgMessage.SelectionStart = newCursor;
                        }
                        textBox_xcgMessage.ForeColor = updated.Length % 3 == 2 ? Color.Black : Color.Red;
                    }
                }
            }
            else if (sender == button_xcgClean)
            {
                lib.ClearHistory();
            }
            else if (sender == checkBox_xcgExtendedLog)
            {
                lib.LogFormat = checkBox_xcgExtendedLog.Checked ? 1 : 0;
                lib.LogAddEvent(UartEvent.SLIP);
            }
            else if (sender == button_xcgCrcCcit)
            {
                Crc16Ccitt crc = new Crc16Ccitt(InitialCrcValue.NonZero1);
                byte[] data = BytesFromImage(textBox_xcgMessage.Text);

                if (checkBox_xcgIgnoreStartSign.Checked)
                {
                    if (textBox_xcgMessage.Text.StartsWith("04-"))
                    {
                        byte[] data2 = new byte[data.Length - 1];
                        Array.Copy(data, 1, data2, 0, data2.Length);
                        data = data2;
                    }
                }
                byte[] crcBytes = crc.ComputeChecksumBytes(data);

                textBox_xcgMessage.Text += BitConverter.ToString(crcBytes);

            }
            else if (sender == button_updateComNames)
            {
                UpdateComNanes();
            }
            else if (sender == checkBox_connectConnectionParamsTab)
            {
                if (checkBox_xcgOpenPort.Checked != checkBox_connectConnectionParamsTab.Checked)
                {
                    checkBox_xcgOpenPort.Checked = checkBox_connectConnectionParamsTab.Checked;
                }
            }
            else if (sender == comboBox_fncCommand)
            {
                if (comboBox_fncCommand.Text.Length > 2)
                {
                    char p1k = comboBox_fncCommand.Text[0];
                    char p2k = comboBox_fncCommand.Text[1];
                    int p1, p2;
                    if (p1k >= '0' && p1k <= '9')
                        p1 = p1k - '0';
                    else
                        p1 = 10 + p1k - 'A';
                    if (p2k >= '0' && p2k <= '9')
                        p2 = p2k - '0';
                    else
                        p2 = 10 + p2k - 'A';
                    byte key = (byte)(p1 * 16 + p2);
                    textBox_cconstructorLength.Text = "1";
                    textBox_outDataConstructor.Text = "04-01-00-" + p1k + p2k + "-" +
                        BitConverter.ToString(scrc.ComputeChecksumBytes(new byte[] { 1, 0, key }));

                    if (CommandDescriber.ContainsKey(key))
                    {
                        textBox_showCmdParams.Text = "Входные параметры " + CommandDescriber[key][1].ToString() + Environment.NewLine + "Выходные параметры " + CommandDescriber[key][2].ToString();

                    }
                    else
                        textBox_showCmdParams.Text = "Описание команды отсутсвует.";
                }
                else
                {
                    textBox_showCmdParams.Text = string.Empty;
                }
            }
            else if (sender == button_addData)
            {
                int commandLenth = int.Parse(textBox_cconstructorLength.Text);
                if (commandLenth < 1)
                {
                    textBox_outDataConstructor.Text = "Выберите команду";
                }
                else
                {
                    string cmdLast = textBox_outDataConstructor.Text;
                    try
                    {
                        string cmd = "";

                        List<byte> cmdLastBytes = new List<byte>(FTag.FTLVParcer.StringHexToBytes(cmdLast.Substring(3, cmdLast.Length - 9)));


                        if (radioButton_cconstructorByte.Checked)
                        {
                            byte b;
                            byte.TryParse(textBox_valueAddToConstructor.Text, out b);
                            cmdLastBytes.Add(b);
                            commandLenth++;
                            cmdLastBytes[0] = (byte)(commandLenth % 256);
                            cmdLastBytes[1] = (byte)(commandLenth / 256);
                            if (textBox_valueAddToConstructor.Text != b.ToString()) textBox_valueAddToConstructor.Text = b.ToString();
                            //extBox_outDataConstructor.Text += "-"+b.ToString("X2");
                            byte[] crcBytes = TerminalFnExchange.scrc.ComputeChecksumBytes(cmdLastBytes.ToArray());
                            cmdLastBytes.AddRange(crcBytes);
                            cmd = "04-" + BitConverter.ToString(cmdLastBytes.ToArray());

                        }
                        else if (radioButton_cconstructorUint16.Checked)
                        {

                            uint x;
                            if (uint.TryParse(textBox_valueAddToConstructor.Text, out x))
                            {
                                commandLenth += 2;
                                cmdLastBytes[0] = (byte)(commandLenth % 256);
                                cmdLastBytes[1] = (byte)(commandLenth / 256);
                                cmdLastBytes.AddRange(new byte[] { (byte)(x % 256), (byte)(x / 256) });
                                byte[] crcBytes = TerminalFnExchange.scrc.ComputeChecksumBytes(cmdLastBytes.ToArray());
                                cmdLastBytes.AddRange(crcBytes);
                                cmd = "04-" + BitConverter.ToString(cmdLastBytes.ToArray());
                            }
                            else
                            {
                                cmd = cmdLast;
                                UpdateStatusArea("введенная строка не UINT16");
                            }
                            //string cmdLast = textBox_outDataConstructor.Text;
                            //cmd = "04-" + (commandLenth % 256).ToString("X2") + "-" + (commandLenth / 256).ToString("X2") + cmdLast.Substring(8) + "-" + (x % 256).ToString("X2") + "-" + (x / 256).ToString("X2");

                        }
                        else if (radioButton_cconstructorUint32.Checked)
                        {

                            uint x;
                            if (uint.TryParse(textBox_valueAddToConstructor.Text, out x))
                            {
                                commandLenth += 4;
                                cmdLastBytes[0] = (byte)(commandLenth % 256);
                                cmdLastBytes[1] = (byte)(commandLenth / 256);
                                cmdLastBytes.AddRange(new byte[] { (byte)(x % 256), (byte)((x / 256) % 256), (byte)(((x / 256 / 256) % 256)), (byte)((x / 256 / 256 / 256 / 256)) });
                                byte[] crcBytes = TerminalFnExchange.scrc.ComputeChecksumBytes(cmdLastBytes.ToArray());
                                cmdLastBytes.AddRange(crcBytes);
                                cmd = "04-" + BitConverter.ToString(cmdLastBytes.ToArray());


                            }
                            else
                            {
                                cmd = cmdLast;
                                UpdateStatusArea("введенная строка не UINT32");
                            }

                            /*cmd = "04-" + (commandLenth % 256).ToString("X2") + "-" + (commandLenth / 256).ToString("X2") + cmdLast.Substring(8) + "-" +
                                (x % 256).ToString("X2") + "-" +
                                (x / 256 % 256).ToString("X2") + "-" +
                                (x / 256 / 256 % 256).ToString("X2") + "-" +
                                (x / 256 / 256 / 256).ToString("X2");*/
                        }
                        else if (radioButton_cconstructorAsciiStr.Checked)
                        {
                            byte[] strBytes = Encoding.ASCII.GetBytes(textBox_valueAddToConstructor.Text);
                            commandLenth += strBytes.Length;
                            cmdLastBytes[0] = (byte)(commandLenth % 256);
                            cmdLastBytes[1] = (byte)(commandLenth / 256);
                            cmdLastBytes.AddRange(strBytes);
                            byte[] crcBytes = TerminalFnExchange.scrc.ComputeChecksumBytes(cmdLastBytes.ToArray());
                            cmdLastBytes.AddRange(crcBytes);
                            cmd = "04-" + BitConverter.ToString(cmdLastBytes.ToArray());
                            //cmd = "04-" + (commandLenth % 256).ToString("X2") + "-" + (commandLenth / 256).ToString("X2") + cmdLast.Substring(8) + "-" + BitConverter.ToString(strBytes);
                        }
                        else if (radioButton_cconstructorDtFormat.Checked)
                        {
                            //byte[] timeBytes = new byte[5];
                            //commandLenth += 5;
                            DateTime dt = new DateTime(1900, 1, 1, 0, 0, 0);
                            if (
                                (DateTime.TryParseExact(textBox_valueAddToConstructor.Text, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                                || DateTime.TryParseExact(textBox_valueAddToConstructor.Text, "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                                || DateTime.TryParseExact(textBox_valueAddToConstructor.Text, "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                                || DateTime.TryParseExact(textBox_valueAddToConstructor.Text, "yyyy MM dd HH mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                                || DateTime.TryParseExact(textBox_valueAddToConstructor.Text, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                                || DateTime.TryParseExact(textBox_valueAddToConstructor.Text, "yyyy.MM.ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                                || DateTime.TryParseExact(textBox_valueAddToConstructor.Text, "yyyy/MM/ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                                || DateTime.TryParseExact(textBox_valueAddToConstructor.Text, "yyyy MM ddTHH mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                                ) && (dt - new DateTime(2017, 01, 01)).TotalHours > 0)
                            {
                                commandLenth += 5;
                                cmdLastBytes[0] = (byte)(commandLenth % 256);
                                cmdLastBytes[1] = (byte)(commandLenth / 256);
                                cmdLastBytes.AddRange(new byte[] { (byte)(dt.Year % 100), (byte)(dt.Month), (byte)(dt.Day), (byte)dt.Hour, (byte)dt.Minute });
                                cmdLastBytes.AddRange(scrc.ComputeChecksumBytes(cmdLastBytes.ToArray()));
                                cmd = "04-" + BitConverter.ToString(cmdLastBytes.ToArray());

                                /*cmd = "04-" + (commandLenth % 256).ToString("X2") + "-" + (commandLenth / 256).ToString("X2") + cmdLast.Substring(8) + "-" +
                                (dt.Year % 100).ToString("X2") + "-" +
                                dt.Month.ToString("X2") + "-" +
                                dt.Day.ToString("X2") + "-" +
                                dt.Hour.ToString("X2") + "-" +
                                dt.Minute.ToString("X2");*/
                            }
                            else
                            {
                                cmd = cmdLast;
                                //commandLenth -= 5;
                                UpdateStatusArea("Некорректный формат даты", "гггг.ММ.ДД чч.мм");
                            }
                        }
                        else if (radioButton_cconstructorDateFormat.Checked)
                        {

                            DateTime dt = new DateTime(1900, 1, 1, 0, 0, 0);
                            if (
                                (DateTime.TryParseExact(textBox_valueAddToConstructor.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                                || DateTime.TryParseExact(textBox_valueAddToConstructor.Text, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                                || DateTime.TryParseExact(textBox_valueAddToConstructor.Text, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                                || DateTime.TryParseExact(textBox_valueAddToConstructor.Text, "yyyy MM dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                                || DateTime.TryParseExact(textBox_valueAddToConstructor.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)
                                ) && (dt - new DateTime(2017, 1, 1)).TotalHours > 0)
                            {
                                commandLenth += 3;
                                cmdLastBytes[1] = (byte)(commandLenth % 256);
                                cmdLastBytes[1] = (byte)(commandLenth / 256);
                                cmdLastBytes.AddRange(new byte[] { (byte)(dt.Year % 100), (byte)(dt.Month), (byte)(dt.Day) });
                                cmdLastBytes.AddRange(scrc.ComputeChecksumBytes(cmdLastBytes.ToArray()));
                                cmd = cmd = "04-" + BitConverter.ToString(cmdLastBytes.ToArray());

                                /*cmd = "04-" + (commandLenth % 256).ToString("X2") + "-" + (commandLenth / 256).ToString("X2") + cmdLast.Substring(8) + "-" +
                                (dt.Year % 100).ToString("X2") + "-" +
                                dt.Month.ToString("X2") + "-" +
                                dt.Day.ToString("X2");*/
                            }
                            else
                            {
                                cmd = cmdLast;
                                //commandLenth -= 3;
                                UpdateStatusArea("Некорректный формат даты", "гггг.ММ.ДД");
                            }

                        }
                        else if (radioButton_hexData.Checked)
                        {
                            byte[] bytesFrStr = BytesFromImage(textBox_valueAddToConstructor.Text);
                            if (bytesFrStr.Length > 0)
                            {
                                commandLenth += bytesFrStr.Length;
                                cmdLastBytes[0] = (byte)(commandLenth % 256);
                                cmdLastBytes[1] = (byte)(commandLenth / 256);
                                cmdLastBytes.AddRange(bytesFrStr);
                                cmdLastBytes.AddRange(scrc.ComputeChecksumBytes(cmdLastBytes.ToArray()));
                                cmd = "04-" + BitConverter.ToString(cmdLastBytes.ToArray());
                            }
                            else
                            {
                                cmd = cmdLast;
                                UpdateStatusArea("Проверьте формат строки");
                            }
                            /*if (bytesFrStr.Length > 0)
                            {
                                cmd = cmd = "04-" + (commandLenth % 256).ToString("X2") + "-" + (commandLenth / 256).ToString("X2") + cmdLast.Substring(8) + "-" + textBox_valueAddToConstructor.Text;
                                commandLenth += bytesFrStr.Length;
                            }
                            else
                            {
                                cmd = cmdLast;
                            }*/
                        }
                        else if (radioButton_cconstructorUint40.Checked)
                        {

                            ulong x;
                            if (ulong.TryParse(textBox_valueAddToConstructor.Text, out x))
                            {
                                commandLenth += 5;
                                cmdLastBytes.AddRange(new byte[] { (byte)(x % 256), (byte)(x / 256 % 256), (byte)(x / 256 / 256 % 256), (byte)(x / 256 / 256 / 256 % 256), (byte)(x / 256 / 256 / 256 / 256) });
                                cmdLastBytes.AddRange(scrc.ComputeChecksumBytes(cmdLastBytes.ToArray()));
                                cmd = "04-" + BitConverter.ToString(cmdLastBytes.ToArray());

                            }
                            else
                            {
                                UpdateStatusArea("Введенное значение не является Uint40");
                                cmd = cmdLast;
                            }
                            /*cmd = "04-" + (commandLenth % 256).ToString("X2") + "-" + (commandLenth / 256).ToString("X2") + cmdLast.Substring(8) + "-" +
                                (x % 256).ToString("X2") + "-" +
                                (x / 256 % 256).ToString("X2") + "-" +
                                (x / 256 / 256 % 256).ToString("X2") + "-" +
                                (x / 256 / 256 / 256 % 256).ToString("X2") + "-" +
                                (x / 256 / 256 / 256 / 256).ToString("X2");*/

                        }

                        textBox_outDataConstructor.Text = cmd;
                        textBox_cconstructorLength.Text = commandLenth.ToString();
                    }
                    catch (Exception ex)
                    {
                        UpdateStatusArea(ex.Message);
                    }



                }

            }
            else if (sender == comboBox_ruleTabFdTypeChooser)
            {
                if (_ffdLastChosen > 0 /*&& _fdRulesSelIndLast >= 0*/)
                {
                    if (comboBox_ruleTabFdTypeChooser.SelectedIndex != _fdRulesSelIndLast)
                    {
                        // Индекс изменился
                        // Быбрано новое значение для  указателя документов
                        // загузка существующих правил
                        
                        LogHandle.ol("sel ind chngd " + _fdRulesSelIndLast);
                        int ruleKey = TagNumberChosen(comboBox_ruleTabFdTypeChooser.Text) + _ffdLastChosen * 65536;
                        if (FTag.TFNCommonRules.ContainsKey(ruleKey))
                        {
                            FillTFNRuleTable(FTag.TFNCommonRules[ruleKey]);
                        }
                        else
                        {
                            dataGridView_termFdRules.Rows.Clear();
                        }
                    }
                    else
                    {
                        // Индекс не изменился
                        // открытие списка указателя документов
                        // Сохранение таблицы и отправка данных в контейнер
                        LogHandle.ol("sel ind not chngd " + _fdRulesSelIndLast);
                        int stlvType = TagNumberChosen(comboBox_ruleTabFdTypeChooser.Text);
                        FTag.TFTagRuleSet set = ReadTableRules(stlvType, _ffdLastChosen);
                        if(set.Rules.Count>0)
                            FTag.TFNCommonRules[_ffdLastChosen * 65536 + stlvType]= set;
                    }
                }
                
                _fdRulesSelIndLast = comboBox_ruleTabFdTypeChooser.SelectedIndex;
            }
            else if(sender == radioButton_ffdRulesSwitcher_2&& radioButton_ffdRulesSwitcher_2.Checked)
            {
                int stlvType = TagNumberChosen(comboBox_ruleTabFdTypeChooser.Text);
                if (_ffdLastChosen > 0 && stlvType >0)
                {
                    // ранее была выбрана ФФД сохраняем таблицу
                    //int stlvType = _TagNumberChosen(comboBox_fdTypeChooser.Text);
                    FTag.TFTagRuleSet set = ReadTableRules(stlvType, _ffdLastChosen);
                    if (set.Rules.Count > 0)
                        FTag.TFNCommonRules[_ffdLastChosen * 65536 + stlvType] = set;
                }
                _ffdLastChosen = 2;
                // Загрузка и отображение таблицы
                int ruleKey = _ffdLastChosen * 65536 + stlvType;
                if (FTag.TFNCommonRules.ContainsKey(ruleKey))
                {
                    FillTFNRuleTable(FTag.TFNCommonRules[ruleKey]);
                }
                else
                {
                    dataGridView_termFdRules.Rows.Clear();
                }
            }
            else if (sender == radioButton_ffdRulesSwitcher_3 && radioButton_ffdRulesSwitcher_3.Checked)
            {
                int stlvType = TagNumberChosen(comboBox_ruleTabFdTypeChooser.Text);
                if (_ffdLastChosen > 0 && stlvType > 0)
                {
                    // ранее была выбрана ФФД сохраняем таблицу
                    //int stlvType = _TagNumberChosen(comboBox_fdTypeChooser.Text);
                    FTag.TFTagRuleSet set = ReadTableRules(stlvType, _ffdLastChosen);
                    if (set.Rules.Count > 0)
                        FTag.TFNCommonRules[_ffdLastChosen * 65536 + stlvType] = set;
                }
                _ffdLastChosen = 3;
                // Загрузка и отображение таблицы
                int ruleKey = _ffdLastChosen * 65536 + stlvType;
                if (FTag.TFNCommonRules.ContainsKey(ruleKey))
                {
                    FillTFNRuleTable(FTag.TFNCommonRules[ruleKey]);
                }
                else
                {
                    dataGridView_termFdRules.Rows.Clear();
                }
            }
            else if (sender == radioButton_ffdRulesSwitcher_4 && radioButton_ffdRulesSwitcher_4.Checked)
            {
                int stlvType = TagNumberChosen(comboBox_ruleTabFdTypeChooser.Text);
                if (_ffdLastChosen > 0 && stlvType > 0)
                {
                    // ранее была выбрана ФФД сохраняем таблицу
                    //int stlvType = _TagNumberChosen(comboBox_fdTypeChooser.Text);
                    FTag.TFTagRuleSet set = ReadTableRules(stlvType, _ffdLastChosen);
                    if (set.Rules.Count > 0)
                        FTag.TFNCommonRules[_ffdLastChosen * 65536 + stlvType] = set;
                }
                _ffdLastChosen = 4;
                // Загрузка и отображение таблицы
                int ruleKey = _ffdLastChosen * 65536 + stlvType;
                if (FTag.TFNCommonRules.ContainsKey(ruleKey))
                {
                    FillTFNRuleTable(FTag.TFNCommonRules[ruleKey]);
                }
                else
                {
                    dataGridView_termFdRules.Rows.Clear();
                }
            }
            else if(sender == button_saveTFNCommonFdRules)
            {
                FTag.SaveTFNRules();
            }
            else if(sender == button_loadTFNR)
            {
                FTag.LoadTFNRules();
            }
            else if(sender == button_fdRulesTabApplyCurrentTable)
            {
                int stlvType = TagNumberChosen(comboBox_ruleTabFdTypeChooser.Text);
                int ffdChosen = 0;
                if (radioButton_ffdRulesSwitcher_2.Checked)
                    ffdChosen = 2;
                else if (radioButton_ffdRulesSwitcher_3.Checked)
                    ffdChosen = 3;
                else if (radioButton_ffdRulesSwitcher_4.Checked)
                    ffdChosen = 4;
                if (ffdChosen > 0 && stlvType > 0)
                {
                    FTag.TFTagRuleSet set = ReadTableRules(stlvType, ffdChosen);
                    if (set.Rules.Count > 0)
                        FTag.TFNCommonRules[_ffdLastChosen * 65536 + stlvType] = set;
                }
            }
            else if(sender == textBox__tabFdToPerf_EditorValueMain)
            {
                TreeNode trvn = treeView_tabPerfStlvMiniConstructor.SelectedNode;
                if (textBox__tabFdToPerf_EditorValueMain.Tag == null)
                {
                    // нечего редактировать - либо инициализация поля при выборе, либо на узле тоже пусто
                    //if(trvn.Tag != null)
                    return;
                }
                


                if(!(textBox__tabFdToPerf_EditorValueMain.Tag is FTag))
                {
                    LogHandle.ol("Некорректно переданы данные в поле редактирования ! ! ! ! Требуется проверка кода");
                    return;
                }

                FTag fo = textBox__tabFdToPerf_EditorValueMain.Tag as FTag;
                if(fo.Type == FTag.FDataType.STLV)
                {
                    // редактирование stlv тега в текстовом варианте невозможно
                    textBox__tabFdToPerf_EditorValueMain.Text = fo.Representation;
                    UpdateStatusArea("Невозможно так редактировать STLV","Выберите вложенные теги");
                    return ;
                }

                var rr = dataGridView_ftagListToPerform.SelectedCells;
                if (rr==null || rr.Count == 0)
                {
                    // если пользователю вздумается редактировать поле не выбрав значение таблицы
                    UpdateStatusArea("Невозможно определить выделенную строку таблицы","Выберите строку с реквизитом");
                    return;
                }

                int rInd = rr[0].RowIndex;
                var row = dataGridView_ftagListToPerform.Rows[rInd];

                if (rInd >= 0)
                {
                    
                    if(trvn == null)
                    {
                        LogHandle.ol("Не удалось определить выделенный узел TreeView представления тегов");
                        UpdateStatusArea("", "Выберите тег для редактирования");
                        return;
                    }
                    int tagNumber = 0;
                    if(trvn.Tag == fo && fo.TagNumber != 0)
                    {
                        tagNumber = fo.TagNumber;
                    }
                    else
                    {
                        if (trvn.Text.Contains("("))
                        {
                            int.TryParse(trvn.Text.Substring(0, trvn.Text.IndexOf('(')), out tagNumber);
                        }
                        else
                        {
                            int.TryParse(trvn.Text, out tagNumber);
                        }
                    }
                    if (tagNumber <= 0)
                    {
                        UpdateStatusArea("bad tag number","");
                    }


                    //                    FTag f = null;
                    fo.Rebuild(textBox__tabFdToPerf_EditorValueMain.Text);
                    if (trvn.Level > 0)
                    {
                        (trvn.Parent.Tag as FTag).RebuildPrezentation();
                    }
                    if (fo.Recreated > 0)
                    {
                        textBox__tabFdToPerf_EditorValueMain.ForeColor = Color.Black;
                        if (trvn.Text.Contains("(Empty)"))
                        {
                            trvn.Text = fo.TagNumber.ToString();
                        }
                        
                        RefreshRowTagInfo(row);
                        
                    }
                    else if(fo.Recreated < 0)
                    {
                        textBox__tabFdToPerf_EditorValueMain.ForeColor = Color.Red;
                        trvn.Text = fo.TryCreateNumber + "(Empty)";
                        if ((row.Tag as List<FTag>).Count == 1)
                        {
                            RefreshRowTagInfo(row, fo.RecreatingError, 3);
                        }
                    }
                    else
                    {
                        // тут должны быть только пустые теги
                        LogHandle.ol("EMPTY TAG need to debug!!!!");
                    }
                    
                }
            }
            else if(sender == button_perfTabAppendTlvElem)
            {
                TreeNode trvn = treeView_tabPerfStlvMiniConstructor.SelectedNode;
                var rr = dataGridView_ftagListToPerform.SelectedCells;
                if (rr == null || rr.Count == 0)
                {
                    // если пользователю вздумается редактировать поле не выбрав значение таблицы
                    UpdateStatusArea("Невозможно добавить", "Выберите строку с реквизитом");
                    return;
                }

                int rInd = rr[0].RowIndex;
                var row = dataGridView_ftagListToPerform.Rows[rInd];
                int tagNumberRow = 0;
                int.TryParse(row.Cells[0].Value.ToString(), out tagNumberRow);


                int tagNumberSelected = 0;
                if (trvn != null)
                {
                    FTag trvnFtag = trvn.Tag as FTag;
                    if (trvnFtag.TagNumber > 0)
                    {
                        tagNumberSelected = trvnFtag.TagNumber;
                    }
                    if(tagNumberSelected == 0)
                    {
                        tagNumberSelected = trvnFtag.TryCreateNumber;
                    }
                }

                ContextMenu cm = new ContextMenu();
                MenuItem menuAddRoot = new MenuItem(tagNumberRow.ToString() + ": корневой элемент [суб]структуры", CommonConrolsEventHandler);
                menuAddRoot.Tag = tagNumberRow;
                int menuIndex = cm.MenuItems.Add(menuAddRoot);
                if (tagNumberSelected > 0 && FTag.typeMap.ContainsKey(tagNumberSelected) && FTag.typeMap[tagNumberSelected] == FTag.FDataType.STLV)
                {
                    // чтение правил и добавление меню
                    int ffdChosen = comboBox_performTabFfdToPerform.SelectedIndex+2;
                    int key = 65536 * ffdChosen + tagNumberSelected;
                    if (FTag.TFNCommonRules.ContainsKey(key))
                    {
                        foreach(var rule in FTag.TFNCommonRules[key].Rules)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append(rule.TagNumber.ToString());
                            if (FTag.fnsNames.ContainsKey(rule.TagNumber))
                            {
                                sb.Append('.');
                                sb.Append(FTag.fnsNames[rule.TagNumber]);
                                sb.Append(' ');
                                int subLenth = 40 - sb.Length;
                                if (FTag.userFrandlyNames.ContainsKey(rule.TagNumber))
                                {
                                    string s = FTag.userFrandlyNames[rule.TagNumber];
                                    if(subLenth > 0)
                                    {
                                        if (s.Length > subLenth)
                                        {
                                            sb.Append(s.Substring(0, subLenth));
                                            sb.Append("...");
                                        }
                                        else
                                        {
                                            sb.Append(s);
                                        }
                                    }
                                    
                                }
                            }
                            MenuItem menu = new MenuItem(sb.ToString(), CommonConrolsEventHandler);
                            menu.Tag = rule.TagNumber;
                            cm.MenuItems.Add(menu);
                        }
                    }
                }
                cm.Show(sender as Control, new Point(15, 15));
                
            }
            else if(sender == button_perfTabRemoveTlvElem)
            {
                TreeNode trvn = treeView_tabPerfStlvMiniConstructor.SelectedNode;
                var rr = dataGridView_ftagListToPerform.SelectedCells;
                if (rr == null || rr.Count == 0)
                {
                    // если пользователю вздумается редактировать поле не выбрав значение таблицы
                    UpdateStatusArea("Невозможно удалить", "Не получен индекс строки с реквизитом");
                    return;
                }

                int rInd = rr[0].RowIndex;
                var row = dataGridView_ftagListToPerform.Rows[rInd];
                if (trvn != null)
                {
                    if (trvn.Level > 0)
                    {
                        FTag ftagToRemove = trvn.Tag as FTag;
                        FTag parent = trvn.Parent.Tag as FTag;
                        if (parent.Type == FTag.FDataType.STLV)
                        {
                            bool p = parent.Nested.Remove(ftagToRemove);
                            if (p)
                                trvn.Remove();
                            else
                            {
                                UpdateStatusArea("Ошибка структуры.", "Дочерний тег не найден в каталоге родителя!!!!!!!!!!!!");
                            }
                        }
                        else
                        {
                            UpdateStatusArea("Ошибка структуры.", "Попытка удалить дочерний элемент из не STLV тега");
                        }
                    }
                    else // корневой тег [суб]структуры
                    {
                        if (treeView_tabPerfStlvMiniConstructor.Nodes.Count == 1)
                        {
                            // елиственный корневой тег - производим очистку данных
                            FTag rootFtag = trvn.Tag as FTag;
                            int rootTagNumber = rootFtag.TagNumber;
                            if(rootTagNumber == 0)
                            {
                                rootTagNumber = rootFtag.TryCreateNumber;
                            }

                            if (rootFtag.Type == FTag.FDataType.STLV)
                            {
                                rootFtag.Nested.Clear();
                                rootFtag.RebuildPrezentation();
                                trvn.Nodes.Clear();
                            }
                            else
                            {
                                rootFtag.ClearFTag();
                                trvn.Text = rootTagNumber + "(Empty)";
                            }
                        }
                        else
                        {
                            
                            
                            if (row.Tag is List<FTag>)
                            {
                                List<FTag> l = row.Tag as List<FTag>;
                                bool rm = l.Remove(trvn.Tag as FTag);
                                if (rm)
                                {
                                    treeView_tabPerfStlvMiniConstructor.Nodes.Remove(trvn);
                                }
                                else
                                {
                                    UpdateStatusArea("Невозможно удалить", "тег отсутвует в списке выбранного реквизита");
                                }
                            }
                            else
                            {
                                UpdateStatusArea("структурная ошибка", "отсутвует список тегов в строке реквизита");
                            }

                        }
                    }
                }
                //var rr = dataGridView_ftagListToPerform.SelectedCells;
                //if (rr == null || rr.Count == 0)
                //{
                //    // если пользователю вздумается редактировать поле не выбрав значение таблицы
                //    UpdateStatusArea("Невозможно добавить", "Выберите строку с реквизитом");
                //    return;
                //}
                //int rInd = rr[0].RowIndex;
                //var row = dataGridView_ftagListToPerform.Rows[rInd];
                RefreshRowTagInfo(row);
            }
            else if(sender is MenuItem)
            {
                //LogHandle.ol((sender as Control).Parent.ToString());


                var rr = dataGridView_ftagListToPerform.SelectedCells;
                if (rr == null || rr.Count == 0)
                {
                    // если пользователю вздумается редактировать поле не выбрав значение таблицы
                    UpdateStatusArea("Невозможно добавить", "Выберите строку с реквизитом");
                    return;
                }
                int rInd = rr[0].RowIndex;
                var row = dataGridView_ftagListToPerform.Rows[rInd];
                int tagNumberRow = 0;
                int.TryParse(row.Cells[0].Value.ToString(), out tagNumberRow);
                TreeNode trvn = treeView_tabPerfStlvMiniConstructor.SelectedNode;

                MenuItem mitem = (MenuItem)sender;
                Control c = mitem.Parent.GetContextMenu().SourceControl;
                if(c == button_perfTabAppendTlvElem)
                {
                    FTag fTagNew = null;
                    TreeNode nodeNew = null;
                    if (mitem.Index == 0)
                    {
                        // добавление корневого элемента
                        // желательно сделать проверку на доступность большого количества элементов
                        // один по идее всегда должен быть
                        List<FTag> l = row.Tag as List<FTag>;

                        if (FTag.typeMap.ContainsKey(tagNumberRow) && FTag.typeMap[tagNumberRow] == FTag.FDataType.STLV)
                        {
                            fTagNew = new FTag(tagNumberRow, new List<FTag>(), false);
                            nodeNew = treeView_tabPerfStlvMiniConstructor.Nodes.Add(tagNumberRow.ToString());
                        }
                        else
                        {
                            fTagNew = FTag.EmptyNumerredFtag(tagNumberRow);
                            nodeNew = treeView_tabPerfStlvMiniConstructor.Nodes.Add(tagNumberRow.ToString() + "(Empty)");
                        }
                        nodeNew.Tag = fTagNew;
                        if (row.Tag is List<FTag>)
                            (row.Tag as List<FTag>).Add(fTagNew);
                    }
                    else
                    {
                        if (trvn != null)
                        {
                            if (trvn.Tag != null && (trvn.Tag is FTag))
                            {
                                FTag tag = trvn.Tag as FTag;
                                if (tag.Type == FTag.FDataType.STLV)
                                {
                                    int tn = (int)mitem.Tag;
                                    if (FTag.typeMap.ContainsKey(tn) && FTag.typeMap[tn] == FTag.FDataType.STLV)
                                    {
                                        fTagNew = new FTag(tn, new List<FTag>(), false);
                                        (trvn.Tag as FTag).Nested.Add(fTagNew);
                                        (trvn.Tag as FTag).RebuildPrezentation();
                                        nodeNew = trvn.Nodes.Add(tn.ToString());
                                    }
                                    else
                                    {
                                        fTagNew = FTag.EmptyNumerredFtag(tn);
                                        (trvn.Tag as FTag).Nested.Add(fTagNew);
                                        (trvn.Tag as FTag).RebuildPrezentation();
                                        nodeNew = trvn.Nodes.Add(tn.ToString() + "(Empty)");
                                    }
                                    nodeNew.Tag = fTagNew;
                                }
                                else
                                {
                                    LogHandle.ol("Bad rules selected tag is not STLV and cant contain nested tags");
                                }
                            }
                            else
                            {
                                LogHandle.ol("Selected node hasnt ftag link");
                            }

                        }
                        else
                        {
                            LogHandle.ol("No selected node in TreeView");
                        }
                    }
                }

                
                RefreshRowTagInfo(row);
                
            }
            else if (sender == numericUpDown_fdNumber1 || sender == numericUpDown_fdNumber2)
            {
                lib.X1 = (int)numericUpDown_fdNumber1.Value;
                lib.X2 = (int)numericUpDown_fdNumber2.Value;
            }
            else if(sender == button_itemsEditor)
            {
                FormItemExtendedEditor itemEditor = new FormItemExtendedEditor();
                itemEditor.ShowDialog();
                var rr = dataGridView_ftagListToPerform.SelectedCells;
                if (rr == null || rr.Count == 0)
                {
                    // если пользователю вздумается редактировать поле не выбрав значение таблицы
                    UpdateStatusArea("Невозможно добавить", "Выберите строку с реквизитом");
                    return;
                }

                int rInd = rr[0].RowIndex;
                var row = dataGridView_ftagListToPerform.Rows[rInd];
                int tagNumberRow = 0;
                int.TryParse(row.Cells[0].Value.ToString(), out tagNumberRow);

                List<FTag> itemFtags = new List<FTag>();
                foreach(var item in itemEditor.CheqItems)
                {
                    itemFtags.AddRange(item.GetItemFtag(comboBox_performTabFfdToPerform.SelectedIndex+1));
                }
                row.Tag = itemFtags;
                MmapTreeTlvStruct(itemFtags);
            }
        }



        private int TagNumberChosen(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            int op = s.IndexOf('[')+1;
            int cl = s.IndexOf(']') ;
            return int.Parse(s.Substring(op, cl - op));
        }

        FTag.TFTagRuleSet ReadTableRules(int stlvType, int format)
        {
            FTag.TFTagRuleSet rule = new FTag.TFTagRuleSet(stlvType, format);
            foreach (DataGridViewRow row in dataGridView_termFdRules.Rows)
            {
                int tagNumber = -1;
                string dv = row.Cells[4].Value == null ? null : (string)row.Cells[4].Value;
                if (int.TryParse((string)row.Cells[0].Value, out tagNumber))
                    rule.AddRule(tagNumber, row.Cells[2].Value as string, row.Cells[3].Value as string, dv);
            }
            return rule;
        }
        void FillTFNRuleTable(FTag.TFTagRuleSet rule)
        {
            dataGridView_termFdRules.Rows.Clear();
            int rowInd = 0;
            foreach (var tag in rule.Rules)
            {
                int tagNumber = tag.TagNumber;
                string tagName = "";
                if (FTag.fnsNames.ContainsKey(tagNumber))
                {
                    tagName = FTag.fnsNames[tagNumber];
                }
                DataGridViewRow row = dataGridView_termFdRules.Rows[rowInd++];
                dataGridView_termFdRules.Rows.Add(tag.TagNumber.ToString(),tagName, tag.TagCriticalityUF, tag.DataSourceUF, tag.DefaultData);


            }

        }

        private void _AvailibleSetting(bool connected)
        {
            if (connected)
            {
                button_updateComNames.Enabled = false;
                comboBox_connsttsPortName.Enabled = false;
                comboBox_connsttsBaudrate.Enabled = false;
                textBox_connTimeout.Enabled = false;
                label_settsAvailabilityMsg.Text = "Для изменения настроек отключитесть от порта";
                button_xcgSend.Enabled = true;
            }
            else
            {
                button_updateComNames.Enabled = true;
                comboBox_connsttsPortName.Enabled = true;
                comboBox_connsttsBaudrate.Enabled = true;
                textBox_connTimeout.Enabled = true;
                label_settsAvailabilityMsg.Text = "";
                button_xcgSend.Enabled = false;
            }
        }

        public void UpdateComNanes()
        {
            comboBox_connsttsPortName.Items.Clear();
            foreach (string portName in SerialPort.GetPortNames()) comboBox_connsttsPortName.Items.Add(portName);
            if (comboBox_connsttsPortName.Items.Count > 0) comboBox_connsttsPortName.SelectedIndex = 0;
        }

        public string VisualLogUpdate
        {
            set
            {
                if (textBox_xcgRawData != null)
                {
                    if (InvokeRequired)
                        Invoke(new Action(() => _UpdateTextLog(value)));
                    else
                    {
                        _UpdateTextLog(value);
                    }
                }
                //}

            }
        }
        private void _UpdateTextLog(string s)
        {
            textBox_xcgRawData.Text = s;
            textBox_xcgRawData.SelectionStart = textBox_xcgRawData.Text.Length;
            textBox_xcgRawData.ScrollToCaret();
            //textBox_xcgRawData.AppendText(Environment.NewLine);
        }

        public void SimpleAddText(string s)
        {
            if (InvokeRequired)
                BeginInvoke(new Action(() => textBox_xcgRawData.AppendText(s)));
            else
                textBox_xcgRawData.AppendText(s);
        }



        public void UpdateStatusArea(string dlMsg, string dmMsg = null, double percent = -1)
        {
            if(!(string.IsNullOrEmpty(dlMsg) || dlMsg.StartsWith("Прочитан") ||!string.IsNullOrEmpty(dmMsg)))
                LogHandle.ol("TFN_IR: "+dlMsg+"\t\t"+dmMsg);
            if (InvokeRequired)
                Invoke(new Action(() => StatusAreaUpdate(dlMsg, dmMsg, percent)));
            else
                StatusAreaUpdate(dlMsg, dmMsg, percent);
        }
        private void StatusAreaUpdate(string dlMsg, string dmMsg, double percent)
        {
            if (string.IsNullOrEmpty(dlMsg))
                textBox_downMsgLeft.Text = "";
            else
                textBox_downMsgLeft.Text = dlMsg;
            if (string.IsNullOrEmpty(dmMsg))
                textBox_downMidleMsg.Text = "";
            else
                textBox_downMidleMsg.Text = dmMsg;
            if (percent < 0 && progressBar_downProgress.Value > 0)
            {
                progressBar_downProgress.Value = 0;
                progressBar_downProgress.Update();
            }
            else if (percent >= 0 && percent < 100 && progressBar_downProgress.Value != (int)Math.Round(percent))
            {
                progressBar_downProgress.Value = (int)Math.Round(percent);
                progressBar_downProgress.Update();
            }
            else if (100 == (int)Math.Round(percent) && progressBar_downProgress.Value < 100)
            {
                progressBar_downProgress.Value = (int)Math.Round(percent);
                progressBar_downProgress.PerformStep();
            }
        }


        private void ExampleFnRequest(object sender, EventArgs e)
        {
            if (sender == button_requestFnNumber)
            {
                lib.ReadFnNumberAsync(true);
            }
            else if(sender == button_sampleGetShiftParams)
            {
                lib.GetShiftParamsAsync(true);
            }
            else if (sender == button_fnGetStatus)
            {
                lib.GetFnStatusAsync(true);
            }
            else if (sender == button_fnInfoGetFfd)
            {
                lib.GetFfdAsync();
            }
            else if (sender == button_readFdTask)
            {
                treeView_fnReadedContent.Nodes.Clear();
                lib.X1 = (int)numericUpDown_fdNumber1.Value;
                lib.X2 = (int)numericUpDown_fdNumber2.Value;
                lib.ReadFdTlvList();

            }
            else if (sender == button_brakeOperation)
            {
                lib.StopFnExchange();
                textBox_downMidleMsg.Text = "Операция прервана";
            }
            else if (sender == button_saveFdToFile)
            {
                if (treeView_fnReadedContent.SelectedNode != null)
                {
                    if (treeView_fnReadedContent.SelectedNode.Tag != null)
                    {
                        try
                        {
                            //treeView_fnReadedContent.SelectedNode.Parent
                            string fileName;
                            if (treeView_fnReadedContent.SelectedNode.Parent != null)
                                fileName = treeView_fnReadedContent.SelectedNode.Parent.Text.Replace(":", "");
                            else
                                fileName = "Parsed_tlv_" + treeView_fnReadedContent.SelectedNode.Text.Replace(":", "");

                            string suffix;
                            if (comboBox_formatToSave.SelectedIndex == 0)
                                suffix = ".json";
                            else if (comboBox_formatToSave.SelectedIndex == 1)
                                suffix = ".tlv";
                            else
                                suffix = ".txt";
                            long i = 0;
                            if (File.Exists(fileName + suffix))
                            {
                                //i++;
                                //string s = fileName;
                                while (File.Exists(fileName + "(" + i + ")" + suffix))
                                {
                                    i++;
                                }
                                fileName = fileName + "(" + i + ")";
                            }


                            FTag f = (FTag)treeView_fnReadedContent.SelectedNode.Tag;
                            if (comboBox_formatToSave.SelectedIndex == 0)
                            {
                                FiscalPrinter.FnReadedDocument fd = new FiscalPrinter.FnReadedDocument();
                                fd.Tags = new List<FTag>();
                                fd.Tags.Add(f);
                                fileName += ".json";
                                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName), JsonChequeConstructor.CreateJsonString(fd));
                                UpdateStatusArea(fileName, "сохранен.");
                            }
                            else if (comboBox_formatToSave.SelectedIndex > 0)
                            {
                                List<Byte> tlvContent = new List<byte>();
                                tlvContent.Add((byte)(f.TagNumber % 256));
                                tlvContent.Add((byte)(f.TagNumber / 256));
                                tlvContent.Add((byte)(f.RawData.Length % 256));
                                tlvContent.Add((byte)(f.RawData.Length / 256));
                                tlvContent.AddRange(f.RawData);
                                if (comboBox_formatToSave.SelectedIndex == 1)
                                {
                                    fileName += ".tlv";
                                    File.WriteAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName), tlvContent.ToArray());
                                    UpdateStatusArea(fileName, "сохранен.");
                                }
                                else if (comboBox_formatToSave.SelectedIndex == 2)
                                {
                                    fileName += ".txt";
                                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName), BitConverter.ToString(tlvContent.ToArray()));
                                    UpdateStatusArea(fileName, "сохранен.");
                                }
                            }
                            else
                            {
                                UpdateStatusArea("Выберите формат");
                            }



                        }
                        catch (Exception ecx)
                        {
                            UpdateStatusArea("Ошибка при сохранении:", ecx.Message);
                        }
                    }
                    else
                    {
                        UpdateStatusArea("Ошибка при сохнанении", "Некорректная разметка данных");
                    }

                }
                else
                {
                    UpdateStatusArea("ФД не выбран");
                }
            }
            else if (sender == button_requestExpirationDate)
            {
                lib.GetFnExpirationAsync();
            }
            else if (sender == button_parceHexString)
            {

                List<FTag> fTags = new List<FTag>();
                try
                {
                    fTags.AddRange(FTag.FTLVParcer.ParseStructure(FTag.FTLVParcer.StringHexToBytes(textBox_strHexToParce.Text)));
                    if (fTags.Count > 0)
                    {
                        treeView_fnReadedContent.Nodes.Clear();
                        AddReadedFdList(fTags);
                    }
                }
                catch (Exception exc)
                {
                    UpdateStatusArea("Exception occure", exc.Message);
                }

            }
            else if (sender == button_getRegistrationParams)
            {
                lib.GetRegistrationParamsAsync();
            }
            else if (sender == button_getFnExchangeStatus)
            {
                lib.GetFnExchangeAsync();
            }
            else if (sender == button_readFdVariant2)
            {

                treeView_fnReadedContent.Nodes.Clear();
                lib.X1 = (int)numericUpDown_fdNumber1.Value;
                lib.X2 = (int)numericUpDown_fdNumber2.Value;
                lib.ReadArchiveAsync(checkBox_getExtraTlvInfo.Checked);

            }
            else if (sender == button_getFullFnInfo || sender == button_getFullFnInfo2)
            {

                if (!checkBox_xcgOpenPort.Checked)
                {
                    checkBox_xcgOpenPort.Checked = true;
                }
                if (checkBox_xcgOpenPort.Checked)
                {
                    this.Enabled = false;
                    CleanFnInfo();
                    ExampleGetFnInformation();
                }
                else
                {
                    UpdateStatusArea("Не удалось открыть порт", "", 0);
                }


            }
            else if (sender == button_perfTabBeginDocument)
            {
                int doc = TagNumberChosen(comboBox_performTabDocType.Text);
                // возможно для некоторых форм потребуются доп. дааные поэтому пока так
                lib.BeginDocument(doc, true);
            }
            else if (sender == button_perfTabFinishDocument)
            {
                int doc = TagNumberChosen(comboBox_performTabDocType.Text);
                if (doc == FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_SHIFT || doc == FTAG_FISCAL_DOCUMENT_TYPE_OPEN_SHIFT||doc == FTAG_FISCAL_DOCUMENT_TYPE_CALCULATION_REPORT)
                    lib.PerformDocument(doc, null, true);
            }
            else if (sender == button_perfTabSendSingleRow)
            {
                DataGridViewRow dataGridViewRow = null;
                foreach (DataGridViewRow row in dataGridView_ftagListToPerform.Rows)
                {
                    if (row.Cells[IND_CELL_PROCESSING_STATUS].Tag == null && row.Tag != null && row.Tag is List<FTag>)
                    {
                        dataGridViewRow = row;
                        break;
                    }
                }
                if (dataGridViewRow != null)
                {
                    List<FTag> list = dataGridViewRow.Tag as List<FTag>;
                    int tagNumber = 0;
                    int.TryParse(dataGridViewRow.Cells[IND_CELL_TAG_NUMBER].Value.ToString(), out tagNumber);
                    if (tagNumber != 0)
                    {
                        lib.SendTlvData(list, true, dataGridViewRow);
                    }
                    else
                    {
                        UpdateStatusArea("Внутренняя ошибка", "Формат данных не соответсвует вызываемому методу", 0);
                    }

                }

            }
            else if (sender == button_perfTabCancelFd)
            {
                lib.AbortDocument(true);

            }
            else if (sender == button_sendAllRows)
            {
                new Thread(() =>
                {
                    int empty = 0;
                    int bad = 0;
                    foreach (DataGridViewRow row in dataGridView_ftagListToPerform.Rows)
                    {
                        if (row.Tag == null || !(row.Tag is List<FTag>) || (row.Tag as List<FTag>).Count == 0)
                        {
                            empty++;
                            continue;
                        }
                        int rez = lib.SendTlvData(row.Tag as List<FTag>, true, row).Result;

                        if (rez != 0)
                            bad++;
                    }

                    UpdateStatusArea("Строк передано с тегами " + dataGridView_ftagListToPerform.Rows.Count, "пустых " + empty + "; ошибок: " + bad);

                }).Start();

            }
            else if (sender == button__performTabPerformFast)
            {
                foreach (DataGridViewRow r in dataGridView_ftagListToPerform.Rows)
                {
                    r.Cells[IND_CELL_PROCESSING_STATUS].Tag = null;
                }

                int doc = TagNumberChosen(comboBox_performTabDocType.Text);
                int key = (comboBox_performTabFfdToPerform.SelectedIndex + 2) * 65536 + doc;
                object[] dataForClose = textBox_perfTabDataForClosing.Tag == null ? null : textBox_perfTabDataForClosing.Tag as object[];
                new Thread(() =>
                {
                    if (
                    doc == FTAG_FISCAL_DOCUMENT_TYPE_OPEN_SHIFT ||
                    doc == FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_SHIFT ||
                    doc == FTAG_FISCAL_DOCUMENT_TYPE_CALCULATION_REPORT
                    )
                    {
                        int operation = lib.BeginDocument(doc, false).Result;
                        if (operation == 0)
                        {
                            int ftagNumber = 0;
                            // ОК - продолжаем 
                            FTag.TFTagRuleSet ruleset = FTag.TFNCommonRules[key];

                            //bool criticalBrake = false;
                            List<int> notAcceptedFtags = new List<int>();
                            foreach (DataGridViewRow row in dataGridView_ftagListToPerform.Rows)
                            {

                                ftagNumber = int.Parse(row.Cells[0].Value.ToString());
                                if (ftagNumber != FTAG_ITEM)
                                {
                                    operation = lib.SendTlvData(row.Tag as List<FTag>, true, row).Result;
                                    if (operation != 0 && operation != INTERRUPTED)
                                    {
                                        if (ruleset.FtagInfo(ftagNumber).Critical)
                                        {
                                            string fnAnswer = "";
                                            if (FnAnswerCode.ContainsKey(operation))
                                                fnAnswer = "[" + operation + "] " + FnAnswerCode[operation];
                                            else
                                                fnAnswer = "Код ответа ФН: " + operation;
                                            UpdateStatusArea("Отказ в принятиитии критичного тега " + FTag.userFrandlyNames[doc], fnAnswer);
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (FTag item in row.Tag as List<FTag>)
                                    {
                                        List<FTag> l = new List<FTag>();
                                        operation = lib.SendTlvData(row.Tag as List<FTag>, true, null).Result;
                                        if (operation != OK || operation != INTERRUPTED)
                                        {
                                            row.Cells[IND_CELL_PROCESSING_STATUS].Tag = new object[] { "Не принят предмет расчета" };
                                            string fnAnswer = "";
                                            if (FnAnswerCode.ContainsKey(operation))
                                                fnAnswer = "[" + operation + "] " + FnAnswerCode[operation];
                                            else
                                                fnAnswer = "Код ответа ФН: " + operation;
                                            UpdateStatusArea("Отказ в принятии предмета расчета " + FTag.userFrandlyNames[doc], fnAnswer);

                                            return;
                                        }

                                    }

                                }
                            }

                            operation = lib.PerformDocument(doc, dataForClose, true).Result;

                        }
                        else
                        {

                            string fnAnswer = "";
                            if (FnAnswerCode.ContainsKey(operation))
                                fnAnswer = "[" + operation + "] " + FnAnswerCode[operation];
                            else
                                fnAnswer = "Код ответа ФН: " + operation;
                            UpdateStatusArea("Неудачная попытка открыть " + FTag.userFrandlyNames[doc], fnAnswer);

                        }
                    }

                }).Start();
            }


        }


       

        async void ExampleGetFnInformation()
        {

            await lib.ReadFnNumberAsync();
            long fnNum = -1;
            long.TryParse(lib.GetFnInfoParam(FTAG_FN_NUMBER).Trim(), out fnNum);
            if (fnNum>0)
            {
                Thread.Sleep(80);

                textBox_fnBriefInfo.Text = lib.GetFnInfoParam(FTAG_FN_NUMBER);
                await lib.GetFnStatusAsync();

                if (lib.StageOfUsage>0)
                {
                    if (lib.LastFd > 0)
                    {
                        await lib.GetFnExchangeAsync();

                        await lib.GetRegistrationParamsAsync();
                    }

                    textBox_fnBriefInfo.AppendText("\t"+lib.GetFnInfoParam(FNDESCR_STAGE_OF_APPLICATION)
                        + Environment.NewLine+ lib.GetFnInfoParam(FNDESCR_SHIFT_STATE)
                        + "\t\t" + lib.GetFnInfoParam(FNDESCR_OPENED_DOC));
                    

                    UpdateStatusArea("", "");

                }
                else
                {
                    UpdateStatusArea("Не удалось получить статус ФН", "Попробуйте повторить запрос");

                }
            }
            else
            {
                UpdateStatusArea( "Не удалось получить номер ФН");
            }
            if (InvokeRequired)
                Invoke(new Action(() => this.Enabled = true));
            else
                this.Enabled = true;
            

        }

        byte[] BytesFromImage(string s)
        {
            try
            {
                int dataSize = s.Length / 3 + (s.Length % 3 > 0 ? 1 : 0);
                byte[] dataBtes = new byte[dataSize];
                for (int i = 0; i < s.Length; i += 3)
                {
                    dataBtes[i / 3] = (byte)(16 * (char.IsDigit(s[i]) ? (s[i] - '0') : (10 + s[i] - 'A')) + (char.IsDigit(s[i + 1]) ? (s[i + 1] - '0') : (10 + s[i + 1] - 'A')));
                }
                return dataBtes;
            }
            catch
            {
                return new byte[0];
            }

        }


        /*
         *  Обновление информации в интерфейсе 
         *  Если вызвана без параметров обноляется вся панель
         *  если с 
         */

        public void UpdateInfoPanel(List<int> listParams = null, string s = null)
        {
            if (this.InvokeRequired)
                Invoke(new Action(() =>
                {
                    UpdateUiInfo(listParams);
                }));
            else
            {
                UpdateUiInfo(listParams);
            }

        }
        private void UpdateUiInfo(List<int> listParams = null, string s = null)
        {

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Заводской номер ФН:\t\t\t" + lib.GetFnInfoParam(FTAG_FN_NUMBER));
                sb.AppendLine("Этап применения:\t\t\t\t"+ lib.GetFnInfoParam(FNDESCR_STAGE_OF_APPLICATION));
                sb.AppendLine("Версия ФФД:\t\t\t\t"+ lib.GetFnInfoParam(FTAG_FFD));
                
                sb.AppendLine("Открытый документ:\t\t\t"+ lib.GetFnInfoParam(FNDESCR_OPENED_DOC));
                sb.AppendLine("Состояние смены:\t\t\t\t"+ lib.GetFnInfoParam(FNDESCR_SHIFT_STATE));
                sb.AppendLine("Флаги предупреждений:\t\t\t"+ lib.GetFnInfoParam(FNDESCR_ALARMING_FLAGS));
                sb.AppendLine("Время последнего ФД:\t\t\t"+ lib.GetFnInfoParam(FNDESCR_LAST_PERFORMED_FD_TIME));
                sb.AppendLine("Номер последнего ФД:\t\t\t"+ lib.GetFnInfoParam(FNDESCR_LAST_PERFORMED_FD_NUMBER));
                
                sb.AppendLine("Срок действия ключа фискального признака:\t"+ lib.GetFnInfoParam(FNDESCR_EXPIRATION_DATE));
                sb.AppendLine("Регистраций выполнено:\t\t\t" + lib.GetFnInfoParam(FNDESCR_EXPIRATION_PERFORMED_REGS));
                sb.AppendLine("Регистраций осталось:\t\t\t" + lib.GetFnInfoParam(FNDESCR_EXPIRATION_AVAILABLE_REGS));
                sb.AppendLine("Статус обмена с ОФД:\t\t\t" + lib.GetFnInfoParam(FNDESCR_OFD_EXCHANGE_STATUS) + " " + lib.GetFnInfoParam(FNDESCR_OFD_EXCHANGE_STATUS_EXT));
                sb.AppendLine("Производится чтение:\t\t\t" + lib.GetFnInfoParam(FNDESCR_OFD_EXCHANGE_ACTIVE));
                sb.AppendLine("Непереданных ФД:\t\t\t" + lib.GetFnInfoParam(FNDESCR_OFD_EXCHANGE_UNSENT_DOCS));
                sb.AppendLine("Номер первого непереданного ФД:\t\t" + lib.GetFnInfoParam(FNDESCR_OFD_EXCHANGE_UNSENT_FIRST_DOC_NUM)+ " от: " + lib.GetFnInfoParam(FNDESCR_OFD_EXCHANGE_UNSENT_FIRST_DOC_TIME));
                richTextBox_fnTechCondition.Text = sb.ToString();
                if (lib.LastFd > 0)
                {
                    if (numericUpDown_fdNumber2.Value == 0 && numericUpDown_fdNumber1.Value == 0)
                    {
                        numericUpDown_fdNumber2.Value = lib.LastFd;
                        numericUpDown_fdNumber1.Value = lib.LastFd - 100 > 0 ? lib.LastFd - 100 : 1;
                    }
                    //numericUpDown_fdNumber2.Value = lib.LastFd;
                    if (numericUpDown_fdNumber1.Value == 0)
                    {
                        numericUpDown_fdNumber1.Value = 1;
                    }
                    if (numericUpDown_fdNumber2.Value == 0)
                    {
                        numericUpDown_fdNumber2.Value = 1;
                    }
                }
                comboBox_performTabFfdToPerform.SelectedIndex = lib.FfdFtagFormat <= 0 ? 0 : lib.FfdFtagFormat - 2;
            

        }



        public void CleanFnInfo()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    richTextBox_fnTechCondition.Text = "";
                    dataGridView_regParams.Rows.Clear();
                }));
            }
            else
            {
                richTextBox_fnTechCondition.Text = "";
                dataGridView_regParams.Rows.Clear();
            }
            
        }

        //private List<FTag> fnContent = new List<FTag>();

        public void ReadedFdToPanelFT(List<FTag> fdList)
        {
            if (InvokeRequired)
                Invoke(new Action(() => AddReadedFdList(fdList)));
            else
                AddReadedFdList(fdList);
        }

        public void AddReadedFdList(List<FTag> fdList)
        {

            treeView_fnReadedContent.BeginUpdate();
            foreach (var ftag in fdList)
            {
                TreeNode tn = new TreeNode();
                tn.Tag = ftag;
                tn.Text = ftag.ToString(checkBox_preferUserFrandly.Checked ? "UF" : null);
                tn.ToolTipText = ftag.Representation;
                if (ftag.Type == FTag.FDataType.STLV)
                {
                    MapBranches(ftag, ref tn);
                }
                treeView_fnReadedContent.Nodes.Add(tn);

            }
            treeView_fnReadedContent.EndUpdate();
        }

        public void AddFdToTree(FnReadedDocument fd)
        {
            if (!fd.Equals(FnReadedDocument.EmptyFD))
            {
                treeView_fnReadedContent.BeginUpdate();
                TreeNode tn = new TreeNode();
                StringBuilder sb = new StringBuilder("ФД:"+fd.Number.ToString());
                if(fd.Number>=10000)
                    sb.Append(" ");
                else if (fd.Number >= 1000)
                    sb.Append("   ");
                else if (fd.Number >= 100)
                    sb.Append("    ");
                else if (fd.Number >= 10)
                    sb.Append("     ");
                else
                {
                    sb.Append("       ");
                }
                sb.Append(fd.Time.ToString("dd.MM.yy HH:mm  "));
                
                sb.Append((fdDocTypes.ContainsKey(fd.Type) ? fdDocTypes[fd.Type] : "код документа(" + fd.Type + ")"));
                if (fd.OperationTypeInfo > 0)
                {
                    sb.Append(' ');
                    sb.Append(FiscalOperationType[fd.OperationTypeInfo]);
                    sb.Append(' ');
                    sb.Append(fd.Summ.ToString("C2"));
                }

                if (fd.OfdSignInfo == 1)
                {
                    sb.Append(" не отправлен");
                }
                else if (fd.OfdSignInfo == 2)
                {
                    sb.Append(" отправлен");
                }
                tn.Text = sb.ToString();//fd.Number + " " + (fdDocTypes.ContainsKey(fd.Type)? fdDocTypes[fd.Type]:"код документа("+fd.Type+")")  +


                //fd.ReeprezentOL;
                TreeNode tnFdNum = new TreeNode();
                tnFdNum.Text = "ФД: " + fd.Number;
                tnFdNum.Tag = fd.Number;
                tn.Nodes.Add(tnFdNum);

                TreeNode tnType = new TreeNode();
                tnType.Text = fdDocTypes[fd.Type];
                tnType.Tag = fd.Type;
                tn.Nodes.Add(tnType);

                //TreeNode tnFdNumber = new TreeNode();
                //tnFdNumber.Text = fd.Number.ToString();
                //tnFdNumber.Tag = fd.Number;
                //tn.Nodes.Add(tnType);

                TreeNode tnTime = new TreeNode();
                tnTime.Text = fd.Time.ToString("dd.MM.yyyy HH:mm");
                tnTime.Tag = fd.Time;
                tn.Nodes.Add(tnTime);

                if (fd.Type == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE
                    || fd.Type == FTAG_FISCAL_DOCUMENT_TYPE_BSO
                    || fd.Type == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE
                    || fd.Type == FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION)
                {
                    TreeNode tnSumm = new TreeNode();
                    tnSumm.Text = "Итог: " + fd.Summ.ToString();
                    tnSumm.Tag = fd.Summ;
                    tn.Nodes.Add(tnSumm);

                    TreeNode tnOpType = new TreeNode();
                    tnOpType.Text = FiscalOperationType[fd.OperationTypeInfo];
                    tnOpType.Tag = fd.OperationTypeInfo;
                    tn.Nodes.Add(tnOpType);
                }
                TreeNode tnFiscalSign = new TreeNode();
                tnFiscalSign.Text = "ФП: " + fd.FiscalSign;
                tnFiscalSign.Tag = fd.FiscalSign;
                tn.Nodes.Add(tnFiscalSign);

                if (fd.OfdSignInfo > 0)
                {
                    TreeNode tnOfd = new TreeNode();
                    tnOfd.Text = fd.OfdSignInfo == 1 ? "Не отправлен в ОФД" : "Отправлен в ОФД";
                    tnOfd.Tag = fd.OfdSignInfo - 1;
                    tn.Nodes.Add(tnOfd);
                }
                if (fd.Tags != null && fd.Tags.Count > 0)
                {
                    TreeNode tnFtags = new TreeNode();
                    tnFtags.Text = "TLV структура ФД";
                    if (fd.Tags.Count == 1)
                        tnFtags.Tag = fd.Tags[0];
                    foreach (var ft in fd.Tags)
                    {
                        MapBranches(ft, ref tnFtags);
                    }
                    tn.Nodes.Add(tnFtags);
                }
                treeView_fnReadedContent.Nodes.Add(tn);
                treeView_fnReadedContent.EndUpdate();
            }
        }


        private void MapBranches(FTag ftag, ref TreeNode tn)
        {
            if (ftag.Nested != null)
            {
                //int i = 0;
                foreach (var ftag2 in ftag.Nested)
                {
                    string apendix = "";
                    if(ftag2.TagNumber == 1059&&ftag2.Nested!=null&&ftag2.Nested.Count>0)
                    {
                        string name = "";
                        string sum = "";
                        foreach (var ftag3 in ftag2.Nested)
                        {
                            if (ftag3.TagNumber == FTAG_ITEM_NAME)
                                name = ftag3.ValueStr;
                            else if(ftag3.TagNumber == FTAG_ITEM_SUM)
                                sum = ftag3.ValueDouble.ToString("F2")+'\u20bd';
                        }
                        if (!string.IsNullOrEmpty(name))
                        {
                            if (name.Length > 20)
                                apendix = " "+name.Substring(0, 19)+"...";
                            else
                                apendix = " " + name;
                            apendix += " ";
                        }
                        apendix += sum;
                    }

                    TreeNode tn2 = new TreeNode(ftag2.ToString(checkBox_preferUserFrandly.Checked ? "UF" : null)+apendix);
                    tn2.Tag = ftag2;
                    //tn2.Text = ftag2.ToString();
                    if (ftag2.Type == FTag.FDataType.STLV)
                        MapBranches(ftag2, ref tn2);
                    tn.Nodes.Add(tn2);
                }
            }
        }

        private void TreeView_fnReadedContent_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (treeView_fnReadedContent.SelectedNode.Tag != null)
                {
                    if (treeView_fnReadedContent.SelectedNode.Tag is FTag)
                    {
                        FTag f = (FTag)treeView_fnReadedContent.SelectedNode.Tag;
                        button_saveFdToFile.Enabled = f.TagNumber < 100;
                        textBox_tagInfoTagNumber.Text = f.TagNumber.ToString();
                        textBox_tagInfoTagDataType.Text = f.Type.ToString();
                        textBox_tagInfoFnsName.Text = FTag.fnsNames.ContainsKey(f.TagNumber) ? FTag.fnsNames[f.TagNumber] : "??";
                        textBox_tagInfoUserFriendlyName.Text = FTag.userFrandlyNames.ContainsKey(f.TagNumber) ? FTag.userFrandlyNames[f.TagNumber] : "Нет в словаре";
                        textBox_tagInfoStringPresentation.Text = f.Representation;
                        if (f.Type == FTag.FDataType.Bit_MASK
                            || f.Type == FTag.FDataType.BYTE
                            || f.Type == FTag.FDataType.FVLN
                            || f.Type == FTag.FDataType.NUMBER
                            || f.Type == FTag.FDataType.Uint16
                            || f.Type == FTag.FDataType.Uint32
                            || f.Type == FTag.FDataType.U32UT
                            || f.Type == FTag.FDataType.VLN
                            || f.Type == FTag.FDataType.Byte_ARRAY)
                        {
                            if (f.Type == FTag.FDataType.Byte_ARRAY)
                            {
                                if (f.TagNumber == FiscalPrinter.FTAG_DOC_FISCAL_SIGN)
                                    textBox_tagInfoTagNumericValue.Text = f.ValueInt.ToString();
                                else
                                    textBox_tagInfoTagNumericValue.Text = "";
                            }
                            else if (f.Type == FTag.FDataType.U32UT)
                            {
                                if (f.ValueInt > 0)
                                    textBox_tagInfoTagNumericValue.Text = f.ValueInt.ToString();
                                else if (f.ValueLong > 0)
                                {
                                    textBox_tagInfoTagNumericValue.Text = f.ValueLong.ToString();
                                }
                                else
                                    textBox_tagInfoTagNumericValue.Text = "";
                            }
                            else if (f.Type == FTag.FDataType.FVLN)
                            {
                                textBox_tagInfoTagNumericValue.Text = f.ValueDouble.ToString();
                            }
                            else
                            {
                                //LogHandle.ol("Проверить необходимость этой ветки");
                                if ( f.ValueLong != 0)
                                {
                                    textBox_tagInfoTagNumericValue.Text = f.ValueLong.ToString();
                                }
                                else
                                    textBox_tagInfoTagNumericValue.Text = f.ValueInt.ToString();
                            }

                        }
                        else
                        {
                            textBox_tagInfoTagNumericValue.Text = "";
                        }
                        if (f.RawData != null)
                        {
                            textBox_tagInfoRawData.Text = "Размер данных(bytes) " + f.RawData.Length
                                + Environment.NewLine + "[" + BitConverter.ToString(f.RawData) + "]";
                        }
                        else
                        {
                            textBox_tagInfoRawData.Text = "";
                        }
                    }
                    else
                    {
                        textBox_tagInfoRawData.Text = "";
                        textBox_tagInfoTagNumericValue.Text = "";
                        button_saveFdToFile.Enabled = false;
                        textBox_tagInfoTagNumber.Text = "";
                        textBox_tagInfoTagDataType.Text = "";
                        textBox_tagInfoFnsName.Text = "";
                        textBox_tagInfoUserFriendlyName.Text = "";
                        if (treeView_fnReadedContent.SelectedNode.Tag is DateTime)
                        {
                            textBox_tagInfoStringPresentation.Text = ((DateTime)treeView_fnReadedContent.SelectedNode.Tag).ToString("dd.MM.yyyy HH:mm");
                        }
                        else
                        {
                            textBox_tagInfoStringPresentation.Text = treeView_fnReadedContent.SelectedNode.Tag.ToString();
                        }

                    }

                }
                else
                {
                    textBox_tagInfoRawData.Text = "";
                    textBox_tagInfoTagNumericValue.Text = "";
                    textBox_tagInfoStringPresentation.Text = "";
                    button_saveFdToFile.Enabled = false;
                    textBox_tagInfoTagNumber.Text = "";
                    textBox_tagInfoTagDataType.Text = "";
                    textBox_tagInfoFnsName.Text = "";
                    textBox_tagInfoUserFriendlyName.Text = "";
                }

            }
            catch (Exception ex)
            {
                LogHandle.olta(ex.Message);
            }
        }


        public void Disconnect()
        {
            if (checkBox_connectConnectionParamsTab.Checked)
            {
                if (InvokeRequired)
                    Invoke(new Action(() => { checkBox_connectConnectionParamsTab.Checked = false; }));
                else
                    checkBox_connectConnectionParamsTab.Checked = false;
            }

        }

        internal void AddFdToTv(FnReadedDocument fd)
        {
            if (InvokeRequired)
                Invoke(new Action(() => AddFdToTree(fd)));
            else
                AddFdToTree(fd);
        }

        int _lastTimeSource = 0;
        bool _skipTimeSetEvent = false;
        public bool SkipUiTimeSetEvent
        {
            set => _skipTimeSetEvent = value;
            get => _skipTimeSetEvent;
        }
        private void TimeSettingChanging(object sender, EventArgs e)
        {
            if (_skipTimeSetEvent)
                return;

            //LogHandle.ol(sender.ToString());
            int ts = tabControl_timeModuleMainSwitcher.SelectedIndex;
            // смена вкладки останавливаем смещенное время
            if (ts != _lastTimeSource) 
            {
                _skipTimeSetEvent = true;
                radioButton_timerStop1.Checked = true;
                radioButton_timerStop2.Checked = true;
                lib.StopTimer();
                _lastTimeSource = ts;
                
                //_skipConnectionEvent = false;
            }
            // вкладка остается прежней
            else 
            {
                // radio обрабатываем только на активной вкладке и включееные, отключенные и сдругой вкладки игнорируем 
                if(sender == radioButton_timerStart1 && radioButton_timerStart1.Checked && ts == 1)
                {
                    lib.LaunchTimer();
                }
                else if (sender == radioButton_timerStart2 && radioButton_timerStart2.Checked && ts == 2)
                {
                    lib.LaunchTimer();
                }
                else if(sender == radioButton_timerStop1 && radioButton_timerStop1.Checked && ts == 1)
                {
                    lib.StopTimer();
                }
                else if (sender == radioButton_timerStop2 && radioButton_timerStop2.Checked && ts == 2)
                {
                    lib.StopTimer();
                }
                else if(sender == numericUpDown_plusMin || sender == dateTimePicker_setDtForFd)
                {
                    _skipTimeSetEvent = true;
                    radioButton_timerStop1.Checked = true;
                    radioButton_timerStop2.Checked = true;
                    lib.StopTimer();
                    
                }
            }
            _skipTimeSetEvent = false;
            lib.ChangeDtSetting(ts, dateTimePicker_setDtForFd.Value, (int)numericUpDown_plusMin.Value);
            //LogHandle.ol(ts.ToString());
        }

        private void StopUpdateTime(object sender, FormClosingEventArgs e)
        {
            ExampleFnRequest(button_brakeOperation, e);
            timer.Stop();
            timer.Dispose();
        }

        private void tabControl_timeModuleMainSwitcher_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //LogHandle.olta("tabControl_timeModuleMainSwitcher_Selecting");
            int ts = tabControl_timeModuleMainSwitcher.SelectedIndex;
            DateTime dt = dateTimePicker_setDtForFd.Value;
            int shift = (int)numericUpDown_plusMin.Value;
        }

        int sizeIncreaze = 0;
        public void FillRegTable()
        {
            List<int> keys = new List<int>(lib.FnInfo.Keys);
            
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    dataGridView_regParams.Rows.Clear();
                    foreach (var k in lib.RegFTags.Keys)
                    {
                        var f = lib.RegFTags[k];
                        string tagName;
                        if (FTag.userFrandlyNames.ContainsKey(f.TagNumber))
                            tagName = FTag.userFrandlyNames[f.TagNumber];
                        else
                            tagName = "Нет названия тега в словаре";
                        dataGridView_regParams.Rows.Add(f.TagNumber, tagName, f.Representation);
                    }
                    this.Width = this.Width + (sizeIncreaze++ % 2 == 1 ? 1 : -1);
                    this.Height = this.Height - (sizeIncreaze % 2 == 1 ? 1 : -1);
                }));
            }
            else
            {
                dataGridView_regParams.Rows.Clear();
                foreach (var k in lib.RegFTags.Keys)
                {
                    var f = lib.RegFTags[k];
                    string tagName;
                    if (FTag.userFrandlyNames.ContainsKey(f.TagNumber))
                        tagName = FTag.userFrandlyNames[f.TagNumber];
                    else
                        tagName = "Нет названия тега в словаре";
                    dataGridView_regParams.Rows.Add(f.TagNumber, tagName, f.Representation);
                }

                this.Width = this.Width + (sizeIncreaze++ % 2 == 1 ? 1 : -1);
                this.Height = this.Height + (sizeIncreaze % 2 == 1 ? 1 : -1);

            }

        }


        bool _textBox__tabFdToPerf_EditorValueMain_skipper = false;
        private void FdPerfGridRowSelect(object sender, DataGridViewCellEventArgs e)
        {

            int rowInd = e.RowIndex;
            if (rowInd >= 0)
            {
                if (dataGridView_ftagListToPerform.Rows[rowInd].Tag!=null && dataGridView_ftagListToPerform.Rows[rowInd].Tag is List<FTag>)
                {
                    int tn = 0;
                    int.TryParse(dataGridView_ftagListToPerform.Rows[rowInd].Cells[0].Value.ToString(), out tn);
                    MmapTreeTlvStruct(dataGridView_ftagListToPerform.Rows[rowInd].Tag as List<FTag>);
                    button_itemsEditor.Enabled = tn == FTAG_ITEM;
                }
                else
                {
                    UpdateStatusArea("ошибка!", "Список тегов пуст или передан в некорректном формате", 0);
                }
            }
            else
            {
                UpdateStatusArea("ошибка!", "Не удалось получить номер выбранной строки", 0);
            }
            //_textBox__tabFdToPerf_EditorValueMain_skipper = false;
        }

        

        private void Stlv_miniConstructorSelecttag(object sender, TreeViewEventArgs e)
        {
            TreeNode trvn = treeView_tabPerfStlvMiniConstructor.SelectedNode;

            FTag f = (FTag)trvn.Tag;
            int tagNumber = f.TagNumber;
            if(tagNumber == 0)
            {
                tagNumber = f.TryCreateNumber;
            }
            if(tagNumber == 0)
            {
                // пустой тэг
                int.TryParse(trvn.Text.Substring(0,trvn.Text.IndexOf(')')), out tagNumber);
            }

            if (FTag.userFrandlyNames.ContainsKey(tagNumber))
            {
                textBox_perfTab_ftagUFName.Text = FTag.userFrandlyNames[tagNumber];
            }
            else if (FTag.fnsNames.ContainsKey(tagNumber))
            {
                textBox_perfTab_ftagUFName.Text = FTag.fnsNames[tagNumber];
            }
            else
            {
                textBox_perfTab_ftagUFName.Text = "???";
            }
            bool vlnType = false;
            if (FTag.typeMap.ContainsKey(tagNumber))
            {
                FTag.FDataType type = FTag.typeMap[tagNumber];
                
                if (type == FTag.FDataType.VLN)
                {
                    vlnType = true;
                    textBox_perfTabSelectedTNType.Text = type.ToString()+" (копейки)";
                }
                else
                {
                    textBox_perfTabSelectedTNType.Text = type.ToString();
                }   
            }
            else
            {
                textBox_perfTabSelectedTNType.Text = FTag.typeMap[0].ToString();
            }
            textBox_tabFdToPerf_EditorNumberMain.Text = tagNumber.ToString();
            textBox__tabFdToPerf_EditorValueMain.Tag = null;// отключаем обработку редактирования тега
            if (vlnType)
            {
                textBox__tabFdToPerf_EditorValueMain.Text = f.ValueLong.ToString();
            }
            else
            { textBox__tabFdToPerf_EditorValueMain.Text = f.Representation; }
            textBox__tabFdToPerf_EditorValueMain.Tag = f;
        }


        // указатели на индекс ячеек
        public const int
            IND_CELL_TAG_NUMBER = 0,
            IND_CELL_TAG_NAME = 1,
            IND_CELL_TAG_UF_NAME = 2,
            IND_CELL_TAG_PRESENTATION_STR = 3,
            IND_CELL_PROCESSING_STATUS = 4;
        // row.Cells[IND_CELL_PROCESSING_STATUS]
        // данный столбец будет содержать object[] 
        // в массиве будут либо коды ответов ФН, либо string с сообщением
        // если массив - нал или не содержит элементов считаем что строка не передавалась в ФН
        // если передавать нечеого при обработре заполняем object[]{"Нет данных для епредачи"} или что-то подобное
        // если тег пустой возможно стоит заполнить ошибкой создания тега FTag.RecreatingError
        // если данных несколько значение строки заполняется через ';'


        /*
         * status доп. сообщение: обработки строки, некорректного создания тега или реация ФНа
         * statusSign применяется только если status не пустой
         * statusSign : 0 - Black, 1 - Gray, 2 - Green, 3 - Red
         */
        public void RefreshRowTagInfoExternul(DataGridViewRow row, string status = null, int statusSign = 0)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => RefreshRowTagInfo( row, status , statusSign)));
            }
            else
            {
                RefreshRowTagInfo(row, status, statusSign);
            }
        }

        void RefreshRowTagInfo(DataGridViewRow row, string status = null, int statusSign = 0)
        {
            if (row == null)
            {
                UpdateStatusArea("!!!!! Ошибочный вызов метода !!!!! ", "Передан NULL");
                return;
            }


            if (row.Tag != null && row.Tag is List<FTag>)
                {
                    List<FTag> list = (List<FTag>)row.Tag;
                    int tn = 0;
                    int.TryParse(row.Cells[IND_CELL_TAG_NUMBER].Value.ToString(), out tn);
                    row.Cells[IND_CELL_TAG_NUMBER].Style.ForeColor = Color.Black;
                    row.Cells[IND_CELL_TAG_NAME].Style.ForeColor = Color.Black;
                    row.Cells[IND_CELL_TAG_PRESENTATION_STR].Style.ForeColor = Color.Black;
                    row.Cells[IND_CELL_PROCESSING_STATUS].Style.ForeColor = Color.Black;
                    if (FTag.fnsNames.ContainsKey(tn))
                    {
                        row.Cells[IND_CELL_TAG_NAME].Value = FTag.fnsNames[tn];
                    }
                    else
                    {
                        row.Cells[IND_CELL_TAG_NAME].Value = "???";
                    }
                    if (list.Count == 0)
                    {
                        row.Cells[IND_CELL_TAG_PRESENTATION_STR].Value = "";
                        row.Cells[IND_CELL_PROCESSING_STATUS].Style.ForeColor = Color.Gray;
                        row.Cells[IND_CELL_PROCESSING_STATUS].Value = "#EMPTY";
                    }
                    else if (list.Count == 1)
                    {
                        if (list[0] != null)
                        {
                            row.Cells[IND_CELL_TAG_PRESENTATION_STR].Value = list[0].Representation;
                        }
                        else
                        {
                            row.Cells[IND_CELL_TAG_PRESENTATION_STR].Value = "";
                            row.Cells[IND_CELL_PROCESSING_STATUS].Style.ForeColor = Color.Gray;
                            row.Cells[IND_CELL_PROCESSING_STATUS].Value = "#EMPTY";
                        }
                    }
                    else
                    {
                        row.Cells[IND_CELL_TAG_PRESENTATION_STR].Value = "Вложенных тегов: " + list.Count;
                    }
                }
            else if (row.Tag == null)
                {
                    LogHandle.ol("Не заполнены данные при создании строки таблицы!");
                    int tn = 0;
                    int.TryParse(row.Cells[IND_CELL_TAG_NUMBER].Value.ToString(), out tn);
                    if (tn > 0)
                    {
                        if (FTag.fnsNames.ContainsKey(tn))
                        {
                            row.Cells[IND_CELL_TAG_NAME].Value = FTag.fnsNames[tn];
                        }
                        else
                        {
                            row.Cells[IND_CELL_TAG_NAME].Value = "???";
                        }
                        row.Tag = new List<FTag>();
                        row.Cells[IND_CELL_TAG_PRESENTATION_STR].Value = "";
                        row.Cells[IND_CELL_PROCESSING_STATUS].Style.ForeColor = Color.Gray;
                        row.Cells[IND_CELL_PROCESSING_STATUS].Value = "#EMPTY";
                    }
                    else
                    {
                        row.Cells[IND_CELL_TAG_NUMBER].Style.ForeColor = Color.Red;
                        row.Cells[IND_CELL_TAG_NAME].Style.ForeColor = Color.Red;
                        row.Cells[IND_CELL_TAG_NAME].Value = "bad tag number";
                        row.Cells[IND_CELL_TAG_PRESENTATION_STR].Value = "";
                        row.Cells[IND_CELL_PROCESSING_STATUS].Style.ForeColor = Color.Red;
                        row.Cells[IND_CELL_PROCESSING_STATUS].Value = "ERROR";
                    }
                }
            else
                {
                    LogHandle.ol("!!!! incorrect filled data !!!!");
                }
            if (!string.IsNullOrEmpty(status))
                {
                    row.Cells[IND_CELL_PROCESSING_STATUS].Value = status;
                    if (statusSign == 1)
                    { row.Cells[IND_CELL_PROCESSING_STATUS].Style.ForeColor = Color.Gray; }
                    else if (statusSign == 2)
                    {
                        row.Cells[IND_CELL_PROCESSING_STATUS].Style.ForeColor = Color.Green;
                    }
                    else if (statusSign == 3)
                    {
                        row.Cells[IND_CELL_PROCESSING_STATUS].Style.ForeColor = Color.Red;
                    }
                    else
                    {
                        row.Cells[IND_CELL_PROCESSING_STATUS].Style.ForeColor = Color.Black;
                    }
                }
            else
                {
                    row.Cells[IND_CELL_PROCESSING_STATUS].Style.ForeColor = Color.Black;
                    row.Cells[IND_CELL_PROCESSING_STATUS].Value = "";
                }
            if (row.Cells[IND_CELL_PROCESSING_STATUS].Tag != null)
            {
                if (row.Cells[IND_CELL_PROCESSING_STATUS].Tag is object[])
                {
                    StringBuilder s = new StringBuilder();
                    object[] objs = row.Cells[IND_CELL_PROCESSING_STATUS].Tag as object[];
                    bool allOk = true;
                    foreach (object obj in objs)
                    {
                        if (obj is int)
                        {
                            int code = (int)obj;
                            if(code!=0)
                                allOk = false;
                            if (FnAnswerCode.ContainsKey(code))
                            {
                                s.Append(FnAnswerCode[code]);
                                s.Append("; ");
                            }
                            else
                            {
                                s.Append("Код ответа ");
                                s.Append(code);
                                s.Append("; ");
                            }
                        }
                        else
                        {
                            allOk = false;
                            s.Append(obj.ToString());
                        }
                    }
                    row.Cells[IND_CELL_PROCESSING_STATUS].Value = s.ToString();
                    if(allOk)
                    {
                        row.Cells[IND_CELL_PROCESSING_STATUS].Style.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        row.Cells[IND_CELL_PROCESSING_STATUS].Style.BackColor = Color.MistyRose;
                    }
                }

            }
            else
            {
                row.Cells[IND_CELL_PROCESSING_STATUS].Style.BackColor = Color.LightGray;
            }
        }


        void MmapTreeTlvStruct(List<FTag> list) // Пустого списка тут передаваться не должно
        {
            _textBox__tabFdToPerf_EditorValueMain_skipper = true;
            TreeView table = treeView_tabPerfStlvMiniConstructor;
            table.Nodes.Clear();
            foreach (FTag f in list)
            {
                if(f == null)
                {
                    
                    LogHandle.ol("Пропуск разметки дерева для тега == null, попадать сюда не должны");
                    continue;
                }
                TreeNode node = null;
                if (f.TagNumber == 0)
                {
                    node = table.Nodes.Add(f.TryCreateNumber.ToString() + "(Empty)");
                }
                else
                {
                    node = table.Nodes.Add(f.TagNumber.ToString());
                    if(f.Type == FTag.FDataType.STLV)
                    {
                        MapBranches(f, ref node);
                    }
                }
                node.Tag = f;
            }
            if(list.Count == 1 && table.Nodes.Count>0)
            {
                table.SelectedNode = table.Nodes[0];
            }
            _textBox__tabFdToPerf_EditorValueMain_skipper = false;
        }
        
    }
}
