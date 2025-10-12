using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace FR_Operator
{
    public partial class AppSettings : Form
    {

        public AppSettings()
        {
            InitializeComponent();

            if(_usingCustomSno) radioButton_cs_usingSno.Checked = true;
            else radioButton_cs_dontUsingSno.Checked = true;
            checkBox_atolUsePropertyData.Checked = _atolUsePropertyData;
            checkBox_setFiscalSignAsPropertyData.Checked = _appendFiscalSignAsPropertyData;
            radioButton_overrideOriginalPropertyData.Enabled = _appendFiscalSignAsPropertyData;
            radioButton_saveOriginalPropertyData.Enabled = _appendFiscalSignAsPropertyData;
            textBox_correctionOrderNumberDefault.Text = _correctionOrderNumberDefault;
            if (_overridePropertyData)
                radioButton_overrideOriginalPropertyData.Checked = true;
            else
                radioButton_saveOriginalPropertyData.Checked = true;
            if(_overrideCorrectionDocumentDate)
                radioButton_correctionDocumentDateOveeride.Checked = true;
            else
                radioButton_correctionDocumentDateSaveOriginal.Checked = true;
            if(_overrideCorrectionOrderNumber)
                radioButton_correctionOrderNumberOverride.Checked = true;
            else
                radioButton_correctionOrderNumberSaveOriginal.Checked = true;
            comboBox_settingsItemProductTypeDefault.SelectedIndex = _itemProductType;
            comboBox_settingsItemPaymentTypeDefault.SelectedIndex = _itemPaymentType;
            comboBox_settingsItemTaxRateDefault.SelectedIndex = _itemTaxRate;
            textBox_itemNameDefault.Text = string.IsNullOrEmpty(_itemName)?"":_itemName;
            if(_itemPrice>0) 
                textBox_itemPriceDefault.Text = _itemPrice.ToString();
            if(_itemQuantity>0) 
                textBox_itemQuantityDefault.Text = _itemQuantity.ToString();
            checkBox_atolAppendMeasure120.Checked = _autoUnit120SetZero;
            comboBox_copayIntFd.SelectedIndex = (int)CoPayInterfaceDoc;
            textBox_emuDelay.Text = _emulatorDelay.ToString();
            checkBox_shtrihPrintPropertyData.Checked = _shtrihPrintPropertyData;
            textBox_infoPrefix.Text = _extendedTextInfoPrefix;
            comboBox_indentBefore.SelectedIndex = _extendedTextInfoOffseBefore;
            comboBox_indentAfter.SelectedIndex = _extendedTextInfoOffseAfter;
            comboBox_infiStr1.SelectedIndex = _extendedTextInfoStrFormat[0];
            comboBox_infiStr2.SelectedIndex = _extendedTextInfoStrFormat[1];
            comboBox_infiStr3.SelectedIndex = _extendedTextInfoStrFormat[2];
            textBox_atolFontForPrinting.Text = _atolFontForPrinting.ToString();
            checkBox_fillItemPaymetTypeDef.Checked = _atolFillItemsPaymentTypeDefault4;
            textBox_shtrihFontForPrinting.Text = _shtrihFomtForPrinting.ToString();
            comboBox_cleanAfterPrint.SelectedIndex = _extendedTextInfoCleanAfterPrint ? 0 : 1;
            comboBox_terminalFnComPrefer.SelectedIndex = _terminalPreferPort;
            textBox_cashierDefault.Text = _cashierDefault;
            textBox_cashierInnDefault.Text = _cashierInnDefault;
            comboBox_tetminalFn1115Method.SelectedIndex = _terminalFnTag1115Filling;
            checkBox_terminalSkipItemsInCoorectionFfd2.Checked = _tfnSkipItemsInCorrectionFfd2;
            checkBox_correctionOrderNumber_CorrectionOrderExistance.Checked = _corretion_order_number_existance == 1;
            checkBox_ovverrideRetailAddress.Checked = _overideRetailAddress;
            checkBox_ovverrideRetailPlace.Checked = _overideRetailPlace;

            if (_shtrihCloseCheckMethod == 0)
                radioButton_shtrihCloseCheqMethodOld.Checked = true;
            else if(_shtrihCloseCheckMethod == 1)
            {
                radioButton_shtrihCloseCheckEx2.Checked = true;
            }
            else if(_shtrihCloseCheckMethod == 2)
            {
                radioButton_shtrihCloseCheckEx3.Checked = true;
            }

            if(_shtrihRegisterItemMethod == 0)
            {
                radioButton_shtrihRegRuleMain.Checked = true;
            }
            else if(_shtrihRegisterItemMethod == 1)
            {
                radioButton_shtrihRegRuleOld.Checked = true;
            }

            foreach(var header in _jsonHeaders)
            {
                listBox_jsonHeaders.Items.Add(header);
            }

            _skipProcessing = false;

            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add("Копировать ссылку");
            cm.MenuItems[0].Click += linkLabel_copyLink;
            linkLabel_doc.ContextMenu = cm;

        }

        FiscalPrinter _fr = null;
        public FiscalPrinter FR 
        {
            set 
            {
                skipControlDev = true;
                _fr = value; 
                if( _fr == null || !(_fr is ShtrihAdapter) || (_fr is ShtrihAdapter && !_fr.IsConnected) ) 
                { 
                    groupBox_shtrihDeviceSettings.Enabled = false;
                    label_frShtrihStatus.Text = "Нет соединения с ККТ.";
                }
                else if (_fr != null && (_fr is ShtrihAdapter) && _fr.IsConnected)
                {
                    ShtrihAdapter shtrih = (ShtrihAdapter)_fr;
                    groupBox_shtrihDeviceSettings.Enabled = true;
                    label_frShtrihStatus.Text = "Соединение установлено.";
                    _skipProcessing = true;
                    int taxesRule = shtrih.TaxesFillsFirmware;
                    if(taxesRule == 0||taxesRule == 2||taxesRule == 3)
                    {
                        radioButton_shtrihSwichFwTaxesOn.Checked = true;
                    }
                    else if(taxesRule == 1)
                    {
                        radioButton_shtrihSwichFwTaxesOff.Checked = true;
                    }

                    int frintOff = shtrih.EnablingPrinting;
                    if(frintOff == 2)
                    {
                        radioButton_shtrihSwichPrintingOff.Checked = true;
                    }
                    else if(frintOff == 0)
                    {
                        radioButton_shtrihSwichPrintingOn.Checked = true;
                    }
                    _skipProcessing = false;
                }
                skipControlDev = false;
            } 
        }

        private void linkLabel_copyLink(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenu cm = item.GetContextMenu();
            Control parent = cm.SourceControl;

            if (parent == linkLabel_doc)
            {
                Clipboard.SetText("https://docs.google.com/document/d/1iyOS7q8_ULj-dHfb7LYhAu0a19XmvymBdfMWLfGu1Xw/edit?usp=sharing");
            }
            
        }


        private static List<string> _jsonHeaders = new List<string>();
        private const string JSON_PATH = "js.path=";
        public static List<string> JsonHeaders { get => _jsonHeaders; }
        //!!  Атолы !! 
        /*
         * 
         * все поля для работы с атолами
         */

        public static bool AtolAbility = true;
        private static int _atolFontForPrinting = 1;
        private const string ATOL_FONT_FOR_PRINTING = "atol_font_for_printing=";
        private static int _shtrihFomtForPrinting = 1;

        private static bool _atolFillItemsPaymentTypeDefault4 = true;
        public static bool AtolFillItemsPaymentTypeDefault
        {
            get => _atolFillItemsPaymentTypeDefault4;
        }
        public const string ATOL_FILL_ITEMS_PT_DEF = "Atol_fill_itemsPaymentTypeDefault4=";
        public static int AtolFontForPrinting
        {
            get => _atolFontForPrinting;
        }
        public static int ShtrihFontForPrinting
        {
            get => _shtrihFomtForPrinting;
        }

        public static bool AtolUseCorrectionDescriber
        {
            get => _atolUseCorrectionDescriber;
            set
            {
                _atolUseCorrectionDescriber = value;
                SaveSettings();
            }
        }
        private static bool _atolUseCorrectionDescriber = false;
        private const string ATOL_USE_PROPERTY_DATA_KEY = "AtolUsePropertyData=true";
        private const string ATOL_DONT_USE_PROPERTY_DATA_KEY = "AtolUsePropertyData=false";
        private static bool _atolUsePropertyData = true;
        public static bool AtolUsePropertyData
        {
            get => _atolUsePropertyData;
            set
            {
                _atolUsePropertyData = value;
                SaveSettings();
            }
        }
        public static bool AutoUnit120SetZero
        {
            get => _autoUnit120SetZero;
            set => _autoUnit120SetZero = value;
        }
        private static bool _autoUnit120SetZero = true;
        private const string AUTO_UNIT120_SET_ZERO_KEY = "Atol_items.unit120_setZero=true";
        private const string UNIT120_SAVE_EMPTY_KEY = "Atol_items.unit120_setZero=false";


        // !! TerminalFn !!
        /*
         * предпочитаемый порт
         * 0  - первый
         * 1  - последний
         * 
         * >1 номер порта или последний доделать
         */
        private static int _terminalPreferPort = 0;
        public static int TerminalPreferPort
        {
            get => _terminalPreferPort;
        }

        // заполнение тега 1115
        // 0 - все суммы, 1 - только ненулевые
        private const string TFN_NDS_AMOUNTS_FILL_METHOD = "terminalfn_nds_amounts_fill_method=";
        private static int _terminalFnTag1115Filling = 1;
        public static int TFNFillTagNdsAmountsMethod
        {
            get => _terminalFnTag1115Filling;
        }
        // items в ЧК 1.05
        private static bool _tfnSkipItemsInCorrectionFfd2 = true;
        public static bool TFN_SkipItemsInCoorectionFfd2
        {
            get => _tfnSkipItemsInCorrectionFfd2;
        }
        private const string TFN_SKIP_ITEMS_IN_CORRECTION_FFD2 = "terminalfn_skip_items_in_corrections_ffd2=";

        private static int _terminalFnLogLevel = 0;
        private static int _terminalFnLogFormat = 1;
        public static int TerminalFnLogLevel 
        { 
            get => _terminalFnLogLevel;
            set { _terminalFnLogLevel = value; SaveSettings(); }
        }
        public static int TerminalFnLogFormat
        {
            get => _terminalFnLogFormat;
            set { _terminalFnLogFormat = value;  SaveSettings(); }
        }
        private const string TERMINALFN_LOG_LEVEL = "terminalfn_log_level=";
        private const string TERMINALFN_LOG_FORMAT = "terminalfn_log_format=";
        private const string TERMINALFN_PREFER_PORT = "terminalfn_prefer_port=";

        static private DateTime _minAvailableFdTime = new DateTime(2024, 01, 01);
        static public DateTime MinAvailableFdTimeFn
        {
            get => _minAvailableFdTime;
        }
        private const string TERMINALFN_MIN_FDTIME = "min_allowed_fd_time=";

        // ШТРИХИ
        /*
         * Все настройки  Штрих|Poscenter
         */
        // печать информации о корректируемом ФД
        private const string SHTRIH_PRINT_PROPERTY_DATA_KEY ="Shtrih_Print_PropertyData=true";
        private static bool _shtrihPrintPropertyData = false;
        public static bool ShtrihPrintPropertyData {get => _shtrihPrintPropertyData;}
        // регистрация позиции
        private const string SHTRIH_REGISTER_ITEM = "ShtrihRegisterItemMethod=";
        private static int _shtrihRegisterItemMethod = 0;
        public static int ShtrihRegisterItemMethod { get => _shtrihRegisterItemMethod;}
        // разблокировка старого драйвера
        private const string SHTRIH_IGNORE_OLD_DRIVER = "Shtrih_debug_ignore_old_driver=true";
        private static bool _shtrihIgnoreOldDriver = false;
        public static bool ShtrihIgnoreOldDriver { get => _shtrihIgnoreOldDriver; }
        // мктод закрытия чеков
        private const string SHTRIH_CLOSE_CHECK_METHOD = "Shtrih_close_check_method=";
        static int _shtrihCloseCheckMethod = 1;
        public static int ShtrihCloseCheckMethod { get => _shtrihCloseCheckMethod;}

        private const string SHTRIH_FONT_FOR_PRINTING = "shtrih_font_for_printing=";

        // COMMON

        // Заполнять номер предписания в зависимости от типа коррекции
        // 1 только в коррекциях по предписанию; 0 всегда если не пустой
        private const string CORRECTION_ORDER_NUMBER_TYPE_DEPENDACE = "order_number_existance=";
        private static int _corretion_order_number_existance = 1;
        public static int CorrectionOrderExistance
        {
            get => _corretion_order_number_existance;
        }

        private const string SEND_BI_DOC_INFO_IGNORE_BUYER_INN = "send_bi_doc_info_regardless_inn=";
        private static bool _always_send_buyer_doc_data = false;
        public static bool AlwaysSendBuyerDocData
        {
            get => _always_send_buyer_doc_data;
            set  
            {
                _always_send_buyer_doc_data = value;
                SaveSettings();
            }
        }

        private const string EXTENDED_TEXT_INFO_PREFIX = "ext_text_prefix=";
        private static string _extendedTextInfoPrefix = "Информация о корректируемом документе";
        
        private const string EXTENDED_TEXT_INFO_OFFSET_BEFORE = "ext_text_offset_before=";
        private static int _extendedTextInfoOffseBefore = 0;
        private const string EXTENDED_TEXT_INFO_OFFSET_AFTER = "ext_text_offset_after=";
        private static int _extendedTextInfoOffseAfter = 0;

        private static int[] _extendedTextInfoStrFormat = new int[3];
        private const string EXTENDED_TEXT_INFO_STRINGS = "ext_text_str_format";
        private const int EXTENDED_TEXT_INFO_FORMATS = 5;
        public static int ExtendedInfoTopOffset { get => _extendedTextInfoOffseBefore;}
        public static int ExtendedInfoBottomOffset { get => _extendedTextInfoOffseAfter;}

        private static bool _extendedTextInfoCleanAfterPrint = true;
        private const string EXT_TXT_INFO_DONT_CLEAN_AFTER_PRINT = "ext_text_clean_after_print=false";
        public static bool ExtendedInfoCleanAfterPrint { get => _extendedTextInfoCleanAfterPrint;}

        public static bool jsonAvailable = true;

        public static bool PrintExtendedTextInfo
        {
            get
            {
                return _extendedTextInfoStrFormat[0] + _extendedTextInfoStrFormat[1] + _extendedTextInfoStrFormat[2] > 0;
            }
        }
        public static List<string> ExtendedTextInfo(FiscalPrinter.FnReadedDocument fd)
        {
            List<string> stringsForPrinting = new List<string>();
            if (PrintExtendedTextInfo)
            {
                if(!string.IsNullOrEmpty(_extendedTextInfoPrefix))
                    stringsForPrinting.Add(_extendedTextInfoPrefix);
                foreach (int x in _extendedTextInfoStrFormat)
                    if (x > 0)
                        stringsForPrinting.Add(_FdInfo(fd, x));
            }
            return stringsForPrinting;
        }
        static string _FdInfo(FiscalPrinter.FnReadedDocument fd, int type)
        {
            switch (type)
            {
                case 1:
                    return "ФД:" + fd.Number + "    ФП:" + fd.FiscalSign;
                case 2:
                    return "Дата: " + fd.Time.ToString("dd.MM.yyyy  HH:mm");
                case 3:
                    return "ФД:" + fd.Number.ToString() + " ФП:" + fd.FiscalSign + " " + fd.Time.ToString("dd.MM.yy");
                case 4:
                    if (fd.Cheque != null  && fd.Cheque.IsPropertiesData)
                    {
                        if(fd.Cheque.PropertiesData != fd.FiscalSign)
                            return "ФД содержит 1192=" + fd.Cheque.PropertiesData;
                        else
                            return "Не содержал 1192, в него добавлен ФП";
                    }
                    return "ФД не содержит 1192";
                case 5:
                    return fd.ReeprezentOL;
                case 6:
                    return "ФП: " + fd.FiscalSign;
                default:
                    return "";
            }
        }


        public static void LoadSettings()
        {
            LogHandle.ol("Загружаем настройки");
            try
            {
                if (File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileSettings)))
                {
                    string parameter = null;
                    using (StreamReader sr = new StreamReader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileSettings), Encoding.UTF8))
                    {
                        while ((parameter = sr.ReadLine()) != null)
                        {
                            if (parameter.StartsWith("#[") || parameter.Trim() == "")
                                continue;

                            if (parameter == USING_CUSTOM_SNO_KEY)
                            {
                                _usingCustomSno = true;
                            }
                            else if (parameter.StartsWith(CASHIER_DEFAULT_KEY))
                            {
                                _cashierDefault = parameter.Substring(CASHIER_DEFAULT_KEY.Length);
                            }
                            else if (parameter.StartsWith(CASHIER_INN_DEFAULT_KEY))
                            {
                                string inn = parameter.Substring(CASHIER_INN_DEFAULT_KEY.Length);
                                if (FiscalPrinter.CorrectInn(inn))
                                {
                                    _cashierInnDefault = inn;
                                }
                                else
                                {
                                    _cashierInnDefault = "";
                                }
                            }
                            else if (parameter == DONT_USING_CUSTOM_SNO_KEY)
                            {
                                _usingCustomSno = false;
                            }
                            else if (parameter.StartsWith(ATOL_FILL_ITEMS_PT_DEF))
                            {
                                _atolFillItemsPaymentTypeDefault4 = parameter.Substring(ATOL_FILL_ITEMS_PT_DEF.Length).ToLower() == "true";
                            }
                            else if (parameter == ATOL_USE_PROPERTY_DATA_KEY)
                            {
                                _atolUsePropertyData = true;
                            }
                            else if (parameter == ATOL_DONT_USE_PROPERTY_DATA_KEY)
                            {
                                _atolUsePropertyData = false;
                            }
                            else if (parameter.StartsWith(ATOL_FONT_FOR_PRINTING))
                            {
                                int font = 0;
                                int.TryParse(parameter.Substring(ATOL_FONT_FOR_PRINTING.Length), out font);
                                _atolFontForPrinting = font;
                            }
                            else if (parameter == APPEND_FISCAL_SIGN_AS_PROPERTY_DATA_KEY)
                            {
                                _appendFiscalSignAsPropertyData = true;
                            }
                            else if (parameter.StartsWith(OVERRIDE_PROPERTY_DATA_VALUE_KEY))
                            {
                                _overridePropertyData = parameter.EndsWith("true");
                            }
                            else if (parameter.StartsWith(CORRECTION_ORDER_NUMBER_DEFAULT_KEY))
                            {
                                _correctionOrderNumberDefault = parameter.Substring(CORRECTION_ORDER_NUMBER_DEFAULT_KEY.Length);
                            }
                            else if (parameter.StartsWith(OVERRIDER_CORRECTION_ORDER_NUMBER_KEY))
                            {
                                _overrideCorrectionOrderNumber = parameter.EndsWith("true");
                            }
                            else if (parameter.StartsWith(OVERRIDE_CORRECTION_DOCUMENT_DATE_KEY))
                            {
                                _overrideCorrectionDocumentDate = parameter.EndsWith("true");
                            }
                            else if (parameter.StartsWith(ITEM_PRODUCT_TYPE_DEFAULT_KEY))
                            {
                                _itemProductType = int.Parse(parameter.Substring(ITEM_PRODUCT_TYPE_DEFAULT_KEY.Length));
                            }
                            else if (parameter.StartsWith(ITEM_PAYMENT_TYPE_DEFAULT_KEY))
                            {
                                _itemPaymentType = int.Parse((string)parameter.Substring(ITEM_PAYMENT_TYPE_DEFAULT_KEY.Length));
                            }
                            else if (parameter.StartsWith(ITEM_TAX_RATE_DEFAULT_KEY))
                            {
                                _itemTaxRate = int.Parse((string)parameter.Substring(ITEM_TAX_RATE_DEFAULT_KEY.Length));
                            }
                            else if (parameter.StartsWith(ITEM_NAME_DEFAULT_KEY))
                            {
                                _itemName = parameter.Substring(ITEM_NAME_DEFAULT_KEY.Length);
                            }
                            else if (parameter.StartsWith(ITEM_PRICE_DEFAULT_KEY))
                            {
                                _itemPrice = double.Parse(parameter.Substring(ITEM_PRICE_DEFAULT_KEY.Length));
                            }
                            else if (parameter.StartsWith(ITEM_QUANTITY_DEFAULT_KEY))
                            {
                                _itemQuantity = double.Parse(parameter.Substring(ITEM_QUANTITY_DEFAULT_KEY.Length));
                            }
                            else if (parameter.StartsWith(CO_PAY_INTERFACE_DOC_KEY))
                            {
                                if (parameter.EndsWith(CO_PAY_OFF))
                                    CoPayInterfaceDoc = CoPayMethods.CO_PAY_OFF;
                                else if (parameter.EndsWith(CO_PAY_ECASH))
                                    CoPayInterfaceDoc = CoPayMethods.CO_PAY_ECASH;
                                else if (parameter.EndsWith(CO_PAY_PREPAID))
                                    CoPayInterfaceDoc = CoPayMethods.CO_PAY_PREPAID;

                                else if (parameter.EndsWith(CO_PAY_CREDIT))
                                    CoPayInterfaceDoc = CoPayMethods.CO_PAY_CREDIT;
                                else if (parameter.EndsWith(CO_PAY_PROVISION))
                                    CoPayInterfaceDoc = CoPayMethods.CO_PAY_PROVISION;
                                else
                                    CoPayInterfaceDoc = CoPayMethods.CO_PAY_CASH;

                            }
                            else if (parameter.StartsWith(SEND_BI_DOC_INFO_IGNORE_BUYER_INN))
                            {
                                string s = parameter.Substring(SEND_BI_DOC_INFO_IGNORE_BUYER_INN.Length);
                                _always_send_buyer_doc_data = s == "true";
                            }
                            else if (parameter.Equals(AUTO_UNIT120_SET_ZERO_KEY))
                            {
                                _autoUnit120SetZero = true;
                            }
                            else if (parameter.Equals(UNIT120_SAVE_EMPTY_KEY))
                            {
                                _autoUnit120SetZero = false;
                            }
                            else if (parameter.StartsWith(EMULATOR_DELAY))
                            {
                                try
                                {
                                    _emulatorDelay = Convert.ToInt32(parameter.Substring(EMULATOR_DELAY.Length));
                                }
                                catch { }
                            }
                            else if (parameter.StartsWith(SHTRIH_REGISTER_ITEM))
                            {
                                int method = 0;
                                int.TryParse(parameter.Substring(SHTRIH_REGISTER_ITEM.Length), out method);
                                if (method == 1)
                                {
                                    _shtrihRegisterItemMethod = method;
                                }
                            }
                            else if (parameter == SHTRIH_PRINT_PROPERTY_DATA_KEY)
                            {
                                _shtrihPrintPropertyData = true;
                            }
                            else if (parameter.StartsWith(SHTRIH_CLOSE_CHECK_METHOD))
                            {
                                uint method = 1;
                                uint.TryParse(parameter.Substring(SHTRIH_CLOSE_CHECK_METHOD.Length), out method);
                                if (method <= 3)
                                    _shtrihCloseCheckMethod = (int)method;
                            }
                            else if (parameter.StartsWith(SHTRIH_FONT_FOR_PRINTING))
                            {
                                int font = 1;
                                int.TryParse(parameter.Substring(SHTRIH_FONT_FOR_PRINTING.Length), out font);
                                _shtrihFomtForPrinting = font;
                            }
                            else if (parameter == SHTRIH_IGNORE_OLD_DRIVER)
                            {
                                _shtrihIgnoreOldDriver = true;
                            }
                            else if (parameter == EXTENDED_TEXT_INFO_PREFIX)
                            {
                                _extendedTextInfoPrefix = parameter.Substring(EXTENDED_TEXT_INFO_PREFIX.Length);
                            }
                            else if (parameter.StartsWith(EXTENDED_TEXT_INFO_OFFSET_BEFORE))
                            {
                                int offset = 0;
                                int.TryParse(parameter.Substring(EXTENDED_TEXT_INFO_OFFSET_BEFORE.Length), out offset);
                                if (offset < 0)
                                    offset = 0;
                                if (offset > 5)
                                    offset = 5;
                                _extendedTextInfoOffseBefore = offset;
                            }
                            else if (parameter.StartsWith(EXTENDED_TEXT_INFO_OFFSET_AFTER))
                            {
                                int offset = 0;
                                int.TryParse(parameter.Substring(EXTENDED_TEXT_INFO_OFFSET_AFTER.Length), out offset);
                                if (offset < 0)
                                    offset = 0;
                                if (offset > 5)
                                    offset = 5;
                                _extendedTextInfoOffseAfter = offset;
                            }
                            else if (parameter.StartsWith(EXTENDED_TEXT_INFO_STRINGS))
                            {
                                int n = parameter[EXTENDED_TEXT_INFO_STRINGS.Length] - '0';
                                int k = 0;
                                if (n >= 0 && n < 3)
                                    int.TryParse(parameter.Substring(EXTENDED_TEXT_INFO_STRINGS.Length + 2), out k);
                                if (k > 0 && k <= EXTENDED_TEXT_INFO_FORMATS)
                                {
                                    _extendedTextInfoStrFormat[n] = k;
                                }
                            }
                            else if (parameter == EXT_TXT_INFO_DONT_CLEAN_AFTER_PRINT)
                            {
                                _extendedTextInfoCleanAfterPrint = false;
                            }
                            else if (parameter.StartsWith(JSON_PATH))
                            {
                                _jsonHeaders.Add(parameter.Substring(JSON_PATH.Length));
                            }
                            else if (parameter.StartsWith(TERMINALFN_LOG_LEVEL))
                            {
                                int level = 0;
                                int.TryParse(parameter.Substring(TERMINALFN_LOG_LEVEL.Length), out level);
                                if (level >= 0 && level < 5)
                                    _terminalFnLogLevel = level;
                            }
                            else if (parameter.StartsWith(TERMINALFN_LOG_FORMAT))
                            {
                                int format = 0;
                                int.TryParse(parameter.Substring(TERMINALFN_LOG_FORMAT.Length), out format);
                                if (format == 1)
                                    _terminalFnLogFormat = format;
                                else
                                    _terminalFnLogFormat = 0;

                            }
                            else if (parameter.StartsWith(TERMINALFN_MIN_FDTIME))
                            {
                                DateTime.TryParseExact(parameter.Substring(TERMINALFN_MIN_FDTIME.Length), "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _minAvailableFdTime);
                            }
                            else if (parameter.StartsWith(TERMINALFN_PREFER_PORT))
                            {
                                int t;
                                if (int.TryParse(parameter.Substring(TERMINALFN_PREFER_PORT.Length), out t))
                                    if (t >= 0)
                                        _terminalPreferPort = t;
                            }
                            else if (parameter.StartsWith(TFN_NDS_AMOUNTS_FILL_METHOD))
                            {
                                int t;
                                if (int.TryParse(parameter.Substring(TFN_NDS_AMOUNTS_FILL_METHOD.Length), out t))
                                    if (t == 0 || t == 1)
                                        _terminalFnTag1115Filling = t;
                            }
                            else if (parameter.StartsWith(TFN_SKIP_ITEMS_IN_CORRECTION_FFD2))
                            {
                                if (parameter.Substring(TFN_SKIP_ITEMS_IN_CORRECTION_FFD2.Length).ToLower().Equals("false"))
                                {
                                    _tfnSkipItemsInCorrectionFfd2 = false;
                                }
                            }
                            else if (parameter.StartsWith(CORRECTION_ORDER_NUMBER_TYPE_DEPENDACE))
                            {
                                int t = 0;
                                if (int.TryParse(parameter.Substring(CORRECTION_ORDER_NUMBER_TYPE_DEPENDACE.Length), out t))
                                {
                                    if (t == 0 || t == 1)
                                    {
                                        _corretion_order_number_existance = t;
                                    }
                                }
                                    
                            }
                            else if (parameter.StartsWith(OFD_EXP_SET))
                            {
                                int numsSt = parameter.IndexOf(":");
                                string satName = parameter.Substring(OFD_EXP_SET.Length, numsSt - OFD_EXP_SET.Length);
                                string[] digits = parameter.Substring(numsSt + 1).Split(';');
                                List<int> ints = new List<int>();
                                foreach(var num in digits)
                                {
                                    if(!string.IsNullOrEmpty(num))
                                        ints.Add(int.Parse(num));
                                }
                                OfdExportSet[satName] = ints.ToArray();
                            }
                            else if (parameter.StartsWith(OVERRIDE_RETAIL_ADRESS))
                            {
                                if (parameter.Substring(OVERRIDE_RETAIL_ADRESS.Length).ToLower() == "true")
                                {
                                    _overideRetailAddress = true;
                                }
                            }
                            else if (parameter.StartsWith(OVERRIDE_RETAIL_PLACE))
                            {
                                if (parameter.Substring(OVERRIDE_RETAIL_PLACE.Length).ToLower() == "true")
                                {
                                    _overideRetailPlace = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    LogHandle.ol("Файл настроек отсутвует используются настройки по умолчанию");
                    _jsonHeaders.Add("[].ticket.document.receipt");
                    _jsonHeaders.Add("[].ticket.document.receiptCorrection");
                    _jsonHeaders.Add("[].ticket.document.bso");
                    _jsonHeaders.Add("[].ticket.document.bsoCorrection");
                }
            }
            catch(Exception e) { LogHandle.ol("При чтении настроек произошла ошибка; "+e.Message); }
            FTag.LoadTFNRules();
        }
        public static void SaveSettings() 
        { 
            StringBuilder sb = new StringBuilder("#[COMMON]");
            sb.AppendLine();
            // common settings
            if (_usingCustomSno)
            {
                sb.AppendLine(USING_CUSTOM_SNO_KEY);
            }
            else
            {
                sb.AppendLine(DONT_USING_CUSTOM_SNO_KEY);
            }    
            if (_appendFiscalSignAsPropertyData)
            {
                sb.AppendLine(APPEND_FISCAL_SIGN_AS_PROPERTY_DATA_KEY);
            }  
            else
            {
                sb.AppendLine(DONT_APPEND_FISCAL_SIGN_AS_PROPERTY_DATA_KEY);
            }
            sb.AppendLine(OVERRIDE_PROPERTY_DATA_VALUE_KEY + (_overridePropertyData?"true":"false"));
            sb.AppendLine(CORRECTION_ORDER_NUMBER_DEFAULT_KEY+_correctionOrderNumberDefault);
            sb.AppendLine(CORRECTION_ORDER_NUMBER_TYPE_DEPENDACE+_corretion_order_number_existance);
            sb.AppendLine(OVERRIDER_CORRECTION_ORDER_NUMBER_KEY + (_overrideCorrectionOrderNumber ? "true" : "false"));
            sb.AppendLine(OVERRIDE_CORRECTION_DOCUMENT_DATE_KEY+(_overrideCorrectionDocumentDate?"true":"false"));
            sb.AppendLine(ITEM_PRODUCT_TYPE_DEFAULT_KEY + _itemProductType);
            sb.AppendLine(ITEM_PAYMENT_TYPE_DEFAULT_KEY + _itemPaymentType);
            sb.AppendLine(ITEM_TAX_RATE_DEFAULT_KEY + _itemTaxRate);
            if(!string.IsNullOrEmpty(_itemName))
                sb.AppendLine(ITEM_NAME_DEFAULT_KEY+_itemName);
            if(_itemQuantity>0)
                sb.AppendLine(ITEM_QUANTITY_DEFAULT_KEY+_itemQuantity);
            if (_itemPrice > 0)
                sb.AppendLine(ITEM_PRICE_DEFAULT_KEY+_itemPrice);
            sb.Append(CO_PAY_INTERFACE_DOC_KEY);
            if (CoPayInterfaceDoc == CoPayMethods.CO_PAY_CASH)
                sb.AppendLine(CO_PAY_CASH);
            else if (CoPayInterfaceDoc == CoPayMethods.CO_PAY_ECASH)
                sb.AppendLine(CO_PAY_ECASH);
            else if (CoPayInterfaceDoc == CoPayMethods.CO_PAY_PREPAID)
                sb.AppendLine(CO_PAY_PREPAID);
            else if (CoPayInterfaceDoc == CoPayMethods.CO_PAY_CREDIT)
                sb.AppendLine(CO_PAY_CREDIT);
            else if (CoPayInterfaceDoc == CoPayMethods.CO_PAY_PROVISION)
                sb.AppendLine(CO_PAY_PROVISION);
            else if(CoPayInterfaceDoc == CoPayMethods.CO_PAY_OFF)
                sb.AppendLine(CO_PAY_OFF);
            if (_extendedTextInfoOffseBefore > 0)
            {
                sb.AppendLine(EXTENDED_TEXT_INFO_OFFSET_BEFORE+_extendedTextInfoOffseBefore);
            }
            if (!string.IsNullOrEmpty(_extendedTextInfoPrefix)&& _extendedTextInfoPrefix != "Информация о корректируемом документе")
            {
                sb.AppendLine(EXTENDED_TEXT_INFO_PREFIX + _extendedTextInfoPrefix);
            }
            for (int i = 0; i < 3; i++)
                if (_extendedTextInfoStrFormat[i] > 0)
                    sb.AppendLine(EXTENDED_TEXT_INFO_STRINGS + i + "=" + _extendedTextInfoStrFormat[i]);
            if(_extendedTextInfoOffseAfter > 0)
            {
                sb.AppendLine(EXTENDED_TEXT_INFO_OFFSET_AFTER + _extendedTextInfoOffseAfter);
            }
            if (!_extendedTextInfoCleanAfterPrint)
            {
                sb.AppendLine(EXT_TXT_INFO_DONT_CLEAN_AFTER_PRINT);
            }
            if (!string.IsNullOrEmpty(_cashierDefault))
            {
                sb.AppendLine(CASHIER_DEFAULT_KEY + _cashierDefault);
            }
            if (!string.IsNullOrEmpty(_cashierInnDefault))
            {
                sb.AppendLine(CASHIER_INN_DEFAULT_KEY + _cashierInnDefault);
            }
            sb.AppendLine(SEND_BI_DOC_INFO_IGNORE_BUYER_INN + _always_send_buyer_doc_data.ToString().ToLower());
            sb.AppendLine(OVERRIDE_RETAIL_ADRESS+_overideRetailAddress);
            sb.AppendLine(OVERRIDE_RETAIL_PLACE+_overideRetailPlace);
         
            
            sb.AppendLine();
            if (_jsonHeaders.Count > 0)
            {
                sb.AppendLine("#JSON HEADERS");
                foreach(var header in _jsonHeaders)
                {
                    sb.AppendLine(JSON_PATH+header);
                }
                sb.AppendLine();
            }
            // shtrih settings
            sb.AppendLine("#[SHTRIH]");
            if (_shtrihPrintPropertyData)
            {
                sb.AppendLine(SHTRIH_PRINT_PROPERTY_DATA_KEY);
            }
            if (_shtrihFomtForPrinting != 1)
            {
                sb.AppendLine(SHTRIH_FONT_FOR_PRINTING+_shtrihFomtForPrinting);
            }
            if (_shtrihIgnoreOldDriver)
            {
                sb.AppendLine(SHTRIH_IGNORE_OLD_DRIVER);
            }
            sb.AppendLine(SHTRIH_REGISTER_ITEM + _shtrihRegisterItemMethod);
            sb.AppendLine(SHTRIH_CLOSE_CHECK_METHOD + _shtrihCloseCheckMethod);
            // atol settings
            sb.AppendLine();
            sb.AppendLine("#[ATOL]");

            if (_atolUsePropertyData)
            {
                sb.AppendLine(ATOL_USE_PROPERTY_DATA_KEY);
            }
            else
            {
                sb.AppendLine(ATOL_DONT_USE_PROPERTY_DATA_KEY);
            }
            if (_autoUnit120SetZero)
            {
                sb.AppendLine(AUTO_UNIT120_SET_ZERO_KEY);
            }
            else { sb.AppendLine(UNIT120_SAVE_EMPTY_KEY); }
            if(_atolFontForPrinting != 0) 
            {
                sb.AppendLine(ATOL_FONT_FOR_PRINTING+_atolFontForPrinting);
            }
            sb.AppendLine(ATOL_FILL_ITEMS_PT_DEF+_atolFillItemsPaymentTypeDefault4);
            sb.AppendLine();
            sb.AppendLine("#[EMULATOR]");
            sb.AppendLine(EMULATOR_DELAY+_emulatorDelay);
            sb.AppendLine("#[TERMINAL_FN]");
            if( _terminalFnLogLevel != 0 )
                sb.AppendLine(TERMINALFN_LOG_LEVEL+_terminalFnLogLevel);
            sb.AppendLine(TERMINALFN_LOG_FORMAT + _terminalFnLogFormat);
            if((_minAvailableFdTime - new DateTime(2024,1,1)).TotalMinutes != 0)
            {
                sb.AppendLine(TERMINALFN_MIN_FDTIME + _minAvailableFdTime.ToString("dd.MM.yyyy"));
            }
            sb.AppendLine(TERMINALFN_PREFER_PORT+_terminalPreferPort);
            sb.AppendLine(TFN_NDS_AMOUNTS_FILL_METHOD + _terminalFnTag1115Filling);
            sb.AppendLine(TFN_SKIP_ITEMS_IN_CORRECTION_FFD2 + _tfnSkipItemsInCorrectionFfd2);

            sb.AppendLine();
            foreach (var set in OfdExportSet.Keys)
            {
                sb.Append(OFD_EXP_SET);
                sb.Append(set);
                sb.Append(':');
                foreach (var number in OfdExportSet[set])
                {
                    sb.Append(number);
                    sb.Append(';');
                }
                sb.AppendLine();
            }

            try
            {
                using (StreamWriter sw = new StreamWriter(fileSettings, false, System.Text.Encoding.UTF8))
                {
                    sw.Write(sb.ToString());
                }
                LogHandle.ol("Settings updated");
            }
            catch (Exception ex)
            {
                LogHandle.ol("При сохраниии настроек произошла ошибка: "+ex.Message);
            }
        }

        private bool _skipProcessing = true;
        private void ChangingSettings(object sender, EventArgs e)
        {
            if (_skipProcessing) 
                return;

            if (sender == checkBox_atolUsePropertyData)
            {
                _atolUsePropertyData = checkBox_atolUsePropertyData.Checked;
            }
            else if(sender == textBox_cashierDefault)
            {
                _cashierDefault = textBox_cashierDefault.Text;
            }
            else if (sender == textBox_cashierInnDefault)
            {
                string inn = textBox_cashierInnDefault.Text;
                if (FiscalPrinter.CorrectInn(inn))
                {
                    _cashierInnDefault = inn;
                    textBox_cashierInnDefault.ForeColor = System.Drawing.Color.Black;
                }
                else
                {
                    _cashierInnDefault = "";
                    textBox_cashierInnDefault.ForeColor = System.Drawing.Color.Red;
                }

            }
            else if(sender == radioButton_cs_usingSno)
            {
                if(radioButton_cs_usingSno.Checked)
                {
                    LogHandle.ol("Usage custom SNO true");
                    _usingCustomSno = true;
                }
            }
            else if(sender == radioButton_cs_dontUsingSno && radioButton_cs_dontUsingSno.Checked)
            {
                if (radioButton_cs_dontUsingSno.Checked)
                {
                    LogHandle.ol("Usage custom SNO false");
                    _usingCustomSno = false;
                }
            }
            else if (sender == checkBox_setFiscalSignAsPropertyData)
            {
                _appendFiscalSignAsPropertyData = checkBox_setFiscalSignAsPropertyData.Checked;
                radioButton_overrideOriginalPropertyData.Enabled = _appendFiscalSignAsPropertyData;
                radioButton_saveOriginalPropertyData.Enabled = _appendFiscalSignAsPropertyData;
            }
            else if(sender == radioButton_overrideOriginalPropertyData )
            {
                if (radioButton_overrideOriginalPropertyData.Checked)
                    _overridePropertyData = true;
                else
                    return;
            }
            else if ( sender == radioButton_saveOriginalPropertyData)
            {
                if (radioButton_saveOriginalPropertyData.Checked)
                    _overridePropertyData = false;
                else
                    return;
            }
            else if(sender == textBox_correctionOrderNumberDefault)
            {
                _correctionOrderNumberDefault = textBox_correctionOrderNumberDefault.Text;
            }
            else if (sender == radioButton_correctionOrderNumberOverride)
            {
                if (radioButton_correctionOrderNumberOverride.Checked)
                    _overrideCorrectionOrderNumber = true;
                else
                    return;
            }
            else if(sender == radioButton_correctionOrderNumberSaveOriginal)
            {
                if (radioButton_correctionOrderNumberSaveOriginal.Checked)
                    _overrideCorrectionOrderNumber = false;
                else 
                    return;
            }
            else if (sender == radioButton_correctionDocumentDateSaveOriginal)
            {
                if (radioButton_correctionDocumentDateSaveOriginal.Checked)
                    _overrideCorrectionDocumentDate = false;
                else 
                    return;
            }
            else if(sender == radioButton_correctionDocumentDateOveeride)
            {
                if (radioButton_correctionDocumentDateOveeride.Checked)
                    _overrideCorrectionDocumentDate = true;
                else
                    return;
            }
            else if(sender == comboBox_settingsItemProductTypeDefault)
            {
                if(comboBox_settingsItemProductTypeDefault.SelectedIndex == 28 || comboBox_settingsItemProductTypeDefault.SelectedIndex == 29)
                {
                    comboBox_settingsItemProductTypeDefault.SelectedIndex = _itemProductType;
                }
                else
                {
                    _itemProductType = comboBox_settingsItemProductTypeDefault.SelectedIndex;
                }
            }
            else if(sender == comboBox_settingsItemPaymentTypeDefault)
            {
                _itemPaymentType = comboBox_settingsItemPaymentTypeDefault.SelectedIndex;
            }
            else if(sender == comboBox_settingsItemTaxRateDefault)
            {
                _itemTaxRate = comboBox_settingsItemTaxRateDefault.SelectedIndex;
            }
            else if(sender == textBox_itemNameDefault)
            {
                _itemName = textBox_itemNameDefault.Text;
            }
            else if(sender == textBox_itemPriceDefault)
            {
                try { _itemPrice = double.Parse(FiscalPrinter.ReplaceBadDecimalSeparatorPoint(textBox_itemPriceDefault.Text)); } catch { _itemPrice = -1; }
            }
            else if (sender == textBox_itemQuantityDefault)
            {
                try { _itemQuantity = double.Parse(FiscalPrinter.ReplaceBadDecimalSeparatorPoint(textBox_itemQuantityDefault.Text)); } catch { _itemQuantity = -1; }
            }
            else if(sender == comboBox_copayIntFd)
            {
                switch (comboBox_copayIntFd.SelectedIndex)
                {
                    case (int)CoPayMethods.CO_PAY_OFF:
                        CoPayInterfaceDoc = CoPayMethods.CO_PAY_OFF;
                        break;
                    case (int)CoPayMethods.CO_PAY_ECASH:
                        CoPayInterfaceDoc = CoPayMethods.CO_PAY_ECASH;
                        break;
                    case (int)CoPayMethods.CO_PAY_PREPAID:
                        CoPayInterfaceDoc = CoPayMethods.CO_PAY_PREPAID;
                        break;
                    case (int)CoPayMethods.CO_PAY_CREDIT:
                        CoPayInterfaceDoc = CoPayMethods.CO_PAY_CREDIT;
                        break;
                    case (int)CoPayMethods.CO_PAY_PROVISION:
                        CoPayInterfaceDoc = CoPayMethods.CO_PAY_PROVISION;
                        break;
                    case (int)CoPayMethods.CO_PAY_CASH:
                    default:
                        CoPayInterfaceDoc = CoPayMethods.CO_PAY_CASH;
                        break;
                }
            }
            else if (sender == checkBox_fillItemPaymetTypeDef)
            {
                _atolFillItemsPaymentTypeDefault4 = checkBox_fillItemPaymetTypeDef.Checked;
            }
            else if (sender == checkBox_atolAppendMeasure120)
            {
                _autoUnit120SetZero = checkBox_atolAppendMeasure120.Checked;
            }
            else if (sender == textBox_emuDelay)
            {
                try
                {
                    _emulatorDelay = Convert.ToInt32(textBox_emuDelay.Text);
                }
                catch { }
            }

            else if (sender == textBox_infoPrefix)
            {
                _extendedTextInfoPrefix = textBox_infoPrefix.Text;
            }
            else if (sender == comboBox_infiStr1)
            {
                _extendedTextInfoStrFormat[0] = comboBox_infiStr1.SelectedIndex;
            }
            else if (sender == comboBox_infiStr2)
            {
                _extendedTextInfoStrFormat[1] = comboBox_infiStr2.SelectedIndex;
            }
            else if (sender == comboBox_infiStr3)
            {
                _extendedTextInfoStrFormat[2] = comboBox_infiStr3.SelectedIndex;
            }
            else if (sender == comboBox_indentBefore)
            {
                _extendedTextInfoOffseBefore = comboBox_indentBefore.SelectedIndex;
            }
            else if (sender == comboBox_indentAfter)
            {
                _extendedTextInfoOffseAfter = comboBox_indentAfter.SelectedIndex;
            }
            else if (sender == textBox_atolFontForPrinting)
            {
                int font = 1;
                int.TryParse(textBox_atolFontForPrinting.Text, out font);
                _atolFontForPrinting = font;
            }
            else if (sender == checkBox_shtrihPrintPropertyData)
            {
                _shtrihPrintPropertyData = checkBox_shtrihPrintPropertyData.Checked;
            }
            else if (sender == textBox_shtrihFontForPrinting)
            {
                int font = 1;
                int.TryParse(textBox_shtrihFontForPrinting.Text, out font);
                if (font > 0 && font < 9)
                    _shtrihFomtForPrinting = font;
            }
            else if (sender == radioButton_shtrihCloseCheckEx2 && radioButton_shtrihCloseCheckEx2.Checked)
            {
                _shtrihCloseCheckMethod = 1;
            }
            else if (sender == radioButton_shtrihCloseCheqMethodOld && radioButton_shtrihCloseCheqMethodOld.Checked)
            {
                _shtrihCloseCheckMethod = 0;
            }
            else if (sender == radioButton_shtrihCloseCheckEx3 && radioButton_shtrihCloseCheckEx3.Checked)
            {
                _shtrihCloseCheckMethod = 2;
            }
            else if (sender == radioButton_shtrihRegRuleOld && radioButton_shtrihRegRuleOld.Checked)
            {
                _shtrihRegisterItemMethod = 1;
            }
            else if (sender == radioButton_shtrihRegRuleMain && radioButton_shtrihRegRuleMain.Checked)
            {
                _shtrihRegisterItemMethod = 0;
            }
            else if (sender == comboBox_cleanAfterPrint)
            {
                _extendedTextInfoCleanAfterPrint = comboBox_cleanAfterPrint.SelectedIndex == 0;
            }
            else if (sender == button_addJsonHeader)
            {
                string s = textBox_jsonHeaerToAdd.Text;
                if (!_jsonHeaders.Contains(s))
                {
                    _jsonHeaders.Add(s);
                    listBox_jsonHeaders.Items.Add(s);
                }
            }
            else if (sender == button_removeJsonHeader)
            {
                var listToRemove = listBox_jsonHeaders.SelectedItems;

                foreach (var item in listToRemove)
                {
                    _jsonHeaders.Remove(item.ToString());
                }
                listBox_jsonHeaders.Items.Clear();
                listBox_jsonHeaders.Items.AddRange(_jsonHeaders.ToArray());
            }
            else if (sender == comboBox_terminalFnComPrefer)
            {
                _terminalPreferPort = comboBox_terminalFnComPrefer.SelectedIndex;
            }
            else if(sender == comboBox_tetminalFn1115Method)
            {
                _terminalFnTag1115Filling = comboBox_tetminalFn1115Method.SelectedIndex;
            }
            else if(sender == checkBox_terminalSkipItemsInCoorectionFfd2)
            {
                _tfnSkipItemsInCorrectionFfd2 = checkBox_terminalSkipItemsInCoorectionFfd2.Checked;
            }
            else if(sender == checkBox_correctionOrderNumber_CorrectionOrderExistance)
            {
                _corretion_order_number_existance = checkBox_correctionOrderNumber_CorrectionOrderExistance.Checked ? 1 : 0;
            }
            else if(sender == checkBox_ovverrideRetailAddress)
            {
                _overideRetailAddress = checkBox_ovverrideRetailAddress.Checked;
            }
            else if (sender == checkBox_ovverrideRetailPlace)
            {
                _overideRetailPlace = checkBox_ovverrideRetailPlace.Checked;
            }
            SaveSettings();
        }


        private const string fileSettings = "FdEditorSettings.ini";

        private const string USING_CUSTOM_SNO_KEY = "UsingCustomSno=true";
        private const string DONT_USING_CUSTOM_SNO_KEY = "UsingCustomSno=false";
        private static bool _usingCustomSno = true; 
        public static bool UsingCustomSno
        {
            get => _usingCustomSno;
            set 
            { 
                _usingCustomSno = value; 
                SaveSettings();
            }
        }


        private const string CASHIER_DEFAULT_KEY = "Cashier_default=";
        private static string _cashierDefault = "";
        public static string CashierDefault { get => _cashierDefault; set => _cashierDefault = value; }

        private const string CASHIER_INN_DEFAULT_KEY = "Cashier_inn_default=";
        private static string _cashierInnDefault = "";
        public static string CashierInnDefault { get => _cashierInnDefault; set => _cashierInnDefault = value; }


        private const string APPEND_FISCAL_SIGN_AS_PROPERTY_DATA_KEY = "AppendFiscalSignAsPropertyData=true";
        private const string DONT_APPEND_FISCAL_SIGN_AS_PROPERTY_DATA_KEY = "AppendFiscalSignAsPropertyData=false";
        private static bool _appendFiscalSignAsPropertyData = false;
        public static bool AppendFiscalSignAsPropertyData
        {
            get => _appendFiscalSignAsPropertyData;
            set
            { 
                _appendFiscalSignAsPropertyData = value;
                SaveSettings();
            }
        }

        private const string OVERRIDE_PROPERTY_DATA_VALUE_KEY = "OverridePropertyData=";
        private static bool _overridePropertyData = false;
        public static bool OverridePropertyData
        {
            get => _overridePropertyData;
            set
            {
                _overridePropertyData = value;
                SaveSettings();
            }
        }

        private const string OVERRIDE_CORRECTION_DOCUMENT_DATE_KEY = "OverrideCorrectionDocumentDate=";
        private static bool _overrideCorrectionDocumentDate = false;
        public static bool OverrideCorrectionDocumentDate
        {
            get => _overrideCorrectionDocumentDate;
            set
            {
                _overrideCorrectionDocumentDate = value;
                SaveSettings();
            }
        }

        private const string CORRECTION_ORDER_NUMBER_DEFAULT_KEY = "CorrectionOrderNumberDefault=";
        private static string _correctionOrderNumberDefault = "";
        public static string CorrectionOrderNumberDefault
        {
            get => _correctionOrderNumberDefault;
            set
            {
                if (value == null)
                    _correctionOrderNumberDefault = "";
                else
                    _correctionOrderNumberDefault= value;
                SaveSettings();
            }
        }
        private const string OVERRIDER_CORRECTION_ORDER_NUMBER_KEY = "OverrideCorrectionOrderNumber=";
        private static bool _overrideCorrectionOrderNumber = false;
        public static bool OverrideCorrectionOrderNumber
        {
            get => _overrideCorrectionOrderNumber;
            set
            {
                _overrideCorrectionOrderNumber = value;
                SaveSettings();
            }
        }

        private const string ITEM_PRODUCT_TYPE_DEFAULT_KEY = "items.productType=";
        private static int _itemProductType = 1;            // товар
        public static int ItemProductType
        {
            get => _itemProductType;
            set
            {
                if(value >= 0 && value < 34 && value != 28 && value != 29)
                    _itemProductType = value;
            }
        }

        private static string ITEM_PAYMENT_TYPE_DEFAULT_KEY = "items.paymentType=";
        private static int _itemPaymentType = 4;            // полный расчет
        public static int ItemPaymentType
        {
            get => _itemPaymentType;
            set
            {
                if(value>=0&&value<8)
                    _itemPaymentType = value;
            }
        }

        private const string ITEM_TAX_RATE_DEFAULT_KEY = "items.taxRate=";
        private static int _itemTaxRate = 6;                // без ндс
        public static int ItemTaxRate
        {
            get => _itemTaxRate;
            set
            {
                if(value>=0&&value <=10)
                    _itemTaxRate = value;
            }
        }

        private const string ITEM_NAME_DEFAULT_KEY = "items.name=";
        private static string _itemName = null;
        public static string ItemName
        {
            get => _itemName;
            set => _itemName = value;
        }

        private const string ITEM_PRICE_DEFAULT_KEY = "items.price=";
        private static double _itemPrice = -1;
        public static double ItemPrice
        {
            get => _itemPrice;
            set => _itemPrice = value;
        }

        private const string ITEM_QUANTITY_DEFAULT_KEY = "items.quantity=";
        private static double _itemQuantity = -1;
        public static double ItemQuantity
        {
            get => _itemQuantity;
            set => _itemQuantity = value;
        }

        private const string CO_PAY_INTERFACE_DOC_KEY = "Co-payInterfaceDocument=";
        private const string CO_PAY_OFF = "off";
        private const string CO_PAY_CASH = "cash";
        private const string CO_PAY_ECASH = "cashless";
        private const string CO_PAY_PREPAID = "prepaid";
        private const string CO_PAY_CREDIT = "credit";
        private const string CO_PAY_PROVISION = "provision";
        public enum CoPayMethods
        {
            CO_PAY_OFF = (int) 0,
            CO_PAY_CASH = (int) 1,
            CO_PAY_ECASH = (int) 2,
            CO_PAY_PREPAID = (int) 3,
            CO_PAY_CREDIT = (int) 4,
            CO_PAY_PROVISION = (int) 5,
        }
        public static CoPayMethods CoPayInterfaceDoc = CoPayMethods.CO_PAY_CASH;

        private static bool _overideRetailAddress = false;
        const string OVERRIDE_RETAIL_ADRESS = "Use_retail_address=";
        public static bool OverideRetailAddress
        {
            get => _overideRetailAddress;
            set => _overideRetailAddress = value;
        }
        private static bool _overideRetailPlace = false;
        const string OVERRIDE_RETAIL_PLACE = "Use_retail_place=";
        public static bool OverideRetailPlace
        {
            get => _overideRetailPlace;
            set => _overideRetailPlace = value;
        }


        // пресеты настроек выгрузок ОФД
        private const string OFD_EXP_SET = "ofd_export_set=";
        public static Dictionary<string, int[]> OfdExportSet = new Dictionary<string, int[]>();
        //public const int OFD_EXPORT_FIELDS = 15;

        public static int EmulatorDelay
        {
            get => _emulatorDelay;
            set 
            { 
                _emulatorDelay = value; 
                SaveSettings();
            }
        }
        private static int _emulatorDelay = 10;
        private const string EMULATOR_DELAY = "EmulatorPerformingDelay=";

        


        private void linkLabel_doc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender == linkLabel_doc)
            {
                if (e.Button == MouseButtons.Left)
                {
                    try
                    {
                        linkLabel_doc.LinkVisited = true;
                        System.Diagnostics.Process.Start("https://docs.google.com/document/d/1iyOS7q8_ULj-dHfb7LYhAu0a19XmvymBdfMWLfGu1Xw/edit?usp=sharing");

                    }
                    catch (Exception exc)
                    {
                        LogHandle.ol(exc.Message);
                    }
                }
            }
            
        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (sender == button_saveRules)
            {
                XmlDocument xDoc = new XmlDocument();
                XmlDeclaration xmlDeclaration = xDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xDoc.AppendChild(xmlDeclaration);
                XmlElement xRoot = xDoc.CreateElement("tagrules");
                xDoc.AppendChild(xRoot);
                XmlElement tagDescribes = xDoc.CreateElement("tagdescribes");
                XmlComment xmlRuleComment1 = xDoc.CreateComment("В данной таблице содержатся правила разбора TLV структур");
                XmlComment xmlRuleComment2 = xDoc.CreateComment("Не советую менять если вы не знаете протокол ФФД");
                XmlComment xmlRuleComment3 = xDoc.CreateComment("Все типы кроме UNKNOWN описаны в протоколе ФНС");
                XmlComment xmlRuleComment5 = xDoc.CreateComment("НОМЕР ТЕГА(number=) уникальное значение, если его повторять то запишется последняя запись");
                XmlComment xmlRuleComment6 = xDoc.CreateComment("Тип тега. Основное правило разбора. Может принимать значения: ");
                XmlComment xmlRuleComment7 = xDoc.CreateComment("UNKNOWN - для неизвестных или ошибочных тегов");
                XmlComment xmlRuleComment8 = xDoc.CreateComment("Byte_ARRAY массив байт (например ФП)");
                XmlComment xmlRuleComment9 = xDoc.CreateComment("Bit_MASK обычно программа представляет его как целое число(напр. регистрационные параметры)");
                XmlComment xmlRuleComment10 = xDoc.CreateComment("BYTE целое число 0-255");
                XmlComment xmlRuleComment11 = xDoc.CreateComment("FVLN число с плавающей запятой(напр. к-во предмета расчета)");
                XmlComment xmlRuleComment12 = xDoc.CreateComment("NUMBER целое число");
                XmlComment xmlRuleComment13 = xDoc.CreateComment("OBJECT последовательность данных");
                XmlComment xmlRuleComment14 = xDoc.CreateComment("STRING строка в формате CP866");
                XmlComment xmlRuleComment15 = xDoc.CreateComment("STLV составной тег(напр. предмет расчета)");
                XmlComment xmlRuleComment16 = xDoc.CreateComment("Uint16 положительное двухбайтовое число 0-65535");
                XmlComment xmlRuleComment17 = xDoc.CreateComment("Uint32 положительное четырехбайтовое число 0-4294967296");
                XmlComment xmlRuleComment18 = xDoc.CreateComment("U32UT время UnixTime(количество секунд с 1970г)");
                XmlComment xmlRuleComment19 = xDoc.CreateComment("VLN положительное целое число с переменной длиной(денежные суммы в копейках)");
                XmlComment xmlRuleComment20 = xDoc.CreateComment("Значение чувствительно к регистру! Если заполнено не правильно в словарь не попадет");
                XmlComment xmlRuleComment21 = xDoc.CreateComment("ТЕКСТ ТЕГА \"User friendly\" название тега влияющее на строковое представление тега в логах или окне терминала");
                tagDescribes.AppendChild(xmlRuleComment1);
                tagDescribes.AppendChild(xmlRuleComment2);
                tagDescribes.AppendChild(xmlRuleComment3);
                tagDescribes.AppendChild(xmlRuleComment5);
                tagDescribes.AppendChild(xmlRuleComment6);
                tagDescribes.AppendChild(xmlRuleComment7);
                tagDescribes.AppendChild(xmlRuleComment8);
                tagDescribes.AppendChild(xmlRuleComment9);
                tagDescribes.AppendChild(xmlRuleComment10);
                tagDescribes.AppendChild(xmlRuleComment11);
                tagDescribes.AppendChild(xmlRuleComment12);
                tagDescribes.AppendChild(xmlRuleComment13);
                tagDescribes.AppendChild(xmlRuleComment14);
                tagDescribes.AppendChild(xmlRuleComment15);
                tagDescribes.AppendChild(xmlRuleComment16);
                tagDescribes.AppendChild(xmlRuleComment17);
                tagDescribes.AppendChild(xmlRuleComment18);
                tagDescribes.AppendChild(xmlRuleComment19);
                tagDescribes.AppendChild(xmlRuleComment20);
                tagDescribes.AppendChild(xmlRuleComment21);
                xRoot.AppendChild(tagDescribes);

                List<int> listTagsNumbers = new List<int>(FTag.typeMap.Keys);
                foreach (int t in listTagsNumbers)
                {
                    XmlElement ftagElem = xDoc.CreateElement("tag");
                    tagDescribes.AppendChild(ftagElem);
                    XmlAttribute ftagNumber = xDoc.CreateAttribute("number");
                    ftagNumber.Value = t.ToString();
                    ftagElem.Attributes.Append(ftagNumber);

                    XmlAttribute ftagElemType = xDoc.CreateAttribute("type");
                    XmlText typeText = xDoc.CreateTextNode(FTag.typeMap[t].ToString());
                    ftagElemType.AppendChild(typeText);
                    ftagElem.Attributes.Append(ftagElemType);
                    if (FTag.userFrandlyNames.ContainsKey(t))
                    {
                        XmlText ufName = xDoc.CreateTextNode(FTag.userFrandlyNames[t]);
                        ftagElem.AppendChild(ufName);
                    }
                }
                XmlElement fnsTokens = xDoc.CreateElement("fnstokens");
                xRoot.AppendChild(fnsTokens);
                XmlComment xmlComment1 = xDoc.CreateComment("Данная таблица влияет на разбор и сбор чеков в формате JSON.");
                XmlComment xmlComment2 = xDoc.CreateComment("Номер тега может повторяться.");
                XmlComment xmlComment3 = xDoc.CreateComment("Текстовое название(токен) тега - уникальное значение, если значения повторяются в правило попадет полследнее значение(пример повторения тег 1227 в 1.05 \"buyer\" в 1.2 \"buyerInformation.buyer\").");
                XmlComment xmlComment4 = xDoc.CreateComment("Так же таблица влияет на строковое представление тега в логах и окне терминала.");
                fnsTokens.AppendChild(xmlComment1);
                fnsTokens.AppendChild(xmlComment2);
                fnsTokens.AppendChild(xmlComment3);
                fnsTokens.AppendChild(xmlComment4);
                List<string> tokens = new List<string>(FTag.structuredTagNames.Keys);
                // номера как токены
                /*Dictionary<string,string> numsAsTokens = new Dictionary<string,string>();
                Dictionary<string, int> invertedFnsNames = new Dictionary<string, int>();
                foreach(int t in FTag.fnsNames.Keys)
                {
                    invertedFnsNames[FTag.fnsNames[t]] = t;
                }


                foreach(var oldToken in tokens)
                {


                    string nat = "";
                    if (oldToken.Contains("."))
                    {
                        string[] oldSubTokens = oldToken.Split('.');
                        foreach(string oldSubToken in oldSubTokens)
                        {
                            if (nat.Length > 0)
                            {
                                nat = nat+".";
                            }
                            nat = nat + invertedFnsNames[oldSubToken];
                        }
                    }
                    else
                    {
                        nat = invertedFnsNames[oldToken].ToString();
                    }
                    
                    numsAsTokens[oldToken] = nat;
                    nat = "";
                }*/

                foreach (string s in tokens)
                {
                    XmlElement tokenRule = xDoc.CreateElement("token");
                    fnsTokens.AppendChild(tokenRule);

                    XmlAttribute ftagNumber = xDoc.CreateAttribute("number");
                    ftagNumber.Value = FTag.structuredTagNames[s].ToString();
                    tokenRule.Attributes.Append(ftagNumber);

                    XmlText fnsTokenStr = xDoc.CreateTextNode(s/*numsAsTokens[s]*/);
                    tokenRule.AppendChild(fnsTokenStr);
                }

                xDoc.Save(textBox_saveRuleAsXml.Text);
            }
            else if (sender == button_loadRules)
            {
                
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Xml Files|*.xml|All files (*.*)|*.*";
                openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (openFileDialog1.ShowDialog(this)== DialogResult.OK)
                {

                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(openFileDialog1.FileName);
                    XmlElement xRoot = xDoc.DocumentElement;
                    if (xRoot != null)
                    {
                        // обход всех узлов в корневом элементе
                        List<object[]> tagRules = new List<object[]>();
                        List<object[]> tokensRule = new List<object[]>();
                        foreach (XmlElement xnode in xRoot)
                        {
                            if (xnode.Name == "tagdescribes")
                            {
                                // разбор наименований и типов тегов
                                foreach (object xo in xnode.ChildNodes)
                                {
                                    if (xo is XmlElement)
                                    {
                                        XmlElement xtagrule = (XmlElement)xo;
                                        if (xtagrule.Name == "tag")
                                        {
                                            int number = -1000000;
                                            string type = string.Empty;
                                            string userFriendlyName = string.Empty;
                                            var xtNumber = xtagrule.GetAttribute("number");
                                            type = xtagrule.GetAttribute("type");
                                            int.TryParse(xtNumber, out number);
                                            userFriendlyName = xtagrule.InnerText;
                                            if (number >= -1 && !string.IsNullOrEmpty(type))
                                            {
                                                tagRules.Add(new object[] { number, type, userFriendlyName });
                                            }
                                            else
                                            {
                                                LogHandle.ol("Bad record num=" + number + "; type=" + type + "; uf=" + userFriendlyName);
                                            }
                                        }
                                    }

                                }
                            }
                            else if (xnode.Name == "fnstokens")
                            {
                                // разбор правил распознавания json
                                foreach (object xo in xnode.ChildNodes)
                                {
                                    if (xo is XmlElement)
                                    {
                                        XmlElement xtoken = (XmlElement)xo;
                                        if (xtoken.Name == "token")
                                        {
                                            int number = -1000000;
                                            //string type = string.Empty;
                                            string fnstoken = string.Empty;
                                            var xtNumber = xtoken.GetAttribute("number");
                                            //type = xtoken.GetAttribute("type");
                                            int.TryParse(xtNumber, out number);
                                            fnstoken = xtoken.InnerText;
                                            if (number >= -1 && !string.IsNullOrEmpty(fnstoken))
                                            {
                                                tokensRule.Add(new object[] { number, fnstoken });
                                            }
                                        }
                                    }

                                }


                            }

                        }
                        int oldRulesFtag = FTag.typeMap.Count;
                        int oldRulesTokens = FTag.structuredTagNames.Count;

                        LogHandle.ol("FTag rules before: " + oldRulesFtag + "; Tokens before: " + oldRulesTokens);
                        LogHandle.ol("Recods rules founded: " + tagRules.Count + "; Tokens founded: " + tokensRule.Count);

                        Dictionary<int, FTag.FDataType> mainRecognitionRule = new Dictionary<int, FTag.FDataType>();
                        Dictionary<int, string> userFriendlyTagNames = new Dictionary<int, string>();
                        Dictionary<int, string> fnsJsonDict = new Dictionary<int, string>();
                        Dictionary<string, int> fnsJsonRulesStructured = new Dictionary<string, int>();
                        int badRecodsCounter = 0;
                        foreach (var objRule in tagRules)
                        {
                            try
                            {
                                if (objRule[0] is int && objRule[1] != null && objRule[1] is string)
                                {
                                    int tagNumber = (int)(objRule[0]);
                                    string tagType = objRule[1] as string;
                                    string ufName = (string)objRule[2];
                                    if (tagNumber >= -1)
                                    {
                                        if (!string.IsNullOrEmpty(ufName))
                                        {
                                            userFriendlyTagNames[tagNumber] = ufName;
                                        }


                                        if (tagType == "UNKNOWN")
                                        {
                                            mainRecognitionRule[tagNumber] = FTag.FDataType.UNKNOWN;
                                        }
                                        else if (tagType == "VLN")
                                        {
                                            mainRecognitionRule[tagNumber] = FTag.FDataType.VLN;
                                        }
                                        else if (tagType == "U32UT")
                                        {
                                            mainRecognitionRule[tagNumber] = FTag.FDataType.U32UT;
                                        }
                                        else if (tagType == "Uint32")
                                        {
                                            mainRecognitionRule[tagNumber] = FTag.FDataType.Uint32;
                                        }
                                        else if (tagType == "Uint16")
                                        {
                                            mainRecognitionRule[tagNumber] = FTag.FDataType.Uint16;
                                        }
                                        else if (tagType == "STLV")
                                        {
                                            mainRecognitionRule[tagNumber] = FTag.FDataType.STLV;
                                        }
                                        else if (tagType == "STRING")
                                        {
                                            mainRecognitionRule[tagNumber] = FTag.FDataType.STRING;
                                        }
                                        else if (tagType == "OBJECT")
                                        {
                                            mainRecognitionRule[tagNumber] = FTag.FDataType.OBJECT;
                                        }
                                        else if (tagType == "NUMBER")
                                        {
                                            mainRecognitionRule[tagNumber] = FTag.FDataType.NUMBER;
                                        }
                                        else if (tagType == "FVLN")
                                        {
                                            mainRecognitionRule[tagNumber] = FTag.FDataType.FVLN;
                                        }
                                        else if (tagType == "BYTE")
                                        {
                                            mainRecognitionRule[tagNumber] = FTag.FDataType.BYTE;
                                        }
                                        else if (tagType == "Bit_MASK")
                                        {
                                            mainRecognitionRule[tagNumber] = FTag.FDataType.Bit_MASK;
                                        }
                                        else if (tagType == "Byte_ARRAY")
                                        {
                                            mainRecognitionRule[tagNumber] = FTag.FDataType.Byte_ARRAY;
                                        }
                                        else
                                        {
                                            badRecodsCounter++;
                                            continue;
                                        }

                                    }
                                }
                                else
                                {
                                    badRecodsCounter++;
                                }
                            }
                            catch (Exception exc)
                            {
                                badRecodsCounter++;
                                LogHandle.ol(exc.Message);
                            }


                        }

                        foreach (var objTokenRule in tokensRule)
                        {
                            try
                            {
                                if (objTokenRule[0] is int && objTokenRule[1] != null && objTokenRule[1] is string)
                                {
                                    int tagNumber = (int)objTokenRule[0];
                                    string token = (string)objTokenRule[1];
                                    if (!string.IsNullOrEmpty(token) && tagNumber > 0)
                                    {
                                        fnsJsonRulesStructured[token] = tagNumber;
                                        if (token.Contains("."))
                                        {
                                            token = token.Substring(token.LastIndexOf(".") + 1);
                                        }
                                        fnsJsonDict[tagNumber] = token;
                                    }
                                    else
                                    {
                                        badRecodsCounter++;
                                    }
                                }
                            }
                            catch (Exception exc)
                            {
                                LogHandle.ol(exc.Message);
                                badRecodsCounter++;
                            }

                        }

                        LogHandle.ol("FNS tokens rules founded: " + fnsJsonRulesStructured.Count + "; Tokens before: " + FTag.structuredTagNames.Count);
                        LogHandle.ol("FNS names founded: " + fnsJsonDict.Count + "; FNS names before: " + FTag.fnsNames.Count);

                        DialogResult dialogResult = MessageBox.Show("Названий тегов распознано: " + fnsJsonRulesStructured.Count 
                            + Environment.NewLine+"Правил до замены: " + FTag.structuredTagNames.Count,
                            "Заменить правила?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.OK)
                        {
                            foreach (string tok in fnsJsonRulesStructured.Keys)
                            {
                                string s = tok;
                                if (tok.Contains("."))
                                {
                                    //int ditInd = tok.LastIndexOf(".") + 1;
                                    s = tok.Substring(tok.LastIndexOf(".") + 1);
                                }
                                if (s != fnsJsonRulesStructured[tok].ToString())
                                {
                                    LogHandle.ol(s + "----" + fnsJsonRulesStructured[tok]);
                                }

                            }
                            FTag.rulesChanged = true;
                            FTag.fnsNames = fnsJsonDict;
                            FTag.structuredTagNames = fnsJsonRulesStructured;
                            FTag.userFrandlyNames = userFriendlyTagNames;
                            FTag.typeMap = mainRecognitionRule;
                            if (comboBox_defaultRootTagValue.SelectedIndex == 0)
                            {
                                FTag.DefaultFormCode = 0;
                            }
                            else if (comboBox_defaultRootTagValue.SelectedIndex == 1)
                            {
                                FTag.DefaultFormCode = 3;
                            }
                            else if (comboBox_defaultRootTagValue.SelectedIndex == 2)
                            {
                                FTag.DefaultFormCode = 31;
                            }
                        }

                        
                    }
                }
            }
            else if(sender == comboBox_defaultRootTagValue)
            {
                if (comboBox_defaultRootTagValue.SelectedIndex == 0)
                {
                    FTag.DefaultFormCode = 0;
                }
                else if (comboBox_defaultRootTagValue.SelectedIndex == 1)
                {
                    FTag.DefaultFormCode = 3;
                }
                else if (comboBox_defaultRootTagValue.SelectedIndex == 2)
                {
                    FTag.DefaultFormCode = 31;
                }
            }

            
        }

        bool skipControlDev = false;
        private void ControlDevice(object sender, EventArgs e)
        {
            if(skipControlDev)
                return;
            ShtrihAdapter s = null;
            if(_fr is ShtrihAdapter)
                s = _fr as ShtrihAdapter;
            bool failSet= false;
            if (sender == radioButton_shtrihSwichPrintingOn&& radioButton_shtrihSwichPrintingOn.Checked)
            {
                s.EnablingPrinting = 0;
                int t = s.EnablingPrinting;
                if (s.CheckEnablingPrinting != 0) 
                {
                    failSet = true;
                }
                
            }
            else if (sender == radioButton_shtrihSwichPrintingOff && radioButton_shtrihSwichPrintingOff.Checked)
            {
                s.EnablingPrinting = 2;
                int t = s.EnablingPrinting;
                if (s.CheckEnablingPrinting != 2)
                {
                    failSet = true;
                }
            }
            else if (sender == radioButton_shtrihSwichFwTaxesOn && radioButton_shtrihSwichFwTaxesOn.Checked)
            {
                s.TaxesFillsFirmware = 0;
                int t = s.TaxesFillsFirmware;
                if (s.CheckTaxesFillsFirmware != 0)
                {
                    failSet = true;
                }
            }
            else if (sender == radioButton_shtrihSwichFwTaxesOff && radioButton_shtrihSwichFwTaxesOff.Checked)
            {
                s.TaxesFillsFirmware = 1;
                int t = s.TaxesFillsFirmware;
                if (s.CheckTaxesFillsFirmware != 1)
                {
                    failSet = true;
                }
            }
            if (failSet)
            {
                if(sender is RadioButton)
                {
                    (sender as RadioButton).Checked = false;
                    label_frShtrihStatus.Text = "Ошибка при установке настройки, попробуйте через ДТО";
                }

            }
        }
    }
}
