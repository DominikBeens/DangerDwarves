using Photon.Pun;
using UnityEngine;

namespace Y3P1
{
    public class Player : MonoBehaviourPunCallbacks
    {

        public static GameObject localPlayerObject;
        public static Player localPlayer;

        [Header("Player UI")]
        [SerializeField] private GameObject playerScreenSpaceUIPrefab;
        [SerializeField] private GameObject playerWorldSpaceUIPrefab;
        [SerializeField] private Vector3 playerUISpawnOffset = new Vector3(0, 3, 0.2f);

        [Header("Cameras")]
        [SerializeField] private GameObject playerUICam;
        [SerializeField] private GameObject characterCam;

        [Header("Death")]
        [SerializeField] private GameObject deathCanvas;

        [Header("Cowl Coth")]
        public CapsuleCollider collider1;
        public CapsuleCollider collider2;

        #region Components
        [HideInInspector] public WeaponChargeCanvas weaponChargeCanvas;
        [HideInInspector] public AnimationEventsDwarf dwarfAnimEvents;
        [HideInInspector] public HeadTracking rangedWeaponLookAt;
        [HideInInspector] public PlayerController playerController;
        [HideInInspector] public WeaponSlot weaponSlot;
        [HideInInspector] public HelmetSlot helmetSlot;
        [HideInInspector] public TrinketSlot trinketSlot;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public PlayerCamera playerCam;
        [HideInInspector] public Entity entity;
        [HideInInspector] public Inventory myInventory;
        [HideInInspector] public DwarfAnimationsScript dwarfAnimController;
        [HideInInspector] public IKControl iKControl;
        [HideInInspector] public PlayerAppearance playerAppearance;
        [HideInInspector] public ReviveZone reviveZone;
        [HideInInspector] public CameraShake cameraShake;
        [HideInInspector] public Teleporter teleporter;
        [HideInInspector] public PickAndPlayFromList audio;
        #endregion

        private void Awake()
        {
            if (photonView.IsMine || !PhotonNetwork.IsConnected)
            {
                localPlayerObject = gameObject;
                localPlayer = this;
            }

            GatherPlayerComponents();
            Initialise();
        }

        private void GatherPlayerComponents()
        {
            playerController = GetComponentInChildren<PlayerController>();
            weaponChargeCanvas = GetComponentInChildren<WeaponChargeCanvas>();
            rb = GetComponentInChildren<Rigidbody>();
            playerCam = GetComponentInChildren<PlayerCamera>();
            entity = GetComponentInChildren<Entity>();
            weaponSlot = GetComponentInChildren<WeaponSlot>();
            helmetSlot = GetComponentInChildren<HelmetSlot>();
            trinketSlot = GetComponentInChildren<TrinketSlot>();
            myInventory = GetComponentInChildren<Inventory>();
            dwarfAnimController = GetComponentInChildren<DwarfAnimationsScript>();
            dwarfAnimEvents = GetComponentInChildren<AnimationEventsDwarf>();
            iKControl = GetComponentInChildren<IKControl>();
            playerAppearance = GetComponentInChildren<PlayerAppearance>();
            reviveZone = GetComponentInChildren<ReviveZone>();
            cameraShake = GetComponentInChildren<CameraShake>();
            teleporter = GetComponentInChildren<Teleporter>();
            audio = GetComponentInChildren<PickAndPlayFromList>();
        }

        private void Initialise()
        {
            playerCam.Initialise(IsConnectedAndMine());
            playerController.Initialise(IsConnectedAndMine());
            weaponChargeCanvas.Initialise(IsConnectedAndMine());
            dwarfAnimEvents.Initialise(IsConnectedAndMine());
            dwarfAnimController.Initialise(IsConnectedAndMine());
            rangedWeaponLookAt.Initialise(IsConnectedAndMine());
            myInventory.Initialise(IsConnectedAndMine());
            reviveZone.Initialise(IsConnectedAndMine());

            weaponSlot.Initialise(IsConnectedAndMine());
            helmetSlot.Initialise(IsConnectedAndMine());
            trinketSlot.Initialise(IsConnectedAndMine());

            playerUICam.SetActive(IsConnectedAndMine() ? true : false);
            characterCam.SetActive(IsConnectedAndMine() ? true : false);
            Destroy(IsConnectedAndMine() ? null : rb);

            if (PhotonNetwork.IsConnected)
            {
                CreateWorldSpacePlayerUI();
            }

            if (!IsConnectedAndMine())
            {
                CreateScreenSpacePlayerUI(entity);

                SetLayer(transform, 14);

                foreach (Collider col in GetComponentsInChildren<Collider>())
                {
                    if (!col.GetComponent<Entity>())
                    {
                        col.enabled = false;
                    }
                }

                localPlayer.entity.health.UpdateHealth();
            }
            else
            {
                playerAppearance.RandomizeAppearance();
                playerController.OnDodge += PlayerController_OnDodge;

                entity.OnDeath.AddListener(() => Entity_OnDeath());
                entity.OnHit.AddListener(() => cameraShake.Trauma = 0.5f);

                DontDestroyOnLoad(gameObject);
            }
        }

        private void CreateWorldSpacePlayerUI()
        {
            PlayerUI playerUI = Instantiate(playerWorldSpaceUIPrefab, transform.position + playerUISpawnOffset, Quaternion.identity, playerController.body).GetComponent<PlayerUI>();
            playerUI.Initialise(this, IsConnectedAndMine(), null);
        }

        private void CreateScreenSpacePlayerUI(Entity entity)
        {
            PlayerUI playerUI = Instantiate(playerScreenSpaceUIPrefab, UIManager.instance.otherPlayersUISpawn.position, Quaternion.identity, UIManager.instance.otherPlayersUISpawn).GetComponent<PlayerUI>();
            playerUI.Initialise(this, IsConnectedAndMine(), entity);
        }

        private void PlayerController_OnDodge(bool dodgeStart)
        {
            rangedWeaponLookAt.enabled = !dodgeStart;
            entity.health.isInvinsible = dodgeStart;
        }

        private void Entity_OnDeath()
        {
            deathCanvas.SetActive(true);
            NotificationManager.instance.NewNotification("<color=red>" + PhotonNetwork.NickName + "</color> has been downed!");
        }

        public void Respawn(bool hub)
        {
            deathCanvas.SetActive(false);
            localPlayer.entity.Revive(hub ? 100 : 50);
            audio.PlaySFXGettingRevived(0);

            if (hub)
            {
                transform.position = DungeonManager.isInDungeon ? DungeonManager.openDungeon.startSpawn.position : GameManager.instance.PlayerSpawn.position;
            }
        }

        public bool IsConnectedAndMine()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "WotrScene")
            {
                return true;
            }
            return PhotonNetwork.IsConnected && photonView.IsMine ? true : false;
        }

        public void SetLayer(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            foreach (Transform child in root)
            {
                SetLayer(child, layer);
            }
        }

        public override void OnDisable()
        {
            playerController.OnDodge -= PlayerController_OnDodge;
        }
    }
}
