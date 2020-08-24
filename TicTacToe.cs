using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe
{
    public enum PlayerID
    {
        X,
        O,
        None
    }

    [Game(Name = "Tic Tac Toe")]
    public sealed class TicTacToe : IGame
    {
        [GameSetting(Name = "Board Length", DefaultValue = 3)]
        public int BoardLength { get; set; }

        Array2D<PlayerID> board;
        int cellsFilled;
        (int, int) playerCursorPos;
        (int, int) boardStartingPos => (
            Console.WindowWidth / 2 - BoardLength / 2,
            Console.WindowHeight / 2 - BoardLength / 2
            );

        PlayerID currentPlayerPlaying = PlayerID.X;

        public void Play()
        {
            Console.Title = "Tic Tac Toe!";
            UserInterface.SetTopPanelData("Welcome to Tic Tac Toe!\nMove using arrow keys &\n place pieces by pressing Z.");
            board = new Array2D<PlayerID>((uint)BoardLength, (uint)BoardLength, PlayerID.None);

            while (true)
            {
                // Draw the board.
                drawBoard();

                // Set the cursor position.
                (Console.CursorLeft, Console.CursorTop) = (boardStartingPos.Item1 + playerCursorPos.Item1, boardStartingPos.Item2 + playerCursorPos.Item2);

                // Move the player cursor on key press.
                var keyInfo = Console.ReadKey(intercept: true);
                var (moveX, moveY) = keyInfo.Key switch
                {
                    ConsoleKey.RightArrow => (1, 0),
                    ConsoleKey.LeftArrow => (-1, 0),
                    ConsoleKey.UpArrow => (0, -1),
                    ConsoleKey.DownArrow => (0, 1),
                    _ => (0, 0)
                };
                tryMovePlayerCursorPos(moveX, moveY);


                if (keyInfo.Key == ConsoleKey.Z)
                {
                    bool canPlace = board[playerCursorPos.Item1, playerCursorPos.Item2] == PlayerID.None;
                    if (canPlace)
                    {
                        Console.Beep(currentPlayerPlaying switch
                        {
                            PlayerID.X => 500,
                            PlayerID.O => 700,
                            _ => 0
                        }, 100);

                        // Set the current cell according to the current user playing
                        board[playerCursorPos.Item1, playerCursorPos.Item2] = currentPlayerPlaying;
                        cellsFilled++;

                        // Change current user playing
                        currentPlayerPlaying = currentPlayerPlaying switch
                        {
                            PlayerID.X => PlayerID.O,
                            PlayerID.O => PlayerID.X,
                            _ => throw new Exception("User playing was not O or X")
                        };

                        var winner = checkWinner();
                        if(winner != PlayerID.None)
                        {
                            drawBoard();
                            UserInterface.SetBottomPanelData(winner switch
                            {
                                PlayerID.X => "X",
                                PlayerID.O => "O",
                                _ => throw new InvalidOperationException()
                            } + " won!\nPress any key to exit...");
                            Console.Beep(400, 100);
                            Console.Beep(450, 100);
                            Console.Beep(600, 100);
                            Console.ReadKey(intercept: true);
                            return;
                        } else if (cellsFilled == board.Width * board.Height)
                        {
                            drawBoard();
                            UserInterface.SetBottomPanelData("It's a draw!\nPress any key to exit...");
                            Console.Beep(600, 100);
                            Console.Beep(400, 100);
                            Console.Beep(200, 400);
                            Console.ReadKey(intercept: true);
                            return;
                        }
                    }
                    else
                    {
                        Console.Beep(300, 100);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if there's an user who has won in the board.
        /// </summary>
        /// <returns>The player who has won, or PlayerID.None otherwise.</returns>
        PlayerID checkWinner()
        {
            // Check horizontally
            for (int y = 0; y < BoardLength; y++)
            {
                PlayerID chainOwner = board[0, y];
                if (chainOwner == PlayerID.None) continue;

                int chainLength = 0;
                for (int x = 0; x < BoardLength; x++)
                {
                    var thisCell = board[x, y];
                    if (thisCell == chainOwner)
                    {
                        chainLength++;
                        if (chainLength == BoardLength) return chainOwner;
                    }
                    else
                        break;
                }
            }

            // Check vertically
            for (int x = 0; x < BoardLength; x++)
            {
                PlayerID chainOwner = board[x, 0];
                if (chainOwner == PlayerID.None) continue;

                int chainLength = 0;
                for (int y = 0; y < BoardLength; y++)
                {
                    var thisCell = board[x, y];
                    if (thisCell == chainOwner)
                    {
                        chainLength++;
                        if (chainLength == BoardLength) return chainOwner;
                    }
                    else
                        break;
                }
            }

            // Check diagonally (right)
            {
                PlayerID chainOwner = board[0, 0];
                if (chainOwner != PlayerID.None)
                {
                    int chainLength = 0;
                    for (int d = 0; d < BoardLength; d++)
                    {
                        var thisCell = board[d, d];
                        if (thisCell == chainOwner)
                        {
                            chainLength++;
                            if (chainLength == BoardLength) return chainOwner;
                        }
                        else
                            break;
                    }
                }
            }

            // Check diagonally (left)
            {
                PlayerID chainOwner = board[BoardLength - 1, 0];
                if (chainOwner != PlayerID.None)
                {
                    int chainLength = 0;
                    for (int d = 0; d < BoardLength; d++)
                    {
                        var thisCell = board[BoardLength - d - 1, d];
                        if (thisCell == chainOwner)
                        {
                            chainLength++;
                            if (chainLength == BoardLength) return chainOwner;
                        }
                        else
                            break;
                    }
                }
            }

            return PlayerID.None;
        }

        /// <summary>
        /// Tries to change the player's cursor position by X and Y amount and clamps to the board's borders.
        /// </summary>
        void tryMovePlayerCursorPos(int x, int y) =>
            playerCursorPos = (Math.Clamp(playerCursorPos.Item1 + x, 0, (int)board.Width - 1),
                Math.Clamp(playerCursorPos.Item2 + y, 0, (int)board.Height - 1));

        /// <summary>
        /// Draws the board.
        /// </summary>
        void drawBoard()
        {
            for (int y = 0; y < board.Height; y++)
            {
                (Console.CursorLeft, Console.CursorTop) = (boardStartingPos.Item1, (int)(boardStartingPos.Item2 + y));
                for (int x = 0; x < board.Width; x++)
                {
                    var cellState = board[x, y];
                    char cellChar;
                    (Console.ForegroundColor, cellChar) = cellState switch
                    {
                        PlayerID.X => (ConsoleColor.Blue, 'X'),
                        PlayerID.O => (ConsoleColor.Red, 'O'),
                        _ => (ConsoleColor.DarkGray, '.')
                    };
                    Console.Write(cellChar);
                }
            }
        }
    }
}
