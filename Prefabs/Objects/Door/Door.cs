using System;
using Godot;

public partial class Door : Node2D {
	private StaticBody2D doorCollision;
	private Sprite2D doorSprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		doorCollision = GetNode<StaticBody2D>("Collision");
		doorSprite = GetNode<Sprite2D>("Collision/Sprite");
	}

	private void OnPlayerActivate(Node2D body) {
		// Disable collision and hide door
		doorCollision.ProcessMode = ProcessModeEnum.Disabled;
		doorSprite.SetDeferred("visible", false);
	}

	private async void OnPlayerDeactivate(Node2D body) {
		// Enable collision and hide door after 2 seconds
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		doorCollision.ProcessMode = ProcessModeEnum.Always;
		doorSprite.SetDeferred("visible", true);
	}

}
