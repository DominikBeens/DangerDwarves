using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;
using Y3P1;

public class ReviveZone : MonoBehaviourPunCallbacks
{

    private bool initialised;
    private bool checkForInput;
    private bool reviving;

    private Collider reviveZoneCollider;

    [SerializeField] private GameObject reviveZoneObject;
    [SerializeField] private float reviveSpeed;
    [SerializeField] private Image progressImage;
    [SerializeField] private GameObject progressPanel;
    [SerializeField] private GameObject interactIndicator;

    public event Action OnStartRevive = delegate { };
    public event Action OnEndRevive = delegate { };

    public void Initialise(bool local)
    {
        reviveZoneCollider = GetComponent<Collider>();

        if (!local)
        {
            return;
        }

        initialised = true;

        Player.localPlayer.entity.OnDeath.AddListener(() => ToggleRevivable(true));
        Player.localPlayer.entity.OnRevive.AddListener(() => ToggleRevivable(false));
    }

    public void ToggleRevivable(bool b)
    {
        if (b)
        {
            photonView.RPC("SetReviveZone", RpcTarget.AllBuffered, b);
        }
        else
        {
            photonView.RPC("SetReviveZone", RpcTarget.All, b);
        }
    }

    [PunRPC]
    private void SetReviveZone(bool b)
    {
        reviveZoneObject.SetActive(b);
        reviveZoneCollider.enabled = b;

        if (!b && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RemoveRPCs(photonView);
        }
    }

    private void Update()
    {
        if (checkForInput && !photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleRevive(true);
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                ToggleRevive(false);
            }
        }

        if (reviving)
        {
            if (Player.localPlayer.entity.health.isDead)
            {
                ToggleRevive(false);
            }

            progressImage.fillAmount += Time.deltaTime * reviveSpeed;

            if (progressImage.fillAmount == 1)
            {
                Revive();
            }
        }
        else
        {
            progressImage.fillAmount = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !photonView.IsMine)
        {
            checkForInput = true;
            interactIndicator.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !photonView.IsMine)
        {
            checkForInput = false;
            interactIndicator.SetActive(false);
            ToggleRevive(false);
        }
    }

    public void ToggleRevive(bool b)
    {
        if (!Player.localPlayer.entity.health.isDead)
        {
            reviving = b;
            progressPanel.SetActive(b);

            if (b)
            {
                OnStartRevive();
            }
            else
            {
                OnEndRevive();
            }
        }
    }

    private void Revive()
    {
        reviving = false;
        checkForInput = false;
        photonView.RPC("SyncRevive", RpcTarget.All);
    }

    [PunRPC]
    private void SyncRevive()
    {
        //ToggleReviveZone(false);

        if (initialised)
        {
            Player.localPlayer.Respawn(false);
            NotificationManager.instance.NewNotification("<color=red>" + PhotonNetwork.NickName + "</color> has been revived!");
        }
    }

    public override void OnDisable()
    {
        Player.localPlayer.entity.OnDeath.RemoveListener(() => ToggleRevivable(true));
        Player.localPlayer.entity.OnRevive.RemoveListener(() => ToggleRevivable(false));
    }
}