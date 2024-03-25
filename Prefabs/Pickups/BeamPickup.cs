using System;
using Godot;
using static Inventory;

public partial class BeamPickup : Collectible {
	// To compare against in player script
	[Export]
	public BeamTypes Type = BeamTypes.None;

	private bool IsCollected = false;

	public new void Collect() {
		if (IsCollected) {
			return;
		}
		IsCollected = true;
		base.Collect();
	}
}
