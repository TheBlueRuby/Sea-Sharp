using Godot;
using System;

public partial class PauseHandler : Node {
	bool paused = false;
	bool uiVisible = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (Input.IsActionJustPressed("pause") && (paused == uiVisible)) {
			TogglePaused();
			SetUiVisible(paused);
		}
	}

	public void TogglePaused() {
		SetPaused(!paused);
	}

	public void SetPaused(bool newPaused) {
		if (uiVisible && !newPaused) {
			SetUiVisible(newPaused);
		}
		paused = newPaused;
		GetTree().Paused = paused;

	}

	public void SetUiVisible(bool visible) {
		GetTree().Root.GetNode<CanvasLayer>("GameLoop/PauseMenu").Visible = visible;
		uiVisible = visible;
	}
}
