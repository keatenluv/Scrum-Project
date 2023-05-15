using RRRPG.Properties;
using RRRPGLib;
using System.Media;

namespace RRRPG
{
    public partial class FrmMain : Form
    {
        private bool debug;
        private int state;
        private int _points;
        private int points
        {
            get { return _points; }
            set
            {
                _points = value;
                PointsLabel.Text = $"Points: {value}";
            }
        }
        private Rectangle rect = new Rectangle(0, 0, 7, 100);
        private CancellationTokenSource tokenSource = new();
        private CancellationToken token;
        private Graphics g;
        private SoundPlayer soundPlayer;
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
            token = tokenSource.Token;
            debug = true;
            KeyPreview = true;
            g = panel1.CreateGraphics();
            soundPlayer = new SoundPlayer(Resources.Mus_Title_Bg_Music);
            soundPlayer.PlayLooping();
            //labelAmmo.Visible = false;
            btnDoIt.Visible = false;
            lblOpponentSpeak.Visible = false;
            lblPlayerSpeak.Visible = false;
            weapon = Weapon.MakeWeapon(WeaponType.MAGIC_WAND);
            state = -1;
            points = 0;
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
                points = 0;
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
                    if (!QuickTimeEvent(opponent).Result)
                    {
                        opponent.ShowKill();
                        opponent.SayGunWentOff();
                        state = 7;
                        return;
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
                points += 100;
                opponent.SayBoned();
                btnStart.Visible = true;
                tmrPlayMusicAfterGameOver.Enabled = true;
                panWeaponSelect.Visible = true;
                state = -1;
                tmrStateMachine.Enabled = false;
            }
        }

        private async void btnDoIt_Click(object sender, EventArgs e)
        {
            //Chamber had bullet
            if (player.PullTrigger(weapon))
            {
                //Player failed Quick Time event
                if (!await QuickTimeEvent(player))
                {
                    player.ShowKill();
                    player.SayGunWentOff();
                    state = 3;
                    tmrStateMachine.Interval = 2200;
                    tmrStateMachine.Enabled = true;
                    return;
                }
                //Player Dodged
                points += 50;
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
            if (debug)
            {
                string ammo = "";
                weapon.Chambers.ForEach(item => ammo += $",{item} ");
                labelAmmo.Text = ammo;
            }
            opponent = Character.MakeOpponent(type, picOpponent, lblOpponentSpeak);
            player = Character.MakePlayer(type, picPlayer, lblPlayerSpeak);
            this.BackgroundImage = weaponBackgroundMap[type];
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        // Returns true if passed
        private async Task<bool> QuickTimeEvent(Character character)
        {
            if (character.Type == "opponent")
            {
                //Simulate chance of opponent to dodge
                if (weapon.RandNumber() < character.Stats.Reflex)
                    return true;

                return false;
            }
            int reactionTime = (int)((1.0f - weapon.Velocity + character.Stats.Reflex) * 1000);
            MessageBox.Show($"{reactionTime}");
            this.KeyPress += FrmMain_KeyPress;
            tmrQuickTimeAnimation.Interval = reactionTime / (panel1.Width / (rect.Width - 2)); // div by number of rects we need
            tmrQuickTimeAnimation.Start();
            var result = false;
            try
            {
                await Task.Delay(reactionTime, token);
            }
            catch (TaskCanceledException)
            {
                result = true;
            }
            ResetQuickActionAnimation();
            tmrQuickTimeAnimation.Stop();
            this.KeyPress -= FrmMain_KeyPress;

            return result;
        }

        private void ResetQuickActionAnimation()
        {
            tmrQuickTimeAnimation.Stop();
            g.Clear(TransparencyKey);
            g.ResetTransform();
        }

        private void FrmMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                tokenSource.Cancel();
            }
        }
        private void tmrQuickTimeAnimation_Tick(object sender, EventArgs e)
        {
            g.FillRectangle(Brushes.Green, rect);
            g.TranslateTransform(rect.Width, 0);
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
            tokenSource.Dispose();
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


    }
}