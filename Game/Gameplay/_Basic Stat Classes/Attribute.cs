// Attribute.cs
// May 30, 2012
// 
// Class for all character attributes in game.

public class Attribute : BaseStat
{
}

/// <summary>
//Enum with list of attributes names.
/// </summary>
public enum AttributeName
{
    Strength, //physical strength. Increases melee attack and armor
    Dexterity, //agility, Increases ranged attack
    Intelligence, //mental acuity. Increases magic skills power and magic defence
    Constitution, //health and endurance. Increases stamina
    Charisma,     //leadership, charm, personality
    COUNT
}
