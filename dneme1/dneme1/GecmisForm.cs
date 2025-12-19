using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace dneme1
{
    public partial class GecmisForm : Form
    {
        public GecmisForm()
        {
            InitializeComponent();
        }

        private void GecmisForm_Load(object sender, EventArgs e)
        {
            // ÖRNEK müşteri iletişim bilgisi
            string musteriIletisim = "0500";

            MusteriGecmisGetir();
            DataGridAyarla();
        }

        private void MusteriGecmisGetir()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT * FROM musteri_gecmis()";

                using (var da = new NpgsqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void DataGridAyarla()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void MusteriGecmisGetir(string arama = "")
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = @"SELECT * FROM musteri_gecmis()
                       WHERE musteriad ILIKE @arama";

                using (var da = new NpgsqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@arama", "%" + arama + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            MusteriGecmisGetir(txtAra.Text.Trim());
        }

        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            MusteriGecmisGetir(txtAra.Text.Trim());//her değiştiğinde ara
        }
}
}
