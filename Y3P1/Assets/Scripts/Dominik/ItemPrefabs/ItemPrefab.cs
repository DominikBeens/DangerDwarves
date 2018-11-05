using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class ItemPrefab : MonoBehaviourPunCallbacks, IPunObservable
{

    protected Rigidbody rb;
    protected bool isDropped;
    [HideInInspector] public bool isDecoy;
    protected DroppedItemLabel droppedItemLabel;

    public Item myItem;

    public enum ItemType { Undefined, Sword, Axe, Hammer, Cleaver, Crossbow, Chefshat, Cowl, DemonStaff, SpiderStaff }
    public ItemType itemType;

    [SerializeField] protected GameObject interactCollider;
    [SerializeField] protected Collider objectCollider;
    [SerializeField] protected List<ParticleSystem> weaponRarityParticles = new List<ParticleSystem>();
    [SerializeField] protected Vector3 dropRotationAdjustment;
    [SerializeField] protected Renderer renderer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // If you join a game and a dropped item has no name it means that this player already received the data sync RPC in a previous login and cannot get it again 
        // on another login. To prevent weird things from happening, disable the item. Other players can still see it.
        if (isDropped && !transform.parent && string.IsNullOrEmpty(myItem.itemName))
        {
            gameObject.SetActive(false);
            return;
        }

        if (!string.IsNullOrEmpty(myItem.itemName))
        {
            SetMaterial();
            SetRarityParticleColors();
        }
    }

    public virtual void Drop(byte[] itemData)
    {
        // WARNING! This RPC needs to be implemented in each inherited class for this be possible to call!
        photonView.RPC("SyncDropData", RpcTarget.AllBuffered, itemData);
    }

    [PunRPC]
    private void SyncDropData(byte[] itemData)
    {
        ByteObjectConverter boc = new ByteObjectConverter();
        myItem = (Item)boc.ByteArrayToObject(itemData);

        isDropped = true;
        rb.isKinematic = false;

        interactCollider.SetActive(true);
        objectCollider.enabled = true;

        transform.eulerAngles += dropRotationAdjustment;
        transform.Rotate(new Vector3(0, UnityEngine.Random.Range(0, 360), 0), Space.World);

        SpawnDroppedItemLabel();

        //DroppedItemManager.instance.RegisterDroppedItem(photonView.ViewID, myItem);
    }

    protected void SpawnDroppedItemLabel()
    {
        droppedItemLabel = ObjectPooler.instance.GrabFromPool("DroppedItemLabel", transform.position + Vector3.up * 0.5f, Quaternion.identity).GetComponent<DroppedItemLabel>();
        droppedItemLabel.Initialise(myItem.itemName, myItem, this);
    }

    public void PickUp()
    {
        isDropped = false;
        rb.isKinematic = true;

        interactCollider.SetActive(false);
        objectCollider.enabled = false;

        Player.localPlayer.myInventory.AddItem(myItem);
        photonView.RPC("PickUpDestroy", RpcTarget.All);
    }

    [PunRPC]
    private void PickUpDestroy()
    {
        //DroppedItemManager.instance.RemoveDroppedItem(photonView.ViewID);
        droppedItemLabel.anim.SetTrigger("Pickup");

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RemoveRPCs(photonView);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void SetMaterial()
    {
        switch (itemType)
        {
            case ItemType.Undefined:
                break;
            case ItemType.Sword:

                renderer.material = Database.hostInstance.swordMats[myItem.materialIndex];
                break;
            case ItemType.Axe:

                renderer.material = Database.hostInstance.axeMats[myItem.materialIndex];
                break;
            case ItemType.Hammer:

                renderer.material = Database.hostInstance.hammerMats[myItem.materialIndex];
                break;
            case ItemType.Crossbow:

                renderer.material = Database.hostInstance.crossbowMats[myItem.materialIndex];
                break;
        }
    }

    private void SetRarityParticleColors()
    {
        if (myItem == null)
        {
            return;
        }

        Color rarityColor = new Color();
        switch (myItem.itemRarity)
        {
            case Item.ItemRarity.common:

                rarityColor = Color.white;
                break;
            case Item.ItemRarity.rare:

                rarityColor = new Color(0, 0.835f, 1, 1);
                break;
            case Item.ItemRarity.epic:

                rarityColor = Color.magenta;
                break;
            case Item.ItemRarity.legendary:

                rarityColor = Color.yellow;
                break;
        }

        for (int i = 0; i < weaponRarityParticles.Count; i++)
        {
            ParticleSystem.MainModule psMainModule = weaponRarityParticles[i].main;
            psMainModule.startColor = rarityColor;
        }
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isDropped);
            stream.SendNext(interactCollider.activeInHierarchy);
            stream.SendNext(objectCollider.enabled);
        }
        else
        {
            isDropped = (bool)stream.ReceiveNext();
            interactCollider.SetActive((bool)stream.ReceiveNext());
            objectCollider.enabled = (bool)stream.ReceiveNext();
        }
    }
}
