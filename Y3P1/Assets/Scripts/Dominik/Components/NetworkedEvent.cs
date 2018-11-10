using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkedEvent : MonoBehaviour 
{

    private List<Entity> players = new List<Entity>();

    [SerializeField] private Animator anim;
    [SerializeField] private string animBoolName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Entity entity = other.GetComponentInChildren<Entity>();
            if (entity && !players.Contains(entity))
            {
                players.Add(entity);
                CheckPlayerCount();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Entity entity = other.GetComponentInChildren<Entity>();
            if (entity && players.Contains(entity))
            {
                players.Remove(entity);
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
    }
}
