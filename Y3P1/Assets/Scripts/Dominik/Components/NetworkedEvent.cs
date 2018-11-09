using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkedEvent : MonoBehaviour 
{

    private List<GameObject> players = new List<GameObject>();

    [SerializeField] private Animator anim;
    [SerializeField] private string animBoolName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<Entity>())
        {
            if (!players.Contains(other.gameObject))
            {
                players.Add(other.gameObject);
                CheckPlayerCount();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<Entity>())
        {
            if (players.Contains(other.gameObject))
            {
                players.Remove(other.gameObject);
            }
        }
    }

    private void CheckPlayerCount()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (players.Count == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                TriggerEvent(true);
            }
        }
    }

    public void TriggerEvent(bool b)
    {
        if (anim)
        {
            anim.SetBool(animBoolName, b);
        }

        NotificationManager.instance.NewNotification("REEEEEE");
    }
}
