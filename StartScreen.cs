using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace TicTacToe
{
    internal sealed class StartScreen
    {
        private interface ISettingDisplayer
        {
            void Draw(bool isSelected);

            void OnInput(ConsoleKeyInfo keyInfo);
        }

        private class GameDisplayer : ISettingDisplayer
        {
            public Type GameSelected => availableGames[gameSelectedIndex];

            public event EventHandler<Type> OnGameSelectedChanged;

            private Type[] availableGames;
            private int gameSelectedIndex;

            private GameAttribute gameSelectedMetadata => (GameAttribute)GameSelected.GetCustomAttribute(typeof(GameAttribute));

            public GameDisplayer()
            {
                availableGames = (
                    from type in Assembly.GetExecutingAssembly().GetTypes()
                    where type.GetCustomAttribute(typeof(GameAttribute)) != null
                    select type
                ).ToArray();
            }

            public void Draw(bool isSelected)
            {
                Console.ResetColor();
                Console.Write("Game");
                Console.CursorLeft++;
                Console.BackgroundColor = isSelected ? ConsoleColor.Gray : ConsoleColor.DarkGray;
                Console.ForegroundColor = isSelected ? ConsoleColor.Black : ConsoleColor.White;
                Console.Write(gameSelectedMetadata.Name);
            }

            public void OnInput(ConsoleKeyInfo keyInfo)
            {
                switch(keyInfo.Key)
                {
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        gameSelectedIndex--;
                        if (gameSelectedIndex < 0) gameSelectedIndex = 0;
                        OnGameSelectedChanged(this, GameSelected);
                        break;

                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        gameSelectedIndex++;
                        if (gameSelectedIndex >= availableGames.Length) gameSelectedIndex = availableGames.Length - 1;
                        OnGameSelectedChanged(this, GameSelected);
                        break;
                }
            }
        }

        private sealed class GameSettingDisplayer : ISettingDisplayer
        {
            public readonly PropertyInfo Property;
            public readonly GameSettingAttribute Metadata;
            public object Value { get; private set; }

            public GameSettingDisplayer(PropertyInfo prop, GameSettingAttribute meta)
            {
                Property = prop;
                Metadata = meta;
                Value = meta.DefaultValue;
            }

            public void Draw(bool isSelected)
            {
                Console.ResetColor();
                Console.Write(Metadata.Name);
                Console.CursorLeft++;
                Console.BackgroundColor = isSelected ? ConsoleColor.Gray : ConsoleColor.DarkGray;
                Console.ForegroundColor = isSelected ? ConsoleColor.Black : ConsoleColor.White;
                Console.Write(Value.ToString());
            }

            public void OnInput(ConsoleKeyInfo keyInfo)
            {
                if (!(Value is int)) return;

                switch (keyInfo.Key)
                {
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        Value = (int)Value - 1;
                        break;

                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        Value = (int)Value + 1;
                        break;
                }
            }
        }

        private sealed class SettingContainer : IEnumerable<ISettingDisplayer>
        {
            public GameDisplayer GameDisplayer { get; set; } = new GameDisplayer();
            public List<GameSettingDisplayer> GameSettings { get; private set; } = new List<GameSettingDisplayer>();
            public int Length => GameSettings.Count + 1;

            public SettingContainer()
            {
                GameDisplayer.OnGameSelectedChanged += (_, game) => updateGameSettings();

                updateGameSettings();
            }

            void updateGameSettings()
            {
                GameSettings.Clear();
                foreach (var property in GameDisplayer.GameSelected.GetProperties())
                {
                    var metadata = (GameSettingAttribute)property.GetCustomAttribute(typeof(GameSettingAttribute), false);
                    if (metadata != null)
                    {
                        GameSettings.Add(new GameSettingDisplayer(property, metadata));
                    }
                }
            }

            IEnumerator<ISettingDisplayer> IEnumerable<ISettingDisplayer>.GetEnumerator()
            {
                yield return GameDisplayer;
                foreach (var obj in GameSettings) yield return obj;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public IGame Show()
        {
            var settingContainer = new SettingContainer();
            int selectedSetting = 0;

            ConsoleKeyInfo lastKey = new ConsoleKeyInfo('\0', ConsoleKey.NoName, false, false, false);
            while (true)
            {
                const int startX = 2;

                int settingIndex = 0;
                foreach(var setting in settingContainer)
                {
                    (Console.CursorLeft, Console.CursorTop) = (startX, UserInterface.TopPanelHeight + settingIndex + 1);
                    bool isSelected = selectedSetting == settingIndex;
                    if (isSelected)
                        setting.OnInput(lastKey);
                    setting.Draw(isSelected);

                    settingIndex++;
                }

                lastKey = Console.ReadKey(intercept: true);

                switch (lastKey.Key)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        selectedSetting--;
                        if (selectedSetting < 0) selectedSetting = 0;
                        break;

                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        selectedSetting++;
                        if (selectedSetting >= settingContainer.Length) selectedSetting = settingContainer.Length - 1;
                        break;

                    case ConsoleKey.Enter:
                        IGame game = (IGame)Activator.CreateInstance(settingContainer.GameDisplayer.GameSelected);
                        foreach(var setting in settingContainer.GameSettings)
                        {
                            setting.Property.SetValue(game, setting.Value);
                        }
                        return game;
                }
            }
        }
    }
}
