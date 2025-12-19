using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace dneme1
{
    public partial class DukkanEkleForm : Form
    {
        public DukkanEkleForm()
        {
            InitializeComponent();
        }

        private void DukkanEkleForm_Load(object sender, EventArgs e)
        {
            DukkanlariGetir();
            GridAyarla();
        }

        // 🔹 DÜKKANLARI LİSTELE
        private void DukkanlariGetir()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT dukkanno, ad, adres, telefonno FROM dukkan ORDER BY ad";

                    using (var da = new NpgsqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dukkanGridView.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dükkanlar yüklenemedi: " + ex.Message);
            }
        }

        // 🔹 GRID AYARLARI
        private void GridAyarla()
        {
            dukkanGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dukkanGridView.ReadOnly = true;
            dukkanGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dukkanGridView.AllowUserToAddRows = false;

            dukkanGridView.Columns["dukkanno"].HeaderText = "No";
            dukkanGridView.Columns["ad"].HeaderText = "Dükkan Adı";
            dukkanGridView.Columns["adres"].HeaderText = "Adres";
            dukkanGridView.Columns["telefonno"].HeaderText = "Telefon";
        }

        // 🔹 EKLE
        private void btnDukkanEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDukkanAdi.Text))
            {
                MessageBox.Show("Dükkan adı boş olamaz!");
                return;
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO dukkan (ad, adres, telefonno)
                        VALUES (@ad, @adres, @telefon)
                    ";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ad", txtDukkanAdi.Text);
                        cmd.Parameters.AddWithValue("@adres", txtAdres.Text);
                        cmd.Parameters.AddWithValue("@telefon", txtTelefon.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Dükkan eklendi.");

                Temizle();
                DukkanlariGetir();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ekleme hatası: " + ex.Message);
            }
        }

        // 🔹 SİL
        private void btnDukkanSil_Click(object sender, EventArgs e)
        {
            if (dukkanGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Silmek için bir dükkan seçiniz!");
                return;
            }

            int dukkanno = Convert.ToInt32(dukkanGridView.SelectedRows[0].Cells["dukkanno"].Value);

            if (MessageBox.Show("Seçilen dükkan silinsin mi?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = "DELETE FROM dukkan WHERE dukkanno = @no";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@no", dukkanno);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Dükkan silindi.");
                DukkanlariGetir();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme hatası: " + ex.Message);
            }
        }

        // 🔹 YENİLE
        private void btnYenile_Click(object sender, EventArgs e)
        {
            DukkanlariGetir();
        }

        // 🔹 TEXTBOX TEMİZLE
        private void Temizle()
        {
            txtDukkanAdi.Clear();
            txtAdres.Clear();
            txtTelefon.Clear();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
