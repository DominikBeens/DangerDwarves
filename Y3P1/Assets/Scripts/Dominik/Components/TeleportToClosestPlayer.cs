using UnityEngine;

public class TeleportToClosestPlayer : MonoBehaviour
{

    [SerializeField] private float seekRange;

    private void Start()
    {
        Entity closestPlayer = EntityManager.instance.GetClosestPlayer(transform, seekRange);
        if (closestPlayer)
        {
            transform.position = closestPlayer.transform.position;
        }
    }
}
