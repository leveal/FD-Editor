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
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            this.Icon = Resources.fd_editpr_16_2;
            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add("Копировать ссылку");
            cm.MenuItems[0].Click += linkLabel_copyLink;
            linkLabel_doc.ContextMenu = cm;
            linkLabel_update.ContextMenu = cm;
        }

        private void linkLabel_copyLink(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            LogHandle.ol(sender.GetType().ToString());
            MenuItem item = sender as MenuItem;
            ContextMenu cm = item.GetContextMenu();
            Control parent = cm.SourceControl;
            //LogHandle.ol("link clicked");
            if(parent == linkLabel_doc)
            {
                Clipboard.SetText("https://docs.google.com/document/d/1iyOS7q8_ULj-dHfb7LYhAu0a19XmvymBdfMWLfGu1Xw/edit?usp=sharing");
            }
            else if(parent == linkLabel_update)
            {
                Clipboard.SetText("https://disk.yandex.ru/d/3UVsTq2L8V9bQw");
            }
           
        }

        private void linkLabel_doc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(sender == linkLabel_doc)
            {
                if(e.Button == MouseButtons.Left)
                {
                    try
                    {
                        linkLabel_doc.LinkVisited = true;
                        System.Diagnostics.Process.Start("https://docs.google.com/document/d/1iyOS7q8_ULj-dHfb7LYhAu0a19XmvymBdfMWLfGu1Xw/edit?usp=sharing");

                    }
                    catch(Exception exc)
                    {
                        LogHandle.ol(exc.Message);
                    }
                }

            }
            else if(sender == linkLabel_update)
            {
                if (e.Button == MouseButtons.Left)
                {
                    try
                    {
                        linkLabel_doc.LinkVisited = true;
                        System.Diagnostics.Process.Start("https://disk.yandex.ru/d/3UVsTq2L8V9bQw");

                    }
                    catch (Exception exc)
                    {
                        LogHandle.ol(exc.Message);
                    }
                }
            }
            
        }
    }
}
