using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StickmanFighter.Map
{
    public abstract class Map
    {
        protected List<Platform> platforms = new();
        public abstract List<Vector2> SpawnPoints { get; }

        public void Draw(SpriteBatch sb)
        {
            DrawBackground(sb);
            foreach (var p in platforms)
                p.Draw(sb);
        }

        protected abstract void DrawBackground(SpriteBatch sb);

        public bool ResolveCollisions(ref Vector2 pos, ref Vector2 vel, int w, int h, float prevFeetY)
        {
            bool grounded = false;
            foreach (var p in platforms)
                if (p.ResolveCollision(ref pos, ref vel, w, h, prevFeetY))
                    grounded = true;
            return grounded;
        }

        public bool IsInVoid(Vector2 pos)
        {
            return pos.Y > Game1.ScreenHeight + 300
                || pos.X < -600
                || pos.X > Game1.ScreenWidth + 600;
        }
    }

    public class ArenaMap : Map
    {
        public ArenaMap()
        {
            platforms.Add(new Platform(100, 540, 1080, 40, false, new Color(70, 90, 110)));
            platforms.Add(new Platform(80,  400, 280,  22, true,  new Color(80, 105, 125)));
            platforms.Add(new Platform(920, 400, 280,  22, true,  new Color(80, 105, 125)));
            platforms.Add(new Platform(490, 330, 300,  20, true,  new Color(90, 115, 135)));
            platforms.Add(new Platform(200, 255, 140,  18, true,  new Color(85, 110, 130)));
            platforms.Add(new Platform(940, 255, 140,  18, true,  new Color(85, 110, 130)));
        }

        public override List<Vector2> SpawnPoints => new()
        {
            new Vector2(250,  510),
            new Vector2(1030, 510),
            new Vector2(300,  370),
            new Vector2(980,  370),
        };

        protected override void DrawBackground(SpriteBatch sb)
        {
            int w = Game1.ScreenWidth;
            int h = Game1.ScreenHeight;

            for (int i = 0; i < 10; i++)
            {
                float t = i / 10f;
                var c = Color.Lerp(new Color(15, 18, 35), new Color(30, 42, 70), t);
                sb.Draw(Game1.Pixel, new Rectangle(0, i * (h / 10), w, h / 10 + 1), c);
            }
        }
    }
}
