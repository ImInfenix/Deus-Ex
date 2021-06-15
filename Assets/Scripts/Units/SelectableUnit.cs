using InfenixTools.DataStructures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableUnit : MonoBehaviour
{
    public static Dictionary<uint, SelectableUnit> selectableUnits;
    private static IdGenerator idGenerator;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject selectionIndicatorPrefab;

    [SerializeField]
    private Transform selectionIndicatorAnchorPoint;

    private GameObject selectionIndicator;

    [SerializeField]
    private GameManager.PlayerType unitType;
    public GameManager.PlayerType UnitType => unitType;

    [HideInInspector]
    public Vector2Int gridPosition;

    public int defaultMovement = 1;

    private int remainingMovement;
    public int RemaniningMovement => remainingMovement;

    private uint id;
    public uint Id => id;

    private void Awake()
    {
        if (selectableUnits == null)
            selectableUnits = new Dictionary<uint, SelectableUnit>();
        if (idGenerator == null)
            idGenerator = new IdGenerator();

        id = idGenerator.Next;
        selectableUnits.Add(id, this);
    }

    private void OnEnable()
    {
        HumanTurnState.OnEnter += ResetUnitForNewTurn;
        GodTurnState.OnEnter += ResetUnitForNewTurn;
    }

    private void OnDisable()
    {
        HumanTurnState.OnEnter -= ResetUnitForNewTurn;
        GodTurnState.OnEnter -= ResetUnitForNewTurn;
    }

    public void SetSelected(bool value)
    {
        if (value)
        {
            selectionIndicator = Instantiate(selectionIndicatorPrefab);
            selectionIndicator.transform.SetParent(selectionIndicatorAnchorPoint, false);

            foreach (var tile in FindObjectOfType<Board>().GetNeighbours(gridPosition))
            {
                tile.IsHighlighted = true;
            }
        }
        else
        {
            foreach (var tile in FindObjectOfType<Board>().GetNeighbours(gridPosition))
            {
                tile.IsHighlighted = false;
            }

            Destroy(selectionIndicator);
        }
    }

    public void MoveTo(Vector2Int gridPosition)
    {
        remainingMovement--;
        SetSelected(false);
        this.gridPosition = gridPosition;

        if (GameManager.Instance.CheckForHumanWinCondition())
            GodVisibiltyHandler.Show(true);
        else if (UnitType == GameManager.PlayerType.God)
            GodVisibiltyHandler.Show(false);

        StartCoroutine(Move(FindObjectOfType<Board>().GetTile(gridPosition).transform.position));

        if (unitType == GameManager.PlayerType.Human)
            (HumanTurnState.Instance as HumanTurnState).unitsMovedDuringTurn++;
    }

    private IEnumerator Move(Vector3 to)
    {
        if (animator != null)
            animator.SetBool("IsWalking", true);

        transform.LookAt(to);

        Vector3 from = transform.position;
        float time = 0;
        while (time < 1f)
        {
            transform.position = Vector3.Lerp(from, to, time);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = to;

        if (animator != null)
            animator.SetBool("IsWalking", false);
    }

    private void ResetUnitForNewTurn()
    {
        remainingMovement = defaultMovement;
    }

    public static SelectableUnit GetGodUnit()
    {
        foreach (var unit in selectableUnits.Values)
            if (unit.unitType == GameManager.PlayerType.God)
                return unit;

        return null;
    }

    public bool IsNextTo(SelectableUnit unit)
    {
        return Vector2Int.Distance(gridPosition, unit.gridPosition) <= 1.1;
    }
}
