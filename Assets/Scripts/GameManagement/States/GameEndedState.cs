using InfenixTools.DesignPatterns;
using UnityEngine;
using UnityEngine.UI;

public class GameEndedState : State<GameEndedState, GameManager>
{
    [SerializeField]
    private GameObject winnerScreen;
    [SerializeField]
    private Text winnerText;

    [HideInInspector]
    public GameManager.PlayerType winningPlayer;

    protected override void Awake()
    {
        base.Awake();

        winnerScreen.SetActive(false);
    }

    public override void Enter(GameManager objectInstance)
    {
        base.Enter(objectInstance);

        FindObjectOfType<StateDisplayer>().SetState("Win Screen");

        winnerScreen.gameObject.SetActive(true);
        winnerText.text = $"{winningPlayer} wins";
        FindObjectOfType<TopViewCamera>().automaticallyRotate = true;
    }
}
