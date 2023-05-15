namespace RRRPGLib;

/// <summary>
/// Possible results to pulling the trigger
/// </summary>
public enum PullTriggerResult
{
    /// <summary>
    /// Happends when there was a bullet in the chamber
    /// A player still has a chance to dodge
    /// </summary>
    FIRED,
    /// <summary>
    /// This is the default value before we 
    /// conclude what the real value is. Its just
    /// here to initialize the variable and shouldn't be
    /// used for anything more than that
    /// </summary>
    UNKNOWN,

    /// <summary>
    /// The weapon went off and was not dodged
    /// </summary>
    GOT_SHOT,

    /// <summary>
    /// The weapon went off but was successfully dodged.
    /// I recommend making an animated gif showing the character
    /// dodging the weapon's discharge.
    /// </summary>
    WENT_OFF_BUT_DODGED,

    /// <summary>
    /// The weapon tried to go off, but misfired. There is a small
    /// chance of this happening based on the weapon. I recommend
    /// making an animated gif showing the character being relieved
    /// that the weapon misfired
    /// </summary>
    MISFIRE,

    /// <summary>
    /// Bullet wasn't in the chamber. Nothing happened.
    /// </summary>
    DIDNT_GO_OFF,
}


/// <summary>
/// Class for one of the Russian roulette weapons
/// </summary>
public class Weapon
{
    #region Public Fields / Properties
    /// <summary>
    /// List of chambers. The count of items in the list denotes
    /// the number of chambers. A true value indicates a "bullet"
    /// in that chamber whereas a false value indicates the chamber
    /// is empty. This is currently not implemented but is definitely
    /// open for you to include in your version of the game
    /// </summary>
    public List<bool> Chambers { get; private init; }

    /// <summary>
    /// If you include the list of chambers, use this to hold on
    /// to the current chamber index. You should randomize this 
    /// at the start of the game and re-randomize it if the player
    /// re-spins the chamber.
    /// </summary>
    public int CurChamberIndex { get; private set; }

    /// <summary>
    /// The type of weapon this is, see <see cref="WeaponType"/>
    /// </summary>
    public WeaponType Type { get; private init; }

  /// <summary>
  /// The chance that the weapon could misfire. A value of 0.0
  /// means the weapon will never misfire. A value of 1.0 means
  /// the weapon always misfires. A value of 0.5 means there is
  /// a 50% chance of misfire. And so on.
  /// </summary>
  public float ChanceOfMisfire { get; set; }

    /// <summary>
    /// The amount of damage this weapon is capable of dealing if
    /// it goes off and the character doesn't dodge. This is currently
    /// not implemented. Each character has a health and each weapon 
    /// has a different amount of damage, so all the pieces are there
    /// if you want to implement this.
    /// </summary>
    public float Damage { get; private init; }

    /// <summary>
    /// Used to combat the dodging ability of the character. This is done
    /// by taking the <see cref="Stats.Reflex"/> of the character and subtracting
    /// this weapons velocity. The result is the chance to dodge.
    /// </summary>
    public float Velocity { get; private init; }
    #endregion



  #region Private Fields / Properties
  /// <summary>
  /// Used to generate random numbers for pulling the trigger
  /// </summary>
  private Random rand;
  #endregion

    #region Ctor
    /// <summary>
    /// Explicit constructor. This is NOT to be used directly and is marked as private to enforce this
    /// </summary>
    /// <param name="chambers">Sets field <see cref="Chambers"/></param>
    /// <param name="type">Sets field <see cref="Type"/></param>
    /// <param name="chanceOfMisfire">Sets field <see cref="ChanceOfMisfire"/></param>
    /// <param name="damage">Sets field <see cref="Damage"/></param>
    /// <param name="velocity">Sets field <see cref="Velocity"/></param>
    private Weapon(List<bool> chambers, WeaponType type, float chanceOfMisfire, float damage, float velocity)
    {
        Chambers = chambers;
        Type = type;
        ChanceOfMisfire = chanceOfMisfire;
        Damage = damage;
        Velocity = velocity;
        rand = new Random();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Simulates pulling the trigger of the weapon. This is fired after clicking the "Try your luck"
    /// button or by the opponent. 
    /// </summary>
    /// <param name="character">The character that holds the weapon and is trying their luck. Used 
    /// to retrieve the correct stats so calculations can be made</param>
    /// <returns>A <see cref="PullTriggerResult"/> enum value that denotes the result</returns>
    public PullTriggerResult PullTrigger(Character character)
    {
        if (CurChamberIndex == Chambers.Count)
            CurChamberIndex = 0;

        if (!Chambers[CurChamberIndex++])
            return PullTriggerResult.DIDNT_GO_OFF;

        if (ChanceOfMisfire < rand.NextSingle())
            return PullTriggerResult.DIDNT_GO_OFF;

        return PullTriggerResult.UNKNOWN;
    }
    /// <summary>
    /// Instead of using the constructor, use this method to create a new
    /// weapon. Just send it the weapon type and a brand new weapon will
    /// be created with the proper stats
    /// </summary>
    /// <param name="type">The type of weapon to create</param>
    /// <returns>A newly created weapon based on the type sent</returns>
    public static Weapon MakeWeapon(WeaponType type)
    {
        Weapon weapon = type switch
        {
            WeaponType.BOW => MakeBow(),
            WeaponType.WATER_GUN => MakeWaterGun(),
            WeaponType.CORK_GUN => MakeCorkGun(),
            WeaponType.NERF_REVOLVER => MakeNerfRev(),
            WeaponType.MAGIC_WAND => MakeMagicWand(),
            _ => null
        };
        weapon.PopulateChamber();
        return weapon;
    }
    #endregion

    #region Private Methods

    private int RamdomChamber()
    {
        return rand.Next(0, Chambers.Capacity);
    }

    private void PopulateChamber()
    {
        CurChamberIndex = 0;
        int chamber = RamdomChamber();
        for (int i = 0; i < Chambers.Capacity; i++)
        {
            Chambers.Add(false);
            if (i == chamber) { Chambers[i] = true; }
        }
    }
    /// <summary>
    /// Used to retrieve a random value from 0.0 to 1.0
    /// </summary>
    /// <returns>A random value from 0.0 to 1.0</returns>
    public float RandNumber()
    {
        return rand.NextSingle();
    }

    private static Weapon MakeNerfRev()
    {
        return new Weapon(
          chambers: new List<bool>(6),
          type: WeaponType.NERF_REVOLVER,
          chanceOfMisfire: 0.3f,
          damage: 25,
          velocity: 0.4f);
    }
    private static Weapon MakeCorkGun()
    {
        return new Weapon(
          chambers: new List<bool>(4),
          type: WeaponType.CORK_GUN,
          chanceOfMisfire: 0.4f,
          damage: 40,
          velocity: 0.2f);
    }
    private static Weapon MakeWaterGun()
    {
        return new Weapon(
          chambers: new List<bool>(8),
          type: WeaponType.WATER_GUN,
          chanceOfMisfire: 0.1f,
          damage: 20,
          velocity: 0.5f);
    }
    private static Weapon MakeBow()
    {
        return new Weapon(
          chambers: new List<bool>(3),
          type: WeaponType.BOW,
          chanceOfMisfire: 0.2f,
          damage: 50,
          velocity: 0.7f);
    }
    private static Weapon MakeMagicWand()
    {
        return new Weapon(
          chambers: new List<bool>(5),
          type: WeaponType.MAGIC_WAND,
          chanceOfMisfire: 0.3f,
          damage: 100,
          velocity: 0.2f);
    }
    #endregion
}
