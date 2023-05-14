using RRRPG.Properties;
using RRRPGLib;
using System.Media;

namespace RRRPG;

public partial class FrmTitle : Form {
  private SoundPlayer soundPlayer;
  public static bool isMuted = false;
  
  public FrmTitle() {
    InitializeComponent();
  }

  private void btnPlay_Click(object sender, EventArgs e) {
    ResourcesRef.Resources = Resources.ResourceManager;
    Hide();
    soundPlayer.Stop();
    FrmMain frmMain = new FrmMain();
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
    FormManager.openForms.Remove(this);
    FormManager.CloseAll();
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
        setting.Owner = this;
        setting.ShowDialog();
        setting.Text = "Settings";
        /*using (Setting setting = new Setting())
        {
            setting.ShowDialog();
        }*/
    }
}
