using Godot;
using Godot.Collections;
using System;

public partial class Player : CharacterBody2D
{
    public const float Speed = 50.0f;
    public const float LimitSpeed = 300f;
    public const float JumpVelocity = -120.0f;
    public float FadingJumpVelocity = 0;
    public int DefaultAmountOfJump = 3;
    public int AmountOfJump = 3;
    public bool JumpOver = true;
    public float d1Velocity = 0;

    public float SavedVelocity = 0;
    public float DashVelocity = 750;
    public float DefaultDashTime = 0.2f;
    public float TempDashTime = 0;
    public float DefaultDashTimeout = 1f;
    public float TempDashTimeout = 0;
    public AnimatedSprite2D AttackAnimation;

    public override void _Ready()
    {
        AttackAnimation = (AnimatedSprite2D)GetNode("MeeleAttack");
        AttackAnimation.AnimationFinished += AttackAnimation_AnimationFinished;
        base._Ready();
    }

    private void AttackAnimation_AnimationFinished()
    {
        AttackAnimation.Stop();
    }

    Dictionary<string, int> Materials { get; set; } = new Dictionary<string, int>();
    public void AddMaterial(string key, int value)
    {
        if (Materials.TryGetValue(key, out int v))
            Materials[key] = value + v;
        else
            Materials.Add(key, value);
    }
    public int GetMaterial(string key) => Materials.TryGetValue(key, out int v) ? v : 0;


    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public void ReloadJumpAmount()
    {
        AmountOfJump = DefaultAmountOfJump;
    }
    public void DoStartJump(ref Vector2 velocity)
    {
        if (AmountOfJump > 0)
        {
            JumpOver = false;
            FadingJumpVelocity = JumpVelocity;
            velocity.Y = FadingJumpVelocity;
            AmountOfJump -= 1;
        }
    }
    public void DoContinueJump(ref Vector2 velocity)
    {
        if (MathF.Abs(FadingJumpVelocity) > 1f)
        {
            FadingJumpVelocity /= 1.25f;
            velocity.Y += FadingJumpVelocity;
        }
    }
    public void JumpProcess(ref Vector2 velocity, double delta)
    {
        bool onFloor = IsOnFloor();
        bool jumpPressed = Input.IsActionJustPressed("ui_accept");
        bool jumpPressing = Input.IsActionPressed("ui_accept");
        if (Input.IsActionJustReleased("ui_accept"))
            JumpOver = true;

        if (!onFloor)
        {
            velocity.Y += gravity * (float)delta;
            if (jumpPressed)
                DoStartJump(ref velocity);
            else if (jumpPressing && !JumpOver)
                DoContinueJump(ref velocity);
        }
        else
        {
            ReloadJumpAmount();
            if (jumpPressed)
                DoStartJump(ref velocity);
        }

    }

    public void DashProcess(ref Vector2 velocity, Vector2 vector, double delta)
    {
        if (Input.IsKeyPressed(Key.Shift) && vector.X != 0 && TempDashTimeout == 0)
        {
            GD.Print("Dash Start");
            SavedVelocity = velocity.X;

            velocity.X = DashVelocity * vector.X;
            velocity.Y = 0;

            TempDashTime = DefaultDashTime;
            TempDashTimeout = DefaultDashTimeout;
        }

        if (TempDashTime != 0 && TempDashTime - (float)delta <= 0)
        {
            velocity.X = SavedVelocity;
            GD.Print("Dash Was");
        }
        if (TempDashTimeout != 0 && TempDashTimeout - (float)delta <= 0)
            GD.Print("Dash Reload");

        TempDashTime = Mathf.MoveToward(TempDashTime, 0, (float)delta);
        TempDashTimeout = Mathf.MoveToward(TempDashTimeout, 0, (float)delta);

    }

    public void AttackProcess()
    {
        if (Input.IsKeyPressed(Key.F))
        {
            AttackAnimation.Play();
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        AttackProcess();

        Vector2 velocity = Velocity;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        //Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");



        if (direction.X > 0)
            AttackAnimation.FlipH = false;
        else if (direction.X < 0)
            AttackAnimation.FlipH = true;

        DashProcess(ref velocity, direction, delta);

        if (TempDashTime == 0)
        {
            JumpProcess(ref velocity, delta);

            if (direction != Vector2.Zero)
            {
                velocity.X += direction.X * Speed;
                if (MathF.Abs(velocity.X) > LimitSpeed)
                    velocity.X = direction.X * LimitSpeed;
                d1Velocity = velocity.X;
            }
            else
            {
                if (Math.Abs(d1Velocity) > 1f)
                {
                    d1Velocity *= 0.92f;
                    velocity.X = d1Velocity;
                }
                //velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            }
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
