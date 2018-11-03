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
    [SerializeField] private List<EntitySpawner> propSpawners = new List<EntitySpawner>();

    private void Awake()
    {
        entitySpawners = GetComponentsInChildren<EntitySpawner>();
        burnables = GetComponentsInChildren<Burnable>();
    }

    public void StartDungeon()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SetUpDungeon());
            // Generate difficulty.
        }
    }

    public void CloseDungeon()
    {
        for (int i = 0; i < entitySpawners.Length; i++)
        {
            entitySpawners[i].CanSpawn = true;
        }

        for (int i = 0; i < propSpawners.Count; i++)
        {
            propSpawners[i].CanSpawn = true;
        }

        for (int i = 0; i < burnables.Length; i++)
        {
            burnables[i].ResetObject();
        }

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RemoveRPCs(EntityManager.instance.photonView);
            // Cleanup all alive enemies and drops.
        }
    }

    private IEnumerator SetUpDungeon()
    {
        for (int i = 0; i < propSpawners.Count; i++)
        {
            propSpawners[i].TriggerSpawnMasterClient();
            yield return new WaitForSeconds(0.02f);
        }
    }
}
