using Microsoft.Xna.Framework.Input;

namespace StickmanFighter.Input
{
    public class InputScheme
    {
        public Keys Left, Right, Jump, Attack, Block;

        public InputScheme(Keys left, Keys right, Keys jump, Keys attack, Keys block)
        {
            Left = left;
            Right = right;
            Jump = jump;
            Attack = attack;
            Block = block;
        }

        public static InputScheme[] All =
        {
            new(Keys.A,       Keys.D,       Keys.W,       Keys.F,       Keys.G),
            new(Keys.Left,    Keys.Right,   Keys.Up,      Keys.J, Keys.K),
            new(Keys.J,       Keys.L,       Keys.I,       Keys.U,       Keys.O),
            new(Keys.NumPad4, Keys.NumPad6, Keys.NumPad8, Keys.NumPad7, Keys.NumPad9),
        };
    }
}
