using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventorySlot : MonoBehaviour {
    [SerializeField] private ShopInventory MyShop;

    public void SlotEnter()
    {
        MyShop.SetCurrent(this);
    }

    public void SlotExit()
    {
        MyShop.SetCurrent(null);
    }

}
