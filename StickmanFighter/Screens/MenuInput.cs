using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace StickmanFighter.Screens
{
    public class MenuInput
    {
        private KeyboardState prevKeys;
        private GamePadState  prevPad;

        public MenuInput()
        {
            prevKeys = Keyboard.GetState();
            prevPad  = GamePad.GetState(PlayerIndex.One);
        }

        public bool Left    { get; private set; }
        public bool Right   { get; private set; }
        public bool Confirm { get; private set; }
        public bool Back    { get; private set; }

        public void Update()
        {
            var keys = Keyboard.GetState();
            var pad  = GamePad.GetState(PlayerIndex.One);

            Left    = JustPressed(keys, Keys.Left)  || JustPressed(keys, Keys.Q) || JustBtn(pad, Buttons.DPadLeft)  || StickLeft(pad);
            Right   = JustPressed(keys, Keys.Right) || JustPressed(keys, Keys.E) || JustBtn(pad, Buttons.DPadRight) || StickRight(pad);
            Confirm = JustPressed(keys, Keys.Enter) || JustBtn(pad, Buttons.A)   || JustBtn(pad, Buttons.Start);
            Back    = JustPressed(keys, Keys.Escape)|| JustBtn(pad, Buttons.B)   || JustBtn(pad, Buttons.Back);

            prevKeys = keys;
            prevPad  = pad;
        }

        private bool JustPressed(KeyboardState keys, Keys key) => keys.IsKeyDown(key) && !prevKeys.IsKeyDown(key);
        private bool JustBtn(GamePadState pad, Buttons btn)    => pad.IsButtonDown(btn) && !prevPad.IsButtonDown(btn);
        private bool StickLeft(GamePadState  pad) => pad.ThumbSticks.Left.X < -0.5f && prevPad.ThumbSticks.Left.X >= -0.5f;
        private bool StickRight(GamePadState pad) => pad.ThumbSticks.Left.X >  0.5f && prevPad.ThumbSticks.Left.X <=  0.5f;
    }
}
