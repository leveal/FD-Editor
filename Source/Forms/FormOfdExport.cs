using FR_Operator.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FR_Operator
{
    public partial class FormOfdExport: Form
    {
        public FormOfdExport(FiscalPrinter kkt)
        {
            InitializeComponent();
            this.Icon = Resources.fd_editpr_16_2;

            if (!string.IsNullOrEmpty(AppSettings.ItemName))
            {
                _itemsNameDefault = AppSettings.ItemName;
            }
            _overrideAddressOriginal = AppSettings.OverideRetailAddress;
            _overridePlaceOriginal = AppSettings.OverideRetailPlace;
            fiscalPrinter = kkt;
            form = this;
            log = richTextBox_log;
            skip_handle_sign = true;
            textBox_startFrom.Text = _startIndex.ToString();
            textBox_lastReportLine.Text = _endIndex.ToString();
            comboBox_checkId.SelectedIndex = _pointer_checkId;
            comboBox_docTypeChooser.SelectedIndex = _pointer_documentTypeM2;
            comboBox_correctionDate.SelectedIndex = _pointer_correctionDate;
            comboBox_correctionType.SelectedIndex = _pointer_correctionTypeM1;
            comboBox_correctionOrderNumber.SelectedIndex = _pointer_correctionOrderNumber;
            textBox_correctionOrderNumber.Text = _correctionOrderNumberDefault;
            comboBox_itemsName.SelectedIndex = _pointer_itemsName;
            comboBox_itemsQuantity.SelectedIndex = _pointer_itemsQuantity;
            comboBox_itemsPaymentTypeSign.SelectedIndex = _pointer_itemsPaymentTypeSignM7;
            comboBox_itemsPaymentTypeSignDefault.SelectedIndex = 4;
            comboBox_itemsProductType.SelectedIndex = _pointer_itemsProductTypeM33;
            comboBox_itemsProductTypeDefault.SelectedIndex = 1;
            textBox_itemsQuantityDefault.Text = _itemsQuantityDefault.ToString();
            textBox_itemsQuantityDefault.ForeColor = Color.Black;
            textBox_itemsNameDefault.Text = _itemsNameDefault;
            comboBox_itemsPrice.SelectedIndex = _pointer_itemsPrice;
            comboBox_itemsSum.SelectedIndex = _pointer_itemsSum;
            comboBox_itemsUnit120.SelectedIndex = _pointer_itemsUnit120;
            comboBox_itemsNdsRate.SelectedIndex = _pointer_itemsNdsRate;
            comboBox_itemsNdsRateDefault.SelectedIndex = _itemsNdsRateDefault;
            comboBox_cashier.SelectedIndex = _pointer_cashier;
            textBox_cashierDefault.Text = _cashierDefault.ToString();
            comboBox_selectedSno.SelectedIndex = _pointer_sno;
            if (_snoDefault == 1) 
            { 
                comboBox_snoDefault.SelectedIndex = 0; 
            }
            else if (_snoDefault == 2)
            {
                comboBox_snoDefault.SelectedIndex = 1;
            }
            else if(_snoDefault == 4)
            {
                comboBox_snoDefault.SelectedIndex = 2;
            }
            else if (_snoDefault == 16)
            {
                comboBox_snoDefault.SelectedIndex = 3;
            }
            else if (_snoDefault == 32)
            {
                comboBox_snoDefault.SelectedIndex = 4;
            }
            if (_itemsUnit120Default == 0)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 0;
            }
            else if (_itemsUnit120Default == 10)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 1;
            }
            else if (_itemsUnit120Default == 11)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 2;
            }
            else if (_itemsUnit120Default == 12)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 3;
            }
            else if (_itemsUnit120Default == 20)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 4;
            }
            else if (_itemsUnit120Default == 21)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 5;
            }
            else if (_itemsUnit120Default == 22)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 6;
            }
            else if (_itemsUnit120Default == 30)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 7;
            }
            else if (_itemsUnit120Default == 31)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 8;
            }
            else if (_itemsUnit120Default == 32)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 9;
            }
            else if (_itemsUnit120Default == 40)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 10;
            }
            else if (_itemsUnit120Default == 41)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 11;
            }
            else if (_itemsUnit120Default == 42)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 12;
            }
            else if (_itemsUnit120Default == 50)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 13;
            }
            else if (_itemsUnit120Default == 51)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 14;
            }
            else if (_itemsUnit120Default == 70)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 15;
            }
            else if (_itemsUnit120Default == 71)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 16;
            }
            else if (_itemsUnit120Default == 72)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 17;
            }
            else if (_itemsUnit120Default == 73)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 18;
            }
            else if (_itemsUnit120Default == 80)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 19;
            }
            else if (_itemsUnit120Default == 81)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 20;
            }
            else if (_itemsUnit120Default == 82)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 21;
            }
            else if (_itemsUnit120Default == 83)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 22;
            }
            else if (_itemsUnit120Default == 255)
            {
                comboBox_itemsUnit120Default.SelectedIndex = 23;
            }
            comboBox_checkPaidCash.SelectedIndex = _pointer_checkPaidCash;
            comboBox_checkPaidEcash.SelectedIndex = _pointer_checkPaidEcash;
            comboBox_checkPaidPrepaid.SelectedIndex = _pointer_checkPaidPrepaid;
            comboBox_checkPaidCredit.SelectedIndex = _pointer_checkPaidCredit;
            comboBox_checkPaidProvision.SelectedIndex = _pointer_checkPaidProvision;
            comboBox_propertiesData.SelectedIndex = _pointer_PropiertyData;
            checkBox_closeShiftSign.Checked = _closeShiftOnEroor;
            textBox_dtFormat.Text = _dtFormat;

            checkBox_enableManualTaxes.Checked = _manualTaxes;
            groupBox_manualTax.Enabled = _manualTaxes;
            comboBox_checkTax20.SelectedIndex = _pointer_checkTax_20;
            comboBox_checkTax10.SelectedIndex = _pointer_checkTax_10;
            comboBox_checkTax20120.SelectedIndex = _pointer_checkTax_20120;
            comboBox_checkTax10110.SelectedIndex = _pointer_checkTax_10110;
            comboBox_checkTax0.SelectedIndex = _pointer_checkTax_0;
            comboBox_checkTaxFree.SelectedIndex = _pointer_checkTax_free;
            comboBox_checkTax5.SelectedIndex = _pointer_checkTax_5;
            comboBox_checkTax7.SelectedIndex = _pointer_checkTax_7;
            comboBox_checkTax5105.SelectedIndex = _pointer_checkTax_5105;
            comboBox_checkTax7107.SelectedIndex = _pointer_checkTax_7107;
            comboBox_retailAddress.SelectedIndex = _pointer_retailAddress;
            comboBox_retailPlace.SelectedIndex = _pointer_retailPlace;

            if (fiscalPrinter != null)
            {
                _originalDontPrintSign = fiscalPrinter.DontPrint;
                // при закрытии формы восстановить знак ! ! !
                checkBox_dontPrint.Checked = fiscalPrinter.DontPrint;
            }
            comboBox_operationType.SelectedIndex = _pointer_operationTypeM5;
            comboBox_emailPhone.SelectedIndex = _pointer_emailPhone;


            // дальнейшие установки будут обрабатываться в хандлере
            skip_handle_sign = false;
            
            if (kkt!=null&&FiscalPrinter.KKMInfoTransmitter!= null && FiscalPrinter.KKMInfoTransmitter.ContainsKey(FiscalPrinter.FR_REGISTERD_SNO_KEY))
            {
                // устанавливаем сно по умолчанию
                if (FiscalPrinter.KKMInfoTransmitter[FiscalPrinter.FR_REGISTERD_SNO_KEY].Contains(FiscalPrinter.SNO_PSN))
                {
                    comboBox_snoDefault.SelectedIndex = 4;
                }
                else
                {
                    if (FiscalPrinter.KKMInfoTransmitter[FiscalPrinter.FR_REGISTERD_SNO_KEY].Contains(FiscalPrinter.SNO_ESHN))
                    {
                        comboBox_snoDefault.SelectedIndex = 3;
                    }
                    else
                    {
                        if (FiscalPrinter.KKMInfoTransmitter[FiscalPrinter.FR_REGISTERD_SNO_KEY].Contains(FiscalPrinter.SNO_USN_DR))
                        {
                            comboBox_snoDefault.SelectedIndex = 2;
                        }
                        else
                        {
                            if (FiscalPrinter.KKMInfoTransmitter[FiscalPrinter.FR_REGISTERD_SNO_KEY].Contains(FiscalPrinter.SNO_USN_DOHOD))
                            {
                                comboBox_snoDefault.SelectedIndex = 1;
                            }
                            else
                            {
                                if (FiscalPrinter.KKMInfoTransmitter[FiscalPrinter.FR_REGISTERD_SNO_KEY].Contains(FiscalPrinter.SNO_TRADITIONAL))
                                {
                                    comboBox_snoDefault.SelectedIndex = 0;
                                }
                            }
                        }
                    }
                }
                
            }

            comboSet.Add(comboBox_checkId);
            comboSet.Add(comboBox_operationType);
            comboSet.Add(comboBox_docTypeChooser);
            comboSet.Add(comboBox_propertiesData);
            comboSet.Add(comboBox_selectedSno);
            comboSet.Add(comboBox_cashier);
            comboSet.Add(comboBox_cashierInn);
            comboSet.Add(comboBox_correctionDate);
            comboSet.Add(comboBox_correctionType);
            comboSet.Add(comboBox_correctionOrderNumber);
            comboSet.Add(comboBox_itemsName);
            comboSet.Add(comboBox_itemsQuantity);
            comboSet.Add(comboBox_itemsPrice);
            comboSet.Add(comboBox_itemsSum);
            comboSet.Add(comboBox_itemsPaymentTypeSign);
            comboSet.Add(comboBox_itemsProductType);
            comboSet.Add(comboBox_itemsPaymentTypeSignDefault);
            comboSet.Add(comboBox_itemsProductTypeDefault);
            comboSet.Add(comboBox_itemsNdsRate);
            comboSet.Add(comboBox_itemsUnit120);
            comboSet.Add(comboBox_itemsNdsRateDefault);
            comboSet.Add(comboBox_itemsUnit120Default);
            comboSet.Add(comboBox_checkPaidCash);
            comboSet.Add(comboBox_checkPaidEcash);
            comboSet.Add(comboBox_checkPaidPrepaid);
            comboSet.Add(comboBox_checkPaidCredit);
            comboSet.Add(comboBox_checkPaidProvision);
            comboSet.Add(checkBox_enableManualTaxes);
            comboSet.Add(comboBox_checkTax20);
            comboSet.Add(comboBox_checkTax10);
            comboSet.Add(comboBox_checkTax20120);
            comboSet.Add(comboBox_checkTax10110);
            comboSet.Add(comboBox_checkTax0);
            comboSet.Add(comboBox_checkTaxFree);
            comboSet.Add(comboBox_checkTax5);
            comboSet.Add(comboBox_checkTax7);
            comboSet.Add(comboBox_checkTax5105);
            comboSet.Add(comboBox_checkTax7107);
            comboSet.Add(comboBox_snoDefault);
            comboSet.Add(checkBox_allowEmptyPropertyData);
            comboSet.Add(textBox_startFrom);
            comboSet.Add(comboBox_retailAddress);
            comboSet.Add(comboBox_retailPlace);
            comboSet.Add(comboBox_emailPhone);
            MapPresetts();
        }
        bool _overrideAddressOriginal = false;
        bool _overridePlaceOriginal = false;


        List<Control> comboSet = new List<Control>();


        public FiscalPrinter fiscalPrinter;

        object[,] data = null;
        int _startIndex = 2;
        int _endIndex = 999999;

        int _pointer_checkId = 0;
        
        int _pointer_operationTypeM5 = 1;
        Dictionary<string, int> _operationTypesMap = new Dictionary<string, int>();

        int _pointer_documentTypeM2 = 1;
        Dictionary<string, int> _docTypeMap = new Dictionary<string, int>();

        int _pointer_correctionDate = 0;
        string _dtFormat = "dd.MM.yyyy";

        int _pointer_correctionTypeM1 = 0;

        int _pointer_correctionOrderNumber = 0;
        string _correctionOrderNumberDefault = "Б/Н";

        int _pointer_itemsName = 0;
        string _itemsNameDefault = "Корректировка выручки";

        int _pointer_itemsQuantity = 0;
        double _itemsQuantityDefault = 1.0;

        int _pointer_itemsPrice = 0;
        int _pointer_itemsSum = 0;

        int _pointer_itemsPaymentTypeSignM7 = 0;
        int _itemsPaymentTypeSignDefault = 4;
        Dictionary<string, int> _itemsPaymentTypeMap = new Dictionary<string, int>();

        int _pointer_itemsProductTypeM33 = 0;
        int _itemsProductTypeDefault = 1;
        Dictionary<string, int> _itemsProductTypeMap =  new Dictionary<string, int>();

        int _pointer_itemsUnit120 = 0;
        int _itemsUnit120Default = 0;

        int _pointer_itemsNdsRate = 0;
        int _itemsNdsRateDefault = 6;

        int _pointer_checkPaidCash = 0;
        int _pointer_checkPaidEcash = 0;
        int _pointer_checkPaidPrepaid = 0;
        int _pointer_checkPaidCredit = 0;
        int _pointer_checkPaidProvision = 0;

        int _pointer_sno = 0;
        int _snoDefault = 1;

        int _pointer_PropiertyData = 0;
        int _pointer_cashier = 0;
        string _cashierDefault = string.IsNullOrEmpty(AppSettings.CashierDefault) ? FiscalPrinter.DEFAULT_CASHIER : AppSettings.CashierDefault; 

        bool _closeShiftOnEroor = true;

        bool _manualTaxes = false;
        int _pointer_checkTax_20 = 0;
        int _pointer_checkTax_10 = 0;
        int _pointer_checkTax_20120 = 0;
        int _pointer_checkTax_10110 = 0;
        int _pointer_checkTax_0 = 0;
        int _pointer_checkTax_free = 0;
        int _pointer_checkTax_5 = 0;
        int _pointer_checkTax_7 = 0;
        int _pointer_checkTax_5105 = 0;
        int _pointer_checkTax_7107 = 0;
        bool _allowEmptyPropertyData = false;

        int _pointer_retailAddress = 0;
        string _retailAddressDefalt = string.Empty;

        int _pointer_retailPlace = 0;
        string _retailPlaceDefalt = string.Empty;

        int _pointer_emailPhone = 0;
        string _emailPhoneDefault = string.Empty;

        public static bool breakOperation = false; 

        bool skip_handle_sign = false;
        private void EventHandler(object sender, EventArgs e)
        {
            if (skip_handle_sign)
            {
                return;
            }
            if (sender == button_openExcel)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    data = null;
                    Microsoft.Office.Interop.Excel.Application _excelApp = null;
                    Microsoft.Office.Interop.Excel.Workbook workbook = null;
                    try
                    {
                        _excelApp = new Microsoft.Office.Interop.Excel.Application();
                        _excelApp.Visible = false;

                        string fileName = openFileDialog.FileName;

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

                        data = (object[,])excelRange.get_Value(
                                    Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault);

                        AddMessage(data.GetUpperBound(0).ToString() + " строк в документе.");

                    }
                    catch (Exception ex)
                    {
                        //0x8007000Eif(E_OUTOFMEMORY)
                        AddMessage("MS Excel: " + ex.Message);
                        if (ex.Message.Contains("E_OUTOFMEMORY"))
                        {
                            AddMessage("\r\n\r\nпопробуйте сохранить отчет в формате Excel 97-2003\r\nи открыть заново");
                        }
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
                    if (data != null && data.Length > 0)
                    {
                        tabControl1.Enabled = true;
                    }
                    else
                    {
                        tabControl1.Enabled = false;
                    }
                }
            }
            else if (sender == textBox_startFrom)
            {
                int t = 2;
                if (int.TryParse(textBox_startFrom.Text, out t))
                {
                    if (t > 0)
                    {
                        AddMessage("Стартовая строка установлена: " + t);
                        _startIndex = t;
                    }
                    else
                    {
                        AddMessage("Некорректное значение стартовой строки, установлено 2");
                        _startIndex = 2;
                    }
                    textBox_startFrom.ForeColor = Color.Black;
                }
                else
                {
                    AddMessage("Невозможно разобрать значение стартовой строки, установлено 2");
                    _startIndex = 2;
                    textBox_startFrom.ForeColor = Color.Red;
                }
            }
            else if (sender == textBox_lastReportLine)
            {
                int t = 99999;
                if (int.TryParse(textBox_lastReportLine.Text, out t))
                {
                    if (t > 0)
                    {
                        AddMessage("Конечная строка установлена: " + t);
                        _endIndex = t;
                    }
                    else
                    {
                        AddMessage("Некорректное значение конечной строки, установлено 99999");
                        _endIndex = 99999;
                    }
                    textBox_lastReportLine.ForeColor = Color.Black;
                }
                else
                {
                    AddMessage("Невозможно разобрать значение конечной строки, установлено 99999");
                    _endIndex = 99999;
                    textBox_lastReportLine.ForeColor = Color.Red;
                }
            }
            else if (sender == comboBox_checkId)
            {
                _pointer_checkId = comboBox_checkId.SelectedIndex;
                if (_pointer_checkId == 0)
                {
                    AddMessage(data.GetUpperBound(0) + " значений в таблице.");
                }
                else
                {
                    if (_pointer_checkId > 0 && _pointer_checkId < data.GetUpperBound(1))
                    {
                        // корректное значение
                        int checkes = 0;
                        bool error = false;
                        string lastId = "AAAAAFFFFFFaaaafffffBBBBBBbbbbbbDDDDDdddddd1234567890-1234567890";
                        for (int i = _startIndex; i < _endIndex && i <= data.GetUpperBound(0); i++)
                        {
                            if (data[i, _pointer_checkId] == null)
                            {
                                AddMessage("Ошибка! Строка: " + i + " - пустрое значение");
                                break;
                            }
                            if (data[i, _pointer_checkId].ToString() != lastId)
                            {
                                checkes++;
                                lastId = data[i, _pointer_checkId].ToString();
                            }
                        }
                        if (!error)
                        {
                            AddMessage(checkes + " чеков обнаружено");
                        }
                    }
                    else
                    {
                        AddMessage("Установлено некорректное значение, применится настройка каждач строка новый чек");
                        _pointer_checkId = 0;
                    }
                }
            }
            else if (sender == comboBox_operationType)
            {
                _pointer_operationTypeM5 = comboBox_operationType.SelectedIndex;
                dataGridView_operationTypeMap.Rows.Clear();

                if (_pointer_operationTypeM5 > 4)
                {
                    int operationTypeIndex = _pointer_operationTypeM5 - 4;
                    List<string> op_types = new List<string>();
                    for (int i = _startIndex; i <= data.GetUpperBound(0) && i < _endIndex; i++)
                    {
                        if (data[i, operationTypeIndex] == null)
                        {
                            AddMessage("Ошибка! Строка: " + i + " - пустрое значение");
                            break;
                        }
                        if (!op_types.Contains(data[i, operationTypeIndex].ToString()))
                        {
                            op_types.Add(data[i, operationTypeIndex].ToString());
                        }
                    }
                    if (op_types.Count > 20)
                    {
                        AddMessage("Ошибка! Слишком много значений в выбранном столбце");
                    }
                    else
                    {
                        dataGridView_operationTypeMap.Rows.Clear();
                        foreach (var opType in op_types)
                        {
                            string opTypeFounded = "";
                            if (opType == "1" || opType.ToLower() == "приход")
                            {
                                opTypeFounded = "1 Приход";
                            }
                            else if (opType == "2" || opType.ToLower() == "возврат" || opType.ToLower() == "возврат прихода")
                            {
                                opTypeFounded = "2 Возврат прихода";
                            }
                            else if (opType == "3" || opType.ToLower() == "расход")
                            {
                                opTypeFounded = "3 Расход";
                            }
                            else if (opType == "4" || opType.ToLower() == "возврат расхода")
                            {
                                opTypeFounded = "4 Возврат расхода";
                            }
                            dataGridView_operationTypeMap.Rows.Add(opType, opTypeFounded);
                        }

                    }
                }

            }
            else if (sender == comboBox_correctionDate)
            {
                _pointer_correctionDate = comboBox_correctionDate.SelectedIndex;
                if (_pointer_correctionDate > 0)
                {
                    int i = 0;
                    for (i = _startIndex; i < _endIndex && i <= data.GetUpperBound(0); i++)
                    {
                        if (data[i, _pointer_correctionDate] is null)
                        {
                            AddMessage("Ошибка! Строка: " + i + "  - не заполнена дата коррекции");
                            break;
                        }
                        if (data[i, _pointer_correctionDate] is DateTime)
                        {
                            continue;
                        }
                        if (!DateTime.TryParseExact(data[i, _pointer_correctionDate].ToString(),
                            "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                        {
                            AddMessage("Ошибка! Строка: " + i + "  - не распознана дата. Формат даты должен соответсвовать дд.ММ.гггг");
                            break;
                        }
                    }

                }

            }
            else if (sender == comboBox_correctionType)
            {
                _pointer_correctionTypeM1 = comboBox_correctionType.SelectedIndex;
            }
            else if (sender == comboBox_correctionOrderNumber)
            {
                _pointer_correctionOrderNumber = comboBox_correctionOrderNumber.SelectedIndex;
            }
            else if (sender == comboBox_itemsName)
            {
                _pointer_itemsName = comboBox_itemsName.SelectedIndex;
            }
            else if (sender == textBox_itemsNameDefault)
            {
                _itemsNameDefault = textBox_itemsNameDefault.Text;
            }
            else if (sender == comboBox_itemsQuantity)
            {
                _pointer_itemsQuantity = comboBox_itemsQuantity.SelectedIndex;
            }
            else if (sender == comboBox_retailAddress)
            {
                _pointer_retailAddress = comboBox_retailAddress.SelectedIndex;
                if (_pointer_retailAddress > 0)
                {
                    AddMessage("Включаем настройку перезаписи адреса расчетов");
                    AppSettings.OverideRetailAddress = true;
                }
                else
                {
                    AddMessage("Выключаем настройку перезаписи адреса расчетов");
                    AppSettings.OverideRetailAddress = false;
                }
            }
            else if (sender == comboBox_retailPlace)
            {
                _pointer_retailPlace = comboBox_retailPlace.SelectedIndex;
                if (_pointer_retailPlace > 0)
                {
                    AddMessage("Включаем настройку перезаписи места расчетов");
                    AppSettings.OverideRetailPlace = true;
                }
                else
                {
                    AddMessage("Выключаем настройку перезаписи места расчетов");
                    AppSettings.OverideRetailPlace = false;
                }
            }
            else if(sender == textBox_retailPlaceDefault)
            {
                _retailPlaceDefalt = textBox_retailPlaceDefault.Text;
            }
            else if (sender == textBox_retailAdressDefault)
            {
                _retailAddressDefalt = textBox_retailAdressDefault.Text;
            }
            else if (sender == textBox_itemsQuantityDefault)
            {
                if (double.TryParse(FiscalPrinter.ReplaceBadDecimalSeparatorPoint(textBox_itemsQuantityDefault.Text), out _itemsQuantityDefault))
                {
                    if (Math.Abs(_itemsQuantityDefault) > 0.00000001)
                    {
                        AddMessage("Количество по умолчанию установлено: " + _itemsQuantityDefault);
                        textBox_itemsQuantityDefault.ForeColor = Color.Black;
                    }
                    else
                    {
                        AddMessage("Нулевое количество запрещено, установлено: 1.0");
                        _itemsQuantityDefault = 1.0;
                        textBox_itemsQuantityDefault.ForeColor = Color.Red;
                    }
                }
                else
                {
                    AddMessage("Введеная строка не преобразуется в число, установлено: 1.0");
                    _itemsQuantityDefault = 1.0;
                    textBox_itemsQuantityDefault.ForeColor = Color.Red;
                }
            }
            else if (sender == comboBox_itemsPrice)
            {
                _pointer_itemsPrice = comboBox_itemsPrice.SelectedIndex;
            }
            else if (sender == comboBox_itemsSum)
            {
                _pointer_itemsSum = comboBox_itemsSum.SelectedIndex;
            }
            else if (sender == comboBox_itemsPaymentTypeSign)
            {
                _pointer_itemsPaymentTypeSignM7 = comboBox_itemsPaymentTypeSign.SelectedIndex;
                dataGridView_itemsPaymentTypeMap.Rows.Clear();
                if (_pointer_itemsPaymentTypeSignM7 > 0 && _pointer_itemsPaymentTypeSignM7 <= data.GetUpperBound(1))
                {
                    List<string> ips = new List<string>();
                    int errors = 0;
                    for (int i = _startIndex; i <= data.GetUpperBound(0) && i < _endIndex; i++)
                    {
                        if (data[i, _pointer_itemsPaymentTypeSignM7] != null)
                        {
                            if (!ips.Contains(data[i, _pointer_itemsPaymentTypeSignM7].ToString()))
                                ips.Add(data[i, _pointer_itemsPaymentTypeSignM7].ToString());
                        }
                        else
                        {
                            AddMessage(i + " строка пустое значение");
                            errors++;
                        }
                        if (ips.Count > 25)
                        {
                            AddMessage("Найдено слишком много признаков расчета возможно выбран неправильный столбец");
                            errors += 16;
                        }
                        if (errors > 15)
                        {
                            AddMessage("Превышено количество неправильных значений проверка прервана.");
                            break;
                        }
                    }
                    if (errors <= 15)
                    {
                        foreach (string s in ips)
                        {
                            int t = -1;
                            int.TryParse(s, out t);
                            if (t > 0 && t <= 7)
                            {
                                if (t == 1)
                                {
                                    dataGridView_itemsPaymentTypeMap.Rows.Add(s, "1 - Предоплата 100%");
                                }
                                else if (t == 2)
                                {
                                    dataGridView_itemsPaymentTypeMap.Rows.Add(s, "2 - Частичная предоплата");
                                }
                                else if (t == 3)
                                {
                                    dataGridView_itemsPaymentTypeMap.Rows.Add(s, "3 - Аванс");
                                }
                                else if (t == 4)
                                {
                                    dataGridView_itemsPaymentTypeMap.Rows.Add(s, "4 - Полный расчет");
                                }
                                else if (t == 5)
                                {
                                    dataGridView_itemsPaymentTypeMap.Rows.Add(s, "5 - Частичный расчет и кредит");
                                }
                                else if (t == 6)
                                {
                                    dataGridView_itemsPaymentTypeMap.Rows.Add(s, "6 - Передача в кредит");
                                }
                                else if (t == 7)
                                {
                                    dataGridView_itemsPaymentTypeMap.Rows.Add(s, "7 - Оплата куредита");
                                }
                            }
                            else
                            {
                                dataGridView_itemsPaymentTypeMap.Rows.Add(s);
                            }
                        }
                    }
                }
            }
            else if (sender == comboBox_itemsPaymentTypeSignDefault)
            {
                _itemsPaymentTypeSignDefault = comboBox_itemsPaymentTypeSignDefault.SelectedIndex;
            }
            else if (sender == comboBox_itemsProductType)
            {
                _pointer_itemsProductTypeM33 = comboBox_itemsProductType.SelectedIndex;
                dataGridView_itemsProductTypeMap.Rows.Clear();
                if (_pointer_itemsProductTypeM33 > 0 && _pointer_itemsProductTypeM33 <= data.GetUpperBound(1))
                {
                    List<string> ipt = new List<string>();
                    int errors = 0;
                    for (int i = _startIndex; i <= data.GetUpperBound(0) && i < _endIndex; i++)
                    {
                        if (data[i, _pointer_itemsProductTypeM33] != null)
                        {
                            if (!ipt.Contains(data[i, _pointer_itemsProductTypeM33].ToString()))
                                ipt.Add(data[i, _pointer_itemsProductTypeM33].ToString());
                        }
                        else
                        {
                            AddMessage(i + " строка пустое значение");
                            errors++;
                        }
                        if (ipt.Count > 45)
                        {
                            AddMessage("Найдено слишком много признаков расчета возможно выбран неправильный столбец");
                            errors += 46;
                        }
                        if (errors > 15)
                        {
                            AddMessage("Превышено количество неправильных значений проверка прервана.");
                            break;
                        }
                    }
                    if (errors < 45 && ipt.Count < 45)
                    {
                        foreach (string s in ipt)
                        {
                            if (s == "1")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "1. Товар" );
                            }
                            else if(s == "2")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "2. Подакцизный товар");
                            }
                            else if (s == "3")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "3. Работа");
                            }
                            else if (s == "4")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "4. Услуга");
                            }
                            else if (s == "5")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "5. Ставка азартной игры");
                            }
                            else if (s == "6")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "6. Выигрыш азартной игры");
                            }
                            else if (s == "7")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "7. Лотерейный билет");
                            }
                            else if (s == "8")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "8. Выигрыш лотереи");
                            }
                            else if (s == "9")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "9. Предоставление РИД");
                            }
                            else if (s == "10")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "10. Платеж");
                            }
                            else if (s == "11")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "11. Агеннтское вознаграждение");
                            }
                            else if (s == "12")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "12. Составной предмет расчета");
                            }
                            else if (s == "13")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "13. Иной предмет расчета");
                            }
                            else if (s == "14")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "14. Имущественное право");
                            }
                            else if (s == "15")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "15. Внереализационный доход");
                            }
                            else if (s == "16")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "16. Страховые взносы");
                            }
                            else if (s == "17")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "17. Торговый сбор");
                            }
                            else if (s == "18")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "18. Курортный сбор");
                            }
                            else if (s == "19")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "19. Залог");
                            }
                            else if (s == "20")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "20. Расход");
                            }
                            else if (s == "21")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "21. Взносы ОПС ИП");
                            }
                            else if (s == "22")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "22. Взносы ОПС");
                            }
                            else if (s == "23")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "23. Взносы ОМС ИП");
                            }
                            else if (s == "24")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "24. Взносы ОМС");
                            }
                            else if (s == "25")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "25. Взносы ОСС");
                            }
                            else if (s == "26")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "26. Платеж казино");
                            }
                            else if (s == "27")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "27. Выдача ДС");
                            }
                            else if (s == "30")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "30. АТНМ");
                            }
                            else if (s == "31")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "31. АТМ");
                            }
                            else if (s == "32")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "32. ТНМ");
                            }
                            else if (s == "33")
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s, "33. ТМ");
                            }
                            else
                            {
                                dataGridView_itemsProductTypeMap.Rows.Add(s);
                            }
                                
                        }
                    }

                }

            }
            else if (sender == comboBox_itemsProductTypeDefault)
            {
                _itemsProductTypeDefault = comboBox_itemsProductTypeDefault.SelectedIndex;
            }
            else if (sender == comboBox_itemsUnit120)
            {
                _pointer_itemsUnit120 = comboBox_itemsUnit120.SelectedIndex;
            }
            else if (sender == comboBox_itemsUnit120Default)
            {
                int t = comboBox_itemsUnit120.SelectedIndex;
                if (t == 0)
                {
                    _itemsUnit120Default = 0;
                }
                else if (t == 1)
                {
                    _itemsUnit120Default = 10;
                }
                else if (t == 2)
                {
                    _itemsUnit120Default = 11;
                }
                else if (t == 3)
                {
                    _itemsUnit120Default = 12;
                }
                else if (t == 4)
                {
                    _itemsUnit120Default = 20;
                }
                else if (t == 5)
                {
                    _itemsUnit120Default = 21;
                }
                else if (t == 6)
                {
                    _itemsUnit120Default = 22;
                }
                else if (t == 7)
                {
                    _itemsUnit120Default = 30;
                }
                else if (t == 8)
                {
                    _itemsUnit120Default = 31;
                }
                else if (t == 9)
                {
                    _itemsUnit120Default = 32;
                }
                else if (t == 10)
                {
                    _itemsUnit120Default = 40;
                }
                else if (t == 11)
                {
                    _itemsUnit120Default = 41;
                }
                else if (t == 12)
                {
                    _itemsUnit120Default = 42;
                }
                else if (t == 13)
                {
                    _itemsUnit120Default = 50;
                }
                else if (t == 14)
                {
                    _itemsUnit120Default = 51;
                }
                else if (t == 15)
                {
                    _itemsUnit120Default = 70;
                }
                else if (t == 16)
                {
                    _itemsUnit120Default = 71;
                }
                else if (t == 17)
                {
                    _itemsUnit120Default = 72;
                }
                else if (t == 18)
                {
                    _itemsUnit120Default = 73;
                }
                else if (t == 19)
                {
                    _itemsUnit120Default = 80;
                }
                else if (t == 20)
                {
                    _itemsUnit120Default = 81;
                }
                else if (t == 21)
                {
                    _itemsUnit120Default = 82;
                }
                else if (t == 22)
                {
                    _itemsUnit120Default = 83;
                }
                else if (t == 23)
                {
                    _itemsUnit120Default = 255;
                }
            }
            else if (sender == comboBox_itemsNdsRate)
            {
                _pointer_itemsNdsRate = comboBox_itemsNdsRate.SelectedIndex;
            }
            else if (sender == comboBox_itemsNdsRateDefault)
            {
                _itemsNdsRateDefault = comboBox_itemsNdsRateDefault.SelectedIndex;
            }
            else if (sender == comboBox_checkPaidCash)
            {
                _pointer_checkPaidCash = comboBox_checkPaidCash.SelectedIndex;
            }
            else if (sender == comboBox_checkPaidEcash)
            {
                _pointer_checkPaidEcash = comboBox_checkPaidEcash.SelectedIndex;
            }
            else if (sender == comboBox_checkPaidCash)
            {
                _pointer_checkPaidPrepaid = comboBox_checkPaidPrepaid.SelectedIndex;
            }
            else if (sender == comboBox_checkPaidCredit)
            {
                _pointer_checkPaidCredit = comboBox_checkPaidCredit.SelectedIndex;
            }
            else if (sender == comboBox_checkPaidProvision)
            {
                _pointer_checkPaidProvision = comboBox_checkPaidProvision.SelectedIndex;
            }
            else if (sender == comboBox_propertiesData)
            {
                _pointer_PropiertyData = comboBox_propertiesData.SelectedIndex;
            }
            else if (sender == checkBox_closeShiftSign)
            {
                _closeShiftOnEroor = checkBox_closeShiftSign.Checked;
            }
            else if (sender == textBox_dtFormat)
            {
                _dtFormat = textBox_dtFormat.Text;
            }
            else if (sender == textBox_correctionOrderNumber)
            {
                _correctionOrderNumberDefault = textBox_correctionOrderNumber.Text;
            }
            else if (sender == button_checkOutEmulatorRun)
            {
                breakOperation = false;
                _operationTypesMap.Clear();
                foreach (DataGridViewRow row in dataGridView_operationTypeMap.Rows)
                {
                    int operationType = 0;
                    if (row.Cells[1].Value == null || row.Cells[1].Value.ToString() == "")
                    {
                        AddMessage("Не заполнена таблица признаков расчета чека !!!");
                    }
                    else
                    {
                        operationType = row.Cells[1].Value.ToString()[0] - '0';
                        if (operationType <= 0 || operationType > 4)
                        {
                            AddMessage("Не удается разобрать признак расчета для строки: " + row.Cells[0].Value.ToString());
                        }
                        else
                        {
                            _operationTypesMap[row.Cells[0].Value.ToString()] = operationType;
                        }
                    }
                }
                _docTypeMap.Clear();
                foreach (DataGridViewRow row in dataGridView_documentTypeMap.Rows)
                {
                    /*if(row.Cells[1].Value == null)
                    {
                        AddMessage("Не заполнена таблица соответсвия типа документа !!!");
                        continue;
                    }
                    var s = row.Cells[1].Value.ToString();*/
                    if (row.Cells[1].Value == null || row.Cells[1].Value.ToString() == "")
                    {
                        AddMessage("Не заполнена таблица соответсвия типа документа !!!");
                    }
                    else
                    {
                        if (row.Cells[1].Value.ToString() == "3(4) Чек(БСО)")
                        {
                            _docTypeMap[row.Cells[0].Value.ToString()] = FiscalPrinter.FD_DOCUMENT_NAME_CHEQUE;
                        }
                        else if (row.Cells[1].Value.ToString() == "31(41) Чек коррекции(БСО коррекции)")
                        {
                            _docTypeMap[row.Cells[0].Value.ToString()] = FiscalPrinter.FD_DOCUMENT_NAME_CORRECTION_CHEQUE;
                        }
                        else
                        {
                            AddMessage("Не удается разобрать признак документа для строки: " + row.Cells[0].Value.ToString());
                        }
                    }
                }
                _itemsPaymentTypeMap.Clear();
                foreach (DataGridViewRow row in dataGridView_itemsPaymentTypeMap.Rows)
                {
                    /*if (row.Cells[1].Value == null)
                    {
                        AddMessage("Не заполнена таблица соответсвия признака способа расчета !!!");
                        continue;
                    }
                    var s = row.Cells[1].Value.ToString();*/
                    if (row.Cells[1].Value == null || row.Cells[1].Value.ToString() == "")
                    {
                        AddMessage("Не заполнена таблица соответсвия признак способа расчета!!!");
                    }
                    else
                    {
                        if (row.Cells[1].Value.ToString() == "1 - Предоплата 100%")
                        {
                            _itemsPaymentTypeMap[row.Cells[0].Value.ToString()] = 1;
                        }
                        else if (row.Cells[1].Value.ToString() == "2 - Частичная предоплата")
                        {
                            _itemsPaymentTypeMap[row.Cells[0].Value.ToString()] = 2;
                        }
                        else if (row.Cells[1].Value.ToString() == "3 - Аванс")
                        {
                            _itemsPaymentTypeMap[row.Cells[0].Value.ToString()] = 3;
                        }
                        else if (row.Cells[1].Value.ToString() == "4 - Полный расчет")
                        {
                            _itemsPaymentTypeMap[row.Cells[0].Value.ToString()] = 4;
                        }
                        else if (row.Cells[1].Value.ToString() == "5 - Частичный расчет и кредит")
                        {
                            _itemsPaymentTypeMap[row.Cells[0].Value.ToString()] = 5;
                        }
                        else if (row.Cells[1].Value.ToString() == "6 - Передача в кредит")
                        {
                            _itemsPaymentTypeMap[row.Cells[0].Value.ToString()] = 6;
                        }
                        else if (row.Cells[1].Value.ToString() == "7 - Оплата куредита")
                        {
                            _itemsPaymentTypeMap[row.Cells[0].Value.ToString()] = 7;
                        }
                        else
                        {
                            AddMessage("Не удается разобрать признак способа рвсчета для строки: " + row.Cells[1].Value.ToString());
                        }
                    }
                }
                _itemsProductTypeMap.Clear();
                foreach (DataGridViewRow row in dataGridView_itemsProductTypeMap.Rows)
                {
                    /*if (row.Cells[1].Value == null)
                    {
                        AddMessage("Не заполнена таблица соответсвия признака предмета расчета !!!");
                        continue;
                    }
                    //int itemsProductType = 0;*/
                    if (row.Cells[1].Value == null || row.Cells[1].Value.ToString() == "")
                    {
                        AddMessage("Не заполнена таблица признаков предмета расчета чека !!!");
                    }
                    else
                    {
                        string s = row.Cells[1].Value.ToString();
                        string key = row.Cells[0].Value.ToString();
                        int t = 0;
                        if (s[1] == '.')
                        {
                            t = s[0] - '0';
                        }
                        else if (s[2] == '.')
                        {
                            t = 10 * (s[0] - '0') + s[1] - '0';
                        }

                        if (t > 0 || t < 33 || t != 28 || t != 29)
                        {
                            _itemsProductTypeMap[key] = t;
                        }
                        else
                        {
                            AddMessage("Не удается разобрать признак предмета расчета для строки: " + row.Cells[0].Value.ToString());
                        }
                    }
                }



                int lastEmuDelay = AppSettings.EmulatorDelay;
                AppSettings.EmulatorDelay = 0;
                FrEmulator emu = new FrEmulator(null, false);

                bool checkOut;
                if (checkBox_admissibilityErrors.Checked)
                {
                    int errors = 0;
                    int.TryParse(textBox_errorsAllowed.Text, out errors);
                    checkOut = ProcessingExcelReport(emu, 500, errors);
                }
                else
                {
                    checkOut = ProcessingExcelReport(emu);
                }
                AddMessage("Результат проверки настроек " + checkOut);
                emu.ReadDeviceCondition();
                List<FiscalPrinter.FnReadedDocument> fds = new List<FiscalPrinter.FnReadedDocument>();
                for (int i = 3; i <= emu.LastFd; i++)
                {
                    fds.Add(emu.ReadFD(i, true));
                }
                textBox_perfomingInformation.Text = MainForm.ChequesCount(fds);
                if (checkOut)
                {
                    if (fiscalPrinter != null && fiscalPrinter.IsConnected)
                    {
                        button_performCorrections.Enabled = checkOut;
                    }
                    else
                    {
                        button_performCorrections.Enabled = false;
                        AddMessage("Нет соединения с ККТ.");
                    }
                }
                AppSettings.EmulatorDelay = lastEmuDelay;


            }
            else if (sender == comboBox_selectedSno)
            {
                _pointer_sno = comboBox_selectedSno.SelectedIndex;
            }
            else if (sender == comboBox_snoDefault)
            {
                if (comboBox_snoDefault.SelectedIndex == 0)
                {
                    _snoDefault = 1;
                    AddMessage("СНО по умолчанию установлена: ОСНО");
                }
                else if (comboBox_snoDefault.SelectedIndex == 1)
                {
                    _snoDefault = 2;
                    AddMessage("СНО по умолчанию установлена: УСНД");
                }
                else if (comboBox_snoDefault.SelectedIndex == 2)
                {
                    _snoDefault = 4;
                    AddMessage("СНО по умолчанию установлена: УСНДР");
                }
                else if (comboBox_snoDefault.SelectedIndex == 3)
                {
                    _snoDefault = 16;
                    AddMessage("СНО по умолчанию установлена: ЕСХН");
                }
                else if (comboBox_snoDefault.SelectedIndex == 4)
                {
                    _snoDefault = 32;
                    AddMessage("СНО по умолчанию установлена: ПСН");
                }

            }
            else if (sender == comboBox_docTypeChooser)
            {
                _pointer_documentTypeM2 = comboBox_docTypeChooser.SelectedIndex;
                dataGridView_documentTypeMap.Rows.Clear();
                if (_pointer_documentTypeM2 > 1)
                {
                    int errors = 0;
                    List<string> list = new List<string>();
                    for (int i = _startIndex; i < _endIndex && i <= data.GetUpperBound(0); i++)
                    {
                        try
                        {
                            if (data[i, _pointer_documentTypeM2 - 1] == null)
                            {
                                throw new Exception("Пустое поле");
                            }
                            if (!list.Contains(data[i, _pointer_documentTypeM2 - 1].ToString()))
                            {
                                list.Add(data[i, _pointer_documentTypeM2 - 1].ToString());
                            }
                        }
                        catch (Exception exc)
                        {
                            AddMessage("Строка " + i + " - " + exc.Message);
                            errors++;
                        }
                        if (errors > 15)
                        {
                            AddMessage("Много ошибок просмотр документка завершен");
                            break;
                        }
                    }
                    foreach (var s in list)
                    {
                        dataGridView_documentTypeMap.Rows.Add(s);
                    }
                }
            }
            else if (sender == button_presetsFirstOfd)
            {
                textBox_startFrom.Text = "4";
                comboBox_checkId.SelectedIndex = 3;
                comboBox_operationType.SelectedIndex = 13;
                comboBox_docTypeChooser.SelectedIndex = 10;
                comboBox_correctionDate.SelectedIndex = 8;
                textBox_dtFormat.Text = "yyyy-MM-ddTHH:mm";
                comboBox_itemsName.SelectedIndex = 17;
                comboBox_itemsQuantity.SelectedIndex = 19;
                comboBox_itemsPrice.SelectedIndex = 18;
                comboBox_itemsSum.SelectedIndex = 20;
                comboBox_itemsProductType.SelectedIndex = 16;
                comboBox_itemsPaymentTypeSign.SelectedIndex = 13;
                comboBox_checkPaidCash.SelectedIndex = 14;
                comboBox_checkPaidEcash.SelectedIndex = 15;
                comboBox_cashier.SelectedIndex = 6;
            }
            else if (sender == button_performCorrections)
            {
                breakOperation = false;
                int errorsAllowed = 10;
                int.TryParse(textBox_errorsAllowed.Text, out errorsAllowed);
                if (data.GetUpperBound(0) > 15 && !(fiscalPrinter is FrEmulator))
                {
                    new Thread(() =>
                    {
                        pst = new ProcessingStatus();
                        pst.Focus();
                        pst.Show();
                        pst.WindowState = FormWindowState.Normal;
                        Application.Run(pst);
                    }).Start();
                }
                int closeShiftEvery = 0;
                try
                {
                    string fdCounterToCloseShift = "";
                    foreach (char c in comboBox_closeShiftEvery.Text)
                    {
                        if (char.IsDigit(c))
                        {
                            fdCounterToCloseShift += c;
                        }
                    }

                    closeShiftEvery = int.Parse(fdCounterToCloseShift);
                }
                catch
                {
                    AddMessage("Не удалось разобрать колчество оформленных ФД после которых закрывается смена, закрытие отключено");
                    closeShiftEvery = 0;
                }
                fiscalPrinter.OpenShift();
                bool checkOut = ProcessingExcelReport(fiscalPrinter, 5, errorsAllowed, comboBox_pauseAfterCheque.SelectedIndex * 1000, closeShiftEvery);
                if (checkOut)
                {
                    AddMessage("Результат корректировки " + checkOut);
                }
                if (pst != null && pst.Created)
                {
                    pst.AllDone();
                }
            }
            else if (sender == checkBox_dontPrint)
            {
                if (fiscalPrinter != null)
                {
                    fiscalPrinter.DontPrint = checkBox_dontPrint.Checked;
                }
            }
            else if (sender == textBox_cashierDefault)
            {
                _cashierDefault = textBox_cashierDefault.Text;
            }
            else if (sender == comboBox_cashier)
            {
                _pointer_cashier = comboBox_cashier.SelectedIndex;
            }
            else if (sender == comboBox_selectedSno)
            {
                _pointer_sno = comboBox_selectedSno.SelectedIndex;
            }
            else if (sender == checkBox_enableManualTaxes)
            {
                _manualTaxes = checkBox_enableManualTaxes.Checked;
                groupBox_manualTax.Enabled = _manualTaxes;
                if (!_manualTaxes)
                {
                    comboBox_checkTax20.SelectedIndex = 0;
                    comboBox_checkTax10.SelectedIndex = 0;
                    comboBox_checkTax20120.SelectedIndex = 0;
                    comboBox_checkTax10110.SelectedIndex = 0;
                    comboBox_checkTax0.SelectedIndex = 0;
                    comboBox_checkTaxFree.SelectedIndex = 0;
                    comboBox_checkTax5.SelectedIndex = 0;
                    comboBox_checkTax7.SelectedIndex = 0;
                    comboBox_checkTax5105.SelectedIndex = 0;
                    comboBox_checkTax7107.SelectedIndex = 0;
                    AddMessage("Включен подсчет сумм налогов программой");

                }
                else
                {
                    AddMessage("Отключен подсчет сумм налогов программой, если ничего не заполнять в теги 1102-1107, 1115 попадут нули");
                }
            }
            else if (sender == comboBox_checkTax20)
            {
                _pointer_checkTax_20 = comboBox_checkTax20.SelectedIndex;
            }
            else if (sender == comboBox_checkTax10)
            {
                _pointer_checkTax_10 = comboBox_checkTax10.SelectedIndex;
            }
            else if (sender == comboBox_checkTax20120)
            {
                _pointer_checkTax_20120 = comboBox_checkTax20120.SelectedIndex;
            }
            else if (sender == comboBox_checkTax10110)
            {
                _pointer_checkTax_10110 = comboBox_checkTax10110.SelectedIndex;
            }
            else if (sender == comboBox_checkTax0)
            {
                _pointer_checkTax_0 = comboBox_checkTax0.SelectedIndex;
            }
            else if (sender == comboBox_checkTaxFree)
            {
                _pointer_checkTax_free = comboBox_checkTaxFree.SelectedIndex;
            }
            else if (sender == comboBox_checkTax5)
            {
                _pointer_checkTax_5 = comboBox_checkTax5.SelectedIndex;
            }
            else if (sender == comboBox_checkTax7)
            {
                _pointer_checkTax_7 = comboBox_checkTax7.SelectedIndex;
            }
            else if (sender == comboBox_checkTax5105)
            {
                _pointer_checkTax_5105 = comboBox_checkTax5105.SelectedIndex;
            }
            else if (sender == comboBox_checkTax7107)
            {
                _pointer_checkTax_7107 = comboBox_checkTax7107.SelectedIndex;
            }
            else if (sender == button_savePreset)
            {
                string settingName = textBox_presetName.Text.Trim();
                bool badName = false;
                foreach (Char c in settingName)
                {
                    if (Char.IsLetterOrDigit(c) || c == ' ' || c == '.')
                    {
                        continue;
                    }
                    else
                    {
                        badName = true;
                    }
                }
                if (badName || settingName == "")
                {
                    AddMessage("Некорректное название настройки(допустимы буквы, цифры, точка и пробел между вышеперечисленными)");
                    return;
                }
                if (AppSettings.OfdExportSet.ContainsKey(settingName))
                {
                    AddMessage(settingName + " - Настройка перезаписывается");
                    AppSettings.OfdExportSet.Remove(settingName);
                }
                List<int> ints = new List<int>();
                foreach (var control in comboSet)
                {
                    if (control is CheckBox)
                    {
                        ints.Add((control as CheckBox).Checked ? 1 : 0);
                    }
                    else if (control is System.Windows.Forms.ComboBox)
                    {
                        ints.Add((control as System.Windows.Forms.ComboBox).SelectedIndex);
                    }
                    else if (control is System.Windows.Forms.TextBox)
                    {

                        if (int.TryParse((control as System.Windows.Forms.TextBox).Text, out int t))
                        {
                            ints.Add(t);
                        }
                        else
                        {
                            ints.Add(1);
                        }
                    }
                }
                AppSettings.OfdExportSet[settingName] = ints.ToArray();
                AppSettings.SaveSettings();
                MapPresetts();
                AddMessage("Настройки записаны в конфигурационный файл");
            }
            else if (sender == listBox_presets)
            {
                string settingName = listBox_presets.SelectedItem.ToString();
                if (settingName != null)
                {
                    textBox_presetName.Text = settingName;
                    AddMessage("Загружаем настройки - " + settingName);


                    if (AppSettings.OfdExportSet.ContainsKey(settingName))
                    {
                        int[] setting = AppSettings.OfdExportSet[settingName];
                        if (setting.Length != comboSet.Count)
                        {
                            AddMessage("Размер настроек отличается от количества элементов интерфейса, возможно сохрание другой версии программы");
                        }
                        if (setting.Length >= 40)
                        {
                            // сразу устанавливаем первую строку откуда идем по файлу
                            textBox_startFrom.Text = setting[40].ToString();
                        }
                        for (int i = 0; i < setting.Length && i < comboSet.Count; i++)
                        {
                            if (comboSet[i] is CheckBox)
                            {
                                (comboSet[i] as CheckBox).Checked = setting[i] != 0;
                            }
                            else if (comboSet[i] is System.Windows.Forms.ComboBox)
                            {
                                System.Windows.Forms.ComboBox combo = comboSet[i] as System.Windows.Forms.ComboBox;
                                if (setting[i] >= combo.Items.Count || setting[i] < 0)
                                {
                                    AddMessage(combo.Name + " - настройка выходит за допустимый диапазон, пропускаем");
                                }
                                else
                                { combo.SelectedIndex = setting[i]; }
                            }
                            else if (comboSet[i] is System.Windows.Forms.TextBox)
                            {
                                (comboSet[i] as System.Windows.Forms.TextBox).Text = setting[i].ToString();
                            }
                            else
                            {
                                AddMessage(comboSet[i].Name + " - некорректный элемент интерфейса, наcтройка пропущена");
                            }
                        }
                    }
                }
                else
                {
                    AddMessage("Null settings");
                }
                
            }
            else if (sender == checkBox_allowEmptyPropertyData)
            {
                _allowEmptyPropertyData = checkBox_allowEmptyPropertyData.Checked;
            }
            else if(sender == comboBox_emailPhone)
            {
                _pointer_emailPhone = comboBox_emailPhone.SelectedIndex;
            }
            else if(sender == textBox_emailPhone)
            {
                _emailPhoneDefault = textBox_emailPhone.Text;
            }
        }

        void MapPresetts()
        {
            listBox_presets.Items.Clear();
            foreach (string s in AppSettings.OfdExportSet.Keys)
            {
                listBox_presets.Items.Add(s);
            }
        }

        ProcessingStatus pst = null;

        /*
         * statsOutOnLines выводить информацию об обработке каждые...
         * reportName = null или пустая не пишем отчет - переведено в поле
         */

        private bool ProcessingExcelReport(FiscalPrinter fr, int statsOutOnLines = 500, /*string reportName = null,*/ int errorsAllowed = 0, int pause = 0, int closeShiftEvery = 0)
        {
            errRows = new List<int>();
            if (statsOutOnLines == 0)
            {
                statsOutOnLines = 10;
            }
            int errorsOccured = 0;
            int fdSended = 0;
            breakOperation = false;
            if (data != null||data.Length == 0)
            {
                int rows = data.GetUpperBound(0);
                int cols = data.GetUpperBound(1);
                int stop = _endIndex;
                if(rows < stop)
                {
                    stop = rows;
                }
                bool errorSettings = false;
                // блок проверки настроек: проверяем настройки на невыход за диапазон
                {
                    if (_startIndex > stop)
                    {
                        AddMessage("Начальная строка превышает разре таблицы или конечную строку");
                        errorSettings = true;
                    }
                    if (_pointer_PropiertyData > cols)
                    {
                        AddMessage("Указатель на доп реквизит чека некорректен");
                        errorSettings = true;
                    }
                    if (_pointer_checkId > cols)
                    {
                        AddMessage("Указатель на идентификатор чека выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_operationTypeM5 - 4 > cols|| _pointer_operationTypeM5 == 0)
                    {
                        AddMessage("Указатель на признак расчета некорректен");
                        errorSettings = true;
                        // добавить проверку за заполненность таблицы
                    }


                    if (_pointer_documentTypeM2 - 1 > cols || _pointer_documentTypeM2 < 0)
                    {
                        AddMessage("Указатель на тип документа некорректен");
                        errorSettings = true;
                        // добавить проверку за заполненность таблицы
                    }
                    if (_pointer_correctionDate > cols)
                    {
                        AddMessage("Указатель на дату коррекции выходит за диапазон таблицы");
                        errorSettings = true;

                    }
                    if (_pointer_correctionTypeM1 - 1 > cols)
                    {
                        AddMessage("Указатель на тип коррекции выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_correctionOrderNumber > cols)
                    {
                        AddMessage("Указатель на номер предписания коррекции выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_itemsName > cols)
                    {
                        AddMessage("Указатель на наименование предмета расчета выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_itemsQuantity > cols)
                    {
                        AddMessage("Указатель на количество предмета расчета выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_itemsPrice > cols)
                    {
                        AddMessage("Указатель на цену предмета расчета выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_itemsSum > cols || _pointer_itemsSum == 0)
                    {
                        AddMessage("Указатель на сумму за предмет расчета выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_itemsPaymentTypeSignM7 > cols)
                    {
                        AddMessage("Указатель на признак способа расчета выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_itemsProductTypeM33 > cols)
                    {
                        AddMessage("Указатель на признак предмета расчета выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_itemsUnit120 > cols)
                    {
                        AddMessage("Указатель на меру предмета расчета выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_itemsNdsRate > cols)
                    {
                        AddMessage("Указатель на ставку НДС предмета расчета выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkPaidCash > cols)
                    {
                        AddMessage("Указатель на оплату наличными выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkPaidEcash > cols)
                    {
                        AddMessage("Указатель на оплату безналичными выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkPaidPrepaid > cols)
                    {
                        AddMessage("Указатель на оплату аванс выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkPaidCredit > cols)
                    {
                        AddMessage("Указатель на оплату кредит выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkPaidProvision > cols)
                    {
                        AddMessage("Указатель на оплату иным типом оплаты выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if(_pointer_sno > cols)
                    {
                        AddMessage("Указатель на CHO выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if(_pointer_cashier > cols)
                    {
                        AddMessage("Указатель на кассира выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkTax_20 > cols)
                    {
                        AddMessage("Указатель на НДС 20% выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkTax_10 > cols)
                    {
                        AddMessage("Указатель на НДС 10% выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkTax_20120 > cols)
                    {
                        AddMessage("Указатель на НДС 20/120 выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkTax_10110 > cols)
                    {
                        AddMessage("Указатель на НДС 10/110 выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkTax_0 > cols)
                    {
                        AddMessage("Указатель на НДС 0% выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkTax_free > cols)
                    {
                        AddMessage("Указатель на без НДС выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkTax_5 > cols)
                    {
                        AddMessage("Указатель на НДС 5% выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkTax_7 > cols)
                    {
                        AddMessage("Указатель на НДС 7% выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkTax_5105 > cols)
                    {
                        AddMessage("Указатель на НДС 5/105 выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_checkTax_7107 > cols)
                    {
                        AddMessage("Указатель на НДС 7/107 выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_retailAddress > cols - 1)
                    {
                        AddMessage("Указатель адрес расчетов выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if (_pointer_retailPlace > cols - 1)
                    {
                        AddMessage("Указатель места расчетов выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                    if(_pointer_emailPhone > cols)
                    {
                        AddMessage("Указатель адрес покупателя выходит за диапазон таблицы");
                        errorSettings = true;
                    }
                }
                
                if (errorSettings)
                {
                    AddMessage("Устраните ошибку в настройках");
                    return false;
                }
                if (!string.IsNullOrEmpty(logFile))
                {
                    errRows = new List<int>();
                }
                fr.ReadDeviceCondition();
                
                int checkesPerformed = 0;
                string lastCheckId = "";
                DateTime startDt = DateTime.Now;
                FiscalCheque check = new FiscalCheque();
                

                for (int i = _startIndex; i <= stop; i++)
                {
                    if (breakOperation)
                    {
                        AddMessage("Обработка прервана пользователем на строке " + (i - 1));
                        break;
                    }

                    if (errorsOccured > errorsAllowed)
                    {
                        AddMessage("Превышен уровень ошибок обработка прервана на строке "+(i-1));
                        break;
                    }

                    // статистика и расчет времени
                    if (checkesPerformed >= 5 && i % statsOutOnLines == 0 && i - _startIndex > 0)
                    {
                        double secondsPassed = (DateTime.Now - startDt).TotalSeconds;
                        double secondsLeft = Math.Round((stop - i) * secondsPassed / (i - _startIndex));
                        int minsLeft = ((int)(secondsLeft / 60)) % 60;
                        int hoursLeft = ((int)(secondsLeft / 3600)) % 24;
                        string timeLeft = "Примерно осталось: ";
                        if (hoursLeft > 0) timeLeft += hoursLeft + " ч, ";
                        if (minsLeft > 0 || hoursLeft > 0) timeLeft += minsLeft + "мин, ";
                        timeLeft += secondsLeft % 60 + " сек";

                        // тут  вывести информацию о корректировке
                        string statistic = "Строка " + i 
                            + Environment.NewLine + "Оформлено чеков " + checkesPerformed
                            + Environment.NewLine + "Ошибок " + errorsOccured
                            + Environment.NewLine + "Оформлено чеков " + checkesPerformed
                            + Environment.NewLine + timeLeft;
                        // добавить состояние ФР: последний ФД и к-во неотправленных/от
                        if(pst!=null&& pst.Created)
                        {
                            pst.Message(statistic, false);
                        }

                    }
                    string subExtErr = "";
                    AddReportLogMsg("Строка " + i);
                    try
                    {
                        //работа с чеками
                        if(check == null)
                        {
                            check = new FiscalCheque();
                        }
                        if(_pointer_checkId == 0 || data[i, _pointer_checkId].ToString() != lastCheckId)
                        {
                            // каждая строка новый чек или изменился ид.чека - пробивааем чек
                            AddReportLogMsg("Признак нового чека, закрываем накопления предыдущих строк");
                            if (check.Items.Count > 0)
                            {
                                subExtErr = "Оформление ФД";
                                double paydSum = Math.Round(check.Cash + check.ECash + check.Prepaid + check.Credit + check.Provision, 2);
                                // перерасчет  налогов
                                if(!_manualTaxes)
                                    check.Control(true);
                                // если в чеке оккругление компенсируем
                                check.TotalSum = paydSum;

                                int lastFdNumber = fr.LastFd;
                                AddReportLogMsg("\t" + check.ToString(FiscalCheque.SHORT_INFO));
                                bool rezultPerfoming = fr.PerformFD(check);
                                if (rezultPerfoming)
                                {
                                    AddReportLogMsg("\t\tOK");
                                    checkesPerformed++;
                                    
                                }
                                else
                                {
                                    errRows.Add(i-1);
                                    errorsOccured++;
                                    fr.ReadDeviceCondition();
                                    if (fr.LastFd != lastFdNumber)
                                    {
                                        // номер чека изменился
                                        AddReportLogMsg("\tчек закрылся с ошибкой " + (FiscalPrinter.KKMInfoTransmitter.ContainsKey(FiscalPrinter.FR_LAST_ERROR_MSG_KEY) ? FiscalPrinter.KKMInfoTransmitter[FiscalPrinter.FR_LAST_ERROR_MSG_KEY] : "null"),1);
                                        checkesPerformed++;
                                    }
                                    else
                                    {
                                        int lastFdAfterError = fr.LastFd;
                                        // номер чека не изменился
                                        if (_closeShiftOnEroor)
                                        {
                                            fr.CloseShift();
                                            fr.OpenShift();
                                            fr.ReadDeviceCondition();
                                            if(lastFdAfterError == fr.LastFd)
                                            {
                                                AddReportLogMsg("\tЗакрытие и открытие смены не увеличило номер ФД",1);
                                                errorsOccured += 2;
                                            }
                                            else
                                            {
                                                AddReportLogMsg("\tЗакрытие и открытие смены увеличило номер ФД", 1);
                                            }
                                            AddReportLogMsg("\t пытаемся пробить повторно");
                                            rezultPerfoming = fr.PerformFD(check);
                                            if (rezultPerfoming)
                                            {
                                                AddReportLogMsg("\t\t\tповторное оформление чека прошло без ошибок", 1);
                                                checkesPerformed++;
                                            }
                                            else
                                            {
                                                AddReportLogMsg("\tчек закрылся с ошибкой " + (FiscalPrinter.KKMInfoTransmitter.ContainsKey(FiscalPrinter.FR_LAST_ERROR_MSG_KEY) ? FiscalPrinter.KKMInfoTransmitter[FiscalPrinter.FR_LAST_ERROR_MSG_KEY] : "null"), 1);
                                                errorsOccured++;
                                            }
                                        }
                                    }
                                }
                                if (rezultPerfoming) // оформился ФД
                                {
                                    if (pause > 0) 
                                    {
                                        // пауза для отправки чека в ОФД
                                        Thread.Sleep(pause);
                                    }
                                    if (closeShiftEvery > 0 && checkesPerformed % closeShiftEvery == 0)
                                    {
                                        fr.CloseShift();
                                        fr.OpenShift();
                                    }
                                }

                                subExtErr = "";

                            }
                            else
                            {
                                AddReportLogMsg("Нечего пробивать.");
                            }
                            check = new FiscalCheque();
                        }

                        // накапливаем чек
                        // признак расчета
                        subExtErr = "1192 propiertiesData ";
                        if (_pointer_PropiertyData > 0)
                        {
                            if (_allowEmptyPropertyData)
                            {
                                if(data[i, _pointer_PropiertyData]!=null&& data[i, _pointer_PropiertyData].ToString()!="")
                                    check.PropertiesData = data[i, _pointer_PropiertyData].ToString();
                            }
                            else
                            {
                                check.PropertiesData = data[i, _pointer_PropiertyData].ToString();
                            }
                        }
                        subExtErr = "СНО";
                        if (_pointer_sno == 0)
                        {
                            check.Sno = _snoDefault;
                        }
                        else
                        {
                            check.Sno = int.Parse(data[i, _pointer_sno].ToString());
                        }
                        subExtErr = "Emal|Phone";
                        if (_pointer_emailPhone == 0)
                        {
                            if (!string.IsNullOrEmpty(_emailPhoneDefault))
                            {
                                check.EmailPhone = _emailPhoneDefault;
                            }
                        }
                        else // emailPhone из таблицы
                        {
                            if (data[i, _pointer_emailPhone] != null)
                            {
                                check.EmailPhone = data[i, _pointer_emailPhone].ToString();
                            }
                        }
                        subExtErr = "Адрес расчетов";
                        if (_pointer_retailAddress > 0)
                        {
                            if(_pointer_retailAddress == 1)
                            {
                                if (!string.IsNullOrEmpty(_retailAddressDefalt))
                                {
                                    check.RetailAddress = _retailAddressDefalt;
                                }
                            }
                            else if(_pointer_retailAddress > 1)
                            {
                                if (data[i, _pointer_retailAddress - 1] != null)
                                {
                                    check.RetailAddress = data[i, _pointer_retailAddress - 1].ToString();
                                }
                            }
                        }
                        subExtErr = "Место расчетов";
                        if (_pointer_retailPlace > 0)
                        {
                            if (_pointer_retailPlace == 1)
                            {
                                if (!string.IsNullOrEmpty(_retailPlaceDefalt))
                                {
                                    check.RetailPlace = _retailPlaceDefalt;
                                }
                            }
                            else if (_pointer_retailPlace > 1)
                            {
                                if (data[i, _pointer_retailPlace - 1] != null)
                                {
                                    check.RetailPlace = data[i, _pointer_retailPlace - 1].ToString();
                                }
                            }
                        }
                        // добавить разбор СНО
                        subExtErr = "Кассир"; 
                        if (_pointer_cashier == 0)
                        {
                            check.Cashier = _cashierDefault;
                        }
                        else if (_pointer_cashier > 0)
                        {
                            check.Cashier = data[i, _pointer_cashier].ToString();
                        }
                        subExtErr = "Признак расчета";
                        if (_pointer_operationTypeM5 <= 4)
                        {
                            check.CalculationSign = _pointer_operationTypeM5;
                        }
                        else
                        {
                            if (data[i, _pointer_operationTypeM5 - 4] != null)
                            {
                                string keyOperType = data[i, _pointer_operationTypeM5 - 4].ToString();
                                if (_operationTypesMap.ContainsKey(keyOperType))
                                { check.CalculationSign = _operationTypesMap[keyOperType]; }
                                else
                                {
                                    throw new Exception("Нет значения типа операции в словаре");
                                }
                            }
                            else
                            {
                                throw new Exception("Нет значения типа операции в таблице");
                            }

                        }
                        // тип документа
                        subExtErr = "Тип документа";
                        if (_pointer_documentTypeM2 == 1)
                        {
                            check.Document = FiscalPrinter.FD_DOCUMENT_NAME_CHEQUE;
                        }
                        else if (_pointer_documentTypeM2 == 0)
                        {
                            check.Document = FiscalPrinter.FD_DOCUMENT_NAME_CORRECTION_CHEQUE;
                        }
                        else
                        {
                            if (data[i, _pointer_documentTypeM2 - 1] != null)
                            {
                                string keyDocType = data[i, _pointer_documentTypeM2 - 1].ToString();
                                if (_docTypeMap.ContainsKey(keyDocType))
                                { check.Document = _docTypeMap[keyDocType]; }
                                else
                                {
                                    throw new Exception("Отсутвует значение в словаре типа документа");
                                }
                            }
                            else
                            {
                                throw new Exception("Отсутвует значение в таблице типа документа");
                            }
                        }
                        subExtErr = "Реквизиты коррекции";
                        // Реквизиты коррекции
                        if(check.Document == FiscalPrinter.FD_DOCUMENT_NAME_CORRECTION_CHEQUE)
                        {

                            if (_pointer_correctionDate == 0)
                            {
                                check.CorrectionDocumentDate = dateTimePicker_defaultCorrectionDate.Value;
                            }
                            else
                            {
                                if (data[i, _pointer_correctionDate] is DateTime)
                                {
                                    check.CorrectionDocumentDate = (DateTime)(data[i, _pointer_correctionDate]);
                                }
                                else
                                {
                                    check.CorrectionDocumentDate = DateTime.ParseExact(data[i, _pointer_correctionDate].ToString(), _dtFormat, CultureInfo.InvariantCulture);
                                }
                            }
                            if (_pointer_correctionTypeM1 <= 1)
                            {
                                check.CorrectionTypeFtag = _pointer_correctionTypeM1;
                            }
                            else
                            {
                                if (data[i, _pointer_correctionTypeM1].ToString() == "1")
                                {
                                    check.CorrectionTypeFtag = 1;
                                }
                                else
                                {
                                    check.CorrectionTypeFtag = 0;
                                }
                            }
                            if (_pointer_correctionOrderNumber == 0)
                            {
                                check.CorrectionOrderNumber = _correctionOrderNumberDefault;
                            }
                            else
                            {
                                check.CorrectionOrderNumber = data[i, _pointer_correctionOrderNumber].ToString();
                            }
                        }


                        /*
                         * * * разбираем предметрасчета * * * 
                         */
                        subExtErr = "Предмет расчета";
                        string itemsName = string.Empty;
                        double itemsQuantity = _itemsQuantityDefault;
                        double itemsSum = 0;
                        double itemsPrice = 0;
                        int itemsPaymentType = 0;
                        int itemsPproductType = 0;
                        int itemsNdsRate = 0;
                        subExtErr = "Предмет расчета: наименование";
                        if (_pointer_itemsName == 0)
                        {
                            itemsName = _itemsNameDefault;
                        }
                        else
                        {
                            itemsName = data[i, _pointer_itemsName].ToString(); ;
                        }
                        if (_pointer_itemsQuantity > 0)
                        {
                            object o = data[i, _pointer_itemsQuantity];
                            if(o is double || o is float || o is int || o is uint)
                            {
                                itemsQuantity = (double)o;
                            }
                            else //if(o is string)
                            {
                                itemsQuantity = double.Parse(o.ToString());
                            }

                        }
                        subExtErr = "Предмет расчета: сумма";
                        object itSumOb = data[i, _pointer_itemsSum];
                        if(itSumOb is double || itSumOb is int || itSumOb is uint)
                        {
                            itemsSum = Math.Round((double)itSumOb,2);
                        }
                        else
                        {
                            itemsSum = Math.Round(double.Parse(itSumOb.ToString()),2);
                        }
                        subExtErr = "Предмет расчета: цена";
                        if (_pointer_itemsPrice == 0)
                        {
                            itemsPrice = Math.Round(itemsSum / itemsQuantity,2);
                        }
                        else
                        {
                            object ipObj = data[i, _pointer_itemsPrice];
                            if (ipObj is double)
                                itemsPrice = Math.Round((double)(ipObj),2);
                            else
                            {
                                itemsPrice = Math.Round(double.Parse(ipObj.ToString()),2);
                            }
                        }
                        subExtErr = "Предмет расчета: признак способа расчета";
                        if (_pointer_itemsPaymentTypeSignM7 == 0)
                        {
                            itemsPaymentType = _itemsPaymentTypeSignDefault;
                        }
                        else
                        {
                            if(data[i, _pointer_itemsPaymentTypeSignM7] == null)
                            {
                                throw new Exception("Отсутвует знаение в таблице");
                            }
                            string itemsPaymentTypeKey = data[i, _pointer_itemsPaymentTypeSignM7].ToString();
                            if (_itemsPaymentTypeMap.ContainsKey(itemsPaymentTypeKey))
                            { itemsPaymentType = _itemsPaymentTypeMap[itemsPaymentTypeKey]; }
                            else {
                                throw new Exception("Отсутвует знаение в словаре");
                            }

                        }
                        subExtErr = "Предмет расчета: признак предмета расчета";
                        if (_pointer_itemsProductTypeM33 == 0)
                        {
                            itemsPproductType = _itemsProductTypeDefault;
                        }
                        else
                        {
                            if(data[i, _pointer_itemsProductTypeM33] == null)
                            {

                            }
                            string itemsProductTypeKey = data[i, _pointer_itemsProductTypeM33].ToString();
                            if (_itemsProductTypeMap.ContainsKey(itemsProductTypeKey))
                            { itemsPproductType = _itemsProductTypeMap[itemsProductTypeKey]; }
                            else
                            {
                                throw new Exception("Отсутвует знаение в словаре");
                            }

                        }
                        subExtErr = "Предмет расчета: ставка НДС";
                        if (_pointer_itemsNdsRate == 0)
                        {
                            itemsNdsRate = _itemsNdsRateDefault;
                        }
                        else
                        {
                            if (data[i, _pointer_itemsNdsRate] is int || data[i, _pointer_itemsNdsRate] is uint)
                            {
                                itemsNdsRate = (int)data[i, _pointer_itemsNdsRate];
                            }
                            else
                            {
                                itemsNdsRate = int.Parse(data[i, _pointer_itemsNdsRate].ToString());
                            }
                        }
                        subExtErr = "Предмет расчета";
                        ConsumptionItem item = new ConsumptionItem(itemsName, itemsPrice, itemsQuantity, itemsSum, itemsPproductType, itemsPaymentType, itemsNdsRate);
                        subExtErr = "items.itemsQuantityMeasure 2108";
                        if (_pointer_itemsUnit120 == 0)
                        {
                            item.Unit120 = _itemsUnit120Default;
                        }
                        else
                        {
                            if (_pointer_itemsUnit120 == 0)
                            {
                                if (data[i, _pointer_itemsUnit120] is int || data[i, _pointer_itemsUnit120] is uint)
                                {
                                    item.Unit120 = (int)data[i, _pointer_itemsUnit120];
                                }
                                else
                                {
                                    item.Unit120 = int.Parse(data[i, _pointer_itemsUnit120].ToString());
                                }
                            }
                        }
                        check.Items.Add(item);

                        
                        subExtErr = "Оплата чека";
                        /*
                         * Оплата чека
                         */

                        if (_pointer_checkPaidCash > 0)
                        {
                            object cashObj = data[i, _pointer_checkPaidCash];
                            if(cashObj is double || cashObj is float || cashObj is int || cashObj is uint)
                            {
                                check.Cash = Math.Round((double)cashObj, 2);
                            }
                            else
                            {
                                check.Cash = Math.Round(double.Parse(cashObj.ToString()), 2);
                            }
                        }
                        if (_pointer_checkPaidEcash > 0)
                        {
                            object ecashObj = data[i, _pointer_checkPaidEcash];
                            if (ecashObj is double || ecashObj is float || ecashObj is int || ecashObj is uint)
                            {
                                check.ECash = Math.Round((double)ecashObj, 2);
                            }
                            else
                            {
                                check.ECash = Math.Round(double.Parse(ecashObj.ToString()), 2);
                            }
                        }
                        if (_pointer_checkPaidPrepaid > 0)
                        {
                            object PrepObj = data[i, _pointer_checkPaidPrepaid];
                            if (PrepObj is double || PrepObj is float || PrepObj is int || PrepObj is uint)
                            {
                                check.Prepaid = Math.Round((double)PrepObj, 2);
                            }
                            else
                            {
                                check.Prepaid = Math.Round(double.Parse(PrepObj.ToString()), 2);
                            }
                        }
                        if (_pointer_checkPaidCredit > 0)
                        {
                            object credObj = data[i, _pointer_checkPaidCredit];
                            if (credObj is double || credObj is float || credObj is int || credObj is uint)
                            {
                                check.Credit = Math.Round((double)credObj, 2);
                            }
                            else
                            {
                                check.Credit = Math.Round(double.Parse(credObj.ToString()), 2);
                            }
                        }
                        if (_pointer_checkPaidProvision > 0)
                        {
                            object otherObj = data[i, _pointer_checkPaidProvision];
                            if (otherObj is double || otherObj is float || otherObj is int || otherObj is uint)
                            {
                                check.Provision = Math.Round((double)otherObj, 2);
                            }
                            else
                            {
                                check.Provision = Math.Round(double.Parse(otherObj.ToString()), 2);
                            }
                        }

                        if (_manualTaxes)
                        {
                            subExtErr = "НДС 20 чека";
                            if (_pointer_checkTax_20 > 0)
                            {
                                check.Nds20 = Math.Round(double.Parse(data[i, _pointer_checkTax_20].ToString()), 2);
                            }
                            subExtErr = "НДС 10 чека";
                            if (_pointer_checkTax_10 > 0)
                            {
                                check.Nds10 = Math.Round(double.Parse(data[i, _pointer_checkTax_10].ToString()), 2);
                            }
                            subExtErr = "НДС 20|120 чека";
                            if (_pointer_checkTax_20120 > 0)
                            {
                                check.Nds20120 = Math.Round(double.Parse(data[i, _pointer_checkTax_20120].ToString()), 2);
                            }
                            subExtErr = "НДС 10|110 чека";
                            if (_pointer_checkTax_10110 > 0)
                            {
                                check.Nds10110 = Math.Round(double.Parse(data[i, _pointer_checkTax_10110].ToString()), 2);
                            }
                            subExtErr = "НДС 0 чека";
                            if (_pointer_checkTax_0 > 0)
                            {
                                check.Nds0 = Math.Round(double.Parse(data[i, _pointer_checkTax_0].ToString()), 2);
                            }
                            subExtErr = "6E3 НДС  чека";
                            if (_pointer_checkTax_free > 0)
                            {
                                check.NdsFree = Math.Round(double.Parse(data[i, _pointer_checkTax_free].ToString()), 2);
                            }
                            subExtErr = "НДС 5 чека";
                            if (_pointer_checkTax_5 > 0)
                            {
                                check.Nds5 = Math.Round(double.Parse(data[i, _pointer_checkTax_5].ToString()), 2);
                            }
                            subExtErr = "НДС 7 чека";
                            if (_pointer_checkTax_7 > 0)
                            {
                                check.Nds7 = Math.Round(double.Parse(data[i, _pointer_checkTax_7].ToString()), 2);
                            }
                            subExtErr = "НДС 5|105 чека";
                            if (_pointer_checkTax_5105 > 0)
                            {
                                check.Nds5105 = Math.Round(double.Parse(data[i, _pointer_checkTax_5105].ToString()), 2);
                            }
                            subExtErr = "НДС 7|107 чека";
                            if (_pointer_checkTax_7107 > 0)
                            {
                                check.Nds7107 = Math.Round(double.Parse(data[i, _pointer_checkTax_7107].ToString()), 2);
                            }
                        }


                    }
                    catch (Exception exc)
                    {
                        errRows.Add(i);
                        if (subExtErr != "")
                            AddReportLogMsg(subExtErr,1);
                        AddReportLogMsg("Строка: "+i+" \t"+exc.Message,1);
                        LogHandle.ol(exc.StackTrace);
                        errorsOccured++;
                    }
                    fr.ReadDeviceCondition();
                    if (_pointer_checkId > 0)
                    { lastCheckId = data[i, _pointer_checkId].ToString(); }
                    
                }
                if (check.Items.Count > 0)
                {
                    double paydSum = Math.Round(check.Cash + check.ECash + check.Prepaid + check.Credit + check.Provision, 2);
                    // перерасчет  налогов
                    check.Control(true);
                    // если в чеке оккругление компенсируем
                    check.TotalSum = paydSum;
                    if (!fr.PerformFD(check))
                    { errorsOccured++; }
                    fr.ReadDeviceCondition();
                }

            }
            else
            {
                AddMessage("Таблица не причитана или пустая"); 
                errorsOccured = 100000;
            }
            if (errRows.Count > 0)
            {
                StringBuilder sb = new StringBuilder("Строки с ошибками: ");
                foreach (int row in errRows)
                    sb.Append(row.ToString()+ ", ");
                AddReportLogMsg(sb.ToString(),1);
            }

            return errorsOccured <= errorsAllowed;
        }

        static Form form = null;
        static RichTextBox log = null;
        public static void AddMessage(string message)
        {
            if (form != null && form.Created && log != null)
            {
                MethodInvoker method = delegate
                {
                    if (log.Text.Length > 2048)
                    {
                        log.Text = "log cleared..." + Environment.NewLine;

                    }
                    log.AppendText(message + Environment.NewLine);
                    log.SelectionStart = log.TextLength;
                    log.ScrollToCaret();
                };
                if (form.InvokeRequired)
                    form.BeginInvoke(method);
                else
                    method.Invoke();
            }
            LogHandle.ol(message);
        }

        string logFile = null;
        List<int> errRows;
        void AddReportLogMsg(string msg, int lvl = 0)
        {
            if(!string.IsNullOrEmpty(logFile))
                System.IO.File.AppendAllText(logFile, msg + Environment.NewLine);
            if (lvl != 0)
                AddMessage(msg);
        }

        bool _originalDontPrintSign = false;
        private void FormOfdExport_FormClosing(object sender, FormClosingEventArgs e)
        {
            fiscalPrinter.DontPrint = _originalDontPrintSign;
            AddMessage("Восстанавливаем оригинальные настройки перезаписи адреса и места");
            AppSettings.OverideRetailAddress = _overrideAddressOriginal;
            AppSettings.OverideRetailPlace = _overridePlaceOriginal;
        }

    }
}
