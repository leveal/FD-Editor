using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FR_Operator.FiscalPrinter;

namespace FR_Operator
{
    public class ConsumptionItem           //1059 предмет расчета STLV 
    {

        public ConsumptionItem(string name, double price, double quantity, double sum,
            int productType = FD_ITEM_TYPE_PRODUCT_LOC, int paymentType = FD_ITEM_PAYMENT_TOTAL_CALC_LOC, int ndsRate = NDS_TYPE_FREE_LOC)
        {
            if (name == null)
                name = "";
            _name = name.Length<=128 ? name : name.Substring(0,128);
            _price = price;
            _quantity = quantity;
            _sum = sum;
            _productType = productType;
            _paymentType = paymentType;
            _ndsRate = ndsRate;
            Control();
        }
        public ConsumptionItem()
        {
            _name = " ";
        }
        public static ConsumptionItem SAMPLE { get { return new ConsumptionItem("Круассан с малиновым джемом", 1.11, 1, 1.11); } }

        string _name = "";
        // 1030 наименование предметарасчета string [1,128]
        public string Name
        {
            get { return _name; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _name = value;
                
                while (_name.IndexOf('\n') >= 0)
                {
                    _name.Remove(_name.IndexOf('\n'));
                }
                while (_name.IndexOf('\r') >= 0)
                {
                    _name.Remove(_name.IndexOf('\r'));
                }
                if(_name.Length>128)
                    _name = _name.Substring(0,128);
            }
        }

        int _paymentType = 0;
        // 1214 признак способа расчета BYTE {1,2,3,4,5,6,7} признак  способа расчета
        public int PaymentType
        {
            get { return _paymentType; }
            set { if (value >= 0 && value <= 7) _paymentType = value; }
        }

        int _productType = 0;
        // 1212 признак предмета расчета BYTE [1,33]
        public int ProductType
        {
            get { return _productType; }
            set
            {
                if (value >= 0 && value <= 33) _productType = value;
            }
        }

        double _quantity = 1;
        // 1023 количество предмета расчета FVLN (0,18446744073709551615]
        public double Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                Control();
            }
        }

        double _sum = 0;
        /*  1043 VLN  [0,281474976710655]   
         *  стоимость предмета расчета с    
         *  учетом скидок и наценок         */
        public double Sum
        {
            get { return _sum; }
            set
            {
                _sum = value;
                Control();
            }
        }

        double _price = 0;
        /*  1079 VLN    [0,281474976710655] 
         *  цена за единицу предмета расчета с учетом  */
        public double Price
        {
            get { return _price; }
            set
            {
                if (value >= 0)
                    _price = value;
                Control();
            }
        }

        int _ndsRate = 0;
        /* 1199 BYTE {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}
         *  ставка НДС */
        public int NdsRate
        {
            get { return _ndsRate; }
            set
            {
                if (value >= 0 && value <= 10)
                    _ndsRate = value;
                Control();
            }
        }

        double _ndsSum = 0;
        /*  1200 [0,281474976710655] 
         *  сумма НДС за предмет расчета */
        public double UnitNnds
        {
            get 
            { 
                return _ndsSum; 
            }
            set { _ndsSum = value; }
        }

        // 1197 единица ижмерения 1.05
        string _unit105 = null;
        public string Unit105
        {
            get { return _unit105 == null ? "" : _unit105; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _unit105 = value;
            }
        }
        // 2108 единица измерения 1.2
        int _unit120 = -1;
        public int Unit120 
        { 
            get { return _unit120; } 
            set 
            { 
                if(value>=0&&(
                    value == FD_ITEM_MEASURE_UNIT_LOC ||
                    value == FD_ITEM_MEASURE_GRAM_LOC ||
                    value == FD_ITEM_MEASURE_KG_LOC ||
                    value == FD_ITEM_MEASURE_TON_LOC ||
                    value == FD_ITEM_MEASURE_SM_LOC ||
                    value == FD_ITEM_MEASURE_DM_LOC ||
                    value == FD_ITEM_MEASURE_METR_LOC ||
                    value == FD_ITEM_MEASURE_QSM_LOC ||
                    value == FD_ITEM_MEASURE_QDM_LOC ||
                    value == FD_ITEM_MEASURE_QMETR_LOC ||
                    value == FD_ITEM_MEASURE_ML_LOC ||
                    value == FD_ITEM_MEASURE_LITR_LOC ||
                    value == FD_ITEM_MEASURE_CUBEM_LOC ||
                    value == FD_ITEM_MEASURE_KWH_LOC ||
                    value == FD_ITEM_MEASURE_GKL_LOC ||
                    value == FD_ITEM_MEASURE_DAY_LOC ||
                    value == FD_ITEM_MEASURE_HOUR_LOC ||
                    value == FD_ITEM_MEASURE_MIN_LOC ||
                    value == FD_ITEM_MEASURE_SEC_LOC ||
                    value == FD_ITEM_MEASURE_KBYTE_LOC ||
                    value == FD_ITEM_MEASURE_MBYTE_LOC ||
                    value == FD_ITEM_MEASURE_GBYTE_LOC ||
                    value == FD_ITEM_MEASURE_TBYTE_LOC ||
                    value == FD_ITEM_MEASURE_OTHER_LOC
                    ))
                    _unit120 = value; 
            }
        }

        // 1162 Код товара 1.05
        string _code = "";
        public string Code105
        {
            get { return _code == null ? "" : _code; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _code = value.Length > 32 ? value.Substring(0, 32) : value;
            }
        }


        /*
         *  Агенские теги:
         *  1222	paymentAgentByProductType	признак агента по предмету расчета		            BitMask{1,2,4,8,16,32,64} байт
         *  1223	paymentAgentData 	данные агента	                            	            STLV
         *  +--1075	paymentAgentData.transferOperatorPhone	телефон оператора перевода	            СТРОКА ^\\+[0-9]{9,15}$ [0,19] *
         *  +--1044	paymentAgentData.paymentAgentOperation	операция платежного агента	            строка [0-24]
         *  +--1073	paymentAgentData.paymentAgentPhone	телефон платежного агента	            	СТРОКА ^\\+[0-9]{9,15}$ [0,19] *
         *  +--1074	paymentAgentData.paymentOperatorPhone	телефон оператора по приему платежей	СТРОКА ^\\+[0-9]{9,15}$ [0,19] *
         *  +--1026	paymentAgentData.transferOperatorName	наименование оператора перевода	    	строка [0-64]
         *  +--1005	paymentAgentData.transferOperatorAddress	адрес оператора перевода	    	строка [0-256]
         *  +--1016	paymentAgentData.transferOperatorInn	ИНН оператора перевода	    	 	    СТРОКА 10,12 цифр или 10 цифр + 2 пробела
         */
        // 1222 Признак агента по предмету расчета
        int _paymentAgentByProductType = 0;
        public int PaymentAgentByProductType
        {
            get { return _paymentAgentByProductType; }
            set { if (value>=0)_paymentAgentByProductType = value; }
        }
        // 1075 телефон оператора перевода
        string _transferOperatorPhone = string.Empty;
        public string TransferOperatorPhone
        {
            get => _transferOperatorPhone;
            set
            {
                if(value == null)
                {
                    _transferOperatorPhone = string.Empty;
                }
                else if (value.Length > 19)
                {
                    _transferOperatorPhone = value.Substring(0, 19);
                }
                else
                {
                    _transferOperatorPhone = value;
                }
            }
        }
        // 1044 операция платежного агента
        string _paymentAgentOperation = string.Empty;
        public string PaymentAgentOperation
        {
            get => _paymentAgentOperation;
            set
            {
                if(value == null)
                {
                    _paymentAgentOperation = string.Empty;
                }
                else if(value.Length> 24)
                {
                    _paymentAgentOperation = value.Substring(0, 24);
                }
                else
                {
                    _paymentAgentOperation = value;
                }
            }
        }
        // 1073 телефон платежного агента
        string _paymentAgentPhone = string.Empty;
        public string PaymentAgentPhone
        {
            get => _paymentAgentPhone;
            set
            {
                if(value == null)
                {
                    _paymentAgentPhone = string.Empty;
                }
                else if (value.Length > 19)
                {
                    _paymentAgentPhone = value.Substring(0,19);
                }
                else
                {
                    _paymentAgentPhone = value;
                }
            }
        }
        // 1074 телефон оператора по приему платежей
        string _paymentOperatorPhone = string.Empty;
        public string PaymentOperatorPhone
        {
            get => _paymentOperatorPhone;
            set
            {
                if(value == null)
                {
                    _paymentOperatorPhone = string.Empty;
                }
                else if (value.Length > 19)
                {
                    _paymentOperatorPhone = value.Substring(0,19);
                }
                else
                {
                    _paymentOperatorPhone = value;
                }
            }
        }
        // 1026 наименование оператора перевода
        string _transferOperatorName = string.Empty;
        public string TransferOperatorName
        {
            get => _transferOperatorName;
            set
            {
                if (value == null)
                {
                    _transferOperatorName = string.Empty;
                }
                else if (value.Length > 64)
                {
                    _transferOperatorName = value.Substring(0, 64);
                }
                else
                {
                    _transferOperatorName = value;
                }
            }
        }
        // 1005 адрес оператора перевода
        string _transferOperatorAddress = string.Empty;
        public string TransferOperatorAddress
        {
            get => _transferOperatorAddress;
            set
            {
                if (value == null)
                {
                    _transferOperatorAddress = string.Empty;
                }
                else if (value.Length > 256)
                {
                    _transferOperatorAddress = value.Substring(0, 256);
                }
                else
                {
                    _transferOperatorAddress = value;
                }
            }
        }
        // 1016 ИНН оператора перевода
        string _transferOperatorInn = string.Empty;
        public string TransferOperatorInn
        {
            get => _transferOperatorInn;
            set 
            {
                if (CorrectInn(value)) { _transferOperatorInn = value; }
                else _transferOperatorInn = string.Empty;
            }
        }
        // 1223 данные агента
        public bool IsPaymentAgentData
        {
            get => 
                (!string.IsNullOrEmpty(_transferOperatorPhone)) ||
                (!string.IsNullOrEmpty(_paymentAgentOperation)) ||
                (!string.IsNullOrEmpty(_paymentAgentPhone)) ||
                (!string.IsNullOrEmpty(_paymentOperatorPhone)) ||
                (!string.IsNullOrEmpty(_transferOperatorName)) ||
                (!string.IsNullOrEmpty(_transferOperatorAddress)) ||
                (!string.IsNullOrEmpty(_transferOperatorInn));
        }
        public FTag PaymentAgentData
        {
            get
            {
                List<FTag> pad = new List<FTag>();
                if (!string.IsNullOrEmpty(_transferOperatorPhone))
                {
                    pad.Add(new FTag(FTAG_PAD_TRANSFER_OPERATOR_PHONE, _transferOperatorPhone, true));
                }
                if (!string.IsNullOrEmpty(_paymentAgentOperation))
                {
                    pad.Add(new FTag(FTAG_PAD_PAYPENT_AGENT_OPERATION, _paymentAgentOperation, true));
                }
                if (!string.IsNullOrEmpty(_paymentAgentPhone))
                {
                    pad.Add(new FTag(FTAG_PAD_PAYMENT_AGENT_PHONE, _paymentAgentPhone, true));
                }
                if (!string.IsNullOrEmpty(_paymentOperatorPhone))
                {
                    pad.Add(new FTag(FTAG_PAD_PAYMENT_OPERATOR_PHONE, _paymentOperatorPhone, true));
                }
                if (!string.IsNullOrEmpty(_transferOperatorName))
                {
                    pad.Add(new FTag(FTAG_PAD_TRANSFER_OPERATOR_NAME, _transferOperatorName, true));
                }
                if (!string.IsNullOrEmpty(_transferOperatorAddress))
                {
                    pad.Add(new FTag(FTAG_PAD_TRANSFER_OPERATOR_ADDRESS, _transferOperatorAddress, true));
                }
                if (!string.IsNullOrEmpty(_transferOperatorInn))
                {
                    pad.Add(new FTag(FTAG_PAD_TRANSFER_OPERATOR_INN, _transferOperatorInn, true));
                }
                FTag padFtag = new FTag(FTAG_ITEM_PAYMENT_AGENT_DATA, pad, true);
                return padFtag;
            }
        }


        /*
         * 1226	providerInn	 ИНН поставщика	+	+	+	10Ц+2" " или 12Ц
         * 1224	providerData	данные поставщика	+	+	+	STLV
         * 1171	providerData.providerPhone	телефон поставщика	+	+	+	СТРОКА ^\\+[0-9]{9,15}$ [0,19] *
         * 1225	providerData.providerName	наименование поставщика	+	+	+	строка [0-256]
         */

        // 1226 ИНН поставщика
        string _providerInn = "";
        public string ProviderInn
        {
            get { return _providerInn; }
            set { if(CorrectInn(value))_providerInn = value; }
        }
        // 1171 телефон поставщика
        string _providerPhone = string.Empty;
        public string ProviderPhone
        {
            get => _providerPhone;
            set
            {
                if(value == null)
                {
                    _providerPhone = string.Empty;
                }
                else if (value.Length > 19)
                {
                    _providerPhone = value.Substring(0, 19);
                }
                else
                {
                    _providerPhone = value;
                }
            }
        }
        // 1225 наименование поставщика
        string _providerName = string.Empty;
        public string ProviderName
        {
            get => _providerName;
            set
            {
                if (value == null)
                {
                    _providerName = string.Empty;
                }
                else if (value.Length > 19)
                {
                    _providerName = value.Substring(0, 19);
                }
                else
                {
                    _providerName = value;
                }
            }
        }
        // 1224 данные поставщика
        public bool IsProviderData
        {
            get => (!string.IsNullOrEmpty(_providerPhone)) ||
                (!string.IsNullOrEmpty(_providerName));
        }
        public FTag ProviderData
        {
            get
            {
                List<FTag> pd = new List<FTag>();
                if (!string.IsNullOrEmpty(_providerPhone))
                {
                    pd.Add(new FTag(FTAG_PD_PROVIDER_PHONE, _providerPhone, true));
                }
                if (!string.IsNullOrEmpty(_providerName))
                {
                    pd.Add(new FTag(FTAG_PD_PROVIDER_NAME, _providerName, true));
                }
                FTag provData = new FTag(FTAG_ITEM_PROVIDER_DATA, pd, true);
                return provData;
            }
        }

        // 1231 Номер таможенной декларации 
        string _customEntryNum = "";
        public string CustomEntryNum
        {
            get { return _customEntryNum; }
            set { if(value!=null)_customEntryNum = value; }
        }

        //1230 Код страны происхождения товара
        string _originalCountryCode = "";
        public string OriginalCountryCode
        {
            get { return _originalCountryCode; }
            set { if(value!=null)_originalCountryCode = value; }
        }

        int _checkRezult = FD_ITEM_CONTROL_CRITICAL_ERROR;
        int _errorFlags = FD_ITEM_CONTROL_ZERO_QUANTITY_BF;
        public int Correctness
        { get { return _checkRezult; } }

        public void Control()
        {
            _checkRezult = FD_ITEM_CONTROL_OK;
            _errorFlags = FD_ITEM_CONTROL_OK_BF;

            if (_sum == 0 && _quantity > 1E-6 && _price > 1E-6) _sum = _quantity * _price;
            if (_quantity == 0)
            {
                _checkRezult = FD_ITEM_CONTROL_CRITICAL_ERROR;
                _errorFlags = FD_ITEM_CONTROL_ZERO_QUANTITY_BF;
            }
            else if (_quantity < 0 || _quantity > 1.8446744073709551615E19)
            {
                _checkRezult = FD_ITEM_CONTROL_CRITICAL_ERROR;
                _errorFlags = FD_ITEM_CONTROL_BAD_QUANTITY_BF;
            }

            if (_price < 0 || _price > 2.81474976710655E14)
            {
                _checkRezult = FD_ITEM_CONTROL_CRITICAL_ERROR;
                _errorFlags = FD_ITEM_CONTROL_BAD_PRICE_BF | _errorFlags;
            }

            if (_sum < 0 || _sum > 2.81474976710655E14 || Math.Abs(Math.Round(_price * _quantity, 2) - Math.Round(_sum, 2)) > 0.01)
            {
                _checkRezult = FD_ITEM_CONTROL_CRITICAL_ERROR;
                _errorFlags = _errorFlags | FD_ITEM_CONTROL_BAD_SUM_BF;
            }


            switch (_ndsRate)
            {
                case NDS_TYPE_EMPTY_LOC:
                    _ndsSum = 0;
                    break;
                case NDS_TYPE_FREE_LOC:
                case NDS_TYPE_0_LOC:
                    _ndsSum = _sum;
                    break;
                case NDS_TYPE_20120_LOC:
                case NDS_TYPE_20_LOC:
                    _ndsSum = Math.Round(_sum / 6, 2);
                    break;
                case NDS_TYPE_10110_LOC:
                case NDS_TYPE_10_LOC:
                    _ndsSum = Math.Round(_sum / 11, 2);
                    break;
                case NDS_TYPE_5_LOC:
                case NDS_TYPE_5105_LOC:
                    _ndsSum = Math.Round(_sum / 21, 2);
                    break;
                case NDS_TYPE_7_LOC:
                case NDS_TYPE_7107_LOC:
                    _ndsSum = Math.Round(_sum*7.0 / 107, 2);
                    break;
            }

            //добавить проверки на коды товаров егаисы отрасли итп
            //_checkRezult = FD_ITEM_CONTROL_OK;
        }
        
        public string ToString(string format = null)
        {
            if (format != null && format.StartsWith("ushort"))
            {
                int n = 6;
                int.TryParse(format.Substring(6),out n);
                return Sum.ToString("F2") + " " + (Name.Length > n ? Name.Substring(0, n) + ".." : Name);
            }
            return string.Format("\'{0} \' {1}x{2}={3}", _name, _quantity, _price, _sum);
        }

        public ConsumptionItem Clone()
        {
            var cloned = new ConsumptionItem(_name, _price, _quantity, _sum, _productType, _paymentType, _ndsRate);
            if (!string.IsNullOrEmpty(_unit105))cloned._unit105 = _unit105;
            if(_unit120>=0)cloned._unit120 = _unit120;
            if(!string.IsNullOrEmpty(_code))cloned.Code105 = _code;
            cloned.PaymentAgentByProductType = _paymentAgentByProductType;
            if (!string.IsNullOrEmpty(_transferOperatorPhone))
            {
                cloned.TransferOperatorPhone = _transferOperatorPhone;
            }
            if (!string.IsNullOrEmpty(_paymentAgentOperation))
            {
                cloned.PaymentAgentOperation = _paymentAgentOperation;
            }
            if (!string.IsNullOrEmpty(_paymentAgentPhone))
            {
                cloned.PaymentAgentPhone = _paymentAgentPhone;
            }
            if (!string.IsNullOrEmpty(_paymentOperatorPhone))
            {
                cloned.PaymentOperatorPhone = _paymentOperatorPhone;
            }
            if (!string.IsNullOrEmpty(_transferOperatorName))
            {
                cloned.TransferOperatorName = _transferOperatorName;
            }
            if (!string.IsNullOrEmpty(_transferOperatorAddress))
            {
                cloned.TransferOperatorAddress = _transferOperatorAddress;
            }
            if (!string.IsNullOrEmpty(_transferOperatorInn))
            {
                cloned.TransferOperatorInn = _transferOperatorInn;
            }
            
            if (!string.IsNullOrEmpty(_providerInn))
            {
                cloned.ProviderInn = _providerInn;
            }
            if (!string.IsNullOrEmpty(_providerPhone))
            {
                cloned.ProviderPhone = _providerPhone;
            }
            if (!string.IsNullOrEmpty(_providerName))
            {
                cloned.ProviderName = _providerName;
            }
            if (!string.IsNullOrEmpty(_customEntryNum))
            {
                cloned.CustomEntryNum = _customEntryNum;
            }
            if (!string.IsNullOrEmpty(_originalCountryCode))
            {
                cloned.OriginalCountryCode = _originalCountryCode;
            }
            
            return cloned;
        }

        public List<FTag> GetItemFtag(int ffd)
        {
            List<FTag> nested = new List<FTag>();
            FTag fquant = new FTag(FTAG_ITEM_QUANTITY, _quantity, true);
            nested.Add(fquant);
            FTag fsum = new FTag(FTAG_ITEM_SUM, (long)Math.Round(_sum*100), true);
            nested.Add(fsum);
            FTag fprice = new FTag(FTAG_ITEM_PRICE, (int)Math.Round(_price*100.0), true);       // возможно тут нужен контроль расхождения сумм
            nested.Add(fprice);
            FTag fpt = new FTag(FTAG_ITEM_PAYMENT_TYPE, _paymentType, true);
            nested.Add(fpt);
            if(fpt.ValueInt != 3)
            {
                FTag finame = new FTag(FTAG_ITEM_NAME, _name, true);
                nested.Add(finame);
            }
            //unit
            if(ffd == 4||ffd == 120)
            {
                FTag fum120;
                if (_unit120 > 0)
                {
                    fum120 = new FTag(FTAG_ITEM_UNIT_MEASURE_120, _unit120, true);
                }
                else
                {
                    fum120 = new FTag(FTAG_ITEM_UNIT_MEASURE_120, 0, true);
                }
                nested.Add(fum120);
            }
            else
            {
                if (!string.IsNullOrEmpty(_unit105))
                {
                    FTag fum105 = new FTag(FTAG_ITEM_UNIT_MEASURE_105, _unit105, true);
                    nested.Add(fum105);
                }
            }
            FTag finds = new FTag(FTAG_ITEM_NDS_RATE,_ndsRate,true);
            nested.Add(finds);
            FTag fiprt = new FTag(FTAG_ITEM_PRODUCT_TYPE, _productType, true);
            nested.Add(fiprt);
            // данные агента
            if ( _paymentAgentByProductType > 0) // доделать формирование BitMask
            {
                FTag iagentbyprod = new FTag(FTAG_ITEM_PAYMENT_AGENT_BY_PRODUCT_TYPE, _paymentAgentByProductType, true);
                nested.Add(iagentbyprod);
            }
            if (IsPaymentAgentData)
            {
                nested.Add(PaymentAgentData);
            }

            if (!string.IsNullOrEmpty(_providerInn))
            {
                FTag fprovInn = new FTag(FTAG_ITEM_PROVIDER_INN, _providerInn, true);
                nested.Add(fprovInn);
            }
            if (IsProviderData)
            {
                nested.Add(ProviderData);
            }


            if (!string.IsNullOrEmpty(_customEntryNum))
            {
                FTag fcustomEntry = new FTag(FTAG_ITEM_CUSTOM_ENTRY_NUM, _customEntryNum,true);
                nested.Add(fcustomEntry);
            }
            if (!string.IsNullOrEmpty(_originalCountryCode))
            {
                FTag foricountry = new FTag(FTAG_ITEM_ORIGINAL_COUNTRY_CODE, _originalCountryCode,true);
                nested.Add(foricountry);
            }

            FTag item = new FTag(FTAG_ITEM, nested, true);
            List<FTag> items = new List<FTag>();
            items.Add(item);
            return items;
        } 
    }
}
