using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.EnhancedTouch;

public enum TileType
{
    Empty,
    X,
    O
}

public class GameManager : MonoBehaviour
{
    private ETouch.Finger _moveFinger;
    private Vector3 _fingerMoveAmount;

    private const int BOARD_AXIS_LENGTH = 3;
    /*private const string X_PIECE_TAG = "xPiece";
    private const string O_PIECE_TAG = "oPiece";*/
    private const string EMPTY_TILE_TAG = "emptyTile";

    private TileType[,] _board = new TileType[3, 3];
    private TileType _currentPlayer;
    private TileType _playerSymbol;
    private bool _isInputBlocked = false;

    private bool _skipVisualUpdates = false;
    public bool SkipVisualUpdates { get => _skipVisualUpdates; set => _skipVisualUpdates = value; }

    [Header("Setup Components")]
    [SerializeField] private PiecePooler _piecePooler;
    [SerializeField] private Transform[] _boardPositionsLeft;
    [SerializeField] private Transform[] _boardPositionsMid;
    [SerializeField] private Transform[] _boardPositionsRight;
    [SerializeField] private Transform[,] _boardPositions;
    [SerializeField] private XOBot _bot;
    [SerializeField] private float _botMoveDelay = 1.0f;

    [Header("Debugging")]
    [SerializeField] private bool _isDebugEnabled = true;

    private void OnEnable()
    {
        ETouch.EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += OnFingerDown;
    }
    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= OnFingerDown;
        ETouch.EnhancedTouchSupport.Disable();
    }
    private void Start()
    {
        Debugger.IsDebugEnabled = _isDebugEnabled; 
        InitGame();
    }

    private void InitGame() // clear board and reroll player symbol 
    {
        for (int i = 0; i < BOARD_AXIS_LENGTH; i++)
        {
            for (int j = 0; j < BOARD_AXIS_LENGTH; j++)
            {
                _board[i, j] = TileType.Empty;
            }
        }

        _boardPositions = new Transform[BOARD_AXIS_LENGTH, BOARD_AXIS_LENGTH];
        for (int i = 0; i < BOARD_AXIS_LENGTH; i++)
        {
            _boardPositions[0, i] = _boardPositionsLeft[i];
            _boardPositions[1, i] = _boardPositionsMid[i];
            _boardPositions[2, i] = _boardPositionsRight[i];
        }

        SetupNewHuman();
    }
    private void SetupNewHuman()
    {
        _currentPlayer = Random.value > 0.5f ? TileType.X : TileType.O;
        _playerSymbol = _currentPlayer;
    }

    private IEnumerator BotTurnRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        MakeBotMove();
    }
    private void UpdateBoardVisuals()
    {
        for (int i = 0; i < BOARD_AXIS_LENGTH; i++)
        {
            for (int j = 0; j < BOARD_AXIS_LENGTH; j++)
            {
                if (_board[i, j] != TileType.Empty)
                {
                    TileType type = _board[i, j];
                    Transform parentTile = _boardPositions[i, j]; // formula to get the right tile in the array from with a 3x3 board

                    if (parentTile.childCount < 1)
                    {
                        GameObject piece = _piecePooler.GetPiece(type, parentTile, true);
                        Debug.Log($"Drawing piece of type: {type}");
                    }
                }
            }
        }
    }

    /*private bool HandlePlayerInput() // need to change to Enhanced Touch
    {
        if (_isInputBlocked) return false;

        if (Input.GetMouseButtonDown(0))
        {
            Debugger.Log("Input recieved");

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float maxRayDistance = 100f;
            int layerMask = LayerMask.GetMask("Tiles");
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, maxRayDistance, layerMask);

            if (hit.collider != null && hit.collider.CompareTag(EMPTY_TILE_TAG))
            {
                Vector2Int movePos = new((int)hit.transform.position.x, (int)hit.transform.position.y);
                MakeMove(movePos);
                _isInputBlocked = true;
                return true;
            }
        }
        return false;
    }*/
    private void MakeBotMove()
    {
        _isInputBlocked = false;
        Vector2Int aiMove = _bot.GetBestMove(this);
        MakeMove(aiMove);
        TileType oppositeOfCurrent = _currentPlayer == TileType.X ? TileType.O : TileType.X;
        Debug.Log($"Bot placed a piece of type: {oppositeOfCurrent} at: {aiMove}");

        GetStateScore();
    }

    private bool CheckWin(TileType player)
    {
        for (int i = 0; i < _board.GetLength(0); i++)
        {
            if (_board[i, 0] == player && _board[i, 1] == player && _board[i, 2] == player) return true; // row
            if (_board[0, i] == player && _board[1, i] == player && _board[2, i] == player) return true; // column
        }

        // diagonal
        if (_board[0, 0] == player && _board[1, 1] == player && _board[2, 2] == player) return true;
        if (_board[0, 2] == player && _board[1, 1] == player && _board[2, 0] == player) return true;

        return false;
    }


    #region Modified code form ChatGPT, comments are mine.
    private bool IsBoardFull()
    {
        foreach (TileType tile in _board)
        {
            if (tile == TileType.Empty) // If any spot is empty, the board isn't full
                return false;
        }
        return true; // If no empty spots are found, the board is full
    }
    private int EvaluatePerPiece()
    {
        int xScore = 0;
        int oScore = 0;

        // Check rows
        for (int row = 0; row < 3; row++)
        {
            xScore += EvaluationMethod(new Vector2Int(row, 0), new Vector2Int(row, 1), new Vector2Int(row, 2), TileType.X);
            oScore += EvaluationMethod(new Vector2Int(row, 0), new Vector2Int(row, 1), new Vector2Int(row, 2), TileType.O);
        }

        // Check columns
        for (int col = 0; col < 3; col++)
        {
            xScore += EvaluationMethod(new Vector2Int(0, col), new Vector2Int(1, col), new Vector2Int(2, col), TileType.X);
            oScore += EvaluationMethod(new Vector2Int(0, col), new Vector2Int(1, col), new Vector2Int(2, col), TileType.O);
        }

        // Check diagonals
        xScore += EvaluationMethod(new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2), TileType.X);
        oScore += EvaluationMethod(new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2), TileType.O);

        xScore += EvaluationMethod(new Vector2Int(0, 2), new Vector2Int(1, 1), new Vector2Int(2, 0), TileType.X);
        oScore += EvaluationMethod(new Vector2Int(0, 2), new Vector2Int(1, 1), new Vector2Int(2, 0), TileType.O);

        // Draw detection: if board is full and no one is winning, it's a tie
        if (IsBoardFull() && xScore == 0 && oScore == 0)
        {
            return 0; // Tie
        }

        // Return the difference in scores, indicating the leading player
        if (xScore > oScore) return 1; // X is leading
        if (oScore > xScore) return -1; // O is leading
        return 0; // No one is leading
    }
    private int EvaluationMethod(Vector2Int a, Vector2Int b, Vector2Int c, TileType piece)
    {
        TileType[] line = new TileType[BOARD_AXIS_LENGTH]; // Create an array for the line (row, column, or diagonal)
        line[0] = _board[a.x, a.y];
        line[1] = _board[b.x, b.y];
        line[2] = _board[c.x, c.y];

        // Track counts for X, O, and empty
        int xCount = 0;
        int oCount = 0;
        int emptyCount = 0;

        foreach (TileType tileType in line)
        {
            if (tileType == TileType.X) xCount++;
            else if (tileType == TileType.O) oCount++;
            else emptyCount++;
        }

        // If the line contains X, O, and an empty tile, return 0 (neutral scenario)
        if (xCount == 1 && oCount == 1 && emptyCount == 1)
        {
            return 0;
        }

        // Score based on favorable positions
        if (xCount == 3) return 1;  // X wins
        if (oCount == 3) return -1; // O wins
        if (xCount == 2 && emptyCount == 1) return 1;  // X leaning toward victory
        if (oCount == 2 && emptyCount == 1) return -1; // O leaning toward victory

        // Default case: no clear advantage
        return 0;
    }
    #endregion


    public List<Vector2Int> GetAvailableMoves()// using Vector2Int cause floats are not relevant 
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        // no open scope cause doing only one action and won't be expanded. I think it's more readable that way cause you instantly know there is only one action at the end.
        for (int i = 0; i < BOARD_AXIS_LENGTH; i++)
            for (int j = 0; j < BOARD_AXIS_LENGTH; j++)
                if (_board[i, j] == TileType.Empty)
                    moves.Add(new Vector2Int(i, j));

        return moves;
    }

    public void MakeMove(Vector2Int move)
    {
        _board[move.x, move.y] = _currentPlayer;

        if (!_skipVisualUpdates)
        {
            _currentPlayer = _currentPlayer == TileType.X ? TileType.O : TileType.X;
            UpdateBoardVisuals();
        }
    }
    public void UndoMove(Vector2Int move)
    {
        _board[move.x, move.y] = TileType.Empty;
    }

    /// <returns>True if condition for CheckWin met or no more moves availabe</returns>
    public bool IsGameOver()
    {
        if (CheckWin(TileType.X) || CheckWin(TileType.O)) return true;
        if (GetAvailableMoves().Count == 0) return true;
        return false;
    }

    /// <returns>1 if 'X' is leading || -1 if 'O' is leading || 0 if no leader</returns>
    public int GetStateScore()
    {
        // If there's a winner, return the corresponding score based on who is leading
        int boardScore = EvaluatePerPiece();

        if (boardScore == 1) return 1;  // X is leading
        if (boardScore == -1) return -1; // O is leading

        // If the game is a tie (no one is leading), return 0
        return 0;
    }

    private void OnFingerDown(Finger finger)
    {
        if (_isInputBlocked && _currentPlayer != _playerSymbol) return;
        Debugger.Log("Input recieved");

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float maxRayDistance = 100f;
        int layerMask = LayerMask.GetMask("Tiles");
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, maxRayDistance, layerMask);

        if (hit.collider != null && hit.collider.CompareTag(EMPTY_TILE_TAG))
        {
            Vector2Int movePos = new((int)hit.transform.position.x +1, (int)hit.transform.position.y +1); // So I can choose the right move based on the transform position
            MakeMove(movePos);
            _isInputBlocked = true;
            _skipVisualUpdates = true;
            StartCoroutine(BotTurnRoutine(_botMoveDelay));
        }
    }
}