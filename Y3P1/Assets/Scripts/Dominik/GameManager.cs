﻿using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Y3P1;

public class GameManager : MonoBehaviourPunCallbacks
{

    public static GameManager instance;
    private bool leavingGame;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject bountyManagerPrefab;
    [SerializeField] private GameObject projectileManagerPrefab;
    [SerializeField] private GameObject notificationManagerPrefab;
    [SerializeField] private GameObject dungeonManagerPrefab;
    //[SerializeField] private GameObject droppedItemManagerPrefab;

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
    }

    private void Start()
    {
        if (!Y3P1.Player.localPlayerObject && playerPrefab)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0.1f, 0), Quaternion.identity);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if (!FindObjectOfType<BountyManager>())
            {
                PhotonNetwork.InstantiateSceneObject(bountyManagerPrefab.name, Vector3.zero, Quaternion.identity);
            }

            if (!FindObjectOfType<ProjectileManager>())
            {
                PhotonNetwork.InstantiateSceneObject(projectileManagerPrefab.name, Vector3.zero, Quaternion.identity);
            }

            if (!FindObjectOfType<NotificationManager>())
            {
                PhotonNetwork.InstantiateSceneObject(notificationManagerPrefab.name, Vector3.zero, Quaternion.identity);
            }

            if (!FindObjectOfType<DungeonManager>())
            {
                PhotonNetwork.InstantiateSceneObject(dungeonManagerPrefab.name, Vector3.zero, Quaternion.identity);
            }

            //if (!FindObjectOfType<DroppedItemManager>())
            //{
            //    PhotonNetwork.InstantiateSceneObject(droppedItemManagerPrefab.name, Vector3.zero, Quaternion.identity);
            //}
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string easterEgg = newPlayer.NickName.ToUpper() == "ANGAITYA" ? "\nEveryone is proud of her!" : "";
            NotificationManager.instance.NewNotification("<color=red>" + newPlayer.NickName + "</color> has entered the hub." + easterEgg);

            BountyManager.instance.photonView.RPC("SyncActiveBounty", RpcTarget.Others, BountyManager.activeBounty.bountyName, BountyManager.activeBounty.progress);
            DungeonManager.instance.photonView.RPC("SyncActiveDungeon", RpcTarget.Others, DungeonManager.openDungeon.dungeonName);
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            NotificationManager.instance.NewNotification("<color=red>" + otherPlayer.NickName + "</color> has left the hub.");
        }
    }

    public void LeaveRoom()
    {
        leavingGame = true;
        SafeManager.instance.SaveGame();
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.instance.LoadScene(0, false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning(cause);

#if UNITY_EDITOR
        return;
#else
        if (cause != DisconnectCause.DisconnectByClientLogic || cause != DisconnectCause.DisconnectByServerLogic)
        {
            Destroy(Y3P1.Player.localPlayer.playerCam.gameObject);
            PhotonNetwork.Destroy(Y3P1.Player.localPlayerObject);
        }
#endif

        //        if (leavingGame)
        //        {
        //            Destroy(Y3P1.Player.localPlayer.playerCam.gameObject);
        //            PhotonNetwork.Destroy(Y3P1.Player.localPlayerObject);
        //        }
        //        else
        //        {
        //#if !UNITY_EDITOR
        //            if (cause != DisconnectCause.DisconnectByClientLogic || cause != DisconnectCause.DisconnectByServerLogic)
        //            {
        //                PhotonNetwork.Reconnect();
        //            }
        //#endif
        //        }

        //        leavingGame = false;
    }
}
