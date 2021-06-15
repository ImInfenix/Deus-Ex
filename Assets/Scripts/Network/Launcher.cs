using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Infenix.DeusEX
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        private enum DisplayStatus { Connection, Connecting, Lobby, None }

        [Header("Configuration")]
        [SerializeField]
        private byte maxPlayers = 2;

        [Header("HUDs objects")]
        [SerializeField]
        private GameObject launcherHUD;
        [SerializeField]
        private GameObject lobbyHUD;

        [Header("Launcher parts")]
        [SerializeField]
        private GameObject controlPanel;
        [SerializeField]
        private GameObject progressLabel;

        string gameVersion = "1";
        bool isConnecting;
        bool isQuitting;

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            isQuitting = false;
        }

        private void Start()
        {
            ShowLauncher(DisplayStatus.Connection);
        }

        private void OnApplicationQuit()
        {
            isQuitting = true;
        }

        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;

                ShowLauncher(DisplayStatus.Lobby);
            }
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            ShowLauncher(DisplayStatus.Connection);

            isConnecting = false;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayers });
        }

        public void Connect()
        {
            ShowLauncher(DisplayStatus.Connecting);

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinLobby();
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        private void ShowLauncher(DisplayStatus status)
        {
            if (isQuitting) return;

            switch (status)
            {
                case DisplayStatus.Connection:
                    launcherHUD.SetActive(true);
                    lobbyHUD.SetActive(false);

                    controlPanel.SetActive(true);
                    progressLabel.SetActive(false);
                    break;

                case DisplayStatus.Connecting:
                    launcherHUD.SetActive(true);
                    lobbyHUD.SetActive(false);

                    controlPanel.SetActive(false);
                    progressLabel.SetActive(true);
                    break;

                case DisplayStatus.Lobby:
                    launcherHUD.SetActive(false);
                    lobbyHUD.SetActive(true);
                    break;
            }
        }
    }
}
