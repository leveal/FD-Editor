using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FR_Operator.FiscalPrinter;

namespace FR_Operator
{
    public partial class FormImport: Form
    {
        public FormImport()
        {
            InitializeComponent();
            textBox_taskPath.Text = Path.Combine(System.Windows.Forms.Application.StartupPath, "task");

            //comboBox_checkLinkRowIndex.SelectedIndex = 33;


            textBox_readFromStr.Text = CurrentRow.ToString();
            _skipHandle = true;
            if (index_buyerName > 0)
            {
                comboBox_indBuyerName.SelectedIndex = index_buyerName - 1;
            }
            if (index_buyerInn > 0)
            {
                comboBox_indBuyerInn.SelectedIndex = index_buyerInn - 1;
            }
            if (index_buyerBirthDate > 0)
            {
                comboBox_indBuyerBirthDate.SelectedIndex = index_buyerBirthDate - 1;
            }
            if (index_buyerCitizenship > 0)
            {
                comboBox_indBuyerCitizenship.SelectedIndex = index_buyerCitizenship - 1;
            }
            if (index_buyerDocumentCode > 0)
            {
                comboBox_indBuyerDocumentCode.SelectedIndex = index_buyerDocumentCode - 1;
            }
            if (index_buyerDocumentData > 0)
            {
                comboBox_indBuyerDocData.SelectedIndex = index_buyerDocumentData - 1;
            }
            if (index_buyerAddress > 0)
            {
                comboBox_indBuyerAddress.SelectedIndex = index_buyerAddress - 1;
            }
            checkBox_changeProductType2to26.Checked = ChangeServiceToPaymentCasino;
            checkBox_requirementData.Checked = RequirementData;
            _skipHandle = false;
        }

        public static object[,] table = null;
        public static int _ind_checkLinkRow = -1;

        public static int index_buyerName = -1;
        public static int index_buyerInn = -1;
        public static int index_buyerBirthDate = -1;
        public static int index_buyerCitizenship = -1;
        public static int index_buyerDocumentCode = -1;
        public static int index_buyerDocumentData = -1;
        public static int index_buyerAddress = -1;
        public string excFile = "";

        public static int CurrentRow = 2;

        private bool _brakeLoading = false;
        public static bool ChangeServiceToPaymentCasino = true;
        public static bool Change1ToPaymentCasino = true;
        public static bool RequirementData = true;


        bool _skipHandle = false;
        private void ControlsActionHandler(object sender, EventArgs e)
        {
            if (_skipHandle)
                return;
            if (sender == checkBox_requirementData)
            {
                RequirementData = checkBox_requirementData.Checked;
                AddMessage("RequirementData: " + RequirementData);
            }
            else if (sender == button_openExcel)
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|All files (*.*)|*.*";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    excFile = fileDialog.FileName;
                    object[,] valueArray = null;
                    // тут добавить вариант чтения с другой библиотекой 
                    // возможно стоит чтение в отдельный поток перевести или диалог с прогрес баром выбросить
                    Microsoft.Office.Interop.Excel.Application _excelApp = null;
                    Microsoft.Office.Interop.Excel.Workbook workbook = null;
                    try
                    {
                        _excelApp = new Microsoft.Office.Interop.Excel.Application();
                        _excelApp.Visible = false;

                        string fileName = fileDialog.FileName;

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

                        //get an object array of all of the cells in the worksheet (their values)
                        //var vals = /*(object[,])*/excelRange.get_Value(
                        //            Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueMSPersistXML);

                        valueArray = (object[,])excelRange.get_Value(
                                    Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault);

                        AddMessage(valueArray.GetUpperBound(0).ToString() + " строк в документе.");
                        table = valueArray;

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
                }



            }
            else if (sender == comboBox_checkLinkRowIndex)
            {
                _ind_checkLinkRow = comboBox_checkLinkRowIndex.SelectedIndex + 1;
                // перепроверка индекса
                int hi = _ind_checkLinkRow / 26;
                int li = _ind_checkLinkRow % 26;
                string s = "";
                if (hi > 0) s += 'A';
                s += (char)('A' + li - 1);
                //AddMessage("\r\nСсылка на чек выбран столбец: " + s);
            }
            else if (sender == comboBox_indBuyerName)
            {
                index_buyerName = comboBox_indBuyerName.SelectedIndex + 1;
                // перепроверка индекса
                int hi = index_buyerName / 26;
                int li = index_buyerName % 26;
                string s = "";
                if (hi > 0) s += 'A';
                s += (char)('A' + li - 1);
                AddMessage("\r\nФИО покупателя выбран столбец: " + s);
                if (table != null)
                {
                    try
                    {
                        int values = 0;
                        int rows = table.GetUpperBound(0);
                        for (int i = CurrentRow; i <= rows; i++)
                        {
                            if (table[i, index_buyerName] != null && table[i, index_buyerName].ToString() != "")
                                values++;
                        }
                        AddMessage(" обнаружено "+values +" значений");
                    }
                    catch(Exception exc)
                    {
                        AddMessage(exc.Message);
                        index_buyerName = -1;
                        AddMessage("Некорректная настройка выберите другое значение");
                    }
                }
                else
                {
                    AddMessage("Таблица не открыта, подсчет невозможен");
                }
            }
            else if (sender == comboBox_indBuyerInn)
            {
                index_buyerInn = comboBox_indBuyerInn.SelectedIndex + 1;
                // перепроверка индекса
                int hi = index_buyerInn / 26;
                int li = index_buyerInn % 26;
                string s = "";
                if (hi > 0) s += 'A';
                s += (char)('A' + li - 1);
                AddMessage("\r\nИНН покупателя выбран столбец: " + s);
                if (table != null)
                {
                    try
                    {
                        int values = 0;
                        int rows = table.GetUpperBound(0);
                        for (int i = CurrentRow; i <= rows; i++)
                        {
                            if (table[i, index_buyerInn] != null && table[i, index_buyerInn].ToString() != "")
                                values++;
                        }
                        AddMessage(" обнаружено " + values + " значений");
                    }
                    catch (Exception exc)
                    {
                        AddMessage(exc.Message);
                        index_buyerInn = -1;
                        AddMessage("Некорректная настройка выберите другое значение");
                    }
                }
                else
                {
                    AddMessage("Таблица не открыта, подсчет невозможен");
                }


            }
            else if (sender == comboBox_indBuyerBirthDate)
            {
                index_buyerBirthDate = comboBox_indBuyerBirthDate.SelectedIndex + 1;
                // перепроверка индекса
                int hi = index_buyerBirthDate / 26;
                int li = index_buyerBirthDate % 26;
                string s = "";
                if (hi > 0) s += 'A';
                s += (char)('A' + li - 1);
                AddMessage("\r\nДР покупателя выбран столбец: " + s);
                if (table != null)
                {
                    try
                    {
                        int values = 0;
                        int rows = table.GetUpperBound(0);
                        for (int i = CurrentRow; i <= rows; i++)
                        {
                            if (table[i, index_buyerBirthDate] != null && table[i, index_buyerBirthDate].ToString() != "")
                                values++;
                        }
                        AddMessage(" обнаружено " + values + " значений");
                    }
                    catch (Exception exc)
                    {
                        AddMessage(exc.Message);
                        index_buyerBirthDate = -1;
                        AddMessage("Некорректная настройка выберите другое значение");
                    }
                }
                else
                {
                    AddMessage("Таблица не открыта, подсчет невозможен");
                }
            }
            else if (sender == comboBox_indBuyerCitizenship)
            {
                index_buyerCitizenship = comboBox_indBuyerCitizenship.SelectedIndex + 1;
                // перепроверка индекса
                int hi = index_buyerCitizenship / 26;
                int li = index_buyerCitizenship % 26;
                string s = "";
                if (hi > 0) s += 'A';
                s += (char)('A' + li - 1);
                AddMessage("\r\nГражданство покупателя выбран столбец: " + s);
                if (table != null)
                {
                    try
                    {
                        int values = 0;
                        int rows = table.GetUpperBound(0);
                        for (int i = CurrentRow; i <= rows; i++)
                        {
                            if (table[i, index_buyerCitizenship] != null && table[i, index_buyerCitizenship].ToString() != "")
                                values++;
                        }
                        AddMessage(" обнаружено " + values + " значений");
                    }
                    catch (Exception exc)
                    {
                        AddMessage(exc.Message);
                        index_buyerCitizenship = -1;
                        AddMessage("Некорректная настройка выберите другое значение");
                    }
                }
                else
                {
                    AddMessage("Таблица не открыта, подсчет невозможен");
                }
            }
            else if (sender == comboBox_indBuyerDocumentCode)
            {
                index_buyerDocumentCode = comboBox_indBuyerDocumentCode.SelectedIndex + 1;
                // перепроверка индекса
                int hi = index_buyerDocumentCode / 26;
                int li = index_buyerDocumentCode % 26;
                string s = "";
                if (hi > 0) s += 'A';
                s += (char)('A' + li - 1);
                AddMessage("\r\nКод документа покупателя выбран столбец: " + s);

                if (table != null)
                {
                    try
                    {
                        int values = 0;
                        int rows = table.GetUpperBound(0);
                        for (int i = CurrentRow; i <= rows; i++)
                        {
                            if (table[i, index_buyerDocumentCode] != null && table[i, index_buyerDocumentCode].ToString() != "")
                                values++;
                        }
                        AddMessage(" обнаружено " + values + " значений");
                    }
                    catch (Exception exc)
                    {
                        AddMessage(exc.Message);
                        index_buyerDocumentCode = -1;
                        AddMessage("Некорректная настройка выберите другое значение");
                    }
                }
                else
                {
                    AddMessage("Таблица не открыта, подсчет невозможен");
                }

            }
            else if (sender == comboBox_indBuyerDocData)
            {
                index_buyerDocumentData = comboBox_indBuyerDocData.SelectedIndex + 1;
                // перепроверка индекса
                int hi = index_buyerDocumentData / 26;
                int li = index_buyerDocumentData % 26;
                string s = "";
                if (hi > 0) s += 'A';
                s += (char)('A' + li - 1);
                AddMessage("\r\nДанные документа покупателя выбран столбец: " + s);

                if (table != null)
                {
                    try
                    {
                        int values = 0;
                        int rows = table.GetUpperBound(0);
                        for (int i = CurrentRow; i <= rows; i++)
                        {
                            if (table[i, index_buyerDocumentData] != null && table[i, index_buyerDocumentData].ToString() != "")
                                values++;
                        }
                        AddMessage(" обнаружено " + values + " значений");
                    }
                    catch (Exception exc)
                    {
                        AddMessage(exc.Message);
                        index_buyerDocumentData = -1;
                        AddMessage("Некорректная настройка выберите другое значение");
                    }
                }
                else
                {
                    AddMessage("Таблица не открыта, подсчет невозможен");
                }
            }
            else if (sender == comboBox_indBuyerAddress)
            {
                index_buyerAddress = comboBox_indBuyerAddress.SelectedIndex + 1;
                // перепроверка индекса
                int hi = index_buyerAddress / 26;
                int li = index_buyerAddress % 26;
                string s = "";
                if (hi > 0) s += 'A';
                s += (char)('A' + li - 1);
                AddMessage("\r\nАдрес покупателя на чек выбран столбец: " + s);

                if (table != null)
                {
                    try
                    {
                        int values = 0;
                        int rows = table.GetUpperBound(0);
                        for (int i = CurrentRow; i <= rows; i++)
                        {
                            if (table[i, index_buyerAddress] != null && table[i, index_buyerAddress].ToString() != "")
                                values++;
                        }
                        AddMessage(" обнаружено " + values + " значений");
                    }
                    catch (Exception exc)
                    {
                        AddMessage(exc.Message);
                        index_buyerAddress = -1;
                        AddMessage("Некорректная настройка выберите другое значение");
                    }
                }
                else
                {
                    AddMessage("Таблица не открыта, подсчет невозможен");
                }

            }
            else if (sender == button_selectFolder)
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.SelectedPath = textBox_taskPath.Text;//Application.StartupPath;// AppDomain.CurrentDomain.BaseDirectory;

                    DialogResult result = dialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        textBox_taskPath.Text = dialog.SelectedPath;
                    }

                }
            }
            else if (sender == button_brake)
            {
                _brakeLoading = true;
            }
            else if(sender  == textBox_readFromStr)
            {
                int t = 2;
                bool r = int.TryParse(textBox_readFromStr.Text, out t);
                if( (t == 2 && !r ) || t <= 0)
                {
                    textBox_readFromStr.ForeColor = Color.Red;
                    CurrentRow = 2;
                    AddMessage("Не удалось распознать первую строку значение установлено 2");
                }
                else
                {
                    textBox_readFromStr.ForeColor = Color.Black;
                    CurrentRow = t;
                    textBox_readFromStr.Text =  CurrentRow.ToString();
                    AddMessage("Начинаем разбирать со строки "+CurrentRow);
                }
                
            }
            else if (sender == checkBox_changeProductType2to26)
            {
                ChangeServiceToPaymentCasino = checkBox_changeProductType2to26.Checked;
            }
            else if (sender == checkBox_changeProductType1to26)
            {
                Change1ToPaymentCasino = checkBox_changeProductType1to26.Checked;
            }

        }


        static int counter = 0;

        void AddMessage(string msg)
        {
            LogHandle.ol(msg);
            if (InvokeRequired)
            {
                BeginInvoke(new System.Action(() =>
                {
                    if (counter % 100 == 50)
                    {
                        if (richTextBox1.Text.Length > 2096)
                        {
                            richTextBox1.Text = "Текстовое поле очищено пред. данные доступны в файле LoadChecks.log" + Environment.NewLine;
                        }
                    }
                    richTextBox1.AppendText(msg + Environment.NewLine);
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                }));
            }
            else
            {
                if (counter % 100 == 50)
                {
                    if (richTextBox1.Text.Length > 2096)
                    {
                        richTextBox1.Text = "Текстовое поле очищено пред. данные доступны в файле LoadChecks.log" + Environment.NewLine;
                    }
                }
                richTextBox1.AppendText(msg + Environment.NewLine);
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
        }

    }
}
