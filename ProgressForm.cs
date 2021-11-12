using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XdataAnalyze
{
    public partial class ProgressForm : Form
    {
        public ProgressForm(BackgroundWorker worker)
        {
            InitializeComponent();
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //设置进度条基础属性
            this.progressBar1.Value = 0;
            this.progressBar1.Style = ProgressBarStyle.Blocks;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Minimum = 0;
            this.progressBar1.MarqueeAnimationSpeed = 100;
            this.progressBar1.Step = 1;
        }
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            //lblStatus.Text = "";
        }

        //工作完成后执行的事件  
        public void OnProcessCompleted(object sender, EventArgs e)
        {
            this.Close();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
    }
}
