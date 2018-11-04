using Photon.Pun;
using UnityEngine;

public class Brazier : MonoBehaviourPunCallbacks
{

    private bool isLighted;
    [SerializeField] private GameObject particle;

    public void Light()
    {
        if (!isLighted)
        {
            photonView.RPC("LightBrazier", RpcTarget.AllBuffered);
        }
    }

    public void Extinguish()
    {
        if (isLighted)
        {
            photonView.RPC("ExtinguishBrazier", RpcTarget.All);
        }
    }

    [PunRPC]
    private void LightBrazier()
    {
        isLighted = true;
        particle.SetActive(true);
    }

    [PunRPC]
    private void ExtinguishBrazier()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RemoveRPCs(photonView);
        }

        isLighted = false;
        particle.SetActive(false);
    }
}
