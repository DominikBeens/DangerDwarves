using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Y3P1;

public class ShopInventory : MonoBehaviour {
    [SerializeField] private List<ShopInventorySlot> allSlots = new List<ShopInventorySlot>();
    [SerializeField] private List<Item> allItems = new List<Item>();
    [SerializeField] private List<Image> allImages = new List<Image>();

    public enum ShopBS { Buy, Sell }
    public ShopBS shopBS;

    private ShopInventorySlot currentSlot;

    [SerializeField] private GameObject panel;

    public void SetCurrent(ShopInventorySlot current)
    {
        currentSlot = current;
        if(currentSlot != null)
        {
            allItems[GetIndex(currentSlot)].SendInfo();
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
    }

    private void BuyItem()
    {
        if (currentSlot != null)
        {
            int index = GetIndex(currentSlot);
            if (allItems[index] != null)
            {

                if (Player.localPlayer.myInventory.CheckFull() && CheckGold())
                {
                    Player.localPlayer.myInventory.AddItem(allItems[index]);
                    ExtractGold();
                    allImages[index].enabled = false;
                    allItems[index] = null;
                }
            }
        }
    }

    private void ExtractGold()
    {
        Player.localPlayer.myInventory.totalGoldAmount -= allItems[GetIndex(currentSlot)].CalculateValue();
    }

    private bool CheckGold()
    {
        bool Enough = false;
        int index = GetIndex(currentSlot);
        int amount = allItems[index].CalculateValue();
        if(amount <= Player.localPlayer.myInventory.totalGoldAmount)
        {
            Enough = true;
        }
        return Enough;
    }
}
