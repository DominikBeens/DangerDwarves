using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;
using Photon.Pun;

public class ShopManager : MonoBehaviourPunCallbacks
{

    [Header("Shop Type")]
    public static ShopManager instance;
    public ShopInventory ShopInventory;
    private bool ugh;
    public enum ShopType {Equipment, Potions}
    public ShopType shopType;
    public string st;
    [Header("Buy back")]
    public static List<Item> buyBackItems = new List<Item>();
    [SerializeField] private int sizeShop;

    [Header("Other")]
    [SerializeField] private int cooldownInMin;
    private float nextTime;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.05f);
        Restock();
        if (ShopInventory.IsOpen())
        {
            ShopInventory.OpenClose();
        }
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
        if(ShopInventory.shopBS == ShopInventory.ShopBS.Buyback)
        {
            ShopInventory.SetToBuyback();
        }

    }

    public void OpenShop()
    {
        ShopInventory.OpenClose();
    }

    public void Restock()
    {
        Player.localPlayer.myInventory.CalculateArmor();
        ShopInventory.RemoveAll();
        switch (shopType)
        {
            case ShopType.Equipment:
                for (int i = 0; i < sizeShop; i++)
                {
                    ShopInventory.AddItem(LootRandomizer.instance.EquipmentToShop(Player.localPlayer.myInventory.aIL.averageILevel));
                }
                break;
            case ShopType.Potions:
                for (int i = 0; i < sizeShop; i++)
                {
                    Item temp = LootRandomizer.instance.PotionToShop();
                    ShopInventory.AddItem(temp);
                }
                break;
        }
    }

    [PunRPC]
    public void SendRestock()
    {
        Restock();
    }



    private void Update()
    {
        if(Time.time > nextTime)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NotificationManager.instance.NewNotification("The " + st + " shop has been restocked");
                photonView.RPC("SendRestock", RpcTarget.All);
            }
            nextTime += (cooldownInMin*60);
        }

        if (ShopInventory.IsOpen())
        {
            if (Input.GetButtonDown("Cancel"))
            {
                ShopInventory.OpenClose();
            }
        }
    }

}
