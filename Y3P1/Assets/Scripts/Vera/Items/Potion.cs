using UnityEngine;
using Y3P1;

[System.Serializable]
public class Potion : Item
{

    private int myMaterialIndex;
    private int index;
    public enum PotionType { WeaponBuff, Heal}

    public PotionType potionType;

    // StatusEffect type and how long the buffs sits on the inflicted target.
    public StatusEffects.StatusEffectType effectType;
    public float statusEffectDuration = 3f;

    // The amount of time this buff lasts on the player who drank it.
    public float buffDuration = 6f;
    public float potionDrinkCooldown = 30f;


    public override int PotionNum()
    {
        return index;
    }

    public override int StartPotion(int rarity)
    {
        index = Random.Range(0, 5);
        Database.hostInstance.GetPotionSprite(index);
        Database.hostInstance.GetPotionObject(index);
        effectType = (StatusEffects.StatusEffectType)index;

        myMaterialIndex = Database.hostInstance.GetPotionMaterial(index);
        spriteIndex = Database.hostInstance.GetPotionSprite(index);
        prefabIndex = Database.hostInstance.GetPotionObject(index);

        itemName = GetPotionName();
        Debug.Log(itemName);
        itemRarity = (ItemRarity)rarity;
        statusEffectDuration = GetStatusEffectDuration();
        buffDuration = GetBuffDuration();
        itemLevel = 1;

        return index;
    }

    public void Drink()
    {
        switch (potionType)
        {
            case PotionType.WeaponBuff:
                Player.localPlayer.weaponSlot.AddBuff(new WeaponSlot.WeaponBuff { type = effectType, statusEffectDuration = statusEffectDuration, endTime = Time.time + buffDuration }, buffDuration);
                Player.localPlayer.dwarfAnimController.Oil();
                break;

            case PotionType.Heal:
                Player.localPlayer.entity.Hit(GetHealAmount() * Player.localPlayer.entity.health.GetMaxHealth(), Stats.DamageType.Melee);
                break;
        }
    }

    private string GetPotionName()
    {
        switch (potionType)
        {
            case PotionType.WeaponBuff:

                switch (effectType)
                {
                    case StatusEffects.StatusEffectType.Bleed:
                        return "Blood Imbue";

                    case StatusEffects.StatusEffectType.Slow:
                        return "Slowness Imbue";
                    
                    case StatusEffects.StatusEffectType.ArmorBreak:
                        return "Broken Armor Imbue";

                    case StatusEffects.StatusEffectType.WeaponBreak:
                        return "Broken Weapons Imbue";

                    case StatusEffects.StatusEffectType.Poison:
                        return "Poison Imbue";

                    default:
                        return "StatusEffectType Not Found! (Potion.GetPotionName())";
                }

            case PotionType.Heal:
                return "Health Potion";

            default:
                return "";
        }
    }

    private float GetBuffDuration()
    {
        switch (itemRarity)
        {
            case ItemRarity.common:

                return 15f;
            case ItemRarity.rare:

                return 17f;
            case ItemRarity.epic:

                return 20f;
            case ItemRarity.legendary:

                return 25f;
            default:

                return buffDuration;
        }
    }

    private float GetStatusEffectDuration()
    {
        switch (effectType)
        {
            case StatusEffects.StatusEffectType.Bleed:

                return 3f;
            case StatusEffects.StatusEffectType.Slow:

                return 2f;
            case StatusEffects.StatusEffectType.ArmorBreak:

                return 5f;
            case StatusEffects.StatusEffectType.WeaponBreak:

                return 4f;
            case StatusEffects.StatusEffectType.Poison:

                return 3f;
            default:

                return statusEffectDuration;
        }
    }

    private int GetHealAmount()
    {
        switch (itemRarity)
        {
            case ItemRarity.common:
                return 20;

            case ItemRarity.rare:
                return 30;

            case ItemRarity.epic:
                return 40;

            case ItemRarity.legendary:
                return 50;

            default:
                return 0;
        }
    }
}
