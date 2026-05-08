using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StickmanFighter.Map
{
    public class Platform
    {
        public Rectangle Bounds       { get; }
        public bool      PassThrough  { get; }

        private Color color;

        public Platform(int x, int y, int w, int h, bool passThrough, Color color)
        {
            Bounds      = new Rectangle(x, y, w, h);
            PassThrough = passThrough;
            this.color  = color;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Game1.Pixel, Bounds, color);
            sb.Draw(Game1.Pixel, new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 3), Color.Lerp(color, Color.White, 0.4f));
        }

        public bool FixCollision(ref Vector2 pos, ref Vector2 vel, int w, int h, float prevFeetY)
        {
            float left  = pos.X - w / 2f;
            float right = pos.X + w / 2f;
            float feet  = pos.Y;

            if (right <= Bounds.Left || left >= Bounds.Right)
                return false;

            bool wasAbove   = prevFeetY <= Bounds.Top + 2f;
            bool crossedTop = feet >= Bounds.Top && feet <= Bounds.Top + h * 0.6f;

            if (wasAbove && crossedTop && vel.Y >= 0)
            {
                pos = new Vector2(pos.X, Bounds.Top);
                vel = new Vector2(vel.X, 0);
                return true;
            }

            if (!PassThrough)
            {
                bool insidePlat = feet > Bounds.Top && pos.Y - h < Bounds.Bottom;
                if (insidePlat)
                {
                    if (vel.X > 0 && pos.X < Bounds.Center.X)
                        pos = new Vector2(Bounds.Left - w / 2f, pos.Y);
                    else if (vel.X < 0 && pos.X > Bounds.Center.X)
                        pos = new Vector2(Bounds.Right + w / 2f, pos.Y);
                    vel = new Vector2(0, vel.Y);
                }
            }

            return false;
        }
    }
}
