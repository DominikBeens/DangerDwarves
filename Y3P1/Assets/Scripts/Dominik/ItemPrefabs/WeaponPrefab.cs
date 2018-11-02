﻿using Photon.Pun;
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

            int collidersFound = Physics.OverlapSphereNonAlloc(transform.position, weapon.attackRange, meleeHits, hitLayerMask);

            bool pvp = false;
            for (int i = 0; i < collidersFound; i++)
            {
                if (meleeHits[i].transform.tag == "PvPZone")
                {
                    pvp = true;
                }
            }

            for (int i = 0; i < collidersFound; i++)
            {
                Vector3 toHit = meleeHits[i].transform.position - Player.localPlayer.playerController.body.position;
                if (Vector3.Dot(Player.localPlayer.playerController.body.forward, toHit) > 0)
                {
                    Entity entity = meleeHits[i].GetComponent<Entity>();
                    if (entity)
                    {
                        if (pvp ? meleeHits[i].transform.tag == "Player" : meleeHits[i].transform.tag != "Player")
                        {
                            entity.Hit(-(weapon.baseDamage + Player.localPlayer.entity.CalculateDamage(Stats.DamageType.Melee)), Stats.DamageType.Melee, WeaponSlot.weaponBuffs);

                            // TODO: Change this to an event or a parameter in Entity.Hit()
                            UIManager.instance.playerStatusCanvas.Hit();

                            if (weapon.knockBack > 0 && !pvp)
                            {
                                entity.KnockBack(toHit, weapon.knockBack);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(prefabToSpawnOnHit))
                    {
                        GameObject newSpawn = ObjectPooler.instance.GrabFromPool(prefabToSpawnOnHit, meleeHits[i].ClosestPoint(transform.position), Quaternion.identity);
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

    [PunRPC]
    private void SyncDropData(byte[] itemData)
    {
        ByteObjectConverter boc = new ByteObjectConverter();
        myItem = (Item)boc.ByteArrayToObject(itemData);

        isDropped = true;

        interactCollider.SetActive(true);
        objectCollider.enabled = true;

        transform.eulerAngles += dropRotationAdjustment;
        transform.Rotate(new Vector3(0, UnityEngine.Random.Range(0, 360), 0), Space.World);

        SpawnDroppedItemLabel();

        //DroppedItemManager.instance.RegisterDroppedItem(photonView.ViewID, myItem);
    }

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
