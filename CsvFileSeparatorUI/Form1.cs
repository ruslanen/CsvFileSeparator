using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvFileSeparator;

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

            textBox1.Text =  openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var csvFileSeparator = new CsvFileSeparator.CsvFileSeparator(textBox1.Text, (int)numericUpDown1.Value, checkBox1.Checked, checkBox2.Checked);
            csvFileSeparator.Process();
            MessageBox.Show("Операция завершена", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}