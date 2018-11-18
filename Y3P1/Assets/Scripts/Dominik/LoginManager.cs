using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
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
    private int activeDwarves;
    private int openRooms;

    private enum ConnectSetting { Offline, Random, Custom };
    private ConnectSetting currentConnectSetting;
    private string currentConnectionRoomName;

    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject connectionProgress;
    [SerializeField] private Transform dwarfLookAt;
    [SerializeField] private float cameraSmoothSpeed = 2f;
    [SerializeField] private List<GameObject> miniDwarves = new List<GameObject>();
    [SerializeField] private List<GameObject> miniBraziers = new List<GameObject>();
    [SerializeField] private GameObject dwarf;
    [SerializeField] private GameObject credits;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        SetUpNameInputField();
        connectionProgress.SetActive(false);

        cam = Camera.main;
        camTransform = Camera.main.transform;
        camTransform.eulerAngles = Vector3.zero;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            int checkActiveDwarves = PhotonNetwork.CountOfPlayers - 1;
            if (checkActiveDwarves != activeDwarves)
            {
                activeDwarves = checkActiveDwarves;
                UpdateActivePlayers();
            }

            int checkOpenRooms = PhotonNetwork.CountOfRooms;
            if (checkOpenRooms != openRooms)
            {
                openRooms = checkOpenRooms;
                UpdateOpenRooms();
            }
        }

        Vector3 mouseInWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 7));
        Vector3 lookat = new Vector3(mouseInWorldPos.x / 10, mouseInWorldPos.y / 10, 125);
        Quaternion targetRotation = Quaternion.LookRotation(lookat - transform.position, Vector3.up);
        camTransform.rotation = Quaternion.Slerp(camTransform.rotation, targetRotation, Time.deltaTime * cameraSmoothSpeed);

        dwarfLookAt.position = new Vector3(mouseInWorldPos.x, mouseInWorldPos.y - 2, -6);
    }

    private void UpdateActivePlayers()
    {
        for (int i = 0; i < miniDwarves.Count; i++)
        {
            miniDwarves[i].SetActive((i <= activeDwarves - 1) ? true : false);
        }
    }

    private void UpdateOpenRooms()
    {
        for (int i = 0; i < miniBraziers.Count; i++)
        {
            miniBraziers[i].SetActive((i < openRooms) ? true : false);
        }
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

    public void GoToURL(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
    }

    public void ToggleCredits()
    {
        dwarf.SetActive(!dwarf.activeInHierarchy);
        credits.SetActive(!credits.activeInHierarchy);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        connectionProgress.SetActive(false);

        if (preparingOfflineMode)
        {
            Connect(ConnectSetting.Offline);
        }
    }

    private void SetUpPanelsWhenConnecting()
    {
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