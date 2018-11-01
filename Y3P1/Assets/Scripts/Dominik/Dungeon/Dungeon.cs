using Photon.Pun;
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
            for (int i = 0; i < propSpawners.Count; i++)
            {
                propSpawners[i].TriggerSpawnMasterClient();
            }

            // Generate difficulty.
        }
    }

    public void CloseDungeon()
    {
        for (int i = 0; i < entitySpawners.Length; i++)
        {
            entitySpawners[i].SetCanSpawn(true);
        }

        for (int i = 0; i < burnables.Length; i++)
        {
            burnables[i].ResetObject();
        }

        if (PhotonNetwork.IsMasterClient)
        {
            // Cleanup all alive enemies and drops.
        }
    }
}
