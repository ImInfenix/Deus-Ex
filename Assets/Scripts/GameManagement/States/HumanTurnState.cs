using InfenixTools.DesignPatterns;
using UnityEngine;

public class HumanTurnState : State<HumanTurnState, GameManager>
{
    private bool requestEndOfTurn;
    [HideInInspector]
    public bool endTurn;
    [SerializeField]
    private GameObject endOfTurnButton;

    [HideInInspector]
    public int unitsMovedDuringTurn;

    public override void Enter(GameManager objectInstance)
    {
        base.Enter(objectInstance);

        FindObjectOfType<StateDisplayer>().SetState("Human turn");
        requestEndOfTurn = false;
        endTurn = false;
        if (GameManager.Instance.LocalPlayerType == GameManager.PlayerType.Human)
            endOfTurnButton.SetActive(true);
        unitsMovedDuringTurn = 0;
    }

    public override void Execute(GameManager objectInstance)
    {
        base.Execute(objectInstance);

        if (endTurn)
            objectInstance.StateMachine.ChangeState(GodTurnState.Instance, objectInstance);
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
        if (GameManager.Instance.LocalPlayerType == GameManager.PlayerType.Human)
            FindObjectOfType<GameCommunicationsHandler>().EndTurn();
    }
}
