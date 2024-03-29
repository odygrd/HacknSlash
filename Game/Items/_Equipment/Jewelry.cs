///<summary>
///Jewelry.cs
///July 25,2012
///Class representing player jewels
///</summary>
using UnityEngine;

public class Jewelry : BuffItem
{
	private JewelrySlot _slot;  //Store the slot the jewelry is in.

	public Jewelry ()
	{
		_slot = JewelrySlot.PocketItem;
	}

	public Jewelry (JewelrySlot slot)
	{
		_slot = slot;
	}
	
	public JewelrySlot Slot {
		get{ return _slot;}
		set{ _slot = value;}
	}
}

public enum JewelrySlot
{
	EarRings,
	Necklaces,
	Bracelets,
	Rings,
	PocketItem
}