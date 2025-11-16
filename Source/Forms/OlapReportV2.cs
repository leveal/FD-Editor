using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using FR_Operator.Properties;
using System.Data.SqlClient;

namespace FR_Operator
{
    internal partial class OlapReportV2 : Form
    {
        public OlapReportV2(MainForm mainWindow)
        {
            InitializeComponent();
            Icon = Resources.fd_editpr_16_2;
            _mainWindowDontPrintFlagOriginal = mainWindow.DontPrintFlag;
            comboBox_olapCorrectionType.SelectedIndex = 0;
            comboBox_correctionRuleDocumentType.SelectedIndex = 1;
            comboBox_correctionDateSource.SelectedIndex = _ind_correctionDate;
            comboBox_cheqNumber.SelectedIndex = _ind_cheqNumber;
            dataGridView_olapPaymetType.Rows.Clear();
            dataGridView_olapPaymetType.AllowUserToAddRows = false;
            comboBox_cheqPayment.SelectedIndex = _ind_paymentType;
            comboBox_itemName.SelectedIndex = _ind_itemName;
            comboBox_itemQuantity.SelectedIndex = _ind_itemQuantity;
            comboBox_itemSum.SelectedIndex = _ind_itemSum;
            comboBox_itemNdsSum.SelectedIndex = 6;
            comboBox_itemPropertySource.SelectedIndex = 0;
            comboBox_cheq_itemType.SelectedIndex = 1;
            comboBox_cheq_operationSign.SelectedIndex = 1;
            comboBox_cheq_itemPaymentTypeSign.SelectedIndex = 4;
            this.mainWindow = mainWindow;
            comboBox_cheq_sno.SelectedIndex = mainWindow.ChosenSno;
            comboBox_olapErrorsAllowed.SelectedIndex = 6;
            comboBox_olapItemPaymentTypeSign.SelectedIndex = 0;
            comboBox_olapItemMeasureUnit.SelectedIndex = 0;
            checkBox_olapDisablePrint.Checked = true;
            comboBox_FQ_precision.SelectedIndex = 5;
            comboBox_propertiesData.SelectedIndex = 12;
            comboBox_buyerAddress.SelectedIndex = _ind_buyerAddress;
            textBox_buyerAddressDefault.Text = _buyerAddressDefault;
            if (mainWindow.fiscalPrinter != null && mainWindow.fiscalPrinter is FrEmulator)
            {
                groupBox_emuFrSetting.Visible = true;
                if(AppSettings.EmulatorDelay == 0)
                {
                    groupBox_emuFrSetting.Enabled = false;
                    label_emuFrStatus.Text = "Установлена нулевая задержка оформления ФД.";
                }
                else
                {
                    groupBox_emuFrSetting.Enabled = true;
                    label_emuFrStatus.Text = "Установлена задержка " + AppSettings.EmulatorDelay + " мc оформления ФД для ускорения" + Environment.NewLine +
                        "обработки задания рекомендуется обнулить задержку";
                }
            }
            _fe = new FormEnabled();
        }
        MainForm mainWindow;
        public FiscalPrinter FR
        {
            set => _fr = value;
        }
        private FiscalPrinter _fr = null;

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g;
            string sText;
            int iX;
            float iY;
            SizeF sizeText;
            TabControl ctlTab;
            ctlTab = (TabControl)sender;

            g = e.Graphics;

            sText = ctlTab.TabPages[e.Index].Text;
            sizeText = g.MeasureString(sText, ctlTab.Font);
            iX = 15;//e.Bounds.Left + 6;
            iY = e.Bounds.Top + (e.Bounds.Height - sizeText.Height) / 2;
            g.DrawString(sText, ctlTab.Font, Brushes.Black, iX, iY);
        }

        private void ActionControls(object sender, EventArgs e)
        {
            if(sender == button_goTo2_date) // файл
            {
                try
                {
                    _startFrom = int.Parse(comboBox_olapStartReadFrom.Text);
                }
                catch (Exception ex)
                {
                    textBox_olapReadPreInfo.Text = ex.Message;
                    return;
                }
                checkEventCancel = false;
                tabControl_steps.SelectedIndex = 1;// реквизиты ЧК
            } 
            else if (sender == button_goTo3_checkNum)
            {
                _useCorrectionCheque = comboBox_correctionRuleDocumentType.SelectedIndex == 1;
                _correctionType = comboBox_olapCorrectionType.SelectedIndex;
                if(textBox_cheque_orderNum.Text != "")
                    _correctionOrderNumber = textBox_cheque_orderNum.Text;
                _ind_correctionDate = comboBox_correctionDateSource.SelectedIndex;
                checkEventCancel = false;
                tabControl_steps.SelectedIndex = 2;// номер чека
                _ind_propertiesData = comboBox_propertiesData.SelectedIndex;
                if (!checkBox_propertiesData.Checked)
                {
                    _ind_propertiesData = -1;
                }
                
            } 
            else if (sender == button_goTo4_cheqPayment)
            {
                _ind_cheqNumber = comboBox_cheqNumber.SelectedIndex;
                checkEventCancel = false;
                tabControl_steps.SelectedIndex = 3; //оплата
                AnalizePaymentTable();
            } 
            else if (sender == button_goTo5_itemProperty)
            {
                bool filled = true;
                foreach (DataGridViewRow r in dataGridView_olapPaymetType.Rows)
                {
                    if (r.Cells[1].Value == null || r.Cells[1].Value as string == "")
                    {
                        filled = false;
                        textBox_paymentMessage.Text = "Заполните типы оплат в таблице и нажмите далее";
                        return;
                    }
                }
                if (filled && dataGridView_olapPaymetType.Rows.Count>0)
                {
                    textBox_paymentMessage.Text = "";
                    checkEventCancel = false;
                    tabControl_steps.SelectedIndex = 4; // признак предмета расчета
                }  
            } 
            else if (sender == button_goTo6_itemName)
            {
                _itemProductTypeDefault = comboBox_cheq_itemType.SelectedIndex;
                checkEventCancel = false;
                tabControl_steps.SelectedIndex = 5; //наименование предмета расчета
            }
            else if (sender == button_goTo7__itemQuantity)
            {
                checkEventCancel = false;
                tabControl_steps.SelectedIndex = 6; //количество предмета расчета
                _CheckQuantitiesValues();
            }
            else if (sender == button_goTo8_itemSum)
            {
                checkEventCancel = false;
                tabControl_steps.SelectedIndex = 7; //сумма за предмет расчета
            }
            else if (sender == button_goTo9_tax)
            {
                checkEventCancel = false;
                tabControl_steps.SelectedIndex = 8; //налог
            }
            else if (sender == button_goTo10_processong)
            {
                
                LogHandle.ol("Предварительный расчет");
                int cheqNumberLast = 0;
                int chequesTotalCount = 0;
                string paymentTypeLast = "";
                int i = 0;
                double[] sums = new double[5];
                DateTime correctionDate = dateTimePicker_correctionDateDafault.Value;
                bool useTableCorrectionDate = _ind_correctionDate >0 ;
                string extErrorDecribe = "";
                
                try
                {
                    extErrorDecribe = "дата: ";
                    if (_startFrom > 6 && useTableCorrectionDate)
                    {
                        for (int k = _startFrom; k >= 6; k--)
                        {
                            if (_table[k, _ind_correctionDate] != null && _table[k, _ind_correctionDate] is DateTime)
                            {
                                correctionDate = (DateTime)_table[k, _ind_correctionDate];
                            }

                        }
                    }

                    Dictionary<string, int> paymentTypeIndex = _fillPayments();
                    bool useTableDate = _ind_correctionDate != 0;
                    //extErrorDecribe = "номер чека: ";
                    for (i = _startFrom; i < _table.GetUpperBound(0); i++)
                    {
                        if (i == 11)
                            LogHandle.ol("pause debug");
                        extErrorDecribe = "номер чека: ";
                        int currentCheqNumber = 0;
                        string currentPaymentType = "";
                        if (_yarusNumbersQueue && _table[i, _ind_cheqNumber] != null)
                        {
                            int chNum = -1;
                            if (_table[i, _ind_cheqNumber] is string)
                            {
                                if (!int.TryParse(_table[i, _ind_cheqNumber] as string, out chNum))
                                {
                                    double d = -1;
                                    double.TryParse(_table[i, _ind_cheqNumber] as string, out d);
                                    chNum = (int)Math.Round(d, 0);
                                }
                            }
                            else if(_table[i, _ind_cheqNumber] is int)
                            {
                                chNum = (int)_table[i, _ind_cheqNumber];
                            }
                            else if(_table[i, _ind_cheqNumber] is double)
                            {
                                chNum = (int)Math.Round((double)_table[i, _ind_cheqNumber]);
                            }
                            else if (_table[i, _ind_cheqNumber] is decimal)
                            {
                                chNum = (int)Math.Round((decimal)_table[i, _ind_cheqNumber]);
                            }
                            if (chNum == cheqNumberLast)
                                _table[i, _ind_cheqNumber] = null;
                        }

                        if (_table[i, _ind_cheqNumber] != null)
                        {
                            // обнаружен новый чек или окончание предыдущего чека
                            // закрываем предыдущий чек
                            
                            if (_table[i, _ind_cheqNumber] is string && (_table[i, _ind_cheqNumber] as string).EndsWith(" всего"))
                            {
                                // окончание предыдущего чека с комбинированной оплатой
                                // дальнеших действий со строкой не требуется
                                paymentTypeLast = "";
                                continue;
                            }
                            chequesTotalCount++;
                            currentCheqNumber = _table[i, _ind_cheqNumber] is string ? int.Parse(_table[i, _ind_cheqNumber] as string) : (int)((double)_table[i, _ind_cheqNumber]);
                            if (currentCheqNumber != cheqNumberLast  || currentCheqNumber == 1 || cheqNumberLast == 0)
                            {
                                // новый чек имеед на 1 больший номер или начало смены и новый номер 1 или начинаем не сначала файла excel
                                cheqNumberLast = currentCheqNumber;
                            }
                            else
                            {
                                // прерываем обработку файла из-за некорректного номера чека
                                throw new Exception("Ошибка с порядком номеров чеков предыдущий номер чека " + cheqNumberLast + " чек в текущей строке " + currentCheqNumber);
                            }
                        }
                        extErrorDecribe = "наименование номенклатуры: ";
                        if (_table[i, _ind_itemName] != null)
                        {
                            // непустая товарная позиция
                            if (_table[i, _ind_itemName] is string && (_table[i, _ind_itemName] as string) != "")
                            {
                                // считываем реквизиты предмета расчета
                                string itemName = _table[i, _ind_itemName] as string;
                                double itemSum = 0;
                                double itemQuantity = 0;
                                extErrorDecribe = "дата: ";
                                if (useTableCorrectionDate && _table[i, _ind_correctionDate] != null && _table[i, _ind_correctionDate] is DateTime) 
                                    correctionDate = (DateTime)_table[i, _ind_correctionDate];
                                extErrorDecribe = "сумма: ";
                                if (_table[i, _ind_itemSum] != null) 
                                {
                                    if(_table[i, _ind_itemSum] is string) itemSum = double.Parse(_table[i, _ind_itemSum] as string);
                                    else itemSum = (double)_table[i, _ind_itemSum]; 
                                }
                                else
                                {
                                    throw new Exception("Отсутсвует сумма за предмет расчета в стоблце "+(char)('A'+ _ind_itemSum-1)+" номер стоблца "+ _ind_itemSum);
                                }
                                extErrorDecribe = "к-во: ";
                                if (_table[i, _ind_itemQuantity]!=null)
                                    if(_table[i, _ind_itemQuantity] is string) itemQuantity = double.Parse(_table[i, _ind_itemQuantity] as string);
                                    else itemQuantity = (double)_table[i, _ind_itemQuantity];
                                else
                                {
                                    throw new Exception("Отсутсвует количество предмета расчета в стоблце " + (char)('A' + _ind_itemQuantity - 1) + "(" + _ind_itemQuantity +")");
                                }
                                extErrorDecribe = "оплата: ";

                                if (_table[i, _ind_paymentType] == null && !string.IsNullOrEmpty(paymentTypeLast))
                                    currentPaymentType = paymentTypeLast;
                                else
                                {
                                    currentPaymentType = _table[i, _ind_paymentType] as string;
                                    paymentTypeLast = currentPaymentType;
                                    // данный блок сдесь для контроля формата таблицы
                                }
                                sums[paymentTypeIndex[currentPaymentType]] += itemSum;
                                extErrorDecribe = "Ставка налога";
                                if (_ind_itemNdsRate > 0)
                                {
                                    int tax = -1;
                                    if (_table[i, _ind_itemNdsRate] is string)
                                        tax = (int)Math.Round(double.Parse(_table[i, _ind_itemNdsRate] as string));
                                    else if (_table[i, _ind_itemNdsRate] is double)
                                        tax = (int)Math.Round((double)_table[i, _ind_itemNdsRate]);
                                    else if (_table[i, _ind_itemNdsRate] is int)
                                        tax = (int)_table[i, _ind_itemNdsRate];
                                    if(tax < 1 || tax > 10)
                                    {
                                        throw new Exception("Ставка налога выходит за установленный диапазон[1-10]: " + tax);
                                    }
                                }
                                extErrorDecribe = " признак предмета расчета.";
                                
                                int itemPropertieProductType = _itemProductTypeDefault;
                                if (_ind_itemPropertieProductType > 0 && _table[i, _ind_itemPropertieProductType] != null)
                                {
                                    int payment_type = -1;
                                    if (_table[i, _ind_itemPropertieProductType] is string)
                                        payment_type = (int)Math.Round(double.Parse(_table[i, _ind_itemPropertieProductType] as string));
                                    else if (_table[i, _ind_itemPropertieProductType] is double)
                                        payment_type = (int)Math.Round((double)_table[i, _ind_itemPropertieProductType]);
                                    else if (_table[i, _ind_itemPropertieProductType] is int)
                                        payment_type = (int)_table[i, _ind_itemPropertieProductType];
                                    if (payment_type < 1 || payment_type > 33 || payment_type == 28 || payment_type == 29)
                                    {
                                        payment_type = _itemProductTypeDefault;
                                        
                                    }
                                    itemPropertieProductType = payment_type;
                                }
                                if (itemPropertieProductType < 1 || itemPropertieProductType > 33 || itemPropertieProductType == 28 || itemPropertieProductType == 29)
                                    throw new Exception("Признак предмета расчета выходит за установленный диапазон[1-27][30-33]: " + itemPropertieProductType);


                            }
                            else
                            {
                                // прерываем обработку файла из-за некорректного названия товарной позиции
                                throw new Exception("Название предмета расчета не является строкой или строка пустая " + _table[i, _ind_itemName].GetType() + "\t " + _table[i, _ind_itemName].ToString());

                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    LogHandle.ol("Строка " + i + " "+extErrorDecribe+ ex.Message);
                    textBox_precalculateTable.Text = "Строка " + i + " " + extErrorDecribe+ Environment.NewLine + ex.Message;
                    chequesTotalCount = -1;
                }
                checkEventCancel = false;
                tabControl_steps.SelectedIndex = 9;
                if (chequesTotalCount != -1)
                {
                    textBox_precalculateTable.Text = "ИТОГО:" + Environment.NewLine +
                        "Чеков " + chequesTotalCount.ToString() + Environment.NewLine +
                        "Общаяя сумма " + Math.Round(sums[0] + sums[1] + sums[2] + sums[3] + sums[4], 2) + Environment.NewLine +
                        (sums[0] > 0.0099 ? "Наличными " + Math.Round(sums[0], 2) + Environment.NewLine : "") +
                        (sums[1] > 0.0099 ? "Безналичными " + Math.Round(sums[1], 2) + Environment.NewLine : "") +
                        (sums[2] > 0.0099 ? "Аванс " + Math.Round(sums[2], 2) + Environment.NewLine : "") +
                        (sums[3] > 0.0099 ? "Кредит " + Math.Round(sums[3], 2) + Environment.NewLine : "") +
                        (sums[4] > 0.0099 ? "Иной тип " + Math.Round(sums[4], 2) + Environment.NewLine : "");
                }
                if(_fr != null && !_fr.IsConnected)
                {
                    label_noFr.Visible = true;
                    button_beginCorrectionProcess.Enabled = false; //тут переключить потом
                }
                
            }
            else if(sender == comboBox_cheqNumber)
            {
                button_goTo4_cheqPayment.Enabled = comboBox_cheqNumber.SelectedIndex != 0;
            }
            else if(sender == comboBox_cheqPayment)
            {
                _ind_paymentType = comboBox_cheqPayment.SelectedIndex;
                if (_ind_paymentType == 0)
                {
                    button_goTo5_itemProperty.Enabled = false;
                }
                else
                {
                    button_goTo5_itemProperty.Enabled = true;
                    AnalizePaymentTable();
                }
            }
            else if(sender == comboBox_itemName)
            {
                _ind_itemName = comboBox_itemName.SelectedIndex;
                button_goTo6_itemName.Enabled = _ind_itemName != 0;
            }
            else if(sender == comboBox_itemQuantity)
            {
                _ind_itemQuantity = comboBox_itemQuantity.SelectedIndex;
                button_goTo7__itemQuantity.Enabled = _ind_itemQuantity != 0;
                _CheckQuantitiesValues();
            }
            else if(sender == comboBox_itemSum)
            {
                _ind_itemSum = comboBox_itemSum.SelectedIndex;
                button_goTo8_itemSum.Enabled = _ind_itemSum != 0;
            }
            else if(sender == comboBox_itemNdsSum)
            {
                //if (comboBox_itemNdsSum.SelectedIndex == 0) comboBox_itemNdsSum.SelectedIndex = 6;
                _taxRateDafault = comboBox_itemNdsSum.SelectedIndex;
                button_goTo10_processong.Enabled = _taxRateDafault != 0 || _ind_itemNdsRate > 0 ;
            }
            else if(sender == button_olap2ExcelOpenFile)
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|All files (*.*)|*.*";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    object[,] valueArray = null;
                    Microsoft.Office.Interop.Excel.Application _excelApp = null;
                    Microsoft.Office.Interop.Excel.Workbook workbook = null;
                    try
                    {
                        _excelApp = new Microsoft.Office.Interop.Excel.Application();
                        _excelApp.Visible = false;

                        string fileName = fileDialog.FileName;

                        //open the workbook
                        workbook = _excelApp.Workbooks.Open(fileName,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                            Type.Missing, Type.Missing);

                        //select the first sheet
                        Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];

                        //find the used range in worksheet
                        Microsoft.Office.Interop.Excel.Range excelRange = worksheet.UsedRange;

                        //get an object array of all of the cells in the worksheet (their values)
                        valueArray = (object[,])excelRange.get_Value(
                                    Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault);

                        textBox_olapReadPreInfo.Text = valueArray.GetUpperBound(0).ToString() + " строк в документе.";
                        _table = valueArray;
                        button_goTo2_date.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        textBox_olapReadPreInfo.Text = "MS Excel: " + ex.Message;
                    }
                    finally
                    {
                        if (workbook != null)
                        {
                            //clean up stuffs
                            workbook.Close(false, Type.Missing, Type.Missing);
                            Marshal.ReleaseComObject(workbook);
                        }
                        if (_excelApp != null)
                        {
                            _excelApp.Quit();
                            Marshal.FinalReleaseComObject(_excelApp);
                        }
                    }
                    
                    
                }
            }
            else if(sender == button_beginCorrectionProcess)
            {
                progressBar_excelLines.Value = 0;
                Thread thread = new Thread(PerformCorrections);
                thread.Start();
                label_startedIndicator.Visible = true;
                button_beginCorrectionProcess.Enabled = false;
            }
            else if(sender == button_interruptor)
            {
                _interruptor = true;
                label_startedIndicator.Text = "Коректировка прервана";
                Thread.Sleep(1000);
                _fe.FormCloseEnabled(this, true);
            }
            else if(sender == checkBox_extendReport)
            {
                _extendedReport = checkBox_extendReport.Checked;
            }
            else if(sender == comboBox_cheq_sno)
            {
                switch (comboBox_cheq_sno.SelectedIndex)
                {
                    case 0:
                        _sno = 1;
                        break;
                    case 1:
                        _sno = 2;
                        break;
                    case 2:
                        _sno = 4;
                        break;
                    case 3:
                        _sno = 16;
                        break;
                    case 4:
                        _sno = 32;
                        break;
                }
            }
            else if(sender == comboBox_cheq_operationSign)
            {
                if(comboBox_cheq_operationSign.SelectedIndex == 0)
                    comboBox_cheq_operationSign.SelectedIndex = _calculationSign;
                else
                    _calculationSign = comboBox_cheq_operationSign.SelectedIndex;
            }
            else if(sender == comboBox_cheq_itemPaymentTypeSign)
            {
                if(comboBox_cheq_itemPaymentTypeSign.SelectedIndex == 0)
                {
                    comboBox_cheq_itemPaymentTypeSign.SelectedIndex = _itemPaymentTypeDefault;
                }
                else
                {
                    _itemPaymentTypeDefault = comboBox_cheq_itemPaymentTypeSign.SelectedIndex;
                }
            }
            else if(sender == comboBox_olapErrorsAllowed)
            {
                _errorsAllowed = int.Parse(comboBox_olapErrorsAllowed.Text);
            }
            else if(sender == comboBox_olapItemMeasureUnit)
            {
                _itemMeasureUnit120 = int.Parse(comboBox_olapItemMeasureUnit.Text.Substring(0, comboBox_olapItemMeasureUnit.Text.IndexOf('.')));
                
            }
            else if(sender == checkBox_olapDisablePrint)
            {
                if (_fr != null && _fr.IsConnected)
                {
                    _fr.DontPrint = checkBox_olapDisablePrint.Checked;
                }
                mainWindow.DontPrintFlag = checkBox_olapDisablePrint.Checked;
            }
            else if(sender == radioButton_FQ_integerBehavior || sender == radioButton_FQ_fraqQuantToName || sender == radioButton_FQ_deepCalculate || sender == radioButton_FQ_allQuantToName)
            {
                if (radioButton_FQ_integerBehavior.Checked) 
                    _fractionalQuantitiesRule = 0;
                if (radioButton_FQ_fraqQuantToName.Checked)
                    _fractionalQuantitiesRule = 1;
                if (radioButton_FQ_allQuantToName.Checked)
                    _fractionalQuantitiesRule = 2;
                if (radioButton_FQ_deepCalculate.Checked)
                    _fractionalQuantitiesRule = 3;
            }
            else if(sender == comboBox_FQ_precision)
            {
                _fraqPrecision = comboBox_FQ_precision.SelectedIndex;
            }
            else if(sender == button_emuSetZeroDelay)
            {
                AppSettings.EmulatorDelay = 0;
                AppSettings.SaveSettings();
                groupBox_emuFrSetting.Enabled = false;
                label_emuFrStatus.Text = "Установлена нулевая задержка оформления ФД.";
            }
            else if(sender == checkBox_yarusChequeNumbersQueue)
            {
                _yarusNumbersQueue = checkBox_yarusChequeNumbersQueue.Checked;
            }
            else if(sender == checkBox_userProperties1084)
            {
                comboBox_userPropertiesPropertyValue1086.Enabled = checkBox_userProperties1084.Checked;
                comboBox_userPropertiesPropertyName1085.Enabled = checkBox_userProperties1084.Checked;
                if (!checkBox_userProperties1084.Checked)
                {
                    comboBox_userPropertiesPropertyValue1086.SelectedIndex = 0;
                    comboBox_userPropertiesPropertyName1085.SelectedIndex = 0;
                }
            }
            else if(sender == comboBox_userPropertiesPropertyValue1086)
            {
                if(comboBox_userPropertiesPropertyValue1086.SelectedIndex == 0)
                {
                    _ind_userPropertiesPropertyValue = -1;
                }
                else
                {
                    _ind_userPropertiesPropertyValue = comboBox_userPropertiesPropertyValue1086.SelectedIndex;
                }
            }
            else if (sender == comboBox_userPropertiesPropertyName1085)
            {
                if (comboBox_userPropertiesPropertyName1085.SelectedIndex == 0)
                {
                    _ind_userPropertiesPropertyName = -1;
                }
                else
                {
                    _ind_userPropertiesPropertyName = comboBox_userPropertiesPropertyName1085.SelectedIndex;
                }
            }
            else if(sender == comboBox_itemTaxIndex)
            {
                _ind_itemNdsRate = comboBox_itemTaxIndex.SelectedIndex;
                if (_ind_itemNdsRate > 0)
                {
                    comboBox_itemNdsSum.SelectedIndex = 0;
                    comboBox_itemNdsSum.Enabled = false;
                }
                else
                {
                    comboBox_itemNdsSum.Enabled = true;
                }

                button_goTo10_processong.Enabled = _taxRateDafault > 0 || _ind_itemNdsRate > 0;
            }
            else if(sender == comboBox_itemPropertySource)
            {
                _ind_itemPropertieProductType = comboBox_itemPropertySource.SelectedIndex;
            }
            else if(sender == comboBox_buyerAddress)
            {
                _ind_buyerAddress = comboBox_buyerAddress.SelectedIndex;
            }
            else if(sender == textBox_buyerAddressDefault)
            {
                _buyerAddressDefault = textBox_buyerAddressDefault.Text;
            }

        }

        void AnalizePaymentTable()
        {
            if(_table == null)
            {
                
                return;
            }
            List<string> paimentTypes = new List<string>();
            for (int i = _startFrom; i < _table.GetUpperBound(0); i++)
            {
                //LogHandle.ol( i+1+" строка Excel файла");
                if (_table[i, _ind_paymentType] != null)
                {
                    if (_table[i, _ind_paymentType] is string
                        && !(_table[i, _ind_paymentType] as string).EndsWith("всего")
                        && !paimentTypes.Contains(_table[i, _ind_paymentType]))
                    {
                        paimentTypes.Add(_table[i, _ind_paymentType] as string);
                        LogHandle.ol("Найден тип оплаты: " + _table[i, _ind_paymentType]);
                    }
                    if (paimentTypes.Count > 15)
                    {
                        textBox_paymentMessage.Text = "Более 15-ти типов оплаты\r\nисправьте настройки или отредактируйте excel-файл";
                        return;
                    }
                }
            }

            dataGridView_olapPaymetType.Rows.Clear();
            if (paimentTypes.Count <= 10 )
            {
                dataGridView_olapPaymetType.Rows.Clear();
                foreach (var s in paimentTypes)
                {
                    if (s is string)
                    {
                        dataGridView_olapPaymetType.Rows.Add();
                        dataGridView_olapPaymetType.Rows[dataGridView_olapPaymetType.Rows.Count - 1].Cells[0].Value = s;
                        if (_cashPresets.Contains(s))
                        {
                            (dataGridView_olapPaymetType.Rows[dataGridView_olapPaymetType.Rows.Count - 1].Cells[1] as DataGridViewComboBoxCell).Value = "НАЛИЧНЫЕ";
                        }
                        if (_eCashPresets.Contains(s))
                        {
                            (dataGridView_olapPaymetType.Rows[dataGridView_olapPaymetType.Rows.Count - 1].Cells[1] as DataGridViewComboBoxCell).Value = "БЕЗНАЛИЧНЫЕ";
                        }
                    }
                }
            }

            foreach(DataGridViewRow r in dataGridView_olapPaymetType.Rows)
            {
                if (r.Cells[1].Value == null || r.Cells[1].Value as string == "")
                {
                    textBox_paymentMessage.Text = "Заполните типы оплат в таблице";
                    return;
                }
            }
            
        }


        private object[,] _table;
        private int _startFrom = 6;
        private int _correctionType = 0;
        private string _correctionDescriber = "";
        private string _correctionOrderNumber = "б/н";

        private int _calculationSign = 1;
        private int _sno = 2;
        private int _itemProductTypeDefault = 1;
        private int _taxRateDafault = 6;

        private int _ind_propertiesData = -1;
        private int _ind_userPropertiesPropertyName = -1;
        private int _ind_userPropertiesPropertyValue = -1;
        private int _ind_correctionDate = 3;
        private int _ind_cheqNumber = 4;
        private int _ind_paymentType = 5;
        private int _ind_itemName = 6;
        private int _ind_itemQuantity = 7;
        private int _ind_itemSum = 8;
        private int _itemPaymentTypeDefault = 1;
        private int _itemMeasureUnit120 = 0;
        private int _errorsAllowed = 10;
        private int _fractionalQuantitiesRule = 0;
        private int _fraqPrecision = 5;
        private int _ind_itemNdsRate = -1;
        private int _ind_itemPropertieProductType = -1;

        private bool _interruptor = false;
        private bool _extendedReport = false;
        private bool _useCorrectionCheque = true;
        private bool _yarusNumbersQueue = true;
        private int _ind_buyerAddress = 0;
        private string _buyerAddressDefault = string.Empty;

        private bool _mainWindowDontPrintFlagOriginal = false;

        private List<string> _cashPresets = new List<string>()
        {
            "Наличные",
            "наличные",
            "НАЛИЧНЫЕ",
        };
        private List<string> _eCashPresets = new List<string>()
        {
            "безналичные",
            "Безналичные",
            "БЕЗНАЛИЧНЫЕ",
            "Безнал",
            "Банк Внешний",
            "Сбер API",
            "Интернет Эквайринг",
            "Sberbank",
            "СБП QR Промсвязбанк",
            "Банковские карты",
            "Сбербанк",
            "Переносной терминал"
        };
        private static List<string> _paymentTypes = new List<string>
        {
            "НАЛИЧНЫЕ",
            "БЕЗНАЛИЧНЫЕ",
            "АВАНС(ЗАЧЁТ)",
            "КРЕДИТА(ЗАЧЁТ)",
            "ВП"
        };



        bool checkEventCancel = true;
        private void tabControl_steps_Selecting(object sender, TabControlCancelEventArgs e)
        {
            e.Cancel = checkEventCancel;
            checkEventCancel = true;
        }

        private Dictionary<string, int> _fillPayments()
        {
            Dictionary<string, int> paymentTypeIndex = new Dictionary<string, int>();
            foreach (DataGridViewRow r in dataGridView_olapPaymetType.Rows)
            {
                string r1str = (string)r.Cells[1].Value;
                if (r1str == "НАЛИЧНЫЕ")
                    paymentTypeIndex.Add((string)r.Cells[0].Value, 0);
                else if (r1str == "БЕЗНАЛИЧНЫЕ") 
                    paymentTypeIndex.Add((string)r.Cells[0].Value, 1);
                else if (r1str == "АВАНС(ЗАЧЁТ)")
                    paymentTypeIndex.Add((string)r.Cells[0].Value, 2);
                else if (r1str == "КРЕДИТ(ЗАЧЁТ)")
                    paymentTypeIndex.Add((string)r.Cells[0].Value, 3);
                else if (r1str == "ВП")
                    paymentTypeIndex.Add((string)r.Cells[0].Value, 4);
            }
            return paymentTypeIndex;
        }

        
        FormEnabled _fe;    // блокировка закрытия окна
        //  метод нужно переделать 64-бидный ДТО Штриха крашится при работе с ним из отдельного потока
        //  1 вариант пересоздать FiscalRegister в этом потоке
        //  2 вариант запустить эту форму как отдельный поток, а не как диалог и инвокировать задания для ФР и результаты обратно 
        void PerformCorrections()
        {
            List<int> errorList = new List<int>();
            _fe.FormCloseEnabled(this, false);
            MassActionReporter reporter = new MassActionReporter(ref _table);
            LogHandle.ol("Приступаем к корректировке");
            int cheqNumberLast = 0;
            //int chequesTotalCount = 0;
            if(_fr == null || !_fr.IsConnected)
            {
                LogHandle.ol("Соединение с ФР не установлено");
                _fe.FormCloseEnabled(this, true);
                BeginInvoke(new Action(()=> textBox_precalculateTable.Text = "Нет соединения с ФР установите соединение и заполните форму заново"));
                return;
            }
            int errorsInSession = 0;
            FiscalCheque cheque = null;
            string paymentTypeLast = "";
            int i = 0;
            double[] sums = new double[5];
            DateTime correctionDate = dateTimePicker_correctionDateDafault.Value;
            bool useTableCorrectionDate = _ind_correctionDate >0;
            string subError = "";
            string propertiesDataCurrentLine = "";
            string propertiesDataLastLine = "";
            string userPropertiesPropertyNameCurrentLine = "";
            string userPropertiesPropertyNameLastLine = "";
            string userPropertiesPropertyValueCurrentLine = "";
            string userPropertiesPropertyValueLastLine = "";
            string buyerAddressLastLine = string.Empty;
            string buyerAddressCurrentLine = string.Empty;


            if (_startFrom > 6 && useTableCorrectionDate) 
            {
                for (int k = _startFrom; k >= 6; k--)
                {
                    subError = " дата расчета ";
                    if (useTableCorrectionDate && _table[k, _ind_correctionDate] != null && _table[k, _ind_correctionDate] is DateTime)
                    {
                        correctionDate = (DateTime)_table[k, _ind_correctionDate];
                        break;    
                    }
                }
            }// если начинаем не с начала файла ищем дату коррекции
            subError = "";
            try
            {
                Dictionary<string, int> paymentTypeIndex = _fillPayments();
                bool useTableDate = _ind_correctionDate != 0;

                int rows = _table.GetUpperBound(0) ;
                int percent = (rows - _startFrom)/100;

                if (percent == 0) 
                    percent = 1;
                
                double[] discountRounder = new double[] {0.0,0.0,0.0,0.0,0.0};

                DateTime t0 = DateTime.Now;
                DateTime ts = DateTime.Now;

                
                //bool rounderSum = false; 
                for (i = _startFrom; i <= _table.GetUpperBound(0); i++)
                {
                    if (i == rows)
                    {
                        if (_table[i, _ind_itemSum] == null)
                            continue;
                    }
                    subError = "Адрес покупателя";
                    if(!string.IsNullOrEmpty(buyerAddressCurrentLine))
                    {
                        buyerAddressLastLine = buyerAddressCurrentLine;
                    }
                    buyerAddressCurrentLine = string.Empty;
                    if (_ind_buyerAddress>0)
                    {
                        if (_table[i, _ind_buyerAddress] != null)
                        {
                            buyerAddressCurrentLine = _table[i,_ind_buyerAddress].ToString();
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(_buyerAddressDefault))
                        {
                            buyerAddressCurrentLine = _buyerAddressDefault;
                        }
                    }

                    subError = "1192 доп. реквизит чека";
                    if(!string.IsNullOrEmpty(propertiesDataCurrentLine))
                        propertiesDataLastLine = propertiesDataCurrentLine;
                    propertiesDataCurrentLine = null;

                    if (_ind_propertiesData > 0 && _table[i,_ind_propertiesData]!=null)
                    {
                        if (_table[i, _ind_propertiesData] is string && (_table[i, _ind_propertiesData] as string).Length > 0)
                            propertiesDataCurrentLine = _table[i, _ind_propertiesData] as string;
                        else
                            propertiesDataCurrentLine = _table[i, _ind_propertiesData].ToString();
                        if(propertiesDataCurrentLine != null && (propertiesDataCurrentLine.Trim().Length == 0 || propertiesDataCurrentLine == "0"))
                        {
                            propertiesDataCurrentLine = string.Empty;
                        }
                    }
                    subError = "1084";
                    if(!string.IsNullOrEmpty(userPropertiesPropertyNameCurrentLine))
                        userPropertiesPropertyNameLastLine = userPropertiesPropertyNameCurrentLine;
                    userPropertiesPropertyNameCurrentLine = null;
                    if (!string.IsNullOrEmpty(userPropertiesPropertyValueCurrentLine))
                        userPropertiesPropertyValueLastLine = userPropertiesPropertyValueCurrentLine;
                    userPropertiesPropertyValueCurrentLine = null;
                    if (_ind_userPropertiesPropertyName > 0 
                        && _ind_userPropertiesPropertyValue > 0 
                        && _table[i, _ind_userPropertiesPropertyName] != null 
                        && _table[i, _ind_userPropertiesPropertyValue] != null)
                    {
                        userPropertiesPropertyNameCurrentLine = _table[i, _ind_userPropertiesPropertyName].ToString();
                        userPropertiesPropertyValueCurrentLine = _table[i, _ind_userPropertiesPropertyValue].ToString();

                    }


                    subError = "";
                    if (errorsInSession >= _errorsAllowed)
                    {
                        _interruptor = true;
                        BeginInvoke(new Action(() => label_rate.Text += "\t\t\t\t   обработка прервана"));
                        throw new Exception("Достигнут допустимый лимит ошибок при обработке.");
                    }

                    if (i % percent == 0)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            int progressPs = (int)Math.Round(100.0 * (i - _startFrom) / (rows - _startFrom));
                            if(progressPs>100)
                                progressPs = 100;
                            if(progressPs<0)
                                progressPs = 0;
                            progressBar_excelLines.Value = progressPs;
                        }));
                        
                    }
                    if(i % 16 == 0)
                    {
                        DateTime t1 = DateTime.Now;
                        double deltaT = (t1 - t0).TotalMilliseconds;
                        double rate = Math.Round(16.0*60000.0/deltaT,1);
                        double absRate = (60 * (i - _startFrom) / (t1 - ts).TotalSeconds);

                        int remainMin = (int)Math.Round((rows - i) / (((double)(i - _startFrom)) * 60.0 / (t1 - ts).TotalSeconds));
                        BeginInvoke(new Action(() => label_rate.Text = "Обрабатывается " + Math.Round(absRate,1) + " строк в минуту    строка:"+i +" из "+rows+"  примерно осталось "+(remainMin/60) + "ч: " + (remainMin % 60) + " мин"
                        + Environment.NewLine+errorsInSession+" ошибок ФР")); 
                        t0 =t1;
                    }
                    if (_extendedReport)
                    {
                        List<object> exline = new List<object>();
                        for(int j = 0; j < _table.GetUpperBound(1); j++) exline.Add(_table[i,j]);
                        reporter.ExcelLine(i, exline);
                    }
                    else
                    {
                        reporter.ExcelLine(i);
                    }
                    if (_interruptor)
                    {
                        LogHandle.ol("Операция прервана пользователем");
                        BeginInvoke(new Action(() => label_rate.Text += "\t\t\t\t   обработка прервана"));
                        break;
                    }
                    int currentCheqNumber = 0;
                    string currentPaymentType = "";
                    subError = " номер чека ";
                    if (_yarusNumbersQueue && _table[i, _ind_cheqNumber] != null)
                    {
                        int chNum = -1;
                        if (_table[i, _ind_cheqNumber] is string)
                        {
                            if (!int.TryParse(_table[i, _ind_cheqNumber] as string, out chNum))
                            {
                                double d = -1;
                                double.TryParse(_table[i, _ind_cheqNumber] as string, out d);
                                chNum = (int)Math.Round(d, 0);
                            }
                        }
                        else if (_table[i, _ind_cheqNumber] is int)
                        {
                            chNum = (int)_table[i, _ind_cheqNumber];
                        }
                        else if (_table[i, _ind_cheqNumber] is double)
                        {
                            chNum = (int)Math.Round((double)_table[i, _ind_cheqNumber]);
                        }
                        else if (_table[i, _ind_cheqNumber] is decimal)
                        {
                            chNum = (int)Math.Round((decimal)_table[i, _ind_cheqNumber]);
                        }
                        if (chNum == cheqNumberLast)
                            _table[i, _ind_cheqNumber] = null;
                    }

                    if (_table[i, _ind_cheqNumber] != null)
                    {

                        _fr.ReadDeviceCondition();
                        int lastFd = _fr.LastFd;
                        reporter.ExcelExtraLine("обнаружен новый чек - закрываем чек предыдущей строки");
                        // обнаружен новый чек или окончание предыдущего чека
                        // закрываем предыдущий чек
                        if (cheque != null) 
                        {
                            if (_table[i, _ind_cheqNumber] is string && (_table[i, _ind_cheqNumber] as string).EndsWith(" всего"))
                            {
                                // текущий чек заканчивается на этой строке на следующей строке будет новый чек или закрытие смены
                                if (!string.IsNullOrEmpty(propertiesDataCurrentLine))
                                {
                                    cheque.PropertiesData = propertiesDataCurrentLine;
                                    propertiesDataLastLine = string.Empty;
                                    propertiesDataCurrentLine = string.Empty;
                                }
                                if (!string.IsNullOrEmpty(userPropertiesPropertyNameCurrentLine) && !string.IsNullOrEmpty(userPropertiesPropertyValueCurrentLine))
                                {
                                    cheque.PropertiesPropertyName = userPropertiesPropertyNameCurrentLine;
                                    cheque.PropertiesPropertyValue = userPropertiesPropertyValueCurrentLine;
                                    userPropertiesPropertyValueCurrentLine = string.Empty;
                                    userPropertiesPropertyNameCurrentLine = string.Empty;
                                    userPropertiesPropertyValueLastLine = string.Empty;
                                    userPropertiesPropertyNameLastLine = string.Empty;
                                }
                                if (!string.IsNullOrEmpty(buyerAddressCurrentLine))
                                {
                                    cheque.EmailPhone = buyerAddressCurrentLine;
                                    buyerAddressCurrentLine = string.Empty;
                                    buyerAddressLastLine = string.Empty;
                                }

                            }

                            if (!string.IsNullOrEmpty(propertiesDataLastLine))
                            {
                                cheque.PropertiesData = propertiesDataLastLine;
                                propertiesDataLastLine = propertiesDataCurrentLine;
                                propertiesDataCurrentLine = string.Empty;
                            }
                            if (!string.IsNullOrEmpty(buyerAddressLastLine))
                            {
                                cheque.EmailPhone = buyerAddressLastLine;
                                buyerAddressLastLine = buyerAddressCurrentLine;
                                buyerAddressCurrentLine = string.Empty;
                            }
                            if (!string.IsNullOrEmpty(userPropertiesPropertyNameLastLine) && !string.IsNullOrEmpty(userPropertiesPropertyValueLastLine))
                            {
                                cheque.PropertiesPropertyName = userPropertiesPropertyNameLastLine;
                                userPropertiesPropertyNameLastLine = userPropertiesPropertyNameCurrentLine;
                                userPropertiesPropertyNameCurrentLine= string.Empty;
                                cheque.PropertiesPropertyValue = userPropertiesPropertyValueLastLine;
                                userPropertiesPropertyValueLastLine = userPropertiesPropertyValueCurrentLine;
                                userPropertiesPropertyValueCurrentLine = string.Empty;
                            }


                            cheque.Control();

                            double discRnd = 0.0;
                            foreach (var d in discountRounder) discRnd += d;
                            if (discRnd>0.0099)
                            {
                                LogHandle.ol("Компенсация округления");
                                cheque.TotalSum =   Math.Round(cheque.TotalSum  - discRnd, 2);
                                cheque.Cash =       Math.Round(cheque.Cash      - discountRounder[0], 2);
                                cheque.ECash =      Math.Round(cheque.ECash     - discountRounder[1], 2);
                                cheque.Prepaid =    Math.Round(cheque.Prepaid   - discountRounder[2], 2);
                                cheque.Credit =     Math.Round(cheque.Credit    - discountRounder[3], 2);
                                cheque.Provision =  Math.Round(cheque.Provision - discountRounder[4], 2);


                            }
                            discountRounder = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0 };

                            //emuCollector.Add(cheque);
                            if (cheque.TotalSum > 0.0090)
                            {
                                reporter.ExcelExtraLine((i - 1).ToString());
                                if (!_fr.PerformFD(cheque))
                                {
                                    MassActionReporter.AppendCorrFd(cheque, false);
                                    // проверяем ФР
                                    _fr.ReadDeviceCondition();
                                    if (lastFd + 1 == _fr.LastFd)
                                    {
                                        reporter.ExcelExtraLine("Номер документа в ФН увеличился на 1 продолжаем операцию. ФД:" + _fr.LastFd);
                                    }
                                    else
                                    {
                                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                        // исключительная сиуация
                                        if (lastFd == _fr.LastFd)
                                        {
                                            _fr.CancelDocument();
                                            errorsInSession++;
                                            // чек не был оформлен
                                            // пытаеся решить проблему исчерпания ресурса хранения ФН
                                            bool closeShift = _fr.CloseShift();
                                            reporter.ExcelExtraLine("Пытаемся закрыть смену ");
                                            bool openShift = _fr.OpenShift();
                                            reporter.ExcelExtraLine("Пытаемся открыть смену ");
                                            _fr.ReadDeviceCondition();
                                            if (lastFd < _fr.LastFd)
                                            {
                                                reporter.ExcelExtraLine("Закрытие-открытие смены увеличило ФД пытаемся пробить чек повторно");
                                                bool cheqoperation = _fr.PerformFD(cheque);
                                                if (cheqoperation)
                                                {
                                                    MassActionReporter.AppendCorrFd(cheque, true);
                                                }
                                                else
                                                {
                                                    reporter.ExcelExtraLine(FiscalPrinter.KKMInfoTransmitter[FiscalPrinter.FR_LAST_ERROR_MSG_KEY]);
                                                    errorsInSession++;
                                                    errorList.Add(i - 1);
                                                    if(++errorsInSession >= _errorsAllowed)// если превышен допустимый уроень ошиюок 
                                                        throw new Exception("Превышен допустимый уроень ошибок: "); //прерываем операцию
                                                }
                                            }

                                        }
                                        
                                    }
                                }
                                else
                                {
                                    MassActionReporter.AppendCorrFd(cheque, true);
                                }
                            }
                            else
                            {
                                reporter.ExcelExtraLine("Передача нулевого чека - пропускаем операцию");
                            }
                            
                        }
                        else
                        {
                            discountRounder = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
                            LogHandle.ol("попытка передать пустой документ - строка " + i);
                        }

                        
                        
                        cheque = null;
                        if (_table[i, _ind_cheqNumber] is string && (_table[i, _ind_cheqNumber] as string).EndsWith(" всего"))
                        {
                            // окончание предыдущего чека с комбинированной оплатой
                            // дальнеших действий со строкой не требуется
                            paymentTypeLast = "";
                            propertiesDataLastLine = string.Empty;
                            propertiesDataCurrentLine = string.Empty;
                            buyerAddressCurrentLine = string.Empty;
                            buyerAddressLastLine = string.Empty;
                            userPropertiesPropertyNameCurrentLine = string.Empty;
                            userPropertiesPropertyNameLastLine = string.Empty;
                            userPropertiesPropertyValueCurrentLine = string.Empty;
                            userPropertiesPropertyValueLastLine = string.Empty;

                            continue;
                        }
                        //chequesTotalCount++;
                        
                        currentCheqNumber = _table[i, _ind_cheqNumber] is string ? int.Parse(_table[i, _ind_cheqNumber] as string) : (int)((double)_table[i, _ind_cheqNumber]);
                        if (currentCheqNumber != cheqNumberLast || currentCheqNumber == 1 || cheqNumberLast == 0)// учитывая что чеки идут с пропусками номеров скорей всего это условие участок стоит удалить
                        {
                            
                            cheqNumberLast = currentCheqNumber;
                        }
                        else
                        {
                            // прерываем обработку файла из-за некорректного номера чека
                            throw new Exception("Ошибка с порядком номеров чеков предыдущий номер чека " + cheqNumberLast + " чек в текущей строке " + currentCheqNumber);
                        }
                    }

                    subError = " наименование номенклатуры ";
                    if (_table[i, _ind_itemName] != null)
                    {
                        // непустая товарная позиция
                        if (_table[i, _ind_itemName] is string && (_table[i, _ind_itemName] as string) != "") // !!!!!!обработать условие для авансов, наверно в блоке else{}
                        {
                            // считываем реквизиты предмета расчета
                            string itemName = _table[i, _ind_itemName] as string;
                            double itemSum = 0;
                            double itemQuantity = 0;
                            int taxRate = 0;
                            int itemPropertieProductType = _itemProductTypeDefault;

                            subError = " дата коррекции ";
                            if (useTableCorrectionDate && _table[i, _ind_correctionDate] != null )
                            {
                                if (_table[i, _ind_correctionDate] is DateTime)
                                    correctionDate = (DateTime)_table[i, _ind_correctionDate];
                                else
                                    correctionDate = DateTime.ParseExact(_table[i, _ind_correctionDate] as string,"dd.MM.yyyy",CultureInfo.InvariantCulture);
                            }
                                
                            subError = " сумма позиции ";
                            if (_table[i, _ind_itemSum] != null)
                            {
                                if(_table[i, _ind_itemSum] is string) itemSum = Math.Round(double.Parse(_table[i, _ind_itemSum] as string),2);
                                else itemSum = Math.Round((double)_table[i, _ind_itemSum],2);
                            }
                            else
                            {
                                throw new Exception("Отсутсвует сумма за предмет расчета в стоблце " + (char)('A' + _ind_itemSum - 1) + " номер стоблца " + _ind_itemSum);
                            }
                            subError = " количество ";
                            if (_table[i, _ind_itemQuantity] != null)
                                if(_table[i, _ind_itemQuantity] is string) itemQuantity = double.Parse(_table[i, _ind_itemQuantity] as string);
                                else itemQuantity = (double)_table[i, _ind_itemQuantity];
                            else
                            {
                                throw new Exception("Отсутсвует количество предмета расчета в стоблце " + (char)('A' + _ind_itemQuantity - 1) + "(" + _ind_itemQuantity + ")");
                            }
                            subError = " тип оплаты ";
                            if (_table[i, _ind_paymentType] == null)
                                currentPaymentType = paymentTypeLast;
                            else
                            {
                                currentPaymentType = _table[i, _ind_paymentType] as string;
                                paymentTypeLast = currentPaymentType;
                                // данный блок сдесь для контроля формата таблицы
                            }
                            subError = " ставка налога";

                            if (_taxRateDafault > 0)
                            {
                                taxRate = _taxRateDafault;
                            }
                            if (_ind_itemNdsRate > 0)
                            {
                                int tax = -1;
                                if (_table[i, _ind_itemNdsRate] is string)
                                    tax = (int)Math.Round(double.Parse(_table[i, _ind_itemNdsRate] as string));
                                else if (_table[i, _ind_itemNdsRate] is double)
                                    tax = (int)Math.Round((double)_table[i, _ind_itemNdsRate]);
                                else if (_table[i, _ind_itemNdsRate] is int)
                                    tax = (int)_table[i, _ind_itemNdsRate];
                                if (tax < 1 || tax > 10)
                                {
                                    throw new Exception("Ставка налога выходит за установленный диапазон[1-10]: " + tax);
                                }
                                taxRate = tax;
                            }
                            subError = " Признак предмета расчета.";
                            if (_ind_itemPropertieProductType > 0 && _table[i, _ind_itemPropertieProductType]!=null)
                            {
                                int payment_type = -1;
                                object opt = _table[i, _ind_itemPropertieProductType];
                                if (opt is string)
                                    payment_type = (int)Math.Round(double.Parse(opt as string));
                                else if (opt is double)
                                    payment_type = (int)Math.Round((double)opt);
                                else if (_table[i, _ind_itemPropertieProductType] is int)
                                    payment_type = (int)_table[i, _ind_itemPropertieProductType];
                                if (payment_type < 1 || payment_type > 33 || payment_type==28 || payment_type ==29)
                                {
                                    payment_type = _itemProductTypeDefault;
                                    if (payment_type < 1 || payment_type > 33 || payment_type == 28 || payment_type == 29)
                                        throw new Exception("Признак предмета расчета выходит за установленный диапазон[1-27][30-33]: " + payment_type);
                                }
                                itemPropertieProductType = payment_type;
                            }

                            subError = "";


                            if (cheque == null)
                            {
                                cheque = new FiscalCheque();
                                cheque.Sno = _sno;
                                cheque.CalculationSign = _calculationSign;
                                if (_useCorrectionCheque)
                                {
                                    cheque.Document = FiscalPrinter.FD_DOCUMENT_NAME_CORRECTION_CHEQUE;
                                    cheque.CorrectionTypeNotFtag = _correctionType;
                                    cheque.CorrectionDocumentDate = correctionDate;
                                    cheque.CorrectionDocDescriber = _correctionDescriber;
                                    cheque.CorrectionOrderNumber = _correctionOrderNumber;
                                }
                            }
                            if (!string.IsNullOrEmpty(propertiesDataCurrentLine))
                            {
                                cheque.PropertiesData = propertiesDataCurrentLine;
                                propertiesDataCurrentLine = string.Empty;
                                propertiesDataLastLine = string.Empty;
                            }
                            if (!string.IsNullOrEmpty(propertiesDataLastLine)&&string.IsNullOrEmpty(cheque.PropertiesData))
                            {
                                cheque.PropertiesData = propertiesDataLastLine;
                                propertiesDataLastLine = string.Empty;
                            }
                            if (!string.IsNullOrEmpty(buyerAddressCurrentLine))
                            {
                                cheque.EmailPhone = buyerAddressCurrentLine;
                                buyerAddressCurrentLine = string.Empty;
                                buyerAddressLastLine = string.Empty;
                            }
                            if (!string.IsNullOrEmpty(buyerAddressLastLine))
                            {
                                cheque.EmailPhone = buyerAddressLastLine;
                                buyerAddressLastLine = string.Empty ;
                            }
                            if (!string.IsNullOrEmpty(userPropertiesPropertyNameCurrentLine)&&!string.IsNullOrEmpty(userPropertiesPropertyValueCurrentLine))
                            {
                                cheque.PropertiesPropertyName = userPropertiesPropertyNameCurrentLine;
                                cheque.PropertiesPropertyValue = userPropertiesPropertyValueCurrentLine;
                                userPropertiesPropertyNameCurrentLine = string.Empty;
                                userPropertiesPropertyNameLastLine = string.Empty;
                                userPropertiesPropertyValueCurrentLine = string.Empty;
                                userPropertiesPropertyValueLastLine = string.Empty;
                            }
                            if (!string.IsNullOrEmpty(userPropertiesPropertyNameLastLine) && !string.IsNullOrEmpty(userPropertiesPropertyValueLastLine))
                            {
                                cheque.PropertiesPropertyName = userPropertiesPropertyNameLastLine;
                                cheque.PropertiesPropertyValue = userPropertiesPropertyValueLastLine;
                                userPropertiesPropertyNameCurrentLine = string.Empty;
                                userPropertiesPropertyNameLastLine = string.Empty;
                                userPropertiesPropertyValueCurrentLine = string.Empty;
                                userPropertiesPropertyValueLastLine = string.Empty;
                            }


                            if (itemQuantity > 0.0001)
                            {
                                if (itemSum<0)
                                {
                                    LogHandle.ol("Округление чека " + itemSum);
                                    switch (paymentTypeIndex[currentPaymentType])
                                    {
                                        case 0:
                                            discountRounder[0] += -itemSum;
                                            break;
                                        case 1:
                                            discountRounder[1] += -itemSum;
                                            break;
                                        case 2:
                                            discountRounder[2] += -itemSum;
                                            break;
                                        case 3:
                                            discountRounder[3] += -itemSum;
                                            break;
                                        case 4:
                                            discountRounder[4] += -itemSum;
                                            break;
                                        default:
                                            throw new Exception("Некорректный или незаполненный платеж");
                                            //break;
                                    }
                                    continue;
                                }
                                double itemPrice = Math.Round(itemSum / itemQuantity, 2);

                                if (_fractionalQuantitiesRule == 1)
                                {
                                    if(Math.Abs(itemQuantity - Math.Round(itemQuantity)) > 0.0001)
                                    {
                                        itemName += " " + itemQuantity.ToString("N3") + " шт.";
                                        itemQuantity = 1.0;
                                        itemPrice = itemSum;
                                    }
                                }
                                else if(_fractionalQuantitiesRule == 2)
                                {
                                    itemName += " " + itemQuantity.ToString() + " шт.";
                                    itemQuantity = 1.0;
                                    itemPrice = itemSum;
                                }
                                else if(_fractionalQuantitiesRule == 3)
                                {
                                    if (itemSum > 0.009) 
                                    { 
                                        itemQuantity = Math.Round(itemSum / itemPrice, _fraqPrecision); 
                                    }
                                    
                                }

                                ConsumptionItem item = new ConsumptionItem(itemName, itemPrice, itemQuantity, itemSum, itemPropertieProductType, _itemPaymentTypeDefault, taxRate);
                                item.Unit120 = _itemMeasureUnit120;
                                cheque.AddItem(item);

                                switch (paymentTypeIndex[currentPaymentType])
                                {
                                    case 0:
                                        cheque.Cash += itemSum;
                                        break;
                                    case 1:
                                        cheque.ECash += itemSum;
                                        break;
                                    case 2:
                                        cheque.Prepaid += itemSum;
                                        break;
                                    case 3:
                                        cheque.Credit += itemSum;
                                        break;
                                    case 4:
                                        cheque.Provision += itemSum;
                                        break;
                                    default:
                                        throw new Exception("Некорректный или незаполненный платеж");
                                        //break;
                                }
                            }
                            else
                            {
                                reporter.ExcelExtraLine("в строке "+i+" предмет расчета с количеством 0 - пропускаем.");
                            }
                            

                        }
                        else
                        {
                            // прерываем обработку файла из-за некорректного названия товарной позиции
                            throw new Exception("Название предмета расчета не является строкой или строка пустая " + _table[i, _ind_itemName].GetType() + "\t " + _table[i, _ind_itemName].ToString());

                        }
                    }


                }
                if (errorList.Count > 0)
                {
                    string errors = "Строки с ошибками ФР: ";
                    foreach (var errr in errorList)
                        errors += errr.ToString() + ";";
                    reporter.ExcelExtraLine(errors);
                }
                if (!_interruptor && cheque != null && cheque.Items.Count != 0)
                {
                    cheque.Control();
                    Invoke(new Action(() => progressBar_excelLines.Value = 100));
                    MassActionReporter.AppendCorrFd(cheque, _fr.PerformFD(cheque));
                }
                if (!_interruptor) Invoke(new Action(() => progressBar_excelLines.PerformStep()));
            }
            catch (Exception ex)
            {
                LogHandle.ol("Строка " + i + " "+ subError + ex.Message);
                Invoke(new Action(() => textBox_precalculateTable.Text = "Строка " + i + Environment.NewLine + ex.Message));
                
            }

            if(i+1 >= _table.GetUpperBound(0))
            {
                Invoke(new Action(() =>label_startedIndicator.Text = "Корректировка успешно завершена"));
            }
            else
            {
                Invoke(new Action(() => label_startedIndicator.Text = "Корректировка прервана"));
            }
            _fe.FormCloseEnabled(this, true);
            if (InvokeRequired) BeginInvoke(new Action(() => button_interruptor.Enabled = false));
            return;
        }

        private void OlapReportV2_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainWindow.DontPrintFlag = _mainWindowDontPrintFlagOriginal;
        }




        private void _CheckQuantitiesValues()
        {
            if (_table == null)
            {
                textBox_FQ_quantitiesPreprocessingResult.Text = "Данные не загружены";
                return;
            }
            
            int quaNum = 0,
                errors = 0;
            bool fractionalValues = false;
            for (int i = _startFrom; i < _table.GetUpperBound(0); i++)
            {
                if(errors > 111)
                {
                    goto BadColumn;
                }

                object quantity = _table[i, _ind_itemQuantity];
                if(quantity == null) 
                    continue;
                try
                {
                    double d = 0.0;
                    if (quantity is string)
                    {
                        if ((quantity as string).EndsWith("всего"))
                            continue;
                        d = double.Parse(quantity as string);

                    }
                    else 
                        d =  (double) quantity;
                    quaNum++;
                    if(!fractionalValues && d != 0.0 && Math.Abs(Math.Truncate(d)-d)>=0.001)
                        fractionalValues = true;
                }
                catch
                {
                    errors++;
                    continue;
                }
            }
            label_fractionalQuantitiesMessage.Visible = fractionalValues;
            groupBox_fractionalQuantitiesSettings.Enabled = fractionalValues;

            textBox_FQ_quantitiesPreprocessingResult.Text = "Обнаружено "+quaNum + " значений";
            if (fractionalValues)
            {
                radioButton_FQ_deepCalculate.Checked = true;
                textBox_FQ_quantitiesPreprocessingResult.Text += " присутсвует дробное количество/весовой товар";
            }
            
            return;

        BadColumn:
            textBox_FQ_quantitiesPreprocessingResult.Text = "Проверка прервана. Возможно выбран некорректный столбец - более 100 ошибочных значений";

        }

        private int _unlockerFraqtionalOption = 0;
        private void label6_Click(object sender, EventArgs e)
        {
            if (_unlockerFraqtionalOption++ > 5)
            {
                groupBox_fractionalQuantitiesSettings.Enabled = true;
            }
        }


        
    }
}
