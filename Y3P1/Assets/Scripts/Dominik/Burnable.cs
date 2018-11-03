using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Burnable : MonoBehaviourPunCallbacks
{

    private bool canBurn = true;
    private List<Vector3> rigidbodyStartPositions = new List<Vector3>();

    [SerializeField] private List<MeshRenderer> renderers = new List<MeshRenderer>();
    [SerializeField] private List<Rigidbody> rigidbodies = new List<Rigidbody>();
    [SerializeField] private Collider webCollider;
    [SerializeField] private ParticleSystem burnParticle;
    [SerializeField] private float burnTime;
    [SerializeField] private GameObject entityToSpawnOnDeath;
    [SerializeField] [Range(0, 100)] private int spawnOnDeathChance;
    [SerializeField] private float rigidbodyExplodeForce = 1f;

    private void Awake()
    {
        for (int i = 0; i < rigidbodies.Count; i++)
        {
            rigidbodyStartPositions.Add(rigidbodies[i].transform.position);
        }
    }

    public void Burn()
    {
        if (canBurn)
        {
            photonView.RPC("StartBurn", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void StartBurn()
    {
        canBurn = false;

        burnParticle.Play();
        Invoke("DisableObject", burnTime);
    }

    private void DisableObject()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].enabled = false;
        }
        webCollider.enabled = false;
        burnParticle.Stop();

        if (PhotonNetwork.IsMasterClient)
        {
            if (entityToSpawnOnDeath && spawnOnDeathChance != 0)
            {
                int random = Random.Range(0, 101);
                if (random < spawnOnDeathChance)
                {
                    PhotonNetwork.InstantiateSceneObject(entityToSpawnOnDeath.name, transform.position, Quaternion.identity);
                }
            }
        }

        for (int i = 0; i < rigidbodies.Count; i++)
        {
            rigidbodies[i].isKinematic = false;
            rigidbodies[i].AddForce(Random.insideUnitSphere.normalized * rigidbodyExplodeForce, ForceMode.Impulse);
        }
    }

    public void ResetObject()
    {
        CancelInvoke();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RemoveRPCs(photonView);
        }

        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].enabled = true;
        }

        for (int i = 0; i < rigidbodies.Count; i++)
        {
            rigidbodies[i].isKinematic = true;
            rigidbodies[i].transform.position = rigidbodyStartPositions[i];
        }

        webCollider.enabled = true;
        burnParticle.Stop();

        canBurn = true;
    }
}
