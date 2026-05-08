using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StickmanFighter.Input;

namespace StickmanFighter.Entities
{
    public class Fighter : Entity
    {
        public override int Width  => 28;
        public override int Height => 70;

        public string Name          { get; }
        public Color  Color         { get; }
        public int    Lives         { get; private set; }
        public bool   IsAlive       { get; private set; } = true;
        public float  DamagePercent { get; private set; }

        public static Color[] PlayerColors = { Color.Tomato, Color.LimeGreen, Color.CornflowerBlue, Color.Gold };

        private const float MoveSpeed  = 340f;
        private const float Jump  = -750f;
        private const float AirControl = 0.8f;

        private int   jumpsLeft  = 2;
        private float prevFeetY  = 9999f;

        private const float MaxCharge      = 1.2f;
        private const float AttackDuration = 0.22f;
        private const float KnockbackTime  = 0.15f;
        private const float IFrameTime     = 0.5f;

        private bool  isCharging    = false;
        private float chargeTimer   = 0f;
        private bool  isAttackActive  = false;
        private float attackTimer   = 0f;
        private float firedCharge   = 0f;
        private bool  hitDelivered  = false;
        private float knockbackTimer = 0f;
        private float iFrameTimer   = 0f;
        private float hitBlinkTimer      = 0f;

        public bool  IsBlocking  { get; private set; }
        public float ChargeRatio => Math.Clamp(chargeTimer / MaxCharge, 0f, 1f);

        private float facing    = 1f;
        private float walkCycle = 0f;
        private bool  isWalking = false;

        private PlayerInput input;

        public Fighter(string name, Color color, Vector2 startPos, PlayerInput input, int lives = 3)
            : base(startPos)
        {
            Name       = name;
            Color      = color;
            Lives      = lives;
            prevFeetY  = startPos.Y;
            this.input = input;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsAlive) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            input.Update();
            TickTimers(dt);
            Move(dt);
            HandleAttack(dt);
            IsBlocking = input.BlockHeld && IsOnGround && !isCharging && !isAttackActive;

            if (isWalking) walkCycle += dt * 8f;
        }

        private void TickTimers(float dt)
        {
            if (iFrameTimer    > 0) iFrameTimer    -= dt;
            if (knockbackTimer > 0) knockbackTimer -= dt;
            if (hitBlinkTimer > 0) hitBlinkTimer -= dt;
            if (attackTimer    > 0)
            {
                attackTimer -= dt;
                if (attackTimer <= 0) { isAttackActive = false; hitDelivered = false; }
            }
        }

        private void Move(float dt)
        {
            float vx = Velocity.X;

            if (knockbackTimer > 0)
            {
                if (input.MoveLeft)  facing = -1f;
                if (input.MoveRight) facing  =  1f;
                isWalking = false;
            }
            else if (IsOnGround)
            {
                if (input.MoveLeft)  { 
                    vx = -MoveSpeed; facing = -1f; isWalking = true; 
                }
                else if (input.MoveRight) { 
                    vx =  MoveSpeed; facing =  1f; isWalking = true; 
                }
                else {
                    vx *= 0.70f; isWalking = false; 
                }
            }
            else
            {
                if (input.MoveLeft)  {
                    vx = -MoveSpeed * AirControl; facing = -1f; 
                }
                else if (input.MoveRight) {
                    vx =  MoveSpeed * AirControl; facing =  1f; 
                }
                else {
                    vx *= 0.96f; 
                }
                isWalking = false;
            }

            Velocity = new Vector2(vx, Velocity.Y);

            if (input.JumpPressed && jumpsLeft > 0)
            {
                Velocity   = new Vector2(Velocity.X, Jump);
                jumpsLeft--;
                IsOnGround = false;
            }

            ApplyGravity(dt);
            prevFeetY  = Position.Y;
            Position  += Velocity * dt;
        }

        private void HandleAttack(float dt)
        {
            if (input.AttackPressed && !IsBlocking && !isAttackActive)
            {
                isCharging  = true;
                chargeTimer = 0f;
            }

            if (isCharging && input.AttackHeld)
                chargeTimer = Math.Min(chargeTimer + dt, MaxCharge);

            if (isCharging && input.AttackReleased)
            {
                firedCharge  = ChargeRatio;
                isCharging   = false;
                chargeTimer  = 0f;
                isAttackActive = true;
                attackTimer  = AttackDuration;
                hitDelivered = false;
            }
        }

        public void ApplyMapCollision(Map.Map map)
        {
            var pos = Position;
            var vel = Velocity;

            bool grounded = map.FixCollisions(ref pos, ref vel, Width, Height, prevFeetY);

            if (grounded && !IsOnGround) jumpsLeft = 2;

            IsOnGround = grounded;
            Position   = pos;
            Velocity   = vel;

            if (map.IsInVoid(Position)) FallIntoVoid();
        }

        public Rectangle? GetAttackHitbox()
        {
            if (!isAttackActive || hitDelivered) return null;

            int hx = facing > 0
                ? (int)Position.X + Width / 2
                : (int)Position.X - Width / 2 - 65;

            return new Rectangle(hx, (int)Position.Y - Height + 5, 65, Height - 10);
        }

        public float ConsumeHit()
        {
            hitDelivered = true;
            return firedCharge;
        }

        public void TakeHit(float charge, Vector2 attackerPos)
        {
            if (iFrameTimer > 0) return;

            float damage    = Lerp(6f, 38f, charge);
            float knockback = Lerp(350f, 1500f, charge) * (1f + DamagePercent * 0.007f);
            float dir       = Position.X >= attackerPos.X ? 1f : -1f;
            float upward    = Lerp(0.12f, 0.22f, charge);

            if (IsBlocking)
            {
                Velocity   = new Vector2(dir * knockback * 0.15f, -60f);
                hitBlinkTimer = 0.08f;
                IsOnGround = false;
                return;
            }

            DamagePercent  = Math.Min(DamagePercent + damage, 300f);
            Velocity       = new Vector2(dir * knockback, -knockback * upward);
            iFrameTimer    = IFrameTime;
            knockbackTimer = KnockbackTime;
            hitBlinkTimer = 0.18f;
            IsOnGround     = false;
        }

        private void FallIntoVoid()
        {
            Lives--;
            DamagePercent = 0f;
            iFrameTimer   = 2.5f;

            if (Lives <= 0)
            {
                IsAlive = false;
            }
            else
            {
                Position   = new Vector2(640, 150);
                Velocity   = Vector2.Zero;
                IsOnGround = false;
                prevFeetY  = 150f;
            }
        }

        //
        //Draw the fighter, partially AI written.
        //

        public override void Draw(SpriteBatch sb)
        {
            if (!IsAlive) return;

            bool  flash = hitBlinkTimer > 0f && (int)(hitBlinkTimer * 30) % 2 == 0;
            Color c     = flash ? Color.White : Color;
            Color dark  = Color.Lerp(c, Color.Black, 0.5f);

            float swing = isWalking ? MathF.Sin(walkCycle) * 12f : 0f;
            float px = Position.X;
            float py = Position.Y;

            var footL     = new Vector2(px - 7,  py);
            var footR     = new Vector2(px + 7,  py);
            var kneeL     = new Vector2(px - 7 - swing, py - 20);
            var kneeR     = new Vector2(px + 7 + swing, py - 20);
            var hips      = new Vector2(px,      py - 36);
            var shoulders = new Vector2(px,      py - 54);
            var head      = new Vector2(px,      py - 65);

            DrawLine(sb, footL, kneeL, c, 3);
            DrawLine(sb, kneeL, hips,  c, 3);
            DrawLine(sb, footR, kneeR, c, 3);
            DrawLine(sb, kneeR, hips,  c, 3);
            DrawLine(sb, hips, shoulders, c, 4);

            DrawArms(sb, shoulders, c, swing);
            DrawHead(sb, head, c, dark);

            if (IsBlocking) DrawShield(sb, px, py);
            if (isCharging && chargeTimer > 0.05f) DrawChargeBar(sb, head);
        }

        private void DrawArms(SpriteBatch sb, Vector2 sh, Color c, float swing)
        {
            if (isAttackActive)
            {
                DrawLine(sb, sh, new Vector2(sh.X + facing * 20, sh.Y + 6),  c, 3);
                DrawLine(sb, new Vector2(sh.X + facing * 20, sh.Y + 6), new Vector2(sh.X + facing * 44, sh.Y + 2), c, 3);
                DrawLine(sb, sh, new Vector2(sh.X - facing * 12, sh.Y + 14), c, 3);
                DrawLine(sb, new Vector2(sh.X - facing * 12, sh.Y + 14), new Vector2(sh.X - facing * 20, sh.Y + 26), c, 3);
            }
            else if (isCharging)
            {
                float raise = ChargeRatio * 18f;
                DrawLine(sb, sh, new Vector2(sh.X - 20, sh.Y - raise), c, 3);
                DrawLine(sb, new Vector2(sh.X - 20, sh.Y - raise), new Vector2(sh.X - 28, sh.Y - 8 - raise), c, 3);
                DrawLine(sb, sh, new Vector2(sh.X + 20, sh.Y - raise), c, 3);
                DrawLine(sb, new Vector2(sh.X + 20, sh.Y - raise), new Vector2(sh.X + 28, sh.Y - 8 - raise), c, 3);
            }
            else if (IsBlocking)
            {
                DrawLine(sb, sh, new Vector2(sh.X + facing * 12, sh.Y + 6),  c, 3);
                DrawLine(sb, new Vector2(sh.X + facing * 12, sh.Y + 6), new Vector2(sh.X + facing * 30, sh.Y), c, 3);
                DrawLine(sb, sh, new Vector2(sh.X + facing * 14, sh.Y + 16), c, 3);
                DrawLine(sb, new Vector2(sh.X + facing * 14, sh.Y + 16), new Vector2(sh.X + facing * 30, sh.Y + 12), c, 3);
            }
            else
            {
                DrawLine(sb, sh, new Vector2(sh.X - 18 + swing, sh.Y + 13), c, 3);
                DrawLine(sb, new Vector2(sh.X - 18 + swing, sh.Y + 13), new Vector2(sh.X - 26 + swing * 1.4f, sh.Y + 27), c, 3);
                DrawLine(sb, sh, new Vector2(sh.X + 18 - swing, sh.Y + 13), c, 3);
                DrawLine(sb, new Vector2(sh.X + 18 - swing, sh.Y + 13), new Vector2(sh.X + 26 - swing * 1.4f, sh.Y + 27), c, 3);
            }
        }

        private void DrawHead(SpriteBatch sb, Vector2 pos, Color c, Color dark)
        {
            int r = 10;
            DrawRect(sb, new Rectangle((int)pos.X - r,     (int)pos.Y - r, r * 2,     r * 2), c);
            DrawRect(sb, new Rectangle((int)pos.X - r + 2, (int)pos.Y - r - 2, r * 2 - 4, 4), c);
            DrawRect(sb, new Rectangle((int)pos.X - r + 2, (int)pos.Y + r - 1, r * 2 - 4, 3), c);
            DrawRect(sb, new Rectangle((int)(pos.X + facing * 2 - 4), (int)pos.Y - 2, 3, 3), dark);
            DrawRect(sb, new Rectangle((int)(pos.X + facing * 2 + 2), (int)pos.Y - 2, 3, 3), dark);
        }

        private void DrawShield(SpriteBatch sb, float px, float py)
        {
            var r = new Rectangle((int)(px + facing * 14) - 8, (int)py - 58, 16, 50);
            DrawRect(sb, r, Color.LightSkyBlue * 0.7f);
            DrawRect(sb, new Rectangle(r.X, r.Y, 2, r.Height), Color.White * 0.85f);
        }

        private void DrawChargeBar(SpriteBatch sb, Vector2 head)
        {
            DrawRect(sb, new Rectangle((int)head.X - 21, (int)head.Y - 26, 42, 6), Color.Black * 0.6f);
            int w = (int)(42 * ChargeRatio);
            if (w > 0)
                DrawRect(sb, new Rectangle((int)head.X - 21, (int)head.Y - 26, w, 6),
                    Color.Lerp(Color.Yellow, Color.OrangeRed, ChargeRatio));
        }

        private static float Lerp(float a, float b, float t) => a + (b - a) * t;
    }
}
