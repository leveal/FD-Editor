using DrvFRLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static FR_Operator.FTag;

namespace FR_Operator
{
    internal class TerminalFnExchange :FiscalPrinter
    {
        protected SerialPort _port;
        TerminalUi _terminalUi;
        public const int
            SENDING_DATA_ERROR = -32,
            RECEVING_DATA_ERROR = -16,
            CLOSING_ERROR = -8,
            INTERRUPTED = -4,
            OK = 0,
            NOT_SET = -3,
            NOT_OPENED = -2;

        const int _SERVICE_CODE_TIMEOUT_OFF = -1;
        Dictionary<int, string> uartRezultPrezentation = new Dictionary<int, string>()
        {
            [SENDING_DATA_ERROR] = "Ошибка при отправке данных",
            [RECEVING_DATA_ERROR] = "Ошибка при получении данных",
            [CLOSING_ERROR] = "Ошибка закрытия порта",
            [INTERRUPTED] = "Прервано",
            [OK] = "Ошибок нет",
            [NOT_SET] = "Некорректная настройка или номер порта",
            [NOT_OPENED] = "Порт не открыт",
            [_SERVICE_CODE_TIMEOUT_OFF] = "Истек таймаут операции"
        };

        private const uint _MIN_TAG_NUMBER = 1000;
        private const uint _MAX_TAG_NUMBER = 3000;


        const byte START_SIGN = 0x4;

        /*
         * подготовленные команды для запросов без параметров
         */
        static readonly byte[] CMD_GET_FD_TLV_STRUCT = new byte[6] { START_SIGN, (byte)0x1, (byte)0x0, CODE_GET_FD_TLV_STRUCT, (byte)0xAE, (byte)0xD3 };
        static readonly byte[] CMD_GET_REG_TLV_STRUCT = new byte[6] { START_SIGN, (byte)0x1, (byte)0x0, CODE_GET_REG_TLV_STRUCT, (byte)0x8F, (byte)0xC3 };
        static readonly byte[] CMD_GET_FN_STATUS = new byte[6] { START_SIGN, (byte)0x1, (byte)0x0, CODE_GET_FN_STATUS, (byte)0xFF, (byte)0xCD };
        static readonly byte[] CMD_GET_FN_EXPIRATION = new byte[6] { START_SIGN, 1, 0, CODE_GET_FN_EXPIRATION, 0xBD, 0xED };
        static readonly byte[] CMD_ABORT_DOCUMENT = new byte[] { START_SIGN, 0x1, 0x0, CODE_ABORT_FD, 0x6A, 0x9B };
        static readonly byte[] CMD_GET_OFD_EXCHANGE = new byte[6] { START_SIGN, 1, 0, CODE_GET_EXCHANGE_STATUS, 0xCE, 0xDF };
        static readonly byte[] CMD_OPEN_SHIFT = new byte[6] { START_SIGN,1,0,CODE_PERFORM_OPEN_SHIFT,0xDF,0xC9 };
        static readonly byte[] CMD_CLOSE_SHIFT = new byte[6] { START_SIGN, 1, 0, CODE_PERFORM_CLOSE_SHIFT, 0x19, 0xA9 };
        static readonly byte[] CMD_GET_SHIFT_PARAMS = new byte[6] { START_SIGN, 0x1, 0x0, CODE_GET_SHIFT_PARAMS, 0x9D, 0xE9 };
        static readonly byte[] CMD_PERFORM_CALCULATION_REPORT = new byte[6] { START_SIGN, 0x1, 0x0, CODE_PERFORM_CALCULATION_REPORT, 0xB4, 0x78 };

        //коды описания ФН отсутсвующие в ФФД
        public const int FNDESCR_STAGE_OF_APPLICATION = 100000;
        public const int FNDESCR_OPENED_DOC = 100001;
        public const int FNDESCR_SHIFT_STATE = 100002;
        public const int FNDESCR_ALARMING_FLAGS = 100003;
        public const int FNDESCR_LAST_PERFORMED_FD_TIME = 100004;
        public const int FNDESCR_LAST_PERFORMED_FD_NUMBER = 100005;
        public const int FNDESCR_EXPIRATION_DATE = 100006;
        public const int FNDESCR_EXPIRATION_PERFORMED_REGS = 100007;
        public const int FNDESCR_EXPIRATION_AVAILABLE_REGS = 100008;
        public const int FNDESCR_OFD_EXCHANGE_STATUS = 100020;                      
        public const int FNDESCR_OFD_EXCHANGE_STATUS_EXT = 100021;                      
        public const int FNDESCR_OFD_EXCHANGE_ACTIVE = 100022;
        public const int FNDESCR_OFD_EXCHANGE_UNSENT_DOCS = 100023;                 // эти теги есть но с разными номерами
        public const int FNDESCR_OFD_EXCHANGE_UNSENT_FIRST_DOC_NUM = 100024;        // эти теги есть но с разными номерами
        public const int FNDESCR_OFD_EXCHANGE_UNSENT_FIRST_DOC_TIME = 100025;       // эти теги есть но с разными номерами
        public const int FNDESCR_OFD_EXCHANGE_FD_STRING = 100026;
        
        //public const int FNDESCR_REGISRATION_FFD = 101209;                          // для панели регмистрация

        public const int DOWN_LEFT_MSG = 110000;        // interface describer

        public static Dictionary<int, string> FnAnswerCode = new Dictionary<int, string>()
        {
            [-1] = "Истек таймаут операции", // 
            [0] = "OK",
            [0x1] = "Неизвестная команда, неверный формат посылки или неизвестные параметры",
            [0x2] = "Другое состояние ФН",
            [0x3] = "Отказ ФН",
            [0x4] = "Отказ КС",
            [0x5] = "Параметры команды не соответствуют сроку жизни ФН",
            [0x7] = "Некорректная дата и/или время",
            [0x8] = "Нет запрошенных данных",
            [0x9] = "Некорректное значение параметров команды",
            [0x0A] = "В данном режиме функционирования ФН команда не разрешена(только для ФФД-1.1)",
            [0x0B] = "Во входящем сообщении команды 07h переданы в ФН данные, которые должен формировать ФН",
            [0x0C] = "Дублирование данных",
            [0x0D] = "Отсутствуют необходимые данные",
            [0x0E] = "Количество позиций в документе, превысило допустимый предел",
            [0x10] = "Превышение размеров TLV данных",
            [0x11] = "Нет транспортного соединения",
            [0x12] = "Исчерпан ресурс ФН",
            [0x14] = "Ограничение ресурса ФН",
            [0x16] = "Смена истекла",
            [0x17] = "Некорректные данные о промежутке времени между фискальными документами",
            [0x18] = "Некорректный реквизит, передан в ФН",
            [0x19] = "Реквизит не соответствует установкам при регистрации",
            [0x20] = "Ответ не может быть принят",
            [0x23] = "Отрицательный ответ сервиса обновления ключей проверки КМ",
            [0x24] = "Неизвестный ответ сервиса обновления ключей проверки кодов проверки",
            [0x30] = "Требуется повтор процедуры обновления ключей проверки КМ",
            [0x32] = "Запрещена работа с маркированным товарами",
            [0x33] = "Нарушена правильная последовательность подачи команд для обработки маркированных товаров",
            [0x34] = "Работа с маркированными товарами временно заблокирована",
            [0x35] = "Переполнена таблица проверки кодов маркировки",
            [0x36] = "Превышен период 90 дней со времени последнего обновления ключей проверки",
            [0x3C] = "В блоке TLV отсутствуют необходимые реквизиты",
            [0x3E] = "В реквизите 2007 содержится КМ, который ранее не проверялся в ФН"

        };

        /*
         * Список команд ФН
         */
        const byte CODE_BEGIN_REG_REPORT_105 = 0x2;
        const byte CODE_PERFORM_REG_REPORT_105 = 0x3;
        const byte CODE_BEGIN_CLOSE_FN_REPORT = 0x4;
        const byte CODE_PERFORM_CLOSE_FN_REPORT = 0x5;

        const byte CODE_ABORT_FD = 0x6;
        const byte CODE_SEND_TLV_LIST = 0x7;

        const byte CODE_GET_SHIFT_PARAMS = 0x10;
        const byte CODE_BEGIN_OPEN_SHIFT = 0x11;
        const byte CODE_PERFORM_OPEN_SHIFT = 0x12;
        const byte CODE_BEGIN_CLOSE_SHIFT = 0x13;
        const byte CODE_PERFORM_CLOSE_SHIFT = 0x14;
        const byte CODE_BEGIN_RECEIPT = 0x15;
        const byte CODE_PERFORM_RECEIPT_AND_CORRECTION = 0x16;
        const byte CODE_BEGIN_CORRECTION_RECEIPT = 0x17;
        const byte CODE_BEGIN_CALCULATION_REPORT = 0x18;
        const byte CODE_PERFORM_CALCULATION_REPORT = 0x19;

        const byte CODE_GET_EXCHANGE_STATUS = 0x20;

        const byte CODE_GET_FN_STATUS = 0x30;    //
        const byte CODE_GET_FN_NUMBER = 0x31;    // Обновляет FnInfo[FTAG_FN_NUMDER]
        const byte CODE_GET_FN_EXPIRATION = 0x32;
        const byte CODE_GET_FN_VERSION = 0x33;

        const byte CODE_GET_FN_FORMAT = 0x3A;

        const byte CODE_GET_FN_IMPLEMENTATION = 0x3F;

        const byte CODE_GET_ARCHIVED_FD_INFO = 0x40;
        const byte CODE_GET_FD_ODF_SIGN = 0x41;
        const byte CODE_GET_QUANTITY_NOT_SIGNET_FD = 0x42;

        const byte CODE_FN_REG_INFO = 0x43;
        const byte CODE_FN_REG_PARAM = 0x44;
        const byte CODE_GET_FD_ROOT_STLV_HEAD = 0x45;
        const byte CODE_GET_FD_TLV_STRUCT = 0x46;
        const byte CODE_GET_REG_TLV_STRUCT = 0x47;

        /*
         *  описание команды ФН для интерфей
         *  0 - Название комманды str
         *  1 - Входные параметры
         *  2 - Выходные данные
         */
        public static Dictionary<byte, object[]> CommandDescriber = new Dictionary<byte, object[]>()
        {
            [CODE_BEGIN_REG_REPORT_105] = new object[] { "Начать отчет о регистрации/перерегистрации ФФД 1.05", "Тип отчета Byte", "EMPTY" },
            [CODE_PERFORM_REG_REPORT_105] = new object[] { "Сформировать отчет о регистрации/перерегистрации ФФД-1.0 или ФФД 1.05", "ДатаВремя Bytes_5, Рег.номер ККТ ASCII_Bytes_20, Код СНО Byte, Режим работы Byte [Для перерегистраций без замены ФН Код причины перерегистрации Byte]", "Номер ФД Uint_32, ФПД Bytes_4" },
            [CODE_BEGIN_CLOSE_FN_REPORT] = new object[] { "Начать закрытие ФН", "EMPTY", "EMPTY" },
            [CODE_PERFORM_CLOSE_FN_REPORT] = new object[] { "Закрыть ФН", "ДатаВремя Bytes_5, Рег.номер ASCII_Bytes_20", "Номер ФД Uint_32, ФПД Bytes_4" },
            [CODE_ABORT_FD] = new object[] { "Отменить ФД", "EMPTY", "EMPTY" },
            [CODE_SEND_TLV_LIST] = new object[] { "Передать данные в формате TLV(list)", "TLV_LIST", "EMPTY" },

            [CODE_GET_SHIFT_PARAMS] = new object[] { "Запрос параметров текущей смены", "EMPTY", "Состояние смены Byte, Номер смены Uint16, Номер чека Uint16" },
            [CODE_BEGIN_OPEN_SHIFT] = new object[] { "Начать открытие смены", "ДатаВремя Bytes_5", "EMPTY" },
            [CODE_PERFORM_OPEN_SHIFT] = new object[] { "Открыть смену", "EMPTY", "Номер смены:Uint16, Номер ФД:Uint32, ФП:Uint32LE [Флаги предупреждений для ФФД 1.1 и 1.2:Byte]" },
            [CODE_BEGIN_CLOSE_SHIFT] = new object[] {"Начать закрытие смены", "ДатаВремя Bytes_5", "EMPTY" },
            [CODE_PERFORM_CLOSE_SHIFT] = new object[] {"Закрыть смену", "EMPTY", "Номер смены:Uint16, Номер ФД:Uint32, ФП:Uint32LE [Флаги предупреждений для ФФД 1.1 и 1.2:Byte]" },
            [CODE_BEGIN_RECEIPT] = new object[] {"Начать формирование чека", "ДатаВремя Bytes_5", "EMPTY" },
            [CODE_PERFORM_RECEIPT_AND_CORRECTION] = new object[] {"Cформировать чек", "ДатаВремя(печать) Bytes_5, Признак расчета Byte, Итог чека Uint40(LE)", "Номер чека за смену Uint16, Номер ФД Uint32, ФПД Uint32" },
            [CODE_BEGIN_CORRECTION_RECEIPT] = new object[] { "Начать формирование чека коррекции", "ДатаВремя Bytes_5", "EMPTY" },
            [CODE_BEGIN_CALCULATION_REPORT] = new object[] { "Начать формирование отчета о состоянии расчетов", "ДатаВремя Bytes_5", "EMPTY" },
            [CODE_PERFORM_CALCULATION_REPORT] = new object[] { "Сформировать отчет о состоянии расчетов", "EMPTY", "Номер ФД:Uint32, ФП:Uint32LE, К-во непереданных Uint32, Дата непереданного ФД Byte3" },

            [CODE_GET_EXCHANGE_STATUS] = new object[] { "Cтатус инф. обмена", "EMPTY", "FN_EXCHANGE_STATUS_BYTES2_MESSAGES_UINT16_UNSENT_FD_FIRST_UINT32_DTFROM_BYTES5" },

            [CODE_GET_FN_STATUS] = new object[] { "Запрос статуса ФН", "EMPTY", "FN_STATUS_BYTES5_DT_BYTSE5_FN_STRING16_LASTFD_UINT32" },
            [CODE_GET_FN_NUMBER] = new object[] { "Запрос № ФН", "EMPTY", "Строка_16" },
            [CODE_GET_FN_EXPIRATION] = new object[] { "Запрос срока действия ФН", "EMPTY", "DATE_BYTES3_AVAILABLE_REGS_BYTE_DONE_REGS_BYTE" },

            [CODE_GET_FN_VERSION] = new object[] { "Запрос вер. ФН", "EMPTY", "FN_VERSION_STRING16_BYTE" },

            [CODE_GET_FN_FORMAT] = new object[] { "Запрос формата ФН", "EMPTY", "FFD_VER_BYTE_FFD_MAX_BYTE" },

            [CODE_GET_FN_IMPLEMENTATION] = new object[] { "Запрос исп. ФН", "EMPTY", "FN_IMPL_STRING48" },

            [CODE_GET_ARCHIVED_FD_INFO] = new object[] { "Найти фискальный документ по номеру", "ФД:Uint32", "Код ФД:Byte наличие квитанции:Byte данные документа DataN" },
            [CODE_GET_FD_ODF_SIGN] = new object[] { "Запрос квитанции о получении фискального документа ОФД по номеру документа","Номер ФД Uint32", "DataN" },
            [CODE_GET_QUANTITY_NOT_SIGNET_FD] = new object[] { "– Запрос количества ФД, на которые нет квитанции", "EMPTY", "Количество неподтверждённых ФД Uint16" },
            [CODE_FN_REG_INFO] = new object[] { "Запрос итогов рег. ФН", "_VARIANT_EMPTY__VARIANT_REG_NUM_BYTE", "REG_FN_DESCRIBTION" },
            [CODE_FN_REG_PARAM] = new object[] { "Запрос параметра открытия/перерегистрации ФН", "Вариант 1: № тега:Uint16; Вариант 2:№ регистрации:Ubyte, № тега:Uint16(FFFFh-все параметры)", "TLV структура; если вариант 2 teg№=FFFFh отсутствуют"},
            [CODE_GET_FD_ROOT_STLV_HEAD] = new object[] { "Запрос ФД в TLV формате", "ФД:Uint32", "Заголовок STLV - КОД_ФД:Uint16 Размер_ФД:Uint16" },
            [CODE_GET_FD_TLV_STRUCT] = new object[] { "Чтение TLV ФД", "EMPTY", "TLV структура" },
            [CODE_GET_REG_TLV_STRUCT] = new object[] { "Чтение TLV ФД", "EMPTY", "TLV структура" }
        };
        
        /*
         * для признака соединение должно выполняться 2 условия
         * 1-е открытый ком порт
         * 2-е ФН должен отвечать на запрос статуса и номер ФН должен парсится как число long
         */
        static string _tfn_lib_version = "1";

        bool[] CheckSendedFtags(List<FTag> ftags)
        {
            bool exeedMaxNumber = false;
            bool lowerMinNumber = false;

            foreach (var f in ftags)
            {
                if (f.TagNumber > _MAX_TAG_NUMBER)
                {
                    exeedMaxNumber = true;
                }
                if (f.TagNumber < _MIN_TAG_NUMBER)
                {
                    lowerMinNumber = true;
                }
                if (f.Type == FDataType.STLV)
                {
                    bool[] badFlags = CheckSendedFtags(f.Nested);
                    if (badFlags[0])
                    {
                        lowerMinNumber = true;
                    }
                    if (badFlags[1])
                    {
                        exeedMaxNumber = true;
                    }
                }
                if (lowerMinNumber && exeedMaxNumber)
                    break;
            }
            return new bool[] { lowerMinNumber, exeedMaxNumber };
        }


        public const string NO_FD_RULE_SET = "Отсутствуют правила формирования ФД.";
        string BAD_DATA_INTEGRITY = "Нарушена целостьность данных.";
        private static long _idRequest;         // идентификатор uart-сообщения
        private bool _deviceAvailable = false;
        private List<byte[]> _buffer_level_0 = new List<byte[]>();      // буфер для объединения разорванных ответов ФН
        private List<byte> _buffer_level_1 = null;                      // буфер для сбора тегов в tlv-raw-data
        List<FTag> ftagList = new List<FTag>();                         // сюда попадают разобранные tlv ФД
        List<FnReadedDocument> fdReaded = new List<FnReadedDocument>(); // для считынных данных из архива ФН

        public Dictionary<int,FTag> RegFTags = new Dictionary<int, FTag>();   //  Регистрационные параметры ФН
        public FTag GetRegFtag(int tagNumber)
        {
            FTag f = null;
            if (RegFTags.ContainsKey(tagNumber))
                return RegFTags[tagNumber];
            return f;
        }

        
        string _fnOperationMessage = "";

        private int _chequeRequest = 0;

        private int _shiftNumber = 0;

        private int _stageOfUsage = -1;         // Этап применения ФН
        public int StageOfUsage { get => _stageOfUsage; }

        /*
         * *******************************************************
         * * Основной метод для установки времени формируемых ФД *
         * *******************************************************
         *                          ToDo
         * реализовать различные варианты возврата даты-времени:
         * 1й вариант текущее время компьютера - сделано
         * 2й вариант статичное время настраиваемое где-то в интерфейсе  - сделано
         * 3й смещение от текущего (установить статичное в интерфейсе расчитать смещение и сдвигать относительно текущего)  - сделано
         *    доп. реализовать ход времени в интерфейсе - сделано
         * 4й запрашивать и возвращать время последнего ФД (для моего случая важная опция)
         *    доп. смещать относительно него на дни/часы/минуты  (для моего случая важная опция)
         *    
         *    доп. для всех кроме первого при работе с этой настройкой в MainForm выводить диалог о разнице с текущем временем
         *    не диалог а вывод на титле
         */
        public DateTime GetTimeForFd
        {
            get
            {

                //return new DateTime(2024, 03, 30);
                DateTime now = DateTime.Now;
                if (_timeSource == 0)
                {
                    if ((now - _minAvailableTimeFd).TotalMinutes < 0)
                        _dtErrorSet = true;
                    return DateTime.Now;
                }
                    
                if(_shiftLaunched)
                {
                    DateTime timeSh1 = now.Add(_spanShiftAbs.Negate());
                    if ((timeSh1 - _minAvailableTimeFd).TotalMinutes < 0)
                    {
                        _dtErrorSet = true;
                        return now;
                    }
                    return timeSh1;
                }
                if(_timeSource == 1)
                {
                    if ((_shiftedAbsTime - _minAvailableTimeFd).TotalMinutes < 0)
                    {
                        _dtErrorSet = true;
                        return now;
                    }
                    return _shiftedAbsTime;
                }
                if(_timeSource == 2)
                {
                    if (_lastFdTime.CompareTo(_minAvailableTimeFd) < 0)
                    {
                        _timeSource = 0;
                        return DateTime.Now;
                    }
                    DateTime lastFdTime = _lastFdTime.AddMilliseconds(1);
                    if((_lastFdTime - _minAvailableTimeFd).TotalSeconds < 0)
                    {
                        lastFdTime = DateTime.Now;
                    }
                    if((lastFdTime - _minAvailableTimeFd).TotalMinutes < 0)
                    {
                        _dtErrorSet = true;
                        return now;
                    }
                    return _lastFdTime.AddMinutes(_fdPlusMins);
                }
                
                //BadDateShift:
                // Сюда ходить не должны
                LogAddEvent(new UartEvent(MsgSource.ERROR,null, "Ошибка при получении времени - передаем текущее время ПК"));
                return DateTime.Now;
            }
        }
        //DateTime dtFrom = DateTime.MinValue;
        private DateTime _minAvailableTimeFd = new DateTime(2024,01,01);
        bool _dtErrorSet = false;

        
        /*
         * Данный блок неактуален позже проверить и удалить
         * основной источник времени _timeSource
         * 0 - текущее время 
         * 1 - настраивамое абсолютное значение
         * 2 - смещение относительно последнего ФД
         * _shiftLaunched был ли произведен запуск времени
         *      
         *      _timeSource = 0 
         *      => возвращается текущее время все остальные настройки игнорируются
         *      
         *      _timeSource = 1  _shiftLaunched = false
         *      =>  возвращается _shiftedAbsTime
         *    при запуске времени производится расчет _spanShiftAbs = (DateTime.Now-_shiftedAbsTime) и устанавливается _shiftLaunched true
         *      _timeSource = 1  _shiftLaunched = true
         *      => возвращается DateTime.Now - _spanShiftAbs
         *      
         *      
         *      _timeSource = 2 
         *     *!!Возможность активации данного значения должно зависеть от наличия в памяти данных о последнем ФД
         *
         *      _timeSource = 2 _shiftLaunched = false
         *      =>  возвращается данные из запроса статуса дата последнего ФД + _fdPlusMins
         *    при запуске времени производится расчет _spanShiftAbs = (DateTime.Now-lastFd.Time) и устанавливается _shiftLaunched true
         *      _timeSource = 2 _shiftLaunched = true
         *      => возвращается DateTime.Now - _spanShiftAbs
         *      
         *      При смене настройки _shiftLaunched => false
         *      
         *      стоит сделать вложенный класс ? *
         */
        int _timeSource = 0;
        public int TimeSource { get => _timeSource; }
        bool _shiftLaunched = false;
        //public bool ShiftLaunched { get => _shiftLaunched; }
        
        TimeSpan _spanShiftAbs = TimeSpan.Zero;
        DateTime _shiftedAbsTime = DateTime.Now;
        //public DateTime ShifAbsTime { get => _shiftedAbsTime; }
        
        DateTime _lastFdTime = DateTime.MinValue;
        
        int _fdPlusMins = 0;
        /*
         *   публичные методы выше удалить, реализовано без них
         */



        public int PlusMin { get => _fdPlusMins; }

        byte[] DatetimeBytes
        {
            get
            {
                DateTime fdTime = GetTimeForFd;
                
                byte[] bytes = new byte[5];
                bytes[0] = (byte)(fdTime.Year-2000);
                bytes[1] = (byte)fdTime.Month;
                bytes[2] = (byte)fdTime.Day;
                bytes[3] = (byte)fdTime.Hour;
                bytes[4] = (byte)fdTime.Minute;
                return bytes;
            }
        }
        byte[] DateBytes
        {
            get
            {
                DateTime fdTime = GetTimeForFd;
                byte[] bytes = new byte[3];
                bytes[0] = (byte)(fdTime.Year - 2000);
                bytes[1] = (byte)fdTime.Month;
                bytes[2] = (byte)fdTime.Day;
                return bytes;
            }
        }

        public void ChangeDtSetting(int src, DateTime dt, int shift)
        {
            //bool changed = src != _timeSource;
            _timeSource = src;
            _shiftedAbsTime = dt;
            _fdPlusMins = shift;
        }
        public void StopTimer() 
        {
            _shiftLaunched = false;
        }
        public bool IsTimeLaunched { get => _shiftLaunched; }
        public void LaunchTimer()
        {
            if (_terminalUi != null)
            {
                _terminalUi.FlowOfTime(true);
            }
            if (_timeSource == 1)
            {
                _spanShiftAbs = DateTime.Now - _shiftedAbsTime;
            }
            else if(_timeSource == 2)
            {
                if (_lastFdTime.CompareTo(_minAvailableTimeFd) > 0)
                {
                    _spanShiftAbs = DateTime.Now - _lastFdTime;
                }
                else
                {
                    _spanShiftAbs = TimeSpan.Zero;
                }

                
            }

            _shiftLaunched = true;
        }


        public TerminalFnExchange(MainForm mainWnd=null)
        {

            _tfn_lib_version = "app-"+System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            var ports = SerialPort.GetPortNames();
            if (ports.Length > 0)
            {
                if(AppSettings.TerminalPreferPort == 0)
                {
                    ComPort = ports[0];        // тут добавить чтение настройки о предпочитаемом порте
                }
                else if(AppSettings.TerminalPreferPort == 1)
                {
                    ComPort = ports[ports.Length - 1];
                }
            }
            _logLevel = AppSettings.TerminalFnLogLevel;
            _formatLog = AppSettings.TerminalFnLogFormat;
            if(mainWnd!=null)
            {
                _ui = mainWnd;
                KKMInfoTransmitter[FR_CONNECTION_SETTINGS_KEY] = ConnectionReprezentation();
                KKMInfoTransmitter[FR_FIRMWARE_KEY] = _tfn_lib_version;
            }
            _minAvailableTimeFd = AppSettings.MinAvailableFdTimeFn;
        }


        public void ShowSettings()
        {
            _formatLog = AppSettings.TerminalFnLogFormat;
            _logLevel = AppSettings.TerminalFnLogLevel;
            _terminalUi = new TerminalUi(this);
            _terminalUi.ShowDialog();
            
        }

        private static string _comName = "";
        private static int _baudRate = 115200, _timeOut = 1000, _parity = 0, _dataBits = 8, _stopBits = 1;
        private bool _lockParams = false;

        

        public bool SetConnectionParams(string comName, int baudrate, int timeout = 1000, int parity = 0, int dataBits = 8, int stopBits = 1)
        {
            if (_lockParams) return false;

            if (comName.Length <= 3 || !comName.ToUpper().StartsWith("COM")) return false;
            int comNumber = -1;
            try { comNumber = int.Parse(comName.Substring(3)); } catch { comNumber = -1; }
            if ((comNumber <= 0) ||
                (baudrate != 115200 && baudrate != 921600 && baudrate != 1200 && baudrate != 2400 && baudrate != 4800 && baudrate != 9600 && baudrate != 19200 && baudrate != 28800 && baudrate != 38400 && baudrate != 57600 && baudrate != 76800 && baudrate != 230400) ||
                (timeout <= 0) ||
                (parity != 0 && parity != 1) ||
                (dataBits != 7 && dataBits != 8) ||
                (stopBits != 1 && stopBits != 2)) return false;

            _comName = comName;
            _baudRate = baudrate;
            _timeOut = timeout;
            _parity = parity;
            _dataBits = dataBits;
            _stopBits = stopBits;
            return true;
        }
        public string ComPort
        {
            get => _comName;
            set
            {
                if (value.Length <= 3 || !value.ToUpper().StartsWith("COM")) return;
                int comNumber = -1;
                try { comNumber = int.Parse(value.Substring(3)); } catch { comNumber = -1; }
                if (comNumber <= 0) return;
                _comName = value;
            }
        }
        public int Baudrate
        {
            get => _baudRate;
            set
            {
                if ((value != 115200 && value != 921600 && value != 1200 && value != 2400 && value != 4800 && value != 9600 && value != 19200 && value != 28800 && value != 38400 && value != 57600 && value != 76800 && value != 230400)) return;
                _baudRate = value;
            }
        }
        public int Timeout
        {
            get => _timeOut;
            set
            {
                if (value > 0) _timeOut = value;
            }
        }

        private bool _dontLockChannel = false;
        public bool DontLockChannel
        {
            get => _dontLockChannel;
            set => _dontLockChannel = value;
        }

        public enum MsgSource
        {
            INPUT,
            OUTPUT,
            ERROR,
            INFO
        }
        static int _logLevel = 0;
        public int LogLevel
        {
            set 
            { 
                _logLevel = value; 
                AppSettings.TerminalFnLogLevel = value;
            }
            get => _logLevel;
        }
        static int _formatLog = 0;
        public int LogFormat 
        { 
            set 
            {
                _formatLog = value;
                AppSettings.TerminalFnLogFormat = value;
            }
            get => _formatLog;
        
        }




        int _historySize = 200;
        public int SetHistorySize { set => _historySize = value > 0 ? value : _historySize; }
        static long _uartEventNumber = 0;
        public long UartEvents { get => _uartEventNumber; }
        public struct UartEvent
        {
            public UartEvent(MsgSource type, object data = null, string describe = null)
            {
                this.type = type;
                this.data = data;
                if (type == MsgSource.INPUT && data is string && (data as string) == "@#$%^&*")
                {
                    time = DateTime.MinValue;
                    presentation = "";  
                }
                else
                {
                    time = DateTime.Now;
                    if (type == MsgSource.INFO || type == MsgSource.ERROR)
                    {
                        presentation = data != null ? data.ToString() : _errorPresentation;
                    }
                    else
                    {
                        presentation =  data != null ? BitConverter.ToString(data as byte[]) : "";
                    }
                }
                id = _uartEventNumber++;
                if (describe != null)
                    describtion = describe;
                else
                    describtion = "";
                // 0 - пишем в файл только ошибки
                if(_logLevel >= 0)
                {
                    if(type == MsgSource.ERROR && this.RealEvent())
                    {
                        LogHandle.olta(this.ToString());
                    }
                }
                // 1 - пишем в файл ошибки и информационные данные
                if (_logLevel >= 1)
                {
                    if(type == MsgSource.INFO && this.RealEvent())
                    {
                        LogHandle.olta(this.ToString());
                    }
                }
                // 2 - пишем в файл так же ненулевой обмен
                if(_logLevel>=2)    
                {
                    if (RealEvent()&&(type == MsgSource.INPUT || type == MsgSource.OUTPUT) && data != null && data is byte[] && (data as byte[]).Length>0)
                    {
                        LogHandle.olta(this.ToString());
                    }
                }
                // 3 - добавляем пустые срабатывания _port.DataRecieved
                if (_logLevel >= 3)
                {
                    if (RealEvent()&&(type == MsgSource.INPUT || type == MsgSource.OUTPUT) && (data == null||(data != null && data is byte[] && (data as byte[]).Length == 0)))
                    {
                        LogHandle.olta(this.ToString());
                    }
                }
                // добавляем синтетические события
                if(_logLevel >= 4 && !RealEvent()) 
                {
                    LogHandle.olta(DateTime.Now.ToString("HH:mm:ss,fff\t")+"____not_event__");
                }
            }
            DateTime time;
            public MsgSource type;
            public object data;
            string presentation;
            string describtion;
            public long id;
            public override string ToString()
            {
                if (time == DateTime.MinValue) return "____not_event__";
                if (_formatLog == 0)
                {
                    return type.ToString() + ":\t" + presentation;
                }
                //  добавить строковое представление данных обмена
                //  string describtion = "";
                //  if (eventsDataPresentation.ContainsKey(id)) describtion = eventsDataPresentation[id];
                return time.ToString("HH:mm:ss.fff\t") + type.ToString() + Environment.NewLine + ((type==MsgSource.OUTPUT||type==MsgSource.INPUT)?id.ToString("d8")+":":"") + presentation + Environment.NewLine + (string.IsNullOrEmpty(describtion)?"":describtion + Environment.NewLine);
            }
            public override int GetHashCode()
            {
                return (int)id;
            }

            public bool RealEvent() { return time > DateTime.MinValue; }
            public static UartEvent SLIP = new UartEvent(MsgSource.INPUT, "@#$%^&*");
            public static bool operator ==(UartEvent c1, UartEvent c2)
            {
                return c1.Equals(c2);
            }
            public static bool operator !=(UartEvent c1, UartEvent c2)
            {
                return !c1.Equals(c2);
            }

            public override bool Equals(object o)
            {
                if (o == null) return false;
                if (o is UartEvent)
                {
                    UartEvent c1 = (UartEvent)o;
                    return c1.id == this.id;
                        /*(this.type == c1.type
                        && this.time == c1.time
                        && this.presentation == c1.presentation
                        && ((data == null && c1.data == null) ||
                            (data is string && c1.data is string && data.Equals(c1.data)) ||
                            (data != null && c1.data != null && data is byte[] && c1.data is byte[] && (data as byte[]).Length == (c1.data as byte[]).Length) && Enumerable.SequenceEqual(data as byte[], c1.data as byte[])));*/
                }
                return false;
            }
        }
        List<UartEvent> _events = new List<UartEvent>();
        static Dictionary<long, string> eventsDataPresentation = new Dictionary<long, string>();
        public void LogAddEvent(UartEvent ev)
        {
            if (ev.RealEvent()) _events.Add(ev);
            if (_events.Count > _historySize) _events.RemoveAt(0);
            if (_terminalUi != null && _terminalUi.Created)
            {
                if (ev.id % 256 == 0) 
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (UartEvent e in _events)
                    {
                        //if (e.slip()) continue;
                        sb.Append(e.ToString());
                        //sb.Append(e.ToString());
                        sb.Append(Environment.NewLine);
                    }
                    _terminalUi.VisualLogUpdate = sb.ToString();
                }
                else
                {
                    _terminalUi.SimpleAddText(ev+Environment.NewLine);
                }
            }
        }
        public void ClearHistory()
        {
            _events.Clear();
            eventsDataPresentation.Clear();
            if (_terminalUi != null && _terminalUi.Created) _terminalUi.VisualLogUpdate = "";
        }

        private void UnlocUartOut()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            _deviceAvailable = true;
        }

        private void LockUartOut(long id)
        {
            _deviceAvailable = false;
            _idRequest = id;
            _timer = new System.Timers.Timer();
            _timer.AutoReset = false;
            _timer.Elapsed += TimerElapsed;
            _timer.Interval = _timeOut;
            _timer.Enabled = true;
            _timer.Start();
        }


        public int OpenPort()
        {
            int rezult = 0;
            LogAddEvent(new UartEvent(MsgSource.INFO, "Открываем порт " + ConnectionReprezentation()));
            if (string.IsNullOrEmpty(_comName) || !_comName.ToUpper().StartsWith("COM"))
            {
                _errorPresentation = "Не настроен порт";
                rezult = NOT_SET;
                goto NotOpened;
            }
            int portNumber = -1;
            try
            {
                portNumber = int.Parse(_comName.Substring(3));
            }
            catch
            {
                _errorPresentation = "Ошибка в настройках порта";
                rezult = NOT_SET;
                goto NotOpened;
            }
            if (portNumber < 1)
            {
                _errorPresentation = "Некоректный номер порта";
                rezult = NOT_SET;
                goto NotOpened;
            }
            if (_port == null) _port = new SerialPort();
            if (_port.IsOpen)
            {
                try
                {
                    ClosePort();
                }
                catch (Exception ex)
                {
                    _errorPresentation = ex.Message;
                    rezult = CLOSING_ERROR;
                    goto NotOpened;
                }
            }
            _port = new SerialPort();
            _port.PortName = _comName;
            _port.BaudRate = _baudRate;
            _port.ReadTimeout = _timeOut;
            _port.Parity = _parity == 0 ? Parity.None : Parity.Odd;
            _port.DataBits = _dataBits;
            _port.StopBits = _stopBits == 1 ? StopBits.One : StopBits.Two;
            try
            {
                _port.Open();
                if (!_port.IsOpen)
                {
                    _errorPresentation = "Неизвестная ошибка";
                    rezult = NOT_OPENED;
                    goto NotOpened;
                }
                _port.DataReceived += PortDataReceived;
                //_isConnected = true;
            }
            catch (Exception ex)
            {
                _errorPresentation = ex.Message;
                rezult = NOT_OPENED;
                goto NotOpened;
            }
            _brakeOperationReading = false;
            _deviceAvailable = true;
            return OK;

        NotOpened:
            LogAddEvent(new UartEvent(MsgSource.ERROR));
            return rezult;
        }

        public int ClosePort()
        {

            if (_connected)
            {
                Disconnect();
                return OK;
            }
                

            LogAddEvent(new UartEvent(MsgSource.INFO, "Закрываем порт."));
            if (_port == null)
            {
                LogAddEvent(new UartEvent(MsgSource.ERROR, "Проблема с инициализацией порта"));
                return CLOSING_ERROR;
            }
            if (!_port.IsOpen)
            {
                LogAddEvent(new UartEvent(MsgSource.ERROR, "Порт уже закрыт"));
            }
            else
            {
                try
                {
                    _port.Close();
                    _port.DataReceived -= PortDataReceived;
                    if (_port.IsOpen)
                    {
                        LogAddEvent(new UartEvent(MsgSource.ERROR, "Не удалось освободить порт"));
                        return CLOSING_ERROR;
                    }
                    
                }
                catch (Exception ex)
                {
                    LogAddEvent(new UartEvent(MsgSource.ERROR, ex.Message));
                    return CLOSING_ERROR;
                }
            }
            _port.Dispose();
            //_port = new SerialPort();
            _deviceAvailable = false;
            _brakeOperationReading = true;

            /*if(_ui!=null && _ui.Created)
            {
                _ui.Disconnect();
            }*/
            return OK;
        }

        public static Crc16Ccitt scrc = new Crc16Ccitt(InitialCrcValue.NonZero1);

        public int SendData(byte[] bytes)
        {
            //UartEvent ue = new UartEvent(MsgSource.OUTPUT, bytes);
            LaunchTimer();
            if (_brakeOperationReading)
            {
                _errorPresentation = "Операция прервана";
                LogAddEvent(new UartEvent(MsgSource.INFO));
                return INTERRUPTED;
            }
            if (!_deviceAvailable)
            {
                _errorPresentation = "Устройство недоступно или уже используется";
                LogAddEvent(new UartEvent(MsgSource.ERROR,null,"Невозможно отправить данные "+BitConverter.ToString(bytes)));
                return SENDING_DATA_ERROR;
            }
            if (!_dontLockChannel)
                LockUartOut(_uartEventNumber);
            ResetQueue();
            string cmdInfo = null;
            if (bytes != null)
            {
                if (bytes.Length>=6)
                {
                    if (bytes[0] == START_SIGN)
                    {
                        byte[] command = new byte[bytes.Length-3];
                        Array.Copy(bytes, 1, command, 0, bytes.Length - 3);
                        byte[] crcBytes = scrc.ComputeChecksumBytes(command);
                        if (crcBytes[0] == bytes[bytes.Length-2] && crcBytes[1] == bytes[bytes.Length - 1])
                        {
                            if (command[0] + command[1]*256 == bytes.Length - 5)
                            {
                                _command_sent = command[2];
                                if (CommandDescriber.ContainsKey(command[2]))
                                {
                                    // тут сделать дополнительниый анализ отправленной команды
                                    cmdInfo = CommandDescriber[command[2]][0].ToString();
                                }
                                else
                                {
                                    cmdInfo = "Словарь не содержит описания команды " + command[2];
                                }

                            }
                            else
                            {
                                cmdInfo = "Incorrect command length.";
                            }

                        }
                        else
                        {
                            cmdInfo = "Bad data. Incorrect CRC.";
                        }
                    }
                    else
                    {
                        cmdInfo = "START_SIGN(04h) missed - Некорректный протокол обмена";
                    }
                }
                else
                {
                    cmdInfo = "Неизвестная команда";
                }
               
            }
            UartEvent ue = new UartEvent(MsgSource.OUTPUT, bytes, cmdInfo);

            LogAddEvent(ue);
            if (_port == null)
            {
                LogAddEvent(new UartEvent(MsgSource.ERROR, "Проблема с инициализацией порта"));
                UnlocUartOut();
                return SENDING_DATA_ERROR;
            }
            if (!_port.IsOpen)
            {
                LogAddEvent(new UartEvent(MsgSource.ERROR, "Порт не открыт"));
                UnlocUartOut();
                return NOT_OPENED;
            }
            try
            {
                //LogHandle.olta("send "+bytes);
                _port.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                UnlocUartOut();
                LogAddEvent(new UartEvent(MsgSource.ERROR, ex.Message));
                return SENDING_DATA_ERROR;
            }
            return OK;
        }

        public bool PortIsOpened
        {
            get => _port != null && _port.IsOpen;
        }

        protected static string _errorPresentation = "";
        public string ErrorPresentation
        {
            get
            {
                return _errorPresentation;
            }
        }

        
            

        //public string ConnectionReprezentation() { return _comName + ":" + _baudRate; }

        public enum InitialCrcValue { Zeros, NonZero1 = 0xffff, NonZero2 = 0x1D0F }
        public class Crc16Ccitt
        {
            const ushort poly = 4129;
            ushort[] table = new ushort[256];
            ushort initialValue = 0;

            public ushort ComputeChecksum(byte[] bytes)
            {
                ushort crc = this.initialValue;
                for (int i = 0; i < bytes.Length; ++i)
                {
                    crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
                }
                return crc;
            }

            public byte[] ComputeChecksumBytes(byte[] bytes)
            {
                ushort crc = ComputeChecksum(bytes);
                return BitConverter.GetBytes(crc);
            }

            public Crc16Ccitt(InitialCrcValue initialValue)
            {
                this.initialValue = (ushort)initialValue;
                ushort temp, a;
                for (int i = 0; i < table.Length; ++i)
                {
                    temp = 0;
                    a = (ushort)(i << 8);
                    for (int j = 0; j < 8; ++j)
                    {
                        if (((temp ^ a) & 0x8000) != 0)
                        {
                            temp = (ushort)((temp << 1) ^ poly);
                        }
                        else
                        {
                            temp <<= 1;
                        }
                        a <<= 1;
                    }
                    table[i] = temp;
                }
            }
        }

        
        
        internal async Task ReadFdTlvAsync(bool clearRawData = true)
        {
            _buffer_level_1 = new List<byte>();
            if (_numberFdToRead > 0)
            {
                bool readTlv = true;
                
                byte[] cmd = new byte[] { 5, 0, CODE_GET_FD_ROOT_STLV_HEAD, (byte)(_numberFdToRead % 256),(byte)(_numberFdToRead/256%256),(byte)(_numberFdToRead/256/256%256),(byte)(_numberFdToRead/256/256/256) };
                var crcbyset = scrc.ComputeChecksumBytes(cmd);
                byte[] command = new byte[] { START_SIGN, 5, 0, CODE_GET_FD_ROOT_STLV_HEAD, (byte)(_numberFdToRead % 256), (byte)(_numberFdToRead / 256 % 256), (byte)(_numberFdToRead / 256 / 256 % 256), (byte)(_numberFdToRead / 256 / 256 / 256),crcbyset[0], crcbyset[1] };
                LogHandle.olta("Запрашиваем заголовок ФД " + _numberFdToRead);
                tcs_levell_0 = new TaskCompletionSource<bool>();
                SendData(command);
                await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));
                LogHandle.olta("Запрос заголовка завершен");
                if (tcs_levell_0.Task.IsCompleted)
                {
                    tcs_levell_0?.TrySetException(new Exception("Brake operation"));
                    
                    // !!! проверить как ФН отвечает на архивные документы
                    if (_fnAnwerCodeInt == OK)
                    {
                        while (readTlv && _fnAnwerCodeInt == OK)
                        {
                            
                            tcs_levell_0 = new TaskCompletionSource<bool>();
                            SendData(CMD_GET_FD_TLV_STRUCT);
                            await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));
                            readTlv = tcs_levell_0.Task.IsCompleted;
                        }
                        if (readTlv)
                        {
                            var f = FiscalPrinter.TranslateFtagsList(_buffer_level_1.ToArray());//FTLVParcer.ParseStructure(_buffer_level_1.ToArray());
                            //LogHandle.olta(f.Count.ToString());
                            //if(_ui!=null)_ui.AddReadedFdList(f);
                            if (f.Tags != null&& f.Tags.Count>0)
                                ftagList.AddRange(f.Tags);
                        }
                        else
                        {
                            LogHandle.olta("При обмене с ФН произошла ошибка, выводим накопленные данные"
                                +Environment.NewLine + _buffer_level_1 == null ? "Empty":BitConverter.ToString(_buffer_level_1.ToArray()));
                        }
                        if(clearRawData)
                            _buffer_level_1 = null;
                    }
                }
                else
                {
                    _fnAnwerCodeInt = _SERVICE_CODE_TIMEOUT_OFF;
                    _fnOperationMessage = "Истек таймаут";
                    ResetQueue();
                    
                    //LogAddEvent(new UartEvent(MsgSource.INFO, "Проверяем доступность порта"));
                    //if(!PortIsOpened)
                    //    ClosePort();

                }


            }

        }

        internal void StopFnExchange()
        {
            _brakeOperationReading = true;
            tcs_levell_0?.TrySetCanceled();
        }

        private bool _brakeOperationReading = false;

        internal async Task ReadFdTlvList()
        {
            ftagList.Clear();
            _brakeOperationReading = false;
            await GetFnStatusAsync();
            if(_num1<1)_num1 = 1;
            if(_num2<1)_num2 = 1;
            if(_num1>_lastFD)_num1 = _lastFD;
            if(_num2>LastFd)_num2 = _lastFD;

            if (_num1 > _num2)
            {
                int t = _num1;
                _num1 = _num2;
                _num2 = t;
            }
            double countFd = _num2 - _num1;
            bool showProgress = countFd > 10;
            int fdCounter = ftagList.Count;
            for (int i = _num1; i<= _num2 && ! _brakeOperationReading; i++)
            {
                _numberFdToRead = i;
                await ReadFdTlvAsync();
                if (_terminalUi != null && _terminalUi.Created)
                {
                    if (ftagList.Count > fdCounter)
                    {
                        _terminalUi.UpdateStatusArea("Прочитан " + i, null, showProgress ? 100.0 * (i - _num1) / countFd : -1);
                        fdCounter = ftagList.Count;
                    }
                    else
                    {
                        _terminalUi.UpdateStatusArea("ФД " + i+" не прочитан", null, showProgress ? 100.0 * (i - _num1) / countFd : -1);
                    }
                    
                }

            }

            if (_terminalUi != null) _terminalUi.ReadedFdToPanelFT(ftagList);
        }

        private int _num1 = 0;
        private int _num2 = 0;
        public int X1 { get => _num1; set => _num1 = value; }
        public int X2 { get => _num2; set => _num2 = value; }



        List<int> _uiParamsToUpdate = new List<int>();
        public List<int> ParamsToUpdate { get => _uiParamsToUpdate; }
        
        public Dictionary<int,string> FnInfo = new Dictionary<int,string>();
        public string GetFnInfoParam(int param)
        {
            if (FnInfo.ContainsKey(param))
            {
                return FnInfo[param];
            }
            if(param < 65535)
            {
                return "Отсутвует параметр: "+param;
            }
            return "Нет данных";
        }



        int _unsentDocs = 0;
        int _firstUnsentDocNumber = 0;
        DateTime _firstUnsentTime = new DateTime(1970,1,1);

        int _fnAnwerCodeInt = -1;
        int _command_sent = -1;
        string _performedFdInfo = "";


        int _ffdFtagFormat = -1;
        new public int FfdFtagFormat
        {
            get => _ffdFtagFormat;
            set
            {
                _ffdFtagFormat= value;
                if (value == 0)
                    _ffdVer = 0;
                else if (value == 1)
                    _ffdVer = FR_FFD100;
                else if (value == 2)
                    _ffdVer = FR_FFD105;
                else if (value == 3)
                    _ffdVer = FR_FFD110;
                else if (value == 4)
                    _ffdVer = FR_FFD120;
                else if (value > 4)
                    _ffdVer = 125;
            }
        }



        int _shiftStateFiscalPrinter = FiscalPrinter.NONE;
        public int ShiftStateFPCompatible
        {
            get { return _shiftStateFiscalPrinter; }
        }




        public string FnOperationMessage {  get { return _fnOperationMessage; } }



        private int _dataQueue = 0;
        private void ResetQueue()
        {
            _dataQueue = 0;
            _buffer_level_0.Clear();
        }
        Encoding ascii = Encoding.ASCII;

        
        TaskCompletionSource<bool> tcs_levell_0 = null;
        //bool _badExchangeLevel_0 = false;


        void PortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_brakeOperationReading)
            {
                _errorPresentation = "Операция прервана";
                LogAddEvent(new UartEvent(MsgSource.INFO));
                tcs_levell_0?.TrySetResult(true);
                return;
            }
            try
            {
                byte[] dataBytes = new byte[_port.BytesToRead];
                _port.Read(dataBytes, 0, dataBytes.Length);
                _port.DiscardInBuffer();
                if (dataBytes == null || dataBytes.Length == 0)
                {
                    //LogAddEvent(new UartEvent(MsgSource.INPUT,dataBytes,"Получено пустое сообщение"));
                    return;
                }
                _buffer_level_0.Add(dataBytes);

                LogAddEvent(new UartEvent(MsgSource.INPUT, dataBytes));

                DateTime dtRecieved = DateTime.Now;
                int timeAwaiting = 0;
                // ожидание обработки предыдущего получения данных
                while(_dataQueue > 0 )
                {
                    if (timeAwaiting == 0)
                    {
                        _errorPresentation = "Производится обработка данных, bytes recieved "+ dataBytes.Length;
                        LogAddEvent(new UartEvent(MsgSource.INFO));
                        //_buffer.Add(dataBytes);
                    }
                    Thread.Sleep(5);
                    timeAwaiting += 5;
                    if (timeAwaiting > _timeOut) { break; }
                }
                
                if (timeAwaiting > _timeOut)
                {
                    _fnOperationMessage = "За установленный таймаут не получен или получен неполный ответ от ФН";
                    _errorPresentation = "Истек таймаут.";
                    _fnAnwerCodeInt = _SERVICE_CODE_TIMEOUT_OFF;
                    LogAddEvent(new UartEvent(MsgSource.ERROR));
                    ResetQueue();
                    LogAddEvent(new UartEvent(MsgSource.INFO, "Истек таймаут", "Проверяем доступность порта"));
                    if (!PortIsOpened)
                        ClosePort();
                    return;
                }
                if (_buffer_level_0.Count == 0) //обработка уже завершена
                    return;
                // соединение данных
                _dataQueue++;   // блокируем выполнение дальнейших операций
                List<byte> answerAsList = new List<byte>();
                foreach(var data in _buffer_level_0)
                {
                    answerAsList.AddRange(data);
                }
                int anwerLenth = answerAsList.Count;
                if (anwerLenth > 3)
                {
                    // получен признак начала сообщения
                    if (answerAsList[0] == 4)
                    {

                        if(_command_sent == 0x35)
                        {
                            // структура ответного сообщения тут отличается поэтому дальше не идем
                            // хз как это обрабатывать информации не нашел
                            
                            ResetQueue();
                            UnlocUartOut();
                            tcs_levell_0?.TrySetResult(true);
                            return;
                        }

                        if (
                             
                             answerAsList[1] + 256 * answerAsList[2] > anwerLenth - 5)
                        {
                            _errorPresentation = "Сообщение неполноe";
                            LogAddEvent(new UartEvent(MsgSource.INFO));
                            // где-то тут иногда появляются потери данных
                            // возможно не срабатывает событие dataReceved 
                            // как вариант подождать 50мс и запросить повторно что осталось в порту

                        }
                        else if(((int)answerAsList[1] + 256 * (int)answerAsList[2]) < anwerLenth - 5)
                        {
                            // ФН ссобщил о меньшем размере сообщения чем передал
                            ResetQueue();
                            _errorPresentation = _fnOperationMessage = BAD_DATA_INTEGRITY;
                            LogAddEvent(new UartEvent(MsgSource.ERROR,null, "ФН собщил о меньшем размере сообщения чем передал, возможно проблема в интерфейсе связи"));
                            //_badExchangeLevel_0 = true;
                            tcs_levell_0?.TrySetResult(true);
                            return;
                        }
                        else
                        {
                            byte[] answerByteArray = new byte[answerAsList.Count-3];
                            answerAsList.CopyTo(1,answerByteArray,0,answerByteArray.Length);
                            bool alarm = false;
                            // Сообщение полное
                            _fnAnwerCodeInt = answerAsList[3];
                            if (_fnAnwerCodeInt >= 128)
                            {
                                //_errorPresentation = "В ответе ФН присутствуют предупреждения. Для уточнения необходимо запростить статус ФН.";
                                //LogAddEvent(new UartEvent(MsgSource.INFO));
                                alarm = true;
                                _fnAnwerCodeInt -= 128;
                            }
                            _errorPresentation = "Ответ ФН:(" + _fnAnwerCodeInt + ") " + (FnAnswerCode.ContainsKey(_fnAnwerCodeInt) ? FnAnswerCode[_fnAnwerCodeInt] : "Отсутвует описание кода ответа ФН.") + (alarm ? " присутсвуют предупреждения, подробности в статусе ФН" : "") ;
                            byte[] crcSumBytes = scrc.ComputeChecksumBytes(answerByteArray);
                            if (crcSumBytes[0] == answerAsList[answerAsList.Count-2] && crcSumBytes[1] == answerAsList[answerAsList.Count - 1])
                            {
                                string describtion = null;
                                StringBuilder sbDescribtion = new StringBuilder();
                                // тут обрабатываем внешние запросы
                                switch (_command_sent)
                                {
                                    case CODE_SEND_TLV_LIST:

                                    case CODE_BEGIN_OPEN_SHIFT:
                                    case CODE_BEGIN_CLOSE_SHIFT:
                                    case CODE_BEGIN_RECEIPT:
                                        /*if(_fnAnwerCodeInt == OK)
                                        {
                                                // фн возвращает только код ответа
                                        }*/
                                        break;
                                    case CODE_GET_SHIFT_PARAMS:
                                        if (_fnAnwerCodeInt == OK)
                                        {
                                            string shiftState = answerAsList[4] == 0 ? "Смена закрыта" : "Смена открыта";
                                            _chequeRequest = answerAsList[7] + 256 * answerAsList[8];
                                            _shiftNumber = answerAsList[5] + 256 * answerAsList[6];
                                            describtion = shiftState + "; номер смены:  " + _shiftNumber + ";  номер чека за смену:" + _chequeRequest;
                                        }
                                        break;
                                    case CODE_PERFORM_OPEN_SHIFT:
                                    case CODE_PERFORM_CLOSE_SHIFT:
                                        if (_fnAnwerCodeInt == OK)
                                        {
                                            int shiftNumber = answerAsList[4] + 256 * answerAsList[5];
                                            uint docNumber = 0;
                                            if (answerAsList.Count > 9)
                                                docNumber = answerAsList[6] + 256u * answerAsList[7] + 65536u * answerAsList[8] + 16777216u * answerAsList[9];
                                            uint fpd = 0;
                                            if (answerAsList.Count > 13)
                                                fpd = answerAsList[10] + 256u * answerAsList[11] + 65536u * answerAsList[12] + 16777216u * answerAsList[13];
                                            // добавить проверку флагов предупреждений
                                            describtion = string.Format("Смена: {0}; ФД: {1}; ФПД: {2}", shiftNumber, docNumber, fpd);
                                            _performedFdInfo = describtion.ToString();
                                        }
                                        break;
                                    case CODE_PERFORM_CALCULATION_REPORT:
                                        if (_fnAnwerCodeInt == OK)
                                        {
                                            uint docNumber = 0;
                                            if (answerAsList.Count > 7)
                                                docNumber = answerAsList[4] + 256u * answerAsList[5] + 65536u * answerAsList[6] + 16777216u * answerAsList[7];
                                            uint fpd = 0;
                                            if (answerAsList.Count > 10)
                                                fpd = answerAsList[8] + 256u * answerAsList[9] + 65536u * answerAsList[10] + 16777216u * answerAsList[11];
                                            uint unsent = 0;
                                            if (answerAsList.Count > 15)
                                            {
                                                unsent = answerAsList[12] + 256u * answerAsList[13] + 65536u * answerAsList[14] + 16777216u * answerAsList[15];
                                            }
                                            DateTime dt = new DateTime(1970, 1, 1);
                                            if (answerAsList.Count > 18 && answerAsList[16] + answerAsList[17] + answerAsList[18] > 0)
                                            {
                                                dt = new DateTime(2000 + answerAsList[16], answerAsList[17], answerAsList[18]);
                                            }
                                            describtion = string.Format("ФД: {0}; ФПД: {1}; Непередано: {2}; от: {3}", docNumber, fpd, unsent, dt.ToString("dd.MM.yyyy"));
                                            _performedFdInfo = describtion.ToString();
                                        }
                                        break;
                                    case CODE_GET_EXCHANGE_STATUS:
                                        if (_fnAnwerCodeInt == OK)
                                        {
                                            byte statusExch = answerAsList[4];
                                            sbDescribtion.AppendLine("Статус информационного обмена: " + statusExch);
                                            StringBuilder sbExtStatus = new StringBuilder();
                                            if ((statusExch & 0b_0000_0001) != 0)
                                            {
                                                sbDescribtion.AppendLine("Транспортное соединение установлено");
                                                sbExtStatus.Append("Cоед. установлено");
                                            }
                                            if ((statusExch & 0b_0000_0010) != 0)
                                            {
                                                sbDescribtion.AppendLine("Есть сообщение для передачи в ОФД");
                                                if (sbExtStatus.Length > 0)
                                                    sbExtStatus.Append("; ");
                                                sbExtStatus.Append("Есть сообщение(я) для ОФД");
                                            }
                                            if ((statusExch & 0b_0000_0100) != 0)
                                            {
                                                if (sbExtStatus.Length > 0)
                                                    sbExtStatus.Append("; ");
                                                sbExtStatus.Append("Ожидание ответа от ОФД");
                                                sbDescribtion.AppendLine("Ожидание ответного квитанции от ОФД");
                                            }
                                            if ((statusExch & 0b_0000_1000) != 0)
                                            {
                                                if (sbExtStatus.Length > 0)
                                                    sbExtStatus.Append("; ");
                                                sbExtStatus.Append("Есть команда от ОФД");
                                                sbDescribtion.AppendLine("Есть команда от ОФД");
                                            }
                                            if ((statusExch & 0b_0001_0000) != 0)
                                            {
                                                if (sbExtStatus.Length > 0)
                                                    sbExtStatus.Append("; ");
                                                sbExtStatus.Append("Изменились настройки");
                                                sbDescribtion.AppendLine("Изменились настройки соединения с ОФД");
                                            }
                                            if ((statusExch & 0b_0010_0000) != 0)
                                            {
                                                if (sbExtStatus.Length > 0)
                                                    sbExtStatus.Append("; ");
                                                sbExtStatus.Append("Ожидание ответа от ОФД");
                                                sbDescribtion.AppendLine("Ожидание ответа на команду от ОФД");
                                            }
                                            sbDescribtion.AppendLine("Начато чтение сообщения для ОФД: "+ (answerAsList[5]==1?"да":"нет"));
                                            _unsentDocs = answerAsList[6] + 256 * answerAsList[7];
                                            sbDescribtion.AppendLine("Количество сообщений для ОФД: " + _unsentDocs);
                                            _firstUnsentDocNumber = answerAsList[8] + 256 * answerAsList[9] + 256 * 256 * answerAsList[10] + 256 * 256 * 256 * answerAsList[11];
                                            sbDescribtion.AppendLine("Номер первого непереданного ФД: " + _firstUnsentDocNumber);
                                            if((uint)answerAsList[13] + (uint)answerAsList[14] > 1)
                                            {
                                                _firstUnsentTime = new DateTime(2000 + answerAsList[12], answerAsList[13], answerAsList[14], answerAsList[15], answerAsList[16],0);
                                                sbDescribtion.AppendLine("Время первого непереданного: " + _firstUnsentTime.ToString("dd.MM.yyyy HH:mm"));
                                                FnInfo[FNDESCR_OFD_EXCHANGE_UNSENT_FIRST_DOC_TIME] = _firstUnsentTime.ToString("dd.MM.yyyy HH:mm");
                                                if (!_uiParamsToUpdate.Contains(FNDESCR_OFD_EXCHANGE_UNSENT_FIRST_DOC_TIME)) _uiParamsToUpdate.Add(FNDESCR_OFD_EXCHANGE_UNSENT_FIRST_DOC_TIME);
                                            }
                                            FnInfo[FNDESCR_OFD_EXCHANGE_STATUS] = statusExch.ToString();
                                            FnInfo[FNDESCR_OFD_EXCHANGE_ACTIVE] = answerAsList[5].ToString();
                                            FnInfo[FNDESCR_OFD_EXCHANGE_UNSENT_DOCS] = _unsentDocs.ToString();
                                            FnInfo[FNDESCR_OFD_EXCHANGE_UNSENT_FIRST_DOC_NUM] = _firstUnsentDocNumber.ToString();
                                            FnInfo[FNDESCR_OFD_EXCHANGE_STATUS_EXT] = sbExtStatus.ToString();
                                            FnInfo[FNDESCR_OFD_EXCHANGE_FD_STRING] = _unsentDocs.ToString() + "/" + _firstUnsentTime.ToString("dd.MM.yyyy HH:mm");
                                            if (!_uiParamsToUpdate.Contains(FNDESCR_OFD_EXCHANGE_STATUS)) _uiParamsToUpdate.Add(FNDESCR_OFD_EXCHANGE_STATUS);
                                            if (!_uiParamsToUpdate.Contains(FNDESCR_OFD_EXCHANGE_ACTIVE)) _uiParamsToUpdate.Add(FNDESCR_OFD_EXCHANGE_ACTIVE);
                                            if (!_uiParamsToUpdate.Contains(FNDESCR_OFD_EXCHANGE_UNSENT_DOCS)) _uiParamsToUpdate.Add(FNDESCR_OFD_EXCHANGE_UNSENT_DOCS);
                                            if (!_uiParamsToUpdate.Contains(FNDESCR_OFD_EXCHANGE_UNSENT_FIRST_DOC_NUM)) _uiParamsToUpdate.Add(FNDESCR_OFD_EXCHANGE_UNSENT_FIRST_DOC_NUM);
                                            
                                        }
                                        describtion = sbDescribtion.ToString();
                                        break;
                                    case CODE_GET_FN_NUMBER:
                                        if (_fnAnwerCodeInt == OK)
                                        {
                                            byte[] fnNum = new byte[16];
                                            for (int i = 0; i < 16; i++)
                                            {
                                                fnNum[i] = answerAsList[i + 4];
                                            }
                                            string fnNumber = ascii.GetString(fnNum);
                                            describtion = fnNumber;
                                            FnInfo[FiscalPrinter.FTAG_FN_NUMBER] = fnNumber;
                                            if (!_uiParamsToUpdate.Contains(FiscalPrinter.FTAG_FN_NUMBER)) _uiParamsToUpdate.Add(FiscalPrinter.FTAG_FN_NUMBER);
                                        }
                                        break;
                                    case CODE_GET_FN_EXPIRATION:
                                        if(_fnAnwerCodeInt == OK)
                                        {
                                            List<int> paramsToUpdateExpir = new List<int>();
                                            DateTime dtExpira = new DateTime(1970, 1, 1);
                                            //if (answerAsList[4] > 0)
                                            //{
                                                if(answerAsList[5]==0 || answerAsList[6] == 0)
                                                {
                                                    LogHandle.olta("Не удалось получить дату окончания ФН  2000+ГГ:"+ answerAsList[4]+" ММ:"+ answerAsList[5]+" ДД:"+ answerAsList[6]);
                                                }
                                                else
                                                {
                                                    dtExpira = new DateTime(2000 + answerAsList[4], answerAsList[5], answerAsList[6]);
                                                }
                                                FnInfo[FNDESCR_EXPIRATION_DATE] = dtExpira.ToString("dd.MM.yyyy");
                                                sbDescribtion.AppendLine("" + dtExpira.ToString("dd.MM.yyyy"));
                                                FnInfo[FNDESCR_EXPIRATION_AVAILABLE_REGS] = answerAsList[7].ToString();
                                                FnInfo[FNDESCR_EXPIRATION_PERFORMED_REGS] = answerAsList[8].ToString();
                                                sbDescribtion.AppendLine("Выполнено регистраций: " + answerAsList[8] + "  осталось: " + answerAsList[7]);

                                                if (!_uiParamsToUpdate.Contains(FNDESCR_EXPIRATION_DATE))
                                                {
                                                    _uiParamsToUpdate.Add(FNDESCR_EXPIRATION_DATE);
                                                }
                                                if (!_uiParamsToUpdate.Contains(FNDESCR_EXPIRATION_AVAILABLE_REGS))
                                                {
                                                    _uiParamsToUpdate.Add(FNDESCR_EXPIRATION_AVAILABLE_REGS);
                                                }
                                                if (!_uiParamsToUpdate.Contains(FNDESCR_EXPIRATION_PERFORMED_REGS))
                                                {
                                                    _uiParamsToUpdate.Add(FNDESCR_EXPIRATION_PERFORMED_REGS);
                                                }
                                                describtion = sbDescribtion.ToString();
                                            //}
                                        }
                                        break;
                                    case CODE_GET_FN_STATUS:
                                        if (_fnAnwerCodeInt == OK)
                                        {
                                            List<int> paramsToUpdate = new List<int>();
                                            //string fnStageOfApplication = "";
                                            _stageOfUsage = answerAsList[4];
                                            if (_stageOfUsage == 1)
                                            {
                                                FnInfo[FNDESCR_STAGE_OF_APPLICATION] = "Готовность ФН к формированию отчета о регистрации ККТ...";
                                                sbDescribtion.AppendLine("ФН не фискализирован");
                                            }
                                            else if (_stageOfUsage == 3)
                                            {
                                                FnInfo[FNDESCR_STAGE_OF_APPLICATION] = "Эксплуатация ФН с формированием фискальных документов...";
                                                sbDescribtion.AppendLine("ФН фискализирован");
                                            }
                                            else if (_stageOfUsage == 7)
                                            {
                                                FnInfo[FNDESCR_STAGE_OF_APPLICATION] = "Передача фискальных документов ОФД без формирования ФД";
                                                sbDescribtion.AppendLine("ФН закрыт, данные не переданы");
                                            }
                                            else if (_stageOfUsage == 15)
                                            {
                                                FnInfo[FNDESCR_STAGE_OF_APPLICATION] = "Обеспечение возможности считывания фискальных данных, хранящихся в памяти ФН";
                                                sbDescribtion.AppendLine("ФН закрыт");
                                            }
                                            else
                                            {
                                                FnInfo[FNDESCR_STAGE_OF_APPLICATION] = "Code: " + answerAsList[4].ToString("X2") + " - описание для данного кода отсутсвует.";
                                            }
                                            if (!_uiParamsToUpdate.Contains(FNDESCR_STAGE_OF_APPLICATION)) _uiParamsToUpdate.Add(FNDESCR_STAGE_OF_APPLICATION);
                                            //FnInfo[STAGE_OF_APPLICATION] = fnStageOfApplication;
                                            switch (answerAsList[5])
                                            {
                                                case 0:
                                                    FnInfo[FNDESCR_OPENED_DOC] = "Нет открытого документа";
                                                    break;
                                                case 1:
                                                    FnInfo[FNDESCR_OPENED_DOC] = "Отчёт о регистрации." + (answerAsList[6] == 1 ? "   – получены данные документа" : "");
                                                    break;
                                                case 2:
                                                    FnInfo[FNDESCR_OPENED_DOC] = "Отчёт об открытии смены." + (answerAsList[6] == 1 ? "   – получены данные документа" : "");
                                                    break;
                                                case 4:
                                                    FnInfo[FNDESCR_OPENED_DOC] = "Кассовый чек." + (answerAsList[6] == 1 ? "   – получены данные документа" : "");
                                                    break;
                                                case 8:
                                                    FnInfo[FNDESCR_OPENED_DOC] = "Отчёт о закрытии смены." + (answerAsList[6] == 1 ? "   – получены данные документа" : "");
                                                    break;
                                                case 0x10:
                                                    FnInfo[FNDESCR_OPENED_DOC] = "Отчёт о закрытии фискального накопителя." + (answerAsList[6] == 1 ? "   – получены данные документа" : "");
                                                    break;
                                                case 0x11:
                                                    FnInfo[FNDESCR_OPENED_DOC] = "Бланк строкой отчетности." + (answerAsList[6] == 1 ? "   – получены данные документа" : "");
                                                    break;
                                                case 0x12:
                                                    FnInfo[FNDESCR_OPENED_DOC] = "Отчет об изменении параметров регистрации в связи с заменой ФН." + (answerAsList[6] == 1 ? "   – получены данные документа" : "");
                                                    break;
                                                case 0x13:
                                                    FnInfo[FNDESCR_OPENED_DOC] = "Отчет об изменении параметров регистрации." + (answerAsList[6] == 1 ? "   – получены данные документа" : "");
                                                    break;
                                                case 0x14:
                                                    FnInfo[FNDESCR_OPENED_DOC] = "Кассовый чек коррекции." + (answerAsList[6] == 1 ? "   – получены данные документа" : "");
                                                    break;
                                                case 0x15:
                                                    FnInfo[FNDESCR_OPENED_DOC] = "БСО коррекции." + (answerAsList[6] == 1 ? "   – получены данные документа" : "");
                                                    break;
                                                case 0x17:
                                                    FnInfo[FNDESCR_OPENED_DOC] = " Отчет о текущем состоянии расчетов." + (answerAsList[6] == 1 ? "   – получены данные документа" : "");
                                                    break;
                                                default:
                                                    FnInfo[FNDESCR_OPENED_DOC] = "Code(" + answerAsList[5] + ") описание данного кода отсутсвует." + (answerAsList[6] == 1 ? "   – получены данные документа" : "");
                                                    break;
                                            }
                                            sbDescribtion.AppendLine(FnInfo[FNDESCR_OPENED_DOC]);
                                            if (!_uiParamsToUpdate.Contains(FNDESCR_OPENED_DOC)) _uiParamsToUpdate.Add(FNDESCR_OPENED_DOC);
                                            if (answerAsList[7] == 0)
                                            {
                                                FnInfo[FNDESCR_SHIFT_STATE] = "Cмена закрыта";
                                                //_shiftStateFiscalPrinter = FR_SHIFT_CLOSED;
                                                _shiftState = FR_SHIFT_CLOSED;
                                            }
                                            else if (answerAsList[7] == 1)
                                            {
                                                FnInfo[FNDESCR_SHIFT_STATE] = "Cмена открыта";
                                                //_shiftStateFiscalPrinter = FR_SHIFT_OPEN;
                                                _shiftState = FR_SHIFT_OPEN;
                                            }
                                            else
                                            {
                                                FnInfo[FNDESCR_SHIFT_STATE] = "Состояние смены (" + answerAsList[7] + ") - отсутвует описание кода";
                                            }
                                            sbDescribtion.AppendLine(FnInfo[FNDESCR_SHIFT_STATE]);
                                            if (!_uiParamsToUpdate.Contains(FNDESCR_SHIFT_STATE)) _uiParamsToUpdate.Add(FNDESCR_SHIFT_STATE);
                                            StringBuilder sbAF = new StringBuilder();
                                            byte alarmingByte = answerAsList[8];
                                            if (alarmingByte != 0)
                                                sbAF.AppendLine("Предупреждения:");

                                            if ((alarmingByte & 0b_0000_0001) != 0)
                                            {
                                                sbAF.AppendLine("  Срочная замена ФН (до окончания срока действия 3 дня)");
                                            }
                                            if ((alarmingByte & 0b_0000_0010) != 0)
                                            {
                                                sbAF.AppendLine("  Исчерпание ресурса ФН (до окончания срока действия 30 дней)");
                                            }
                                            if ((alarmingByte & 0b_0000_0100) != 0)
                                            {
                                                sbAF.AppendLine("  Переполнение памяти ФН (Архив ФН заполнен на 99%)");
                                            }
                                            if ((alarmingByte & 0b_0000_1000) != 0)
                                            {
                                                sbAF.AppendLine("  Превышено время ожидания ответа ОФД");
                                            }
                                            if ((alarmingByte & 0b_0001_0000) != 0)
                                            {
                                                sbAF.AppendLine("  Отказ по данным ФЛК(признак передается в Подтверждении от ОФД)");
                                            }
                                            if ((alarmingByte & 0b_0010_0000) != 0)
                                            {
                                                sbAF.AppendLine("  Требуется настройка ККТ(признак передается в Подтверждении от ОФД)");
                                            }
                                            if ((alarmingByte & 0b_0100_0000) != 0)
                                            {
                                                sbAF.AppendLine("  ОФД аннулирован (признак передается в Подтверждении от ОФД)");
                                            }
                                            if (sbAF.Length == 0)
                                                sbAF.Append("Предупреждения отсутвуют");
                                            sbDescribtion.AppendLine(sbAF.ToString());
                                            FnInfo[FNDESCR_ALARMING_FLAGS] = sbAF.ToString();
                                            if (!_uiParamsToUpdate.Contains(FNDESCR_ALARMING_FLAGS)) _uiParamsToUpdate.Add(FNDESCR_ALARMING_FLAGS);
                                            DateTime lastFdTime = new DateTime(1970,1,1);
                                            if((int)answerAsList[9]+ (int)answerAsList[10]+ (int)answerAsList[11] > 0)
                                            {
                                                lastFdTime = new DateTime(2000 + answerAsList[9], answerAsList[10], answerAsList[11], answerAsList[12], answerAsList[13], 0);
                                                FnInfo[FNDESCR_LAST_PERFORMED_FD_TIME] = lastFdTime.ToString(FiscalPrinter.DEFAULT_DT_FORMAT);
                                                _lastFdTime = lastFdTime;
                                            }
                                            else
                                            {
                                                FnInfo[FNDESCR_LAST_PERFORMED_FD_TIME] = "Нет даты или дата некорректна";
                                            }
                                            
                                            if (!_uiParamsToUpdate.Contains(FNDESCR_LAST_PERFORMED_FD_TIME)) _uiParamsToUpdate.Add(FNDESCR_LAST_PERFORMED_FD_TIME);
                                            byte[] fnNumberBytes = new byte[16];
                                            for (int i = 14; i < 14 + 16; i++)
                                                fnNumberBytes[i - 14] = answerAsList[i];
                                            FnInfo[FiscalPrinter.FTAG_FN_NUMBER] = ascii.GetString(fnNumberBytes);
                                            sbDescribtion.AppendLine("ФН:" + FnInfo[FiscalPrinter.FTAG_FN_NUMBER]);
                                            if (!_uiParamsToUpdate.Contains(FiscalPrinter.FTAG_FN_NUMBER)) _uiParamsToUpdate.Add(FiscalPrinter.FTAG_FN_NUMBER);
                                            int lastFdNumber = (int)answerAsList[30] + (int)answerAsList[31] * 256 + (int)answerAsList[32] * 256 * 256 + (int)answerAsList[33] * 256 * 256 * 256;
                                            _lastFD = lastFdNumber;
                                            FnInfo[FNDESCR_LAST_PERFORMED_FD_NUMBER] = lastFdNumber.ToString();
                                            if (!_uiParamsToUpdate.Contains(FNDESCR_LAST_PERFORMED_FD_NUMBER)) _uiParamsToUpdate.Add(FNDESCR_LAST_PERFORMED_FD_NUMBER);
                                            sbDescribtion.Append("Последний ФД " + _lastFD);
                                            sbDescribtion.AppendLine(" от " + FnInfo[FNDESCR_LAST_PERFORMED_FD_TIME]);
                                            describtion = sbDescribtion.ToString();
                                        }
                                        break;
                                    case CODE_GET_FN_FORMAT:
                                        if(_fnAnwerCodeInt == OK)
                                        {
                                            FfdFtagFormat = (int)answerAsList[4];
                                            sbDescribtion.AppendLine(FFDStr);
                                            if (answerAsList[5] == 4)
                                                sbDescribtion.AppendLine("Максимально возможный (4)1.2");
                                            else if (answerAsList[5] == 3)
                                                sbDescribtion.AppendLine("Максимально возможный (3)1.1");
                                            else if (answerAsList[5] == 2)
                                                sbDescribtion.AppendLine("Максимально возможный (2)1.05");
                                            else
                                                sbDescribtion.AppendLine("Максимально возможный (" + answerAsList[5] + ") нет описания значения.");
                                            FnInfo[FiscalPrinter.FTAG_FFD] = FFDStr;
                                            if (!_uiParamsToUpdate.Contains(FiscalPrinter.FTAG_FFD)) _uiParamsToUpdate.Add(FiscalPrinter.FTAG_FFD);
                                            describtion = sbDescribtion.ToString();
                                        }
                                        
                                        break;
                                    case CODE_GET_ARCHIVED_FD_INFO:
                                        if(_fnAnwerCodeInt == OK)
                                        {
                                            int type = answerAsList[4];
                                            //type += 65536 * (1+ answerAsList[5]);     // тут это усложняет дальнейшую обработку
                                            DateTime time = new DateTime(2000 + answerAsList[6], answerAsList[7], answerAsList[8], answerAsList[9], answerAsList[10], 0);
                                            int fdNumber = answerAsList[11] + 256 * answerAsList[12] + 65536 * answerAsList[13] + 256 * 65536 * answerAsList[14];
                                            uint fiscalSign = (uint)answerAsList[15] + (uint)256 * answerAsList[16] + (uint)65536 * answerAsList[17] + (uint)256 * 65536 * answerAsList[18];
                                            double summ = 0;
                                            if ( type == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE 
                                                || type == FTAG_FISCAL_DOCUMENT_TYPE_BSO
                                                || type == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE
                                                || type == FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION)
                                            {
                                                type += 16777216 * answerAsList[19];
                                                summ = (double)(((long)answerAsList[20] + 256 * (long)answerAsList[21] + 65536 * (long)answerAsList[22] + 16777216 * (long)answerAsList[23] + 4294967296 * (long)answerAsList[24]) / 100.0);
                                            }// тип и сумма операции
                                            type += 65536 * (1 + answerAsList[5]);
                                            FnReadedDocument fd = new FnReadedDocument(type,time,fdNumber,summ,fiscalSign.ToString());
                                            describtion = fd.Reprezent;
                                            fdReaded.Add(fd);
                                        }
                                        break;
                                    case CODE_GET_FD_ROOT_STLV_HEAD:
                                        if (_fnAnwerCodeInt == OK)
                                        {
                                            int fdType = (int)answerAsList[4] + (int)answerAsList[5] * 256;
                                            sbDescribtion.AppendLine("Тип ФД(" + (fdType) + "): " + FiscalPrinter.fdDocTypes[fdType]);
                                            sbDescribtion.AppendLine("Размер(байт): " + (answerAsList[6] + answerAsList[7] * 256));
                                            // тут сделать приемник данных (лист тегов и тип документа либо просто корневой стлв тег)
                                            
                                            if(_buffer_level_1!=null)
                                                for (int i = 4; i < 8; i++)
                                                    _buffer_level_1.Add(answerAsList[i]);
                                            describtion = sbDescribtion.ToString();
                                        }
                                        break;
                                    case CODE_FN_REG_PARAM:
                                        // При запросе всех параметров регистрации кроме кода ответа данных нет
                                        // При запросе определенного параметра возвращается TLV структура и обработка похожа на запрос tlv структур
                                        if (_fnAnwerCodeInt==OK && answerAsList.Count > 6)
                                        {
                                            int tagNumberFd = (int)answerAsList[4] + (int)answerAsList[5] * 256;
                                            int tlvLength = (int)answerAsList[6] + (int)answerAsList[7] * 256;
                                            if (answerAsList.Count - 10 == tlvLength)
                                            {
                                                byte[] rawDataTlv = new byte[tlvLength];
                                                Array.Copy(answerAsList.ToArray(), 8, rawDataTlv, 0, tlvLength);
                                                FTag tag = new FTag(tagNumberFd, rawDataTlv);
                                                sbDescribtion.AppendLine(tag.ToString());
                                                ftagList.Add(tag);
                                                RegFTags[tag.TagNumber] = tag;
                                                /*if (_buffer_level_1 != null && _fnAnwerCodeInt == OK)
                                                {
                                                    _buffer_level_1.Add(answerAsList[4]);
                                                    _buffer_level_1.Add(answerAsList[5]);
                                                    _buffer_level_1.Add(answerAsList[6]);
                                                    _buffer_level_1.Add(answerAsList[7]);
                                                    _buffer_level_1.AddRange(rawDataTlv);
                                                }*/
                                            }
                                            else
                                            {
                                                sbDescribtion.AppendLine("Длина ответа ФН " + answerAsList.Count + " длина тега " + tlvLength);
                                            }

                                        }
                                        break ;
                                    case CODE_GET_FD_TLV_STRUCT:
                                    case CODE_GET_REG_TLV_STRUCT:
                                        if (_fnAnwerCodeInt == OK || _fnAnwerCodeInt == 8)
                                        {
                                            // по запросам выходило что даже при ответе нет данных возвращается последний тег
                                            // будем пытаться его искать даже при ошибке
                                            if (answerAsList.Count > 7)
                                            {
                                                int tagNumberFd = (int)answerAsList[4] + (int)answerAsList[5] * 256;
                                                int tlvLength = (int)answerAsList[6] + (int)answerAsList[7] * 256;
                                                if (answerAsList.Count - 10 == tlvLength)
                                                {
                                                    //sbDescribtion.AppendLine("Длина ответа ФН и длина возвращенного тега совпали"); // эту строку в последствии удалить
                                                    byte[] rawDataTlv = new byte[tlvLength];
                                                    Array.Copy(answerAsList.ToArray(), 8, rawDataTlv, 0, tlvLength);
                                                    FTag ft = new FTag(tagNumberFd, rawDataTlv);
                                                    if(_command_sent == CODE_GET_REG_TLV_STRUCT)
                                                    {
                                                        RegFTags[ft.TagNumber] = ft;
                                                    }
                                                    sbDescribtion.AppendLine(ft.ToString());
                                                    if (_buffer_level_1 != null && _fnAnwerCodeInt == OK)
                                                    {
                                                        _buffer_level_1.Add(answerAsList[4]);
                                                        _buffer_level_1.Add(answerAsList[5]);
                                                        _buffer_level_1.Add(answerAsList[6]);
                                                        _buffer_level_1.Add(answerAsList[7]);
                                                        _buffer_level_1.AddRange(rawDataTlv);
                                                    }
                                                }
                                                else
                                                {
                                                    sbDescribtion.AppendLine("Длина ответа ФН " + answerAsList.Count + " длина тега " + tlvLength);
                                                }
                                            }
                                            describtion = sbDescribtion.ToString();
                                        }
                                        
                                        break;
                                    
                                    default:
                                        break;
                                }
                                UnlocUartOut();
                                LogAddEvent(new UartEvent(MsgSource.INFO, null, describtion));
                                _buffer_level_0.Clear();
                                //_deviceAvailable = true;
                                tcs_levell_0?.TrySetResult(true); // передаем в верхний обработчик данные о завершении запроса
                                return;
                            }
                            _errorPresentation = "Ошибка в контрольной сумме полученного сообщения - нарушен протокол обмена";
                            LogAddEvent(new UartEvent(MsgSource.ERROR));
                            //_badExchangeLevel_0 = true;
                            tcs_levell_0?.TrySetResult(true);
                        }
                    }
                    else
                    {
                        ResetQueue();
                        UnlocUartOut();
                        _errorPresentation = _fnOperationMessage = "Нарушен протокол обмена.";
                        LogAddEvent(new UartEvent(MsgSource.ERROR,null,"NO START SIGN[04h]"));
                        //_badExchangeLevel_0 = true;
                        tcs_levell_0?.TrySetResult(true);
                        return;
                    }
                }
                else
                {
                    /*  
                     *  получено сообщение 1..3 байта
                     * 
                     *  возможно произошла потеря связи
                     *  либо фн разорвал собщение на части
                     *  ожидаем дополучение данных
                     *  
                     */

                }

                _dataQueue--;
            }
            catch (Exception ex)
            {
                _errorPresentation = ex.Message;
                LogAddEvent(new UartEvent(MsgSource.ERROR));
                ResetQueue();
                Thread.Sleep(_timeOut / 3);
                UnlocUartOut();
                
            } 
            
        }

        public async Task<int> GetFnExpirationAsync(bool updateUi = true)
        {
            tcs_levell_0 = new TaskCompletionSource<bool>();
            if (_port != null && _port.IsOpen)
            {
                tcs_levell_0 = new TaskCompletionSource<bool>();
                SendData(CMD_GET_FN_EXPIRATION);
                await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));
                if (tcs_levell_0.Task.IsCompleted)
                {
                    _fnOperationMessage = "Ошибок нет.";
                }
                else
                {
                    _fnOperationMessage = "Истек таймаут";
                    ResetQueue();
                    LogAddEvent(new UartEvent(MsgSource.INFO, "Истек таймаут", "Проверяем доступность порта"));
                    if (!PortIsOpened)
                        ClosePort();
                    return _SERVICE_CODE_TIMEOUT_OFF;
                }
                if (updateUi && _terminalUi != null && _terminalUi.Created)
                {
                    _terminalUi.UpdateInfoPanel();
                }
            }
            return OK;
        }


        public async Task<int> ReadFnNumberAsync(bool updateUi = true)
        {
            FnInfo.Remove(FiscalPrinter.FTAG_FN_NUMBER);
            if(_port != null && _port.IsOpen)
            {
                byte[] cmd = new byte[] { 1, 0, CODE_GET_FN_NUMBER };
                var crcbyset = scrc.ComputeChecksumBytes(cmd);
                byte[] command = new byte[6] { START_SIGN, 1, 0, CODE_GET_FN_NUMBER, crcbyset[0], crcbyset[1] };
                tcs_levell_0 = new TaskCompletionSource<bool>();
                SendData(command);
                
                await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));
                if (tcs_levell_0.Task.IsCompleted)
                {
                    _fnOperationMessage = "Ошибок нет.";
                }
                else
                {
                    _fnOperationMessage = "Истек таймаут";
                    ResetQueue();
                    LogAddEvent(new UartEvent(MsgSource.INFO, "Истек таймаут", "Проверяем доступность порта"));
                    if(!PortIsOpened)
                        ClosePort();
                    return _SERVICE_CODE_TIMEOUT_OFF;
                }
                if (updateUi && _terminalUi != null && _terminalUi.Created)
                {
                    _terminalUi.UpdateInfoPanel();
                }
                return OK;
            }
            else
            {
                _errorPresentation = "Порт не открыт";
                LogAddEvent(new UartEvent(MsgSource.ERROR));
                _fnOperationMessage = "Нет соединения";
                return NOT_OPENED;
            }
        }
        

        public async Task<int> GetFnStatusAsync(bool updateUi = true)
        {
            if (_port!=null&&_port.IsOpen)
            {
                //byte[] cmd = { 1, 0, GET_FN_STATUS };
                //var crcbyset = scrc.ComputeChecksumBytes(cmd);
                //byte[] command = new byte[6] { START_SIGN, 1, 0, GET_FN_STATUS, crcbyset[0], crcbyset[1] };
                tcs_levell_0 = new TaskCompletionSource<bool>();
                SendData(CMD_GET_FN_STATUS);
                await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));
                if (tcs_levell_0.Task.IsCompleted)
                {
                    _fnOperationMessage = "Ошибок нет.";
                }
                else
                {
                    _fnOperationMessage = "Истек таймаут";
                    ResetQueue();
                    LogAddEvent(new UartEvent(MsgSource.INFO, "Истек таймаут","Проверяем доступность порта"));
                    if(!PortIsOpened)
                        ClosePort();
                    return _SERVICE_CODE_TIMEOUT_OFF;
                }
                if (updateUi && _terminalUi != null && _terminalUi.Created)
                {
                    _terminalUi.UpdateInfoPanel();
                }
                return OK;
            }
            else
            {
                _errorPresentation = "Порт не открыт";
                LogAddEvent(new UartEvent(MsgSource.ERROR));
                _fnOperationMessage = "Нет соединения";
                return NOT_OPENED;
            }

        }

        public string FFDStr
        {
            get
            {
                if (_ffdFtagFormat == 0)
                    return "(0)НФ";

                if (_ffdFtagFormat == 2)
                    return "(2)1.05";
                if (_ffdFtagFormat == 3)
                    return "(3)1.1";
                if (_ffdFtagFormat == 4)
                    return "(4)1.2";
                return "("+_ffdFtagFormat+")";
            }
        }

        public async void GetFfdAsync()
        {
            byte[] cmd = { 1, 0, CODE_GET_FN_FORMAT };
            var crcbyset = scrc.ComputeChecksumBytes(cmd);
            byte[] command = new byte[6] { START_SIGN, 1, 0, CODE_GET_FN_FORMAT, crcbyset[0], crcbyset[1] };
            tcs_levell_0 = new TaskCompletionSource<bool>();
            SendData(command);
            await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));
            if (tcs_levell_0.Task.IsCompleted)
            {
                _fnOperationMessage = "Ошибок нет.";
                _terminalUi.UpdateInfoPanel();
            }
            else
            {
                _fnOperationMessage = "Истек таймаут";
                ResetQueue();

            }
        }

        System.Timers.Timer _timer =null;


        List<FnReadedDocument> _fnArchList = new List<FnReadedDocument>();
        public async Task ReadArchiveAsync(bool readTlv)
        {
            _brakeOperationReading = false;
            await GetFnStatusAsync();
            if (_num1 < 1) _num1 = 1;
            if (_num2 < 1) _num2 = 1;
            if (_num1 > _lastFD) _num1 = _lastFD;
            if (_num2 > LastFd) _num2 = _lastFD;

            if (_num1 > _num2)
            {
                int t = _num1;
                _num1 = _num2;
                _num2 = t;
            }
            double countFd = _num2 - _num1;
            bool showProgress = countFd > 10;
            //int fdCounter = ftagList.Count;

            int fdCnt = _fnArchList.Count;
            for (int i = _num1; i <= _num2 && !_brakeOperationReading; i++)
            {
                _numberFdToRead = i;
                await GetArchivedFdInfo(i,readTlv);

                if (_terminalUi != null && _terminalUi.Created)
                {
                    if (!_acrchiveReadedDocument.Equals(FnReadedDocument.EmptyFD))
                    {
                        _fnArchList.Add(_acrchiveReadedDocument);
                        _terminalUi.UpdateStatusArea("Прочитан " + i, null, showProgress ? 100.0 * (i - _num1) / countFd : -1);
                        //fdCounter = ftagList.Count;
                        _terminalUi.AddFdToTv(_acrchiveReadedDocument);
                    }
                    else
                    {
                        _terminalUi.UpdateStatusArea("ФД " + i + " не прочитан", null, showProgress ? 100.0 * (i - _num1) / countFd : -1);
                    }

                }

            }


        }

        FnReadedDocument _acrchiveReadedDocument;
        public async Task<int> GetArchivedFdInfo(int fdNumber, bool readTlv)
        {
            fdReaded.Clear();
            _acrchiveReadedDocument = FnReadedDocument.EmptyFD;
            List<byte> cmd = new List<byte>();
            cmd.Add(5);
            cmd.Add(0);
            cmd.Add(CODE_GET_ARCHIVED_FD_INFO);
            cmd.Add((byte)(fdNumber % 256));
            cmd.Add((byte)(fdNumber / 256 % 256));
            cmd.Add((byte)(fdNumber / 256 / 256 % 256));
            cmd.Add((byte)(fdNumber / 256 / 256 / 256));
            byte[] crcBytes = scrc.ComputeChecksumBytes(cmd.ToArray());
            cmd.Add(crcBytes[0]);
            cmd.Add(crcBytes[1]);
            byte[] request = new byte[cmd.Count + 1];
            Array.Copy(cmd.ToArray(), 0, request, 1, cmd.Count);
            request[0] = START_SIGN;
            tcs_levell_0 = new TaskCompletionSource<bool>();
            SendData(request);
            await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));
            if (tcs_levell_0.Task.IsCompleted)
            {
                if (fdReaded.Count > 0)
                {
                    if (readTlv)
                    {
                        int attemp = 0;

                        _numberFdToRead = fdNumber;
                        if(_buffer_level_1!=null) 
                            _buffer_level_1.Clear();
                        if(ftagList!=null)
                            ftagList.Clear();
                        await ReadFdTlvAsync(false);
                        if (_buffer_level_1.Count > 0)
                        {
                            FnReadedDocument fnReadedDocument = TranslateFtagsList(_buffer_level_1.ToArray(), fdReaded[0].Type + 16777216 * fdReaded[0].OperationTypeInfo + 65536 * fdReaded[0].OfdSignInfo);
                            
                            _acrchiveReadedDocument = fnReadedDocument;
                            return OK;
                        }
                    }
                    _acrchiveReadedDocument = fdReaded[0];
                    return OK;
                }
                // документа нет - либо нет данных в ФН, либо ошибка при создании ФД должно отобразиться в логах
            }
            
            return RECEVING_DATA_ERROR;
        } 


        //private DateTime _lastPortCheckAvailability = DateTime.MinValue;
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _deviceAvailable = true;
            _errorPresentation = "Канал отправки разблокирован по таймауту.";
            LogAddEvent(new UartEvent(MsgSource.ERROR, null, "Устройство не ответило на запрос id=" + _idRequest.ToString("d8")));
            _fnAnwerCodeInt = -1;
            
            tcs_levell_0?.TrySetResult(true);
        }

        public async Task<int> GetRegistrationParamsAsync(bool updateUi=true, int tagNumber = 0xffff)
        {
            updateUi = updateUi && _terminalUi != null && _terminalUi.Created;
            if (_buffer_level_1 == null)
                _buffer_level_1 = new List<byte>();
            else
                _buffer_level_1.Clear();
            if (!FnInfo.ContainsKey(FNDESCR_EXPIRATION_PERFORMED_REGS))//тут запрашиваем срок действия ФН для получения номера последней регистрации
            {
                await GetFnExpirationAsync(); // проверить реакцию на неактивированный ФН
            }
            // будем считать что получили данные о номере отчета о регистрации
            uint t;
            uint.TryParse(GetFnInfoParam(FNDESCR_EXPIRATION_PERFORMED_REGS), out t);
            if (t == 0)
            {
                // ФН не активирован
                _errorPresentation = "Не получен номер последней регистрации ФН, возможно ФН не активирован.";
                LogAddEvent(new UartEvent(MsgSource.ERROR));
                return RECEVING_DATA_ERROR;
            }

            
            List<byte> cmd = new List<byte>();
            //cmd.Add(4);
            cmd.Add(4);
            cmd.Add(0);
            cmd.Add(CODE_FN_REG_PARAM);
            cmd.Add((byte) t);
            if(tagNumber == 0xffff)
            {
                RegFTags.Clear();
                cmd.Add(0xff);
                cmd.Add(0xff);
            }
            else
            {
                cmd.Add((byte)(tagNumber % 256));
                cmd.Add((byte)(tagNumber / 256));
            }
            byte[] crcBytes = scrc.ComputeChecksumBytes(cmd.ToArray());
            cmd.Add(crcBytes[0]);
            cmd.Add(crcBytes[1]);
            byte[] request = new byte[cmd.Count+1];
            Array.Copy(cmd.ToArray(),0, request,1,cmd.Count);
            request[0] = START_SIGN;
            tcs_levell_0 = new TaskCompletionSource<bool>();
            SendData(request);
            await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));

            if (tcs_levell_0.Task.IsCompleted)
            {
                if(tagNumber == 0xffff)
                {
                    while (_fnAnwerCodeInt == OK)
                    {
                        tcs_levell_0 = new TaskCompletionSource<bool>();
                        SendData(CMD_GET_REG_TLV_STRUCT);
                        await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));
                    }
                    //var regFtags = FTag.FTLVParcer.ParseStructure(_buffer_level_1.ToArray());

                    //RegFTags.AddRange(regFtags);
                    if(updateUi && _terminalUi!=null && _terminalUi.Created )
                    {
                        _terminalUi.FillRegTable();
                    }

                    if (RegFTags.ContainsKey(FTAG_FFD))
                    {
                        var ftffd = RegFTags[FTAG_FFD];
                        if (ftffd != null)
                        {

                            FnInfo[ftffd.TagNumber] = ftffd.Representation;
                            if (ftffd.TagNumber == FTAG_FFD)
                            {
                                FfdFtagFormat = (int)(ftffd.ValueInt);
                            }
                        }
                    }
                    else
                    {
                        FnInfo[1209] = "1.0 или НФ";
                    }

                    if (updateUi && _terminalUi != null && _terminalUi.Created)
                        _terminalUi.UpdateInfoPanel();
                    return OK;
                }

                return _fnAnwerCodeInt==OK ? OK : RECEVING_DATA_ERROR;

            }
            else
            {
                _fnOperationMessage = "Истек таймаут";
                ResetQueue();
                /*if ((DateTime.Now - _lastPortCheckAvailability).TotalMilliseconds > 10 * _timeOut)
                {
                    LogAddEvent(new UartEvent(MsgSource.INFO, "Истек таймаут", "Проверяем доступность порта"));
                    if (!PortIsOpened)
                        ClosePort();
                    _lastPortCheckAvailability = DateTime.Now;
                }*/
                return _SERVICE_CODE_TIMEOUT_OFF;
            }
        }

        


        internal async Task<int> GetFnExchangeAsync(bool updateUi = true)
        {
            tcs_levell_0 = new TaskCompletionSource<bool>();
            SendData(CMD_GET_OFD_EXCHANGE);
            await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));

            if (tcs_levell_0.Task.IsCompleted)
            {
                if (updateUi&& _terminalUi != null && _terminalUi.Created)
                {
                    _terminalUi.UpdateInfoPanel();
                }
                return OK;
            }
            else
            {
                if (updateUi&& _terminalUi != null && _terminalUi.Created)
                {
                    _terminalUi.UpdateStatusArea(_fnAnwerCodeInt.ToString(), FnAnswerCode[_fnAnwerCodeInt]);
                }
                return _SERVICE_CODE_TIMEOUT_OFF;
            }
        }




        /*
         *  tfn_ir - требуется ли реакция интерфейса ТФН
         *  Данная команда чаще всего будет посылаться в виде начала последовательности комманд 
         *  и реакция интерфейса будет замедлять обмен
         *  
         *  
         *  
         */
        internal async Task<int> GetShiftParamsAsync(bool tfn_ir = false)
        {
            tfn_ir = tfn_ir && _terminalUi != null && _terminalUi.Created;
            byte[] request = CMD_GET_SHIFT_PARAMS;

            tcs_levell_0 = new TaskCompletionSource<bool>();
            int send = SendData(request);
            if (send != 0)
            {
                if (tfn_ir && _terminalUi != null && _terminalUi.Created)
                {
                    _terminalUi.UpdateStatusArea(uartRezultPrezentation[send]);
                }
                return send;
            }
            await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));
            if (tcs_levell_0.Task.IsCompleted)
            {
                if (tfn_ir && _terminalUi != null && _terminalUi.Created)
                {
                    if (FnAnswerCode.ContainsKey(_fnAnwerCodeInt))
                        _terminalUi.UpdateStatusArea("Ответ ФН: [" + _fnAnwerCodeInt + "] " + FnAnswerCode[_fnAnwerCodeInt], _performedFdInfo);
                    else
                    {
                        _terminalUi.UpdateStatusArea("Ответ ФН: [" + _fnAnwerCodeInt + "] Код ответа ФН отсутвует в словаре", "");
                    }
                }
                return _fnAnwerCodeInt;
            }
            if (tfn_ir && _terminalUi != null && _terminalUi.Created)
            {
                _terminalUi.UpdateStatusArea(uartRezultPrezentation[RECEVING_DATA_ERROR]);
            }
            return RECEVING_DATA_ERROR; ;
        }


        internal async Task<int> AbortDocument(bool tfn_ir = false)
        {
            
            _performedFdInfo = "";
            tfn_ir = tfn_ir && _terminalUi != null && _terminalUi.Created;
            byte[] request = CMD_ABORT_DOCUMENT;
            _brakeOperationReading = false;
            tcs_levell_0 = new TaskCompletionSource<bool>();
            int send = SendData(request);
            if (send != 0)
            {
                if (tfn_ir && _terminalUi != null && _terminalUi.Created)
                {
                    _terminalUi.UpdateStatusArea(uartRezultPrezentation[send]);
                }
                return send;
            }
            await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));
            if (tcs_levell_0.Task.IsCompleted)
            {
                if (tfn_ir && _terminalUi != null && _terminalUi.Created)
                {
                    if (FnAnswerCode.ContainsKey(_fnAnwerCodeInt))
                        _terminalUi.UpdateStatusArea("Ответ ФН: [" + _fnAnwerCodeInt + "] " + FnAnswerCode[_fnAnwerCodeInt], _performedFdInfo);
                    else
                    {
                        _terminalUi.UpdateStatusArea("Ответ ФН: [" +_fnAnwerCodeInt + "] Код ответа ФН отсутвует в словаре", "");
                    }
                }
                return _fnAnwerCodeInt;
            }
            if (tfn_ir && _terminalUi != null && _terminalUi.Created)
            {
                _terminalUi.UpdateStatusArea(uartRezultPrezentation[RECEVING_DATA_ERROR]);
            }
            return RECEVING_DATA_ERROR;
        }

        public async Task<int> SendTlvData(List<FTag> ftags, bool tfn_ir = false, DataGridViewRow row = null)
        {
            if(ftags == null)
            {
                LogHandle.ol("Не передан контейнер с тегами");
                return INTERRUPTED;
            }
            foreach (FTag tag in ftags)
            {
                LogAddEvent(new UartEvent(MsgSource.INFO, "Передаем теги",tag.ToString()));
            }
            // доп проверка тегов
            // {пренижен номер тега, превышен номер тега}
            // добавить ФЛК
            //bool[] badFtagFlsgs = _CheckSendedFtags(ftags);
            //StringBuilder sberrrr = new StringBuilder();
            //if (badFtagFlsgs[0])
            //{
            //    sberrrr.AppendLine("В списке тегов передан тег с номером ниже минимально-разрешенного");
            //}
            //if (badFtagFlsgs[1])
            //{
            //    sberrrr.AppendLine("В списке тегов передан тег с номером выше максимально-разрешенного");
            //}
            //foreach(var bad in badFtagFlsgs)
            //{
            //    if (bad)
            //    {
            //        LogAddEvent(new UartEvent(MsgSource.ERROR,null, sberrrr.ToString()));
            //        return INTERRUPTED;
            //    }
            //}

            tfn_ir = tfn_ir && _terminalUi != null && _terminalUi.Created;
            List<byte> cmd = new List<byte>();
            cmd.AddRange(new byte[] { 0x0,0x0,CODE_SEND_TLV_LIST});

            foreach (FTag ftag in ftags)
            {
                if (ftag.TagNumber > 0) // возможно тут стоит сделать  больше 1000
                {
                    ftag.RawDataConstructor();
                    if (ftag.RawData != null && ftag.RawData.Length > 0)
                    {
                        cmd.AddRange(ftag.TagAsBytes());
                    }
                }
            }
            int dataLength = cmd.Count;
            if(dataLength == 3)
            {
                if (tfn_ir)
                {
                    _terminalUi.UpdateStatusArea("Нечего передавать", "",0);
                }
                if (row != null)
                {
                    row.Cells[TerminalUi.IND_CELL_PROCESSING_STATUS].Tag = new object[] { "Нет данных для передачи" };
                    _terminalUi.RefreshRowTagInfoExternul(row);
                    return INTERRUPTED;
                }
            }

            cmd[0] = (byte)((dataLength - 2) % 256);
            cmd[1] = (byte)((dataLength - 2) / 256);
            byte[] crcBytes = scrc.ComputeChecksumBytes(cmd.ToArray());
            cmd.Add(crcBytes[0]);
            cmd.Add(crcBytes[1]);
            byte[] request = new byte[cmd.Count + 1];
            Array.Copy(cmd.ToArray(), 0, request, 1, cmd.Count);
            request[0] = START_SIGN;
            tcs_levell_0 = new TaskCompletionSource<bool>();
            int send = SendData(request);
            if (send != 0)
            {
                if (tfn_ir )
                {
                    _terminalUi.UpdateStatusArea(uartRezultPrezentation[send]);
                    
                }
                if (row != null)
                {
                    row.Cells[TerminalUi.IND_CELL_PROCESSING_STATUS].Tag = new object[] { uartRezultPrezentation[send] };
                    _terminalUi.RefreshRowTagInfoExternul(row);
                }
                return SENDING_DATA_ERROR;
            }

            await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));
            if (tcs_levell_0.Task.IsCompleted)
            {
                if (tfn_ir)
                {
                    if (FnAnswerCode.ContainsKey(_fnAnwerCodeInt))
                        _terminalUi.UpdateStatusArea("Ответ ФН: [" + _fnAnwerCodeInt + "] " + FnAnswerCode[_fnAnwerCodeInt], _performedFdInfo);
                    else
                    {
                        _terminalUi.UpdateStatusArea("Ответ ФН: [" + _fnAnwerCodeInt + "] Код ответа ФН отсутвует в словаре", "");
                    }
                    if (row != null)
                    {
                        row.Cells[TerminalUi.IND_CELL_PROCESSING_STATUS].Tag = new object[] { _fnAnwerCodeInt };
                        _terminalUi.RefreshRowTagInfoExternul(row);
                    }
                }
                
                return _fnAnwerCodeInt;
            }
            if (tfn_ir )
            {
                _terminalUi.UpdateStatusArea(uartRezultPrezentation[RECEVING_DATA_ERROR]);
                
            }
            if (row != null)
            {
                row.Cells[TerminalUi.IND_CELL_PROCESSING_STATUS].Tag = new object[] { uartRezultPrezentation[RECEVING_DATA_ERROR] };
                _terminalUi.RefreshRowTagInfoExternul(row);
            }
            return RECEVING_DATA_ERROR;
        }
        

        /*
         * Универсальный метод открытия документов
         * 
         * */
        internal async Task<int> BeginDocument(int documentCode, bool tfn_ir = false)
        {
            _brakeOperationReading = false;
            tfn_ir = tfn_ir && _terminalUi!=null && _terminalUi.Created;
            List<byte> cmd = new List<byte>();
            if (documentCode == FTAG_FISCAL_DOCUMENT_TYPE_OPEN_SHIFT)
            {
                cmd.Add(6);     // длина сообщения
                cmd.Add(0);     // длина сообщения
                cmd.Add(CODE_BEGIN_OPEN_SHIFT);
                cmd.AddRange(DatetimeBytes);
            }
            else if(documentCode == FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_SHIFT)
            {
                cmd.Add(6);     // длина сообщения
                cmd.Add(0);     // длина сообщения
                cmd.Add(CODE_BEGIN_CLOSE_SHIFT);
                cmd.AddRange(DatetimeBytes);
            }
            else if (documentCode == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE)
            {
                cmd.Add(6);     // длина сообщения
                cmd.Add(0);     // длина сообщения
                cmd.Add(CODE_BEGIN_RECEIPT);
                cmd.AddRange(DatetimeBytes);

            }
            else if(documentCode == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE)
            {
                cmd.Add(6);     // длина сообщения
                cmd.Add(0);     // длина сообщения
                cmd.Add(CODE_BEGIN_CORRECTION_RECEIPT);
                cmd.AddRange(DatetimeBytes);
            }
            else if (documentCode == FTAG_FISCAL_DOCUMENT_TYPE_CALCULATION_REPORT)
            {
                cmd.Add(6);     // длина сообщения
                cmd.Add(0);     // длина сообщения
                cmd.Add(CODE_BEGIN_CALCULATION_REPORT);
                cmd.AddRange(DatetimeBytes);
            }
            if (cmd.Count == 0)
            {
                LogHandle.olta("Передан код документа не реализованный в методе: " + documentCode);
                if (tfn_ir)
                {
                    _terminalUi.UpdateStatusArea("Код ФД: " + documentCode, NOT_SUPPORTED_THIS_VER);
                }
                return INTERRUPTED;
            }
            byte[] crcBytes = scrc.ComputeChecksumBytes(cmd.ToArray());
            cmd.Add(crcBytes[0]);
            cmd.Add(crcBytes[1]);
            byte[] request = new byte[cmd.Count + 1];
            Array.Copy(cmd.ToArray(), 0, request, 1, cmd.Count);
            request[0] = START_SIGN;
            tcs_levell_0 = new TaskCompletionSource<bool>();
            int send = SendData(request);
            if (send != 0)
            {
                if (tfn_ir )
                {
                    _terminalUi.UpdateStatusArea(uartRezultPrezentation[send]);

                }
                return send;
            }
            await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));
            if (tcs_levell_0.Task.IsCompleted)
            {
                if (tfn_ir )
                {
                    if (FnAnswerCode.ContainsKey(_fnAnwerCodeInt))
                        _terminalUi.UpdateStatusArea("Ответ ФН: [" + _fnAnwerCodeInt + "] " + FnAnswerCode[_fnAnwerCodeInt], _performedFdInfo);
                    else
                    {
                        _terminalUi.UpdateStatusArea("Ответ ФН: [" + _fnAnwerCodeInt + "] Код ответа ФН отсутвует в словаре", "");
                    }
                }
                return _fnAnwerCodeInt;
            }
            if (tfn_ir )
            {
                _terminalUi.UpdateStatusArea(uartRezultPrezentation[RECEVING_DATA_ERROR]);
            }
            return RECEVING_DATA_ERROR;
        }


        /*
         *
         */
        internal async Task<int> PerformDocument(int documentCode, object[] data = null, bool tfn_ir = false)
        {
            _brakeOperationReading = false;
            tfn_ir = tfn_ir && _terminalUi != null && _terminalUi.Created ;
            _performedFdInfo = "";
            byte[] request = null;
            if(documentCode == FTAG_FISCAL_DOCUMENT_TYPE_OPEN_SHIFT)
            {
                request = CMD_OPEN_SHIFT;
            }
            else if(documentCode == FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_SHIFT)
            {
                request=CMD_CLOSE_SHIFT;
            }
            else if(documentCode == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE 
                || documentCode==FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE)
            {
                if (data == null || data.Length < 2)
                {
                    LogHandle.olta("Не переданы данные или переданы не все необходимые данные для формирования ФД");
                    if (tfn_ir)
                    {
                        _terminalUi.UpdateStatusArea("Код ФД: " + documentCode, "Нет данных для формирования");
                    }
                    return INTERRUPTED;
                }
                object o1 = data[0];
                object o2 = data[1];
                bool o1ok = o1 is sbyte
                    || o1 is byte
                    || o1 is short
                    || o1 is ushort
                    || o1 is int
                    || o1 is uint
                    || o1 is long
                    || o1 is ulong;
                bool o2ok = o2 is byte
                    || o2 is short
                    || o2 is ushort
                    || o2 is int
                    || o2 is uint
                    || o2 is long
                    || o2 is ulong;
                if (!(o1ok && o2ok))
                {
                    LogHandle.olta("Переданы некорректные данные для формирования ФД");
                    if (tfn_ir)
                    {
                        _terminalUi.UpdateStatusArea("Код ФД: " + documentCode, "некорректные данные для формирования");
                    }
                    return INTERRUPTED;
                }
                int opSign = int.Parse(o1.ToString());
                long total = long.Parse(o2.ToString());
                List<byte> cmd = new List<byte>();
                cmd.Add(0xC);       // длина сообщения
                cmd.Add(0);         // длина сообщения
                cmd.Add(CODE_PERFORM_RECEIPT_AND_CORRECTION);
                cmd.AddRange(DatetimeBytes);
                cmd.Add((byte)(opSign % 256));
                //long total = (long)o2;
                cmd.Add((byte)(total % 256));
                total /= 256;
                cmd.Add((byte)(total % 256));
                total /= 256;
                cmd.Add((byte)(total % 256));
                total /= 256;
                cmd.Add((byte)(total % 256));
                total /= 256;
                cmd.Add((byte)(total % 256));
                byte[] crcBytes = scrc.ComputeChecksumBytes(cmd.ToArray());
                cmd.Add(crcBytes[0]);
                cmd.Add(crcBytes[1]);
                request = new byte[cmd.Count + 1];
                Array.Copy(cmd.ToArray(), 0, request, 1, cmd.Count);
                request[0] = START_SIGN;
            }
            else if(documentCode == FTAG_FISCAL_DOCUMENT_TYPE_CALCULATION_REPORT)
            {
                request = CMD_PERFORM_CALCULATION_REPORT;
            }
            if (request == null || request.Length == 0)
            {
                LogHandle.olta("Передан код документа не реализованный в методе: " + documentCode);
                if (tfn_ir)
                {
                    _terminalUi.UpdateStatusArea("Код ФД: " + documentCode, NOT_SUPPORTED_THIS_VER);
                }
                return INTERRUPTED;
            }


            tcs_levell_0 = new TaskCompletionSource<bool>();
            int send = SendData(request);
            if (send != 0)
            {
                if (tfn_ir )
                {
                    _terminalUi.UpdateStatusArea(uartRezultPrezentation[send]);
                }
                return send;
            }
            await Task.WhenAny(tcs_levell_0.Task, Task.Delay(_timeOut));
            if (tcs_levell_0.Task.IsCompleted)
            {
                if (tfn_ir )
                {
                    if (FnAnswerCode.ContainsKey(_fnAnwerCodeInt))
                        _terminalUi.UpdateStatusArea("Ответ ФН: [" + _fnAnwerCodeInt + "] " + FnAnswerCode[_fnAnwerCodeInt], _performedFdInfo);
                    else
                    {
                        _terminalUi.UpdateStatusArea("Ответ ФН: [" + _fnAnwerCodeInt + "] Код ответа ФН отсутвует в словаре", "");
                    }
                    if (_fnAnwerCodeInt == 0)
                    {
                        foreach (DataGridViewRow r in _terminalUi.dataGridView_ftagListToPerform.Rows)
                        {
                            r.Cells[TerminalUi.IND_CELL_PROCESSING_STATUS].Tag = null; ;
                        }
                    }
                }
                

                return _fnAnwerCodeInt;
            }
            if (tfn_ir )
            {
                _terminalUi.UpdateStatusArea(uartRezultPrezentation[RECEVING_DATA_ERROR]);
            }
            return RECEVING_DATA_ERROR;
        }

        FTag GetFtagIc(int tagNumber)
        {
            if(tagNumber == FTAG_CASHIER_NAME)
            {
                if (KKMInfoTransmitter.ContainsKey(FR_CASHIER_NAME_KEY) && !string.IsNullOrEmpty(KKMInfoTransmitter[FR_CASHIER_NAME_KEY]))
                {
                    return new FTag(FTAG_CASHIER_NAME, KKMInfoTransmitter[FR_CASHIER_NAME_KEY], true);
                }
            }
            else if (tagNumber == FTAG_CASHIER_INN)
            {
                if (KKMInfoTransmitter.ContainsKey(FR_CASHIER_INN_KEY) && !string.IsNullOrEmpty(KKMInfoTransmitter[FR_CASHIER_INN_KEY]))
                {
                    return new FTag(FTAG_CASHIER_INN, KKMInfoTransmitter[FR_CASHIER_INN_KEY], true);
                }
            }
            else if(tagNumber == FTAG_APPLIED_TAXATION_TYPE)
            {
                FTag f = GetRegFtag(FTAG_REGISTERED_SNO);
                // сделать настройку сно по умлочанию и выбор из нескольких(первая/последняя)
                if (f != null)
                {
                    uint snos = f.ValueInt;
                    //uint osno = 1;
                    if( snos % 2 ==1)
                    {
                        return new FTag(FTAG_APPLIED_TAXATION_TYPE, 1, true);
                    }
                    if(snos % 4 == 2)
                    {
                        return new FTag(FTAG_APPLIED_TAXATION_TYPE, 2, true);
                    }
                    if (snos % 8 == 4)
                    {
                        return new FTag(FTAG_APPLIED_TAXATION_TYPE, 4, true);
                    }
                    if (snos % 32 == 16)
                    {
                        return new FTag(FTAG_APPLIED_TAXATION_TYPE, 16, true);
                    }
                    if(snos==32)
                        return new FTag(FTAG_APPLIED_TAXATION_TYPE, 32, true);
                }

            }


            return null;
        }

        /*
         * методы FiscalPrinter
         */

        public override bool Connect()
        {
            if(_port==null)
                _port = new SerialPort();
            if (!_port.IsOpen)
            {
                if (OpenPort() != OK)
                {
                    //LogHandle.ol("Не удалось открыть ком-порт");
                    RezultMsg("Не удалось открыть ком-порт "+ConnectionReprezentation());
                    _connected = false;
                    return false;
                } 
            }
            // порт открыт
            FnInfo.Clear();
            _brakeOperationReading = false;
            var resultStatus = Task.Run(async () => await GetFnStatusAsync()).Result;
            if(resultStatus != OK)
            {
                RezultMsg(uartRezultPrezentation[resultStatus]);
                _connected = false;
                return false;
            }
            KKMInfoTransmitter[FR_STATUS_MODE_KEY] = GetFnInfoParam(FNDESCR_OPENED_DOC);

            // данные от ФНа получены 
            if (_fnAnwerCodeInt!=OK)
            {
                if (FnAnswerCode.ContainsKey(_fnAnwerCodeInt))
                {
                    RezultMsg(FnAnswerCode[_fnAnwerCodeInt]);
                }
                else
                {
                    RezultMsg(_fnAnwerCodeInt+" - ответ ФН(нет в словаре)");
                }
                _connected = false;
                return false;
            }

            long fn = -1;
            long.TryParse(FnInfo[FTAG_FN_NUMBER], out fn);
            if (fn <= 0)
            {
                RezultMsg("Не удалось расшифровать номер ФН в ответе на запрос статуса");
                _connected = false;
                return false;
            }
            if (_stageOfUsage == 1) // 3 фискальный режим, 7 ФН закрыт данные не переданы, 15 ФН закрыт данные переданы
            {
                RezultMsg("ФН не активирован, работа в данном интерфейсе невозможна.");
                _connected = false;
                return false;
            }
            ftagList.Clear();
            KKMInfoTransmitter[FR_FIRMWARE_KEY] = _tfn_lib_version;
            //var expiraReq = Task.Run(async () => await _GetFnExpirationAsync()).Result;

            var resultKkt = Task.Run(async () => await GetRegistrationParamsAsync(false,FTAG_KKT_NUMBER_FACTORY)).Result;
            if ( resultKkt == OK && RegFTags.Count > 0 )
            {
                FnInfo[FTAG_KKT_NUMBER_FACTORY] = ftagList[0].ValueStr;
                KKMInfoTransmitter[FR_SERIAL_KEY] = ftagList[0].ValueStr;
            }
            else
            {
                FnInfo[FTAG_KKT_NUMBER_FACTORY] = "Нет данных";
                KKMInfoTransmitter[FR_SERIAL_KEY] = "Нет данных";
            }
            ftagList.Clear();

            var resultUser = /*resultKkt;//*/Task.Run(async () => await GetRegistrationParamsAsync(false, FTAG_USER)).Result;
            if ( resultUser == OK && ftagList.Count > 0 ) 
            {
                FnInfo[FTAG_USER] = ftagList[0].ValueStr;
                KKMInfoTransmitter[FR_OWNER_USER_KEY] = ftagList[0].ValueStr;
            }
            else
            {
                FnInfo[FTAG_USER] = "Нет данных";
                KKMInfoTransmitter[FR_OWNER_USER_KEY] = "Нет данных";
            }
            ftagList.Clear();

            var resultRetailAddress = /*resultKkt;//*/Task.Run(async () => await GetRegistrationParamsAsync(false, FTAG_RETAIL_PLACE_ADRRESS)).Result;
            if (resultRetailAddress == OK && ftagList.Count > 0)
            {
                FnInfo[FTAG_RETAIL_PLACE_ADRRESS] = ftagList[0].ValueStr;
                KKMInfoTransmitter[FR_OWNER_ADDRESS_KEY] = ftagList[0].ValueStr;
            }
            else
            {
                FnInfo[FTAG_RETAIL_PLACE_ADRRESS] = "Нет данных";
                KKMInfoTransmitter[FR_OWNER_ADDRESS_KEY] = "Нет данных";
            }
            ftagList.Clear();

            var resultSno = /*resultKkt;//*/Task.Run(async () => await GetRegistrationParamsAsync(false, FTAG_REGISTERED_SNO)).Result;
            if (resultSno == OK && ftagList.Count > 0)
            {
                FnInfo[FTAG_REGISTERED_SNO] = ftagList[0].ValueInt.ToString();
                string sb = "";
                uint taxes = ftagList[0].ValueInt;
                if (taxes % 2 != 0)
                {
                    sb = SNO_TRADITIONAL;
                    _chosenSno = FR_SNO_OSN;
                    taxes--;
                    if (taxes != 0) sb += ',';
                }
                if (taxes % 4 != 0)
                {
                    sb += SNO_USN_DOHOD;
                    _chosenSno = FR_SNO_USN_D;
                    taxes -= 2;
                    if (taxes != 0) sb += ',';
                }
                if (taxes % 8 != 0)
                {
                    sb += SNO_USN_DR;
                    _chosenSno = FR_SNO_USN_D_R;
                    taxes -= 4;
                    if (taxes != 0) sb += ',';
                }
                if (taxes % 16 != 0)
                {
                    sb = "ЕНВД";
                    _chosenSno = FR_SNO_ENVD;
                    taxes -= 8;
                    if (taxes != 0) sb += ',';
                }
                if (taxes % 32 != 0)
                {
                    sb += SNO_ESHN;
                    _chosenSno = FR_SNO_ESHN;
                    taxes -= 16;
                    if (taxes != 0) sb += ',';
                }
                if (taxes != 0)
                {
                    sb += SNO_PSN;
                    _chosenSno = FR_SNO_PSN;
                }
                KKMInfoTransmitter[FR_REGISTERD_SNO_KEY] = sb;
            }
            else
            {
                FnInfo[FTAG_REGISTERED_SNO] = "Нет данных";
                KKMInfoTransmitter[FR_REGISTERD_SNO_KEY] = "Нет данных";
            }
            ftagList.Clear();

            var resultKktVer = /*resultKkt;//*/Task.Run(async () => await GetRegistrationParamsAsync(false, FTAG_KKT_VERSION)).Result;
            if (resultKktVer == OK && ftagList.Count > 0)
            {
                FnInfo[FTAG_KKT_VERSION] = ftagList[0].ValueStr;
                KKMInfoTransmitter[FR_MODEL_KEY] = "(1188)kktVer: "+ftagList[0].ValueStr;
            }
            else
            {
                FnInfo[FTAG_KKT_VERSION] = "Нет данных";
                KKMInfoTransmitter[FR_MODEL_KEY] = "Нет данных";
            }
            ftagList.Clear();


            var resultFfd = /*resultKkt;//*/Task.Run(async () => await GetRegistrationParamsAsync(false, FTAG_FFD)).Result;
            if (resultFfd == OK && ftagList.Count > 0)
            {
                FfdFtagFormat = (int)ftagList[0].ValueInt;
                //_ffdVer = (int)ftagList[0].ValueInt;
                FnInfo[FTAG_FFD] = ftagList[0].ValueInt.ToString();
                KKMInfoTransmitter[FR_FFDVER_KEY] = FFDStr;
            }
            else
            {
                FnInfo[FTAG_FFD] = "Нет данных";
                KKMInfoTransmitter[FR_FFDVER_KEY] = "Нет данных";
            }
            ftagList.Clear();

            KKMInfoTransmitter[FR_SHIFT_STATE_KEY] = GetFnInfoParam(FNDESCR_SHIFT_STATE);

            int resultExchangeStatus = Task.Run(async () => await GetFnExchangeAsync(false)).Result;
            KKMInfoTransmitter[FR_OFD_EXCHANGE_STATUS_KEY] = GetFnInfoParam(FNDESCR_OFD_EXCHANGE_FD_STRING);
            KKMInfoTransmitter[FR_LAST_FD_NUMBER_KEY] = GetFnInfoParam(FNDESCR_LAST_PERFORMED_FD_NUMBER);

            _ui.UpdateUiKkmDescribtion();

            if (_stageOfUsage==3)
                RezultMsg("Соединение установлено");
            else if(_stageOfUsage == 7 || _stageOfUsage == 15)
            {
                RezultMsg("ФН зарыт, возможно только чтение ФД");
            }
            else
            {
                RezultMsg(_stageOfUsage+ " - этап применения. Нет описания, рекомендуется провести диагностику ФН.");
            }
            int t =Task.Run(async () => await GetRegistrationParamsAsync()).Result;
            _connected = true;
            return true;
        }


        public override void Disconnect()
        {
            _connected = false;
            if (_port!=null)
            {
                ClosePort();
            }
            ClearInfo();
            _ui.UpdateUiKkmDescribtion();
        }

        public override void ReleaseLib()
        {
            if (_port!=null)
            {
                if(_port.IsOpen)
                {
                    Disconnect();
                }
            }
            _port = null;
        }

        public override void ReadDeviceCondition()
        {
            _brakeOperationReading = false;
            var statusResult = Task.Run(async () => await GetFnStatusAsync(false)).Result;
            var exchangeResult = Task.Run(async () => await GetFnExchangeAsync(false)).Result;
            var shiftResult = Task.Run(async () => await GetShiftParamsAsync(false)).Result;

            KKMInfoTransmitter[FR_STATUS_MODE_KEY] = GetFnInfoParam(FNDESCR_OPENED_DOC);
            KKMInfoTransmitter[FR_LAST_FD_NUMBER_KEY] = _lastFD.ToString();
            KKMInfoTransmitter[FR_SHIFT_STATE_KEY] = GetFnInfoParam(FNDESCR_SHIFT_STATE);
            KKMInfoTransmitter[FR_OFD_EXCHANGE_STATUS_KEY] = GetFnInfoParam(FNDESCR_OFD_EXCHANGE_FD_STRING);
            //KKMInfoTransmitter[FR_FIRMWARE_KEY] = "ТОЛЬКО ЧТЕНИЕ ФД!";
            _ui.UpdateUiKkmDescribtion();
        }


        public override bool ConnectionWindow()
        {
            // данный метод вызывается только из Mainform
            _formatLog = AppSettings.TerminalFnLogFormat;
            _logLevel = AppSettings.TerminalFnLogLevel;
            _terminalUi = new TerminalUi(this);
            _terminalUi.ShowDialog();
            KKMInfoTransmitter[FR_CONNECTION_SETTINGS_KEY] = ConnectionReprezentation();
            _ui.UpdateConnectionParams(KKMInfoTransmitter[FR_CONNECTION_SETTINGS_KEY]);

            if ( _port != null && PortIsOpened )
            {
                if(!_connected)
                {
                    _ui.ConnectSwicher.Checked = false;
                    _ui.ConnectSwicher.Checked = true;
                }
                return true;
            }
            else
                _ui.ConnectSwicher.Checked = false;
            return false;
        }

        public override string ConnectionReprezentation()
        {
            return _comName + ":" + _baudRate;
        }

        public override bool OpenShift()
        {
            
            int key = FTAG_FISCAL_DOCUMENT_TYPE_OPEN_SHIFT + 65536 * _ffdFtagFormat;
            if(!FTag.TFNCommonRules.ContainsKey(key)|| FTag.TFNCommonRules[key].Rules.Count == 0)
            {
                RezultMsg(NO_FD_RULE_SET);
                return false;
            }
            int operation = Task.Run(async () => await BeginDocument(FTAG_FISCAL_DOCUMENT_TYPE_OPEN_SHIFT, false)).Result;
            if (operation != OK)
            {
                RezultMsg(_errorPresentation);
                operation = Task.Run(async () => await AbortDocument()).Result;
                return false;
            }
            foreach (var tlvRule in FTag.TFNCommonRules[key].Rules)
            {
                int tlvNumber = tlvRule.TagNumber;
                int sourceData = tlvRule.DataSource;
                string defData = tlvRule.DefaultData;
                List<FTag> l = null;
                FTag f = null;
                //if(tlvNumber == 1188||tlvNumber == 1189)
                //{
                //    LogHandle.ol("debug get reg params");
                //}
                if (sourceData == TFTagRuleSet.RSOURCE_IGNORE)
                {
                    continue;
                }
                else if(sourceData == TFTagRuleSet.RSOURCE_REG_PARAM)
                {
                    f = GetRegFtag(tlvNumber);
                }
                else if(sourceData == TFTagRuleSet.RSOURCE_INCLASS)
                {
                    f = GetFtagIc(tlvNumber);
                }
                // не перебран TFTagRuleSet.RSOURCE_OVERRIDE
                // если такое знаечение то  f - null
                if (f == null && !string.IsNullOrEmpty(defData))
                {
                    try
                    {
                        f = new FTag(tlvNumber,defData,true);
                    }
                    catch
                    {
                        continue;
                    }
                }
                if(f!=null&& f.TagNumber != 0)
                {
                    l = new List<FTag>();
                    l.Add(f);
                }
                if (l == null || l.Count == 0) //стоит добавить проверку на ФЛК обязательных тегов
                    continue;
                var checkRezult = CheckSendedFtags(l);
                bool badFtg = false;
                foreach(var bad in checkRezult)
                {
                    if (bad)
                        badFtg = true;
                }
                if (badFtg)
                {
                    LogHandle.ol("bad ftag in list skip operation");
                    RezultMsg("Проблема с передаваемыми тегами");
                    return false;
                }

                operation = Task.Run(async () => await SendTlvData(l)).Result;
                if (operation != OK)
                {
                    if(tlvRule.Critical)
                    {
                        RezultMsg(_errorPresentation);
                        operation = Task.Run(async () => await AbortDocument()).Result;
                        return false;
                    }
                }
            }
            operation = Task.Run(async () => await PerformDocument(FTAG_FISCAL_DOCUMENT_TYPE_OPEN_SHIFT)).Result;
            if (operation == OK)
            {
                RezultMsg(SUCCESS_MSG);
                return true;
            }
            RezultMsg(_errorPresentation);
            operation = Task.Run(async () => await AbortDocument()).Result;
            return false;
        }

        public override bool CloseShift()
        {
            int key = FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_SHIFT + 65536 * _ffdFtagFormat;
            if (!FTag.TFNCommonRules.ContainsKey(key) || FTag.TFNCommonRules[key].Rules.Count == 0)
            {
                RezultMsg(NO_FD_RULE_SET);
                return false;
            }
            int operation = Task.Run(async () => await BeginDocument(FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_SHIFT, false)).Result;
            if (operation != OK)
            {
                RezultMsg(_errorPresentation);
                operation = Task.Run(async () => await AbortDocument()).Result;
                return false;
            }
            foreach (var tlvRule in FTag.TFNCommonRules[key].Rules)
            {
                int tlvNumber = tlvRule.TagNumber;
                int sourceData = tlvRule.DataSource;
                string defData = tlvRule.DefaultData;
                List<FTag> l = null;
                FTag f = null;
                if (sourceData == TFTagRuleSet.RSOURCE_IGNORE)
                {
                    continue;
                }
                else if (sourceData == TFTagRuleSet.RSOURCE_REG_PARAM)
                {
                    f = GetRegFtag(tlvNumber);
                }
                else if (sourceData == TFTagRuleSet.RSOURCE_INCLASS)
                {
                    f = GetFtagIc(tlvNumber);
                }
                // не перебран TFTagRuleSet.RSOURCE_OVERRIDE
                // если такое знаечение то  f - null
                if (f == null && !string.IsNullOrEmpty(defData))
                {
                    try
                    {
                        f = new FTag(tlvNumber, defData, true);
                    }
                    catch
                    {
                        continue;
                    }
                }
                if (f != null && f.TagNumber != 0)
                {
                    l = new List<FTag>();
                    l.Add(f);
                }
                if (l == null || l.Count == 0) //стоит добавить проверку на ФЛК обязательных тегов
                    continue;
                var checkRezult = CheckSendedFtags(l);
                bool badFtg = false;
                foreach (var bad in checkRezult)
                {
                    if (bad)
                        badFtg = true;
                }
                if (badFtg)
                {
                    LogHandle.ol("bad ftag in list skip operation");
                    RezultMsg("Проблема с передаваемыми тегами");
                    return false;
                }



                operation = Task.Run(async () => await SendTlvData(l)).Result;
                if (operation != OK)
                {
                    if (tlvRule.Critical)
                    {
                        RezultMsg(_errorPresentation);
                        operation = Task.Run(async () => await AbortDocument()).Result;
                        return false;
                    }
                }
            }
            operation = Task.Run(async () => await PerformDocument(FTAG_FISCAL_DOCUMENT_TYPE_CLOSE_SHIFT)).Result;
            if (operation == OK)
            {
                RezultMsg(SUCCESS_MSG);
                return true;
            }
            RezultMsg(_errorPresentation);
            operation = Task.Run(async () => await AbortDocument()).Result;
            return false;
        }

        public override bool PerformFD(FiscalCheque doc)
        {
            //throw new NotImplementedException();
            LogHandle.olta("Отправляем чек в ФН\r\n"+doc.ToString(FiscalCheque.FULL_INFO));
            int totalInPens = (int)Math.Round(doc.TotalSum * 100);
            int paymentInPens = (int)Math.Round(100 * (doc.Cash + doc.ECash + doc.Prepaid + doc.Credit + doc.Provision));

            if (totalInPens < paymentInPens) 
            {
                RezultMsg("ФД неоплачен");
                return false;
            }
            if (totalInPens > paymentInPens)
            {
                // сделать проверку на наличные со сдачей
                RezultMsg("ФД переплачен");
                return false;
            }

            int fdType = 0;
            if(doc.Document == FD_DOCUMENT_NAME_CHEQUE)
            {
                fdType = FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE;
            }
            else
            {
                fdType = FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE;
            }

            int key = fdType + 65536 * _ffdFtagFormat;
            if (!FTag.TFNCommonRules.ContainsKey(key) || FTag.TFNCommonRules[key].Rules.Count == 0)
            {
                RezultMsg(NO_FD_RULE_SET);
                return false;
            }
            int operation = Task.Run(async () => await BeginDocument(fdType, false)).Result;
            if (operation != OK)
            {
                RezultMsg(_errorPresentation);
                operation = Task.Run(async () => await AbortDocument()).Result;
                return false;
            }
            foreach (var tlvRule in FTag.TFNCommonRules[key].Rules)
            {
                int tlvNumber = tlvRule.TagNumber;
                //if(tlvNumber == 1084) 
                //    LogHandle.ol("debug 1084");
                int sourceData = tlvRule.DataSource;
                string defData = tlvRule.DefaultData;
                List<FTag> l = null;
                FTag f = null;
                if(tlvNumber == FTAG_ITEM)
                {
                    foreach(ConsumptionItem i in doc.Items)
                    {
                        if (FfdFtagFormat == 2 && doc.DocumentNameFtagType == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE && AppSettings.TFN_SkipItemsInCoorectionFfd2)
                        {
                            // в чеках коррекции ПР не обязательны
                            continue;
                        }
                            
                        List<FTag> item = i.GetItemFtag(_ffdFtagFormat);    // тут переделать на key
                        operation = Task.Run(async () => await SendTlvData(item)).Result;
                        if (operation != OK)
                        {
                            RezultMsg(_errorPresentation);
                            operation = Task.Run(async () => await AbortDocument()).Result;
                            return false;
                        }
                    }
                }
                else
                {
                    if (sourceData == TFTagRuleSet.RSOURCE_IGNORE)
                    {
                        continue;
                    }
                    else if (sourceData == TFTagRuleSet.RSOURCE_REG_PARAM)
                    {
                        f = GetRegFtag(tlvNumber);
                        if (f != null && f.TagNumber > 0)
                        {
                            l = new List<FTag>();
                            l.Add(f);
                        }
                    }
                    else if (sourceData == TFTagRuleSet.RSOURCE_INCLASS)
                    {
                        l = doc.GetFtagList(tlvNumber);
                        if(tlvNumber == FTAG_APPLIED_TAXATION_TYPE)
                        {
                            if (l == null || l.Count == 0 || l[0].ValueInt == 0)
                            {
                                FTag sno = GetFtagIc(FTAG_APPLIED_TAXATION_TYPE);
                                l = new List<FTag>();
                                l.Add(sno);
                            }
                        }

                    }
                    if ((l == null || l.Count == 0) && !string.IsNullOrEmpty(defData))
                    {
                        try
                        {
                            f = new FTag(tlvNumber, defData, true);
                            l = new List<FTag>();
                            l.Add(f);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    if (l == null||l.Count==0)
                    {
                        //LogHandle.ol("Тег: "+tlvNumber+" - нет данных в чеке");
                        continue;
                    }
                    if (l == null || l.Count == 0) //стоит добавить проверку на ФЛК обязательных тегов
                        continue;
                    var checkRezult = CheckSendedFtags(l);
                    bool badFtg = false;
                    foreach (var bad in checkRezult)
                    {
                        if (bad)
                            badFtg = true;
                    }
                    if (badFtg)
                    {
                        LogHandle.ol("bad ftag in list skip operation");
                        RezultMsg("Проблема с передаваемыми тегами");
                        return false;
                    }


                    operation = Task.Run(async () => await SendTlvData(l)).Result;
                    if (operation != OK)
                    {
                        if (tlvRule.Critical)
                        {
                            RezultMsg(_errorPresentation);
                            operation = Task.Run(async () => await AbortDocument()).Result;
                            return false;
                        }
                    }

                }
            }
            object[] dataForClose = new object[] { (object)(doc.CalculationSign), (object)((int)(Math.Round(doc.TotalSum * 100.0))) };
            operation = Task.Run(async () => await PerformDocument(fdType, dataForClose, false)).Result;
            if (operation != OK)
            {
                RezultMsg(_errorPresentation);
                operation = Task.Run(async () => await AbortDocument()).Result;
                return false;
            }
            RezultMsg(SUCCESS_MSG);
            return true;
        }

        public override FnReadedDocument ReadFD(int docNumber, bool parce = false)
        {
            try
            {
                int rezultReading = Task.Run(async () => await GetArchivedFdInfo(docNumber, parce)).Result;

                if (rezultReading == OK)
                {
                    RezultMsg("Ошибок нет");
                }
                else
                {
                    RezultMsg("Ошибка " + rezultReading);
                }
                return _acrchiveReadedDocument;
            }
            catch (Exception e)
            {
                RezultMsg(e.Message);
                return FnReadedDocument.EmptyFD;
            }
            
        }

        public override bool ContinuePrint()
        {
            return true;
        }

        public override bool CancelDocument()
        {
            int rezult = Task.Run(async () => await AbortDocument()).Result;
            if (rezult == OK)
            {
                RezultMsg(SUCCESS_MSG);
                return true;
            }
            RezultMsg(_errorPresentation);
            //AbortDocument();
            return false;
        }

        public override bool CashRefill(double sum, bool income = true)
        {
            RezultMsg(OK.ToString());
            return true;
        }

        public override bool ChangeDate(int appendDay = 0, DateTime date = default)
        {
            return false;
        }

        private int _numberFdToRead = 0;
        public int NumberFdToRead { get => _numberFdToRead; set => _numberFdToRead = value; }

    }
}
