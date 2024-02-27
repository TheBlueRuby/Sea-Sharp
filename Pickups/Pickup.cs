using System;
using Godot;

public partial class Pickup : Node2D {
	
	// To compare against in player script
	[Export]
	public string pickupType = "test";

	public void Collect() {
		// Builtin Godot method that queues the node for deletion
		QueueFree();
	}
}
