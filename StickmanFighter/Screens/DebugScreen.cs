using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StickmanFighter.Screens
{
    public class DebugScreen : Screen
    {
        private Screen returnTo;
        private MenuInput input = new();

        public DebugScreen(Game1 game, Screen returnTo) : base(game)
        {
            this.returnTo = returnTo;
        }

        public override void Update(GameTime gameTime)
        {
            input.Update();
            if (input.Back) Game.GoTo(returnTo);
        }

        public override void Draw(SpriteBatch sb)
        {
            DrawRect(sb, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), new Color(5, 5, 15));
            DrawTextCentered(sb, "CONTROLLER DEBUG   F1 or ESC = back", Game1.ScreenWidth / 2, 20, Color.White, 2);

            for (int i = 0; i < 4; i++)
                DrawPadInfo(sb, (PlayerIndex)i, i);
        }

        private void DrawPadInfo(SpriteBatch sb, PlayerIndex index, int slot)
        {
            var pad = GamePad.GetState(index);
            int x = 40 + slot * 310;
            int y = 65;

            Color header = pad.IsConnected ? Color.LimeGreen : Color.Red;
            DrawText(sb, $"PAD {slot + 1}  {(pad.IsConnected ? "CONNECTED" : "NOT FOUND")}", new Vector2(x, y), header, 2);

            if (!pad.IsConnected) return;

            y += 30;
            float lx = pad.ThumbSticks.Left.X;
            float ly = pad.ThumbSticks.Left.Y;
            DrawText(sb, $"STICK  X {lx:+0.00;-0.00}  Y {ly:+0.00;-0.00}", new Vector2(x, y), Color.Yellow, 1);

            int cx2 = x + 200, cy2 = y + 6;
            DrawRect(sb, new Rectangle(cx2 - 20, cy2 - 20, 40, 40), new Color(30, 30, 50));
            DrawRect(sb, new Rectangle(cx2 - 2 + (int)(lx * 18), cy2 - 2 - (int)(ly * 18), 4, 4), Color.Yellow);

            y += 20;
            var buttons = new (string label, Buttons btn)[]
            {
                ("A  Cross",    Buttons.A),
                ("B  Circle",   Buttons.B),
                ("X  Square",   Buttons.X),
                ("Y  Triangle", Buttons.Y),
                ("Start",       Buttons.Start),
                ("Back/Select", Buttons.Back),
                ("DPad Up",     Buttons.DPadUp),
                ("DPad Down",   Buttons.DPadDown),
                ("DPad Left",   Buttons.DPadLeft),
                ("DPad Right",  Buttons.DPadRight),
            };

            foreach (var (label, btn) in buttons)
            {
                bool held = pad.IsButtonDown(btn);
                DrawText(sb, (held ? "> " : "  ") + label, new Vector2(x, y), held ? Color.LimeGreen : Color.Gray, 1);
                y += 14;
            }
        }
    }
}
