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
    public partial class NewItemForm : Form
    {
        FrameBll fBll = new FrameBll();
        public NewItemForm()
        {
            InitializeComponent();
        }

        private void NewItemForm_Load(object sender, EventArgs e)
        {
           
        }

        private void btnCannel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            
            Frame model = new Frame();
            model.Name = this.txtZ_name.Text.Trim();
            DataSet ds = fBll.GetFrame(model);
            if(ds.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("已存在该名称");
                return;
            }
            model.Code = this.txtIdent_code.Text.Trim();
            
            model.Type = 0;
            if(!radLittle.Checked)
                model.Type = 1;
            int i=fBll.InsFrame(model);
            if(i<1)
            {
                MessageBox.Show("添加失败");
            }
            else
            {
                SelectItemForm s = new SelectItemForm();
                s.Refresh();
                this.Close();
            }
        }
    }
}
