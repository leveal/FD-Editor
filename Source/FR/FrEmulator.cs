using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace FR_Operator
{
    internal class FrEmulator : FiscalPrinter
    {
        public FrEmulator(MainForm ui, bool interfaceReaction = true)
        {
            _ui = ui;
            _fnMemory.Add(1, new FnReadedDocument(FTAG_FISCAL_DOCUMENT_TYPE_FISCAL_REPORT, DateTime.Now.AddMinutes(-60), 1, 0, (_emptyFp ? "" : ((new Random()).Next()).ToString("D10"))));
            _fnMemory.Add(2, new FnReadedDocument(FTAG_FISCAL_DOCUMENT_TYPE_OPEN_SHIFT, DateTime.Now.AddMinutes(-10), 2, 0, (_emptyFp ? "" : ((new Random()).Next()).ToString("D10"))));
            _lastFD = 2;
            if(interfaceReaction)KKMInfoTransmitter[FR_CONNECTION_SETTINGS_KEY] = "......";
            _interfaceReaction = interfaceReaction;
        }

        public override bool CancelDocument()
        {
            return true;
        }

        public override bool CashRefill(double sum, bool income = true)
        {
            return true;
        }

        public override bool ChangeDate(int appendDay = 0, DateTime date = default)
        {
            return true;
        }

        public override bool CloseShift()
        {
            if(_shiftState != FR_SHIFT_CLOSED)
            {
                _lastFD++;
                _fnMemory.Add(_lastFD, new FnReadedDocument(FiscalPrinter.FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_SHIFT, DateTime.Now, _lastFD, 0, (_emptyFp ? "" : ((new Random()).Next()).ToString("D10"))));
                _shiftState = FR_SHIFT_CLOSED;
                //ReadDeviceCondition();
                return true;
            }
            return false;
        }

        public override bool Connect()
        {
            _ffdVer = FR_FFD120;
            _connected = true;

            if(_shiftState!=FR_SHIFT_CLOSED) _shiftState = FR_SHIFT_OPEN;
            if (_interfaceReaction)
            {
                KKMInfoTransmitter[FR_SHIFT_STATE_KEY] = "Смена открыта";
                KKMInfoTransmitter[FR_OWNER_USER_KEY] = "Рога и копыта";
                KKMInfoTransmitter[FR_TIME_KEY] = DateTime.Now.ToString(DEFAULT_DT_FORMAT);
                KKMInfoTransmitter[FR_LAST_FD_NUMBER_KEY] = _lastFD.ToString();
                KKMInfoTransmitter[FR_SERIAL_KEY] = "emu-0123456";
                KKMInfoTransmitter[FR_FIRMWARE_KEY] = "emu-v-7";
                KKMInfoTransmitter[FR_FFDVER_KEY] = "1.2";
                KKMInfoTransmitter[FR_REGISTERD_SNO_KEY] = "УСД";
                KKMInfoTransmitter[FR_OWNER_ADDRESS_KEY] = "Moscow";
                KKMInfoTransmitter[FR_OFD_EXCHANGE_STATUS_KEY] = "empty";
                KKMInfoTransmitter[FR_STATUS_MODE_KEY] = "empty";
                KKMInfoTransmitter[FR_MODEL_KEY] = "emu-frf";
                
            }
            KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = "";
            this.ReadDeviceCondition();
            return true;
        }

        public override string ConnectionReprezentation()
        {
            return "Эмулятор ФР";
        }

        public override bool ConnectionWindow()
        {
            return SettingsWindow();
        }

        private bool SettingsWindow()
        {
            Form emuSettings = new Form();
            emuSettings.Text = "Эмулятор ФР настройка";
            emuSettings.Size = new System.Drawing.Size(265,175);
            emuSettings.ClientSize = new System.Drawing.Size(265, 175);
            emuSettings.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            

            Label lb_info = new Label();
            lb_info.Text = "СНО по умолч.";
            lb_info.Location = new System.Drawing.Point(20, 10);
            emuSettings.Controls.Add(lb_info);

            ComboBox cmb_sno = new ComboBox();
            cmb_sno.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmb_sno.FormattingEnabled = true;
            cmb_sno.Items.AddRange(new object[]
            {
                "Нет",
                SNO_TRADITIONAL,
                SNO_USN_DOHOD,
                SNO_USN_DR,
                "ЕСХН",
                SNO_PSN
            });
            cmb_sno.Location = new System.Drawing.Point(140, 7);
            cmb_sno.Name = "cmb_sno_default";
            cmb_sno.Width = 70;
            if(_snoDefaul <=2 )
            {
                cmb_sno.SelectedIndex = _snoDefaul;
            }
            else if (_snoDefaul == 4)
            {
                cmb_sno.SelectedIndex = 3;
            }
            else if (_snoDefaul == 16)
            {
                cmb_sno.SelectedIndex = 4;
            }
            else if (_snoDefaul == 32)
            {
                cmb_sno.SelectedIndex = 5;
            }
            cmb_sno.SelectedIndexChanged += EventWindowHandler;
            KKMInfoTransmitter[FR_REGISTERD_SNO_KEY] = cmb_sno.Text;
            emuSettings.Controls.Add(cmb_sno);

            CheckBox checkBox_fillFds = new CheckBox();
            checkBox_fillFds.Location = new System.Drawing.Point(20, 35);
            checkBox_fillFds.Size = new System.Drawing.Size(145, 17);
            checkBox_fillFds.Text = "Заполнить чеками";
            checkBox_fillFds.Checked = false;
            emuSettings.Controls.Add(checkBox_fillFds);

            TextBox tb_cheques = new TextBox();
            tb_cheques.Text = _chequesNumToFill.ToString();
            tb_cheques.Name = "tb_cheques_to_fill";
            tb_cheques.Location = new System.Drawing.Point(167,34);
            tb_cheques.Size = new System.Drawing.Size(33,17);
            emuSettings.Controls.Add(tb_cheques);

            Label lb_cheques = new Label();
            lb_cheques.Text = "шт";
            lb_cheques.Location = new System.Drawing.Point(205, 35);
            emuSettings.Controls.Add(lb_cheques);

            Label lb_delay = new Label();
            lb_delay.Text = "Задержка оформления";
            lb_delay.Location = new System.Drawing.Point(20, 60);
            lb_delay.Size = new System.Drawing.Size(128, 17);
            emuSettings.Controls.Add(lb_delay);

            CheckBox checkBox_emptyFiscalSign = new CheckBox();
            checkBox_emptyFiscalSign.Location = new System.Drawing.Point(20, 83);
            checkBox_emptyFiscalSign.Size = new System.Drawing.Size(145, 17);
            checkBox_emptyFiscalSign.Text = "Пустой ФП";
            checkBox_emptyFiscalSign.Checked = _emptyFp;
            checkBox_emptyFiscalSign.CheckedChanged += EventWindowHandler;
            emuSettings.Controls.Add(checkBox_emptyFiscalSign);

            TextBox tb_delay = new TextBox();
            tb_delay.Location = new System.Drawing.Point(167, 59);
            tb_delay.Size = new System.Drawing.Size(33,17);
            tb_delay.Name = "tb_delay";
            tb_delay.Text = AppSettings.EmulatorDelay.ToString();
            emuSettings.Controls.Add(tb_delay);

            Label lb_milisec = new Label();
            lb_milisec.Text = "мс";
            lb_milisec.Location = new System.Drawing.Point(205, 60);
            emuSettings.Controls.Add(lb_milisec);

            Button bt_cleanFd = new Button();
            bt_cleanFd.Text = "Удалить чеки из памяти эмулятора";
            bt_cleanFd.Location = new System.Drawing.Point(25,110);
            bt_cleanFd.Size = new System.Drawing.Size(220,24);
            bt_cleanFd.Click += EventWindowHandler;
            emuSettings.Controls.Add(bt_cleanFd);

            Button btn_ok = new Button();
            btn_ok.Text = "OK";
            btn_ok.Location = new System.Drawing.Point(25, 145);
            btn_ok.Size = new System.Drawing.Size(90, 24);
            btn_ok.Click += EventWindowHandler;
            emuSettings.Controls.Add(btn_ok);

            Button btn_cancel = new Button();
            btn_cancel.Text = "OTMEHA";
            btn_cancel.Location = new System.Drawing.Point(155, 145);
            btn_cancel.Size = new System.Drawing.Size(90, 24);
            btn_cancel.Click += EventWindowHandler;
            emuSettings.Controls.Add(btn_cancel);
            emuSettings.ShowDialog();

            return emuSettings.DialogResult == DialogResult.OK;
        }

        private void EventWindowHandler(object sender, EventArgs e)
        {
            Form dialogWindow = (sender as Control).Parent as Form;
            if (sender is Button && (sender as Button).Text == "OK")
            {
                foreach(var tb in dialogWindow.Controls.OfType<TextBox>())
                {
                    if(tb.Name == "tb_delay")
                    {
                        try 
                        {
                            if(string.IsNullOrEmpty(tb.Text))
                                AppSettings.EmulatorDelay = 0;
                            else
                                AppSettings.EmulatorDelay = int.Parse(tb.Text); 
                        } catch { AppSettings.EmulatorDelay = 90; }
                    }
                    else if(tb.Name == "tb_cheques_to_fill")
                    {
                        try { _chequesNumToFill = int.Parse(tb.Text); } catch { _chequesNumToFill = 45; }
                    }
                    if (_chequesNumToFill > 200000)
                    {
                        _chequesNumToFill = 100000;
                    }
                }
                int z = 0;
                foreach(var ch in dialogWindow.Controls.OfType<CheckBox>())
                {
                    
                    if(ch.Text == "Заполнить чеками" && ch.Checked)
                    {
                        int lastDelay = AppSettings.EmulatorDelay;

                        AppSettings.EmulatorDelay = 0;
                        int iq = MainForm.SAMLE_ITEMS.Length;
                        for (int i = 0; i < _chequesNumToFill; i++)
                        {
                            int items = _random.Next(8) + 1;
                            FiscalCheque cheque = new FiscalCheque();
                            double total = 0;
                            for (int j = 0; j < items; j++)
                            {
                                double price = Math.Round(_random.NextDouble() * 500.0, 0);
                                ConsumptionItem item = new ConsumptionItem(MainForm.SAMLE_ITEMS[(i * (j+1) + z++ % (1+j) ) % iq], price, 1, price, AppSettings.ItemProductType, AppSettings.ItemPaymentType, AppSettings.ItemTaxRate);
                                item.Unit120 = 0;
                                cheque.Items.Add(item);
                                total += price;
                            }
                            cheque.TotalSum = total;
                            if (i * items % 2 == 0)
                            {
                                cheque.Cash = total;
                            }
                            else
                            {
                                cheque.ECash = total;
                            }
                            cheque.Control();
                            PerformFD(cheque);
                        }
                        CloseShift();
                        AppSettings.EmulatorDelay = lastDelay;
                        LogHandle.ol("В эмулятор добавлено чеков "+_chequesNumToFill);
                        ReadDeviceCondition();
                        if(_interfaceReaction)_ui.UpdateUiKkmDescribtion();
                    }
                    
                }
                dialogWindow.DialogResult = DialogResult.OK;
                dialogWindow.Close();
            }
            else if(sender is Button && (sender as Button).Text == "OTMEHA")
            {
                dialogWindow.DialogResult = DialogResult.Cancel;
                dialogWindow.Close();
            }
            else if(sender is Button && (sender as Button).Text == "Удалить чеки из памяти эмулятора")
            {
                _fnMemory.Clear();
                _fnMemory.Add(1, new FnReadedDocument(FiscalPrinter.FTAG_FISCAL_DOCUMENT_TYPE_FISCAL_REPORT, DateTime.Now.AddMinutes(-60), 1, 0, (_emptyFp ? "" : ((new Random()).Next()).ToString("D10"))));
                _fnMemory.Add(2, new FnReadedDocument(FiscalPrinter.FTAG_FISCAL_DOCUMENT_TYPE_OPEN_SHIFT, DateTime.Now.AddMinutes(-10), 2, 0, (_emptyFp? "":((new Random()).Next()).ToString("D10"))) );
                _lastFD = 2;
                _shiftState = FR_SHIFT_OPEN;
                LogHandle.ol("Память эмулятора очищена");
                ReadDeviceCondition();
            }
            else if(sender is CheckBox)
            {
                CheckBox cb = sender as CheckBox;
                if(cb.Text == "Пустой ФП")
                {
                    _emptyFp = cb.Checked;
                }
            }
            else if(sender is ComboBox)
            {
                ComboBox cmb = sender as ComboBox;
                if(cmb.Name == "cmb_sno_default")
                {
                    if (cmb.SelectedIndex <= 2)
                    {
                        _snoDefaul = cmb.SelectedIndex;
                    }
                    else if (cmb.SelectedIndex == 3)
                    {
                        _snoDefaul = 4;
                    }
                    else if (cmb.SelectedIndex == 4)
                    {
                        _snoDefaul = 16;
                    }
                    else if (cmb.SelectedIndex == 5)
                    {
                        _snoDefaul = 32;
                    }
                    KKMInfoTransmitter[FR_REGISTERD_SNO_KEY] = cmb.Text;
                }
               
            }

        }


        public override bool ContinuePrint()
        {
            return true;
        }

        public override void Disconnect()
        {
            _connected = false;
            if (_interfaceReaction)
            {
                KKMInfoTransmitter[FR_STATUS_MODE_KEY] = "";
                KKMInfoTransmitter[FR_OWNER_USER_KEY] = "";
                KKMInfoTransmitter[FR_TIME_KEY] = "";
                KKMInfoTransmitter[FR_LAST_FD_NUMBER_KEY] = "";
                KKMInfoTransmitter[FR_SERIAL_KEY] = "";
                KKMInfoTransmitter[FR_FIRMWARE_KEY] = "";
                KKMInfoTransmitter[FR_FFDVER_KEY] = "";
                KKMInfoTransmitter[FR_REGISTERD_SNO_KEY] = "";
                KKMInfoTransmitter[FR_OWNER_ADDRESS_KEY] = "";
                KKMInfoTransmitter[FR_OFD_EXCHANGE_STATUS_KEY] = "";
                KKMInfoTransmitter[FR_STATUS_MODE_KEY] = "";
                KKMInfoTransmitter[FR_MODEL_KEY] = "";
                KKMInfoTransmitter[FR_SHIFT_STATE_KEY] = "";
                _ui.UpdateUiKkmDescribtion();
            }
            return;
        }

        public override bool OpenShift()
        {
            if (_shiftState == FR_SHIFT_CLOSED)
            {
                _lastFD++;
                _fnMemory.Add(_lastFD, new FnReadedDocument(FiscalPrinter.FTAG_FISCAL_DOCUMENT_TYPE_OPEN_SHIFT, DateTime.Now, _lastFD, 0, (_emptyFp ? "" : ((new Random()).Next()).ToString("D10"))));
                _shiftState = FR_SHIFT_OPEN;
                return true;
            }
            return false;
        }

        bool _interfaceReaction = true;
        
        Random _random = new Random();
        bool _emptyFp = true;
        private int _snoDefaul = 2;
        private int _chequesNumToFill = 45;
        public override bool PerformFD(FiscalCheque doc)
        {
            if(_shiftState == FR_SHIFT_CLOSED)
                OpenShift();
            if(AppSettings.EmulatorDelay > 0)
                Thread.Sleep(AppSettings.EmulatorDelay);
            bool badDocToPerfome = false;

            string badMsg = "";
            if(doc.Items.Count == 0 && doc.Document != FD_DOCUMENT_NAME_CORRECTION_CHEQUE)
            {
                badMsg = "пустой чек\t";
                badDocToPerfome = true;
            }

            double itemsSum=0;
            foreach (var item in doc.Items)
            {
                itemsSum += item.Sum;
                if(itemsSum < -0.0099)
                {
                    badMsg = "Отрицательная сумма предмета расчета";
                    badDocToPerfome=true;
                    break;
                }

                if(Math.Abs(Math.Round(item.Sum,2)-Math.Round(item.Price*item.Quantity,2)) > 0.011)
                {
                    badMsg = "Расхождение сумм более копейки в предмете расчета " + item.ToString();
                    badDocToPerfome = true;
                    break;
                }
                if(item.Quantity < 0.00000001)
                {
                    badMsg = "Нулевое количество в предмете расчета " + item.ToString();
                    badDocToPerfome = true;
                    break;
                }
                if (item.ProductType <= 0 || item.ProductType > 33 || item.ProductType == 28 || item.ProductType == 29)
                {
                    badMsg = "Некорректное значение 1212=" + item.ProductType;
                    badDocToPerfome = true;
                    break;
                }
                if(item.PaymentType<=0|| item.PaymentType > 7)
                {
                    badMsg = "Некорректное значение 1214[1-7]=" + item.PaymentType;
                    badDocToPerfome = true;
                    break;
                }
                if (item.NdsRate < 0 || item.NdsRate > 10)
                {
                    badMsg = "Некорректная ставка НДС[1-10]= " + item.NdsRate;
                    badDocToPerfome = true;
                    break;
                }
            }
            itemsSum = Math.Round(itemsSum,2);
                
            if (doc.TotalSum > itemsSum + 0.001 && doc.Document != FD_DOCUMENT_NAME_CORRECTION_CHEQUE)
            {
                badMsg += "итог больше сумм предметов расчета\t";
                badDocToPerfome = true;
            }
            if (doc.Document == FD_DOCUMENT_NAME_CORRECTION_CHEQUE && doc.Items.Count > 0)
            {
                //  количество предметов расчета больше нуля соответствует ФФД > 1.05
                if(doc.TotalSum > itemsSum + 0.001)
                {
                    badMsg += "итог больше сумм предметов расчета\t";
                    badDocToPerfome = true;
                }
                else
                {
                    // итог меньше сумм предметов расчета
                    double roundedItemsSum = (double)((int)(itemsSum));
                    if (doc.TotalSum < roundedItemsSum - 0.001)
                    {
                        badMsg += "итог критично меньше сумм предметов расчета\t";
                        badDocToPerfome = true;
                    }
                }
            }

            if((doc.Document != FD_DOCUMENT_NAME_CORRECTION_CHEQUE && doc.Items.Count == 0) && doc.TotalSum < itemsSum &&  doc.TotalSum < Math.Round(itemsSum, 0))
            {
                KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = "некорректен итог чека\t";
                badDocToPerfome = true;
            }
            double paynentSumms = Math.Round(doc.Cash + doc.ECash + doc.Prepaid + doc.Credit + doc.Provision, 2);
            if (doc.IsNotPaid||doc.IsOverPaid)
            {
                if (doc.IsOverPaid||Math.Abs(doc.TotalSum-paynentSumms)>0.99) // возможно округлени
                {
                    badMsg += "некоректна оплата\t";
                    badDocToPerfome = true;
                }
            }

            if (doc.TotalSum - paynentSumms >= 0.009)
            {
                badMsg += "Сумма оплат меньше итога чека";
                badDocToPerfome = true;
            }
            if (doc.TotalSum - paynentSumms <= -0.009 && doc.Cash + 0.001 > paynentSumms - doc.TotalSum)
            {
                badMsg += "Сумма оплат больше итога чека";
                badDocToPerfome = true;
            }
            if (doc.Sno == 0 && _snoDefaul == 0)
            {
                badMsg += "Не выбрана СНО";
                badDocToPerfome = true;
            }

            if (badDocToPerfome)
            {
                KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = badMsg;
                if (_interfaceReaction) RezultMsg("Ошибка "+badMsg);
                return false;
            }
            if (doc.WithChange)
            {
                double delta = doc.Cash + doc.ECash + doc.Prepaid + doc.Credit + doc.Provision - doc.TotalSum;
                if (delta < 0)
                {
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = "Проблема с подсчетами";
                    if (_interfaceReaction) RezultMsg("!!!Проблема с подсчетами!!!");
                    return false;
                }
                doc = doc.Clone() as FiscalCheque;
                doc.Cash -= delta;
            }


            int fs = _random.Next();
            FnReadedDocument frd = new FnReadedDocument(doc.DocumentNameFtagType, DateTime.Now, ++_lastFD, doc.TotalSum, (_emptyFp? "" : fs.ToString("D10")),(FiscalCheque)doc.Clone());
            if(frd.Cheque.Sno == 0)
            {
                frd.Cheque.Sno = _snoDefaul;
            }
            _fnMemory.Add(_lastFD, frd);

            return true;
        }

        public override void ReadDeviceCondition()
        {
            if (_interfaceReaction)
            {
                KKMInfoTransmitter[FR_TIME_KEY] = DateTime.Now.ToString(DEFAULT_DT_FORMAT);
                KKMInfoTransmitter[FR_LAST_FD_NUMBER_KEY] = _lastFD.ToString();
                if (_shiftState == FR_SHIFT_CLOSED)
                    KKMInfoTransmitter[FR_SHIFT_STATE_KEY] = "Закрыта";
                else if (_shiftState == FR_SHIFT_OPEN)
                    KKMInfoTransmitter[FR_SHIFT_STATE_KEY] = "Открыта";
                else
                    KKMInfoTransmitter[FR_SHIFT_STATE_KEY] = _shiftState.ToString();
                if (_connected) _ui.UpdateUiKkmDescribtion();
            }
        }

        public override FnReadedDocument ReadFD(int docNumber, bool parce = false)
        {
            if (_fnMemory.ContainsKey(docNumber))
            {
                FnReadedDocument fd = _fnMemory[docNumber];
                FnReadedDocument fdExport = new FnReadedDocument(fd.Type, fd.Time, fd.Number, fd.Summ, fd.FiscalSign, (parce&&fd.Cheque!=null) ? fd.Cheque.Clone() as FiscalCheque : null);
                if (AppSettings.AppendFiscalSignAsPropertyData && fdExport.Cheque != null && !string.IsNullOrEmpty(fdExport.FiscalSign))
                {
                    if (!string.IsNullOrEmpty(fdExport.Cheque.PropertiesData))
                    {
                        if (AppSettings.OverridePropertyData)
                        {
                            fdExport.Cheque.PropertiesData = fdExport.FiscalSign;
                        }
                    }
                    else
                    {
                        fdExport.Cheque.PropertiesData = fdExport.FiscalSign;
                    }  
                }
                return fdExport;
            }
            return FnReadedDocument.EmptyFD;
        }

        public override void ReleaseLib()
        {
            if(_connected)
                Disconnect();
            _fnMemory.Clear();
            _lastFD = 0;
        }
        
        private Dictionary<int,FnReadedDocument> _fnMemory = new Dictionary<int, FnReadedDocument>();
    }
}
