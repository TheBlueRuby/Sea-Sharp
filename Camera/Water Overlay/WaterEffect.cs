using System;
using Godot;

public partial class WaterEffect : Node2D {
	// Layers of water for the parallax effect.
	Sprite2D layer1;
	Sprite2D layer2;
	private const int maxDist = 64;
	private const float speed = 0.2f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		layer1 = GetNode<Sprite2D>("Layer1");
		layer2 = GetNode<Sprite2D>("Layer2");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		Vector2 pos1 = layer1.Position;
		Vector2 pos2 = layer2.Position;

		// Move layer1 up-right
		pos1.X += speed;
		pos1.Y -= speed;

		// Move layer2 down-right
		pos2.X += speed;
		pos2.Y += speed;

		// If layers are out of the screen, reset their position
		if (pos1.X > maxDist) {
			pos1.X -= maxDist;
			pos1.Y += maxDist;
		}
		if (pos2.X > maxDist) {
			pos2.X -= maxDist;
			pos2.Y -= maxDist;
		}

		layer1.Position = pos1;
		layer2.Position = pos2;

	}
}
