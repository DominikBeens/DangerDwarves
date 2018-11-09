using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class SpawnNetworkedObjOnFire : MonoBehaviour 
{

    [SerializeField] private List<GameObject> toSpawn = new List<GameObject>();

    private void Awake()
    {
        Projectile parentProjectile = GetComponentInParent<Projectile>();
        if (parentProjectile)
        {
            parentProjectile.OnFire += (t) =>
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.InstantiateSceneObject(toSpawn[Random.Range(0, toSpawn.Count)].name, transform.position, Quaternion.identity);
                }
            };
        }
    }
}
