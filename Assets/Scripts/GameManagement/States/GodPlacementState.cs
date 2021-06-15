using InfenixTools.DesignPatterns;
using UnityEngine;

public class GodPlacementState : State<GodPlacementState, GameManager>
{
    [SerializeField]
    private GameObject godPrefab;

    bool placed;

    public override void Enter(GameManager objectInstance)
    {
        base.Enter(objectInstance);
        FindObjectOfType<StateDisplayer>().SetState("God placement");
        placed = false;

        GodVisibiltyHandler.Show(false);
    }

    public override void Execute(GameManager objectInstance)
    {
        base.Execute(objectInstance);

        if (GameManager.Instance.IsGod())
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceUnit();
            }
        }

        if (placed)
            objectInstance.StateMachine.ChangeState(HumanTurnState.Instance, objectInstance);
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
        Vector3 unitPosition = FindObjectOfType<Board>().GetTile(gridPosition).transform.position;

        Instantiate(godPrefab, unitPosition, Quaternion.identity).GetComponent<SelectableUnit>().gridPosition = gridPosition; ;
        placed = true;
    }
}
