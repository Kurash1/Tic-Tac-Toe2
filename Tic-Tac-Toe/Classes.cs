using Pastel;
using System.Runtime.InteropServices;

namespace ExtensionMethods {
    public static class IntExtensions
    {
        public static bool inRange(this int value,  int min, int max)
        {
            return (value <= max) && (value >= min);
        }
    }
}
namespace Tic_Tac_Toe
{
    class spot
    {
        public player owner;
        public spot(player owner)
        {
            this.owner = owner;
        }
    }
    class player
    {
        public ConsoleColor color;
        public char character;
        public spot capital;
        public player(ConsoleColor color, char character, spot capital)
        {
            this.color = color;
            this.character = character;
            this.capital = capital;
        }
        public player(ConsoleColor color, char character)
        {
            this.color = color;
            this.character = character;
            this.capital = capital;
        }
    }
}