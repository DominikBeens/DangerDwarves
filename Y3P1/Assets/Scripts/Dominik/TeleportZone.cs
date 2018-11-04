using UnityEngine;
using Y3P1;

public class TeleportZone : MonoBehaviour
{

    public void Teleport(Transform destination)
    {
        Player.localPlayer.teleporter.Teleport(destination.position);
    }
}
