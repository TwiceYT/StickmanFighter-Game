using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StickmanFighter.Entities;
using System;

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
            if (input.Back) Game.Exit();
        }

        public override void Draw(SpriteBatch sb)
        {
            int centerX = Game1.ScreenWidth / 2;
            int centerY = Game1.ScreenHeight / 2;

            DrawBackground(sb);
            DrawTitle(sb, centerX);
            DrawFighters(sb, centerX, centerY);
            DrawControls(sb, centerX);
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

            DrawTextCentered(sb, "STICKMAN FIGHTER", cx, 65, Color.White, 4);
            DrawTextCentered(sb, "Last man standing wins", cx, 155, Color.LightSteelBlue, 2);
            DrawRect(sb, new Rectangle(cx - 300, 182, 600, 2), Color.SlateGray);
        }

        private void DrawFighters(SpriteBatch sb, int centerX, int centerY)
        {
            float bob = MathF.Sin(timer * 2f) * 6f;
            DrawStickman(sb, new Vector2(centerX - 130, centerY + bob), Fighter.PlayerColors[0], 1f);
            DrawStickman(sb, new Vector2(centerX + 130, centerY - bob), Fighter.PlayerColors[1], -1f);
            DrawTextCentered(sb, "VS", centerX, centerY - 20, Color.Gold, 3);

            if ((int)(timer * 5) % 2 == 0)
                for (int i = 0; i < 5; i++)
                {
                    float a = i * MathF.PI * 2f / 5f + timer * 4f;
                    DrawRect(sb,
                        new Rectangle((int)(centerX + MathF.Cos(a) * 18), (int)(centerY + MathF.Sin(a) * 10), 4, 4),
                        Color.Yellow);
                }
        }

        private void DrawStickman(SpriteBatch sb, Vector2 pos, Color c, float facing)
        {
            float x = pos.X, y = pos.Y;
            DrawRect(sb, new Rectangle((int)x - 12, (int)y - 72, 24, 22), c);
            DrawRect(sb, new Rectangle((int)x - 3, (int)y - 50, 6, 28), c);
            DrawRect(sb, new Rectangle((int)x - 14, (int)y - 20, 5, 22), c);
            DrawRect(sb, new Rectangle((int)x + 9, (int)y - 20, 5, 22), c);
            DrawRect(sb, new Rectangle((int)(x + facing * 6), (int)y - 46, (int)(facing * 26), 5), c);
        }

        private void DrawControls(SpriteBatch sb, int centerX)
        {
            int y = Game1.ScreenHeight - 155;
            DrawRect(sb, new Rectangle(centerX - 420, y - 8, 840, 145), Color.Black * 0.6f);

            DrawTextCentered(sb, "Cross/Enter = Start          Circle/Esc = Quit", centerX, y + 5, Color.White, 2);
            DrawRect(sb, new Rectangle(centerX - 380, y + 28, 760, 1), Color.SlateGray * 0.6f);

            DrawTextCentered(sb, "Gamepad: Stick/Dpad = Move   A = Jump   X = Attack (hold to charge)   B = Block",
                centerX, y + 38, Color.LightGreen, 1);

            DrawTextCentered(sb, "F1 = controller debug screen", centerX, y + 96, Color.Gray, 1);
        }
    }
}
