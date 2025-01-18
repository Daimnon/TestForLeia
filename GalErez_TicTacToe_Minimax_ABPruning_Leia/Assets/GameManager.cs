using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Difficulity
{ 
    Easy, 
    Medium, 
    Hard, 
    Unbeatable // cannot be played with alpha pruning
}

public class GameManager : MonoBehaviour
{
    private const int MIN_VALUE = -10000; // just a randomly extreme low value
    private const int MAX_VALUE = 10000; // just a randomly extreme high value

    private int[] _board; // generated on start
    private int _turn = 1; // turn = 1 implies player 1 turn, same for player 2 aka the bot
    private string[] _shapes = new string[] { string.Empty, string.Empty , string.Empty }; // first shape should be always empty, 'x' and 'o' will be randomly assign afterwards.

    [Header("Configuration")]
    [SerializeField] private float _botMoveDelay = 0.4f;
    [SerializeField] private Difficulity _botLevel = Difficulity.Unbeatable;
    [SerializeField] private int[] _botForeSightDepth = new int[4] { 1, 2, 4, 9 };
    [SerializeField] private bool _useAlphaBetaPruning = false;

    [Header("Tiles")]
    [SerializeField] private TextMeshProUGUI[] _tileTexts;

    [Header("Game State UI")]
    [SerializeField] private GameObject _overlay;
    [SerializeField] private TextMeshProUGUI _turnTextTMP;
    [SerializeField] private TextMeshProUGUI _difficultyTextTMP;
    [SerializeField] private TextMeshProUGUI _useABPurningTMP;
    [SerializeField] private GameObject _againBtn;

    private void RandomizeShapes()
    {
        List<string> possibleShapes = new List<string> { "X", "O" };

        _shapes[1] = possibleShapes[Random.Range(0, possibleShapes.Count)];
        _shapes[2] = _shapes[1] == "X" ? "O" : "X";
    }
    private void ResetGame()
    {
        if ((int)_botLevel > 2 && _useAlphaBetaPruning) _botLevel = Difficulity.Hard;
        _difficultyTextTMP.text = _botLevel.ToString();

        RandomizeShapes();

        _board = new int[9];
        _turn = 1;

        UpdateUI();
        _againBtn.SetActive(false);
        StartCoroutine(BotMove());
    }
    private void UpdateUI()
    {
        for (int i = 0; i < _board.Length; i++)
        {
            _tileTexts[i].text = _shapes[_board[i]];
        }

        _turnTextTMP.text = _turn == 1 ? "Player's Turn" : "Bot Turn";
    }

    private void MakeMove(int tileID)
    {
        _board[tileID] = _turn;
        _turn = _turn == 1 ? 2 : 1;

        UpdateUI();
        CheckEndGame();
        StartCoroutine(BotMove());
    }
    private IEnumerator BotMove()
    {
        yield return new WaitForSeconds(_botMoveDelay);

        if (_turn == 2)
        {
            int depth = _botForeSightDepth[(int)_botLevel];
            Minimax(_board, depth, true, depth, MIN_VALUE, MAX_VALUE);
        }
    }

    #region Minimax
    private int EvaluateState(int[] board, int depth)
    {
        int gameOver = CheckVictory(board);
        if (depth == 0 || gameOver != -1)
        {
            if (gameOver == _turn) return 1;
            else if (gameOver != _turn && gameOver > 0) return -1;
            else return 0;
        }
        return int.MinValue;
    }
    private List<int> GetPossibleMoves(int[] spacesToCheck)
    {
        List<int> possibleMoves = new();
        for (int i = 0; i < spacesToCheck.Length; i++)
        {
            if (spacesToCheck[i] == 0) possibleMoves.Add(i);
        }

        return possibleMoves;
    }
    private int[] CreateTempBoard(int[] board, int move, int turn)
    {
        int[] newBoard = (int[])board.Clone();
        newBoard[move] = turn;
        return newBoard;
    }
    private int MaxPlayer(int[] currentBoard, int depth, int initialDepth, int alpha, int beta)
    {
        int maxEvaluation = MIN_VALUE;
        List<int> possibleMoves = GetPossibleMoves(currentBoard); // get available moves
        List<int> bestMoves = new();

        foreach (int move in possibleMoves)
        {
            // creating a temporary board in order to evaluate minimax
            int[] tempBoard = CreateTempBoard(currentBoard, move, _turn);
            int evaluation = Minimax(tempBoard, depth - 1, false, initialDepth, alpha, beta);

            // storing best moves
            if (initialDepth == depth)
            {
                if (evaluation > maxEvaluation)
                {
                    bestMoves.Clear();
                    bestMoves.Add(move);
                }
                else if (evaluation == maxEvaluation)
                {
                    bestMoves.Add(move);
                }
            }
            maxEvaluation = Mathf.Max(maxEvaluation, evaluation);

            if (_useAlphaBetaPruning)
            {
                alpha = Mathf.Max(alpha, maxEvaluation);
                if (beta <= alpha) break; // beta cutoff - if the beta score (best of minimizer) is less or equal than the alpha, it means there is no need to try more options
            }
        }

        // If at the root level, choose a random best move
        if (initialDepth == depth && bestMoves.Count > 0)
        {
            int theBestMove = bestMoves[Random.Range(0, bestMoves.Count)];
            MakeMove(theBestMove);
        }

        return maxEvaluation;
    }
    private int MinPlayer(int[] currentBoard, int depth, int initialDepth, int alpha, int beta)
    {
        int minEvaluation = MAX_VALUE;
        List<int> possibleMoves = GetPossibleMoves(currentBoard); // get available moves

        foreach (int move in possibleMoves)
        {
            int[] tempBoard = CreateTempBoard(currentBoard, move, _turn == 1 ? 2 : 1);
            int evaluation = Minimax(tempBoard, depth - 1, true, initialDepth, alpha, beta);

            minEvaluation = Mathf.Min(minEvaluation, evaluation);

            if (_useAlphaBetaPruning)
            {
                beta = Mathf.Min(beta, minEvaluation);
                if (alpha >= beta) break; // alpha cutoff - if the alpha score (best of maximizer) is greater than or equal to the beta, it means there is no need to try more options
            }
        }

        return minEvaluation;
    }
    private int Minimax(int[] currentBoard, int depth, bool isMaxSide, int initialDepth, int alpha, int beta)
    {
        int stateEvaluation = EvaluateState(currentBoard, depth); // could be terminal state or depth state

        if (stateEvaluation != int.MinValue)
            return stateEvaluation;

        if (isMaxSide) return MaxPlayer(currentBoard, depth, initialDepth, alpha, beta);
        else return MinPlayer(currentBoard, depth, initialDepth, alpha, beta);
    }
    #endregion

    #region Victory methods
    private int CheckVictory(int[] board)
    {
        List<int> tilesID = new();

        // rows & columns calculations
        int rowStart = 0;
        for (int row = 0; row < 3; row++)
        {
            if (board[rowStart] == board[rowStart + 1] && board[rowStart + 1] == board[rowStart + 2])
            {
                if (board[rowStart] != 0)
                    tilesID.Add(board[rowStart]);
            }

            rowStart += 3; // next row
        }

        int columnStart = 0;
        for (int column = 0; column < 3; column++)
        {
            if (board[columnStart] == board[columnStart + 3] && board[columnStart + 3] == board[columnStart + 6])
            {
                if (board[columnStart] != 0)
                    tilesID.Add(board[columnStart]);
            }

            columnStart++; // next column
        }

        // diagonal calculations
        if (board[0] == board[4] && board[4] == board[8])
        {
            if (board[0] != 0)
                tilesID.Add(board[0]);
        }

        if (board[6] == board[4] && board[4] == board[2])
        {
            if (board[6] != 0)
                tilesID.Add(board[6]);
        }

        if (tilesID.Count > 0)
        {
            for (int i = 0; i < tilesID.Count; i++)
            {
                if (tilesID[i] == 1) return 1;
                else if (tilesID[i] == 2) return 2;
            }
        }
        else
        {
            int freeSpaces = 0;
            for (int i = 0; i < _board.Length; i++)
            {
                if (board[i] == 0)
                    freeSpaces++;
            }

            if (freeSpaces == 0) return 0;
            else return -1;
        }
        return -1;
    }
    private void PlayerVictory()
    {
        _turnTextTMP.text = "Player's Victory!";
    }
    private void BotVictory()
    {
        _turnTextTMP.text = "Bot's Victory!";
    }
    private void Tie()
    {
        _turnTextTMP.text = "It's a tie!";
    }
    private void CheckEndGame()
    {
        int winner = CheckVictory(_board);

        if (winner == 0) Tie();
        else if (winner == 1) PlayerVictory();
        else if (winner == 2) BotVictory();

        if (winner != -1) EndGame();
    }
    private void EndGame()
    {
        _turn = -1;
        _againBtn.SetActive(true);
    }
    #endregion

    public void StartGame()
    {
        _overlay.SetActive(false);
        ResetGame();
    }
    public void UsePruning()
    {
        _useAlphaBetaPruning = !_useAlphaBetaPruning;
        _useABPurningTMP.text = _useAlphaBetaPruning ? "Yes" : "No";
    }
    public void ChooseTile(int tileID)
    {
        if (_turn != -1 && _turn == 1) // making sure it's the player's turn
            if (_board[tileID] == 0)
                MakeMove(tileID);
    }
    public void PlayAgain()
    {
        UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.buildIndex);
    }
    public void ChangeDifficulty()
    {
        int nextDiff = (int)_botLevel + 1;
        if (nextDiff > 3) _botLevel = Difficulity.Easy;
        else _botLevel = (Difficulity)nextDiff;
        _difficultyTextTMP.text = _botLevel.ToString();
    }
}
