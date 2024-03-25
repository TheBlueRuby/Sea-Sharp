using System;

/// <summary>
/// A class that represents an inventory system for the player
/// </summary>
public class Inventory {
	/// <summary>Represents the types of beams that a player can have.</summary>
	public enum BeamTypes {
		None = 0,
		BubbleBeam = 1,
		PressureBeam = 2,
		IceBeam = 4,
		HeatBeam = 8,
	}

	/// <summary>Represents the types of items that a player can have.</summary>
	public enum ItemTypes {
		Test = 0,
		AnglerCap = 1,
		Flippers = 2,
		Propeller = 4,
		PressureSuit = 8,
	}

	/// <summary>Currently equipped beam.</summary>
	public BeamTypes ActiveBeam { get; set; }

	/// <summary>Currently equipped items.</summary>
	public BitArray ActiveItems { get; set; }

	/// <summary>Beams that have been collected by the player.</summary>
	public BitArray BeamsOwned { get; set; }

	/// <summary>Items that have been collected by the player.</summary>
	public BitArray ItemsOwned { get; set; }

	/// <summary>Initializes a new instance of the <see cref="Inventory"/> class.</summary>
	public Inventory() {
		ActiveBeam = BeamTypes.None;
		ActiveItems = new BitArray();
		BeamsOwned = new BitArray();
		ItemsOwned = new BitArray();
	}

	/// <summary>
	/// Sets the active state of a specific item.
	/// </summary>
	/// <param name="item">The name of the item.</param>
	/// <param name="active">The active state to set.</param>
	public void SetActiveItem(ItemTypes item, bool active) {
		if (HasItem(item)) {
			ActiveItems.SetBitVal((uint)item, active);
		}
	}

	/// <summary>
	/// Sets the active beam.
	/// </summary>
	/// <param name="beam">The beam type to set as active.</param>
	public void SetActiveBeam(BeamTypes beam) {
		if (HasBeam(beam)) {
			ActiveBeam = beam;
		}
	}

	/// <summary>
	/// Checks if the player has a specific beam type.
	/// </summary>
	/// <param name="beam">The beam type to check.</param>
	/// <returns>True if the player has the specified beam type, false otherwise.</returns>
	public bool HasBeam(BeamTypes beam) {
		return BeamsOwned.CheckBitPow((int)beam);
	}

	/// <summary>
	/// Checks if the player has a specific item.
	/// </summary>
	/// <param name="item">The name of the item to check.</param>
	/// <returns>True if the player has the item; otherwise, false.</returns>
	public bool HasItem(ItemTypes item) {
		return ItemsOwned.CheckBitPow((int)item);
	}

	/// <summary>
	/// Modifies the ownership of a specific beam type.
	/// </summary>
	/// <param name="beam">The beam type to modify.</param>
	/// <param name="add">A boolean value indicating whether to add or remove the beam type.</param>
	public void ModifyBeams(BeamTypes beam, bool add) {
		if (add) {
			BeamsOwned.SetBit((uint)beam);
		} else {
			BeamsOwned.ClearBit((uint)beam);
		}
	}

	/// <summary>
	/// Modifies the items of the player by either adding or removing an item.
	/// </summary>
	/// <param name="item">The item to be modified.</param>
	/// <param name="add">A boolean value indicating whether to add or remove the item.</param>
	public void ModifyItems(ItemTypes item, bool add) {
		if (add) {
			ItemsOwned.SetBit((uint)item);
		} else {
			ItemsOwned.ClearBit((uint)item);
		}
	}

	/// <summary>
	/// Converts a string representation of an item to its corresponding ItemTypes enum value.
	/// </summary>
	/// <param name="item">The string representation of the item.</param>
	/// <returns>The corresponding ItemTypes enum value.</returns>
	public static ItemTypes StringToItemType(string item) {
		return (ItemTypes)Enum.Parse(typeof(ItemTypes), item);
	}

	/// <summary>
	/// Converts a string representation of a beam to its corresponding BeamTypes enum value.
	/// </summary>
	/// <param name="beam">The string representation of the beam.</param>
	/// <returns>The corresponding BeamTypes enum value.</returns>
	public static BeamTypes StringToBeamType(string beam) {
		return (BeamTypes)Enum.Parse(typeof(BeamTypes), beam);
	}

	public string PrintOwnedItems() {
		string output = "";
		output += "Test: " + ItemsOwned.CheckBitPow((int)ItemTypes.Test) + "\n";
		output += "AnglerCap: " + ItemsOwned.CheckBitPow((int)ItemTypes.AnglerCap) + "\n";
		output += "Flippers: " + ItemsOwned.CheckBitPow((int)ItemTypes.Flippers) + "\n";
		output += "Propeller: " + ItemsOwned.CheckBitPow((int)ItemTypes.Propeller) + "\n";
		output += "PressureSuit: " + ItemsOwned.CheckBitPow((int)ItemTypes.PressureSuit) + "\n";


		return output;
	}

	/// <summary>
	/// Gets the inventory data as a formatted string.
	/// </summary>
	/// <returns>A formatted string representing the inventory data.</returns>
	public string GetInventory() {
		return $"{ItemsOwned.PrintArray()}-{BeamsOwned.PrintArray()}-{ActiveBeam}-{ActiveItems.PrintArray()}";
	}


	/// <summary>
	/// Sets the inventory data based on the provided formatted string.
	/// </summary>
	/// <param name="saveData">A formatted string representing the inventory data.</param>
	public void SetInventory(string saveData) {
		string[] saveFields = saveData.Split("-");
		ItemsOwned = BitArray.FromString(saveFields[0]);
		BeamsOwned = BitArray.FromString(saveFields[1]);
		ActiveBeam = (BeamTypes)Enum.Parse(typeof(BeamTypes), saveFields[2]);
		ActiveItems = BitArray.FromString(saveFields[3]);
	}
}
