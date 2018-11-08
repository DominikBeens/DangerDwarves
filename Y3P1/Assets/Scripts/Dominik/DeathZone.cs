using UnityEngine;

public class DeathZone : MonoBehaviour 
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.gameObject.layer == 9)
        {
            Entity entity = other.GetComponent<Entity>();
            if (entity)
            {
                entity.Hit(-10000, Stats.DamageType.Melee);
            }
        }
    }
}
