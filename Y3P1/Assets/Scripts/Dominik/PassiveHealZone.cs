using UnityEngine;

public class PassiveHealZone : MonoBehaviour
{

    private float nextTick;
    private Collider[] entitiesInRange = new Collider[10];

    [SerializeField] private int healAmount = 1;
    [SerializeField] private float healInterval = 1f;
    [SerializeField] private float healRange;
    [SerializeField] private LayerMask hitLayerMask;

    private void Update()
    {
        if (Time.time >= nextTick)
        {
            nextTick = Time.time + healInterval;
            Heal();
        }
    }

    private void Heal()
    {
        int collidersFound = Physics.OverlapSphereNonAlloc(transform.position, healRange, entitiesInRange, hitLayerMask);

        for (int i = 0; i < collidersFound; i++)
        {
            if (entitiesInRange[i].tag == "Player" && entitiesInRange[i].gameObject.layer == 9)
            {
                Entity entity = entitiesInRange[i].GetComponent<Entity>();
                entity.Hit(healAmount, Stats.DamageType.AOE);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, healRange);
    }
}
