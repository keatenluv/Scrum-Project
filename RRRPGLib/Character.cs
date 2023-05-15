using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using static RRRPGLib.ResourcesRef;
using ImgState = RRRPGLib.CharacterImgStateType;
using TalkState = RRRPGLib.CharacterTalkStateType;

namespace RRRPGLib;

/// <summary>
/// Standard class for creating a character
/// </summary>
public class Character
{
    #region Public Fields / Properties
    /// <summary>
    /// Holds the stats for this character
    /// </summary>
    // Public property for the opponent's name
    public string Name { get; set; }
    public Stats Stats { get; private set; }
    public string Type { get; private set; }
    #endregion

    #region Private Fields / Properties
    private FortitudeType fortitude;
    private PictureBox pic;
    private Label lblTalk;
    private Dictionary<ImgState, Bitmap> imgMap;
    private Dictionary<TalkState, (string dialog, UnmanagedMemoryStream audio)> dialogMap;
    #endregion

    #region Constructor
    /// <summary>
    /// Default constructor that just calls the explicit constructor
    /// sending it null for the arguments
    /// </summary>
    public Character() : this(null, null)
    {

    }

  /// <summary>
  /// Explicit constructor
  /// </summary>
  /// <param name="pic">Holds the picture box for the character</param>
  /// <param name="lblTalk">Holds the label to contain character's dialog</param>
  public Character(PictureBox pic, Label lblTalk) {
    
    this.pic = pic;
    this.lblTalk = lblTalk;
    this.fortitude = FortitudeType.NORMAL;
  }
  #endregion
 
  #region Public Methods
  /// <summary>
  /// Create an opponent suitable for the given weapon
  /// </summary>
  /// <param name="weaponType">Type of weapon to create the opponent for</param>
  /// <param name="pic">PictureBox to hold the image of the opponent</param>
  /// <param name="lblTalk">Label to hold the opponent's dialog</param>
  /// <returns>The opponent created</returns>
  public static Character MakeOpponent(WeaponType weaponType, PictureBox pic, Label lblTalk) {
    Character c = weaponType switch {
      WeaponType.MAGIC_WAND => MakeMagicWandOpponent(),
      WeaponType.NERF_REVOLVER => MakeNerfRevolverOpponent(),
      WeaponType.BOW => MakeBowOpponent(),
      WeaponType.CORK_GUN => MakeCorkGunOpponent(),
      WeaponType.WATER_GUN => MakeWaterGunOpponent(),
    };

        // Assign specific names based on the weapon type
        c.Name = weaponType switch
        {
            WeaponType.CORK_GUN => "Willy Wonka    ",
            WeaponType.MAGIC_WAND => "   Gandalf   ",
            WeaponType.WATER_GUN => "    Fizz     ",
            WeaponType.NERF_REVOLVER => "    Robert    ",
            WeaponType.BOW => "   Yoshi   ",
            _ => c.Name // Use the default name if no specific name is defined
        } ;

    c.pic = pic;
    c.Type = "opponent";
    c.lblTalk = lblTalk;
    c.ShowIdle();
    c.Shutup();
    return c;
  }

  /// <summary>
  /// Create a player with the appropriate animations and dialog for the given weapon
  /// </summary>
  /// <param name="weaponType">Type of weapon to create the player for</param>
  /// <param name="pic">PictureBox to hold the image of the player</param>
  /// <param name="lblTalk">Label to hold the player's dialog</param>
  /// <returns></returns>
  public static Character MakePlayer(WeaponType weaponType, PictureBox pic, Label lblTalk) {
    Character c = weaponType switch {
      WeaponType.MAGIC_WAND => MakeMagicWandPlayer(),
      WeaponType.NERF_REVOLVER => MakeNerfRevolverPlayer(),
      WeaponType.BOW => MakeBowPlayer(),
      WeaponType.CORK_GUN => MakeCorkGunPlayer(),
      WeaponType.WATER_GUN => MakeWaterGunPlayer(),
    };
        
    c.Type = "player";
    c.pic = pic;
    c.lblTalk = lblTalk;
    c.ShowIdle();
    c.Shutup();
    return c;
  }
    
    /// <summary>
    /// Handles what happens when the character pulls the trigger of the weapon
    /// </summary>
    /// <param name="weapon">Weapon in play</param>
    /// <returns>True if the character got shot, false otherwise</returns>
    public bool PullTrigger(Weapon weapon) {
    var result = weapon.PullTrigger(this);
        var result = weapon.PullTrigger(this);
        if (result == PullTriggerResult.DIDNT_GO_OFF)
            return false;

        return true;
    }

    /// <summary>
    /// Allows the character to say the dialog that goes with the <see cref="TalkState.TALK_SMACK"/> state
    /// </summary>
    public void SaySmack() => Say(dialogMap[TalkState.TALK_SMACK]);

    /// <summary>
    /// Allows the character to say the dialog that goes with the <see cref="TalkState.SAY_OW"/> state
    /// </summary>
    public void SayOw() => Say(dialogMap[TalkState.SAY_OW]);

    /// <summary>
    /// Allows the character to say the dialog that goes with the <see cref="TalkState.BONED"/> state
    /// </summary>
    public void SayBoned() => Say(dialogMap[TalkState.BONED]);

    /// <summary>
    /// Allows the character to say the dialog that goes with the <see cref="TalkState.GUN_WENT_OFF"/> state
    /// </summary>
    public void SayGunWentOff() => Say(dialogMap[TalkState.GUN_WENT_OFF]);

    /// <summary>
    /// Allows the character to say the dialog that goes with the <see cref="TalkState.SURVIVED"/> state
    /// </summary>
    public void SaySurvived() => Say(dialogMap[TalkState.SURVIVED]);

    /// <summary>
    /// Temporarily stops the character from talking and hides their dialog label
    /// </summary>
    public void Shutup()
    {
        lblTalk.Text = "";
        lblTalk.Visible = false;
    }

    /// <summary>
    /// Allows the character to show the animation/image that goes with the <see cref="ImgState.IDLE"/> state
    /// </summary>
    public void ShowIdle() => Show(imgMap[ImgState.IDLE]);

    /// <summary>
    /// Allows the character to show the animation/image that goes with the <see cref="ImgState.NO_WEAPON"/> state
    /// </summary>
    public void ShowNoWeapon() => Show(imgMap[ImgState.NO_WEAPON]);

    /// <summary>
    /// Allows the character to show the animation/image that goes with the <see cref="ImgState.READY"/> state
    /// </summary>
    public void ShowReady() => Show(imgMap[ImgState.READY]);

    /// <summary>
    /// Allows the character to show the animation/image that goes with the <see cref="ImgState.KILL"/> state
    /// </summary>
    public void ShowKill() => Show(imgMap[ImgState.KILL]);

    #endregion

    #region Private Methods
    private static Character MakeWaterGunOpponent()
    {
        Character c = new Character();
        c.Stats = new(luck: 0.7f, health: 100, reflex: 0.1f);
        c.imgMap = new() {
      {ImgState.IDLE, Resources.GetObject("Img_FireGoblin_Idle") as Bitmap },
      {ImgState.NO_WEAPON, Resources.GetObject("Img_FireGoblin_NoWeapon") as Bitmap },
      {ImgState.READY, Resources.GetObject("Img_FireGoblin_Ready") as Bitmap },
      {ImgState.KILL, Resources.GetObject("Img_FireGoblin_Kill") as Bitmap },
    };
        // create dialog map
        c.dialogMap = new() {
      {TalkState.TALK_SMACK, ("Why did I choose a water gun?", null) },
      {TalkState.SAY_OW, ("Ow", null) },
      {TalkState.BONED, ("So cold!", null) },
      {TalkState.GUN_WENT_OFF, ("Oh noes",Resources.GetStream("FireGoblin1")) },
      {TalkState.SURVIVED, ("My fire continues to roar!", Resources.GetStream("FireGoblin2")) },
    };
        return c;
    }
    private static Character MakeCorkGunOpponent()
    {
        Character c = new Character();
        c.Stats = new(luck: 0.0f, health: 100, reflex: 0.0f);
        c.imgMap = new() {
      {ImgState.IDLE, Resources.GetObject("willyWonka") as Bitmap },
      {ImgState.NO_WEAPON, Resources.GetObject("willyWonkaNoGun") as Bitmap },
      {ImgState.READY, Resources.GetObject("willyWonkaSurvives") as Bitmap },
      {ImgState.KILL, Resources.GetObject("wonka_dying") as Bitmap },
    };
    c.dialogMap = new() {
      {TalkState.TALK_SMACK, ("Welcome!",Resources.GetStream("Willy1")) },
      {TalkState.SAY_OW, ("A little nonsense now and then is relished by the wisest men.", null) },
      {TalkState.BONED, ("Is it my soul that calls upon my name?", null) },
      {TalkState.GUN_WENT_OFF, ("Candy is dandy, but liquor is quicker.",Resources.GetStream("Wonka3")) },
      {TalkState.SURVIVED, ("The suspense is terrible. I hope it’ll last.",Resources.GetStream("Willy2")) },
    };
    return c;
  }
  private static Character MakeBowOpponent() {
    Character c = new Character();
    c.Stats = new(luck: 0.0f, health: 100, reflex: 0.0f);
    c.imgMap = new() {
      {ImgState.IDLE, Resources.GetObject("Yoshi_Idle") as Bitmap },
      {ImgState.NO_WEAPON, Resources.GetObject("Yoshi_NoWeapon") as Bitmap },
      {ImgState.READY, Resources.GetObject("Yoshi_Bow") as Bitmap },
      {ImgState.KILL, Resources.GetObject("Yoshi_Dying") as Bitmap },
    };
    c.dialogMap = new() {

      {TalkState.TALK_SMACK, ("YOSHI", Resources.GetStream("yoshi")) },
      {TalkState.SAY_OW, ("", Resources.GetStream("yoshi_tongue")) },
      {TalkState.BONED, ("", Resources.GetStream("discord_leave")) },
      {TalkState.GUN_WENT_OFF, ("",  Resources.GetStream("discord_leave")) },
      {TalkState.SURVIVED, ("", null) },
    };
    return c;
  }
  private static Character MakeNerfRevolverOpponent() {
    Character c = new Character();
    c.Stats = new(luck: 0.0f, health: 100, reflex: 0.0f);
    c.imgMap = new() {
      {ImgState.IDLE, Resources.GetObject("Nerk_Kid_Ready") as Bitmap },
      {ImgState.NO_WEAPON, Resources.GetObject("Nerf_Kid") as Bitmap },
      {ImgState.READY, Resources.GetObject("Nerf_Kid_Ready") as Bitmap },
      {ImgState.KILL, Resources.GetObject("nerf_death") as Bitmap },
    };
    c.dialogMap = new() {
      {TalkState.TALK_SMACK, ("WELCOME TO NERF FEST", Resources.GetStream("NERF_FEST")) },
      {TalkState.SAY_OW, ("", null) },
      {TalkState.BONED, ("", null) },
      {TalkState.GUN_WENT_OFF, ("", Resources.GetStream("nerf_death1")) },
      {TalkState.SURVIVED, ("YOU GOT TROUBLE NOW", Resources.GetStream("YOU_GOT_TROUBLE")) },
    };
        return c;
    }
    private static Character MakeMagicWandOpponent()
    {
        Character c = new Character();
        c.Stats = new(luck: 0.4f, health: 100, reflex: 0.5f);
        c.imgMap = new() {
      {ImgState.IDLE, Resources.GetObject("Img_Wizard_Idle") as Bitmap },
      {ImgState.NO_WEAPON, Resources.GetObject("Img_Wizard_NoWeapon") as Bitmap },
      {ImgState.READY, Resources.GetObject("Img_Wizard_Ready") as Bitmap },
      {ImgState.KILL, Resources.GetObject("Img_Wizard_Kill") as Bitmap },
    };
    c.dialogMap = new() {
      {TalkState.TALK_SMACK, ("I will outlast you!", Resources.GetStream("Wizard1")) },
      {TalkState.SAY_OW, ("This hurts more than I thought!", null) },
      {TalkState.BONED, ("That didn't go as planned", null) },
      {TalkState.GUN_WENT_OFF, ("Well, that sucks!", Resources.GetStream("Wizard3")) },
      {TalkState.SURVIVED, ("I knew I would live!", Resources.GetStream("Wizard2")) },
    };
        return c;
    }

    private static Character MakeMagicWandPlayer()
    {
        Character c = new Character();
        c.Stats = new(luck: 0.5f, health: 100, reflex: 0.3f);
        c.imgMap = new() {
      {ImgState.IDLE, Resources.GetObject("Img_Bender_Idle") as Bitmap },
      {ImgState.NO_WEAPON, Resources.GetObject("Img_Bender_Idle") as Bitmap },
      {ImgState.READY, Resources.GetObject("Img_Bender_Ready_MagicWand") as Bitmap },
      {ImgState.KILL, Resources.GetObject("Img_Bender_Kill_MagicWand") as Bitmap },
    };
        c.dialogMap = new() {
      {TalkState.TALK_SMACK, ("Bite my shiny metal ass!", Resources.GetStream("Snd_Bender_BiteMyShinyMetalAss")) },
      {TalkState.SAY_OW, ("Ow ow ow!!!", null) },
      {TalkState.BONED, ("Oh, I'm boned!", Resources.GetStream("Snd_Bender_ImBoned")) },
      {TalkState.GUN_WENT_OFF, ("Oh My God!", Resources.GetStream("Snd_Bender_OhMyGod")) },
      {TalkState.SURVIVED, ("Hahahaha!", Resources.GetStream("Snd_Bender_Laugh")) },
    };
        return c;
    }
    private static Character MakeNerfRevolverPlayer()
    {
        Character c = new Character();
        c.Stats = new(luck: 0.5f, health: 100, reflex: 0.3f);
        c.imgMap = new() {
      {ImgState.IDLE, Resources.GetObject("Img_Bender_Idle") as Bitmap },
      {ImgState.NO_WEAPON, Resources.GetObject("Img_Bender_Idle") as Bitmap },
      {ImgState.READY, Resources.GetObject("Bener_Ready_Nerf") as Bitmap },
      {ImgState.KILL, Resources.GetObject("bender_dying_nerf") as Bitmap },
    };
        c.dialogMap = new() {
      {TalkState.TALK_SMACK, ("Bite my shiny metal ass!", Resources.GetStream("Snd_Bender_BiteMyShinyMetalAss")) },
      {TalkState.SAY_OW, ("Ow ow ow!!!", null) },
      {TalkState.BONED, ("Oh, I'm boned!", Resources.GetStream("Snd_Bender_ImBoned")) },
      {TalkState.GUN_WENT_OFF, ("Oh My God!", Resources.GetStream("Snd_Bender_OhMyGod")) },
      {TalkState.SURVIVED, ("Hahahaha!", Resources.GetStream("Snd_Bender_Laugh")) },
    };
        return c;
    }
    private static Character MakeBowPlayer()
    {
        Character c = new Character();
        c.Stats = new(luck: 0.5f, health: 100, reflex: 0.3f);
        c.imgMap = new() {
      {ImgState.IDLE, Resources.GetObject("Img_Bender_Idle") as Bitmap },
      {ImgState.NO_WEAPON, Resources.GetObject("Img_Bender_Idle") as Bitmap },
      {ImgState.READY, Resources.GetObject("Img_Bender_Ready_MagicWand") as Bitmap },
      {ImgState.KILL, Resources.GetObject("Img_Bender_Kill_MagicWand") as Bitmap },
    };
        c.dialogMap = new() {
      {TalkState.TALK_SMACK, ("Bite my shiny metal ass!", Resources.GetStream("Snd_Bender_BiteMyShinyMetalAss")) },
      {TalkState.SAY_OW, ("Ow ow ow!!!", null) },
      {TalkState.BONED, ("Oh, I'm boned!", Resources.GetStream("Snd_Bender_ImBoned")) },
      {TalkState.GUN_WENT_OFF, ("Oh My God!", Resources.GetStream("Snd_Bender_OhMyGod")) },
      {TalkState.SURVIVED, ("Hahahaha!", Resources.GetStream("Snd_Bender_Laugh")) },
    };
        return c;
    }
    private static Character MakeCorkGunPlayer()
    {
        Character c = new Character();
        c.Stats = new(luck: 0.5f, health: 100, reflex: 0.3f);
        c.imgMap = new() {
      {ImgState.IDLE, Resources.GetObject("Img_Bender_Idle") as Bitmap },
      {ImgState.NO_WEAPON, Resources.GetObject("Img_Bender_Idle") as Bitmap },
      {ImgState.READY, Resources.GetObject("Img_Bender_Ready_MagicWand") as Bitmap },
      {ImgState.KILL, Resources.GetObject("Img_Bender_Kill_MagicWand") as Bitmap },
    };
        c.dialogMap = new() {
      {TalkState.TALK_SMACK, ("Bite my shiny metal ass!", Resources.GetStream("Snd_Bender_BiteMyShinyMetalAss")) },
      {TalkState.SAY_OW, ("Ow ow ow!!!", null) },
      {TalkState.BONED, ("Oh, I'm boned!", Resources.GetStream("Snd_Bender_ImBoned")) },
      {TalkState.GUN_WENT_OFF, ("Oh My God!", Resources.GetStream("Snd_Bender_OhMyGod")) },
      {TalkState.SURVIVED, ("Hahahaha!", Resources.GetStream("Snd_Bender_Laugh")) },
    };
        return c;
    }
    private static Character MakeWaterGunPlayer()
    {
        Character c = new Character();
        c.Stats = new(luck: 0.5f, health: 100, reflex: 0.3f);
        c.imgMap = new() {
      {ImgState.IDLE, Resources.GetObject("Img_Bender_Idle") as Bitmap },
      {ImgState.NO_WEAPON, Resources.GetObject("Img_Bender_Idle") as Bitmap },
      {ImgState.READY, Resources.GetObject("Img_Bender_Ready_WaterGun") as Bitmap },
      {ImgState.KILL, Resources.GetObject("Img_Bender_Kill_WaterGun") as Bitmap },
    };
        c.dialogMap = new() {
      {TalkState.TALK_SMACK, ("You look thirsty. Want some water?", null) },
      {TalkState.SAY_OW, ("It's freezing!", null) },
      {TalkState.BONED, ("Oh, I'm boned!", Resources.GetStream("Snd_Bender_ImBoned")) },
      {TalkState.GUN_WENT_OFF, ("This might not be so bad", null) },
      {TalkState.SURVIVED, ("Hahahaha!", Resources.GetStream("Snd_Bender_Laugh")) },
    };
        return c;
    }

    private void Say(string dialog)
    {
        Say((dialog, null));
    }
    private void Say((string dialog, UnmanagedMemoryStream audio) msg)
    {
        lblTalk.Text = msg.dialog;
        lblTalk.Visible = true;
        if (msg.audio is not null)
        {
            SoundManager.Play(msg.audio);
        }
    }

    private void Show(Bitmap img)
    {
        pic.Image = img;
    }
    #endregion
}
