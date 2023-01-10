using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "Movement", menuName = "Assets/Character", order = 0)]

public class CharacterBase : ScriptableObject
{
    #region General
    [Header("General")]
    [SerializeField] private new string name;
    [SerializeField] private string fullName;
    [SerializeField] private string ancestry;
    [SerializeField] private string gender;
    [SerializeField] private string origin;
    [SerializeField] private int born;
    [SerializeField] private string positionPrefix;
    [SerializeField] private CombatPosition position;
    [SerializeField] private string hairColor;
    [SerializeField] private string eyeColor;
    [SerializeField] private string occupation;
    [SerializeField] private string affiliations;

    [TextArea(10, 20)]
    [SerializeField] private string description;
    [SerializeField] private Sprite cardImage;
    [SerializeField] private Sprite iconImage;

    public string Name { get { return name; } }
    public string FullName { get { return fullName; } }
    public string Ancestry { get { return ancestry; } }
    public string Gender { get { return gender; } }
    public string Origin { get { return origin; } }
    public int Born { get { return born; } }
    public string PositionPrefix { get { return positionPrefix; } }
    public CombatPosition Position { get { return position; } }
    public string HairColor { get { return hairColor; } }
    public string EyeColor { get { return eyeColor; } }
    public string Occupation { get { return occupation; } }
    public string Affiliations { get { return affiliations; } }
    public string Description { get { return description; } }
    public Sprite CardImage { get { return cardImage; } }
    public Sprite IconImage { get { return iconImage; } }

    public string FullDescription //stats not included
    {
        get
        {
            string fd = "Full Name: " + fullName + "\n"
                + "Ancestry: " + ancestry + "\n"
                + "Gender: " + gender + "\n"
                + "Origin: " + origin + "\n"
                + "Born: " + born + "\n"
                + "Position: " + positionPrefix + " " + position + "\n"
                + "Hair Color: " + hairColor + "\n"
                + "Eye Color: " + eyeColor + "\n"
                + "Affiliations: " + affiliations + "\n\n"
                + "Description: " + description + "\n"
                ;
            return fd;
        }
    }
    #endregion

    #region Stats
    [Header("Stats")]
    [SerializeField] ModeType modeType;
    [SerializeField] int maxStage;
    [SerializeField] float initialMight;
    [SerializeField] float initialDexterity;
    [SerializeField] float initialCharm;
    [SerializeField] float initialInitiation = 0;

    [SerializeField] List<LearnableAbility> learnableAbility;
    public List<LearnableAbility> LearnableAbility { get { return learnableAbility; } }
    public ModeType ModeType { get { return modeType; } }
    public int MaxStage { get { return maxStage; } }
    public float InitialMight { get { return initialMight; } }
    public float InitialDexterity { get { return initialDexterity; } }
    public float InitialCharm { get { return initialCharm; } }
    public float InitialInitiation { get { return initialInitiation; } }
    #endregion

    #region Movement
    [Header("Movement")]
    [SerializeField] private MovementType movementType;
    [SerializeField] private int movementRange;

    public MovementType MovementType { get { return movementType; } }
    public int MovementRange { get { return movementRange; } }
    #endregion
}

[System.Serializable]
public class LearnableAbility
{
    [SerializeField] string abilityName;
    [SerializeField] AbilityBase aBase;
    [SerializeField] int stage;
    
    public AbilityBase ABase
    {
        get { return aBase; }
    }
    public int Stage
    {
        get { return stage; }
    }
    public string AbilityName
    {
        get { return abilityName; }
    }
}
public enum ModeType
{
    Might,
    Dexterity,
    Charm
}
public enum CombatPosition
{
    Assailant,
    Reinforcer,
    Controller,
    Operative
}
public enum MovementType
{
    PawnUp,
    PawnDown,
    PawnRight,
    PawnLeft,
    PawnHorizontal,
    PawnVertical,
    Rook,
    BishopUp,
    BishopDown,
    Bishop,
    Queen,
    Horse
}

public enum Stat
{
    Might,
    Dexterity,
    Charm,
    HP,
    REGEN,
    CP,
    AC,

    //Boosting Stats
    Accuracy
}

