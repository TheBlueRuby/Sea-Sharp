using System;
using Godot;

public partial class Clam : Node2D {
	[Export]
	private PackedScene clamPearl;
	private Sprite2D texture;
	private bool isPearlCollected;

	private Texture2D closedTexture;
	private Texture2D openTexture;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		clamPearl ??= GD.Load<PackedScene>("res://Prefabs/Pickups/PickupBase.tscn");
		texture = GetNode<Sprite2D>("Sprite2D");
		closedTexture = GD.Load<Texture2D>("res://Prefabs/Objects/Clam/clam.png");
		openTexture = GD.Load<Texture2D>("res://Prefabs/Objects/Clam/clam_open.png");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	private void OnPlayerEnter(Node2D body) {
		texture.Texture = openTexture;
		if (!isPearlCollected) {
			Collectible collectible = (Collectible)clamPearl.Instantiate();
			CallDeferred("add_child", collectible);
		}
	}

	private void OnPlayerCollect(Node2D body) {
		isPearlCollected = true;
	}


	private void OnPlayerExit(Node2D body) {
		texture.Texture = closedTexture;

		Node pickup = GetNodeOrNull("Pickup");
		pickup?.QueueFree();
	}

}