using System;
using Godot;
using static SeaSharp.Inventory;

namespace SeaSharp {
	public partial class Collectible : Node2D {
		private GpuParticles2D particles;
		private Sprite2D sprite;

		private Node MetSys;
		private Node MetSysCompat;

		private PackedScene pickupText;

		public string PickupType { get; set; } = "Test";

		/// <summary>
		/// Initialization function
		/// </summary>
		public override void _Ready() {
			// Load the particle effect
			particles = GetNode<GpuParticles2D>("PickupParticles");
			sprite = GetNode<Sprite2D>("Sprite2D");

			// Initialise the MetSys pointer
			MetSys = GetTree().Root.GetNode<Node>(Utils.Paths.SceneTree.MetSys);
			MetSysCompat = GetTree().Root.GetNode<Node>(Utils.Paths.SceneTree.MetSysCompat);

			// Set object owner for MetSys
			Owner = GetTree().Root.GetNode(Utils.Paths.SceneTree.Map);

			// Register as a MetSys object.
			// If the object has already been registered, delete.
			if ((bool)MetSysCompat.Call("register_obj_marker", this)) {
				QueueFree();
			}
			pickupText = GD.Load<PackedScene>(Utils.Paths.Resources.Menus + "DialogPopup.tscn");

		}

		/// <summary>
		/// Collects the object when the player touches it.
		/// Also spawns particles and logs collection in MetSys 
		/// </summary>
		public virtual async void Collect() {
			// Hide the texture so particles are visible
			sprite.Visible = false;

			// Spawn a set of particles
			particles.Emitting = true;

			// Store MetSys Object
			await ToSignal(GetTree().CreateTimer(0.5), "timeout");
			MetSysCompat.Call("store_obj", this);

			DialogPopup pickupTextInstance = pickupText.Instantiate<DialogPopup>();
			pickupTextInstance.StringType = "items";
			pickupTextInstance.StringId = PickupType.ToString();
			GetTree().Root.GetNode(Utils.Paths.SceneTree.HUD).AddChild(pickupTextInstance);
			GetTree().Root.GetNode<PauseHandler>(Utils.Paths.SceneTree.PauseHandler).SetPaused(true);

			// Waits until particles are done emitting
			await ToSignal(GetTree().CreateTimer(3), "timeout");

			// Delete the object
			QueueFree();
		}
	}
}
