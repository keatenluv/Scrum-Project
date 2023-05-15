
namespace RRRPG
{
    partial class Setting
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
            this.btnBack = new System.Windows.Forms.Button();
            this.txtBoxChance = new System.Windows.Forms.TextBox();
            this.comboBoxWeapons = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnBack
            // 
            this.btnBack.Image = global::RRRPG.Properties.Resources.back_button;
            this.btnBack.Location = new System.Drawing.Point(12, 12);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(147, 41);
            this.btnBack.TabIndex = 0;
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // txtBoxChance
            // 
            this.txtBoxChance.Location = new System.Drawing.Point(210, 107);
            this.txtBoxChance.Name = "txtBoxChance";
            this.txtBoxChance.Size = new System.Drawing.Size(74, 23);
            this.txtBoxChance.TabIndex = 2;
            this.txtBoxChance.TextChanged += new System.EventHandler(this.txtBoxChance_TextChanged);
            // 
            // comboBoxWeapons
            // 
            this.comboBoxWeapons.FormattingEnabled = true;
            this.comboBoxWeapons.Location = new System.Drawing.Point(50, 107);
            this.comboBoxWeapons.Name = "comboBoxWeapons";
            this.comboBoxWeapons.Size = new System.Drawing.Size(121, 23);
            this.comboBoxWeapons.TabIndex = 3;
            this.comboBoxWeapons.SelectedIndexChanged += new System.EventHandler(this.comboBoxWeapons_SelectedIndexChanged);
            // 
            // Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::RRRPG.Properties.Resources.Settings_Background;
            this.ClientSize = new System.Drawing.Size(674, 521);
            this.Controls.Add(this.comboBoxWeapons);
            this.Controls.Add(this.txtBoxChance);
            this.Controls.Add(this.btnBack);
            this.Name = "Setting";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnBack;
        private TextBox txtBoxChance;
        private ComboBox comboBoxWeapons;
    }
}