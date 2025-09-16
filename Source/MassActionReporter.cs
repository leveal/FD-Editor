using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FR_Operator.FiscalPrinter;

namespace FR_Operator
{
    internal class MassActionReporter
    {
        const string SKIP_DOCUMENT = "действия не требуются";
        public MassActionReporter(int quantity)
        {
            this._reportFileName = "report_"+DateTime.Now.ToString(FiscalPrinter.DEFAULT_DT_FORMAT_FILENAME)+".txt";
            LogHandle.ol("Начинаем операцию корректировки");
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _reportFileName), string.Format("В обработке {0} документов {1}------------------------------------------------------------------------------------{2}",quantity, Environment.NewLine, Environment.NewLine));
            _failedSt1 = new List<int>();
            _failedSt2 = new List<int>();
            _failedSt3 = new List<int>();
            _warnList = new List<int>();
            this._quantity = quantity;
            _errorCounter = 0;
        }




        private string _reportFileName = null; 
        private int _quantity = 0,_originalFdNum = -1;
        List<int> _failedSt1;           // чтение
        List<int> _failedSt2;           // отмена операции
        List<int> _failedSt3;           // прямая
        List<int> _warnList;

        public void ReadDoc(int fd,FiscalCheque cheque)
        {
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _reportFileName), Environment.NewLine+fd + " " + (cheque!=null?cheque.ToString(FiscalCheque.SHORT_INFO):"")+"-------->");
            _originalFdNum = fd;
        }
        public void ReadFaled()
        {
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _reportFileName), "Не удалось прочитать документ"+Environment.NewLine);
            _failedSt1.Add(_originalFdNum);
            _errorCounter++;
        }
        public void Step2Failed()
        {
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _reportFileName), "Не удалось провесьти вторую операцию" + Environment.NewLine);
            _failedSt2.Add(_originalFdNum);
            _errorCounter++;
        }
        public void Step3Failed()
        {
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _reportFileName), "Не удалось провесьти третью операцию" + Environment.NewLine);
            _failedSt2.Add(_originalFdNum);
            _errorCounter++;
        }
        public void WarnDoc()
        {
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _reportFileName), " *необходима проверка* ");
            _warnList.Add(_originalFdNum);
            _errorCounter++;
        }
        public void ProcessingStepOne(FiscalCheque cheque, int fdNum)
        {
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _reportFileName), fdNum + " " + cheque.ToString(FiscalCheque.SHORT_INFO));
        }
        public void ProcessingStepTwo(FiscalCheque cheque, int fdNum)
        {
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _reportFileName), "-------->"+fdNum + " " + cheque.ToString(FiscalCheque.SHORT_INFO));
        }
        public void SkipDocument()
        {
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _reportFileName), "");
        }
        public void OperationOK()
        {
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _reportFileName), " - ok");
        }

        public void ErrorsListOutput()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Список ошибок при обработке документов");
            sb.AppendLine();
            sb.AppendLine("Ошибка чтения документа (Операция не выполнена)");
            foreach(int i in _failedSt1)
            {
                sb.Append(i + ",");
            }
            sb.AppendLine();
            sb.AppendLine("Ошибка при оформлении второй операции + пропуск третьей");
            foreach (int i in _failedSt2)
            {
                sb.Append(i + ",");
            }
            sb.AppendLine();
            sb.AppendLine("Ошибка при оформлении третьей операции");
            foreach (int i in _failedSt3)
            {
                sb.Append(i + ",");
            }
            sb.AppendLine();
            sb.AppendLine("Возможно ошибки в документах");
            foreach (int i in _warnList)
            {
                sb.Append(i + ",");
            }
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _reportFileName), sb.ToString());
            _errorCounter = 0;
        }

        private static int _errorCounter = 0;
        public static int ErrorCounter
        {
            get { return _errorCounter; }
        }


        //public static void IcrementErrCounter() { _errorCounter++; }
        public static void InitializeT2Vls(bool pf = false)
        {
            
            if (pf)
            {
                string head = "Расшифровка корректировки документов" + Environment.NewLine + "------------------------------------------------------------------------------------------------------------" + Environment.NewLine+
                    "При чтении не удалось прочитать "+_errorCounter+" документов - информация в " + _task2RepotFileNameReadedFdList+Environment.NewLine+
                    Environment.NewLine;
                _task2RepotFileNameReadedFdList = "Operations_" + DateTime.Now.ToString(FiscalPrinter.DEFAULT_DT_FORMAT_FILENAME) + "_correctind_fd_list.txt";
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _task2RepotFileNameReadedFdList), head);
                _errorCounter = 0;
            }
            else
            {
                _errorCounter = 0;
                _task2RepotFileNameReadedFdList = "Operations_" + DateTime.Now.ToString(FiscalPrinter.DEFAULT_DT_FORMAT_FILENAME) + "_readed_fd_list.txt";
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _task2RepotFileNameReadedFdList), "Начинаем операцию корректировки! В этом файле будет список считанных документов" + Environment.NewLine + "------------------------------------------------------------------------------------------------------------" + Environment.NewLine);
            }
        }



        public static void AppendReadedFD(FnReadedDocument frd)
        {
            if(frd.Cheque == null)
            {
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _task2RepotFileNameReadedFdList), frd.ReeprezentOL);
                if (frd.Type == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CHEQUE 
                    || frd.Type == FTAG_FISCAL_DOCUMENT_TYPE_RECEIPT_CORRECTION_CHEQUE)
                {
                    _errorCounter++;
                    File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _task2RepotFileNameReadedFdList), "\t\t" + "считать не удалось");
                }
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _task2RepotFileNameReadedFdList),Environment.NewLine);
            }
            else
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _task2RepotFileNameReadedFdList), frd.ReeprezentOL + "\t\t" + frd.Cheque.ToString(FiscalCheque.SHORT_INFO) + Environment.NewLine);
        }
        public static void AppendCorrFd(FiscalCheque cheque, bool rezultOperation)
        {
            if (cheque == null)
            {
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _task2RepotFileNameReadedFdList), "\tпустой документ(пропуск операции)" + Environment.NewLine);
                //_errorCounter++;
            }
            else
            {
                //string line = "\t" + cheque.ToString(FiscalCheque.SHORT_INFO) + (rezultOperation?"\tOK": "\t!!!ОШИБКА ОПЕРАЦИИ!!! - " + KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY])+Environment.NewLine;
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _task2RepotFileNameReadedFdList), "\t" + cheque.ToString(FiscalCheque.SHORT_INFO) + (rezultOperation ? "\tOK" : "\t!!!ОШИБКА ОПЕРАЦИИ!!! - " + KKMInfoTransmitter[FR_LAST_ERROR_MSG_KEY]) + Environment.NewLine);
                if (!rezultOperation) _errorCounter++;
            }
        }


        public static void SummingUpReadRezults()
        {
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _task2RepotFileNameReadedFdList), Environment.NewLine+Environment.NewLine + "Всего ошибок "+_errorCounter);
        }

        private static string _task2RepotFileNameReadedFdList = "";


        /*  для обраьотки эксель олап
         *  результат фискальной операции будет добавляться методом AppendCorrFd
         *  public static void AppendCorrFd(FiscalCheque cheque, bool rezultOperation)
         *  для расширенной записи лога в ExcelLine нужно передавать строку
        */
        public MassActionReporter(ref object[,] task) 
        {
            _reportFileName = "Excel_task_" + DateTime.Now.ToString(FiscalPrinter.DEFAULT_DT_FORMAT_FILENAME) + ".txt";
            _task2RepotFileNameReadedFdList = _reportFileName;
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _reportFileName), "Корректировка из Excel файла " + Environment.NewLine + task.GetUpperBound(0) + " строк в документе" + Environment.NewLine + Environment.NewLine);
        }

        public void ExcelLine(int lineNumber, List<object> line = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(lineNumber.ToString());
            sb.Append('\t');
            if (line != null)
            {
                foreach (object o in line)
                {
                    if (o == null)
                    {
                        sb.Append("null\t");
                    }
                    else
                    {
                        sb.Append(o.ToString());
                    }
                }
            }
            sb.Append(Environment.NewLine);
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _reportFileName),  sb.ToString());
        }

        public void ExcelExtraLine(string str)
        {
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _reportFileName), str + Environment.NewLine);
        }


    }
}
