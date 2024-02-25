using Godot;
using System;

public partial class Play : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Pressed += Play_Pressed;
	}

    private void Play_Pressed()
    {
        GetTree().ChangeSceneToFile("res://Level/Level.tscn");
        GD.Print("Hello?");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
