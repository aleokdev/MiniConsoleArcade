using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    public static class UserInterface
    {
        public static int TopPanelHeight { get; private set; }
        public static int BottomPanelHeight { get; private set; }

        public static void SetTopPanelData(string text)
        {
            string[] lines = text.Split('\n');

            Console.CursorTop = 0;
            foreach(string line in lines)
            {
                Console.CursorLeft = Console.WindowWidth / 2 - line.Length / 2;
                Console.Write(line);

                Console.CursorTop++;
            }

            Console.CursorLeft = 0;
            Console.Write(new string('=', Console.WindowWidth));
            TopPanelHeight = ++Console.CursorTop;
        }

        public static void SetBottomPanelData(string text)
        {
            string[] lines = text.Split('\n');

            Console.CursorTop = Console.WindowHeight - 1;
            foreach (string line in lines.Reverse())
            {
                Console.CursorLeft = Console.WindowWidth / 2 - line.Length / 2;
                Console.Write(line);

                Console.CursorTop--;
            }

            Console.CursorLeft = 0;
            Console.Write(new string('=', Console.WindowWidth));
            TopPanelHeight = Console.WindowHeight - Console.CursorTop;
        }
    }
}
