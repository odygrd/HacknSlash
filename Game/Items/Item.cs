//Item.cs
//July 25,2012
//Basic player item slot class

using UnityEngine;

public class Item
{
	private string _name;
	private int _ivalue;
	private int _curDur;
	private int _maxDur;
	private RarityType _rarity;
    private Texture2D _icon; //The items icon
    public Material IconMat;

    //void Awake()
    //{
    //    Init(); 
    //}

    //Init method initialises 
	public Item()
	{
		_name = "Need Name";
		_ivalue = 0;
		_maxDur = 100;
		_curDur = _maxDur;
		_rarity = RarityType.Common;
	}
	
	public Item(string name, int ivalue, RarityType rarity, int maxDur, int curDur)
	{
		_name = name;
		_ivalue = ivalue;
		_maxDur = maxDur;
		_curDur = curDur;
		_rarity = rarity;
	}
	
	public string Name {
		get{ return _name;}
		set{ _name = value;}
	}
	
	public int Ivalue {
		get{ return _ivalue;}
		set{ _ivalue = value;}
	}
	
	public RarityType Rarity {
		get{ return _rarity;}
		set{ _rarity = value;}
	}
	
	public int MaxDurability {
		get{ return _maxDur;}
		set{ _maxDur = value;}
	}
	
	public int CurDurability {
		get{ return _curDur;}
		set{ _curDur = value;}
	}

    public Texture2D Icon
    {
        get { return _icon;}
        set {_icon = value;}
    }

    //use virtual so we can override the method it in an inherit class
    public virtual string Tooltip()
    {
        return Name + "\n"+
            "Value: " + Ivalue + "\n" +
            "Durability: "  + CurDurability + "/" + MaxDurability + "\n";

    }
}

public enum RarityType
{
	Common,
	Uncommon,
	Rare
}