using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCreationHandler : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject playerEntryPrefab;

    Dictionary<Player, PlayerEntry> existingPlayers;

    private void Awake()
    {
        existingPlayers = new Dictionary<Player, PlayerEntry>();
    }

    public override void OnJoinedRoom()
    {
        AddEntry(PhotonNetwork.LocalPlayer, true);

        foreach (Player p in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (p.IsLocal)
                continue;
            AddEntry(p);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddEntry(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player leftPlayer)
    {
        RemoveEntry(leftPlayer);
    }

    private void AddEntry(Player newPlayer, bool isLocalPlayer = false)
    {
        PlayerEntry newEntry = Instantiate(playerEntryPrefab, transform).GetComponent<PlayerEntry>();
        newEntry.attachedPlayer = newPlayer;
        newEntry.gameFinder = this;

        newEntry.GetComponentInChildren<Text>().text = newPlayer.NickName;
        newEntry.GetComponent<Button>().enabled = isLocalPlayer;

        existingPlayers.Add(newPlayer, newEntry);
    }

    private void RemoveEntry(Player leftPlayer)
    {
        if (existingPlayers.TryGetValue(leftPlayer, out PlayerEntry entry))
        {
            existingPlayers.Remove(leftPlayer);
            Destroy(entry.gameObject);
        }
    }

    public void SetPlayerReady(Player player, bool value)
    {
        photonView.RPC(nameof(SetPlayerReadyRPC), RpcTarget.AllViaServer, player, value);
    }

    [PunRPC]
    private void SetPlayerReadyRPC(Player player, bool value)
    {
        existingPlayers[player].SetReady(value);

        if (PhotonNetwork.IsMasterClient)
            CheckForGameToLaunch();
    }

    private void CheckForGameToLaunch()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
            return;

        foreach (PlayerEntry pe in existingPlayers.Values)
            if (!pe.IsReady)
                return;

        PhotonNetwork.LoadLevel("Game");
    }
}
