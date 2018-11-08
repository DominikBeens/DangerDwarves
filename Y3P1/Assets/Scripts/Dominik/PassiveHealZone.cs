using UnityEngine;
using Y3P1;

public class PassiveHealZone : MonoBehaviour
{

    private float nextTick;
    private Collider[] entitiesInRange = new Collider[10];

    [SerializeField] private int healPercentage = 2;
    [SerializeField] private float healInterval = 1f;
    [SerializeField] private float healRange;
    [SerializeField] private bool refillSecondaryCharge;
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
                Player.localPlayer.entity.statusEffects.AddEffect(6, healInterval, healPercentage);
                Player.localPlayer.weaponSlot.AddBuff(new WeaponSlot.WeaponBuff { type = StatusEffects.StatusEffectType.Heal, endTime = Time.time + healInterval }, healInterval);

                if (refillSecondaryCharge)
                {
                    Player.localPlayer.entity.statusEffects.AddEffect(5, healInterval);
                    Player.localPlayer.weaponSlot.AddBuff(new WeaponSlot.WeaponBuff { type = StatusEffects.StatusEffectType.WeaponCharge, endTime = Time.time + healInterval }, healInterval);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, healRange);
    }
}
