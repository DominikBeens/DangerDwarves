
[System.Serializable]
public class Item
{

    public string itemName;
    public enum ItemRarity { Common = 0, Rare = 1, Epic = 2, Legendary = 3 }
    public ItemRarity itemRarity;
    public int spriteIndex;
    public Stats myStats;
    public int prefabIndex;
    public int itemLevel = -1;
    public int materialIndex = 0;
    public bool sold;

    public virtual void StartUp(string name, int rarity,int Mysprite,Stats myStat,int myObj,int iLevel,int material = 0)
    {
        itemName = name;
        itemRarity = (ItemRarity)rarity;
        spriteIndex = Mysprite;
        myStats = myStat;
        prefabIndex = myObj;
        itemLevel = iLevel;
        materialIndex = material;
    }

    public void SendInfo()
    {

        StatsInfo.instance.SetText(ItemInfo(),DamageInfo() ,WeaponInfo(), RangedInfo(), MeleeInfo(), HelmetInfo(), TrinketInfo(), ValueInfo());
    }
    
    public string[] ItemInfo()
    {
        string[] newInfo;
        if (myStats != null)
        {
            newInfo = new string[] { RarityInfo(), "Item level: <color=#00A8FF>" + itemLevel.ToString(), "Stamina: <color=#00A8FF>" + myStats.stamina.ToString(), "Strength: <color=#00A8FF>" + myStats.strength.ToString(), "Agility: <color=#00A8FF>" + myStats.agility.ToString(), "WillPower: <color=#00A8FF>" + myStats.willpower.ToString(), "Defence: <color=#00A8FF>" + myStats.defense.ToString() };
        }
        else if (this is Potion)
        {
            string info = Database.hostInstance.potionDiscription[PotionNum()];
            Potion temp = (Potion)this;
            if(temp.potionType == Potion.PotionType.Heal)
            {
                info = Database.hostInstance.potionDiscription[PotionNum()] + " <color=#00A8FF>" + temp.Percentage() + "</color> health";
            }
            newInfo = new string[] {"<color=white>" + itemName, itemRarity.ToString(), info };
        }
        else
        {
            newInfo = new string[] { RarityInfo(), "Item level: <color=#00A8FF>" + itemLevel.ToString() };
        }
        
        return newInfo;
    }

    public virtual int PotionNum()
    {
        return -1;
    }

    public virtual int StartPotion(int rarity, int type)
    {
        return -1;
    }

    public virtual void StartGold(int amount)
    {
        
    }

    public virtual string[] DamageInfo()
    {
        return null;
    }

    public virtual string[] WeaponInfo()
    {
        return null;
    }

    public virtual string[] HelmetInfo()
    {
        return null;
    }

    public virtual string[] TrinketInfo()
    {
        return null;
    }

    public virtual string[] RangedInfo()
    {
        return null;
    }

    public virtual string[] MeleeInfo()
    {
        return null;
    }

    private string RarityInfo()
    {
        if(itemRarity == ItemRarity.Common)
        {
            return "<color=white>" + itemName;
        }
        if(itemRarity == ItemRarity.Epic)
        {
            return "<color=purple>" + itemName;
        }
        if(itemRarity == ItemRarity.Rare)
        {
            return "<color=#00A8FF>" + itemName;
        }
        return "<color=#FFA500>" + itemName;
    }

    public virtual void StartWeapon(int baseDamage_, float fireRate, string sS, float sFR, float charge, float fS, int aS, int dS,bool buff,bool single)
    {

    }

    public virtual void StartRanged(float fP, int aP, int dP)
    {

    }

    public virtual void StartMelee(float range,float knockback)
    {

    }

    public virtual void Awake()
    {

    }

    public string[] ValueInfo()
    {
        if (sold)
        {
            return new string[] { "Buy Value: <color=yellow" + CalculateValue().ToString() + "<color=white> Gold" };
        }
        return new string[] { "Value: <color=yellow>" + CalculateValue().ToString() + "<color=white> Gold"};
    }

    public int CalculateValue()
    {
        float value = (GoldStats() * ((int)itemRarity + 1)) * ((itemLevel + 1) / 10 + 1) + itemLevel;
        if (sold)
        {
            float i = value * 1.5f;
            value = (int)i;
        }
        return (int)value;
    }

    public float GoldStats()
    {
        float g = 1;
        if (myStats != null)
        {
            g--;
            g += myStats.willpower;
            g += myStats.stamina;
            g += myStats.strength;
            g += myStats.agility;
            g += myStats.defense;

            //g = g / 10;
        }

        return g;
    }

}
