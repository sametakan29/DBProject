using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace dneme1
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }

        private void LogForm_Load(object sender, EventArgs e)
        {
            LoglariGetir();
            GridAyarla();
        }

        private void LoglariGetir()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            bakimtarihi,
                            bakimturu,
                            personelrolno
                        FROM bakimkaydi
                        ORDER BY bakimtarihi DESC;
                    ";

                    using (var da = new NpgsqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        logDataGridView.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loglar alınamadı: " + ex.Message);
            }
        }

        private void GridAyarla()
        {
            logDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            logDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            logDataGridView.ReadOnly = true;

            if (logDataGridView.Columns.Contains("bakimtarihi"))
                logDataGridView.Columns["bakimtarihi"].HeaderText = "Tarih";

            if (logDataGridView.Columns.Contains("bakimturu"))
                logDataGridView.Columns["bakimturu"].HeaderText = "Bakım Türü";

            if (logDataGridView.Columns.Contains("personelrolno"))
                logDataGridView.Columns["personelrolno"].HeaderText = "Personel Rol No";
        }
    }
}
