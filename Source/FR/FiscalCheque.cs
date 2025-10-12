using System;
using System.Collections.Generic;
using System.Text;
using static FR_Operator.FiscalPrinter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace FR_Operator
{
    public class FiscalCheque : ICloneable
    {
        public int FFDVer
        {
            get { return _docVersion; }
            set
            {
                if (value == FR_FFD105 || value == FR_FFD110 || value == FR_FFD120) _docVersion = value;
                else if (value == 2)
                { _docVersion = FR_FFD105; }
                else if (value == 3)
                {
                    _docVersion = FR_FFD110;
                }
                else if (value == 4)
                {
                    _docVersion = FR_FFD120;
                }
            }
        }
        private int _docVersion = FR_FFD105;

        public int Sno
        {
            get
            {
                if (_sno > FR_REGISTERD_SNO_KEY)
                    return _sno - FR_REGISTERD_SNO_KEY;
                else
                    return _sno;
            }
            set
            {
                if ((value > FR_REGISTERD_SNO_KEY && value <= FR_SNO_PSN) || value == 1 || value == 2 || value == 4 || value == 8 || value == 16 || value == 32)
                    _sno = value;
            }
        }
        private int _sno = NONE;

        // чек/чек коррекции
        public int Document
        {
            get 
            { 
                return _documentName; 
            }
            set
            {
                if (value == FD_DOCUMENT_NAME_CHEQUE || value == FD_DOCUMENT_NAME_CORRECTION_CHEQUE) _documentName = value;
                else if (value == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE || /*value == FD_DOC_TYPE_BSO ||*/ value == FTAG_FISCAL_DOCUMENT_TYPE_BSO) _documentName = FD_DOCUMENT_NAME_CHEQUE;
                else if (value == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE || /*value == FD_DOC_TYPE_CORRECTION_BSO ||*/ value == FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION) _documentName = FD_DOCUMENT_NAME_CORRECTION_CHEQUE;
            }
        }
        private int _documentName = FD_DOCUMENT_NAME_CHEQUE;
        public int DocumentNameFtagType { 
            get 
            {
                if (_documentName == FD_DOCUMENT_NAME_CHEQUE) return FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE;
                if (_documentName == FD_DOCUMENT_NAME_CORRECTION_CHEQUE) return FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE;
                if (_documentName == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE || _documentName == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE) return _documentName;
                return 0;
            } 
        }

        // Признак расчета
        public int CalculationSign
        {
            get
            {
                if (_calculationSign > FD_CALCULATION_SIGN) return _calculationSign - FD_CALCULATION_SIGN;
                else return _calculationSign;
            }
            set
            {
                _calculationSign = value;
                //возможно нужно добавить проверку на корректность соответсвия ФФД
            }
        }
        private int _calculationSign = FD_CALCULATION_INCOME_LOC;

        // Кассир
        public string Cashier
        {
            get
            {
                return _cashier==null ? "":_cashier;
            }
            set
            {
                if (value != null && value != "")
                {
                    if (value.Length > _cashierMaxLenth) _cashier = value.Substring(0, _cashierMaxLenth);
                    else _cashier = value;
                }

            }
        }
        private string _cashier = AppSettings.CashierDefault;
        private static int _cashierMaxLenth = 50;

        // ИНН кассира
        public string CashierInn
        {
            get { return _cashierInn == null ? "" : _cashierInn; }
            set
            {
                if (CorrectInn(value))
                    _cashierInn = value;
            }
        }
        private string _cashierInn = AppSettings.CashierInnDefault;

        public string EmailPhone
        {
            get { return _emailPhone == null ? "" :  _emailPhone; }
            set { _emailPhone = value; }
        }
        private string _emailPhone = "";

        // Сведения о покупателе
        public bool BuyerInformation
        {
            get
            {
                return (!string.IsNullOrEmpty(_buyerName) )
                    || (!string.IsNullOrEmpty(_buyerInn) )
                    || (!string.IsNullOrEmpty(_buyerInformationBuyerBirthday) )
                    || (!string.IsNullOrEmpty(_buyerInformationBuyerCitizenship))
                    || (!string.IsNullOrEmpty(_buyerInformationBuyerDocumentCode))
                    || (!string.IsNullOrEmpty(_buyerInformationBuyerDocumentData))
                    || (!string.IsNullOrEmpty(_buyerInformationBuyerAddress));
            }
        }
        public string BuyerInformationBuyer
        {
            get { return _buyerName; }
            set { _buyerName = value; }
        }
        private string _buyerName = "";
        public string BuyerInformationBuyerInn
        {
            get { return _buyerInn; }
            set 
            {
                value = value.Trim();
                if (CorrectInn(value)) _buyerInn = value;
                else _buyerInn = "";
            }
        }
        private string _buyerInn = "";

        public string BuyerInformationBuyerBirthday
        {
            get { return _buyerInformationBuyerBirthday; }
            set 
            {
                try
                {
                    string biBdStr = value;
                    if (biBdStr.Length > 10)
                    {
                        biBdStr=biBdStr.Trim();
                        if (biBdStr.Length > 10)
                        {
                            biBdStr = biBdStr.Substring(0, 10);
                        }
                    }

                    DateTime dt = new DateTime(1900,1,1);
                    dt = DateTime.ParseExact(biBdStr, DEFAULT_D_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
                    _buyerInformationBuyerBirthday = dt.ToString(DEFAULT_D_FORMAT);
                }
                catch 
                {
                    _buyerInformationBuyerBirthday = "";
                    LogHandle.ol("Передан некорректный формат даты в тег ДР покупателя: "+value);
                }
            }
        }
        private string _buyerInformationBuyerBirthday = "";
        public string BuyerInformationBuyerCitizenship
        {
            get { return _buyerInformationBuyerCitizenship; }
            set
            {
                try
                {
                    if (value != null)
                    {
                        string countryCode = value.Trim();
                        if (countryCode.Length <= 3)
                        {
                            int code = int.Parse(countryCode);
                            _buyerInformationBuyerCitizenship = countryCode;
                        }
                        else
                        {
                            _buyerInformationBuyerCitizenship = "";
                        }
                    }
                    else
                    {
                        _buyerInformationBuyerCitizenship = "";
                    }

                }
                catch 
                {
                    _buyerInformationBuyerCitizenship = "";
                }
            }
        }
        private string _buyerInformationBuyerCitizenship = "";
        public string BuyerInformationBuyerDocumentCode
        {
            get { return _buyerInformationBuyerDocumentCode; }
            set 
            {
                if (value == null || value == "")
                {
                    _buyerInformationBuyerDocumentCode = string.Empty;
                    return;
                }
                int docCode = -1;
                string docCodeStr = value.Trim();
                int.TryParse(docCodeStr, out docCode);

                if (docCode > 0)
                {
                    if( docCode == 21 ||
                        docCode == 22 ||
                        docCode == 26 ||
                        docCode == 27 ||
                        docCode == 28 ||
                        docCode == 31 ||
                        docCode == 32 ||
                        docCode == 33 ||
                        docCode == 34 ||
                        docCode == 35 ||
                        docCode == 36 ||
                        docCode == 37 ||
                        docCode == 38 ||
                        docCode == 40)
                    {
                        // данные соответсвуют ФФД
                        _buyerInformationBuyerDocumentCode = docCode.ToString();
                        return;
                    }
                    if(docCode < 30 && docCode >=20 )
                    {
                        // данные не соответсвуют ФФД устнавливаем 28 Иные документы гр РФ
                        _buyerInformationBuyerDocumentCode = "28";
                        return;
                    }
                    if(docCode == 39 ||docCode ==30)
                    {
                        // данные не соответсвуют ФФД устнавливаем 32 Иные документы ин гр
                        _buyerInformationBuyerDocumentCode = "32";
                        return;
                    }

                }
                // код документа отрицательный или его невозможно распознать
                _buyerInformationBuyerDocumentCode = "";

            }
        }
        private string _buyerInformationBuyerDocumentCode = "";
        public string BuyerInformationBuyerDocumentData
        {
            get { return _buyerInformationBuyerDocumentData; }
            set 
            {
                if (value != null)
                {
                    if (value.Length > 64)
                    {
                        _buyerInformationBuyerDocumentData = value.Substring(0, 64);
                    }
                    else
                        _buyerInformationBuyerDocumentData = value;
                }
                
            }
        }
        private string _buyerInformationBuyerDocumentData = "";
        public string BuyerInformationBuyerAddress
        {
            get { return _buyerInformationBuyerAddress; }
            set
            {
                if(value != null)
                {
                    if (value.Length <= 256)
                    {
                        _buyerInformationBuyerAddress = value;
                    }
                    else
                    {
                        _buyerInformationBuyerAddress = value.Substring(0, 256);
                    }
                }
                    
            }
        }
        private string _buyerInformationBuyerAddress = "";

        // 1192  property data
        private string _propertiesData = "";
        public string PropertiesData
        {
            get => _propertiesData;
            set
            {
                if( value!=null)
                {
                    if(value.Length <= 16 )
                        _propertiesData = value;
                    else 
                        _propertiesData = value.Substring(0, 16);
                }
                
            }
        }
        public bool IsPropertiesData
        {
            get => !string.IsNullOrEmpty(_propertiesData);
        }

        // 1174 Основание для коррекции составной STLV
        //      1173 Тип коррекции
        // !!!! убрать константы и оставить только теги - множественность плодит ошибки
        public int CorrectionTypeNotFtag
        {
            get
            {
                if (_correctionTypeNotFtag > FD_CORRECTION_TYPE)
                    return _correctionTypeNotFtag - FD_CORRECTION_TYPE_SELF_MADE;
                else
                    return _correctionTypeNotFtag;
            }
            set
            {
                if (value == FD_CORRECTION_TYPE_SELF_MADE || value == FD_CORRECTION_TYPE_BY_ORDER) 
                { 
                    _correctionTypeNotFtag = value;
                    if (value == FD_CORRECTION_TYPE_SELF_MADE)
                        _correctionTypeFtag = 0;
                    else if (value == FD_CORRECTION_TYPE_BY_ORDER)
                        _correctionTypeFtag = 1;
                }
                else if(value == 1|| value == 0)
                {
                    _correctionTypeFtag = value;
                    if(value == 0)
                    {
                        _correctionTypeNotFtag = FD_CORRECTION_TYPE_SELF_MADE;
                    }
                    else if(value == 1)
                    {
                        _correctionTypeNotFtag = FD_CORRECTION_TYPE_BY_ORDER;
                    }
                }
            }
        }
        public int CorrectionTypeFtag
        {
            get
            {
                return _correctionTypeFtag;
            }
            set
            {
                if(value == 0)
                {
                    _correctionTypeNotFtag = FD_CORRECTION_TYPE_SELF_MADE;
                    _correctionTypeFtag = 0;
                }
                else if(value == 1)
                {
                    _correctionTypeNotFtag = FD_CORRECTION_TYPE_BY_ORDER;
                    _correctionTypeFtag = 1;
                }
            }
        }
        private int _correctionTypeNotFtag = FD_CORRECTION_TYPE_SELF_MADE;
        private int _correctionTypeFtag = 0;
        //      1177 Описание коррекции *упраздненный тег*
        public string CorrectionDocDescriber
        {
            get { return _correctionDocDescriber; }
            set
            {
                if (value != null && value != "")
                    if (value.Length > 31) value = value.Substring(0, 31);
                _correctionDocDescriber = value;
            }
        }
        private string _correctionDocDescriber = "*";
        //      1178 Дата документа основания для коррекции
        public DateTime CorrectionDocumentDate
        {
            get { return _correctionDocDate; }
            set { _correctionDocDate = value.Date; }
        }
        private DateTime _correctionDocDate = DateTime.Now.Date;
        //      1179 Номер документа основания для коррекции
        public string CorrectionOrderNumber
        {
            get { return _correctionOrderNumber; }
            set
            {
                if (value != null /*&& value != ""*/)
                    if (value.Length > 31) value = value.Substring(0, 31);
                _correctionOrderNumber = value;
            }
        }
        private string _correctionOrderNumber =  AppSettings.CorrectionOrderNumberDefault;

        // 1084 properties{1085 properties.propertyName; 1086 properties.propertyValue }
        private string _propertiesPropertyName = string.Empty;
        private string _propertiesPropertyValue = string.Empty;
        public bool IsProperties1084 { get => !string.IsNullOrEmpty(_propertiesPropertyName) || !string.IsNullOrEmpty(_propertiesPropertyValue); }
        public string PropertiesPropertyName
        {
            get => _propertiesPropertyName; 
            set => _propertiesPropertyName = value;
        }
        public string PropertiesPropertyValue
        {
            get => _propertiesPropertyValue; 
            set => _propertiesPropertyValue = value; 
        }

        // Предметы расчета
        List<ConsumptionItem> _items = new List<ConsumptionItem>();
        public List<ConsumptionItem> Items
        {
            get { return _items; }
        }

        public void ClearItems()
        {
            _items.Clear();
            Control();
        }

        public void AddItem(ConsumptionItem item)
        {
            _items.Add(item);
        }

        private double[] _sums = new double[16];    // итоговые суммы чека
        
        //          оплата
        // 1020 ИТОГ
        public double TotalSum
        {
            get { return _sums[FD_SUMS_TOTAL_SUM_LOC]; }
            // округление сделать в отдельном методе
            set
            {
                if (value >=0 )
                    _sums[FD_SUMS_TOTAL_SUM_LOC] = Math.Round(value, 2);
            }

        }
        // 1031 Сумма по чеку (БСО) наличными
        public double Cash
        {
            get { return _sums[FD_SUMS_PAY_CASH_LOC]; }
            set
            {
                if (value >= 0)
                    _sums[FD_SUMS_PAY_CASH_LOC] = Math.Round(value, 2);

            }
        }
        // 1081 Сумма по чеку(БСО) электронными/безналичными
        public double ECash
        {
            get { return _sums[FD_SUMS_PAY_ECASH_LOC]; }
            set
            {
                if (value >= 0)
                    _sums[FD_SUMS_PAY_ECASH_LOC] = Math.Round(value, 2);
            }
        }
        // 1215 Сумма по чеку (БСО) предоплатой (зачетом аванса и (или) предыдущих платежей)
        public double Prepaid
        {
            get { return _sums[FD_SUMS_PAY_PREPAID_LOC]; }
            set
            {
                if (value >= 0)
                    _sums[FD_SUMS_PAY_PREPAID_LOC] = Math.Round(value, 2);
            }
        }
        // 1216 Сумма по чеку (БСО) постоплатой (в кредит)
        public double Credit
        {
            get { return _sums[FD_SUMS_PAY_CREDIT_LOC]; }
            set
            {
                if (value >= 0)
                    _sums[FD_SUMS_PAY_CREDIT_LOC] = Math.Round(value, 2);
            }
        }
        // 1217 Сумма по чеку (БСО) встречным предоставлением
        public double Provision
        {
            get { return _sums[FD_SUMS_PAY_PROVISION_LOC]; }
            set
            {
                if (value >= 0)
                    _sums[FD_SUMS_PAY_PROVISION_LOC] = Math.Round(value, 2);
            }
        }

        //          налоги
        // 1102 Сумма НДС чека по ставке 20%
        public double Nds20
        {
            get { return _sums[FD_SUMS_NDS_20_LOC]; }
            set
            {
                if (value >= 0)
                    _sums[FD_SUMS_NDS_20_LOC] = value;
            }
        }
        // 1103 Сумма НДС чека по ставке 10%
        public double Nds10
        {
            get { return _sums[FD_SUMS_NDS_10_LOC]; }
            set
            {
                if (value >= 0)
                    _sums[FD_SUMS_NDS_10_LOC] = value;
            }
        }
        // 1104 Сумма расчета по чеку с НДС по ставке 0%
        public double Nds0
        {
            get { return _sums[FD_SUMS_NDS_0_LOC]; }
            set
            {
                if (value >= 0)
                    _sums[FD_SUMS_NDS_0_LOC] = value;
            }
        }
        // 1105 Сумма расчета по чеку без НДС
        public double NdsFree
        {
            get { return _sums[FD_SUMS_NDS_FREE_LOC]; }
            set
            {
                if (value >= 0 )
                    _sums[FD_SUMS_NDS_FREE_LOC] = value;
            }
        }
        // 1106 Сумма НДС чека по расч. ставке 20/120
        public double Nds20120
        {
            get { return _sums[FD_SUMS_NDS_20120_LOC]; }
            set
            {
                if (value >= 0 )
                    _sums[FD_SUMS_NDS_20120_LOC] = value;
            }
        }
        // 1107 Сумма НДС чека по расч. ставке 10/110
        public double Nds10110
        {
            get { return _sums[FD_SUMS_NDS_10110_LOC]; }
            set
            {
                if (value >= 0)
                    _sums[FD_SUMS_NDS_10110_LOC] = value;
            }
        }
        public double Nds5
        {
            get { return _sums[FD_SUMS_NDS_5_LOC]; }
            set
            {
                if (value >= 0)
                    _sums[FD_SUMS_NDS_5_LOC] = value;
            }
        }
        public double Nds7
        {
            get { return _sums[FD_SUMS_NDS_7_LOC]; }
            set
            {
                if (value >= 0)
                    _sums[FD_SUMS_NDS_7_LOC] = value;
            }
        }
        public double Nds5105
        {
            get { return _sums[FD_SUMS_NDS_5105_LOC]; }
            set
            {
                if (value >= 0)
                    _sums[FD_SUMS_NDS_5105_LOC] = value;
            }
        }
        public double Nds7107
        {
            get { return _sums[FD_SUMS_NDS_7107_LOC]; }
            set
            {
                if (value >= 0)
                    _sums[FD_SUMS_NDS_7107_LOC] = value;
            }
        }

        // 1009 retailAddress
        string _retailAddress = string.Empty;
        public string RetailAddress
        {
            get => _retailAddress;
            set => _retailAddress = value;
        }

        // 1187 retailPlace
        string _retailPlace = string.Empty;
        public string RetailPlace
        {
            get => _retailPlace;
            set => _retailPlace = value;
        }
        // 1125 internetPayment
        bool _internetPayment;
        public bool InternetPayment
        {
            get => _internetPayment;
            set => _internetPayment = value;
        }

        bool _availableCommonTaxes = true;
        public bool AvailableCommonTaxes
        {
            get { return _availableCommonTaxes; }
        }

        // ФЛК документа 0 - ошибок и предупреждений нет
        private int _docControlFlags = 0;
        public int DocControlFlags
        {
            get { return _docControlFlags; }
        }
        public bool IsNotPaid
        {
            get { return (_docControlFlags & FD_DC_ERROR_NOT_ENOUTH_PAID_BF) != 0; }
        }
        public bool IsOverPaid
        {
            get { return (_docControlFlags & FD_DC_ERROR_OVERPAID_CRITICAL_BF) != 0; }
        }
        public bool WithChange
        {
            get { return (_docControlFlags & FD_DC_ERROR_OVERPAID_WITH_CHANGE_BF) != 0; }
        }
        public void PaySumsClear(bool reCalculate = false)
        {
            _sums[FD_SUMS_PAY_CASH_LOC] = 0;
            _sums[FD_SUMS_PAY_ECASH_LOC] = 0;
            _sums[FD_SUMS_PAY_PREPAID_LOC] = 0;
            _sums[FD_SUMS_PAY_CREDIT_LOC] = 0;
            _sums[FD_SUMS_PAY_PROVISION_LOC] = 0;
            if (reCalculate)
                Control(false);
        }

        public void Control(bool count = true)
        {
            if (_documentName == FD_DOCUMENT_NAME_CORRECTION_CHEQUE && _sums[FD_SUMS_TOTAL_SUM_LOC] > 0.001 && _items.Count == 0 && _docVersion < FR_FFD110)
                count = false;

            if (count)
            {
                _sums[FD_SUMS_TOTAL_SUM_LOC] = 0;
                _sums[FD_SUMS_NDS_20_LOC] = 0;
                _sums[FD_SUMS_NDS_10_LOC] = 0;
                _sums[FD_SUMS_NDS_20120_LOC] = 0;
                _sums[FD_SUMS_NDS_10110_LOC] = 0;
                _sums[FD_SUMS_NDS_0_LOC] = 0;
                _sums[FD_SUMS_NDS_FREE_LOC] = 0;
                _sums[FD_SUMS_NDS_5_LOC] = 0;
                _sums[FD_SUMS_NDS_7_LOC] = 0;
                _sums[FD_SUMS_NDS_5105_LOC] = 0;
                _sums[FD_SUMS_NDS_7107_LOC] = 0;
                _availableCommonTaxes = true;
                double noTaxesSum = 0;
                foreach (ConsumptionItem item in _items)
                {
                    _sums[FD_SUMS_TOTAL_SUM_LOC] += item.Sum;
                    if (item.NdsRate != 0)
                        _sums[item.NdsRate + 5] += item.UnitNnds;
                    else noTaxesSum += item.Sum;
                }
                for (int i = 0; i < 12; i++) _sums[i] = Math.Round(_sums[i], 2);

                _availableCommonTaxes = noTaxesSum >= 0.01;
            }
            _docControlFlags = 0;       // поменять
            //LogHandle.ol("Checking FD");

            double tally = _sums[FD_SUMS_PAY_CASH_LOC]
                + _sums[FD_SUMS_PAY_ECASH_LOC]
                + _sums[FD_SUMS_PAY_PREPAID_LOC]
                + _sums[FD_SUMS_PAY_CREDIT_LOC]
                + _sums[FD_SUMS_PAY_PROVISION_LOC];

            double deltaPenns = Math.Round(tally, 2) - Math.Round(_sums[FD_SUMS_TOTAL_SUM_LOC], 2);
            if (deltaPenns > -0.001 && deltaPenns < 0.001)
            {
                LogHandle.ol("Doc paid exactly");
                _docControlFlags = _docControlFlags | FD_DC_PAID_EXACT_BF;
            }
            else if (deltaPenns > 0.01 && deltaPenns <= _sums[FD_SUMS_PAY_CASH_LOC] + 0.001)
            {
                LogHandle.ol("Doc paid with change");
                _docControlFlags = _docControlFlags | FD_DC_ERROR_OVERPAID_WITH_CHANGE_BF;
            }
            else if (deltaPenns > 0.011)
            {
                LogHandle.ol("Doc overpaid criticaly");
                _docControlFlags = _docControlFlags | FD_DC_ERROR_OVERPAID_CRITICAL_BF;
                _docControlFlags = _docControlFlags | FD_DC_CRITICAL_ERROR_BF;
            }
            else if (deltaPenns < 0.001)
            {
                LogHandle.ol("Doc not paid enouth");
                _docControlFlags = _docControlFlags | FD_DC_ERROR_NOT_ENOUTH_PAID_BF;
                _docControlFlags = _docControlFlags | FD_DC_CRITICAL_ERROR_BF;
            }


            // добавить проверку сумм НДС
            foreach (ConsumptionItem item in _items)
            {
                if (item.Correctness == FD_ITEM_CONTROL_OK)
                    continue;
                if (item.Correctness == FD_ITEM_CONTROL_CRITICAL_ERROR)
                {
                    LogHandle.ol("Doc contains 1 or more bad item in " + item);
                    _docControlFlags = _docControlFlags | FD_DC_ERROR_BAD_ITEM_BF;
                    break;
                }
                if (item.Correctness == FD_ITEM_CONTROL_WARN_CODE)
                    _docControlFlags = _docControlFlags | FD_DC_WARN_WARNED_ITEM_BF;
            }
        }

        public string Condition()
        {
            if (_docControlFlags == 0)
                return "нет информации";
            StringBuilder errors = new StringBuilder();
            for (int i = 1; i < 214748364; i *= 2)
            {
                if ((_docControlFlags & i) != 0 && DocControl.ContainsKey(i))
                {
                    errors.Append(DocControl[i]);
                    errors.Append(Environment.NewLine);
                }
            }
            return errors.ToString();
        }


        public string ToString(int type = NONE)
        {
            
            double taxSum = Math.Round(_sums[FD_SUMS_NDS_20_LOC] +
                    _sums[FD_SUMS_NDS_20120_LOC] +
                    _sums[FD_SUMS_NDS_10_LOC] +
                    _sums[FD_SUMS_NDS_10110_LOC]+
                    _sums[FD_SUMS_NDS_5_LOC]+
                    _sums[FD_SUMS_NDS_5105_LOC]+
                    _sums[FD_SUMS_NDS_7_LOC]+
                    _sums[FD_SUMS_NDS_7107_LOC],2) ;
            StringBuilder s = new StringBuilder();

            if ( type == NONE)
            {
                if (_documentName == FD_DOCUMENT_NAME_CHEQUE)
                    s.Append("Чек ");
                else
                    s.Append("Чек коррекции ");
                s.Append(FiscalPrinter.FiscalOperationType[CalculationSign]);
                
                s.Append(" ИТ=");
                s.Append(_sums[FD_SUMS_TOTAL_SUM_LOC]);
                if(taxSum > 0.0099)
                {
                    s.Append(" НДС=");
                    s.Append(taxSum);
                }
                if (_sums[FD_SUMS_PAY_CASH_LOC] > 0.0099)
                {
                    s.Append(" Нал=");
                    s.Append(_sums[FD_SUMS_PAY_CASH_LOC]);
                }
                if (_sums[FD_SUMS_PAY_ECASH_LOC] > 0.0099)
                {
                    s.Append(" БН=");
                    s.Append(_sums[FD_SUMS_PAY_ECASH_LOC]);
                }
                if (_sums[FD_SUMS_PAY_PREPAID_LOC] > 0.0099)
                {
                    s.Append(" Ав=");
                    s.Append(_sums[FD_SUMS_PAY_PREPAID_LOC]);
                }
                if (_sums[FD_SUMS_PAY_CREDIT_LOC] > 0.0099)
                {
                    s.Append(" Кр=");
                    s.Append(_sums[FD_SUMS_PAY_CREDIT_LOC]);
                }
                if (_sums[FD_SUMS_PAY_PROVISION_LOC] > 0.0099)
                {
                    s.Append(" ВП=");
                    s.Append(_sums[FD_SUMS_PAY_PROVISION_LOC]);
                }
                /*double tax = Nds10 + Nds10110 + Nds20 + Nds20120 + Nds5 + Nds7 + Nds5105 + Nds7107;
                if (tax > 0.0099)
                {
                    s.Append(" в т.ч. НДС=" + Math.Round(tax, 2));
                }*/
                
            }
            if(type == SHORT_INFO)
            {
                if (_documentName == FD_DOCUMENT_NAME_CHEQUE)
                { s.Append("Чек "); }
                else
                { s.Append("Кор "); }
                if (_calculationSign == 0)
                { s.Append("ошибка "); }
                else if (_calculationSign == 1)
                {
                    s.Append("прх{");
                }
                else if(_calculationSign == 2)
                {
                    s.Append("вп{");
                }
                else if(_calculationSign == 3)
                {
                    s.Append("рсх{");
                }
                else if(_calculationSign == 4)
                {
                    s.Append("вр{");
                }
                s.Append("ИТ");
                s.Append(_sums[FD_SUMS_TOTAL_SUM_LOC]);
                if (Cash > 0.0099)
                {
                    s.Append(" Н=");
                    s.Append(Cash);
                }
                if (ECash > 0.0099)
                {
                    s.Append(" Б=");
                    s.Append(ECash);
                }
                if (Prepaid > 0.0099)
                {
                    s.Append(" А=");
                    s.Append(Prepaid);
                }
                if (Credit > 0.0099)
                {
                    s.Append(" К=");
                    s.Append(Credit);
                }
                if (Provision > 0.0099)
                {
                    s.Append(" В=");
                    s.Append(Provision);
                }
                double tax = Nds10 + Nds10110 + Nds20 + Nds20120 + Nds5 + Nds7 + Nds5105 + Nds7107;
                if (tax > 0.0099)
                {
                    s.Append(" НДС=" + Math.Round(tax, 2));
                }
                s.Append('}');
            }
            if(type == FULL_INFO)
            {
                s.AppendLine(ToString(NONE));
                s.AppendLine("\tСНО:" + TaxSystem[Sno]);
                if (AppSettings.OverideRetailAddress && !string.IsNullOrEmpty(_retailAddress))
                {
                    s.AppendLine("Адрес расчетов: " + _retailAddress);
                }
                if (AppSettings.OverideRetailPlace)
                {
                    s.AppendLine("Место расчетов: " + _retailPlace);
                }
                if (_internetPayment)
                {
                    s.AppendLine("Расчет в интернете");
                }
                if (IsPropertiesData)
                {
                    s.AppendLine("1192-доп реквизит чека: "+_propertiesData);
                }
                if (IsProperties1084)
                {
                    s.AppendLine("1084-доп реквизит пользователя{ 1085-propertiesPropertyName:" + _propertiesPropertyName+ "; 1086-propertiesPropertyValue: " + _propertiesPropertyValue+"; }");
                }
                if(!string.IsNullOrEmpty(_cashier))
                    s.Append("\tКассир:"+_cashier);
                if(!string.IsNullOrEmpty(_cashierInn))
                    s.Append("\tИНН кассира:"+_cashierInn);
                if (!string.IsNullOrEmpty(_cashier) || !string.IsNullOrEmpty(_cashierInn))
                    s.AppendLine();

                if (!string.IsNullOrEmpty(_emailPhone))
                    s.AppendLine("\ttel/email покупателя:"+_emailPhone);
                if (!string.IsNullOrEmpty(_buyerName))
                    s.Append("\tПокупатель:"+_buyerName);
                if (!string.IsNullOrEmpty(_buyerInn))
                    s.Append("\tИНН покупателя:" + _buyerInn);
                if (!string.IsNullOrEmpty(_buyerInformationBuyerBirthday))
                    s.Append("\tД/Р покупателя:" + _buyerInformationBuyerBirthday);
                if (!string.IsNullOrEmpty(_buyerInformationBuyerCitizenship))
                    s.Append("\tГражданство покупателя:" + _buyerInformationBuyerCitizenship);
                if (!string.IsNullOrEmpty(_buyerInformationBuyerDocumentCode))
                    s.Append("\tКод документа покупателя:" + _buyerInformationBuyerDocumentCode);
                if (!string.IsNullOrEmpty(_buyerInformationBuyerDocumentData))
                    s.Append("\tДанные документа покупателя:" + _buyerInformationBuyerDocumentData);
                if (!string.IsNullOrEmpty(_buyerInformationBuyerAddress))
                    s.Append("\tАдрес покупателя:" + _buyerInformationBuyerAddress);
                if (BuyerInformation)
                    s.AppendLine();

                /*if (!string.IsNullOrEmpty(_propertiesData))
                    s.AppendLine("\tДополнительный реквизит чека:" + _propertiesData);*/

                if(_documentName == FD_DOCUMENT_NAME_CORRECTION_CHEQUE)
                {
                    s.AppendLine("Тип коррекции: " + (_correctionTypeNotFtag == 0 || _correctionTypeNotFtag == 11311 ? "0 Самостоятельно" : "1 По предписанию"));
                    s.AppendLine("Дата расчета: "+_correctionDocDate.ToString(DEFAULT_D_FORMAT));
                    if (!string.IsNullOrEmpty(_correctionOrderNumber))
                        s.AppendLine("Номер предписания: "+_correctionOrderNumber);
                }

                
                if(type == FULL_INFO)
                {
                    s.AppendLine("\tПредметы расчета:");
                    foreach(var item in _items)s.AppendLine("\t\t"+ ItemTypeDscr[item.ProductType,1]+"\t"+ ItemPaymentTypeDscr[item.PaymentType] + "\t" + TaxRateDscr[item.NdsRate] + "\t" + item.ToString());
                }

            }
            else if (type == EXTENDED_PF)
            {
                if (_documentName == FD_DOCUMENT_NAME_CHEQUE)
                    s.Append("Чек ");
                else
                    s.Append("Чек коррекции ");
                s.Append(FiscalOperationType[CalculationSign]);
                s.AppendLine("\t\tСНО: " + TaxSystem[Sno]);
                if (AppSettings.OverideRetailAddress && !string.IsNullOrEmpty(_retailAddress))
                {
                    s.AppendLine("Адрес расчетов: " + _retailAddress);
                }
                if (AppSettings.OverideRetailPlace && !string.IsNullOrEmpty(_retailPlace))
                {
                    s.AppendLine("Место расчетов: " + _retailPlace);
                }
                if (_internetPayment)
                {
                    s.AppendLine("Признак расчетов в интернет");
                }
                if (_documentName == FD_DOCUMENT_NAME_CORRECTION_CHEQUE)
                {
                    s.AppendLine("Тип коррекции:" + (_correctionTypeNotFtag == 0 || _correctionTypeNotFtag == 11311 ? "0 Самостоятельно" : "1 По предписанию"));
                    s.AppendLine("Дата расчета:" + _correctionDocDate.ToString(DEFAULT_D_FORMAT));
                    if (!string.IsNullOrEmpty(_correctionOrderNumber))
                        s.AppendLine("Номер предписания:" + _correctionOrderNumber);
                }

                if (IsPropertiesData)
                {
                    s.AppendLine("1192-доп реквизит чека: " + _propertiesData);
                }
                if (IsProperties1084)
                {
                    s.AppendLine("1084-доп реквизит пользователя{ 1085-propertiesPropertyName:" + _propertiesPropertyName + "; 1086-propertiesPropertyValue: " + _propertiesPropertyValue + "; }");
                }
                if (!string.IsNullOrEmpty(_cashier))
                    s.Append("Кассир: " + _cashier);
                if (!string.IsNullOrEmpty(_cashierInn))
                    s.Append("\tИНН кассира: " + _cashierInn);
                if (!string.IsNullOrEmpty(_cashier) || !string.IsNullOrEmpty(_cashierInn))
                    s.AppendLine();



                if (!string.IsNullOrEmpty(_emailPhone))
                    s.AppendLine("\ttel/email покупателя: " + _emailPhone);
                if (!string.IsNullOrEmpty(_buyerName))
                    s.Append("\tПокупатель: " + _buyerName);
                if (!string.IsNullOrEmpty(_buyerInn))
                    s.Append("\tИНН покупателя: " + _buyerInn);
                if (!string.IsNullOrEmpty(_buyerInformationBuyerBirthday))
                    s.Append("\tД/Р покупателя: " + _buyerInformationBuyerBirthday);
                if (!string.IsNullOrEmpty(_buyerInformationBuyerCitizenship))
                    s.Append("\tГражданство покупателя: " + _buyerInformationBuyerCitizenship);
                if (!string.IsNullOrEmpty(_buyerInformationBuyerDocumentCode))
                    s.Append("\tКод документа покупателя: " + _buyerInformationBuyerDocumentCode);
                if (!string.IsNullOrEmpty(_buyerInformationBuyerDocumentData))
                    s.Append("\tДанные документа покупателя: " + _buyerInformationBuyerDocumentData);
                if (!string.IsNullOrEmpty(_buyerInformationBuyerAddress))
                    s.Append("\tАдрес покупателя: " + _buyerInformationBuyerAddress);
                if (BuyerInformation)
                    s.AppendLine();



                s.AppendLine("предметы расчета:");
                for (int i = 0; i < Items.Count; i++)
                {
                    s.AppendLine("  [" + (i + 1) + "] " + Items[i].ToString());
                    s.AppendLine("\t\t" + ItemTypeDscr[Items[i].ProductType, 1] + "\t" + ItemPaymentTypeDscr[Items[i].PaymentType] + "\t" + TaxRateDscr[Items[i].NdsRate]);
                }
                s.AppendLine();

                s.Append("ИТОГ = ");
                s.AppendLine(_sums[FD_SUMS_TOTAL_SUM_LOC].ToString());
                int taxIncluded = 0;
                if (_sums[FD_SUMS_NDS_FREE_LOC] > 0.009)
                {
                    s.AppendLine(" Сумма расчета по ставке БЕЗ НДС: " + _sums[FD_SUMS_NDS_FREE_LOC]);
                    //taxIncluded++;
                }
                if (_sums[FD_SUMS_NDS_0_LOC] > 0.009)
                {
                    s.AppendLine(" Сумма расчета по ставке НДС 0%: " + _sums[FD_SUMS_NDS_0_LOC]);
                    //taxIncluded++;
                }
                if (_sums[FD_SUMS_NDS_20_LOC] > 0.009)
                {
                    s.AppendLine(" Сумма НДС по ставке НДС 20%: " + _sums[FD_SUMS_NDS_20_LOC]);
                    taxIncluded++;
                }
                if (_sums[FD_SUMS_NDS_10_LOC] > 0.009)
                {
                    s.AppendLine(" Сумма НДС по ставке НДС 10%: " + _sums[FD_SUMS_NDS_10_LOC]);
                    taxIncluded++;
                }
                if (_sums[FD_SUMS_NDS_20120_LOC] > 0.009)
                {
                    s.AppendLine(" Сумма НДС по ставке НДС 20/120: " + _sums[FD_SUMS_NDS_20120_LOC]);
                    taxIncluded++;
                }
                if (_sums[FD_SUMS_NDS_10110_LOC] > 0.009)
                {
                    s.AppendLine(" Сумма НДС по ставке НДС 10/110: " + _sums[FD_SUMS_NDS_10110_LOC]);
                    taxIncluded++;
                }
                if (_sums[FD_SUMS_NDS_5_LOC] > 0.009)
                {
                    s.AppendLine(" Сумма НДС по ставке НДС 5%: " + _sums[FD_SUMS_NDS_5_LOC]);
                    taxIncluded++;
                }
                if (_sums[FD_SUMS_NDS_7_LOC] > 0.009)
                {
                    s.AppendLine(" Сумма НДС по ставке НДС 7%: " + _sums[FD_SUMS_NDS_7_LOC]);
                    taxIncluded++;
                }
                if (_sums[FD_SUMS_NDS_5105_LOC] > 0.009)
                {
                    s.AppendLine(" Сумма НДС по ставке НДС 5/105: " + _sums[FD_SUMS_NDS_5105_LOC]);
                    taxIncluded++;
                }
                if (_sums[FD_SUMS_NDS_7107_LOC] > 0.009)
                {
                    s.AppendLine(" Сумма НДС по ставке НДС 7/107: " + _sums[FD_SUMS_NDS_7107_LOC]);
                    taxIncluded++;
                }
                if (taxSum > 0.0099 && taxIncluded>1)
                {
                    s.Append(" Общая сумма НДС в чеке = ");
                    s.AppendLine(taxSum.ToString());
                }
                s.AppendLine();
                s.AppendLine("Оплата чека:");
                if (_sums[FD_SUMS_PAY_CASH_LOC] > 0.0099)
                {
                    s.Append(" Наличные = ");
                    s.AppendLine(_sums[FD_SUMS_PAY_CASH_LOC].ToString());
                }
                if (_sums[FD_SUMS_PAY_ECASH_LOC] > 0.0099)
                {
                    s.Append(" Безналичные = ");
                    s.AppendLine(_sums[FD_SUMS_PAY_ECASH_LOC].ToString());
                }
                if (_sums[FD_SUMS_PAY_PREPAID_LOC] > 0.0099)
                {
                    s.Append(" Аванс = ");
                    s.AppendLine(_sums[FD_SUMS_PAY_PREPAID_LOC].ToString());
                }
                if (_sums[FD_SUMS_PAY_CREDIT_LOC] > 0.0099)
                {
                    s.Append(" Постоплата = ");
                    s.AppendLine(_sums[FD_SUMS_PAY_CREDIT_LOC].ToString());
                }
                if (_sums[FD_SUMS_PAY_PROVISION_LOC] > 0.0099)
                {
                    s.Append(" Иной тип оплаты = ");
                    s.AppendLine(_sums[FD_SUMS_PAY_PROVISION_LOC].ToString());
                }
                

            }
            return s.ToString();
        }

        public object Clone()
        {
            FiscalCheque cloned = new FiscalCheque();
            foreach(var item in _items)
            {
                cloned.Items.Add(item.Clone());
            }
            cloned.FFDVer = _docVersion;
            cloned.Sno = this._sno;
            cloned.Document = _documentName;
            cloned.CalculationSign = _calculationSign;
            if(!string.IsNullOrEmpty(_cashier)) 
                cloned.Cashier = _cashier;
            if (!string.IsNullOrEmpty(_cashierInn)) 
                cloned.CashierInn = _cashierInn;
            if (!string.IsNullOrEmpty(_emailPhone))
                cloned.EmailPhone = _emailPhone;
            if (!string.IsNullOrEmpty(_buyerName))
                cloned.BuyerInformationBuyer = _buyerName;
            if (!string.IsNullOrEmpty(_buyerInn))
                cloned.BuyerInformationBuyerInn = _buyerInn;
            if (!string.IsNullOrEmpty(_buyerInformationBuyerBirthday))
                cloned.BuyerInformationBuyerBirthday = _buyerInformationBuyerBirthday;
            if (!string.IsNullOrEmpty(_buyerInformationBuyerCitizenship))
                cloned.BuyerInformationBuyerCitizenship = _buyerInformationBuyerCitizenship;
            if (!string.IsNullOrEmpty(_buyerInformationBuyerDocumentCode))
                cloned.BuyerInformationBuyerDocumentCode = _buyerInformationBuyerDocumentCode;
            if (!string.IsNullOrEmpty(_buyerInformationBuyerDocumentData))
                cloned.BuyerInformationBuyerDocumentData = _buyerInformationBuyerDocumentData;
            if (!string.IsNullOrEmpty(_buyerInformationBuyerAddress))
                cloned.BuyerInformationBuyerAddress = _buyerInformationBuyerAddress;
            if (!string.IsNullOrEmpty(_propertiesData))
                cloned.PropertiesData = _propertiesData;
            cloned.CorrectionTypeNotFtag = _correctionTypeNotFtag;
            if (!string.IsNullOrEmpty(_correctionDocDescriber))
                cloned.CorrectionDocDescriber = _correctionDocDescriber;
            cloned.CorrectionDocumentDate = _correctionDocDate;
            if (!string.IsNullOrEmpty(_correctionOrderNumber))
                cloned.CorrectionOrderNumber = _correctionOrderNumber;
            if(!string.IsNullOrEmpty(_propertiesPropertyName))
                cloned.PropertiesPropertyName = _propertiesPropertyName;
            if(!string.IsNullOrEmpty(_propertiesPropertyValue))
                cloned.PropertiesPropertyValue = _propertiesPropertyValue;
            if(!string.IsNullOrEmpty(_retailAddress))
                cloned.RetailAddress = _retailAddress;
            if(!string.IsNullOrEmpty(_retailPlace))
                cloned.RetailPlace = _retailPlace;
            if (this._internetPayment)
            {
                cloned.InternetPayment = _internetPayment;
            }
            cloned.Cash = this.Cash;
            cloned.ECash = this.ECash;
            cloned.Prepaid = this.Prepaid;
            cloned.Credit = this.Credit;
            cloned.Provision = this.Provision;
            cloned.Nds0 = this.Nds0;
            cloned.Nds10110 = this.Nds10110;
            cloned.Nds20 = this.Nds20;
            cloned.Nds10 = this.Nds10;
            cloned.Nds20120 = this.Nds20120;
            cloned.NdsFree = this.NdsFree;
            cloned.Nds5 = this.Nds5;
            cloned.Nds7 = this.Nds7;
            cloned.Nds5105 = this.Nds5105;
            cloned.Nds7107 = this.Nds7107;
            cloned.TotalSum = this.TotalSum;

            return cloned;
        }

        public static readonly int SHORT_INFO = 1;
        public static readonly int EXTENDED_PF = 2;
        public static readonly int FULL_INFO = 3;

        List<string> strings = new List<string>();
        public List<string> ExtendedInfoForPrinting { get => strings; }
        public bool IsExtendedInfoForPrinting
        {
            get
            {
                int simbol = 0;
                foreach (string s in strings) 
                {
                    simbol += s.Length;
                }
                return simbol > 0;
            }
        }

        public List<FTag> GetFtagList(int tagNumber)
        {
            List<FTag> l = new List<FTag>();
            FTag f = null;
            // добавить тег 1005 transferOperatorAddress
            if (tagNumber == FTAG_DESTINATION_EMAIL)//1008
            {
                if (!string.IsNullOrEmpty(_emailPhone))
                {
                    try { f = new FTag(FTAG_DESTINATION_EMAIL, _emailPhone, true); } catch { }
                    if(f!=null && f.TagNumber>0)           // сделать доп. обработку при формировании тегов
                        l.Add(f);
                }
            }
            // добавить тег 1016 transferOperatorInn
            // ?? добавить тег 1020 ?? формируется при закрытии чека
            else if (tagNumber == FTAG_CASHIER_NAME)//1021
            {
                if (!string.IsNullOrEmpty(_cashier))
                {
                    try { f = new FTag(FTAG_CASHIER_NAME, _emailPhone, true); } catch { }
                    if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                        l.Add(f);
                }
            }
            // 1026 transferOperatorName
            else if (tagNumber == FTAG_CASH_TOTAL_SUM)//1031
            {
                //if (Cash > 0.009)
                //{
                try { f = new FTag(FTAG_CASH_TOTAL_SUM, (int)Math.Round(Cash*100.0), true); } catch { }
                if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                    l.Add(f);
                //}
            }
            // 1036 machineNumber
            // 1044 paymentAgentOperation
            else if (tagNumber == FTAG_APPLIED_TAXATION_TYPE)//1055
            {
                if (Sno > 0)
                {
                    try { f = new FTag(FTAG_APPLIED_TAXATION_TYPE, Sno, true); } catch { }
                    if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                        l.Add(f);
                }
            }
            // 1057 paymentAgentType
            // !!!! 1059 items обрабатывается отдельно !!!!
            // 1073 paymentAgentPhone
            // 1074 paymentOperatorPhone
            // 1075 transferOperatorPhone
            else if (tagNumber == FTAG_ECASH_TOTAL_SUM)//1081
            {
                //if (ECash > 0.009)
                //{
                    try { f = new FTag(FTAG_ECASH_TOTAL_SUM, (int)Math.Round(ECash * 100.0), true); } catch { }
                    if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                        l.Add(f);
                //}
            }
            else if (tagNumber == FTAG_PRORERTIES_1084)//1084
            {
                if (!string.IsNullOrEmpty(_propertiesPropertyName) && !string.IsNullOrEmpty(_propertiesPropertyValue))
                {
                    try 
                    {
                        FTag f1085 = new FTag(FTAG_PROPERTIES_PROPERTY_NAME, _propertiesPropertyName, true);
                        FTag f1086 = new FTag(FTAG_PROPERTIES_PROPERTY_NAME, _propertiesPropertyValue, true);
                        List<FTag> sub = new List<FTag>();
                        sub.Add(f1085);
                        sub.Add(f1086);
                        f = new FTag(FTAG_PRORERTIES_1084, sub, true);
                        l.Add(f);
                    } catch { }
                }
            }
            else if(tagNumber == FTAG_NDS20_DOCUMENT_SUM) // 1102
            {
                //if (Nds20 > 0.009)
                //{
                    try { f = new FTag(FTAG_NDS20_DOCUMENT_SUM, (int)Math.Round(Nds20 * 100.0), true); } catch { }
                    if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                        l.Add(f);
                //}
            }
            else if (tagNumber == FTAG_NDS10_DOCUMENT_SUM) // 1103
            {
                //if (Nds10 > 0.009)
                //{
                    try { f = new FTag(FTAG_NDS10_DOCUMENT_SUM, (int)Math.Round(Nds10 * 100.0), true); } catch { }
                    if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                        l.Add(f);
                //}
            }
            else if (tagNumber == FTAG_NDS0_DOCUMENT_SUM) // 1104
            {
                //if (Nds0 > 0.009 || Nds5 + Nds5105 + Nds7 + Nds7107 > 0.009)
                //{
                    try { f = new FTag(FTAG_NDS0_DOCUMENT_SUM, (int)Math.Round(Nds0 * 100.0), true); } catch { }
                    if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                        l.Add(f);
                //}
            }
            else if (tagNumber == FTAG_NDS_FREE_DOCUMENT_SUM) // 1105
            {
                //if (NdsFree > 0.009)
                //{
                    try { f = new FTag(FTAG_NDS_FREE_DOCUMENT_SUM, (int)Math.Round(NdsFree * 100.0), true); } catch { }
                    if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                        l.Add(f);
                //}
            }
            else if (tagNumber == FTAG_NDS20120_DOCUMENT_SUM) // 1106
            {
                //if (Nds20120 > 0.009)
                //{
                    try { f = new FTag(FTAG_NDS20120_DOCUMENT_SUM, (int)Math.Round(Nds20120 * 100.0), true); } catch { }
                    if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                        l.Add(f);
                //}
            }
            else if (tagNumber == FTAG_NDS10110_DOCUMENT_SUM) // 1107
            {
                //if (Nds10110 > 0.009)
                //{
                    try { f = new FTag(FTAG_NDS10110_DOCUMENT_SUM, (int)Math.Round(Nds10110 * 100.0), true); } catch { }
                    if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                        l.Add(f);
                //}
            }
            //1108 internetSign
            else if(tagNumber == FTAG_AMOUNTS_RECEIPT_NDS) // 1115
            {
                if (AppSettings.TFNFillTagNdsAmountsMethod == 0) // метод ЧК штриха добавить настройки
                {
                    FTag nds5vln = new FTag(FTAG_AMOUNTS_NDS_NDSSUM,(int)Math.Round(Nds5*100.0),true);
                    FTag nds5st = new FTag(FTAG_ITEM_NDS_RATE, NDS_TYPE_5_LOC, true);
                    List<FTag> nest5 = new List<FTag>();
                    nest5.Add(nds5vln);
                    nest5.Add(nds5st);
                    FTag amountNds5 = new FTag(FTAG_AMOUNTS_NDS, nest5, true);

                    FTag nds7vln = new FTag(FTAG_AMOUNTS_NDS_NDSSUM, (int)Math.Round(Nds7 * 100.0), true);
                    FTag nds7st = new FTag(FTAG_ITEM_NDS_RATE, NDS_TYPE_7_LOC, true);
                    List<FTag> nest7 = new List<FTag>();
                    nest7.Add(nds7vln);
                    nest7.Add(nds7st);
                    FTag amountNds7 = new FTag(FTAG_AMOUNTS_NDS, nest7, true);

                    FTag nds5105vln = new FTag(FTAG_AMOUNTS_NDS_NDSSUM, (int)Math.Round(Nds5105 * 100.0), true);
                    FTag nds5105st = new FTag(FTAG_ITEM_NDS_RATE, NDS_TYPE_5105_LOC, true);
                    List<FTag> nest5105 = new List<FTag>();
                    nest5105.Add(nds5105vln);
                    nest5105.Add(nds5105st);
                    FTag amountNds5105 = new FTag(FTAG_AMOUNTS_NDS, nest5105, true);

                    FTag nds7107vln = new FTag(FTAG_AMOUNTS_NDS_NDSSUM, (int)Math.Round(Nds7107 * 100.0), true);
                    FTag nds7107st = new FTag(FTAG_ITEM_NDS_RATE, NDS_TYPE_7107_LOC, true);
                    List<FTag> nest7107 = new List<FTag>();
                    nest7107.Add(nds7107vln);
                    nest7107.Add(nds7107st);
                    FTag amountNds7107 = new FTag(FTAG_AMOUNTS_NDS, nest7107, true);

                    List<FTag> amountsNdsSums = new List<FTag>();
                    amountsNdsSums.Add(amountNds5);
                    amountsNdsSums.Add(amountNds7);
                    amountsNdsSums.Add(amountNds5105);
                    amountsNdsSums.Add(amountNds7107);
                    FTag famountsNdsSums = new FTag(FTAG_AMOUNTS_RECEIPT_NDS, amountsNdsSums, true);
                    l = new List<FTag>();
                    l.Add(famountsNdsSums);
                }
                else   // метод атола
                {
                    
                    FTag famountsNdsSums = new FTag(FTAG_AMOUNTS_RECEIPT_NDS, new List<FTag>(), true);
                    if (Nds5 > 0.009)
                    {
                        FTag nds5vln = new FTag(FTAG_AMOUNTS_NDS_NDSSUM, (int)Math.Round(Nds5 * 100.0), true);
                        FTag nds5st = new FTag(FTAG_ITEM_NDS_RATE, NDS_TYPE_5_LOC, true);
                        List<FTag> nest5 = new List<FTag>();
                        nest5.Add(nds5vln);
                        nest5.Add(nds5st);
                        FTag amountNds5 = new FTag(FTAG_AMOUNTS_NDS, nest5, true);
                        famountsNdsSums.Nested.Add(amountNds5);
                    }

                    if (Nds7 > 0.009)
                    {
                        FTag nds7vln = new FTag(FTAG_AMOUNTS_NDS_NDSSUM, (int)Math.Round(Nds7 * 100.0), true);
                        FTag nds7st = new FTag(FTAG_ITEM_NDS_RATE, NDS_TYPE_7_LOC, true);
                        List<FTag> nest7 = new List<FTag>();
                        nest7.Add(nds7vln);
                        nest7.Add(nds7st);
                        FTag amountNds7 = new FTag(FTAG_AMOUNTS_NDS, nest7, true);
                        famountsNdsSums.Nested.Add(amountNds7);
                    }

                    if (Nds5105 > 0.009)
                    {
                        FTag nds5105vln = new FTag(FTAG_AMOUNTS_NDS_NDSSUM, (int)Math.Round(Nds5105 * 100.0), true);
                        FTag nds5105st = new FTag(FTAG_ITEM_NDS_RATE, NDS_TYPE_5105_LOC, true);
                        List<FTag> nest5105 = new List<FTag>();
                        nest5105.Add(nds5105vln);
                        nest5105.Add(nds5105st);
                        FTag amountNds5105 = new FTag(FTAG_AMOUNTS_NDS, nest5105, true);
                        famountsNdsSums.Nested.Add(amountNds5105);
                    }

                    if (Nds7107 > 0.009)
                    {
                        FTag nds7107vln = new FTag(FTAG_AMOUNTS_NDS_NDSSUM, (int)Math.Round(Nds7107 * 100.0), true);
                        FTag nds7107st = new FTag(FTAG_ITEM_NDS_RATE, NDS_TYPE_7107_LOC, true);
                        List<FTag> nest7107 = new List<FTag>();
                        nest7107.Add(nds7107vln);
                        nest7107.Add(nds7107st);
                        FTag amountNds7107 = new FTag(FTAG_AMOUNTS_NDS, nest7107, true);
                        famountsNdsSums.Nested.Add(amountNds7107);
                    }
                    if (famountsNdsSums.Nested.Count > 0)
                    {
                        famountsNdsSums.RawDataConstructor();
                        l = new List<FTag>();
                        l.Add(famountsNdsSums);
                    }
                }
            }
            // 1117 sellerAddress
            // 1125 internetPayment
            else if (tagNumber == FTAG_INTERNET_PAYMENT)
            {
                if (_internetPayment)
                {
                    try { f = new FTag(FTAG_INTERNET_PAYMENT, 1, true); } catch { }
                    if (f != null)
                    {
                        l.Add(f);
                    }
                }
            }
            // 1171 providerPhone
            else if (tagNumber == FTAG_CORRECTION_TYPE)
            {

                try { f = new FTag(FTAG_CORRECTION_TYPE, CorrectionTypeFtag, true); } catch { }
                if (f != null)
                {
                    l.Add(f);
                }
            }
            else if (tagNumber == FTAG_CORRECTION_BASE)
            {
                try
                {
                    FTag fcorrdocdate = new FTag(FTAG_CORRECTION_DOC_DATE, CorrectionDocumentDate, true);

                    FTag fcorrOrdNum = null;

                    if (!string.IsNullOrEmpty(_correctionOrderNumber))
                    {
                        if (AppSettings.CorrectionOrderExistance == 1 && _correctionTypeFtag == 1
                            || AppSettings.CorrectionOrderExistance == 0
                            )
                        {
                            fcorrOrdNum = new FTag(FTAG_CORRECTION_ORDER_NUMBER, _correctionOrderNumber, true);
                        }

                    }
                    List<FTag> cl = new List<FTag>();
                    cl.Add(fcorrdocdate);
                    if (fcorrOrdNum != null && fcorrOrdNum.TagNumber > 0)
                    {
                        cl.Add(fcorrOrdNum);
                    }
                    FTag corrBase = new FTag(FTAG_CORRECTION_BASE, cl, true);
                    l.Add(corrBase);
                }
                catch { }
            }
            else if (tagNumber == FTAG_PROPERTIES_DATA) //1192
            {
                if (IsPropertiesData)
                {
                    try
                    {
                        f = new FTag(FTAG_PROPERTIES_DATA, _propertiesData, true);
                        l = new List<FTag>();
                        l.Add(f);
                    } catch { }
                }
            }
            else if (tagNumber == FTAG_CASHIER_INN) //1203
            {
                if (!string.IsNullOrEmpty(_cashierInn))
                {
                    try { f = new FTag(FTAG_CASHIER_INN, _cashierInn, true); } catch { }
                    if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                        l.Add(f);
                }
            }
            else if (tagNumber == FTAG_PREPAID_TOTAL_SUM) //1215
            {
                //if (Prepaid > 0.009)
                //{
                try { f = new FTag(FTAG_PREPAID_TOTAL_SUM, (int)Math.Round(Prepaid * 100.0), true); } catch { }
                if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                    l.Add(f);
                //}
            }
            else if (tagNumber == FTAG_CREDIT_TOTAL_SUM) //1216
            {
                //if (Credit > 0.009)
                //{
                try { f = new FTag(FTAG_CREDIT_TOTAL_SUM, (int)Math.Round(Credit * 100.0), true); } catch { }
                if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                    l.Add(f);
                //}
            }
            else if (tagNumber == FTAG_PROVISION_TOTAL_SUM) //1217
            {
                //if (Provision > 0.009)
                //{
                try { f = new FTag(FTAG_PROVISION_TOTAL_SUM, (int)Math.Round(Provision * 100.0), true); } catch { }
                if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                    l.Add(f);
                //}
            }
            else if (tagNumber == FTAG_BUYER_INFORMATION_BUYER)//1227
            {
                if (!string.IsNullOrEmpty(_buyerName))
                {
                    try { f = new FTag(FTAG_BUYER_INFORMATION_BUYER, _buyerName, true); } catch { }
                    if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                        l.Add(f);
                }
            }
            else if (tagNumber == FTAG_BUYER_INFORMATION_BUYER_INN)//1228
            {
                if (!string.IsNullOrEmpty(_buyerInn))
                {
                    try { f = new FTag(FTAG_BUYER_INFORMATION_BUYER_INN, _buyerInn, true); } catch { }
                    if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                        l.Add(f);
                }
            }
            // 1234 allECashProp
            else if (tagNumber == FTAG_BUYER_INFORMATION) // 1256
            {
                if (BuyerInformation)
                {
                    List<FTag> nested = new List<FTag>();
                    if (!string.IsNullOrEmpty(_buyerName))
                    {
                        try { nested.Add(new FTag(FTAG_BUYER_INFORMATION_BUYER, _buyerName, true)); } catch { }
                    }
                    if (!string.IsNullOrEmpty(_buyerInn))
                    {
                        try { nested.Add(new FTag(FTAG_BUYER_INFORMATION_BUYER_INN, _buyerInn, true)); } catch { }
                    }
                    if (!string.IsNullOrEmpty(_buyerInformationBuyerBirthday))
                    {
                        try { nested.Add(new FTag(FTAG_BI_BIRTHDAY, _buyerInformationBuyerBirthday, true)); } catch { }
                    }
                    if (!string.IsNullOrEmpty(_buyerInformationBuyerCitizenship))
                    {
                        try { nested.Add(new FTag(FTAG_BI_CITIZENSHIP, _buyerInformationBuyerCitizenship, true)); } catch { }
                    }
                    if (!string.IsNullOrEmpty(_buyerInformationBuyerDocumentCode))
                    {
                        try { nested.Add(new FTag(FTAG_BI_DOCUMENT_CODE, _buyerInformationBuyerDocumentCode, true)); } catch { }
                    }
                    if (!string.IsNullOrEmpty(_buyerInformationBuyerDocumentData))
                    {
                        try { nested.Add(new FTag(FTAG_BI_DOCUMENT_DATA, _buyerInformationBuyerDocumentData, true)); } catch { }
                    }
                    if (!string.IsNullOrEmpty(_buyerInformationBuyerAddress))
                    {
                        try { nested.Add(new FTag(FTAG_BI_ADDRESS, _buyerInformationBuyerAddress, true)); } catch { }
                    }
                    try { f = new FTag(FTAG_BUYER_INFORMATION, nested, true); } catch { }
                    if (f != null && f.TagNumber > 0)           // сделать доп. обработку при формировании тегов
                        l.Add(f);
                }
            }
            else if (tagNumber == FTAG_RETAIL_PLACE_ADRRESS)
            {
                if (!string.IsNullOrEmpty(_retailAddress))
                {
                    try { f = new FTag(FTAG_RETAIL_PLACE_ADRRESS, _retailAddress, true); } catch { }
                    if (f != null && f.TagNumber > 0)           
                        l.Add(f);
                }
            }
            else if (tagNumber == FTAG_RETAIL_PLACE)
            {
                if (!string.IsNullOrEmpty(_retailPlace))
                {
                    try { f = new FTag(FTAG_RETAIL_PLACE, _retailPlace, true); } catch { }
                }
                if (f != null && f.TagNumber > 0)           
                    l.Add(f);
            }
            // 1261 industryReceiptDetails
            // 1270 operationalDetails
            // 2107 checkingLabeledProdResult

            if (l != null && l.Count > 0)
            {
                return l;
            }

            return null;
        }

        public List<FTag> CreateTask()
        {
            List<FTag> fTags = new List<FTag>();
            int ruleSetKey = 4 * 65536 + DocumentNameFtagType;
            if (FTag.TFNCommonRules.ContainsKey(ruleSetKey))
            {
                var ruleSet = FTag.TFNCommonRules[ruleSetKey];
                foreach( var rule in ruleSet.Rules)
                {
                    if(rule.DataSource == FTag.TFTagRuleSet.RSOURCE_INCLASS || rule.DataSource == FTag.TFTagRuleSet.RSOURCE_REG_PARAM)
                    {
                        if(rule.TagNumber == FTAG_ITEM)
                        {
                            foreach(var item in Items)
                            {
                                fTags.AddRange(item.GetItemFtag(4));
                            }
                        }
                        else
                        {
                            var l = GetFtagList(rule.TagNumber);
                            if (l != null && l.Count > 0)
                            { fTags.AddRange(l); }
                            else
                            {
                                if (!string.IsNullOrEmpty(rule.DefaultData))
                                {
                                    try
                                    {
                                        FTag defFtag = new FTag(rule.TagNumber, rule.DefaultData, false);
                                        if (defFtag.TagNumber == rule.TagNumber)
                                        {
                                            fTags.Add(defFtag);
                                        }
                                    }
                                    catch {  }
                                }
                            }
                        }
                        
                    }
                    else if(rule.DataSource == FTag.TFTagRuleSet.RSOURCE_OVERRIDE)
                    {
                        try
                        {
                            FTag defFtag = new FTag(rule.TagNumber, rule.DefaultData, false);
                            if (defFtag.TagNumber == rule.TagNumber)
                            {
                                fTags.Add(defFtag);
                            }
                        }
                        catch { }
                    }
                }
            }
            return fTags;
        }
    }
}
