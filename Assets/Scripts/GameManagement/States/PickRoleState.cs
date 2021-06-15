using InfenixTools.DesignPatterns;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PickRoleState : State<PickRoleState, GameManager>
{
    [Header("Role selection")]
    [SerializeField]
    private GameObject roleSelectionHUD;
    [SerializeField]
    private Text humanPlayerText;
    [SerializeField]
    private Text godPlayerText;

    [Header("Game")]
    [SerializeField]
    private GameObject playerHUD;

    Player humanPlayer;
    Player godPlayer;

    public Player HumanPlayer
    {
        get => humanPlayer;
        set
        {
            humanPlayer = value;
            humanPlayerText.text = humanPlayer == null ? string.Empty : humanPlayer.NickName;
        }
    }
    public Player GodPlayer
    {
        get => godPlayer;
        set
        {
            godPlayer = value;
            godPlayerText.text = godPlayer == null ? string.Empty : godPlayer.NickName;
        }
    }

    [HideInInspector]
    public bool launchGame;

    public override void Enter(GameManager objectInstance)
    {
        base.Enter(objectInstance);

        roleSelectionHUD.SetActive(true);
        playerHUD.SetActive(false);
        FindObjectOfType<TopViewCamera>().automaticallyRotate = true;

        humanPlayer = null;
        godPlayer = null;

        launchGame = false;
    }

    public override void Execute(GameManager objectInstance)
    {
        base.Execute(objectInstance);

        if (launchGame)
            objectInstance.StateMachine.ChangeState(HumanPlacementState.Instance, objectInstance);
    }

    public override void Exit(GameManager objectInstance)
    {
        base.Exit(objectInstance);

        roleSelectionHUD.SetActive(false);
        playerHUD.SetActive(true);
        FindObjectOfType<TopViewCamera>().automaticallyRotate = false;

        objectInstance.AssignPlayers(humanPlayer, godPlayer);
    }

    public void SwapRoles()
    {
        FindObjectOfType<GameCommunicationsHandler>().SwapRoles();
    }

    public void LaunchGame()
    {
        FindObjectOfType<GameCommunicationsHandler>().LaunchGame();
    }
}
