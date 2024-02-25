using Godot;
using System;

public partial class Quit : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Pressed += Quit_Pressed;
	}

    private void Quit_Pressed()
    {
        this.GetTree().Quit();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
