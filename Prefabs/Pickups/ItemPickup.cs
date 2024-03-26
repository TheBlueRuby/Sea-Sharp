using System;
using Godot;
using static Inventory;

public partial class ItemPickup : Collectible {
	// To compare against in player script
	[Export]
	public ItemTypes Type = ItemTypes.Test;

	private bool IsCollected = false;

	/// <summary>
	/// Altered version of Collect() to remove a race condition with 1-time pickups
	/// </summary>
	public new void Collect() {
		if (IsCollected) {
			return;
		}
		IsCollected = true;
		base.Collect();
	}
}
