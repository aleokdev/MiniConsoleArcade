using System;

namespace TicTacToe
{
    internal class Program
    {
        static void Main(string[] args) {
            var game = new Game();
            game.OnWin += Game_OnWin;
            game.OnDraw += Game_OnDraw;
            game.Loop();
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
