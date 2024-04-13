using System;
using Godot;
using static SeaSharp.Inventory;
namespace SeaSharp {
	public partial class Pickup : Collectible {
		// To compare against in player script
		[Export]
		public string Type { get; set; } = "test";
	}
}
