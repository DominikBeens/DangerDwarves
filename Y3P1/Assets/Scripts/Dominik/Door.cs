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

    [PunRPC]
    private void OpenDoor()
    {
        isOpen = true;
        anim.SetTrigger("Open");
        interactPopupVisual.SetActive(false);
    }
}
