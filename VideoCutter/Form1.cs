using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using Microsoft.VisualBasic;




namespace VideoCutter

     
{
    public partial class VideoCutter : Form
    {
        string output;
        private  Process proc = new Process();
        public double Progress { get; set; }





        public VideoCutter()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox1.Checked==true)
            {
                textBox2.Text = "00:00";
            }
            else
            {
                textBox2.Text = "";

            }





        }

        private void btnOpen_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title ="Select Source Video";
            ofd.Filter = "Supported Video File (*.avi,*.flv, *.mov,*.mp4) | *.avi;*.flv; *.mov;*.mp4";
            if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string input = ofd.FileName;
                textBox1.Text = input;
                string temp=Path.GetTempFileName();
                //temp = input;
                output = Path.Combine("C:/saveVideo/", $"{Path.GetFileNameWithoutExtension(temp)}.mp4");
                axWindowsMediaPlayer1.URL = textBox1.Text;
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = axWindowsMediaPlayer1.Ctlcontrols.currentPositionString;


        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = axWindowsMediaPlayer1.Ctlcontrols.currentPositionString;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox2.Checked == true)
            {
               
                textBox3.Text = axWindowsMediaPlayer1.currentMedia.durationString;
            }
            else
            {
                textBox3.Text = "";

            }





        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            StartConvert();
            progressBar1.Minimum = 0;   
            progressBar1.Maximum = 100;
            for (var i = 0; i<=progressBar1.Maximum; i++)
                backgroundWorker1.ReportProgress(i);


        }

        public void StartConvert()
        {
            Process proc = new Process();
            Control.CheckForIllegalCrossThreadCalls = false;


            //string cmd = $"-i \"{textBox1.Text}\" -ss \"{textBox2.Text}\" -to \"{textBox3.Text}\" -c:v copy -c:a copy \"{output}\"";
            string cmd = $"-i \"{textBox1.Text}\" -ss \"{textBox2.Text}\" -to \"{textBox3.Text}\" -vf \"eq=brightness=0.5\"  \"{output}\"";
          //  string cmd = $"-i \"{textBox1.Text}\" -ss \"{textBox2.Text}\" -to \"{textBox3.Text}\" \"{output}\""; //-c:v copy -c:a copy


            // string cmd = "-i \"" + textBox1.Text + "\" -ss \"" + textBox2.Text + "\" -to \"" + textBox3.Text + "\" -c:v copy -c:a copy \"" + output + "\"";
            Console.WriteLine(cmd);
            proc.StartInfo.FileName = @"C:\ffmpeg\ffmpeg.exe";
            Console.WriteLine(proc.StartInfo.FileName);
            proc.StartInfo.Arguments = cmd;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput= true;
            proc.StartInfo.CreateNoWindow = true;   
            proc.Start();
            this.Text = "Status: Converting....";
            StreamReader sr = proc.StandardError;
            while(!sr.EndOfStream)
            {
                getTotalSecondProcessed(sr.ReadLine());



            }


        }

        private void getTotalSecondProcessed(string line)
        {
            try
            {
                string[] split = line.Split(' ');
                foreach (var row in split)
                {
                if (row.StartsWith("time"))
                    {
                        var time = row.Split('=');
                        Progress = TimeSpan.Parse(time[1]).TotalSeconds;


                    }
                
                
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and display or log the error message
                MessageBox.Show("An error occurred: " + ex.Message);
                Console.WriteLine(ex.Message);
            }

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;

        } 

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Complete....");

        }

        private void btnConvert_Click(object sender, EventArgs e)
        {

            backgroundWorker1.RunWorkerAsync();





        }

        private void VideoCutter_Load(object sender, EventArgs e)
        {
            backgroundWorker1.WorkerReportsProgress = true;
        }

        
    }
}
