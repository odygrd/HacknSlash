// Modified stat.cs
// May 30,2012
//
// ModifiedStat class extents BaseStat and manages different modifiers for every stat.

using System.Collections.Generic; //Genereic is added so we can use List<>.

public class ModifiedStat : BaseStat
{
	private List<ModifyingAttribute> _mods; 	 //A list of structs that modify this stat.
	private int _modValue;                 		 //The amount added to the basevalue from the modifiers.
	
	/// <summary>
	/// Initializes a new instance of the <see cref="ModifiedStat"/> class.
	/// Class Constructor.
	/// </summary>
	public ModifiedStat ()
	{
		_mods = new List<ModifyingAttribute> ();
		_modValue = 0;
	}
	
	/// <summary>
	/// Reset _modValue to zero and check to see if we have at least one ModifyingAttribute in our list of mods
	/// If we do, then interate through the list and add the AdjustedBaseValue * ratio to our modValue.
	/// </summary>
	private void CalculateModValue ()
	{
		_modValue = 0;
		if (_mods.Count > 0)
			foreach (ModifyingAttribute att in _mods) {
				_modValue += (int)(att.attribute.AdjustedBaseValue * att.ratio); 
			}
	}
	
	/// <summary>
	/// Adds a ModifyingAttribute to our list of mods for this ModifiedStat.
	/// </summary>
	/// <param name='mod'>
	/// Mod.
	/// </param>
	public void AddModifier (ModifyingAttribute mod)
	{
		_mods.Add (mod);
	}

    public void ClearModifiers()
    {
        _mods.Clear();
    }

	/// <summary>
	/// Update this instance.
	/// </summary>
	public void Update ()
	{
		CalculateModValue ();
	}
	
	/// <summary>
	/// This method is overiding the AdjustedBaseValue in BaseStat.cs 
	/// Calculate the AdjustedBaseValue from BaseValue + BuffValue + _modValue
	/// </summary>
	/// <value>
	/// The adjusted base value.
	/// </value>
	public new int AdjustedBaseValue {
		get{ return BaseValue + BuffValue + _modValue;}
	}
	
}

/// <summary>
/// Struct that holds an attribute and a ratio that will be added as ModifyingAttribute.
/// </summary>
public struct ModifyingAttribute
{
	public Attribute attribute;   //Attribute to be used as modifier.
	public float ratio;    	      //Percent of the attributes adjusted baseValue.
	
	/// <summary>
	/// Initializes a new instance of the <see cref="ModifyingAttribute"/> struct.
	/// </summary>
	public ModifyingAttribute (Attribute att, float rat)
	{
		attribute = att;
		ratio = rat;
	}
}
