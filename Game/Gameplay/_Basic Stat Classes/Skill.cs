// Skill.cs
// May 30, 2012
// 
// Skill class, contains the character main skills.

public class Skill : ModifiedStat
{
}

/// <summary>
/// Enum with a list of skills that player can learn.
/// </summary>
public enum SkillName
{
	Melee_Attack_Power,
    Ranged_Attack_Power,
	Spell_Power,
    Armor,
    Magic_Defence
}