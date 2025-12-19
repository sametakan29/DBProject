namespace dneme1
{
    partial class DukkanEkleForm
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
            this.dukkanGridView = new System.Windows.Forms.DataGridView();
            this.btnDukkanEkle = new System.Windows.Forms.Button();
            this.btnYenile = new System.Windows.Forms.Button();
            this.btnDukkanSil = new System.Windows.Forms.Button();
            this.txtDukkanAdi = new System.Windows.Forms.TextBox();
            this.txtAdres = new System.Windows.Forms.TextBox();
            this.txtTelefon = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dukkanGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dukkanGridView
            // 
            this.dukkanGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dukkanGridView.Location = new System.Drawing.Point(537, 64);
            this.dukkanGridView.Name = "dukkanGridView";
            this.dukkanGridView.RowHeadersWidth = 51;
            this.dukkanGridView.RowTemplate.Height = 24;
            this.dukkanGridView.Size = new System.Drawing.Size(721, 345);
            this.dukkanGridView.TabIndex = 0;
            // 
            // btnDukkanEkle
            // 
            this.btnDukkanEkle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnDukkanEkle.Location = new System.Drawing.Point(537, 493);
            this.btnDukkanEkle.Name = "btnDukkanEkle";
            this.btnDukkanEkle.Size = new System.Drawing.Size(214, 54);
            this.btnDukkanEkle.TabIndex = 1;
            this.btnDukkanEkle.Text = "ekle";
            this.btnDukkanEkle.UseVisualStyleBackColor = true;
            this.btnDukkanEkle.Click += new System.EventHandler(this.btnDukkanEkle_Click);
            // 
            // btnYenile
            // 
            this.btnYenile.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnYenile.Location = new System.Drawing.Point(1044, 493);
            this.btnYenile.Name = "btnYenile";
            this.btnYenile.Size = new System.Drawing.Size(214, 54);
            this.btnYenile.TabIndex = 2;
            this.btnYenile.Text = "yenile";
            this.btnYenile.UseVisualStyleBackColor = true;
            this.btnYenile.Click += new System.EventHandler(this.btnYenile_Click);
            // 
            // btnDukkanSil
            // 
            this.btnDukkanSil.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnDukkanSil.Location = new System.Drawing.Point(790, 493);
            this.btnDukkanSil.Name = "btnDukkanSil";
            this.btnDukkanSil.Size = new System.Drawing.Size(214, 54);
            this.btnDukkanSil.TabIndex = 3;
            this.btnDukkanSil.Text = "sil";
            this.btnDukkanSil.UseVisualStyleBackColor = true;
            this.btnDukkanSil.Click += new System.EventHandler(this.btnDukkanSil_Click);
            // 
            // txtDukkanAdi
            // 
            this.txtDukkanAdi.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtDukkanAdi.Location = new System.Drawing.Point(61, 99);
            this.txtDukkanAdi.Name = "txtDukkanAdi";
            this.txtDukkanAdi.Size = new System.Drawing.Size(351, 41);
            this.txtDukkanAdi.TabIndex = 4;
            // 
            // txtAdres
            // 
            this.txtAdres.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtAdres.Location = new System.Drawing.Point(61, 218);
            this.txtAdres.Name = "txtAdres";
            this.txtAdres.Size = new System.Drawing.Size(351, 41);
            this.txtAdres.TabIndex = 5;
            // 
            // txtTelefon
            // 
            this.txtTelefon.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtTelefon.Location = new System.Drawing.Point(61, 334);
            this.txtTelefon.Name = "txtTelefon";
            this.txtTelefon.Size = new System.Drawing.Size(351, 41);
            this.txtTelefon.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(119, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 32);
            this.label1.TabIndex = 7;
            this.label1.Text = "Dükkan Adı";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(119, 183);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(191, 32);
            this.label2.TabIndex = 8;
            this.label2.Text = "Dükkan Adres";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(119, 299);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(200, 32);
            this.label3.TabIndex = 9;
            this.label3.Text = "Dükkan Tel No";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label4.Location = new System.Drawing.Point(826, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(142, 32);
            this.label4.TabIndex = 10;
            this.label4.Text = "Dükkanlar";
            // 
            // DukkanEkleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1298, 587);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTelefon);
            this.Controls.Add(this.txtAdres);
            this.Controls.Add(this.txtDukkanAdi);
            this.Controls.Add(this.btnDukkanSil);
            this.Controls.Add(this.btnYenile);
            this.Controls.Add(this.btnDukkanEkle);
            this.Controls.Add(this.dukkanGridView);
            this.Name = "DukkanEkleForm";
            this.Text = "DukkanEkleForm";
            this.Load += new System.EventHandler(this.DukkanEkleForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dukkanGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dukkanGridView;
        private System.Windows.Forms.Button btnDukkanEkle;
        private System.Windows.Forms.Button btnYenile;
        private System.Windows.Forms.Button btnDukkanSil;
        private System.Windows.Forms.TextBox txtDukkanAdi;
        private System.Windows.Forms.TextBox txtAdres;
        private System.Windows.Forms.TextBox txtTelefon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}