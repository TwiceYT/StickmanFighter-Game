using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StickmanFighter.Entities;
using StickmanFighter.Input;

namespace StickmanFighter.Screens
{
    public class GameScreen : Screen
    {
        private List<Fighter> fighters = new();
        private Map.Map map;

        public GameScreen(Game1 game, int playerCount, PlayerInput[] inputs) : base(game)
        {
            map = new Map.ArenaMap();

            string[] names = { "P1", "P2", "P3", "P4" };

            for (int i = 0; i < playerCount; i++)
                fighters.Add(new Fighter(names[i], Fighter.PlayerColors[i], map.SpawnPoints[i], inputs[i]));
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var f in fighters)
                f.Update(gameTime);

            foreach (var f in fighters.Where(f => f.IsAlive))
                f.ApplyMapCollision(map);

            CheckHits();
            CheckWinner();
        }

        private void CheckHits()
        {
            var alive = fighters.Where(f => f.IsAlive).ToList();

            foreach (var attacker in alive)
            {
                var hitbox = attacker.GetAttackHitbox();
                if (hitbox == null) continue;

                float charge = attacker.ConsumeHit();

                foreach (var defender in alive)
                {
                    if (defender == attacker) continue;
                    if (defender.Bounds.Intersects(hitbox.Value))
                        defender.TakeHit(charge, attacker.Position);
                }
            }
        }

        private void CheckWinner()
        {
            var alive = fighters.Where(f => f.IsAlive).ToList();
            if (alive.Count <= 1)
                Game.GoTo(new GameOverScreen(Game, alive.Count == 1 ? alive[0].Name : "Nobody"));
        }

        public override void Draw(SpriteBatch sb)
        {
            map.Draw(sb);

            foreach (var f in fighters)
                f.Draw(sb);

            DrawHUD(sb);
        }

        private void DrawHUD(SpriteBatch sb)
        {
            int panelW = 200, panelH = 72, margin = 10;
            int y = Game1.ScreenHeight - panelH - margin;

            for (int i = 0; i < fighters.Count; i++)
                DrawPanel(sb, fighters[i], new Rectangle(margin + i * (panelW + margin), y, panelW, panelH));
        }

        private void DrawPanel(SpriteBatch sb, Fighter f, Rectangle p)
        {
            DrawRect(sb, p, Color.Black * 0.9f);

            Color border = f.IsAlive ? f.Color : Color.Gray;
            DrawRect(sb, new Rectangle(p.X,         p.Y,          p.Width, 2),        border);
            DrawRect(sb, new Rectangle(p.X,         p.Bottom - 2, p.Width, 2),        border);
            DrawRect(sb, new Rectangle(p.X,         p.Y,          2,       p.Height), border);
            DrawRect(sb, new Rectangle(p.Right - 2, p.Y,          2,       p.Height), border);

            DrawText(sb, f.Name, new Vector2(p.X + 8, p.Y + 6), f.IsAlive ? f.Color : Color.Gray, 2);

            for (int i = 0; i < f.Lives; i++)
                DrawMiniStick(sb, new Vector2(p.X + 12 + i * 22, p.Y + 40), f.Color);

            string pct = $"{(int)f.DamagePercent}%";
            Color pctColor;

            if 
                (f.DamagePercent < 100) pctColor = Color.Lerp(Color.LightGreen, Color.Yellow, f.DamagePercent / 100f);
            else 
                pctColor = Color.Lerp(Color.Yellow, Color.OrangeRed, (f.DamagePercent - 100f) / 200f);

            DrawText(sb, pct, new Vector2(p.X + 8, p.Y + 50), pctColor, 2);

            if (!f.IsAlive)
                DrawText(sb, "KO", new Vector2(p.Right - 32, p.Y + 24), Color.Red, 2);
        }

        private void DrawMiniStick(SpriteBatch sb, Vector2 pos, Color c)
        {
            float x = pos.X, y = pos.Y;
            DrawRect(sb, new Rectangle((int)x - 4, (int)y - 16, 8, 8), c);
            DrawRect(sb, new Rectangle((int)x - 1, (int)y - 8,  2, 8), c);
            DrawRect(sb, new Rectangle((int)x - 4, (int)y,      4, 2), c);
            DrawRect(sb, new Rectangle((int)x,     (int)y,      4, 2), c);
        }
    }
}
