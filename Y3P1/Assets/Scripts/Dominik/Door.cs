using Photon.Pun;
using UnityEngine;

public class Door : MonoBehaviourPunCallbacks
{

    private bool isOpen;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject interactPopupVisual;

    public void Open()
    {
        if (!isOpen)
        {
            photonView.RPC("OpenDoor", RpcTarget.AllBuffered);
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            photonView.RPC("CloseDoor", RpcTarget.All);
        }
    }

    [PunRPC]
    private void OpenDoor()
    {
        isOpen = true;
        anim.SetTrigger("Open");
        interactPopupVisual.SetActive(false);
    }

    [PunRPC]
    private void CloseDoor()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RemoveRPCs(photonView);
        }

        isOpen = false;
        anim.SetTrigger("Close");
        interactPopupVisual.SetActive(true);
    }
}
