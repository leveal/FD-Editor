using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FR_Operator
{
    public partial class FormItemExtendedEditor: Form
    {
        public FormItemExtendedEditor(List<ConsumptionItem> items = null, int selectedItem = -1)
        {
            InitializeComponent();
            if(items == null)
            {
                items = new List<ConsumptionItem>();
            }
            CheqItems = items;
            if(CheqItems == null)
            {
                CheqItems = new List<ConsumptionItem>();
            }
            for (int i = 0; i < CheqItems.Count; i++ )
            {
                var pos = CheqItems[i];
                dataGridView_items.Rows.Add(pos.ToString("ushort6"));
                dataGridView_items.Rows[i].Tag = pos;
                if (pos.Correctness == FiscalPrinter.FD_ITEM_CONTROL_OK)
                {
                    dataGridView_items.Rows[i].Cells[0].Style.BackColor = Color.White;
                    dataGridView_items.Rows[i].Cells[0].Style.SelectionBackColor = Color.DodgerBlue;
                }
                else
                {
                    dataGridView_items.Rows[i].Cells[0].Style.BackColor = Color.Red;
                    dataGridView_items.Rows[i].Cells[0].Style.SelectionBackColor = Color.Orange;
                }
            }

            if (dataGridView_items.Rows.Count > 0)
            {
                dataGridView_items.Rows[0].Cells[0].Selected = true;
            }
            
        }
        //int _currentSelectedItem = -1;
        private bool _skipHandleItemEdit = false;
        private void ItemsFieldsEditActionHandler(object sender, EventArgs e)
        {
            if (_skipHandleItemEdit)
            {
                return;
            }
            int rowSelected = -1;
            if (dataGridView_items.SelectedCells.Count > 0)
            {
                rowSelected = dataGridView_items.SelectedCells[0].RowIndex;
            }
            if (rowSelected >= 0)
            {
                if (dataGridView_items.Rows[rowSelected].Tag == null)
                {
                    LogHandle.ol("create empty item");
                    dataGridView_items.Rows[rowSelected].Tag = new ConsumptionItem();
                }
                ConsumptionItem item = dataGridView_items.Rows[rowSelected].Tag as ConsumptionItem;
                if (sender == textBox_itemsName)
                {
                    item.Name = textBox_itemsName.Text;
                    if (item.Name.Length < 7)
                    {
                        dataGridView_items.Rows[rowSelected].Cells[0].Value = item.ToString("ushort6");
                    }
                }
                else if(sender == comboBox_itemsTaxRate)
                {
                    item.NdsRate = comboBox_itemsTaxRate.SelectedIndex;
                }
                else if(sender == textBox_itemsQuantity)
                {
                    double d = -1;
                    double.TryParse(FiscalPrinter.ReplaceBadDecimalSeparatorPoint(textBox_itemsQuantity.Text), out d);
                    if (d < 0.000001)
                    {
                        textBox_itemsQuantity.ForeColor = Color.Red;
                        d = 1;
                    }
                    else
                    {
                        textBox_itemsQuantity.ForeColor = Color.Black;
                    }
                    item.Quantity = d;
                    if (checkBox_calculateSums.Checked)
                    {
                        _skipHandleItemEdit = true;
                        item.Sum = Math.Round(item.Price * d,2);
                        textBox_itemsSum.Text = item.Sum.ToString("F2");
                        _skipHandleItemEdit = false;
                    }
                    item.Control();
                    if(item.Correctness == FiscalPrinter.FD_ITEM_CONTROL_OK)
                    {
                        dataGridView_items.Rows[rowSelected].Cells[0].Style.BackColor = Color.White;
                        dataGridView_items.Rows[rowSelected].Cells[0].Style.SelectionBackColor = Color.DodgerBlue;
                    }
                    else
                    {
                        dataGridView_items.Rows[rowSelected].Cells[0].Style.BackColor = Color.Red;
                        dataGridView_items.Rows[rowSelected].Cells[0].Style.SelectionBackColor = Color.Orange;
                    }
                    dataGridView_items.Rows[rowSelected].Cells[0].Value = item.ToString("ushort6");
                }
                else if(sender == textBox_itemsPrice)
                {
                    double d = -1;
                    double.TryParse(FiscalPrinter.ReplaceBadDecimalSeparatorPoint(textBox_itemsPrice.Text), out d);
                    d = Math.Round(d, 2);
                    if (d < -0.001)
                    {
                        textBox_itemsPrice.ForeColor = Color.Red;
                        d = 0;
                    }
                    else
                    {
                        textBox_itemsPrice.ForeColor = Color.Black;
                    }
                    item.Price = d;
                    if (checkBox_calculateSums.Checked)
                    {
                        _skipHandleItemEdit = true;
                        item.Sum = Math.Round(item.Quantity * d, 2);
                        textBox_itemsSum.Text = item.Sum.ToString("F2");
                        _skipHandleItemEdit = false;
                    }
                    item.Control();
                    if (item.Correctness == FiscalPrinter.FD_ITEM_CONTROL_OK)
                    {
                        dataGridView_items.Rows[rowSelected].Cells[0].Style.BackColor = Color.White;
                        dataGridView_items.Rows[rowSelected].Cells[0].Style.SelectionBackColor = Color.DodgerBlue;
                    }
                    else
                    {
                        dataGridView_items.Rows[rowSelected].Cells[0].Style.BackColor = Color.Red;
                        dataGridView_items.Rows[rowSelected].Cells[0].Style.SelectionBackColor = Color.Orange;
                    }
                    dataGridView_items.Rows[rowSelected].Cells[0].Value = item.ToString("ushort6");
                }
                else if (sender == textBox_itemsSum)
                {
                    double d = -1;
                    double.TryParse(FiscalPrinter.ReplaceBadDecimalSeparatorPoint(textBox_itemsSum.Text), out d);
                    d = Math.Round(d, 2);
                    if (d < 0.001)
                    {
                        textBox_itemsSum.ForeColor = Color.Red;
                        d = 0;
                    }
                    else
                    {
                        textBox_itemsSum.ForeColor = Color.Black;
                    }
                    item.Sum = d;
                    if (checkBox_calculateSums.Checked)
                    {
                        _skipHandleItemEdit = true;
                        try { item.Price = Math.Round(d / item.Quantity, 2); } catch { }
                        textBox_itemsPrice.Text = item.Price.ToString("F2");
                        _skipHandleItemEdit = false;
                    }
                    item.Control();
                    if (item.Correctness == FiscalPrinter.FD_ITEM_CONTROL_OK)
                    {
                        dataGridView_items.Rows[rowSelected].Cells[0].Style.BackColor = Color.White;
                        dataGridView_items.Rows[rowSelected].Cells[0].Style.SelectionBackColor = Color.DodgerBlue;
                    }
                    else
                    {
                        dataGridView_items.Rows[rowSelected].Cells[0].Style.BackColor = Color.Red;
                        dataGridView_items.Rows[rowSelected].Cells[0].Style.SelectionBackColor = Color.Orange;
                    }
                    dataGridView_items.Rows[rowSelected].Cells[0].Value = item.ToString("ushort6");
                }
                else if(sender == comboBox_itemsProductType)
                {
                    int ipt = comboBox_itemsProductType.SelectedIndex;
                    if(ipt == 28||ipt == 29)
                    {
                        _skipHandleItemEdit = true;
                        comboBox_itemsProductType.SelectedIndex = item.ProductType;
                        _skipHandleItemEdit = false;
                        return;
                    }
                    item.ProductType = ipt;
                }
                else if(sender == comboBox_itemsPaymentType)
                {
                    item.PaymentType = comboBox_itemsPaymentType.SelectedIndex;
                }
                else if(sender == textBox_unitMeasure105)
                {
                    item.Unit105 = textBox_unitMeasure105.Text;
                }
                else if(sender == comboBox_unitMasure120)
                {
                    switch (comboBox_unitMasure120.SelectedIndex)
                    {
                        case -1:
                        case 0:
                            item.Unit120 = 0;
                            break;
                        case 1:
                            item.Unit120 = 10;
                            break;
                        case 2:
                            item.Unit120 = 11;
                            break;
                        case 3:
                            item.Unit120 = 12;
                            break;
                        case 4:
                            item.Unit120 = 20;
                            break;
                        case 5:
                            item.Unit120 = 21;
                            break;
                        case 6:
                            item.Unit120 = 22;
                            break;
                        case 7:
                            item.Unit120 = 30;
                            break;
                        case 8:
                            item.Unit120 = 31;
                            break;
                        case 9:
                            item.Unit120 = 32;
                            break;
                        case 10:
                            item.Unit120 = 40;
                            break;
                        case 11:
                            item.Unit120 = 41;
                            break;
                        case 12:
                            item.Unit120 = 42;
                            break;
                        case 13:
                            item.Unit120 = 50;
                            break;
                        case 14:
                            item.Unit120 = 51;
                            break;
                        case 15:
                            item.Unit120 = 70;
                            break;
                        case 16:
                            item.Unit120 = 71;
                            break;
                        case 17:
                            item.Unit120 = 72;
                            break;
                        case 18:
                            item.Unit120 = 73;
                            break;
                        case 19:
                            item.Unit120 = 80;
                            break;
                        case 20:
                            item.Unit120 = 81;
                            break;
                        case 21:
                            item.Unit120 = 82;
                            break;
                        case 22:
                            item.Unit120 = 83;
                            break;
                        case 23:
                            item.Unit120 = 255;
                            break;
                    }
                }
                else if(sender == textBox_itemsPaymentAgentByProductType)
                {
                    int t = -1;
                    int.TryParse(textBox_itemsPaymentAgentByProductType.Text, out t);
                    if (textBox_itemsPaymentAgentByProductType.Text.Length > 0)
                    {
                        if (t < 0 || t > 255)
                        {
                            textBox_itemsPaymentAgentByProductType.ForeColor = Color.Red;
                            t = 0;
                        }
                        else
                        {
                            textBox_itemsPaymentAgentByProductType.ForeColor = Color.Black;
                        }
                    }
                    else
                    {
                        t = 0;
                    }
                    item.PaymentAgentByProductType = t;
                }
                else if(sender == textBox_itemsPaymentAgentDataTransferOperatorInn)
                {
                    if (FiscalPrinter.CorrectInn(textBox_itemsPaymentAgentDataTransferOperatorInn.Text))
                    {
                        item.TransferOperatorInn = textBox_itemsPaymentAgentDataTransferOperatorInn.Text;
                        textBox_itemsPaymentAgentDataTransferOperatorInn.ForeColor = Color.Black;
                    }
                    else
                    { 
                        item.TransferOperatorInn = "";
                        textBox_itemsPaymentAgentDataTransferOperatorInn.ForeColor = Color.Red;
                    }
                }
                else if(sender == textBox_itemsPaymentAgentDataTransferOperatorPhone)
                {
                    item.TransferOperatorPhone = textBox_itemsPaymentAgentDataTransferOperatorPhone.Text;
                }
                else if(sender == textBox_itemsPaymentAgentDataTransferOperatorAddress)
                {
                    item.TransferOperatorAddress = textBox_itemsPaymentAgentDataTransferOperatorAddress.Text;
                }
                else if(sender == textBox_itemsPaymentAgentDataTransferOperatorName)
                {
                    item.TransferOperatorName = textBox_itemsPaymentAgentDataTransferOperatorName.Text;
                }
                else if(sender == textBox_itemsPaymentAgentDataPaymentOperatorPhone)
                {
                    item.PaymentOperatorPhone = textBox_itemsPaymentAgentDataPaymentOperatorPhone.Text;
                }
                else if(sender == textBox_itemsPaymentAgentDataPpaymentAgentOperation)
                {
                    item.PaymentAgentOperation = textBox_itemsPaymentAgentDataPpaymentAgentOperation.Text;
                }
                else if(sender == textBox_itemsPaymentAgentDataPaymentAgentPhone)
                {
                    item.PaymentAgentPhone = textBox_itemsPaymentAgentDataPaymentAgentPhone.Text;
                }
                else if(sender  == textBox_itemsProviderInn)
                {
                    if (FiscalPrinter.CorrectInn(textBox_itemsProviderInn.Text))
                    {
                        item.ProviderInn = textBox_itemsProviderInn.Text;
                        textBox_itemsProviderInn.ForeColor = Color.Black;
                    }
                    else
                    { 
                        item.ProviderInn = "";
                        textBox_itemsProviderInn.ForeColor = Color.Red;
                    }
                }
                else if(sender == textBox_itemsProviderDataProviderName)
                {
                    item.ProviderName = textBox_itemsProviderDataProviderName.Text;
                }
                else if(sender == textBox_itemsProviderDataProviderPhone)
                {
                    item.ProviderPhone = textBox_itemsProviderDataProviderPhone.Text;
                }
            }


        }




        public List<ConsumptionItem> CheqItems = null;

        private void dataGridView_items_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            _skipHandleItemEdit = true;
            int rowSelected = e.RowIndex;
            LogHandle.ol("Row selected "+rowSelected);
            
            if (rowSelected >= 0)
            {
                if(dataGridView_items.Rows[rowSelected].Tag!=null && dataGridView_items.Rows[rowSelected].Tag is ConsumptionItem)
                {
                    ConsumptionItem item = dataGridView_items.Rows[rowSelected].Tag as ConsumptionItem;
                    textBox_itemsName.Text = item.Name;
                    textBox_itemsQuantity.Text = item.Quantity.ToString();
                    textBox_itemsPrice.Text = item.Price.ToString();
                    textBox_itemsSum.Text = item.Sum.ToString();
                    comboBox_itemsTaxRate.SelectedIndex = item.NdsRate;
                    comboBox_itemsProductType.SelectedIndex = item.ProductType;
                    comboBox_itemsPaymentType.SelectedIndex = item.PaymentType;
                    if (item.Unit120 >= 0)
                    {
                        if(item.Unit120 == 0)
                        {
                            comboBox_unitMasure120.SelectedIndex = 0;
                        }
                        else if(item.Unit120 == 10)
                        {
                            comboBox_unitMasure120.SelectedIndex = 1;
                        }
                        else if (item.Unit120 == 11)
                        {
                            comboBox_unitMasure120.SelectedIndex = 2;
                        }
                        else if (item.Unit120 == 12)
                        {
                            comboBox_unitMasure120.SelectedIndex = 3;
                        }
                        else if (item.Unit120 == 20)
                        {
                            comboBox_unitMasure120.SelectedIndex = 4;
                        }
                        else if (item.Unit120 == 21)
                        {
                            comboBox_unitMasure120.SelectedIndex = 5;
                        }
                        else if (item.Unit120 == 22)
                        {
                            comboBox_unitMasure120.SelectedIndex = 6;
                        }
                        else if (item.Unit120 == 30)
                        {
                            comboBox_unitMasure120.SelectedIndex = 7;
                        }
                        else if (item.Unit120 == 31)
                        {
                            comboBox_unitMasure120.SelectedIndex = 8;
                        }
                        else if (item.Unit120 == 32)
                        {
                            comboBox_unitMasure120.SelectedIndex = 9;
                        }
                        else if (item.Unit120 == 40)
                        {
                            comboBox_unitMasure120.SelectedIndex = 10;
                        }
                        else if (item.Unit120 == 41)
                        {
                            comboBox_unitMasure120.SelectedIndex = 11;
                        }
                        else if (item.Unit120 == 42)
                        {
                            comboBox_unitMasure120.SelectedIndex = 12;
                        }
                        else if (item.Unit120 == 50)
                        {
                            comboBox_unitMasure120.SelectedIndex = 13;
                        }
                        else if (item.Unit120 == 51)
                        {
                            comboBox_unitMasure120.SelectedIndex = 14;
                        }
                        else if (item.Unit120 == 70)
                        {
                            comboBox_unitMasure120.SelectedIndex = 15;
                        }
                        else if (item.Unit120 == 71)
                        {
                            comboBox_unitMasure120.SelectedIndex = 16;
                        }
                        else if (item.Unit120 == 72)
                        {
                            comboBox_unitMasure120.SelectedIndex = 17;
                        }
                        else if (item.Unit120 == 73)
                        {
                            comboBox_unitMasure120.SelectedIndex = 18;
                        }
                        else if (item.Unit120 == 80)
                        {
                            comboBox_unitMasure120.SelectedIndex = 19;
                        }
                        else if (item.Unit120 == 81)
                        {
                            comboBox_unitMasure120.SelectedIndex = 20;
                        }
                        else if (item.Unit120 == 82)
                        {
                            comboBox_unitMasure120.SelectedIndex = 21;
                        }
                        else if (item.Unit120 == 83)
                        {
                            comboBox_unitMasure120.SelectedIndex = 22;
                        }
                        else if (item.Unit120 == 255)
                        {
                            comboBox_unitMasure120.SelectedIndex = 23;
                        }
                    }
                    else
                    {
                        comboBox_unitMasure120.SelectedIndex = -1;
                    }
                    textBox_unitMeasure105.Text = item.Unit105;
                    if (item.PaymentAgentByProductType > 0)
                    {
                        textBox_itemsPaymentAgentByProductType.Text = item.PaymentAgentByProductType.ToString();
                    }
                    else
                    {
                        textBox_itemsPaymentAgentByProductType.Text = "";
                    }
                    textBox_itemsPaymentAgentDataTransferOperatorInn.Text = item.TransferOperatorInn;
                    textBox_itemsPaymentAgentDataTransferOperatorPhone.Text = item.TransferOperatorPhone;
                    textBox_itemsPaymentAgentDataTransferOperatorAddress.Text = item.TransferOperatorAddress;
                    textBox_itemsPaymentAgentDataTransferOperatorName.Text = item.TransferOperatorName;
                    textBox_itemsPaymentAgentDataPaymentOperatorPhone.Text = item.PaymentOperatorPhone;
                    textBox_itemsPaymentAgentDataPpaymentAgentOperation.Text = item.PaymentAgentOperation;
                    textBox_itemsPaymentAgentDataPaymentAgentPhone.Text = item.PaymentAgentPhone;
                    textBox_itemsProviderInn.Text = item.ProviderInn;
                    textBox_itemsProviderDataProviderName.Text = item.ProviderName;
                    textBox_itemsProviderDataProviderPhone.Text = item.ProviderPhone;
                    textBox_itemsCustomEntryNum.Text = item.CustomEntryNum;
                }
                else
                {
                    LogHandle.ol("Selected row has no item recreate");
                    dataGridView_items.Rows[rowSelected].Tag = new ConsumptionItem();
                    dataGridView_items_RowEnter(sender, e);
                }

            }
            _skipHandleItemEdit = false;

        }

        private void ChangeList(object sender, EventArgs e)
        {
            if(sender == button_addPosition)
            {
                dataGridView_items.Rows.Add();
                var r = dataGridView_items.Rows[dataGridView_items.Rows.Count - 1];
                var i = new ConsumptionItem();
                if (!string.IsNullOrEmpty(AppSettings.ItemName))
                {
                    i.Name = AppSettings.ItemName;
                }
                i.PaymentType = AppSettings.ItemPaymentType;
                i.NdsRate = AppSettings.ItemTaxRate;
                i.ProductType = AppSettings.ItemProductType;
                i.Unit120 = 0;
                r.Tag = i;
                r.Cells[0].Value = i.ToString("ushort6");
            }
            else if (sender == button_removePosition)
            {
                int rowSelected = -1;
                if (dataGridView_items.SelectedCells.Count > 0)
                {
                    rowSelected = dataGridView_items.SelectedCells[0].RowIndex;
                }
                if(rowSelected >= 0)
                {
                    dataGridView_items.Rows.RemoveAt(rowSelected);
                }
            }
        }

        private void FormItemExtendedEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(CheqItems == null)
            {
                CheqItems = new List<ConsumptionItem>();
            }
            CheqItems.Clear();
            foreach(DataGridViewRow row in dataGridView_items.Rows)
            {
                if(row.Tag != null && row.Tag is ConsumptionItem i)
                {
                    CheqItems.Add(i);
                }
            }

        }
    }
}
