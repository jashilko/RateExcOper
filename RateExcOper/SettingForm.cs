using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace RateExcOper
{
    public partial class SettingForm : Form
    {
        public string Host
        {
            get { return tbHost.Text.Trim(); }
        }

        public string Database
        {
            get { return tbDatabase.Text.Trim(); }
        }

        public string User
        {
            get { return tbUser.Text.Trim(); }
        }

        public string Pass
        {
            get { return tbPass.Text.Trim(); }
        }

        public string Port
        {
            get { return tbPort.Text.Trim(); }
        }

        public bool SSL
        {
            get { return cbSSL.Checked; }
        }

        public SettingForm(string Host, string Database, string User, string Pass, string Port, bool SSL)
        {
            InitializeComponent();
            tbHost.Text = Host;
            tbDatabase.Text = Database;
            tbUser.Text = User;
            tbPass.Text = Pass;
            tbPort.Text = Port;
            cbSSL.Checked = SSL;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=" + tbHost.Text.Trim() + ";Port=" + tbPort.Text.Trim() + ";Database=" + tbDatabase.Text.Trim() + 
                ";User Id=" + tbUser.Text.Trim() + ";Password=" + tbPass.Text.Trim() + ";SSL=" + cbSSL.Checked);
            try
            {
                conn.Open();
                conn.Close();
                MessageBox.Show("Успешное подключение к базе", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception exc)
            {
                //throw new Exception("Не удалось подключиться к базе данных: " + exc.Message);
                MessageBox.Show("Не удалось подключиться к базе данных:\r\n" + exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

    }
}
