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
    public partial class FormPrelookFd: Form
    {
        public FormPrelookFd(string text)
        {
            InitializeComponent();
            this.Icon = Resources.fd_editpr_16_2;
            richTextBox1.Text = text;
        }
    }
}
