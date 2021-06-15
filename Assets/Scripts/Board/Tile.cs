using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType { Empty, Mountain, Forest, River, Temple }

    [Header("Properties")]
    public Vector2Int gridPosition;
    public TileType type;

    [Header("Properties")]
    [SerializeField]
    private Texture2D defaultTexture;
    [SerializeField]
    private Texture2D highlightedTexture;

    [HideInInspector]
    public Board board;

    private bool isHighlighted;
    public bool IsHighlighted
    {
        get => isHighlighted;
        set
        {
            isHighlighted = value;
            Texture2D toUse = isHighlighted ? highlightedTexture : defaultTexture;
            meshRenderer.material.SetTexture("_MainTex", toUse);
        }
    }

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        IsHighlighted = false;
    }
}
