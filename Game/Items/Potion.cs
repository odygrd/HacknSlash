using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Potion : Item
{
    static readonly string[] potionNames = new[] {
            "Lesser Healing Potion",
            "Greater Healing Potion",
            "Super Healing Potion",
            "Lesser Mana Potion",
            "Greater Mana Potion",
            "Super Mana Potion"
                                                 };

    static readonly string[] potionDescription = new[] {
            "\n Restores between \n 200 and 450 health",
            "\n Restores between \n 400 and 650 health",
            "\n Restores between \n 600 and 950 health",
            "\n Restores between \n 150 and 300 mana",
            "\n Restores between \n 250 and 450 mana",  
            "\n Restores between \n 400 and 650 mana"
                                                };

    private readonly PotionType _potionType;
    private readonly string _description;

    //create random potion
    public Potion()
    {
        _potionType = (PotionType)(Random.Range(0, (int) PotionType.COUNT));
        Name = potionNames[(int) _potionType];
        _description = potionDescription[(int) _potionType];
    }

    //create specific potion
    public Potion(PotionType potionType)
    {
        _potionType = potionType;
        Name = potionNames[(int)_potionType];
        _description = potionDescription[(int)_potionType];
    }


    //Method to use potion and restore player hp/mp
    public void UsePotion()
    {
        switch (_potionType)
        {
            case PotionType.Lesser_Healing:
                PlayerChar.Instance.GetHeal(Random.Range(200, 450));
                break;
            case PotionType.Greater_Healing:
                PlayerChar.Instance.GetHeal(Random.Range(400, 650));
                break;
            case PotionType.Super_Healing:
                PlayerChar.Instance.GetHeal(Random.Range(600, 950));
                break;
            case PotionType.Lesser_Mana:
                PlayerChar.Instance.GetVital((int) VitalName.Mana).CurValue += Random.Range(150, 300);
                break;
            case PotionType.Greater_Mana:
                PlayerChar.Instance.GetVital((int) VitalName.Mana).CurValue += Random.Range(250, 450);
                break;
            case PotionType.Super_Mana:
                PlayerChar.Instance.GetVital((int) VitalName.Mana).CurValue += Random.Range(400, 650);
                break;
        }
    }

    public override string Tooltip()
    {
        return Name + "\n" + _description + "\n";
    }	
}

//Enum with potion names
public enum PotionType
{
    Lesser_Healing,
    Greater_Healing,
    Super_Healing,
    Lesser_Mana,
    Greater_Mana,
    Super_Mana,
    COUNT
}
