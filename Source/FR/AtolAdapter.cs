using Atol.Drivers10.Fptr;
using DrvFRLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace FR_Operator
{
    internal class AtolAdapter : FiscalPrinter
    {
        IFptr fptr = null;

        public AtolAdapter(MainForm ui)
        {
            this._ui = ui;
            fptr = new Fptr();
            KKMInfoTransmitter[FR_CONNECTION_SETTINGS_KEY] = ConnectionReprezentation();
        }
        public static string GetDrvVersion
        {
            get 
            {
                try
                {
                    IFptr tfptr = new Fptr();
                    string s = tfptr.version();
                    tfptr.destroy();
                    return s;
                }
                catch { return NO_DRIVER_FOUNDED; }
            }
        }


        public override void ReleaseLib()
        {
            LogHandle.ol("Releasing atol library");
            if (_connected)
                Disconnect();
            fptr.destroy();
            fptr = null;
        }

        public override bool Connect()
        {
            LogHandle.ol("Establishing a connection to the device");


            if (fptr != null)
            {
                int open = fptr.open();
                LogHandle.ol(fptr.errorDescription());
                _connected = open == 0;
                if (_connected)
                {
                    ReadDeviceCondition();
                    RezultMsg(CONNECTION_ESTABLISHED);
                }
                else
                    RezultMsg(fptr.errorDescription());
            }
            else
                RezultMsg(NO_DRIVER_FOUNDED);
            return _connected;
        }

        public override void Disconnect()
        {
            LogHandle.ol("disconnect atol device");
            if (fptr.isOpened())
                fptr.close();
            _connected = false;
            ClearInfo();
            _ui.UpdateUiKkmDescribtion();
        }

        public override void ReadDeviceCondition()
        {
            if (fptr == null)
                return;

            bool notReaded = KKMInfoTransmitter[FR_SERIAL_KEY] == "";
            fptr.setParam(Constants.LIBFPTR_PARAM_DATA_TYPE, Constants.LIBFPTR_DT_STATUS);
            fptr.queryData();
            uint mode = fptr.getParamInt(Constants.LIBFPTR_PARAM_MODE);
            KKMInfoTransmitter[FR_STATUS_MODE_KEY]=mode.ToString();
            uint shiftState = fptr.getParamInt(Constants.LIBFPTR_PARAM_SHIFT_STATE);
            string shift = "";
            if (shiftState == Constants.LIBFPTR_SS_CLOSED)
            {
                shift = "Смена закрыта";
                _shiftState = FR_SHIFT_CLOSED;
            }
            else if (shiftState == Constants.LIBFPTR_SS_OPENED)
            {
                shift = "Смена открыта";
                _shiftState = FR_SHIFT_OPEN;
            }
            else if (shiftState == Constants.LIBFPTR_SS_EXPIRED)
            {
                shift = "Смена истекла";
                _shiftState = FR_SHIFT_EXPIRED;
            }
            KKMInfoTransmitter[FR_SHIFT_STATE_KEY] = shift;
            string fr_time = fptr.getParamDateTime(Constants.LIBFPTR_PARAM_DATE_TIME).ToString(DEFAULT_DT_FORMAT);
            LogHandle.ol("FR time " + fr_time);
            KKMInfoTransmitter[FR_TIME_KEY] = fr_time;
            if (notReaded)
            {
                KKMInfoTransmitter[FR_MODEL_KEY] = fptr.getParamString(Constants.LIBFPTR_PARAM_MODEL_NAME);
                LogHandle.ol(fptr.getParamString(Constants.LIBFPTR_PARAM_MODEL_NAME));
                fptr.setParam(Constants.LIBFPTR_PARAM_DATA_TYPE, Constants.LIBFPTR_DT_SERIAL_NUMBER);
                fptr.queryData();
                string serial = fptr.getParamString(Constants.LIBFPTR_PARAM_SERIAL_NUMBER);
                LogHandle.ol(serial);
                KKMInfoTransmitter[FR_SERIAL_KEY] = serial;
                fptr.setParam(Constants.LIBFPTR_PARAM_DATA_TYPE, Constants.LIBFPTR_DT_UNIT_VERSION);
                fptr.setParam(Constants.LIBFPTR_PARAM_UNIT_TYPE, Constants.LIBFPTR_UT_CONFIGURATION);
                fptr.queryData();
                string kkm_firmware = fptr.getParamString(Constants.LIBFPTR_PARAM_UNIT_VERSION);
                LogHandle.ol(kkm_firmware);
                KKMInfoTransmitter[FR_FIRMWARE_KEY] = kkm_firmware;
                fptr.setParam(Constants.LIBFPTR_PARAM_FN_DATA_TYPE, Constants.LIBFPTR_FNDT_REG_INFO);
                fptr.fnQueryData();
                string userOwner = fptr.getParamString(1048);
                LogHandle.ol(userOwner);
                KKMInfoTransmitter[FR_OWNER_USER_KEY] = userOwner;
                String paymentsAddress = fptr.getParamString(1009);
                KKMInfoTransmitter[FR_OWNER_ADDRESS_KEY] = paymentsAddress;
                KKMInfoTransmitter[FR_REGFNS_KEY] = fptr.getParamString(1037);
                LogHandle.ol(paymentsAddress);
                uint taxationTypes = fptr.getParamInt(1062);
                string taxSystems = "";
                if ((taxationTypes & Constants.LIBFPTR_TT_OSN) != 0)
                {
                    taxSystems = SNO_TRADITIONAL;
                    _chosenSno = FR_SNO_OSN;
                    taxationTypes -= Constants.LIBFPTR_TT_OSN;
                    if (taxationTypes != 0) taxSystems += ',';
                }
                if ((taxationTypes & Constants.LIBFPTR_TT_USN_INCOME) != 0)
                {
                    taxSystems += SNO_USN_DOHOD;
                    _chosenSno = FR_SNO_USN_D;
                    taxationTypes -= Constants.LIBFPTR_TT_USN_INCOME;
                    if (taxationTypes != 0) taxSystems += ',';
                }
                if ((taxationTypes & Constants.LIBFPTR_TT_USN_INCOME_OUTCOME) != 0)
                {
                    taxSystems += SNO_USN_DR;
                    _chosenSno = FR_SNO_USN_D_R;
                    taxationTypes -= Constants.LIBFPTR_TT_USN_INCOME_OUTCOME;
                    if (taxationTypes != 0) taxSystems += ',';
                }
                if ((taxationTypes & Constants.LIBFPTR_TT_ESN) != 0)
                {
                    taxSystems += SNO_ESHN;
                    _chosenSno = FR_SNO_ESHN;
                    taxationTypes -= Constants.LIBFPTR_TT_ESN;
                    if (taxationTypes != 0) taxSystems += ',';
                }
                if ((taxationTypes & Constants.LIBFPTR_TT_ENVD) != 0)
                {
                    taxSystems += "ЕНВД";
                    _chosenSno = FR_SNO_ENVD;
                    taxationTypes -= Constants.LIBFPTR_TT_ENVD;
                    if (taxationTypes != 0) taxSystems += ',';
                }
                if ((taxationTypes & Constants.LIBFPTR_TT_PATENT) != 0)
                {
                    taxSystems += SNO_PSN;
                    _chosenSno = FR_SNO_PSN;
                    taxationTypes -= Constants.LIBFPTR_TT_PATENT;
                    if (taxationTypes != 0) taxSystems += ',';
                }
                LogHandle.ol(taxSystems);
                KKMInfoTransmitter[FR_REGISTERD_SNO_KEY] = taxSystems;
                uint ffdVersion = fptr.getParamInt(1209);
                string ffd = "";
                if (ffdVersion == Constants.LIBFPTR_FFD_1_0_5)
                {
                    ffd = "1.05";
                    _ffdVer = FR_FFD105;
                }
                else if (ffdVersion == Constants.LIBFPTR_FFD_1_1)
                {
                    _ffdVer = FR_FFD110;
                    ffd = "1.1";
                }
                else if (ffdVersion == Constants.LIBFPTR_FFD_1_2)
                {
                    _ffdVer = FR_FFD120;
                    ffd = "1.2";
                }
                KKMInfoTransmitter[FR_FFDVER_KEY] = ffd;
                LogHandle.ol("FFD: " + ffd);
            }
            fptr.setParam(Constants.LIBFPTR_PARAM_FN_DATA_TYPE, Constants.LIBFPTR_FNDT_LAST_DOCUMENT);
            fptr.fnQueryData();
            _lastFD = (int)fptr.getParamInt(Constants.LIBFPTR_PARAM_DOCUMENT_NUMBER);
            LogHandle.ol("Последний док. № " + _lastFD);
            KKMInfoTransmitter[FR_LAST_FD_NUMBER_KEY] = _lastFD.ToString();
            fptr.setParam(Constants.LIBFPTR_PARAM_FN_DATA_TYPE, Constants.LIBFPTR_FNDT_OFD_EXCHANGE_STATUS);
            fptr.fnQueryData();
            if (notReaded)
            {
                KKMInfoTransmitter[FR_FN_SERIAL_KEY] = fptr.getParamString(Constants.LIBFPTR_PARAM_SERIAL_NUMBER);
            }
            uint unsentCount = fptr.getParamInt(Constants.LIBFPTR_PARAM_DOCUMENTS_COUNT);
            DateTime dateTimeUnsent = fptr.getParamDateTime(Constants.LIBFPTR_PARAM_DATE_TIME);
            LogHandle.ol("Непереданных ФД: " + unsentCount + " от " + dateTimeUnsent.ToString(DEFAULT_DT_FORMAT));
            KKMInfoTransmitter[FR_OFD_EXCHANGE_STATUS_KEY] = unsentCount.ToString() + " / " + dateTimeUnsent.ToString(DEFAULT_DT_FORMAT);
            notReaded = KKMInfoTransmitter[FR_SERIAL_KEY] == "";
            if (notReaded)
                RezultMsg(BAD_SURVEY);
            _ui.UpdateUiKkmDescribtion();
        }

        public override bool ConnectionWindow()
        {
            if (fptr.showProperties(Constants.LIBFPTR_GUI_PARENT_NATIVE, _ui.Handle) == 0)
            {
                KKMInfoTransmitter[FR_CONNECTION_SETTINGS_KEY] = ConnectionReprezentation();
                _ui.UpdateConnectionParams(KKMInfoTransmitter[FR_CONNECTION_SETTINGS_KEY]);
                RezultMsg(SUCCESS_MSG);
                return true;
            }
            RezultMsg("");
            return false;
        }

        public override string ConnectionReprezentation()
        {
            if (fptr != null)
            {
                string port = fptr.getSingleSetting(Constants.LIBFPTR_SETTING_PORT),
                    connectionParams;
                if (port == Constants.LIBFPTR_PORT_USB.ToString()) { connectionParams = "USB"; }
                else if (port == Constants.LIBFPTR_PORT_COM.ToString())
                {
                    connectionParams = "COM" + fptr.getSingleSetting(Constants.LIBFPTR_SETTING_COM_FILE) + ':' + fptr.getSingleSetting(Constants.LIBFPTR_SETTING_BAUDRATE);
                }
                else if (port == Constants.LIBFPTR_PORT_TCPIP.ToString())
                {
                    connectionParams = "TCP:" + fptr.getSingleSetting(Constants.LIBFPTR_SETTING_IPADDRESS) + ':' + fptr.getSingleSetting(Constants.LIBFPTR_SETTING_IPPORT);
                }
                else
                {
                    connectionParams = "Unknown mb bluetooth";
                }
                RezultMsg(SUCCESS_MSG);
                return connectionParams;
            }
            RezultMsg(NO_DRIVER_FOUNDED);
            return "";
        }

        public override bool OpenShift()
        {
            //throw new NotImplementedException();
            if (fptr != null)
            {
                if (_connected)
                {
                    if (_shiftState != FR_SHIFT_OPEN || _shiftState != FR_SHIFT_EXPIRED)
                    {
                        if (KKMInfoTransmitter[FR_CASHIER_NAME_KEY] != "")
                        {
                            LogHandle.ol("Регистрация кассира " + KKMInfoTransmitter[FR_CASHIER_NAME_KEY]);
                            fptr.setParam(FTAG_CASHIER_NAME, KKMInfoTransmitter[FR_CASHIER_NAME_KEY]);
                            if (CorrectInn(KKMInfoTransmitter[FR_CASHIER_INN_KEY]))
                                fptr.setParam(FTAG_CASHIER_INN, KKMInfoTransmitter[FR_CASHIER_INN_KEY]);
                            fptr.operatorLogin();
                        }
                        if (_dontPrint)
                            fptr.setParam(Constants.LIBFPTR_PARAM_REPORT_ELECTRONICALLY, true);
                        if (fptr.openShift() == 0)
                        {
                            RezultMsg(SUCCESS_MSG);
                            _shiftState = FR_SHIFT_OPEN;
                            return true;
                        }
                        RezultMsg(fptr.errorDescription());
                        return false;
                    }
                    RezultMsg("Смена уже открыта");
                    return false;
                }
                RezultMsg(CONNECTION_NOT_ESTABLISHED);
                return false;
            }
            RezultMsg(NO_DRIVER_FOUNDED);
            return false;
        }

        public override bool CloseShift()
        {
            if (fptr != null)
            {
                if (_connected)
                {
                    if (_shiftState != FR_SHIFT_CLOSED)
                    {
                        LogHandle.ol("Закрываем смену");
                        if (KKMInfoTransmitter[FR_CASHIER_NAME_KEY] != "")
                        {
                            LogHandle.ol("Регистрация кассира " + KKMInfoTransmitter[FR_CASHIER_NAME_KEY] + " inn-" + KKMInfoTransmitter[FR_CASHIER_INN_KEY]);
                            fptr.setParam(FTAG_CASHIER_NAME, KKMInfoTransmitter[FR_CASHIER_NAME_KEY]);
                            if (CorrectInn(KKMInfoTransmitter[FR_CASHIER_INN_KEY]))
                                fptr.setParam(FTAG_CASHIER_INN, KKMInfoTransmitter[FR_CASHIER_INN_KEY]);
                            if (fptr.operatorLogin() != 0)
                                LogHandle.ol(fptr.errorDescription());
                        }
                        if (_dontPrint)
                            fptr.setParam(Constants.LIBFPTR_PARAM_REPORT_ELECTRONICALLY, true);
                        fptr.setParam(Constants.LIBFPTR_PARAM_REPORT_TYPE, Constants.LIBFPTR_RT_CLOSE_SHIFT);
                        //if(_dontPrint) fptr.setParam(Constants.LIBFPTR_PARAM_RECEIPT_ELECTRONICALLY, 1);
                        if (fptr.report() != 0)
                        {
                            string error = fptr.errorDescription();
                            LogHandle.ol(error);
                            RezultMsg(error);
                            return false;
                        }
                        RezultMsg(SUCCESS_MSG);
                        return true;
                    }
                    RezultMsg("Смена уже закрыта");
                    return false;
                }
                RezultMsg(CONNECTION_NOT_ESTABLISHED);
                return false;
            }
            RezultMsg(NO_DRIVER_FOUNDED);
            return false;
        }

        public override bool PerformFD(FiscalCheque doc)
        {
            LogHandle.ol("Отправляем документ в ФР" + Environment.NewLine + doc.ToString(FiscalCheque.FULL_INFO));
            if (fptr == null)
            {
                RezultMsg(NO_DRIVER_FOUNDED);
                return false;
            }
            if(!_connected)
            {
                RezultMsg(CONNECTION_NOT_ESTABLISHED);
                return false;
            }
            if(doc == null)
            {
                RezultMsg("Передан пустой документ");
                return false;
            }
            if(_shiftState==FR_SHIFT_CLOSED)
                OpenShift();

            if (doc.Cashier != DEFAULT_CASHIER)
            {
                fptr.setParam(FTAG_CASHIER_NAME, doc.Cashier);
                if (!string.IsNullOrEmpty(doc.CashierInn))
                {
                    fptr.setParam(FTAG_CASHIER_INN, doc.CashierInn);
                }
                fptr.operatorLogin();
            }
            if (AppSettings.OverideRetailAddress && !string.IsNullOrEmpty(doc.RetailAddress))
            {
                fptr.setParam(FTAG_RETAIL_PLACE_ADRRESS, doc.RetailAddress);
            }
            if (AppSettings.OverideRetailPlace && !string.IsNullOrEmpty(doc.RetailPlace))
            {
                fptr.setParam(FTAG_RETAIL_PLACE, doc.RetailPlace);
            }
            if (!string.IsNullOrEmpty(doc.EmailPhone))
            {
                fptr.setParam(FTAG_DESTINATION_EMAIL, doc.EmailPhone);
            }
            if (doc.InternetPayment)
            {
                fptr.setParam(FTAG_INTERNET_PAYMENT, true);
            }

            if (doc.BuyerInformation /*&& doc.Document == FD_DOCUMENT_NAME_CHEQUE*/)    
            {
                if (FfdVer <= FR_FFD110)
                {
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyer))
                    {
                        fptr.setParam(FTAG_BUYER_INFORMATION_BUYER, doc.BuyerInformationBuyer);
                    }

                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerInn))
                    {
                        fptr.setParam(FTAG_BUYER_INFORMATION_BUYER_INN, doc.BuyerInformationBuyerInn);
                    }
                }
                else
                {
                    List<FTag> bi = new List<FTag>();
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyer))
                    {
                        bi.Add( new FTag(FTAG_BUYER_INFORMATION_BUYER, doc.BuyerInformationBuyer, true) );
                    }
                    bool buyerInn = (!string.IsNullOrEmpty(doc.BuyerInformationBuyerInn)) && CorrectInn(doc.BuyerInformationBuyerInn);
                    if (buyerInn)
                    {
                        bi.Add( new FTag(FTAG_BUYER_INFORMATION_BUYER_INN, doc.BuyerInformationBuyerInn,true) );
                    }
                    if(AppSettings.AlwaysSendBuyerDocData || (!buyerInn))
                    {
                        if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerBirthday))
                        {
                            bi.Add(new FTag(FTAG_BI_BIRTHDAY, doc.BuyerInformationBuyerBirthday, true));
                        }
                        if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerCitizenship))
                        {
                            bi.Add(new FTag(FTAG_BI_CITIZENSHIP, doc.BuyerInformationBuyerCitizenship, true));
                        }
                        if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerDocumentCode))
                        {
                            bi.Add(new FTag(FTAG_BI_DOCUMENT_CODE, doc.BuyerInformationBuyerDocumentCode, true));
                        }
                        if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerDocumentData))
                        { 
                            bi.Add(new FTag(FTAG_BI_DOCUMENT_DATA, doc.BuyerInformationBuyerDocumentData, true)); 
                        }
                    }
                    if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerAddress))
                    {
                        bi.Add(new FTag(FTAG_BI_ADDRESS, doc.BuyerInformationBuyerAddress,true));
                    }
                    byte[] clientInfo = new FTag(FTAG_BUYER_INFORMATION, bi, true).RawData;
                    Debug.WriteLine(BitConverter.ToString(clientInfo));
                    fptr.setParam(FTAG_BUYER_INFORMATION, clientInfo);
                }
            }

            

            if (doc.Document == FD_DOCUMENT_NAME_CHEQUE)
            {
                if(doc.CalculationSign == FD_CALCULATION_INCOME_LOC)
                {
                    fptr.setParam(Constants.LIBFPTR_PARAM_RECEIPT_TYPE, Constants.LIBFPTR_RT_SELL);
                }
                else if(doc.CalculationSign == FD_CALCULATION_BACK_INCOME_LOC)
                {
                    fptr.setParam(Constants.LIBFPTR_PARAM_RECEIPT_TYPE, Constants.LIBFPTR_RT_SELL_RETURN);
                }
                else if(doc.CalculationSign == FD_CALCULATION_EXPENCE_LOC)
                {
                    fptr.setParam(Constants.LIBFPTR_PARAM_RECEIPT_TYPE, Constants.LIBFPTR_RT_BUY);
                }
                else if(doc.CalculationSign == FD_CALCULATION_BACK_EXPENCE_LOC)
                {
                    fptr.setParam(Constants.LIBFPTR_PARAM_RECEIPT_TYPE, Constants.LIBFPTR_RT_BUY_RETURN);
                }
                else
                {
                    RezultMsg("Неизвестный признак расчета");
                    return false;
                }
            }
            else if(doc.Document == FD_DOCUMENT_NAME_CORRECTION_CHEQUE)
            {

                var corrInfoFtags = new List<FTag>();
                corrInfoFtags.Add(new FTag(FTAG_CORRECTION_DOC_DATE, doc.CorrectionDocumentDate,true));

                if (!string.IsNullOrEmpty(doc.CorrectionOrderNumber))
                {
                    if (AppSettings.CorrectionOrderExistance == 1 && doc.CorrectionTypeFtag == 1
                            || AppSettings.CorrectionOrderExistance == 0)
                        corrInfoFtags.Add(new FTag(FTAG_CORRECTION_ORDER_NUMBER, doc.CorrectionOrderNumber,true));
                }
                //string fw = KKMInfoTransmitter[FR_FIRMWARE_KEY];

                byte[] correctionInfo = new FTag(FTAG_CORRECTION_BASE, corrInfoFtags, true).RawData;//fptr.getParamByteArray(Constants.LIBFPTR_PARAM_TAG_VALUE);
                LogHandle.ol(BitConverter.ToString(correctionInfo));

                if (doc.CalculationSign == FD_CALCULATION_INCOME_LOC)
                {
                    fptr.setParam(Constants.LIBFPTR_PARAM_RECEIPT_TYPE, Constants.LIBFPTR_RT_SELL_CORRECTION);
                }
                else if (doc.CalculationSign == FD_CALCULATION_BACK_INCOME_LOC)
                {
                    fptr.setParam(Constants.LIBFPTR_PARAM_RECEIPT_TYPE, Constants.LIBFPTR_RT_SELL_RETURN_CORRECTION);
                }
                else if (doc.CalculationSign == FD_CALCULATION_EXPENCE_LOC)
                {
                    fptr.setParam(Constants.LIBFPTR_PARAM_RECEIPT_TYPE, Constants.LIBFPTR_RT_BUY_CORRECTION);
                }
                else if (doc.CalculationSign == FD_CALCULATION_BACK_EXPENCE_LOC)
                {
                    fptr.setParam(Constants.LIBFPTR_PARAM_RECEIPT_TYPE, Constants.LIBFPTR_RT_BUY_RETURN_CORRECTION);
                }
                else
                {
                    RezultMsg("Неизвестный тип чека");
                    return false;
                }
                

                fptr.setParam(FTAG_CORRECTION_TYPE, doc.CorrectionTypeFtag);
                
                fptr.setParam(FTAG_CORRECTION_BASE,correctionInfo);
            }

            if (doc.IsPropertiesData && AppSettings.AtolUsePropertyData)
            {
                fptr.setParam(FTAG_PROPERTIES_DATA, doc.PropertiesData);
            }
            if (doc.IsProperties1084)
            {
                List<FTag> tags = new List<FTag>();
                tags.Add(new FTag(FTAG_PROPERTIES_PROPERTY_NAME, doc.PropertiesPropertyName, true));
                tags.Add(new FTag(FTAG_PROPERTIES_PROPERTY_VALUE, doc.PropertiesPropertyValue, true));
                FTag tag1084 = new FTag(FTAG_PRORERTIES_1084, tags, true);
                fptr.setParam(FTAG_PRORERTIES_1084, tag1084.RawData);
                LogHandle.ol("Add tag 1084  ");
            }

            // добавить проверку на отсутсвие в настиройках СНО по умолчанию
            if (AppSettings.UsingCustomSno&&doc.Sno!=0) 
            {
                fptr.setParam(FTAG_APPLIED_TAXATION_TYPE,doc.Sno);
            }

            if (_dontPrint)
            {
                fptr.setParam(Constants.LIBFPTR_PARAM_RECEIPT_ELECTRONICALLY, true);    

            }

            LogHandle.ol("Открываем чек openReceipt");
            if(fptr.openReceipt()!=0)
            {
                RezultMsg(fptr.errorDescription());
                KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                return false;
            }
            
            // печать доп информации о ФД
            if (!_dontPrint && AppSettings.PrintExtendedTextInfo && doc.IsExtendedInfoForPrinting)
            {
                for(int i = 0; i < AppSettings.ExtendedInfoTopOffset; i++) 
                {
                    fptr.printText();
                }
                foreach(string s in doc.ExtendedInfoForPrinting)
                {
                    fptr.setParam(Constants.LIBFPTR_PARAM_TEXT, s);
                    fptr.setParam(Constants.LIBFPTR_PARAM_ALIGNMENT, Constants.LIBFPTR_ALIGNMENT_RIGHT);
                    fptr.setParam(Constants.LIBFPTR_PARAM_FONT, AppSettings.AtolFontForPrinting);
                    fptr.setParam(Constants.LIBFPTR_PARAM_FONT_DOUBLE_WIDTH, false);
                    fptr.setParam(Constants.LIBFPTR_PARAM_FONT_DOUBLE_HEIGHT, false);
                    fptr.printText();

                }
                if (AppSettings.ExtendedInfoCleanAfterPrint)
                {
                    doc.ExtendedInfoForPrinting.Clear();
                }

                for (int i = 0; i < AppSettings.ExtendedInfoBottomOffset; i++)
                {
                    fptr.printText();
                }
            }



            double tally = 0;
            foreach(ConsumptionItem item in doc.Items)
            {
                tally += item.Sum;
                if( _ffdVer<FR_FFD110 && doc.Document == FD_DOCUMENT_NAME_CORRECTION_CHEQUE)
                {
                    LogHandle.ol("Коррекции в ФФД 1.05 не пооддерживают предметы расчета пропускаем регистрацию позиции");
                    continue;
                }
                fptr.setParam(Constants.LIBFPTR_PARAM_COMMODITY_NAME, item.Name);
                fptr.setParam(Constants.LIBFPTR_PARAM_PRICE, item.Price);
                fptr.setParam(Constants.LIBFPTR_PARAM_QUANTITY, item.Quantity);
                fptr.setParam(Constants.LIBFPTR_PARAM_POSITION_SUM, item.Sum);
                if (item.ProductType != NONE)
                {
                    fptr.setParam(FTAG_ITEM_PRODUCT_TYPE, item.ProductType);
                }
                if (item.PaymentType != NONE)
                {
                    fptr.setParam(FTAG_ITEM_PAYMENT_TYPE, item.PaymentType);
                }
                if(item.NdsRate != NONE)
                {

                    if(item.NdsRate == NDS_TYPE_20_LOC)
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT20);
                    else if (item.NdsRate == NDS_TYPE_10_LOC)
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT10);
                    else if (item.NdsRate == NDS_TYPE_20120_LOC)
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT120);
                    else if (item.NdsRate == NDS_TYPE_10110_LOC)
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT110);
                    else if (item.NdsRate == NDS_TYPE_0_LOC)
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT0);
                    else if (item.NdsRate == NDS_TYPE_FREE_LOC)
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_NO);
                    else if (item.NdsRate == NDS_TYPE_5_LOC)
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT5);
                    else if (item.NdsRate == NDS_TYPE_7_LOC)
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT7);
                    else if (item.NdsRate == NDS_TYPE_5105_LOC)
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT105);
                    else if (item.NdsRate == NDS_TYPE_7107_LOC)
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT107);
                    else if (item.NdsRate == NDS_TYPE_22_LOC)
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT22);
                    else if (item.NdsRate == NDS_TYPE_22122_LOC)
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT122);
                }

                if (!string.IsNullOrEmpty(item.Unit105))
                {
                    if(_ffdVer<FR_FFD120)fptr.setParam(FTAG_ITEM_UNIT_MEASURE_105, item.Unit105);
                }
                if (_ffdVer >= FR_FFD120)
                {
                    if (item.Unit120 > -1)
                    {
                        fptr.setParam(FTAG_ITEM_UNIT_MEASURE_120, item.Unit120);
                    }
                    else
                    {
                        if(AppSettings.AutoUnit120SetZero) fptr.setParam(FTAG_ITEM_UNIT_MEASURE_120, 0);
                    }
                }
                if (!string.IsNullOrEmpty(item.Code105)&&_ffdVer< FR_FFD120) // 1162
                {
                    fptr.setParam(FTAG_ITEM_PRODUCT_CODE, Encoding.ASCII.GetBytes(item.Code105));
                }// добавить 1163 для ФФД 1.2
                else
                {
                    fptr.setParam(1163, Encoding.ASCII.GetBytes(item.Code105));
                }
                if (!string.IsNullOrEmpty(item.ProviderInn))
                {
                    fptr.setParam(FTAG_ITEM_PROVIDER_INN, item.ProviderInn);
                }
                if (item.IsProviderData)
                {
                    fptr.setParam(FTAG_ITEM_PROVIDER_DATA, item.ProviderData.RawData);
                }
                if (!string.IsNullOrEmpty(item.CustomEntryNum))
                {
                    fptr.setParam(FTAG_ITEM_CUSTOM_ENTRY_NUM, item.CustomEntryNum);
                }
                if (!string.IsNullOrEmpty(item.OriginalCountryCode))
                {
                    fptr.setParam(FTAG_ITEM_ORIGINAL_COUNTRY_CODE, item.OriginalCountryCode);
                }
                if (item.PaymentAgentByProductType > 0)
                {
                    fptr.setParam(FTAG_ITEM_PAYMENT_AGENT_BY_PRODUCT_TYPE, item.PaymentAgentByProductType);
                }
                if (item.IsPaymentAgentData)
                {
                    fptr.setParam(FTAG_ITEM_PAYMENT_AGENT_DATA, item.PaymentAgentData.RawData);
                }

                LogHandle.ol("Регистрируем позицию registration");
                if (fptr.registration() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }
            if (doc.Document == FD_DOCUMENT_NAME_CORRECTION_CHEQUE && doc.FFDVer <= FR_FFD105 && tally == 0 && doc.TotalSum < 0.0099)
            {
                LogHandle.ol("Нулевой итог чека коррекции. ");
            }
            LogHandle.ol("Регистрируем итог receiptTotal");
            fptr.setParam(Constants.LIBFPTR_PARAM_SUM, doc.TotalSum);
            if (fptr.receiptTotal() != 0)
            {
                RezultMsg(fptr.errorDescription());
                KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                LogHandle.ol("Отменяем чек cancelReceipt");
                fptr.cancelReceipt();
                LogHandle.ol(fptr.errorDescription());
                return false;
            }

            if (doc.Nds20 > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем {0} НДС 20% receiptTax", doc.Nds20));
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT20);
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_SUM, doc.Nds20);
                if (fptr.receiptTax() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }
            if (doc.Nds10 > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем {0} НДС 10% receiptTax", doc.Nds10));
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT10);
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_SUM, doc.Nds10);
                if (fptr.receiptTax() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }
            if (doc.Nds20120 > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем {0} НДС 20/120% receiptTax", doc.Nds20120));
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT120);
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_SUM, doc.Nds20120);
                if (fptr.receiptTax() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }
            if (doc.Nds10110 > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем {0} НДС 10/110% receiptTax", doc.Nds10110));
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT110);
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_SUM, doc.Nds10110);
                if (fptr.receiptTax() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }

            /*
             * добавсяем новые ставки 2025
             */
            if (doc.Nds5 > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем {0} НДС 5% receiptTax", doc.Nds5));
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT5);
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_SUM, doc.Nds5);
                if (fptr.receiptTax() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }
            if (doc.Nds7 > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем {0} НДС 5% receiptTax", doc.Nds7));
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT7);
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_SUM, doc.Nds7);
                if (fptr.receiptTax() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }
            if (doc.Nds5105 > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем {0} НДС 5% receiptTax", doc.Nds5105));
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT105);
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_SUM, doc.Nds5105);
                if (fptr.receiptTax() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }
            if (doc.Nds7107 > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем {0} НДС 5% receiptTax", doc.Nds7107));
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT107);
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_SUM, doc.Nds7107);
                if (fptr.receiptTax() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }
            // 22 - 2026г
            if (doc.Nds22 > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем {0} НДС 22% receiptTax", doc.Nds22));
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT22);
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_SUM, doc.Nds22);
                if (fptr.receiptTax() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }
            if (doc.Nds22122 > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем {0} НДС 22/122 receiptTax", doc.Nds22122));
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT122);
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_SUM, doc.Nds22122);
                if (fptr.receiptTax() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }


            /*
             * добавсяем нулевые ставки
             */

            if ( doc.Nds0 > 0.0099 )
            {
                LogHandle.ol(string.Format("Регистрируем {0} сумму с НДС 0% receiptTax", doc.Nds0));
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT0);
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_SUM, doc.Nds0);
                if (fptr.receiptTax() != 0)
                {
                    LogHandle.ol(fptr.errorDescription());
                    LogHandle.ol("ККТ не приняла сумму налога, возможно округление суммы итога в чеке, анализируем чек....");
                    double deltaTax, taxSumItems = 0;
                    foreach (ConsumptionItem item in doc.Items)
                    {
                        if (item.NdsRate > 0)
                            taxSumItems += item.Sum;
                    }
                    deltaTax = Math.Round(Math.Abs(taxSumItems - doc.TotalSum), 2);
                    if (deltaTax > 0.0099)
                    {
                        LogHandle.ol("Сумма итога отличается от суммы предметов расчета на deltaTax=" + deltaTax);
                        LogHandle.ol("Пытаемся передать НДС 0 = НДС 0 - deltaTax");
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_VAT0);
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_SUM, Math.Round(doc.Nds0 - deltaTax, 2));
                        if (fptr.receiptTax() != 0)
                        {
                            LogHandle.ol("Ошибку исправить не удалось прерываем оформление ФД");
                            RezultMsg(fptr.errorDescription());
                            KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                            LogHandle.ol("Отменяем чек cancelReceipt");
                            fptr.cancelReceipt();
                            LogHandle.ol(fptr.errorDescription());
                            return false;
                        }
                        goto ContinueTaxZero;
                    }

                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                ContinueTaxZero:
                    LogHandle.ol("Ошибка исправлена продолжаем оформление документа");

                }
            }
            if (doc.NdsFree > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем {0} сумму без НДС receiptTax", doc.NdsFree));
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_NO);
                fptr.setParam(Constants.LIBFPTR_PARAM_TAX_SUM, doc.NdsFree);
                if (fptr.receiptTax() != 0)
                {
                    LogHandle.ol(fptr.errorDescription());
                    LogHandle.ol("ККТ не приняла сумму налога, возможно округление суммы итога в чеке, анализируем чек....");
                    double deltaTax, taxSumItems = 0;
                    foreach (ConsumptionItem item in doc.Items)
                    {
                        if(item.NdsRate >0) 
                            taxSumItems += item.Sum;
                    }
                    deltaTax = Math.Round(Math.Abs(taxSumItems - doc.TotalSum),2);
                    if (deltaTax>0.0099)
                    {
                        LogHandle.ol("Сумма итога отличается от суммы предметов расчета на deltaTax=" + deltaTax);
                        LogHandle.ol("Пытаемся передать Без НДС = Без НДС - deltaTax");
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, Constants.LIBFPTR_TAX_NO);
                        fptr.setParam(Constants.LIBFPTR_PARAM_TAX_SUM, Math.Round(doc.NdsFree - deltaTax,2));
                        if (fptr.receiptTax() != 0)
                        {
                            LogHandle.ol("Ошибку исправить не удалось прерываем оформление ФД");
                            RezultMsg(fptr.errorDescription());
                            KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                            LogHandle.ol("Отменяем чек cancelReceipt");
                            fptr.cancelReceipt();
                            LogHandle.ol(fptr.errorDescription());
                            return false;
                        }
                        goto ContinueNoTax;
                    }

                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                ContinueNoTax:
                    LogHandle.ol("Ошибка исправлена продолжаем оформление документа");

                }
            }
            

            if (doc.ECash > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем оплату безналичными {0} payment", doc.ECash));
                fptr.setParam(Constants.LIBFPTR_PARAM_PAYMENT_TYPE, Constants.LIBFPTR_PT_ELECTRONICALLY);
                fptr.setParam(Constants.LIBFPTR_PARAM_PAYMENT_SUM, doc.ECash);
                if (fptr.payment() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }
            if (doc.Prepaid > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем оплату зачет аванса {0} payment", doc.Prepaid));
                fptr.setParam(Constants.LIBFPTR_PARAM_PAYMENT_TYPE, Constants.LIBFPTR_PT_PREPAID);
                fptr.setParam(Constants.LIBFPTR_PARAM_PAYMENT_SUM, doc.Prepaid);
                if (fptr.payment() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }
            if (doc.Credit > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем оплату кредит {0} payment", doc.Credit));
                fptr.setParam(Constants.LIBFPTR_PARAM_PAYMENT_TYPE, Constants.LIBFPTR_PT_CREDIT);
                fptr.setParam(Constants.LIBFPTR_PARAM_PAYMENT_SUM, doc.Credit);
                if (fptr.payment() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }
            if (doc.Provision > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем оплату ВП {0} payment", doc.Provision));
                fptr.setParam(Constants.LIBFPTR_PARAM_PAYMENT_TYPE, Constants.LIBFPTR_PT_OTHER);
                fptr.setParam(Constants.LIBFPTR_PARAM_PAYMENT_SUM, doc.Provision);
                if (fptr.payment() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }
            if (doc.Cash > 0.0099)
            {
                LogHandle.ol(string.Format("Регистрируем оплату наличными {0} payment", doc.Cash));
                fptr.setParam(Constants.LIBFPTR_PARAM_PAYMENT_TYPE, Constants.LIBFPTR_PT_CASH);
                fptr.setParam(Constants.LIBFPTR_PARAM_PAYMENT_SUM, doc.Cash);
                if (fptr.payment() != 0)
                {
                    RezultMsg(fptr.errorDescription());
                    KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                    LogHandle.ol("Отменяем чек cancelReceipt");
                    fptr.cancelReceipt();
                    LogHandle.ol(fptr.errorDescription());
                    return false;
                }
            }

            LogHandle.ol("Закрываем чек fptr.closeReceipt();");
            if (fptr.closeReceipt() != 0)
            {
                RezultMsg(fptr.errorDescription());
                KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY] = fptr.errorDescription();
                LogHandle.ol("Отменяем чек cancelReceipt");
                fptr.cancelReceipt();
                LogHandle.ol(fptr.errorDescription());
                return false;
            }

            return true;
        }

        public override FnReadedDocument ReadFD(int docNumber, bool parce = false)
        {
            int effort = 0;

            AttempToRead:

            fptr.setParam(Constants.LIBFPTR_PARAM_FN_DATA_TYPE, Constants.LIBFPTR_FNDT_DOCUMENT_BY_NUMBER);
            fptr.setParam(Constants.LIBFPTR_PARAM_DOCUMENT_NUMBER, docNumber);
            fptr.fnQueryData();
            double tally = fptr.getParamDouble(FTAG_TOTAL_SUM);
            string docFiscalSign = fptr.getParamString(Constants.LIBFPTR_PARAM_FISCAL_SIGN);
            DateTime dateTime = fptr.getParamDateTime(Constants.LIBFPTR_PARAM_DATE_TIME);

            uint documentType = fptr.getParamInt(Constants.LIBFPTR_PARAM_FN_DOCUMENT_TYPE);
            
            if (parce && (documentType == Constants.LIBFPTR_FN_DOC_RECEIPT||documentType == Constants.LIBFPTR_FN_DOC_CORRECTION|| documentType == Constants.LIBFPTR_FN_DOC_BSO || documentType == Constants.LIBFPTR_FN_DOC_BSO_CORRECTION))
            {
                List<FTag> ftagList = new List<FTag>();
                fptr.setParam(Constants.LIBFPTR_PARAM_RECORDS_TYPE, Constants.LIBFPTR_RT_FN_DOCUMENT_TLVS);
                fptr.setParam(Constants.LIBFPTR_PARAM_DOCUMENT_NUMBER, docNumber);
                fptr.beginReadRecords();
                FiscalCheque cheque = new FiscalCheque();
                if(documentType == Constants.LIBFPTR_FN_DOC_RECEIPT)
                {
                    cheque.Document = FD_DOCUMENT_NAME_CHEQUE;
                }
                else /*if(documentType == Constants.LIBFPTR_FN_DOC_CORRECTION)*/
                {
                    cheque.Document = FD_DOCUMENT_NAME_CORRECTION_CHEQUE;
                }
                //uint documentType = fptr.getParamInt(Constants.LIBFPTR_PARAM_FN_DOCUMENT_TYPE);
                //uint documentSize = fptr.getParamInt(Constants.LIBFPTR_PARAM_COUNT);
                String recordsID = fptr.getParamString(Constants.LIBFPTR_PARAM_RECORDS_ID);
                fptr.setParam(Constants.LIBFPTR_PARAM_RECORDS_ID, recordsID);
                int tegs = 0;
                
                while(fptr.readNextRecord() == Constants.LIBFPTR_OK)
                {
                    tegs++;
                    byte[] tagValue = fptr.getParamByteArray(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    uint tagNumber = fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_NUMBER);
                    String tagName = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_NAME);
                    uint tagType = fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_TYPE);

                    ftagList.Add(new FTag((int)tagNumber,tagValue));

                    if (tagNumber == FTAG_DESTINATION_EMAIL)
                    {
                        cheque.EmailPhone = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if (tagNumber == FTAG_RETAIL_PLACE_ADRRESS)
                    {
                        cheque.RetailAddress = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if(tagNumber == FTAG_DOC_FISCAL_SIGN)
                    {
                        if (docFiscalSign==null||docFiscalSign==""||docFiscalSign.Length<5)
                        {
                            byte[] bytes = fptr.getParamByteArray(Constants.LIBFPTR_PARAM_TAG_VALUE);
                            uint fs = 0;
                            int i = bytes.Length - 4;
                            for (; i >= 0 && i < bytes.Length; i++)
                            {
                                fs += (uint)(bytes[i] * Math.Pow(256, bytes.Length - i - 1));
                            }
                            docFiscalSign = fs.ToString("D10");
                        }
                    }
                    else if(tagNumber == FTAG_PROPERTIES_DATA)
                    {
                        cheque.PropertiesData = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if(tagNumber == FTAG_TOTAL_SUM)
                    {
                        cheque.TotalSum = 0.01*fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if(tagNumber == FTAG_CASHIER_NAME)
                    {
                        cheque.Cashier = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if(tagNumber == FTAG_CASH_TOTAL_SUM)
                    {
                        cheque.Cash = 0.01 * fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if(tagNumber == FTAG_OPERATION_TYPE)
                    {
                        cheque.CalculationSign = (int)fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if(tagNumber == FTAG_APPLIED_TAXATION_TYPE)
                    {
                        cheque.Sno = (int)fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if(tagNumber == FTAG_ITEM)
                    {
                        LogHandle.ol("Parcing item");
                        ConsumptionItem item = parceStlvItem(fptr.getParamByteArray(Constants.LIBFPTR_PARAM_TAG_VALUE));
                        if(item.Correctness!= FD_ITEM_CONTROL_CRITICAL_ERROR)
                            cheque.Items.Add( item );
                        else
                        {
                            cheque = null;
                            break;
                        }
                    }
                    else if(tagNumber == FTAG_ECASH_TOTAL_SUM)
                    {
                        cheque.ECash = 0.01*fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if (tagNumber == FTAG_PRORERTIES_1084)
                    {
                        var rawdata1084 = fptr.getParamByteArray(Constants.LIBFPTR_PARAM_TAG_VALUE);
                        FTag tag1084 = new FTag(FTAG_PRORERTIES_1084, rawdata1084);
                        if (tag1084.Nested!=null&& tag1084.Nested.Count == 2 && tag1084.Nested[0].TagNumber == FTAG_PROPERTIES_PROPERTY_NAME)
                        {
                            cheque.PropertiesPropertyName = tag1084.Nested[0].ValueStr;
                            cheque.PropertiesPropertyValue = tag1084.Nested[1].ValueStr;
                        }
                        else if(tag1084.Nested != null && tag1084.Nested.Count == 2 && tag1084.Nested[0].TagNumber == FTAG_PROPERTIES_PROPERTY_VALUE)
                        {
                            cheque.PropertiesPropertyName = tag1084.Nested[1].ValueStr;
                            cheque.PropertiesPropertyValue = tag1084.Nested[0].ValueStr;
                        }
                        else
                        {
                            LogHandle.ol("Некорректный или неизвестный формат тега 1084 воспроизведение невозможно");
                        }
                    }
                    else if(tagNumber == FTAG_NDS20_DOCUMENT_SUM)
                    {
                        cheque.Nds20 = 0.01*fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if(tagNumber == FTAG_NDS10_DOCUMENT_SUM)
                    {
                        cheque.Nds10 = 0.01* fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if (tagNumber == FTAG_NDS0_DOCUMENT_SUM)
                    {
                        cheque.Nds0 = 0.01 * fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);   // в атол добавляется сумма налога, сумму без ндс на чек не нашел как
                    }
                    else if (tagNumber == FTAG_NDS_FREE_DOCUMENT_SUM)
                    {
                        cheque.NdsFree = 0.01 * fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);   // в атол добавляется сумма налога, сумму без ндс на чек не нашел как
                    }
                    else if (tagNumber == FTAG_NDS20120_DOCUMENT_SUM)
                    {
                        cheque.Nds20120 = 0.01 * fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if (tagNumber == FTAG_NDS10110_DOCUMENT_SUM)
                    {
                        cheque.Nds10110 = 0.01 * fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if (tagNumber == FTAG_INTERNET_PAYMENT)
                    {
                        cheque.InternetPayment = fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE) > 0;
                    }
                    else if (tagNumber == FTAG_CORRECTION_TYPE)
                    {
                        cheque.CorrectionTypeNotFtag = (int)fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if (tagNumber == FTAG_CORRECTION_BASE)
                    {
                        var correctionBase = ParceStlvCorrectionBase(fptr.getParamByteArray(Constants.LIBFPTR_PARAM_TAG_VALUE));
                        if (correctionBase.ContainsKey(FTAG_CORRECTION_DESCRIBER))
                            cheque.CorrectionDocDescriber = (string)correctionBase[FTAG_CORRECTION_DESCRIBER];
                        if (correctionBase.ContainsKey(FTAG_CORRECTION_DOC_DATE))
                            cheque.CorrectionDocumentDate = (DateTime)correctionBase[FTAG_CORRECTION_DOC_DATE];
                        if (correctionBase.ContainsKey(FTAG_CORRECTION_ORDER_NUMBER))
                            cheque.CorrectionOrderNumber = (string)correctionBase[FTAG_CORRECTION_ORDER_NUMBER];
                    }
                    else if (tagNumber == FTAG_RETAIL_PLACE)
                    {
                        cheque.RetailPlace = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if (tagNumber == FTAG_CASHIER_INN)
                    {
                        cheque.CashierInn = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if (tagNumber == FTAG_PREPAID_TOTAL_SUM)
                    {
                        cheque.Prepaid = 0.01 * fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if (tagNumber == FTAG_CREDIT_TOTAL_SUM)
                    {
                        cheque.Credit = 0.01 * fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if (tagNumber == FTAG_PROVISION_TOTAL_SUM)
                    {
                        cheque.Provision = 0.01 * fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if (tagNumber == FTAG_BUYER_INFORMATION_BUYER)
                    {
                        cheque.BuyerInformationBuyer = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if (tagNumber == FTAG_BUYER_INFORMATION_BUYER_INN)
                    {
                        cheque.BuyerInformationBuyerInn = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    }
                    else if (tagNumber == FTAG_BUYER_INFORMATION)
                    {

                        List<FTag> biList = FTag.FTLVParcer.ParseStructure(fptr.getParamByteArray(Constants.LIBFPTR_PARAM_TAG_VALUE));
                        foreach (FTag f in biList)
                        {
                            if (f.TagNumber == FTAG_BUYER_INFORMATION_BUYER)
                            {
                                cheque.BuyerInformationBuyer = f.ValueStr;
                            }
                            else if (f.TagNumber == FTAG_BUYER_INFORMATION_BUYER_INN)
                            {
                                cheque.BuyerInformationBuyerInn = f.ValueStr;
                            }
                            else if (f.TagNumber == FTAG_BI_ADDRESS)
                            {
                                cheque.BuyerInformationBuyerAddress = f.ValueStr;
                            }
                            else if (f.TagNumber == FTAG_BI_BIRTHDAY)
                            {
                                cheque.BuyerInformationBuyerBirthday = f.ValueStr;
                            }
                            else if (f.TagNumber == FTAG_BI_DOCUMENT_CODE)
                            {
                                cheque.BuyerInformationBuyerDocumentCode = f.ValueStr;
                            }
                            else if (f.TagNumber == FTAG_BI_DOCUMENT_DATA)
                            {
                                cheque.BuyerInformationBuyerDocumentData = f.ValueStr;
                            }
                            else if (f.TagNumber == FTAG_BI_CITIZENSHIP)
                            {
                                cheque.BuyerInformationBuyerCitizenship = f.ValueStr;
                            }
                        }
                    }
                    else if (tagNumber == FTAG_AMOUNTS_RECEIPT_NDS)
                    {
                        List<FTag> newTaxSumms = FTag.FTLVParcer.ParseStructure(fptr.getParamByteArray(Constants.LIBFPTR_PARAM_TAG_VALUE));
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
                                        cheque.Nds5 = taxAmount;
                                    else if (taxType == NDS_TYPE_7_LOC)
                                        cheque.Nds7 = taxAmount;
                                    else if (taxType == NDS_TYPE_5105_LOC)
                                        cheque.Nds5105 = taxAmount;
                                    else if (taxType == NDS_TYPE_7107_LOC)
                                        cheque.Nds7107 = taxAmount;
                                    else if (taxType == NDS_TYPE_22_LOC)
                                        cheque.Nds22 = taxAmount;
                                    else if (taxType == NDS_TYPE_22122_LOC)
                                        cheque.Nds22122 = taxAmount;
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
                    /*                  Строковое представление и  вывод в консоль документа
                     */
                    object tag= null;
                    if (tagType == Constants.LIBFPTR_TAG_TYPE_STLV)
                    {
                        tag = "Coplex tag here mast be parcing";
                        parceStlv(tagValue);
                    }
                    else if (tagType == Constants.LIBFPTR_TAG_TYPE_ARRAY)
                        tag = string.Concat(tagValue);
                    else if (tagType == Constants.LIBFPTR_TAG_TYPE_FVLN)
                        tag = fptr.getParamDouble(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    else if (tagType == Constants.LIBFPTR_TAG_TYPE_BITS ||
                        tagType == Constants.LIBFPTR_TAG_TYPE_BYTE ||
                        tagType == Constants.LIBFPTR_TAG_TYPE_UINT_16 ||
                        tagType == Constants.LIBFPTR_TAG_TYPE_UINT_32 ||
                        tagType == Constants.LIBFPTR_TAG_TYPE_VLN
                        )
                        tag = fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    else if (tagType == Constants.LIBFPTR_TAG_TYPE_STRING)
                        tag = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    else if (tagType == Constants.LIBFPTR_TAG_TYPE_UNIX_TIME)
                        tag = fptr.getParamDateTime(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    else
                        tag = fptr.getParamBool(Constants.LIBFPTR_PARAM_TAG_VALUE);

                    LogHandle.ol(String.Format("Tag {0} - {1} Value {2}", tagNumber, tagName, tag.ToString()));
                    fptr.setParam(Constants.LIBFPTR_PARAM_RECORDS_ID, recordsID);  
                }
                if (tegs == 0) cheque = null;
                if(!CorrectnessOfReading(cheque))
                {
                    if(effort == 0)
                    {
                        effort++;
                        LogHandle.ol("Документ не прочитался с первого раза ждем 150мс и повторяем чтение");
                        System.Threading.Thread.Sleep(150);
                        fptr.setParam(Constants.LIBFPTR_PARAM_DATA_TYPE, Constants.LIBFPTR_DT_STATUS);
                        int c = fptr.queryData();
                        LogHandle.ol("Проверка связи с устройством\t" + (NONE == c));
                        if (c == NONE)
                            goto AttempToRead;
                    }
                    if(effort == 1)
                    {
                        effort++;
                        LogHandle.ol("Документ не прочитался со второго раза ждем 200мс и повторяем чтение");
                        System.Threading.Thread.Sleep(200);
                        fptr.setParam(Constants.LIBFPTR_PARAM_DATA_TYPE, Constants.LIBFPTR_DT_STATUS);
                        int c = fptr.queryData();
                        LogHandle.ol("Проверка связи с устройством\t" + (NONE == c));
                        if (c == NONE)
                        {
                            fptr.setParam(Constants.LIBFPTR_PARAM_FN_DATA_TYPE, Constants.LIBFPTR_FNDT_FN_INFO);
                            c = fptr.fnQueryData();
                            LogHandle.ol("Запрос статуса ФН\t" + (NONE == c));
                            LogHandle.ol(fptr.getParamString(Constants.LIBFPTR_PARAM_SERIAL_NUMBER));
                            goto AttempToRead;
                        }
                    }
                    if(effort == 2)
                    {
                        effort++;
                        LogHandle.ol("Документ не прочитался с третьего раза ждем 250мс");
                        System.Threading.Thread.Sleep(250);
                        fptr.setParam(Constants.LIBFPTR_PARAM_DATA_TYPE, Constants.LIBFPTR_DT_STATUS);
                        int c = fptr.queryData();
                        LogHandle.ol("Проверка связи с устройством\t" + (NONE == c));
                        if (c == NONE)
                        {
                            LogHandle.ol("Read device condition");
                            ReadDeviceCondition();
                            goto AttempToRead;
                        }
                    }
                }
                if(cheque != null && AppSettings.AppendFiscalSignAsPropertyData )
                {
                    if (AppSettings.OverridePropertyData)
                        cheque.PropertiesData = docFiscalSign;
                    else
                    {
                        if (!cheque.IsPropertiesData)
                            cheque.PropertiesData = docFiscalSign;
                    }
                }
                if (cheque != null && (AppSettings.OverrideCorrectionDocumentDate || cheque.Document == FD_DOCUMENT_NAME_CHEQUE))
                {
                    cheque.CorrectionDocumentDate = dateTime;
                }
                if (cheque != null && AppSettings.OverrideCorrectionOrderNumber)
                {
                    cheque.CorrectionOrderNumber = AppSettings.CorrectionOrderNumberDefault;
                }

                FnReadedDocument fdReaded = new FnReadedDocument((int)documentType, dateTime, docNumber, tally, docFiscalSign, cheque);
                FTag root = new FTag((int)documentType, ftagList,false);
                List<FTag> rootList = new List<FTag>();
                rootList.Add(root);
                fdReaded.Tags = rootList;
                return fdReaded; //new FnReadedDocument((int)documentType, dateTime, docNumber, tally, docFiscalSign, cheque);
            }

            return new FnReadedDocument((int)documentType, dateTime, docNumber, tally, docFiscalSign);

        }

        private ConsumptionItem parceStlvItem(byte[] val)
        {
            string name = " ", 
                providerInn = null, 
                unitMeasure = null, 
                productCode = null,
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
            int paymetType = 0, productType = 0, ndsRate = 0, paymentAgentByProductType=0,measureType120 = -1;
            double price = -1, quantity = 0, itemSum = 0;
            fptr.setParam(Constants.LIBFPTR_PARAM_RECORDS_TYPE, Constants.LIBFPTR_RT_PARSE_COMPLEX_ATTR);
            fptr.setParam(Constants.LIBFPTR_PARAM_TAG_VALUE, val);
            fptr.beginReadRecords();
            String recordsID = fptr.getParamString(Constants.LIBFPTR_PARAM_RECORDS_ID);
            while (fptr.readNextRecord() == Constants.LIBFPTR_OK)
            {
                //byte[] tagValue = fptr.getParamByteArray(Constants.LIBFPTR_PARAM_TAG_VALUE);
                uint tagNumber = fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_NUMBER);
                if(tagNumber == FTAG_ITEM_NAME)
                {
                    name = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                }
                else if(tagNumber == FTAG_ITEM_PROVIDER_INN)
                {
                    providerInn = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                }
                else if(tagNumber == FTAG_ITEM_UNIT_MEASURE_105)
                {
                    unitMeasure = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                }
                else if(tagNumber == FTAG_ITEM_PRODUCT_CODE)
                {
                    productCode = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                }
                else if(tagNumber == FTAG_ITEM_PAYMENT_TYPE)
                {
                    paymetType = (int)fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                }
                else if(tagNumber == FTAG_ITEM_PRODUCT_TYPE)
                {
                    productType = (int)fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                }
                else if(tagNumber == FTAG_ITEM_NDS_RATE)
                {
                    ndsRate = (int)fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                }
                else if(tagNumber == FTAG_ITEM_PRICE)
                {
                    price = fptr.getParamDouble(Constants.LIBFPTR_PARAM_TAG_VALUE);
                }
                else if(tagNumber== FTAG_ITEM_QUANTITY)
                {
                    quantity = fptr.getParamDouble(Constants.LIBFPTR_PARAM_TAG_VALUE);
                }
                else if(tagNumber == FTAG_ITEM_SUM)
                {
                    itemSum = fptr.getParamDouble(Constants.LIBFPTR_PARAM_TAG_VALUE);
                }
                else if(tagNumber  == FTAG_ITEM_PAYMENT_AGENT_BY_PRODUCT_TYPE)
                {
                    paymentAgentByProductType = (int)fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                }
                else if(tagNumber == FTAG_ITEM_PAYMENT_AGENT_DATA)
                {
                    var padBytes = fptr.getParamByteArray(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    FTag pad = new FTag(FTAG_ITEM_PROVIDER_DATA, padBytes);
                    foreach(FTag f in pad.Nested)
                    {
                        if(f.TagNumber == FTAG_PAD_TRANSFER_OPERATOR_PHONE)
                        {
                            transferOperatorPhone = f.ValueStr;
                        }
                        else if(f.TagNumber == FTAG_PAD_PAYPENT_AGENT_OPERATION)
                        {
                            paymentAgentOperation = f.ValueStr;
                        }
                        else if(f.TagNumber == FTAG_PAD_PAYMENT_AGENT_PHONE)
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
                }
                else if (tagNumber == FTAG_ITEM_PROVIDER_DATA)
                {
                    var padBytes = fptr.getParamByteArray(Constants.LIBFPTR_PARAM_TAG_VALUE);
                    FTag pd = new FTag(FTAG_ITEM_PROVIDER_DATA, padBytes);
                    foreach (FTag f in pd.Nested)
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
                    
                }
                else if (tagNumber == FTAG_ITEM_CUSTOM_ENTRY_NUM)
                {
                    customEntryNum = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                }
                else if (tagNumber == FTAG_ITEM_ORIGINAL_COUNTRY_CODE)
                {
                    originalCountryCode = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                }
                else if (tagNumber == FTAG_ITEM_UNIT_MEASURE_120)
                {
                    measureType120 = (int)fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                }
                /*  Строковое представление предмета расчета
                 * 
                 * String tagName = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_NAME);
                uint tagType = fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_TYPE);
                object tag = null;
                if (tagType == Constants.LIBFPTR_TAG_TYPE_STLV)
                    tag = "Coplex tag here mast be parcing";
                else if (tagType == Constants.LIBFPTR_TAG_TYPE_ARRAY)
                    tag = string.Concat(tagValue);
                else if (tagType == Constants.LIBFPTR_TAG_TYPE_FVLN)
                    tag = fptr.getParamDouble(Constants.LIBFPTR_PARAM_TAG_VALUE);
                else if (tagType == Constants.LIBFPTR_TAG_TYPE_BITS ||
                    tagType == Constants.LIBFPTR_TAG_TYPE_BYTE ||
                    tagType == Constants.LIBFPTR_TAG_TYPE_UINT_16 ||
                    tagType == Constants.LIBFPTR_TAG_TYPE_UINT_32 ||
                    tagType == Constants.LIBFPTR_TAG_TYPE_VLN
                    )
                    tag = fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                else if (tagType == Constants.LIBFPTR_TAG_TYPE_STRING)
                    tag = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                else if (tagType == Constants.LIBFPTR_TAG_TYPE_UNIX_TIME)
                    tag = fptr.getParamDateTime(Constants.LIBFPTR_PARAM_TAG_VALUE);
                else
                    tag = fptr.getParamBool(Constants.LIBFPTR_PARAM_TAG_VALUE);

                Debug.WriteLine("{4} Tag {0} Name {1} Type {2} Value {3}", tagNumber, tagName, tagType, tag.ToString(), recordsID);*/
                fptr.setParam(Constants.LIBFPTR_PARAM_RECORDS_ID, recordsID);
            }
            if (paymetType<=0&& AppSettings.AtolFillItemsPaymentTypeDefault)
            {
                paymetType = 4;
            }

            ConsumptionItem item = new ConsumptionItem(name, price, quantity, itemSum, productType, paymetType, ndsRate);
            if(!string.IsNullOrEmpty(unitMeasure))
                item.Unit105 = unitMeasure;
            if(!string.IsNullOrEmpty(customEntryNum))
                item.CustomEntryNum = customEntryNum;
            if(!string.IsNullOrEmpty(providerInn))
                item.ProviderInn = providerInn;
            if(!string.IsNullOrEmpty(originalCountryCode))
                item.OriginalCountryCode = originalCountryCode;
            if(measureType120>-1)
                item.Unit120 = measureType120;
            item.PaymentAgentByProductType = paymentAgentByProductType;
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
            item.Control();
            return item;
        }

        private Dictionary<int, object> ParceStlvCorrectionBase(byte[] val)
        {
            Dictionary<int, object> retval = new Dictionary<int, object>();
            fptr.setParam(Constants.LIBFPTR_PARAM_RECORDS_TYPE, Constants.LIBFPTR_RT_PARSE_COMPLEX_ATTR);
            fptr.setParam(Constants.LIBFPTR_PARAM_TAG_VALUE, val);
            fptr.beginReadRecords();
            String recordsID = fptr.getParamString(Constants.LIBFPTR_PARAM_RECORDS_ID);
            while (fptr.readNextRecord() == Constants.LIBFPTR_OK)
            {
                uint tagNumber = fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_NUMBER);
                
                if (tagNumber == FTAG_CORRECTION_DESCRIBER)
                    retval[FTAG_CORRECTION_DESCRIBER] = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                else if (tagNumber == FTAG_CORRECTION_DOC_DATE)
                    retval[FTAG_CORRECTION_DOC_DATE] = fptr.getParamDateTime(Constants.LIBFPTR_PARAM_TAG_VALUE);
                else if (tagNumber == FTAG_CORRECTION_ORDER_NUMBER)
                    retval[FTAG_CORRECTION_ORDER_NUMBER] = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                fptr.setParam(Constants.LIBFPTR_PARAM_RECORDS_ID, recordsID);
            }
            return retval;
        }

        private void parceStlv(byte[] val)
        {
            fptr.setParam(Constants.LIBFPTR_PARAM_RECORDS_TYPE, Constants.LIBFPTR_RT_PARSE_COMPLEX_ATTR);
            fptr.setParam(Constants.LIBFPTR_PARAM_TAG_VALUE, val);
            fptr.beginReadRecords();
            String recordsID = fptr.getParamString(Constants.LIBFPTR_PARAM_RECORDS_ID);
            while (fptr.readNextRecord() == Constants.LIBFPTR_OK)
            {
                byte[] tagValue = fptr.getParamByteArray(Constants.LIBFPTR_PARAM_TAG_VALUE);
                uint tagNumber = fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_NUMBER);
                String tagName = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_NAME);
                uint tagType = fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_TYPE);
                object tag = null;
                if (tagType == Constants.LIBFPTR_TAG_TYPE_STLV)
                    tag = "Coplex tag here mast be parcing"; 
                else if (tagType == Constants.LIBFPTR_TAG_TYPE_ARRAY)
                    tag = string.Concat(tagValue);
                else if (tagType == Constants.LIBFPTR_TAG_TYPE_FVLN)
                    tag = fptr.getParamDouble(Constants.LIBFPTR_PARAM_TAG_VALUE);
                else if (tagType == Constants.LIBFPTR_TAG_TYPE_BITS ||
                    tagType == Constants.LIBFPTR_TAG_TYPE_BYTE ||
                    tagType == Constants.LIBFPTR_TAG_TYPE_UINT_16 ||
                    tagType == Constants.LIBFPTR_TAG_TYPE_UINT_32 ||
                    tagType == Constants.LIBFPTR_TAG_TYPE_VLN
                    )
                    tag = fptr.getParamInt(Constants.LIBFPTR_PARAM_TAG_VALUE);
                else if (tagType == Constants.LIBFPTR_TAG_TYPE_STRING)
                    tag = fptr.getParamString(Constants.LIBFPTR_PARAM_TAG_VALUE);
                else if (tagType == Constants.LIBFPTR_TAG_TYPE_UNIX_TIME)
                    tag = fptr.getParamDateTime(Constants.LIBFPTR_PARAM_TAG_VALUE);
                else
                    tag = fptr.getParamBool(Constants.LIBFPTR_PARAM_TAG_VALUE);

                Debug.WriteLine(" Tag {0} Name {1} Type {2} Value {3}", tagNumber, tagName, tagType, tag.ToString());
                fptr.setParam(Constants.LIBFPTR_PARAM_RECORDS_ID, recordsID);
            }


        }


        public override bool ContinuePrint()
        {
            if(fptr!=null)
            {
                if(fptr.continuePrint()!=0)
                {
                    RezultMsg(fptr.errorDescription());
                    return false;
                }
                RezultMsg(SUCCESS_MSG);
                return true;
            }
            RezultMsg(NO_DRIVER_FOUNDED);
            return false;
        }

        public override bool CancelDocument()
        {
            if(fptr!= null && _connected)
            {
                int  r = fptr.cancelReceipt();
                LogHandle.ol("Отменяем документ");
                RezultMsg(fptr.errorDescription());
                return r == NONE;
            }
            RezultMsg(CONNECTION_NOT_ESTABLISHED);
            return false;
        }

        public override bool ChangeDate(int appendDay = 0, DateTime date = default)
        {
            if (appendDay == 0)
            {
                LogHandle.ol("Установка даты "+date);
                fptr.setParam(Constants.LIBFPTR_PARAM_DATE_TIME, date);
                return (fptr.writeDateTime() == NONE);
            }
            if (appendDay > 0)
            {
                LogHandle.ol("добавляем дни к дате "+appendDay);
                fptr.setParam(Constants.LIBFPTR_PARAM_DATA_TYPE, Constants.LIBFPTR_DT_DATE_TIME);
                fptr.queryData();
                fptr.setParam(Constants.LIBFPTR_PARAM_DATE_TIME, fptr.getParamDateTime(Constants.LIBFPTR_PARAM_DATE_TIME).AddDays(appendDay));
                return fptr.writeDateTime() == NONE;
            }
            return false;
        }

        public override bool CashRefill(double sum, bool income = true)
        {
            if (_dontPrint)
            {
                fptr.setParam(65902, true); // Constants.LIBFPTR_PARAM_DOCUMENT_ELECTRONICALLY  почему то не во всех библиотеках есть эта константа
            }
            fptr.setParam(Constants.LIBFPTR_PARAM_SUM, sum);
            int rezult = income ? fptr.cashIncome() : fptr.cashOutcome();
            RezultMsg(fptr.errorDescription());
            return rezult == 0;
        }
    }
}
