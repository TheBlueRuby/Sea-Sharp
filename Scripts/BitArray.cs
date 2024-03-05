using System.Diagnostics;

[DebuggerDisplay("{PrintArray(), nq}")]
public class BitArray {
	private int array;

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
	/// Sets the value of a specific bit in the BitArray.
	/// </summary>
	/// <param name="bit">The index of the bit to set.</param>
	/// <param name="on">The value to set the bit to. True for 1, false for 0.</param>
	public void SetBitVal(int bit, bool on) {
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
	public void SetBit(int bit) {
		array |= bit;
	}

	/// <summary>
	/// Clears the specified bit in the BitArray.
	/// </summary>
	/// <param name="bit">The index of the bit to clear.</param>
	public void ClearBit(int bit) {
		array &= ~array | bit;
	}

	public string PrintArray() {
		string output = "";
		for (int i = 0; i < 32; i++) {
			output += CheckBit(i) ? "1" : "0";
		}
		return output;
	}

}