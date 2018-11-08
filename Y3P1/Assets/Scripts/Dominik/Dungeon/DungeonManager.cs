using Photon.Pun;
using UnityEngine;
using Y3P1;
using System;

public class DungeonManager : MonoBehaviourPunCallbacks
{

    public static DungeonManager instance;
    public static Dungeon openDungeon;
    public static bool isInDungeon;

    public static event Action OnDungeonClosed = delegate { };

    [SerializeField] private GameObject dungeonCanvas;
    [SerializeField] private GameObject dungeonOverview;
    [SerializeField] private GameObject openDungeonOverview;
    [SerializeField] private Transform openDungeonSpawn;
    [SerializeField] private GameObject dungeonUIPrefab;
    [SerializeField] private Transform availableDungeonsSpawn;
    [SerializeField] private GameObject insideDungeonCanvas;

    [Space(10)]

    [SerializeField] private Dungeon[] allDungeons;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else if (instance && instance != this)
        {
            Destroy(this);
        }

        ToggleDungeonCanvas(false);
        insideDungeonCanvas.SetActive(false);

        allDungeons = FindObjectsOfType<Dungeon>();
        SetupAllDungeonUI();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (dungeonCanvas.activeInHierarchy)
            {
                ToggleDungeonCanvas(false);
            }
        }
    }

    public void ToggleDungeonCanvas(bool b)
    {
        dungeonCanvas.SetActive(b);
    }

    private void SetupAllDungeonUI()
    {
        for (int i = 0; i < allDungeons.Length; i++)
        {
            DungeonUI newDungeonUI = Instantiate(dungeonUIPrefab, availableDungeonsSpawn.position, Quaternion.identity, availableDungeonsSpawn).GetComponent<DungeonUI>();
            newDungeonUI.Setup(allDungeons[i]);
        }
    }

    public void ActivateDungeon(string dungeonName)
    {
        photonView.RPC("SyncActiveDungeon", RpcTarget.All, dungeonName);
        NotificationManager.instance.NewNotification("<color=yellow>" + PhotonNetwork.NickName + "</color> has <b>opened</b> the dungeon: <color=yellow>" + dungeonName + "</color>!");
    }

    [PunRPC]
    private void SyncActiveDungeon(string dungeonName)
    {
        if (openDungeon != null)
        {
            return;
        }

        for (int i = 0; i < allDungeons.Length; i++)
        {
            if (allDungeons[i].dungeonName == dungeonName)
            {
                openDungeon = allDungeons[i];
                openDungeon.StartDungeon();

                DungeonUI newDungeonUI = Instantiate(dungeonUIPrefab, openDungeonSpawn.position, Quaternion.identity, openDungeonSpawn).GetComponent<DungeonUI>();
                newDungeonUI.Setup(openDungeon);
            }
        }

        ToggleView();
    }

    public void CancelDungeon(string dungeonName)
    {
        photonView.RPC("SyncCancelDungeon", RpcTarget.All);
        NotificationManager.instance.NewNotification("<color=yellow>" + PhotonNetwork.NickName + "</color> has <b>closed</b> the dungeon: <color=yellow>" + dungeonName + "</color>!");
    }

    [PunRPC]
    private void SyncCancelDungeon()
    {
        openDungeon.CloseDungeon();
        openDungeon = null;
        OnDungeonClosed();
        Destroy(openDungeonSpawn.childCount > 0 ? openDungeonSpawn.GetChild(0).gameObject : null);
        ToggleView();

        if (isInDungeon)
        {
            TeleportOutOfDungeon("Dungeon has been closed.");
        }
    }

    private void ToggleView()
    {
        dungeonOverview.SetActive(openDungeon == null ? true : false);
        openDungeonOverview.SetActive(openDungeon == null ? false : true);
    }

    public void TeleportToDungeon(string dungeonName)
    {
        Player.localPlayer.teleporter.Teleport(openDungeon.startSpawn.position);
        ToggleDungeonCanvas(false);
        insideDungeonCanvas.SetActive(true);
        isInDungeon = true;
    }

    public void TeleportOutOfDungeon(string message)
    {
        Player.localPlayer.teleporter.Teleport(GameManager.instance.PlayerSpawn.position, message);
        insideDungeonCanvas.SetActive(false);
        isInDungeon = false;
    }

    public bool HasOpenUI()
    {
        return dungeonCanvas.activeInHierarchy;
    }
}
