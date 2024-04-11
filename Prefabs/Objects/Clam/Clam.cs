using System;
using Godot;

public partial class Clam : Node2D {
	[Export]
	private PackedScene clamPearl;
	private Sprite2D texture;

	private Texture2D closedTexture;
	private Texture2D openTexture;
	
	/// <summary>
	/// Initialization function
	/// </summary>
	public override void _Ready() {
		clamPearl ??= GD.Load<PackedScene>("res://Prefabs/Pickups/PickupBase.tscn");
		texture = GetNode<Sprite2D>("Sprite2D");
		closedTexture = GD.Load<Texture2D>("res://Prefabs/Objects/Clam/clam.png");
		openTexture = GD.Load<Texture2D>("res://Prefabs/Objects/Clam/clam_open.png");
	}
	private void OnPlayerEnter(Node2D body) {
		texture.Texture = openTexture;
		Collectible collectible = (Collectible)clamPearl.Instantiate();
		CallDeferred("add_child", collectible);

	}


	private void OnPlayerExit(Node2D body) {
		texture.Texture = closedTexture;

		Node pickup = GetNodeOrNull("Pickup");
		pickup?.QueueFree();
	}

}
