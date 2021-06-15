using InfenixTools.DataStructures;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    private Vector2Int boardSize = new Vector2Int(8, 8);
    [SerializeField]
    private float tileSize = 1f;

    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private GameObject templePrefab;

    [Header("Board characteristics")]
    [SerializeField]
    private int temples = 3;

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
                GameObject instance = Instantiate(tilePrefab, transform);
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

    public Tile[] GetFourNeighbours(Vector2Int gridPosition)
    {
        List<Vector2Int> positionsToTest = new List<Vector2Int>() {
            gridPosition + Vector2Int.up,
            gridPosition + Vector2Int.down,
            gridPosition + Vector2Int.right,
            gridPosition + Vector2Int.left
        };
        return GetTiles(positionsToTest);
    }

    public Tile[] GetHeightNeighbours(Vector2Int gridPosition)
    {
        List<Vector2Int> positionsToTest = new List<Vector2Int>() {
            gridPosition + Vector2Int.up,
            gridPosition + Vector2Int.up + Vector2Int.left,
            gridPosition + Vector2Int.up + Vector2Int.right,
            gridPosition + Vector2Int.down,
            gridPosition + Vector2Int.down + Vector2Int.left,
            gridPosition + Vector2Int.down + Vector2Int.right,
            gridPosition + Vector2Int.right,
            gridPosition + Vector2Int.left
        };
        return GetTiles(positionsToTest);
    }

    public Tile[] GetTiles(List<Vector2Int> positionsToTest)
    {
        List<Tile> neighbours = new List<Tile>();

        foreach (Vector2Int pos in positionsToTest)
        {
            if (gridObjects.IsPositionInGrid(pos))
                neighbours.Add(gridObjects.Get(pos));
        }

        return neighbours.ToArray();
    }

    public void PlaceTemples()
    {
        for (int n = 0; n < temples; n++)
        {
            Vector2Int chosenRandomPosition = Vector2Int.zero;
            bool foundPossiblePlace = false;
            while (!foundPossiblePlace)
            {
                chosenRandomPosition = new Vector2Int(Random.Range(1, boardSize.x - 1), Random.Range(1, boardSize.y - 1));
                if (gridObjects.Get(chosenRandomPosition).type == Tile.TileType.Empty)
                {
                    bool isPositionFree = true;
                    foreach (var t in GetHeightNeighbours(chosenRandomPosition))
                    {
                        if (t.type == Tile.TileType.Temple)
                        {
                            isPositionFree = false;
                            break;
                        }
                    }
                    foundPossiblePlace = isPositionFree;
                }
            }

            gridObjects.Get(chosenRandomPosition).type = Tile.TileType.Temple;
            FindObjectOfType<GameCommunicationsHandler>().PlaceTile(Tile.TileType.Temple, chosenRandomPosition);
        }
    }

    public void PlaceTileAt(Tile.TileType tileType, int x, int y)
    {
        Tile t = gridObjects.Get(x, y);

        GameObject toInstantiate = null;

        switch (tileType)
        {
            case Tile.TileType.Mountain:
                break;
            case Tile.TileType.Forest:
                break;
            case Tile.TileType.River:
                break;
            case Tile.TileType.Temple:
                toInstantiate = templePrefab;
                break;
        }

        t.type = tileType;
        Instantiate(toInstantiate, t.transform, false);
    }
}
