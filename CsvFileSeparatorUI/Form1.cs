using System;
using System.Windows.Forms;

namespace CsvFileSeparatorUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            textBox1.Text = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;
            progressBar1.Visible = true;
            progressBar1.Maximum = (int)numericUpDown1.Value + 1;
            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // Для того, чтобы не казалось, что ничего не происходит, делается небольшое заполнение progressBar'а,
            // так как на начальном этапе чтения файла может понадобиться большее время
            backgroundWorker1.ReportProgress(1);
            var csvFileSeparator = new CsvFileSeparator.CsvFileSeparator(textBox1.Text, (int)numericUpDown1.Value, checkBox1.Checked, checkBox2.Checked);
            csvFileSeparator.Process(backgroundWorker1);
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.PerformStep();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            groupBox1.Enabled = true;
            progressBar1.Visible = false;
            progressBar1.Value = 0;

            MessageBox.Show("Операция завершена", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}