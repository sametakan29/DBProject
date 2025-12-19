using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace dneme1
{
    public partial class AnaForm : Form
    {

        public AnaForm()
        {
            InitializeComponent();
        }

        private void AnaForm_Load(object sender, EventArgs e)
        {
            string errorMessage;
            if (!DatabaseHelper.TestConnection(out errorMessage))
            {
                MessageBox.Show(errorMessage);
                return;
            }
            BakimListViewAyarla();
            BakimUyarilariniGetir();
            DukkanlariDoldur();


        }
        private void DukkanlariDoldur()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT dukkanno, ad FROM dukkan ORDER BY ad";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var dr = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(dr);

                    dukkanSecComboBox.DataSource = null;

                    if (dt.Rows.Count == 0)
                    {
                        var result = MessageBox.Show(
                            "Henüz hiç dükkan yok.\nÞimdi dükkan eklemek ister misiniz?",
                            "Dükkan Bulunamadý",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (result == DialogResult.Yes)
                        {
                            DukkanEkleForm frm = new DukkanEkleForm();
                            frm.ShowDialog();

                            // Form kapandýktan sonra tekrar doldur
                            DukkanlariDoldur();
                        }

                        return;
                    }


                    dukkanSecComboBox.DisplayMember = "ad";
                    dukkanSecComboBox.ValueMember = "dukkanno";
                    dukkanSecComboBox.DataSource = dt;
                    dukkanSecComboBox.SelectedIndex = -1;

                    dukkanSecComboBox.Enabled = true;
                    girisCombBox.Enabled = true;
                }
            }
        }


        private void BakimListViewAyarla()
        {
            bakimListView.Clear();
            bakimListView.View = View.Details;

            bakimListView.Columns.Add("Parça", 150);
            bakimListView.Columns.Add("Son Bakým", 100);
            bakimListView.Columns.Add("Geçen Gün", 90);
            bakimListView.Columns.Add("Periyot", 80);
            bakimListView.Columns.Add("Durum", 90);
            bakimListView.Columns.Add("Uyarý", 400);
        }

        private void boyaYapBttn_Click(object sender, EventArgs e)
        {
            // Personel seçili mi kontrol et
            if (girisCombBox.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen önce bir personel seçiniz!", "Uyarý",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Seçili personelin rolno'sunu al
            int personelRolNo = Convert.ToInt32(girisCombBox.SelectedValue);


            // BoyaYap formunu personel bilgisi ile aç
            BoyaYap frm = new BoyaYap(personelRolNo);
            frm.Show();
        }

        private void haznelerBttn_Click(object sender, EventArgs e)
        {
            hazneler frm = new hazneler();
            frm.Show(); //formu göster
        }





        private void bakimBttn_Click(object sender, EventArgs e)
        {
            if (girisCombBox.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen bir personel seçiniz!");
                return;
            }

            if (bakimListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Lütfen bakým yapýlacak parçayý seçiniz!");
                return;
            }

            int personelRolNo = Convert.ToInt32(girisCombBox.SelectedValue);
            string parcaAdi = bakimListView.SelectedItems[0].Text;

            BakimYap(personelRolNo, parcaAdi);
        }




        private void bakimList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void BakimUyarilariniGetir()
        {
            try
            {
                bakimListView.Items.Clear();

                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT * FROM public.bakim_uyarilari();";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string parca = reader["parca_adi"].ToString();

                            string sonBakim = reader["son_bakim_tarihi"] == DBNull.Value
                                ? "-"
                                : Convert.ToDateTime(reader["son_bakim_tarihi"]).ToString("dd.MM.yyyy");

                            string gecenGun = reader["gecen_gun"].ToString();
                            string periyot = reader["bakim_periyodu"].ToString();
                            string durum = reader["durum"].ToString();
                            string mesaj = reader["uyari_mesaji"].ToString();

                            ListViewItem item = new ListViewItem(parca);
                            item.SubItems.Add(sonBakim);
                            item.SubItems.Add(gecenGun);
                            item.SubItems.Add(periyot);
                            item.SubItems.Add(durum);
                            item.SubItems.Add(mesaj);

                            // DURUMA GÖRE RENK
                            if (durum == "KRÝTÝK")
                                item.BackColor = System.Drawing.Color.LightCoral;
                            else if (durum == "GECÝKMÝÞ")
                                item.BackColor = System.Drawing.Color.Khaki;
                            else if (durum == "YAPILMADI")
                                item.BackColor = System.Drawing.Color.LightGray;
                            else
                                item.BackColor = System.Drawing.Color.LightGreen;

                            bakimListView.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bakým uyarýlarý alýnamadý: " + ex.Message);
            }
        }




        private void girisCombBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void PersonelDoldur(int dukkanno)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
            SELECT 
                r.rolno,
                p.personelad || ' ' || p.personelsoyad AS ad
            FROM personel p
            INNER JOIN roller r ON r.rolno = p.rolno
WHERE r.dukkanno = @dukkan
            ORDER BY ad";

                    using (var da = new NpgsqlDataAdapter(query, conn))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@dukkan", dukkanno);

                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        girisCombBox.DataSource = null;
                        girisCombBox.DisplayMember = "ad";     // görünen
                        girisCombBox.ValueMember = "rolno";  // DB’ye giden
                        girisCombBox.DataSource = dt;
                        girisCombBox.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Personel yüklenemedi: " + ex.Message);
            }
        }





        private void BakimYap(int personelRolNo, string parcaAdi)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT * FROM public.bakim_yap(@rolno, @parca);";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@rolno", personelRolNo);
                        cmd.Parameters.AddWithValue("@parca", parcaAdi);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bool basarili = Convert.ToBoolean(reader["basarili"]);
                                string mesaj = reader["mesaj"].ToString();

                                MessageBox.Show(mesaj);

                                if (basarili)
                                {
                                    // Bakim listesini yenile
                                    BakimUyarilariniGetir();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void gecmisBttn_Click(object sender, EventArgs e)
        {
            // BoyaYap formunu personel bilgisi ile aç
            GecmisForm frm = new GecmisForm();
            frm.Show();
        }

        private void loglarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // loglar formunu personel bilgisi ile aç
            LogForm frm = new LogForm();
            frm.Show();
        }
        private void personelEkleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // rolekle formunu aç
            RolEkleForm frm = new RolEkleForm();
            frm.Show();
        }
        private void dukkanEkleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // dukkanekle formunu aç
            DukkanEkleForm frm = new DukkanEkleForm();
            frm.Show();
        }

        private void dukkanSecComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dukkanSecComboBox.SelectedIndex < 0)
                return;

            if (dukkanSecComboBox.SelectedValue == null)
                return;

            PersonelDoldur(Convert.ToInt32(dukkanSecComboBox.SelectedValue));
        }


    }
} 