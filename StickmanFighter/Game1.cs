using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using StickmanFighter.Screens;

namespace StickmanFighter
{
    public class Game1 : Game
    {
        public const int ScreenWidth = 1280;
        public const int ScreenHeight = 720;

        public static Texture2D Pixel = null!;

        private GraphicsDeviceManager graphics;
        private SpriteBatch sb = null!;
        private Screen currentScreen = null!;
        private KeyboardState prevKeys;


        public static SoundEffect HitSound = null!;
        public static SoundEffect MissSound = null!;
        public static SoundEffect DeadSound = null!;
        public static SoundEffect SelectSound = null!;

        //Set the default volumes for both soundeffects and backgroundmusic.
        public float SfxVolume = 0.5f;
        public float MusicVolume = 0.5f;

        Song backgroundMusic = null!;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            Content.RootDirectory = "Content";
            Window.Title = "Stickman Fighter"; // Sets the tab name
        }

        protected override void Initialize()
        {
            SoundEffect.MasterVolume = SfxVolume;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            sb = new SpriteBatch(GraphicsDevice);

            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });


            HitSound = Content.Load<SoundEffect>("soft-punch1");
            MissSound = Content.Load<SoundEffect>("airhit");
            DeadSound = Content.Load<SoundEffect>("death");
            SelectSound = Content.Load<SoundEffect>("select-soundeffect");

            backgroundMusic = Content.Load<Song>("backgroundMusic");
            MediaPlayer.Volume = MusicVolume;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);

            GoTo(new MainMenuScreen(this));
        }

        protected override void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState();

            if (keys.IsKeyDown(Keys.F1) && !prevKeys.IsKeyDown(Keys.F1))
                GoTo(new DebugScreen(this, currentScreen));

            prevKeys = keys;
            currentScreen.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            sb.Begin();
            currentScreen.Draw(sb);
            sb.End();
            base.Draw(gameTime);
        }

        public void GoTo(Screen screen)
        {
            currentScreen = screen;
        }
    }
}

