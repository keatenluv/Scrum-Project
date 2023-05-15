using RRRPG.Properties;
using RRRPGLib;
using System.Media;

namespace RRRPG;

public partial class FrmTitle : Form {
  private SoundPlayer soundPlayer;
  public static bool isMuted = false;
  public Dictionary<string, float> weaponData = new Dictionary<string, float>();

  public FrmTitle() {
    InitializeComponent();
    FormManager.openForms.Add(this);
    weaponData.Add("Cork Gun", 0.4f);
    weaponData.Add("Water Gun", 0.1f);
    weaponData.Add("Nerf Revolver", 0.3f);
    weaponData.Add("Bow", 0.2f);
    weaponData.Add("Magic Wand", 0.2f);
    }

  public void SetWeaponData(Dictionary<string, float> weapons)
    {
        weaponData = weapons;
    }

  private void btnPlay_Click(object sender, EventArgs e) {
    ResourcesRef.Resources = Resources.ResourceManager;
    Hide();
    soundPlayer.Stop();
    FrmMain frmMain = new FrmMain();
    frmMain.SetWeaponData(weaponData);
    frmMain.ShowDialog();
    FormManager.openForms.Add(frmMain);
  }

  private void FrmTitle_Load(object sender, EventArgs e) {
    soundPlayer = new SoundPlayer(Resources.Mus_Title_Bg_Music_3);
    if (!isMuted)
    {
        soundPlayer.PlayLooping();

    }
    else { btnMute.Image = Resources.muted; }
    FormManager.openForms.Add(this);
  }

  private void FrmTitle_FormClosed(object sender, FormClosedEventArgs e) {
        Application.Exit();
    }

  private void btnMute_Click(object sender, EventArgs e)
  {
    if (isMuted){
        isMuted = false;
        soundPlayer.PlayLooping();
        btnMute.Image = Resources.not_muted;
    }
    else{
        isMuted = true;
        soundPlayer.Stop();
        btnMute.Image = Resources.muted;
    }
  }

    private void btnSettings_Click(object sender, EventArgs e)
    {
        Hide();
        soundPlayer.Stop();
        Setting setting = new Setting();
        setting.SetWeaponData(weaponData);
        setting.Owner = this;
        setting.ShowDialog();
        setting.Text = "Settings";
        /*using (Setting setting = new Setting())
        {
            setting.ShowDialog();
        }*/
    }
}
