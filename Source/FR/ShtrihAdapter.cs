using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrvFRLib;

namespace FR_Operator
{
    internal class ShtrihAdapter : FiscalPrinter
    {
        DrvFR Driver = null;
        private const int OLD_DRIVER_ERROR = -99098;
        private bool _oldDriver = false;

        public ShtrihAdapter(MainForm ui)
        {
            this._ui = ui;
            Driver = new DrvFR();
            KKMInfoTransmitter[FR_CONNECTION_SETTINGS_KEY] = ConnectionReprezentation();
            string drvVer = Driver.DriverVersion;
            if (drvVer != null && drvVer.Length > 0)
            {
                var vers = drvVer.Split('.');
                if (vers.Count() >= 1) int.TryParse(vers[0],out driverVerPrime);
                if (vers.Count() >= 2) int.TryParse(vers[1],out driverVerSub);

                if (driverVerPrime < 5)
                {
                    _oldDriver = true;
                }
                else if(driverVerPrime == 5)
                {

                    if (driverVerSub < 18)
                    {
                        _oldDriver = true;
                    }
                }

            }
            if (AppSettings.ShtrihIgnoreOldDriver)
                _oldDriver = false;
            if (_oldDriver)
            {
                RezultMsg("Для работы со ставками НДС 5,7 требуется обновить драйвер");
            }
            
        }

        public override void ReleaseLib()
        {
            if (Driver != null&&_connected) 
            {
                Disconnect();
            }

            LogHandle.ol("Shtrih-ReleaseLib");
        }

        public override bool Connect()
        {
            LogHandle.ol("Establishing a connection to the device");
            if (Driver != null)
            {
                int queryStatus = Driver.GetECRStatus();
                LogHandle.ol("Driver.GetECRStatus " + queryStatus);
                if (queryStatus == 0)
                {
                    Driver.CheckConnection();
                    _connected = Driver.ResultCode==NONE;
                    ReadDeviceCondition();
                    RezultMsg(CONNECTION_ESTABLISHED);
                    _ui.UpdateUiKkmDescribtion();
                    string fw = KKMInfoTransmitter[FR_FIRMWARE_KEY];
                    _useCorrDescriber = fw.Contains("2017") ;
                    int t= TaxesFillsFirmware;
                    t= EnablingPrinting;
                }
                else
                {
                    LogHandle.ol(Driver.ErrorDescription);
                    RezultMsg(Error_codes_dict.ContainsKey(queryStatus) ? Error_codes_dict[queryStatus] : "Ошибка " + queryStatus);
                }
            }
            else
                RezultMsg(NO_DRIVER_FOUNDED);
            return _connected;
        }

        public override void Disconnect()
        {

            LogHandle.ol("disconect shtrih-device");
            _taxesFillFirmware = -1;
            _enablingPrinting = -1;
            if (_connected)
                Driver.Disconnect();
            ClearInfo();
            _ui.UpdateUiKkmDescribtion();
            _connected = false;
        }


        
        /*
         * 0 - обычные штрихи
         * 1 - КЯ (прошивка С2)
         */
        private int _deviceVer = 0;

        public override void ReadDeviceCondition()
        {
            KKMInfoTransmitter[FR_TIME_KEY] = Driver.ECRDate.AddHours(Driver.ECRTime.Hour).AddMinutes(Driver.ECRTime.Minute).ToString(DEFAULT_DT_FORMAT);
            bool notReaded = KKMInfoTransmitter[FR_SERIAL_KEY] == "";
            if (notReaded)
            {
                string firmware = Driver.ECRSoftVersion + " " + Driver.ECRSoftDate.ToString(DEFAULT_D_FORMAT);
                firmwareBuild = Driver.ECRSoftDate;
                KKMInfoTransmitter[FR_FIRMWARE_KEY] = firmware;//Driver.ECRSoftVersion + " " + Driver.ECRSoftDate.ToString(DEFAULT_D_FORMAT);
                LogHandle.ol("Firmware " + firmware);
                
                if (firmware.ToUpper().StartsWith("C.2"))
                {
                    _deviceVer = 1;
                }
            }

            string shift_state;
            Driver.FNGetStatus();
            _lastFD = Driver.DocumentNumber;
            KKMInfoTransmitter[FR_LAST_FD_NUMBER_KEY] = _lastFD.ToString();
            if (Driver.FNSessionState == 1)
            {
                Driver.GetECRStatus();
                if (Driver.IsFM24HoursOver)
                {
                    shift_state = "Смена истекла";
                    _shiftState = FR_SHIFT_EXPIRED;
                }
                else
                {
                    shift_state = "Смена открыта";
                    _shiftState = FR_SHIFT_OPEN;
                }
            }
            else
            {
                shift_state = "Смена закрыта";
                _shiftState = FR_SHIFT_CLOSED;
            }
            KKMInfoTransmitter[FR_SHIFT_STATE_KEY] = shift_state;
            if (notReaded)
            {
                LogHandle.ol(shift_state);
                LogHandle.ol("Last fd: "+ KKMInfoTransmitter[FR_LAST_FD_NUMBER_KEY]);
                /*
                 * настройка отключения печати считывается и сиспользуется то в _PrintDisableFlagSet и _PrintDisableFlagRestore
                if (_deviceVer == 1) *//* прошивки С2 устройства с КЯ *//*
                {
                    Driver.TableNumber = 1;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 48;
                }
                else *//*if(_deviceVer == 0) обычный штрих *//*
                {
                    Driver.TableNumber = 17;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 7;
                }

                Driver.ReadTable();
                _originalPrintDisableFlag = Driver.ValueOfFieldInteger;*/

                Driver.GetDeviceMetrics();
                KKMInfoTransmitter[FR_MODEL_KEY] = Driver.UDescription;
                LogHandle.ol(KKMInfoTransmitter[FR_MODEL_KEY]);
                Driver.ReadSerialNumber();
                KKMInfoTransmitter[FR_SERIAL_KEY] = Driver.SerialNumber;
                LogHandle.ol(KKMInfoTransmitter[FR_SERIAL_KEY]);
                if (_deviceVer == 1) /* прошивки С2 устройства с КЯ */
                {
                    Driver.TableNumber = 13;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 7;
                }
                else /*if(_deviceVer == 0) обычный штрих */
                {
                    Driver.TableNumber = 18;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 7;
                }
                
                Driver.ReadTable();
                KKMInfoTransmitter[FR_OWNER_USER_KEY] = UserConversion(Driver.ValueOfFieldString);
                LogHandle.ol(KKMInfoTransmitter[FR_OWNER_USER_KEY]);
                
                if(_deviceVer == 1)
                {
                    Driver.TableNumber = 13;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 9;
                }
                else /*if(_deviceVer == 0) обычный штрих */
                {
                    Driver.TableNumber = 18;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 9;
                }
                
                Driver.ReadTable();
                KKMInfoTransmitter[FR_OWNER_ADDRESS_KEY] = Driver.ValueOfFieldString;
                LogHandle.ol(KKMInfoTransmitter[FR_OWNER_ADDRESS_KEY]);
                if(_deviceVer == 1)
                {
                    Driver.TableNumber = 13;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 5;
                }
                else
                {
                    Driver.TableNumber = 18;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 5;
                }
                
                Driver.ReadTable();
                string sb = "";
                int taxes = Driver.ValueOfFieldInteger;
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
                LogHandle.ol("Taxation types "+sb);
                if(_deviceVer == 1)
                {
                    Driver.TableNumber = 10;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 29;
                }
                else
                {
                    Driver.TableNumber = 17;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 17;
                }
                
                Driver.ReadTable();
                int ffd_ver = Driver.ValueOfFieldInteger;
                if (ffd_ver == 2)
                {
                    KKMInfoTransmitter[FR_FFDVER_KEY] = "1.05";
                    _ffdVer = 105;
                }
                else if (ffd_ver == 3)
                {
                    KKMInfoTransmitter[FR_FFDVER_KEY] = "1.1";
                    _ffdVer = 110;
                }
                else if (ffd_ver > 3)
                {
                    KKMInfoTransmitter[FR_FFDVER_KEY] = "1.2";
                    _ffdVer = 120;
                }
                LogHandle.ol("FFD " + KKMInfoTransmitter[FR_FFDVER_KEY]);
                if(_deviceVer == 1)
                {
                    Driver.TableNumber = 13;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 3;
                }
                else
                {
                    Driver.TableNumber = 18;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 3;
                }
                
                Driver.ReadTable();
                KKMInfoTransmitter[FR_REGFNS_KEY] = Driver.ValueOfFieldString;
                LogHandle.ol("rn " + KKMInfoTransmitter[FR_REGFNS_KEY]);
                if(_deviceVer == 1)
                {
                    Driver.TableNumber = 13;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 4;
                }
                else
                {
                    Driver.TableNumber = 18;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 4;
                }
                
                Driver.ReadTable();
                KKMInfoTransmitter[FR_FN_SERIAL_KEY] = Driver.ValueOfFieldString;
                LogHandle.ol("fn serial "+ KKMInfoTransmitter[FR_FN_SERIAL_KEY]);
            }
            if(_deviceVer == 1)
            {
                Driver.TableNumber = 15;
            }
            else
            {
                Driver.TableNumber = 20;
            }
            
            Driver.RowNumber = 1;
            Driver.FieldNumber = 3;
            Driver.ReadTable();
            string exchengeStatus = Driver.ValueOfFieldString + " / ";

            if(_deviceVer == 1)
            {
                Driver.TableNumber = 15;
            }
            else
            {
                Driver.TableNumber = 20;
            }
            
            Driver.RowNumber = 1;
            Driver.FieldNumber = 5;
            Driver.ReadTable();
            KKMInfoTransmitter[FR_OFD_EXCHANGE_STATUS_KEY] = exchengeStatus + Driver.ValueOfFieldString;

            int mode = Driver.ECRMode;
            KKMInfoTransmitter[FR_STATUS_MODE_KEY] = Mode_list.ContainsKey(mode) ? Mode_list[mode] : mode.ToString();

            notReaded = KKMInfoTransmitter[FR_SERIAL_KEY] == "";
            if (notReaded) RezultMsg("");
            _ui.UpdateUiKkmDescribtion();
        }

        public override bool ConnectionWindow()
        {
            if (Driver.ShowProperties() == 0)
            {
                KKMInfoTransmitter[FR_CONNECTION_SETTINGS_KEY] = ConnectionReprezentation();
                _ui.UpdateConnectionParams(KKMInfoTransmitter[FR_CONNECTION_SETTINGS_KEY]);
                RezultMsg(SUCCESS_MSG);
                return true;
            }
            RezultMsg(KKMInfoTransmitter[INFO_MESSAGE_KEY]);
            return false;
        }

        public override string ConnectionReprezentation()
        {
            if (Driver != null)
            {
                string connectionParams;
                if (Driver.ConnectionType == SHTRIH_CONNECTION_TYPE_RS232)
                {
                    connectionParams = "COM" + Driver.ComNumber + ":" + _baudrate[Driver.BaudRate];
                }
                else if (Driver.ConnectionType == SHTRIH_CONNECTION_TYPE_ТСР)
                {
                    connectionParams = "TCP:" + Driver.IPAddress + ":" + Driver.TCPPort;
                }
                else
                    connectionParams = "DCOM/TCP server or smth unknown";
                RezultMsg(SUCCESS_MSG);
                return connectionParams;
            }
            else
                RezultMsg("Драйвер не инициализирован");
            return "";
        }

        public override bool OpenShift()
        {
            if (Driver == null)
            {
                RezultMsg(NO_DRIVER_FOUNDED);
                return false;
            }
            if (!_connected)
            {
                RezultMsg(CONNECTION_NOT_ESTABLISHED);
                return false;
            }
            if (_shiftState != FR_SHIFT_CLOSED)
            {
                RezultMsg("Смена уже открыта");
                return false;
            }
            _PrintDisableFlagSet();
            Driver.BeginDocument();
            
            int errorCode = Driver.FNBeginOpenSession();
            if (errorCode != NONE && errorCode != 94)
            {
                LogHandle.ol("FNBeginOpenSession");
                RezultMsg("Ошибка " + errorCode + " " + (Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ErrorDescription));
                return false;
            }
            if (!string.IsNullOrEmpty(KKMInfoTransmitter[FR_CASHIER_NAME_KEY]))
            {
                Driver.TagNumber = FTAG_CASHIER_NAME;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = KKMInfoTransmitter[FR_CASHIER_NAME_KEY];
                Driver.FNSendTag();
                if (!string.IsNullOrEmpty(KKMInfoTransmitter[FR_CASHIER_INN_KEY]))
                {
                    Driver.TagNumber = FTAG_CASHIER_INN;
                    Driver.TagType = SI_ttString;
                    Driver.TagValueStr = KKMInfoTransmitter[FR_CASHIER_NAME_KEY];
                    Driver.FNSendTag();
                }
            }
            errorCode = Driver.FNOpenSession();
            if (errorCode != NONE)
            {
                LogHandle.ol("FNOpenSession");
                RezultMsg("Ошибка " + errorCode + " " + (Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ErrorDescription));
                _PrintDisableFlagRestore();
                return false;
            }
            Driver.EndDocument();
            if (Driver.WaitForPrinting() != 0)
            {
                RezultMsg("Ошибка при печати Z " + Driver.ResultCodeDescription);
                return false;
            }
            RezultMsg(SUCCESS_MSG);
            return true;
        }

        public override bool CloseShift()
        {
            if (Driver == null)
            {
                RezultMsg(NO_DRIVER_FOUNDED);
                return false;
            }
            if (!_connected)
            {
                RezultMsg(CONNECTION_NOT_ESTABLISHED);
                return false;
            }
            if (_shiftState == FR_SHIFT_CLOSED)
            {
                RezultMsg("Смена уже закрыта");
                return false;
            }
            _PrintDisableFlagSet();
            int errorCode = Driver.BeginDocument();
            LogHandle.ol("BeginDocument " + errorCode);
            errorCode = Driver.FNBeginCloseSession();
            if (errorCode != NONE && errorCode != 94)
            {
                LogHandle.ol("FNBeginCloseSession");
                RezultMsg("Ошибка " + errorCode + " " + (Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ErrorDescription));
                _PrintDisableFlagRestore();
                return false;
            }
            if (!string.IsNullOrEmpty(KKMInfoTransmitter[FR_CASHIER_NAME_KEY]))
            {
                Driver.TagNumber = FTAG_CASHIER_NAME;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = KKMInfoTransmitter[FR_CASHIER_NAME_KEY];
                Driver.FNSendTag();
                if (!string.IsNullOrEmpty(KKMInfoTransmitter[FR_CASHIER_INN_KEY]))
                {
                    Driver.TagNumber = FTAG_CASHIER_INN;
                    Driver.TagType = SI_ttString;
                    Driver.TagValueStr = KKMInfoTransmitter[FR_CASHIER_NAME_KEY];
                    Driver.FNSendTag();
                }
            }
            errorCode = Driver.FNCloseSession();
            if (errorCode != NONE)
            {
                LogHandle.ol("FNCloseSession");
                RezultMsg("Ошибка " + errorCode + " " + (Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ErrorDescription));
                _PrintDisableFlagRestore();
                return false;
            }
            Driver.EndDocument();
            if (Driver.WaitForPrinting() != 0)
            {
                RezultMsg("При печати Z возникла ошибка "+Driver.ResultCodeDescription);
                return false;
            }
            RezultMsg(SUCCESS_MSG);
            return true;
        }

        public override bool PerformFD(FiscalCheque doc)
        {
            LogHandle.ol("Отправляем документ в ФР" + Environment.NewLine + doc.ToString(FiscalCheque.FULL_INFO));
            if (doc == null)
            {
                RezultMsg(EMPTY_FDOC);
                return false;
            }
            if (Driver == null)
            {
                RezultMsg(NO_DRIVER_FOUNDED);
                return false;
            }
            if (!_connected)
            {
                RezultMsg(CONNECTION_NOT_ESTABLISHED);
                return false;
            }
            if (doc == null)
            {
                RezultMsg("Передан пустой документ");
                return false;
            }
            /*if (_oldDriver)
            {
                RezultMsg("Обновите драйвер");
                return false;
            }*/

            if (!_dontPrint)
            {

                if (doc.IsExtendedInfoForPrinting)
                {
                    //отступ
                    if (AppSettings.ExtendedInfoTopOffset > 0)
                    {
                        Driver.Password = 30;
                        Driver.UseReceiptRibbon = true;
                        Driver.StringQuantity = AppSettings.ExtendedInfoTopOffset;
                        Driver.FeedDocument();
                    }

                    foreach (string s in doc.ExtendedInfoForPrinting)
                    {
                        Driver.Password = 30;
                        Driver.UseReceiptRibbon = true;
                        Driver.StringForPrinting = s;
                        Driver.FontType = AppSettings.ShtrihFontForPrinting;
                        Driver.PrintStringWithFont();
                    }

                    if (AppSettings.ExtendedInfoCleanAfterPrint)
                    {
                        doc.ExtendedInfoForPrinting.Clear();
                    }
                    //отступ
                    if (AppSettings.ExtendedInfoBottomOffset > 0)
                    {
                        Driver.Password = 30;
                        Driver.UseReceiptRibbon = true;
                        Driver.StringQuantity = AppSettings.ExtendedInfoBottomOffset;
                        Driver.FeedDocument();
                    }
                }


                if (AppSettings.ShtrihPrintPropertyData && doc.IsPropertiesData)
                {
                    Driver.Password = 30;
                    Driver.UseReceiptRibbon = true;
                    Driver.StringForPrinting = doc.PropertiesData;
                    Driver.FontType = AppSettings.ShtrihFontForPrinting;
                    Driver.PrintStringWithFont();
                }
            }


            _PrintDisableFlagSet();
            if (doc.Document == FD_DOCUMENT_NAME_CHEQUE)  
            {
                bool p1 = _PerformChequeCommon(doc);
                _PrintDisableFlagRestore();
                ReadDeviceCondition();
                return p1;
            }
            else if (doc.Document == FD_DOCUMENT_NAME_CORRECTION_CHEQUE) //добавить метод пробития для старых прошивок
            {
                bool p1 = _PerformCorrectionOptimal(doc);
                _PrintDisableFlagRestore();
                ReadDeviceCondition();
                return p1;
            }
            RezultMsg("Неизвестный документ");
            return false;
        }

        private bool _PerformChequeCommon(FiscalCheque doc)
        {
            int errorCode = NONE;
            if (doc.CalculationSign == FD_CALCULATION_INCOME_LOC) Driver.CheckType = 0;
            else if (doc.CalculationSign == FD_CALCULATION_BACK_INCOME_LOC) Driver.CheckType = 2;
            else if (doc.CalculationSign == FD_CALCULATION_EXPENCE_LOC) Driver.CheckType = 1;
            else if (doc.CalculationSign == FD_CALCULATION_BACK_EXPENCE_LOC) Driver.CheckType = 3;

            if(errorCode != NONE)
            {
                RezultMsg("OpenCheck. Ошибка " + errorCode + " " + (Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ResultCodeDescription));
                KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = Driver.ResultCodeDescription;
                //_PrintDisableFlagRestore();
                return false;
            }
            errorCode = Driver.OpenCheck();
            if (errorCode != NONE)
            {
                RezultMsg("OpenCheck. Ошибка " + errorCode + " " + (Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ResultCodeDescription));
                KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = Driver.ResultCodeDescription;
                //_PrintDisableFlagRestore();
                //if(AppSettings.shtrihChequeMethod == AppSettings.ShtrihPerformChequeMethods.Buffereing)Driver.EndDocument();
                return false;
            }
            if (AppSettings.OverideRetailAddress && !string.IsNullOrEmpty(doc.RetailAddress))
            {
                LogHandle.ol("Перезапись адреса места установки " + doc.RetailAddress);
                Driver.TagNumber = FTAG_RETAIL_PLACE_ADRRESS;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = doc.RetailAddress;
                Driver.FNSendTag();
            }
            if (AppSettings.OverideRetailPlace && !string.IsNullOrEmpty(doc.RetailPlace))
            {
                LogHandle.ol("Перезапись места установки " + doc.RetailPlace);
                Driver.TagNumber = FTAG_RETAIL_PLACE;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = doc.RetailPlace;
                Driver.FNSendTag();
            }
            //1008 Телефон или электронный адрес покупателя
            if (!string.IsNullOrEmpty(doc.EmailPhone))
            {
                Driver.TagType = SI_ttString;
                Driver.TagNumber = FTAG_DESTINATION_EMAIL;
                Driver.TagValueStr = doc.EmailPhone;
                int r = Driver.FNSendTag();
                LogHandle.ol(r+"  Добавляем электронный адрес " + doc.EmailPhone);
            }
            if (doc.InternetPayment)
            {
                LogHandle.ol("Добавляем признак расчета в интернет ");
                Driver.TagType = SI_ttByte;
                Driver.TagNumber = FTAG_INTERNET_PAYMENT;
                Driver.TagValueInt = 1;
                Driver.FNSendTag();
            }

            if (doc.IsPropertiesData)
            {
                Driver.TagNumber = FTAG_PROPERTIES_DATA;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = doc.PropertiesData;
                Driver.FNSendTag();
            }
            if (doc.IsProperties1084)
            {
                Driver.TagNumber = FTAG_PRORERTIES_1084;
                Driver.FNBeginSTLVTag();
                int my_TagID = Driver.TagID;

                Driver.TagID = my_TagID;
                Driver.TagNumber = FTAG_PROPERTIES_PROPERTY_NAME;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = doc.PropertiesPropertyName;
                LogHandle.ol("Добавляем наименование доп. рекв. пользователя " + doc.PropertiesPropertyName + "\tответ драйвера:  " + Driver.FNAddTag());

                Driver.TagID = my_TagID;
                Driver.TagNumber = FTAG_PROPERTIES_PROPERTY_VALUE;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = doc.PropertiesPropertyValue;
                LogHandle.ol("Добавляем наименование доп. рекв. пользователя " + doc.PropertiesPropertyValue + "\tответ драйвера:  " + Driver.FNAddTag());
                LogHandle.ol(Driver.FNSendSTLVTag().ToString());
            }

            double itemsSumm = 0;
            foreach (ConsumptionItem item in doc.Items)
            {
                itemsSumm += item.Sum;
                if (doc.Document == FD_DOCUMENT_NAME_CORRECTION_CHEQUE && _ffdVer <= FR_FFD105) continue; // в чеках коррекции 1.05 нет предметов расчета
                if (!_RegisterItem(item, doc.CalculationSign, ref errorCode))
                {
                    _CriticalCheqErrorServiceOperations(errorCode);
                    //_PrintDisableFlagRestore();
                    return false;
                }
            }
            

            //1021 Кассир
            if (!string.IsNullOrEmpty(doc.Cashier) && doc.Cashier != DEFAULT_CASHIER)
            {
                LogHandle.ol("Добавляем кассира " + doc.Cashier);
                Driver.TagNumber = FTAG_CASHIER_NAME;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = doc.Cashier;
                Driver.FNSendTag();
                //1203 ИНН кассира
                if (!string.IsNullOrEmpty(doc.CashierInn))
                {
                    LogHandle.ol("Добавляем ИНН кассира " + doc.Cashier);
                    Driver.TagNumber = FTAG_CASHIER_INN;
                    Driver.TagType = SI_ttString;
                    Driver.TagValueStr = doc.CashierInn;
                    Driver.FNSendTag();
                }
            }
            if(doc.BuyerInformation)
            {
                if (_ffdVer < FR_FFD110)
                {
                    //1227 Покупатель (клиент)
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyer))
                    {
                        Driver.TagType = SI_ttString;
                        Driver.TagNumber = FTAG_BUYER_INFORMATION_BUYER;
                        Driver.TagValueStr = doc.BuyerInformationBuyer;
                        LogHandle.ol("Добавляем наименование пользователя (режим 1.05)" + doc.BuyerInformationBuyer + "\tответ драйвера:  " + Driver.FNSendTag());
                    }
                    //1228 ИНН покупателя (клиента)
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerInn))
                    {
                        Driver.TagNumber = FTAG_BUYER_INFORMATION_BUYER_INN;
                        Driver.TagType = SI_ttString;
                        Driver.TagValueStr = doc.BuyerInformationBuyerInn;
                        LogHandle.ol("Добавляем ИНН покупателя (режим 1.05)" + doc.BuyerInformationBuyerInn + "\tответ драйвера:  " + Driver.FNSendTag());
                    }
                }
                else
                {
                    Driver.TagNumber = FTAG_BUYER_INFORMATION;
                    Driver.FNBeginSTLVTag();
                    int my_TagID = Driver.TagID;
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyer))
                    {
                        Driver.TagID = my_TagID;
                        Driver.TagNumber = FTAG_BUYER_INFORMATION_BUYER;
                        Driver.TagType = SI_ttString;
                        Driver.TagValueStr = doc.BuyerInformationBuyer;
                        LogHandle.ol("Добавляем наименование пользователя (режим 1.2)" + doc.BuyerInformationBuyer + "\tответ драйвера:  " + Driver.FNAddTag());

                    }
                    bool buyerInn = (!string.IsNullOrEmpty(doc.BuyerInformationBuyerInn)) && CorrectInn(doc.BuyerInformationBuyerInn);

                    if (buyerInn)
                    {
                        Driver.TagID = my_TagID;
                        Driver.TagNumber = FTAG_BUYER_INFORMATION_BUYER_INN;
                        Driver.TagType = SI_ttString;
                        Driver.TagValueStr = doc.BuyerInformationBuyerInn;
                        LogHandle.ol("Добавляем ИНН покупателя (режим 1.2)" + doc.BuyerInformationBuyerInn + "\tответ драйвера:  " + Driver.FNAddTag());
                    }
                    if( AppSettings.AlwaysSendBuyerDocData || (!buyerInn ))
                    {
                        if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerCitizenship))
                        {
                            Driver.TagID = my_TagID;
                            Driver.TagNumber = FTAG_BI_CITIZENSHIP;
                            Driver.TagType = SI_ttString;
                            Driver.TagValueStr = doc.BuyerInformationBuyerCitizenship;
                            LogHandle.ol("Добавляем гражданство покупателя (режим 1.2)" + doc.BuyerInformationBuyerCitizenship + "\tответ драйвера:  " + Driver.FNAddTag());
                        }
                        if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerDocumentCode))
                        {
                            Driver.TagID = my_TagID;
                            Driver.TagNumber = FTAG_BI_DOCUMENT_CODE;
                            Driver.TagType = SI_ttString;
                            Driver.TagValueStr = doc.BuyerInformationBuyerDocumentCode;
                            LogHandle.ol("Добавляем код документа покупателя (режим 1.2)" + doc.BuyerInformationBuyerInn + "\tответ драйвера:  " + Driver.FNAddTag());
                        }
                        if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerDocumentData))
                        {
                            Driver.TagID = my_TagID;
                            Driver.TagNumber = FTAG_BI_DOCUMENT_DATA;
                            Driver.TagType = SI_ttString;
                            Driver.TagValueStr = doc.BuyerInformationBuyerDocumentData;
                            LogHandle.ol("Добавляем данные документа покупателя (режим 1.2)" + doc.BuyerInformationBuyerDocumentData + "\tответ драйвера:  " + Driver.FNAddTag());
                        }
                        if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerBirthday))
                        {
                            Driver.TagID = my_TagID;
                            Driver.TagNumber = FTAG_BI_BIRTHDAY;
                            Driver.TagType = SI_ttString;
                            Driver.TagValueStr = doc.BuyerInformationBuyerBirthday;
                            LogHandle.ol("Добавляем ДР покупателя (режим 1.2)" + doc.BuyerInformationBuyerBirthday + "\tответ драйвера:  " + Driver.FNAddTag());
                        }
                    }
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerAddress))
                    {
                        Driver.TagID = my_TagID;
                        Driver.TagNumber = FTAG_BI_ADDRESS;
                        Driver.TagType = SI_ttString;
                        Driver.TagValueStr = doc.BuyerInformationBuyerAddress;
                        LogHandle.ol("Добавляем адрес покупателя (режим 1.2)" + doc.BuyerInformationBuyerAddress + "\tответ драйвера:  " + Driver.FNAddTag());
                    }
                    LogHandle.ol(Driver.FNSendSTLVTag().ToString());
                }
            }
            

            
            int t = (int)Math.Round((itemsSumm - doc.TotalSum) * 100);
            
            if (AppSettings.ShtrihCloseCheckMethod == 0)
            {
                if (t > 0)
                {
                    LogHandle.ol("Данный метод регистрации чека не поддерживает не округление итога");
                    _CriticalCheqErrorServiceOperations();
                    //_PrintDisableFlagRestore();
                    RezultMsg("Используется метод оформления чеков не поддерживающий округление итога");
                    return false;
                }
                if (doc.Prepaid + doc.Credit + doc.Provision > 0.0099)
                {
                    LogHandle.ol("Данный метод регистрации чека не поддерживает авансы кредиты и ВП");
                    _CriticalCheqErrorServiceOperations();
                    //_PrintDisableFlagRestore();
                    RezultMsg("Используется метод оформления чеков не поддерживающий авансы кредиты и ВП");
                    return false;
                }
                // 1055 СНО
                if (AppSettings.UsingCustomSno)
                {
                    LogHandle.ol("Попытка записать СНО в чек");
                    Driver.TagNumber = FTAG_APPLIED_TAXATION_TYPE;
                    Driver.TagType = SI_ttByte;
                    Driver.TagValueInt = doc.Sno;
                    //Driver.TagValueBin = doc.Sno;
                    Driver.FNSendTag();
                    LogHandle.ol(Driver.ErrorDescription);
                }
                LogHandle.ol("добавляем суммы чека");
                Driver.StringForPrinting = "";
                Driver.Summ1 = (decimal)doc.Cash;
                Driver.Summ2 = (decimal)doc.ECash;
                Driver.Summ3 = 0;
                Driver.Summ4 = 0;
                Driver.DiscountOnCheck = 0;
                Driver.Tax1 = 0;
                Driver.Tax2 = 0;
                Driver.Tax3 = 0;
                Driver.Tax4 = 0;
                errorCode = Driver.CloseCheck();
                if (errorCode != NONE)
                {
                    _CriticalCheqErrorServiceOperations(errorCode);
                    //_PrintDisableFlagRestore();
                    return false;
                }
            }
            else
            {
                if (!_CloseCheckCM(doc, itemsSumm, ref errorCode))
                {
                    return false;
                }
            }
            
            LogHandle.ol("Ожидание печати документа");
            errorCode = Driver.WaitForPrinting();
            if (errorCode != 0)
            {
                RezultMsg("Чек проведен с ошибкой при печати " + errorCode + " " + (Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ResultCodeDescription));
                // при отсутствии бумаги попытка программироваания таблиц выдаст ошибку 
                // восстановление флага отключения печати бессыленно т.к. без печати ошибки ошибки печати быть не может
                return false;
            }
            //_PrintDisableFlagRestore();
            RezultMsg(SUCCESS_MSG);
            return true;
        }


        bool strictCode1162Work = false;
        bool _RegisterItem(ConsumptionItem item, int sign, ref int errorCode)
        {
            //int errorCode = 0;
            LogHandle.ol(item.ToString());
            Driver.Quantity = item.Quantity;
            Driver.Price = (decimal)item.Price;
            Driver.StringForPrinting = item.Name;
            if (item.NdsRate == NDS_TYPE_EMPTY_LOC)
                Driver.Tax1 = SI_TAX_ZERO;
            else if (item.NdsRate == NDS_TYPE_20_LOC)
                Driver.Tax1 = SI_TAX_20;
            else if (item.NdsRate == NDS_TYPE_10_LOC)
                Driver.Tax1 = SI_TAX_10;
            else if (item.NdsRate == NDS_TYPE_FREE_LOC)
                Driver.Tax1 = SI_TAX_FREE;
            else if (item.NdsRate == NDS_TYPE_0_LOC)
                Driver.Tax1 = SI_TAX_0;
            else if (item.NdsRate == NDS_TYPE_10110_LOC)
                Driver.Tax1 = SI_TAX_10110;
            else if (item.NdsRate == NDS_TYPE_20120_LOC)
                Driver.Tax1 = SI_TAX_20120;
            else if (item.NdsRate == NDS_TYPE_5_LOC)
                Driver.Tax1 = SI_TAX_5;
            else if (item.NdsRate == NDS_TYPE_7_LOC)
                Driver.Tax1 = SI_TAX_7;
            else if (item.NdsRate == NDS_TYPE_5105_LOC)
                Driver.Tax1 = SI_TAX_5105;
            else if (item.NdsRate == NDS_TYPE_7107_LOC)
                Driver.Tax1 = SI_TAX_7107;

            Driver.Department = 0;
            if (AppSettings.ShtrihRegisterItemMethod==1)
            {
                Driver.Tax2 = 0;
                Driver.Tax3 = 0;
                Driver.Tax4 = 0;
                switch (sign)
                {
                    case FD_CALCULATION_INCOME_LOC:
                        errorCode = Driver.Sale();
                        break;
                    case FD_CALCULATION_BACK_INCOME_LOC:
                        errorCode = Driver.ReturnSale();
                        break;
                    case FD_CALCULATION_EXPENCE_LOC:
                        errorCode = Driver.Buy();
                        break;
                    case FD_CALCULATION_BACK_EXPENCE_LOC:
                        errorCode = Driver.ReturnBuy();
                        break;
                    default:
                        errorCode = 94;
                        break;
                }
            }
            else
            {
                Driver.CheckType = sign;
                Driver.Summ1Enabled = item.Sum > 0;
                Driver.Summ1 = (decimal)item.Sum;
                Driver.PaymentTypeSign = item.PaymentType; //Признак способа расчета для 
                Driver.PaymentItemSign = item.ProductType; //Признак предмета расчета, например "Подакцизный товар"                                     //организация передачи наименования товарной позиции. Мах длинна не более 128 сим.
                Driver.StringForPrinting = item.Name; // данное наименование товара на чеке печатается и передается в ОФД.
                if (item.Unit120 > -1)
                {
                    Driver.MeasureUnit = item.Unit120;
                }
                else { Driver.MeasureUnit = 0; }
                errorCode = Driver.FNOperation();
            }
            if (errorCode != NONE)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(item.Unit105)&&_ffdVer<FR_FFD120)
            {
                Driver.TagNumber = FTAG_ITEM_UNIT_MEASURE_105;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = item.Unit105;
                Driver.FNSendTagOperation();
            }

            if (item.PaymentAgentByProductType > 0)
            {
                Driver.TagNumber = FTAG_ITEM_PAYMENT_AGENT_BY_PRODUCT_TYPE;
                Driver.TagType = SI_ttByte;
                Driver.TagValueInt = item.PaymentAgentByProductType;
                Driver.FNSendTagOperation();
            }
            if (item.IsPaymentAgentData)
            {
                Driver.TagNumber = FTAG_ITEM_PAYMENT_AGENT_DATA;
                Driver.FNBeginSTLVTag();

                int my_TagID = Driver.TagID;
                if (!string.IsNullOrEmpty(item.TransferOperatorPhone))
                {
                    Driver.TagID = my_TagID;
                    Driver.TagNumber = FTAG_PAD_TRANSFER_OPERATOR_PHONE;
                    Driver.TagType = SI_ttString;
                    Driver.TagValueStr = item.TransferOperatorPhone;
                    Driver.FNAddTag();
                }
                if (!string.IsNullOrEmpty(item.PaymentAgentOperation))
                {
                    Driver.TagID = my_TagID;
                    Driver.TagNumber = FTAG_PAD_PAYPENT_AGENT_OPERATION;
                    Driver.TagType = SI_ttString;
                    Driver.TagValueStr = item.PaymentAgentOperation;
                    Driver.FNAddTag();
                }
                if (!string.IsNullOrEmpty(item.PaymentAgentPhone))
                {
                    Driver.TagID = my_TagID;
                    Driver.TagNumber = FTAG_PAD_PAYMENT_AGENT_PHONE;
                    Driver.TagType = SI_ttString;
                    Driver.TagValueStr = item.PaymentAgentPhone;
                    Driver.FNAddTag();
                }
                if (!string.IsNullOrEmpty(item.PaymentOperatorPhone))
                {
                    Driver.TagID = my_TagID;
                    Driver.TagNumber = FTAG_PAD_PAYMENT_OPERATOR_PHONE;
                    Driver.TagType = SI_ttString;
                    Driver.TagValueStr = item.PaymentOperatorPhone;
                    Driver.FNAddTag();
                }
                if (!string.IsNullOrEmpty(item.TransferOperatorName))
                {
                    Driver.TagID = my_TagID;
                    Driver.TagNumber = FTAG_PAD_TRANSFER_OPERATOR_NAME;
                    Driver.TagType = SI_ttString;
                    Driver.TagValueStr = item.TransferOperatorName;
                    Driver.FNAddTag();
                }
                if (!string.IsNullOrEmpty(item.TransferOperatorAddress))
                {
                    Driver.TagID = my_TagID;
                    Driver.TagNumber = FTAG_PAD_TRANSFER_OPERATOR_ADDRESS;
                    Driver.TagType = SI_ttString;
                    Driver.TagValueStr = item.TransferOperatorAddress;
                    Driver.FNAddTag();
                }
                if (!string.IsNullOrEmpty(item.TransferOperatorInn))
                {
                    Driver.TagID = my_TagID;
                    Driver.TagNumber = FTAG_PAD_TRANSFER_OPERATOR_INN;
                    Driver.TagType = SI_ttString;
                    Driver.TagValueStr = item.TransferOperatorInn;
                    Driver.FNAddTag();
                }
                Driver.FNSendSTLVTagOperation();
            }


            if (!string.IsNullOrEmpty(item.ProviderInn))
            {
                Driver.TagNumber = FTAG_ITEM_PROVIDER_INN;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = item.ProviderInn;
                Driver.FNSendTagOperation();
            }
            if (item.IsProviderData)
            {
                Driver.TagNumber = FTAG_ITEM_PROVIDER_DATA;
                Driver.FNBeginSTLVTag();

                int my_TagID = Driver.TagID;
                if (!string.IsNullOrEmpty(item.ProviderPhone))
                {
                    Driver.TagID = my_TagID;
                    Driver.TagNumber = FTAG_PD_PROVIDER_PHONE;
                    Driver.TagType = SI_ttString;
                    Driver.TagValueStr = item.ProviderPhone;
                    Driver.FNAddTag();
                }
                if (!string.IsNullOrEmpty(item.ProviderName))
                {
                    Driver.TagID = my_TagID;
                    Driver.TagNumber = FTAG_PD_PROVIDER_NAME;
                    Driver.TagType = SI_ttString;
                    Driver.TagValueStr = item.ProviderName;
                    Driver.FNAddTag();
                }
                Driver.FNSendSTLVTagOperation();
            }


            if (!string.IsNullOrEmpty(item.CustomEntryNum))
            {
                Driver.TagNumber = FTAG_ITEM_CUSTOM_ENTRY_NUM;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = item.CustomEntryNum;
                Driver.FNSendTagOperation();
            }
            if (!string.IsNullOrEmpty(item.OriginalCountryCode))
            {
                Driver.TagNumber = FTAG_ITEM_ORIGINAL_COUNTRY_CODE;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = item.OriginalCountryCode;
                Driver.FNSendTagOperation();
            }
            if (!string.IsNullOrEmpty(item.Code105))//1162 Код товара
            {
                Driver.MarkingType = 0;
                Driver.BarCode = item.Code105;
                if (Driver.FNSendItemCodeData() != NONE && strictCode1162Work)      // для строгого контроля 1162 установиьт 0
                {
                    RezultMsg("Добавление кода товара FNSendItemCodeData " + Driver.ResultCodeDescription);
                    LogHandle.ol("отменяем чек FNCancelDocument");
                    _CriticalCheqErrorServiceOperations(errorCode);
                    //_PrintDisableFlagRestore();
                    return false;
                }
            }
            
            return true;
        }

        private void _CriticalCheqErrorServiceOperations(int errorCode = NONE)
        {
            RezultMsg("Ошибка " + errorCode + " " + (Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ResultCodeDescription));
            KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = Driver.ResultCodeDescription;
            LogHandle.ol("отменяем чек FNCancelDocument");
            errorCode = Driver.FNCancelDocument();
            LogHandle.ol(Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ErrorDescription);

            LogHandle.ol("отменяем чек SysAdminCancelCheck");
            errorCode = Driver.SysAdminCancelCheck();
            LogHandle.ol(Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ResultCodeDescription);
            
        }

        private bool _PerformCorrectionOptimal(FiscalCheque doc)
        {
            LogHandle.ol("Чек коррекции");
            //_PrintDisableFlagSet();

            if (doc.CalculationSign == 1)
            {
                Driver.CheckType = 0;
            }
            else if (doc.CalculationSign == 2)
            {
                Driver.CheckType = 2;
            }
            else if (doc.CalculationSign == 3)
            {
                Driver.CheckType = 1;
            }
            else if (doc.CalculationSign == 4)
            {
                Driver.CheckType = 3;
            }

            if (_ffdVer <= 105)
            {
                LogHandle.ol("FNBeginCorrectionReceipt");


                if (Driver.FNBeginCorrectionReceipt() != 0)
                {
                    RezultMsg(Driver.ResultCodeDescription);
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = Driver.ResultCodeDescription;
                    //_PrintDisableFlagRestore();
                    return false;
                }
            }
            else
            {
                LogHandle.ol("FNOpenCheckCorrection");
                

                if (Driver.FNOpenCheckCorrection() != 0)
                {
                    RezultMsg(Driver.ResultCodeDescription);
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = Driver.ResultCodeDescription;
                    //_PrintDisableFlagRestore();
                    return false;
                }
            }
            int errorCode = NONE;
            if (AppSettings.CorrectionOrderExistance == 1 && doc.CorrectionTypeFtag == 1
                            || AppSettings.CorrectionOrderExistance == 0 )
            {
                Driver.TagNumber = FTAG_CORRECTION_ORDER_NUMBER;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = doc.CorrectionOrderNumber;
                Driver.FNSendTag();

            }
           

            Driver.TagNumber = FTAG_CORRECTION_DOC_DATE;
            Driver.TagType = SI_ttUnixTime;
            Driver.TagValueDateTime = doc.CorrectionDocumentDate;
            Driver.FNSendTag();
            
            Driver.CalculationSign = doc.CalculationSign;
            if (_ffdVer <= 105 /*&& _useCorrDescriber*/)
            {
                //Driver.CorrectionType = doc.CorrectionTypeNotFtag;
                //Driver.CalculationSign = doc.CalculationSign;
                //Driver.TagNumber = FTAG_CORRECTION_DESCRIBER;
                //Driver.TagType = SI_ttString;
                //Driver.TagValueStr = doc.CorrectionDocDescriber;
                //Driver.FNSendTag();
            }
            else
            {
                Driver.TagNumber = FTAG_CORRECTION_TYPE;
                Driver.TagType = SI_ttByte;
                Driver.TagValueInt = doc.CorrectionTypeFtag;
                Driver.FNSendTag();
            }

            //1021 Кассир
            if (!string.IsNullOrEmpty(doc.Cashier) && doc.Cashier != DEFAULT_CASHIER)
            {
                LogHandle.ol("Добавляем кассира " + doc.Cashier);
                Driver.TagNumber = FTAG_CASHIER_NAME;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = doc.Cashier;
                Driver.FNSendTag();
                //1203 ИНН кассира
                if (!string.IsNullOrEmpty(doc.CashierInn))
                {
                    LogHandle.ol("Добавляем ИНН кассира " + doc.Cashier);
                    Driver.TagNumber = FTAG_CASHIER_INN;
                    Driver.TagType = SI_ttString;
                    Driver.TagValueStr = doc.CashierInn;
                    Driver.FNSendTag();
                }
            }

            if (doc.BuyerInformation)
            {
                if (_ffdVer < FR_FFD110)
                {
                    //1227 Покупатель (клиент)
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyer))
                    {
                        Driver.TagType = SI_ttString;
                        Driver.TagNumber = FTAG_BUYER_INFORMATION_BUYER;
                        Driver.TagValueStr = doc.BuyerInformationBuyer;
                        LogHandle.ol("Добавляем наименование пользователя (режим 1.05)" + doc.BuyerInformationBuyer + "\tответ драйвера:  " + Driver.FNSendTag());
                    }
                    //1228 ИНН покупателя (клиента)
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerInn))
                    {
                        Driver.TagNumber = FTAG_BUYER_INFORMATION_BUYER_INN;
                        Driver.TagType = SI_ttString;
                        Driver.TagValueStr = doc.BuyerInformationBuyerInn;
                        LogHandle.ol("Добавляем ИНН покупателя (режим 1.05)" + doc.BuyerInformationBuyerInn + "\tответ драйвера:  " + Driver.FNSendTag());
                    }
                }
                else
                {
                    Driver.TagNumber = FTAG_BUYER_INFORMATION;
                    Driver.FNBeginSTLVTag();
                    int my_TagID = Driver.TagID;
                    bool buyerInn = (!string.IsNullOrEmpty(doc.BuyerInformationBuyerInn) && CorrectInn(doc.BuyerInformationBuyerInn));
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyer))
                    {
                        Driver.TagID = my_TagID;
                        Driver.TagNumber = FTAG_BUYER_INFORMATION_BUYER;
                        Driver.TagType = SI_ttString;
                        Driver.TagValueStr = doc.BuyerInformationBuyer;
                        LogHandle.ol("Добавляем наименование пользователя (режим 1.2)" + doc.BuyerInformationBuyer + "\tответ драйвера:  " + Driver.FNAddTag());

                    }
                    if (buyerInn)
                    {
                        Driver.TagID = my_TagID;
                        Driver.TagNumber = FTAG_BUYER_INFORMATION_BUYER_INN;
                        Driver.TagType = SI_ttString;
                        Driver.TagValueStr = doc.BuyerInformationBuyerInn;
                        LogHandle.ol("Добавляем ИНН покупателя (режим 1.2)" + doc.BuyerInformationBuyerInn + "\tответ драйвера:  " + Driver.FNAddTag());
                    }
                    if(AppSettings.AlwaysSendBuyerDocData || (!buyerInn))
                    {
                        if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerCitizenship))
                        {
                            Driver.TagID = my_TagID;
                            Driver.TagNumber = FTAG_BI_CITIZENSHIP;
                            Driver.TagType = SI_ttString;
                            Driver.TagValueStr = doc.BuyerInformationBuyerCitizenship;
                            LogHandle.ol("Добавляем гражданство покупателя " + doc.BuyerInformationBuyerCitizenship + "\tответ драйвера:  " + Driver.FNAddTag());
                        }
                        if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerDocumentCode))
                        {
                            Driver.TagID = my_TagID;
                            Driver.TagNumber = FTAG_BI_DOCUMENT_CODE;
                            Driver.TagType = SI_ttString;
                            Driver.TagValueStr = doc.BuyerInformationBuyerDocumentCode;
                            LogHandle.ol("Добавляем код документа покупателя " + doc.BuyerInformationBuyerInn + "\tответ драйвера:  " + Driver.FNAddTag());
                        }
                        if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerDocumentData))
                        {
                            Driver.TagID = my_TagID;
                            Driver.TagNumber = FTAG_BI_DOCUMENT_DATA;
                            Driver.TagType = SI_ttString;
                            Driver.TagValueStr = doc.BuyerInformationBuyerDocumentData;
                            LogHandle.ol("Добавляем данные документа покупателя " + doc.BuyerInformationBuyerDocumentData + "\tответ драйвера:  " + Driver.FNAddTag());
                        }
                        if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerBirthday))
                        {
                            Driver.TagID = my_TagID;
                            Driver.TagNumber = FTAG_BI_BIRTHDAY;
                            Driver.TagType = SI_ttString;
                            Driver.TagValueStr = doc.BuyerInformationBuyerBirthday;
                            LogHandle.ol("Добавляем ДР покупателя " + doc.BuyerInformationBuyerBirthday + "\tответ драйвера:  " + Driver.FNAddTag());
                        }
                    }
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerAddress))
                    {
                        Driver.TagID = my_TagID;
                        Driver.TagNumber = FTAG_BI_ADDRESS;
                        Driver.TagType = SI_ttString;
                        Driver.TagValueStr = doc.BuyerInformationBuyerAddress;
                        LogHandle.ol("Добавляем адрес покупателя " + doc.BuyerInformationBuyerAddress + "\tответ драйвера:  " + Driver.FNAddTag());
                    }
                    LogHandle.ol(Driver.FNSendSTLVTag().ToString());
                }
            }
            if (AppSettings.OverideRetailAddress && !string.IsNullOrEmpty(doc.RetailAddress))
            {
                LogHandle.ol("Перезапись адреса места установки " + doc.RetailAddress);
                Driver.TagNumber = FTAG_RETAIL_PLACE_ADRRESS;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = doc.RetailAddress;
                Driver.FNSendTag();
            }
            if (AppSettings.OverideRetailPlace && !string.IsNullOrEmpty(doc.RetailPlace))
            {
                LogHandle.ol("Перезапись места установки " + doc.RetailPlace);
                Driver.TagNumber = FTAG_RETAIL_PLACE;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = doc.RetailPlace;
                Driver.FNSendTag();
            }
            //1008 Телефон или электронный адрес покупателя
            if (!string.IsNullOrEmpty(doc.EmailPhone))
            {
                LogHandle.ol("Добавляем электронный адрес " + doc.EmailPhone);
                Driver.TagType = SI_ttString;
                Driver.TagNumber = FTAG_DESTINATION_EMAIL;
                Driver.TagValueStr = doc.EmailPhone;
                Driver.FNSendTag();
            }
            if (doc.InternetPayment)
            {
                LogHandle.ol("Добавляем признак расчета в интернет ");
                Driver.TagType = SI_ttByte;
                Driver.TagNumber = FTAG_INTERNET_PAYMENT;
                Driver.TagValueInt = 1;
                Driver.FNSendTag();
            }

            if (doc.IsPropertiesData)
            {
                Driver.TagNumber = FTAG_PROPERTIES_DATA;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = doc.PropertiesData;
                Driver.FNSendTag();
            }
            if (doc.IsProperties1084)
            {
                Driver.TagNumber = FTAG_PRORERTIES_1084;
                LogHandle.ol("Формируем 1084 "+Driver.FNBeginSTLVTag());
                int my_TagID = Driver.TagID;

                Driver.TagID = my_TagID;
                Driver.TagNumber = FTAG_PROPERTIES_PROPERTY_NAME;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = doc.PropertiesPropertyName;
                LogHandle.ol("Добавляем наименование доп. рекв. пользователя " + doc.PropertiesPropertyName + "\tответ драйвера:  " + Driver.FNAddTag());

                Driver.TagID = my_TagID;
                Driver.TagNumber = FTAG_PROPERTIES_PROPERTY_VALUE;
                Driver.TagType = SI_ttString;
                Driver.TagValueStr = doc.PropertiesPropertyValue;
                LogHandle.ol("Добавляем наименование доп. рекв. пользователя " + doc.PropertiesPropertyValue + "\tответ драйвера:  " + Driver.FNAddTag());
                LogHandle.ol(Driver.FNSendSTLVTag().ToString());
            }
            double itemsSumm = 0;
            foreach (ConsumptionItem item in doc.Items)
            {
                itemsSumm += item.Sum;
                if (_ffdVer <= FR_FFD105) continue;
                if (!_RegisterItem(item, doc.CalculationSign, ref errorCode))
                {
                    //_CriticalCheqErrorServiceOperations();
                    _CriticalCheqErrorServiceOperations(errorCode);
                    //_PrintDisableFlagRestore();
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(doc.BuyerInformationBuyer) || !string.IsNullOrEmpty(doc.BuyerInformationBuyerInn))
            {
                if (_ffdVer <= FR_FFD110)
                {
                    //1227 Покупатель (клиент)
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyer))
                    {
                        Driver.TagType = SI_ttString;
                        Driver.TagNumber = FTAG_BUYER_INFORMATION_BUYER;
                        Driver.TagValueStr = doc.BuyerInformationBuyer;
                        LogHandle.ol("Добавляем наименование пользователя (режим 1.05)" + doc.BuyerInformationBuyer + "\tответ драйвера:  " + Driver.FNSendTag());
                    }
                    //1228 ИНН покупателя (клиента)
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerInn))
                    {
                        Driver.TagNumber = FTAG_BUYER_INFORMATION_BUYER_INN;
                        Driver.TagType = SI_ttString;
                        Driver.TagValueStr = doc.BuyerInformationBuyerInn;
                        LogHandle.ol("Добавляем ИНН покупателя (режим 1.05)" + doc.BuyerInformationBuyerInn + "\tответ драйвера:  " + Driver.FNSendTag());
                    }
                }
                else
                {
                    Driver.TagNumber = FTAG_BUYER_INFORMATION;
                    Driver.FNBeginSTLVTag();
                    int my_TagID = Driver.TagID;
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyer))
                    {
                        Driver.TagID = my_TagID;
                        Driver.TagNumber = FTAG_BUYER_INFORMATION_BUYER;
                        Driver.TagType = SI_ttString;
                        Driver.TagValueStr = doc.BuyerInformationBuyer;
                        LogHandle.ol("Добавляем наименование пользователя (режим 1.2)" + doc.BuyerInformationBuyer + "\tответ драйвера:  " + Driver.FNAddTag());

                    }
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerInn))
                    {
                        Driver.TagID = my_TagID;
                        Driver.TagNumber = FTAG_BUYER_INFORMATION_BUYER_INN;
                        Driver.TagType = SI_ttString;
                        Driver.TagValueStr = doc.BuyerInformationBuyerInn;
                        LogHandle.ol("Добавляем ИНН покупателя (режим 1.2)" + doc.BuyerInformationBuyerInn + "\tответ драйвера:  " + Driver.FNAddTag());

                    }
                    Driver.FNSendSTLVTag();
                }
            }

            if (_ffdVer <= 105)
            {
                Driver.CorrectionType = doc.CorrectionTypeNotFtag;
                Driver.CalculationSign = doc.CalculationSign;
                LogHandle.ol("Регистрируем суммы");
                Driver.Summ1 = (decimal)doc.TotalSum;
                Driver.Summ2 = (decimal)doc.Cash;
                Driver.Summ3 = (decimal)doc.ECash;
                Driver.Summ4 = (decimal)doc.Prepaid;
                Driver.Summ5 = (decimal)doc.Credit;
                Driver.Summ6 = (decimal)doc.Provision;
                Driver.Summ7 = (decimal)doc.Nds20;
                Driver.Summ8 = (decimal)doc.Nds10;
                Driver.Summ9 = (decimal)doc.Nds0;
                Driver.Summ10 = (decimal)doc.NdsFree;
                Driver.Summ11 = (decimal)doc.Nds20120;
                Driver.Summ12 = (decimal)doc.Nds10110;
                if (AppSettings.UsingCustomSno)
                    Driver.TaxType = doc.Sno;
                if (AppSettings.ShtrihCloseCheckMethod == 2 /*|| AppSettings.ShtrihCloseCheckMethod ==0*/) // ShtrihCloseCheckMethod == 0 запрещен
                {
                    if (_oldDriver && !AppSettings.ShtrihIgnoreOldDriver)
                    {
                        _CriticalCheqErrorServiceOperations(SinExep_DRIVER_NEED_TO_UPDATE);    // синтетическая ошибка, отсутвие полей библиотеки перехватить try/catch не получается
                        //_PrintDisableFlagRestore();
                        return false;
                    }

                    Driver.Summ13 = (decimal)doc.Nds5;
                    Driver.Summ14 = (decimal)doc.Nds7;
                    Driver.Summ15 = (decimal)doc.Nds5105;
                    Driver.Summ16 = (decimal)doc.Nds7107;
                    if (AppSettings.UsingCustomSno)
                        Driver.TaxType = doc.Sno;
                    LogHandle.ol("FNBuildCorrectionReceipt3");
                    if (Driver.FNBuildCorrectionReceipt3() != 0)
                    {
                        errorCode = NONE;
                        RezultMsg(Driver.ResultCodeDescription);
                        LogHandle.ol("отменяем чек FNCancelDocument");
                        errorCode = Driver.FNCancelDocument();
                        LogHandle.ol(Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ErrorDescription);
                        LogHandle.ol("отменяем чек SysAdminCancelCheck");
                        errorCode = Driver.SysAdminCancelCheck();
                        LogHandle.ol(Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ResultCodeDescription);
                        return false;
                    }
                }
                else if(AppSettings.ShtrihCloseCheckMethod == 1)
                {
                    LogHandle.ol("FNBuildCorrectionReceipt2");
                    bool existsNewTaxes = doc.Nds5 + doc.Nds5105 + doc.Nds7 + doc.Nds7107 > 0.009;
                    if (
                        (existsNewTaxes /*&& _taxesFillFirmware == 1*/ && !AppSettings.ShtrihIgnoreOldDriver) // новые ставки НДС/// *НДС считает верхнее ПО* --- настройка _taxesFillFirmware == 1 не поможет при отсутствии предметов расчета
                        || (existsNewTaxes && (_oldDriver && !AppSettings.ShtrihIgnoreOldDriver)) // новые ставки не пропустит старый драйвер и не игнорировать старый драйвер 
                        )
                    {
                        _CriticalCheqErrorServiceOperations(SinExep_BAD_SETTING_FOR_CORRECT_FD);    // синтетическая ошибка, используются новые НДС команда закрытия чека без новых ставок и отключен подсчет налогов прошивкой
                        //_PrintDisableFlagRestore();
                        return false;
                    }
                    if (existsNewTaxes && firmwareBuild.Year < 2025 && !AppSettings.ShtrihIgnoreOldDriver)
                    {
                        _CriticalCheqErrorServiceOperations(SinExep_NOT_COMPATIBLE_FIRMWARE);    // Прошивка не подходит для новых ставок
                        //_PrintDisableFlagRestore();
                        return false;
                    }

                    if (Driver.FNBuildCorrectionReceipt2() != 0)
                    {
                        errorCode = NONE;
                        RezultMsg(Driver.ResultCodeDescription);
                        LogHandle.ol("отменяем чек FNCancelDocument");
                        errorCode = Driver.FNCancelDocument();
                        LogHandle.ol(Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ErrorDescription);
                        LogHandle.ol("отменяем чек SysAdminCancelCheck");
                        errorCode = Driver.SysAdminCancelCheck();
                        LogHandle.ol(Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ResultCodeDescription);
                        return false;
                    }

                }

            }
            else
            {
                if(!_CloseCheckCM(doc,itemsSumm,ref errorCode))
                {
                    return false;
                }

            }

            if (Driver.WaitForPrinting() != 0)
            {
                RezultMsg("Документ проведен с ошибкой при печати");
                return false;
            }
            RezultMsg(SUCCESS_MSG);
            return true;
        }


        bool _CloseCheckCM(FiscalCheque doc,double itemsSumm, ref int errorCode)
        {
            if (AppSettings.UsingCustomSno)
            {
                Driver.TaxType = doc.Sno;
            }
            errorCode = NONE;
            if (AppSettings.UsingCustomSno)
            {
                Driver.TaxType = doc.Sno;
            }
            //Driver.CheckSubTotal();
            LogHandle.ol("добавляем суммы чека");

            Driver.RoundingSumm = (int)Math.Round((itemsSumm - doc.TotalSum) * 100);
            if (itemsSumm - doc.TotalSum > 1)
            {
                LogHandle.ol("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                LogHandle.ol("!!!!!!!!!!!Округление на чек больше 1р!!!!!!!!!!!!!!!!!!!!!!");
                LogHandle.ol("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                // при закрытии получим ошибку
            }
            //Driver.DiscountOnCheck = itemsSumm - doc.TotalSum;
            Driver.Summ1 = (decimal)doc.Cash;
            Driver.Summ2 = (decimal)doc.ECash;
            Driver.Summ3 = 0;
            Driver.Summ4 = 0;
            Driver.Summ5 = 0;
            Driver.Summ6 = 0;
            Driver.Summ7 = 0;
            Driver.Summ8 = 0;
            Driver.Summ9 = 0;
            Driver.Summ10 = 0;
            Driver.Summ11 = 0;
            Driver.Summ12 = 0;
            Driver.Summ13 = 0;
            Driver.Summ14 = (decimal)doc.Prepaid;
            Driver.Summ15 = (decimal)doc.Credit;
            Driver.Summ16 = (decimal)doc.Provision;
            Driver.TaxValue1 = (decimal)doc.Nds20;
            Driver.TaxValue2 = (decimal)doc.Nds10;
            Driver.TaxValue3 = (decimal)doc.Nds0;
            Driver.TaxValue5 = (decimal)doc.Nds20120;
            Driver.TaxValue6 = (decimal)doc.Nds10110;
            Driver.TaxValue4 = (decimal)doc.NdsFree;
            if (AppSettings.ShtrihCloseCheckMethod == 2)
            {
                if (_oldDriver && !AppSettings.ShtrihIgnoreOldDriver)
                {
                    _CriticalCheqErrorServiceOperations(SinExep_DRIVER_NEED_TO_UPDATE);    // синтетическая ошибка, перехватить try/catch не получается
                    //_PrintDisableFlagRestore();
                    return false;
                }
                Driver.TaxValue7 = (decimal)doc.Nds5;
                Driver.TaxValue8 = (decimal)doc.Nds7;
                Driver.TaxValue9 = (decimal)doc.Nds5105;
                Driver.TaxValue10 = (decimal)doc.Nds7107;
                LogHandle.ol("Закрываем чек FNCloseCheckEx");
                errorCode = Driver.FNCloseCheckEx3();
            }
            else if (AppSettings.ShtrihCloseCheckMethod == 1)
            {
                bool existsNewTaxes = doc.Nds5 + doc.Nds5105 + doc.Nds7 + doc.Nds7107 > 0.009;
                if (
                    (existsNewTaxes && _taxesFillFirmware == 1 && !AppSettings.ShtrihIgnoreOldDriver) // новые ставки НДС и НДС считает верхнее ПО
                    || (existsNewTaxes && (_oldDriver && !AppSettings.ShtrihIgnoreOldDriver)) // новые ставки не пропустит старый драйвер и не игнорировать старый драйвер 
                    )
                {
                    _CriticalCheqErrorServiceOperations(SinExep_BAD_SETTING_FOR_CORRECT_FD);    // синтетическая ошибка, используются новые НДС команда закрытия чека без новых ставок и отключен подсчет налогов прошивкой
                    //_PrintDisableFlagRestore();
                    return false;
                }
                if (existsNewTaxes && firmwareBuild.Year < 2025 && !AppSettings.ShtrihIgnoreOldDriver)
                {
                    _CriticalCheqErrorServiceOperations(SinExep_NOT_COMPATIBLE_FIRMWARE);    // Прошивка не подходит для новых ставок
                    //_PrintDisableFlagRestore();
                    return false;
                }
                LogHandle.ol("Закрываем чек FNCloseCheckEx");
                errorCode = Driver.FNCloseCheckEx();
            }
            if (errorCode != 0)
            {
                _CriticalCheqErrorServiceOperations(errorCode);
                //_PrintDisableFlagRestore();
                return false;
            }
            return true;
        }



        public override FnReadedDocument ReadFD(int docNumber,bool parce = false)
        {
            if (Driver == null)
            {
                RezultMsg(NO_DRIVER_FOUNDED);
                return FnReadedDocument.EmptyFD;
            }
            if (!_connected)
            {
                RezultMsg(CONNECTION_NOT_ESTABLISHED);
                return FnReadedDocument.EmptyFD;
            }
            Driver.DocumentNumber = docNumber;
            int request = Driver.FNFindDocument();
            //LogHandle.ol(Driver.StringForPrinting);
            if (request !=0)
            {
                LogHandle.ol("Ошибка при чтении документа "+docNumber + " "+Driver.ResultCodeDescription);
                return FnReadedDocument.EmptyFD;
            }
            int type = Driver.DocumentType;

            DateTime time = DateTime.Now;

            FnReadedDocument frd = new FnReadedDocument(type, Driver.Date.AddHours(Driver.Time.Hour).AddMinutes(Driver.Time.Minute), docNumber, (double)Driver.Summ1, Driver.FiscalSignAsString);
            if(parce && (type==FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE||type==FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE || type == FTAG_FISCAL_DOCUMENT_TYPE_BSO || type == FTAG_FISCAL_DOCUMENT_TYPE_BSO_CORRECTION))
            {
                int effort = 0;
            AttempToRead:
                effort++;
                Driver.Password = 30;
                Driver.DocumentNumber = docNumber;
                Driver.FNRequestFiscalDocumentTLV();
                int dType = Driver.DocumentType;
                if (dType == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE) dType = FD_DOCUMENT_NAME_CHEQUE;
                else if (dType == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE) dType = FD_DOCUMENT_NAME_CORRECTION_CHEQUE;

                int lnth = Driver.DataLength;
                Driver.Password = 30;
                string tlvImage = "";
                StringBuilder sbtlvImage = new StringBuilder();
                
                while (Driver.FNReadFiscalDocumentTLV() == 0)
                {
                    string s = Driver.TLVData;
                    sbtlvImage.Append(s);
                }
                tlvImage = sbtlvImage.ToString();
                FnReadedDocument frd2 = TranslateFtagsList(Encoding.GetEncoding(1251).GetBytes(tlvImage), dType);
                if (/*false/*отладка* / && */ CorrectnessOfReading(frd2.Cheque)&&frd2.Time > new DateTime(2017,6,1)) 
                {
                    
                    //LogHandle.ol("Прочитан документ:" + Encoding.UTF8.GetBytes(tlvImage));
                    LogHandle.ol("Прочитан документ" + Environment.NewLine + BitConverter.ToString(Encoding.GetEncoding(1251).GetBytes(tlvImage)) +Environment.NewLine);

                    
                    uint ut=0;
                    if(!uint.TryParse(frd2.FiscalSign,out ut)||ut==0)
                        frd2.FiscalSign = frd.FiscalSign;  // !!!!! заплатка переделать парсинг ТЛВ структуры - пофиксено оставлено при условии некоррекного ФП

                    frd2.RebuildPrezentation();
                    // данный блок реализован в вызываемом методе TranslateFtagsList
                    /*if(frd2.Cheque != null)
                    {
                        if (AppSettings.AppendFiscalSignAsPropertyData *//*&& frd2.Cheque != null /*&& !string.IsNullOrEmpty(frd2.FiscalSign) && string.IsNullOrEmpty(frd2.Cheque.PropertiesData)*//*)
                        {
                            if (AppSettings.OverridePropertyData)
                                frd2.Cheque.PropertiesData = frd2.FiscalSign;
                            else
                            {
                                if (!frd2.Cheque.IsPropertiesData)
                                    frd2.Cheque.PropertiesData = frd2.FiscalSign;
                            }
                        }
                        if (AppSettings.OverrideCorrectionOrderNumber)
                        {
                            frd2.Cheque.CorrectionOrderNumber = AppSettings.CorrectionOrderNumberDefault;
                        }
                        if (AppSettings.OverrideCorrectionDocumentDate)
                        {
                            frd2.Cheque.CorrectionDocumentDate = frd2.Time;
                        }
                    }*/

                    return frd2;
                }
                LogHandle.ol("Неудачный разбор TLV документа"+Environment.NewLine+tlvImage+Environment.NewLine+"Разбираем строковое представление");

                Driver.DocumentNumber = docNumber;
                Driver.ShowTagNumber = true;
                Driver.RequestDocumentType = 0;
                if (Driver.FNGetDocumentAsString() == 0)
                {
                    string docAsStr = "КАССОВЫЙ ЧЕК";//Driver.StringForPrinting;
                    if(string.IsNullOrEmpty(docAsStr))
                    {
                        LogHandle.ol("Ошибка при чтении(пустая строка)");
                    }
                    else
                    {
                        //char[] delimiterChars = { '\n', '\r' };
                        List<string> lines = new List<string>();//docAsStr.Split(delimiterChars).ToList();
                        lines.Add("КАССОВЫЙ ЧЕК");
                        for (int i = 0; i < lines.Count; i++)
                            if (lines[i].Trim().Length == 0) lines.RemoveAt(i--);
                        
                        FiscalCheque recognizedFd = new FiscalCheque();
                        string fdName = lines[0].ToUpper();
                        if (fdName.Equals("КАССОВЫЙ ЧЕК"))
                            recognizedFd.Document = FD_DOCUMENT_NAME_CHEQUE;
                        else if (fdName.Equals("КАССОВЫЙ ЧЕК КОРРЕКЦИИ"))
                            recognizedFd.Document = FD_DOCUMENT_NAME_CORRECTION_CHEQUE;
                        else
                        {
                            LogHandle.ol("Неизвестное начало документа обработка прекращена"+ Environment.NewLine + fdName);
                            recognizedFd = null;
                        }
                        List < FTag > ftagsStrRepr = new List<FTag>();
                        if(recognizedFd != null)
                        {
                            int docLines = lines.Count;
                            try
                            {
                                List<FTag> item = new List<FTag>();
                                recognizedFd = null;
                                // !!!!! этот блок отключен!!!!!
                                for (int i = 2; i < docLines; i++)
                                {
                                   
                                    string line = lines[i];
                                    if (line.StartsWith("1008"))
                                    {
                                        recognizedFd.EmailPhone = line.Substring(line.IndexOf(':') + 1);
                                        ftagsStrRepr.Add(new FTag(1008, recognizedFd.EmailPhone,false));
                                    }         // телефон или эл почта
                                    else if (line.StartsWith("1020"))
                                    {
                                        recognizedFd.TotalSum = double.Parse(ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                        ftagsStrRepr.Add(new FTag(1020, (int)Math.Round(100 * recognizedFd.TotalSum),false));
                                    }    // итог чека
                                    else if (line.StartsWith("1021"))
                                    {
                                        recognizedFd.Cashier = line.Substring(line.IndexOf(":") + 1);
                                        ftagsStrRepr.Add(new FTag(1021, recognizedFd.Cashier, false));
                                    }   // кассир
                                    else if (line.StartsWith("1192"))
                                    {   
                                        recognizedFd.PropertiesData = line.Substring(line.IndexOf(":") + 1);
                                        ftagsStrRepr.Add(new FTag(1192, recognizedFd.PropertiesData, false));
                                    }   // доп реквизит чека
                                    else if (line.StartsWith("1031"))
                                    {
                                        recognizedFd.Cash = double.Parse(ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                        ftagsStrRepr.Add(new FTag(1031, (int)Math.Round(100 * recognizedFd.Cash), false));
                                    }    // оплачено наличными
                                    else if (line.StartsWith("1054"))
                                    {
                                        recognizedFd.CalculationSign = line[line.IndexOf(':') + 1] - '0';
                                        ftagsStrRepr.Add(new FTag(1054, recognizedFd.CalculationSign, false));
                                    }    // признак расчета
                                    else if (line.StartsWith("1055"))
                                    {
                                        int s = line.IndexOf(':') + 1;
                                        int sno = line[s] - '0';
                                        if (sno == 1 && line[s + 1] == '6') sno = 16;
                                        else if (sno == 3 && line[s + 1] == '2') sno = 32;
                                        recognizedFd.Sno = sno;
                                        ftagsStrRepr.Add(new FTag(1055, recognizedFd.Sno, false));
                                    }    // СНО:4 (УДМР)
                                    else if (line.StartsWith("1059"))
                                    {
                                        
                                        ++i;
                                        double quantity = 0, price = 0, sum = 0, unitNds = 0;
                                        int ndsRate = 0, productType = 0, paymentType = 0, unitMeasure120 = -1;
                                        string name = " ", code = "", unitMeasure = "", providerInn = "", customEntryNum = "", originalCountryCode = "";
                                        do                              // разбираем предметы расчета 
                                        {
                                            line = lines[i];
                                            LogHandle.ol("Item: " + line);
                                            if (line.StartsWith(" 1023"))
                                            {
                                                quantity = double.Parse(ReplaceBadDecimalSeparatorPoint( line.Substring(line.IndexOf(":") + 1)));
                                                item.Add(new FTag(1023, quantity, false));
                                            }   // количество предмета расчета
                                            else if(line.StartsWith(" 1030"))
                                            {
                                                name = line.Substring(line.IndexOf(":") + 1);
                                                item.Add(new FTag(1030,name,false));
                                            }   // Наименование предмета расчета
                                            else if(line.StartsWith(" 1043"))
                                            {
                                                sum = double.Parse( ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                                item.Add(new FTag(1043, (int)Math.Round(100 * sum), false));
                                            }   // Стоимость предмета расчета с учетом скидок и наценок
                                            else if(line.StartsWith("1059"))
                                            {
                                                
                                                ConsumptionItem item1 = new ConsumptionItem(name,price,quantity,sum,productType,paymentType,ndsRate);
                                                if (!string.IsNullOrEmpty(code))
                                                    item1.Code105 = code;
                                                if (!string.IsNullOrEmpty(unitMeasure))
                                                    item1.Unit105 = unitMeasure;
                                                if (!string.IsNullOrEmpty(providerInn))
                                                    item1.ProviderInn = providerInn;
                                                if (!string.IsNullOrEmpty(customEntryNum))
                                                    item1.CustomEntryNum = customEntryNum;
                                                if (!string.IsNullOrEmpty(originalCountryCode))
                                                    item1.OriginalCountryCode = originalCountryCode;
                                                if(unitMeasure120 > -1)
                                                    item1.Unit120 = unitMeasure120;
                                                if (item1.Correctness != FD_ITEM_CONTROL_CRITICAL_ERROR)
                                                    recognizedFd.Items.Add(item1);
                                                else
                                                {
                                                    LogHandle.ol("Некорректно считан предмет расчета " + item1);
                                                    recognizedFd = null;
                                                    break;
                                                }
                                                ftagsStrRepr.Add(new FTag(1059, item, false));
                                                item = new List<FTag>();
                                                unitMeasure120 = -1;
                                                quantity = 0;
                                                price = 0; 
                                                sum = 0; 
                                                unitNds = 0;
                                                ndsRate = 0; 
                                                productType = 0; 
                                                paymentType = 0;
                                                name = " "; 
                                                code = "";
                                                unitMeasure = "";
                                                providerInn = "";
                                                customEntryNum = "";
                                                originalCountryCode = "";
                                            }   // Предмет расчета
                                            else if(line.StartsWith(" 1079"))
                                            {
                                                price = double.Parse(ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                                item.Add(new FTag(1079, (int)Math.Round(100 * price), false));
                                            }   // Цена за единицу предмета расчета с учетом скидок и наценок
                                            else if (line.StartsWith(" 1162"))
                                            {
                                                // тут необходима доработка text.hex->string | rawdata
                                                // code = line.Substring(line.IndexOf(":") + 1);
                                            }   // Код товара
                                            else if(line.StartsWith(" 1197"))
                                            {
                                                unitMeasure = line.Substring(line.IndexOf(":") + 1);
                                                item.Add(new FTag(1197, unitMeasure, false));
                                            }   // Единица измерения предмета расчета
                                            else if(line.StartsWith(" 1198"))
                                            {
                                                unitNds = double.Parse(ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                                item.Add(new FTag(1198, (int)Math.Round(100 * unitNds), false));
                                            }   //  Размер НДС за единицу предмета расчета
                                            else if(line.StartsWith(" 1199"))
                                            {
                                                ndsRate = int.Parse(line.Substring(line.IndexOf(":") + 1));
                                                item.Add(new FTag(1199, ndsRate, false));
                                            }   // СТАВКА НДС:6
                                            else if(line.StartsWith(" 1212"))
                                            {
                                                productType = int.Parse(line.Substring(line.IndexOf(":") + 1));
                                                item.Add(new FTag(1212, productType, false));
                                            }   // Признак предмета расчета
                                            else if(line.StartsWith(" 1214"))
                                            {
                                                paymentType = int.Parse(line.Substring(line.IndexOf(":") + 1));
                                                item.Add(new FTag(1214, paymentType, false));
                                            }   // ПРИЗН. СПОСОБА РАСЧ.
                                            else if(line.StartsWith(" 1226"))
                                            {
                                                providerInn = line.Substring(line.IndexOf(":") + 1);
                                                item.Add(new FTag(1226, providerInn, false));
                                            }   // ИНН поставщика
                                            else if(line.StartsWith(" 1231"))
                                            {
                                                customEntryNum = line.Substring(line.IndexOf(":") + 1);
                                                item.Add(new FTag(1231, customEntryNum, false));
                                            }   // Номер таможенной декларации
                                            else if(line.StartsWith(" 2108"))
                                            {
                                                unitMeasure120 = int.Parse(line.Substring(line.IndexOf(":") + 1));
                                                item.Add(new FTag(2108, unitMeasure120, false));
                                            }

                                        } while (++i < docLines && (line[0] == ' ' || line.StartsWith("1059")  ));
                                        i-=2;
                                        if (quantity > 0)
                                        {
                                            if(item.Count>0)
                                                ftagsStrRepr.Add(new FTag(1059, item, false));

                                            ConsumptionItem item0 = new ConsumptionItem(name, price, quantity, sum, productType, paymentType, ndsRate);
                                            if (!string.IsNullOrEmpty(code))
                                                item0.Code105 = code;
                                            if (!string.IsNullOrEmpty(unitMeasure))
                                                item0.Unit105 = unitMeasure;
                                            if (!string.IsNullOrEmpty(providerInn))
                                                item0.ProviderInn = providerInn;
                                            if (!string.IsNullOrEmpty(customEntryNum))
                                                item0.CustomEntryNum = customEntryNum;
                                            if (!string.IsNullOrEmpty(originalCountryCode))
                                                item0.OriginalCountryCode = originalCountryCode;

                                            if (item0.Correctness != FD_ITEM_CONTROL_CRITICAL_ERROR)
                                                recognizedFd.Items.Add(item0);
                                            else
                                            {
                                                LogHandle.ol("Ошибка в последнем предмете расчета " + item0);
                                            }
                                        }
                                        
                                    }    // ПРЕДМЕТ РАСЧЕТА
                                    else if (line.StartsWith("1081"))
                                    {
                                        recognizedFd.ECash = double.Parse(ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                        ftagsStrRepr.Add(new FTag(1081, (int)Math.Round(100 * recognizedFd.ECash),false));
                                    }    // безналичными
                                    else if (line.StartsWith("1102"))
                                    {
                                        recognizedFd.Nds20 = double.Parse(ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                        ftagsStrRepr.Add(new FTag(1102, (int)Math.Round(100 * recognizedFd.Nds20), false));
                                    }    // СУММА НДС 20%
                                    else if (line.StartsWith("1103"))
                                    {
                                        recognizedFd.Nds10 = double.Parse(ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                        ftagsStrRepr.Add(new FTag(1103, Math.Round(100 * recognizedFd.Nds10), false));
                                    }    // СУММА НДС 10%
                                    else if (line.StartsWith("1104"))
                                    {
                                        recognizedFd.Nds0 = double.Parse(ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                        ftagsStrRepr.Add(new FTag(1104, (int)Math.Round(100 * recognizedFd.Nds0), false));
                                    }    // СУММА С НДС 0%
                                    else if (line.StartsWith("1105"))
                                    {
                                        recognizedFd.NdsFree = double.Parse(ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                        ftagsStrRepr.Add(new FTag(1105, (int)Math.Round(100 * recognizedFd.NdsFree), false));
                                    }    // СУММА БЕЗ НДС
                                    else if (line.StartsWith("1106"))
                                    {
                                        recognizedFd.Nds20120 = double.Parse(ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                        ftagsStrRepr.Add(new FTag(1106, (int)Math.Round(100 * recognizedFd.Nds20120), false));
                                    }    // Сумма НДС чека по расч. ставке 20/120
                                    else if (line.StartsWith("1107"))
                                    {
                                        recognizedFd.Nds10110 = double.Parse(ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                        ftagsStrRepr.Add(new FTag(1107, (int)Math.Round(100 * recognizedFd.Nds10110), false));
                                    }    // СУММА НДС 10/110:0.20
                                    else if (line.StartsWith("1173"))
                                    {
                                        recognizedFd.CorrectionTypeNotFtag = line[line.IndexOf(":") + 1]-'0';
                                        ftagsStrRepr.Add(new FTag(1173, recognizedFd.CorrectionTypeNotFtag, false));
                                    }    // Тип коррекции
                                    else if(line.StartsWith(" 1177"))
                                    {
                                        recognizedFd.CorrectionDocDescriber = line.Substring(line.IndexOf(":") + 1);
                                    }    // Описание коррекции
                                    else if(line.StartsWith(" 1178"))
                                    {
                                        recognizedFd.CorrectionDocumentDate = DateTime.ParseExact(line.Substring(line.IndexOf(':') + 1), DEFAULT_D_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
                                        if (ftagsStrRepr[ftagsStrRepr.Count - 1].TagNumber != 1174)
                                        {
                                            ftagsStrRepr.Add(new FTag(1174, new List<FTag>(), false));
                                        }
                                        ftagsStrRepr[ftagsStrRepr.Count - 1].Nested.Add(new FTag(1178, recognizedFd.CorrectionDocumentDate, false));
                                    }    // Дата документа основания для коррекции
                                    else if(line.StartsWith(" 1179"))
                                    {
                                        recognizedFd.CorrectionOrderNumber = line.Substring(line.IndexOf(':') + 1);
                                        if (ftagsStrRepr[ftagsStrRepr.Count - 1].TagNumber != 1174)
                                        {
                                            ftagsStrRepr.Add(new FTag(1174, new List<FTag>(), false));
                                        }
                                        ftagsStrRepr[ftagsStrRepr.Count - 1].Nested.Add(new FTag(1179, recognizedFd.CorrectionOrderNumber, false));
                                    }    // Номер документа основания для коррекции
                                    else if (line.StartsWith("1203"))
                                    {
                                        recognizedFd.CashierInn = line.Substring(line.IndexOf(":") + 1);
                                        ftagsStrRepr.Add(new FTag(1179, recognizedFd.CorrectionOrderNumber, false));
                                    }    // ИНН Кассира
                                    else if (line.StartsWith("1215"))
                                    {
                                        recognizedFd.Prepaid = double.Parse(ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                        ftagsStrRepr.Add(new FTag(1215, (int)Math.Round(100 * recognizedFd.Prepaid), false));
                                    }    // ПРЕДВАРИТЕЛЬНАЯ ОПЛАТА (АВАНС)
                                    else if (line.StartsWith("1216"))
                                    {
                                        recognizedFd.Credit = double.Parse(ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                        ftagsStrRepr.Add(new FTag(1216, (int)Math.Round(100 * recognizedFd.Credit), false));
                                    }    // ПОСЛЕДУЮЩАЯ ОПЛАТА (КРЕДИТ):1.00
                                    else if (line.StartsWith("1217"))
                                    {
                                        recognizedFd.Provision = double.Parse(ReplaceBadDecimalSeparatorPoint(line.Substring(line.IndexOf(":") + 1)));
                                        ftagsStrRepr.Add(new FTag(1217, (int)Math.Round(100 * recognizedFd.Provision), false));
                                    }    // Встречное предложение
                                    else if (line.StartsWith("1227"))
                                    {
                                        recognizedFd.BuyerInformationBuyer = line.Substring(line.IndexOf(":") + 1);
                                        ftagsStrRepr.Add(new FTag(1227, recognizedFd.BuyerInformationBuyer, false));
                                    }    // Покупатель (клиент)
                                    else if (line.StartsWith(" 1227"))
                                    {
                                        recognizedFd.BuyerInformationBuyer = line.Substring(line.IndexOf(":") + 1);
                                        ftagsStrRepr.Add(new FTag(1227, recognizedFd.BuyerInformationBuyer, false));
                                    }   // покупатель 1.2
                                    else if(line.StartsWith("1228"))
                                    {
                                        recognizedFd.BuyerInformationBuyerInn = line.Substring(line.IndexOf(":") + 1);
                                        ftagsStrRepr.Add(new FTag(1228, recognizedFd.BuyerInformationBuyerInn, false));
                                    }     // ИНН покупателя (клиента)
                                    else if (line.StartsWith(" 1228"))
                                    {
                                        recognizedFd.BuyerInformationBuyerInn = line.Substring(line.IndexOf(":") + 1);
                                        ftagsStrRepr.Add(new FTag(1228, recognizedFd.BuyerInformationBuyerInn, false));
                                    }   // ИНН покупателя 1.2
                                    else if(line.StartsWith(" 1243"))
                                    {
                                        recognizedFd.BuyerInformationBuyerBirthday = line.Substring(line.IndexOf(":") + 1);
                                    }
                                    else if (line.StartsWith(" 1244"))
                                    {
                                        recognizedFd.BuyerInformationBuyerCitizenship = line.Substring(line.IndexOf(":") + 1);
                                    }
                                    else if (line.StartsWith(" 1245"))
                                    {
                                        recognizedFd.BuyerInformationBuyerDocumentCode = line.Substring(line.IndexOf(":") + 1);
                                    }
                                    else if (line.StartsWith(" 1246"))
                                    {
                                        recognizedFd.BuyerInformationBuyerDocumentData = line.Substring(line.IndexOf(":") + 1);
                                    }
                                    else if (line.StartsWith(" 1254"))
                                    {
                                        recognizedFd.BuyerInformationBuyerAddress = line.Substring(line.IndexOf(":") + 1);
                                    }

                                    LogHandle.ol(line);
                                }

                                if (CorrectnessOfReading(recognizedFd))
                                {
                                    frd.Cheque = recognizedFd;
                                    ftagsStrRepr.Add(new FTag(FTAG_DATE_TIME,frd.Time,false));
                                    ftagsStrRepr.Add(new FTag(FTAG_DOC_FISCAL_SIGN,frd.FiscalSign,false));
                                    ftagsStrRepr.Add(new FTag(FTAG_FD, frd.Number, false));
                                    List <FTag> fdListTagsStr = new List<FTag>();
                                    FTag rootStrRecogn = new FTag(frd.Type, ftagsStrRepr, false);
                                    fdListTagsStr.Add(rootStrRecogn);
                                    frd.Tags = fdListTagsStr;
                                }
                                    
                                else
                                {
                                    if (effort < 4)
                                    {
                                        LogHandle.ol("Ошибка чтения ждем 100 мс и повторяем попытку");
                                        System.Threading.Thread.Sleep(50);
                                        goto AttempToRead;
                                    }
                                    frd.Cheque = null;
                                }

                            }
                            catch(Exception ex)
                            {
                                LogHandle.ol("При разборе документа произошла ошибка" + Environment.NewLine + ex.Message);
                                recognizedFd = null;
                            }
                            
                        }
                    }

                }
                else { LogHandle.ol("Ошибка при чтении документа " + Driver.ResultCodeDescription); }
            }

            if (parce && frd.Cheque == null) RezultMsg(DOC_NOT_READED);
            if(AppSettings.AppendFiscalSignAsPropertyData && frd.Cheque != null /*&& !string.IsNullOrEmpty(frd.FiscalSign) && string.IsNullOrEmpty(frd.Cheque.PropertiesData)*/)
            {
                if(AppSettings.OverridePropertyData)
                    frd.Cheque.PropertiesData = frd.FiscalSign;
                else
                {
                    if(!frd.Cheque.IsPropertiesData)
                        frd.Cheque.PropertiesData = frd.FiscalSign;
                    
                }
            }
            if(frd.Cheque != null && AppSettings.OverrideCorrectionOrderNumber)
            {
                frd.Cheque.CorrectionOrderNumber = AppSettings.CorrectionOrderNumberDefault;
            }
            if(frd.Cheque != null && (AppSettings.OverrideCorrectionDocumentDate || frd.Cheque.Document == FD_DOCUMENT_NAME_CHEQUE))
            {
                frd.Cheque.CorrectionDocumentDate = frd.Time;
            }
            return frd;
        }

        private bool _useCorrDescriber = false;

        int driverVerPrime, driverVerSub, driverVerBuild;
        DateTime firmwareBuild = new DateTime(1970, 1, 1);

        int _enablingPrinting = -1;
        public int EnablingPrinting
        {
            get
            {
                if (_enablingPrinting < 0)
                {
                    Driver.TableNumber = 17;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 7;
                    if (Driver.ReadTable() == 0)
                        _enablingPrinting = Driver.ValueOfFieldInteger;
                    else
                        _enablingPrinting = -1;
                }
                return _enablingPrinting;
            }
            set
            {
                _enablingPrinting = value;
                Driver.TableNumber = 17;
                Driver.RowNumber = 1;
                Driver.FieldNumber = 7;
                Driver.ValueOfFieldInteger = value;
                if (Driver.WriteTable() != 0)
                {
                    _enablingPrinting = -1;
                }
                   
            }
        }
        public int CheckEnablingPrinting { get => _enablingPrinting; }


        // автозаполнение налогов прошивкой
        // 0 - прошивкой
        // 1 - верхним ПО
        int _taxesFillFirmware = -1;
        public int TaxesFillsFirmware
        {
            get
            {
                if (_taxesFillFirmware<0)
                {
                    Driver.TableNumber = 1;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 14;
                    if (Driver.ReadTable() == 0)
                        _taxesFillFirmware = Driver.ValueOfFieldInteger;
                    else
                        _taxesFillFirmware = -1;
                }
                return _taxesFillFirmware;
            }
            set
            {
                _taxesFillFirmware = value;
                Driver.TableNumber = 1;
                Driver.RowNumber = 1;
                Driver.FieldNumber = 14;
                Driver.ValueOfFieldInteger = value;
                if (Driver.WriteTable() != 0) 
                {
                    _taxesFillFirmware = -1;
                }
                    
            }
        }
        public int CheckTaxesFillsFirmware { get => _taxesFillFirmware; }

        private const int
            SHTRIH_CONNECTION_TYPE_RS232 = 0,
            SHTRIH_CONNECTION_TYPE_SERVER_KKM_TCP = 1,
            SHTRIH_CONNECTION_TYPE_SERVER_KKM_DCOM = 2,
            SHTRIH_CONNECTION_TYPE_ESCAPE = 3,
            SHTRIH_CONNECTION_TYPE_EMULATOR = 5,
            SHTRIH_CONNECTION_TYPE_ТСР = 6,

            SI_ttByte = 0,      //.Тип Byte
            SI_ttUint16 = 1,    //.Тип Uint16
            SI_ttUint32 = 2,    //.Тип UInt32
            SI_ttVLN = 3,       //.Тип VLN
            SI_ttFVLN = 4,      //.Тип FVLN
            SI_ttBitMask = 5,   //.Тип "битовое поле"
            SI_ttUnixTime = 6,  //.Тип "время"
            SI_ttString = 7,    //. Тип "строка"
            SI_ttSTLV = 8,      //. Тип STLV.

            SI_TAX_ZERO = 0,    //. БЕЗ НДС
            SI_TAX_20 = 1,      //. НДС 20%
            SI_TAX_10 = 2,      //, НДС 10%
            SI_TAX_0 = 3,       //. НДС 0%
            SI_TAX_FREE = 4,    //. БЕЗ НДС
            SI_TAX_20120 = 5,   //. НДС 18/118
            SI_TAX_10110 = 6,   //. НДС 10/110

            SI_TAX_5 = 7,       //. НДС 10/110
            SI_TAX_7 = 8,       //. НДС 10/110
            SI_TAX_5105 = 9,    //. НДС 10/110
            SI_TAX_7107 = 10;   //. НДС 10/110
            


        private static int _originalPrintDisableFlag = -1;
        private void _PrintDisableFlagSet()
        {
            if(_dontPrint)
            {
                if(_deviceVer == 1)
                {
                    Driver.TableNumber = 1;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 48;
                }
                else
                {
                    Driver.TableNumber = 17;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 7;
                }
                
                Driver.ReadTable();
                _originalPrintDisableFlag = Driver.ValueOfFieldInteger;
                if (_deviceVer == 1)
                {
                    if(_originalPrintDisableFlag == 0 )
                    {
                        Driver.TableNumber = 1;
                        Driver.RowNumber = 1;
                        Driver.FieldNumber = 48;
                        Driver.ValueOfFieldInteger = 1;
                        Driver.WriteTable();
                    }

                }
                else
                {
                    if (_originalPrintDisableFlag == 0)
                    {
                        Driver.TableNumber = 17;
                        Driver.RowNumber = 1;
                        Driver.FieldNumber = 7;
                        Driver.ValueOfFieldInteger = 1;
                        Driver.WriteTable();
                    }
                }
                
            }
        }
        private void _PrintDisableFlagRestore()
        {

            if( _dontPrint && !(_originalPrintDisableFlag == 2 || _originalPrintDisableFlag == -1))
            {
                if(_deviceVer == 1)
                {
                    Driver.TableNumber = 1;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 48;
                }
                else
                {
                    Driver.TableNumber = 17;
                    Driver.RowNumber = 1;
                    Driver.FieldNumber = 7;
                }

                Driver.ValueOfFieldInteger = _originalPrintDisableFlag;
                Driver.WriteTable();
            }
        }

        public override bool ContinuePrint()
        {
            if (Driver != null)
            {
                if (Driver.ContinuePrint() != 0)
                {
                    RezultMsg(Driver.ResultCodeDescription);
                    return false;
                }
                RezultMsg(SUCCESS_MSG);
                return true;
            }
            else
                RezultMsg(NO_DRIVER_FOUNDED);
            return false;
        }

        public override bool CancelDocument()
        {
            if(Driver != null && _connected)
            {
                LogHandle.ol("отменяем чек FNCancelDocument");
                int errorCode = Driver.FNCancelDocument();
                RezultMsg(Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ErrorDescription);
                bool fnCancel = errorCode == NONE;
                LogHandle.ol("отменяем чек SysAdminCancelCheck");
                errorCode = Driver.SysAdminCancelCheck();
                if(fnCancel) LogHandle.ol(Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ResultCodeDescription);
                else RezultMsg(Error_codes_dict.ContainsKey(errorCode) ? Error_codes_dict[errorCode] : Driver.ErrorDescription);
                return fnCancel || errorCode == NONE;
            }
            RezultMsg(CONNECTION_NOT_ESTABLISHED);
            return false;
        }

        public override bool ChangeDate(int appendDay = 0, DateTime date = default)
        {
            if(appendDay == 0)
            {
                Driver.Date = date.Date;
                bool s1 = Driver.SetDate()==NONE;
                bool c1  = Driver.ConfirmDate()==NONE;
                Driver.Time = date;
                bool t1 = Driver.SetTime()==NONE;
                return s1&&c1&&t1;
            }
            if(appendDay > 0)
            {
                DateTime dtc = Driver.ECRDate.AddDays(appendDay);
                Driver.Date = dtc.Date;
                bool s2 = Driver.SetDate() == NONE;
                bool c2 = Driver.ConfirmDate() == NONE;
                return c2 && s2;
            }
            return false;
        }

        public override bool CashRefill(double sum, bool income = true)
        {
            _PrintDisableFlagSet();
            Driver.Summ1 = (decimal)sum;
            int result = income ? Driver.CashIncome() : Driver.CashOutcome();
            RezultMsg(Error_codes_dict.ContainsKey(result) ? Error_codes_dict[result] : Driver.ErrorDescription);
            _PrintDisableFlagRestore();
            return result == NONE;
        }

        private static readonly string[] _baudrate = 
        { 
            "2400", 
            "4800", 
            "9600", 
            "19200", 
            "38400", 
            "57600", 
            "115200", 
            "230400", 
            "460800", 
            "921600" 
        };

        static Dictionary<int, string> Mode_list = new Dictionary<int, string>()
        {
            [0] = "Принтер в рабочем режиме",
            [1]= "Выдача данных",
            [2]= "Открытая смена, 24 часа не кончились",
            [3]= "Открытая смена, 24 часа кончились",
            [4]= "Закрытая смена",
            [5]= "Блокировка по неправильному паролю налогового инспектора",
            [6]= "Ожидание подтверждения ввода даты",
            [7]= "Разрешение изменения положения десятичной точки",
            [8]= "Открытый документ",
            [9]= "Режим разрешения технологического обнуления",
            [10]= "Тестовый прогон",
            [11]= "Печать полного фискального отчета",
            [12]= "Печать длинного отчета ЭКЛЗ",
            [13]= "Работа с фискальным подкладным документом",
            [14]= "Печать подкладного документа",
            [15]= "Фискальный подкладной документ сформирован",
        };

        const int SinExep_NOT_COMPATIBLE_FIRMWARE = -99099;
        const int SinExep_DRIVER_NEED_TO_UPDATE = -99098;
        const int SinExep_BAD_SETTING_FOR_CORRECT_FD = -99097;
        static Dictionary<int, string> Error_codes_dict = new Dictionary<int, string>()
        {
            [SinExep_NOT_COMPATIBLE_FIRMWARE] = "Прошивка не подходит для ставок НДС 5,7,105,107. Если уверены в том что делаете добавьте строку в настройки Shtrih_debug_ignore_old_driver=true и перезапустите программу",
            [SinExep_DRIVER_NEED_TO_UPDATE] = "Обновите драйвер до 5.18 или выше, или смените настройки программы. Если уверены в том что делаете добавьте строку в настройки Shtrih_debug_ignore_old_driver=true и перезапустите программу",
            [SinExep_BAD_SETTING_FOR_CORRECT_FD] = "Некорректная настройка ККТ или программы, Оформленный ФД возможно будет текорректен. Если уверены в том что делаете добавьте строку в настройки Shtrih_debug_ignore_old_driver=true и перезапустите программу",
            [-17] = "Порт не открыт",
            [-13] = "Подытог чека не изменился",
            [-12] = "Не поддерживается в данной версии драйвера",
            [-11] = "Ошибка протокола",
            [-8] = "Connect timed out.",
            [-7] = "Неверная длина ответа",
            [-6] = "Нет связи",
            [-5] = "Нет связи",
            [-4] = "Нет связи",
            [-3] = "СOM-порт занят другим приложением",
            [-2] = "СOM-порт не доступен",
            [-1] = "Нет связи",
            [0] = "Ошибок нет",
            [1] = "Неисправен накопитель ФП 1, ФП 2 или часы",
            [2] = "Неверное состояние ФН",
            [47] = "Таймаут обмена с ФН",
            [48] = "ФН не отвечает",
            [51] = "Некорректные параметры в команде",
            [52] = "Нет данных",
            [53] = "Некорректный параметр при данных настройках",
            [54] = "Некорректные параметры в команде для данной реализации ККТ",
            [55] = "Команда не поддерживается в данной реализации ККТ",
            [69] = "Cумма всех типов оплаты меньше итога чека",
            [70] = "Не хватает наличности в кассе",
            [72] = "Переполнение итога чека",
            [73] = "Операция невозможна в открытом чеке данного типа",
            [74] = "Открыт чек - операция невозможна",
            [75] = "Буфер чека переполнен",
            [76] = "Переполнение накопления по обороту налогов в смене",
            [77] = "Вносимая безналичной оплатой сумма больше суммы чека",
            [78] = "Смена превысила 24 часа",
            [79] = "Неверный пароль",
            [80] = "Идет печать предыдущей команды",
            [85] = "Чек закрыт – операция невозможна",
            [88] = "Ожидание команды продолжения печати",
            [92] = "Понижено напряжение 24В",
            [93] = "Таблица не определена",
            [94] = "Некорректная операция",
            [95] = "Отрицательный итог чека",
            [107] = "Нет чековой ленты",
            [113] = "Ошибка отрезчика",
            [114] = "Команда не поддерживается в данном подрежиме",
            [115] = "Команда не поддерживается в данном режиме",
            [116] = "Ошибка ОЗУ",
            [117] = "Ошибка питания",
            [120] = "Замена ПО",
            [121] = "Замена ФП",
            [132] = "Переполнение наличности",
            [133] = "Переполнение по продажам в смене",
            [134] = "Переполнение по покупкам в смене",
            [135] = "Переполнение по возвратам продаж в смене",
            [136] = "Переполнение по возвратам покупок в смене",
        };
    }
}
