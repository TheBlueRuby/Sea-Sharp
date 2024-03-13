using System;
using Godot;
using static Inventory;

public partial class ItemPickup : Collectible {
	// To compare against in player script
	[Export]
	public ItemTypes Type = ItemTypes.Test;
}
