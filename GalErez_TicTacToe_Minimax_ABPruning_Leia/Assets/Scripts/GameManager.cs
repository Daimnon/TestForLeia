using UnityEngine;
using System.Collections.Generic;

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

    private void Start()
    {
        InitGame();
    }

    /*public void Update() // shouldn't be in update
    {
        if (_currentPlayer == TileType.X)
        {
            // Player's turn (human)
            if (PlayerHasMadeMove()) // You would need to define how the player makes a move
            {
                // Call the method to make the player's move
                MakeMove(playerMove);
            }
        }
        else if (_currentPlayer == TileType.O)
        {
            // AI's turn
            if (!GameOver()) // Only run if the game is not over
            {
                Vector2Int aiMove = xOBot.GetBestMove(this); // AI chooses the best move
                MakeMove(aiMove); // Make the AI's move
            }
        }
    }*/

    public void InitGame() // clear board and reroll player
    {
        for (int i = 0; i < _board.GetLength(0); i++)
        {
            for (int j = 0; j < _board.GetLength(1); j++)
            {
                _board[i, j] = TileType.Empty;
            }
        }

        _currentPlayer = Random.value > 0.5f ? TileType.X : TileType.O;
    }

    public bool GameOver()
    {
        return CheckWin(TileType.X) || CheckWin(TileType.O) || GetAvailableMoves().Count == 0;
    }

    public int GetScore()
    {
        if (CheckWin(TileType.X)) return 1;
        if (CheckWin(TileType.O)) return -1;
        return 0;
    }

    private bool CheckWin(TileType player)
    {
        // Check rows, columns, and diagonals for a win
        for (int i = 0; i < 3; i++)
        {
            if (_board[i, 0] == player && _board[i, 1] == player && _board[i, 2] == player)
                return true;
            if (_board[0, i] == player && _board[1, i] == player && _board[2, i] == player)
                return true;
        }

        if (_board[0, 0] == player && _board[1, 1] == player && _board[2, 2] == player) return true;
        if (_board[0, 2] == player && _board[1, 1] == player && _board[2, 0] == player) return true;

        return false;
    }

    public List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (_board[i, j] == TileType.Empty)
                {
                    moves.Add(new Vector2Int(i, j));
                }
            }
        }

        return moves;
    }

    public void MakeMove(Vector2Int move)
    {
        _board[move.x, move.y] = _currentPlayer;
        _currentPlayer = _currentPlayer == TileType.X ? TileType.O : TileType.X;
    }

    public void UndoMove(Vector2Int move)
    {
        _board[move.x, move.y] = TileType.Empty;
        _currentPlayer = _currentPlayer == TileType.X ? TileType.O : TileType.X;
    }
}
