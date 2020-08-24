using System;

namespace TicTacToe
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.BufferWidth = Console.WindowWidth = 40;
            Console.BufferHeight = Console.WindowHeight = 20;

            var startScreen = new StartScreen();

            while (true)
            {
                Console.Title = "Play Settings";
                UserInterface.Reset();
                Console.ResetColor();
                Console.Clear();
                UserInterface.SetTopPanelData("Press up/down to scroll through settings\nLeft/right to change values\nEnter to play");
                IGame game = startScreen.Show();
                Console.ResetColor();
                Console.Clear();

                game.Play();
            }
        }
    }
}
