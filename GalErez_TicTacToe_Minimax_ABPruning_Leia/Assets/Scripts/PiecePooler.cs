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

        _xPool = new ObjectPool<GameObject>(
            createFunc: () => CreatePiece(TileType.X),
            actionOnGet: xPiece => OnGetPiece(xPiece, true), // action on Get
            actionOnRelease: xPiece => OnReturnPiece(xPiece),
            actionOnDestroy: Destroy,
            collectionCheck: false,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize);

        _oPool = new ObjectPool<GameObject>(
            createFunc: () => CreatePiece(TileType.O),
            actionOnGet: oPiece => OnGetPiece(oPiece, true), // action on Get
            actionOnRelease: oPiece => OnReturnPiece(oPiece),
            actionOnDestroy: Destroy,
            collectionCheck: false,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize);
    }

    /*private void InitPool(ObjectPool<GameObject> pool, TileType pieceType)
    {
        Debugger.Log("created pool");
        for (int i = 0; i < _defaultCapacity; i++)
        {
            Debugger.Log("trying to create piece");

            GameObject piece = CreatePiece(pieceType); // Create a new instance
            Debugger.Log("piece created");
            pool.Release(piece);
            Debugger.Log("piece added to pool");
        }
    }*/
    private void SetupPiece(GameObject piece)
    {
        piece.transform.SetParent(transform);
        piece.transform.localPosition = Vector2.zero;
        piece.SetActive(false); 
    }
    private GameObject CreatePiece(TileType type)
    {
        GameObject piece = null;
        if (type == TileType.X)
        {
            if (_xPool.CountAll >= _defaultCapacity) return null;
            piece = Instantiate(_xPrefab);
            SetupPiece(piece);
            _xPool.Release(piece);
            Debugger.Log("xPiece created");
        }
        else if (type == TileType.O)
        {
            if (_oPool.CountAll >= _defaultCapacity) return null;
            piece = Instantiate(_oPrefab);
            SetupPiece(piece);
            _oPool.Release(piece);
            Debugger.Log("oPiece created");
        }
        return piece;
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
    private void OnGetPiece(GameObject piece, bool shouldActivate)
    {
        Debugger.Log($"Piece activated: {piece.name}, shouldActivate: {shouldActivate}");
        piece.transform.SetParent(null);
        piece.SetActive(shouldActivate);
    }
    private void OnReturnPiece(GameObject piece)
    {
        piece.SetActive(false); // Deactivate the piece
        piece.transform.SetParent(transform); // Set the parent to the PiecePooler object
        piece.transform.localPosition = Vector3.zero; // Reset position
    }
    #endregion
}
