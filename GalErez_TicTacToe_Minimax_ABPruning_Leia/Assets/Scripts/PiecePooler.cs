using System.Collections.Generic;
using UnityEngine;

public class PiecePooler : MonoBehaviour
{
    /*[Header("Prefabs")]
    [SerializeField] private GameObject _xPrefab;
    [SerializeField] private GameObject _oPrefab;

    [Header("Configuration for each pool")]
    [SerializeField] private int _defaultCapacity = 5;
    [SerializeField] private int _maxSize = 5;

    private List<GameObject> _xPool;
    private List<GameObject> _oPool;

    private void Awake()
    {
        _xPool = new();
        _oPool = new();
    }
    private void Start()
    {
        InitPool(_xPool, TileType.X);
        InitPool(_oPool, TileType.O);
    }

    private void InitPool(List<GameObject> pool, TileType pieceType)
    {
        GameObject prefab = pieceType == TileType.X ? _xPrefab : _oPrefab;
        for (int i = 0; i < _defaultCapacity; i++)
        {
            if (pool.Count >= _defaultCapacity || pool.Count >= _maxSize) return;
            GameObject piece = Instantiate(prefab);
            SetupPiece(piece);
            pool.Add(piece);
            Debugger.Log($"{pieceType} created");
        }
    }
    private void SetupPiece(GameObject piece) // set's the new piece's to be ready for work 
    {
        piece.transform.SetParent(transform);
        piece.transform.localPosition = Vector2.zero;
        piece.SetActive(false); 
    }

    #region Public method - please don't overlook the summary :)
    /// <summary>
    /// Activates a piece and detach it from the pool - no longer it's child in hierarchy.
    /// </summary>
    /// <param name="pieceType">The type of the piece. Will be either TileType.X or TileType.O. It will never be TileType.Empty.</param>
    /// <returns> A Piece as GameObject</returns>
    public GameObject GetPiece(TileType pieceType, Transform newParent, bool isActive)
    {
        Debug.Log($"Requesting piece of type: {pieceType}");
        List<GameObject> pool = pieceType == TileType.X ? _xPool : _oPool;

        if (pool.Count < 1) return null;

        GameObject piece = pool[0];
        piece.gameObject.SetActive(isActive);
        piece.transform.SetParent(newParent);
        piece.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        pool.Remove(piece);
        return piece;
    }

    /// <summary>
    /// Dectivates a piece and attach it to the pool - will be it's child in hierarchy.
    /// </summary>
    /// <param name="piece">The object returning to the pool.</param>
    /// <param name="pieceType">The type of the piece. Will be either TileType.X or TileType.O. It will never be TileType.Empty.</param>
    public void ReturnPiece(GameObject piece, TileType pieceType)
    {
        Debug.Log($"Returning piece: {piece.name}");
        List<GameObject> pool = pieceType == TileType.X ? _xPool : _oPool;
        piece.gameObject.SetActive(false);
        piece.transform.SetParent(transform);
        piece.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        pool.Add(piece);
    }
    #endregion*/
}
