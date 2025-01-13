using UnityEngine;

public class XOBot : MonoBehaviour
{
    [SerializeField] private int _maxDepthSearch = 9;

    public int Minimax(GameManager gameState, int depth, bool isMaximizingPlayer)
    {
        if (gameState.IsGameOver()) return gameState.GetStateScore();
        if (depth >= _maxDepthSearch) return gameState.GetStateScore();

        int bestScore = isMaximizingPlayer ? int.MinValue : int.MaxValue;

        foreach (Vector2Int move in gameState.GetAvailableMoves())
        {
            gameState.MakeMove(move);
            int score = Minimax(gameState, depth + 1, !isMaximizingPlayer);
            gameState.UndoMove(move);

            bestScore = isMaximizingPlayer ? Mathf.Max(bestScore, score) : Mathf.Min(bestScore, score);
        }

        return bestScore;
    }

    public Vector2Int GetBestMove(GameManager gameState)
    {
        int bestScore = int.MinValue;
        Vector2Int bestMove = Vector2Int.zero;

        foreach (var move in gameState.GetAvailableMoves())
        {
            gameState.MakeMove(move);
            int score = Minimax(gameState, 0, false);
            gameState.UndoMove(move);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }
}
