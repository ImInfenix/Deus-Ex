using InfenixTools.DataStructures;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    [SerializeField]
    private Vector2 boardSize = new Vector2(8, 8);
    [SerializeField]
    private float tileSize = 1f;

    [SerializeField]
    private GameObject tileTemplate;

    private GenericGrid<Tile> gridObjects;

    private void Start()
    {
        CreateNewBoard();
    }

    private void CreateNewBoard()
    {
        gridObjects = new GenericGrid<Tile>(8, 8, tileSize, null);

        for (int i = 0; i < boardSize.x; i++)
        {
            for (int j = 0; j < boardSize.y; j++)
            {
                GameObject instance = Instantiate(tileTemplate, transform);
                Tile tile = instance.GetComponent<Tile>();
                tile.type = Tile.TileType.Empty;
                instance.transform.position = new Vector3(i - boardSize.x / 2f + tileSize / 2f, 0, j - boardSize.y / 2f + tileSize / 2f);
                tile.gridPosition = new Vector2Int(i, j);
                tile.board = this;
                gridObjects.Set(i, j, tile);
            }
        }
    }

    public Tile GetTile(Vector2Int position)
    {
        return gridObjects.Get(position);
    }

    public Tile[] GetNeighbours(Vector2Int gridPosition)
    {
        List<Vector2Int> positionsToTest = new List<Vector2Int>() {
            gridPosition + Vector2Int.up,
            gridPosition + Vector2Int.down,
            gridPosition + Vector2Int.right,
            gridPosition + Vector2Int.left
        };
        List<Tile> neighbours = new List<Tile>();

        foreach (Vector2Int pos in positionsToTest)
        {
            if (gridObjects.IsPositionInGrid(pos))
                neighbours.Add(gridObjects.Get(pos));
        }

        return neighbours.ToArray();
    }
}
