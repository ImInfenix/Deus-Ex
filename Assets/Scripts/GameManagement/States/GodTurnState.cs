using InfenixTools.DesignPatterns;
using UnityEngine;

public class GodTurnState : State<GodTurnState, GameManager>
{
    private bool requestEndOfTurn;
    [HideInInspector]
    public bool endTurn;
    [SerializeField]
    private GameObject endOfTurnButton;

    public override void Enter(GameManager objectInstance)
    {
        base.Enter(objectInstance);

        FindObjectOfType<StateDisplayer>().SetState("God turn");
        requestEndOfTurn = false;
        endTurn = false;
        if (GameManager.Instance.LocalPlayerType == GameManager.PlayerType.God)
            endOfTurnButton.SetActive(true);
    }

    public override void Execute(GameManager objectInstance)
    {
        base.Execute(objectInstance);

        if (endTurn)
            objectInstance.StateMachine.ChangeState(HumanTurnState.Instance, objectInstance);
    }

    public override void Exit(GameManager objectInstance)
    {
        base.Exit(objectInstance);

        endOfTurnButton.SetActive(false);
    }

    public void RequestEndOfTurn()
    {
        if (requestEndOfTurn == true)
            return;

        requestEndOfTurn = true;
        if (GameManager.Instance.LocalPlayerType == GameManager.PlayerType.God)
            FindObjectOfType<GameCommunicationsHandler>().EndTurn();
    }
}
