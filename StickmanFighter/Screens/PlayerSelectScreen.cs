using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StickmanFighter.Entities;
using StickmanFighter.Input;

namespace StickmanFighter.Screens
{
    public class PlayerSelectScreen : Screen
    {
        private int playerCount = 2;
        private bool[] useGamepad  = { false, false, false, false };
        private MenuInput input  = new();

        private PlayerIndex[] padIndex = { PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four };

        public PlayerSelectScreen(Game1 game) : base(game)
        {
            var connected = new List<PlayerIndex>();
            foreach (PlayerIndex indx in Enum.GetValues(typeof(PlayerIndex)))
                if (GamepadInput.IsConnected(indx))
                    connected.Add(indx);

            for (int i =  0; i < Math.Min(connected.Count, 4); i++)
            {
                padIndex[i]  = connected[i];
                useGamepad[i] = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            input.Update();

            if (input.Left)    playerCount = Math.Max(2, playerCount - 1);
            if (input.Right)   playerCount = Math.Min(4, playerCount + 1);
            if (input.Confirm) StartGame();
            if (input.Back)    Game.GoTo(new MainMenuScreen(Game));

            var keys = Keyboard.GetState();
            for (int i = 0; i < 4; i++)
                if (keys.IsKeyDown(Keys.D1 + i) && GamepadInput.IsConnected(padIndex[i]))
                    useGamepad[i] = !useGamepad[i];
        }

        private void StartGame()
        {
            var inputs = new PlayerInput[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                bool padOk = useGamepad[i] && GamepadInput.IsConnected(padIndex[i]);
                inputs[i] = padOk
                    ? (PlayerInput)new GamepadInput(padIndex[i])
                    : new KeyboardInput(InputScheme.All[i]);
            }
            Game.GoTo(new GameScreen(Game, playerCount, inputs));
        }

        public override void Draw(SpriteBatch sb)
        {
            int centerX = Game1.ScreenWidth  / 2;
            int centerY = Game1.ScreenHeight / 2;

            DrawRect(sb, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), new Color(12, 14, 28));

            DrawTextCentered(sb, "STICKMAN FIGHTER", centerX, 50,  Color.Gold, 4);
            DrawTextCentered(sb, "SELECT PLAYERS", centerX, 118, Color.White, 3);

            DrawTextCentered(sb, $"< {playerCount} >", centerX, 200, Color.Gold, 6);
            DrawTextCentered(sb, "Left/Right or Q/E to change", centerX, 268, Color.LightSteelBlue, 1);

            DrawCards(sb, centerX, centerY);

            DrawTextCentered(sb, "Cross/Enter = start      Circle/Esc = back",
                centerX, Game1.ScreenHeight - 50, Color.White, 2);
            DrawTextCentered(sb, "Dpad or Q/E to change count    Press 1/2/3/4 to toggle gamepad per slot",
                centerX, Game1.ScreenHeight - 28, Color.LightSteelBlue, 1);
        }

        private void DrawCards(SpriteBatch sb, int cx, int cy)
        {
            string[] kbNames = { "WASD", "ARROWS", "IJKL", "NUM" };

            for (int i = 0; i < 4; i++)
            {
                bool  active   = i < playerCount;
                Color c        = active ? Fighter.PlayerColors[i] : Color.DimGray;
                int   px       = cx - 285 + i * 190;
                int   py       = cy + 80;
                bool  padReady = GamepadInput.IsConnected((PlayerIndex)i);
                bool  usingPad = useGamepad[i] && padReady;

                DrawRect(sb, new Rectangle(px - 44, py - 90, 88, 140), Color.Black * 0.4f);

                if (active)
                {
                    DrawRect(sb, new Rectangle(px - 44, py - 90, 88,  2), c);
                    DrawRect(sb, new Rectangle(px - 44, py + 50, 88,  2), c);
                    DrawRect(sb, new Rectangle(px - 44, py - 90,  2, 140), c);
                    DrawRect(sb, new Rectangle(px + 42,  py - 90,  2, 140), c);
                }

                DrawMiniStick(sb, new Vector2(px, py - 10), c);
                DrawTextCentered(sb, $"P{i + 1}", px, py + 22, c, 2);

                if (active)
                {
                    string inputLabel = usingPad ? "GAMEPAD" : kbNames[i];
                    DrawTextCentered(sb, inputLabel, px, py + 38, usingPad ? Color.LightGreen : Color.LightGray, 1);

                    if (padReady && !usingPad)
                        DrawTextCentered(sb, $"pad {(int)padIndex[i] + 1} avail", px, py + 52, Color.DarkGreen, 1);
                }
            }
        }

        private void DrawMiniStick(SpriteBatch sb, Vector2 pos, Color c)
        {
            float x = pos.X, y = pos.Y;
            DrawRect(sb, new Rectangle((int)x - 8,  (int)y - 68, 16, 16), c);
            DrawRect(sb, new Rectangle((int)x - 2,  (int)y - 52,  4, 24), c);
            DrawRect(sb, new Rectangle((int)x - 10, (int)y - 28,  4, 16), c);
            DrawRect(sb, new Rectangle((int)x + 6,  (int)y - 28,  4, 16), c);
            DrawRect(sb, new Rectangle((int)x - 14, (int)y - 44, 10,  4), c);
            DrawRect(sb, new Rectangle((int)x + 4,  (int)y - 44, 10,  4), c);
        }
    }
}
