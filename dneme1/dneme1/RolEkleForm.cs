using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace dneme1
{
    public partial class RolEkleForm : Form
    {
        public RolEkleForm()
        {
            InitializeComponent();
        }

        private void RolEkleForm_Load(object sender, EventArgs e)
        {
            DukkanlariGetir();
            GridAyarla();

            cmbDukkan.SelectedIndexChanged += cmbDukkan_SelectedIndexChanged;
            btnEkle.Click += btnEkle_Click;
            btnSil.Click += btnSil_Click;
        }

        //  DÜKKANLARI COMBOBOX'A DOLDUR
        private void DukkanlariGetir()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT dukkanno, ad FROM dukkan ORDER BY ad";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var dr = cmd.ExecuteReader())
                {
                    Dictionary<int, string> dukkanlar = new Dictionary<int, string>();

                    while (dr.Read())
                        dukkanlar.Add((int)dr["dukkanno"], dr["ad"].ToString());

                    cmbDukkan.DataSource = new BindingSource(dukkanlar, null);
                    cmbDukkan.DisplayMember = "Value";
                    cmbDukkan.ValueMember = "Key";
                    cmbDukkan.SelectedIndex = -1;
                }
            }
        }

        //  DATAGRID AYARLARI
        private void GridAyarla()
        {
            dgvRoller.AutoGenerateColumns = true;
            dgvRoller.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRoller.MultiSelect = false;
            dgvRoller.ReadOnly = true;
        }

        //  SEÇİLEN DÜKKANA GÖRE ROLLER
        private void cmbDukkan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDukkan.SelectedIndex == -1) return;

            int dukkanno = (int)((KeyValuePair<int, string>)cmbDukkan.SelectedItem).Key;

            RollerGetir(dukkanno);
            DukkanaGorePersonelleriGetir(dukkanno);
        }

        //  🆕 ROLLERİ GETİR - O DÜKKANA AİT TÜM ROLLER
        private void RollerGetir(int dukkanno)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // O dükkana ait TÜM rolleri getir (personel olsun olmasın)
                string sql = @"SELECT DISTINCT rolno, roladi, rolyetki
                               FROM roller
                               WHERE dukkanno = @dukkan
                               ORDER BY roladi";

                using (var da = new NpgsqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@dukkan", dukkanno);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Eğer bu dükkanda hiç rol yoksa, varsayılan rolleri ekle
                    if (dt.Rows.Count == 0)
                    {
                        VarsayilanRolleriEkle(dukkanno);
                        // Tekrar getir
                        da.Fill(dt);
                    }

                    cmbRol.DataSource = dt;
                    cmbRol.DisplayMember = "roladi";
                    cmbRol.ValueMember = "rolno";
                    cmbRol.SelectedIndex = -1;
                }
            }
        }

        //  🆕 VARSAYILAN ROLLERİ EKLE (Eğer dükkanda hiç rol yoksa)
        private void VarsayilanRolleriEkle(int dukkanno)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // Boya Ustası ve Müdür rollerini ekle
                string sql = @"INSERT INTO roller (roladi, rolyetki, dukkanno)
                               VALUES 
                               ('Boya Ustası', 'BOYACI', @dukkan),
                               ('Müdür', 'YONETICI', @dukkan)";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dukkan", dukkanno);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //  ROL EKLE - Personel ekle
        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (cmbDukkan.SelectedIndex == -1)
            {
                MessageBox.Show("Önce dükkan seçiniz!");
                return;
            }

            if (cmbRol.SelectedIndex == -1)
            {
                MessageBox.Show("Rol seçiniz!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPersonelAd.Text) ||
                string.IsNullOrWhiteSpace(txtPersonelSoyAd.Text) ||
                string.IsNullOrWhiteSpace(txtPersoneliletisim.Text))
            {
                MessageBox.Show("Personel bilgileri boş olamaz!");
                return;
            }

            int rolno = Convert.ToInt32(cmbRol.SelectedValue);

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string sql = @"INSERT INTO personel
                                   (rolno, personelad, personelsoyad, personeliletisim, personelrolno)
                                   VALUES (@rolno, @ad, @soyad, @iletisim, @personelrolno)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@rolno", rolno);
                        cmd.Parameters.AddWithValue("@ad", txtPersonelAd.Text.Trim());
                        cmd.Parameters.AddWithValue("@soyad", txtPersonelSoyAd.Text.Trim());
                        cmd.Parameters.AddWithValue("@iletisim", txtPersoneliletisim.Text.Trim());
                        cmd.Parameters.AddWithValue("@personelrolno", rolno); // personelrolno = rolno
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("✅ Personel başarıyla eklendi!");

                int dukkanno = (int)((KeyValuePair<int, string>)cmbDukkan.SelectedItem).Key;
                DukkanaGorePersonelleriGetir(dukkanno);

                txtPersonelAd.Clear();
                txtPersonelSoyAd.Clear();
                txtPersoneliletisim.Clear();
                cmbRol.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ HATA: " + ex.Message);
            }
        }

        //  ROL SİL - Personel sil
        private void btnSil_Click(object sender, EventArgs e)
        {
            if (dgvRoller.SelectedRows.Count == 0)
            {
                MessageBox.Show("Silmek için personel seçiniz!");
                return;
            }

            string ad = dgvRoller.SelectedRows[0].Cells["personelad"].Value.ToString();
            string soyad = dgvRoller.SelectedRows[0].Cells["personelsoyad"].Value.ToString();

            var result = MessageBox.Show(
                $"{ad} {soyad} personelini silmek istediğinize emin misiniz?",
                "Personel Sil",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            string iletisim = dgvRoller.SelectedRows[0].Cells["personeliletisim"].Value.ToString();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string sql = "DELETE FROM personel WHERE personeliletisim = @iletisim";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@iletisim", iletisim);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("✅ Personel silindi!");

                int dukkanno = (int)((KeyValuePair<int, string>)cmbDukkan.SelectedItem).Key;
                DukkanaGorePersonelleriGetir(dukkanno);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ HATA: " + ex.Message);
            }
        }

        //  DÜKKANA GÖRE PERSONELLERİ GETİR
        private void DukkanaGorePersonelleriGetir(int dukkanno)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                SELECT 
                    p.personelad,
                    p.personelsoyad,
                    p.personeliletisim,
                    r.roladi as rol
                FROM personel p
                JOIN roller r ON r.rolno = p.rolno
                WHERE r.dukkanno = @dukkan
                ORDER BY p.personelad";

                using (var da = new NpgsqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@dukkan", dukkanno);

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvRoller.DataSource = dt;

                    // Kolon başlıklarını düzenle
                    if (dgvRoller.Columns.Count > 0)
                    {
                        dgvRoller.Columns["personelad"].HeaderText = "Ad";
                        dgvRoller.Columns["personelsoyad"].HeaderText = "Soyad";
                        dgvRoller.Columns["personeliletisim"].HeaderText = "İletişim";
                        dgvRoller.Columns["rol"].HeaderText = "Rol";
                    }
                }
            }
        }
    }
}