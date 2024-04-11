using Godot;
using System;

namespace SeaSharp {
	public partial class Teleporter : Area2D {
		[Export]
		public string roomToGoTo { get; set; }

		private Node MetSysCompat;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready() {
			MetSysCompat = GetTree().Root.GetNode<Node>("MetSysCompat");
		}

		/// <summary>
		/// Teleports player to center of specified room
		/// </summary>
		/// <param name="body">The player that enters the area</param>
		private void OnPlayerEnter(Node2D body) {
			if (body is Player player) {
				player.GlobalPosition = new Vector2(160, 90); //Center of room
				MetSysCompat.Call("load_room", roomToGoTo);
			}
		}
	}
}
