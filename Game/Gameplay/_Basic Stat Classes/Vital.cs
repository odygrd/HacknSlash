// Vital.cs
// May 30,2012
// 
// Contains all character vital elements.

public class Vital : ModifiedStat
{
	private int _curValue; //The vital's current value.
	
   	public Vital ()
	{
        BaseValue = 500;
		_curValue = 0;
	}

    public Vital(int basevalue)
    {
        BaseValue = basevalue;
        _curValue = 0;
    }
	
	/// <summary>
	/// Gets or sets the current value.
	/// When getting curValue make sure it is not bigger than our AdjusterBaseValue.
	/// If it is set it back to our AdjustedBaseValue.
	/// </summary>
	/// <value>
	/// The current value.
	/// </value>
	public int CurValue {
		get {
			if (_curValue > AdjustedBaseValue)  //curValue can't be bigger than adjustedbasevalue.
				_curValue = AdjustedBaseValue;  //ex. if we have set health = 100, it can't be 110.
            if (_curValue < 0)
                _curValue = 0;
			return _curValue;
		}
		set{ _curValue = value;}	
	}
}

/// <summary>
/// Enaum list containing the vital elements.
/// </summary>
public enum VitalName
{
	Health,
	Mana
}