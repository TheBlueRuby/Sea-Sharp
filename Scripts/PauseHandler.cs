using Godot;
using System;

public partial class PauseHandler : Node
{
	bool paused = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("pause")) {
			paused = !paused;
			GetTree().Paused = paused;
		}
	}
}
