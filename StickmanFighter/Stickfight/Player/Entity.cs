using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StickmanFighter.Entities
{
    public abstract class Entity
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public bool IsOnGround { get; protected set; }

        public abstract int Width { get; }
        public abstract int Height { get; }

        public Rectangle Bounds => new Rectangle(
            (int)Position.X - Width / 2,
            (int)Position.Y - Height,
            Width, Height);

        protected const float Gravity = 1800f;
        protected const float MaxFallSpeed = 1200f;

        protected Entity(Vector2 startPos)
        {
            Position = startPos;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch sb);

        protected void ApplyGravity(float dt)
        {
            if (IsOnGround) return;
            float vy = Velocity.Y + Gravity * dt;
            Velocity = new Vector2(Velocity.X, Math.Min(vy, MaxFallSpeed));
        }

        protected void DrawRect(SpriteBatch sb, Rectangle rect, Color color)
        {
            sb.Draw(Game1.Pixel, rect, color);
        }

        protected void DrawLine(SpriteBatch sb, Vector2 from, Vector2 to, Color color, int thickness = 2)
        {
            var delta = to - from;
            float len = delta.Length();
            float angle = MathF.Atan2(delta.Y, delta.X);
            sb.Draw(Game1.Pixel, new Rectangle((int)from.X, (int)from.Y, (int)len, thickness),
                null, color, angle, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}
