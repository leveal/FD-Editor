using FR_Operator.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FR_Operator
{
    public partial class BuyerInformationWindow : Form
    {
        public BuyerInformationWindow()
        {
            _skipListener = true;
            InitializeComponent();
            this.Icon = Resources.fd_editpr_16_2;
        }
        internal void AddFormDoc(FiscalCheque doc)
        {
            this.doc = doc;
            checkBox_alwaysSendBuyerData.Checked = AppSettings.AlwaysSendBuyerDocData;
            if (doc != null && doc.BuyerInformation)
            {
                if (!string.IsNullOrEmpty(doc.BuyerInformationBuyer))
                {
                    textBox_BuyerInformationBuyer.Text = doc.BuyerInformationBuyer;
                }
                if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerInn))
                {
                    textBox_BuyerInformationBuyerInn.Text = doc.BuyerInformationBuyerInn;
                }
                if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerBirthday))
                {
                    textBox_BuyerInformationBuyerBirthday.Text = doc.BuyerInformationBuyerBirthday;
                }
                if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerCitizenship))
                {
                    textBox_BuyerInformationBuyerCitizenship.Text = doc.BuyerInformationBuyerCitizenship;
                }
                if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerDocumentCode))
                {
                    if (string.IsNullOrEmpty(doc.BuyerInformationBuyerDocumentCode))
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 0;
                    else if (doc.BuyerInformationBuyerDocumentCode == "21")
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 1;
                    else if (doc.BuyerInformationBuyerDocumentCode == "22")
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 2;
                    else if (doc.BuyerInformationBuyerDocumentCode == "26")
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 3;
                    else if (doc.BuyerInformationBuyerDocumentCode == "27")
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 4;
                    else if (doc.BuyerInformationBuyerDocumentCode == "28")
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 5;
                    else if (doc.BuyerInformationBuyerDocumentCode == "31")
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 6;
                    else if (doc.BuyerInformationBuyerDocumentCode == "32")
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 7;
                    else if (doc.BuyerInformationBuyerDocumentCode == "33")
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 8;
                    else if (doc.BuyerInformationBuyerDocumentCode == "34")
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 9;
                    else if (doc.BuyerInformationBuyerDocumentCode == "35")
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 10;
                    else if (doc.BuyerInformationBuyerDocumentCode == "36")
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 11;
                    else if (doc.BuyerInformationBuyerDocumentCode == "37")
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 12;
                    else if (doc.BuyerInformationBuyerDocumentCode == "38")
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 13;
                    else if (doc.BuyerInformationBuyerDocumentCode == "40")
                        comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex = 14;
                }
                if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerDocumentData))
                {
                    textBox_BuyerInformationBuyerDocumentData.Text = doc.BuyerInformationBuyerDocumentData;
                }
                if (!string.IsNullOrEmpty(doc.BuyerInformationBuyerAddress))
                {
                    textBox_BuyerInformationBuyerAddress.Text = doc.BuyerInformationBuyerAddress;
                }
            }
            _skipListener = false;
        }
        FiscalCheque doc;

        private void BuyerInformation_fieldChanged(object sender, EventArgs e)
        {
            if(_skipListener)return;
            if(sender == textBox_BuyerInformationBuyer)
            {
                doc.BuyerInformationBuyer = textBox_BuyerInformationBuyer.Text;
            }
            else if (sender == textBox_BuyerInformationBuyerInn)
            {
                doc.BuyerInformationBuyerInn = textBox_BuyerInformationBuyerInn.Text;
                if (string.IsNullOrEmpty(doc.BuyerInformationBuyerInn))
                {
                    textBox_BuyerInformationBuyerInn.ForeColor = Color.Red;
                }
                else
                {
                    textBox_BuyerInformationBuyerInn.ForeColor=Color.Black;
                }
                
            }
            else if (sender == textBox_BuyerInformationBuyerBirthday)
            {
                doc.BuyerInformationBuyerBirthday = textBox_BuyerInformationBuyerBirthday.Text;
                if (string.IsNullOrEmpty(doc.BuyerInformationBuyerBirthday))
                {
                    textBox_BuyerInformationBuyerBirthday.ForeColor = Color.Red;
                }
                else
                {
                    textBox_BuyerInformationBuyerBirthday.ForeColor = Color.Black;
                }
            }
            else if (sender == textBox_BuyerInformationBuyerCitizenship)
            {
                doc.BuyerInformationBuyerCitizenship = textBox_BuyerInformationBuyerCitizenship.Text;
                if (string.IsNullOrEmpty(doc.BuyerInformationBuyerCitizenship))
                {
                    textBox_BuyerInformationBuyerCitizenship.ForeColor = Color.Red;
                }
                else
                {
                    textBox_BuyerInformationBuyerCitizenship.ForeColor = Color.Black;
                }
            }
            else if (sender == comboBox_BuyerInformationBuyerDocumentCode)
            {
                if (comboBox_BuyerInformationBuyerDocumentCode.SelectedIndex >= 0)
                {
                    if (comboBox_BuyerInformationBuyerDocumentCode.Text.Length > 2)
                        doc.BuyerInformationBuyerDocumentCode = comboBox_BuyerInformationBuyerDocumentCode.Text.Substring(0, 2);
                    else
                        doc.BuyerInformationBuyerDocumentCode = comboBox_BuyerInformationBuyerDocumentCode.Text;
                }
            }
            else if (sender == textBox_BuyerInformationBuyerDocumentData)
            {
                doc.BuyerInformationBuyerDocumentData = textBox_BuyerInformationBuyerDocumentData.Text;
            }
            else if (sender == textBox_BuyerInformationBuyerAddress)
            {
                doc.BuyerInformationBuyerAddress = textBox_BuyerInformationBuyerAddress.Text;
            }
            else if(sender == checkBox_alwaysSendBuyerData)
            {
                AppSettings.AlwaysSendBuyerDocData = checkBox_alwaysSendBuyerData.Checked;
            }
        }
        private bool _skipListener = true;

        private void button1_Click(object sender, EventArgs e)
        {
            doc.BuyerInformationBuyer = "";
            doc.BuyerInformationBuyerAddress = "";
            doc.BuyerInformationBuyerBirthday = "";
            doc.BuyerInformationBuyerCitizenship = "";
            doc.BuyerInformationBuyerDocumentCode = "";
            doc.BuyerInformationBuyerDocumentData = "";
            doc.BuyerInformationBuyerInn = "";
            this.Close();
        }
    }
}
