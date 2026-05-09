using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StickmanFighter.Screens
{
    public class GameOverScreen : Screen
    {
        private string winner;
        private float timer;
        private MenuInput input = new();

        public GameOverScreen(Game1 game, string winner) : base(game)
        {
            this.winner = winner;
        }

        public override void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            input.Update();

            if (timer < 1.5f) return;

            if (input.Confirm) Game.GoTo(new PlayerSelectScreen(Game));
            if (input.Back) Game.GoTo(new MainMenuScreen(Game));
        }

        public override void Draw(SpriteBatch sb)
        {
            int cx = Game1.ScreenWidth / 2;
            int cy = Game1.ScreenHeight / 2;

            DrawRect(sb, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), new Color(8, 10, 20));
            DrawConfetti(sb);

            DrawRect(sb, new Rectangle(cx - 280, cy - 120, 560, 240), new Color(20, 28, 55));
            DrawRect(sb, new Rectangle(cx - 280, cy - 120, 560, 2), Color.CornflowerBlue);
            DrawRect(sb, new Rectangle(cx - 280, cy + 118, 560, 2), Color.CornflowerBlue);
            DrawRect(sb, new Rectangle(cx - 280, cy - 120, 2, 240), Color.CornflowerBlue);
            DrawRect(sb, new Rectangle(cx + 278, cy - 120, 2, 240), Color.CornflowerBlue);

            DrawTextCentered(sb, "WINNER", cx, cy - 100, Color.Gold, 3);
            DrawTextCentered(sb, winner, cx, cy - 48, Color.CornflowerBlue, 5);

            if (timer > 1.5f)
            {
                DrawTextCentered(sb, "Cross/Enter = play again", cx, cy + 52, Color.LightSteelBlue, 2);
                DrawTextCentered(sb, "Circle/Esc = main menu", cx, cy + 78, Color.LightSteelBlue, 2);
            }
        }

        private void DrawConfetti(SpriteBatch sb)
        {
            Color[] colors = { Color.Yellow, Color.Cyan, Color.Magenta, Color.LimeGreen, Color.Orange };
            for (int i = 0; i < 30; i++)
            {
                float x = (i * 131 + (int)(timer * 55) * (i + 1)) % Game1.ScreenWidth;
                float y = (i * 47 + (int)(timer * 38)) % Game1.ScreenHeight;
                DrawRect(sb, new Rectangle((int)x, (int)y, 6, 6), colors[i % colors.Length] * 0.7f);
            }
        }
    }
}
