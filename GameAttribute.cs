using System;

namespace TicTacToe
{
    public interface IGame {
        void Play();
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GameAttribute : Attribute
    {
        public string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class GameSettingAttribute : Attribute
    {
        public string Name { get; set; }
        public object DefaultValue { get; set; }
    }
}
