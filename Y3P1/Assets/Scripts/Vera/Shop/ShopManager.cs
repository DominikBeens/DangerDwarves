using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;
using Photon.Pun;

public class ShopManager : MonoBehaviourPunCallbacks
{

    [Header("Shop Type")]
    public static ShopManager instance;
    public ShopInventory shopInventory;
    private bool ugh;
    public enum ShopType {Equipment, Potions}
    public ShopType shopType;
    public string st;
    [Header("Buy back")]
    public static List<Item> buyBackItems = new List<Item>();
    [SerializeField] private int sizeShop;



    private void Awake()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.05f);
        Restock();
        if (shopInventory.IsOpen())
        {
            shopInventory.OpenClose();
        }
        AllShopManager.instance.AddShop(this);
    }

    public void SellItem(Item toSell)
    {       
        Player.localPlayer.myInventory.UpdateGold(toSell.CalculateValue());
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

    [PunRPC]
    public void SendRestock()
    {
        Restock();
    }

    public void RS()
    {
        photonView.RPC("SendRestock", RpcTarget.All);
    }

    public void LeaveShop()
    {
        NotificationManager.instance.NewNotification("test123");
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
