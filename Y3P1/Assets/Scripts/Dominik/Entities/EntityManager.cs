using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class EntityManager : MonoBehaviourPunCallbacks
{

    public static EntityManager instance;

    public EntitySpawner[] allEntitySpawners;
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

        allEntitySpawners = FindObjectsOfType<EntitySpawner>();
    }

    [PunRPC]
    private void SpawnEntities(int spawner, int spawnAmount, float spawnRange, bool spawnImmortal)
    {
        EntitySpawner origin = allEntitySpawners[spawner];

        if (!PhotonNetwork.IsMasterClient)
        {
            origin.CanSpawn = false;
            return;
        }

        if (!origin.CanSpawn)
        {
            return;
        }

        origin.CanSpawn = false;

        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 spawnPos = spawnRange == 0 ? origin.transform.position : origin.GetRandomPos();
            if (spawnPos != Vector3.zero)
            {
                GameObject newSpawn = PhotonNetwork.InstantiateSceneObject(origin.GetRandomEntity(), spawnPos, origin.transform.rotation);
                newSpawn.transform.localScale = origin.transform.localScale;
                Entity newEntity = newSpawn.GetComponentInChildren<Entity>();
                if (newEntity)
                {
                    newEntity.health.isImmortal = spawnImmortal;
                }
            }
            else
            {
                // This happens when GetRandomPos() couldnt find a valid position to spawn an entity.
                // Lets just skip this spawn if this happens.
            }
        }
    }

    public int GetSpawnerIndex(EntitySpawner spawner)
    {
        int index = -1;
        for (int i = 0; i < allEntitySpawners.Length; i++)
        {
            if (allEntitySpawners[i] == spawner)
            {
                return i;
            }
        }

        return index;
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