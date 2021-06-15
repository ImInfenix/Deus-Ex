using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameCommunicationsHandler : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Player playerOne = null, playerTwo = null;
            foreach (Player p in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (playerOne == null)
                    playerOne = p;
                else if (playerTwo == null)
                    playerTwo = p;
                else break;
            }
            photonView.RPC(nameof(AssignRoleToPlayers), RpcTarget.AllViaServer, playerOne, playerTwo);
        }
    }

    public void SwapRoles()
    {
        PickRoleState instance = PickRoleState.Instance as PickRoleState;
        photonView.RPC(nameof(AssignRoleToPlayers), RpcTarget.AllViaServer, instance.GodPlayer, instance.HumanPlayer);
    }

    [PunRPC]
    private void AssignRoleToPlayers(Player humanPlayer, Player godPlayer)
    {
        ((PickRoleState)PickRoleState.Instance).HumanPlayer = humanPlayer;
        ((PickRoleState)PickRoleState.Instance).GodPlayer = godPlayer;
    }

    public void LaunchGame()
    {
        photonView.RPC(nameof(SendLaunchMessage), RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void SendLaunchMessage()
    {
        (PickRoleState.Instance as PickRoleState).launchGame = true;
    }

    public void PlaceUnit(Vector2Int gridPosition)
    {
        photonView.RPC(nameof(PlaceUnitAt), RpcTarget.AllViaServer, gridPosition.x, gridPosition.y, (int)GameManager.Instance.LocalPlayerType);
    }

    [PunRPC]
    private void PlaceUnitAt(int x, int y, int unitType)
    {
        GameManager.PlayerType playerType = (GameManager.PlayerType)unitType;
        switch (playerType)
        {
            case GameManager.PlayerType.Human:
                (HumanPlacementState.Instance as HumanPlacementState).PlaceUnit(new Vector2Int(x, y));
                break;
            case GameManager.PlayerType.God:
                (GodPlacementState.Instance as GodPlacementState).PlaceUnit(new Vector2Int(x, y));
                break;
        }
    }

    public void EndTurn()
    {
        photonView.RPC(nameof(EndTurnRPC), RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void EndTurnRPC()
    {
        (HumanTurnState.Instance as HumanTurnState).endTurn = true;
        (GodTurnState.Instance as GodTurnState).endTurn = true;
    }

    public void MoveUnit(uint id, Vector2Int position)
    {
        photonView.RPC(nameof(MoveUnitRPC), RpcTarget.AllViaServer, (int)id, position.x, position.y);
    }

    [PunRPC]
    public void MoveUnitRPC(int id, int x, int y)
    {
        SelectableUnit.selectableUnits[(uint)id].MoveTo(new Vector2Int(x, y));
    }

    public void PlaceTile(Tile.TileType tileType, Vector2Int position)
    {
        photonView.RPC(nameof(PlaceTileRPC), RpcTarget.AllViaServer, (int)tileType, position.x, position.y);
    }

    [PunRPC]
    public void PlaceTileRPC(int tileType, int x, int y)
    {
        FindObjectOfType<Board>().PlaceTileAt((Tile.TileType)tileType, x, y);
    }
}
