using UnityEngine;

public class XOBot : MonoBehaviour
{
    [SerializeField] private int _maxDepthSearch = 9;

    public int Minimax(GameManager gameState, int depth, bool isMaximizingPlayer)
    {
        if (gameState.IsGameOver() || depth >= _maxDepthSearch)
        {
            return gameState.GetStateScore();
        }

        int bestScore = isMaximizingPlayer ? int.MinValue : int.MaxValue;

        foreach (Vector2Int move in gameState.GetAvailableMoves())
        {
            gameState.MakeMove(move);
            int score = Minimax(gameState, depth + 1, !isMaximizingPlayer);
            gameState.UndoMove(move);

            if (isMaximizingPlayer)
            {
                bestScore = Mathf.Max(bestScore, score);
            }
            else
            {
                bestScore = Mathf.Min(bestScore, score);
            }
        }

        return bestScore;
    }

    public Vector2Int GetBestMove(GameManager gameState)
    {
        int bestScore = int.MinValue;
        Vector2Int bestMove = Vector2Int.zero;

        foreach (Vector2Int move in gameState.GetAvailableMoves())
        {
            Debugger.Log($"Evaluating move: {move}");
            gameState.MakeMove(move);
            int score = Minimax(gameState, 1, false);  // Make sure depth starts from 1
            gameState.UndoMove(move);

            Debugger.Log($"Move {move} resulted in score: {score}");

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        gameState.SkipVisualUpdates = false;
        return bestMove;
    }
}
