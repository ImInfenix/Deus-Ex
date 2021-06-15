using InfenixTools.DesignPatterns;
using Photon.Realtime;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    StateMachine<GameManager> stateMachine;
    public StateMachine<GameManager> StateMachine => stateMachine;

    private GameCommunicationsHandler communicationsHandler;
    public GameCommunicationsHandler CommunicationsHandler => communicationsHandler;

    [HideInInspector]
    public Player humanPlayer;
    [HideInInspector]
    public Player godPlayer;

    public enum PlayerType { Human, God }
    private PlayerType localPlayerType;

    public PlayerType LocalPlayerType => localPlayerType;

    protected override void Awake()
    {
        base.Awake();
        communicationsHandler = gameObject.AddComponent<GameCommunicationsHandler>();
    }

    private void Start()
    {
        stateMachine = new StateMachine<GameManager>(PickRoleState.Instance, this);
    }

    private void Update()
    {
        stateMachine.Update(this);
    }

    public void AssignPlayers(Player human, Player god)
    {
        humanPlayer = human;
        godPlayer = god;

        Player local = Photon.Pun.PhotonNetwork.LocalPlayer;
        if (local == human)
        {
            localPlayerType = PlayerType.Human;
        }
        else if (local == god)
        {
            localPlayerType = PlayerType.God;
        }

        FindObjectOfType<PlayerTeamDisplayer>().SetTeam(localPlayerType.ToString());
    }

    public bool IsMine(PlayerType unitType)
    {
        return unitType == localPlayerType;
    }

    public bool IsHuman()
    {
        return localPlayerType == PlayerType.Human;
    }

    public bool IsGod()
    {
        return localPlayerType == PlayerType.God;
    }

    public void RequestEndOfTurn()
    {
        (HumanTurnState.Instance as HumanTurnState).RequestEndOfTurn();
        (GodTurnState.Instance as GodTurnState).RequestEndOfTurn();
    }

    private void OnEnable()
    {
        HumanTurnState.OnExit += CheckForGodVisibilty;
        GodTurnState.OnExit += CheckForGodVisibilty;
    }

    private void OnDisable()
    {
        HumanTurnState.OnExit -= CheckForGodVisibilty;
        GodTurnState.OnExit -= CheckForGodVisibilty;
    }

    private void CheckForGodVisibilty()
    {
        SelectableUnit god = SelectableUnit.GetGodUnit();

        foreach (var unit in SelectableUnit.selectableUnits.Values)
        {
            if (unit.UnitType == PlayerType.Human && god.IsNextTo(unit))
            {
                GodVisibiltyHandler.Show(true);
                return;
            }
        }

        GodVisibiltyHandler.Show(false);
    }

    public bool CheckForHumanWinCondition()
    {
        bool humanWin = false;
        SelectableUnit god = SelectableUnit.GetGodUnit();
        foreach (var unit in SelectableUnit.selectableUnits.Values)
        {
            if (unit.UnitType == PlayerType.Human && unit.gridPosition == god.gridPosition)
                humanWin = true;
        }
        if (humanWin)
        {
            (GameEndedState.Instance as GameEndedState).winningPlayer = PlayerType.Human;
            stateMachine.ChangeState(GameEndedState.Instance, this);
        }

        return humanWin;
    }
}
