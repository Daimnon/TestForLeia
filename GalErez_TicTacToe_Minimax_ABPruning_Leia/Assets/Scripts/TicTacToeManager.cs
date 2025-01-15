using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class TicTacToeManager : MonoBehaviour
{
    private const string EMPTY_TILE_TAG = "emptyTile";

    public enum SpaceState { Empty = 0, Player1 = 1, Player2 = 2 }

    [Header("Game Info")]
    [SerializeField] private int turn = 1;
    [SerializeField] private bool player1AI = false;
    [SerializeField] private bool player2AI = false;
    [SerializeField] private float timeBetweenAIMove;

    public enum AILevel { VeryEasy, Easy, Medium, Impossible }

    [SerializeField] private AILevel aiLevel;
    [SerializeField] private int[] aiDepth;

    [Header("Game State")]
    [SerializeField] private SpaceState[] spaces; // Array of SpaceState enums

    [Header("Tile Info")]
    [SerializeField] private Transform[] tileTransforms; // Reference to the tile Transforms

    [Header("Piece Pooling")]
    [SerializeField] private PiecePooler piecePooler;

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

    void Start()
    {
        ResetGameState();
        CheckAI();
    }

    private void CheckAI()
    {
        // Check if it's AI's turn and start the AI move coroutine
        if ((player1AI && turn == 1) || (player2AI && turn == 2))
        {
            StartCoroutine(AIMove());
        }
    }

    void ResetGameState()
    {
        // The predefined order based on the physical layout of tileTransforms
        int[] spaceOrder = new int[]
        {
        6, 7, 8, // Top row
        3, 4, 5, // Middle row
        0, 1, 2  // Bottom row
        };

        SpaceState[] reorderedSpaces = new SpaceState[9];
        for (int i = 0; i < spaceOrder.Length; i++)
        {
            reorderedSpaces[spaceOrder[i]] = SpaceState.Empty;
        }

        spaces = reorderedSpaces; // Assign the reordered array
        turn = 1; // Player 1 starts
    }

    private void OnFingerDown(ETouch.Finger finger)
    {
        Debug.Log("Input received");

        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(finger.screenPosition);
        touchPosition.z = 0; // Ensure we are in 2D space

        float maxRayDistance = 100f;
        int layerMask = LayerMask.GetMask("Tiles");
        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero, maxRayDistance, layerMask);

        if (hit.collider != null && hit.collider.CompareTag(EMPTY_TILE_TAG))
        {
            MakeMove(hit.transform); // Make the move
        }
    }

    // Method that handles the logic of making a move at the given index
    private void MakeMove(Transform tile)
    {
        int spaceToMove = GetTileIndex(tile);
        if (spaces[spaceToMove] != SpaceState.Empty) return; // Skip if the space is already occupied

        // Visualize the piece using the pool
        TileType currentPieceType = turn == 1 ? TileType.X : TileType.O;
        GameObject piece = piecePooler.GetPiece(currentPieceType, tile.transform, true);

        if (piece != null)
        {
            spaces[spaceToMove] = turn == 1 ? SpaceState.Player1 : SpaceState.Player2; // Mark the tile as occupied by the current player
            turn = turn == 1 ? 2 : 1; // Switch turns
            CheckEndGame(); // Check if the game has ended
        }

        // Check if AI needs to make a move
        CheckAI(); // Moved here to ensure AI move happens only after the player move
    }

    // AI's Move logic
    IEnumerator AIMove()
    {
        yield return new WaitForSeconds(timeBetweenAIMove); // Delay for AI's move

        if ((player1AI && turn == 1) || (player2AI && turn == 2)) // Check if it's AI's turn
        {
            int depth = aiDepth[(int)aiLevel];
            minimax(spaces, depth, true, depth);
        }
        else
        {
            // If it's the player's turn, do nothing
            yield break;
        }
    }

    int minimax(SpaceState[] currentSpaces, int depth, bool maximizingPlayer, int initialDepth)
    {
        // Check if the game is over or if we've reached the maximum depth
        int gameOver = CheckWin(currentSpaces);
        if (depth == 0 || gameOver != -1)
        {
            if (gameOver == 1)
            {
                return 1;  // Player 1 wins
            }
            else if (gameOver == 2)
            {
                return -1;  // Player 2 wins
            }
            else if (gameOver == 0)
            {
                return 0;  // Tie
            }
        }

        if (maximizingPlayer)  // Player 1's turn (AI)
        {
            int maxEval = int.MinValue;
            int bestMove = -1;
            for (int space = 0; space < 9; space++)
            {
                if (currentSpaces[space] == SpaceState.Empty)
                {
                    currentSpaces[space] = SpaceState.Player2;  // Simulate Player 1's move
                    int eval = minimax(currentSpaces, depth - 1, false, initialDepth);  // Recursively evaluate
                    currentSpaces[space] = SpaceState.Empty;  // Undo the move

                    if (eval > maxEval)
                    {
                        maxEval = eval;
                        bestMove = space;  // Best move for Player 1
                    }
                }
            }

            // Log best move and evaluation
            Debug.Log($"Player 1 (AI) Best Move: {bestMove}, Evaluation: {maxEval}");

            // Only make the move on the first call (when depth == initialDepth)
            if (initialDepth == depth && bestMove != -1)
            {
                spaces[bestMove] = SpaceState.Player2;  // Make the best move for Player 1
                MakeMove(tileTransforms[bestMove]);
                turn = 2;  // Switch to Player 2
            }

            return maxEval;
        }
        else  // Player 2's turn (opponent)
        {
            int minEval = int.MaxValue;
            for (int space = 0; space < 9; space++)
            {
                if (currentSpaces[space] == SpaceState.Empty)
                {
                    currentSpaces[space] = SpaceState.Player1;  // Simulate Player 2's move
                    int eval = minimax(currentSpaces, depth - 1, true, initialDepth);  // Recursively evaluate
                    currentSpaces[space] = SpaceState.Empty;  // Undo the move

                    minEval = Mathf.Min(minEval, eval);
                }
            }

            return minEval;
        }
    }

    // This method checks if the game has ended (win/tie conditions)
    void CheckEndGame()
    {
        int win = CheckWin(spaces);

        if (win == 0)
            Tie();
        else if (win == 1)
            Player1Wins();
        else if (win == 2)
            Player2Wins();
    }

    // Winning condition checks
    void Player1Wins() { Debug.Log("Player 1 Wins!"); }
    void Player2Wins() { Debug.Log("Player 2 Wins!"); }
    void Tie() { Debug.Log("Tie!"); }

    // Method for replaying the game
    public void PlayAgain()
    {
        ResetGameState();
        //piecePooler.ReturnAllPieces(); // Return all pieces to the pool when restarting
    }

    // Check for the winning conditions: rows, columns, and diagonals
    int CheckWin(SpaceState[] spacesToCheck)
    {
        List<SpaceState> spaceNum = new List<SpaceState>();

        // Rows
        int rowStart = 0;
        for (int row = 0; row < 3; row++)
        {
            if (spacesToCheck[rowStart] == spacesToCheck[rowStart + 1] && spacesToCheck[rowStart + 1] == spacesToCheck[rowStart + 2])
            {
                if (spacesToCheck[rowStart] != SpaceState.Empty)
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
                if (spacesToCheck[columnStart] != SpaceState.Empty)
                    spaceNum.Add(spacesToCheck[columnStart]);
            }

            columnStart++;
        }

        // Diagonal Up
        if (spacesToCheck[0] == spacesToCheck[4] && spacesToCheck[4] == spacesToCheck[8])
        {
            if (spacesToCheck[0] != SpaceState.Empty)
                spaceNum.Add(spacesToCheck[0]);
        }

        // Diagonal Down
        if (spacesToCheck[6] == spacesToCheck[4] && spacesToCheck[4] == spacesToCheck[2])
        {
            if (spacesToCheck[6] != SpaceState.Empty)
                spaceNum.Add(spacesToCheck[6]);
        }

        if (spaceNum.Count > 0)
        {
            for (int i = 0; i < spaceNum.Count; i++)
            {
                if (spaceNum[i] == SpaceState.Player1)
                {
                    return 1;
                }
                else if (spaceNum[i] == SpaceState.Player2)
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
                if (spacesToCheck[i] == SpaceState.Empty)
                    freeSpaces++;
            }

            if (freeSpaces == 0)
                return 0;
            else
                return -1;
        }

        return -1;
    }

    // Helper method to get the index of a tile based on its transform position
    int GetTileIndex(Transform tile)
    {
        for (int i = 0; i < tileTransforms.Length; i++)
        {
            if (tileTransforms[i] == tile)
            {
                return i;
            }
        }
        return -1; // Return -1 if not found
    }
}
