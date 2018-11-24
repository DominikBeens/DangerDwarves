using Photon.Pun;
using System;
using UnityEngine;
using Y3P1;

public class WeaponPrefab : ItemPrefab
{

    private Collider[] meleeHits = new Collider[30];
    private Action startMelee;
    private Action endMelee;

    public Transform projectileSpawn;
    [SerializeField] private MeleeWeaponTrail weaponTrail;
    [SerializeField] private string prefabToSpawnOnHit;
    [SerializeField] private LayerMask hitLayerMask;

    protected override void Awake()
    {
        base.Awake();

        if (photonView.IsMine)
        {
            WeaponSlot.OnUsePrimary += WeaponSlot_OnUsePrimary;
            WeaponSlot.OnUseSecondary += WeaponSlot_OnUseSecondary;
            WeaponSlot.OnEquipWeapon += WeaponSlot_OnEquipWeapon;

            startMelee = delegate { SetWeaponTrail(true); };
            endMelee = delegate { SetWeaponTrail(false); };
            WeaponSlot.OnStartMelee += startMelee;
            WeaponSlot.OnEndMelee += endMelee;
        }

        projectileSpawn = transform.GetChild(0).transform;
    }

    private void WeaponSlot_OnUsePrimary()
    {
        if (isDropped || isDecoy)
        {
            return;
        }

        // Ranged Attack.
        if (WeaponSlot.currentWeapon is Weapon_Ranged)
        {
            Weapon_Ranged weapon = WeaponSlot.currentWeapon as Weapon_Ranged;

            ProjectileManager.ProjectileData data = new ProjectileManager.ProjectileData
            {
                spawnPosition = ProjectileManager.instance.GetProjectileSpawn(this, weapon.primaryProjectile),
                spawnRotation = projectileSpawn.rotation,
                projectilePool = weapon.primaryProjectile,
                speed = ProjectileManager.instance.GetProjectileSpeed(weapon.force, weapon.primaryProjectile),
                damage = weapon.baseDamage + Player.localPlayer.entity.CalculateDamage(Stats.DamageType.Ranged),
                amount = weapon.amountOfProjectiles,
                coneOfFireInDegrees = weapon.coneOfFireInDegrees,
                mousePos = PlayerController.mouseInWorldPos,
                projectileOwnerID = Player.localPlayer.photonView.ViewID,
                projectileVisual = ProjectileManager.instance.GetProjectileVisual(itemType)
            };
            ProjectileManager.instance.FireProjectile(data);
        }
        // Melee Attack.
        else
        {
            Weapon_Melee weapon = WeaponSlot.currentWeapon as Weapon_Melee;

            // Find all colliders in a small radius.
            int collidersFound = Physics.OverlapSphereNonAlloc(transform.position, weapon.attackRange, meleeHits, hitLayerMask);

            // When were in a pvp zone, set pvp to true.
            bool pvp = false;
            for (int i = 0; i < collidersFound; i++)
            {
                if (meleeHits[i].transform.tag == "PvPZone")
                {
                    pvp = true;
                }
            }

            // Loops through all hit colliders and checks if the player is facing them using Vector3.Dot() because we dont want to accidentally hit something behind us.
            for (int i = 0; i < collidersFound; i++)
            {
                Vector3 toHit = meleeHits[i].transform.position - Player.localPlayer.playerController.body.position;
                if (Vector3.Dot(Player.localPlayer.playerController.body.forward, toHit) > 0)
                {
                    // If were facing the hit collider, check if they are an entity.
                    Entity entity = meleeHits[i].GetComponent<Entity>();
                    if (entity)
                    {
                        // If were in a pvp zone check if we hit a player. If were not in a pvp zone then we dont want to hit other players, no friendly fire!
                        if (pvp ? meleeHits[i].transform.tag == "Player" : meleeHits[i].transform.tag != "Player")
                        {
                            // Calculate this weapons damage and hit the entity.
                            entity.Hit(-(weapon.baseDamage + Player.localPlayer.entity.CalculateDamage(Stats.DamageType.Melee)), Stats.DamageType.Melee, WeaponSlot.weaponBuffs);

                            // TODO: Change this to an event or a parameter in Entity.Hit()
                            UIManager.instance.playerStatusCanvas.Hit(false);

                            // If the weapon has a knockback value, entity.KnockBack will send an RPC and apply a bit of force to the entity.
                            if (weapon.knockBack > 0 && !pvp)
                            {
                                entity.KnockBack(toHit, weapon.knockBack);
                            }
                        }
                    }

                    // Spawn an optional object at the place where we hit the collider.
                    // This is used for hitmarkers.
                    if (!string.IsNullOrEmpty(prefabToSpawnOnHit))
                    {
                        ObjectPooler.instance.GrabFromPool(prefabToSpawnOnHit, meleeHits[i].ClosestPoint(transform.position), Quaternion.identity);
                    }
                }
            }
        }
    }

    private void WeaponSlot_OnUseSecondary(Weapon.SecondaryType secondaryType)
    {
        if (isDropped || isDecoy)
        {
            return;
        }

        Weapon weapon = WeaponSlot.currentWeapon;

        ProjectileManager.ProjectileData data = new ProjectileManager.ProjectileData
        {
            spawnPosition = ProjectileManager.instance.GetProjectileSpawn(this, weapon.secondaryProjectile),
            spawnRotation = (secondaryType == Weapon.SecondaryType.Attack) ? projectileSpawn.rotation : Player.localPlayer.transform.rotation,
            projectilePool = weapon.secondaryProjectile,
            speed = ProjectileManager.instance.GetProjectileSpeed(weapon.secondaryForce, weapon.secondaryProjectile),
            damage = weapon.baseDamage + Player.localPlayer.entity.CalculateDamage(Stats.DamageType.Secondary),
            amount = weapon.secondaryAmountOfProjectiles,
            coneOfFireInDegrees = weapon.secondaryConeOfFireInDegrees,
            mousePos = PlayerController.mouseInWorldPos,
            projectileOwnerID = Player.localPlayer.photonView.ViewID,
            projectileVisual = ProjectileManager.instance.GetProjectileVisual(itemType)
        };
        ProjectileManager.instance.FireProjectile(data);
    }

    // Sets the item data when equiping a weapon.
    private void WeaponSlot_OnEquipWeapon(Weapon weapon)
    {
        if (isDropped || isDecoy)
        {
            return;
        }

        if (weapon != null)
        {
            myItem = WeaponSlot.currentWeapon;
        }
    }

    // Called when an item gets dropped.
    [PunRPC]
    private void SyncDropData(byte[] itemData)
    {
        // Set the item data. Item data gets converted to a byte array so that were able to send it across the network.
        ByteObjectConverter boc = new ByteObjectConverter();
        myItem = (Item)boc.ByteArrayToObject(itemData);

        isDropped = true;
        rb.isKinematic = false;

        interactCollider.SetActive(true);
        objectCollider.enabled = true;

        // Randomize the rotation.
        transform.eulerAngles += dropRotationAdjustment;
        transform.Rotate(new Vector3(0, UnityEngine.Random.Range(0, 360), 0), Space.World);

        SpawnDroppedItemLabel();

        //DroppedItemManager.instance.RegisterDroppedItem(photonView.ViewID, myItem);
    }

    // Tell the master client to destroy this object. Every client sees the pickup animation.
    [PunRPC]
    private void PickUpDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RemoveRPCs(photonView);
            PhotonNetwork.Destroy(gameObject);
        }

        droppedItemLabel.anim.SetTrigger("Pickup");
    }

    // Toggle the weapon trail of a melee weapon.
    public void SetWeaponTrail(bool b)
    {
        if (weaponTrail)
        {
            weaponTrail.Emit = b;
        }
    }

    public override void OnDisable()
    {
        if (photonView.IsMine)
        {
            WeaponSlot.OnUsePrimary -= WeaponSlot_OnUsePrimary;
            WeaponSlot.OnUseSecondary -= WeaponSlot_OnUseSecondary;
            WeaponSlot.OnEquipWeapon -= WeaponSlot_OnEquipWeapon;

            WeaponSlot.OnStartMelee -= startMelee;
            WeaponSlot.OnEndMelee -= endMelee;
        }
    }
}
