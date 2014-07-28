///<summary>
///Armor.cs
///July 25,2012
///Player armor class
///</summary>
using UnityEngine;
public class Armor : BuffItem
{
    private int _armorLevel; //armor level of this peace of armor
    private EquipmentSlot _slot;

    public Armor()
    {
        _armorLevel = 0;
    }

    public Armor(int armorLevel, EquipmentSlot slot)
    {
        _armorLevel = armorLevel;
        _slot = slot;
    }

    public int ArmorLevel
    {
        get { return _armorLevel; }
        set { _armorLevel = value; }
    }

    public EquipmentSlot Slot
    {
        get { return _slot; }
        set { _slot = value; }
    }

    public override string Tooltip()
    {
        return Name + "\n" +
            "Armor: " + _armorLevel + "\n" +
            "Durability: " + CurDurability + "/" + MaxDurability + "\n";

    }
}
