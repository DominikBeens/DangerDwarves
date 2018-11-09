using UnityEngine;

public class SendHitEvent : MonoBehaviour
{

    private CollisionEventZone colEventZone;
    private Entity myEntity;
    private AI myAI;

    private void Awake()
    {
        colEventZone = GetComponent<CollisionEventZone>();
        colEventZone.OnZoneEnterEvent.AddListener(() => RegisterDamage(colEventZone.EventCaller));

        myEntity = transform.root.GetComponentInChildren<Entity>();
        myAI = transform.root.GetComponentInChildren<AI>();
    }

    public void RegisterDamage(Transform transform)
    {
        if (transform.tag == "Player" && transform.gameObject.layer == 9)
        {
            Entity entity = transform.GetComponent<Entity>();
            entity.Hit(-myEntity.CalculateDamage(myAI.CurrentAttack.damageType), myAI.CurrentAttack.damageType);
        }
    }
}
