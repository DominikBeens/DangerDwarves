using Photon.Pun;
using UnityEngine;

public class AllShopManager : MonoBehaviourPunCallbacks
{
    public static AllShopManager instance;
    private bool canRestock;
    private bool initialisedStocks;
    private float nextTime;
    private ShopManager[] allShops;

    [Header("Other")]
    [SerializeField] private int cooldownInMin;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        allShops = FindObjectsOfType<ShopManager>();
    }

    public void OpenShop()
    {
        for (int i = 0; i < allShops.Length; i++)
        {
            if (allShops[i].shopInventory.IsOpen())
            {
                allShops[i].shopInventory.OpenClose();
            }
        }
    }

    private void Update()
    {
        if (Time.time > nextTime && canRestock)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NotificationManager.instance.NewNotification("<color=yellow>The shops have been restocked");
                photonView.RPC("RestockShops", RpcTarget.All);
            }
            else
            {
                if (!initialisedStocks)
                {
                    RestockShops();
                    initialisedStocks = true;
                }
            }

            nextTime += (cooldownInMin * 60);
        }
    }

    [PunRPC]
    private void RestockShops()
    {
        for (int i = 0; i < allShops.Length; i++)
        {
            allShops[i].Restock();
        }
    }

    public void StartRestocking()
    {
        canRestock = true;
    }
}
