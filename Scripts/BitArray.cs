using System.Diagnostics;
using System.Text;

[DebuggerDisplay("{PrintArray(), nq}")]
public class BitArray {
	private uint array;

	/// <summary>
	/// Initializes the class
	/// </summary>
	public BitArray() {
		array = 0;
	}

	/// <summary>
	/// Checks if a specific bit is set in the array.
	/// </summary>
	/// <param name="bit">The bit to check.</param>
	/// <returns>True if the bit is set, otherwise false.</returns>
	public bool CheckBit(int pos) {
		return (array & (1 << pos)) != 0;
	}

	/// <summary>
	/// Checks if a specific bit is set in the array.
	/// Assumes pos is a power of 2
	/// </summary>
	/// <param name="bit">The bit to check as a power of 2.</param>
	/// <returns>True if the bit is set, otherwise false.</returns>
	public bool CheckBitPow(int pos) {
		return (array & pos) != 0;
	}

	/// <summary>
	/// Sets the value of a specific bit in the BitArray.
	/// </summary>
	/// <param name="bit">The index of the bit to set.</param>
	/// <param name="on">The value to set the bit to. True for 1, false for 0.</param>
	public void SetBitVal(uint bit, bool on) {
		if (on) {
			SetBit(bit);
		} else {
			ClearBit(bit);
		}
	}

	/// <summary>
	/// Sets the specified bit in the BitArray.
	/// </summary>
	/// <param name="bit">The bit to set.</param>
	public void SetBit(uint bit) {
		array |= bit;
	}

	/// <summary>
	/// Clears the specified bit in the BitArray.
	/// </summary>
	/// <param name="bit">The index of the bit to clear.</param>
	public void ClearBit(uint bit) {
		array &= ~array | bit;
	}

	/// <summary>
	/// Returns a string representation of the BitArray
	/// </summary>
	/// <returns>The BitArray as a string</returns>
	public string PrintArray() {
		StringBuilder output = new();
		for (int i = 1; i <= 32; i++) {
			output.Append(CheckBit(i) ? "1" : "0");
		}
		return output.ToString();
	}

	/// <summary>
	/// Creates a BitArray from a string representation.
	/// </summary>
	/// <param name="stringRep">The string representation of the BitArray.</param>
	/// <returns>The BitArray created from the string representation.</returns>
	public static BitArray FromString(string stringRep) {
		BitArray output = new BitArray();
		for (uint i = 0; i < 32; i++) {
			if (stringRep[(int)i] == '1') {
				output.SetBit(2 ^ i);
			}
		}
		return output;
	}

}
