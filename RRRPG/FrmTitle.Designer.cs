namespace RRRPG {
  partial class FrmTitle {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTitle));
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnMute = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPlay
            // 
            this.btnPlay.BackColor = System.Drawing.Color.Transparent;
            this.btnPlay.BackgroundImage = global::RRRPG.Properties.Resources.Img_Title_Play_Button;
            this.btnPlay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPlay.Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnPlay.ForeColor = System.Drawing.Color.Black;
            this.btnPlay.Location = new System.Drawing.Point(66, 516);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(235, 76);
            this.btnPlay.TabIndex = 0;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = false;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnMute
            // 
            this.btnMute.Image = global::RRRPG.Properties.Resources.not_muted;
            this.btnMute.Location = new System.Drawing.Point(931, 0);
            this.btnMute.Name = "btnMute";
            this.btnMute.Size = new System.Drawing.Size(50, 48);
            this.btnMute.TabIndex = 1;
            this.btnMute.UseVisualStyleBackColor = true;
            this.btnMute.Click += new System.EventHandler(this.btnMute_Click);
            // 
            // FrmTitle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::RRRPG.Properties.Resources.Img_Title;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(980, 725);
            this.Controls.Add(this.btnMute);
            this.Controls.Add(this.btnPlay);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmTitle";
            this.Text = "Russian Roulette RPG";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmTitle_FormClosed);
            this.Load += new System.EventHandler(this.FrmTitle_Load);
            this.ResumeLayout(false);

    }

    #endregion

    private Button btnPlay;
        private Button btnMute;
    }
}