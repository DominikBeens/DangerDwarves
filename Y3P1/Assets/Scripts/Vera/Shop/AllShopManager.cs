using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AllShopManager : MonoBehaviourPunCallbacks
{
    public static AllShopManager instance;
    [SerializeField] private List<ShopManager> allShops = new List<ShopManager>();
    [Header("Other")]
    [SerializeField] private int cooldownInMin;
    private float nextTime;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void OpenShop()
    {
        for (int i = 0; i < allShops.Count; i++)
        {
            if (allShops[i].shopInventory.IsOpen())
            {
                allShops[i].shopInventory.OpenClose();
            }
        }
    }

    public void AddShop(ShopManager toAdd)
    {
        allShops.Add(toAdd);
    }

    private void Update()
    {
        if (Time.time > nextTime)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NotificationManager.instance.NewNotification("<color=yellow>The shops have been restocked");
                if(allShops.Count != 0)
                {
                    for (int i = 0; i < allShops.Count; i++)
                    {
                        allShops[i].RS();
                    }
                }

            }
            nextTime += (cooldownInMin * 60);
        }

    }
}
