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

    public class Game
    {
        public int BoardLength { get; set; } = 3;

        public event EventHandler<PlayerID> OnWin;
        public event EventHandler OnDraw;

        Array2D<PlayerID> board;
        int cellsFilled;
        (int, int) boardPos;
        (int, int) playerCursorPos;
        PlayerID currentPlayerPlaying = PlayerID.X;

        public void Loop()
        {
            Console.Title = "Tic Tac Toe!";
            DrawHeader();

            boardPos = (1, Console.CursorTop);

            board = new Array2D<PlayerID>((uint)BoardLength, (uint)BoardLength, PlayerID.None);

            while (true)
            {
                DrawBoard();
                (Console.CursorLeft, Console.CursorTop) = (boardPos.Item1 + playerCursorPos.Item1, boardPos.Item2 + playerCursorPos.Item2);

                var keyInfo = Console.ReadKey(intercept: true);
                var (moveX, moveY) = keyInfo.Key switch
                {
                    ConsoleKey.RightArrow => (1, 0),
                    ConsoleKey.LeftArrow => (-1, 0),
                    ConsoleKey.UpArrow => (0, -1),
                    ConsoleKey.DownArrow => (0, 1),
                    _ => (0, 0)
                };
                TryMovePlayerCursorPos(moveX, moveY);

                if (keyInfo.Key == ConsoleKey.Enter)
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
                        if (winner != PlayerID.None)
                        {
                            OnWin?.Invoke(this, winner);
                            return;
                        }
                        else if (cellsFilled == board.Width * board.Height)
                        {
                            OnDraw?.Invoke(this, null);
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
                    } else
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

        void DrawHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Welcome to ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Tic Tac Toe");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("!\n");
            Console.ResetColor();
        }

        void TryMovePlayerCursorPos(int x, int y)
        {
            playerCursorPos = (Math.Clamp(playerCursorPos.Item1 + x, 0, (int)board.Width - 1),
                Math.Clamp(playerCursorPos.Item2 + y, 0, (int)board.Height - 1));
        }

        void DrawBoard()
        {
            for (int y = 0; y < board.Height; y++)
            {
                (Console.CursorLeft, Console.CursorTop) = (boardPos.Item1, (int)(boardPos.Item2 + y));
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
