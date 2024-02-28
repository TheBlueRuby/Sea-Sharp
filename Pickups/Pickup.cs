using System;
using Godot;

public partial class Pickup : Node2D {

	// To compare against in player script
	[Export]
	public string pickupType = "test";

	public PickupParticles particles;
	public Sprite2D sprite;

	public override void _Ready() {
		// Load the particle effect
		particles = GetNode<PickupParticles>("PickupParticles");
		sprite = GetNode<Sprite2D>("Sprite2D");

	}

	public void Collect() {
		// Hide the texture so particles are visible
		sprite.Visible = false;

		// Spawn a set of particles
		particles.Burst();

		// Delete the object
		QueueFree();
	}
}
