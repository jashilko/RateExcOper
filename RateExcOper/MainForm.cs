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

namespace RateExcOper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void backgroundWorkerUpdateList_DoWork(object sender, DoWorkEventArgs e)
        {
            ;
        }

        private void backgroundWorkerUpdateList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ;
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
    }
}
