using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XdataAnalyze
{
    public partial class DataForm : Form
    {
        public event EventHandler AddSubItemEvent; //使用默认的事件处理委托
        public Action<string,string,string> ChangeText;
        public Action<int,string,string> InsertText;
        public DataForm()
        {
            InitializeComponent();
        }

        private void btnCannel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DataForm_Load(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 0;
             if(SetItemForm.lv!=null)
            {
                this.txtZ_name.Text = SetItemForm.lv.SubItems[1].Text;
                this.comboBox1.SelectedItem = SetItemForm.lv.SubItems[2].Text;
                this.txtxishu.Text= SetItemForm.lv.SubItems[3].Text;

            }
             else
            {
                this.txtZ_name.Text = "";
                this.txtxishu.Text = "1";
            }
            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //SetItemForm sf = new SetItemForm();
            //AddSubItemEvent += sf.AddEvent;
            //AddSubItemEvent(this, new MyEventArg() { Name = this.txtZ_name.Text,DataType=comboBox1.SelectedItem.ToString() });
            if (ChangeText != null)//判断事件是否为空
            {
                ChangeText(txtZ_name.Text,comboBox1.SelectedItem.ToString(),txtxishu.Text);//执行委托实例  
                this.Close();
            }
            //else if(InsertText!=null)
            //{
            //    InsertText(txtZ_name.Text, comboBox1.SelectedItem.ToString());//执行委托实例  
            //    this.Close();
            //}

        }
    }

    internal class MyEventArg : EventArgs
    {
        public string Name { get; set; }
        public string DataType { get; set; }
    }
}
