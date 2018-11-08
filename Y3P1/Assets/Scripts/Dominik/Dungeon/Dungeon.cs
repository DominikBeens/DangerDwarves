﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{

    private EntitySpawner[] entitySpawners;
    private Burnable[] burnables;

    [Header("Dungeon Stats")]
    public string dungeonName;
    [TextArea] public string dungeonDescription;

    [Space(10)]

    public Transform startSpawn;
    [SerializeField] private Transform dungeonCleanupCenter;
    [SerializeField] private float dungeonCloseCleanupRange = 100f;

    private void Awake()
    {
        entitySpawners = GetComponentsInChildren<EntitySpawner>();
        burnables = GetComponentsInChildren<Burnable>();
    }

    public void StartDungeon()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(InitialisePersistentSpawners());
            StartCoroutine(SetUpDungeon());
        }
    }

    public void CloseDungeon()
    {
        for (int i = 0; i < entitySpawners.Length; i++)
        {
            if (entitySpawners[i])
            {
                if (entitySpawners[i].spawnerType == EntitySpawner.SpawnerType.Humanoid || entitySpawners[i].spawnerType == EntitySpawner.SpawnerType.RespawnableProp)
                {
                    entitySpawners[i].CanSpawn = true;
                }
            }
            else
            {
                Debug.LogWarning(dungeonName + ": Found missing entry in EntitySpawners list!");
            }
        }

        for (int i = 0; i < burnables.Length; i++)
        {
            burnables[i].ResetObject();
        }

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CleanUpDungeon());
        }
    }

    private IEnumerator InitialisePersistentSpawners()
    {
        for (int i = 0; i < entitySpawners.Length; i++)
        {
            if (entitySpawners[i])
            {
                if (entitySpawners[i].spawnerType == EntitySpawner.SpawnerType.Static && entitySpawners[i].CanSpawn)
                {
                    entitySpawners[i].TriggerSpawnMasterClient();
                    yield return new WaitForSeconds(0.02f);
                }
            }
        }
    }

    private IEnumerator SetUpDungeon()
    {
        for (int i = 0; i < entitySpawners.Length; i++)
        {
            if (entitySpawners[i])
            {
                if (entitySpawners[i].spawnerType == EntitySpawner.SpawnerType.RespawnableProp)
                {
                    entitySpawners[i].TriggerSpawnMasterClient();
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
                Debug.LogWarning(dungeonName + ": Found missing entry in EntitySpawners list!");
            }
        }
    }

    private IEnumerator CleanUpDungeon()
    {
        PhotonNetwork.RemoveRPCs(EntityManager.instance.photonView);

        Collider[] colliders = Physics.OverlapSphere(dungeonCleanupCenter.position, dungeonCloseCleanupRange);
        for (int i = colliders.Length - 1; i >= 0; i--)
        {
            if (!colliders[i])
            {
                continue;
            }

            Entity entity = colliders[i].GetComponent<Entity>();
            if (entity && entity.gameObject.tag != "Player")
            {
                entity.canDropLoot = false;
                entity.DestroyEntity();
                yield return new WaitForSeconds(0.01f);
                continue;
            }

            ItemPrefab item = colliders[i].GetComponent<ItemPrefab>();
            if (item)
            {
                PhotonNetwork.RemoveRPCs(item.photonView);
                PhotonNetwork.Destroy(item.gameObject);
                yield return new WaitForSeconds(0.01f);
                continue;
            }

            Door door = colliders[i].GetComponent<Door>();
            if (door)
            {
                door.Close();
                continue;
            }

            Brazier brazier = colliders[i].GetComponent<Brazier>();
            if (brazier)
            {
                brazier.Extinguish();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (dungeonCleanupCenter)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(dungeonCleanupCenter.position, dungeonCloseCleanupRange);
        }
    }
}
