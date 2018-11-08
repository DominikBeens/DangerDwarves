using UnityEngine;
using Photon.Pun;

public class SpawnNetworkedObjOnFire : MonoBehaviour 
{

    [SerializeField] private GameObject toSpawn;

    private void Awake()
    {
        Projectile parentProjectile = GetComponentInParent<Projectile>();
        if (parentProjectile)
        {
            parentProjectile.OnFire += (t) =>
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.InstantiateSceneObject(toSpawn.name, transform.position, Quaternion.identity);
                }
            };
        }
    }
}
