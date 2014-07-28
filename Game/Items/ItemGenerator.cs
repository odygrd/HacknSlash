using UnityEngine;

public static class ItemGenerator
{
    #region Item Names
    //Tables of all items that can be generated
    static readonly string[] offhandNames = new[]{
            "Small Shield",
            "Large Shield"
                                             };

    static readonly string[] helmetNames = new[]{
            "Bandana",
            "Pirate Hat",
            "Robin Hood Hat",
            "Sailor Hat",
            "Leather Helmet",
            "Plate Helmet"
                                             };

    static readonly string[] bootNames = new[]{
            "Wooden Boots",
            "Manticore Skin Boots",
            "Dark Crystal Boots",
            "Chain Boots",
            "Brigandine Boots"
                                              };

    static readonly string[] glovesNames = new[]{
            "Brigandine Gloves",
            "Chain Gloves",
            "Dark Crystal Gloves",
            "Chain Gloves",
            "Brigandine Gloves"
                                                };
    static readonly string[] bodyNames = new[]{
            "Wooden Breastplate",
            "Manticore Skin Shirt",
            "Dark Crystal Breastplate",
            "Chain Mail Shirt",
            "Brigandine Tunic"
                                               };

    static readonly string[] pantsNames = new[]{
            "Wooden Gaiters",
            "Manticore Skin Gaiters",
            "Dark Crystal Gaiters",
            "Chain Gaiters",
            "Brigandine Gaiters"
                                                };

    static readonly string[] oneHandWeaponNames = new[] {
           "Sword",
           "Morning Star",
           "Silifi",
           "Katana",
           "Dream Sword",
           "Defender Axe"
        };

    static readonly string[] rangedWeaponNames = new[] {
           "Hunting Bow",
           "Jade Crossbow"
                                                };

    #endregion

    //if we don't care what type item to create call this method
    public static Item CreateItem()
    {
        //decide what type of item to make

        int temp = Random.Range(0, 10);
        //  int temp = 2;

        var item = new Item();

        if (temp <= 3)
            item = CreateItem(ItemType.Potion);
        else if (temp > 3 && temp <= 7)
            item = CreateItem(ItemType.Armor);
        else if (temp > 7)
            item = CreateItem(ItemType.Weapon);

        return item;    //return the new item
    }

    //if we want to create a specific item type call this one
    public static Item CreateItem(ItemType t)
    {
        var item = new Item();
        switch (t)
        {
            case ItemType.Weapon:
                item = CreateWeapon();
                break;
            case ItemType.Armor:
                item = CreateArmor();
                break;
            case ItemType.Potion:
                item = CreatePotion();
                break;
        }

        //assing the item properties here since they are common for every item
        item.Ivalue = Random.Range(1, 101);
        item.Rarity = RarityType.Common;
        item.MaxDurability = Random.Range(50, 60);
        item.CurDurability = item.MaxDurability; //it starts at full  durability
        //fill in all of the values for that item typ
        return item;
    }

    #region Create Weapon Items
    private static Weapon CreateWeapon()
    {
        int temp = Random.Range(0, 2);
        var weapon = new Weapon();
        switch (temp)
        {
            case 0:
                weapon = CreateWeaponItem(oneHandWeaponNames, WeaponType.OneHandMelee, 20, 40);
                break;
            case 1:
                weapon = CreateWeaponItem(rangedWeaponNames, WeaponType.Ranged, 50, 70);
                break;
        }

        return weapon;
    }
    private static Weapon CreateWeaponItem(string[] weaponNames, WeaponType weapontype, int min, int max)
    {
        var weapon = new Weapon
                         {
                             Name = weaponNames[Random.Range(0, weaponNames.Length)],
                             TypeofWeapon = weapontype,
                             MaxDamage = Random.Range(min, max),
                             DamageVariance = Random.Range(.2f, .76f)
                         };
        weapon.Icon = LoadItemTexture(EquipmentSlot.MainHand, weapon.Name); //assing the icon
        return weapon;
    }
    #endregion

    #region Create Armor Items
    private static Armor CreateArmor()
    {
        int temp = Random.Range(0, 6);
        var armor = new Armor();
        switch (temp)
        {
            case 0:
                armor = CreateArmorItem(offhandNames, EquipmentSlot.OffHand, 30, 45);
                break;
            case 1:
                armor = CreateArmorItem(helmetNames, EquipmentSlot.Helmet, 20, 30);
                break;
            case 2:
                armor = CreateArmorItem(bootNames, EquipmentSlot.Boots, 10, 15);
                break;
            case 3:
                armor = CreateArmorItem(glovesNames, EquipmentSlot.Hands, 10, 15);
                break;
            case 4:
                armor = CreateArmorItem(bodyNames, EquipmentSlot.Body, 40, 55);
                break;
            case 5:
                armor = CreateArmorItem(pantsNames, EquipmentSlot.Legs, 24, 39);
                break;
        }

        return armor;
    }

    //Method to create an armor item
    //Input the equipment slot and the min max armor values
    private static Armor CreateArmorItem(string[] itemNames, EquipmentSlot equipmentSlot, int min, int max)
    {
        var armor = new Armor
                        {
                            Slot = equipmentSlot,
                            Name = itemNames[Random.Range(0, itemNames.Length)],
                            ArmorLevel = Random.Range(min, max)
                        };
        armor.Icon = LoadItemTexture(equipmentSlot, armor.Name);
        return armor;
    }
    #endregion

    #region Create Potions
    private static Potion CreatePotion()
    {
        //create random new potion,item name is setted in potion class
        var potion = new Potion();    
        potion.Icon = (Texture2D)Resources.Load(GameSetting2.MISC_ICON_PATH + potion.Name);
        return potion;
    }

    #endregion

    #region LoadItemTexture Methods
    //Method to load the appropriate item tuxture
    //gets the equipmentslot and the item and loads his texture
    public static Texture2D LoadItemTexture(EquipmentSlot equipmentslot, string itemName)
    {
        var itemTexture = new Texture2D(0, 0);
        switch (equipmentslot)
        {
            case EquipmentSlot.OffHand:
                itemTexture = (Texture2D)Resources.Load(GameSetting2.ARMOR_ICON_PATH + "Offhands/" + itemName);
                break;
            case EquipmentSlot.Helmet:
                itemTexture = (Texture2D)Resources.Load(GameSetting2.ARMOR_ICON_PATH + "Helmets/" + itemName);
                break;
            case EquipmentSlot.Boots:
                itemTexture = (Texture2D)Resources.Load(GameSetting2.ARMOR_ICON_PATH + "Boots/" + itemName);
                break;
            case EquipmentSlot.Hands:
                itemTexture = (Texture2D)Resources.Load(GameSetting2.ARMOR_ICON_PATH + "Gloves/" + itemName);
                break;
            case EquipmentSlot.Body:
                itemTexture = (Texture2D)Resources.Load(GameSetting2.ARMOR_ICON_PATH + "Body/" + itemName);
                break;
            case EquipmentSlot.Legs:
                itemTexture = (Texture2D)Resources.Load(GameSetting2.ARMOR_ICON_PATH + "Pants/" + itemName);
                break;
            case EquipmentSlot.MainHand:
                itemTexture = (Texture2D)Resources.Load(GameSetting2.WEAPON_ICON_PATH + itemName);
                break;
        }
        return itemTexture;
    }

    #endregion

}

public enum ItemType
{
    Weapon,
    Armor,
    Potion,
    //  Scroll,
    COUNT
}