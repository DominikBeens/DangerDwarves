using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class Dungeon : MonoBehaviour 
{

    private EntitySpawner[] entitySpawners;

    [Header("Dungeon Stats")]
    public string dungeonName;
    [TextArea] public string dungeonDescription;

    [Space(10)]

    [SerializeField] private Transform startSpawn;

    private void Awake()
    {
        entitySpawners = GetComponentsInChildren<EntitySpawner>();
    }

    public void StartDungeon()
    {

    }

    public void CloseDungeon()
    {
        for (int i = 0; i < entitySpawners.Length; i++)
        {
            entitySpawners[i].ResetSpawner();
        }
    }

    public void TeleportToDungeon()
    {
        Player.localPlayer.transform.position = startSpawn.position;
    }
}
