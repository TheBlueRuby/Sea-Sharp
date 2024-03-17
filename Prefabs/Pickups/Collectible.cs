using System;
using Godot;
using static Inventory;

public partial class Collectible : Node2D {
	public GpuParticles2D particles;
	public Sprite2D sprite;

	private Node MetSys;
	private Node MetSysCompat;

	public override void _Ready() {
		// Load the particle effect
		particles = GetNode<GpuParticles2D>("PickupParticles");
		sprite = GetNode<Sprite2D>("Sprite2D");

		// Initialise the MetSys pointer
		MetSys = GetTree().Root.GetNode<Node>("MetSys");
		MetSysCompat = GetTree().Root.GetNode<Node>("MetSysCompat");

		// Set object owner for MetSys
		Owner = GetTree().Root.GetNode("GameLoop/Map");

		// Register as a MetSys object.
		// If the object has already been registered, delete.
		if ((bool)MetSysCompat.Call("register_obj_marker", this)) {
			QueueFree();
		}

	}

	public async void Collect() {
		// Store MetSys Object
		MetSysCompat.Call("store_obj", this);

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
