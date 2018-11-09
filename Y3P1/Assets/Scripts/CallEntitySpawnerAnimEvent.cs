using UnityEngine;

public class CallEntitySpawnerAnimEvent : MonoBehaviour
{

    [SerializeField] private EntitySpawner spawner;

    public void CallSpawn()
    {
        spawner.TriggerSpawnMasterClient();
    }
}
