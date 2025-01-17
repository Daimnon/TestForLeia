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

public class TicTacToeManager : MonoBehaviour
{
    private int[] spaces;

    [Header("Configuration")]
    [SerializeField] private int _turn = 1; // turn = 1 implies player 1 turn, same for player 2 aka the bot
    [SerializeField] private float _botMoveDelay = 0.4f;

    [SerializeField] private Difficulity _botLevel = Difficulity.Hard;
    [SerializeField] private int[] _botForeSightDepth = new int[4] { 1, 2, 4, 9 };

    [Header("Tiles")]
    [SerializeField] private TextMeshProUGUI[] _tileTexts;
    [SerializeField] private string[] _shapes; // first shape should be empty

    [Header("Game State UI")]
    [SerializeField] TextMeshProUGUI gameStateText;
    [SerializeField] GameObject playAgainButton;

    void Start()
    {
        ResetGameState();
    }

    void ResetGameState()
    {
        spaces = new int[9];

        _turn = 1;

        UpdateUI();

        playAgainButton.SetActive(false);

        StartCoroutine(AIMove());
    }

    void UpdateUI()
    {
        for (int i = 0; i < spaces.Length; i++)
        {
            _tileTexts[i].text = _shapes[spaces[i]];
        }

        gameStateText.text = "Player " + _turn + "'s Turn";
    }

    public void SpaceClicked(int spaceClicked)
    {
        if (_turn != -1 && _turn == 1) // making sure it's the player's turn
            if (spaces[spaceClicked] == 0)
                MakeMove(spaceClicked);
    }

    IEnumerator AIMove()
    {
        yield return new WaitForSeconds(_botMoveDelay);

        if (_turn == 2)
        {
            int depth = _botForeSightDepth[(int)_botLevel];
            minimax(spaces, depth, true, depth);
        }
    }

    int minimax(int[] currentSpaces, int depth, bool maximizingPlayer, int initialDepth)
    {
        int gameOver = CheckWin(currentSpaces);
        if (depth == 0 || gameOver != -1)
        {
            if (gameOver == _turn)
            {
                return 1;
            }
            else if (gameOver != _turn && gameOver > 0)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        if (maximizingPlayer)
        {
            int maxEval = -10000;
            List<int> possibleMoves = GetPossibleMoves(currentSpaces);

            List<int> bestMove = new List<int>();
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                int[] newSpaces = new int[9];
                for (int space = 0; space < 9; space++)
                {
                    newSpaces[space] = currentSpaces[space];
                }
                newSpaces[possibleMoves[i]] = _turn;

                int eval = minimax(newSpaces, depth - 1, false, initialDepth);
                if (initialDepth == depth)
                {
                    if (eval > maxEval)
                    {
                        bestMove.Clear();
                        bestMove.Add(i);
                    }
                    else if (eval == maxEval)
                    {
                        bestMove.Add(i);
                    }
                }
                maxEval = Mathf.Max(maxEval, eval);
            }

            if (initialDepth == depth)
            {
                int moveChosen = bestMove[Random.Range(0, bestMove.Count)];
                MakeMove(possibleMoves[moveChosen]);
            }

            return maxEval;
        }
        else
        {
            int minEval = 10000;
            List<int> possibleMoves = GetPossibleMoves(currentSpaces);

            for (int i = 0; i < possibleMoves.Count; i++)
            {
                int[] newSpaces = new int[9];
                for (int space = 0; space < 9; space++)
                {
                    newSpaces[space] = currentSpaces[space];
                }
                newSpaces[possibleMoves[i]] = _turn == 1 ? 2 : 1;

                int eval = minimax(newSpaces, depth - 1, true, initialDepth);
                minEval = Mathf.Min(minEval, eval);
            }

            return minEval;
        }
    }

    void MakeMove(int spaceToMove)
    {
        spaces[spaceToMove] = _turn;
        _turn = _turn == 1 ? 2 : 1;
        UpdateUI();

        CheckEndGame();

        StartCoroutine(AIMove());
    }

    List<int> GetPossibleMoves(int[] spacesToCheck)
    {
        List<int> possibleMoves = new List<int>();
        for (int i = 0; i < spacesToCheck.Length; i++)
        {
            if (spacesToCheck[i] == 0)
                possibleMoves.Add(i);
        }

        return possibleMoves;
    }

    void CheckEndGame()
    {
        int win = CheckWin(spaces);

        if (win == 0)
            Tie();
        else if (win == 1)
            Player1Wins();
        else if (win == 2)
            Player2Wins();

        if (win != -1)
            EndGame();
    }

    void Player1Wins()
    {
        gameStateText.text = "Player 1 Wins!";
    }

    void Player2Wins()
    {
        gameStateText.text = "Player 2 Wins!";
    }

    void Tie()
    {
        gameStateText.text = "Tie!";
    }

    void EndGame()
    {
        _turn = -1;

        playAgainButton.SetActive(true);
    }

    int CheckWin(int[] spacesToCheck)
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
            for (int i = 0; i < spaces.Length; i++)
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

    public void PlayAgain()
    {
        ResetGameState();
    }
}
