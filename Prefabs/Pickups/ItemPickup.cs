using System;
using Godot;
using static Inventory;

public partial class ItemPickup : Collectible {
	// To compare against in player script
	[Export]
	public ItemTypes Type = ItemTypes.Test;

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
