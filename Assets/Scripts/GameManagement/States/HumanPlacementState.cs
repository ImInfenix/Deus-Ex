using InfenixTools.DesignPatterns;
using UnityEngine;

public class HumanPlacementState : State<HumanPlacementState, GameManager>
{
    [SerializeField]
    private int unitsToPlace = 5;
    private int placedUnits;

    [SerializeField]
    private GameObject humanPrefab;

    private Board board;

    public override void Enter(GameManager objectInstance)
    {
        base.Enter(objectInstance);

        placedUnits = 0;
        board = FindObjectOfType<Board>();

        FindObjectOfType<StateDisplayer>().SetState("Human placement");
    }

    public override void Execute(GameManager objectInstance)
    {
        base.Execute(objectInstance);

        if (GameManager.Instance.IsHuman())
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceUnit();
            }
        }

        if (placedUnits >= unitsToPlace)
            objectInstance.StateMachine.ChangeState(GodPlacementState.Instance, objectInstance);
    }

    public void TryPlaceUnit()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            Tile t = hit.collider.GetComponent<Tile>();
            if (t == null)
                return;

            FindObjectOfType<GameCommunicationsHandler>().PlaceUnit(t.gridPosition);
        }
    }

    public void PlaceUnit(Vector2Int gridPosition)
    {
        Vector3 unitPosition = board.GetTile(gridPosition).transform.position;

        Instantiate(humanPrefab, unitPosition, Quaternion.identity).GetComponent<SelectableUnit>().gridPosition = gridPosition; ;
        placedUnits++;
    }
}
