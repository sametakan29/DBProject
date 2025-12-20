using System;
using System.Linq;
using System.Windows.Forms;
using Npgsql;

namespace dneme1
{
    public partial class hazneler : Form
    {
        private const int MaxHazneSayisi = 12;
        private const int ProgressBarMax = 2000;

        // Her haznenin pigment bilgilerini saklamak için
        private class HazneBilgi
        {
            public string PigmentIsim { get; set; }
            public int KalanMiktar { get; set; }
        }

        // Hazne bilgilerini tutacak dictionary
        private System.Collections.Generic.Dictionary<int, HazneBilgi> hazneBilgileri =
            new System.Collections.Generic.Dictionary<int, HazneBilgi>();

        public hazneler()
        {
            InitializeComponent();
        }

        private void hazneler_Load(object sender, EventArgs e)
        {
            HazneBilgileriniYukle();
            ButonEventleriniEkle();
        }

        // Tüm butonlara event handler'ları ekle
        private void ButonEventleriniEkle()
        {
            for (int i = 1; i <= MaxHazneSayisi; i++)
            {
                Button ekleBttn = FindControl<Button>($"ekleBttn{i}");
                Button cikarBttn = FindControl<Button>($"cikarBttn{i}");

                if (ekleBttn != null)
                {
                    ekleBttn.Tag = i; // Hangi hazne olduğunu sakla
                    ekleBttn.Click += EkleBttn_Click;
                }

                if (cikarBttn != null)
                {
                    cikarBttn.Tag = i;
                    cikarBttn.Click += CikarBttn_Click;
                }
            }
        }

        // EKLE butonu için event handler
        private void EkleBttn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            int hazneNo = (int)btn.Tag;
            TextBox miktarTextBox = FindControl<TextBox>($"pigMikt{hazneNo}");

            if (miktarTextBox == null)
            {
                MessageBox.Show($"pigMikt{hazneNo} TextBox bulunamadı!", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Miktar kontrolü
            if (!int.TryParse(miktarTextBox.Text, out int miktar) || miktar <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir miktar giriniz!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hazne bilgisi var mı kontrol et
            if (!hazneBilgileri.ContainsKey(hazneNo))
            {
                MessageBox.Show($"Hazne {hazneNo} için pigment bilgisi bulunamadı!", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            HazneBilgi hazne = hazneBilgileri[hazneNo];

            // Stok hareketini kaydet
            if (StokHareketEkle(hazne.PigmentIsim, miktar, "EKLE"))
            {
                // Başarılı, UI'ı güncelle
                hazne.KalanMiktar += miktar;
                UpdateHazne(hazneNo, hazne.PigmentIsim, hazne.KalanMiktar);
                miktarTextBox.Clear();
                MessageBox.Show($"{miktar} gr başarıyla eklendi!\n{hazne.PigmentIsim}: {hazne.KalanMiktar} gr",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ÇIKAR butonu için event handler
        private void CikarBttn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            int hazneNo = (int)btn.Tag;
            TextBox miktarTextBox = FindControl<TextBox>($"pigMikt{hazneNo}");

            if (miktarTextBox == null)
            {
                MessageBox.Show($"pigMikt{hazneNo} TextBox bulunamadı!", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Miktar kontrolü
            if (!int.TryParse(miktarTextBox.Text, out int miktar) || miktar <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir miktar giriniz!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hazne bilgisi var mı kontrol et
            if (!hazneBilgileri.ContainsKey(hazneNo))
            {
                MessageBox.Show($"Hazne {hazneNo} için pigment bilgisi bulunamadı!", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            HazneBilgi hazne = hazneBilgileri[hazneNo];

            // Yeterli stok var mı kontrol et
            if (hazne.KalanMiktar < miktar)
            {
                MessageBox.Show($"Yetersiz stok!\nTalep: {miktar} gr\nMevcut: {hazne.KalanMiktar} gr",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Onay al
            DialogResult result = MessageBox.Show(
                $"{hazne.PigmentIsim}\n{miktar} gr çıkarmak istediğinize emin misiniz?\n\nMevcut: {hazne.KalanMiktar} gr\nKalacak: {hazne.KalanMiktar - miktar} gr",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            // Stok hareketini kaydet
            if (StokHareketEkle(hazne.PigmentIsim, miktar, "CIKAR"))
            {
                // Başarılı, UI'ı güncelle
                hazne.KalanMiktar -= miktar;
                UpdateHazne(hazneNo, hazne.PigmentIsim, hazne.KalanMiktar);
                miktarTextBox.Clear();
                MessageBox.Show($"{miktar} gr başarıyla çıkarıldı!\n{hazne.PigmentIsim}: {hazne.KalanMiktar} gr",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool StokHareketEkle(string pigmentIsim, int miktar, string tur)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string query = @"
            INSERT INTO stok_hareket 
            (pigmentisim, pigmentmarka, miktar, tur, tarih)
            VALUES 
            (@pigmentisim, 'Varsayılan', @miktar, @tur, NOW())";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@pigmentisim", pigmentIsim);
                    command.Parameters.AddWithValue("@miktar", miktar);
                    command.Parameters.AddWithValue("@tur", tur);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }


        private void HazneBilgileriniYukle()
        {
            try
            {
                string errorMessage;
                if (!DatabaseHelper.TestConnection(out errorMessage))
                {
                    MessageBox.Show($"Veritabanı bağlantısı başarısız: {errorMessage}",
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM public.hazne_listesi()";

                    using (var command = new NpgsqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        int index = 1;
                        hazneBilgileri.Clear(); // Önce temizle

                        while (reader.Read() && index <= MaxHazneSayisi)
                        {
                            string isim = reader["pigment_isim"].ToString();
                            int miktar = Convert.ToInt32(reader["kalan_gr"]);

                            // Hazne bilgilerini sakla
                            hazneBilgileri[index] = new HazneBilgi
                            {
                                PigmentIsim = isim,
                                KalanMiktar = miktar
                            };

                            UpdateHazne(index, isim, miktar);
                            index++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hazne bilgileri yüklenirken hata:\n{ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Generic kontrol bulma fonksiyonu
        private T FindControl<T>(string name) where T : Control
        {
            return Controls.Find(name, true).FirstOrDefault() as T;
        }

        // ProgressBar ve Label güncelleme fonksiyonu
        private void UpdateHazne(int index, string isim, int miktar)
        {
            ProgressBar progressBar = FindControl<ProgressBar>($"progressBar{index}");
            Label pigm = FindControl<Label>($"pigm{index}");

            if (progressBar == null)
            {
                // MessageBox.Show($"progressBar{index} bulunamadı!");
                return;
            }

            if (pigm == null)
            {
                // MessageBox.Show($"pigm{index} bulunamadı!");
                return;
            }

            // ProgressBar güncelle
            progressBar.Maximum = ProgressBarMax;
            progressBar.Value = Math.Min(miktar, progressBar.Maximum);

            // Label güncelle
            pigm.Text = $"{isim} - {miktar} gr";
        }
    }
}