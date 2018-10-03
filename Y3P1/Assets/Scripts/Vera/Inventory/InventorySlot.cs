﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Y3P1;

public class InventorySlot : MonoBehaviour {
    [SerializeField] private Inventory inventory;
    public enum SlotType {all,weapon,helmet,trinket,nothing}
    public SlotType slotType;
    [SerializeField] private Image myItem;
    
    public bool CheckSlotType()
    {
        if(slotType == SlotType.all)
        {
            return true;
        }
        return false;
    }

    public void EquipWeapon(Weapon toEquip)
    {
        if(Player.localPlayer != null)
        {
            Player.localPlayer.weaponSlot.EquipWeapon(toEquip);
            inventory.SetInfo();
        }
    }

    public void EquipTrinket(Trinket toEquip)
    {
        if (Player.localPlayer != null)
        {
            Player.localPlayer.trinketSlot.EquipTrinket(toEquip);
            inventory.SetInfo();
        }
    }

    public void EquipHelmet(Helmet toEquip)
    {
        if (Player.localPlayer != null)
        {
            Player.localPlayer.helmetSlot.EquipHelmet(toEquip);
            inventory.SetInfo();
        }
    }

    public void SetImage(Sprite mySprite)
    {
        myItem.sprite = mySprite;
    }
    
    public void EnableImage()
    {
        myItem.enabled = true;
    }

    public void DisableImage()
    {
        myItem.enabled = false;
    }

    private void Start()
    {
        inventory = Player.localPlayer.myInventory;
        if (inventory == null)
        {
            Debug.Log("No inventory Found");
            Destroy(gameObject);
            return;
        }

        //myItem = GetComponentInChildren<Image>();
        //myItem.enabled = false;
        //inventory.AddSlots();
    }

    public void OnMouseEnter()
    {
        inventory.SetCurrentSlot(this);
    }

    public void OnDrag()
    {
        inventory.StartDragging();
        myItem.color = new Color(255, 255, 255, 0.25f);
        print(myItem.color);
    }

    public void OnDrop()
    {
        inventory.StopDragging();
        myItem.color = new Color(255, 255, 255, 1);
    }

    public void OnMouseExit()
    {
        inventory.SetCurrentSlot(null);
        if (!inventory.IsDragging())
        {

            inventory.SetLastSlot(this);
        }
    }
}
