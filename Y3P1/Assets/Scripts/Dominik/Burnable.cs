using Photon.Pun;
using UnityEngine;

public class Burnable : MonoBehaviourPunCallbacks
{

    private bool canBurn = true;

    [SerializeField] private MeshRenderer webRenderer;
    [SerializeField] private Collider webCollider;
    [SerializeField] private ParticleSystem burnParticle;
    [SerializeField] private float burnTime;

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
        webRenderer.enabled = false;
        webCollider.enabled = false;
        burnParticle.Stop();
    }

    public void ResetObject()
    {
        CancelInvoke();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RemoveRPCs(photonView);
        }

        webRenderer.enabled = true;
        webCollider.enabled = true;
        burnParticle.Stop();

        canBurn = true;
    }
}
