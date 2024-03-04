using System;
using Godot;
using static Inventory;

public partial class Collectible : Node2D {
	public GpuParticles2D particles;
	public Sprite2D sprite;

	public override void _Ready() {
		// Load the particle effect
		particles = GetNode<GpuParticles2D>("PickupParticles");
		sprite = GetNode<Sprite2D>("Sprite2D");

	}

	public async void Collect() {
		// Hide the texture so particles are visible
		sprite.Visible = false;

		// Spawn a set of particles
		particles.Emitting = true;

		// Waits until particles are done emitting
		await ToSignal(GetTree().CreateTimer(3), "timeout");

		// Delete the object
		QueueFree();
	}
}
