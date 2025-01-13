using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public enum TileType
{
    Empty,
    X,
    O
}

public class GameManager : MonoBehaviour
{
    private TileType[,] _board = new TileType[3, 3];
    private TileType _currentPlayer;

    [Header("Setup Components")]
    [SerializeField] private PiecePooler piecePooler;
    [SerializeField] private Transform[] boardPositions;
    [SerializeField] private XOBot _bot;

    private bool _isGameActive = true;
    public bool IsGameActive => _isGameActive;

    private const int BOARD_AXIS_LENGTH = 3;

    [Header("Debugging")]
    [SerializeField] private bool _isDebugEnabled = true;

    private void Start()
    {
        Debugger.IsDebugEnabled = _isDebugEnabled; 
        InitGame();
    }
    private void Update() // wanted to use this instead: "IEnumerator CheckTurnRoutine()" but looks like the coroutine overhead will be a greater cost than using the update method 
    {
        if (_isGameActive)
        {
            if (_currentPlayer == TileType.X)
            {
                HandlePlayerInput();
            }
            else if (_currentPlayer == TileType.O)
            {
                MakeBotMove();
            }
        }

        // if it was a coroutine it would look like so:
        /*private IEnumerator CheckTurnRoutine()
        {
            if (!_isGameActive) yield break;

            if (_currentPlayer == TileType.X && _isGameActive)
            {
                HandlePlayerInput();
            }
            else if (_currentPlayer == TileType.O && _isGameActive)
            {
                MakeAIMove();
            }
            yield return null;
        }*/
        // and would be called after each turn so basically it will work most of the time, so starting and stopping the Coroutine will be expansive IMO.
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

        _currentPlayer = Random.value > 0.5f ? TileType.X : TileType.O;
        _isGameActive = true;
        UpdateBoardVisuals();
    }
    private void ChooseNextPlayer()
    {
        _currentPlayer = _currentPlayer == TileType.X ? TileType.O : TileType.X;
    }

    private void UpdateBoardVisuals()
    {
        for (int i = 0; i < BOARD_AXIS_LENGTH; i++)
        {
            for (int j = 0; j < _board.GetLength(1); j++)
            {
                if (_board[i, j] != TileType.Empty)
                {
                    TileType type = _board[i, j];
                    GameObject piece = piecePooler.GetPiece(type);
                    piece.transform.position = boardPositions[i * BOARD_AXIS_LENGTH + j].position;
                }
            }
        }
    }

    private void HandlePlayerInput() // need to change to Enhanced Touch
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                for (int i = 0; i < boardPositions.Length; i++)
                {
                    if (hit.transform == boardPositions[i])
                    {
                        Vector2Int move = new Vector2Int(i / BOARD_AXIS_LENGTH, i % BOARD_AXIS_LENGTH);
                        if (_board[move.x, move.y] == TileType.Empty)
                        {
                            MakeMove(move);
                            _isGameActive = IsGameOver();
                            return;
                        }
                    }
                }
            }
        }
    }
    private void MakeBotMove()
    {
        Vector2Int aiMove = _bot.GetBestMove(this);
        MakeMove(aiMove);
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

        // Return the difference in scores, indicating the leading player
        if (xScore > oScore) return 1; // X is leading
        if (oScore > xScore) return -1; // O is leading
        return 0; // No one is leading
    }
    private int EvaluationMethod(Vector2Int a, Vector2Int b, Vector2Int c, TileType piece) // just really didn't want to spend the time to make this evaluation.
    {
        TileType[] line = new TileType[BOARD_AXIS_LENGTH]; // create an array for the line (row, column, or diagonal)
        line[0] = _board[a.x, a.y];
        line[1] = _board[b.x, b.y];
        line[2] = _board[c.x, c.y];

        // track how many slots are occupied by the player and how many are empty for all partys
        int xCount = 0;
        int emptyCount = 0;
        int oCount = 0;

        foreach (TileType tileType in line)
        {
            if (tileType == piece) xCount++;
            else if (tileType == TileType.Empty) emptyCount++;
            else oCount++;
        }

        // evaluate actual score
        if (xCount == 3) return 1;  // x wins
        if (oCount == 3) return -1; // o wins
        if (xCount == 2 && emptyCount == 1) return 1; // x has 2 pieces and 1 empty space, they are leaning toward victory
        if (oCount == 2 && emptyCount == 1) return -1; // o has 2 pieces and 1 empty space, they are leaning toward victory
        return 0; // no leader
    }
    #endregion


    public List<Vector2Int> GetAvailableMoves()// using Vector2Int cause floats are not relevant 
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        // no open scope cause doing only one action and won't be expanded. I think it's more readable that way cause you instantly know there is only one action at the end.
        for (int i = 0; i < _board.GetLength(0); i++)
            for (int j = 0; j < _board.GetLength(1); j++)
                if (_board[i, j] == TileType.Empty)
                    moves.Add(new Vector2Int(i, j));

        return moves;
    }
    public void MakeMove(Vector2Int move)
    {
        _board[move.x, move.y] = _currentPlayer;
        ChooseNextPlayer();
        UpdateBoardVisuals();
    }
    public void UndoMove(Vector2Int move)
    {
        _board[move.x, move.y] = TileType.Empty;
        ChooseNextPlayer();
        UpdateBoardVisuals();
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
}