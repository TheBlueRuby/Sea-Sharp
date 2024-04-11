using System;
using Godot;
using static Inventory;

public partial class BeamPickup : Collectible {
	// To compare against in player script
	[Export]
	public BeamTypes Type { get; set; } = BeamTypes.None;

	private bool IsCollected = false;

	public override void _Ready() {
		PickupType = Type.ToString();
		base._Ready();
	}


	/// <summary>
	/// Altered version of Collect() to remove a race condition with 1-time pickups
	/// </summary>
	public override void Collect() {
		if (IsCollected) {
			return;
		}
		IsCollected = true;
		base.Collect();
	}
}
