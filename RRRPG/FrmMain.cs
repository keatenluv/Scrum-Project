using RRRPG.Properties;
using RRRPGLib;
using System.Media;

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

        public FrmMain()
        {
            InitializeComponent();
            FormManager.openForms.Add(this);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            soundPlayer = new SoundPlayer(Resources.Mus_Title_Bg_Music);
            soundPlayer.PlayLooping();
            //labelAmmo.Visible = false;
            btnDoIt.Visible = false;
            lblOpponentSpeak.Visible = false;
            lblPlayerSpeak.Visible = false;
            weapon = Weapon.MakeWeapon(WeaponType.MAGIC_WAND);
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
            // Player Shot
            else if (state == 3)
            {
                player.SayOw();
                state = 4;
                tmrStateMachine.Interval = 2500;
            }
            //Player Dies
            else if (state == 4)
            {
                player.SayBoned();
                btnStart.Visible = true;
                tmrPlayMusicAfterGameOver.Enabled = true;
                panWeaponSelect.Visible = true;
                state = -1;
                tmrStateMachine.Enabled = false;
            }
            // Opponent's turn
            else if (state == 5)
            {
                player.Shutup();
                opponent.ShowReady();
                state = 6;
            }
            //Opponent pulls trigger
            else if (state == 6)
            {
                //Chamber had bullet
                if (opponent.PullTrigger(weapon))
                {
                    //Opponent did not dodge
                    if (!QuickTimeEvent(opponent))
                    {
                        opponent.ShowKill();
                        opponent.SayGunWentOff();
                        state = 7;
                    }
                }
                opponent.ShowNoWeapon();
                opponent.SaySurvived();
                state = 1;
            }
            // Opponent Shot
            else if (state == 7)
            {
                opponent.SayOw();
                state = 8;
                tmrStateMachine.Interval = 2500;
            }
            // Opponent Dies
            else if (state == 8)
            {
                opponent.SayBoned();
                btnStart.Visible = true;
                tmrPlayMusicAfterGameOver.Enabled = true;
                panWeaponSelect.Visible = true;
                state = -1;
                tmrStateMachine.Enabled = false;
            }
        }

        private void btnDoIt_Click(object sender, EventArgs e)
        {
            //Chamber had bullet
            if (player.PullTrigger(weapon))
            {
                //Player failed Quick Time event
                if (!QuickTimeEvent(player))
                {
                    player.ShowKill();
                    player.SayGunWentOff();
                    state = 3;
                    tmrStateMachine.Interval = 2200;
                    tmrStateMachine.Enabled = true;
                    return;
                }

            }
            //Chamber didn't have bullet or dodged.
            player.ShowNoWeapon();
            player.SaySurvived();
            state = 5;
            tmrStateMachine.Interval = 1500;
            tmrStateMachine.Enabled = true;
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
            //DEBUG
            string ammo = "";
            weapon.Chambers.ForEach(item => ammo += $",{item} ");
            labelAmmo.Text = ammo;
            //END
            opponent = Character.MakeOpponent(type, picOpponent, lblOpponentSpeak);
            player = Character.MakePlayer(type, picPlayer, lblPlayerSpeak);
            this.BackgroundImage = weaponBackgroundMap[type];
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        // Returns true if passed
        private bool QuickTimeEvent(Character character)
        {
            if (character.Type == "opponent")
            {
                //Simulate chance of opponent to dodge
                if (weapon.RandNumber() == character.Stats.Reflex)
                    return true;

                return false;
            }
            // Player quick time event
            return true;
        }

        private void picWeaponSelectMagicWand_Click(object sender, EventArgs e)
        {
            SelectWeapon(WeaponType.MAGIC_WAND);
        }

        private void picWeaponSelectCorkGun_Click(object sender, EventArgs e)
        {
            SelectWeapon(WeaponType.CORK_GUN);
        }

        private void picWeaponSelectWaterGun_Click(object sender, EventArgs e)
        {
            SelectWeapon(WeaponType.WATER_GUN);
        }

        private void picWeaponSelectNerfRev_Click(object sender, EventArgs e)
        {
            SelectWeapon(WeaponType.NERF_REVOLVER);
        }

        private void picWeaponSelectBow_Click(object sender, EventArgs e)
        {
            SelectWeapon(WeaponType.BOW);
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormManager.openForms.Remove(this);
            FormManager.CloseAll();
        }

        private void tmrPlayMusicAfterGameOver_Tick(object sender, EventArgs e)
        {
            if (btnStart.Visible)
            {
                soundPlayer.PlayLooping();
            }
            tmrPlayMusicAfterGameOver.Enabled = false;
        }

        private void picPlayer_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}