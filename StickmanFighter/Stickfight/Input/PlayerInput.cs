using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace StickmanFighter.Input
{
    public abstract class PlayerInput
    {
        public abstract void Update();
        public abstract bool MoveLeft { get; }
        public abstract bool MoveRight { get; }
        public abstract bool JumpPressed { get; }
        public abstract bool AttackPressed { get; }
        public abstract bool AttackHeld { get; }
        public abstract bool AttackReleased { get; }
        public abstract bool BlockHeld { get; }
    }

    public class KeyboardPlayerInput : PlayerInput
    {
        private InputScheme scheme;
        private KeyboardState cur, prev;

        public KeyboardPlayerInput(InputScheme scheme) => this.scheme = scheme;

        public override void Update()
        {
            prev = cur;
            cur = Keyboard.GetState();
        }

        public override bool MoveLeft => cur.IsKeyDown(scheme.Left);
        public override bool MoveRight => cur.IsKeyDown(scheme.Right);
        public override bool JumpPressed => cur.IsKeyDown(scheme.Jump) && !prev.IsKeyDown(scheme.Jump);
        public override bool AttackPressed => cur.IsKeyDown(scheme.Attack) && !prev.IsKeyDown(scheme.Attack);
        public override bool AttackHeld => cur.IsKeyDown(scheme.Attack);
        public override bool AttackReleased => !cur.IsKeyDown(scheme.Attack) && prev.IsKeyDown(scheme.Attack);
        public override bool BlockHeld => cur.IsKeyDown(scheme.Block);
    }

    public class GamepadInput : PlayerInput
    {
        private PlayerIndex index;
        private GamePadState cur, prev;

        private const float DeadZone = 0.3f;
        private const float JumpTilt = 0.6f;

        public GamepadInput(PlayerIndex index) => this.index = index;

        public override void Update()
        {
            prev = cur;
            cur = GamePad.GetState(index);
        }

        public override bool MoveLeft => cur.ThumbSticks.Left.X < -DeadZone || cur.IsButtonDown(Buttons.DPadLeft);
        public override bool MoveRight => cur.ThumbSticks.Left.X > DeadZone || cur.IsButtonDown(Buttons.DPadRight);

        public override bool JumpPressed =>
            Pressed(Buttons.A) ||
            Pressed(Buttons.DPadUp) ||
            (cur.ThumbSticks.Left.Y > JumpTilt && prev.ThumbSticks.Left.Y <= JumpTilt);

        public override bool AttackPressed => Pressed(Buttons.B);
        public override bool AttackHeld => cur.IsButtonDown(Buttons.B);
        public override bool AttackReleased => !cur.IsButtonDown(Buttons.B) && prev.IsButtonDown(Buttons.B);
        public override bool BlockHeld => cur.IsButtonDown(Buttons.X);

        private bool Pressed(Buttons btn) => cur.IsButtonDown(btn) && !prev.IsButtonDown(btn);

        public static bool IsConnected(PlayerIndex index) => GamePad.GetState(index).IsConnected;
    }
}
