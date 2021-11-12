using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Model;
using BLL;
using Common;

namespace XdataAnalyze
{
    public partial class SetItemForm : Form
    {
        public static ListViewItem lv ;
        string _name = string.Empty;
        FrameBll fBll = new FrameBll();
        FrameDataBll fdataBll = new FrameDataBll();
        bool changed = false;
        public SetItemForm()
        {
            InitializeComponent();
        }

        private void btnCannel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetItemForm_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectItemForm.Frame_name))
                _name = SelectItemForm.Frame_name;

            Frame model = new Frame
            {
                Name = _name
            };
            DataSet ds = fBll.GetFrame(model);
            DataRow dr = ds.Tables[0].Rows[0];
            this.txtZ_name.Text = _name;
            this.txtIdent_code.Text = dr["frame_code"].ToString();
            int i = int.Parse(dr["frame_type"].ToString());
            if (i == 0)
            {
                radLittle.Checked = true;
            }
            else
            {
                radBig.Checked = true;
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            lv = null;
            DataForm df = new DataForm
            {
                Text = "添加数据域"
            };
            df.ChangeText = (str1, str2, str3) =>
            {
                ListViewItem lvi = new ListViewItem
                {
                    ImageIndex = 0,

                    Text = (listView1.Items.Count + 1).ToString()
                };

                lvi.SubItems.Add(str1);

                lvi.SubItems.Add(str2);
                lvi.SubItems.Add(str3);
                this.listView1.Items.Add(lvi);
            };
            df.ShowDialog();
        }

        private void btnIns_Click(object sender, EventArgs e)
        {
            lv = null;
            DataForm df = new DataForm
            {
                Text = "插入数据域"
            };
            
            df.ChangeText = (str1, str2,str3) =>
            {
                ListViewItem lvi = new ListViewItem
                {
                    ImageIndex = 0,

                    Text = (listView1.Items.Count + 1).ToString()
                };
               
                lvi.SubItems.Add(str1);

                lvi.SubItems.Add(str2);
                lvi.SubItems.Add(str3);
                if (this.listView1.SelectedItems.Count > 0)
                {
                    int index = this.listView1.SelectedItems[0].Index;
                    lvi.Text = this.listView1.SelectedItems[0].Text;
                    foreach (ListViewItem liv in listView1.Items)
                    {
                        index = this.listView1.SelectedItems[0].Index;
                        int index1 = int.Parse(liv.Text);
                        if (index1 > index)
                            liv.Text = (index1 + 1).ToString();
                    }
                   this.listView1.Items.Insert(index, lvi);
                }
                else
                {
                    this.listView1.Items.Add(lvi);
                }
                
            };
            df.ShowDialog();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EidtDataItem();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            //int Index = 0;
            //if (this.listView1.SelectedItems.Count > 0)//判断listview有被选中项
            //{
            //    Index = this.listView1.SelectedItems[0].Index;//取当前选中项的index,SelectedItems[0]这必须为0       
            //    listView1.Items[Index].Remove();
            //} 
            if (listView1.SelectedItems.Count>0&&MessageBox.Show("确定要删除该数据域吗？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                //for (int i = listView1.SelectedItems.Count - 1; i >= 0; i--)
                //{
                //    ListViewItem item = listView1.SelectedItems[i];
                //    listView1.Items.Remove(item);
                //}
                int index = int.Parse(this.listView1.SelectedItems[0].Text);
                int num = listView1.SelectedItems.Count;
                foreach (ListViewItem lvi in listView1.SelectedItems)  //选中项遍历  
                {

                    listView1.Items.RemoveAt(lvi.Index); // 按索引移除  

                    //listView1.Items.Remove(lvi);   //按项移除  

                }
                foreach (ListViewItem liv in listView1.Items)
                {
                    int index1 = int.Parse(liv.Text);
                    if (index1 >= index)
                        liv.Text = (index1-num ).ToString();
                }
            }

        }
        public void AddEvent(object sender, EventArgs e)
        {
            MyEventArg arg = e as MyEventArg;
            //this.listView1.View = System.Windows.Forms.View.Details;
            ListViewItem lvi = new ListViewItem
            {
                ImageIndex = 0,

                Text = "1"
            };

            lvi.SubItems.Add(arg.Name);

            lvi.SubItems.Add(arg.DataType);

            this.listView1.Items.Add(lvi);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage1)
            {
                //功能代码  
                
            }
            else if (tabControl1.SelectedTab == tabPage2) 
            {
                changed = true;
                DataSet ds = fBll.GetFrame(new Frame() { Name = _name });
                int fid = int.Parse(ds.Tables[0].Rows[0]["ID"].ToString());
                DataSet fdds = fdataBll.GetFrameData(new FrameData() { FrameId = fid });
                if (fdds.Tables[0].Rows.Count >= 0&& listView1.Items.Count==0)
                {
                    //listView1.Items.Clear();
                    foreach (DataRow dr in fdds.Tables[0].Rows)
                    {
                        ListViewItem lvi = new ListViewItem
                        {
                            ImageIndex = 0,

                            Text = dr["data_sort"].ToString()
                        };

                        lvi.SubItems.Add(dr["data_name"].ToString()) ;

                        lvi.SubItems.Add(Common.Enum.getDataType2str(int.Parse(dr["data_type"].ToString())));
                        lvi.SubItems.Add(dr["data_xishu"].ToString());
                        listView1.Items.Add(lvi);
                    }

                }
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            EidtDataItem();
        }
        void EidtDataItem()
        {
            int selectCount = listView1.SelectedItems.Count;  
            if (selectCount > 0)
            {
                lv = listView1.SelectedItems[0];
                //MessageBox.Show(listView1.SelectedItems[0].SubItems[1].Text);
                DataForm df = new DataForm
                {
                    Text = "编辑数据域"
                };
                int index = this.listView1.SelectedItems[0].Index;
                df.ChangeText = (str1, str2,str3) =>
                {
                    listView1.Items.RemoveAt(index);
                    ListViewItem lvi = new ListViewItem
                    {
                        ImageIndex = 0,

                        Text = lv.Text//(index+1).ToString()
                    };

                    lvi.SubItems.Add(str1);
                    lvi.SubItems.Add(str2);
                    lvi.SubItems.Add(str3);

                    this.listView1.Items.Insert(index,lvi);
                };
                df.ShowDialog();

            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                #region 帧格式操作
                Frame upmodel = new Frame();
                upmodel.Name = this.txtZ_name.Text;
                
                Frame model = new Frame();
                DataSet ds = new DataSet();
                ds = fBll.GetFrame(new Frame() { Name = _name });
                int fid = int.Parse(ds.Tables[0].Rows[0]["ID"].ToString());
                int i = 0;
                if (upmodel.Name != _name)
                {
                   DataSet ds1 = fBll.GetFrame(upmodel);
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        MessageBox.Show("已存在该名称");
                        return;
                    }
                    else
                    {
                        upmodel.Code = this.txtIdent_code.Text;
                       
                        //model.Name = _name;
                        //model.Code = ds.Tables[0].Rows[0]["frame_code"].ToString();
                        model.Id =fid ;
                        i = fBll.UpFrame(model, upmodel);
                    }
                }
                else
                {
                    if (ds.Tables[0].Rows[0]["frame_code"].ToString() != this.txtIdent_code.Text)
                    {
                        //更新
                        upmodel.Code = this.txtIdent_code.Text;
                        //model.Name = _name;
                        //model.Code = ds.Tables[0].Rows[0]["frame_code"].ToString();
                        model.Id = fid;
                        i = fBll.UpFrame(model, upmodel);

                    }
                }
                #endregion

                #region 数据域操作

                if (changed)
                {
                    DataSet fdds = fdataBll.GetFrameData(new FrameData() { FrameId = fid });
                    if (fdds.Tables[0].Rows.Count > 0)
                    {
                        fdataBll.DelFrameData(new FrameData() { FrameId = fid });

                    }
                    if (listView1.Items.Count > 0)
                    {


                        foreach (ListViewItem liv in listView1.Items)
                        {
                            FrameData fd = new FrameData();
                            fd.Name = liv.SubItems[1].Text;
                            fd.Sort = int.Parse(liv.Text);
                            fd.Type = Common.Enum.getDataType2int(liv.SubItems[2].Text);
                            fd.FrameId = fid;
                            float ss = 1;
                            float.TryParse(liv.SubItems[3].Text, out ss);
                            fd.Xishu = ss;
                            fdataBll.InsFrameData(fd);

                        }
                    } 
                }
                #endregion
                this.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
