namespace dneme1
{
    partial class RolEkleForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbDukkan = new System.Windows.Forms.ComboBox();
            this.btnEkle = new System.Windows.Forms.Button();
            this.btnSil = new System.Windows.Forms.Button();
            this.dgvRoller = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPersonelSoyAd = new System.Windows.Forms.TextBox();
            this.txtPersonelAd = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPersoneliletisim = new System.Windows.Forms.TextBox();
            this.cmbRol = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRoller)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbDukkan
            // 
            this.cmbDukkan.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.cmbDukkan.FormattingEnabled = true;
            this.cmbDukkan.Location = new System.Drawing.Point(938, 78);
            this.cmbDukkan.Name = "cmbDukkan";
            this.cmbDukkan.Size = new System.Drawing.Size(220, 37);
            this.cmbDukkan.TabIndex = 0;
            // 
            // btnEkle
            // 
            this.btnEkle.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnEkle.Location = new System.Drawing.Point(906, 564);
            this.btnEkle.Name = "btnEkle";
            this.btnEkle.Size = new System.Drawing.Size(167, 68);
            this.btnEkle.TabIndex = 3;
            this.btnEkle.Text = "ekle";
            this.btnEkle.UseVisualStyleBackColor = true;
            this.btnEkle.Click += new System.EventHandler(this.btnEkle_Click);
            // 
            // btnSil
            // 
            this.btnSil.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnSil.Location = new System.Drawing.Point(1119, 564);
            this.btnSil.Name = "btnSil";
            this.btnSil.Size = new System.Drawing.Size(167, 68);
            this.btnSil.TabIndex = 4;
            this.btnSil.Text = "Çıkar";
            this.btnSil.UseVisualStyleBackColor = true;
            this.btnSil.Click += new System.EventHandler(this.btnSil_Click);
            // 
            // dgvRoller
            // 
            this.dgvRoller.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRoller.Location = new System.Drawing.Point(32, 78);
            this.dgvRoller.Name = "dgvRoller";
            this.dgvRoller.RowHeadersWidth = 51;
            this.dgvRoller.RowTemplate.Height = 24;
            this.dgvRoller.Size = new System.Drawing.Size(844, 496);
            this.dgvRoller.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(984, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 29);
            this.label3.TabIndex = 9;
            this.label3.Text = "Dükkan seç";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label4.Location = new System.Drawing.Point(984, 358);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(180, 29);
            this.label4.TabIndex = 13;
            this.label4.Text = "Personel soyad";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label5.Location = new System.Drawing.Point(984, 251);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(135, 29);
            this.label5.TabIndex = 12;
            this.label5.Text = "Pesonel ad";
            // 
            // txtPersonelSoyAd
            // 
            this.txtPersonelSoyAd.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtPersonelSoyAd.Location = new System.Drawing.Point(938, 390);
            this.txtPersonelSoyAd.Name = "txtPersonelSoyAd";
            this.txtPersonelSoyAd.Size = new System.Drawing.Size(283, 34);
            this.txtPersonelSoyAd.TabIndex = 11;
            // 
            // txtPersonelAd
            // 
            this.txtPersonelAd.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtPersonelAd.Location = new System.Drawing.Point(938, 283);
            this.txtPersonelAd.Name = "txtPersonelAd";
            this.txtPersonelAd.Size = new System.Drawing.Size(283, 34);
            this.txtPersonelAd.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label6.Location = new System.Drawing.Point(984, 442);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 29);
            this.label6.TabIndex = 15;
            this.label6.Text = "İletişim";
            // 
            // txtPersoneliletisim
            // 
            this.txtPersoneliletisim.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtPersoneliletisim.Location = new System.Drawing.Point(938, 474);
            this.txtPersoneliletisim.Name = "txtPersoneliletisim";
            this.txtPersoneliletisim.Size = new System.Drawing.Size(283, 34);
            this.txtPersoneliletisim.TabIndex = 14;
            // 
            // cmbRol
            // 
            this.cmbRol.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.cmbRol.FormattingEnabled = true;
            this.cmbRol.Location = new System.Drawing.Point(938, 180);
            this.cmbRol.Name = "cmbRol";
            this.cmbRol.Size = new System.Drawing.Size(220, 37);
            this.cmbRol.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(361, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 36);
            this.label1.TabIndex = 17;
            this.label1.Text = "Personeller";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(1007, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 29);
            this.label2.TabIndex = 18;
            this.label2.Text = "Rol seç";
            // 
            // RolEkleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1357, 677);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbRol);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtPersoneliletisim);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtPersonelSoyAd);
            this.Controls.Add(this.txtPersonelAd);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dgvRoller);
            this.Controls.Add(this.btnSil);
            this.Controls.Add(this.btnEkle);
            this.Controls.Add(this.cmbDukkan);
            this.Name = "RolEkleForm";
            this.Text = "RolEkleForm";
            this.Load += new System.EventHandler(this.RolEkleForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRoller)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbDukkan;
        private System.Windows.Forms.Button btnEkle;
        private System.Windows.Forms.Button btnSil;
        private System.Windows.Forms.DataGridView dgvRoller;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPersonelSoyAd;
        private System.Windows.Forms.TextBox txtPersonelAd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPersoneliletisim;
        private System.Windows.Forms.ComboBox cmbRol;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}