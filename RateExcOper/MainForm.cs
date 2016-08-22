using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RateExcOper.Properties;
using Npgsql;

namespace RateExcOper
{
    public partial class MainForm : Form
    {
        private NpgsqlConnection conn;

        // Здесь хранится состояние окна до сворачивания (максимизированное или нормальное)
        private FormWindowState _OldFormState;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            conn = new NpgsqlConnection("Server=" + Settings.Default.Host + ";Port=" + Settings.Default.Port + ";Database=" + Settings.Default.Database +
                ";User Id=" + Settings.Default.User + ";Password=" + Settings.Default.Pass + ";SSL=" + Settings.Default.SSL);
            try
            {
                conn.Open();

            }
            catch (Exception exc)
            {
                MessageBox.Show("Не удалось подключиться к базе данных:\r\n" + exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void backgroundWorkerUpdateList_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = GetDocsList(conn, datePickerStart.Value, datePickerEnd.Value);
        }

        // Получение списка документов. 
        private static DataView GetDocsList(NpgsqlConnection conn, DateTime dateTime1, DateTime dateTime2)
        {
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT * FROM orders where confirm is not null";
            DataTable resTable = new DataTable();
            using (var reader = cmd.ExecuteReader())
            {
                resTable.Load(reader);
            }            
            return resTable.DefaultView;            
        }

        private void backgroundWorkerUpdateList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null && !string.IsNullOrWhiteSpace(e.Error.Message))
            {
                timerUpdate.Stop();
                MessageBox.Show("Не удалось подключиться к базе данных:\r\n" + e.Error.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (conn.State == ConnectionState.Closed)
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("Не удалось подключиться к базе данных:\r\n" + e.Error.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                timerUpdate.Start();
                return;
            }
            
            object id = null;
            if (dataGridView.SelectedCells.Count > 0)
                id = dataGridView.SelectedCells[0].Value;
            
            //Разворачиваем если пришли новые документы
            if (dataGridView.DataSource != null && CheckNewDocs((DataView)dataGridView.DataSource, (DataView)e.Result))
            {
                Show();
                WindowState = _OldFormState;
            }
            dataGridView.DataSource = e.Result;
            //Выделение строки
            if (id != null && id != DBNull.Value)
                foreach (DataGridViewRow row in dataGridView.Rows)
                    if (row.Cells[0].Value != DBNull.Value && (int)row.Cells[0].Value == (int)id)
                        row.Selected = true;      
             
        }

        //Проверка появления новых документов
        private static bool CheckNewDocs(DataView oldTable, DataView newTable)
        {
            oldTable.Sort = "id";
            foreach (DataRowView row in newTable)
            {
                //if (row["confirm"] != DBNull.Value && (string)row["confirm"] == "Новый")
                if ((row["confirm"] != DBNull.Value) && (Convert.ToInt32(row["confirm"]) == 0))
                    if (oldTable.Find(row["id"]) < 0)
                        return true;
            }
            return false;
        }

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingForm form = new SettingForm(Settings.Default.Host, Settings.Default.Database, 
                Settings.Default.User, Settings.Default.Pass, Settings.Default.Port, Settings.Default.SSL);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Settings.Default["Host"] = form.Host;
                Settings.Default["Database"] = form.Database;
                Settings.Default["User"] = form.User;
                Settings.Default["Pass"] = form.Pass;
                Settings.Default["Port"] = form.Port;
                Settings.Default["SSL"] = form.SSL;
                Settings.Default.Save();
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.TimeOfDay > new TimeSpan(0, 1, 0) && DateTime.Now.TimeOfDay < new TimeSpan(0, 1, 10))
                datePickerStart.Value = datePickerEnd.Value = DateTime.Today;
            if (!backgroundWorkerUpdateList.IsBusy && conn != null)
                backgroundWorkerUpdateList.RunWorkerAsync();            
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Hide();
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            //проверяем, какой кнопкой было произведено нажатие
            if (e.Button == MouseButtons.Left)//если левой кнопкой мыши
            {
                //проверяем текущее состояние окна
                if (WindowState == FormWindowState.Normal || WindowState == FormWindowState.Maximized)//если оно развернуто
                {
                    //сохраняем текущее состояние
                    _OldFormState = WindowState;
                    //сворачиваем окно
                    WindowState = FormWindowState.Minimized;
                }
                else//в противном случае
                {
                    //и показываем на нанели задач
                    Show();
                    //разворачиваем (возвращаем старое состояние "до сворачивания")
                    WindowState = _OldFormState;
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerUpdate.Stop();
            try
            {
                conn.Close();
            }
            catch (Exception) { }
        }
    }
}
