using System;
using Godot;
using static Items;

public partial class Pickup : Collectible {
	// To compare against in player script
	[Export]
	public string Type = "test";
}
