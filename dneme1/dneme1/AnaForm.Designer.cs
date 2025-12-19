namespace dneme1
{
    partial class AnaForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.boyaYapBttn = new System.Windows.Forms.Button();
            this.haznelerBttn = new System.Windows.Forms.Button();
            this.girisCombBox = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.personelEkleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loglarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dukkanEkleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bakimBttn = new System.Windows.Forms.Button();
            this.gecmisBttn = new System.Windows.Forms.Button();
            this.bakimListView = new System.Windows.Forms.ListView();
            this.dukkanSecComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // boyaYapBttn
            // 
            this.boyaYapBttn.Font = new System.Drawing.Font("Arial Black", 30F, System.Drawing.FontStyle.Bold);
            this.boyaYapBttn.Location = new System.Drawing.Point(12, 246);
            this.boyaYapBttn.Name = "boyaYapBttn";
            this.boyaYapBttn.Size = new System.Drawing.Size(295, 94);
            this.boyaYapBttn.TabIndex = 0;
            this.boyaYapBttn.Text = "Boya Yap";
            this.boyaYapBttn.UseVisualStyleBackColor = true;
            this.boyaYapBttn.Click += new System.EventHandler(this.boyaYapBttn_Click);
            // 
            // haznelerBttn
            // 
            this.haznelerBttn.Font = new System.Drawing.Font("Arial Black", 30F, System.Drawing.FontStyle.Bold);
            this.haznelerBttn.Location = new System.Drawing.Point(12, 346);
            this.haznelerBttn.Name = "haznelerBttn";
            this.haznelerBttn.Size = new System.Drawing.Size(312, 78);
            this.haznelerBttn.TabIndex = 1;
            this.haznelerBttn.Text = "Hazneler";
            this.haznelerBttn.UseVisualStyleBackColor = true;
            this.haznelerBttn.Click += new System.EventHandler(this.haznelerBttn_Click);
            // 
            // girisCombBox
            // 
            this.girisCombBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.girisCombBox.FormattingEnabled = true;
            this.girisCombBox.Items.AddRange(new object[] {
            "samet",
            "admin",
            "samet2"});
            this.girisCombBox.Location = new System.Drawing.Point(566, 126);
            this.girisCombBox.Name = "girisCombBox";
            this.girisCombBox.Size = new System.Drawing.Size(310, 46);
            this.girisCombBox.TabIndex = 3;
            this.girisCombBox.Text = "kullanici";
            this.girisCombBox.SelectedIndexChanged += new System.EventHandler(this.girisCombBox_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Arial", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.personelEkleToolStripMenuItem,
            this.loglarToolStripMenuItem,
            this.dukkanEkleToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1373, 34);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // personelEkleToolStripMenuItem
            // 
            this.personelEkleToolStripMenuItem.Name = "personelEkleToolStripMenuItem";
            this.personelEkleToolStripMenuItem.Size = new System.Drawing.Size(107, 30);
            this.personelEkleToolStripMenuItem.Text = "Rol ekle";
            this.personelEkleToolStripMenuItem.Click += new System.EventHandler(this.personelEkleToolStripMenuItem_Click);
            // 
            // loglarToolStripMenuItem
            // 
            this.loglarToolStripMenuItem.Name = "loglarToolStripMenuItem";
            this.loglarToolStripMenuItem.Size = new System.Drawing.Size(173, 30);
            this.loglarToolStripMenuItem.Text = "Bakým kayýtlarý";
            this.loglarToolStripMenuItem.Click += new System.EventHandler(this.loglarToolStripMenuItem_Click);
            // 
            // dukkanEkleToolStripMenuItem
            // 
            this.dukkanEkleToolStripMenuItem.Name = "dukkanEkleToolStripMenuItem";
            this.dukkanEkleToolStripMenuItem.Size = new System.Drawing.Size(150, 30);
            this.dukkanEkleToolStripMenuItem.Text = "Dükkan ekle";
            this.dukkanEkleToolStripMenuItem.Click += new System.EventHandler(this.dukkanEkleToolStripMenuItem_Click);
            // 
            // bakimBttn
            // 
            this.bakimBttn.Font = new System.Drawing.Font("Arial Black", 30F, System.Drawing.FontStyle.Bold);
            this.bakimBttn.Location = new System.Drawing.Point(894, 494);
            this.bakimBttn.Name = "bakimBttn";
            this.bakimBttn.Size = new System.Drawing.Size(312, 78);
            this.bakimBttn.TabIndex = 5;
            this.bakimBttn.Text = "Bakým";
            this.bakimBttn.UseVisualStyleBackColor = true;
            this.bakimBttn.Click += new System.EventHandler(this.bakimBttn_Click);
            // 
            // gecmisBttn
            // 
            this.gecmisBttn.Font = new System.Drawing.Font("Arial Black", 28.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.gecmisBttn.Location = new System.Drawing.Point(12, 430);
            this.gecmisBttn.Name = "gecmisBttn";
            this.gecmisBttn.Size = new System.Drawing.Size(234, 82);
            this.gecmisBttn.TabIndex = 7;
            this.gecmisBttn.Text = "Geçmiþ";
            this.gecmisBttn.UseVisualStyleBackColor = true;
            this.gecmisBttn.Click += new System.EventHandler(this.gecmisBttn_Click);
            // 
            // bakimListView
            // 
            this.bakimListView.HideSelection = false;
            this.bakimListView.Location = new System.Drawing.Point(617, 375);
            this.bakimListView.Name = "bakimListView";
            this.bakimListView.Size = new System.Drawing.Size(745, 113);
            this.bakimListView.TabIndex = 8;
            this.bakimListView.UseCompatibleStateImageBehavior = false;
            this.bakimListView.View = System.Windows.Forms.View.Details;
            // 
            // dukkanSecComboBox
            // 
            this.dukkanSecComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.dukkanSecComboBox.FormattingEnabled = true;
            this.dukkanSecComboBox.Items.AddRange(new object[] {
            "samet",
            "admin",
            "samet2"});
            this.dukkanSecComboBox.Location = new System.Drawing.Point(566, 74);
            this.dukkanSecComboBox.Name = "dukkanSecComboBox";
            this.dukkanSecComboBox.Size = new System.Drawing.Size(310, 46);
            this.dukkanSecComboBox.TabIndex = 9;
            this.dukkanSecComboBox.Text = "dukkan";
            this.dukkanSecComboBox.SelectedIndexChanged += new System.EventHandler(this.dukkanSecComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(439, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 25);
            this.label1.TabIndex = 10;
            this.label1.Text = "Dükkan seç:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(465, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 25);
            this.label2.TabIndex = 11;
            this.label2.Text = "Personel:";
            // 
            // AnaForm
            // 
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1373, 584);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dukkanSecComboBox);
            this.Controls.Add(this.bakimListView);
            this.Controls.Add(this.gecmisBttn);
            this.Controls.Add(this.bakimBttn);
            this.Controls.Add(this.girisCombBox);
            this.Controls.Add(this.haznelerBttn);
            this.Controls.Add(this.boyaYapBttn);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "AnaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Boya Makinesi";
            this.Load += new System.EventHandler(this.AnaForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button boyaYapBttn;
        private System.Windows.Forms.Button haznelerBttn;
        private System.Windows.Forms.ComboBox girisCombBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem personelEkleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loglarToolStripMenuItem;
        private System.Windows.Forms.Button bakimBttn;
        private System.Windows.Forms.Button gecmisBttn;
        private System.Windows.Forms.ListView bakimListView;
        private System.Windows.Forms.ToolStripMenuItem dukkanEkleToolStripMenuItem;
        private System.Windows.Forms.ComboBox dukkanSecComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
