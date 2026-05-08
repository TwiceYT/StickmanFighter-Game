using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StickmanFighter.Entities;

namespace StickmanFighter.Screens
{
    public class MainMenuScreen : Screen
    {
        private MenuInput input = new();
        private float timer;

        public MainMenuScreen(Game1 game) : base(game) { }

        public override void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            input.Update();

            if (input.Confirm) Game.GoTo(new PlayerSelectScreen(Game));
            if (input.Back)    Game.Exit();
        }

        public override void Draw(SpriteBatch sb)
        {
            int cx = Game1.ScreenWidth  / 2;
            int cy = Game1.ScreenHeight / 2;

            DrawBackground(sb);
            DrawTitle(sb, cx);
            DrawFighters(sb, cx, cy);
            DrawControls(sb, cx);
        }

        private void DrawBackground(SpriteBatch sb)
        {
            int w = Game1.ScreenWidth, h = Game1.ScreenHeight;
            for (int i = 0; i < 10; i++)
            {
                float t = i / 10f;
                DrawRect(sb, new Rectangle(0, i * (h / 10), w, h / 10 + 1),
                    Color.Lerp(new Color(15, 18, 35), new Color(30, 42, 70), t));
            }
        }

        private void DrawTitle(SpriteBatch sb, int cx)
        {
            float pulse = (MathF.Sin(timer * 2f) + 1f) * 0.5f;
            DrawRect(sb, new Rectangle(cx - 380, 55, 760, 85),
                Color.Lerp(new Color(20, 50, 120), new Color(50, 90, 200), pulse) * 0.3f);

            DrawTextCentered(sb, "STICKMAN FIGHTER", cx, 65,  Color.White, 4);
            DrawTextCentered(sb, "Last man standing wins",   cx, 155, Color.LightSteelBlue, 2);
            DrawRect(sb, new Rectangle(cx - 300, 182, 600, 2), Color.SlateGray);
        }

        private void DrawFighters(SpriteBatch sb, int cx, int cy)
        {
            float bob = MathF.Sin(timer * 2f) * 6f;
            DrawStickman(sb, new Vector2(cx - 130, cy + bob), Fighter.PlayerColors[0],  1f);
            DrawStickman(sb, new Vector2(cx + 130, cy - bob), Fighter.PlayerColors[1], -1f);
            DrawTextCentered(sb, "VS", cx, cy - 20, Color.Gold, 3);

            if ((int)(timer * 5) % 2 == 0)
                for (int i = 0; i < 5; i++)
                {
                    float a = i * MathF.PI * 2f / 5f + timer * 4f;
                    DrawRect(sb,
                        new Rectangle((int)(cx + MathF.Cos(a) * 18), (int)(cy + MathF.Sin(a) * 10), 4, 4),
                        Color.Yellow);
                }
        }

        private void DrawStickman(SpriteBatch sb, Vector2 pos, Color c, float facing)
        {
            float x = pos.X, y = pos.Y;
            DrawRect(sb, new Rectangle((int)x - 12, (int)y - 72, 24, 22), c);
            DrawRect(sb, new Rectangle((int)x - 3,  (int)y - 50,  6, 28), c);
            DrawRect(sb, new Rectangle((int)x - 14, (int)y - 20,  5, 22), c);
            DrawRect(sb, new Rectangle((int)x + 9,  (int)y - 20,  5, 22), c);
            DrawRect(sb, new Rectangle((int)(x + facing * 6), (int)y - 46, (int)(facing * 26), 5), c);
        }

        private void DrawControls(SpriteBatch sb, int cx)
        {
            int y = Game1.ScreenHeight - 155;
            DrawRect(sb, new Rectangle(cx - 420, y - 8, 840, 145), Color.Black * 0.6f);

            DrawTextCentered(sb, "Cross/Enter = Start          Circle/Esc = Quit", cx, y + 5, Color.White, 2);
            DrawRect(sb, new Rectangle(cx - 380, y + 28, 760, 1), Color.SlateGray * 0.6f);

            DrawTextCentered(sb, "Gamepad: Stick/Dpad = Move   A = Jump   X = Attack (hold to charge)   B = Block",
                cx, y + 38, Color.LightGreen, 1);

            int col1 = cx - 400, col2 = cx + 10, row = y + 58;
            DrawText(sb, "KB P1: WASD  F=Attack  G=Block",       new Vector2(col1, row),      Fighter.PlayerColors[0], 1);
            DrawText(sb, "KB P2: Arrows  1=Attack  2=Block",     new Vector2(col1, row + 16), Fighter.PlayerColors[1], 1);
            DrawText(sb, "KB P3: IJKL  U=Attack  O=Block",       new Vector2(col2, row),      Fighter.PlayerColors[2], 1);
            DrawText(sb, "KB P4: Num4/6/8  7=Attack  9=Block",   new Vector2(col2, row + 16), Fighter.PlayerColors[3], 1);

            DrawTextCentered(sb, "F1 = controller debug screen", cx, y + 96, Color.Gray, 1);
        }
    }
}
