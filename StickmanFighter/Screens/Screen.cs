using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StickmanFighter.Screens
{
    public abstract class Screen
    {
        protected Game1 Game;

        protected Screen(Game1 game)
        {
            Game = game;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch sb);

        protected void DrawRect(SpriteBatch sb, Rectangle rect, Color color)
        {
            sb.Draw(Game1.Pixel, rect, color);
        }

        protected void DrawLine(SpriteBatch sb, Vector2 from, Vector2 to, Color color, int thickness = 2)
        {
            var delta = to - from;
            float angle = MathF.Atan2(delta.Y, delta.X);
            sb.Draw(Game1.Pixel,
                new Rectangle((int)from.X, (int)from.Y, (int)delta.Length(), thickness),
                null, color, angle, Vector2.Zero, SpriteEffects.None, 0f);
        }

        protected void DrawText(SpriteBatch sb, string text, Vector2 pos, Color color, int scale = 2)
        {
            float x = pos.X;
            foreach (char c in text)
            {
                var bmp = Fonts.Get(c);
                for (int row = 0; row < 7; row++)
                    for (int col = 0; col < 5; col++)
                        if (bmp[row * 5 + col])
                            DrawRect(sb, new Rectangle((int)x + col * scale, (int)pos.Y + row * scale, scale, scale), color);
                x += 6 * scale;
            }
        }

        protected void DrawTextCentered(SpriteBatch sb, string text, int cx, int y, Color color, int scale = 2)
        {
            int w = text.Length * 6 * scale;
            DrawText(sb, text, new Vector2(cx - w / 2, y), color, scale);
        }
    }
}
