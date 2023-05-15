using RRRPG.Properties;
using RRRPGLib;
using System.Media;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace RRRPG
{
    public partial class FrmMain : Form
    {
        private SoundPlayer soundPlayer;
        private int state;
        private Character player;
        private Character opponent;
        private Weapon weapon;
        private Dictionary<WeaponType, (PictureBox pic, Label lbl)> weaponSelectMap;
        private Dictionary<WeaponType, Image> weaponBackgroundMap;
        public Dictionary<string, float> weaponData = new Dictionary<string, float>();

        public FrmMain()
        {
            InitializeComponent();
        }

        public void SetWeaponData(Dictionary<string, float> weapons)
        {
            weaponData = weapons;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            soundPlayer = new SoundPlayer(Resources.Mus_Title_Bg_Music);
            if (global::RRRPG.FrmTitle.isMuted)
            {
                btnMute.Image = Resources.muted;
            }
            else
            {
                soundPlayer.PlayLooping();
            }
            btnDoIt.Visible = false;
            lblOpponentSpeak.Visible = false;
            lblPlayerSpeak.Visible = false;
            weapon = Weapon.MakeWeapon(WeaponType.MAGIC_WAND);
            weapon.ChanceOfMisfire = weaponData["Magic Wand"];
            state = -1;
            weaponSelectMap = new() {
                {WeaponType.BOW, (picWeaponSelectBow, lblWeaponSelectBow) },
                {WeaponType.CORK_GUN, (picWeaponSelectCorkGun,lblWeaponSelectCorkGun) },
                {WeaponType.WATER_GUN, (picWeaponSelectWaterGun, lblWeaponSelectWaterGun) },
                {WeaponType.MAGIC_WAND, (picWeaponSelectMagicWand, lblWeaponSelectMagicWand) },
                {WeaponType.NERF_REVOLVER, (picWeaponSelectNerfRev, lblWeaponSelectNerfRev) },
            };

            //each weapon has own background image that loads with WeaponType
            weaponBackgroundMap = new() {
                {WeaponType.BOW, Resources.yoshiBackground },
                {WeaponType.CORK_GUN, Resources.willyBackground },
                {WeaponType.WATER_GUN, Resources.flameBackground },
                {WeaponType.MAGIC_WAND, Resources.wizardBackground },
                {WeaponType.NERF_REVOLVER, Resources.Rambo },
            };
            SelectWeapon(WeaponType.MAGIC_WAND);

            // make background color of player and opponent transparent
            picOpponent.BackColor = Color.Transparent;
            picPlayer.BackColor = Color.Transparent;


            //magicwand stat initialization
            textBox1.Visible = false;

            picWeaponSelectMagicWand.MouseEnter += picWeaponSelectMagicWand_MouseEnter;
            picWeaponSelectMagicWand.MouseLeave += picWeaponSelectMagicWand_MouseLeave;

            //corkgun stats initialization
            Weapon corkGun = Weapon.MakeWeapon(WeaponType.CORK_GUN);
            corkGun.ChanceOfMisfire = weaponData["Cork Gun"];
            string corkGunStats = GetWeaponStats(corkGun);
            textBox3.Text = corkGunStats;
            textBox3.Visible = false;

            picWeaponSelectCorkGun.MouseEnter += picWeaponSelectCorkGun_MouseEnter;
            picWeaponSelectCorkGun.MouseLeave += picWeaponSelectCorkGun_MouseLeave;

            //initialize water gun stats
            Weapon waterGun = Weapon.MakeWeapon(WeaponType.WATER_GUN);
            waterGun.ChanceOfMisfire = weaponData["Water Gun"];
            string waterGunStats = GetWeaponStats(waterGun);
            textBox2.Text = waterGunStats;
            textBox2.Visible = false;

            picWeaponSelectWaterGun.MouseEnter += picWeaponSelectWaterGun_MouseEnter;
            picWeaponSelectWaterGun.MouseLeave += picWeaponSelectWaterGun_MouseLeave;

            //nerfrev stat initialization
            Weapon nerfRevolver = Weapon.MakeWeapon(WeaponType.NERF_REVOLVER);
            nerfRevolver.ChanceOfMisfire = weaponData["Nerf Revolver"];
            string nerfRevolverStats = GetWeaponStats(nerfRevolver);
            textBox4.Text = nerfRevolverStats;
            textBox4.Visible = false;

            picWeaponSelectNerfRev.MouseEnter += picWeaponSelectNerfRev_MouseEnter;
            picWeaponSelectNerfRev.MouseLeave += picWeaponSelectNerfRev_MouseLeave;

            //bow initialization
            Weapon bow = Weapon.MakeWeapon(WeaponType.BOW);
            bow.ChanceOfMisfire = weaponData["Bow"];
            string bowStats = GetWeaponStats(bow);
            textBox5.Text = bowStats;
            textBox5.Visible = false;

            picWeaponSelectBow.MouseEnter += picWeaponSelectBow_MouseEnter;
            picWeaponSelectBow.MouseLeave += picWeaponSelectBow_MouseLeave;

        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            soundPlayer.Stop();
            player.Shutup();
            player.ShowIdle();
            opponent.ShowIdle();
            btnStart.Visible = false;
            opponent.SaySmack();
            tmrStateMachine.Interval = 5000;
            tmrStateMachine.Enabled = true;
            state = 0;
            panWeaponSelect.Visible = false;
        }

        private void tmrDialog_Tick(object sender, EventArgs e)
        {
            if (state == 0)
            {
                opponent.Shutup();
                player.SaySmack();
                state = 1;
            }
            else if (state == 1)
            {
                opponent.Shutup();
                player.Shutup();
                player.ShowReady();
                opponent.ShowNoWeapon();
                btnDoIt.Visible = true;
                tmrStateMachine.Enabled = false;
                state = 2;
            }
            else if (state == 3)
            {
                player.SayOw();
                state = 4;
                tmrStateMachine.Interval = 2500;
            }
            else if (state == 4)
            {
                player.SayBoned();
                btnStart.Visible = true;
                if (!global::RRRPG.FrmTitle.isMuted)
                {
                    tmrPlayMusicAfterGameOver.Enabled = true;
                }
                panWeaponSelect.Visible = true;
                state = -1;
                tmrStateMachine.Enabled = false;
            }
            else if (state == 5)
            {
                player.Shutup();
                opponent.ShowReady();
                state = 6;
            }
            else if (state == 6)
            {
                if (opponent.PullTrigger(weapon))
                {
                    state = 7;
                }
                else
                {
                    state = 1;
                }
            }
            else if (state == 7)
            {
                opponent.SayOw();
                state = 8;
                tmrStateMachine.Interval = 2500;
            }
            else if (state == 8)
            {
                opponent.SayBoned();
                btnStart.Visible = true;
                if (!global::RRRPG.FrmTitle.isMuted)
                {
                    tmrPlayMusicAfterGameOver.Enabled = true;
                }
                panWeaponSelect.Visible = true;
                state = -1;
                tmrStateMachine.Enabled = false;
            }
        }

        private void btnDoIt_Click(object sender, EventArgs e)
        {
            SoundPlayer playClick = new SoundPlayer(Resources.Gun_Dry_Fire_Sound_Effect);
            if (player.PullTrigger(weapon))
            {
                state = 3;
                tmrStateMachine.Interval = 2200;
                tmrStateMachine.Enabled = true;
            }
            else
            {
                state = 5;
                tmrStateMachine.Interval = 1500;
                tmrStateMachine.Enabled = true;
                playClick.Play();
            }
            btnDoIt.Visible = false;
        }

        private void SelectWeapon(WeaponType type)
        {
            Color selectedColor = Color.Yellow;
            foreach (var weaponSel in weaponSelectMap)
            {
                weaponSel.Value.pic.BackColor = Color.Transparent;
                weaponSel.Value.pic.BorderStyle = BorderStyle.None;
                weaponSel.Value.lbl.ForeColor = Color.White;
            }
            weaponSelectMap[type].pic.BackColor = selectedColor;
            weaponSelectMap[type].pic.BorderStyle = BorderStyle.Fixed3D;
            weaponSelectMap[type].lbl.ForeColor = selectedColor;
            weapon = Weapon.MakeWeapon(type);
            opponent = Character.MakeOpponent(type, picOpponent, lblOpponentSpeak);
            player = Character.MakePlayer(type, picPlayer, lblPlayerSpeak);
            this.BackgroundImage = weaponBackgroundMap[type];
            this.BackgroundImageLayout = ImageLayout.Stretch;
            textBox1.Text = GetWeaponStats(weapon);

            // Set the opponent's name and update the label text         
            lblOpponent.Text = opponent.Name;
        }
        //method that gets stats for each weapon type
        private string GetWeaponStats(Weapon weapon)
        {
            string stats = $"WEAPON STATS:{Environment.NewLine}" +
                           $"Type: {weapon.Type}{Environment.NewLine}" +
                           $"Chance of Misfire: {weapon.ChanceOfMisfire}{Environment.NewLine}" +
                           $"Damage: {weapon.Damage}{Environment.NewLine}" +
                           $"Velocity: {weapon.Velocity}";
            return stats;
        }


        private void picWeaponSelectMagicWand_MouseEnter(object sender, EventArgs e)
        {
            Weapon magicWand = Weapon.MakeWeapon(WeaponType.MAGIC_WAND);
            magicWand.ChanceOfMisfire = weaponData["Magic Wand"];
            textBox1.Text = GetWeaponStats(magicWand);
            textBox1.Visible = true;
        }


        private void picWeaponSelectMagicWand_MouseLeave(object sender, EventArgs e)
        {
            textBox1.Visible = false;
        }

        private void picWeaponSelectMagicWand_Click(object sender, EventArgs e)
        {
            SelectWeapon(WeaponType.MAGIC_WAND);
        }
        private void picWeaponSelectCorkGun_MouseEnter(object sender, EventArgs e)
        {
            textBox3.Visible = true;
        }

        private void picWeaponSelectCorkGun_MouseLeave(object sender, EventArgs e)
        {
            textBox3.Visible = false;
        }

        private void picWeaponSelectCorkGun_Click(object sender, EventArgs e)
        {
            SelectWeapon(WeaponType.CORK_GUN);
        }

        private void picWeaponSelectWaterGun_MouseEnter(object sender, EventArgs e)
        {
            textBox2.Visible = true;
        }

        private void picWeaponSelectWaterGun_MouseLeave(object sender, EventArgs e)
        {
            textBox2.Visible = false;
        }

        private void picWeaponSelectWaterGun_Click(object sender, EventArgs e)
        {
            SelectWeapon(WeaponType.WATER_GUN);
        }
        private void picWeaponSelectNerfRev_MouseEnter(object sender, EventArgs e)
        {
            textBox4.Visible = true;
        }

        private void picWeaponSelectNerfRev_MouseLeave(object sender, EventArgs e)
        {
            textBox4.Visible = false;
        }

        private void picWeaponSelectNerfRev_Click(object sender, EventArgs e)
        {
            SelectWeapon(WeaponType.NERF_REVOLVER);
        }
        private void picWeaponSelectBow_MouseEnter(object sender, EventArgs e)
        {
            textBox5.Visible = true;
        }

        private void picWeaponSelectBow_MouseLeave(object sender, EventArgs e)
        {
            textBox5.Visible = false;
        }

        private void picWeaponSelectBow_Click(object sender, EventArgs e)
        {
            SelectWeapon(WeaponType.BOW);
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();

        }

        private void tmrPlayMusicAfterGameOver_Tick(object sender, EventArgs e)
        {
            if (btnStart.Visible)
            {
                soundPlayer.PlayLooping();
            }
            tmrPlayMusicAfterGameOver.Enabled = false;
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            if (state == -1)
            {
                if (global::RRRPG.FrmTitle.isMuted)
                {
                    global::RRRPG.FrmTitle.isMuted = false;
                    btnMute.Image = Resources.not_muted;
                    soundPlayer.PlayLooping();
                }
                else
                {
                    global::RRRPG.FrmTitle.isMuted = true;
                    btnMute.Image = Resources.muted;
                    soundPlayer.Stop();
                }
            }
        }
        private void picPlayer_Click(object sender, EventArgs e)
        {

        }

        private void back_Click(object sender, EventArgs e)
        {
            // Close the current form
            this.Hide();

            // Open FrmTitle
            FrmTitle frmTitle = new FrmTitle();
            frmTitle.SetWeaponData(weaponData);
            frmTitle.Show();
        }

        private void btnWeather_Click(object sender, EventArgs e)
        {
            string apiKey = "784b268b080333f362b3e803c3e3bc12"; // Replace with your OpenWeatherMap API key
            string city = "Ruston";
            string state = "Louisiana";
            string country = "US";

            string url = $"http://api.openweathermap.org/data/2.5/weather?q={city},{state},{country}&appid={apiKey}";

            try
            {
                using (WebClient client = new WebClient())
                {
                    string response = client.DownloadString(url);
                    JObject jsonObject = JObject.Parse(response);

                    // Extract the necessary weather information from the JSON response
                    string description = jsonObject["weather"][0]["description"].ToString();
                    double temperatureInCelsius = (double)jsonObject["main"]["temp"] - 273.15;

                    // Convert temperature to Fahrenheit and round to two decimal places
                    double temperatureInFahrenheit = Math.Round(temperatureInCelsius * 9 / 5 + 32, 2);

                    // Display the weather information in lblWeather
                    lblWeather.Text = $"Weather in {city}, {state}, {country}:{Environment.NewLine}Description: {description}{Environment.NewLine}Temperature: {temperatureInFahrenheit}�F";

                    // Set the background image for lblWeather
                    lblWeather.BackgroundImage = RRRPG.Properties.Resources.rain;
                }
            }
            catch (Exception ex)
            {
                lblWeather.Text = $"Error retrieving weather information: {ex.Message}";
            }
        }





    }
}