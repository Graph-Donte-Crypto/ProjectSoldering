using Godot;
using System;

public abstract partial class Material : Area2D
{
    public abstract string MaterilaName { get; }
	public abstract int MaterialCount { get; }
    // Called when the node enters the scene tree for the first time.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private bool active = true;
    public override void _Ready()
    {
        base._Ready();
        BodyEntered += Material_BodyEntered;
    }

    private void Material_BodyEntered(Node2D body)
    {
        if (active && body is Player player)
        {
            active = false;
            player.AddMaterial(MaterilaName, MaterialCount);
            GD.Print($"Player {MaterilaName} count is {player.GetMaterial(MaterilaName)}");
            var t1 = GetTree().CreateTween();
            var t2 = GetTree().CreateTween();
            t1.TweenProperty(this, "position", Position - new Vector2(0, 50), 0.5);
            t2.TweenProperty(this, "modulate:a", 0, 0.5);
            t1.TweenCallback(Callable.From(QueueFree));
        }
    }

    public override void _PhysicsProcess(double delta)
    {
    }
}
