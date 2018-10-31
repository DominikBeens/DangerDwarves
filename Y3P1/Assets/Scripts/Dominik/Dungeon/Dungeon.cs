using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class Dungeon : MonoBehaviour 
{

    [Header("Dungeon Stats")]
    public string dungeonName;
    [TextArea] public string dungeonDescription;

    [Space(10)]

    [SerializeField] private Transform startSpawn;

    public void StartDungeon()
    {

    }

    public void CloseDungeon()
    {

    }

    public void TeleportToDungeon()
    {
        Player.localPlayer.transform.position = startSpawn.position;
    }
}
