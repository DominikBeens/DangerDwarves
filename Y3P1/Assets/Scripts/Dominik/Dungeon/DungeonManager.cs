using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class DungeonManager : MonoBehaviourPunCallbacks
{

    public static DungeonManager instance;
    public static Dungeon openDungeon;

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

                DungeonUI newDungeonUI = Instantiate(dungeonUIPrefab, openDungeonSpawn.position, Quaternion.identity, openDungeonSpawn).GetComponent<DungeonUI>();
                newDungeonUI.Setup(openDungeon);
            }
        }

        ToggleView();
    }

    public void CancelDungeon(string dungeonName)
    {
        photonView.RPC("SyncCancelDungeon", RpcTarget.All);
        NotificationManager.instance.NewNotification("<color=yellow>" + PhotonNetwork.NickName + "</color> has <b>canceled</b> the dungeon: <color=yellow>" + dungeonName + "</color>!");
    }

    [PunRPC]
    private void SyncCancelDungeon()
    {
        openDungeon.CloseDungeon();
        openDungeon = null;
        Destroy(openDungeonSpawn.childCount > 0 ? openDungeonSpawn.GetChild(0).gameObject : null);
        ToggleView();
    }

    private void ToggleView()
    {
        dungeonOverview.SetActive(openDungeon == null ? true : false);
        openDungeonOverview.SetActive(openDungeon == null ? false : true);
    }

    public void TeleportToDungeon(string dungeonName)
    {
        openDungeon.TeleportToDungeon();
        ToggleDungeonCanvas(false);
        insideDungeonCanvas.SetActive(true);
    }

    public void TeleportOutOfDungeon()
    {
        Player.localPlayer.transform.position = Vector3.zero;
        insideDungeonCanvas.SetActive(false);
    }

    public bool HasOpenUI()
    {
        return dungeonCanvas.activeInHierarchy;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (openDungeon != null)
        {
            photonView.RPC("SyncActiveDungeon", RpcTarget.All, openDungeon.dungeonName);
        }
    }
}
