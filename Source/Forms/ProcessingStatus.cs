using FR_Operator.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FR_Operator
{
    public partial class ProcessingStatus : Form
    {
        public ProcessingStatus()
        {
            InitializeComponent();
            link = this;
            this.Icon = Resources.fd_editpr_16_2;
            comboBox_dynPause.Visible = comboBox_dynPause_Visible;
        }

        public static MainForm main_window = null;
        public static ProcessingStatus link = null;
        public bool comboBox_dynPause_Visible = false;
        public void VisiblePause()
        {
            if (this.Created)
            {
                MethodInvoker method = delegate
                {
                    comboBox_dynPause.Visible = true;
                    comboBox_dynPause_Visible = true;
                };
                if (this.InvokeRequired)
                    this.BeginInvoke(method);
                else
                    method.Invoke();
            }
        }
        public void Message(string message, bool showErrors = true)
        {
            if(!string.IsNullOrEmpty(message))
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action(() => 
                    { 
                        textBox_ProcessingStatusMessage.Text = message;
                        if (showErrors)
                        {
                            textBox_errors.Text = "Проблем в обработке " + MassActionReporter.ErrorCounter.ToString();
                        }
                        else
                        {
                            textBox_errors.Text = "";
                        }
                    }));
                    //Invoke(new Action(() => textBox_errors.Text = "Проблем в обработке "+MassActionReporter.ErrorCounter.ToString()));
                } 
                else
                {
                    textBox_ProcessingStatusMessage.Text = message;
                    if (showErrors)
                    {
                        textBox_errors.Text = "Проблем в обработке " + MassActionReporter.ErrorCounter.ToString();
                    }
                    else
                    {
                        textBox_errors.Text = "";
                    }
                }    
            }
        }

        private void S_allDone()
        {
            if (this.Created)
            {
                MethodInvoker method = delegate
                {
                    this.Close();
                    this.Dispose();
                };
                Thread.Sleep(250);
                if (this.InvokeRequired)
                    this.BeginInvoke(method);
                else
                    method.Invoke();
            }
        }

        public void AllDone()
        {
            S_allDone();
        }

        private void button_processingBreak_Click(object sender, EventArgs e)
        {
            MainForm.processInterruptor = true;
            FormOfdExport.breakOperation = true;
        }

        private void ComboBox_dynPause_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender == comboBox_dynPause)
            {
                if(comboBox_dynPause.SelectedIndex == 0)
                {
                    FormOfdExport.dynamicPause = 0;
                }
                else
                {
                    FormOfdExport.dynamicPause = (int)(Math.Pow(2, comboBox_dynPause.SelectedIndex - 1)+0.01);
                }
            }
        }
    }
}
