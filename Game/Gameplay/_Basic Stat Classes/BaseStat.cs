// BaseStat.cs
// May 30, 2012
// 
// This is the base class for a stat.

public class BaseStat
{
    private string _name;
    private int _baseValue; 					//The base value of the stat.
    private int _buffValue; 					//A buff value for the stat.
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseStat"/> class.
    /// Class Constructor.
    /// </summary>
    public BaseStat()
    {
        _baseValue = 0;
        _buffValue = 0;
        _name = "";
    }

    #region Properties
    /// <summary>
    /// Gets or sets the _baseValue.
    /// </summary>
    /// <value>
    /// The _baseValue.
    /// </value>
    public int BaseValue
    {
        get { return _baseValue; }
        set { _baseValue = value; }
    }

    /// <summary>
    /// Gets or sets the _buffValue.
    /// </summary>
    /// <value>
    /// The _buffValue.
    /// </value>
    public int BuffValue
    {
        get { return _buffValue; }
        set { _buffValue = value; }
    }

    /// <summary>
    /// Gets or sets the _name.
    /// </summary>
    /// <value>
    /// The _name.
    /// </value>
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    /// <summary>
    /// Calculate AdjustedBaseValue and return it.
    /// </summary>
    /// <value>
    /// The adjusted base value.
    /// </value>
    public int AdjustedBaseValue
    {
        get { return _baseValue + _buffValue; }
    }
    #endregion
}
