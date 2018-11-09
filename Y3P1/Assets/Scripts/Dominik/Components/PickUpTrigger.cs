﻿using UnityEngine;
using Y3P1;

public class PickUpTrigger : MonoBehaviour
{

    private bool checkForInput;
    private ItemPrefab myItemPrefab;

    private void Awake()
    {
        myItemPrefab = GetComponentInParent<ItemPrefab>();
    }

    private void Update()
    {
        if (checkForInput && !Player.localPlayer.entity.health.isDead)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (myItemPrefab.myItem is Gold)
                {
                    myItemPrefab.PickUp();
                    Player.localPlayer.dwarfAnimController.Pickup();
                    Player.localPlayer.audio.PlaySFXGettingMoney(5);
                    return;
                }

                if (!Player.localPlayer.myInventory.CheckFull())
                {
                    myItemPrefab.PickUp();
                    Player.localPlayer.dwarfAnimController.Pickup();
                    Player.localPlayer.audio.PlaySFXFindingEquipment(5);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.layer == 9)
            {
                checkForInput = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.layer == 9)
            {
                checkForInput = false;
            }
        }
    }
}
