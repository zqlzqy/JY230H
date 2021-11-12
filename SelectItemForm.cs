using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BLL;
using Model;


namespace XdataAnalyze
{
    public partial class SelectItemForm : Form
    {
        FrameBll fBll = new FrameBll();
        FrameDataBll fdBll = new FrameDataBll();
        public static string Frame_name = string.Empty;
        public Action<string> AcSelectText;
        public SelectItemForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (AcSelectText != null)//判断事件是否为空
            {
                AcSelectText(listBox1.SelectedItem.ToString());//执行委托实例  
                this.Close();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            NewItemForm nf = new NewItemForm();
            nf.FormClosed += new FormClosedEventHandler(F_FormClosed);
            nf.ShowDialog();
           
        }

        private void SelectItemForm_Load(object sender, EventArgs e)
        {

            InitCombox();
        }
        private void F_FormClosed(object sender, FormClosedEventArgs e)
        {
            InitCombox();
        }
        public void InitCombox()
        {
            listBox1.Items.Clear();
            DataSet ds = fBll.GetFrame(null);
            foreach (DataRow dr in ds.Tables[0].Rows)//从dataset表中获取某一列的所有值
            {
                this.listBox1.Items.Add(dr["frame_name"].ToString());
            }
            if (listBox1.Items.Count > 0)
                this.listBox1.SelectedIndex=0;
        }

        private void btnCannel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if(listBox1.Items.Count > 0)
            {
                if (MessageBox.Show("确定要删除该帧格式定义吗？","警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    Frame m = new Frame
                    {
                        Name = listBox1.SelectedItem.ToString()
                    };
                    DataSet ds = fBll.GetFrame(m);
                    int id = int.Parse(ds.Tables[0].Rows[0]["ID"].ToString());

                    int i = fdBll.DelFrameData(new FrameData {FrameId=id });
                     i = fBll.DelFrame(m);
                    if(i>0)
                    {

                        listBox1.Items.RemoveAt(this.listBox1.SelectedIndex);
                        this.listBox1.SelectedIndex =0;
                    }
                   else
                    {
                        MessageBox.Show("删除失败");
                    }
                }
            }
            else
            {
                MessageBox.Show("没有内容可删除了");
             }
                
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            SetComboxItem();
        }

        public void SetComboxItem()
        {
            string name = listBox1.SelectedItem.ToString();
            Frame_name = name;
            SetItemForm sform = new SetItemForm();
            sform.FormClosed += new FormClosedEventHandler(F_FormClosed);
            sform.ShowDialog();
        }
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            SetComboxItem();
        }
    }
}
