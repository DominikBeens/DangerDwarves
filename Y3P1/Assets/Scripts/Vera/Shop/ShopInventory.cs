using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Y3P1;
using TMPro;

public class ShopInventory : MonoBehaviour {

    [Header("Slots")]
    [SerializeField] private List<ShopInventorySlot> allSlots = new List<ShopInventorySlot>();
    [SerializeField] private List<Image> allImages = new List<Image>();
    [SerializeField] private List<Image> allOverLay = new List<Image>();
    [SerializeField] private List<Item> allItems = new List<Item>();

    [Header("Items to sell")]
    [SerializeField] private List<Item> allToSellItems = new List<Item>();


    [SerializeField] private ShopInventorySlot currentSlot;

    [Header("Panels")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private GameObject sellpanel;

    [SerializeField] private TMP_Text typeText;
    public enum ShopBS { Buy, Sell, Buyback }

    [Header("Current panel")]
    public ShopBS shopBS;

    [Header("Colors")]
    [SerializeField] private Color available;
    [SerializeField] private Color unAvailable;

    public void SellAllCommon()
    {
        Player.localPlayer.myInventory.SellCommonItems();
    }

    public void SellAllRare()
    {
        Player.localPlayer.myInventory.SellRareItems();
    }

    public void SellAllEpic()
    {
        Player.localPlayer.myInventory.SellEpicItems();
    }

    public void SellAllLegendary()
    {
        Player.localPlayer.myInventory.SellLegendaryItems();
    }

    public void SetToBuy()
    {
        buyPanel.SetActive(true);
        sellpanel.SetActive(false);
        shopBS = ShopBS.Buy;
        for (int i = 0; i < allItems.Count; i++)
        {
            if (i < allToSellItems.Count)
            {
                allItems[i] = allToSellItems[i];
            }
        }
        typeText.text = "Buy";
        CheckAvailability();
    }

    private void CheckAvailability()
    {
        for (int i = 0; i < allImages.Count; i++)
        {
            if (allItems[i] != null)
            {
                allImages[i].sprite = Database.hostInstance.allSprites[allItems[i].spriteIndex];
                allImages[i].enabled = true;
                if (CheckGold(allItems[i]))
                {
                    print("this item is buyable!");
                    allOverLay[i].color = available;
                }
                else
                {
                    print("this item is too expensive!");
                    allOverLay[i].color = unAvailable;
                }
            }
            else
            {
                print("this item is null!");
                allImages[i].enabled = false;
                allOverLay[i].color = unAvailable;
            }
        }
    }

    public void SetToSell()
    {
        buyPanel.SetActive(false);
        sellpanel.SetActive(true);
        shopBS = ShopBS.Sell;
    }

    public void SetToBuyback()
    {
        buyPanel.SetActive(true);
        sellpanel.SetActive(false);
        shopBS = ShopBS.Buyback;
        for (int i = 0; i < allItems.Count; i++)
        {
            if (i <  ShopManager.buyBackItems.Count)
            {
                allItems[i] = ShopManager.buyBackItems[i];
            }
            else
            {
                allItems[i] = null;
            }
        }
        typeText.text = "Buyback";
        CheckAvailability();
    }

    public void SetCurrent(ShopInventorySlot current)
    {
        currentSlot = current;
        if(currentSlot != null)
        {
            if(allItems[GetIndex(currentSlot)] != null)
            {
                allItems[GetIndex(currentSlot)].SendInfo();
            }

        }
    }

    private int GetIndex(ShopInventorySlot slot)
    {
        int index = -1;
        for (int i = 0; i < allSlots.Count; i++)
        {
            if (slot == allSlots[i])
            {
                index = i;
            }
        }

        return index;
    }

    private void Update()
    {

        if (Input.GetButtonDown("Fire2"))
        {

            BuyItem();
        }
        if(currentSlot != null)
        {
            if(allItems[GetIndex(currentSlot)] != null)
            {
                allItems[GetIndex(currentSlot)].SendInfo();
            }
            else
            {
                StatsInfo.instance.DisablePanel();
            }
        }
        else
        {
            StatsInfo.instance.DisablePanel();
        }
    }

    private void BuyItem()
    {
        if (currentSlot != null)
        {
            int index = GetIndex(currentSlot);
            if (allItems[index] != null)
            {
                if (!Player.localPlayer.myInventory.CheckFull() && CheckGold())
                {
                    Player.localPlayer.myInventory.AddItem(allItems[index]);
                    ExtractGold();
                    allItems[index].sold = false;


                    if (shopBS == ShopBS.Buyback)
                    {
                        for (int i = 0; i < ShopManager.buyBackItems.Count; i++)
                        {
                            ShopManager.buyBackItems.Remove(allItems[index]);
                        }
                        
                    }
                    else
                    {
                        allToSellItems[index] = null;
                    }
                    allItems[index] = null;
                    if (shopBS == ShopBS.Buyback)
                    {
                        SetToBuyback();
                    }
                    CheckAvailability();
                    allItems[index] = null;
                }
            }
        }
    }

    private void ExtractGold()
    {
        Player.localPlayer.myInventory.UpdateGold(-allItems[GetIndex(currentSlot)].CalculateValue());
    }

    private bool CheckGold(Item toCheck = null)
    {
        bool Enough = false;
        if (toCheck != null)
        {
            if (toCheck.CalculateValue() <= Player.localPlayer.myInventory.totalGoldAmount)
            {
                Enough = true;
            }
        }
        else
        {
            int index = GetIndex(currentSlot);
            if (allItems[index] != null)
            {
                int amount = allItems[index].CalculateValue();
                if (amount <= Player.localPlayer.myInventory.totalGoldAmount)
                {
                    Enough = true;
                }
            }
        }
        print(Enough);
        return Enough;
    }

    public void AddItem(Item toAdd)
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            if(toAdd != null)
            {
                toAdd.sold = true;
                allToSellItems.Add(toAdd);
                break;
            }
        }
    }

    public void RemoveAll()
    {
        allToSellItems.Clear();
        for (int i = 0; i < allItems.Count; i++)
        {
            allImages[i].enabled = false;
        }
    }

    public void OpenClose()
    {
        shopPanel.SetActive(!shopPanel.activeInHierarchy);
        Player.localPlayer.myInventory.ToggleInventory(shopPanel.activeInHierarchy);
        if (shopPanel.activeInHierarchy)
        {
            SetToBuy();
            Player.localPlayer.myInventory.window = Inventory.Window.Shop;
        }
        else
        {
            Player.localPlayer.myInventory.window = Inventory.Window.Equipment;
        }
    }

    public bool IsOpen()
    {
        return shopPanel.activeInHierarchy;
    }
}
