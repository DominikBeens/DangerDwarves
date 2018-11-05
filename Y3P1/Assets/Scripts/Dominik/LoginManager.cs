﻿using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine;

// BEWARE! Ugly af and unorganised code ahead. 
public class LoginManager : MonoBehaviourPunCallbacks
{

    private string gameVersion = "1.0.0";
    private const string playerNamePrefKey = "PlayerName";
    private bool isConnecting;
    private bool solo;
    private Camera cam;
    private Transform camTransform;
    private bool preparingOfflineMode;

    private enum ConnectSetting { Offline, Random, Custom };
    private ConnectSetting currentConnectSetting;
    private string currentConnectionRoomName;

    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject playMenuPanel;
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private GameObject connectionProgress;
    [SerializeField] private TextMeshProUGUI roomCountText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Transform dwarfLookAt;
    [SerializeField] private HeadTracking dwarfHeadTracking;
    [SerializeField] private float cameraSmoothSpeed = 2f;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 10;
        PhotonNetwork.SerializationRate = 5;

        SetUpNameInputField();
        //playMenuPanel.SetActive(true);
        //roomPanel.SetActive(false);
        connectionProgress.SetActive(false);

        cam = Camera.main;
        camTransform = Camera.main.transform;
        camTransform.eulerAngles = Vector3.zero;

        dwarfHeadTracking.Initialise(true);

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Update()
    {
        if (roomCountText && playerCountText)
        {
            roomCountText.text = PhotonNetwork.IsConnected ? "Open rooms: <color=red>" + PhotonNetwork.CountOfRooms : "Open rooms: <color=red>?";
            playerCountText.text = PhotonNetwork.IsConnected ? "Active dwarves: <color=red>" + Mathf.Clamp(PhotonNetwork.CountOfPlayers - 1, 0, 9999) : "Active dwarves: <color=red>?";
        }

        Vector3 mouseInWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 7));
        Vector3 lookat = new Vector3(mouseInWorldPos.x / 10, mouseInWorldPos.y / 10, 10);
        Quaternion targetRotation = Quaternion.LookRotation(lookat - transform.position, Vector3.up);
        camTransform.rotation = Quaternion.Slerp(camTransform.rotation, targetRotation, Time.deltaTime * cameraSmoothSpeed);

        dwarfLookAt.position = new Vector3(mouseInWorldPos.x / 7, (mouseInWorldPos.y - 10) / 7, -7);
    }

    private void Connect(ConnectSetting connectSetting, string roomName = null)
    {
        // Player name is empty.
        if (string.IsNullOrEmpty(nameInputField.text) || nameInputField.text.All(char.IsWhiteSpace))
        {
            return;
        }

        currentConnectSetting = connectSetting;
        currentConnectionRoomName = roomName;

        if (currentConnectSetting == ConnectSetting.Offline && PhotonNetwork.IsConnected)
        {
            preparingOfflineMode = true;
            PhotonNetwork.Disconnect();
            return;
        }

        PhotonNetwork.OfflineMode = currentConnectSetting == ConnectSetting.Offline ? true : false;
        isConnecting = true;
        connectionProgress.SetActive(true);

        if (PhotonNetwork.IsConnected)
        {
            OnConnectedToMaster();
        }
        else
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            if (PhotonNetwork.IsConnected)
            {
                switch (currentConnectSetting)
                {
                    case ConnectSetting.Random:

                        PhotonNetwork.JoinRandomRoom();
                        break;
                    case ConnectSetting.Custom:

                        PhotonNetwork.JoinOrCreateRoom(currentConnectionRoomName, new RoomOptions { MaxPlayers = 10, IsVisible = false }, TypedLobby.Default);
                        break;
                    default:
                        PhotonNetwork.JoinRandomRoom();
                        break;
                }
            }
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void PlayButton(bool offline)
    {
        Connect(offline ? ConnectSetting.Offline : ConnectSetting.Random);
    }

    public void QuickJoinButton()
    {
        Connect(ConnectSetting.Random);
    }

    public void JoinCustomButton(TMP_InputField roomName)
    {
        if (string.IsNullOrEmpty(roomName.text) || roomName.text.All(char.IsWhiteSpace))
        {
            return;
        }

        Connect(ConnectSetting.Custom, roomName.text);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 10 }, null);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //playMenuPanel.SetActive(false);
        //roomPanel.SetActive(true);
        connectionProgress.SetActive(false);

        if (preparingOfflineMode)
        {
            Connect(ConnectSetting.Offline);
        }
    }

    private void SetUpPanelsWhenConnecting()
    {
        //playMenuPanel.SetActive(false);
        //roomPanel.SetActive(false);
        connectionProgress.SetActive(true);
    }

    private void SetUpNameInputField()
    {
        string defaultName = "";
        if (nameInputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                nameInputField.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;
    }

    public void SetPlayerName(TMP_InputField inputField)
    {
        if (string.IsNullOrEmpty(inputField.text))
        {
            return;
        }
        PhotonNetwork.NickName = inputField.text;

        PlayerPrefs.SetString(playerNamePrefKey, inputField.text);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}