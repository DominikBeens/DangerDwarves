﻿using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class EntityManager : MonoBehaviourPunCallbacks
{

    public static EntityManager instance;

    public List<Entity> aliveTargets = new List<Entity>();

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

    public void AddToAliveTargets(Entity entity)
    {
        aliveTargets.Add(entity);
        //photonView.RPC("SyncAliveTargets", RpcTarget.AllBuffered, entity.gameObject.GetPhotonView().ViewID);
    }

    //[PunRPC]
    //private void SyncAliveTargets(int photonViewID)
    //{
    //    PhotonView targetPV = PhotonView.Find(photonViewID);
    //    if (!targetPV)
    //    {
    //        return;
    //    }

    //    Entity target = targetPV.gameObject.GetComponent<Entity>();
    //    if (target)
    //    {
    //        if (!aliveTargets.Contains(target))
    //        {
    //            aliveTargets.Add(target);
    //        }
    //    }
    //}

    public void RemoveFromAliveTargets(Entity entity)
    {
        if (aliveTargets.Contains(entity))
        {
            aliveTargets.Remove(entity);
        }
    }

    public Entity GetClosestPlayer(Transform origin, float seekRange)
    {
        Player[] players = FindObjectsOfType<Player>();

        Entity closestPlayer = null;
        float closestDistanceSqr = seekRange;

        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].entity.health.isDead)
            {
                Vector3 toPlayer = players[i].transform.position - origin.position;
                float dSqrToTarget = toPlayer.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closestPlayer = players[i].entity;
                }
            }
        }

        return closestPlayer;
    }
}