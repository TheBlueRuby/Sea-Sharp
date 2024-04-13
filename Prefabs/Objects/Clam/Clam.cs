using System;
using Godot;

namespace SeaSharp {
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
			clamPearl ??= GD.Load<PackedScene>(Utils.Paths.Resources.DefaultPickup);
			texture = GetNode<Sprite2D>("Sprite2D");

			string path = ((Resource)GetScript()).ResourcePath.GetBaseDir() + "/";
			closedTexture = GD.Load<Texture2D>(path + "clam.png");
			openTexture = GD.Load<Texture2D>(path + "clam_open.png");
		}

		/// <summary>
		/// Spawns the "pearl" when the player gets close
		/// </summary>
		/// <param name="body">The player</param>
		private void OnPlayerEnter(Node2D body) {
			if (body is not Player) {
				return;
			}
			texture.Texture = openTexture;
			Collectible collectible = (Collectible)clamPearl.Instantiate();
			CallDeferred("add_child", collectible);

		}

		/// <summary>
		/// Closes the clam when the player gets out of range
		/// </summary>
		/// <param name="body">The player</param>
		private void OnPlayerExit(Node2D body) {
			if (body is not Player) {
				return;
			}
			texture.Texture = closedTexture;

			Node pickup = GetNodeOrNull("Pickup");
			pickup?.QueueFree();
		}

	}
}
