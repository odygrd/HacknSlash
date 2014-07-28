//Weapon.cs
//July 25,2012
//Class representing player weapon

using UnityEngine;

[AddComponentMenu("Item/Weapon Script")]

public class Weapon : BuffItem
{
    private int _maxDamage;
    private float _dmgVar;
    private WeaponType _weapontype;

    public Weapon()
    {
        _maxDamage = 0;
        _dmgVar = 0;
        _weapontype = WeaponType.OneHandMelee;
    }

    public Weapon(int mdmg, float dvar, float mran, WeaponType weapontype)
    {
        _maxDamage = mdmg;
        _dmgVar = dvar;
        _weapontype = weapontype;
    }

    #region Properties

    public int MaxDamage
    {
        get { return _maxDamage; }
        set { _maxDamage = value; }
    }

    public float DamageVariance
    {
        get { return _dmgVar; }
        set { _dmgVar = value; }
    }

    public WeaponType TypeofWeapon
    {
        get { return _weapontype; }
        set {_weapontype = value; }
    }
    #endregion

    public override string Tooltip()
    {
        return Name + "\n" +
         "Value" + Ivalue + "\n" +
         "Durability" + CurDurability + "/" + MaxDurability + "\n" +
          Mathf.Round(MaxDamage * DamageVariance) + "-" + MaxDamage;
    }
}

public enum WeaponType
{
    OneHandMelee,
    Ranged,
    Magic
}
