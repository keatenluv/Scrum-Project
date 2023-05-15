using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RRRPG
{
    public partial class Setting : Form
    {

        Dictionary<string, float> weapons = new Dictionary<string, float>();
        //List<String> weapons = new List<String> { "Cork Gun", "Magic Wand", "Nerf Gun", "Water Gun", "Bow" };

    


        public Setting()
        {

            weapons.Add("Cork Gun", 0.4f);
            weapons.Add("Water Gun", 0.1f);
            weapons.Add("Nerf Revolver", 0.3f);
            weapons.Add("Bow", 0.2f);
            weapons.Add("Magic Wand", 0.2f);

            InitializeComponent();
            foreach (KeyValuePair<string, float> weapon in weapons)
            {
                comboBoxWeapons.Items.Add(weapon.Key); 
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Hide();
            FrmTitle title = new FrmTitle();
            title.SetWeaponData(weapons);
            title.ShowDialog();
        }

        private void txtBoxChance_TextChanged(object sender, EventArgs e)
        {
            float num;
            if (!txtBoxChance.Text.Equals(""))
            {
                if (float.TryParse(txtBoxChance.Text, out num))
                {
                    if (num < 0 || num > 1)
                    {
                        txtBoxChance.Text = "0";
                    }
                    else
                    {
                        weapons[comboBoxWeapons.SelectedItem.ToString()] = num;
                    }
                }
                else
                {
                    txtBoxChance.Text = "0";
                }
            }
        }

        private void comboBoxWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtBoxChance.Text = weapons[comboBoxWeapons.SelectedItem.ToString()].ToString();
            
        }
    }
}
