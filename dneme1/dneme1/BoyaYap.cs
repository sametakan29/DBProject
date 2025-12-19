using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Npgsql;

namespace dneme1
{
    public partial class BoyaYap : Form
    {
        #region Inner Classes

        private class RenkItem
        {
            public string RenkKodu { get; set; }
            public string RenkIsmi { get; set; }
            public string Kartela { get; set; }

            public override string ToString() => $"{RenkIsmi} ({RenkKodu})";
        }

        private class DukkanItem
        {
            public string Ad { get; set; }
            public string TelNo { get; set; }

            public override string ToString() => Ad;
        }

        #endregion

        #region Fields

        private readonly int _personelRolNo;
        private readonly ToolTip _renkTooltip;
        private TextBox _musteriTextBox;
        private ListBox _dukkanListBox;

        #endregion

        #region Constructor

        public BoyaYap(int personelRolNo)
        {
            InitializeComponent();
            _personelRolNo = personelRolNo;
            _renkTooltip = new ToolTip { AutoPopDelay = 5000, InitialDelay = 500 };
        }

        #endregion

        #region Form Events

        private void BoyaYap_Load(object sender, EventArgs e)
        {
            // Control referanslarını önbellekle
            _musteriTextBox = this.Controls.Find("musteri", true).FirstOrDefault() as TextBox;
            _dukkanListBox = this.Controls.Find("listBox3", true).FirstOrDefault() as ListBox;

            HazirRenkleriYukle();
            DukkanlariYukle();
            KgSecenekleriYukle();
        }

        #endregion

        #region Data Loading Methods

        private void HazirRenkleriYukle()
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    const string query = "SELECT * FROM hazir_renkler() ORDER BY renk_ismi";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        listBox1.Items.Clear();

                        while (reader.Read())
                        {
                            listBox1.Items.Add(new RenkItem
                            {
                                RenkKodu = reader["renk_kodu"].ToString(),
                                RenkIsmi = reader["renk_ismi"].ToString(),
                                Kartela = reader["kartela"].ToString()
                            });
                        }
                    }
                }

                if (listBox1.Items.Count > 0)
                    listBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hazır renkler yüklenirken hata oluştu:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DukkanlariYukle()
        {
            try
            {
                if (_dukkanListBox == null) return;

                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    const string query = "SELECT ad, telefonno FROM dukkan ORDER BY ad";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        _dukkanListBox.Items.Clear();

                        while (reader.Read())
                        {
                            _dukkanListBox.Items.Add(new DukkanItem
                            {
                                Ad = reader["ad"].ToString(),
                                TelNo = reader["telefonno"].ToString()
                            });
                        }
                    }
                }

                if (_dukkanListBox.Items.Count > 0)
                    _dukkanListBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dükkanlar yüklenirken hata oluştu:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void KgSecenekleriYukle()
        {
            listBox2.Items.Clear();
            listBox2.Items.AddRange(new object[] { 3, 10, 20 });
            listBox2.SelectedIndex = 1; // Varsayılan 10 kg
        }

        #endregion

        #region ListBox Events

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is RenkItem secilenRenk)
            {
                RenkDetaylariGoster(secilenRenk.RenkKodu);
            }
        }

        #endregion

        #region Renk Detay Methods

        private void RenkDetaylariGoster(string renkKodu)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    const string query = @"
                        SELECT pigmentisim, pigmentmarka, pigment_miktar_gr
                        FROM renk_pigment_detay
                        WHERE renkkodu = @renkkodu
                        ORDER BY pigment_miktar_gr DESC";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@renkkodu", renkKodu);

                        using (var reader = cmd.ExecuteReader())
                        {
                            var detay = new StringBuilder("Renk Formülü:\n");
                            int toplamPigment = 0;

                            while (reader.Read())
                            {
                                string pigment = reader["pigmentisim"].ToString();
                                string marka = reader["pigmentmarka"].ToString();
                                int miktar = Convert.ToInt32(reader["pigment_miktar_gr"]);

                                detay.AppendLine($"• {pigment} ({marka}): {miktar}gr");
                                toplamPigment += miktar;
                            }

                            detay.AppendLine($"\nToplam: {toplamPigment}gr");
                            _renkTooltip.SetToolTip(listBox1, detay.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Renk detayları yüklenirken hata: {ex.Message}");
            }
        }

        #endregion

        #region Boya Yap Button

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Validasyon ve veri toplama
                if (!ValidasyonYap(out var musteriAd, out var musteriSoyad, out var musteriTel,
                    out var musteriAdres, out var dukkan, out var bazKg, out var renkKodu))
                {
                    return;
                }

                // Boya yapma işlemi
                BoyaYapFonk(musteriAd, musteriSoyad, musteriTel, musteriAdres, dukkan, bazKg, renkKodu);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Boya yapılırken hata oluştu:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidasyonYap(out string musteriAd, out string musteriSoyad, out string musteriTel,
            out string musteriAdres, out DukkanItem dukkan, out int bazKg, out string renkKodu)
        {
            // Varsayılan değerler
            musteriAd = musteriSoyad = musteriTel = musteriAdres = renkKodu = "";
            dukkan = null;
            bazKg = 0;

            // Renk kontrolü
            if (!(listBox1.SelectedItem is RenkItem secilenRenk))
            {
                MessageBox.Show("Lütfen bir renk seçin!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            renkKodu = secilenRenk.RenkKodu;

            // Müşteri bilgileri
            if (_musteriTextBox == null)
            {
                MessageBox.Show("Müşteri bilgisi alanı bulunamadı!", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string musteriAdSoyad = _musteriTextBox.Text.Trim();
            musteriTel = textBox1.Text.Trim();
            musteriAdres = string.IsNullOrWhiteSpace(textBox2.Text) ? "Belirtilmedi" : textBox2.Text.Trim();

            // Ad-Soyad ayırma
            string[] adSoyadParcalari = musteriAdSoyad.Split(new[] { ' ' }, 2);
            musteriAd = adSoyadParcalari.Length > 0 ? adSoyadParcalari[0] : "";
            musteriSoyad = adSoyadParcalari.Length > 1 ? adSoyadParcalari[1] : "";

            if (string.IsNullOrEmpty(musteriAd))
            {
                MessageBox.Show("Müşteri adı zorunludur!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(musteriTel))
            {
                MessageBox.Show("Müşteri telefon numarası zorunludur!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Dükkan kontrolü
            if (_dukkanListBox == null || _dukkanListBox.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir dükkan seçin!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            dukkan = (DukkanItem)_dukkanListBox.SelectedItem;

            // Kg kontrolü
            if (listBox2.SelectedItem == null)
            {
                MessageBox.Show("Lütfen kg miktarı seçin!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            bazKg = Convert.ToInt32(listBox2.SelectedItem);

            // Personel kontrolü
            if (_personelRolNo <= 0)
            {
                MessageBox.Show("Personel bilgisi bulunamadı!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void BoyaYapFonk(string musteriAd, string musteriSoyad, string musteriTel,
            string musteriAdres, DukkanItem dukkan, int bazKg, string renkKodu)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                const string query = "SELECT * FROM boya_yap(:p1, :p2, :p3, :p4, :p5, :p6, :p7, :p8, :p9)";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    // Parametreleri ekle
                    cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = musteriAd });
                    cmd.Parameters.Add(new NpgsqlParameter("p2", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = musteriSoyad });
                    cmd.Parameters.Add(new NpgsqlParameter("p3", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = musteriTel });
                    cmd.Parameters.Add(new NpgsqlParameter("p4", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = musteriAdres });
                    cmd.Parameters.Add(new NpgsqlParameter("p5", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = dukkan.Ad });
                    cmd.Parameters.Add(new NpgsqlParameter("p6", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = dukkan.TelNo ?? "" });
                    cmd.Parameters.Add(new NpgsqlParameter("p7", NpgsqlTypes.NpgsqlDbType.Integer) { Value = _personelRolNo });
                    cmd.Parameters.Add(new NpgsqlParameter("p8", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = renkKodu });
                    cmd.Parameters.Add(new NpgsqlParameter("p9", NpgsqlTypes.NpgsqlDbType.Integer) { Value = bazKg });

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bool basarili = reader.GetBoolean(0);
                            string mesaj = reader.GetString(1);

                            if (basarili)
                            {
                                MessageBox.Show(mesaj, "Başarılı",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                FormuTemizle();
                            }
                            else
                            {
                                MessageBox.Show(mesaj, "Hata",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        private void FormuTemizle()
        {
            _musteriTextBox?.Clear();
            textBox1.Clear();
            textBox2.Clear();

            if (_dukkanListBox != null && _dukkanListBox.Items.Count > 0)
                _dukkanListBox.SelectedIndex = 0;

            if (listBox2.Items.Count > 0)
                listBox2.SelectedIndex = 1;

            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = 0;

            _musteriTextBox?.Focus();
        }

        #endregion


        
    }
}