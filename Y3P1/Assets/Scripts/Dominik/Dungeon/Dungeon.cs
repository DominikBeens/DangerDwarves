using Photon.Pun;
using UnityEngine;

public class Dungeon : MonoBehaviour
{

    private EntitySpawner[] entitySpawners;

    [Header("Dungeon Stats")]
    public string dungeonName;
    [TextArea] public string dungeonDescription;

    [Space(10)]

    public Transform startSpawn;

    private void Awake()
    {
        entitySpawners = GetComponentsInChildren<EntitySpawner>();
    }

    public void StartDungeon()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Generate difficulty.
        }
    }

    public void CloseDungeon()
    {
        for (int i = 0; i < entitySpawners.Length; i++)
        {
            entitySpawners[i].ResetSpawner();
        }

        if (PhotonNetwork.IsMasterClient)
        {
            // Cleanup all alive enemies and drops.
        }
    }
}
