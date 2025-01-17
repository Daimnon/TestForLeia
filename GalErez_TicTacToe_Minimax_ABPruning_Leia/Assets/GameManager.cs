using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Difficulity
{ 
    VeryEasy, 
    Easy, 
    Medium, 
    Hard
}

public class GameManager : MonoBehaviour
{
    private int[] _tileIDs; // generated on start

    [Header("Configuration")]
    [SerializeField] private int _turn = 1; // turn = 1 implies player 1 turn, same for player 2 aka the bot
    [SerializeField] private float _botMoveDelay = 0.4f;

    [SerializeField] private Difficulity _botLevel = Difficulity.Hard;
    [SerializeField] private int[] _botForeSightDepth = new int[4] { 1, 2, 4, 9 };

    [Header("Tiles")]
    [SerializeField] private TextMeshProUGUI[] _tileTexts;
    [SerializeField] private string[] _shapes; // first shape should be empty

    [Header("Game State UI")]
    [SerializeField] private TextMeshProUGUI _turnTextTMP;
    [SerializeField] private GameObject _againBtn;

    private void Start()
    {
        ResetGame();
    }

    private void ResetGame()
    {
        _tileIDs = new int[9];
        _turn = 1;

        UpdateUI();
        _againBtn.SetActive(false);
        StartCoroutine(BotMove());
    }
    private void UpdateUI()
    {
        for (int i = 0; i < _tileIDs.Length; i++)
        {
            _tileTexts[i].text = _shapes[_tileIDs[i]];
        }

        _turnTextTMP.text = _turn == 1 ? "Player's Turn" : "Bot Turn";
    }

    private IEnumerator BotMove()
    {
        yield return new WaitForSeconds(_botMoveDelay);

        if (_turn == 2)
        {
            int depth = _botForeSightDepth[(int)_botLevel];
            Minimax(_tileIDs, depth, true, depth);
        }
    }

    private int Minimax(int[] currentBoard, int depth, bool isMaxSide, int initialDepth)
    {
        int gameOver = CheckVictory(currentBoard);
        if (depth == 0 || gameOver != -1)
        {
            if (gameOver == _turn) return 1;
            else if (gameOver != _turn && gameOver > 0) return -1;
            else return 0;
        }

        if (isMaxSide)
        {
            int maxEvaluation = -10000;
            List<int> possibleMoves = GetPossibleMoves(currentBoard);

            List<int> bestMove = new List<int>();
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                int[] tempBoard = new int[9];
                for (int j = 0; j < 9; j++) // j == tileID
                {
                    tempBoard[j] = currentBoard[j];
                }
                tempBoard[possibleMoves[i]] = _turn;

                int evaluation = Minimax(tempBoard, depth - 1, false, initialDepth);
                if (initialDepth == depth)
                {
                    if (evaluation > maxEvaluation)
                    {
                        bestMove.Clear();
                        bestMove.Add(i);
                    }
                    else if (evaluation == maxEvaluation)
                    {
                        bestMove.Add(i);
                    }
                }
                maxEvaluation = Mathf.Max(maxEvaluation, evaluation);
            }

            if (initialDepth == depth)
            {
                int moveChosen = bestMove[Random.Range(0, bestMove.Count)];
                MakeMove(possibleMoves[moveChosen]);
            }

            return maxEvaluation;
        }
        else
        {
            int minEvaluation = 10000;
            List<int> possibleMoves = GetPossibleMoves(currentBoard);

            for (int i = 0; i < possibleMoves.Count; i++)
            {
                int[] tempBoard = new int[9];
                for (int j = 0; j < 9; j++) // j == tileID
                {
                    tempBoard[j] = currentBoard[j];
                }
                tempBoard[possibleMoves[i]] = _turn == 1 ? 2 : 1;

                int evaluation = Minimax(tempBoard, depth - 1, true, initialDepth);
                minEvaluation = Mathf.Min(minEvaluation, evaluation);
            }

            return minEvaluation;
        }
    }

    private void MakeMove(int spaceToMove)
    {
        _tileIDs[spaceToMove] = _turn;
        _turn = _turn == 1 ? 2 : 1;
        UpdateUI();

        CheckEndGame();

        StartCoroutine(BotMove());
    }

    private List<int> GetPossibleMoves(int[] spacesToCheck)
    {
        List<int> possibleMoves = new List<int>();
        for (int i = 0; i < spacesToCheck.Length; i++)
        {
            if (spacesToCheck[i] == 0)
                possibleMoves.Add(i);
        }

        return possibleMoves;
    }

    private void CheckEndGame()
    {
        int winner = CheckVictory(_tileIDs);

        if (winner == 0) Tie();
        else if (winner == 1) PlayerVictory();
        else if (winner == 2) BotVictory();
        if (winner != -1) EndGame();
    }

    #region Victory methods
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
    #endregion

    private void EndGame()
    {
        _turn = -1;
        _againBtn.SetActive(true);
    }

    private int CheckVictory(int[] spacesToCheck)
    {
        List<int> spaceNum = new List<int>();

        // Rows
        int rowStart = 0;
        for (int row = 0; row < 3; row++)
        {
            if (spacesToCheck[rowStart] == spacesToCheck[rowStart + 1] && spacesToCheck[rowStart + 1] == spacesToCheck[rowStart + 2])
            {
                if (spacesToCheck[rowStart] != 0)
                    spaceNum.Add(spacesToCheck[rowStart]);
            }

            rowStart += 3;
        }

        // Columns
        int columnStart = 0;
        for (int column = 0; column < 3; column++)
        {
            if (spacesToCheck[columnStart] == spacesToCheck[columnStart + 3] && spacesToCheck[columnStart + 3] == spacesToCheck[columnStart + 6])
            {
                if (spacesToCheck[columnStart] != 0)
                    spaceNum.Add(spacesToCheck[columnStart]);
            }

            columnStart++;
        }

        // Diagonal Up
        if (spacesToCheck[0] == spacesToCheck[4] && spacesToCheck[4] == spacesToCheck[8])
        {
            if (spacesToCheck[0] != 0)
                spaceNum.Add(spacesToCheck[0]);
        }

        // Diagonal Down
        if (spacesToCheck[6] == spacesToCheck[4] && spacesToCheck[4] == spacesToCheck[2])
        {
            if (spacesToCheck[6] != 0)
                spaceNum.Add(spacesToCheck[6]);
        }

        if (spaceNum.Count > 0)
        {
            for (int i = 0; i < spaceNum.Count; i++)
            {
                if (spaceNum[i] == 1)
                {
                    return 1;
                }
                else if (spaceNum[i] == 2)
                {
                    return 2;
                }
            }
        }
        else
        {
            int freeSpaces = 0;
            for (int i = 0; i < _tileIDs.Length; i++)
            {
                if (spacesToCheck[i] == 0)
                    freeSpaces++;
            }

            if (freeSpaces == 0)
                return 0;
            else
                return -1;
        }

        return -1;
    }

    public void ChooseTile(int tileID)
    {
        if (_turn != -1 && _turn == 1) // making sure it's the player's turn
            if (_tileIDs[tileID] == 0)
                MakeMove(tileID);
    }
    public void PlayAgain()
    {
        ResetGame();
    }
}
