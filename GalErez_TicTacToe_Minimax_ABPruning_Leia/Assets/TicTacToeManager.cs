using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TicTacToeManager : MonoBehaviour
{
    [Header("Game Info")]
    [SerializeField] int turn = 1;
    [SerializeField] bool player1AI = false;
    [SerializeField] bool player2AI = false;
    [SerializeField] float timeBetweenAIMove;

    public enum AILevel { VeryEasy, Easy, Medium, Impossible }

    [SerializeField] AILevel aiLevel;
    [SerializeField] int[] aiDepth;

    [Header("Space Info")]
    [SerializeField] int[] spaces;

    [Header("Space Display")]
    [SerializeField] TextMeshProUGUI[] spaceText;
    [SerializeField] string[] spaceDisplayText;
    [SerializeField] Color[] spaceDisplayColor;

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

        turn = 1;

        UpdateUI();

        playAgainButton.SetActive(false);

        StartCoroutine(AIMove());
    }

    void UpdateUI()
    {
        for (int i = 0; i < spaces.Length; i++)
        {
            spaceText[i].text = spaceDisplayText[spaces[i]];
            spaceText[i].color = spaceDisplayColor[spaces[i]];
        }

        gameStateText.text = "Player " + turn + "'s Turn";
    }

    public void SpaceClicked(int spaceClicked)
    {
        if (turn != -1 &&
            ((turn == 1 && !player1AI) || (turn == 2 && !player2AI)))
        {
            if (spaces[spaceClicked] == 0)
                MakeMove(spaceClicked);
        }
    }

    IEnumerator AIMove()
    {
        yield return new WaitForSeconds(timeBetweenAIMove);

        if ((player1AI && turn == 1) || (player2AI && turn == 2))
        {
            int depth = aiDepth[(int)aiLevel];
            minimax(spaces, depth, true, depth);
        }
    }

    int minimax(int[] currentSpaces, int depth, bool maximizingPlayer, int initialDepth)
    {
        int gameOver = CheckWin(currentSpaces);
        if (depth == 0 || gameOver != -1)
        {
            if (gameOver == turn)
            {
                return 1;
            }
            else if (gameOver != turn && gameOver > 0)
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
                newSpaces[possibleMoves[i]] = turn;

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
                newSpaces[possibleMoves[i]] = turn == 1 ? 2 : 1;

                int eval = minimax(newSpaces, depth - 1, true, initialDepth);
                minEval = Mathf.Min(minEval, eval);
            }

            return minEval;
        }
    }

    void MakeMove(int spaceToMove)
    {
        spaces[spaceToMove] = turn;
        turn = turn == 1 ? 2 : 1;
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
        turn = -1;

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
