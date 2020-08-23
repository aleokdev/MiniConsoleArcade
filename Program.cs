using System;

namespace TicTacToe
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Tic Tac Toe!";
            Console.BufferWidth = Console.WindowWidth = 40;
            Console.BufferHeight = Console.WindowHeight = 20;

            UserInterface.SetTopPanelData("Welcome to Tic Tac Toe!\nMove using arrow keys &\n place pieces by pressing Z.");
            Console.ResetColor();

            var game = new Game();
            var winner = game.Play();
            switch(winner)
            {
                case PlayerID.None:
                    break;

                default:
                    UserInterface.SetBottomPanelData(winner switch
                    {
                        PlayerID.X => "X",
                        PlayerID.O => "O",
                        _ => throw new InvalidOperationException()
                    } + " won!\nPress any key to exit...");
                    Console.Beep(400, 100);
                    Console.Beep(450, 100);
                    Console.Beep(600, 100);
                    break;
            }
            Console.ReadKey(intercept: true);
        }

        private static void Game_OnDraw(object sender, EventArgs e)
        {
            Console.WriteLine("It's a draw!");
        }

        private static void Game_OnWin(object sender, PlayerID player)
        {
            Console.WriteLine(player switch { PlayerID.X => "X", PlayerID.O => "O" } + " won!!");
        }
    }
}
