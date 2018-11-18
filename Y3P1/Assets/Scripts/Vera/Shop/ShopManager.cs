using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class ShopManager : MonoBehaviour
{

    [Header("Shop Type")]
    public static ShopManager instance;
    public ShopInventory shopInventory;
    public enum ShopType {Equipment, Potions}
    public ShopType shopType;
    [Header("Buy back")]
    public static List<Item> buyBackItems = new List<Item>();
    [SerializeField] private int sizeShop;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        if (shopInventory.IsOpen())
        {
            shopInventory.OpenClose();
        }
    }

    public void SellItem(Item toSell)
    {
        if (Player.localPlayer != null)
        {
            Player.localPlayer.myInventory.UpdateGold(toSell.CalculateValue());
        }

        toSell.sold = true;
        buyBackItems.Add(toSell);
        if (buyBackItems.Count > sizeShop)
        {
            buyBackItems.RemoveAt(0);
        }
        if(shopInventory.shopBS == ShopInventory.ShopBS.Buyback)
        {
            shopInventory.SetToBuyback();
        }

    }

    public void OpenShop()
    {
        shopInventory.OpenClose();
    }

    public void Restock()
    {
        Player.localPlayer.myInventory.CalculateArmor();
        shopInventory.RemoveAll();
        switch (shopType)
        {
            case ShopType.Equipment:
                for (int i = 0; i < sizeShop; i++)
                {
                    shopInventory.AddItem(LootRandomizer.instance.EquipmentToShop(Player.localPlayer.myInventory.averageILevel));
                }
                break;
            case ShopType.Potions:
                for (int i = 0; i < sizeShop; i++)
                {
                    Item temp = LootRandomizer.instance.PotionToShop();
                    shopInventory.AddItem(temp);
                }
                break;
        }
    }

    public void LeaveShop()
    {
        if (shopInventory.IsOpen())
        {
            OpenShop();
        }
    }

    private void Update()
    {
        if (shopInventory.IsOpen())
        {
            if (Input.GetButtonDown("Cancel"))
            {
                shopInventory.OpenClose();
            }
        }
    }

}
