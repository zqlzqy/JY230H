using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;
using Model;
using static System.Windows.Forms.ListViewItem;

namespace XdataAnalyze
{
    public partial class MainForm : Form
    {
        DataSet ds = new DataSet();
        DataSet fdds = new DataSet();
        FrameBll fBll = new FrameBll();
        FrameDataBll fdataBll = new FrameDataBll();
        BackgroundWorker worker;
        int fid = 0;
        public MainForm()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            //worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        /// <summary>
        /// 异步 开始事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {


            //需要执行的代码
           
            worker.WorkerReportsProgress = true;
            ReadFile(path);
        }


        /// <summary>
        /// 事件: 异步执行完成后 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("执行完成。", "run", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.Clear();
            selectAllToolStripMenuItem.Click += SelectDataClick;
            openToolStripMenuItem.Click += OpenFileClick;
            toolOpenStripButton1.Click += OpenFileClick;
            toolSavaStripButton2.Click +=SavaFileClick;
            saveToolStripMenuItem.Click+= SavaFileClick;
            exitToolStripMenuItem.Click += CloseClick;

            openToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            toolOpenStripButton1.Enabled = false;
            toolSavaStripButton2.Enabled = false;

        }
        //自己定义个点击事件需要执行的动作
        private void SelectDataClick(object sender, EventArgs e)
        {
           SelectItemForm sl=new SelectItemForm();
            sl.AcSelectText = (str) => {
                ds = fBll.GetFrame(new Frame() { Name = str });
                fid = int.Parse(ds.Tables[0].Rows[0]["ID"].ToString());
                fdds = fdataBll.GetFrameData(new FrameData() { FrameId = fid });
                listView1.Clear();
                if (fdds.Tables[0].Rows.Count > 0)
                {
                    int i = 0;
                    foreach (DataRow dr in fdds.Tables[0].Rows)
                    {
                        listView1.Columns.Add(dr["data_name"].ToString());
                        //设置ListView.Column[0].Width := -1;//列宽根据列内容自适应，此时保证列内容都可见。
                        //设置ListView.Column[0].Width := -2;//列宽根据列标题自适应，此时保证列标题可见。
                        //ColumnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
                        //ListView1.Columns[索引].TextAlign = HorizontalAlignment.Center;
                        listView1.Columns[i].Width = 100;
                        i++;
                    }
                    //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }
                this.Text = this.Text+"-" + str;
                openToolStripMenuItem.Enabled = true;
                toolOpenStripButton1.Enabled = true;
            };

            sl.ShowDialog();
        }
        string path = "";
        void ReadFile(string path)
        {
            if (path == "")
            {
                return;
            }
            using (FileStream fsRead = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
            {
                byte[] buffer = new byte[fsRead.Length];
                int r = fsRead.Read(buffer, 0, buffer.Length);
                string code = ds.Tables[0].Rows[0]["frame_code"].ToString();

                int len = code.Length / 2;
                string[] codeArray = new string[len];
                for (int j = 0; j < len; j++)
                {
                    codeArray[j] = code.Substring(j * 2, 2);
                }
                int i = 0;
                int leng = fdataBll.GetDataTypebyte(fdds, -1);
                byte[] buffers;
               
                while (i < r)
                {
                    //worker.ReportProgress(100/r*i);
                    bool b = true;
                    for (int j = 0; j < len; j++)
                    {
                        int num = j + i;
                        byte b1 = buffer[num];
                        string s = codeArray[j];
                        byte b2 = byte.Parse(s, NumberStyles.HexNumber);
                        if (b1 != b2)
                        {
                            b = false;
                            break;
                        }
                    }
                    if (b)
                    {
                        buffers = new byte[leng];
                        if (i + leng + len > r)
                            Array.Copy(buffer, i + len, buffers, 0, r - i - len);
                        else
                            Array.Copy(buffer, i + len, buffers, 0, leng);
                        if (fdds.Tables[0].Rows.Count > 0)
                        {
                            ListViewItem lvi = new ListViewItem();

                            foreach (DataRow dr in fdds.Tables[0].Rows)
                            {
                                string name = dr["data_name"].ToString();
                                int type = int.Parse(dr["data_type"].ToString());
                                float Xishu = float.Parse(dr["data_xishu"].ToString());
                                int num = dr.Table.Rows.IndexOf(dr);
                                int indx = fdataBll.GetDataTypebyte(fdds, num);
                                string str = Common.Enum.getDataTypeStr(type, indx, buffers, Xishu);
                                if (num == 0)
                                {
                                    lvi.Text = str;
                                }
                                else
                                {
                                    lvi.SubItems.Add(str);
                                }


                            }
                            listView1.Items.Add(lvi);
                        }
                        i += leng + len;

                    }
                    else
                    {
                        i++;
                    }
                }

                //textBox1.Text = Encoding.Default.GetString(buffer, 0, r);
            }
        }
        private void OpenFileClick(object sender, EventArgs e)
        {
            saveToolStripMenuItem.Enabled = true;
            toolSavaStripButton2.Enabled = true;
            listView1.Items.Clear();
            ListView.CheckForIllegalCrossThreadCalls = false;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "二进制文件(*.tlog)|*.tlog|所有文件(*.*)|*.*";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            ofd.Title = "请选择要打开的文件";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                 path = ofd.FileName;
               
                //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                //sw.Start();
                var sw = System.Diagnostics.Stopwatch.StartNew();
                Parallel.Invoke(() => ReadFile(path));
                //Task task1 = Task.Run(() => ReadFile(path));
                
                //new Thread(h =>
                //{
                //    ReadFile(path);
                //}).Start();
                //ReadFile(path);
                //sw.Stop();

               // TimeSpan ts2 = sw.Elapsed;
               //MessageBox.Show("TimeSpan time {0}"+ ts2.TotalMilliseconds);

                //ThreadPool.QueueUserWorkItem(new WaitCallback(ReadFile),path);
                ////创建一个Task实例
                //Task task = new Task(() => ReadFile(path));
                ////开始任务
                //task.Start();
                //if (path == "")
                //{
                //    return;
                //}
                //using (FileStream fsRead = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
                //{
                //    byte[] buffer = new byte[fsRead.Length];
                //    int r = fsRead.Read(buffer, 0, buffer.Length);
                //    string code = ds.Tables[0].Rows[0]["frame_code"].ToString();

                //    int len = code.Length / 2;
                //    string[] codeArray = new string[len];
                //   for(int j=0;j<len;j++)
                //    {
                //        codeArray[j] = code.Substring(j*2, 2);
                //    }
                //    int i = 0;
                //    int leng = fdataBll.GetDataTypebyte(fdds, -1);
                //    byte[] buffers;
                //worker.RunWorkerAsync();
                //显示进度窗体
                //ProgressForm frm = new ProgressForm(this.worker);
                //frm.StartPosition = FormStartPosition.CenterScreen;
                //frm.ShowDialog(this);
                //    while (i<r)
                //    {
                //        bool b = true;
                //        for(int j=0;j<len;j++)
                //        {
                //            int num = j + i;
                //            byte b1 = buffer[num];
                //            string s = codeArray[j];
                //            byte b2 = byte.Parse(s, NumberStyles.HexNumber);
                //            if (b1!=b2)
                //            {
                //                b = false;
                //                break;
                //            } 
                //        }
                //        if(b)
                //        {

                //                //break;
                //            buffers = new byte[leng];
                //            if (i + leng + len > r)
                //                Array.Copy(buffer, i + len, buffers,0,r-i-len);
                //            else
                //                Array.Copy(buffer, i + len, buffers, 0, leng);
                //            if (fdds.Tables[0].Rows.Count > 0)
                //            {
                //                ListViewItem lvi = new ListViewItem();

                //                foreach (DataRow dr in fdds.Tables[0].Rows)
                //                {
                //                    string name = dr["data_name"].ToString();
                //                    int type = int.Parse(dr["data_type"].ToString());
                //                    float Xishu = float.Parse(dr["data_xishu"].ToString());
                //                    int num = dr.Table.Rows.IndexOf(dr);
                //                    int indx= fdataBll.GetDataTypebyte(fdds, num);
                //                    string str = Common.Enum.getDataTypeStr(type,indx, buffers,Xishu);
                //                    if(num==0)
                //                    {
                //                        lvi.Text=str;
                //                    }
                //                    else
                //                    {
                //                        lvi.SubItems.Add(str);
                //                    }


                //                }
                //                listView1.Items.Add(lvi);
                //            }
                //            i += leng + len; ;

                //        }
                //        else
                //        {
                //            i++;
                //        }
                //    }

                //    //textBox1.Text = Encoding.Default.GetString(buffer, 0, r);
                //}

            }
        }
        private void SavaFileClick(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "保存文件";
            sfd.InitialDirectory = @"D:\";
            sfd.Filter = "文本文件| *.txt";
            sfd.ShowDialog();

            string path = sfd.FileName;
            if (path == "")
            {
                return;
            }

            using (FileStream fsWrite = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                StringBuilder sbHead = new StringBuilder();
                StringBuilder sbdata = new StringBuilder();
                if (fdds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in fdds.Tables[0].Rows)
                    {
                        sbHead.AppendFormat("{0,-12}", dr["data_name"].ToString());
                    }
                    sbHead.AppendLine();
                    
                }
                if (listView1.Items.Count > 0)
                {


                    foreach (ListViewItem liv in listView1.Items)
                    {
                        //sbdata.AppendFormat("{0,-14}", liv.Text);
                        for (int i=0;i< liv.SubItems.Count;i++)
                        {
                            sbdata.AppendFormat("{0,-16}", liv.SubItems[i].Text);
                        }
                        sbdata.AppendLine();
                        

                    }
                }
                string str = sbHead.ToString() + sbdata.ToString();
                byte[] buffer = Encoding.Default.GetBytes(str);
                fsWrite.Write(buffer, 0, buffer.Length);
                MessageBox.Show("保存成功");

            }

        }
        private void CloseClick(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
