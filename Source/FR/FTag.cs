using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;


namespace FR_Operator
{
    public class FTag
    {
        /*
         * создаетм теги из массива данных считанных из ФН
         */
        public FTag(int tagNumber, byte[] data)
        {
            _tagNumber = tagNumber;
            _rawData = data;
            if (typeMap.ContainsKey(tagNumber)) _type = typeMap[tagNumber];
            else _type = FDataType.UNKNOWN;
            if (_type != FDataType.UNKNOWN)
            {
                //if (_tagNumber == 1077)
                //    LogHandle.ol("tag 1077");
                switch (_type)
                {
                    case FDataType.Bit_MASK:
                    case FDataType.BYTE:
                    case FDataType.Uint16:
                    case FDataType.Uint32:
                        //if (_tagNumber == 1077)
                        //    LogHandle.ol("tag 1077");
                        _valueInt = 0;
                        for (int i = 0; i < _rawData.Length; i++) _valueInt += (uint)Math.Round(_rawData[i] * Math.Pow(256, i));
                        Representation = _valueInt.ToString();
                        break;
                    case FDataType.STLV:
                        _nested = FTLVParcer.ParseStructure(data);
                        StringBuilder sb = new StringBuilder(BitConverter.ToString(_rawData) + Environment.NewLine);
                        foreach (FTag f in _nested) sb.Append(f.TagNumber.ToString() + "-" + f.Representation + ";  ");
                        Representation = sb.ToString();
                        break;
                    case FDataType.STRING:
                        //Encoding w1257 = Encoding.GetEncoding("windows-1257");
                        //Encoding cp866 = Encoding.GetEncoding("CP866");
                        _valueStr = cp866.GetString(data);
                        Representation = _valueStr;
                        break;
                    case FDataType.VLN:
                        _valueInt = 0;
                        _valueLong = 0;
                        for (int i = 0; i < _rawData.Length; i++) _valueLong += (ulong)Math.Round(_rawData[i] * Math.Pow(256, i));
                        _valueDouble = (double)_valueLong / 100.0;
                        Representation = _valueDouble.ToString();
                        break;
                    case FDataType.FVLN:
                        //Debug.WriteLine(BitConverter.ToString(_rawData));
                        int decimalPoint = data[0];
                        _valueDouble = 0;
                        for (int i = 1; i < data.Length; i++) _valueDouble += (int)Math.Round(data[i] * Math.Pow(256, i - 1));
                        _valueDouble /= Math.Pow(10, decimalPoint);
                        Representation = _valueDouble.ToString();
                        break;
                    case FDataType.U32UT:
                        long time = 0;
                        for (int i = 0; i < _rawData.Length; i++) time += (long)Math.Round(_rawData[i] * Math.Pow(256, i));
                        _valueDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(time);
                        Representation = _valueDateTime.ToString();
                        break;
                    case FDataType.Byte_ARRAY:
                        //var cp866_1 = Encoding.GetEncoding("CP866");
                        //if(TagNumber == 1077)
                        //{
                        //    ///111
                        //    _valueInt = 0;
                        //    for (int i = 2; i < _rawData.Length; i++) _valueInt += (uint)Math.Round(_rawData[i] * Math.Pow(256, _rawData.Length-(i-2)));
                        //    Representation = _valueInt.ToString();
                        //}
                        //else
                        if (tagNumber == FiscalPrinter.FTAG_DOC_FISCAL_SIGN)
                        {
                            uint t = 0;
                            byte[] b = data;
                            try
                            {
                                uint k = 256 * 256 * 256;
                                for (int i = 2; i < b.Length; i++)
                                {
                                    t += (uint)b[i] * k;
                                    k /= 256;
                                }
                            }
                            catch(Exception exc)
                            {
                                LogHandle.ol("parcing fiscal sign"+exc.Message);
                            }
                            if (t != 0)
                            {
                                _valueInt = t;
                                _valueStr = t.ToString();
                                Representation = _valueInt.ToString();
                            }
                        }
                        else
                        {
                            Representation = BitConverter.ToString(data);
                        }

                        
                        break;
                    case FDataType.UNKNOWN:
                    default:
                        Representation = BitConverter.ToString(_rawData);
                        break;
                }
            }
        }

        /*
         * создаем теги из значений в т.ч. расшифровки json
         */
        public FTag(int tagNumber, object value, bool createRawData)
        {

            _tagNumber = tagNumber;
            if (typeMap.ContainsKey(tagNumber)) _type = typeMap[tagNumber];
            else _type = FDataType.UNKNOWN;
            if (_type != FDataType.UNKNOWN)
            {
                if(_type == FDataType.STRING)
                {
                    _valueStr = value as string;
                    Representation = _valueStr;
                }
                else if(
                    _type == FDataType.Bit_MASK ||
                    _type == FDataType.BYTE ||
                    _type == FDataType.Uint16 ||
                    _type == FDataType.Uint32   
                    )
                {
                    if (value is string)
                    {
                        _valueInt = uint.Parse(value as string);
                    }
                    else if(value is int || value is uint)
                    {
                        _valueInt = (uint)(int)value;
                    }
                    else
                    {
                        LogHandle.ol("value neither str not int TN: "+ tagNumber+"  V: "+ value.ToString());
                        uint.TryParse(value.ToString(),out _valueInt);
                    }

                    if (_type == FDataType.BYTE && _valueInt > 255)
                    {
                        throw new Exception("Переданное значение выходит за пределы [0-255]");
                    }
                    if(_type == FDataType.Uint16&&_valueInt>65535)
                    {
                        throw new Exception("Переданное значение выходит за пределы [0-(2^16-1)]");
                    }

                    Representation = _valueInt.ToString();
                }
                else if(_type == FDataType.VLN)
                {
                    if(value is double)
                    {
                        _valueDouble = (double)value/100.0;
                    }
                    else if(value is int)
                    {
                        int price = value is int ? (int)value : int.Parse(value as string);
                        _valueDouble = ((double)price) / 100.0;
                    }
                    else if(value is uint)
                    {
                        uint price = value is uint ? (uint)value : uint.Parse(value as string);
                        _valueDouble = ((double)price) / 100.0;
                    }
                    else
                    {
                        long price = value is long ? (long)value : long.Parse(value as string);
                        _valueDouble = ((double)price / 100.0);
                    }
                    if(_valueLong == 0 && _valueDouble>0.009)
                    {
                        _valueLong = (ulong)Math.Round(_valueDouble*100);
                    }
                    Representation = _valueDouble.ToString();
                }
                else if(_type == FDataType.FVLN)
                {
                    double quantity = 0;
                    if (value is double|| value is float || value is decimal)
                        quantity = (double)value;
                    else
                        quantity = double.Parse(value as string);
                    _valueDouble = quantity;
                    Representation = _valueDouble.ToString();
                }
                else if(_type == FDataType.U32UT)
                {
                    if(value is string)
                    {
                        string s = value as string;
                        IFormatProvider prov = CultureInfo.InvariantCulture;
                        _valueDateTime = new DateTime(1970, 1, 1, 0, 0, 0);
                        DateTimeStyles st = DateTimeStyles.None;
                        if (!DateTime.TryParseExact(s, "dd/MM/yyyy HH:mm", prov, st, out _valueDateTime))
                            if (!DateTime.TryParseExact(s, "dd-MM-yyyy HH:mm", prov, st, out _valueDateTime))
                                if (!DateTime.TryParseExact(s, "dd.MM.yyyyTHH:mm", prov, st, out _valueDateTime))
                                    if (!DateTime.TryParseExact(s, "dd.MM.yyyy HH:mm", prov, st, out _valueDateTime))
                                        if (!DateTime.TryParseExact(s, "dd.MM.yyyy", prov, st, out _valueDateTime))
                                            if(!DateTime.TryParseExact(s, "yyyy.MM.dd HH:mm", prov, st, out _valueDateTime))
                                                if (!DateTime.TryParseExact(s, "yyyy.MM.dd HH:mm:ss", prov, st, out _valueDateTime))
                                                    if (!DateTime.TryParseExact(s, "yyyy/MM/dd HH:mm", prov, st, out _valueDateTime))
                                                        if(!DateTime.TryParseExact(s,"yyyy-MM-dd HH:mm",prov,st, out _valueDateTime))
                                                            if(!DateTime.TryParseExact(s,"yyyy-MM-dd HH:mm:ss",prov,st, out _valueDateTime))
                                                                if (!DateTime.TryParseExact(s, "O", prov, st, out _valueDateTime))
                                                                {
                                                                    long t = -1;
                                                                    long.TryParse(s, out t);
                                                                    if(t >0)
                                                                        _valueDateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(t);
                                                                }

                        Representation = _valueDateTime.ToString();
                    }
                    else if(value is int||value is uint|| value is long||value is ulong)
                    {
                        long time = (long)value;
                        _valueDateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(time);
                    }
                    else if(value is DateTime)
                    {
                        _valueDateTime = (DateTime)value;
                    }
                }
                else if (_type == FDataType.STLV)
                {
                    
                    _nested = value as List<FTag>;
                    Representation = "Вложенных тегов "+_nested.Count;
                }
                else if( _type == FDataType.Byte_ARRAY)
                {
                    if (tagNumber == FiscalPrinter.FTAG_DOC_FISCAL_SIGN)
                    {
                        if(value is byte[])
                        {
                            uint t = 0;
                            byte[] b = (byte[])value;
                            try
                            {
                                uint k = 256*256*256;
                                for(int i = 2; i < b.Length; i++)
                                {
                                    t += (uint)b[i] * k;
                                    k /= 256;
                                }
                            }
                            catch
                            {

                            }
                            if (t != 0)
                            {
                                _valueInt = t;
                                _valueStr = t.ToString();
                            }
                        }
                        else if(value is uint|| value is int || value is long || value is ulong)
                        {
                            _valueInt = (uint)value;
                            _valueStr = value.ToString();
                        }
                        else
                        {
                            _valueStr = value.ToString();
                        }

                    }
                    else if(_tagNumber == FiscalPrinter.FTAG_ADDITIONAL_DATA_OS||
                        _tagNumber == FiscalPrinter.FTAG_ADDITIONAL_DATA_CS)
                    {
                        Representation = value.ToString();
                        
                    }
                    else
                    {
                        if (value is string)
                        {
                            // тут стоит сделать разветвление на хекс образ и на строку в виде цифры
                            if(ulong.TryParse(value as string,out _valueLong))
                            {
                                Representation = _valueLong.ToString();
                            }
                        }
                        else if (value is uint)
                        {
                            _valueInt = (uint)value;
                            Representation = _valueInt.ToString();
                        }
                        else if (value is int)
                        {
                            _valueInt = (uint)(int)value;
                            Representation = _valueInt.ToString();
                        }
                        else if (value is byte[])
                        {
                            Representation = BitConverter.ToString((byte[])value);
                        }
                        else if(value is List<FTag>)
                        {
                            Representation = "Вложенных тегов " + (value as List<FTag>).Count;
                        }
                        else
                        {
                            Representation = value.ToString();
                        }
                    }
                    
                    
                }
                
            }

            if (createRawData)
            {
                RawDataConstructor();
            }
        }

        public void RebuildPrezentation()
        {
            if (_type != FDataType.UNKNOWN)
            {
                if (_type == FDataType.STRING)
                {
                    Representation = _valueStr;
                }
                else if (
                    _type == FDataType.Bit_MASK ||
                    _type == FDataType.BYTE ||
                    _type == FDataType.Uint16 ||
                    _type == FDataType.Uint32
                    )
                {
                    
                    Representation = _valueInt.ToString();
                }
                else if (_type == FDataType.VLN)
                {
                    
                    Representation = _valueDouble.ToString();
                }
                else if (_type == FDataType.FVLN)
                {
                    
                    Representation = _valueDouble.ToString();
                }
                else if (_type == FDataType.U32UT)
                {
                    Representation = _valueDateTime.ToString();
                }
                else if (_type == FDataType.STLV)
                {
                    int skipped = 0;
                    foreach(var sub in Nested)
                    {
                        if(sub.TagNumber == 0)
                            skipped++;
                    }

                    Representation = "Содержит вложенных тегов " + (_nested.Count-skipped);
                }
                else if (_type == FDataType.Byte_ARRAY)
                {
                    if(_valueInt>0)
                        Representation = _valueInt.ToString();
                    else if (_rawData != null)
                    {
                        Representation = BitConverter.ToString(_rawData);
                    }

                }

            }
        }


        
        public string Representation;
        private FDataType _type;
        private int _tagNumber;
        private byte[] _rawData;
        private string _valueStr;
        private uint _valueInt;
        private ulong _valueLong;   // добавлена для денежных сумм
        private double _valueDouble;
        private DateTime _valueDateTime;
        private List<FTag> _nested;
        public FDataType Type { get { return _type; } }
        public byte[] RawData { get { return _rawData; } }
        public int TagNumber { get { return _tagNumber; } }
        public List<FTag> Nested 
        { 
            get { return _nested; }
            set
            {
                if(value!=null && value is List<FTag>)
                {
                    _nested = value;
                }
            }
        }
        public string ValueStr { get { return _valueStr; } }
        public double ValueDouble { get { return _valueDouble; } }
        public uint ValueInt { get { return _valueInt; } }
        public DateTime ValueDT { get { return _valueDateTime; } }
        public ulong ValueLong { get => _valueLong; }
        public byte[] TagAsBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)(_tagNumber % 256));
            bytes.Add((byte)(_tagNumber / 256));
            if (_rawData == null || _rawData.Length == 0)
            {
                RawDataConstructor();
            }
            bytes.Add((byte)(_rawData.Length % 256));
            bytes.Add((byte)(_rawData.Length / 256));
            bytes.AddRange(_rawData);
            return bytes.ToArray();
        }

        public void RawDataConstructor()
        {
            //if (_tagNumber == 1023)
            //    LogHandle.ol("debug 1023");
            if(_type == FDataType.STRING)
            {
                _rawData = cp866.GetBytes(ValueStr);
            }
            else if(_type == FDataType.STLV)
            {
                List<byte> bytes = new List<byte>(); 
                foreach(FTag f in Nested)
                {
                    if (f.TagNumber == 0)// пропускаем пустые теги
                        continue;
                    bytes.AddRange(f.TagAsBytes());
                }
                _rawData = bytes.ToArray();
            }
            else if (_type == FDataType.BYTE)
            {
                _rawData = new byte[1] { (byte)_valueInt };
            }
            else if(_type == FDataType.Uint16)
            {
                _rawData = new byte[2] {(byte)(_valueInt%256), (byte)(_valueInt/256) };
            }
            else if(_type == FDataType.Uint32)
            {
                _rawData = new byte[4] { (byte)(_valueInt % 256), (byte)(_valueInt / 256 % 256), (byte)(_valueInt / 65536 % 256), (byte)(_valueInt / 16777261) };
            }
            else if (_type == FDataType.VLN)
            {
                ulong k = (ulong)Math.Round(_valueDouble*100);
                List<byte> bytes = new List<byte>();
                while(k > 0)
                {
                    bytes.Add((byte)(k % 256));
                    k /= 256;
                }
                if(bytes.Count==0)
                    _rawData = new byte[] { 0 };
                else
                    _rawData = bytes.ToArray();
            }
            else if(_type == FDataType.U32UT)
            {
                _valueInt = (uint)Math.Round((_valueDateTime - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
                _rawData = new byte[4] { (byte)(_valueInt % 256), (byte)(_valueInt / 256 % 256), (byte)(_valueInt / 65536 % 256), (byte)(_valueInt / 16777261) };
            }
            else if(_type == FDataType.Byte_ARRAY)
            {
                if(_tagNumber == FiscalPrinter.FTAG_ADDITIONAL_DATA_CS 
                    || _tagNumber == FiscalPrinter.FTAG_ADDITIONAL_DATA_OS)
                {
                    //длина данных до 32 байт должна быть только в хексе !!!
                    _rawData = FTLVParcer.StringHexToBytes(Representation);
                }
            }
            else if (_type == FDataType.FVLN)
            {
                List<byte> bytes = new List<byte>();
                long k = (int)Math.Round(_valueDouble * 1000000);
                bytes.Add(6);
                while (k > 0)
                {
                    bytes.Add((byte)(k % 256));
                    k /= 256;
                }
                _rawData = bytes.ToArray();
            }
            else if (_type == FDataType.Bit_MASK)
            {
                switch (_tagNumber)
                {
                    case FiscalPrinter.FTAG_APPLIED_TAXATION_TYPE:
                    case FiscalPrinter.FTAG_PAYMENT_AGENT_TYPE:
                    case FiscalPrinter.FTAG_REGISTERED_SNO:
                    case FiscalPrinter.FTAG_OPERATOR_MESSAGE:

                    case FiscalPrinter.FTAG_ITEM_PAYMENT_AGENT_BY_PRODUCT_TYPE:
                    case 2106:
                    case 2112:
                    case 2113:
                        _rawData = new byte[] { (byte)(_valueInt % 256) };
                        break;
                }
            }
        }

        public enum FDataType
        {
            Byte_ARRAY,
            Bit_MASK,
            BYTE,
            FVLN,
            NUMBER,
            OBJECT,
            STRING,
            STLV,
            Uint16,
            Uint32,
            U32UT,
            VLN,
            UNKNOWN
        }

        static Encoding cp866 = Encoding.GetEncoding("CP866");

        public  static Dictionary<int, FDataType> typeMap = new Dictionary<int, FDataType>()
        {
            [-1] = FDataType.UNKNOWN,   // ** Ошибка в разборе
            [0] = FDataType.UNKNOWN,    // ** для пустого тега
            [1] = FDataType.STLV,       // Отчет о регистрации
            [2] = FDataType.STLV,       // Открытие смены
            [3] = FDataType.STLV,       // Кассовый чек
            [4] = FDataType.STLV,       // БСО
            [5] = FDataType.STLV,       // Закрытие смены
            [6] = FDataType.STLV,       // Закрытие ФН
            [7] = FDataType.STLV,       // Подтверждение оператора

            [11] = FDataType.STLV,      // перерегистрация

            [21] = FDataType.STLV,      // отчет о состоянии расчетов

            [31] = FDataType.STLV,      // чек коррекции

            [41] = FDataType.STLV,      // БСО коррекции

            [1001] = FDataType.BYTE,    // Признак автоматического режима   autoMode
            [1002] = FDataType.BYTE,    // Признак автономного режим    offlineMode

            [1005] = FDataType.STRING,  // Адрес оператора перевода items.paymentAgentData.transferOperatorAddress

            [1008] = FDataType.STRING,  // Телефон или электронный адрес покупателя buyerPhoneOrAddress
            [1009] = FDataType.STRING,  // адрес расчетов   retailPlaceAddress

            [1012] = FDataType.U32UT,   // дата, время  dateTime
            [1013] = FDataType.STRING,  // заводской номер ККТ  kktNumber

            [1016] = FDataType.STRING,  // ИНН оператора перевода	AgentData.transferOperatorInn
            [1017] = FDataType.STRING,  // ИНН ОФД  ofdInn
            [1018] = FDataType.STRING,  // ИНН пользователя userInn

            [1020] = FDataType.VLN,     // ИТОГ totalSum
            [1021] = FDataType.STRING,  // кассир   operator

            [1023] = FDataType.FVLN,    // количество предмета расчета  items.quantity

            [1026] = FDataType.STRING,  // наименование оператора перевода (Для банковских платежных агентов (субагентов))  items.paymentAgentData.transferOperatorName

            [1030] = FDataType.STRING,  // наименование предмета расчета    items.name
            [1031] = FDataType.VLN,     // Сумма по чеку (БСО) наличными    cashTotalSum

            [1036] = FDataType.STRING,  // Номер автомата  machineNumber
            [1037] = FDataType.STRING,  // регистрационный номер ККТ   kktRegId
            [1038] = FDataType.Uint32,     // номер смены  shiftNumber

            [1040] = FDataType.Uint32,     // номер ФД fiscalDocumentNumber
            [1041] = FDataType.STRING,  // номер ФН fiscalDriveNumber
            [1042] = FDataType.Uint32,     // номер чека за смену  requestNumber
            [1043] = FDataType.VLN,     // стоимость предмета расчета с учетом скидок и наценок items.sum
            [1044] = FDataType.STRING,  // операция платежного агента	items.paymentAgentData.paymentAgentOperation

            [1046] = FDataType.STRING,  // наименование ОФД ofdName

            [1048] = FDataType.STRING,  // наименование пользователя    user

            [1050] = FDataType.BYTE,    // Признак исчерпания ресурса ФН    fiscalDriveExhaustionSign
            [1051] = FDataType.BYTE,    // Признак необходимости срочной замены ФН  fiscalDriveReplaceRequiredSign
            [1052] = FDataType.BYTE,    // Признак переполнения памяти ФН   fiscalDriveMemoryExceededSign
            [1053] = FDataType.BYTE,    // Признак превышения времени ожидания ответа ОФД   ofdResponseTimeoutSign
            [1054] = FDataType.BYTE,    // признак расчета  operationType
            [1055] = FDataType.Bit_MASK,  // Применяемая система налогообложения  appliedTaxationType
            [1056] = FDataType.BYTE,    // признак шифрования   encryptionSign
            [1057] = FDataType.Bit_MASK,  // Признак агента paymentAgentType
            [1058] = FDataType.Bit_MASK,  // признак платежного агента 
            [1059] = FDataType.STLV,    // предмет расчета  items
            [1060] = FDataType.STRING,  // адрес сайта ФНС  fnsUrl
            [1061] = FDataType.STRING,  // Сайт ОФД
            [1062] = FDataType.Bit_MASK,  // Системы налогообложения taxationType

            [1073] = FDataType.STRING,  // телефон платежного агента    items.paymentAgentData.pamentAgentPhone
            [1074] = FDataType.STRING,  // Телефон оператора по приему платежей items.paymentAgentData.paymentOperatorPhone
            [1075] = FDataType.STRING,  // Телефон оператора перевода   items.paymentAgentData.transferOperatorPhone
            [1077] = FDataType.Byte_ARRAY, // ФПД  fiscalSign

            [1079] = FDataType.VLN,     // Цена за единицу предмета расчета с учетом скидок и наценок   items.price
            [1081] = FDataType.VLN,     // Сумма по чеку (БСО) безналичными ecashTotalSum
            [1082] = FDataType.VLN,     // сумма оплаты безналичными

            [1084] = FDataType.STLV,    // Дополнительный реквизит пользователя properties
            [1085] = FDataType.STRING,  // Наименование дополнительного реквизита пользователя  properties.propertyName
            [1086] = FDataType.STRING,  // Значение дополнительного реквизита пользователя  properties.propertyValue

            [1097] = FDataType.Uint32,     // Количество непереданных ФД   notTransmittedDocumentsQuantity
            [1098] = FDataType.U32UT,   // Дата и время первого из непереданных ФД  notTransmittedDocumentsDateTime

            [1101] = FDataType.BYTE,    // Код причины перерегистрации  correctionReasonCode
            [1102] = FDataType.VLN,     // Сумма НДС чека по ставке 20% nds18
            [1103] = FDataType.VLN,     // Сумма НДС чека по ставке 10% nds10
            [1104] = FDataType.VLN,     // Сумма расчета по чеку с НДС по ставке 0% nds0
            [1105] = FDataType.VLN,     // Сумма расчета по чеку без НДС    ndsNo
            [1106] = FDataType.VLN,     // Сумма НДС чека по ставке 20/120  nds18118
            [1107] = FDataType.VLN,     // Сумма НДС чека по ставке 10/110  nds10110    
            [1108] = FDataType.BYTE,    // Признак ККТ для расчетов только в Интернет   internetSign
            [1109] = FDataType.BYTE,    // признак расчетов за услуги   serviceSign
            [1110] = FDataType.BYTE,    // признак АС БСО bsoSign
            [1111] = FDataType.Uint32,  // Общее количество ФД за смену documentsQuantity

            [1115] = FDataType.STLV,    // суммы ндс чека       (обновление НДС 5,7)
            [1116] = FDataType.Uint32,  // Номер первого непереданного документа    notTransmittedDocumentNumber
            [1117] = FDataType.STRING,  // Адрес электронной почты отправителя чека sellerAddress
            [1118] = FDataType.Uint32,  // Количество кассовых чеков (БСО) за смену receiptQuantity
            [1119] = FDataType.STLV,    // сумма НДС чека       (обновление НДС 5,7)
            [1120] = FDataType.VLN,     // Сумма НДС (обновление НДС 5,7)

            [1125] = FDataType.BYTE,    // признак расчета в «Интернет»
            [1126] = FDataType.BYTE,    // Признак проведения лотереи   lotterySign

            [1129] = FDataType.STLV,    // Счетчики операций «приход»   fiscalDriveSumReports.sellOper
            [1130] = FDataType.STLV,    // Счетчики операций «возврат прихода»  fiscalDriveSumReports.sellReturnOper
            [1131] = FDataType.STLV,    // Счетчики операций «расход»   fiscalDriveSumReports.buyOper
            [1132] = FDataType.STLV,    // Счетчики операций «возврат расхода»  fiscalDriveSumReports.buyReturnOper
            [1133] = FDataType.STLV,    // Счетчики операций по чекам коррекции fiscalDriveSumReports.receiptCorrection
            [1134] = FDataType.Uint32,     // Количество чеков (БСО) со всеми признаками расчетов  shiftSumReports.totalReceiptBsoCount
            [1135] = FDataType.Uint32,     // Количество чеков по признаку расчетов    shiftSumReports.sellOper.receiptBsoCount
            [1136] = FDataType.VLN,     // Итоговая сумма в чеках (БСО) наличными денежными средствами  shiftSumReports.sellOper.cashSum

            [1138] = FDataType.VLN,     // Итоговая сумма в чеках (БСО) электронными средствами платежа fiscalDriveSumReports.sellOper.ecashSum
            [1139] = FDataType.VLN,     // Сумма НДС по ставке 20 %     fiscalDriveSumReports.sellReturnOper.tax18Sum
            [1140] = FDataType.VLN,     // Сумма НДС по ставке 10 %    fiscalDriveSumReports.sellReturnOper.tax10Sum
            [1141] = FDataType.VLN,     // Сумма НДС по расч.ставке 20 / 120    fiscalDriveSumReports.sellReturnOper.tax18118Sum
            [1142] = FDataType.VLN,     // Сумма НДС по расч.ставке 10 / 110    fiscalDriveSumReports.sellReturnOper.tax10110Sum
            [1143] = FDataType.VLN,     // Сумма расчетов с НДС по ставке 0 %   fiscalDriveSumReports.sellReturnOper.tax0Sum
            [1144] = FDataType.Uint32,     // Количество чеков коррекции           fiscalDriveSumReports.sellReturnOper.tax0Sum
            [1145] = FDataType.STLV,    // Счетчики коррекций «приход»          fiscalDriveSumReports.receiptCorrection.sellCorrection
            [1146] = FDataType.STLV,    // Счетчики коррекций «расход»  fiscalDriveSumReports.receiptCorrection.buyCorrection

            [1157] = FDataType.STLV,    // Счетчики итогов ФН   fiscalDriveSumReports
            [1158] = FDataType.STLV,    // Счетчики итогов непереданных ФД  notTransmittedDocumentsSumReports

            [1162] = FDataType.STRING,  // код товара (Base64)  items.productCode
            [1163] = FDataType.STLV,    // код товара ФФД 1.2   items.productCodeNew
            [1171] = FDataType.STRING,  // Телефон поставщика   items.providerData.providerPhone
            [1173] = FDataType.BYTE,    // Тип коррекции    correctionType
            [1174] = FDataType.STLV,    // Основание для коррекции  сorrectionBase

            [1177] = FDataType.STRING,  // Описание коррекции 
            [1178] = FDataType.U32UT,   // Дата документа основания для коррекции   сorrectionBase.correctionDocumentDate
            [1179] = FDataType.STRING,  // Номер документа основания для коррекции  сorrectionBase.correctionDocumentNumber
            [1183] = FDataType.VLN,     // Сумма расчетов без НДС   fiscalDriveSumReports.sellOper.taxFreeSum
            [1184] = FDataType.VLN,     // сумма коррекций без НДС
            [1187] = FDataType.STRING,  // место расчетов   retailPlace
            [1188] = FDataType.STRING,  // версия ККТ   kktVersion
            [1189] = FDataType.BYTE,    // Версия ФФД ККТ   documentKktVersion
            [1190] = FDataType.BYTE,    // версия ФФД ФН    documentFdVersion
            [1191] = FDataType.STRING,  // Дополнительный реквизит предмета расчета items.propertiesItem
            [1192] = FDataType.STRING,  // Дополнительный реквизит чека (БСО)   propertiesData
            [1193] = FDataType.BYTE,    // Признак проведения азартных игр  gamblingSign
            [1194] = FDataType.STLV,    //

            [1197] = FDataType.STRING,  // Единица измерения предмета расчета   items.unit
            [1198] = FDataType.VLN,     // Размер НДС за единицу предмета расчета   items.unitNds
            [1199] = FDataType.BYTE,    // Ставка НДС   items.nds
            [1200] = FDataType.VLN,     // Сумма НДС за предмет расчета items.ndsSum
            [1201] = FDataType.VLN,     // Общая итоговая сумма в чеках (БСО) fiscalDriveSumReports.sellOper.totalSum

            [1203] = FDataType.STRING,  // ИНН кассира  operatorInn

            [1205] = FDataType.Bit_MASK,  // Коды причин изменения сведений о ККТ correctionKktReasonCode
            [1206] = FDataType.Bit_MASK,  // Сообщение оператора  operatorMessage
            [1207] = FDataType.BYTE,    // Признак торговли подакцизными товарами   exciseDutyProductSign

            [1209] = FDataType.BYTE,    // номер версии ФФД fiscalDocumentFormatVer
            
            [1212] = FDataType.BYTE,    // Признак предмета расчета items.productType
            [1213] = FDataType.Uint16,     // Ресурс ключей ФП fdKeyResource
            [1214] = FDataType.BYTE,    // Признак способа расчета  items.paymentType
            [1215] = FDataType.VLN,     // Сумма по чеку(БСО) предоплатой(зачетом аванса и(или) предыдущих платежей)    prepaidSum
            [1216] = FDataType.VLN,     // Сумма по чеку(БСО) постоплатой(в кредит) creditSum
            [1217] = FDataType.VLN,     // Сумма по чеку(БСО) встречным предоставлением creditSum
            [1218] = FDataType.VLN,     // Итоговая сумма в чеках(БСО) предоплатами(авансами)   fiscalDriveSumReports.sellOper.prepaidSum
            [1219] = FDataType.VLN,     // Итоговая сумма в чеках(БСО) постоплатами(кредитами)  fiscalDriveSumReports.sellOper.creditSum
            [1220] = FDataType.VLN,     // Итоговая сумма в чеках(БСО) встречными предоставлениями  fiscalDriveSumReports.sellOper.provisionSum
            [1221] = FDataType.BYTE,    // Признак установки принтера в автомате    printInMachineSign
            [1222] = FDataType.Bit_MASK,  // Признак агента по предмету расчета   items.paymentAgentByProductType
            [1223] = FDataType.STLV,    // Данные агента    items.paymentAgentData
            [1224] = FDataType.STLV,    // Данные поставщика    items.providerData
            [1225] = FDataType.STRING,  // Наименование поставщика  items.providerData.providerName
            [1226] = FDataType.STRING,  // ИНН поставщика   items.providerInn
            [1227] = FDataType.STRING,  // Покупатель(клиент)   buyerInformation.buyer
            [1228] = FDataType.STRING,  // ИНН покупателя(клиента)  buyerInformation.buyerInn
            [1229] = FDataType.VLN,     // сумма акциза с учётом копеек, включенная в стоимость предмета расчёта    items.exciseDuty
            [1230] = FDataType.STRING,  // Код страны происхождения товара  items.originCountryCode
            [1231] = FDataType.STRING,  // Номер таможенной декларации  items.customEntryNum
            [1232] = FDataType.STLV,    // Счетчики по признаку «возврат прихода»   fiscalDriveSumReports.receiptCorrection.sellReturnCorrection
            [1233] = FDataType.STLV,    // Счетчики по признаку «возврат расхода»   fiscalDriveSumReports.receiptCorrection.buyReturnCorrection
            [1234] = FDataType.STLV,    // сведения  обо всехоплатах почекубезналичными
            [1235] = FDataType.STLV,    // сведения об оплате безналичными
            [1236] = FDataType.BYTE,    // признак способа оплаты безналичными 
            [1237] = FDataType.STRING,  // Идентификаторы безналичной оплаты 
            [1238] = FDataType.STRING,  // Дополнительные сведения о безналичной оплате 

            [1243] = FDataType.STRING,  // дата рождения покупателя(клиента)	buyerInformation.buyerBirthday
            [1244] = FDataType.STRING,  // гражданство  buyerInformation.buyerCitizenship
            [1245] = FDataType.STRING,  // код вида документа, удостоверяю щего личность	buyerInformation.buyerDocumentCode
            [1246] = FDataType.STRING,  // данные документа, удостоверяющеголичность	buyerInformation.buyerDocumentData

            [1254] = FDataType.STRING,  // адрес покупателя(клиента)	buyerInformation.buyerAddress

            [1256] = FDataType.STLV,    // сведения о покупателе(клиенте)   buyerInformaton

            [1260] = FDataType.STLV,    // отраслевой реквизит предмета расчета	items.itemsIndustryDetails
            [1261] = FDataType.STLV,    // отраслевой реквизит чека	industryReceiptDetails
            [1262] = FDataType.STRING,  // Идентификатор ФОИВ 	items.itemsIndustryDetails.idFoiv
            [1263] = FDataType.STRING,  // дата документа основания	items.itemsIndustryDetails.foundationDocDateTime
            [1264] = FDataType.STRING,  // номер документа основания	items.itemsIndustryDetails.foundationDocNumber
            [1265] = FDataType.STRING,  // значение отраслевого реквизита	items.itemsIndustryDetails.industryPropValue 

            [1270] = FDataType.STLV,    // операционный реквизит чека	operationalDetails
            [1271] = FDataType.BYTE,    // идентификатор операции	operationalDetails.operationId
            [1272] = FDataType.BYTE,    // operationalDetails.operationData
            [1273] = FDataType.U32UT,   // дата, время операции operationalDetails.dateTime 
            [1274] = FDataType.STRING,  // дополнительный реквизит ОР   additionalPropsFRC
            [1275] = FDataType.Byte_ARRAY, // дополнительные данные ОР     additionalDataFRC
            [1276] = FDataType.STRING,  // Дополнительный реквизит ООС  additionalPropsOS
            [1277] = FDataType.Byte_ARRAY, // Дополнительные данные ООС    additionalDataOS
            [1278] = FDataType.STRING,  // дополнительный реквизит ОЗС  additionalPropsCS
            [1279] = FDataType.Byte_ARRAY, // дополнительные данные ОЗС    additionalDataCS
            [1280] = FDataType.STRING,  // дополнительный реквизит ОТР  additionalPropsCSR
            [1281] = FDataType.Byte_ARRAY, // дополнительные данные ОТР    additionalDataCSR
            [1282] = FDataType.STRING,  // дополнительный реквизит ОЗФН additionalPropsCA
            [1283] = FDataType.Byte_ARRAY, // дополнительные данные ОЗФН   additionalDataCA

            [1290] = FDataType.Bit_MASK,  // признаки условий применения ККТ	usageConditionSigns (4 bytes)
            [1291] = FDataType.STLV,    // дробное количество маркированного товара	items.labeledProdFractionalQuantity
            [1292] = FDataType.STRING,  // дробная часть	items.labeledProdFractionalQuantity.fractionalPart цифры/цифры
            [1293] = FDataType.VLN,     // числитель    items.labeledProdFractionalQuantity.numerator
            [1294] = FDataType.VLN,     // знаменатель  items.labeledProdFractionalQuantity.denominator

            [1300] = FDataType.STRING,  // нераспознанный код товара	items.productCodeNew.undefined
            [1301] = FDataType.STRING,  // КТ EAN-8 items.productCodeNew.ean8  
            [1302] = FDataType.STRING,  // КТ EAN-13 items.productCodeNew.ean13 
            [1303] = FDataType.STRING,  // КТ ITF-14 items.productCodeNew.itf14
            [1304] = FDataType.STRING,  // КТ GS1.0 items.productCodeNew.gs1
            [1305] = FDataType.STRING,  // КТ GS1.М items.productCodeNew.gs1m
            [1306] = FDataType.STRING,  // КТ КМК items.productCodeNew.kmk
            [1307] = FDataType.STRING,  // КТ МИ items.productCodeNew.mi
            [1308] = FDataType.STRING,  // КТ ЕГАИС2.0 items.productCodeNew.egais2
            [1309] = FDataType.STRING,  // КТ ЕГАИС3.0 items.productCodeNew.egais3

            [1320] = FDataType.STRING,  // КТ Ф.1 items.productCodeNew.f1
            [1321] = FDataType.STRING,  // КТ Ф.2 0 items.productCodeNew.f2
            [1322] = FDataType.STRING,  // КТ Ф.3 0 items.productCodeNew.f3  
            [1323] = FDataType.STRING,  // КТ Ф.4 0 items.productCodeNew.f4
            [1324] = FDataType.STRING,  // КТ Ф.5 0 items.productCodeNew.f5
            [1325] = FDataType.STRING,  // КТ Ф.6 0 items.productCodeNew.f6

            [2102] = FDataType.BYTE,    // режим обработки кода маркировки	items.labelCodeProcesMode

            [2104] = FDataType.Uint32,     // количество непереданных уведомлений  undeliveredNotificationsNumber

            [2106] = FDataType.Bit_MASK,  // результат проверки сведений о товаре	items.checkingProdInformationResult
            [2107] = FDataType.BYTE,    // результаты проверки маркированных товаров	checkingLabeledProdResult
            [2108] = FDataType.BYTE,    // мера количества предмета расчета	items.itemsQuantityMeasure

            [2112] = FDataType.Bit_MASK,  // признак некорректных кодов маркировки	invalidLabelCodesSign
            [2113] = FDataType.Bit_MASK,  // признак некорректных запросов и уведомлений	invalidRequestsNotificationsSign

            [2115] = FDataType.STRING,  // контрольный код КМ   items.controlCode
        };


        public static  Dictionary<int, string> userFrandlyNames = new Dictionary<int, string>()
        {
            [1] = "Регистрация",
            [2] = "Открытие смены",
            [3] = "Кассовый чек",
            [4] = "БСО",
            [5] = "Закрытие смены",
            [6] = "Закрытие ФН",
            [7] = "Подтверждение оператора",
            [11] = "Перерегистрация",
            [21] = "Отчет о состоянии расчетов",
            [31] = "Чек коррекции",
            [41] = "БСО коррекции",

            [1001] = "Признак автоматического режима",
            [1002] = "Признак автономного режим",

            [1005] = "Адрес оператора перевода",

            [1008] = "Телефон или электронный адрес",
            [1009] = "Адрес расчетов",

            [1012] = "Дата, время",
            [1013] = "ККТ",

            [1016] = "ИНН оператора перевода",
            [1017] = "ИНН ОФД",
            [1018] = "ИНН",

            [1020] = "ИТОГ",
            [1021] = "Кассир",

            [1023] = "Количество предмета расчета",

            [1026] = "Наименование оператора перевода",

            [1030] = "Наименование",
            [1031] = "Наличными",

            [1036] = "Номер автомата",
            [1037] = "Рег. № ККТ",
            [1038] = "Номер смены",

            [1040] = "ФД",
            [1041] = "ФН",
            [1042] = "Номер чека за смену",
            [1043] = "Стоимость предмета расчета",
            [1044] = "Операция платежного агента",

            [1046] = "Наименование ОФД",

            [1048] = "Пользователя",

            [1050] = "Признак исчерпания ресурса ФН",
            [1051] = "Признак необходимости срочной замены ФН",
            [1052] = "Признак переполнения памяти ФН",
            [1053] = "Признак превышения времени ожидания ответа ОФД",
            [1054] = "признак расчета",
            [1055] = "СНО",  
            [1056] = "Признак шифрования",
            [1057] = "Признак агента",
            [1058] = "Признак платежного агента", 
            [1059] = "Предмет расчета",

            [1060] = "Адрес сайта ФНС",
            [1061] = "Сайт ОФД",
            [1062] = "Системы налогообложения",

            [1073] = "телефон платежного агента",
            [1074] = "Телефон оператора по приему платежей",
            [1075] = "Телефон оператора перевода",
            [1077] = "ФП",

            [1079] = "Цена за единицу предмета расчета",
            [1081] = "Безналичными",
            [1082] = "Сумма оплаты безналичными",

            [1084] = "Доп. реквизит пользователя",    // "Дополнительный реквизит пользователя properties
            [1085] = "Наименование дополнительного реквизита пользователя",
            [1086] = "Значение дополнительного реквизита пользователя",

            [1097] = "Количество непереданных ФД",
            [1098] = "Дата и время первого из непереданных ФД",

            [1101] = "Код причины перерегистрации",
            [1102] = "Сумма НДС чека по ставке 20%",
            [1103] = "Сумма НДС чека по ставке 10%",
            [1104] = "Сумма расчета по чеку с НДС по ставке 0%",
            [1105] = "Сумма расчета по чеку без НДС",
            [1106] = "Сумма НДС чека по ставке 20/120",
            [1107] = "Сумма НДС чека по ставке 10/110",    
            [1108] = "Признак ККТ для расчетов только в Интернет",
            [1109] = "признак расчетов за услуги",
            [1110] = "признак АС БСО",
            [1111] = "Общее количество ФД за смену",

            [1115] = "Cуммы НДС чека",
            [1116] = "Номер первого непереданного документа",
            [1117] = "Адрес электронной почты отправителя чека",
            [1118] = "Количество кассовых чеков за смену",
            [1119] = "Cумма НДС чека",
            [1120] = "Cумма НДС",

            [1125] = "Признак расчета в «Интернет»",
            [1126] = "Признак проведения лотереи",

            [1129] = "Счетчики операций приход",
            [1130] = "Счетчики операций возврат прихода" ,
            [1131] = "Счетчики операций расход",
            [1132] = "Счетчики операций возврат расхода",
            [1133] = "Счетчики операций по чекам коррекции",

            [1145] = "Счетчики коррекций «приход",
            [1146] = "Счетчики коррекций «расход",

            [1157] = "Счетчики итогов ФН",
            [1158] = "Счетчики итогов непереданных ФД",

            [1162] = "Код товара(ФФД 1.05)",
            [1163] = "Код товара(ФФД 1.2)",
            
            [1173] = "Тип коррекции(0-самостоятельно, 1-по проедписанию): ",
            [1174] = "Основание коррекции",

            [1177] = "Описание коррекции(!!!устаревший тег!!! - проверьте актуальноть прошивки ККТ)",// устаревший тег
            [1178] = "Дата документа основания для коррекции",//    сorrectionBase.correctionDocumentDate
            [1179] = "Номер документа основания для коррекции",//   сorrectionBase.correctionDocumentNumber
            [1183] = "Сумма расчетов без НДС",//    fiscalDriveSumReports.sellOper.taxFreeSum
            [1184] = "Сумма коррекций без НДС",// 
            [1187] = "Место расчетов",//    retailPlace
            [1188] = "Версия ККТ",//    kktVersion
            [1189] = "Версия ФФД ККТ",//    documentKktVersion
            [1190] = "Версия ФФД ФН",//     documentFdVersion
            [1191] = "Дополнительный реквизит предмета расчета",//  items.propertiesItem
            [1192] = "Дополнительный реквизит чека",//  (БСО)   propertiesData
            [1193] = "Признак проведения азартных игр",//   gamblingSign
            [1194] = "Cчетчики итогов смены",

            [1197] = "Единица измерения предмета расчета(1.05)",  //    items.unit
            [1198] = "Размер НДС за единицу предмета расчета",  //    items.unitNds
            [1199] = "Ставка НДС",  //   items.nds
            [1200] = "Сумма НДС за предмет расчета",//  items.ndsSum
            [1201] = "Общая итоговая сумма в чеках",//  (БСО) fiscalDriveSumReports.sellOper.totalSum

            [1203] = "ИНН кассира", //   operatorInn

            [1205] = "Коды причин изменения сведений о ККТ",//  correctionKktReasonCode
            [1206] = "Сообщение оператора", //   operatorMessage
            [1207] = "Признак торговли подакцизными товарами",//    exciseDutyProductSign

            [1209] = "номер версии ФФД",    //  fiscalDocumentFormatVer

            [1212] = "Признак предмета расчета",//  items.productType
            [1213] = "Ресурс ключей ФП",//  fdKeyResource
            [1214] = "Признак способа расчета",//   items.paymentType
            [1215] = "Сумма по чеку предоплатой(зачетом аванса)",//  предыдущих платежей)    prepaidSum
            [1216] = "Сумма по чеку постоплатой(в кредит)",//  creditSum
            [1217] = "Сумма по чеку встречным предоставлением",//  creditSum
            [1218] = "Итоговая сумма в чеках(БСО) предоплатами(авансами)",//    fiscalDriveSumReports.sellOper.prepaidSum
            [1219] = "Итоговая сумма в чеках(БСО) постоплатами(кредитами)",//   fiscalDriveSumReports.sellOper.creditSum
            [1220] = "Итоговая сумма в чеках(БСО) встречными предоставлениями",//   fiscalDriveSumReports.sellOper.provisionSum
            [1221] = "Признак установки принтера в автомате",//     printInMachineSign
            [1222] = "Признак агента по предмету расчета",//    items.paymentAgentByProductType



            [1223] = "Данные агента",
            [1224] = "Данные поставщика",

            [1232] = "Счетчики по признаку возврат прихода",
            [1233] = "Счетчики по признаку возврат расхода",
            [1234] = "Cведения обо всех оплатах по чеку безналичными",
            [1235] = "Cведения об оплате безналичными",
            [1236] = "Признак способа оплаты безналичными ",
            [1237] = "Идентификаторы безналичной оплаты",
            [1238] = "Дополнительные сведения о безналичной оплате",

            [1256] = "сведения о покупателе",

            [1260] = "Отраслевой реквизит предмета расчета"	,
            [1261] = "Отраслевой реквизит чека"	,

            [1270] = "Операционный реквизит чека"	,

            [1290] = "Признаки условий применения ККТ",
            [1291] = "Дробное количество маркированного товара",
            [1292] = "дробная часть",//	items.labeledProdFractionalQuantity.fractionalPart цифры/цифры
            [1293] = "Числитель",//    items.labeledProdFractionalQuantity.numerator
            [1294] = "Знаменатель",//  items.labeledProdFractionalQuantity.denominator

            [1300] = "Нераспознанный код товара",//	items.productCodeNew.undefined
            [1301] = "КТ EAN-8",// items.productCodeNew.ean8  
            [1302] = "КТ EAN-13",// items.productCodeNew.ean13 
            [1303] = "КТ ITF-14",// items.productCodeNew.itf14
            [1304] = "КТ GS1.0",// items.productCodeNew.gs1
            [1305] = "КТ GS1.М",// items.productCodeNew.gs1m
            [1306] = "КТ КМК",// items.productCodeNew.kmk
            [1307] = "КТ МИ",// items.productCodeNew.mi
            [1308] = "КТ ЕГАИС2.0",// items.productCodeNew.egais2
            [1309] = "КТ ЕГАИС3.0",// items.productCodeNew.egais3

            [1320] = "КТ Ф.1",// items.productCodeNew.f1
            [1321] = "КТ Ф.2 0",// items.productCodeNew.f2
            [1322] = "КТ Ф.3 0",// items.productCodeNew.f3  
            [1323] = "КТ Ф.4 0",// items.productCodeNew.f4
            [1324] = "КТ Ф.5 0",// items.productCodeNew.f5
            [1325] = "КТ Ф.6 0",// items.productCodeNew.f6

            [2102] = "Режим обработки кода маркировки",//	items.labelCodeProcesMode

            [2104] = "Количество непереданных уведомлений",//  undeliveredNotificationsNumber

            [2106] = "Результат проверки сведений о товаре",//	items.checkingProdInformationResult
            [2107] = "Результаты проверки маркированных товаров",//	checkingLabeledProdResult
            [2108] = "Мера количества предмета расчета",//	items.itemsQuantityMeasure

            [2112] = "Признак некорректных кодов маркировки",//	invalidLabelCodesSign
            [2113] = "Признак некорректных запросов и уведомлений",//	invalidRequestsNotificationsSign

            [2115] = "Контрольный код КМ",//   items.controlCode

        };


        // используется этот словарь для FTag.ToString()
        public static  Dictionary<int, string> fnsNames = new Dictionary<int, string>()
        {

            [1001] = "autoMode",
            [1002] = "offlineMode",
            [1005] = "transferOperatorAddress",
            [1008] = "buyerPhoneOrAddress",
            [1009] = "retailPlaceAddress",
            [1012] = "dateTime",
            [1013] = "kktNumber",
            [1016] = "transferOperatorInn",
            [1017] = "ofdInn",
            [1018] = "userInn",
            [1020] = "totalSum",
            
            [1021] = "operator",
            [1023] = "quantity",
            [1026] = "transferOperatorName",
            [1030] = "name",
            [1031] = "cashTotalSum",
            [1036] = "machineNumber",
            [1037] = "kktRegId",
            [1038] = "shiftNumber",
            [1040] = "fiscalDocumentNumber",
            [1041] = "fiscalDriveNumber",
            [1042] = "requestNumber",
            [1043] = "sum",
            [1044] = "paymentAgentOperation",
            [1046] = "ofdName",
            [1048] = "user",
            [1050] = "fiscalDriveExhaustionSign",
            [1051] = "fiscalDriveReplaceRequiredSign",
            [1052] = "fiscalDriveMemoryExceededSign",
            [1053] = "ofdResponseTimeoutSign",
            [1054] = "operationType",
            [1055] = "appliedTaxationType",
            [1056] = "encryptionSign",
            [1057] = "paymentAgentType",
            [1059] = "items",
            [1060] = "fnsUrl",
            [1062] = "taxationType",
            [1073] = "paymentAgentPhone",
            [1074] = "paymentOperatorPhone",
            [1075] = "transferOperatorPhone",
            [1077] = "fiscalSign",
            [1079] = "price",
            [1081] = "ecashTotalSum",
            [1082] = "ecashSum",
            [1084] = "properties",
            [1085] = "propertyName",
            [1086] = "propertyValue",
            [1097] = "notTransmittedDocumentsQuantity",
            [1098] = "notTransmittedDocumentsDateTime",
            [1102] = "nds18",
            [1103] = "nds10",
            [1104] = "nds0",
            [1105] = "ndsNo",
            [1106] = "nds18118",
            [1107] = "nds10110",
            [1108] = "internetSign",
            [1109] = "serviceSign",
            [1110] = "bsoSign",     // 1.05
            [1111] = "documentsQuantity",

            [1115] = "amountsReceiptNds",     
            [1116] = "notTransmittedDocumentNumber",
            [1117] = "sellerAddress",
            [1118] = "receiptQuantity",
            [1119] = "amountsNds",     
            [1120] = "ndsSum",

            [1125] = "internetPayment",
            [1126] = "lotterySign",
            [1129] = "sellOper",
            [1130] = "sellReturnOper",
            [1131] = "buyOper",
            [1132] = "buyReturnOper",
            [1133] = "receiptCorrection",
            [1134] = "totalReceiptBsoCount",
            [1135] = "receiptBsoCount",
            [1136] = "cashSum",
            [1138] = "ecashSum",
            [1139] = "tax18Sum",
            [1140] = "tax10Sum",
            [1141] = "tax18118Sum",
            [1142] = "tax10110Sum",
            [1143] = "tax0Sum",
            [1144] = "receiptCorrectionCount",
            [1145] = "sellCorrection",
            [1146] = "buyCorrection",
            [1157] = "fiscalDriveSumReports",
            [1158] = "notTransmittedDocumentsSumReports",
            [1162] = "productCode",
            [1163] = "productCodeNew",
            [1171] = "providerPhone",
            [1173] = "correctionType",
            [1174] = "сorrectionBase",
            [1178] = "correctionDocumentDate",
            [1179] = "correctionDocumentNumber",
            [1183] = "taxFreeSum",
            [1187] = "retailPlace",
            [1188] = "kktVersion",
            [1189] = "documentKktVersion",
            [1190] = "documentFdVersion",
            [1191] = "propertiesItem",
            [1192] = "propertiesData",
            [1193] = "gamblingSign",    //1.05
            [1194] = "shiftSumReports",
            [1197] = "unit",
            [1198] = "unitNds",
            [1199] = "nds",
            [1200] = "ndsSum",
            [1201] = "totalSum",
            [1203] = "operatorInn",
            [1205] = "correctionKktReasonCode",
            [1206] = "operatorMessage",
            [1207] = "exciseDutyProductSign",   //1.05
            [1209] = "fiscalDocumentFormatVer",
            [1212] = "productType",
            [1213] = "fdKeyResource",
            [1214] = "paymentType",
            [1215] = "prepaidSum",
            [1216] = "creditSum",
            [1217] = "provisionSum",
            [1218] = "prepaidSum",
            [1219] = "creditSum",
            [1220] = "provisionSum",
            [1221] = "printInMachineSign",  //1.05
            [1222] = "paymentAgentByProductType",
            [1223] = "paymentAgentData",
            [1224] = "providerData",
            [1225] = "providerName",
            [1226] = "providerInn",
            [1227] = "buyer",
            [1228] = "buyerInn",
            [1229] = "exciseDuty",
            [1230] = "originCountryCode",
            [1231] = "customEntryNum",
            [1232] = "sellReturnCorrection",
            [1233] = "buyReturnCorrection",
            [1243] = "buyerBirthday",
            [1244] = "buyerCitizenship",
            [1245] = "buyerDocumentCode",
            [1246] = "buyerDocumentData",
            [1254] = "buyerAddress",
            [1256] = "buyerInformation",
            [1260] = "itemsIndustryDetails",
            [1261] = "industryReceiptDetails",
            [1262] = "idFoiv",
            [1263] = "foundationDocDateTime",
            [1264] = "foundationDocNumber",
            [1265] = "industryPropValue",
            [1270] = "operationalDetails",
            [1271] = "operationId",
            [1272] = "operationData",
            [1273] = "dateTime",
            [1274] = "additionalPropsFRC",
            [1275] = "additionalDataFRC",
            [1276] = "additionalPropsOS",
            [1277] = "additionalDataOS",
            [1278] = "additionalPropsCS",
            [1279] = "additionalDataCS",
            [1280] = "additionalPropsCSR",
            [1281] = "additionalDataCSR",
            [1282] = "additionalPropsCA",
            [1283] = "additionalDataCA",
            [1290] = "usageConditionSigns",
            [1291] = "labeledProdFractionalQuantity",
            [1292] = "fractionalPart",
            [1293] = "numerator",
            [1294] = "denominator",
            [1300] = "undefined",
            [1301] = "ean8",
            [1302] = "ean13",
            [1303] = "itf14",
            [1304] = "gs1",
            [1305] = "gs1m",
            [1306] = "kmk",
            [1307] = "mi",
            [1308] = "egais2",
            [1309] = "egais3",
            [1320] = "f1",
            [1321] = "f2",
            [1322] = "f3",
            [1323] = "f4",
            [1324] = "f5",
            [1325] = "f6",
            [2102] = "labelCodeProcesMode",
            [2104] = "undeliveredNotificationsNumber",
            [2106] = "checkingProdInformationResult",
            [2107] = "checkingLabeledProdResult",
            [2108] = "itemsQuantityMeasure",
            [2112] = "invalidLabelCodesSign",
            [2113] = "invalidRequestsNotificationsSign",
            [2115] = "controlCode",

            [1234] = "allECashProp",
            [1235] = "ecashProp",
            [1236] = "ecashSign",
            [1237] = "ecashIdent",
            [1238] = "ecashAddInfo",

        };

        
        // для кастомных ворматов ОФД
        public static bool rulesChanged = false;
        static int defaultFormCode = 0;
        public static int DefaultFormCode { get => defaultFormCode; set => defaultFormCode = value; }


        public  static Dictionary<string, int> structuredTagNames = new Dictionary<string, int>()
        {
            ["autoMode"] = 1001,
            ["offlineMode"] = 1002,

            ["items.paymentAgentData.transferOperatorAddress"] = 1005, //--items.paymentAgentData.transferOperatorAddress

            ["buyerPhoneOrAddress"] = 1008,
            ["retailPlaceAddress"] = 1009,

            ["dateTime"] = 1012,
            ["kktNumber"] = 1013,

            ["items.paymentAgentData.transferOperatorInn"] = 1016, //--items.paymentAgentData.transferOperatorInn
            ["ofdInn"] = 1017,
            ["userInn"] = 1018,

            ["totalSum"] = 1020,
            ["operator"] = 1021,

            ["items.quantity"] = 1023,   //--items.quantity

            ["paymentAgentData.transferOperatorName"] = 1026,   //--paymentAgentData.transferOperatorName

            ["items.name"] = 1030,   //--tems.name
            ["cashTotalSum"] = 1031,

            ["machineNumber"] = 1036,
            ["kktRegId"] = 1037,
            ["shiftNumber"] = 1038,

            ["fiscalDocumentNumber"] = 1040,
            ["fiscalDriveNumber"] = 1041,
            ["requestNumber"] = 1042,       //          * номер чека за смену
            ["items.sum"] = 1043,       //--items.sum
            ["items.paymentAgentData.paymentAgentOperation"] = 1044,//--items.paymentAgentData.paymentAgentOperation

            ["ofdName"] = 1046,

            ["user"] = 1048,

            ["fiscalDriveExhaustionSign"] = 1050,
            ["fiscalDriveReplaceRequiredSign"] = 1051,
            ["fiscalDriveMemoryExceededSign"] = 1052,
            ["ofdResponseTimeoutSign"] = 1053,
            ["operationType"] = 1054,
            ["appliedTaxationType"] = 1055,
            ["encryptionSign"] = 1056,
            ["paymentAgentType"] = 1057,

            ["items"] = 1059,
            ["fnsUrl"] = 1060,

            ["taxationType"] = 1062,

            ["items.paymentAgentData.paymentAgentPhone"] = 1073,   //--items.paymentAgentData.paymentAgentPhone
            ["items.paymentAgentData.paymentOperatorPhone"] = 1074,   //--items.paymentAgentData.paymentOperatorPhone
            ["items.paymentAgentData.transferOperatorPhone"] = 1075,   //--items.paymentAgentData.transferOperatorPhone

            ["fiscalSign"] = 1077,

            ["items.price"] = 1079,   //--items.price

            ["ecashTotalSum"] = 1081,
            ["allECashProp.ecashProp.ecashSum"] = 1082,     //сумма оплаты безналичными

            ["properties"] = 1084,              //      *Дополнительный реквизит пользователя
            ["properties.propertyName"] = 1085,            
            ["properties.propertyValue"] = 1086,    //  --properties.propertyValue   * значение дополнительного реквизита пользовател

            ["notTransmittedDocumentsQuantity"] = 1097,
            ["notTransmittedDocumentsDateTime"] = 1098,

            ["nds18"] = 1102,
            ["nds10"] = 1103,
            ["nds0"] = 1104,
            ["ndsNo"] = 1105,
            ["nds18118"] = 1106,
            ["nds10110"] = 1107,
            ["internetSign"] = 1108,
            ["serviceSign"] = 1109,
            ["bsoSign"] = 1110,

            ["documentsQuantity"] = 1111,

            ["amountsReceiptNds"]  = 1115,  // суммы НДС чека  
            ["notTransmittedDocumentNumber"] = 1116,
            ["sellerAddress"] = 1117,
            ["receiptQuantity"] = 1118,
            ["amountsReceiptNds.amountsNds"] = 1119,  // Cумма НДС чека
            ["amountsReceiptNds.amountsNds.ndsSum"] = 1120,  // сумма НДС 
            ["amountsReceiptNds.amountsNds.nds"] = 1199,  // ставка НДС
                                                          //["docNdsSums.ndsSums.ndsSum.nds"] = 1199,
            ["internetPayment"] = 1125, 
            ["lotterySign"] = 1126,

            ["fiscalDriveSumReports.sellOper"] = 1129,  //--fiscalDriveSumReports.sellOper
            ["fiscalDriveSumReports.sellReturnOper"] = 1130,  //--fiscalDriveSumReports.sellReturnOper
            ["fiscalDriveSumReports.buyOper"] = 1131,  //--fiscalDriveSumReports.buyOper
            ["fiscalDriveSumReports.buyReturnOper"] = 1132,  //--fiscalDriveSumReports.buyReturnOper
            ["fiscalDriveSumReports.receiptCorrection"] = 1133,  //--fiscalDriveSumReports.receiptCorrection
            ["fiscalDriveSumReports.totalReceiptBsoCount"] = 1134,  //--fiscalDriveSumReports.totalReceiptBsoCount
            ["fiscalDriveSumReports.sellOper.receiptBsoCount"] = 1135,  //--fiscalDriveSumReports.sellOper.receiptBsoCount
            ["fiscalDriveSumReports.sellOper.cashSum"] = 1136,  //--fiscalDriveSumReports.sellOper.cashSum

            ["fiscalDriveSumReports.sellOper.ecashSum"] = 1138,  //--fiscalDriveSumReports.sellOper.ecashSum
            ["fiscalDriveSumReports.sellOper.tax18Sum"] = 1139,  //--fiscalDriveSumReports.sellOper.tax18Sum
            ["fiscalDriveSumReports.sellOper.tax10Sum"] = 1140,  //--fiscalDriveSumReports.sellOper.tax10Sum
            ["fiscalDriveSumReports.sellOper.tax18118Sum"] = 1141,  //--fiscalDriveSumReports.sellOper.tax18118Sum
            ["fiscalDriveSumReports.sellOper.tax10110Sum"] = 1142,  //--fiscalDriveSumReports.sellOper.tax10110Sum
            ["fiscalDriveSumReports.sellOper.tax0Sum"] = 1143,  //--fiscalDriveSumReports.sellOper.tax0Sum
            ["fiscalDriveSumReports.receiptCorrection.receiptCorrectionCount"] = 1144,  //--fiscalDriveSumReports.receiptCorrection.receiptCorrectionCount
            ["fiscalDriveSumReports.receiptCorrection.sellCorrection"] = 1145,  //--fiscalDriveSumReports.receiptCorrection.sellCorrection
            ["fiscalDriveSumReports.receiptCorrection.buyCorrection"] = 1146,  //--fiscalDriveSumReports.receiptCorrection.buyCorrection

            ["fiscalDriveSumReports"] = 1157,
            ["notTransmittedDocumentsSumReports"] = 1158,

            ["items.productCode"] = 1162,
            ["items.productCodeNew"] = 1163,    //--items.productCodeNew

            ["providerPhone"] = 1171,    // items.providerData.providerPhone

            ["correctionType"] = 1173,
            ["сorrectionBase"] = 1174,

            ["сorrectionBase.correctionDocumentDate"] = 1178,   //--сorrectionBase.correctionDocumentDate
            ["сorrectionBase.correctionDocumentNumber"] = 1179,   //--сorrectionBase.correctionDocumentNumber

            ["fiscalDriveSumReports.sellOper.taxFreeSum"] = 1183,   //--fiscalDriveSumReports.sellOper.taxFreeSum

            ["retailPlace"] = 1187,
            ["kktVersion"] = 1188,
            ["documentKktVersion"] = 1189,
            ["documentFdVersion"] = 1190,
            ["items.propertiesItem"] = 1191,    //--items.propertiesItem
            ["propertiesData"] = 1192,
            ["gamblingSign"] = 1193,
            ["shiftSumReports"] = 1194,
            ["items.unit"] = 1197,
            ["items.unitNds"] = 1198,  //--items.unitNds
            ["items.nds"] = 1199,  //--items.nds
            ["items.ndsSum"] = 1200,  //--items.ndsSum
            ["fiscalDriveSumReports.sellOper.totalSum"] = 1201,  //--fiscalDriveSumReports.sellOper.totalSum

            ["operatorInn"] = 1203,

            ["correctionKktReasonCode"] = 1205,
            ["operatorMessage"] = 1206,
            ["exciseDutyProductSign"] = 1207,

            ["fiscalDocumentFormatVer"] = 1209,

            ["items.productType"] = 1212,   //--items.productType
            ["fdKeyResource"] = 1213,
            ["items.paymentType"] = 1214,   //--items.paymentType
            ["prepaidSum"] = 1215,
            ["creditSum"] = 1216,
            ["provisionSum"] = 1217,
            ["fiscalDriveSumReports.sellOper.prepaidSum"] = 1218,   //--fiscalDriveSumReports.sellOper.prepaidSum
            ["fiscalDriveSumReports.sellOper.creditSum"] = 1219,   //--fiscalDriveSumReports.sellOper.creditSum
            ["fiscalDriveSumReports.sellOper.provisionSum"] = 1220,   //--fiscalDriveSumReports.sellOper.provisionSum
            ["printInMachineSign"] = 1221,
            ["items.paymentAgentByProductType"] = 1222,   //--items.paymentAgentByProductType
            ["items.paymentAgentData"] = 1223,   //--items.paymentAgentData
            ["items.providerData"] = 1224,   //--items.providerData
            ["items.providerData.providerName"] = 1225,   //--items.providerData.providerName
            ["items.providerInn"] = 1226,   //--items.providerInn
            ["buyerInformation.buyer"] = 1227,   //--buyerInformation.buyer
            ["buyer"] = 1227,   //--buyer 1.05
            ["buyerInformation.buyerInn"] = 1228,   //--buyerInformation.buyerInn
            ["buyerInn"] = 1228,   //--buyerInn 1.05
            ["items.exciseDuty"] = 1229,   //--items.exciseDuty
            ["items.originCountryCode"] = 1230,   //--items.originCountryCode
            ["items.customEntryNum"] = 1231,   //--items.customEntryNum
            ["fiscalDriveSumReports.receiptCorrection.sellReturnCorrection"] = 1232,   //--fiscalDriveSumReports.receiptCorrection.sellReturnCorrection
            ["fiscalDriveSumReports.receiptCorrection.buyReturnCorrection"] = 1233,   //--fiscalDriveSumReports.receiptCorrection.buyReturnCorrection
            ["allECashProp"] = 1234,    // сведения обо всех оплатах по чеку безналичными
            ["allECashProp.ecashProp"] = 1235,  //сведения об оплате безналичными
            ["allECashProp.ecashProp.ecashSign"] = 1236,    // признак способа оплаты безналичными
            ["allECashProp.ecashProp.ecashIdent"] = 1237,   // Идентификаторы безналичной оплаты
            ["allECashProp.ecashProp.ecashAddInfo"] = 1238, //Дополнительные сведения о безналичной оплате

            ["buyerInformation.buyerBirthday"] = 1243,   //--buyerInformation.buyerBirthday
            ["buyerInformation.buyerCitizenship"] = 1244,   //--buyerInformation.buyerCitizenship
            ["buyerInformation.buyerDocumentCode"] = 1245,   //--buyerInformation.buyerDocumentCode
            ["buyerInformation.buyerDocumentData"] = 1246,   //--buyerInformation.buyerDocumentData

            ["buyerInformation.buyerAddress"] = 1254,   //--buyerInformation.buyerAddress

            ["buyerInformation"] = 1256,

            ["items.itemsIndustryDetails"] = 1260,  //--items.itemsIndustryDetails
            ["industryReceiptDetails"] = 1261,
            ["industryReceiptDetails.idFoiv"] = 1262,   //--industryReceiptDetails.idFoiv
            ["items.itemsIndustryDetails.foundationDocDateTime"] = 1263,   //--items.itemsIndustryDetails.foundationDocDateTime
            ["items.itemsIndustryDetails.foundationDocNumber"] = 1264,   //--items.itemsIndustryDetails.foundationDocNumber
            ["items.itemsIndustryDetails.industryPropValue"] = 1265,   //--items.itemsIndustryDetails.foundationDocNumber

            ["operationalDetails"] = 1270,
            ["operationalDetails.operationId"] = 1271,  //--operationalDetails.operationId
            ["operationalDetails.operationData"] = 1272,  //--operationalDetails.operationData
            ["operationalDetails.dateTime"] = 1273,  //--operationalDetails.dateTime
            ["additionalPropsFRC"] = 1274,
            ["additionalDataFRC"] = 1275,
            ["additionalPropsOS"] = 1276,
            ["additionalDataOS"] = 1277,
            ["additionalPropsCS"] = 1278,
            ["additionalDataCS"] = 1279,
            ["additionalPropsCSR"] = 1280,
            ["additionalDataCSR"] = 1281,
            ["additionalPropsCA"] = 1282,
            ["additionalDataCA"] = 1283,

            ["usageConditionSigns"] = 1290,
            ["items.labeledProdFractionalQuantity"] = 1291,//--items.labeledProdFractionalQuantity
            ["items.labeledProdFractionalQuantity.fractionalPart"] = 1292,//--items.labeledProdFractionalQuantity.fractionalPart
            ["items.labeledProdFractionalQuantity.numerator"] = 1293,//--items.labeledProdFractionalQuantity.numerator
            ["items.labeledProdFractionalQuantity.denominator"] = 1294,//--items.labeledProdFractionalQuantity.denominator

            ["items.productCodeNew.undefined"] = 1300,//--items.productCodeNew.undefined
            ["items.productCodeNew.ean8"] = 1301,//--items.productCodeNew.ean8
            ["items.productCodeNew.ean13"] = 1302,//--items.productCodeNew.ean13
            ["items.productCodeNew.itf14"] = 1303,//--items.productCodeNew.itf14
            ["items.productCodeNew.gs1"] = 1304,//--items.productCodeNew.gs1
            ["items.productCodeNew.gs1m"] = 1305,//--items.productCodeNew.gs1m
            ["items.productCodeNew.kmk"] = 1306,//--items.productCodeNew.kmk
            ["items.productCodeNew.mi"] = 1307,//--items.productCodeNew.mi
            ["items.productCodeNew.egais2"] = 1308,//--items.productCodeNew.egais2
            ["items.productCodeNew.egais3"] = 1309,//--items.productCodeNew.egais3

            ["items.productCodeNew.f1"] = 1320,//--items.productCodeNew.f1
            ["items.productCodeNew.f2"] = 1321,//--items.productCodeNew.f2
            ["items.productCodeNew.f3"] = 1322,//--items.productCodeNew.f3
            ["items.productCodeNew.f4"] = 1323,//--items.productCodeNew.f4
            ["items.productCodeNew.f5"] = 1324,//--items.productCodeNew.f5
            ["items.productCodeNew.f6"] = 1325,//--items.productCodeNew.f6

            ["items.labelCodeProcesMode"] = 2102,//--items.labelCodeProcesMode

            ["undeliveredNotificationsNumber"] = 2104,

            ["items.checkingProdInformationResult"] = 2106,//--items.checkingProdInformationResult
            ["checkingLabeledProdResult"] = 2107,
            ["items.itemsQuantityMeasure"] = 2108,//--items.itemsQuantityMeasure

            ["invalidLabelCodesSign"] = 2112,
            ["invalidRequestsNotificationsSign"] = 2113,

            ["items.controlCode"] = 2115,//--items.controlCode


            // для выгрузок платформы
            ["buyerData"] = 1256,
            ["buyerName"] = 1227,
            ["buyerData.buyerName"] = 1227,
            ["buyerInn"] = 1228,
            ["buyerData.buyerInn"] = 1228,
            ["buyerData.buyerBirthday"] = 1243,   //--buyerInformation.buyerBirthday
            ["buyerData.buyerCitizenship"] = 1244,   //--buyerInformation.buyerCitizenship
            ["buyerData.buyerDocumentCode"] = 1245,   //--buyerInformation.buyerDocumentCode
            ["buyerData.buyerDocumentData"] = 1246,   //--buyerInformation.buyerDocumentData

            ["buyerData.buyerAddress"] = 1254,   //--buyerInformation.buyerAddress
            ["items.measureUnit"] = 2108,
            ["retailAddress"] = 1009,
        };


        public override string ToString()
        {
            if(_tagNumber == 0)
            {
                return "Пустой или неправильный тег";
            }
            return "Tag№_" + _tagNumber + " type_" + _type.ToString() + " fnsName_"+( fnsNames.ContainsKey(_tagNumber)? fnsNames[_tagNumber]:"??") +": " + Representation;
        }
        public string ToString(string format)
        {
            if(format != null && (format.ToLower() == "uf"))
            {
                string s = "("+_tagNumber.ToString() + ") ";
                if (userFrandlyNames.ContainsKey(_tagNumber))
                {
                    if(_type == FDataType.STLV)
                    {
                        if (_tagNumber < 100)
                        {
                            foreach(FTag f in Nested)
                            {
                                if(f.TagNumber == FiscalPrinter.FTAG_FD)
                                {
                                    s += "ФД: " + f.ValueInt+" ";
                                    break;
                                }
                            }

                        }
                        
                        s += userFrandlyNames[_tagNumber];
                        return s;
                    }
                    else
                    {
                        s += userFrandlyNames[_tagNumber] + " "+Representation;
                        return s;
                    }

                }

            }
            
            return ToString();
        }




        public static FTag Empty
        {
            get => new FTag(0,string.Empty,false);
        }

        static public void AddValueTFNCommonRules(TFTagRuleSet ts)
        {
            int key = ts.Format * 65536 + ts.TagNumber;
            if (TFNCommonRules.ContainsKey(key))
            {
                TFNCommonRules.Remove(key);
            }
            TFNCommonRules[key] = ts;
        }
        static public Dictionary<int, TFTagRuleSet> TFNCommonRules = new Dictionary<int, TFTagRuleSet>();
        public class TFTagRuleSet
        {
            public const int RSOURCE_IGNORE = 0;
            public const int RSOURCE_REG_PARAM = 1;
            public const int RSOURCE_OVERRIDE = 2;
            public const int RSOURCE_INCLASS = 3;
            public const int CRITICALITY_CRITICAL = 1;
            public const int CRITICALITY_UNIMPORTANT = 0;
            public TFTagRuleSet(int tagNumber, int format)
            {
                _fdType = tagNumber + 65536 * format;
            }
            int _fdType;
            public int TagNumber { get => _fdType % 65536; }
            public int Format { get => _fdType / 65536; }

            public bool AddRule(int tagNumser, string criticality, string dataSourceStr, string rawdataImg)
            {
                int crit = criticalityName.IndexOf(criticality);
                if (crit < 0)
                {
                    crit = criticalityNameTok.IndexOf(criticality);
                }
                int source = dataSource.IndexOf(dataSourceStr);
                if (source < 0)
                {
                    source = dataSourceTok.IndexOf(dataSourceStr);
                }
                if (tagNumser > 0 && crit >= 0 && source >= 0)
                {
                    Rules.Add(new TFtagRule(tagNumser, crit,source, rawdataImg));
                    return true;
                }
                return false;
            }
            public struct TFtagRule
            {

                public TFtagRule(int tagNumser, int criticality, int dataSource, string rawData)
                {
                    TagNumber = tagNumser;
                    _tagCriticality = criticality;
                    DataSource = dataSource;
                    _defaultRawdata = rawData;
                }
                public int TagNumber;
                int _tagCriticality;
                public int DataSource;
                string _defaultRawdata;
                public int Source { get => DataSource; }
                public string TagCriticalityUF { get => criticalityName[_tagCriticality]; }
                public string TagCriticalityTok { get => criticalityNameTok[_tagCriticality]; }
                public string DataSourceUF { get => dataSource[DataSource]; }
                public string DataSourceTok { get => dataSourceTok[DataSource]; }
                public string DefaultData { get => _defaultRawdata; }
                public bool Critical
                {
                    get => _tagCriticality == CRITICALITY_CRITICAL;
                }

                public static TFtagRule EMPTY_RULE = new TFtagRule(0,0,0,null);
                public static bool IsEmpty(TFtagRule er)
                {
                    return er.TagNumber == 0;
                }
            }
            //  тип правила(STLV.Num + Format * 65536)   -    набор правил
            public List<TFtagRule> Rules = new List<TFtagRule>();

            /*public override bool Equals(object obj)
            {
                if(obj!=null && obj is TFTagRuleSet)
                {
                    TFTagRuleSet obgAsTFTagRuleSet = obj as TFTagRuleSet;
                    return obgAsTFTagRuleSet.TagNumber == _fdType % 65536 && obgAsTFTagRuleSet.Format == _fdType / 65536;
                }
                return false;
            }*/

            public TFtagRule FtagInfo(int tagNumber)
            {
                foreach(var fr in Rules)
                {
                    if(fr.TagNumber == tagNumber)
                        return fr;
                }
                return TFtagRule.EMPTY_RULE;
            }
            public static List<string> criticalityName = new List<string>
            {
                "Продолжить формирование ФД - Не критичный тег",
                "Отмена ФД - Критичный тег",
            };
            public static List<string> criticalityNameTok = new List<string>
            {
                "unimportant",  // 0
                "critical",     // 1
            };

            public static List<string> dataSource = new List<string>
            {
                "Формируется ФНом или необяз.(программа игнорирует)",   // пропускаем тег
                "Из регистрационных параметров",                        // ищем рег параметр если нет проверяем знач по умолчанию
                "Перезапись значения(из данных по умолчанию)",          // оверрайдим внутренний клас или рег при наличии по умолчанию
                "Параметры чека",                                       // внутренний клас если нет проверяем знач по умолчанию
            };
            public static List<string> dataSourceTok = new List<string>
            {
                "ignore",
                "reg_param",
                "override",
                "internal_class",
            };
        }

        public static void SaveTFNRules()
        {
            XmlDocument xDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = xDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xDoc.AppendChild(xmlDeclaration);
            XmlElement xRoot = xDoc.CreateElement("fdstruct");
            xDoc.AppendChild(xRoot);
            XmlElement ffd2 = xDoc.CreateElement("ffd_2");
            XmlElement ffd3 = xDoc.CreateElement("ffd_3");
            XmlElement ffd4 = xDoc.CreateElement("ffd_4");
            xRoot.AppendChild(ffd2);
            xRoot.AppendChild(ffd3);
            xRoot.AppendChild(ffd4);
            foreach(var key in TFNCommonRules.Keys)
            {
                XmlElement ffd = null;
                if (key / 65536 == 2)
                {
                    ffd = ffd2;
                }
                else if(key / 65536 == 3)
                {
                    ffd = ffd3;
                }
                else if(key / 65536 == 4)
                {
                    ffd = ffd4;
                }
                if (ffd != null)
                {
                    var fdFormRulesSet = TFNCommonRules[key];
                    XmlElement ffdForm = xDoc.CreateElement("stlv_form");
                    ffd.AppendChild(ffdForm);
                    XmlAttribute formCode = xDoc.CreateAttribute("code");
                    formCode.Value = fdFormRulesSet.TagNumber.ToString();
                    ffdForm.Attributes.Append(formCode);
                    foreach (var tag in fdFormRulesSet.Rules)
                    {
                        XmlElement ftag = xDoc.CreateElement("tag");
                        ffdForm.AppendChild(ftag);
                        XmlAttribute tNum = xDoc.CreateAttribute("number");
                        tNum.Value = tag.TagNumber.ToString();
                        ftag.Attributes.Append(tNum);
                        XmlAttribute onRefuse = xDoc.CreateAttribute("onrefuse");
                        onRefuse.Value = tag.TagCriticalityTok;
                        ftag.Attributes.Append(onRefuse);
                        XmlAttribute dataSource = xDoc.CreateAttribute("source");
                        dataSource.Value = tag.DataSourceTok;
                        ftag.Attributes.Append(dataSource);
                        if (!string.IsNullOrEmpty(tag.DefaultData))
                        {
                            XmlText text = xDoc.CreateTextNode(tag.DefaultData);
                            ftag.AppendChild(text);
                        }
                    }

                }

            }
            xDoc.Save("terminalfn_ffdstructure.xml");

        }

        public static void LoadTFNRules()
        {
            LogHandle.ol("Load fn rules...");
            int errors = 0;
            int loaded = 0;
            if (File.Exists("terminalfn_ffdstructure.xml"))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("terminalfn_ffdstructure.xml");
                XmlElement xRoot = xDoc.DocumentElement;
                if (xRoot != null)
                {
                    foreach (XmlElement ffd in xRoot)
                    {
                        int formatFd = 0;
                        if (ffd.Name == "ffd_2")
                        {
                            formatFd = 2;
                        }
                        else if (ffd.Name == "ffd_3")
                        {
                            formatFd = 3;
                        }
                        else if (ffd.Name == "ffd_4")
                        {
                            formatFd = 4;
                        }
                        if (formatFd > 0)
                        {
                            foreach (var xmlFfdChild in ffd.ChildNodes)
                            {
                                if (xmlFfdChild is XmlElement && (xmlFfdChild as XmlElement).Name == "stlv_form")
                                {
                                    XmlElement stlvForm = xmlFfdChild as XmlElement;
                                    var tagCodemStr = stlvForm.GetAttribute("code");
                                    int tCode = -1;
                                    int.TryParse(tagCodemStr, out tCode);
                                    if (tCode > 0)
                                    {
                                        TFTagRuleSet tfnCode = new TFTagRuleSet(tCode, formatFd);
                                        foreach (var tlvElrm in stlvForm.ChildNodes)
                                        {
                                            if (tlvElrm is XmlElement && (tlvElrm as XmlElement).Name == "tag")
                                            {
                                                XmlElement xtlvStruct = tlvElrm as XmlElement;
                                                string tNumStr = xtlvStruct.GetAttribute("number");
                                                string tCritStr = xtlvStruct.GetAttribute("onrefuse");
                                                string tSourceStr = xtlvStruct.GetAttribute("source");
                                                string tOverNum = xtlvStruct.InnerText;
                                                int tagNum = -1;
                                                int.TryParse(tNumStr, out tagNum);
                                                if (tagNum > 0)
                                                {
                                                    if (tfnCode.AddRule(tagNum, tCritStr, tSourceStr, tOverNum))
                                                    { loaded++; }
                                                    else
                                                    {
                                                        errors++;
                                                    }

                                                }
                                            }
                                        }
                                        TFNCommonRules[formatFd * 65536 + tfnCode.TagNumber] = tfnCode;


                                    }

                                }
                            }
                        }
                    }
                }

                LogHandle.ol("Правил загужено: " + loaded + " ошибок при загрузке: " + errors);
            }
            else
            {
                LogHandle.ol("Нет файла с правилами формирования ФД terminalfn_ffdstructure.xml");
            }
        }

        
        public static bool Repeatable(int tagNumber)
        {
            if (tagNumber == FiscalPrinter.FTAG_ITEM 
                || tagNumber == FiscalPrinter.FTAG_AMOUNTS_NDS)
            {
                return true;
            }

            return false;
        }

        /*
         *  Блок для редактирования тегов в интерфейсе
         *  
         *  
         *  _tryCreateNumber
         *  для сохранения номера тега после очистки данных
         *  
         *  _recreated
         *  0 - не пересоздавался, 
         *  1 - пересоздан без ошибок,
         * -1 - не удалось пересоздать
         */
        int _recreated = 0;
        int _tryCreateNumber = 0;
        public int ChangeIntValue(int newValue)
        {
            if(_tagNumber == FiscalPrinter.FTAG_OPERATION_TYPE && newValue > 0 && newValue <= 4)
            {
                _valueInt = Convert.ToUInt32(newValue);
                _recreated = 1;
                return 1;
            }
            if( _tagNumber == 4 || _tagNumber == 4 || _tagNumber ==31 || _tagNumber == 41 )
            {
                if( newValue == 4 || newValue == 4 || newValue == 31 || newValue == 41 )
                {
                    _tagNumber = newValue;
                    _recreated = 1;
                    return 1;
                }
                else
                {
                    _recreated = -1;
                    return -1;
                }
            }
            _recreated = -1;
            return -1;
        }
        public int TryCreateNumber { get { return _tryCreateNumber; } }
        public void ClearFTag()
        {
            if(_tagNumber>0)
            {
                var f = EmptyNumerredFtag(_tagNumber);
                _tryCreateNumber = _tagNumber;
                _recreated = -1;
                _recreatingError = "";
                _tagNumber = f._tagNumber;
                _type = f._type;
                _rawData = f._rawData;
                _valueStr = f._valueStr;
                _valueInt = f._valueInt;
                _valueLong = f._valueLong;
                _valueDouble = f._valueDouble;
                _valueDateTime = f._valueDateTime;
                _nested = f._nested;
                Representation = f.Representation;
                return;
            }
            if (_tryCreateNumber > 0)
            {
                var f = EmptyNumerredFtag(_tryCreateNumber);
                _recreated = -1;
                _recreatingError = "";
                _tagNumber = f._tagNumber;
                _type = f._type;
                _rawData = f._rawData;
                _valueStr = f._valueStr;
                _valueInt = f._valueInt;
                _valueLong = f._valueLong;
                _valueDouble = f._valueDouble;
                _valueDateTime = f._valueDateTime;
                _nested = f._nested;
                Representation = f.Representation;
                return;
            }
        }
        public int Recreated { get => _recreated; }
        string _recreatingError = "";
        public string RecreatingError { get => _recreatingError; }
        public void Rebuild(object newValue)
        {
            if (_tagNumber == 0 && _tryCreateNumber == 0)
            {
                _recreated = 0;
                return;
            }
            FTag f = null;

            // для STLV возможно нужно разветвление
            try
            {
                f = new FTag(_tagNumber==0?_tryCreateNumber:_tagNumber, newValue, _rawData != null);
            }
            catch(Exception ex)
            {
                _recreatingError = ex.Message;
            }
            if (f != null && f.TagNumber != 0)
            {
                _recreated = 1;
                _recreatingError = "";
                _tagNumber = f._tagNumber;
                _type = f._type;
                _rawData = f._rawData;
                _valueStr = f._valueStr;
                _valueInt = f._valueInt;
                _valueLong = f._valueLong;
                _valueDouble = f._valueDouble;
                _valueDateTime = f._valueDateTime;
                _nested = f._nested;
                Representation = f.Representation;
                return;
            }
            else
            {
                ClearFTag();
            }
                //_recreated = -1;
            return;
        }

        public static FTag EmptyNumerredFtag(int tagNumber)
        {
            FTag f = Empty;
            f._tryCreateNumber = tagNumber;
            return f;
        }


        public class FTLVParcer
        {
            public static byte[] StringHexToBytes(string reprtezentation)
            {
                List<byte> bytes = new List<byte>();
                if (string.IsNullOrEmpty(reprtezentation)) goto FormatException;
                reprtezentation = reprtezentation.Trim();
                //char[] separators = new char[] { '\r', '\n' };
                reprtezentation = reprtezentation.Replace(Environment.NewLine, "");
                int i = 0;
                while (i < reprtezentation.Length)
                {
                    int num = 0;
                    char c = reprtezentation[i++];
                    if (c >= '0' && c <= '9') num = 16 * (c - '0');
                    else if (c >= 'A' && c <= 'F') num = 16 * (c - 'A' + 10);
                    else if (c >= 'a' && c <= 'f') num = 16 * (c - 'a' + 10);
                    else goto FormatException;
                    if (i == reprtezentation.Length) goto FormatException;
                    c = reprtezentation[i++];
                    if (c >= '0' && c <= '9') num += c - '0';
                    else if (c >= 'A' && c <= 'F') num += c - 'A' + 10;
                    else if (c >= 'a' && c <= 'f') num += c - 'a' + 10;
                    else goto FormatException;
                    if (i != reprtezentation.Length)
                    {
                        c = reprtezentation[i++];
                        if (!char.IsWhiteSpace(c) && c != '-') goto FormatException;
                    }
                    bytes.Add((byte)num);
                }
                return bytes.ToArray();

            FormatException:
                LogHandle.ol("unable transform string to byte array");
                return new byte[0];
            }

            public static List<FTag> ParseStructure(byte[] data)
            {
                List<FTag> list = new List<FTag>();
                if (data.Length > 4)
                {
                    int i = 0;
                    while (i < data.Length)
                    {
                        if (i + 3 >= data.Length)
                        {
                            LogHandle.ol("data integrity is broken no paire tn+s");
                            break;
                        }

                        int tagNumber = data[i++] + data[i++] * 256;

                        int tagSize = 0;
                        tagSize = data[i++] + data[i++] * 256;

                        byte[] bytes;
                        if (i + tagSize > data.Length)
                        {
                            LogHandle.ol("tag integrity is broken TAG:" + tagNumber + " size: " + tagSize + " raw data:" + BitConverter.ToString(data));
                            tagNumber = 0;
                            bytes = new byte[data.Length - i];
                            Array.Copy(data, i, bytes, 0, data.Length - i);
                        }
                        else
                        {
                            bytes = new byte[tagSize];
                            Array.Copy(data, i, bytes, 0, tagSize);
                        }
                        list.Add(new FTag(tagNumber, bytes));
                        i += tagSize;
                    }
                }
                return list;
            }

        }


    }
}
