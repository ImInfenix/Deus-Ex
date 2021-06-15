using InfenixTools.DesignPatterns;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitMovementHandler : Singleton<UnitMovementHandler>
{
    private SelectableUnit selectedUnit;
    public SelectableUnit SelectedUnit => selectedUnit;

    Camera attachedCamera;

    private void Start()
    {
        attachedCamera = Camera.main;
    }

    private void OnEnable()
    {
        HumanTurnState.OnExecute += UpdateHumanLogic;
        GodTurnState.OnExecute += UpdateGodLogic;

        HumanTurnState.OnExit += UnselectUnit;
        GodTurnState.OnExit += UnselectUnit;
    }

    private void OnDisable()
    {
        HumanTurnState.OnExecute -= UpdateHumanLogic;
        GodTurnState.OnExecute -= UpdateGodLogic;

        HumanTurnState.OnExit -= UnselectUnit;
        GodTurnState.OnExit -= UnselectUnit;
    }

    private void UpdateHumanLogic()
    {
        UpdateLogic(GameManager.PlayerType.Human);
    }

    private void UpdateGodLogic()
    {
        UpdateLogic(GameManager.PlayerType.God);
    }

    private void UpdateLogic(GameManager.PlayerType turnPlayerType)
    {
        if (turnPlayerType != GameManager.Instance.LocalPlayerType)
            return;

        if (turnPlayerType == GameManager.PlayerType.Human && (HumanTurnState.Instance as HumanTurnState).unitsMovedDuringTurn >= 4)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            TryToSelectAUnit();
        }

        if (Input.GetMouseButtonDown(1))
        {
            TryMoveSelectedUnit();
        }
    }

    private void TryToSelectAUnit()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Physics.Raycast(attachedCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            SelectableUnit selectedUnit = hit.collider.GetComponentInParent<SelectableUnit>();
            if (selectedUnit != null && GameManager.Instance.IsMine(selectedUnit.UnitType) && selectedUnit.RemaniningMovement > 0)
            {
                SetSelectedUnit(selectedUnit);
                return;
            }
        }

        SetSelectedUnit(null);
    }

    public void SetSelectedUnit(SelectableUnit value)
    {
        if (selectedUnit != null)
            selectedUnit.SetSelected(false);
        selectedUnit = value;
        if (selectedUnit != null)
        {
            selectedUnit.SetSelected(true);
        }
    }

    private void UnselectUnit()
    {
        SetSelectedUnit(null);
    }

    private void TryMoveSelectedUnit()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Physics.Raycast(attachedCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            Tile clickedTile = hit.collider.GetComponentInParent<Tile>();
            if (clickedTile != null && clickedTile.IsHighlighted)
            {
                FindObjectOfType<GameCommunicationsHandler>().MoveUnit(SelectedUnit.Id, clickedTile.gridPosition);
                UnselectUnit();
            }
        }
    }
}
