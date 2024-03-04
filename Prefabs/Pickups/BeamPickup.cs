using System;
using Godot;
using static Inventory;

public partial class BeamPickup : Collectible {
	// To compare against in player script
	[Export]
	public BeamTypes Type = BeamTypes.None;
}
