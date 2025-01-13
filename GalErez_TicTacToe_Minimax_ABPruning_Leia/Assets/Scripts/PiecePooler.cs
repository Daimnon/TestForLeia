using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PiecePooler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _xPrefab;
    [SerializeField] private GameObject _oPrefab;

    [Header("Configuration for each pool")]
    [SerializeField] private int _defaultCapacity = 5;
    [SerializeField] private int _maxSize = 5;

    private ObjectPool<GameObject> _xPool;
    private ObjectPool<GameObject> _oPool;

    private void Awake() // init pools
    {
        _xPool = InitPool(TileType.X);
        _oPool = InitPool(TileType.O);
    }

    private ObjectPool<GameObject> InitPool(TileType pieceType)
    {
        Debugger.Log("Start Initializing pool");
        ObjectPool<GameObject> pool = new (
            createFunc: () => CreatePiece(pieceType),
            actionOnGet: piece => OnGetPiece(piece, true, null),
            actionOnRelease: piece => OnReturnPiece(piece),
            actionOnDestroy: Destroy,
            collectionCheck: false,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize
        );

        Debugger.Log("created pool");
        for (int i = 0; i < _defaultCapacity; i++)
        {
            Debugger.Log("trying to create piece");

            GameObject piece = CreatePiece(pieceType); // Create a new instance
            Debugger.Log("piece created");
            pool.Release(piece);
            Debugger.Log("piece added to pool");
        }

        return pool;
    }
    private GameObject CreatePiece(TileType type)
    {
        GameObject boardPiece = type == TileType.X ? Instantiate(_xPrefab) : Instantiate(_oPrefab);
        // Set the parent to PiecePooler and ensure the position is zeroed
        boardPiece.transform.SetParent(transform);
        boardPiece.transform.localPosition = Vector3.zero;
        boardPiece.SetActive(false); // Deactivate the piece by default
        Debugger.Log("piece created -1");
        return boardPiece;
    }

    #region Public method - please don't overlook the summary :)
    /// <summary>
    /// Activates a piece and detach it from the pool - no longer it's child in hierarchy.
    /// </summary>
    /// <param name="type">The type of the piece. Will be either TileType.X (player) or TileType.O (bot). It will never be TileType.Empty.</param>
    /// <returns> A Piece as GameObject</returns>
    public GameObject GetPiece(TileType type)
    {
        GameObject piece = type == TileType.X ? _xPool.Get() : _oPool.Get();
        return piece;
    }

    /// <summary>
    /// Dectivates a piece and attach it to the pool - will be it's child in hierarchy.
    /// </summary>
    /// <param name="piece">The object returning to the pool.</param>
    /// <param name="type">The type of the piece. Will be either TileType.X or TileType.O. It will never be TileType.Empty.</param>
    public void ReturnPiece(GameObject piece, TileType type)
    {
        if (type == TileType.X)
            _xPool.Release(piece);
        else if (type == TileType.O)
            _oPool.Release(piece);
    }
    #endregion

    #region Pool Events - being invoked when we get or return the objects (see InitPool for usage)
    private void OnGetPiece(GameObject piece, bool shouldActivate, Transform newParent)
    {
        piece.transform.SetParent(newParent);
        piece.transform.localPosition = Vector3.zero;
        piece.SetActive(shouldActivate);
    }
    private void OnReturnPiece(GameObject piece)
    {
        piece.SetActive(false); // Deactivate the piece
        piece.transform.SetParent(this.transform); // Set the parent to the PiecePooler object
        piece.transform.localPosition = Vector3.zero; // Reset position
    }
    #endregion
}
