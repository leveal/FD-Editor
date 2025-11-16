using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FR_Operator
{
    public abstract class FiscalPrinter
    {

        public const int


           /*  
            *  если константа заканчивается на 
            *  KEY - ключ доступа к KKMInfoTransmitter
            *  LOC - значение согласно протоколу ФФД
            *  BF - бинарный флаг
            *  DSCR - описание предмета расчета  
            *  
            */
            NONE = 0,
            //      Номера тегов  ФФД
            FTAG_FISCAL_DOCUMENT_TYPE_FISCAL_REPORT = 1,
            FTAG_FISCAL_DOCUMENT_TYPE_OPEN_SHIFT = 2,
            FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE = 3,
            FTAG_FISCAL_DOCUMENT_TYPE_BSO = 4,
            FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_SHIFT = 5,
            FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_ARCHIVE = 6,

            FTAG_FISCAL_DOCUMENT_TYPE_FISCAL_REPORT_CORRECTION_REREG = 11,

            FTAG_FISCAL_DOCUMENT_TYPE_CALCULATION_REPORT = 21,//21 - Отчет о состоянии расчетов

            FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE = 31,

            FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION = 41,

            FTAG_PAD_TRANSFER_OPERATOR_ADDRESS = 1005,      // адрес оператора перевода

            FTAG_DESTINATION_EMAIL = 1008,                  // Телефон или электронный адрес покупателя
            FTAG_RETAIL_PLACE_ADRRESS = 1009,               // Адрес расчетов

            FTAG_DATE_TIME = 1012,                          // Дата, время
            FTAG_KKT_NUMBER_FACTORY = 1013,                 // заводской номер ККТ

            FTAG_PAD_TRANSFER_OPERATOR_INN = 1016,          // ИНН оператора перевода

            FTAG_USER_INN = 1018,                           // ИНН пользователя

            FTAG_TOTAL_SUM = 1020,                          // ИТОГ
            FTAG_CASHIER_NAME = 1021,                       // Кассир

            FTAG_ITEM_QUANTITY = 1023,                      // Количество предмета расчета

            FTAG_PAD_TRANSFER_OPERATOR_NAME = 1026,         // наименование оператора перевода

            FTAG_ITEM_NAME = 1030,                          // Наименование предмета расчета
            FTAG_CASH_TOTAL_SUM = 1031,                     // Сумма по чеку (БСО) наличными  

            FTAG_FD = 1040,                                 // ФД
            FTAG_FN_NUMBER = 1041,                          // ФН

            FTAG_ITEM_SUM = 1043,                           // Стоимость предмета расчета с учетом скидок и наценок
            FTAG_PAD_PAYPENT_AGENT_OPERATION = 1044,        // операция платежного агента

            FTAG_USER = 1048,                               // Наименование пользователя

            FTAG_OPERATION_TYPE = 1054,                     // Признак расчета (operationType/CalculationSign)
            FTAG_APPLIED_TAXATION_TYPE = 1055,              // СНО

            FTAG_PAYMENT_AGENT_TYPE = 1057,

            FTAG_ITEM = 1059,                               // Предмет расчета

            FTAG_REGISTERED_SNO = 1062,                     // системы налогоболожения в докуменнте регистрации

            FTAG_PAD_PAYMENT_AGENT_PHONE = 1073,            // телефон платежного агента
            FTAG_PAD_PAYMENT_OPERATOR_PHONE = 1074,         // телефон оператора по приему платежей
            FTAG_PAD_TRANSFER_OPERATOR_PHONE = 1075,        // телефон оператора перевода

            FTAG_DOC_FISCAL_SIGN = 1077,                    // Фискальный признак документа

            FTAG_ITEM_PRICE = 1079,                         // цена за единицу предмета расчета

            FTAG_ECASH_TOTAL_SUM = 1081,                    // Безналичными

            FTAG_PRORERTIES_1084 = 1084,
            FTAG_PROPERTIES_PROPERTY_NAME = 1085,
            FTAG_PROPERTIES_PROPERTY_VALUE = 1086,

            FTAG_NDS20_DOCUMENT_SUM = 1102,                 // Сумма НДС чека по ставке 20%
            FTAG_NDS10_DOCUMENT_SUM = 1103,                 // Сумма НДС чека по ставке 10%
            FTAG_NDS0_DOCUMENT_SUM = 1104,                  // Сумма расчета по чеку с НДС по ставке 0%
            FTAG_NDS_FREE_DOCUMENT_SUM = 1105,              // Сумма расчета по чеку без НДС
            FTAG_NDS20120_DOCUMENT_SUM = 1106,              // Сумма НДС чека по расч.ставке 20/120
            FTAG_NDS10110_DOCUMENT_SUM = 1107,              // Сумма НДС чека по расч.ставке 10/110

            FTAG_AMOUNTS_RECEIPT_NDS = 1115,                // Cуммы НДС чека
            FTAG_NOT_TRANSMITTED_DOCUMENT_NUMBER = 1116,
            FTAG_SELLER_ADDRESS = 1117,
            FTAG_RECEIPT_QUANTITY = 1118,
            FTAG_AMOUNTS_NDS = 1119,                        // Cуммы НДС чека
            FTAG_AMOUNTS_NDS_NDSSUM = 1120,                 // сумма НДС 

            FTAG_INTERNET_PAYMENT = 1125,                      // признак расчета в интернет

            FTAG_ITEM_PRODUCT_CODE = 1162,                  // Код товара
            FTAG_ITEM_PRODUCT_CODE_NEW = 1163,              // Код товара ФФД 1.2

            FTAG_PD_PROVIDER_PHONE = 1171,                  // телефон поставщика

            FTAG_CORRECTION_TYPE = 1173,                    // Тип коррекции byte 0-1
            FTAG_CORRECTION_BASE = 1174,                    // Основание для коррекции STLV

            FTAG_CORRECTION_DESCRIBER = 1177,               // Описание коррекции
            FTAG_CORRECTION_DOC_DATE = 1178,                // Дата документа основания для коррекции Number UnixTime
            FTAG_CORRECTION_ORDER_NUMBER = 1179,            // Номер документа основания для коррекции

            FTAG_RETAIL_PLACE = 1187,                       // Место расчетов
            FTAG_KKT_VERSION = 1188,                        // Версия ККТ

            FTAG_PROPERTIES_DATA = 1192,                    // Дополнительный реквизит чека (БСО)

            FTAG_ITEM_UNIT_MEASURE_105 = 1197,              // Единица измерения предмета расчета !!!не реализовано

            FTAG_ITEM_NDS_RATE = 1199,                      // Ставка НДС
            FTAG_ITEM_NDS_SUM = 1200,                       // Сумма НДС за предмет расчета

            FTAG_CASHIER_INN = 1203,                        // ИНН кассира

            FTAG_OPERATOR_MESSAGE = 1206,

            FTAG_FFD = 1209,                                // Версия ФФД    

            FTAG_ITEM_PRODUCT_TYPE = 1212,                  // Признак предмета расчета

            FTAG_ITEM_PAYMENT_TYPE = 1214,                  // Признак способа расчета
            FTAG_PREPAID_TOTAL_SUM = 1215,                  // Сумма по чеку(БСО) предоплатой(зачетом аванса и (или) предыдущих платежей)
            FTAG_CREDIT_TOTAL_SUM = 1216,                   // Сумма по чеку(БСО) постоплатой(в кредит)
            FTAG_PROVISION_TOTAL_SUM = 1217,                // Сумма по чеку(БСО) встречным предоставлением

            FTAG_ITEM_PAYMENT_AGENT_BY_PRODUCT_TYPE = 1222, // Признак агента по предмету расчета
            FTAG_ITEM_PAYMENT_AGENT_DATA = 1223,            // данные агента
            FTAG_ITEM_PROVIDER_DATA = 1224,                 // данные поставщика
            FTAG_PD_PROVIDER_NAME = 1225,                   // наименование поставщика
            FTAG_ITEM_PROVIDER_INN = 1226,                  // ИНН поставщика
            FTAG_BUYER_INFORMATION_BUYER = 1227,            // Покупатель (клиент)
            FTAG_BUYER_INFORMATION_BUYER_INN = 1228,        // ИНН покупателя(клиента)

            FTAG_ITEM_ORIGINAL_COUNTRY_CODE = 1230,         // Код страны происхождения товара
            FTAG_ITEM_CUSTOM_ENTRY_NUM = 1231,              // Номер таможенной декларации !!!не реализовано

            FTAG_BI_BIRTHDAY = 1243,                        // день рождения покупателя
            FTAG_BI_CITIZENSHIP = 1244,                     // гражданство покупателя
            FTAG_BI_DOCUMENT_CODE = 1245,                    // код вида документа, удостоверяющего личность
            FTAG_BI_DOCUMENT_DATA = 1246,             // данные документа, удостоверяю щеголичность
            FTAG_BI_ADDRESS = 1254,                   // адрес покупателя(клиента)

            FTAG_BUYER_INFORMATION = 1256,                  // сведения о покупателе (клиенте) STLV

            FTAG_ADDITIONAL_PROPERS_OS = 1276,
            FTAG_ADDITIONAL_DATA_OS = 1277,
            FTAG_ADDITIONAL_PROPERS_CS = 1278,
            FTAG_ADDITIONAL_DATA_CS = 1279,

            FTAG_ITEM_UNIT_MEASURE_120 = 2108,              // мера количества предмета расчета

            INFO_MESSAGE_KEY = 9000,                        // ответ драйвера
            FR_MODEL_KEY = 10000,                           // Модель ФР 
            FR_SERIAL_KEY = 10010,                          // Заводской номер ФР
            FR_TIME_KEY = 10020,                            // Текущее время ФР
            FR_FIRMWARE_KEY = 10030,                        // Версия прошивки ФР
            FR_SHIFT_STATE_KEY = 10040,                     // Состояние смены
             FR_SHIFT_CLOSED = 10041,   //смена закрыта
             FR_SHIFT_OPEN = 10042,     //смена открыта
             FR_SHIFT_EXPIRED = 10043,  //смена истекла  
            FR_LAST_FD_NUMBER_KEY = 10050,              // Номер последнего ФД
            FR_FFDVER_KEY = 10060,
             FR_FFD100 = 100,         //ФФД 1.0
             FR_FFD105 = 105,         //ФФД 1.05
             FR_FFD110 = 110,         //ФФД 1.1
             FR_FFD120 = 120,         //ФФД 1.2
            FR_OFD_EXCHANGE_STATUS_KEY = 10070,
            FR_OWNER_USER_KEY = 10080,
            FR_OWNER_ADDRESS_KEY = 10090,
            FR_REGISTERD_SNO_KEY = 10100,
             FR_SNO_OSN = 10101, FR_SNO_OSNO_BF = 0b1,
             FR_SNO_USN_D = 10102, FR_SNO_USN_D_BF = 0b10,
             FR_SNO_USN_D_R = 10104, FR_SNO_USN_D_R_BF = 0b100,
             FR_SNO_ENVD = 10108, FR_SNO_ENVD_BF = 0b1000,
             FR_SNO_ESHN = 10116, FR_SNO_ESHN_BF = 0b10000,
             FR_SNO_PSN = 10132, FR_SNO_PSN_BF = 0b100000,
            FR_CONNECTION_SETTINGS_KEY = 10200,
            FR_LAST_ERROR_MSG_KEY = 10220,
            FR_FN_SERIAL_KEY = 10230,
            FR_REGFNS_KEY = 10240,
            FR_STATUS_MODE_KEY = 10250,

            FR_CASHIER_NAME_KEY = 10300,
            FR_CASHIER_INN_KEY = 10301,

            FD_DOCUMENT_NAME_CHEQUE = 11000,
            FD_DOCUMENT_NAME_CORRECTION_CHEQUE = 11001,

            /* контроль предмета расчета */
            FD_ITEM_CONTROL_OK = 11010,                                                     // ошибок нет
            FD_ITEM_CONTROL_WARN_CODE = 11011,                                              // предмет расчета требует код товара(например акциз), но он не указан
            FD_ITEM_CONTROL_CRITICAL_ERROR = 11012,                                         // критическая ошибка например количество = 0
            FD_ITEM_CONTROL_OK_BF = 0,                                                      // ошибок нет или неизвестна
            FD_ITEM_CONTROL_BAD_PRICE_BF = 0b1,                                             // некорректная цена
            FD_ITEM_CONTROL_BAD_QUANTITY_BF = 0b10,                                         // некорректное количество
            FD_ITEM_CONTROL_ZERO_QUANTITY_BF = 0b100,                                       // пустое количество
            FD_ITEM_CONTROL_BAD_SUM_BF = 0b1000,                                            // некорректная сумма



            FD_CALCULATION_SIGN = 11100,
            FD_CALCULATION_INCOME_LOC = 1,                   // приход
            FD_CALCULATION_BACK_INCOME_LOC = 2,         // возврат прихода
            FD_CALCULATION_EXPENCE_LOC = 3,                 // расход
            FD_CALCULATION_BACK_EXPENCE_LOC = 4,       // возврат расхода

             // Значения реквизита "ставка НДС" (тег 1199)
             NDS_TYPE_EMPTY_LOC = 0,                              // Ставка НДС (не выбрана)
             NDS_TYPE_20_LOC = 1,                                    // Ставка НДС (НДС 20%)
             NDS_TYPE_10_LOC = 2,                                    // Ставка НДС (НДС 10%)
             NDS_TYPE_20120_LOC = 3,                              // Ставка НДС (НДС 20/120)
             NDS_TYPE_10110_LOC = 4,                              // Ставка НДС (НДС 10/110)
             NDS_TYPE_0_LOC = 5,                                      // Ставка НДС (НДС 0%)
             NDS_TYPE_FREE_LOC = 6,                                // Ставка НДС (БЕЗ НДС)
             NDS_TYPE_5_LOC = 7,                                // Ставка НДС 5
             NDS_TYPE_7_LOC = 8,                                // Ставка НДС 7
             NDS_TYPE_5105_LOC = 9,                                // Ставка НДС 5106
             NDS_TYPE_7107_LOC = 10,                                // Ставка НДС 7107

             /* Оплата чека*/
             FD_SUMS_TOTAL_SUM_LOC = 0,                            // 1020 Итог 
             FD_SUMS_PAY_CASH_LOC = 1,                              // 1031 Сумма по чеку наличными
             FD_SUMS_PAY_ECASH_LOC = 2,                            // 1081 Сумма по чеку безналичными
             FD_SUMS_PAY_PREPAID_LOC = 3,                        // 1215 Сумма по чеку предоплатой (зачетомаванса и(или) предыдущих платежей)
             FD_SUMS_PAY_CREDIT_LOC = 4,                          // 1216 Сумма по чеку постоплатой(в кредит)
             FD_SUMS_PAY_PROVISION_LOC = 5,                    // 1217 Сумма по чеку встречным представлением
                                                               // суммы НДС в итоге чека
             FD_SUMS_NDS_20_LOC = 6,                              // 1139 сумма НДС 20 в чеке  
             FD_SUMS_NDS_10_LOC = 7,                              // 1140 сумма НДС 10 в чеке  
             FD_SUMS_NDS_20120_LOC = 8,                        // 1141 сумма НДС 20/120 в чеке  
             FD_SUMS_NDS_10110_LOC = 9,                        // 1142 сумма НДС 10/110 в чеке  
             FD_SUMS_NDS_0_LOC = 10,                               // 1143 сумма НДС 0 в чеке  
             FD_SUMS_NDS_FREE_LOC = 11,                         // 1183 сумма без НДС  в чеке  
             FD_SUMS_NDS_5_LOC = 12,
             FD_SUMS_NDS_7_LOC = 13,
             FD_SUMS_NDS_5105_LOC = 14,
             FD_SUMS_NDS_7107_LOC = 15,



            //      форматно логический контроль документа BINARY FLAGS
            FD_DC_OK = 0,                                                                   // нет информации
            FD_DC_CRITICAL_ERROR_BF = 0b1,                                                  // невозможно оформить документ
            FD_DC_ERROR_NOT_ENOUTH_PAID_BF = 0b10,                                          // чек не оплачен
            //FD_DC_WARN_NDS_SUMS_BF = 0b100,                                                 // некорректны суммы НДС 
            FD_DC_ERROR_BAD_ITEM_BF = 0b1000,                                               // некорректен предмет расчета
            FD_DC_WARN_WARNED_ITEM_BF = 0b10000,                                            // некритичная проблема с одним из предметов расчета
            FD_DC_ERROR_OVERPAID_CRITICAL_BF = 0b100000,                                    // критичная переплата в документе
            FD_DC_ERROR_OVERPAID_WITH_CHANGE_BF = 0b1000000,                                // переплата в документе со сдачей наличными
            FD_DC_PAID_EXACT_BF = 0b10000000,                                               // точная оплата документа
            //FD_DC_PAID_WRITE_DOWN_PENNIES_BF = 0b100000000,                                 // округление/списание копеек

            FD_CORRECTION_TYPE = 11310,                                                     // 1174 Основание для коррекции составной STLV
             FD_CORRECTION_TYPE_SELF_MADE = 11311, FD_CORRECTION_TYPE_SELF_LOC = 0,         // 1173 Тип коррекции самостоятельно
             FD_CORRECTION_TYPE_BY_ORDER = 11312, FD_CORRECTION_TYPE_ORDER_LOC = 1,         // 1173 Тип коррекции по предписанию



             //      признак предмета расчета" (тег 1212)
             FD_ITEM_TYPE_DESCRIBER_SHORT = 0, FD_ITEM_TYPE_DESCRIBER_FULL = 1,
             FD_ITEM_TYPE_EMPTY_LOC = 0,                    // не указан
             FD_ITEM_TYPE_PRODUCT_LOC = 1,                // ТОВАР
             FD_ITEM_TYPE_EXCISE_LOC = 2,                  // Подакцизный товар
             FD_ITEM_TYPE_WORK_LOC = 3,                      // РАБОТА
             FD_ITEM_TYPE_SERICE_LOC = 4,                  // услуга
             FD_ITEM_TYPE_GAMBLING_BET_LOC = 5,      // ставка азартной игры
             FD_ITEM_TYPE_GAMBLING_WIN_LOC = 6,      // выигрыш азартной игры
             FD_ITEM_TYPE_RAFFLE_TICKET_LOC = 7,    // ставка лотереи
             FD_ITEM_TYPE_RAFFLE_WIN_LOC = 8,          // выигрыш лотереи
             FD_ITEM_RID_LOC = 9,                             // предсоставление РИД
             FD_ITEM_PAYMENT_LOC = 10,                    // "ПЛАТЕЖ" или "П"(об авансе, задатке, предоплате, кредите)
             FD_ITEM_AGENCY_FEE_LOC = 11,              // агентское вознаграждение
             FD_ITEM_FEE_LOC = 12,                            // ВЫПЛАТА" или "В"  о взносе в счет оплаты, пени, штрафе, вознаграждении, бонусе и ином аналогичном предмете расчета
             FD_ITEM_OTHER_LOC = 13,                        // "ИНОЙ ПРЕДМЕТ РАСЧЕТА" или "ИПР"

             //      признак способа расчета (тег 1214) 
             FD_ITEM_PAYMENT_TYPE_EMPTY_LOC = 0,         // признак способа расчета
             FD_ITEM_PAYMENT_PREPAY_FULL_LOC = 1,      // Предоплата 100%
             FD_ITEM_PAYMENT_PREPAY_PART_LOC = 2,      // Частичная предоплата 
             FD_ITEM_PAYMENT_AVANS_LOC = 3,                  // Аванс
             FD_ITEM_PAYMENT_TOTAL_CALC_LOC = 4,        // Полный расчет
             FD_ITEM_PAYMENT_PART_CALC_LOC = 5,          // Частичный расчет и кредит
             FD_ITEM_PAYMENT_TRANS_CRED_LOC = 6,        // Передача в кредит
             FD_ITEM_PAYMENT_PAYM_CRED_LOC = 7,          // Оплата кредита

            //      мера количества предмета расчета
            FD_ITEM_MEASURE_UNIT_LOC = 0,                                                       // шт или ед
            FD_ITEM_MEASURE_GRAM_LOC = 10,                                                      // грам
            FD_ITEM_MEASURE_KG_LOC = 11,                                                        // килограм
            FD_ITEM_MEASURE_TON_LOC = 12,                                                       // тонны
            FD_ITEM_MEASURE_SM_LOC = 20,                                                        // сантиметр
            FD_ITEM_MEASURE_DM_LOC = 21,                                                        // дециметр
            FD_ITEM_MEASURE_METR_LOC = 22,                                                      // метр
            FD_ITEM_MEASURE_QSM_LOC = 30,                                                       // квадратный сантиметр
            FD_ITEM_MEASURE_QDM_LOC = 31,                                                       // квадратный дециметр
            FD_ITEM_MEASURE_QMETR_LOC = 32,                                                     // квадратный метр
            FD_ITEM_MEASURE_ML_LOC = 40,                                                        // милилитр
            FD_ITEM_MEASURE_LITR_LOC = 41,                                                      // литр
            FD_ITEM_MEASURE_CUBEM_LOC = 42,                                                     // куб метр
            FD_ITEM_MEASURE_KWH_LOC = 50,                                                       // киловатт часы
            FD_ITEM_MEASURE_GKL_LOC = 51,                                                       // гигакаллории
            FD_ITEM_MEASURE_DAY_LOC = 70,                                                       // сутки/дни
            FD_ITEM_MEASURE_HOUR_LOC = 71,                                                      // часы
            FD_ITEM_MEASURE_MIN_LOC = 72,                                                       // минуты
            FD_ITEM_MEASURE_SEC_LOC = 73,                                                       // секунды
            FD_ITEM_MEASURE_KBYTE_LOC = 80,                                                     // килобайты
            FD_ITEM_MEASURE_MBYTE_LOC = 81,                                                     // мегабайты
            FD_ITEM_MEASURE_GBYTE_LOC = 82,                                                     // гигабайты
            FD_ITEM_MEASURE_TBYTE_LOC = 83,                                                     // терабайты
            FD_ITEM_MEASURE_OTHER_LOC = 255;                                                     // другое




            
        public static readonly string[] FiscalOperationType = new string[]
        {
            "нет",
            "приход",
            "возврат прихода",
            "расход",
            "возврат расхода"
        };
        public static readonly string[] FiscalOperationTypeShirt = new string[]
        {
            "",
            "прих.",
            "в.п.",
            "расх.",
            "в.р."
        };

        public static Dictionary<int, string> DocControl = new Dictionary<int, string>
        {
            [FD_DC_OK] = SUCCESS_MSG,
            [FD_DC_CRITICAL_ERROR_BF] = "Невозможно оформить документ",
            [FD_DC_ERROR_NOT_ENOUTH_PAID_BF] = "Документ не оплачен",
            //[FD_DC_WARN_NDS_SUMS_BF] = "Суммы НДС не сходятся",
            [FD_DC_ERROR_BAD_ITEM_BF] = "Некорректный предмет расчета",
            [FD_DC_WARN_WARNED_ITEM_BF] = "Некритичная проблема в предмете расчета",
            [FD_DC_ERROR_OVERPAID_CRITICAL_BF] = "Переплата в документе",
            //[FD_DC_PAID_WRITE_DOWN_PENNIES_BF] = "округление/списание копеек",
            [FD_DC_ERROR_OVERPAID_WITH_CHANGE_BF] = "документ со сдачей"
        };

        public const string
            NO_DRIVER_FOUNDED = "Драйвер не найден",
            SUCCESS_MSG = "Ошибок нет",
            CONNECTION_ESTABLISHED = "Соединение установлено",
            CONNECTION_NOT_ESTABLISHED = "Соединение не установлено",
            BAD_SURVEY = "При опросе устройства возникли ошибки",
            DEFAULT_D_FORMAT = "dd.MM.yyyy",
            DEFAULT_DT_FORMAT = "dd.MM.yyyy HH:mm",
            DEFAULT_DT_FORMAT_FILENAME = "ddMMyyyy HHmm",
            EMPTY_FDOC = "Пустой документ",
            DEFAULT_CASHIER = "СИС. АДМИНИСТРАТОР",
            NOT_SUPPORTED_THIS_VER = "Функция отключена в этой версии",
            DOC_NOT_READED = "Документ не прочитан"
            ;

        protected MainForm _ui = null;

        protected bool _connected = false;

        protected int _shiftState = NONE;
        protected int _lastFD = NONE;
        protected int _ffdVer = NONE;
        protected int _chosenSno = NONE;
        protected bool _dontPrint = false;

        public int FfdVer
        {
            get { return _ffdVer; }
        }
        public int FfdFtagFormat
        {
            get
            {
                if (_ffdVer == FR_FFD100)
                    return 1;
                if (_ffdVer == FR_FFD105)
                    return 2;
                if (_ffdVer == FR_FFD110)
                    return 3;
                if (_ffdVer == FR_FFD120)
                    return 4;
                return NONE;
            }
        }

        public int LastFd
        {
            get { return _lastFD; }
        }

        public int ChosenSno
        {
            get
            {
                if (_chosenSno >= FR_REGISTERD_SNO_KEY)
                    return _chosenSno - FR_REGISTERD_SNO_KEY;
                else return _chosenSno;
            }
        }

        public static Dictionary<int, string> KKMInfoTransmitter = new Dictionary<int, string>()
        {
            [INFO_MESSAGE_KEY] = "",    //
            [FR_MODEL_KEY] = "",
            [FR_SERIAL_KEY] = "",
            [FR_TIME_KEY] = "",
            [FR_FIRMWARE_KEY] = "",
            [FR_SHIFT_STATE_KEY] = "",
            [FR_LAST_FD_NUMBER_KEY] = "",
            [FR_OFD_EXCHANGE_STATUS_KEY] = "",
            [FR_OWNER_USER_KEY] = "",
            [FR_OWNER_ADDRESS_KEY] = "",
            [FR_REGISTERD_SNO_KEY] = "",
            [FR_CONNECTION_SETTINGS_KEY] = "",
            [FR_LAST_ERROR_MSG_KEY] = "",
            [FR_FN_SERIAL_KEY] = "",
            [FR_REGFNS_KEY] = "",
            [FR_FFDVER_KEY] = "",
            [FR_CASHIER_NAME_KEY] = "",
            [FR_CASHIER_INN_KEY] = "",
            [FR_STATUS_MODE_KEY] = "",
        };

        public static readonly string[,] ItemTypeDscr = new string[,]
        {
            {"","не указано" },                     //  0
            {"Т","товар" },                         //  1
            {"АТ","подакцизный товар" },            //  2
            {"Р","Работа" },                        //  3
            {"У","Услуга" },                        //  4
            {"СА","ставка азартной игры" },         //  5
            {"ВА","выигрыш азартной игры" },        //  6
            {"СЛ","ставка/билет лотереи" },         //  7
            {"ВЛ","выигрыш лотереи" },              //  8
            {"РИД","предоставление РИД" },          //  9
            {"П","платеж" },                        // 10
            {"АВ","агенское вознаграждение" },      // 11
            {"В","выплата" },                       // 12
            {"ИПР","иной предмет расчета" },        // 13
            {"И.П.","имущественное право" },        // 14
            {"В.Д.","внерелеазационный доход" },    // 15
            {"С.В.","страховые взносы" },           // 16
            {"Т.С.","торговый сбор" },              // 17
            {"К.С.","курортный сбор" },             // 18
            {"ЗАЛ.","залог" },                      // 19
            {"РАСХ","расход" },                     // 20
            {"ВНОИ","ВЗНОСЫ НА ОПС ИП" },           // 21
            {"ВНО","ВЗНОСЫ НА ОПС" },               // 22
            {"ВНОИ","ВЗНОСЫ НА ОМС ИП" },           // 23
            {"ВНО","ВЗНОСЫ НА ОМС" },               // 24
            {"ВОСС","ВЗНОСЫ НА ОСС" },              // 25
            {"ПК","платеж казино" },                // 26
            {"","" },                               // 27
            {"","" },                               // 28
            {"","" },                               // 29
            {"АТНМ","подакциз. без кода марк." },   // 30
            {"АТН","подакциз. им код марк." },      // 31
            {"ТНМ","марк тов. без кода марк." },    // 32
            {"ТМ","марк тов. с кодом марк." },      // 33
        };
        public static readonly string[] ItemPaymentTypeDscr = new string[]
        {
            "0 Не выбрано",
            "1 Предоплата 100%",
            "2 Частичная предоплата",
            "3 Аванс",
            "4 Полный расчет",
            "5 Частичный расчет и кредит",
            "6 Передача в кредит",
            "7 Оплата кредита"
        };
        public static readonly string[] TaxRateDscr = new string[]
        {
            "",
            "НДС 20%",
            "НДС 10%",
            "НДС 20/120",
            "НДС 10/110",
            "НДС 0%",
            "без НДС",
            "НДС 5%",
            "НДС 7%",
            "НДС 5/105",
            "НДС 7/107",
        };
        public static Dictionary<int, string> TaxSystem = new Dictionary<int, string>
        {
            [0] = "не заполнено",
            [1] = "ОСНО", 
            [2] = "УСН Доход", 
            [4] = "УСН Д-Р", 
            [8] = "ЕНВД", 
            [16] = "ЕСХН", 
            [32] = "ПСН", 
        };
        public const string
            SNO_TRADITIONAL = "ОСНО",
            SNO_USN_DOHOD = "УСНД",
            SNO_USN_DR = "УСНДР",
            SNO_ESHN = "ЕСХН",
            SNO_PSN = "ПСН";

        public static string UserConversion(string urlitso)
        {
            string[,] sequentecesToReplase =
        {
            {"ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ", "ООО" },
            {"ЗАКРЫТОЕ АКЦИОНЕРНОЕ ОБЩЕСТВО", "ЗАО" },
            {"ОТКРЫТОЕ АКЦИОНЕРНОЕ ОБЩЕСТВО", "ОАО" },
            {"ИНДИВИДУАЛЬНЫЙ ПРЕДПРИНИМАТЕЛЬ", "ИП" }
        };

            urlitso = urlitso.ToUpper();
            for (int i = 0; i < 4; ++i)
            {
                urlitso = urlitso.Replace(sequentecesToReplase[i, 0], sequentecesToReplase[i, 1]);
            }
            return urlitso;
        }

        static char decimal_separator = 1.1.ToString().IndexOf(',') == -1 ? '.' : ',';
        public static string ReplaceBadDecimalSeparatorPoint(string s)
        {
            if (decimal_separator == '.' && s.IndexOf(',') != -1)
            {
                s = s.Replace(',', decimal_separator);
            }
            else if (decimal_separator == ',' && s.IndexOf('.') != -1)
            {
                s = s.Replace('.', decimal_separator);
            }
            return s;
        }

        public static bool CorrectInn(string inn)
        {
            if (inn != null)
            {
                if (inn.StartsWith(" ") || inn.EndsWith(" "))
                    inn = inn.Trim();
                if (inn.Length == 10)
                {
                    int ks1 = (
                        2 * (inn[0] - '0') +
                        4 * (inn[1] - '0') +
                        10 * (inn[2] - '0') +
                        3 * (inn[3] - '0') +
                        5 * (inn[4] - '0') +
                        9 * (inn[5] - '0') +
                        4 * (inn[6] - '0') +
                        6 * (inn[7] - '0') +
                        8 * (inn[8] - '0'))
                        % 11
                        % 10;
                    return ks1 == (inn[9] - '0');
                }
                if (inn.Length == 12)
                {
                    // проверка контрольной суммы ФЛ
                    int ksi1 = (
                        7 * (inn[0] - '0') +
                        2 * (inn[1] - '0') +
                        4 * (inn[2] - '0') +
                        10 * (inn[3] - '0') +
                        3 * (inn[4] - '0') +
                        5 * (inn[5] - '0') +
                        9 * (inn[6] - '0') +
                        4 * (inn[7] - '0') +
                        6 * (inn[8] - '0') +
                        8 * (inn[9] - '0'))
                        % 11
                        % 10;
                    int ksi2 = (
                        3 * (inn[0] - '0') +
                        7 * (inn[1] - '0') +
                        2 * (inn[2] - '0') +
                        4 * (inn[3] - '0') +
                        10 * (inn[4] - '0') +
                        3 * (inn[5] - '0') +
                        5 * (inn[6] - '0') +
                        9 * (inn[7] - '0') +
                        4 * (inn[8] - '0') +
                        6 * (inn[9] - '0') +
                        8 * (inn[10] - '0'))
                        % 11
                        % 10;
                    return (ksi1 == (inn[10] - '0')) && (ksi2 == (inn[11] - '0'));
                }
            }
            return false;
        }

        // признак отключения печати
        public bool DontPrint { get { return _dontPrint; } set { _dontPrint = value; } }

        // Соединение с фискальником
        abstract public bool Connect();

        // Освобождение фискальника
        abstract public void Disconnect();

        // Освобождение ресурсоов внешней библиотеки (для атола)
        abstract public void ReleaseLib();

        //Считывание статуса ККМ
        abstract public void ReadDeviceCondition();

        //Свойства драйвера
        abstract public bool ConnectionWindow();

        //строковое представление параметров связи
        abstract public string ConnectionReprezentation();

        //открытие смены
        abstract public bool OpenShift();

        //закрытие смены
        abstract public bool CloseShift();

        // Пробить чек
        abstract public bool PerformFD(FiscalCheque doc);

        // чтение документов
        public abstract FnReadedDocument ReadFD(int docNumber, bool parce = false);

        // Продолжить печать
        public abstract bool ContinuePrint();

        // Отменить документ
        public abstract bool  CancelDocument();

        // внесение наличных
        public abstract bool CashRefill(double sum, bool income = true);

        // установка даты
        // для устнановки даты необходимо чтобы первый параметр был равен нулю
        // 
        public abstract bool ChangeDate(int appendDay = 0, DateTime date = default);

        public bool IsConnected { get { return _connected; } }

        protected void RezultMsg(string message) { _ui.PushMessage(message); }

        protected void ClearInfo()
        {
            KKMInfoTransmitter[FR_TIME_KEY] = "";
            KKMInfoTransmitter[FR_MODEL_KEY] = "";
            KKMInfoTransmitter[FR_SERIAL_KEY] = "";
            KKMInfoTransmitter[FR_FIRMWARE_KEY] = "";
            KKMInfoTransmitter[FR_FN_SERIAL_KEY] = "";
            KKMInfoTransmitter[INFO_MESSAGE_KEY] = "";
            KKMInfoTransmitter[FR_OWNER_USER_KEY] = "";
            KKMInfoTransmitter[FR_SHIFT_STATE_KEY] = "";
            KKMInfoTransmitter[FR_REGISTERD_SNO_KEY] = "";
            KKMInfoTransmitter[FR_LAST_FD_NUMBER_KEY] = "";
            KKMInfoTransmitter[FR_OFD_EXCHANGE_STATUS_KEY] = "";
            KKMInfoTransmitter[FR_OWNER_ADDRESS_KEY] = "";
            KKMInfoTransmitter[FR_REGFNS_KEY] = "";
            KKMInfoTransmitter[FR_FFDVER_KEY] = "";
            KKMInfoTransmitter[FR_CASHIER_NAME_KEY] = "";
            KKMInfoTransmitter[FR_CASHIER_INN_KEY] = "";
        }

        public static Dictionary<int, string> fdDocTypes = new Dictionary<int, string>()
        {
            [NONE] = "Ошибка",
            [FTAG_FISCAL_DOCUMENT_TYPE_FISCAL_REPORT] = "Отчет о регистрации",
            [FTAG_FISCAL_DOCUMENT_TYPE_OPEN_SHIFT] = "Отчет об открытии смены",
            [FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE] = "Кассовый чек",
            [FTAG_FISCAL_DOCUMENT_TYPE_BSO] = "Бланк строгой отчетности",
            [FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_SHIFT] = "Отчѐт о закрытии смены",
            [FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_ARCHIVE] = "Отчет о закрытии фискального накопителя",
            [FTAG_FISCAL_DOCUMENT_TYPE_FISCAL_REPORT_CORRECTION_REREG] = "Отчѐт об изменении параметров регистрации",
            [FTAG_FISCAL_DOCUMENT_TYPE_CALCULATION_REPORT] = "Отчет о состоянии расчетов",
            [FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE] = "Чек коррекции ",
            [FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION] = "БСО Коррекции"
        };
        public static Dictionary<int, string> fdDocTypesShirt = new Dictionary<int, string>()
        {
            [NONE] = "Ошибка",
            [FTAG_FISCAL_DOCUMENT_TYPE_FISCAL_REPORT] = "Рег",
            [FTAG_FISCAL_DOCUMENT_TYPE_OPEN_SHIFT] = "От.См.",
            [FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE] = "Чек",
            [FTAG_FISCAL_DOCUMENT_TYPE_BSO] = "БСО",
            [FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_SHIFT] = "Z-отч.",
            [FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_ARCHIVE] = "Зак.ФН",
            [FTAG_FISCAL_DOCUMENT_TYPE_FISCAL_REPORT_CORRECTION_REREG] = "Перерег",
            [FTAG_FISCAL_DOCUMENT_TYPE_CALCULATION_REPORT] = "Сост.р.",
            [FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE] = "Ч.Кор. ",
            [FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION] = "БСО Кор."
        };


        public struct FnReadedDocument
        {
            public FnReadedDocument(int type, DateTime time, int number, double summ = 0, string fiscalSign = "", FiscalCheque cheque = null, List<FTag> tags = null )
            {
                /*
                 *  type 
                 *  0-й байт тип документа 3 чек, 31 чек коррекции итп...
                 *      * Оригинальное поведение
                 *  *************************************************** 
                 *      * Доработка для работы с ФН
                 *  3-й байт (65536 * [1:0]) признак получения квитанции ОФД
                 *  4-й байт (16777216 * [1-4]) тип операции 1 приход, 2 возврат прихода итп...
                 */
                if (type > 65536)
                {
                    OperationTypeInfo = (byte)(type / 16777216); 
                    type %= 16777216;
                    OfdSignInfo = (byte)(type / 65536);
                    type %= 65536;
                }
                else
                {
                    OperationTypeInfo = 0;
                    OfdSignInfo = 0;
                }
                this.Type = type;
                this.Time = time;
                this.Number = number;
                this.Summ = summ;
                this.FiscalSign = fiscalSign;
                this.Cheque = cheque;
                ReeprezentOL = "";
                Reprezent = "";
                Tags = tags;
                RebuildPrezentation();
            }
            public int Type,
                Number;
            public double Summ;
            public DateTime Time;
            public string FiscalSign,
                Reprezent,
                ReeprezentOL;
            public FiscalCheque Cheque;
            public List<FTag> Tags;
            public byte OfdSignInfo;       // 0 - no info, 1 not signed, 2 signed
            public byte OperationTypeInfo; // 0 - нет операции, 1 приход, 2 Возврат прихода, 3 - Расход, 4 - Возврат расхода
            public void RebuildPrezentation()
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sb1 = new StringBuilder();
                sb.Append(Number);
                sb1.Append(Number);
                sb.Append(' ');
                sb1.Append(' ');
                if (fdDocTypes.ContainsKey(Type))
                {
                    sb.Append(fdDocTypes[Type]);
                    sb1.Append(fdDocTypesShirt[Type]);
                }
                else
                {
                    sb.Append("Неизвестный тип документа");
                    sb1.Append(fdDocTypesShirt[0]);
                }
                if (Type == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE || 
                    Type == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE || 
                    Type == FTAG_FISCAL_DOCUMENT_TYPE_BSO || 
                    Type == FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION)
                {
                    if (OperationTypeInfo > 0)
                    {
                        sb.Append(" " + FiscalOperationType[OperationTypeInfo]);
                        sb1.Append(" " + FiscalOperationTypeShirt[OperationTypeInfo]);
                    }
                    sb.Append(" Сумма ");
                    sb.Append(Summ);
                    sb1.Append(" Сум ");
                    sb1.Append(Summ);
                }
                sb.AppendLine();
                sb.Append("Время ");
                sb1.Append(" Вр ");
                sb.Append(Time.ToString(DEFAULT_DT_FORMAT));
                sb1.Append(Time.ToString(DEFAULT_DT_FORMAT));
                if (!string.IsNullOrEmpty(FiscalSign))
                {
                    sb.Append("  ФП: ");
                    sb.Append(FiscalSign);
                }
                if (OfdSignInfo > 0)
                {
                    sb.AppendLine();
                }
                if (OfdSignInfo == 1)
                {
                    sb.Append("нет квитанции ОФД");
                    sb1.Append(" не отпр.");
                }
                if (OfdSignInfo == 2)
                {
                    sb.Append("получена квитанция ОФД");
                    sb1.Append(" отпр.");
                }
                Reprezent = sb.ToString();
                ReeprezentOL = sb1.ToString();
            }

            public static FnReadedDocument EmptyFD = new FnReadedDocument(NONE, DateTime.Now, 0);

            public bool Equals(FnReadedDocument fd)
            {
                if (this.Type == NONE && fd.Type == NONE) return true;
                return this.Type == fd.Type && this.Number == fd.Number && (this.Time - fd.Time).Minutes == 0 && this.Summ == fd.Summ && this.FiscalSign == fd.FiscalSign;
            }
        }

        public bool CorrectnessOfReading(FiscalCheque cheq)
        {
            if (cheq == null) return false;
            int docType = cheq.Document;
            if (docType != FD_DOCUMENT_NAME_CHEQUE ||
                docType != FD_DOCUMENT_NAME_CORRECTION_CHEQUE ||
                docType != FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE ||
                docType != FTAG_FISCAL_DOCUMENT_TYPE_BSO ||
                docType != FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION
                )
                return true; // сюда должны попадать открытие/закрытие смены, перерегистрация итп хотя возможно это лишний участок из-за фильтрации значения
            if (cheq.TotalSum < 0.00099)
            {
                LogHandle.ol("Нулевой чек");
                if (docType == FD_DOCUMENT_NAME_CHEQUE || (docType == FD_DOCUMENT_NAME_CORRECTION_CHEQUE && cheq.FFDVer > FR_FFD105))
                {
                    if (cheq.Items.Count == 0)
                    {
                        LogHandle.ol("Отсутсвуют предметы расчета");
                        return false;
                    }
                    double itemsSum = 0;
                    foreach (ConsumptionItem item in cheq.Items)
                        itemsSum += item.Sum;
                    if (itemsSum < 0.991)
                        return true;
                    else
                    {
                        LogHandle.ol("Сумма предметов расчета более 1р");
                        return false;
                    }
                }
                else
                    return true;
            }
            else
            {
                if (docType == FD_DOCUMENT_NAME_CHEQUE || (docType == FD_DOCUMENT_NAME_CORRECTION_CHEQUE && cheq.FFDVer>FR_FFD105))
                {
                    LogHandle.ol("Ненулевой чек");
                    if (Math.Abs(cheq.TotalSum - cheq.Cash - cheq.ECash - cheq.Prepaid - cheq.Credit - cheq.Provision) >= 0.01)
                    {
                        LogHandle.ol("Сумма оплат не совпадает с итогом");
                        return false;
                    }
                    if (cheq.Items.Count == 0)
                    {
                        LogHandle.ol("Empty items");
                        return false;
                    }
                    double itemsSum = 0;
                    foreach (ConsumptionItem item in cheq.Items)
                        itemsSum += item.Sum;
                    int intCheqTotal = (int)(cheq.TotalSum + 0.000001);
                    int intItemsSum = (int)(itemsSum + 0.000001);
                    if (intCheqTotal != intItemsSum)
                    {
                        LogHandle.ol("Сумма позиций расходится с итогом");
                        return false;
                    }
                }
                else if (docType == FD_DOCUMENT_NAME_CORRECTION_CHEQUE && cheq.FFDVer <= FR_FFD105)
                {
                    if( Math.Abs(cheq.TotalSum - cheq.Cash - cheq.ECash - cheq.Prepaid - cheq.Credit - cheq.Provision) > 0.999) return false;
                }

            }
            return true;
        }





        public static FnReadedDocument TranslateFtagsList(byte[] data, int documentName = FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE, List<FTag> fd = null)
        {
            int operationTypeInfo = 0, ofdSignInfo = 0;
            if (documentName > 65536)
            {
                operationTypeInfo = (byte)(documentName / 16777216);
                documentName %= 16777216;
                ofdSignInfo = (byte)(documentName / 65536);
                documentName %= 65536;
            }
            else
            {
                operationTypeInfo = 0;
                ofdSignInfo = 0;
            }

            if (documentName == FD_DOCUMENT_NAME_CHEQUE) 
            { 
                documentName = FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE; 
            }
            if (documentName == FD_DOCUMENT_NAME_CORRECTION_CHEQUE) 
            { 
                documentName = FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE; 
            }
            List<FTag> ftagRoot = null;
            List<FTag> ftags = fd == null ? FTag.FTLVParcer.ParseStructure(data) : fd;
            if (ftags.Count > 0)
            {
                if (ftags.Count == 1 && ftags[0].Type == FTag.FDataType.STLV)
                {
                    ftagRoot = ftags;
                    documentName = ftags[0].TagNumber;
                    if (ftags[0].Nested == null || ftags[0].Nested.Count == 0) goto BadData;
                    ftags = ftags[0].Nested;
                }
                else
                {
                    ftagRoot = new List<FTag>();
                    ftagRoot.Add(new FTag(documentName, ftags, false));
                }
                FnReadedDocument doc = new FnReadedDocument();
                doc.Type = documentName;
                doc.Tags = ftagRoot;
                doc.OfdSignInfo = (byte)ofdSignInfo;
                doc.OperationTypeInfo = (byte)operationTypeInfo;
                if (/*documentName == FiscalPrinter.FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE 
                    || documentName == FiscalPrinter.FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE 
                    || documentName == FiscalPrinter.FD_DOCUMENT_NAME_CHEQUE 
                    || documentName == FiscalPrinter.FD_DOCUMENT_NAME_CORRECTION_CHEQUE 
                    || documentName == FiscalPrinter.FTAG_FISCAL_DOCUMENT_TYPE_BSO 
                    || documentName == FiscalPrinter.FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION*/true)
                {
                    bool chequeRec = false;
                    if (documentName == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE
                    || documentName == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE
                    || documentName == FD_DOCUMENT_NAME_CHEQUE
                    || documentName == FD_DOCUMENT_NAME_CORRECTION_CHEQUE
                    || documentName == FTAG_FISCAL_DOCUMENT_TYPE_BSO
                    || documentName == FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION)
                    {
                        doc.Cheque = new FiscalCheque();
                        doc.Cheque.Document = documentName;
                        chequeRec = true;
                    }
                    
                    foreach (FTag tag in ftags)
                    {
                        switch (tag.TagNumber)
                        {
                            case FTAG_DESTINATION_EMAIL:
                                if(chequeRec)doc.Cheque.EmailPhone = tag.ValueStr;      //Скорей всего cheqRec лишнее
                                break;

                            case FTAG_RETAIL_PLACE_ADRRESS:
                                if (chequeRec) doc.Cheque.RetailAddress = tag.ValueStr;
                                break;
                            case FTAG_DATE_TIME:
                                doc.Time = tag.ValueDT;
                                break;
                            case FTAG_PROPERTIES_DATA:
                                if (chequeRec) doc.Cheque.PropertiesData = tag.ValueStr;     //Скорей всего cheqRec лишнее
                                break;
                            
                            case FTAG_DOC_FISCAL_SIGN:
                                
                                byte[] bytes = tag.RawData;
                                uint fs = 0;
                                if (bytes == null)
                                {
                                    if (tag.ValueInt > 0)
                                    {
                                        fs = tag.ValueInt;
                                    }
                                    else
                                    {
                                        uint.TryParse(tag.ValueStr, out fs);
                                    }
                                }
                                else
                                {
                                    int i = bytes.Length - 4;
                                    for (; i >= 0 && i < bytes.Length; i++)
                                    {
                                        fs += (uint)(bytes[i] * Math.Pow(256, bytes.Length - i - 1));
                                    }
                                }    

                                doc.FiscalSign = fs.ToString("D10");
                                break;
                            case FTAG_TOTAL_SUM:
                                doc.Summ = tag.ValueDouble;
                                if (chequeRec) doc.Cheque.TotalSum = tag.ValueDouble;
                                break;
                            case FTAG_CASHIER_NAME:
                                if (chequeRec) doc.Cheque.Cashier = tag.ValueStr;
                                break;
                            // пропускаем теги FTAG_ITEM_QUANTITY = 1023, FTAG_ITEM_NAME = 1030 обработка в 1059
                            case FTAG_CASH_TOTAL_SUM:
                                if (chequeRec) doc.Cheque.Cash = tag.ValueDouble;
                                break;
                            case FTAG_FD:
                                doc.Number = (int)tag.ValueInt;
                                break;
                            case FTAG_OPERATION_TYPE:
                                if (chequeRec) doc.Cheque.CalculationSign = (int)tag.ValueInt;
                                break;
                            case FTAG_APPLIED_TAXATION_TYPE:
                                if (chequeRec) doc.Cheque.Sno = (int)tag.ValueInt;
                                break;
                            case FTAG_ITEM:
                                string name = null,
                                    unit105 = null,
                                    code105 = null, 
                                    providerInn = null, 
                                    customEntryNum = null, 
                                    originalCountryCode = null,
                                    transferOperatorPhone = null,
                                    paymentAgentOperation = null,
                                    paymentAgentPhone = null,
                                    paymentOperatorPhone = null,
                                    transferOperatorName = null,
                                    transferOperatorAddress = null,
                                    transferOperatorInn = null,
                                    providerPhone = null,
                                    providerName = null;
                                double price = -1, quantity = -1, sum = 0, unitNnds = -1;
                                int paymentType = -1, productType = -1, ndsRate = -1, paymentAgentByProductType = -1,unitMeasure120 = -1;
                                /* 
                                 * 1023 количество предмета расчета FVLN (0,18446744073709551615]
                                 * 1030 наименование предметарасчета string [1,128]
                                 * 1043 стоимость предмета расчета с учетом скидок и наценок VLN  [0,281474976710655] стоимость предмета расчета с учетом скидок и наценок
                                 * 1162 Код товара 1.05
                                 * 1079 цена за единицу предмета расчета с учетомVLN    [0,281474976710655] 
                                 * 1197 единица ижмерения 1.05
                                 * 1199 ставка НДС BYTE {1, 2, 3, 4, 5, 6, 8, 9, 10, 11}
                                 * 1200 сумма НДС за предмет расчета [0,281474976710655] 
                                 * 1212 признак предмета расчета BYTE [1,27][30,33]
                                 * 1214 признак способа расчета BYTE {1,2,3,4,5,6,7} признак  способа расчета
                                 * 1222 Признак агента по предмету расчета
                                 * 1226 ИНН поставщика
                                 * 1230 Код страны происхождения товара
                                 * 1231 Номер таможенной декларации 
                                 * 2108 Мера предмета расчета
                                */
                                if (tag.Nested != null && tag.Nested.Count > 0)
                                {
                                    foreach (FTag tagItem in tag.Nested)
                                    {
                                        switch (tagItem.TagNumber)
                                        {
                                            case FTAG_ITEM_QUANTITY:
                                                quantity = tagItem.ValueDouble;
                                                break;
                                            case FTAG_ITEM_NAME:
                                                name = tagItem.ValueStr;
                                                break;
                                            case FTAG_ITEM_SUM:
                                                sum = tagItem.ValueDouble;
                                                break;
                                            case FTAG_ITEM_PRODUCT_CODE:
                                                code105 = tagItem.ValueStr;
                                                break;
                                            case FTAG_ITEM_PRICE:
                                                price = tagItem.ValueDouble;
                                                break;
                                            case FTAG_ITEM_UNIT_MEASURE_105:
                                                unit105 = tagItem.ValueStr;
                                                break;
                                            case FTAG_ITEM_NDS_RATE:
                                                ndsRate = (int)tagItem.ValueInt;
                                                break;
                                            case FTAG_ITEM_NDS_SUM:
                                                unitNnds = tagItem.ValueDouble;
                                                break;
                                            case FTAG_ITEM_PRODUCT_TYPE:
                                                productType = (int)tagItem.ValueInt;
                                                break;
                                            case FTAG_ITEM_PAYMENT_TYPE:
                                                paymentType = (int)tagItem.ValueInt;
                                                break;
                                            case FTAG_ITEM_PAYMENT_AGENT_BY_PRODUCT_TYPE:
                                                paymentAgentByProductType = (int)tagItem.ValueInt;
                                                break;
                                            case FTAG_ITEM_PROVIDER_INN:
                                                providerInn = tagItem.ValueStr;
                                                break;
                                            case FTAG_ITEM_ORIGINAL_COUNTRY_CODE:
                                                originalCountryCode = tagItem.ValueStr;
                                                break;
                                            case FTAG_ITEM_CUSTOM_ENTRY_NUM:
                                                customEntryNum = tagItem.ValueStr;
                                                break;
                                            case FTAG_ITEM_UNIT_MEASURE_120:
                                                unitMeasure120 = (int)tagItem.ValueInt;
                                                break;
                                            case FTAG_ITEM_PAYMENT_AGENT_DATA:
                                                FTag tpad;
                                                if (tagItem.Nested != null && tagItem.Nested.Count == 1 && tagItem.Nested[0].TagNumber == FTAG_ITEM_PAYMENT_AGENT_DATA)
                                                {
                                                    // баг разбора JSON
                                                    tpad = tagItem.Nested[0];
                                                }
                                                else
                                                {
                                                    tpad = tagItem;
                                                }
                                                foreach (FTag f in tpad.Nested)
                                                {
                                                    if (f.TagNumber == FTAG_PAD_TRANSFER_OPERATOR_PHONE)
                                                    {
                                                        transferOperatorPhone = f.ValueStr;
                                                    }
                                                    else if (f.TagNumber == FTAG_PAD_PAYPENT_AGENT_OPERATION)
                                                    {
                                                        paymentAgentOperation = f.ValueStr;
                                                    }
                                                    else if (f.TagNumber == FTAG_PAD_PAYMENT_AGENT_PHONE)
                                                    {
                                                        paymentAgentPhone = f.ValueStr;
                                                    }
                                                    else if (f.TagNumber == FTAG_PAD_PAYMENT_OPERATOR_PHONE)
                                                    {
                                                        paymentOperatorPhone = f.ValueStr;
                                                    }
                                                    else if (f.TagNumber == FTAG_PAD_TRANSFER_OPERATOR_NAME)
                                                    {
                                                        transferOperatorName = f.ValueStr;
                                                    }
                                                    else if (f.TagNumber == FTAG_PAD_TRANSFER_OPERATOR_ADDRESS)
                                                    {
                                                        transferOperatorAddress = f.ValueStr;
                                                    }
                                                    else if (f.TagNumber == FTAG_PAD_TRANSFER_OPERATOR_INN)
                                                    {
                                                        transferOperatorInn = f.ValueStr;
                                                    }
                                                }
                                                break;
                                            case FTAG_ITEM_PROVIDER_DATA:
                                                FTag tpd;
                                                if (tagItem.Nested != null && tagItem.Nested.Count == 1 && tagItem.Nested[0].TagNumber == FTAG_ITEM_PROVIDER_DATA)
                                                {
                                                    // баг разбора JSON
                                                    tpd = tagItem.Nested[0];
                                                }
                                                else
                                                {
                                                    tpd = tagItem;
                                                }
                                                foreach (FTag f in tpd.Nested)
                                                {
                                                    if (f.TagNumber == FTAG_PD_PROVIDER_PHONE)
                                                    {
                                                        providerPhone = f.ValueStr;
                                                    }
                                                    else if (f.TagNumber == FTAG_PD_PROVIDER_NAME)
                                                    {
                                                        providerName = f.ValueStr;
                                                    }
                                                }
                                                break;

                                            default:
                                                LogHandle.ol(tagItem.TagNumber + (FTag.fnsNames.ContainsKey(tag.TagNumber) ? "-" + FTag.fnsNames[tag.TagNumber] : "")+ " распознование тега не поддерживется внутри 1059-item");
                                                break;
                                        }
                                    }
                                    if ( (price >= 0 ||sum >= 0)&& quantity >= 0)
                                    {
                                        if(paymentType == -1 || paymentType == 0)
                                        {
                                            paymentType = FD_ITEM_PAYMENT_TOTAL_CALC_LOC;
                                        }
                                        ConsumptionItem item = new ConsumptionItem();
                                        item.Quantity = quantity;
                                        if (name != null) item.Name = name;
                                        if (price >= 0) item.Price = price;
                                        if (sum >= 0) item.Sum = sum;
                                        if (code105 != null) item.Code105 = code105;
                                        if (ndsRate > 0) item.NdsRate = ndsRate;
                                        if (unitNnds > 0) item.UnitNnds = unitNnds;
                                        if (productType > 0) item.ProductType = productType;
                                        if (paymentType > 0) { item.PaymentType = paymentType; }
                                        if (paymentAgentByProductType > 0) item.PaymentAgentByProductType = paymentAgentByProductType;
                                        if (providerInn != null) item.ProviderInn = providerInn;
                                        if (originalCountryCode != null) item.OriginalCountryCode = originalCountryCode;
                                        if (customEntryNum != null) item.CustomEntryNum = customEntryNum;
                                        if(unitMeasure120>-1)item.Unit120 = unitMeasure120;
                                        if (!string.IsNullOrEmpty(transferOperatorPhone))
                                        {
                                            item.TransferOperatorPhone = transferOperatorPhone;
                                        }
                                        if (!string.IsNullOrEmpty(paymentAgentOperation))
                                        {
                                            item.PaymentAgentOperation = paymentAgentOperation;
                                        }
                                        if (!string.IsNullOrEmpty(paymentAgentPhone))
                                        {
                                            item.PaymentAgentPhone = paymentAgentPhone;
                                        }
                                        if (!string.IsNullOrEmpty(paymentOperatorPhone))
                                        {
                                            item.PaymentOperatorPhone = paymentOperatorPhone;
                                        }
                                        if (!string.IsNullOrEmpty(transferOperatorName))
                                        {
                                            item.TransferOperatorName = transferOperatorName;
                                        }
                                        if (!string.IsNullOrEmpty(transferOperatorAddress))
                                        {
                                            item.TransferOperatorAddress = transferOperatorAddress;
                                        }
                                        if (!string.IsNullOrEmpty(transferOperatorInn))
                                        {
                                            item.TransferOperatorInn = transferOperatorInn;
                                        }
                                        if (!string.IsNullOrEmpty(providerPhone))
                                        {
                                            item.ProviderPhone = providerPhone;
                                        }
                                        if (!string.IsNullOrEmpty(providerName))
                                        {
                                            item.ProviderName = providerName;
                                        }

                                        doc.Cheque.Items.Add(item);
                                    }
                                    else
                                    {
                                        LogHandle.ol("Не удалось распознать предмет расчета");
                                        if (tag.RawData != null) LogHandle.ol(BitConverter.ToString(tag.RawData));
                                        goto BadData;
                                    }
                                }
                                break;

                            case FTAG_ECASH_TOTAL_SUM:
                                if (chequeRec) doc.Cheque.ECash = tag.ValueDouble;
                                break;
                            case FTAG_PRORERTIES_1084:
                                if (chequeRec)
                                {
                                    if (tag.Nested.Count == 2)
                                    {
                                        if (tag.Nested[0].TagNumber == FTAG_PROPERTIES_PROPERTY_NAME && tag.Nested[1].TagNumber == FTAG_PROPERTIES_PROPERTY_VALUE)
                                        {
                                            doc.Cheque.PropertiesPropertyName = tag.Nested[0].ValueStr;
                                            doc.Cheque.PropertiesPropertyValue = tag.Nested[1].ValueStr;
                                        }
                                        else if (tag.Nested[1].TagNumber == FTAG_PROPERTIES_PROPERTY_NAME && tag.Nested[0].TagNumber == FTAG_PROPERTIES_PROPERTY_VALUE)
                                        {
                                            doc.Cheque.PropertiesPropertyName = tag.Nested[1].ValueStr;
                                            doc.Cheque.PropertiesPropertyValue = tag.Nested[0].ValueStr;
                                        }
                                    }
                                }
                                break;
                            case FTAG_NDS20_DOCUMENT_SUM:
                                if (chequeRec) doc.Cheque.Nds20 = tag.ValueDouble;
                                break;
                            case FTAG_NDS10_DOCUMENT_SUM:
                                if (chequeRec) doc.Cheque.Nds10 = tag.ValueDouble;
                                break;
                            case FTAG_NDS0_DOCUMENT_SUM:
                                if (chequeRec) doc.Cheque.Nds0 = tag.ValueDouble;
                                break;
                            case FTAG_NDS_FREE_DOCUMENT_SUM:
                                if (chequeRec) doc.Cheque.NdsFree = tag.ValueDouble;
                                break;
                            case FTAG_NDS20120_DOCUMENT_SUM:
                                if (chequeRec) doc.Cheque.Nds20120 = tag.ValueDouble;
                                break;
                            case FTAG_NDS10110_DOCUMENT_SUM:
                                if (chequeRec) doc.Cheque.Nds10110 = tag.ValueDouble;
                                break;
                            /*
                             * новые ставки
                             */
                            case FTAG_AMOUNTS_RECEIPT_NDS:
                                if (chequeRec)
                                {
                                    List<FTag> newTaxSumms = tag.Nested;
                                    uint taxType = 10000;
                                    double taxAmount = 0;
                                    foreach (FTag t in newTaxSumms)
                                    {
                                        if (t.Nested != null || t.Nested.Count != 0)
                                        {
                                            foreach (FTag t2 in t.Nested)
                                            {
                                                if (t2.TagNumber == FTAG_AMOUNTS_NDS_NDSSUM)
                                                    taxAmount = t2.ValueDouble;
                                                else if (t2.TagNumber == FTAG_ITEM_NDS_RATE)
                                                {
                                                    taxType = t2.ValueInt;
                                                }
                                            }
                                            if (taxType < 9000 && taxAmount > 0)
                                            {
                                                if (taxType == NDS_TYPE_5_LOC)
                                                    doc.Cheque.Nds5 = taxAmount;
                                                else if (taxType == NDS_TYPE_7_LOC)
                                                    doc.Cheque.Nds7 = taxAmount;
                                                else if (taxType == NDS_TYPE_5105_LOC)
                                                    doc.Cheque.Nds5105 = taxAmount;
                                                else if (taxType == NDS_TYPE_7107_LOC)
                                                    doc.Cheque.Nds7107 = taxAmount;
                                            }
                                            else if (taxAmount<0.001)
                                            {
                                                //LogHandle.ol("Нулевые суммы НДС(5;7)");
                                            }
                                            else
                                            {
                                                LogHandle.ol("Ошибка в разборе сумм НДС чека " + t.ToString());
                                            }
                                        }
                                        else
                                        {
                                            LogHandle.ol("Проблема со структурой FTAG_AMOUNTS_RECEIPT_NDS 1115  " + t.ToString());
                                        }

                                    }
                                }
                                break;

                            case FTAG_INTERNET_PAYMENT:
                                if (chequeRec) doc.Cheque.InternetPayment = tag.ValueInt != 0;
                                break;
                            case FTAG_CORRECTION_TYPE:
                                doc.Cheque.CorrectionTypeNotFtag = (int)tag.ValueInt;
                                break;
                            case FTAG_CORRECTION_BASE:  // STLV
                                string describtion = null, orderNum = "";
                                DateTime corrDate = DateTime.MinValue;
                                if (tag.Nested != null)
                                {
                                    foreach (FTag corrTag in tag.Nested)
                                    {
                                        if (corrTag.TagNumber == FTAG_CORRECTION_DESCRIBER) describtion = corrTag.ValueStr;
                                        else if (corrTag.TagNumber == FTAG_CORRECTION_DOC_DATE) { corrDate = corrTag.ValueDT; if (doc.Cheque != null) doc.Cheque.CorrectionDocumentDate = corrTag.ValueDT; }
                                        else if (corrTag.TagNumber == FTAG_CORRECTION_ORDER_NUMBER) orderNum = corrTag.ValueStr;
                                    }
                                }
                                if (!string.IsNullOrEmpty(describtion)) doc.Cheque.CorrectionDocDescriber = describtion;
                                doc.Cheque.CorrectionOrderNumber = orderNum;
                                if (corrDate > new DateTime(2017, 1, 1)) doc.Cheque.CorrectionDocumentDate = corrDate;
                                break;

                            case FTAG_RETAIL_PLACE:
                                if (chequeRec) doc.Cheque.RetailPlace = tag.ValueStr;
                                break;
                            case FTAG_CASHIER_INN:
                                if (chequeRec) doc.Cheque.CashierInn = tag.ValueStr;
                                break;
                            case FTAG_FFD:
                                if (chequeRec) doc.Cheque.FFDVer = (int)tag.ValueInt;
                                break;
                            case FTAG_PREPAID_TOTAL_SUM:
                                if (chequeRec) doc.Cheque.Prepaid = tag.ValueDouble;
                                break;
                            case FTAG_CREDIT_TOTAL_SUM:
                                if (chequeRec) doc.Cheque.Credit = tag.ValueDouble;
                                break;
                            case FTAG_PROVISION_TOTAL_SUM:
                                if (chequeRec) doc.Cheque.Provision = tag.ValueDouble;
                                break;
                            case FTAG_BUYER_INFORMATION_BUYER:
                                if (chequeRec) doc.Cheque.BuyerInformationBuyer = tag.ValueStr;
                                break;
                            case FTAG_BUYER_INFORMATION_BUYER_INN:
                                doc.Cheque.BuyerInformationBuyerInn = tag.ValueStr;
                                break;
                            case FTAG_BUYER_INFORMATION:
                                //doc.cheque.BuyerInformation120 = true;
                                if (tag.Nested != null)
                                {
                                    if(tag.Nested.Count == 1 && tag.Nested[0].TagNumber == FTAG_BUYER_INFORMATION && tag.Nested[0].Nested != null)
                                    {
                                        /*foreach (FTag buyerTag in tag.Nested[0].Nested)
                                        {
                                            // хотфикс для раборки JSON
                                            if (buyerTag.TagNumber == FTAG_BUYER_INFORMATION_BUYER) doc.Cheque.BuyerInformationBuyer = buyerTag.ValueStr;
                                            else if (buyerTag.TagNumber == FTAG_BUYER_INFORMATION_BUYER_INN) doc.Cheque.BuyerInformationBuyerInn = buyerTag.ValueStr;
                                            else if (buyerTag.TagNumber == FTAG_BI_BIRTHDAY) doc.Cheque.BuyerInformationBuyerBirthday = buyerTag.ValueStr;
                                            else if (buyerTag.TagNumber == FTAG_BI_DOCUMENT_CODE) doc.Cheque.BuyerInformationBuyerDocumentCode = buyerTag.ValueStr;
                                            else if (buyerTag.TagNumber == FTAG_BI_DOCUMENT_DATA) doc.Cheque.BuyerInformationBuyerDocumentData = buyerTag.ValueStr;
                                            else if (buyerTag.TagNumber == FTAG_BI_ADDRESS) doc.Cheque.BuyerInformationBuyerAddress = buyerTag.ValueStr;
                                            else if (buyerTag.TagNumber == FTAG_BI_CITIZENSHIP) doc.Cheque.BuyerInformationBuyerCitizenship = buyerTag.ValueStr;
                                        }*/
                                        tag.Nested = tag.Nested[0].Nested;
                                    }

                                    foreach (FTag buyerTag in tag.Nested)
                                    {
                                        if (buyerTag.TagNumber == FTAG_BUYER_INFORMATION_BUYER) doc.Cheque.BuyerInformationBuyer = buyerTag.ValueStr;
                                        else if (buyerTag.TagNumber == FTAG_BUYER_INFORMATION_BUYER_INN) doc.Cheque.BuyerInformationBuyerInn = buyerTag.ValueStr;
                                        else if (buyerTag.TagNumber == FTAG_BI_BIRTHDAY) doc.Cheque.BuyerInformationBuyerBirthday = buyerTag.ValueStr;
                                        else if (buyerTag.TagNumber == FTAG_BI_DOCUMENT_CODE) doc.Cheque.BuyerInformationBuyerDocumentCode = buyerTag.ValueStr;
                                        else if (buyerTag.TagNumber == FTAG_BI_DOCUMENT_DATA) doc.Cheque.BuyerInformationBuyerDocumentData = buyerTag.ValueStr;
                                        else if (buyerTag.TagNumber == FTAG_BI_ADDRESS) doc.Cheque.BuyerInformationBuyerAddress = buyerTag.ValueStr;
                                        else if (buyerTag.TagNumber == FTAG_BI_CITIZENSHIP) doc.Cheque.BuyerInformationBuyerCitizenship = buyerTag.ValueStr;
                                    }

                                        
                                }

                                break;
                            default:
                                int tn = tag.TagNumber;
                                if(tn!= 1041
                                    && tn!= 1037
                                    && tn!= FTAG_USER_INN
                                    && tn!= 1056
                                    && tn!= 1002
                                    && tn!= 1001
                                    && tn!= 1109
                                    && tn!= 1108
                                    && tn!= FTAG_REGISTERED_SNO
                                    && tn!= 1017
                                    && tn!= 1046
                                    && tn!= FTAG_USER
                                    //&& tn!= FTAG_RETAIL_PLACE_ADRRESS
                                    && tn!= FTAG_SELLER_ADDRESS
                                    //&& tn!= 1187
                                    && tn!= 1207
                                    && tn!= 1193
                                    && tn!= 1126
                                    && tn!= 1221
                                    && tn!= 1060
                                    && tn!= FTAG_KKT_NUMBER_FACTORY
                                    && tn!= 1189
                                    && tn!= FTAG_KKT_VERSION
                                    && tn!= 1042
                                    && tn!= 1038
                                    && tn!= FTAG_FN_NUMBER
                                    ) 
                                {
                                    LogHandle.ol(tag.TagNumber + (FTag.fnsNames.ContainsKey(tag.TagNumber) ? "-" + FTag.fnsNames[tag.TagNumber] : "") + " тег чека не поддерживется или не влияет на перепроведение ФД");
                                }
                                break;
                        }

                    }
                }
                if (doc.Cheque != null)
                {
                    if (AppSettings.AppendFiscalSignAsPropertyData)
                    {
                        if (AppSettings.OverridePropertyData)
                            doc.Cheque.PropertiesData = doc.FiscalSign;
                        else
                        {
                            if (!doc.Cheque.IsPropertiesData)
                                doc.Cheque.PropertiesData = doc.FiscalSign;
                        }
                    }
                    if(AppSettings.OverrideCorrectionOrderNumber)
                        doc.Cheque.CorrectionOrderNumber = AppSettings.CorrectionOrderNumberDefault;
                    if (AppSettings.OverrideCorrectionDocumentDate || doc.Cheque.Document == FD_DOCUMENT_NAME_CHEQUE)
                        doc.Cheque.CorrectionDocumentDate = doc.Time;

                    doc.OperationTypeInfo = (byte)doc.Cheque.CalculationSign;
                }
                
                doc.RebuildPrezentation();
                return doc;
            }

        BadData:
            return FnReadedDocument.EmptyFD;
        }
    }
}

